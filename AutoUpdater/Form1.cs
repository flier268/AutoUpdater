using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows.Forms;
using ToolsLibrary;

namespace AutoUpdater
{
    public partial class Form1 : Form
    {        
        #region 跨執行序存取UI
        private delegate void myUICallBack_Close(Form ctl);
        private delegate void myUICallBack_Enable(bool boo, System.Windows.Forms.Timer ctl);

        private delegate void myUICallBack_ControlText(string myStr, Control ctl);
        private void ChangeControlText(string myStr, Control ctl)
        {
            if (this.InvokeRequired)
            {
                myUICallBack_ControlText myUpdate = new myUICallBack_ControlText(ChangeControlText);
                this.Invoke(myUpdate, myStr, ctl);
            }
            else
            {
                ctl.Text = myStr;
            }
        }  
        private void TimerIO(bool boo, System.Windows.Forms.Timer ctl)
        {
            if (this.InvokeRequired)
            {
                myUICallBack_Enable myUpdate = new myUICallBack_Enable(TimerIO);
                this.Invoke(myUpdate, boo, ctl);
            }
            else
            {
                ctl.Enabled = boo;
            }
        }
        private void FormClose(Form ctl)
        {
            if (this.InvokeRequired)
            {
                myUICallBack_Close myUpdate = new myUICallBack_Close(FormClose);
                this.Invoke(myUpdate, ctl);
            }
            else
            {
                ctl.Close();
            }
        }
        #endregion
        IniFile ini_AutoUpdater = new IniFile(Path.Combine(Application.StartupPath ,"AutoUpdater.ini"));
        UpdateInfo updateinfo;
        string arg = "";
        bool WithForm = true;
        public Form1(string[] args)
        {
            InitializeComponent();
          
            if (args.Length > 0)
                arg = args[0];
            if (arg.ToLower() == "-updateself")
            {
                WithForm = true;
                label3.Text = "正在自我檢查更新...";
                ThreadStart th = new ThreadStart(UpdateSelf);
                Thread t = new Thread(th);
                t.Start();
            }
            else if (arg.ToLower()=="-CheckUpdateWithForm".ToLower())
            {
                WithForm = true;
                ThreadStart th = new ThreadStart(UpdateOtherAPP);
                Thread t = new Thread(th);
                t.Start();
            }
            else if (arg.ToLower() == "-CheckUpdateWithoutForm".ToLower())
            {
                WithForm = false;
                ThreadStart th = new ThreadStart(UpdateOtherAPP);
                Thread t = new Thread(th);
                t.Start();
            }

        }


        void UpdateOtherAPP()
        {
            download = new DownloadFiles();
            ChangeControlText("正在讀取更新包資訊...", label3);
            {//下載到temp\APP.ini
                HttpWebRequest httpRequest = (HttpWebRequest)WebRequest.Create(ini_AutoUpdater.IniReadValue("AutoUpdater_URL", "APP_Info_ini", 200));
                HttpWebResponse httpResponse = (HttpWebResponse)httpRequest.GetResponse();
                System.IO.Stream dataStream = httpResponse.GetResponseStream();
                byte[] buffer = new byte[8192];

                FileStream fs = new FileStream(Path.Combine(Path.GetTempPath() , ini_AutoUpdater.IniReadValue("APP_Info", "APP_Name", 200) + ".ini"), FileMode.Create, FileAccess.Write);
                int size = 0;


                do
                {
                    if (download.GetStatus().StopNow)
                        break;
                    size = dataStream.Read(buffer, 0, buffer.Length);
                    if (size > 0)
                        fs.Write(buffer, 0, size);
                } while (size > 0);
                fs.Close();
                httpResponse.Close();
            }

            //載入APP.ini到updateinfo
           
            updateinfo = new UpdateInfo(new IniFile(Path.Combine(Path.GetTempPath() , ini_AutoUpdater.IniReadValue("APP_Info", "APP_Name", 200) + ".ini")));
            updateinfo.LodeIni();

            Version Version_localFile = new Version(), Version_UpdateFile = new Version();

            bool au = false;
            if (CheckFile.GetVersion(updateinfo.APP_FileName) != "Error")
                Version_localFile = new Version(CheckFile.GetVersion(updateinfo.APP_FileName));
            else
            {
                //ChangeControlText("找不到\"" + updateinfo.APP_FileName+"\"", label3);
                au = true;
              //  return;
            }
            try
            {
                Version_UpdateFile = new Version(updateinfo.APP_Version);
            }
            catch { ChangeControlText("APP_Version有問題", label3); }
            if (download.GetStatus().StopNow)
                return;
            int tm ;
            if (!au)
                tm = Version_UpdateFile.CompareTo(Version_localFile);
            else//永遠更新
                tm = 100;
            if (tm <= 0)
                //沒有更新
                if (WithForm)
                {
                    FormClose(this);
                    return;
                }
                else
                {
                    MessageBox.Show("無須更新");
                    return;
                }
            else
            {//發現更新
                ChangeControlText(String.Format("發現新版本（{0}）, 現行版本 {1}", updateinfo.APP_Version, Version_localFile.ToString()), label3);
                
                if (updateinfo.Update_DoNotAsk)
                {
                    UsePackUpdate();
                }
                else
                {                   
                    if (MessageBox.Show(String.Format("現行版本：{0}\r\n最新版本：{1}\r\nChangeLog：\r\n　　{2}", Version_localFile.ToString(), updateinfo.APP_Version, updateinfo.APP_ChangeLog), "是否立即更新？", MessageBoxButtons.YesNo, MessageBoxIcon.Asterisk) == DialogResult.No)
                    {//否
                        FormClose(this);
                    }
                    else
                    {//立即更新
                        UsePackUpdate();
                    }
                }
            }

            
        }
        private void UsePackUpdate()
        {           
             download.DownloadFile(updateinfo.ZIP_URL, updateinfo.ZIP_Name);
             TimerIO(true, timer2);
        }


