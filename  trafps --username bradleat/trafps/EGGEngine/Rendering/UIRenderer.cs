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
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
#endregion

namespace EGGEngine.Rendering
{
    public class UIRenderer 
    {
        #region Variable
        static public SpriteBatch UISpriteBatch = null;
        static public ContentManager UIContent = null;
        static public Game UIGame;


        private Texture2D UITexture;
        private Rectangle UIRectangle;
        private Vector2 UIPosition;
        private float UIAlpha = 1.0f, UITargetAlpha, UIFadeAlpha, UIRotation;
        static private bool UIReady = false;


        private bool isBloom = false;
        private float UIBPara = 0.005f, UITargetBloom, UIFadeBloom;
        static private Effect UIBloom = null;

        private bool UIHiRange = false;

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
      

        public UIRenderer(Texture2D Texture, Game game)
        {
            UIGame = game;
            if (UISpriteBatch == null) UISpriteBatch = new SpriteBatch(game.GraphicsDevice);
            if(UIContent == null) UIContent = new ContentManager(game.Services);

            UITexture = Texture;
          
            UIRectangle = new Rectangle(0, 0, Texture.Width, Texture.Height);

           

            if (UIBloom == null) UIBloom = UIContent.Load<Effect>("Content\\fx\\bloom");

        }


        public UIRenderer(Texture2D Texture, Rectangle Rect, Game game)
        {
            UIGame = game;
            if (UISpriteBatch == null) UISpriteBatch = new SpriteBatch(game.GraphicsDevice);
            if (UIContent == null) UIContent = new ContentManager(game.Services);

            UITexture = Texture;
            UIRectangle = Rect;
           

          
            if (UIBloom == null) UIBloom = UIContent.Load<Effect>("Content\\fx\\bloom");

        }


        public UIRenderer(Texture2D Texture, Rectangle Rect, Vector2 Position, float Rotation, float Alpha, Game game)
        {
            UIGame = game;
            if (UISpriteBatch == null) UISpriteBatch = new SpriteBatch(game.GraphicsDevice);
            if (UIContent == null) UIContent = new ContentManager(game.Services);

            UITexture = Texture;
            UIRectangle = Rect;
            UIPosition = Position;
           
            UIRotation = Rotation;
            UIAlpha = Alpha;

            
            if (UIBloom == null) UIBloom = UIContent.Load<Effect>("Content\\fx\\bloom");

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

            if (UIFadeBloom > 0)
            {
                UIBPara += UIFadeBloom * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (UIBPara >= UITargetBloom)
                {
                    UIFadeBloom = 0;
                    UIBPara = UITargetBloom;
                   
                }
            }
            else if (UIFadeBloom < 0)
            {
                UIBPara += UIFadeBloom * (float)gameTime.ElapsedGameTime.TotalMilliseconds;
                if (UIBPara <= UITargetBloom)
                {
                    UIFadeBloom = 0;
                    UIBPara = UITargetBloom;
                    if (UIBPara == 0.0f)
                        StopBloom();
                }
            }
        }
        #endregion

        #region Draw & Render
        /// <summary>
        /// Draws the frame counter to the screen
        /// </summary>
        static public void PrepareRenderer()
        {
            UIGame.GraphicsDevice.SetRenderTarget(0, null);
            UIGame.GraphicsDevice.Clear(Color.White);
            UISpriteBatch.Begin();
            UIReady = true;
        }

        static public void Render()
        {

            UISpriteBatch.End();
           

            UIReady = false;
        }

        public void Draw()
        {
            if (UIReady == false)
                PrepareRenderer();


            if (UITexture != null && !UITexture.IsDisposed && !UIRectangle.IsEmpty )
            {
                if (this.isBloom == false)
                    UISpriteBatch.Draw(UITexture, UIPosition, UIRectangle, new Color(Color.White, UIAlpha), UIRotation,
                        new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.0f);
                else
                {
                    UISpriteBatch.End();
                    UIBloom.Parameters["mag"].SetValue(this.UIBPara);
                    UIBloom.Parameters["alpha"].SetValue(UIAlpha);
                    UIBloom.Parameters["hirange"].SetValue(this.UIHiRange);

                    UIBloom.Begin();
                    UISpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate,
                        SaveStateMode.SaveState);

                    EffectPass pass = UIBloom.CurrentTechnique.Passes[0];
                    pass.Begin();

                    UISpriteBatch.Draw(UITexture, UIPosition, UIRectangle, new Color(Color.White, UIAlpha), UIRotation,
                        new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.0f);

                    pass.End();
                    UISpriteBatch.End();
                    UIBloom.End();
                    UISpriteBatch.Begin();

                }

            }
        }

