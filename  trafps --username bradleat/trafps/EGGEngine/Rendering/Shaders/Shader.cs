using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace EGGEngine.Rendering.Shaders
{
    class Shader
    {
        protected Matrix world;
        protected Matrix view;
        protected Matrix projection;
        protected Effect effect;
        protected Texture2D[] textures;

        #region Properties
        public Matrix World
        {
            get { return world; }
            set { world = value; }
        }
        public Matrix View
        {
            get { return view; }
            set { view = value; }
        }
        public Matrix Projection
        {
            get { return projection; }
            set { projection = value; }
        }
        public Effect Effect
        {
            get { return effect; }
            set { effect = value; }

        }
        public Texture2D[] Textures
        {
            get { return textures; }
            set { textures = value; }
        }
        #endregion

        public void LoadEffect(ContentManager content, string filename)
        {
            effect = content.Load<Effect>(filename);
        }

        public virtual void ShadeModel(Model model, string technique)
        {
            Matrix[] modelTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(modelTransforms);
            int x = 0;

            foreach (ModelMesh mesh in model.Meshes)
            {
                foreach (Effect currentEffect in mesh.Effects)
                {
                    currentEffect.CurrentTechnique = currentEffect.Techniques[technique];
                    currentEffect.Parameters["xWorld"].SetValue(modelTransforms[mesh.ParentBone.Index] * world);
                    currentEffect.Parameters["xView"].SetValue(view);
                    currentEffect.Parameters["xProjection"].SetValue(projection);
                    currentEffect.Parameters["xTexture"].SetValue(textures[x++]);
                }
                mesh.Draw();
            }
        }
    }
}
