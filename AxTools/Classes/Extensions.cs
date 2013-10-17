using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using WindowsFormsAero.TaskDialog;

namespace AxTools.Classes
{
    internal static class Extensions
    {
        /// <summary>
        /// Shows TaskDialog via form.Invoke() if required, direct call otherwise
        /// </summary>
        /// <param name="form">Parent form of new TaskDialog</param>
        /// <param name="title">Message of new TaskDialog</param>
        /// <param name="text">Content text of new TaskDialog</param>
        /// <param name="button"></param>
        /// <param name="icon"></param>
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

        /// <summary>
        /// Represent array of T as string like "{item1, item2, item3, ...}"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        internal static string AsString<T>(this IEnumerable<T> array)
        {
            return array.Aggregate("{", (current, s) => current + String.Format(" {0},", s)) + " }";
        }

    }
}