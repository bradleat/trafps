using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace EGGEngine.Physics
{
    public class Primatives
    {

        public class Triangle
        {
            private Vector3[] vertices;
            private Vector3 normal;

            public Triangle(Vector3 v1, Vector3 v2, Vector3 v3)
            {
                vertices = new Vector3[3];
                vertices[0] = v1;
                vertices[1] = v2;
                vertices[2] = v3;
                normal = Vector3.Normalize(Vector3.Cross(v2 - v1, v3 - v1));

            }


            public Vector3[] Vertices
            {
                get { return vertices; }
            }
            public Vector3 Normal
            {
                get { return normal; }
            }
        }

        public class TriangleMesh
        {
            private Triangle[] triangles;

            public TriangleMesh(Triangle[] Triangles)
            {
                this.triangles = Triangles;
            }


            public Triangle[] Triangles
            {
                get { return triangles; }
            }

            public void AddToMesh(Triangle[] Triangles)
            {
                Triangle[] oldTriangles = triangles;
                triangles = new Triangle[triangles.Length + triangles.Length];
                for (int i = 0; i < triangles.Length; i++)
                    triangles[i] = oldTriangles[i];
                for (int i = triangles.Length; i < (triangles.Length + triangles.Length); i++)
                    triangles[i] = Triangles[i - triangles.Length];
            }
        }

        public class Line
        {
            private Vector3[] vertices;
            private float length;

            public Line(Vector3 v1, Vector3 v2)
            {
                vertices = new Vector3[2];
                vertices[0] = v1;
                vertices[1] = v2;
                length = Vector3.Distance(v1,v2);
            }

            public Vector3[] Vertices
            {
                get { return vertices; }
            }
            public float Length
            {
                get { return length; }
            }
        }

    }
}
