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
    public class VersionDrawer : Microsoft.Xna.Framework.DrawableGameComponent
    {
        float versionNumber;
        SpriteFont font;
        SpriteBatch spriteBatch;
        Vector2 position;

        public VersionDrawer(Game game, float versionNumber)
            : base(game)
        {
            this.versionNumber = versionNumber;
            this.font = game.Content.Load<SpriteFont>("Debug//console");
            this.spriteBatch = new SpriteBatch(game.GraphicsDevice);
            this.position = new Vector2(1000, 10);
        }

        public override void Draw(GameTime gameTime)
        {
            spriteBatch.Begin();
            spriteBatch.DrawString(font,"v" + versionNumber.ToString(), position, Color.White);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}