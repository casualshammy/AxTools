using System;
using System.IO;
using System.Net;
using System.Text;
using System.Timers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace AxTools.Helpers
{
    internal static class Log
    {
        internal static bool HaveErrors { get; private set; }
        private static readonly object _lock = new object();
        private static readonly StringBuilder _stringBuilder = new StringBuilder();
        private static readonly Timer _timer = new Timer(1000);
        private const string INFO_PREFIX_PATTERN = " [INFO] ";
        private const string ERROR_PREFIX_PATTERN = " [ERROR] ";
        private const string DATETIME_PREFIX_PATTERN = "dd.MM.yyyy HH:mm:ss.fff";

        static Log()
        {
            _timer.Elapsed += TimerOnElapsed;
            _timer.Start();
            Program.Exit += Application_ApplicationExit;
        }

        private static void Application_ApplicationExit()
        {
            Program.Exit -= Application_ApplicationExit;
            TimerOnElapsed(null, null);
        }

        internal static void Info(string text)
        {
            lock (_lock)
            {
                _stringBuilder.AppendLine(string.Concat(DateTime.UtcNow.ToString(DATETIME_PREFIX_PATTERN), INFO_PREFIX_PATTERN, text));
            }
        }

        internal static void Error(string text)
        {
            HaveErrors = true;
            lock (_lock)
            {
                _stringBuilder.AppendLine(string.Concat(DateTime.UtcNow.ToString(DATETIME_PREFIX_PATTERN), ERROR_PREFIX_PATTERN, text));
            }
        }

        internal static void UploadLogAndSendLink(string subject)
        {
            TimerOnElapsed(null, null);
            string gistFileName = "AxTools.log";
            using (WebClient webClient = new WebClient())
            {
                webClient.Headers[HttpRequestHeader.UserAgent] = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2;)";
                string linkToLog;
                try
                {
                    string json =
                        string.Format(@"{{
                            ""description"": ""AxTools log from {0}"",
                            ""public"": false,
                            ""files"": {{
                                ""{1}"": {{
                                    ""content"": {2}
                                }}
                            }}
                        }}", Settings.Instance.UserID, gistFileName, JsonConvert.SerializeObject(File.ReadAllText(Globals.LogFileName)));
                    string jsonResponse = webClient.UploadString("https://api.github.com/gists", "POST", json);
                    dynamic d = JObject.Parse(jsonResponse);
                    linkToLog = d["files"][gistFileName]["raw_url"];
                }
                catch (Exception ex)
                {
                    linkToLog = "Error while uploading log file: " + ex.Message;
                }
                webClient.Credentials = new NetworkCredential(Settings.Instance.UserID, Utils.GetComputerHID());
                webClient.Encoding = Encoding.UTF8;
                subject = string.IsNullOrWhiteSpace(subject) ? string.Format("Error log from {0}", Settings.Instance.UserID) : string.Format("Error log from {0} ({1})", Settings.Instance.UserID, subject);
                string s = string.Format("https://axio.name/axtools/log-reporter/sendEmail.php?body={0}&subject={1}&from-name={2}", linkToLog, subject, "AxTools log system");
                webClient.DownloadString(s);
            }
        }

        private static void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (_stringBuilder.Length != 0)
            {
                AppSpecUtils.CheckCreateTempDir();
                lock (_lock)
                {
                    File.AppendAllText(Globals.LogFileName, _stringBuilder.ToString(), Encoding.UTF8);
                    _stringBuilder.Clear();
                }

            }
        }

    }
}
