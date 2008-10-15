using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace EGGEngine.Rendering
{
    public class PostProcessing
    {
        private VertexPositionTexture[] ppVertices;
        private RenderTarget2D targetRenderedTo;
        private ResolveTexture2D resolveTexture;
        private Effect ppEffect;
        private GraphicsDevice device;

        public PostProcessing(GraphicsDevice device)
        {
            this.device = device;
            InitPostProcessingVertices();
        }

        /// <summary>
        /// Sets up the vertices that cover the viewport
        /// </summary>
        private void InitPostProcessingVertices()
        {
            ppVertices = new VertexPositionTexture[4];
            int i = 0;
            ppVertices[i++] = new VertexPositionTexture(new Vector3(-1, 1, 0),
                new Vector2(0, 0));
            ppVertices[i++] = new VertexPositionTexture(new Vector3(1, 1, 0),
                new Vector2(1, 0));
            ppVertices[i++] = new VertexPositionTexture(new Vector3(-1, -1, 0),
                new Vector2(0, 1));
            ppVertices[i++] = new VertexPositionTexture(new Vector3(1, -1, 0),
                new Vector2(1, 1));
        }

        /// <summary>
        /// Loads the appropriate effect
        /// </summary>
        /// <param name="content">Content manager needed to load the appropriate effect</param>
        /// <param name="filename">The name of the file containing the effect</param>
        public void LoadEffect(ContentManager content, string filename)
        {
            PresentationParameters pp = device.PresentationParameters;
            targetRenderedTo = new RenderTarget2D(device, pp.BackBufferWidth,
                pp.BackBufferHeight, 1, device.DisplayMode.Format);
            resolveTexture = new ResolveTexture2D(device, pp.BackBufferWidth,
                pp.BackBufferHeight, 1, device.DisplayMode.Format);
            ppEffect = content.Load<Effect>(filename);
        }

        /// <summary>
        /// Runs the appropriate postprocess technique
        /// </summary>
        /// <param name="technique">The technique that is to be run</param>
        public void PostProcess(string technique)
        {
            device.ResolveBackBuffer(resolveTexture, 0);
            Texture2D textureRenderedTo = resolveTexture;

            ppEffect.CurrentTechnique = ppEffect.Techniques[technique];
            ppEffect.Begin();
            ppEffect.Parameters["textureToSampleFrom"].SetValue(textureRenderedTo);

            foreach(EffectPass pass in ppEffect.CurrentTechnique.Passes)
            {
                pass.Begin();
                device.VertexDeclaration = new VertexDeclaration(device,
                    VertexPositionTexture.VertexElements);
                device.DrawUserPrimitives<VertexPositionTexture>
                    (PrimitiveType.TriangleStrip, ppVertices, 0, 2);
                pass.End();
            }

            ppEffect.End();
        }

        /// <summary>
        /// Runs the given technique for postprocessing and sets the time if needed
        /// </summary>
        /// <param name="technique">The technique that is to be run</param>
        /// <param name="time">The time parameter used for the change over time effect</param>
        public void PostProcess(string technique, float time)
        {
            ppEffect.Parameters["xTime"].SetValue(time);
            PostProcess(technique);
        }
    }
}
