﻿using System;
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


        protected override void LoadContent()
        {  
                spriteBatch = new SpriteBatch(GraphicsDevice);
                spriteFont = content.Load<SpriteFont>("Content\\Debug\\Font");
        }


        protected override void UnloadContent()
        {
                content.Unload();
        }


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