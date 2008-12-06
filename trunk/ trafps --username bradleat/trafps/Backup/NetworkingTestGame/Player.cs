
#region (License)
//=============================================================================
// System  : Networking Game Loop
// File    : Player.cs
// Author  : Evan
// Note    : Copyright 2008, Portal Games, All Rights Reserved
// Compiler: Microsoft C#
//
// This file contains the Player.
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
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Player : Microsoft.Xna.Framework.DrawableGameComponent
    {
        #region Properties

        Texture2D texture;
        Rectangle spriteRectangle;
        Vector2 position;

        // Input controls can be read from keyboard, gamepad, or the network.
        //public Vector2 TankInput;
        //public Vector2 TurretInput;

      

        // Width and Heigh of sprite in texture
        const int SHIPWIDTH = 30;
        const int SHIPHEIGHT = 30;

        // Screen Area
        Rectangle screenBounds;
        #endregion

        /// <summary>
        /// Creates the Player (singleplayer)
        /// </summary>
        /// <param name="game"></param>
        /// <param name="theTexture"></param>
        /// <param name="gamerIndex"></param>
        public Player(Game game, ref Texture2D theTexture,int gamerIndex)
            : base(game)
          
        {
            // Use the gamer index to compute a starting position, so each player
            // starts in a different place as opposed to all on top of each other.
            position.X =  game.Window.ClientBounds.Width / 4 + (gamerIndex % 5) * game.Window.ClientBounds.Width / 8;
            position.Y = game.Window.ClientBounds.Height / 4 + (gamerIndex / 5) * game.Window.ClientBounds.Height / 5;

            // TODO: Construct any child components here
            texture = theTexture;
            position = new Vector2();

            // Create the source rectangle.
            // This represents where is the sprite picture in surface
            spriteRectangle = new Rectangle(31, 83, SHIPWIDTH, SHIPHEIGHT);

            screenBounds = new Rectangle(0, 0,
                Game.Window.ClientBounds.Width,
                Game.Window.ClientBounds.Height);
        }

        /// <summary>
        /// Starts the Player in the centre along the bottom of the screen
        /// </summary>
        public void PutinStartPosition()
        {
            position.X = screenBounds.Width / 2;
            position.Y = screenBounds.Height - SHIPHEIGHT;
          
        }

        /// <summary>
        /// Changes position using input and keeps within the screen.
        /// </summary>
        /// <param name="keyboard"></param>
        public void Update(/*GameTime gameTime*/ KeyboardState keyboard)
        {
            // TODO: Add your update code here
            //KeyboardState keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.Up))
            {
                position.Y -= 3;
           
            }
            if (keyboard.IsKeyDown(Keys.Down))
            {
                position.Y += 3;

             
            }
            if (keyboard.IsKeyDown(Keys.Left))
            {
                position.X -= 3;

           
            }
            if (keyboard.IsKeyDown(Keys.Right))
            {
                position.X += 3;
             
            }
            


            if (position.X < screenBounds.Left)
            {
                position.X = screenBounds.Left;
            }
            if (position.X > screenBounds.Width - SHIPWIDTH)
            {
                position.X = screenBounds.Width - SHIPWIDTH;
            }
            if (position.Y < screenBounds.Top)
            {
                position.Y = screenBounds.Top;
            }
            if (position.Y > screenBounds.Height - SHIPHEIGHT)
            {
                position.Y = screenBounds.Height - SHIPHEIGHT;
            }
            //base.Update(gameTime);
        }

        /// <summary>
        /// Draws the Ship
        /// </summary>
        /// <param name="gameTime"></param>
        public override void Draw(GameTime gameTime)
        {
            // Get the current spritebatch
            SpriteBatch sBatch =
                (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));

            // Draw the ship
            sBatch.Draw(texture, position, spriteRectangle, Color.White);

           
            base.Draw(gameTime);
        }

        /// <summary>
        /// Gets the screen bounds
        /// </summary>
        /// <returns></returns>
        public Rectangle GetBounds()
        {
            return new Rectangle((int)position.X, (int)position.Y, 
                SHIPWIDTH, SHIPHEIGHT);
        }
        
     
    }
}