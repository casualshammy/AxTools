using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WindowsFormsAero.TaskDialog;

namespace AxTools.Classes
{
    internal static class Extensions
    {
        internal static void ShowTaskDialog(this Form form, string title, string text, TaskDialogButton button, TaskDialogIcon icon)
        {
            if (form.InvokeRequired)
            {
                form.Invoke(new Action(() =>
                    {
                        form.Activate();
                        new TaskDialog(title, "AxTools", text, button, icon).Show(form);
                    }));
            }
            else
            {
                form.Activate();
                new TaskDialog(title, "AxTools", text, button, icon).Show(form);
            }
        }

        internal static string CastToString<T>(this IEnumerable<T> array)
        {
            return array.Aggregate("{", (current, s) => current + String.Format(" {0},", s)) + " }";
        }

    }
}