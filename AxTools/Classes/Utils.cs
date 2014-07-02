using AxTools.Forms;
using AxTools.Helpers;
using AxTools.WinAPI;
using System;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Media;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsAero.TaskDialog;

namespace AxTools.Classes
{
    internal static class Utils
    {
        internal static readonly Random Rnd = new Random();

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
            if (!Directory.Exists(Globals.PluginsPath))
            {
                Directory.CreateDirectory(Globals.PluginsPath);
            }
            if (!Directory.Exists(Globals.PluginsAssembliesPath))
            {
                Directory.CreateDirectory(Globals.PluginsAssembliesPath);
            }
            if (!Directory.Exists(Globals.PluginsSettingsPath))
            {
                Directory.CreateDirectory(Globals.PluginsSettingsPath);
            }
        }

        internal static void Legacy()
        {
            
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

        internal static bool FontIsInstalled(string fontName)
        {
            using (var fontsCollection = new InstalledFontCollection())
            {
                return fontsCollection.Families.Any(i => i.Name == fontName);
            }
        }

        internal static void PlaySystemNotificationAsync()
        {
            Task.Factory.StartNew(() => NativeMethods.sndPlaySoundW("SystemNotification", 65536 | 2));  //SND_ALIAS = 65536; SND_NODEFAULT = 2;);
        }

        internal static void NotifyUser(string title, string message, NotifyUserType type, bool sound)
        {
            if (NativeMethods.GetForegroundWindow() == MainForm.Instance.Handle)
            {
                switch (type)
                {
                    case NotifyUserType.Error:
                        MainForm.Instance.ShowTaskDialog(title, message, TaskDialogButton.OK, TaskDialogIcon.Stop);
                        break;
                    case NotifyUserType.Warn:
                        MainForm.Instance.ShowTaskDialog(title, message, TaskDialogButton.OK, TaskDialogIcon.Warning);
                        break;
                    default:
                        MainForm.Instance.ShowTaskDialog(title, message, TaskDialogButton.OK, TaskDialogIcon.Information);
                        break;
                }
            }
            else
            {
                MainForm.Instance.ShowNotifyIconMessage(title, message, (ToolTipIcon) type);
                if (sound)
                {
                    if (type == NotifyUserType.Error || type == NotifyUserType.Warn)
                    {
                        SystemSounds.Hand.Play();
                    }
                    else
                    {
                        PlaySystemNotificationAsync();
                    }
                }
            }
        }

    }
}