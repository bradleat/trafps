#region License
//=============================================================================
// System  : Debug
// File    : FrameRateCounter.cs
// Author  : Evan
// Note    : Copyright 2008, Portal Games, All Rights Reserved
// Compiler: Microsoft C#
//
// This file contains the FPS Counter.
//
// This code is published under the Microsoft Reciprocal License (Ms-RL). A 
// copy of the license should be distributed with the code. It can also be found
// at the project website: http://www.CodePlex.com/trafps. This notice, the
// author's name, and all copyright notices must remain intact in all
// applications, documentation, and source files.
//
// 
// Todos:   Fix and update the config.ini file to "FPSCounterOn = Yes"
//
// ============================================================================ 
#endregion
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

namespace EGGEngine.Debug
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class FrameRateCounter : DrawableGameComponent
    {
        ContentManager content;
        SpriteBatch spriteBatch;
        SpriteFont spriteFont;
        

        int frameRate = 0;
        int frameCounter = 0;
        TimeSpan elapsedTime = TimeSpan.Zero;


        public FrameRateCounter(Game game)
            : base(game)
        {
            content = new ContentManager(game.Services);
        }

        /// <summary>
        /// Loads the SpriteBatch and font.
        /// </summary>
        protected override void LoadContent()
        {  
                spriteBatch = new SpriteBatch(GraphicsDevice);
                spriteFont = content.Load<SpriteFont>("Content\\Debug\\Font");
        }

        /// <summary>
        /// Updates the frame counter
        /// </summary>
        /// <param name="gameTime">The current game time</param>
        public override void Update(GameTime gameTime)
        {
            elapsedTime += gameTime.ElapsedGameTime;

            if (elapsedTime > TimeSpan.FromSeconds(1))
            {
                elapsedTime -= TimeSpan.FromSeconds(1);
                frameRate = frameCounter;
                frameCounter = 0;
            }
        }

        /// <summary>
        /// Draws the frame counter to the screen
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            frameCounter++;

            string fps = string.Format("fps: {0}", frameRate);

            spriteBatch.Begin();

            spriteBatch.DrawString(spriteFont, fps, new Vector2(33, 33), Color.Black);
            spriteBatch.DrawString(spriteFont, fps, new Vector2(32, 32), Color.White);

            spriteBatch.End();
        }
    }
}