#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Graphics;
#endregion

namespace TRA_Game
{
     class RespawnScreen : MenuScreen
    {
        NetworkSession networkSession;
        float respawnTime = 0;
        int respawnTimeLeft = 6;
        

        /// <summary>
        /// Constructor.
        /// </summary>
        public RespawnScreen(NetworkSession networkSession)
            : base(Resources.Respawn, true)
        {
            this.networkSession = networkSession;
            
            // Flag that there is no need for the game to transition
            // off when the pause menu is on top of it.
            IsPopup = true;

            // Add the Resume Game menu entry.
            MenuEntry resumeGameMenuEntry = new MenuEntry(Resources.ResumeGame);
            resumeGameMenuEntry.Selected += OnCancel;

        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {
            respawnTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (respawnTime >= 1)
            {
                respawnTime = 0;
                respawnTimeLeft -= 1;
            }
            if (respawnTimeLeft == 0)
            {
                OnCancel();
            }
            
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);
        }

        /// <summary>
        /// Draws the pause menu screen. This darkens down the gameplay screen
        /// that is underneath us, and then chains to the base MenuScreen.Draw.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;
            string respawnTimeRemaining = respawnTimeLeft.ToString();

            // Center the text in the viewport.
            Viewport viewport = ScreenManager.GraphicsDevice.Viewport;
            Vector2 viewportSize = new Vector2(viewport.Width, viewport.Height);
            Vector2 textSize = font.MeasureString(respawnTimeRemaining);
            Vector2 textPosition = (viewportSize - textSize) / 2;

            spriteBatch.Begin();
            spriteBatch.DrawString(font, respawnTimeRemaining, textPosition, Color.Yellow);
            spriteBatch.End();

            base.Draw(gameTime);
        }

        

    }
}
