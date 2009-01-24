using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace EGGEngine.Physics.RigidBodies
{
    public class Polyagonal
    {
        Primatives.Triangle[] mesh;
        Primatives.Line[] bones;
        float[] initialBoneLength;

        Vector3[] acceleration;
        Vector3[] position;
        Vector3[] oldPosition;

        #region Properties

        #endregion

        public Polyagonal(Primatives.Triangle[] Mesh, int[] IndexList)
        {
            this.mesh = Mesh;

            this.initialBoneLength = new float[bones.Length];
            this.acceleration = new Vector3[bones.Length * 2];
            this.position = new Vector3[bones.Length * 2];
            oldPosition = position;
        }

        public void Update(Vector3 Acceleration)
        {
            for (int i = 0; i < acceleration.Length; i++)
            {
                Vector3 temp = position[i];
                acceleration[i] = Acceleration;
                position[i] += position[i] - oldPosition[i] + acceleration[i];
                oldPosition[i] = temp;
            }
        }

    }
}
