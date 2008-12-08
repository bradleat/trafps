using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace EGGPipelineExtension
{
    public class Triangle
    {
        private Vector3[] points;

        public Triangle(Vector3 p0, Vector3 p1, Vector3 p2)
        {
            points = new Vector3[3];
            points[0] = p0;
            points[1] = p1;
            points[2] = p2;
        }

        public Vector3[] Points { get { return points; } }
        public Vector3 P0 { get { return points[0]; } }
        public Vector3 P1 { get { return points[1]; } }
        public Vector3 P2 { get { return points[2]; } }
    }

    [ContentProcessor]
    public class ModelTriangleProcessor : ModelProcessor
    {
        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            ModelContent usualModel = base.Process(input, context);

            List<Triangle> triangles = new List<Triangle>();
            triangles = AddVerticesToList(input, triangles);
            usualModel.Tag = triangles.ToArray();

            return usualModel;
        }

        private List<Triangle> AddVerticesToList(NodeContent node, List<Triangle> triangleList)
        {
            MeshContent mesh = node as MeshContent;
            if (mesh != null)
            {
                Matrix absTransform = mesh.AbsoluteTransform;

                foreach (GeometryContent geo in mesh.Geometry)
                {
                    int triangles = geo.Indices.Count / 3;
                    for (int currentTriangle = 0; currentTriangle < triangles; currentTriangle++)
                    {
                        int index0 = geo.Indices[currentTriangle * 3 + 0];
                        int index1 = geo.Indices[currentTriangle * 3 + 1];
                        int index2 = geo.Indices[currentTriangle * 3 + 2];

                        Vector3 v0 = geo.Vertices.Positions[index0];
                        Vector3 v1 = geo.Vertices.Positions[index1];
                        Vector3 v2 = geo.Vertices.Positions[index2];

                        Vector3 transv0 = Vector3.Transform(v0, absTransform);
                        Vector3 transv1 = Vector3.Transform(v1, absTransform);
                        Vector3 transv2 = Vector3.Transform(v2, absTransform);

                        Triangle newTriangle = new Triangle(transv0, transv1, transv2);
                        triangleList.Add(newTriangle);
                    }
                }
            }

            foreach (NodeContent child in node.Children)
                triangleList = AddVerticesToList(child, triangleList);

            return triangleList;
        }        
    }

    [ContentTypeWriter]
    public class TriangleTypeWriter : ContentTypeWriter<Triangle>
    {
        protected override void Write(ContentWriter output, Triangle value)
        {
            output.WriteObject<Vector3>(value.P0);
            output.WriteObject<Vector3>(value.P1);
            output.WriteObject<Vector3>(value.P2);
        }

        public override string GetRuntimeReader(TargetPlatform targetPlatform)
        {
            return typeof(TriangleTypeReader).AssemblyQualifiedName;
        }
    }        

    public class TriangleTypeReader : ContentTypeReader<Triangle>
    {
        protected override Triangle Read(ContentReader input, Triangle existingInstance)
        {
            Vector3 p0 = input.ReadObject<Vector3>();
            Vector3 p1 = input.ReadObject<Vector3>();
            Vector3 p2 = input.ReadObject<Vector3>();

            Triangle newTriangle = new Triangle(p0, p1, p2);

            return newTriangle;
        }
    }
}