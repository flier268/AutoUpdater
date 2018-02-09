using System;
using System.IO;

using System.Net;
using System.Threading;

namespace AutoUpdater
{
    class DownloadFiles
    {
        private double _TOTALSIZE = 0;
        private string url, DownloadToWhere;
        private Status status;
        public DownloadFiles()
        {
            status = new Status();
        }
        private void Download()
        {
            status.Isdownloading = true;
         
            HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(url);
            HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();

            status.MAXSIZE = httpResponse.ContentLength;
            

            System.IO.Stream dataStream = httpResponse.GetResponseStream();
            byte[] buffer = new byte[8192];

            FileStream fs = new FileStream(DownloadToWhere, FileMode.Create, FileAccess.Write);
            int size = 0;

            
            do
            {
                if (status.StopNow)
                    break;
                size = dataStream.Read(buffer, 0, buffer.Length);
                _TOTALSIZE += size;
                status.TOTALSIZE = _TOTALSIZE;//目前下載大小
                status.Progress = _TOTALSIZE / status.MAXSIZE;//目前百分比   
                if (size > 0)
                    fs.Write(buffer, 0, size);
            } while (size > 0);
            status.Isdownloading = false;
            fs.Close();

            httpResponse.Close();
            
          //  Console.WriteLine("Done at " + DateTime.Now.ToString("HH:mm:ss.fff"));
        }
        public void DownloadFile(string url,string DownloadToWhere)
        {
            this.url = url;
            this.DownloadToWhere = DownloadToWhere;
            ThreadStart th = new ThreadStart(Download);
            Thread t = new Thread(th);
            t.Start();
        }
        public Status GetStatus()
        {            
            return status;
        }
        public void Set_StopNow(bool boo)
        {
            status.StopNow = boo;
        }
       
    }
    public class Status
    {
        bool _Isdownloading = false;
        bool _stopNow = false;

        public bool StopNow
        {
            get { return _stopNow; }
            set { _stopNow = value; }
        }

        public bool Isdownloading
        {
            get { return _Isdownloading; }
            set { _Isdownloading = value; }
        }
        Double _MAXSIZE = 0.0, _TOTALSIZE = 0.0, _Progress = 0.0;

        public Double Progress
        {
            get { return _Progress; }
            set { _Progress = value; }
        }

        public Double TOTALSIZE
        {
            get { return _TOTALSIZE; }
            set { _TOTALSIZE = value; }
        }

        public Double MAXSIZE
        {
            get { return _MAXSIZE; }
            set { _MAXSIZE = value; }
        }
        
        
    }
}
