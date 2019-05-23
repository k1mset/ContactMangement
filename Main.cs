using System;
using System.Windows.Forms;

namespace InventoryApplication
{
    public partial class Main : Form
    {
        public Main()
        {
            InitializeComponent();
        }

        // Opens a business contacts form inside of the existing form, allowing for multiple forms to be used
        private void businessToolStripMenuItem_Click(object sender, EventArgs e)
        {
            BizContacts frm = new BizContacts();
            frm.MdiParent = this;
            frm.Show();
        }

        // Sets the layout of forms open to cascade
        private void cascadeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.Cascade);
        }

        // Sets the layout of forms open to tile vertically
        private void tileVerticalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileVertical);
        }

        // Sets the layout of forms open to tile horizontally
        private void tileHorizontalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LayoutMdi(MdiLayout.TileHorizontal);
        }
    }
}
