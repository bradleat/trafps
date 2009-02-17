using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;


namespace TRA_Game
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class HUD : DrawableGameComponent
    {
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
        GraphicsDevice graphics;
        Game game;

        public struct message
        {
            public string title;
            public string text;
            public Vector2 position;
            public SpriteFont font;
            public Color color;
        }
        public List<message> messageList = new List<message>();

        public HUD(Game game, Player player, int bulletAmount, int maxBullets, ContentManager content, SpriteBatch spriteBatch)
            :base(game)
        {
            this.game = game;
            this.player = player;
            this.bulletAmount = bulletAmount;
            this.maxBullets = maxBullets;
            reticule = content.Load<Texture2D>("Untitled");
            healthBar = content.Load<Texture2D>("HealthBar");
            this.viewportCentre = new Vector2(game.Window.ClientBounds.Width / 2, game.Window.ClientBounds.Height / 2);
            spriteOrigin = new Vector2(reticule.Width / 2, reticule.Height / 2);
            messageList = new List<message>();
            this.spriteBatch = spriteBatch;
        }

        public void UpdateBulletAmount(int bulletAmount)
        {
            this.bulletAmount = bulletAmount;
        }
        
        public override void Draw(GameTime gameTime)
        {
            // Saves and return the current spritebatch
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);
            spriteBatch.Draw(reticule, viewportCentre, null, Color.White, 0, spriteOrigin, 0.1f, SpriteEffects.None, 0);
           

            //Draw each message in our array
            foreach (message currentMessage in messageList)
                spriteBatch.DrawString(currentMessage.font, currentMessage.text, currentMessage.position, currentMessage.color); 
            

            //Draw Negative health
            spriteBatch.Draw(healthBar, new Rectangle(800,527, healthBar.Width / 2, 24), new Rectangle(0, 45, healthBar.Width, 44), Color.Gray);
            //Draw the current health level based on the current Health
            spriteBatch.Draw(healthBar, new Rectangle(800, 527, (int)(healthBar.Width * ((double)player.Health / 100)) / 2, 24), new Rectangle(0, 45, healthBar.Width, 44), Color.Red);
            //Draw box around healthbar
            spriteBatch.Draw(healthBar, new Rectangle(800, 527, healthBar.Width/ 2, 24), new Rectangle(0, 0, healthBar.Width, 44), Color.White);

            //spriteBatch.DrawString(font, "Life : " + player.Life, lifePos, Color.White);
            //spriteBatch.DrawString(font, "Pistol :" + bulletAmount + "/ " + maxBullets, pistolPos, Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}