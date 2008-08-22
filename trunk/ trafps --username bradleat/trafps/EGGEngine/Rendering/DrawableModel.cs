using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGGEngine.Rendering
{
    public class DrawableModel
    {
        private Model model;
        private Matrix worldMatrix;
        private Matrix[] modelTransforms;

        #region Properties
        public Model Model
        {
            get { return model; }
        }
        public Matrix WorldMatrix
        {
            get { return worldMatrix; }
            set { worldMatrix = value; }
        }
        #endregion

        public DrawableModel(Model model, Matrix worldMatrix)
        {
            this.model = model;
            this.worldMatrix = worldMatrix;
            modelTransforms = new Matrix[model.Bones.Count];
        }

        public void Draw(Matrix viewMatrix, Matrix projectionMatrix)
        {
            model.CopyAbsoluteBoneTransformsTo(modelTransforms);
            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = worldMatrix;
                    effect.View = viewMatrix;
                    effect.Projection = projectionMatrix;
                }
                mesh.Draw();
            }
        }
    }
}
