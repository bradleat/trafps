using System;
using System.Collections.Generic;
using EGGEngine.ContentReaders;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace EGGPipelineExtension
{
    [ContentProcessor]
    public class ModelTriangleProcessor : ModelProcessor
    {
        public override ModelContent Process(NodeContent input, ContentProcessorContext context)
        {
            List<Triangle[]> modelTriangles = new List<Triangle[]>();
            modelTriangles = AddModelMeshTriangleArrayToList(input, modelTriangles);

            ModelContent usualModel = base.Process(input, context);

            int i = 0;
            foreach (ModelMeshContent mesh in usualModel.Meshes)
            {
                List<Triangle> modelMeshTriangles = new List<Triangle>();
                foreach (ModelMeshPartContent part in mesh.MeshParts)
                {
                    modelMeshTriangles.AddRange(modelTriangles[i++]);
                }
                mesh.Tag = modelMeshTriangles.ToArray();
            }

            return usualModel;
        }

        private List<Triangle[]> AddModelMeshTriangleArrayToList(NodeContent node, List<Triangle[]> triangleList)
        {
            foreach (NodeContent child in node.Children)
                triangleList = AddModelMeshTriangleArrayToList(child, triangleList);

            MeshContent mesh = node as MeshContent;

            if (mesh != null)
            {
                foreach (GeometryContent geo in mesh.Geometry)
                {
                    int triangles = geo.Indices.Count / 3;
                    List<Triangle> nodeTriangles = new List<Triangle>();

                    for (int currentTriangle = 0; currentTriangle < triangles; currentTriangle++)
                    {
                        int index0 = geo.Indices[currentTriangle * 3 + 0];
                        int index1 = geo.Indices[currentTriangle * 3 + 1];
                        int index2 = geo.Indices[currentTriangle * 3 + 2];

                        Vector3 v0 = geo.Vertices.Positions[index0];
                        Vector3 v1 = geo.Vertices.Positions[index1];
                        Vector3 v2 = geo.Vertices.Positions[index2];

                        Triangle newTriangle = new Triangle(v0, v1, v2);
                        nodeTriangles.Add(newTriangle);
                    }
                    triangleList.Add(nodeTriangles.ToArray());
                }
            }

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
}