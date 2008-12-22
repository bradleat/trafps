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
        GameModel gameModel;
        Game1 game;
        EGGEditor eggEditor;
        
        public MoveModel1(DrawableModel model, GameModel gameModel, Game1 game, EGGEditor eggEditor)
        {
            InitializeComponent();
            this.model = model;
            this.gameModel = gameModel;
            this.game = game;
            this.eggEditor = eggEditor;
        }

        private void MoveModel1_Load(object sender, EventArgs e)
        {
            if (model != null)
                label5.Text = model.Position.ToString();
        }

        private void label6_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (checkBox1.Checked)
            {
                eggEditor.deletion = true;
                game.models.Remove(model);
                eggEditor.DeleteFromListBox(model);
                game.Delete(gameModel);
            }
            else
            {
                float floatvalueX = float.Parse(textBox1.Text);
                float floatvalueY = float.Parse(textBox2.Text);
                float floatvalueZ = float.Parse(textBox3.Text);

                model.Position = new Vector3(floatvalueX, floatvalueY, floatvalueZ);
                gameModel.position = model.Position;
            }
            this.Close();

        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            
        }
    }
}
