using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization.Json;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Forms;
using AxTools.Classes.WoW;

namespace AxTools.Classes
{
    internal static class Utils
    {
        internal static readonly Random Rnd = new Random();
        internal static readonly Icon EmptyIcon = Icon.FromHandle(new Bitmap(1, 1).GetHicon());

        internal static T FindForm<T>() where T : Form
        {
            foreach (var i in Application.OpenForms)
            {
                if (i.GetType() == typeof (T)) return i as T;
            }
            return null;
        }

        internal static long CalcDirectorySize(string path)
        {
            DirectoryInfo info = new DirectoryInfo(path);
            long num2 = 0L;
            foreach (FileSystemInfo info2 in info.GetFileSystemInfos())
            {
                if (info2 is FileInfo)
                {
                    num2 += (info2 as FileInfo).Length;
                }
                else if (info2 is DirectoryInfo)
                {
                    num2 += CalcDirectorySize((info2 as DirectoryInfo).FullName);
                }
            }
            return num2;
        }

        internal static void CheckCreateDir()
        {
            if (!Directory.Exists(Globals.TempPath))
            {
                Directory.CreateDirectory(Globals.TempPath);
            }
            if (!Directory.Exists(Globals.UserfilesPath))
            {
                Directory.CreateDirectory(Globals.UserfilesPath);
            }
            if (!Directory.Exists(Globals.CfgPath))
            {
                Directory.CreateDirectory(Globals.CfgPath);
            }
        }

