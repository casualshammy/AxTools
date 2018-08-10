using System.Windows.Forms;

namespace Components
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