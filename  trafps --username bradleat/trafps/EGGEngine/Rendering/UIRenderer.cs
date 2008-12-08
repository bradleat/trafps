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
        private Texture2D UITexture;
        private Rectangle UIRectangle;
        private Vector2 UIPosition;
        private float UIAlpha = 1.0f, UITargetAlpha, UIFadeAlpha, UIRotation;
        #endregion

        #region Get / Set
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

        public void SetRotation(float Rotation)
        {
            this.UIRotation = Rotation;
        }

        public Vector2 GetPosition()
        {
            return UIPosition;
        }

        public Rectangle GetRectangle()
        {
            return UIRectangle;
        }

        public float GetAlpha()
        {
            return this.UIAlpha;
        }

        public float GetRotation()
        {
            return this.UIRotation;
        }

        public Texture2D GetTexture()
        {
            return UITexture;
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Create user interface renderer
        /// </summary>
        /// 

        public UIRenderer(Texture2D Texture, SpriteBatch spriteBatch)
        {
            UITexture = Texture;
            UISpriteBatch = spriteBatch;
            UIRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);

        }


        public UIRenderer(Texture2D Texture, Rectangle Rect, SpriteBatch spriteBatch)
        {
            UITexture = Texture;
            UIRectangle = Rect;
            UISpriteBatch = spriteBatch;
        }


        public UIRenderer(Texture2D Texture, Rectangle Rect, Vector2 Position, float Rotation, float Alpha, SpriteBatch spriteBatch)
        {
            UITexture = Texture;
            UIRectangle = Rect;
            UIPosition = Position;
            UISpriteBatch = spriteBatch;
            UIRotation = Rotation;
            UIAlpha = Alpha;
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

                UISpriteBatch.Draw(UITexture, UIPosition, UIRectangle, new Color(Color.White, UIAlpha), UIRotation,
                    new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.0f);

            }
        }

        public void Draw(Vector2 Position)
        {
            if (UITexture != null && !UITexture.IsDisposed && !UIRectangle.IsEmpty)
            {
                UISpriteBatch.Draw(UITexture, Position, UIRectangle, new Color(Color.White, UIAlpha), UIRotation,
                    new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.0f);
            }
        }

        public void Draw(Vector2 Position, float Rotation)
        {
            if (UITexture != null && !UITexture.IsDisposed && !UIRectangle.IsEmpty)
            {
                UISpriteBatch.Draw(UITexture, Position, UIRectangle, new Color(Color.White, UIAlpha), Rotation,
                     new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.0f);
            }
        }

        public void Draw(Vector2 Position, float Rotation, float Alpha)
        {
            if (UITexture != null && !UITexture.IsDisposed && !UIRectangle.IsEmpty)
            {
                UISpriteBatch.Draw(UITexture, Position, UIRectangle, new Color(Color.White, Alpha), Rotation,
                     new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.0f);
            }
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