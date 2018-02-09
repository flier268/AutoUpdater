using ToolsLibrary;
namespace AutoUpdater
{
    class UpdateInfo
    {
        private string _APP_Name, _APP_Version, _APP_ChangeLog, _APP_FileName,

            _ZIP_URL, _ZIP_Name, _ZIP_MD5, _ZIP_Password;

        public string ZIP_URL
        {
            get { return _ZIP_URL; }
            set { _ZIP_URL = value; }
        }

       

       
        private string[] _Update_BeforeStart_RunAPP, _Update_AfterStart_RunAPP, _Update_CloseAPP;
        private bool _Update_DoNotAsk = false;

        private File_UpdateInfo[] _File_UpdateMod;


        #region 封裝
        internal File_UpdateInfo[] File_UpdateMod
        {
            get { return _File_UpdateMod; }
            set { _File_UpdateMod = value; }
        }
        public string ZIP_Password
        {
            get { return _ZIP_Password; }
            set { _ZIP_Password = value; }
        }
        public string ZIP_MD5
        {
            get { return _ZIP_MD5; }
            set { _ZIP_MD5 = value; }
        }
        public string APP_ChangeLog
        {
            get { return _APP_ChangeLog; }
            set { _APP_ChangeLog = value; }
        }
        public string APP_Version
        {
            get { return _APP_Version; }
            set { _APP_Version = value; }
        }
        public string APP_Name
        {
            get { return _APP_Name; }
            set { _APP_Name = value; }
        }
        public string APP_FileName
        {
            get { return _APP_FileName; }
            set { _APP_FileName = value; }
        }
        public string[] Update_CloseAPP
        {
            get { return _Update_CloseAPP; }
            set { _Update_CloseAPP = value; }
        }
        public string[] Update_AfterStart_RunAPP
        {
            get { return _Update_AfterStart_RunAPP; }
            set { _Update_AfterStart_RunAPP = value; }
        }
        public string[] Update_BeforeStart_RunAPP
        {
            get { return _Update_BeforeStart_RunAPP; }
            set { _Update_BeforeStart_RunAPP = value; }
        }        
        public bool Update_DoNotAsk
        {
            get { return _Update_DoNotAsk; }
            set { _Update_DoNotAsk = value; }
        }
        public string ZIP_Name
        {
            get { return _ZIP_Name; }
            set { _ZIP_Name = value; }
        }
        #endregion
      
        IniFile _ini;
        public UpdateInfo(IniFile ini)
        {
            _ini = ini;
        }
        public void LodeIni()
        {
            _APP_Name = _ini.IniReadValue("APP_Info", "APP_Name", 200);
            _APP_FileName = _ini.IniReadValue("APP_Info", "APP_FileName", 200);
            _APP_Version = _ini.IniReadValue("APP_Info", "APP_Version", 200);
            _APP_ChangeLog = _ini.IniReadValue("APP_Info", "APP_ChangeLog", 1000).Replace(@"\\n", "\n　　");


            _Update_CloseAPP = _ini.IniReadValue("Update_Info", "Update_CloseAPP", 200).Split(';');
            _Update_BeforeStart_RunAPP = _ini.IniReadValue("Update_Info", "Update_BeforeStart_RunAPP", 200).Split(';');
            _Update_AfterStart_RunAPP = _ini.IniReadValue("Update_Info", "Update_AfterStart_RunAPP", 200).Split(';');

            _Update_DoNotAsk = _ini.IniReadValue("Update_Info", "Update_DoNotAsk", 200).ToLower() == "true" ? true : false;

            _ZIP_URL = _ini.IniReadValue("ZIP_Info", "ZIP_URL", 1000);
            _ZIP_Name = _ini.IniReadValue("ZIP_Info", "ZIP_Name", 200);
            _ZIP_MD5 = _ini.IniReadValue("ZIP_Info", "ZIP_MD5", 200);
            _ZIP_Password = _ini.IniReadValue("ZIP_Info", "ZIP_Password", 200);

            string listtemp = _ini.IniReadValue("File_UpdateMod", "List", 1000);
            string[] temp1 = listtemp.Split(';');
            string[] temp2;
            File_UpdateMod = new File_UpdateInfo[temp1.Length];

            for (int i = 0; i < temp1.Length; i++)
            {
                File_UpdateMod[i] = new File_UpdateInfo();
                if (temp1[i].IndexOf(',') >= 0)
                {
                    temp2 = temp1[i].Split(',');
                    File_UpdateMod[i].FileName = temp2[0];
                    File_UpdateMod[i].Mod = temp2[1];
                }
            }
        }
    }
    class File_UpdateInfo
    {
        string _FileName = "", _Mod = "";

        public string Mod
        {
            get { return _Mod; }
            set { _Mod = value; }
        }

        public string FileName
        {
            get { return _FileName; }
            set { _FileName = value; }
        }      
            public File_UpdateInfo(){

        }
        public File_UpdateInfo(string FileName,string Mod)
        {
            _FileName = FileName;
            _Mod = Mod;
        }
    }
}
