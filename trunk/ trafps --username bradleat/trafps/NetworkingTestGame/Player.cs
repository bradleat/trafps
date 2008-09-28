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
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Player : Microsoft.Xna.Framework.DrawableGameComponent
    {
        protected Texture2D texture;
        protected Rectangle spriteRectangle;
        public Vector2 position;

        // Input controls can be read from keyboard, gamepad, or the network.
        //public Vector2 TankInput;
        //public Vector2 TurretInput;

      

        // Width and Heigh of sprite in texture
        public const int SHIPWIDTH = 30;
        public const int SHIPHEIGHT = 30;

        // Screen Area
        public Rectangle screenBounds;


        public Player(Game game, ref Texture2D theTexture,int gamerIndex)
            : base(game)
          
        {
            // Use the gamer index to compute a starting position, so each player
            // starts in a different place as opposed to all on top of each other.
            position.X =  game.Window.ClientBounds.Width / 4 + (gamerIndex % 5) * game.Window.ClientBounds.Width / 8;
            position.Y = game.Window.ClientBounds.Height / 4 + (gamerIndex / 5) * game.Window.ClientBounds.Height / 5;

            // TODO: Construct any child components here
            texture = theTexture;
            position = new Vector2();

            // Create the source rectangle.
            // This represents where is the sprite picture in surface
            spriteRectangle = new Rectangle(31, 83, SHIPWIDTH, SHIPHEIGHT);

            screenBounds = new Rectangle(0, 0,
                Game.Window.ClientBounds.Width,
                Game.Window.ClientBounds.Height);
        }

        public void PutinStartPosition()
        {
            position.X = screenBounds.Width / 2;
            position.Y = screenBounds.Height - SHIPHEIGHT;
          
        }

        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }


        public void Update(/*GameTime gameTime*/ KeyboardState keyboard)
        {
            // TODO: Add your update code here
            //KeyboardState keyboard = Keyboard.GetState();
            if (keyboard.IsKeyDown(Keys.Up))
            {
                position.Y -= 3;
           
            }
            if (keyboard.IsKeyDown(Keys.Down))
            {
                position.Y += 3;

             
            }
            if (keyboard.IsKeyDown(Keys.Left))
            {
                position.X -= 3;

           
            }
            if (keyboard.IsKeyDown(Keys.Right))
            {
                position.X += 3;
             
            }
            


            if (position.X < screenBounds.Left)
            {
                position.X = screenBounds.Left;
            }
            if (position.X > screenBounds.Width - SHIPWIDTH)
            {
                position.X = screenBounds.Width - SHIPWIDTH;
            }
            if (position.Y < screenBounds.Top)
            {
                position.Y = screenBounds.Top;
            }
            if (position.Y > screenBounds.Height - SHIPHEIGHT)
            {
                position.Y = screenBounds.Height - SHIPHEIGHT;
            }
            //base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            // Get the current spritebatch
            SpriteBatch sBatch =
                (SpriteBatch)Game.Services.GetService(typeof(SpriteBatch));

            // Draw the ship
            sBatch.Draw(texture, position, spriteRectangle, Color.White);

           
            base.Draw(gameTime);
        }

        public Rectangle GetBounds()
        {
            return new Rectangle((int)position.X, (int)position.Y, 
                SHIPWIDTH, SHIPHEIGHT);
        }
        
     
    }
}