        // wipe after 28.09
        internal static void Legacy()
        {
            CheckCreateDir();
            if (File.Exists(Application.StartupPath + "\\common\\.newstyle"))
            {
                File.Delete(Application.StartupPath + "\\common\\.newstyle");
                Log.Print("Legacy :: .newstyle file was deleted", false);
            }
            if (Directory.Exists(Application.StartupPath + "\\temp"))
            {
                Directory.Delete(Application.StartupPath + "\\temp", true);
                Log.Print("Legacy :: temp directory was deleted", false);
            }
            if (File.Exists(Application.StartupPath + "\\.settings"))
            {
                File.Move(Application.StartupPath + "\\.settings", Globals.SettingsFilePath);
                Log.Print("Legacy :: .settings file was moved", false);
            }
            if (File.Exists(Application.StartupPath + "\\common\\.radar"))
            {
                File.Move(Application.StartupPath + "\\common\\.radar", Globals.CfgPath + "\\.radar");
                Log.Print("Legacy :: .radar file was moved", false);
            }
            if (File.Exists(Application.StartupPath + "\\common\\.luaconsole"))
            {
                File.Move(Application.StartupPath + "\\common\\.luaconsole", Globals.CfgPath + "\\.luaconsole");
                Log.Print("Legacy :: .luaconsole file was moved", false);
            }
            if (Directory.Exists(Application.StartupPath + "\\common"))
            {
                DirectoryInfo directory = new DirectoryInfo(Application.StartupPath + "\\common");
                foreach (FileInfo i in directory.EnumerateFileSystemInfos().Where(i => i is FileInfo).Cast<FileInfo>())
                {
                    i.MoveTo(Globals.UserfilesPath + "\\" + i.Name);
                    Log.Print("Legacy :: " + i.Name + " file was moved", false);
                }
                Directory.Delete(Application.StartupPath + "\\common", true);
                Log.Print("Legacy :: common directory was deleted", false);
            }
            if (File.Exists(Application.StartupPath + "\\cfg\\.accounts"))
            {
                byte[] file = File.ReadAllBytes(Application.StartupPath + "\\cfg\\.accounts");
                using (TripleDESCryptoServiceProvider provider = new TripleDESCryptoServiceProvider())
                {
                    provider.Key = TruncateHash("siemens2006", provider.KeySize/8);
                    provider.IV = TruncateHash("", provider.BlockSize / 8);
                    using (MemoryStream pMemoryStream = new MemoryStream())
                    {
                        using (ICryptoTransform decryptor = provider.CreateDecryptor())
                        {
                            using (CryptoStream pCryptoStream = new CryptoStream(pMemoryStream, decryptor, CryptoStreamMode.Write))
                            {
                                pCryptoStream.Write(file, 0, file.Length);
                                pCryptoStream.FlushFinalBlock();
                                string[] strings = Encoding.Unicode.GetString(pMemoryStream.ToArray()).Split(new[] { ';' });
                                List<WowAccount> list =
                                    (from i in strings where i != string.Empty && i != ":" select i.Split(new[] {':'}) into accountPair select new WowAccount(accountPair[0], accountPair[1])).ToList();
                                byte[] strangeBytes =
                                {
                                    0x2A, 0x26, 0x44, 0x56, 0x47, 0x2A, 0x37, 0x64, 0x76, 0x47, 0x26, 0x44, 0x2A, 0x48, 0x56, 0x37, 0x68, 0x26, 0x56, 0x68, 0x65, 0x68, 0x76, 0x26, 0x2A, 0x56,
                                    0x48
                                };
                                using (MemoryStream memoryStream = new MemoryStream())
                                {
                                    new BinaryFormatter().Serialize(memoryStream, list);
                                    File.WriteAllBytes(Globals.WowAccountsFilePath, Crypt.Encrypt<RijndaelManaged>(memoryStream.ToArray(), strangeBytes));
                                    File.Delete(Application.StartupPath + "\\cfg\\.accounts");
                                    Log.Print("Legacy :: .accounts has been successfully converted to .wowaccounts", false);
                                }
                            }
                        }
                        
                    }
                }
            }
            if (File.Exists(Application.StartupPath + "\\cfg\\.radar"))
            {
                byte[] file = File.ReadAllBytes(Application.StartupPath + "\\cfg\\.radar");
                using (TripleDESCryptoServiceProvider provider = new TripleDESCryptoServiceProvider())
                {
                    provider.Key = TruncateHash("radar!pushpush", provider.KeySize / 8);
                    provider.IV = TruncateHash("", provider.BlockSize / 8);
                    using (MemoryStream pMemoryStream = new MemoryStream())
                    {
                        using (ICryptoTransform decryptor = provider.CreateDecryptor())
                        {
                            using (CryptoStream pCryptoStream = new CryptoStream(pMemoryStream, decryptor, CryptoStreamMode.Write))
                            {
                                pCryptoStream.Write(file, 0, file.Length);
                                pCryptoStream.FlushFinalBlock();
                                string pText = Encoding.Unicode.GetString(pMemoryStream.ToArray());
                                if (pText.Length > 0)
                                {
                                    List<ObjectToFind> pList = new List<ObjectToFind>();
                                    using (StringReader cStringReader = new StringReader(pText))
                                    {
                                        while (cStringReader.Peek() != -1)
                                        {
                                            string line = cStringReader.ReadLine();
                                            if (!String.IsNullOrWhiteSpace(line))
                                            {
                                                string[] temp = line.Split('#');
                                                pList.Add(new ObjectToFind(Convert.ToBoolean(temp[0]), temp[1], Convert.ToBoolean(temp[2]), Convert.ToBoolean(temp[3])));
                                            }
                                        }
                                    }
                                    using (MemoryStream memoryStream = new MemoryStream())
                                    {
                                        new BinaryFormatter().Serialize(memoryStream, pList);
                                        File.WriteAllBytes(Application.StartupPath + "\\cfg\\.radar2", memoryStream.ToArray());
                                        File.Delete(Application.StartupPath + "\\cfg\\.radar");
                                        Log.Print("Legacy :: .radar has been successfully converted to .radar2", false);
                                    }
                                }
                            }
                        }

                    }
                }
            }
            if (File.Exists(Application.StartupPath + "\\cfg\\.luaconsole"))
            {
                byte[] file = File.ReadAllBytes(Application.StartupPath + "\\cfg\\.luaconsole");
                using (TripleDESCryptoServiceProvider provider = new TripleDESCryptoServiceProvider())
                {
                    provider.Key = TruncateHash("luaconsole!push!push", provider.KeySize / 8);
                    provider.IV = TruncateHash("", provider.BlockSize / 8);
                    using (MemoryStream pMemoryStream = new MemoryStream())
                    {
                        using (ICryptoTransform decryptor = provider.CreateDecryptor())
                        {
                            using (CryptoStream pCryptoStream = new CryptoStream(pMemoryStream, decryptor, CryptoStreamMode.Write))
                            {
                                pCryptoStream.Write(file, 0, file.Length);
                                pCryptoStream.FlushFinalBlock();
                                string pText = Encoding.Unicode.GetString(pMemoryStream.ToArray());
                                File.WriteAllText(Application.StartupPath + "\\cfg\\.luaconsole2", pText, Encoding.UTF8);
                                File.Delete(Application.StartupPath + "\\cfg\\.luaconsole");
                                Log.Print("Legacy :: .luaconsole has been successfully converted to .luaconsole2", false);
                            }
                        }

                    }
                }
            }
            if (File.Exists(Application.StartupPath + "\\cfg\\.luaconsole2"))
            {
                string pText = File.ReadAllText(Application.StartupPath + "\\cfg\\.luaconsole2", Encoding.UTF8);
                if (pText.Length > 0)
                {
                    using (StringReader stringReader = new StringReader(pText))
                    {
                        Settings.LuaConsoleTimerInterval = Convert.ToInt32(stringReader.ReadLine());
                        Settings.LuaConsoleRandomizeTimer = Convert.ToBoolean(stringReader.ReadLine());
                        Settings.LuaConsoleIgnoreGameState = Convert.ToBoolean(stringReader.ReadLine());
                        File.WriteAllText(Globals.CfgPath + "\\.luaconsole3", stringReader.ReadToEnd(), Encoding.UTF8);
                        File.Delete(Application.StartupPath + "\\cfg\\.luaconsole2");
                        Log.Print("Legacy :: .luaconsole2 has been successfully converted to .luaconsole3", false);
                    }
                }
            }
            if (File.ReadAllText(Globals.SettingsFilePath, Encoding.UTF8).Contains("server_ip"))
            {
                File.AppendAllText(Globals.SettingsFilePath, "\r\nGameServer=" + Globals.GameServers[0].Description);
                Log.Print("Legacy :: \"server_ip\" has been successfully converted to \"GameServer\"", false);
            }

            if (File.Exists(Globals.CfgPath + "\\.radar2"))
            {
                byte[] bytes = File.ReadAllBytes(Globals.CfgPath + "\\.radar2");
                if (bytes.Length > 0)
                {
                    List<ObjectToFind> list;
                    using (MemoryStream memoryStream = new MemoryStream(bytes))
                    {
                        list = (List<ObjectToFind>) new BinaryFormatter().Deserialize(memoryStream);
                    }
                    DataContractJsonSerializer serializer = new DataContractJsonSerializer(list.GetType());
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        serializer.WriteObject(memoryStream, list);
                        File.WriteAllBytes(Globals.CfgPath + "\\.radar3", memoryStream.ToArray());
                    }
                    File.Delete(Globals.CfgPath + "\\.radar2");
                    Log.Print("Legacy :: .radar2 has been successfully converted to .radar3", false);
                }
            }

