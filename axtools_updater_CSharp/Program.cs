using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Windows.Forms;
using NDesk.Options;

namespace axtools_updater
{
    internal static class Program
    {
        internal const string AxToolsWebsite = "https://axtools.axio.name";
        private const string FileWithListOfFilesToDelete = "__delete";
        private static string _updateDir;
        private static string _axToolsDir;

        #region OptionSet

        private static readonly OptionSet _cmdOptions = new OptionSet
        {
            {
                "update-dir=", v =>
                {
                    _updateDir = v;
                }
            },
            {
                "axtools-dir=", v =>
                {
                    _axToolsDir = v;
                }
            }
        };

        #endregion

        [STAThread]
        private static void Main(string[] args)
        {
            _cmdOptions.Parse(args);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (_updateDir != null && Directory.Exists(_updateDir) && _axToolsDir != null && Directory.Exists(_axToolsDir))
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
            try
            {
                foreach (FileSystemInfo info in new DirectoryInfo(_updateDir).GetFileSystemInfos())
                {
                    if (info is FileInfo)
                    {
                        File.Delete(Path.Combine(_axToolsDir, info.Name));
                        File.Move(info.FullName, Path.Combine(_axToolsDir, info.Name));
                    }
                    else if (info is DirectoryInfo)
                    {
                        // ReSharper disable once RedundantEmptyFinallyBlock
                        try
                        {
                            DirectoryCopy(info.FullName, Path.Combine(_axToolsDir, info.Name), true);
                        }
                        finally
                        {

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Critical update error. Click OK to continue.\r\nError message:\r\n" + ex.Message);
                Application.Run(new Main());
            }
            try
            {
                DeletePluginsAssemblies();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Non-critical update error. Can't delete old plugins dlls\r\n" + ex.Message);
            }
            try
            {
                DeleteUnusedFiles();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Non-critical update error:\r\n" + ex.Message);
            }
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = Path.Combine(_axToolsDir, "AxTools.exe"),
                    WorkingDirectory = _axToolsDir
                });
            }
            catch (Exception ex)
            {
                MessageBox.Show("Non-critical update error. Please start AxTools manually.\r\nError message:\r\n" + ex.Message);
            }
        }

        internal static void DeleteUnusedFiles()
        {
            if (File.Exists(Path.Combine(_axToolsDir, FileWithListOfFilesToDelete)))
            {
                string[] filesToDelete = File.ReadAllText(Path.Combine(_axToolsDir, FileWithListOfFilesToDelete), Encoding.UTF8).Split(',');
                foreach (string i in filesToDelete)
                {
                    // ReSharper disable once RedundantEmptyFinallyBlock
                    try
                    {
                        File.Delete(Path.Combine(_axToolsDir, i));
                    }
                    finally
                    {

                    }
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

        private static void DeletePluginsAssemblies()
        {
            DirectoryInfo pluginsDirectoryInfo = new DirectoryInfo(Path.Combine(_axToolsDir, "pluginsAssemblies"));
            if (pluginsDirectoryInfo.Exists)
            {
                pluginsDirectoryInfo.Delete(true);
            }
        }

    }
}
