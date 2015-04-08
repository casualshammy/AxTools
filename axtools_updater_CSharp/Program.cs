using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace axtools_updater
{
    static class Program
    {
        private static readonly DirectoryInfo Directory = new DirectoryInfo(Application.StartupPath + "\\update");

        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (Directory.Exists)
            {
                NonUIUpdate();
            }
            else
            {
                Application.Run(new Main());
            }
        }

        private static void NonUIUpdate()
        {
            foreach (FileInfo i in Directory.GetFileSystemInfos().Where(i => i is FileInfo).Cast<FileInfo>())
            {
                File.Delete(Application.StartupPath + "\\" + i.Name);
                File.Move(i.FullName, Application.StartupPath + "\\" + i.Name);
            }
            DeleteUnusedFiles();
            Directory.Delete(true);
            Process.Start(new ProcessStartInfo
            {
                FileName = Application.StartupPath + "\\AxTools.exe",
                WorkingDirectory = Application.StartupPath
            });
        }

        internal static void DeleteUnusedFiles()
        {
            string[] filesToDelete = File.ReadAllText(Application.StartupPath + "\\__delete", Encoding.UTF8).Split(',');
            foreach (string i in filesToDelete)
            {
                try
                {
                    File.Delete(Application.StartupPath + "\\" + i);
                }
                catch
                {
                    
                }
            }
        }
    }
}
