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
using EGGEngine.Physics;
using EGGEngine.Cameras;


namespace TRA_Game
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Player : Microsoft.Xna.Framework.DrawableGameComponent
    {
        private Vector3 position;
        private float rotation;
        private float health;
        private float shield;
        private ModelTypes.PlayerType playerType;
        private Weapon primaryWeapon;
        private Weapon secondaryWeapon;
        private float maxHealth;
        private float maxShield;
        private Model model;
        private EGGEngine.Physics.Player physicsPlayer;
        private int currentAnimationId;
        private FirstPersonCamera camera;
        private Flag flag;
        private int teamID;

        #region Properties
        public Vector3 Position
        {
            get { return position; }
            set { position = value; }
        }
        public float Rotation
        {
            get { return rotation; }
            set { rotation = value; }
        }
        public float Health
        {
            get { return health; }
            set { health = value; }
        }
        public float Shield
        {
            get { return shield; }
            set { shield = value; }
        }
        public Weapon PrimaryWeapon
        {
            get { return primaryWeapon; }
            set { primaryWeapon = value; }
        }
        public Weapon SecondaryWeapon
        {
            get { return secondaryWeapon; }
            set { secondaryWeapon = value; }
        }
        public Flag Flag
        {
            get { return flag; }
            set { flag = value; }
        }
        public int TeamID
        {
            get { return teamID; }
            set { teamID = value; }
        }

        #endregion


        public Player(Game game)//, ModelTypes.PlayerType playerType, Vector3 initialPosition, float initialRotation, Vector3 boundingBoxSize, EGGEngine.Physics.World world, Weapon currentWeapon)
            : base(game)
        {
            /*
            this.playerType = playerType;
            this.position = initialPosition;
            this.rotation = initialRotation;
            this.health = ModelTypes.PlayerHealth[(int)playerType];
            this.maxHealth = this.health;
            this.shield = ModelTypes.ShieldHealth[(int)playerType];
            this.maxShield = this.shield;

            this.model = Game.Content.Load<Model>(ModelTypes.PlayerModelFileName[(int)playerType]);


            physicsPlayer = new EGGEngine.Physics.Player(this.position, this.rotation, boundingBoxSize, world);
            // TODO: Construct any child components here*/
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public void Initialize(ModelTypes.PlayerType playerType, Vector3 initialPosition, float initialRotation, Vector3 boundingBoxSize, EGGEngine.Physics.World world, Weapon currentWeapon)
        {
            this.playerType = playerType;
            this.position = initialPosition;
            this.rotation = initialRotation;
            this.health = ModelTypes.PlayerHealth[(int)playerType];
            this.maxHealth = this.health;
            this.shield = ModelTypes.ShieldHealth[(int)playerType];
            this.maxShield = this.shield;

            this.model = Game.Content.Load<Model>(ModelTypes.PlayerModelFileName[(int)playerType]);


            physicsPlayer = new EGGEngine.Physics.Player(this.position, this.rotation, boundingBoxSize, world);
            // TODO: Add your initialization code here
            base.Initialize();
        }


        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public void Update(GameTime gameTime, MouseState current_Mouse, KeyboardState KeyState)
        {
            physicsPlayer.Rotation -= current_Mouse.X * 0.01f;

            int state = 0;
            bool Jump = false;

            Vector2 moveDirection = Vector2.Zero;


            if (KeyState.IsKeyDown(Keys.W))
            {
                state = 2;
                moveDirection += new Vector2(0, 1);
            }

            if (KeyState.IsKeyDown(Keys.S))
            {
                state = 2;
                moveDirection += new Vector2(0, -1);
            }

            if (KeyState.IsKeyDown(Keys.A))
            {
                state = 2;
                moveDirection += new Vector2(1, 0);
            }

            if (KeyState.IsKeyDown(Keys.D))
            {
                state = 2;
                moveDirection += new Vector2(-1, 0);
            }

            if (KeyState.IsKeyDown(Keys.LeftShift) && state != 0)
            {
                state = 1;
            }

            if (KeyState.IsKeyDown(Keys.X) && state != 0)
            {
                state = 3;
            }

            if (KeyState.IsKeyDown(Keys.Space))
            {
                Jump = true;
            }

            physicsPlayer.Update(moveDirection, state, Jump);

            this.rotation = physicsPlayer.Rotation;
            this.position = physicsPlayer.Position;

            //primaryWeapon.Update(gameTime);

            base.Update(gameTime);
        }


        public void Draw(GameTime gameTime, FirstPersonCamera camera)
        {
            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in model.Meshes)
            {

                // This is where the mesh orientation is set, as well as our camera and projection.
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = Matrix.CreateRotationY(this.rotation) * Matrix.CreateTranslation(this.position);
                    effect.View = camera.ViewMatrix;
                    float aspectRatio = 1.0f;
                    effect.Projection = Matrix.CreatePerspectiveFieldOfView(MathHelper.ToRadians(45.0f),
                        aspectRatio, 1.0f, 10000.0f);
                }
                // Draw the mesh, using the effects set above.
                mesh.Draw();
            }
            base.Draw(gameTime);
        }
    }
}