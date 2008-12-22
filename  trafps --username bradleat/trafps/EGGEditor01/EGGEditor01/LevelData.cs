using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EGGEngine.Rendering;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;


namespace EGGEditor01
{
    
    public struct LevelData
    {
        public List<GameModel> models;
        
        //public List<int> models;
        //public List<Vector3> positions;
    }

    public class Data
    {
        LevelData levelData;

        public Data()
        {
            levelData.models = new List<GameModel>();
            //levelData.positions = new List<Vector3>();
        }
        
        public void Add(GameModel model)
        {
            levelData.models.Add(model);
        }
    }
}
