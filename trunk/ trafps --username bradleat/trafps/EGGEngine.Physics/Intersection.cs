using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace EGGEngine.Physics
{
    class Intersection
    {
        public float LineTriangle(Primatives.Triangle Triangle, Primatives.Line Line)
        {
            {
            float x = -Vector3.Dot((Line.Vertices[0] - Triangle.Vertices[0]), Triangle.Normal) / Vector3.Dot(Line.Vertices[1] - Line.Vertices[0], Triangle.Normal);
            if (x < 0 || x > 1)
                return -1;
            else
            {
                Vector3 intersection = Line.Vertices[0] + x * Vector3.Normalize(Line.Vertices[1] - Line.Vertices[0]);
                if (PointInTriangle(intersection, Triangle.Vertices[0], Triangle.Vertices[1], Triangle.Vertices[2]))
                    return x;
                else
                    return -1;
            }
            }
        }

        bool SameSide(Vector3 p1, Vector3 p2, Vector3 a, Vector3 b)
        {
            Vector3 cp1 = Vector3.Cross(b - a, p1 - a);
            Vector3 cp2 = Vector3.Cross(b - a, p2 - a);
            if (Vector3.Dot(cp1, cp2) >= 0)
                return true;
            else
                return false;
        }
        bool PointInTriangle(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
        {
            if (SameSide(p, a, b, c) &&
                SameSide(p, b, a, c) &&
                SameSide(p, c, a, b))
                return true;
            else
                return false;
        }
    }

}
