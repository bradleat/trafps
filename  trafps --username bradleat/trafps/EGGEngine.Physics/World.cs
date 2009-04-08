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
        public List<int>[][] OctreeReferences = new List<int>[numDivisions + 1][];

        Primatives.TriangleMesh mesh;
        const short numDivisions = 2;
        Octree octree;
        



        #region Properties

        public Stack<Primatives.Triangle> OctMesh(Vector3 Position)
        {
            int[] octlets = octree.allOctlets(Position);

            Stack<Primatives.Triangle> triangles = new Stack<Primatives.Triangle>();

            for(short i = 0; i < octlets.Length; i++)
            {
                foreach (int triangle in OctreeReferences[i][octlets[i]])
                {
                    triangles.Push(mesh.Triangles[triangle]);
                }
            }

            return triangles;
            
        }




        public Primatives.TriangleMesh Mesh
        {
            get { return mesh; }
        }

        #endregion

        public World(Model model)
        {

            int a = 1;
            for (int i = 0; i < numDivisions + 1; i++)
            {
                int numOcts = a;
                OctreeReferences[i] = new List<int>[numOcts];
                a *= 8;
            }

            Dictionary<string, object> tagData = (Dictionary<string, object>)model.Tag;
            Vector3[] vertices = (Vector3[])tagData["Vertices"];
            Primatives.Triangle[] triangles = new Primatives.Triangle[vertices.Length / 3];
            int n = 0;
            for (int i = 0; i < vertices.Length; )
            {
                Vector3 v1 = vertices[i++];
                Vector3 v2 = vertices[i++];
                Vector3 v3 = vertices[i++];

                triangles[n++] = new Primatives.Triangle(v1, v2, v3);
            }

            octree = new Octree(new Primatives.TriangleMesh(triangles), numDivisions);
            this.mesh = octree.OctTaggedMesh;

            for (int i = 0; i < mesh.Triangles.Length; i++)
            {
                int octlet = mesh.Triangles[i].OctTag[0];
                int level = mesh.Triangles[i].OctTag[1];

                if (OctreeReferences[level][octlet] == null)
                {
                    OctreeReferences[level][octlet] = new List<int>();
                }
                OctreeReferences[level][octlet].Add(i);
            }

        }
    }
}
