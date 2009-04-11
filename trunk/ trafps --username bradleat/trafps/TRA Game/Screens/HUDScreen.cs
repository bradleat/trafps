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
    class HUDScreen : GameScreen
    {

        #region Fields
        ContentManager content;
        SpriteBatch spriteBatch;
        Player player;

        Vector2 lifePos = new Vector2(800, 527);
        Vector2 pistolPos = new Vector2(800, 554);
        Vector2 viewportCentre;
        Vector2 spriteOrigin;

        int bulletAmount;
        int maxBullets;

        Texture2D reticule;
        Texture2D healthBar;

        public struct message
        {
            public string title;
            public string text;
            public Vector2 position;
            public SpriteFont font;
            public Color color;
        }
        public List<message> messageList = new List<message>();


      

        #endregion


   

        #region Initialization


        /// <summary>
        /// Constructor.
        /// </summary>
        public HUDScreen(Player player, int bulletAmount, int maxBullets)
        {
            this.player = player;
            this.bulletAmount = bulletAmount;
            this.maxBullets = maxBullets;
            //IsPopup = true;
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
            messageList = new List<message>();
            
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

            reticule = content.Load<Texture2D>("Untitled");
            healthBar = content.Load<Texture2D>("HealthBar");
            this.viewportCentre = new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2, ScreenManager.GraphicsDevice.Viewport.Height / 2);
            spriteOrigin = new Vector2(reticule.Width / 2, reticule.Height / 2);
            
            
        }


        /// <summary>
        /// Unloads graphics content for this screen.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }

        public void UpdateBulletAmount(int bulletAmount)
        {
            this.bulletAmount = bulletAmount;
        }


        #endregion

        #region Update and Draw


        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        public override void HandleInput(InputState input)
        {
            base.HandleInput(input);
        }


        /// <summary>
        /// Draws the background screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            
            
            
            // Saves and return the current spritebatch
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);
            spriteBatch.Draw(reticule, viewportCentre, null, Color.White, 0, spriteOrigin, 0.1f, SpriteEffects.None, 0);

            //Draw each message in our array
            foreach (message currentMessage in messageList)
                spriteBatch.DrawString(currentMessage.font, currentMessage.text, currentMessage.position, currentMessage.color); 
            
            
            //Draw Negative health
            spriteBatch.Draw(healthBar, new Rectangle(800, 527, healthBar.Width / 2, 24), new Rectangle(0, 45, healthBar.Width, 44), Color.Gray);
            //Draw the current health level based on the current Health
            spriteBatch.Draw(healthBar, new Rectangle(800, 527, (int)(healthBar.Width * ((double)player.Health / 100)) / 2, 24), new Rectangle(0, 45, healthBar.Width, 44), Color.Red);
            //Draw box around healthbar
            spriteBatch.Draw(healthBar, new Rectangle(800, 527, healthBar.Width / 2, 24), new Rectangle(0, 0, healthBar.Width, 44), Color.White);

            //spriteBatch.DrawString(font, "Life : " + player.Life, lifePos, Color.White);
            //spriteBatch.DrawString(font, "Pistol :" + bulletAmount + "/ " + maxBullets, pistolPos, Color.White);
            spriteBatch.End();
            



            base.Draw(gameTime);

         
        }


        #endregion
    }
}

