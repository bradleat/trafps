using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EGGEditor01
{

    public partial class EGGEditor : Form
    {
        public EGGEditor()
        {
            InitializeComponent();
        }

        private void pctSurface_MouseHover(object sender, EventArgs e)
        {
            
        }
        private void EGGEditor_Load(object sender, EventArgs e)
        {

        }
        public IntPtr getDrawSurface()  
        {  
            return pctSurface.Handle;  
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

        }

        private void propertyGrid1_Click(object sender, EventArgs e)
        {

        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void checkBox3_CheckedChanged(object sender, EventArgs e)
        {

        }

        private void pctSurface_Click(object sender, EventArgs e)
        {
            pctSurface.Show();
        }

        private void pctSurface_MouseDoubleClick(object sender, EventArgs e)
        {
            if (pctSurface.Visible == true)
            {
                pctSurface.Hide();
            }
            else
            {
                pctSurface.Show();
                pctSurface.PerformLayout();

            }
        }

        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {

        }

        private void aDDToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }
        private void EGGEditor_FormClosed(object sender, EventArgs e)
        {
            Application.Exit();
        }
    }
}
