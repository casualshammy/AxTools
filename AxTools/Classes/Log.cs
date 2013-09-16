using System;
using System.IO;
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

        internal static void Print(string text, bool isError, bool flush = true)
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
        
    }
}
