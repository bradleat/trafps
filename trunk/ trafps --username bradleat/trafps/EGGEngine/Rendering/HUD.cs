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


namespace EGGEngine.Rendering
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class HUD
    {
        SpriteBatch spriteBatch;
        DrawableModel player;

        Vector2 lifePos = new Vector2(800, 527);
        Vector2 pistolPos = new Vector2(800, 554);
        Vector2 viewportCentre;
        Vector2 spriteOrigin;

        int bulletAmount;
        int maxBullets;

        Texture2D reticule;
        GraphicsDevice graphics;

        public HUD(Game game, DrawableModel player, int bulletAmount, int maxBullets, ContentManager content)
        {
            this.player = player;
            this.bulletAmount = bulletAmount;
            this.maxBullets = maxBullets;
            reticule = content.Load<Texture2D>("Untitled");
            this.viewportCentre = new Vector2(game.Window.ClientBounds.Width / 2, game.Window.ClientBounds.Height / 2);
            spriteOrigin = new Vector2(reticule.Width / 2, reticule.Height / 2);
            // TODO: Construct any child components here
        }

        public void UpdateBulletAmount(int bulletAmount)
        {
            this.bulletAmount = bulletAmount;
        }


        public void Draw(SpriteBatch spriteBatch, SpriteFont font)
        {
            // Saves and return the current spritebatch
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend, SpriteSortMode.Deferred, SaveStateMode.SaveState);
            spriteBatch.Draw(reticule, viewportCentre, null, Color.White, 0, spriteOrigin, 0.5f, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, "Life : " + player.Life, lifePos, Color.White);
            spriteBatch.DrawString(font, "Pistol :" + bulletAmount + "/ " + maxBullets, pistolPos, Color.White);
            spriteBatch.End();
        }
    }
}