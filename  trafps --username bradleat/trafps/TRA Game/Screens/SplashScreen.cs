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
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using EGGEngine.Audio;
#endregion

namespace TRA_Game
{
    /// <summary>
    /// The background screen sits behind all the other menu screens.
    /// It draws a background image that remains fixed in place regardless
    /// of whatever transitions the screens on top of it may be doing.
    /// </summary>
    class SplashScreen : GameScreen
    {
        #region Fields

        ContentManager content;
        Texture2D texture;
        bool isAudio;
        float waitCounter;
        AudioManager audioManager;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public SplashScreen()
        {

            //this.isAudio = isAudio;
            TransitionOnTime = TimeSpan.FromSeconds(0.6);
            TransitionOffTime = TimeSpan.FromSeconds(0.6);
            waitCounter = 0;
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

            this.audioManager = (AudioManager)this.ScreenManager.Game.Services.GetService(typeof(AudioManager));


            texture = content.Load<Texture2D>("splash");
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
            waitCounter += gameTime.ElapsedGameTime.Milliseconds;
            
            if (waitCounter > 2000)
            {
                LoadingScreen.Load(ScreenManager, true, new BackgroundScreen(NetworkSessionComponent.Level.shipMap), new MainMenuScreen());
            }
        }


        /// <summary>
        /// Draws the background screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Rectangle fullscreen = new Rectangle(0, 0, viewport.Width, viewport.Height);
            byte fade = TransitionAlpha;

            spriteBatch.Begin(SpriteBlendMode.None);

            spriteBatch.Draw(texture, fullscreen,
                             new Color(fade, fade, fade));

            spriteBatch.End();


        #endregion
        }
    }
}