        public void Draw(Vector2 Position)
        {
            if (UIReady == false)
                PrepareRenderer();

            if (UITexture != null && !UITexture.IsDisposed && !UIRectangle.IsEmpty)
            {
                if (this.isBloom == false)
                    UISpriteBatch.Draw(UITexture, Position, UIRectangle, new Color(Color.White, UIAlpha), UIRotation,
                        new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.0f);
                else
                {
                    UISpriteBatch.End();
                    UIBloom.Parameters["mag"].SetValue(this.UIBPara);
                    UIBloom.Parameters["alpha"].SetValue(UIAlpha);
                    UIBloom.Parameters["hirange"].SetValue(this.UIHiRange);

                    UIBloom.Begin();
                    UISpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate,
                        SaveStateMode.SaveState);

                    EffectPass pass = UIBloom.CurrentTechnique.Passes[0];
                    pass.Begin();

                    UISpriteBatch.Draw(UITexture, Position, UIRectangle, new Color(Color.White, UIAlpha), UIRotation,
                        new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.0f);

                    pass.End();
                    UISpriteBatch.End();
                    UIBloom.End();
                    UISpriteBatch.Begin();

                }
            }
        }

        public void Draw(Vector2 Position, float Rotation)
        {
            if (UIReady == false)
                PrepareRenderer();

            if (UITexture != null && !UITexture.IsDisposed && !UIRectangle.IsEmpty)
            {
                if (this.isBloom == false)
                    UISpriteBatch.Draw(UITexture, Position, UIRectangle, new Color(Color.White, UIAlpha), Rotation,
                        new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.0f);
                else
                {
                    UISpriteBatch.End();
                    UIBloom.Parameters["mag"].SetValue(this.UIBPara);
                    UIBloom.Parameters["alpha"].SetValue(UIAlpha);
                    UIBloom.Parameters["hirange"].SetValue(this.UIHiRange);

                    UIBloom.Begin();
                    UISpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate,
                        SaveStateMode.SaveState);

                    EffectPass pass = UIBloom.CurrentTechnique.Passes[0];
                    pass.Begin();

                    UISpriteBatch.Draw(UITexture, Position, UIRectangle, new Color(Color.White, UIAlpha), Rotation,
                        new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.0f);

                    pass.End();
                    UISpriteBatch.End();
                    UIBloom.End();
                    UISpriteBatch.Begin();

                }
            }
        }

        public void Draw(Vector2 Position, float Rotation, float Alpha)
        {
            if (UIReady == false)
                PrepareRenderer();

            if (UITexture != null && !UITexture.IsDisposed && !UIRectangle.IsEmpty)
            {
                if (this.isBloom == false)
                    UISpriteBatch.Draw(UITexture, Position, UIRectangle, new Color(Color.White, Alpha), Rotation,
                        new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.0f);
                else
                {
                    UISpriteBatch.End();
                    UIBloom.Parameters["mag"].SetValue(this.UIBPara);
                    UIBloom.Parameters["alpha"].SetValue(UIAlpha);
                    UIBloom.Parameters["hirange"].SetValue(this.UIHiRange);

                    UIBloom.Begin();
                    UISpriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Immediate,
                        SaveStateMode.SaveState);

                    EffectPass pass = UIBloom.CurrentTechnique.Passes[0];
                    pass.Begin();

                    UISpriteBatch.Draw(UITexture, Position, UIRectangle, new Color(Color.White, Alpha), Rotation,
                        new Vector2(0, 0), 1.0f, SpriteEffects.None, 0.0f);

                    pass.End();
                    UISpriteBatch.End();
                    UIBloom.End();
                    UISpriteBatch.Begin();

                }
            
            }
        }

       
        #endregion


        #region Effects
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

        public void AddBloomEffect(float TargetBloom, int timeMilliseconds)
        {
            if (TargetBloom == UIBPara)
                return;
            this.UITargetBloom = TargetBloom;
            UIFadeBloom = (TargetBloom - UIBPara) / timeMilliseconds;
            StartBloom();
        }

        public void StartBloom()
        {
            this.isBloom = true;
        }

        public void SetBloomParameter(float BloomParameter)
        {
            this.UIBPara = BloomParameter;
            if (BloomParameter == 0.0f)
                StopBloom();
            else
            StartBloom();
        }

        public void StopBloom()
        {
            this.isBloom = false;
        }
        public bool isBlooming()
        {
            return (UIFadeBloom != 0);
        }

        #endregion


    }
}