using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace axtools_updater
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (Directory.Exists(Application.StartupPath + "\\update"))
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
            DirectoryInfo directory = new DirectoryInfo(Application.StartupPath + "\\update");
            foreach (FileInfo i in directory.GetFileSystemInfos().Where(i => i is FileInfo).Cast<FileInfo>())
            {
                File.Delete(Application.StartupPath + "\\" + i.Name);
                File.Move(i.FullName, Application.StartupPath + "\\" + i.Name);
            }
            directory.Delete(true);
            DeleteUnusedFiles();
            Process.Start(new ProcessStartInfo
            {
                FileName = Application.StartupPath + "\\AxTools.exe",
                WorkingDirectory = Application.StartupPath
            });
        }

        internal static void DeleteUnusedFiles()
        {
            string[] filesToDelete =
            {
                "GreyMagic.dll", "WindowsFormsAero.dll", "MouseKeyboardActivityMonitor.dll", "EQATEC.Profiler.RuntimeFullNet.dll", "app.eqconfig", "axtools_updater.exe", "ICSharpCode.SharpZipLib.dll",
                "wol.jnlp", "Microsoft.WindowsAPICodePack.dll", "Microsoft.WindowsAPICodePack.Shell.dll"
            };
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
