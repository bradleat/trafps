using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Content.Pipeline;
using Microsoft.Xna.Framework.Content.Pipeline.Graphics;
using Microsoft.Xna.Framework.Content.Pipeline.Processors;
using Microsoft.Xna.Framework.Content.Pipeline.Serialization.Compiler;

namespace EGGPipelineExtension
{
    /// <summary>
    /// This class will be instantiated by the XNA Framework Content Pipeline
    /// to write the specified data type into binary .xnb format.
    ///
    /// This should be part of a Content Pipeline Extension Library project.
    /// </summary>
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
