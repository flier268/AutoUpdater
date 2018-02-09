using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Xml;

namespace AutoUpdater
{
    /// <summary>
    /// 程序更新
    /// </summary>
    public class SoftUpdate
	{
		#region 構造函數
		public SoftUpdate() { }

		/// <summary>
		/// 程序更新
		/// </summary>
		/// <param name="file">要更新的文件</param>
		public SoftUpdate(string file,string softName) {
			this.LoadFile = file;
            this.SoftName = softName;
		} 
		#endregion

		#region 屬性
		private string loadFile;
		private string newVerson;
        private string softName;
		private bool isUpdate;

        public static string updateUrl = "";//升級配置的XML文件地址
        public static Version ver ;
        public static Version verson;  
		/// <summary>
		/// 或取是否需要更新
		/// </summary>
		public bool IsUpdate
		{
			get 
			{
				checkUpdate();
				return isUpdate; 
			}
		}
        /// <summary>  
        /// 要检查更新的文件  
        /// </summary>  
        public string LoadFile
        {
            get { return loadFile; }
            set { loadFile = value; }
        }
		/// <summary>
		/// 程序集新版本
		/// </summary>
		public string NewVerson
		{
			get { return newVerson; }
		}

        /// <summary>
        /// 升級的名稱
        /// </summary>
        public string SoftName
        {
            get { return softName; }
            set { softName = value; }
        }

		#endregion

		/// <summary>
		/// 檢查是否需要更新
		/// </summary>
		public void checkUpdate() 
		{
			try {
				WebClient wc = new WebClient();
				Stream stream = wc.OpenRead(updateUrl);
				XmlDocument xmlDoc = new XmlDocument();
				xmlDoc.Load(stream);
				XmlNode list = xmlDoc.SelectSingleNode("Update");
				foreach(XmlNode node in list) {
					if(node.Name == "Soft" && node.Attributes["Name"].Value.ToLower() == SoftName.ToLower()) {
						foreach(XmlNode xml in node) {
							if(xml.Name == "Verson")
								newVerson = xml.InnerText;							
						}
					}
				}

				ver = new Version(newVerson);
				verson = Assembly.LoadFrom(loadFile).GetName().Version;
				int tm = verson.CompareTo(ver);

				if(tm >= 0)
					isUpdate = false;
				else
					isUpdate = true;
                wc.Dispose();
                stream.Dispose();
                xmlDoc = null;
                list = null;                
			}
			catch
            {
                throw new Exception("更新出現錯誤，請確認網絡連接無誤後重試！");
			}
		}

	}
}
