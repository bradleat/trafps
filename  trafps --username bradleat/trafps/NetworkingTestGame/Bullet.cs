using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.Content;

namespace NetworkingTestGame
{

    public class Bullet : Microsoft.Xna.Framework.DrawableGameComponent
    {

        protected Texture2D texture;
        protected Rectangle spriteRectangle;
        protected Vector2 position;
        protected int Yspeed;
        protected int Xspeed;
        protected Random random;

      

        // Width and Heigh of sprite in texture
        protected const int BULLETWIDTH = 12;
        protected const int BULLETHEIGHT = 12;

         public Bullet(Game game, ref Texture2D theTexture)
            : base(game)
        {
            texture = theTexture;            
            position = new Vector2();

            // Create the source rectangle.
            // This represents where is the sprite picture in surface
            spriteRectangle = new Rectangle(20, 16, BULLETWIDTH, BULLETHEIGHT);

            // Initialize the random number generator and put the meteor in 
            // your start position
            //random = new Random(this.GetHashCode());
            random = new Random(this.GetHashCode());
            PutinStartPosition();
        }


        protected void PutinStartPosition( )
        {
            position.X = random.Next(Game.Window.ClientBounds.Width - BULLETWIDTH);
            position.Y = 0;
            Yspeed = 1 + random.Next(9);
            Xspeed = random.Next(3) - 1;
        }



        public override void Draw(GameTime gameTime)
        {
            // Get the current spritebatch
            SpriteBatch sBatch =
                (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));

            // Draw the meteor
            sBatch.Draw(texture, position, spriteRectangle, Color.White);

            base.Draw(gameTime);
        }

        public override void Update(GameTime gameTime)
        {
            
            if ((position.Y >= Game.Window.ClientBounds.Height) ||
                (position.X >= Game.Window.ClientBounds.Width) || (position.X <= 0))
            {
                PutinStartPosition();
            }

            // Move meteor
            position.Y += Yspeed;
            position.X += Xspeed;

            base.Update(gameTime);
        }

        public bool CheckCollision(Rectangle rect)
        {
            Rectangle spriterect = new Rectangle((int)position.X, (int)position.Y,
                BULLETWIDTH, BULLETHEIGHT);
            return spriterect.Intersects(rect);
        }





    }
}
