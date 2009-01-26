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
        Primatives.TriangleMesh[] mesh;
        int divisions = 2;

        #region Properties

        public Primatives.TriangleMesh[] Mesh
        {
            get { return mesh; }
        }



        #endregion

        public World(Primatives.TriangleMesh[] Mesh)
        {
            this.mesh = Mesh;
        }

        //Vector2 BoxSize(Primatives.TriangleMesh Mesh)
        //{
        //   Vector2 boxSize = Vector2.Zero;

        //    for (int i = 0; i < mesh.Triangles.Length; i++)
        //    {
        //        for (int x = 0; x < 3; x++)
        //        {
        //            if (mesh.Triangles[i].Vertices[x].X > boxSize.X)
        //                boxSize.X = mesh.Triangles[i].Vertices[x].X;
        //            if (mesh.Triangles[i].Vertices[x].Z > boxSize.Y)
        //                boxSize.Y = mesh.Triangles[i].Vertices[x].Z;
        //        }
        //    }

        //    return boxSize;
        //}

    }
}
