#region License
//=============================================================================
// System  : UIRenderer
// File    : UIRenderer.cs
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
#endregion

namespace EGGEngine.Rendering
{
    public class UIRenderer
    {
        #region Constants
        static public SpriteBatch UISpriteBatch;

        #endregion

        #region Variable
        Texture2D UITexture;
        Rectangle UIRectangle;
        Vector2 UIPosition;
        float UIAlpha = 1.0f, UITargetAlpha, UIFadeAlpha;
        #endregion

        #region Constructor
        /// <summary>
        /// Create user interface renderer
        /// </summary>
        /// 

        public UIRenderer(Texture2D tex, SpriteBatch spriteBatch)
        {
            UITexture = tex;
            UISpriteBatch = spriteBatch;
            UIRectangle = new Rectangle(0, 0, tex.Width, tex.Height);

        }

        public UIRenderer(Texture2D tex, Vector2 pos, SpriteBatch spriteBatch)
        {
            UITexture = tex;
            UIPosition = pos;
            UIRectangle = new Rectangle(0, 0, tex.Width, tex.Height);
            UISpriteBatch = spriteBatch;

        }

        public UIRenderer(Texture2D tex, Rectangle rec, Vector2 pos, SpriteBatch spriteBatch)
        {
            UITexture = tex;
            UIRectangle = rec;
            UIPosition = pos;
            UISpriteBatch = spriteBatch;
        }

        public UIRenderer(Texture2D tex, Rectangle rec, SpriteBatch spriteBatch)
        {
            UITexture = tex;
            UIRectangle = rec;
            UISpriteBatch = spriteBatch;
        }


        #endregion


        #region Dispose
        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        /// <param name="disposing">Disposing</param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (UITexture != null)
                {
                    UITexture.Dispose();

                }
            }
        }
        #endregion



        #region Update
        /// <summary>
        /// Updates
        /// </summary>
        /// <param name="gameTime">The current game time</param>
        public void Update(GameTime gameTime)
        {
            if (UIFadeAlpha > 0)
            {
                UIAlpha += UIFadeAlpha * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (UIAlpha >= UITargetAlpha)
                {
                    UIFadeAlpha = 0;
                    UIAlpha = UITargetAlpha;
                }
            }
            else if (UIFadeAlpha < 0)
            {
                UIAlpha += UIFadeAlpha * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (UIAlpha <= UITargetAlpha)
                {
                    UIFadeAlpha = 0;
                    UIAlpha = UITargetAlpha;
                }
            }
        }
        #endregion

        #region Draw
        /// <summary>
        /// Draws the frame counter to the screen
        /// </summary>
        public void Draw()
        {
            if (UITexture != null && !UITexture.IsDisposed && !UIRectangle.IsEmpty)
            {

                UISpriteBatch.Draw(UITexture, UIPosition, UIRectangle, new Color(Color.White, UIAlpha));

            }
        }

        public void Draw(Vector2 Position)
        {
            if (UITexture != null && !UITexture.IsDisposed && !UIRectangle.IsEmpty)
            {
                UISpriteBatch.Draw(UITexture, Position, UIRectangle, new Color(Color.White, UIAlpha));
            }
        }

        public void Draw(Vector2 Position, float Rotation)
        {
            if (UITexture != null && !UITexture.IsDisposed && !UIRectangle.IsEmpty)
            {
                UISpriteBatch.Draw(UITexture, Position, UIRectangle, new Color(Color.White, UIAlpha), Rotation
                    , new Vector2(0, 0), 0.0f, SpriteEffects.None, 0.0f);
            }
        }
        #endregion


        #region Position, Rectangle, etc.
        public void SetPosition(Vector2 vec)
        {
            UIPosition = vec;
        }

        public void SetRectangle(Rectangle rec)
        {
            UIRectangle = rec;
        }

        public void SetAlpha(float Alpha)
        {
            this.UIAlpha = Alpha;
        }

        #endregion

        #region Fade Effect


        public void AddFadeEffect(float TargetAlpha, int timeMilliseconds)
        {
            if (TargetAlpha == UIAlpha)
                return;
            this.UITargetAlpha = TargetAlpha;
            UIFadeAlpha = (TargetAlpha - UIAlpha) / timeMilliseconds;
        }
        public bool isFading()
        {
            return (UIFadeAlpha != 0);
        }

        #endregion


    }
}