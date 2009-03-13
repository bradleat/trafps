using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace EGGEngine.Rendering.Shaders
{
    class VertexShader : Shader
    {
        public override void ShadeModel(Model model, string technique)
        {
            Matrix[] modelTransforms = new Matrix[model.Bones.Count];
            model.CopyAbsoluteBoneTransformsTo(modelTransforms);
            int x = 0;

            foreach(ModelMesh mesh in model.Meshes)
            {
                foreach(Effect currentEffect in mesh.Effects)
                {
                    currentEffect.CurrentTechnique = currentEffect.Techniques[technique];
                    currentEffect.Parameters["xWorld"].SetValue(modelTransforms[mesh.ParentBone.Index] * world);
                    currentEffect.Parameters["xView"].SetValue(view);
                    currentEffect.Parameters["xProjection"].SetValue(projection);
                    currentEffect.Parameters["xlightColor"].SetValue(Color.White.ToVector4());
                    currentEffect.Parameters["xlightDirection"].SetValue(new Vector3(0, 0, 1));
                    currentEffect.Parameters["xambientColor"].SetValue(Color.CornflowerBlue.ToVector4());
                    currentEffect.Parameters["xTexture"].SetValue(textures[x++]);
                }
                mesh.Draw();
            }
        }
    }
}
