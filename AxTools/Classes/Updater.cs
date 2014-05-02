using Ionic.Zip;
using System;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

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

        internal static void DownloadAndExtractAddon()
        {
            string zipPath = Globals.TempPath + "\\ax_tools.zip";
            Utils.CheckCreateDir();
            File.Delete(zipPath);
            try
            {
                using (WebClient pWebClient = new WebClient())
                {
                    pWebClient.DownloadFile(Globals.DropboxPath + "/ax_tools.zip", zipPath);
                }
                using (ZipFile zipFile = ZipFile.Read(zipPath, new ReadOptions { Encoding = Encoding.UTF8 }))
                {
                    zipFile.ExtractAll(Settings.WowExe + "\\Interface\\AddOns", ExtractExistingFileAction.OverwriteSilently);
                }
                Log.Print("AddOn component successfully updated");
            }
            catch (Exception ex)
            {
                Log.Print("Download addon error: " + ex.Message, true);
            }
        }

        internal static void DownloadAndExtractTestPlugin()
        {
            if (Directory.Exists(Globals.PluginsPath + "\\TestPlugin"))
            {
                Utils.CheckCreateDir();
                Directory.Delete(Globals.PluginsPath + "\\TestPlugin", true);
                using (WebClient webClient = new WebClient())
                {
                    webClient.DownloadFile(Globals.DropboxPath + "/TestPlugin.zip", Globals.TempPath + "\\TestPlugin.zip");
                }
                using (ZipFile zip = new ZipFile(Globals.TempPath + "\\TestPlugin.zip", Encoding.UTF8))
                {
                    zip.ExtractAll(Application.StartupPath, ExtractExistingFileAction.OverwriteSilently);
                }
            }
        }

    }
}
