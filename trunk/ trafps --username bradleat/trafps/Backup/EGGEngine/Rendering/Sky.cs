#region File Description
//-----------------------------------------------------------------------------
// Sky.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace EGGEngine.Rendering
{
    /// <summary>
    /// Runtime class for loading and rendering a textured skydome
    /// that was created during the build process by the SkyProcessor.
    /// </summary>
    public class Sky
    {
        #region Fields

        Model skyModel;
        Texture skyTexture;

        #endregion


        /// <summary>
        /// Constructor is internal: this should only ever
        /// be called by the SkyReader class.
        /// </summary>
        internal Sky(ContentReader input)
        {
            skyModel = input.ReadObject<Model>();
            skyTexture = input.ReadObject<Texture>();
        }


        /// <summary>
        /// Helper for drawing the skydome mesh.
        /// </summary>
        public void Draw(Matrix view, Matrix projection)
        {
            foreach (ModelMesh mesh in skyModel.Meshes)
            {
                foreach (Effect effect in mesh.Effects)
                {
                    effect.Parameters["View"].SetValue(view);
                    effect.Parameters["Projection"].SetValue(projection);
                    effect.Parameters["Texture"].SetValue(skyTexture);
                }

                mesh.Draw(SaveStateMode.SaveState);
            }
        }
    }


    /// <summary>
    /// Helper for reading a Sky object from the compiled XNB format.
    /// </summary>
    public class SkyReader : ContentTypeReader<Sky>
    {
        protected override Sky Read(ContentReader input, Sky existingInstance)
        {
            return new Sky(input);
        }
    }
}
