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

namespace EGGEditor01
{

    public partial class EGGEditor : Form
    {
        Game1 _game;
        public struct LevelData 
        {
            Vector2 position;
            int Tilenumber;
            public Vector3 position2;
        }
        LevelData levelData;
        public EGGEditor()
        {
            InitializeComponent();
        }
        public EGGEditor(Game game)
        {
            InitializeComponent();
            _game = (EGGEditor01.Game1)game;
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

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string filename;
            saveFileDialog1.Title = "Specify Destination Filename";
            saveFileDialog1.Filter = "Level Files|*.lvl";
            saveFileDialog1.FilterIndex = 1;
            saveFileDialog1.OverwritePrompt = true;


            if (openFileDialog1.ShowDialog() != DialogResult.Cancel)
            {
                filename = saveFileDialog1.FileName;
                SaveLevel(filename);
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
                OpenLevel(filename);
            }
            else
                return;
        }

        private void OpenLevel(string filename)
        {
            IAsyncResult result = null;
            SerializeUtils<LevelData> levelData2 = new SerializeUtils<LevelData>();
            if (!Guide.IsVisible)
            {
                result = Guide.BeginShowStorageDeviceSelector(PlayerIndex.One, null, null);
            }
            if (result.IsCompleted)
            {
                StorageDevice device = Guide.EndShowStorageDeviceSelector(result);
                levelData2.LoadData(device, "map01");
                LoadLevel();
            }
        }
        private void LoadLevel()
        {
            // Code for taking the level data from the file and decode it to load the level
        }
        private void SaveLevel(string filename)
        {
            IAsyncResult result = null;

            SerializeUtils<LevelData> levelData2 = new SerializeUtils<LevelData>();

            //SerializeUtils<int> intData = new SerializeUtils<int>();
            levelData2.Data = levelData;
            if (!Guide.IsVisible)
            {
                result = Guide.BeginShowStorageDeviceSelector(PlayerIndex.One, null, null);
            }
            if (result.IsCompleted)
            {
                StorageDevice device = Guide.EndShowStorageDeviceSelector(result);
                if (device.IsConnected)
                    levelData2.SaveData(device, filename);
            }
        }
    }
}
