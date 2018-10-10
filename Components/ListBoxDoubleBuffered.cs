using System.Windows.Forms;

namespace Components
{
    public sealed class ListViewDoubleBuffered : ListView
    {
        public ListViewDoubleBuffered()
        {
            DoubleBuffered = true;
        }
    }
}