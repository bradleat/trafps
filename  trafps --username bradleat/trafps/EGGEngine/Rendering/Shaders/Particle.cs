using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace EGGEngine.Rendering.Shaders
{
    abstract class Particle : Shader
    {
        protected Texture2D pTexture;
        protected VertexDeclaration vertexDeclaration;
        protected GraphicsDevice device;
        protected Random rand;
        protected int count;
        public Vector3 position;
        protected Vector3 camPos;
        protected Vector3 camUp;

        #region Properties
        public Texture2D PTexture
        {
            get { return pTexture; }
            set { pTexture = value; }
        }
        public VertexDeclaration VertexDeclaration
        {
            get { return vertexDeclaration; }
        }
        public GraphicsDevice Device
        {
            get { return device; }
            set { device = value; }
        }
        public int Count
        {
            get { return count; }
            set { count = value; }
        }
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }
        public Vector3 CamPos
        {
            set { camPos = value; }
        }
        public Vector3 CamUp
        {
            set { camUp = value; }
        }
        #endregion

        public abstract void CreateVertices(float time);
        public abstract void DrawParticles(GameTime gameTime);
    }
}