            if (File.Exists(Application.StartupPath + "\\cfg\\.wowaccounts"))
            {
                byte[] strangeBytes = { 0x2A, 0x26, 0x44, 0x56, 0x47, 0x2A, 0x37, 0x64, 0x76, 0x47, 0x26, 0x44, 0x2A, 0x48, 0x56, 0x37, 0x68, 0x26, 0x56, 0x68, 0x65, 0x68, 0x76, 0x26, 0x2A, 0x56, 0x48 };
                byte[] oldBytes = Crypt.Decrypt<RijndaelManaged>(File.ReadAllBytes(Application.StartupPath + "\\cfg\\.wowaccounts"), strangeBytes);
                List<WowAccount> wowAccounts;
                using (MemoryStream memoryStream = new MemoryStream(oldBytes))
                {
                    wowAccounts = (List<WowAccount>) new BinaryFormatter().Deserialize(memoryStream);
                }
                DataContractJsonSerializer serializer = new DataContractJsonSerializer(wowAccounts.GetType());
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    serializer.WriteObject(memoryStream, wowAccounts);
                    byte[] newBytes = Crypt.Encrypt<RijndaelManaged>(memoryStream.ToArray(), strangeBytes);
                    File.WriteAllBytes(Globals.WowAccountsFilePath, newBytes);
                }
                File.Delete(Application.StartupPath + "\\cfg\\.wowaccounts");
                Log.Print("Legacy :: .wowaccounts has been successfully converted to .wowaccounts2", false);
            }
        }
        private static byte[] TruncateHash(string key, int length)
        {
            using (SHA1CryptoServiceProvider provider = new SHA1CryptoServiceProvider())
            {
                byte[] keyBytes = Encoding.Unicode.GetBytes(key);
                byte[] hash = provider.ComputeHash(keyBytes);
                Array.Resize(ref hash, length);
                return hash;
            }
        }
        
        internal static string GetRandomString(int size)
        {
            StringBuilder builder = new StringBuilder(size);
            for (int i = 0; i < size; i++)
            {
                char ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * Rnd.NextDouble() + 65)));
                if (Rnd.Next(10) % 2 == 0)
                {
                    ch = ch.ToString().ToLower()[0];
                }
                builder.Append(ch);
            }
            return builder.ToString();
        }
        
        //internal static void GetFileInfoFromExtension(string extension, out Icon pIcon, out string type)
        //{
        //    WinAPI.SHFILEINFO psfi = new WinAPI.SHFILEINFO();
        //    WinAPI.SHGetFileInfo(extension, 0x80, ref psfi, Marshal.SizeOf(psfi), 0x111 | 0x410);
        //    pIcon = Icon.FromHandle(psfi.HIcon);
        //    type = psfi.TypeName;
        //    if (!IconHandlers.Contains(psfi.HIcon))
        //    {
        //        IconHandlers.Add(psfi.HIcon);
        //    }
        //}

        internal static bool InternetAvailable
        {
            get
            {
                try
                {
                    using (Ping ping = new Ping())
                    {
                        PingReply pingReply = ping.Send("google.com", 2000);
                        return pingReply != null && (pingReply.Status == IPStatus.Success);
                    }
                }
                catch
                {
                    return false;
                }
            }
        }
    }
}