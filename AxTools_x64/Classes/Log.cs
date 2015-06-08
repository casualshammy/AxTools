using AxTools.Forms;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using WindowsFormsAero.TaskDialog;

namespace AxTools.Classes
{
    internal static class Log
    {
        internal static bool HaveErrors = false;
        private static readonly object _lock = new object();
        private static readonly StringBuilder _stringBuilder = new StringBuilder();
        private const string INFO_PREFIX_PATTERN = @"dd.MM.yyyy HH:mm:ss.fff [IN\FO] "; //  "d", "f", "F", "g", "h", "H", "K", "m", "M", "s", "t", "y", "z", ":", "/"
        private const string ERROR_PREFIX_PATTERN = @"dd.MM.yyyy HH:mm:ss.fff [ERROR] "; // "d", "f", "F", "g", "h", "H", "K", "m", "M", "s", "t", "y", "z", ":", "/"
        
        internal static void Info(string text, bool flush = true)
        {
            Print(text, false, flush);
        }

        internal static void Error(string text, bool flush = true)
        {
            Print(text, true, flush);
        }

        internal static void Print(string text, bool isError = false, bool flush = true)
        {
            try
            {
                lock (_lock)
                {
                    if (isError)
                    {
                        HaveErrors = true;
                        _stringBuilder.AppendLine(string.Concat(DateTime.UtcNow.ToString(ERROR_PREFIX_PATTERN), text));
                    }
                    else
                    {
                        _stringBuilder.AppendLine(string.Concat(DateTime.UtcNow.ToString(INFO_PREFIX_PATTERN), text));
                    }
                    if (flush || _stringBuilder.Length >= 32768)
                    {
                        Utils.CheckCreateDir();
						File.AppendAllText(Globals.LogFileName, _stringBuilder.ToString(), Encoding.UTF8);
						_stringBuilder.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                if (MainForm.Instance != null)
                {
                    MainForm.Instance.ShowTaskDialog("Log file writing error", ex.Message, TaskDialogButton.OK, TaskDialogIcon.Stop);
                }
                else
                {
                    new TaskDialog("Log file writing error", "AxTools", ex.Message, TaskDialogButton.OK, TaskDialogIcon.Stop).Show();
                }
            }
        }

        internal static void Print(object text, bool isError = false, bool flush = true)
        {
            Print(text.ToString(), isError, flush);
        }

        internal static void SendViaEmail(string subject)
        {
            using (SmtpClient smtpClient = new SmtpClient("smtp.gmail.com", 587))
            {
                smtpClient.EnableSsl = true;
                smtpClient.Credentials = new NetworkCredential("axtoolslogsender@gmail.com", "abrakadabra!pushpush");
                using (MailMessage mailMessage = new MailMessage("axtoolslogsender@gmail.com", "axio@axio.name"))
                {
                    mailMessage.SubjectEncoding = Encoding.UTF8;
                    mailMessage.Subject = string.IsNullOrWhiteSpace(subject) ? String.Format("Error log from {0}", Settings.Instance.UserID) : String.Format("Error log from {0} ({1})", Settings.Instance.UserID, subject);
                    mailMessage.BodyEncoding = Encoding.UTF8;
                    mailMessage.Body = File.ReadAllText(Globals.LogFileName, Encoding.UTF8);
                    smtpClient.Send(mailMessage);
                }
            }
        }

    }
}
