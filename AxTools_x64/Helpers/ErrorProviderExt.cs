using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using MetroFramework.Components;

namespace AxTools.Helpers
{
    internal class ErrorProviderExt
    {
        private static readonly Dictionary<Control, Color> ControlColors = new Dictionary<Control, Color>();
        private static readonly MetroToolTip ToolTip = new MetroToolTip();

        internal static void SetError(Control control, string text, Color color)
        {
            if (!ControlColors.ContainsKey(control))
            {
                ControlColors.Add(control, control.BackColor);
                control.LostFocus += ControlOnLostFocus;
                control.Disposed += ControlOnDisposed;
            }
            control.BackColor = color;
            ToolTip.Show(text, control, control.Width, control.Height);
        }

        internal static void ClearError(Control control)
        {
            if (ControlColors.ContainsKey(control))
            {
                control.BackColor = ControlColors[control];
                ToolTip.Hide(control);
                ControlColors.Remove(control);
                control.LostFocus -= ControlOnLostFocus;
                control.Disposed -= ControlOnDisposed;
            }
        }

        private static void ControlOnLostFocus(object sender, EventArgs eventArgs)
        {
            Control control = sender as Control;
            if (control != null)
            {
                ToolTip.Hide(control);
            }
        }

        private static void ControlOnDisposed(object sender, EventArgs eventArgs)
        {
            Control control = sender as Control;
            if (control != null)
            {
                ControlColors.Remove(control);
                control.LostFocus -= ControlOnLostFocus;
                control.Disposed -= ControlOnDisposed;
            }
        }

    }
}
