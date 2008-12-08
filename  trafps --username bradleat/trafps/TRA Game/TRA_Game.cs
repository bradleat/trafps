#region License
//=============================================================================
// System  : Game Loop
// File    : TRA_Game.cs
// Author  : Dustin, Bradley Leaterwood, Wenguang LIU
// Note    : Copyright 2008, Portal Games, All Rights Reserved
// Compiler: Microsoft C#
//
// This file contains the Game Loop.
//
// This code is published under the Microsoft Reciprocal License (Ms-RL). A 
// copy of the license should be distributed with the code. It can also be found
// at the project website: http://www.CodePlex.com/trafps. This notice, the
// author's name, and all copyright notices must remain intact in all
// applications, documentation, and source files.
//
// 
// Todos: 
//
// ============================================================================ 
#endregion

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

namespace TRA_Game
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class TRA_Game : Microsoft.Xna.Framework.Game
    {
        enum GameMode
        {
            _SPLASH,
            _MENU,
            _PLAY

        };

        GameMode gameMode;

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

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

        SplashTitle splashTitle;
        MainMenu mainMenu;

        //Stuff for networking
        DrawableModel person2;
        Vector3 initialPos2 = new Vector3(0, 15, -15);

        public TRA_Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            input = new InputHelper();
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;




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

            person2 = new DrawableModel(Content.Load<Model>(name), Matrix.Identity);

            name = config.SettingGroups["Filenames"].Settings["terrain"].GetValueAsString();
            terrain = new Model();
            terrain = Content.Load<Model>(name);

            sky = Content.Load<Sky>("Models\\sky1");

            splashTitle = new SplashTitle(this);
            Components.Add(splashTitle);
            gameMode = GameMode._SPLASH;

            // Comment this to remove the framerate counter
            if (FPS_Counter_On == true)
            {
                Components.Add(new FrameRateCounter(this));
            }

            base.Initialize();
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
            if (gameMode == GameMode._SPLASH && Components.Contains(splashTitle) == false)
            {
                gameMode = GameMode._MENU;
                mainMenu = new MainMenu(this);
                Components.Add(mainMenu);
            }

            if (gameMode == GameMode._MENU && Components.Contains(mainMenu) == false)
            {
                gameMode = GameMode._PLAY;

            }

            if (gameMode == GameMode._PLAY)
            {
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

                person2.Position = initialPos2;
                person2.WorldMatrix = Matrix.CreateScale(2.0f);


                camera.Update(mouseState, person1.Position);

                person1.WorldMatrix = Matrix.CreateScale(2.0f) * Matrix.CreateRotationY(4.05f);

            }

            // Allows the game to exit
            if (input.ButtonDown(Buttons.B))
                this.Exit();

            if (input.KeyDown(Keys.Escape))
            {
                this.Exit();
            }


            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {

            graphics.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer,
                Color.White, 1, 0);

            GraphicsDevice.RenderState.CullMode = CullMode.None;
            GraphicsDevice.RenderState.DepthBufferEnable = true;
            GraphicsDevice.RenderState.AlphaBlendEnable = false;
            GraphicsDevice.RenderState.AlphaTestEnable = false;

            if (gameMode == GameMode._PLAY)
            {
                person1.Model.Bones[0].Transform = person1.OriginalTransforms[0] * Matrix.CreateRotationX(camera.UpDownRot)
                * Matrix.CreateRotationY(camera.LeftRightRot);
                person1.Draw(camera);
                person2.Draw(camera);

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

            }


            base.Draw(gameTime);
        }
    }
}
