using System.Windows.Forms;

namespace AxTools.Components
{
    public sealed partial class DataGridViewExt : DataGridView
    {
        public DataGridViewExt()
        {
            InitializeComponent();
            DoubleBuffered = true;
        }
    }
}
