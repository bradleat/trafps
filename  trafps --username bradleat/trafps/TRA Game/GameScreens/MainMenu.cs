﻿#region License
//=============================================================================
// System  : MainMenu
// File    : MainMenu.cs
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
using EGGEngine.Helpers;
#endregion

namespace TRA_Game
{
    class MainMenu : DrawableGameComponent
    {
        //all for test

        #region Variables
        Texture2D menu;
        Texture2D chart;
        UIRenderer title;
        UIRenderer chara;
        ContentManager content;
        SpriteBatch SpriteRenderer;
        InputHelper input;
        #endregion

        #region Constructor and LoadContent
        /// <summary>
        /// Create user interface renderer
        /// </summary>
        /// 
        public MainMenu(Game game)
            : base(game)
        {
            input = new InputHelper();
            content = new ContentManager(game.Services);
            SpriteRenderer = new SpriteBatch(game.GraphicsDevice);
        }
        /// <summary>
        /// Loads the SpriteBatch and font.
        /// </summary>
        protected override void LoadContent()
        {


            menu = content.Load<Texture2D>("Content\\menu");
            chart = content.Load<Texture2D>("Content\\chars");
            title = new UIRenderer(menu, SpriteRenderer);
            chara = new UIRenderer(chart, SpriteRenderer);
            chara.AddFadeEffect(0.0f, 1500);
            chara.SetPosition(new Vector2(430, 460));
        }
        #endregion



        #region Update and Draw

        public override void Update(GameTime gameTime)
        {



            if (chara.isFading() != true)
            {
                if (chara.GetAlpha() == 0.0f)
                {
                    chara.AddFadeEffect(1.0f, 1500);
                }
                else
                {
                    chara.AddFadeEffect(0.0f, 1500);
                }
            }

            if (input.KeyDown(Keys.Enter))
            {
                title.Dispose();
                this.Dispose();
            }

            title.Update(gameTime);
            chara.Update(gameTime);
            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            SpriteRenderer.Begin();

            title.Draw();
            chara.Draw();
            SpriteRenderer.End();
            base.Draw(gameTime);
        }
        #endregion

    }
}

