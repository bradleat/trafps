#region File Description
//-----------------------------------------------------------------------------
// BackgroundScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using EasyConfig;
using EGGEngine;
using EGGEngine.Cameras;
using EGGEngine.Debug;
using EGGEngine.Rendering;
using EGGEngine.Rendering.Shaders;
using EGGEngine.Helpers;
using EGGEngine.Utils;
using EGGEngine.Audio;
using EGGEngine.Awards;
using EGGEngine.Physics;
#endregion

namespace TRA_Game
{
    /// <summary>
    /// The background screen sits behind all the other menu screens.
    /// It draws a background image that remains fixed in place regardless
    /// of whatever transitions the screens on top of it may be doing.
    /// </summary>
    class BackgroundScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        NetworkSessionComponent.Level currentLevel;
        EGGEngine.Rendering.Shaders.PostProcessing postProc;
        World world;
        GameLevel level;
        Texture2D texture;
        Model ship_Map;
        Sky sky;
        Matrix[] boneTransforms;
        FirstPersonCamera camera;

        Vector3 initialPos2 = new Vector3(0, 15, -15);
        Vector3 pistolOffset = new Vector3(1, 1, -10);
        Vector3 initalPos1 = new Vector3(0, 15, -2);
        Vector3 translate = Vector3.Zero;
        Vector3 modelPosition = new Vector3(0, -3, -5);
        Vector3 levelPos = new Vector3(0, 0, 0);
        Vector3 avatarOffset = new Vector3(0, 3, 0);

        bool isMultiplayer = false;

        #endregion


   

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public BackgroundScreen(bool isMultiplayer, NetworkSessionComponent.Level currentLevel)
        {
            //this.isMultiplayer = isMultiplayer;
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            this.currentLevel = currentLevel;
        }


        /// <summary>
        /// Loads graphics content for this screen. The background texture is quite
        /// big, so we use our own local ContentManager to load it. This allows us
        /// to unload before going from the menus into the game itself, wheras if we
        /// used the shared ContentManager provided by the Game class, the content
        /// would remain loaded forever.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            camera = new FirstPersonCamera(ScreenManager.GraphicsDevice.Viewport);
            level = LevelCreator.CreateLevel(ScreenManager.Game, currentLevel);
   
            texture = content.Load<Texture2D>("title");
           
        }


        /// <summary>
        /// Unloads graphics content for this screen.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }


        #endregion

        #region Update and Draw


        /// <summary>
        /// Updates the background screen. Unlike most screens, this should not
        /// transition off even if it has been covered by another screen: it is
        /// supposed to be covered, after all! This overload forces the
        /// coveredByOtherScreen parameter to false in order to stop the base
        /// Update method wanting to transition off.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, false);
            MouseState current_Mouse = new MouseState();
            

            camera.Position = new Vector3(-65,18,-100);

            camera.Update(0.8f,current_Mouse);
        }


        /// <summary>
        /// Draws the background screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer,
                Color.White, 1, 0);

            ScreenManager.GraphicsDevice.RenderState.CullMode = CullMode.None;
            ScreenManager.GraphicsDevice.RenderState.DepthBufferEnable = true;
            ScreenManager.GraphicsDevice.RenderState.AlphaBlendEnable = false;
            ScreenManager.GraphicsDevice.RenderState.AlphaTestEnable = false;

            level.sky.Draw(camera.ViewMatrix, camera.ProjectionMatrix);
            level.level.Draw(camera, 1f);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle screenTarget = new Rectangle(0, 0, texture.Width, texture.Height);
            byte fade = TransitionAlpha;

            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);

            spriteBatch.Draw(texture, new Vector2(viewport.Width / 2 - texture.Width/2 , 135), screenTarget,
                             new Color(fade, fade, fade));

            spriteBatch.End();

            base.Draw(gameTime);

            // If the game is transitioning on or off, fade it out to black.
            //if (TransitionPosition > 0)
            //    ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha);

        }


        #endregion
    }
}
