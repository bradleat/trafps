#region License
//=============================================================================
// System  : Game Loop
// File    : TRA_Game.cs
// Author  : Dustin, Bradley Leaterwood
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
using EGGEngine.Helpers;
using EGGEngine.Rendering;
#endregion

namespace TRA_Game
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class TRA_Game : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        //Config File Stuff
        ConfigFile config = new ConfigFile("content\\config.ini");
        
        //Demo Stuff
        DrawableModel model;
        FPSCamera camera;
        PostProcessing postProc; 

        //Debugging Stuff
        bool FPS_Counter_On;

        InputHelper input;

        public TRA_Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            input = new InputHelper();
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

            string name = config.SettingGroups["Filenames"].Settings["model"].GetValueAsString();

            model = new DrawableModel(Content.Load<Model>(name), Matrix.Identity);
            
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
            // TODO: Unload any non ContentManager content here
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

            camera.Update(mouseState);

            model.WorldMatrix = Matrix.CreateScale(5.0f)
                * Matrix.CreateRotationY(MathHelper.ToRadians(0))
                * Matrix.CreateTranslation(new Vector3(0, 0, -50));
            model.Draw(camera.ViewMatrix, camera.ProjectionMatrix);

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
                        
            //Demo Stuff
            GraphicsDevice.RenderState.CullMode = CullMode.None;
            GraphicsDevice.RenderState.DepthBufferEnable = true; 
            GraphicsDevice.RenderState.AlphaBlendEnable = false; 
            GraphicsDevice.RenderState.AlphaTestEnable = false;

            model.Draw(camera.ViewMatrix, camera.ProjectionMatrix);

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
