using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using AxTools.Forms;
using WindowsFormsAero.TaskDialog;

namespace AxTools.Classes
{
    internal static class Log
    {
        internal static bool HaveErrors = false;
        private static readonly object PLock = new object();
        private static readonly StringBuilder PStringBuilder = new StringBuilder();

        internal static void Print(string text, bool isError = false, bool flush = true)
        {
            try
            {
                lock (PLock)
                {
                    if (isError)
                    {
                        HaveErrors = true;
                    }
                    PStringBuilder.AppendLine(string.Concat(DateTime.UtcNow.ToString("dd.MM.yyyy HH:mm:ss.fff"), isError ? " !! " : " // ", text));
                    if (flush || PStringBuilder.Length >= 32000)
                    {
                        Utils.CheckCreateDir();
						File.AppendAllText(Globals.LogFileName, PStringBuilder.ToString(), Encoding.UTF8);
						PStringBuilder.Clear();
                    }
                }
            }
            catch (Exception ex)
            {
                MainForm main = Utils.FindForm<MainForm>();
                if (main != null)
                {
                    main.ShowTaskDialog("Log file writing error", ex.Message, TaskDialogButton.OK, TaskDialogIcon.Stop);
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
                    mailMessage.Subject = string.IsNullOrWhiteSpace(subject) ? String.Format("Error log from {0}", Settings.Regname) : String.Format("Error log from {0} ({1})", Settings.Regname, subject);
                    mailMessage.BodyEncoding = Encoding.UTF8;
                    mailMessage.Body = File.ReadAllText(Globals.LogFileName, Encoding.UTF8) + "\r\n\r\n" + File.ReadAllText(Globals.SettingsFilePath, Encoding.UTF8);
                    smtpClient.Send(mailMessage);
                }
            }
        }

    }
}
