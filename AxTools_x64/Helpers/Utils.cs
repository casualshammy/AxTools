using AxTools.WinAPI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Management;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AxTools.Helpers
{
    public static class Utils
    {
        private static readonly Log2 log = new Log2("Utils");
        private static string _hardwareID;
        internal static readonly Random Rnd = new Random();

        internal static IEnumerable<T> FindForms<T>() where T : Form
        {
            return Application.OpenForms.OfType<T>();
        }

        internal static long CalcDirectorySize(string path)
        {
            DirectoryInfo info = new DirectoryInfo(path);
            var num2 = 0L;
            foreach (FileSystemInfo info2 in info.GetFileSystemInfos())
            {
                if (info2 is FileInfo fileInfo)
                {
                    num2 += fileInfo.Length;
                }
                else if (info2 is DirectoryInfo directoryInfo)
                {
                    num2 += CalcDirectorySize(directoryInfo.FullName);
                }
            }
            return num2;
        }

        internal static IEnumerable<string> FindFiles(string path, string fileName, int depth = int.MaxValue)
        {
            if (depth == 0)
            {
                yield break;
            }
            FileSystemInfo[] infos = null;
            try
            {
                infos = new DirectoryInfo(path).GetFileSystemInfos();
            }
            catch { /*we don't care if we can't access some folder*/ }
            if (infos != null)
            {
                foreach (FileSystemInfo info2 in infos)
                {
                    if (info2 is FileInfo file)
                    {
                        if (file.Name == fileName)
                        {
                            yield return file.FullName;
                        }
                    }
                    else if (info2 is DirectoryInfo directory)
                    {
                        foreach (var p in FindFiles(directory.FullName, fileName, depth - 1))
                            yield return p;
                    }
                }
            }
        }

        internal static string GetRandomString(int size, bool onlyLetters)
        {
            StringBuilder builder = new StringBuilder(size);
            string chars = onlyLetters ? "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ" : "0123456789abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ";
            for (int i = 0; i < size; i++)
            {
                var c = chars[Rnd.Next(0, chars.Length)];
                builder.Append(c);
            }
            return builder.ToString();
        }

        internal static bool InternetAvailable
        {
            get
            {
                try
                {
                    using (Ping ping = new Ping())
                    {
                        PingReply pingReply = ping.Send("8.8.8.8", 2000);
                        return pingReply != null && (pingReply.Status == IPStatus.Success);
                    }
                }
                catch
                {
                    return false;
                }
            }
        }

        public static bool FontIsInstalled(string fontName)
        {
            using (InstalledFontCollection fontsCollection = new InstalledFontCollection())
            {
                return fontsCollection.Families.Any(i => i.Name == fontName);
            }
        }

        internal static void PlaySystemNotificationAsync()
        {
            Task.Factory.StartNew(() => NativeMethods.SndPlaySoundW("SystemNotification", Win32Consts.SND_ALIAS | Win32Consts.SND_NODEFAULT));
        }

        internal static void PlaySystemExclamationAsync()
        {
            Task.Factory.StartNew(() => NativeMethods.SndPlaySoundW("SystemExclamation", Win32Consts.SND_ALIAS | Win32Consts.SND_NODEFAULT));
        }

        internal static Image Base64ToImage(string base64)
        {
            byte[] byteBuffer = Convert.FromBase64String(base64);
            using (MemoryStream memoryStream = new MemoryStream(byteBuffer))
            {
                memoryStream.Position = 0;
                return Image.FromStream(memoryStream);
            }
        }

        [Obfuscation(Exclude = false, Feature = "rename(mode=unicode)")]
        [Obfuscation(Exclude = false, Feature = "constants")]
        [Obfuscation(Exclude = false, Feature = "ctrl flow(type=jump)")]
        internal static string GetComputerHID()
        {
            if (_hardwareID == null)
            {
                try
                {
                    byte[] userName = Encoding.UTF8.GetBytes(Settings2.Instance.UserID);
                    byte[] windowsUserName = Encoding.UTF8.GetBytes(System.Security.Principal.WindowsIdentity.GetCurrent().Name);
                    byte[] machineName = Encoding.UTF8.GetBytes(Environment.MachineName);
                    using (SHA256CryptoServiceProvider sha256 = new SHA256CryptoServiceProvider())
                    {
                        byte[] hash = sha256.ComputeHash(userName.Concat(windowsUserName).Concat(machineName).ToArray());
                        _hardwareID = BitConverter.ToString(hash).Replace("-", "");
                    }
                }
                catch (Exception ex)
                {
                    log.Error($"Exception is thrown in GetComputerHID: {ex.Message}");
                }
            }
            return _hardwareID;
        }

        internal static string CreateMd5ForFolder(string path)
        {
            List<string> files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories).OrderBy(p => p).ToList();
            using (MD5 md5 = MD5.Create())
            {
                for (int i = 0; i < files.Count; i++)
                {
                    string file = files[i];
                    string relativePath = file.Substring(path.Length + 1);
                    byte[] pathBytes = Encoding.UTF8.GetBytes(relativePath.ToLower());
                    md5.TransformBlock(pathBytes, 0, pathBytes.Length, pathBytes, 0);
                    byte[] contentBytes = File.ReadAllBytes(file);
                    if (i == files.Count - 1)
                    {
                        md5.TransformFinalBlock(contentBytes, 0, contentBytes.Length);
                    }
                    else
                    {
                        md5.TransformBlock(contentBytes, 0, contentBytes.Length, contentBytes, 0);
                    }
                }
                return BitConverter.ToString(md5.Hash).Replace("-", "");
            }
        }

        internal static string WordWrap(string text, int chunkSize)
        {
            List<string> words = text.Split(' ').ToList();
            StringBuilder result = new StringBuilder();
            while (words.Any())
            {
                StringBuilder sb = new StringBuilder();
                while (words.Any() && sb.Length + 1 + words.First().Length <= 55)
                {
                    sb.Append(" " + words.First());
                    words.RemoveAt(0);
                }
                result.Append(sb.ToString() + "\r\n");
            }
            return result.ToString().TrimEnd('\n').TrimEnd('\r');
        }

        internal static void LogIfCalledFromUIThread()
        {
            if (Thread.CurrentThread.ManagedThreadId == Program.UIThread)
            {
                StackTrace stackTrace = new StackTrace();
                log.Error($"Trying to call from UI thread; call stack:\r\n{stackTrace}");
            }
        }

        internal static void DirectoryCopy(string sourceDirName, string destDirName, bool copySubDirs)
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

        internal static string SecureString(string input)
        {
            Random rnd = new Random(input.Length);
            int[] indexesToHide = new int[input.Length / 2];
            for (int i = 0; i < indexesToHide.Length; i++)
            {
                var newValue = rnd.Next(0, input.Length);
                while (indexesToHide.Contains(newValue))
                {
                    newValue = rnd.Next(0, input.Length);
                }
                indexesToHide[i] = newValue;
            }
            StringBuilder builder = new StringBuilder(input.Length);
            var counter = 0;
            foreach (char c in input)
            {
                builder.Append(indexesToHide.Contains(counter) ? '*' : c);
                counter++;
            }
            return builder.ToString();
        }
    }
}