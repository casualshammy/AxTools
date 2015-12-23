using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Reflection;
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

        [Obfuscation(Exclude = false, Feature = "constants")]
        internal static void SendViaEmail(string subject)
        {
            using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587))
            {
                smtpClient.EnableSsl = true;
                smtpClient.Credentials = new NetworkCredential("axtoolslogsender@gmail.com", "abrakadabra!pushpush");
                using (MailMessage mailMessage = new MailMessage("axtoolslogsender@gmail.com", "axio@axio.name"))
                {
                    mailMessage.SubjectEncoding = Encoding.UTF8;
                    mailMessage.Subject = string.IsNullOrWhiteSpace(subject) ? string.Format("Error log from {0}", Settings.Instance.UserID) : string.Format("Error log from {0} ({1})", Settings.Instance.UserID, subject);
                    mailMessage.BodyEncoding = Encoding.UTF8;
                    mailMessage.Body = "ERRORS: \r\n" + string.Join("\r\n", File.ReadAllLines(Globals.LogFileName, Encoding.UTF8).Where(l => l.Contains(ERROR_PREFIX_PATTERN)));
                    using (Attachment logFile = new Attachment(Globals.LogFileName, MediaTypeNames.Text.Plain))
                    {
                        mailMessage.Attachments.Add(logFile);
                        smtpClient.Send(mailMessage);
                    }
                }
            }
        }

        private static void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (_stringBuilder.Length != 0)
            {
                AppSpecUtils.CheckCreateDir();
                lock (_lock)
                {
                    File.AppendAllText(Globals.LogFileName, _stringBuilder.ToString(), Encoding.UTF8);
                    _stringBuilder.Clear();
                }

            }
        }

    }
}
