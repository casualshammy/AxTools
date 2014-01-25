using System;
using System.IO;
using System.Net;
using System.Reflection;

namespace AxTools.Classes
{
    internal class Updater
    {
        internal bool UpdateForMainExecutableIsAvailable;

        internal bool UpdateForAddonIsAvailable;

        internal string[] FilesToDownload;

        internal static Updater GetUpdateInfo()
        {
            Updater updater = new Updater();
            string updateString;
            try
            {
                using (WebClient webClient = new WebClient())
                {
                    updateString = webClient.DownloadString(Globals.UpdateFilePath);
                }
            }
            catch (WebException webException)
            {
                Log.Print("[Updater] Fetching error: " + webException.Message);
                return updater;
            }
            catch (Exception ex)
            {
                Log.Print("[Updater] Fetching error: " + ex.Message + " :: " + ex.GetType(), true);
                return updater;
            }
            if (!String.IsNullOrWhiteSpace(updateString))
            {
                using (StringReader stringReader = new StringReader(updateString))
                {
                    while (stringReader.Peek() != -1)
                    {
                        try
                        {
                            string nextString = stringReader.ReadLine();
                            if (nextString != null)
                            {
                                string[] pair = nextString.Split(new[] { ":::::" }, StringSplitOptions.None);
                                switch (pair[0])
                                {
                                    case "CurrentAxToolsVersion":
                                        Version localVersion = Assembly.GetExecutingAssembly().GetName().Version;
                                        Version serverVersion = new Version(pair[1]);
                                        if (localVersion.Major != serverVersion.Major || localVersion.Minor != serverVersion.Minor || localVersion.Build != serverVersion.Build)
                                        {
                                            updater.UpdateForMainExecutableIsAvailable = true;
                                        }
                                        break;
                                    case "CurrentAddonVersion":
                                        if (Directory.Exists(Settings.WowExe + "\\Interface\\AddOns"))
                                        {
                                            string localAddonVersionFile = Settings.WowExe + "\\Interface\\AddOns\\ax_tools\\ax_tools.toc";
                                            string localAddonVersion = string.Empty;
                                            if (File.Exists(localAddonVersionFile))
                                            {
                                                localAddonVersion = File.ReadAllLines(localAddonVersionFile)[1];
                                            }
                                            string serverAddonVersion = pair[1];
                                            if (!File.Exists(localAddonVersionFile) || serverAddonVersion != localAddonVersion)
                                            {
                                                updater.UpdateForAddonIsAvailable = true;
                                            }
                                        }
                                        break;
                                    case "FilesToDownload":
                                        updater.FilesToDownload = pair[1].Split(',');
                                        break;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.Print("[Updater] Parsing error: " + ex.Message, true);
                        }
                    }
                }
            }
            else
            {
                Log.Print("[Updater] Update file fetched, but it's empty!", true);
            }
            return updater;
        }
    
    }
}
