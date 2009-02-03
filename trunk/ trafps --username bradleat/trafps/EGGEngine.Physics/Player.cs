using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

namespace EGGEngine.Physics
{
    public class Player
    {
        Vector3 position;
        Vector3 velocity;
        float rotation;
        float speed;
        Vector3 boundingBoxSize;
        Vector3 oldPosition;
        Intersection intersection;
        Primatives.TriangleMesh world;

        const float walkSpeed = .1f;
        const float runSpeed = .3f;
        const float sprintSpeed = .5f;
        float Gravity = -0.1f;


        #region Properties
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }

        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        public float Speed
        {
            get { return speed; }
        }
        public Vector3 Velocity
        {
            get { return velocity; }
        }
        #endregion

        /// <param name="InitialPosition">The player's Initial Position</param>
        /// <param name="InitialRotation">The player's Initial Rotation</param>
        /// <param name="BoundingBoxSize">The Height, Width and Depth of the player</param>


        public Player(Vector3 InitialPosition, float InitialRotation, Vector3 BoundingBoxSize, World world)
        {
            this.world = world.Mesh;
            this.position = InitialPosition;
            this.rotation = InitialRotation;
            this.boundingBoxSize = BoundingBoxSize;
        }

        /// <param name="State">0 - standing, 1 - walking, 2 running, 3 - sprinting</param>
        /// <param name="MoveDirection">The Direction the player is moving as a Vector2, eg. a for a player running forwards, use Vector2.Forward</param>

        public void Update(Vector2 MoveDirection, int State)
        {

            switch (State)
            {
                case 0:
                    speed *= 0.8f;
                    break;
                case 1:
                    speed += (walkSpeed - speed) * 0.8f;
                    break;
                case 2:
                    speed += (runSpeed - speed) * 0.8f;
                    break;
                case 3:
                    speed += (sprintSpeed - speed) * 0.8f;
                    break;
            }

            oldPosition = position;

            velocity.X = MoveDirection.X * speed;
            velocity.Z = MoveDirection.Y * speed;
            velocity.Y += Gravity;

            position += Vector3.Transform(Velocity, Matrix.CreateRotationY(rotation));

            intersection = new Intersection();

            //float[] intersectionPoints = new float[4];

                Primatives.Line Line = new Primatives.Line(position - boundingBoxSize.Y * Vector3.UnitY / 2, position + boundingBoxSize.Y * Vector3.UnitY / 2);

                for (int i = 0; i < world.Triangles.Length; i++)
                {
                    float intersectionPoint = intersection.LineTriangle(world.Triangles[i], Line);
                    if (intersectionPoint != -1)
                    {
                        velocity.Y = 0f;
                        if (intersectionPoint < 0.3f)
                        {
                            position.Y += intersectionPoint * boundingBoxSize.Y;
                        }
                        else
                        {
                            position = oldPosition;
                        }
                    }
                }


        }
    }

}
