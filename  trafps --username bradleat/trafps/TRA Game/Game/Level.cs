using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using EGGEngine.Rendering;
using EGGEngine.Physics;
using EGGEngine.Cameras;


namespace TRA_Game
{
    public struct GameLevel
    {
        public Level level;
        public Sky sky;
        public Player player;
        public World world;
        public Weapon weapon;
    }
    
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Level : GameComponent 
    {
        private Vector3 position;
        private float rotation;
        private Model model;
        private Matrix[] boneTransforms;

        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        public Model Model
        {
            get { return model; }
        }

        public Level(Game game, string levelModelFilename)
            : base(game)
        {
            model = game.Content.Load<Model>(levelModelFilename);
            boneTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(boneTransforms);
        }

        public void Draw(FirstPersonCamera camera)
        {
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();

                    effect.World = boneTransforms[mesh.ParentBone.Index] * Matrix.CreateRotationY(rotation)
                                                                        * Matrix.CreateTranslation(position);
                    effect.SpecularColor = new Vector3(1, 0, 0);
                    effect.View = camera.ViewMatrix;
                    effect.Projection = camera.ProjectionMatrix;
                }

                mesh.Draw();
            }
        }
    }
}