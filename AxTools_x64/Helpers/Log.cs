using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Timers;

namespace AxTools.Helpers
{
    internal class Log2
    {
        internal bool HaveErrors => _haveErrors;
        private static readonly object _lock = new object();
        private static readonly StringBuilder _stringBuilder = new StringBuilder();
        private static readonly Timer _timer = new Timer(1000);
        private const string ERROR_PREFIX_PATTERN = "[ERROR]";
        private const string DATETIME_PREFIX_PATTERN = "dd.MM.yyyy HH:mm:ss.fff";
        private readonly string _className;
        private static bool _haveErrors = false;

        static Log2()
        {
            _timer.Elapsed += TimerOnElapsed;
            _timer.Start();
            Program.Exit += Application_ApplicationExit;
        }

        internal Log2(string className)
        {
            if (string.IsNullOrWhiteSpace(className))
            {
                throw new ArgumentNullException("className");
            }
            _className = className;
        }

        private static void Application_ApplicationExit()
        {
            Program.Exit -= Application_ApplicationExit;
            TimerOnElapsed(null, null);
        }

        internal void Info(string text)
        {
            lock (_lock)
            {
                _stringBuilder.AppendLine($"{DateTime.UtcNow.ToString(DATETIME_PREFIX_PATTERN)} [INFO] [{_className}] {text}");
            }
        }

        internal void Info(object text)
        {
            lock (_lock)
            {
                _stringBuilder.AppendLine($"{DateTime.UtcNow.ToString(DATETIME_PREFIX_PATTERN)} [INFO] [{_className}] {text}");
            }
        }

        internal void Error(string text)
        {
            _haveErrors = true;
            lock (_lock)
            {
                StackTrace stackTrace = new StackTrace(1);
                _stringBuilder.AppendLine($"{DateTime.UtcNow.ToString(DATETIME_PREFIX_PATTERN)} {ERROR_PREFIX_PATTERN} [{_className}] {text}\r\n{stackTrace.ToString()}");
            }
        }

        internal static void UploadLog(string subject)
        {
            TimerOnElapsed(null, null);
            //Привет-привет, bye-bye!@#$%^&*()_-+=|\[]{};'.,?/
            if (!string.IsNullOrWhiteSpace(subject))
            {
                char[] subjChars = subject.ToCharArray();
                char[] subjCharsCleared = Array.FindAll(subjChars, c => char.IsLetterOrDigit(c) || c == '.' || c == ',' || c == ' ' || c == '-' || c == '!' || c == '?');
                subject = new string(subjCharsCleared);
            }
            var values = new NameValueCollection();
            values.Add("user", Settings2.Instance.UserID);
            values.Add("comment", subject ?? "");
            values.Add("log-file", $"ERRORS:\r\n{string.Join("\r\n", File.ReadAllLines(Globals.LogFileName, Encoding.UTF8).Where(l => l.Contains(ERROR_PREFIX_PATTERN)))}\r\n\r\n\r\n" +
                    $"{File.ReadAllText(Globals.LogFileName, Encoding.UTF8)}");
            using (WebClient wc = new WebClient())
            {
                wc.UploadValues("https://axio.name/axtools/log-reporter/make_log.php", "POST", values);
            }
        }

        private static void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (_stringBuilder.Length != 0)
            {
                lock (_lock)
                {
                    File.AppendAllText(Globals.LogFileName, _stringBuilder.ToString(), Encoding.UTF8);
                    _stringBuilder.Clear();
                }

            }
        }

    }
}
