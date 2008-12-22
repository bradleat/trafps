using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using EGGEngine.Rendering;
using EGGEngine.Utils;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace EGGEditor01
{
    public partial class NewLevel : Form
    {
        Game1 game;
        EGGEditor eggEditor;
        public NewLevel(Game1 game, EGGEditor eggEditor)
        {
            InitializeComponent();
            this.eggEditor = eggEditor;
            this.game = game;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            for (int i = 0; i < game.models.Count; i++)
            {
                eggEditor.DeleteFromListBox(game.models[i]);
            }
            for (int i = 0; i < game.levelData.models.Count; i++)
            {
                game.Delete(game.levelData.models[i]);
            }
            game.models.Clear();
            this.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
