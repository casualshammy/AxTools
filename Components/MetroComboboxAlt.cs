using System.Drawing;
using System.Windows.Forms;
using MetroFramework;
using MetroFramework.Components;
using MetroFramework.Drawing;

namespace Components
{
    public partial class MetroComboboxAlt : ComboBox
    {
        private MetroColorStyle metroStyle = MetroColorStyle.Blue;
        public MetroStyleManager StyleManager { get; set; }

        public MetroColorStyle Style
        {
            get
            {
                return StyleManager != null ? StyleManager.Style : metroStyle;
            }
            set
            {
                metroStyle = value;
            }
        }

        public MetroComboboxAlt()
        {
            InitializeComponent();
            DropDownStyle = ComboBoxStyle.DropDownList;
        }

        protected override void WndProc(ref Message m)
        {
            // ReSharper disable once InconsistentNaming
            int WM_PAINT = 0x000F;
            base.WndProc(ref m);
            if (m.Msg == WM_PAINT)
            {
                using (SolidBrush styleBrush = MetroPaint.GetStyleBrush(Style))
                {
                    using (Graphics e = Graphics.FromHwnd(Handle))
                    {
                        e.FillRectangles(styleBrush, new[]
                        {
                            new Rectangle(0, 0, Width, 2), // up
                            new Rectangle(Width - 2, 0, 2, Height), // right
                            new Rectangle(0, 0, 2, Height), // left
                            new Rectangle(0, Height - 2, Width, 2) // bottom
                        });
                    }
                }
            }
        }
    }
}
