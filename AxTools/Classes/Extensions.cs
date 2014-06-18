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
                form.Invoke((MethodInvoker) delegate
                {
                    form.Activate();
                    new TaskDialog(title, "AxTools", text, button, icon).Show(form);
                });
            }
            else
            {
                form.Activate();
                new TaskDialog(title, "AxTools", text, button, icon).Show(form);
            }
        }

        /// <summary>
        /// Represent array of T as string like "{ item0, item1, item2, ... }"
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        internal static string AsString<T>(this IEnumerable<T> array)
        {
            return array.Aggregate("{", (current, s) => current + String.Format(" {0},", s)) + " }";
        }

        //internal static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        //{
        //    HashSet<TKey> knownKeys = new HashSet<TKey>();
        //    foreach (TSource element in source)
        //    {
        //        if (knownKeys.Add(keySelector(element)))
        //        {
        //            yield return element;
        //        }
        //    }
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns>IEnumerable of Types</returns>
        internal static IEnumerable<Type> ParentTypes(this Type type)
        {
            if (type.BaseType != null)
            {
                yield return type.BaseType;
                foreach (Type b in type.BaseType.ParentTypes())
                {
                    yield return b;
                }
            }
        }

    }
}