using System;
using System.IO;
using System.Net;
using System.Text;
using System.Timers;

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

        internal static void UploadLog(string subject)
        {
            TimerOnElapsed(null, null);
            using (WebClient webClient = new WebClient())
            {
                webClient.Credentials = new NetworkCredential(Settings.Instance.UserID, Utils.GetComputerHID());
                webClient.Encoding = Encoding.UTF8;
                webClient.UploadString(string.Format("https://axio.name/axtools/log-reporter/make_log.php?comment={0}", subject), "POST", File.ReadAllText(Globals.LogFileName, Encoding.UTF8));
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
