using System.Runtime.InteropServices;
using System.Text;

namespace ToolsLibrary
{
    public class IniFile
    {
        public string path;             //INI文件名  

        //聲明寫INI文件的API函數 
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern long WritePrivateProfileString(string section, string key, string val, string filePath);

        //聲明讀INI文件的API函數 
        [DllImport("kernel32.dll", CharSet = CharSet.Unicode)]
        private static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal, int size, string filePath);

        //類的構造函數，傳遞INI文件的路徑和文件名
        public IniFile(string INIPath)
        {
            path = INIPath;
        }

        //寫INI文件
        public void IniWriteValue(string Section, string Key, string Value)
        {
            WritePrivateProfileString(Section, Key, Value, path);
        }

        //讀取INI文件 
        public string IniReadValue(string Section, string Key, int size)
        {
            StringBuilder temp = new StringBuilder(size + 1);
            int i = GetPrivateProfileString(Section, Key, "", temp, size + 1, path);
            return temp.ToString();
        }

    }
}