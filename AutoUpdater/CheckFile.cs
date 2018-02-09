using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace AutoUpdater
{
    class CheckFile
    {
        public static string GetMD5(string file_path)
        {
            if (File.Exists(file_path))
            {
                string myHashName = "MD5";
                String myFile = file_path;

                using (HashAlgorithm ha = HashAlgorithm.Create(myHashName))
                {
                    using (Stream myStream = new FileStream(myFile, FileMode.Open))
                    {
                        byte[] myHash = ha.ComputeHash(myStream);
                        StringBuilder NewHashCode = new StringBuilder(myHash.Length);
                        foreach (byte AddByte in myHash)
                        {
                            NewHashCode.AppendFormat("{0:x2}", AddByte);
                        }
                        return  NewHashCode.ToString();
                    }
                }
            }
            return "Error";
        }
        public static string GetVersion(string file_path)
        {
            if (File.Exists(file_path))
            {
                try
                {
                    System.Diagnostics.FileVersionInfo fv = System.Diagnostics.FileVersionInfo.GetVersionInfo(file_path);
                    return fv.FileVersion == null ? "Error" : fv.FileVersion.Replace(",",".");                    
                }
                catch
                {
                    return "Error";
                }
               
            }
            else
                return "Error";
        }
    }
}
