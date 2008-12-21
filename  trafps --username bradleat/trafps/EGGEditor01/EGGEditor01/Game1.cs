#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using EasyConfig;
using EGGEngine.Cameras;
using EGGEngine.Debug;
using EGGEngine.Rendering;
using EGGEngine.Helpers;
using EGGEngine.Utils;
#endregion

namespace EGGEditor01
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        public struct LevelData
        {
            public Vector2 position;
            int tilenumber;
            public Vector3 position2;
        }

        public List<DrawableModel> models = new List<DrawableModel>();
        
        public LevelData levelData;
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        
        private IntPtr drawSurface;

        //Debugging Stuff
        bool FPS_Counter_On;

        Vector3 initalPos1 = new Vector3(0, 15, 0);

        //Config File Stuff
        ConfigFile config = new ConfigFile("content\\config.ini");


        DrawableModel person1;
        Model terrain;
        FPSCamera camera;
        PostProcessing postProc;
        Vector3 translate = Vector3.Zero;
        InputHelper input;
        Sky sky;

        public Game1(EGGEditor form)
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            input = new InputHelper();
            Components.Add(new GamerServicesComponent(this));
            this.drawSurface = form.getDrawSurface();
            graphics.PreparingDeviceSettings += new EventHandler<PreparingDeviceSettingsEventArgs>(graphics_PreparingDeviceSettings);
            System.Windows.Forms.Control.FromHandle((this.Window.Handle)).VisibleChanged += new EventHandler(Game1_VisibleChanged);
        }

        public string GetName()
        {
            string name = config.SettingGroups["Filenames"].Settings["person"].GetValueAsString();
            return name;        
        }


        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            camera = new FPSCamera(GraphicsDevice.Viewport);
            FPS_Counter_On = config.SettingGroups["DebugFeatures"].Settings["FPSCounterOn"].GetValueAsBool();

            string name = config.SettingGroups["Filenames"].Settings["person"].GetValueAsString();
            person1 = new DrawableModel(Content.Load<Model>(name), Matrix.Identity);

            name = config.SettingGroups["Filenames"].Settings["terrain"].GetValueAsString();
            terrain = new Model();
            terrain = Content.Load<Model>(name);

            sky = Content.Load<Sky>("Models\\sky1");
            // Comment this to remove the framerate counter
            if (FPS_Counter_On == true)
            {
                Components.Add(new FrameRateCounter(this));
            }

            base.Initialize();
        }

        /// <summary>
        /// Event capturing the construction of a draw surface and makes sure this gets redirected to
        /// a predesignated drawsurface marked by pointer drawSurface
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void graphics_PreparingDeviceSettings(object sender, PreparingDeviceSettingsEventArgs e)
        {
            e.GraphicsDeviceInformation.PresentationParameters.DeviceWindowHandle = drawSurface;
        }

        /// <summary>
        /// Occurs when the original gamewindows' visibility changes and makes sure it stays invisible
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Game1_VisibleChanged(object sender, EventArgs e)
        {
            if (System.Windows.Forms.Control.FromHandle((this.Window.Handle)).Visible == true)
                System.Windows.Forms.Control.FromHandle((this.Window.Handle)).Visible = false;
        }


        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);

            //Demo stuff
            postProc = new PostProcessing(GraphicsDevice);
            postProc.LoadEffect(Content, "Effects\\PostProcessing");
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (input.ButtonDown(Buttons.B))
                this.Exit();

            if (input.KeyDown(Keys.Escape))
            {
                this.Exit();
            }
            MouseState mouseState = Mouse.GetState();

            float elapsedSeconds = (float)gameTime.ElapsedGameTime.Milliseconds / 1000.0f;
            float forwardReq = 0;

            Vector3 moveDirection = new Vector3(0, 0, 0);

            if (input.KeyDown(Keys.S))
            {
                forwardReq += 5.0f;
                moveDirection = new Vector3(0, 0, 1);  //Backward
            }
            if (input.KeyDown(Keys.W))
            {
                forwardReq += 5.0f;
                moveDirection = new Vector3(0, 0, -1);  //Forward
            }
            if (input.KeyDown(Keys.A))
            {
                forwardReq += 5.0f;
                moveDirection = new Vector3(-1, 0, 0);  //Left
            }
            if (input.KeyDown(Keys.D))
            {
                forwardReq += 5.0f;
                moveDirection = new Vector3(1, 0, 0);   //Right
            }

            camera.AddToCameraPosition(moveDirection, forwardReq, ref initalPos1, gameTime);
            person1.Position = initalPos1;

            for (int i = 0; i < models.Count; i++)
            {
                models[i].WorldMatrix = Matrix.CreateScale(2.0f);
            }

            camera.Update(mouseState, person1.Position);

            person1.WorldMatrix = Matrix.CreateScale(2.0f) * Matrix.CreateRotationY(4.05f);

            levelData.position2 = person1.Position;
            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer,
                Color.CornflowerBlue, 1, 0);

            GraphicsDevice.RenderState.CullMode = CullMode.None;
            GraphicsDevice.RenderState.DepthBufferEnable = true;
            GraphicsDevice.RenderState.AlphaBlendEnable = false;
            GraphicsDevice.RenderState.AlphaTestEnable = false;

            person1.Model.Bones[0].Transform = person1.OriginalTransforms[0] * Matrix.CreateRotationX(camera.UpDownRot)
                * Matrix.CreateRotationY(camera.LeftRightRot);
            person1.Draw(camera);
           
            for (int i = 0; i < models.Count; i++)
            {
                models[i].Draw(camera);
            }

            foreach (ModelMesh mesh in terrain.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                    effect.EnableDefaultLighting();
                    effect.World = Matrix.Identity;
                    effect.SpecularColor = new Vector3(1, 0, 0);
                    effect.View = camera.ViewMatrix;
                    effect.Projection = camera.ProjectionMatrix;
                }

                mesh.Draw();
            }

            sky.Draw(camera.ViewMatrix, camera.ProjectionMatrix);
            //Demo Stuff
            if (input.KeyDown(Keys.I))
                postProc.PostProcess("Invert");
            if (input.KeyDown(Keys.T))
                postProc.PostProcess("TimeChange",
                    (float)gameTime.TotalGameTime.TotalMilliseconds / 1000.0f);

            base.Draw(gameTime);
        }
    }
}
