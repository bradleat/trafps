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

    public partial class EGGEditor : Form
    {
        Game1 game;
        public bool deletion = false;
        
        
        public EGGEditor()
        {
            InitializeComponent();
            game = new Game1(this);
            this.Show();
            game.Run();
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
            propertyGrid1.Update();
        }
        private void propertyGrid1_TextChanged(object sender, EventArgs e)
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
            if (treeView1.SelectedNode.Text == "Player1")
            {
                string name = game.GetName();
                DrawableModel newModel = new DrawableModel(game.Content.Load<Model>(name), Matrix.Identity);
                GameModel gameModel = new GameModel();
                game.models.Add(newModel);
                MoveModel1 moveModel = new MoveModel1(newModel, gameModel,game, this);
                moveModel.Show();
                listBox1.Items.Add(newModel);
                propertyGrid1.SelectedObject = newModel;
            }
            
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

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filename;
            saveFileDialog1.Title = "Specify Destination Filename";
            saveFileDialog1.Filter = "Level Files|*.lvl";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.OverwritePrompt = true;


            if (saveFileDialog1.ShowDialog() != DialogResult.Cancel)
            {
                filename = saveFileDialog1.FileName;
                game.SaveLevel(filename);
            }
            else
                return;

        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filename;
            openFileDialog1.InitialDirectory = @"C:\";
            openFileDialog1.Title = "Select a File";
            openFileDialog1.Filter = "level File|*.lvl";
            filename = openFileDialog1.FileName;

            if (openFileDialog1.ShowDialog() != DialogResult.Cancel)
            {
                filename = openFileDialog1.FileName;
                game.OpenLevel(filename);
            }
            else
                return;
        }

        public void UpdateListbox()
        {
            listBox1.Update();
        }
        public void AddToListBox(DrawableModel model)
        {
            listBox1.Items.Add(model);
        }
        public void DeleteFromListBox(DrawableModel model)
        {
            listBox1.Items.Remove(model);
        }
        public void UpdatePropertyGrid(DrawableModel model)
        {
            propertyGrid1.Update();
        }
        public void Prop_ChangeSelected(DrawableModel model)
        {
            propertyGrid1.SelectedObject = model;
        }
        public void IncrementProgressBar(int incrementValue)
        {
            if(progressBar1.Visible == false)
                progressBar1.Show();
            progressBar1.Increment(incrementValue);

            if (progressBar1.Value == 100)
                progressBar1.Value = 0;
        }

        
        private void playerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            progressBar1.Show();
            progressBar1.Select();
            progressBar1.Step = 50;     
            string name = game.GetName();
            Model model = game.Content.Load<Model>(name);
            GameModel gameModel = new GameModel();
            DrawableModel newModel = new DrawableModel(model, Matrix.Identity);
            progressBar1.PerformStep();
            game.models.Add(newModel);
            game.Add(gameModel);
            MoveModel1 moveModel = new MoveModel1(newModel,gameModel, game , this);
            moveModel.Show();
            listBox1.Items.Add(newModel);
            propertyGrid1.SelectedObject = newModel;
            progressBar1.PerformStep();
            progressBar1.Value = 0;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (deletion == false)
            {
                DrawableModel item = (DrawableModel)listBox1.SelectedItem;
                MoveModel1 moveModel = new MoveModel1(item, new GameModel(), game, this);
                moveModel.Show(); 
            }
            else
                deletion = true;
        }

        private void newLevelToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NewLevel newLevel = new NewLevel(game, this);
            newLevel.Show();
            
        }

       
    }
}
