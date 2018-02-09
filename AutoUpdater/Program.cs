using System;

using System.Windows.Forms;

namespace AutoUpdater
{
    static class Program
    {
        /// <summary>
        /// 應用程式的主要進入點。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (args.Length > 0)
            {
                if (args[0] != "-updateself" & args[0].ToLower() != "-checkupdatewithform" & args[0].ToLower() != "-checkupdatewithoutform")
                {
                    MessageBox.Show("進行自我更新請執行 AutoUpdater.exe -updateself\n\n按照AutoUpdater.ini內容更新請執行\nAutoUpdater.exe -CheckUpdateWithForm\nOr\nAutoUpdater.exe -CheckUpdateWithoutForm");
                    return;
                }
            }
            else
            {
                MessageBox.Show("進行自我更新請執行 AutoUpdater.exe -updateself\n\n按照AutoUpdater.ini內容更新請執行\nAutoUpdater.exe -CheckUpdateWithForm\nOr\nAutoUpdater.exe -CheckUpdateWithoutForm");
                return;
            }
            Application.Run(new Form1(args));
        }
    }
}
