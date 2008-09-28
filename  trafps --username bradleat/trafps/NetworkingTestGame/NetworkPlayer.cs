using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Net;

namespace NetworkingTestGame
{

    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class NetworkPlayer 
    {
        // Constants control how fast the tank moves and turns.
        const float TankTurnRate = 0.01f;
        const float TurretTurnRate = 0.03f;
        const float TankSpeed = 0.3f;
        const float TankFriction = 0.9f;

        // The current position and rotation of the tank.
        public Vector2 Position;
        public Vector2 Velocity;
        public float TankRotation;
        public float TurretRotation;

        // Input controls can be read from keyboard, gamepad, or the network.
        public Vector2 TankInput;
        public Vector2 TurretInput;

        // Width and Heigh of sprite in texture
        public const int SHIPWIDTH = 30;
        public const int SHIPHEIGHT = 30;

        Rectangle spriteRectangle;

        // Textures used to draw the tank.
        Texture2D networkPlayerTexture;

        Vector2 screenSize;

        // Screen Area
        public Rectangle screenBounds;

        public NetworkPlayer(int gamerIndex, ContentManager content,
                    int screenWidth, int screenHeight)
        {
            // Use the gamer index to compute a starting position, so each player
            // starts in a different place as opposed to all on top of each other.
            Position.X = screenWidth / 4 + (gamerIndex % 5) * screenWidth / 8;
            Position.Y = screenHeight / 4 + (gamerIndex / 5) * screenHeight / 5;

            TankRotation = -MathHelper.PiOver2;
            TurretRotation = -MathHelper.PiOver2;

            networkPlayerTexture = content.Load<Texture2D>("RockRain");

            screenSize = new Vector2(screenWidth, screenHeight);

            spriteRectangle = new Rectangle(31, 83, SHIPWIDTH, SHIPHEIGHT);

            screenBounds = new Rectangle(0, 0,
                screenWidth, screenHeight);

        }

        /// <summary>
        /// Moves the tank in response to the current input settings.
        /// </summary>
        public void Update()
        {
            KeyboardState keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.Up))
            {
                Position.Y -= 3;

            }
            if (keyboard.IsKeyDown(Keys.Down))
            {
                Position.Y += 3;


            }
            if (keyboard.IsKeyDown(Keys.Left))
            {
                Position.X -= 3;


            }
            if (keyboard.IsKeyDown(Keys.Right))
            {
                Position.X += 3;

            }



            if (Position.X < screenBounds.Left)
            {
                Position.X = screenBounds.Left;
            }
            if (Position.X > screenBounds.Width - SHIPWIDTH)
            {
                Position.X = screenBounds.Width - SHIPWIDTH;
            }
            if (Position.Y < screenBounds.Top)
            {
                Position.Y = screenBounds.Top;
            }
            if (Position.Y > screenBounds.Height - SHIPHEIGHT)
            {
                Position.Y = screenBounds.Height - SHIPHEIGHT;
            }
            //base.Update(gameTime);
        }


        /// <summary>
        /// Gradually rotates the tank to face the specified direction.
        /// </summary>
        static float TurnToFace(float rotation, Vector2 target, float turnRate)
        {
            if (target == Vector2.Zero)
                return rotation;

            float angle = (float)Math.Atan2(-target.Y, target.X);

            float difference = rotation - angle;

            while (difference > MathHelper.Pi)
                difference -= MathHelper.TwoPi;

            while (difference < -MathHelper.Pi)
                difference += MathHelper.TwoPi;

            turnRate *= Math.Abs(difference);

            if (difference < 0)
                return rotation + Math.Min(turnRate, -difference);
            else
                return rotation - Math.Min(turnRate, difference);
        }


        /// <summary>
        /// Draws the tank and turret.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            Vector2 origin = new Vector2(networkPlayerTexture.Width / 2, networkPlayerTexture.Height / 2);

            spriteBatch.Draw(networkPlayerTexture, Position, spriteRectangle, Color.White,
                             TankRotation, origin, 1, SpriteEffects.None, 0);
        }
    }
}