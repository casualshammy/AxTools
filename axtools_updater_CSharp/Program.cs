using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace axtools_updater
{
    static class Program
    {
        private static readonly DirectoryInfo UpdateDirectory = new DirectoryInfo(Application.StartupPath + "\\update");
        internal static readonly string AxToolsWebsite = "http://axtools.axio.name";

        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (UpdateDirectory.Exists)
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
            foreach (FileSystemInfo info in UpdateDirectory.GetFileSystemInfos())
            {
                if (info is FileInfo)
                {
                    File.Delete(Application.StartupPath + "\\" + info.Name);
                    File.Move(info.FullName, Application.StartupPath + "\\" + info.Name);
                }
                else if (info is DirectoryInfo)
                {
                    // ReSharper disable once RedundantEmptyFinallyBlock
                    try
                    {
                        DirectoryCopy(info.FullName, Application.StartupPath + "\\" + info.Name, true);
                    }
                    finally
                    {
                        
                    }
                }
            }
            DeleteUnusedFiles();
            UpdateDirectory.Delete(true);
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
                // ReSharper disable once RedundantEmptyFinallyBlock
                try
                {
                    File.Delete(Application.StartupPath + "\\" + i);
                }
                finally
                {
                    
                }
            }
        }

        private static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
        {
            DirectoryInfo dir = new DirectoryInfo(sourceDirName);
            if (!dir.Exists)
            {
                throw new DirectoryNotFoundException("Source directory does not exist or could not be found: " + sourceDirName);
            }
            if (!Directory.Exists(destDirName))
            {
                Directory.CreateDirectory(destDirName);
            }
            FileInfo[] files = dir.GetFiles();
            foreach (FileInfo file in files)
            {
                string temppath = Path.Combine(destDirName, file.Name);
                file.CopyTo(temppath, true);
            }
            if (copySubDirs)
            {
                DirectoryInfo[] dirs = dir.GetDirectories();
                foreach (DirectoryInfo subdir in dirs)
                {
                    string temppath = Path.Combine(destDirName, subdir.Name);
                    DirectoryCopy(subdir.FullName, temppath, true);
                }
            }
        }
    }
}
