using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

namespace AxTools.Helpers
{
    internal static class VoIP
    {

        internal static Dictionary<string, VoipInfo> AvailableVoipClients
        {
            get
            {
                Settings settings = Settings.Instance;
                Dictionary<string, VoipInfo> list = new Dictionary<string, VoipInfo>();
                if (string.IsNullOrWhiteSpace(Settings.Instance.VentriloDirectory))
                {
                    Settings.Instance.VentriloDirectory = GetVentriloPath();
                }
                if (string.IsNullOrWhiteSpace(Settings.Instance.MumbleDirectory))
                {
                    Settings.Instance.MumbleDirectory = GetMumblePath();
                }
                if (string.IsNullOrWhiteSpace(Settings.Instance.RaidcallDirectory))
                {
                    Settings.Instance.RaidcallDirectory = GetRaidcallPath();
                }
                if (string.IsNullOrWhiteSpace(Settings.Instance.TS3Directory))
                {
                    Settings.Instance.TS3Directory = GetTeamspeakPath();
                }
                if (File.Exists(Settings.Instance.TS3Directory + "\\ts3client_win64.exe"))
                {
                    list["Teamspeak 3"] = new VoipInfo(Settings.Instance.TS3Directory + "\\ts3client_win64.exe", "-nosingleinstance", Settings.Instance.TS3Directory);
                }
                else if (File.Exists(Settings.Instance.TS3Directory + "\\ts3client_win32.exe"))
                {
                    list["Teamspeak 3"] = new VoipInfo(Settings.Instance.TS3Directory + "\\ts3client_win32.exe", "-nosingleinstance", Settings.Instance.TS3Directory);
                }
                if (File.Exists(Settings.Instance.VentriloDirectory + "\\Ventrilo.exe"))
                {
                    list["Ventrilo"] = new VoipInfo(Settings.Instance.VentriloDirectory + "\\Ventrilo.exe", "-m", Settings.Instance.VentriloDirectory);
                }
                if (File.Exists(settings.RaidcallDirectory + "\\raidcall.exe"))
                {
                    list["Raidcall"] = new VoipInfo(settings.RaidcallDirectory + "\\raidcall.exe", "", settings.RaidcallDirectory);
                }
                if (File.Exists(settings.MumbleDirectory + "\\mumble.exe"))
                {
                    list["Mumble"] = new VoipInfo(settings.MumbleDirectory + "\\mumble.exe", "", settings.MumbleDirectory);
                }
                GetDiscord(list);
                GetTwitch(list);
                return list;
            }
        }

        private static string GetTeamspeakPath()
        {
            using (RegistryKey regVersion = Registry.ClassesRoot.CreateSubKey("ts3server\\\\shell\\\\open\\\\command"))
            {
                try
                {
                    if (regVersion != null && regVersion.GetValue("") != null)
                    {
                        Regex regex = new Regex("\"(.+)\" .*");
                        Match match = regex.Match(regVersion.GetValue("").ToString());
                        if (match.Success)
                        {
                            return match.Groups[1].Value;
                        }
                    }
                    return string.Empty;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        private static string GetRaidcallPath()
        {
            using (RegistryKey regVersion = Registry.ClassesRoot.CreateSubKey("raidcall\\\\shell\\\\open\\\\command"))
            {
                try
                {
                    if (regVersion != null && regVersion.GetValue("") != null)
                    {
                        Regex regex = new Regex("\"(.+)\" .*");
                        Match match = regex.Match(regVersion.GetValue("").ToString());
                        if (match.Success)
                        {
                            return match.Groups[1].Value;
                        }
                    }
                    return string.Empty;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        private static string GetVentriloPath()
        {
            using (RegistryKey regVersion = Registry.ClassesRoot.CreateSubKey("Ventrilo\\\\shell\\\\open\\\\command"))
            {
                try
                {
                    if (regVersion != null && regVersion.GetValue("") != null)
                    {
                        Regex regex = new Regex("(.+) .*");
                        Match match = regex.Match(regVersion.GetValue("").ToString());
                        if (match.Success)
                        {
                            return match.Groups[1].Value;
                        }
                    }
                    return string.Empty;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        private static string GetMumblePath()
        {
            using (RegistryKey regVersion = Registry.ClassesRoot.CreateSubKey("mumble\\\\shell\\\\open\\\\command"))
            {
                try
                {
                    if (regVersion != null && regVersion.GetValue("") != null)
                    {
                        Regex regex = new Regex("\"(.+)\" .*");
                        Match match = regex.Match(regVersion.GetValue("").ToString());
                        if (match.Success)
                        {
                            return match.Groups[1].Value;
                        }
                    }
                    return string.Empty;
                }
                catch
                {
                    return string.Empty;
                }
            }
        }

        //private static string GetDiscordPath()
        //{
        //    string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Discord");
        //    Log.Info($"[Settings] Looking for Discord client in {path}");
        //    return Directory.Exists(path) ? path : string.Empty;
        //}

        private static void GetDiscord(Dictionary<string, VoipInfo> dic)
        {
            string discordDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Discord");
            string discordExecutable = Path.Combine(discordDir, "Update.exe");
            if (Directory.Exists(discordDir) && File.Exists(discordExecutable))
            {
                dic["Discord"] = new VoipInfo(discordExecutable, "--processStart Discord.exe", discordDir);
            }
        }

        private static void GetTwitch(Dictionary<string, VoipInfo> dic)
        {
            string twitchDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Curse Client\\Bin");
            string twitchExecutable = Path.Combine(twitchDir, "Twitch.exe");
            if (Directory.Exists(twitchDir) && File.Exists(twitchExecutable))
            {
                dic["Twitch"] = new VoipInfo(twitchExecutable, "", twitchDir);
            }
        }

        //private static string GetTwitchPath()
        //{
        //    string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Curse Client\\Bin");
        //    Log.Info($"[Settings] Looking for Twitch client in {path}");
        //    return Directory.Exists(path) ? path : string.Empty;
        //}

    }

    internal class VoipInfo
    {
        
        internal string ExecutablePath;
        internal string ExecutableArguments;
        internal string DirectoryPath;

        internal VoipInfo(string executablePath, string executableArguments, string directoryPath)
        {
            ExecutablePath = executablePath;
            ExecutableArguments = executableArguments;
            DirectoryPath = directoryPath;
    }

    }

}
