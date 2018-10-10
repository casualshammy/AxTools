using AxTools.Helpers;
using Components.Forms;
using System;
using System.Windows.Forms;

namespace AxTools.Forms
{
    public partial class AppSettingsWoWBinds : BorderedMetroForm
    {
        public AppSettingsWoWBinds()
        {
            InitializeComponent();
            StyleManager.Style = Settings2.Instance.StyleColor;
            textBoxTarget.Text = new KeysConverter().ConvertToInvariantString(Settings2.Instance.WoWTargetMouseover);
            textBoxInteract.Text = new KeysConverter().ConvertToInvariantString(Settings2.Instance.WoWInteractMouseover);
            textBoxTarget.KeyDown += textBoxTarget_KeyDown;
            textBoxInteract.KeyDown += textBoxInteract_KeyDown;
            buttonTarget.Click += buttonTarget_Click;
            buttonInteract.Click += buttonInteract_Click;
        }

        private void buttonInteract_Click(object sender, EventArgs e)
        {
            textBoxInteract_KeyDown(null, new KeyEventArgs(Keys.None));
        }

        private void buttonTarget_Click(object sender, EventArgs e)
        {
            textBoxTarget_KeyDown(null, new KeyEventArgs(Keys.None));
        }

        private void textBoxInteract_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.ControlKey && e.KeyCode != Keys.ShiftKey && e.KeyCode != Keys.Menu)
            {
                Keys keys = LetOnlyOneKeyModifier(e);
                textBoxInteract.Text = new KeysConverter().ConvertToInvariantString(keys);
                Settings2.Instance.WoWInteractMouseover = keys;
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private void textBoxTarget_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode != Keys.ControlKey && e.KeyCode != Keys.ShiftKey && e.KeyCode != Keys.Menu)
            {
                Keys keys = LetOnlyOneKeyModifier(e);
                textBoxTarget.Text = new KeysConverter().ConvertToInvariantString(keys);
                Settings2.Instance.WoWTargetMouseover = keys;
                e.Handled = true;
                e.SuppressKeyPress = true;
            }
        }

        private Keys LetOnlyOneKeyModifier(KeyEventArgs args)
        {
            switch (args.Modifiers)
            {
                case Keys.Shift | Keys.Control | Keys.Alt:
                    return args.KeyData & ~Keys.Control & ~Keys.Alt;

                case Keys.Shift | Keys.Control:
                    return args.KeyData & ~Keys.Control;

                case Keys.Shift | Keys.Alt:
                    return args.KeyData & ~Keys.Alt;

                case Keys.Control | Keys.Alt:
                    return args.KeyData & ~Keys.Alt;

                default:
                    return args.KeyData;
            }
        }
    }
}