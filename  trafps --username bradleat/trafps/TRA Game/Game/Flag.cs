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

using EGGEngine.Cameras;


namespace TRA_Game
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Flag 
    {
        private Vector3 position;
        private float rotation;
        private Model model;
        private bool idle;


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
            set { model = value; }
        }
        public bool Idle
        {
            get { return idle; }
            set { idle = value; }
        }

        public Flag(Vector3 initialPosition)
        {
            this.Position = initialPosition;
            this.model = model;
            this.rotation = rotation;
            this.idle = true;
        }

        public void Draw(GameTime gameTime, FirstPersonCamera camera)
        {
            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in model.Meshes)
            {

                // This is where the mesh orientation is set, as well as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = Matrix.CreateRotationY(this.rotation) * Matrix.CreateTranslation(this.position);
                    effect.View = camera.ViewMatrix;
                    float aspectRatio = 1.0f;
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f),
                        aspectRatio, 1.0f, 10000.0f);
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
        }
    }
}