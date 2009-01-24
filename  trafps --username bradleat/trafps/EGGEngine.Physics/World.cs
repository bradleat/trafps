using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace EGGEngine.Physics
{
    public class World
    {
        Primatives.TriangleMesh mesh;

        #region Properties

        public Primatives.Triangle[] triangles
        {
            get { return mesh.Triangles; }
        }

        #endregion

        public World(Primatives.TriangleMesh Mesh)
        {
            this.mesh = Mesh;

        }

    }
}
