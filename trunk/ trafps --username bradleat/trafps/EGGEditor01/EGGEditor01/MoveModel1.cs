using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows.Forms;
using EGGEngine.Rendering;
using EGGEngine.Utils;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGGEditor01
{
    public partial class MoveModel1 : Form
    {
        DrawableModel model;
        public MoveModel1(DrawableModel model)
        {
            InitializeComponent();
            this.model = model;
        }

        private void MoveModel1_Load(object sender, EventArgs e)
        {
            label5.Text = model.Position.ToString();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            
            float floatvalueX = float.Parse(textBox1.Text);
            float floatvalueY = float.Parse(textBox2.Text);
            float floatvalueZ = float.Parse(textBox3.Text);

            model.Position = new Vector3(floatvalueX, floatvalueY, floatvalueZ);
            this.Close();
        }
    }
}
