using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace EGGEngine.ContentReaders
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

        #region Properties
        public Vector3[] Points { get { return points; } }
        public Vector3 P0 { get { return points[0]; } }
        public Vector3 P1 { get { return points[1]; } }
        public Vector3 P2 { get { return points[2]; } }
        #endregion
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