        private void Form1_Shown(object sender, System.EventArgs e)
        {
            
        }

        DownloadFiles download;
        Status temp;
        
        private void timer1_Tick(object sender, EventArgs e)
        {
            temp = download.GetStatus();
            progressBar1.Value = (int)(temp.Progress * 100);
            label1.Text = temp.TOTALSIZE + "/" + temp.MAXSIZE;
            if (temp.Isdownloading == false)
            {
                timer1.Stop();                
                if (System.IO.Directory.Exists(Path.Combine(Application.StartupPath,"temp")))
                    System.IO.Directory.Delete(Path.Combine(Application.StartupPath, "temp"), true);
                UseZip.UnZip(Path.Combine(Application.StartupPath, "AutoUpdater.zip"), Path.Combine(Application.StartupPath, "temp"));
                killself();            
            }
        }
        private bool checkUpdate()
        {
            SoftUpdate.updateUrl = "https://sites.google.com/site/pow7000web/mysoftware/autoupdater/AutoUpdater_version.xml";
            SoftUpdate app = new SoftUpdate(Application.ExecutablePath, "AutoUpdater_version");

            if (app.IsUpdate)
            {
                ChangeControlText("發現更新，開始進行更新...",label3);
                return true;
            }

            return false;
        }        
        void UpdateSelf()
        {
            if(checkUpdate())
            {
                download = new DownloadFiles();
                download.DownloadFile("https://sites.google.com/site/pow7000web/mysoftware/autoupdater/AutoUpdater.zip", Path.Combine(Application.StartupPath,"AutoUpdater.zip"));
                TimerIO(true, timer1);
            }
        }
        void killself()
        {
            string s = Process.GetCurrentProcess().MainModule.FileName;

            Process proRestart = new Process();
            proRestart.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;     //不顯示窗體 
            proRestart.StartInfo.UseShellExecute = true;
            string strArgument = String.Format(" /c   \"copy /y \"{0}\" \"{1}\"\"",Path.Combine(Path.Combine(Application.StartupPath,"temp"), "AutoUpdater.exe"), Path.Combine(Application.StartupPath, "AutoUpdater.exe"));
            //啟動參數 
            proRestart.StartInfo.Arguments = strArgument;
            proRestart.StartInfo.CreateNoWindow = true;
            proRestart.StartInfo.FileName = "c:\\windows\\system32\\cmd.exe";
            proRestart.Start();     //執行 
            Process.GetCurrentProcess().Kill();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            if (arg.ToLower() == "-CheckUpdateWithoutForm".ToLower())
            {
                this.Visible = false;
                this.ShowInTaskbar = false;
            }
        }
        private void Form1_Closing(object sender, System.Windows.Forms.FormClosingEventArgs e)
        {
            download.Set_StopNow(true);
          
        }
        private void timer2_Tick(object sender, EventArgs e)
        {
            temp = download.GetStatus();
            progressBar1.Value = (int)(temp.Progress * 100);
            label1.Text = (temp.TOTALSIZE/1024).ToString("0.00") + "KB /" + (temp.MAXSIZE/1024).ToString("0.00")+"KB";
            if (temp.Isdownloading == false)
            {
                timer2.Stop();
                if (CheckFile.GetMD5(updateinfo.ZIP_Name).ToUpper() == updateinfo.ZIP_MD5.ToUpper())
                {
                    CloseAPP();
                    RunAPP(1);
                    if (System.IO.Directory.Exists(Path.Combine(Application.StartupPath, "temp")))
                        System.IO.Directory.Delete(Path.Combine(Application.StartupPath, "temp"), true);
                    if (updateinfo.ZIP_Password == "")
                        UseZip.UnZip(updateinfo.ZIP_Name, Path.Combine(Application.StartupPath, "temp"));
                    else
                        UseZip.UnZip(updateinfo.ZIP_Name, updateinfo.ZIP_Password, Path.Combine(Application.StartupPath, "temp"));
                    foreach (string fname in System.IO.Directory.GetFiles(Path.Combine(Application.StartupPath, "temp"), "*.*", SearchOption.AllDirectories))
                    {
                        foreach (File_UpdateInfo tt in updateinfo.File_UpdateMod)
                        {
                            switch (tt.Mod)
                            {
                                case "WhenNeed":
                                    if (tt.FileName == (fname.IndexOf("temp\\") == 0 ? fname.Substring(5, fname.Length - 5) : fname))
                                        if (CheckFile.GetMD5(tt.FileName) != CheckFile.GetMD5(fname))
                                            File.Copy(fname, Path.Combine(Directory.GetParent(Path.GetDirectoryName(fname)).FullName, Path.GetFileName(fname)), true);
                                    break;
                                case "Always":
                                    File.Copy(fname, Path.Combine(Directory.GetParent(Path.GetDirectoryName(fname)).FullName, Path.GetFileName(fname)), true);
                                    break;
                                case "WhenNotExist":
                                    File.Copy(fname, Path.Combine(Directory.GetParent(Path.GetDirectoryName(fname)).FullName, Path.GetFileName(fname)), false);
                                    break;
                                case "Ignore":
                                    break;
                                default:
                                    File.Copy(fname, Path.Combine(Directory.GetParent(Path.GetDirectoryName(fname)).FullName, Path.GetFileName(fname)), true);
                                    break;
                            }
                        }
                    }

                    if (System.IO.Directory.Exists(Path.Combine(Application.StartupPath, "temp")))
                        System.IO.Directory.Delete(Path.Combine(Application.StartupPath, "temp"), true);
                    if (File.Exists(updateinfo.ZIP_Name))
                        File.Delete(updateinfo.ZIP_Name);
                    RunAPP(2);
                    ChangeControlText("更新完成！", label3);

                    if (!WithForm)
                    {
                        MessageBox.Show("更新完成！");
                        FormClose(this);
                    }
                    Recheck.Enabled = true;
                }
                else
                {
                    label3.Text = ("檔案驗證失敗，請再試一次");
                    Recheck.Enabled = true;
                }
            }
        }
        private void CloseAPP()
        {
            Process[] ps = Process.GetProcesses();
            for (int i = 0; i < ps.Length; i++)
            {
                for (int j = 0; j < updateinfo.Update_CloseAPP.Length; j++)
                    if (ps[i].ProcessName.ToLower() == updateinfo.Update_CloseAPP[j].ToLower() || ps[i].ProcessName.ToLower() + ".exe" == updateinfo.Update_CloseAPP[j].ToLower())
                    {
                        try
                        {
                            ps[i].CloseMainWindow();
                            ps[i].Kill();
                        }
                        catch
                        {
                        }                       
                    }
            }
        }
        private void RunAPP(int BeforeOrAfter)
        {
            string[] temp1 = null,temp2=new string[2];
            switch(BeforeOrAfter)
            {
                case 1:
                    temp1 = updateinfo.Update_BeforeStart_RunAPP;
                    break;
                case 2:
                    temp1 = updateinfo.Update_AfterStart_RunAPP;
                    break;
            }
            ProcessStartInfo startInfo;
            foreach(string t in temp1)
            {               
                temp2 = t.Split(',');
                if (File.Exists(temp2[0]))
                {
                    startInfo = new ProcessStartInfo(temp2[0]);                    
                    startInfo.Arguments = temp2[1];
                    Process.Start(startInfo);
                }
            }
            
        }

        private void Recheck_Click(object sender, EventArgs e)
        {
            Recheck.Enabled = false;
            if (arg.ToLower() == "-updateself")
            {
                WithForm = true;
                label3.Text = "正在自我檢查更新...";
                ThreadStart th = new ThreadStart(UpdateSelf);
                Thread t = new Thread(th);
                t.Start();
            }
            else if (arg.ToLower() == "-CheckUpdateWithForm".ToLower())
            {
                WithForm = true;
                ThreadStart th = new ThreadStart(UpdateOtherAPP);
                Thread t = new Thread(th);
                t.Start();
            }
            else if (arg.ToLower() == "-CheckUpdateWithoutForm".ToLower())
            {
                WithForm = false;
                ThreadStart th = new ThreadStart(UpdateOtherAPP);
                Thread t = new Thread(th);
                t.Start();
            }
        }
    }
}
