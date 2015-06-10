﻿using AxTools.Forms;
using System;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Timers;
using WindowsFormsAero.TaskDialog;

namespace AxTools.Classes
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

        /// <summary>
        ///     Deprecated
        /// </summary>
        /// <param name="text"></param>
        /// <param name="isError"></param>
        /// <param name="flush"></param>
        internal static void Print(string text, bool isError = false, bool flush = true)
        {
            try
            {
                lock (_lock)
                {
                    if (isError)
                    {
                        HaveErrors = true;
                        _stringBuilder.AppendLine(string.Concat(DateTime.UtcNow.ToString(DATETIME_PREFIX_PATTERN), ERROR_PREFIX_PATTERN, text));
                    }
                    else
                    {
                        _stringBuilder.AppendLine(string.Concat(DateTime.UtcNow.ToString(DATETIME_PREFIX_PATTERN), INFO_PREFIX_PATTERN, text));
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

        /// <summary>
        ///     Deprecated
        /// </summary>
        /// <param name="text"></param>
        /// <param name="isError"></param>
        /// <param name="flush"></param>
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

        private static void TimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            if (_stringBuilder.Length != 0)
            {
                Utils.CheckCreateDir();
                lock (_lock)
                {
                    File.AppendAllText(Globals.LogFileName, _stringBuilder.ToString(), Encoding.UTF8);
                    _stringBuilder.Clear();
                }

            }
        }

    }
}