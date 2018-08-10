using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace AxTools.Helpers
{
    internal static class VoIP
    {
        private static Log2 log = new Log2("VoIP");

        /// <summary>
        /// Launches VoIP with specified name
        /// </summary>
        /// <param name="name"></param>
        /// <exception cref="ArgumentException"> if no VoIP client with specified <paramref name="name"/> found</exception>
        internal static void StartVoIPClient(string name)
        {
            VoipInfo info = GetVoIPInfo(name);
            if (info != null)
            {
                Process.Start(new ProcessStartInfo
                {
                    WorkingDirectory = info.DirectoryPath,
                    FileName = info.ExecutablePath,
                    Arguments = info.ExecutableArguments
                });
                log.Info($"{name} is started");
            }
            else
            {
                throw new ArgumentException("No VoIP client with specified name found", "name");
            }
        }

        private static VoipInfo GetVoIPInfo(string voipName)
        {
            switch (voipName)
            {
                case "Teamspeak 3":
                    if (string.IsNullOrWhiteSpace(Settings2.Instance.TS3Directory) || (!File.Exists(Settings2.Instance.TS3Directory + "\\ts3client_win64.exe") && !File.Exists(Settings2.Instance.TS3Directory + "\\ts3client_win32.exe")))
                    {
                        Settings2.Instance.TS3Directory = GetTeamspeakPath();
                    }
                    if (File.Exists(Settings2.Instance.TS3Directory + "\\ts3client_win64.exe"))
                    {
                        return new VoipInfo(Settings2.Instance.TS3Directory + "\\ts3client_win64.exe", "-nosingleinstance", Settings2.Instance.TS3Directory);
                    }
                    else if (File.Exists(Settings2.Instance.TS3Directory + "\\ts3client_win32.exe"))
                    {
                        return new VoipInfo(Settings2.Instance.TS3Directory + "\\ts3client_win32.exe", "-nosingleinstance", Settings2.Instance.TS3Directory);
                    }
                    break;

                case "Ventrilo":
                    if (string.IsNullOrWhiteSpace(Settings2.Instance.VentriloDirectory) || !File.Exists(Settings2.Instance.VentriloDirectory + "\\Ventrilo.exe"))
                    {
                        Settings2.Instance.VentriloDirectory = GetVentriloPath();
                    }
                    if (File.Exists(Settings2.Instance.VentriloDirectory + "\\Ventrilo.exe"))
                    {
                        return new VoipInfo(Settings2.Instance.VentriloDirectory + "\\Ventrilo.exe", "-m", Settings2.Instance.VentriloDirectory);
                    }
                    break;

                case "Raidcall":
                    if (string.IsNullOrWhiteSpace(Settings2.Instance.RaidcallDirectory) || !File.Exists(Settings2.Instance.RaidcallDirectory + "\\raidcall.exe"))
                    {
                        Settings2.Instance.RaidcallDirectory = GetRaidcallPath();
                    }
                    if (File.Exists(Settings2.Instance.RaidcallDirectory + "\\raidcall.exe"))
                    {
                        return new VoipInfo(Settings2.Instance.RaidcallDirectory + "\\raidcall.exe", "", Settings2.Instance.RaidcallDirectory);
                    }
                    break;

                case "Mumble":
                    if (string.IsNullOrWhiteSpace(Settings2.Instance.MumbleDirectory) || !File.Exists(Settings2.Instance.MumbleDirectory + "\\mumble.exe"))
                    {
                        Settings2.Instance.MumbleDirectory = GetMumblePath();
                    }
                    if (File.Exists(Settings2.Instance.MumbleDirectory + "\\mumble.exe"))
                    {
                        return new VoipInfo(Settings2.Instance.MumbleDirectory + "\\mumble.exe", "", Settings2.Instance.MumbleDirectory);
                    }
                    break;

                case "Discord":
                    string discordDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Discord");
                    string discordExecutable = Path.Combine(discordDir, "Update.exe");
                    if (Directory.Exists(discordDir) && File.Exists(discordExecutable))
                    {
                        return new VoipInfo(discordExecutable, "--processStart Discord.exe", discordDir);
                    }
                    break;

                case "Twitch":
                    string twitchDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "Curse Client\\Bin");
                    string twitchExecutable = Path.Combine(twitchDir, "Twitch.exe");
                    if (Directory.Exists(twitchDir) && File.Exists(twitchExecutable))
                    {
                        return new VoipInfo(twitchExecutable, "", twitchDir);
                    }
                    break;
            }
            return null;
        }

        private static string GetTeamspeakPath()
        {
            log.Info("Looking for TS3 client...");
            foreach (var drive in DriveInfo.GetDrives().Where(l => l.DriveType == DriveType.Fixed))
            {
                var path = Utils.FindFiles(drive.Name, "ts3client_win64.exe", 5).Select(l => Path.GetDirectoryName(l)).FirstOrDefault() ??
                    Utils.FindFiles(drive.Name, "ts3client_win32.exe", 5).Select(l => Path.GetDirectoryName(l)).FirstOrDefault();
                if (path != null)
                {
                    log.Info("TS3 client is found: " + path);
                    return path;
                }
            }
            log.Info("TS3 client is not found!");
            return null;
        }

        private static string GetRaidcallPath()
        {
            log.Info("Looking for Raidcall client...");
            foreach (var drive in DriveInfo.GetDrives().Where(l => l.DriveType == DriveType.Fixed))
            {
                var path = Utils.FindFiles(drive.Name, "raidcall.exe", 5).Select(l => Path.GetDirectoryName(l)).FirstOrDefault();
                if (path != null)
                {
                    log.Info("Raidcall client is found: " + path);
                    return path;
                }
            }
            log.Info("Raidcall client is not found!");
            return null;
        }

        private static string GetVentriloPath()
        {
            log.Info("Looking for Ventrilo client...");
            foreach (var drive in DriveInfo.GetDrives().Where(l => l.DriveType == DriveType.Fixed))
            {
                var path = Utils.FindFiles(drive.Name, "Ventrilo.exe", 5).Select(l => Path.GetDirectoryName(l)).FirstOrDefault();
                if (path != null)
                {
                    log.Info("Ventrilo client is found: " + path);
                    return path;
                }
            }
            log.Info("Ventrilo client is not found!");
            return null;
        }

        private static string GetMumblePath()
        {
            log.Info("Looking for Mumble client...");
            foreach (var drive in DriveInfo.GetDrives().Where(l => l.DriveType == DriveType.Fixed))
            {
                var path = Utils.FindFiles(drive.Name, "mumble.exe", 5).Select(l => Path.GetDirectoryName(l)).FirstOrDefault();
                if (path != null)
                {
                    log.Info("Mumble client is found: " + path);
                    return path;
                }
            }
            log.Info("Mumble client is not found!");
            return null;
        }
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