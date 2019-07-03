using AxTools.Forms;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Windows.Forms;

namespace AxTools.Helpers
{
    public static class Extensions
    {
        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            HashSet<TKey> knownKeys = new HashSet<TKey>();
            foreach (TSource element in source)
            {
                if (knownKeys.Add(keySelector(element)))
                {
                    yield return element;
                }
            }
        }

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

        /// <summary>
        ///     Very fast comparison of byte arrays (memcmp)
        /// </summary>
        /// <param name="b1"></param>
        /// <param name="b2"></param>
        /// <returns>True if sequences are equal, false otherwise</returns>
        public static unsafe bool SequenceEqual(this byte[] b1, byte[] b2)
        {
            fixed (byte* v1 = b1)
            {
                fixed (byte* v2 = b2)
                {
                    return WinAPI.NativeMethods.memcmp(v1, v2, (UIntPtr)b1.Length) == 0;
                }
            }
        }

        /// <summary>
        ///     Forces WebClient to use basic http auth without getting http-401-error
        /// </summary>
        /// <param name="webClient"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public static void ForceBasicAuth(this WebClient webClient, string username, string password)
        {
            string credentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(username + ":" + password));
            webClient.Headers[HttpRequestHeader.Authorization] = $"Basic {credentials}";
        }

        public static void ActivateBrutal(this Form form)
        {
            form.Show();
            form.WindowState = FormWindowState.Normal;
            form.Activate();
        }

        public static void ExecuteInUIThread(this Action action)
        {
            MainWindow.Instance.BeginInvoke(new MethodInvoker(action));
        }

        public static IEnumerable<ToolStripItem> GetAllToolStripItems(this ToolStripItemCollection collection)
        {
            foreach (ToolStripItem toolStripItem in collection)
            {
                if (toolStripItem != null)
                {
                    yield return toolStripItem;
                    if (toolStripItem is ToolStripDropDownItem item && item.HasDropDownItems)
                    {
                        foreach (ToolStripItem v in GetAllToolStripItems(item.DropDownItems))
                        {
                            yield return v;
                        }
                    }
                }
            }
        }
    }
}