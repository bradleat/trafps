using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGGEngine.Rendering.Shaders
{
    public struct VertexExplosion
    {
        public Vector3 Position;
        public Vector4 TexCoord;
        public Vector4 AdditionalInfo;
        public VertexExplosion(Vector3 Position, Vector4 TexCoord, Vector4 AdditionalInfo)
        {
            this.Position = Position;
            this.TexCoord = TexCoord;
            this.AdditionalInfo = AdditionalInfo;
        }
        public static readonly VertexElement[] VertexElements = new VertexElement[]
        {
            new VertexElement(0, 0, VertexElementFormat.Vector3,
                VertexElementMethod.Default, VertexElementUsage.Position, 0),
            new VertexElement(0, 12, VertexElementFormat.Vector4,
                VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(0, 28, VertexElementFormat.Vector4,
                VertexElementMethod.Default, VertexElementUsage.TextureCoordinate, 1),
        };
        public static readonly int SizeInBytes = sizeof(float) * (3 + 4 + 4);
    }

    class Explosion : Particle
    {
        private VertexExplosion[] explosionVertices;

        public Explosion(GraphicsDevice device, int num)
        {
            this.device = device;
            this.rand = new Random();
            this.count = num;
            vertexDeclaration = new VertexDeclaration(device, VertexExplosion.VertexElements);
        }

        public override void CreateVertices(float time)
        {
            explosionVertices = new VertexExplosion[count * 6];

            int i = 0;
            for (int partnr = 0; partnr < count; ++partnr)
            {
                float r1 = (float)rand.NextDouble() - 0.5f;
                float r2 = (float)rand.NextDouble() - 0.5f;
                float r3 = (float)rand.NextDouble() - 0.5f;
                Vector3 moveDirection = new Vector3(r1, r2, r3);
                moveDirection.Normalize();

                float r4 = (float)rand.NextDouble();
                r4 = r4 / 4.0f * 3.0f + 0.25f;

                explosionVertices[i++] = new VertexExplosion(position,
                    new Vector4(1, 1, time, 1000), new Vector4(moveDirection, r4));
                explosionVertices[i++] = new VertexExplosion(position,
                    new Vector4(0, 0, time, 1000), new Vector4(moveDirection, r4));
                explosionVertices[i++] = new VertexExplosion(position,
                    new Vector4(1, 0, time, 1000), new Vector4(moveDirection, r4));

                explosionVertices[i++] = new VertexExplosion(position,
                    new Vector4(1, 1, time, 1000), new Vector4(moveDirection, r4));
                explosionVertices[i++] = new VertexExplosion(position,
                    new Vector4(0, 1, time, 1000), new Vector4(moveDirection, r4));
                explosionVertices[i++] = new VertexExplosion(position,
                    new Vector4(0, 0, time, 1000), new Vector4(moveDirection, r4));
            }
        }

        public override void DrawParticles(GameTime gameTime)
        {
            if (explosionVertices != null)
            {
                effect.CurrentTechnique = effect.Techniques["Explosion"];
                effect.Parameters["xWorld"].SetValue(world);
                effect.Parameters["xProjection"].SetValue(projection);
                effect.Parameters["xView"].SetValue(view);

                effect.Parameters["xCamPos"].SetValue(camPos);
                effect.Parameters["xExplosionTexture"].SetValue(pTexture);
                effect.Parameters["xCamUp"].SetValue(camUp);
                effect.Parameters["xTime"].SetValue((float)gameTime.TotalGameTime.TotalMilliseconds);
                effect.Parameters["xExplosionTexture"].SetValue(pTexture);

                device.RenderState.AlphaBlendEnable = true;
                device.RenderState.SourceBlend = Blend.SourceAlpha;
                device.RenderState.DestinationBlend = Blend.One;
                device.RenderState.DepthBufferWriteEnable = false;

                effect.Begin();
                foreach (EffectPass pass in effect.CurrentTechnique.Passes)
                {
                    pass.Begin();
                    device.VertexDeclaration = vertexDeclaration;
                    device.DrawUserPrimitives<VertexExplosion>
                        (PrimitiveType.TriangleList, explosionVertices, 0, explosionVertices.Length / 3);
                    pass.End();
                }
                effect.End();

                device.RenderState.DepthBufferWriteEnable = true;
            }            
        }
    }
}
