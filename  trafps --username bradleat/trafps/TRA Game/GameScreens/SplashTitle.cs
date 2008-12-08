#region License
//=============================================================================
// System  : SplashTitle
// File    : SplashTitle.cs
// Author  : Wenguang LIU
// Note    : Copyright 2008, Portal Games, All Rights Reserved
// Compiler: Microsoft C#
//
// This file contains the Game Loop.
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

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using EGGEngine.Rendering;
#endregion




namespace TRA_Game
{
    class SplashTitle : DrawableGameComponent
    {
        //all for test

        #region Variables
        Texture2D spTex;
        UIRenderer spRend;
        ContentManager content;
        SpriteBatch SpriteRenderer;
        TimeSpan waitCounter;
        #endregion

        #region Constructor and LoadContent
        /// <summary>
        /// Create user interface renderer
        /// </summary>
        /// 
        public SplashTitle(Game game)
            : base(game)
        {
            content = new ContentManager(game.Services);
            SpriteRenderer = new SpriteBatch(game.GraphicsDevice);
        }
        /// <summary>
        /// Loads the SpriteBatch and font.
        /// </summary>
        protected override void LoadContent()
        {


            spTex = content.Load<Texture2D>("Content\\pgi");
            spRend = new UIRenderer(spTex, SpriteRenderer);
            spRend.SetAlpha(0.0f);
            spRend.AddFadeEffect(1.0f, 800);


        }
        #endregion

        #region Update and Draw

        public override void Update(GameTime gameTime)
        {



            if (spRend.isFading() == false)
            {
                if (spRend.GetAlpha() == 1.0f)
                {
                    if (waitCounter.TotalMilliseconds > 1500)
                    {
                        spRend.AddFadeEffect(0.0f, 800);
                    }
                    else
                    {
                        waitCounter += gameTime.ElapsedGameTime;
                    }
                }
                else
                {
                    spRend.Dispose();
                    this.Dispose();
                }
            }

            spRend.Update(gameTime);

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteRenderer.Begin();

            spRend.Draw();
            SpriteRenderer.End();
            base.Draw(gameTime);
        }
        #endregion

    }
}

