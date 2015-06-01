using System.Windows.Forms;

namespace AxTools.Components
{
    internal sealed class ListViewDoubleBuffered : ListView
    {
        internal ListViewDoubleBuffered()
        {
            DoubleBuffered = true;
        }
    }
}
