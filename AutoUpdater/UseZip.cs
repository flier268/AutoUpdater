using Ionic.Zip;
using System.Text;

namespace AutoUpdater
{
    class UseZip
    {
        public static string UnZip(string zippath,string password, string directory)
        {
            try
            {
                var options = new ReadOptions { StatusMessageWriter = System.Console.Out,Encoding=Encoding.Default};
                using (ZipFile zip = ZipFile.Read(zippath, options))
                {       
                    zip.Password = password;
                    zip.ExtractAll(directory);
                }
            }
            catch (System.Exception ex1)
            {
                return ("exception: " + ex1);
            }
            return "Done";
        }
        public static string  UnZip(string zippath, string directory)
        {
            try
            {
                var options = new ReadOptions { StatusMessageWriter = System.Console.Out ,Encoding=Encoding.Default};
                using (ZipFile zip = ZipFile.Read(zippath, options))
                {
                    zip.ExtractAll(directory);
                }                
            }
            catch (System.Exception ex1)
            {
                return ("exception: " + ex1);
            }
            return "Done";
        }
        public static string ZipDir(string DirectoryToZip, string ZipFileToCreate)
        {
            if (!System.IO.Directory.Exists(DirectoryToZip))
            {
                return ("The directory does not exist!\n");
               
            }
            if (System.IO.File.Exists(DirectoryToZip))
            {
                return ("That zipfile already exists!\n");
                
            }
            if (!ZipFileToCreate.EndsWith(".zip"))
            {
                return ("The filename must end with .zip!\n");
            }
            try
            {
                using (ZipFile zip = new ZipFile())
                {
                    zip.StatusMessageTextWriter = System.Console.Out;
                    zip.AddDirectory(DirectoryToZip);
                    zip.Save(ZipFileToCreate);
                }
            }
            catch (System.Exception ex1)
            {
                return ("exception: " + ex1);
            }
            return ("Done");
        }
    }
}
