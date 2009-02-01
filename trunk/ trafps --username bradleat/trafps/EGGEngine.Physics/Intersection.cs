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


        bool PointInTriangle(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
        {      
            Vector3 v0 = c - a;
            Vector3 v1 = b - a;
            Vector3 v2 = p - a;

            float dot00 = Vector3.Dot(v0, v0);
            float dot01 = Vector3.Dot(v0, v1);
            float dot02 = Vector3.Dot(v0, v2);
            float dot11 = Vector3.Dot(v1, v1);
            float dot12 = Vector3.Dot(v1, v2);

            float invDenom = 1 / (dot00 * dot11 - dot01 * dot01);
            float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
            float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

            return (u > 0) && (v > 0) && (u + v < 1);
        }
    }

}
