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

        public Primatives.TriangleMesh Mesh
        {
            get { return mesh; }
        }



        #endregion

        public World(Model model)
        {
            Dictionary<string, object> tagData = (Dictionary<string, object>)model.Tag;

            Vector3[] vertices = (Vector3[])tagData["Vertices"];
            Primatives.Triangle[] triangles = new Primatives.Triangle[vertices.Length/3];
            int n = 0;
            for (int i = 0; i < vertices.Length; )
            {
                Vector3 v1 = vertices[i++];
                Vector3 v2 = vertices[i++];
                Vector3 v3 = vertices[i++];
                triangles[n++] = new Primatives.Triangle(v1, v2, v3);
            }
            this.mesh = new Primatives.TriangleMesh(triangles);
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
