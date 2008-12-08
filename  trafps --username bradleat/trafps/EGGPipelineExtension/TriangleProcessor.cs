using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;

namespace EGGPipelineExtension
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to apply custom processing to content data, converting an object of
    /// type TInput to TOutput. The input and output types may be the same if
    /// the processor wishes to alter data without changing its type.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    ///
    /// TODO: change the ContentProcessor attribute to specify the correct
    /// display name for this processor.
    /// </summary>
    [ContentProcessor]
    public class TriangleProcessor : ModelProcessor
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
                Matrix abstransform = mesh.AbsoluteTransform;

                foreach (GeometryContent geo in mesh.Geometry)
                {
                    int triangles = geo.Indices.Count / 3;
                    for (int currentTriangle = 0; currentTriangle < triangles; ++currentTriangle)
                    {
                        int index0 = geo.Indices[currentTriangle * 3 + 0];
                        int index1 = geo.Indices[currentTriangle * 3 + 1];
                        int index2 = geo.Indices[currentTriangle * 3 + 2];

                        Vector3 v0 = geo.Vertices.Positions[index0];
                        Vector3 v1 = geo.Vertices.Positions[index1];
                        Vector3 v2 = geo.Vertices.Positions[index2];

                        Vector3 transv0 = Vector3.Transform(v0, abstransform);
                        Vector3 transv1 = Vector3.Transform(v1, abstransform);
                        Vector3 transv2 = Vector3.Transform(v2, abstransform);

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
}