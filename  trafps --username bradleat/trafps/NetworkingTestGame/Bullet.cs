#region (License)
//=============================================================================
// System  : Networking Game Loop
// File    : Bullet.cs
// Author  : Evan
// Note    : Copyright 2008, Portal Games, All Rights Reserved
// Compiler: Microsoft C#
//
// This file contains the Bullets.
//
// This code is published under the Microsoft Reciprocal License (Ms-RL). A 
// copy of the license should be distributed with the code. It can also be found
// at the project website: http://www.CodePlex.com/trafps. This notice, the
// author's name, and all copyright notices must remain intact in all
// applications, documentation, and source files.
//
// 
// Todos: 
//
// ============================================================================ 
#endregion

#region Revision Number
// Revision Number: 0.1.0.0
// Revision Number: Major.Minor.Build.Bug
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

#endregion

namespace NetworkingTestGame
{
    /// <summary>
    /// Contains methods for creating, updating, drawing and checks for collision between bullets
    /// </summary>
    public class Bullet : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Properties
        Texture2D texture;
        Rectangle spriteRectangle;
        Vector2 position;
        int Yspeed;
        int Xspeed;
        Random random;

      

        // Width and Heigh of sprite in texture
        const int BULLETWIDTH = 12;
        const int BULLETHEIGHT = 12;
        #endregion

        /// <summary>
        /// An object which the player must avoid to survive
        /// </summary>
        /// <param name="game"></param>
        /// <param name="theTexture"></param>
        public Bullet(Game game, ref Texture2D theTexture)
            : base(game)
        {
            texture = theTexture;            
            position = new Vector2();

            // Create the source rectangle.
            // This represents where is the sprite picture in surface
            spriteRectangle = new Rectangle(20, 16, BULLETWIDTH, BULLETHEIGHT);

            // Initialize the random number generator and put the meteor in 
            // your start position
            //random = new Random(this.GetHashCode());
            random = new Random(this.GetHashCode());
            PutinStartPosition();
        }

        /// <summary>
        /// Starts the bulets in random positions at the top of the screen
        /// </summary>
        protected void PutinStartPosition( )
        {
            position.X = random.Next(Game.Window.ClientBounds.Width - BULLETWIDTH);
            position.Y = 0;
            Yspeed = 1 + random.Next(9);
            Xspeed = random.Next(3) - 1;
        }


        /// <summary>
        /// Draws the Bullets
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            // Get the current spritebatch
            SpriteBatch sBatch =
                (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));

            // Draw the meteor
            sBatch.Draw(texture, position, spriteRectangle, Color.White);

            base.Draw(gameTime);
        }

        /// <summary>
        /// Updates the position of the bullet.
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Update(GameTime gameTime)
        {
            
            if ((position.Y >= Game.Window.ClientBounds.Height) ||
                (position.X >= Game.Window.ClientBounds.Width) || (position.X <= 0))
            {
                PutinStartPosition();
            }

            // Move meteor
            position.Y += Yspeed;
            position.X += Xspeed;

            base.Update(gameTime);
        }

        /// <summary>
        /// Checks for collision between the bullets
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public bool CheckCollision(Rectangle rect)
        {
            Rectangle spriterect = new Rectangle((int)position.X, (int)position.Y,
                BULLETWIDTH, BULLETHEIGHT);
            return spriterect.Intersects(rect);
        }
    }
}
