#region License
//=============================================================================
// System  : Game
// File    : Octree.cs
// Author  : Hannes Hille
// Note    : Copyright 2008, Portal Games, All Rights Reserved
// Compiler: Microsoft C#
//
// This file is for generation and processing of an octree space partioning
// system.
// 
// This code is published under the Microsoft Reciprocal License (Ms-RL). A 
// copy of the license should be distributed with the code. It can also be found
// at the project website: http://www.CodePlex.com/trafps. This notice, the
// author's name, and all copyright notices must remain intact in all
// applications, documentation, and source files.
//
// ============================================================================
#endregion


using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace EGGEngine.Physics
{
    public class Octree
    {
        Vector3 size;
        Vector3 center;
        int maxDivisions;
        Primatives.TriangleMesh mesh;

        public Octree(Primatives.TriangleMesh Mesh, int MaxLevels)
        {
            Vector3[] maxmin = maxMin(Mesh);
            Vector3 max = maxmin[0];
            Vector3 min = maxmin[1];

            this.mesh = Mesh;

            this.maxDivisions = MaxLevels;

            center = (maxmin[0] + maxmin[1]) / 2;
            size = (maxmin[0] - maxmin[1]);

        }

        public int[] allOctlets(Vector3 Point)
        {
            Vector3 max = size / 2 + center;
            Vector3 min = size / -2 + center;
            Vector3 levelCenter;

            int[] octlets = new int[maxDivisions + 1];

            octlets[0] = 0;

            for(short i = 1; i < maxDivisions + 1; i++)
            {
                levelCenter = (max + min) / 2;

                short leafNode = LeafNode(Point, levelCenter);
                octlets[i] = octlets[i-1] * 8 + leafNode;

                if(leafNode < 4)    max.Z = levelCenter.Z;
                else                min.Z = levelCenter.Z;
                if(leafNode == 0
                   || leafNode == 1  
                   || leafNode == 4
                   || leafNode == 5) max.Y = levelCenter.Y;
                else                 min.Y = levelCenter.Y;
                if(leafNode == 0
                   || leafNode == 2  
                   || leafNode == 4
                   || leafNode == 6) max.X = levelCenter.X;
                else                 min.X = levelCenter.X;
            }

            return octlets;

        }


        Vector3[] maxMin(Primatives.TriangleMesh Mesh)
        {
            Vector3 max = Vector3.Zero;
            Vector3 min = Vector3.Zero;
            Vector3[] maxmin = new Vector3[2];

            foreach(Primatives.Triangle Triangle in Mesh.Triangles)
            {
                for(int i = 0; i < 3; i++)
                {
                    if (Triangle.Vertices[i].X > max.X)
                        max.X = Triangle.Vertices[i].X;
                    if (Triangle.Vertices[i].Y > max.Y)
                        max.Y = Triangle.Vertices[i].Y;
                    if (Triangle.Vertices[i].Z > max.Z)
                        max.Z = Triangle.Vertices[i].Z;

                    if (Triangle.Vertices[i].X < min.X)
                        min.X = Triangle.Vertices[i].X;
                    if (Triangle.Vertices[i].Y < min.Y)
                        min.Y = Triangle.Vertices[i].Y;
                    if (Triangle.Vertices[i].Z < min.Z)
                        min.Z = Triangle.Vertices[i].Z; 
                }
            }

            maxmin[0] = max;
            maxmin[1] = min;

            return maxmin;


        }

        public short LeafNode(Vector3 Point, Vector3 Center)
        {
            short oct = 0;

            if (Point.X > Center.X)
                oct += 1;
            if (Point.Y > Center.Y)
                oct += 2;
            if (Point.Z > Center.Z)
                oct += 4;
            return oct;
        }

        public int[] SmallestOct(Vector3[] Vertices)
        {

            int octlet = 0;
            int finalLevel = 0;

            Vector3 max = size / 2 + center;
            Vector3 min = size / -2 + center;

            for (int level = 0; level < maxDivisions; level++)
            {
                Vector3 levelCenter = (max + min)/2;

                short oct = LeafNode(Vertices[0], levelCenter);

                for (int i = 1; i < Vertices.Length; i++)
                {
                    if (LeafNode(Vertices[i], levelCenter) != oct)
                    {
                        goto Return;
                    }                        
                }

                octlet = (octlet * 8) + oct;

                finalLevel += 1;

                if (oct < 4) max.Z = levelCenter.Z;
                else min.Z = levelCenter.Z;
                if (oct == 0
                   || oct == 1
                   || oct == 4
                   || oct == 5) max.Y = levelCenter.Y;
                else min.Y = levelCenter.Y;
                if (oct == 0
                   || oct == 2
                   || oct == 4
                   || oct == 6) max.X = levelCenter.X;
                else min.X = levelCenter.X;

            }

            goto Return;

            Return:
            {
                int levels = finalLevel;
                return new int[] { octlet, levels };
            }
        }

        public Primatives.TriangleMesh OctTaggedMesh
        {
            get
            {
                for (int i = 0; i < mesh.Triangles.Length; i++)
                {
                    mesh.Triangles[i].OctTag = SmallestOct(mesh.Triangles[i].Vertices);
                }

                return mesh;
            }

        }

        }

    }
