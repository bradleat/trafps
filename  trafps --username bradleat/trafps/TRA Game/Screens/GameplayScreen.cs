#region File Description
//-----------------------------------------------------------------------------
// GameplayScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Threading;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Storage;
using Microsoft.Xna.Framework.GamerServices;
using EasyConfig;
using EGGEngine.Cameras;
using EGGEngine.Debug;
using EGGEngine.Rendering;
using EGGEngine.Helpers;
using EGGEngine.Utils;
using EGGEngine.Audio;
using EGGEngine.Awards;
#endregion

namespace TRA_Game
{
    #region GameVariables
    public struct GameVariables
    {
        public float bulletSpeed;
        public float bulletDamage;
        public float playerHealth;
        public float playerSpeed;
        public float gravity;
    }
    #endregion

    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {
        #region Fields
        NetworkSession networkSession;
        GameTime gameTime;
        ContentManager Content;
        SpriteFont gameFont;
        public static FrameRateCounter fpsCounter;
        ConsoleMenu console;
        
        Vector3 pistolOffset = new Vector3(1, 1, -10);

        Random random = new Random();

        Matrix[] boneTransforms;

        GraphicsDeviceManager graphics;

        //Debugging Stuff
        bool FPS_Counter_On;

        Vector3 initalPos1 = new Vector3(0, 15, -2);

        //Config File Stuff
        ConfigFile config = new ConfigFile("content\\config.ini");


        DrawableModel person1;
        DrawableModel pistol;
        Model ship_Map;
        FPSCamera camera;
        PostProcessing postProc;
        Vector3 translate = Vector3.Zero;
        InputHelper input;
        Sky sky;

        GameVariables gameVariables;

        SplashTitle splashTitle;
        MainMenu mainMenu;

        //Audio
        Audio audioHelper;
        Cue mystery;
        Cue famas_1;
        Cue famas_forearm;
        AudioEmitter emitter = new AudioEmitter();
        AudioListener listener = new AudioListener();

        //Stuff for networking
        DrawableModel person2;
        Vector3 initialPos2 = new Vector3(0, 15, -15);

        float bulletSpeed;
        double lastBulletTime = 0;
        double lastEnemyBulletTime = 0;
        float gameSpeed = 1.0f;
        int bulletAmount;
        const int maxBullets = 20;
        float bulletDamage;
        float gravity;
        int PlayerScore;
        int enemyScore;

        float forwardReq = 0;
        Vector3 moveDirection = new Vector3(0, 0, 0);
        HUD hud;
        HUD.message playerMessage = new HUD.message();
        HUD.message enemyMessage = new HUD.message();
        HUD.message bulletAmountMessage = new HUD.message();
        
        List<HUD.message> messageList;
        AwardsComponent awards;
        Award shootAward;

        
        

        /// <summary>
        /// The logic for deciding whether the game is paused depends on whether
        /// this is a networked or single player game. If we are in a network session,
        /// we should go on updating the game even when the user tabs away from us or
        /// brings up the pause menu, because even though the local player is not
        /// responding to input, other remote players may not be paused. In single
        /// player modes, however, we want everything to pause if the game loses focus.
        /// </summary>
        new bool IsActive
        {
            get
            {
                if (networkSession == null)
                {
                    // Pause behavior for single player games.
                    return base.IsActive;
                }
                else
                {
                    // Pause behavior for networked games.
                    return !IsExiting;
                }
            }
        }
        #endregion

        #region Variables Handling

        void OpenFile(string filename)
        {
        IAsyncResult result = null;
            SerializeUtils<GameVariables> gameVariables = new SerializeUtils<GameVariables>();
            if (!Guide.IsVisible)
            {
                result = Guide.BeginShowStorageDeviceSelector(PlayerIndex.One, null, null);
            }
            if (result.IsCompleted)
            {
                StorageDevice device = Guide.EndShowStorageDeviceSelector(result);
                gameVariables.LoadData(device, filename);
                LoadVariables(gameVariables);
            }
        }
        void LoadVariables(SerializeUtils<GameVariables> gameVariables)
        {
            bulletDamage = gameVariables.Data.bulletDamage;
            bulletSpeed = gameVariables.Data.bulletSpeed;
            gravity = gameVariables.Data.gravity;
            person1.Life = gameVariables.Data.playerHealth;
        }


        void SaveVariables(string filename)
        {
            {
                IAsyncResult result = null;
                SerializeUtils<GameVariables> gameVariables2 = new SerializeUtils<GameVariables>();

                gameVariables2.Data = gameVariables;
                if (!Guide.IsVisible)
                {
                    result = Guide.BeginShowStorageDeviceSelector(PlayerIndex.One, null, null);
                }
                if (result.IsCompleted)
                {
                    StorageDevice device = Guide.EndShowStorageDeviceSelector(result);
                    if (device.IsConnected)
                    {
                        gameVariables2.SaveData(device, filename);
                    }
                }
            }
        }
        #endregion

        #region Constructor, LoadContent, UnloadContent

        /// <summary>
        /// Constructor.
        /// </summary>
        public GameplayScreen(NetworkSession networkSession)
        {
            this.networkSession = networkSession;
            
            
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }


        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (Content == null)
                Content = new ContentManager(ScreenManager.Game.Services, "Content");
            
            ship_Map = new Model();
            input = new InputHelper();
            sky = Content.Load<Sky>("Models\\sky1");
            ship_Map = Content.Load<Model>("ship_map");
            audioHelper = new Audio("Content\\TRA_Game.xgs");
            famas_1 = audioHelper.GetCue("famas-1");
            boneTransforms = new Matrix[ship_Map.Bones.Count];
            famas_forearm = audioHelper.GetCue("famas_forearm");
            ship_Map.CopyAbsoluteBoneTransformsTo(boneTransforms);
            camera = new FPSCamera(ScreenManager.Game.GraphicsDevice.Viewport);
            person1 = new DrawableModel(Content.Load<Model>("Models//model"), Matrix.Identity);
            person2 = new DrawableModel(Content.Load<Model>("Models//model"), Matrix.Identity);
            pistol = new DrawableModel(Content.Load<Model>("Models//pistol"), Matrix.Identity);
            awards = new AwardsComponent(ScreenManager.Game);
            shootAward = new Award { Name = "Shoot!", TextureAssetName = "award-1", ProgressNeeded = 10 };
            shootAward.LoadTexture(Content);
            awards.Awards.Add(shootAward);
            console = new ConsoleMenu(ScreenManager.Game);
            ScreenManager.Game.Components.Add(console);
            FPS_Counter_On = config.SettingGroups["DebugFeatures"].Settings["FPSCounterOn"].GetValueAsBool();
            if (FPS_Counter_On == true)
            {
                fpsCounter = new FrameRateCounter(ScreenManager.Game);
                ScreenManager.Game.Components.Add(fpsCounter);
            }
            ScreenManager.Game.Components.Add(awards = new AwardsComponent(ScreenManager.Game));
            hud = new HUD(ScreenManager.Game, person1, bulletAmount, maxBullets, ScreenManager.Game.Content, ScreenManager.SpriteBatch);
            ScreenManager.Game.Components.Add(hud);
            messageList = hud.messageList;

            playerMessage.title = "p";
            playerMessage.font = ScreenManager.Font;
            playerMessage.position = new Vector2(100, 527);
            playerMessage.text = "Player Score :" + PlayerScore.ToString();
            playerMessage.color = Color.Red;
            messageList.Add(playerMessage);

            enemyMessage.title = "e";
            enemyMessage.font = ScreenManager.Font;
            enemyMessage.position = new Vector2(100, 554);
            enemyMessage.color = Color.Blue;
            enemyMessage.text = "Enemy Score :" + enemyScore.ToString();
            messageList.Add(enemyMessage);

            bulletAmountMessage.title = "b";
            bulletAmountMessage.font = ScreenManager.Font;
            bulletAmountMessage.position = new Vector2(800, 554);
            bulletAmountMessage.color = Color.White;
            bulletAmountMessage.text = "Pistol :" + bulletAmount + "/" + maxBullets;
            messageList.Add(bulletAmountMessage);


            string filename = Environment.CurrentDirectory + "GameVariables";
            OpenFile(filename);

            person2.Position = new Vector3(0, 15, -30);
            bulletAmount = maxBullets;
            person1.Life = 100;
            // A real game would probably have more content than this sample, so
            // it would take longer to load. We simulate that by delaying for a
            // while, giving you a chance to admire the beautiful loading screen.
            Thread.Sleep(1000);
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            ScreenManager.Game.Components.Remove(hud);
            ScreenManager.Game.Components.Remove(fpsCounter);
            ScreenManager.Game.Components.Remove(console);
            Content.Unload();
        }
        #endregion

        #region Update
        /// <summary>
        /// Updates the state of the game.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (IsActive)
            {
                this.gameTime = gameTime;
                MouseState mouseState = Mouse.GetState();

                forwardReq = 0.0f;
                moveDirection = new Vector3(0, 0, 0);

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

                if (input.KeyDown(Keys.CapsLock))
                {
                    string filename = Environment.CurrentDirectory.ToString() + "GameVariables";
                    SaveVariables(filename);
                }

                if (input.KeyDown(Keys.Space))
                {
                    AddPlayerBullet();
                    awards.AddAwardProgress(shootAward, "Player 1", 10);
                }

                

                //UpdateEnemy(gameTime);
                UpdateWeapon(gameTime);

                camera.AddToCameraPosition(moveDirection, forwardReq, ref initalPos1, gameTime, false);
                

                person1.Position = initalPos1;
               
                
                person2.WorldMatrix = Matrix.CreateScale(2.0f);
                camera.Update(mouseState, person1.Position);

                pistol.WorldMatrix = Matrix.CreateScale(0.5f) * Matrix.CreateRotationY(3.2f);
                person1.WorldMatrix = Matrix.CreateScale(2.0f) * Matrix.CreateRotationY(4.05f);
                audioHelper.Update();

                if (person1.isRespawning == true)
                {
                    ScreenManager.AddScreen(new RespawnScreen(networkSession));
                    person1.isRespawning = false;
                }
                hud.UpdateBulletAmount(bulletAmount);
                if (person1.Life != 100)
                    person1.Life += 0.1f;
            }

            for (int i = 0; i < hud.messageList.Count; i++)
            {
                HUD.message currentMessage = hud.messageList[i];
                if (currentMessage.title == "b")
                     currentMessage.text = "Pistol :" + bulletAmount + "/" + maxBullets;
                else if (currentMessage.title == "p")
                    currentMessage.text = "Player Score :" + PlayerScore.ToString();
                else if (currentMessage.title == "e")
                    currentMessage.text = "Enemy Score :" + enemyScore.ToString();
                hud.messageList[i] = currentMessage;
            }
            
           
            
            // If we are in a network game, check if we should return to the lobby.
            if ((networkSession != null) && !IsExiting) 
                if (networkSession.SessionState == NetworkSessionState.Lobby)
                    LoadingScreen.Load(ScreenManager, true,
                                       new BackgroundScreen(true),
                                       new LobbyScreen(networkSession, null, false));

        }
        #endregion

        #region  UpdateEnemy / UpdateWeapon
        private void UpdateEnemy(GameTime gameTime)
        {
            double currentTime = gameTime.TotalGameTime.TotalMilliseconds;
            if (currentTime - lastEnemyBulletTime > 1000)
            {
                float? result;
                Matrix enemyRotation = Matrix.Invert(camera.CameraRotation);
                Vector3 direction = Vector3.Transform(person2.position, enemyRotation);
                Ray ray = new Ray(person2.Position, direction);
                CheckPlayerCollision(ray, out result);
                if (result != null)
                    person1.PlayerRecieveDamage(bulletDamage, initalPos1);
                lastEnemyBulletTime = currentTime;
                audioHelper.Play(famas_1, false, listener, emitter);
            }
        }

        private void UpdateWeapon(GameTime gameTime)
        {
            pistol.position.X = person1.position.X + pistolOffset.X;
            pistol.position.Y = person1.position.Y + pistolOffset.Y;
            pistol.position.Z = person1.position.Z + pistolOffset.Z;
        }
        #endregion

        #region Check Collision
        /// <summary>
        /// Checks if bullet sphere is in player sphere
        /// </summary>
        /// <param name="sphere">the bullet's bounding sphere</param>
        /// <returns>1 if colission
        /// 0 if no collision</returns>
        void CheckPlayerCollision(Ray ray, out float? result)
        {
            //Create the bounding sphere for the player
            BoundingSphere playerSphere = new BoundingSphere();
            playerSphere.Center = new Vector3(person1.Position.X, person1.Position.Y + 5f, person1.Position.Z);
            playerSphere.Radius = 5.0f;

            playerSphere.Intersects(ref ray,out result);
        }
        void CheckEnemyCollision(Ray ray, out float? result)
        {
            BoundingSphere enemySphere = new BoundingSphere();
            // put the center of the sphere in the centre of the model
            enemySphere.Center = new Vector3(person2.Position.X, person2.Position.Y + 5f, person2.Position.Z);
            enemySphere.Radius = 5.0f;

            enemySphere.Intersects(ref ray, out result);
        }
        #endregion

        #region HandleInput
        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            
            if (input == null)
                throw new ArgumentNullException("input");

            if (input.PauseGame)
                // If they pressed pause, bring up the pause menu screen.
                ScreenManager.AddScreen(new PauseMenuScreen(networkSession));
        }



        private void AddPlayerBullet()
        {
                if (bulletAmount > 0)
                { 
                    double currentTime = gameTime.TotalGameTime.TotalMilliseconds;
                    if (currentTime - lastBulletTime > 1000)
                    {
                        float? result;
                        Vector3 direction = Vector3.Transform(new Vector3(0,0,-1), camera.CameraRotation);
                        Ray ray = new Ray(pistol.position, direction);
                        CheckEnemyCollision(ray,out result);
                        if (result != null)
                            person2.EnemyRecieveDamage(bulletDamage);
                        
                        bulletAmount -= 1;
                        
                        lastBulletTime = currentTime;
                        audioHelper.Play(famas_1, false, listener, emitter);
                        
                    }
                }
                else
                {
                    //Reload
                    bulletAmount = maxBullets;
                    audioHelper.Play(famas_forearm, false, listener, emitter);
                } 
            }
        

#endregion 

        #region Draw code
        /// <summary>
        /// Draws the gameplay screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.GraphicsDevice.Clear(ClearOptions.Target | ClearOptions.DepthBuffer,
                Color.White, 1, 0);

            ScreenManager.GraphicsDevice.RenderState.CullMode = CullMode.None;
            ScreenManager.GraphicsDevice.RenderState.DepthBufferEnable = true;
            ScreenManager.GraphicsDevice.RenderState.AlphaBlendEnable = false;
            ScreenManager.GraphicsDevice.RenderState.AlphaTestEnable = false;
   
            person1.Model.Bones[0].Transform = person1.OriginalTransforms[0] * Matrix.CreateRotationX(camera.UpDownRot)
            * Matrix.CreateRotationY(camera.LeftRightRot);
            person1.Draw(camera);
            person2.Draw(camera);
            pistol.Model.Bones[0].Transform = pistol.OriginalTransforms[0] * Matrix.CreateRotationX(camera.UpDownRot)
                * Matrix.CreateRotationY(camera.LeftRightRot);
            pistol.Draw(camera);

            #region Draw Ship / Draw Sky
            foreach (ModelMesh mesh in ship_Map.Meshes)
            {
                foreach (BasicEffect effect in mesh.Effects)
                {
                   
                    effect.EnableDefaultLighting();
                    effect.World = boneTransforms[mesh.ParentBone.Index] * Matrix.CreateScale(4.0f);
                    effect.SpecularColor = new Vector3(1, 0, 0);
                    effect.View = camera.ViewMatrix;
                    effect.Projection = camera.ProjectionMatrix;
                }

                mesh.Draw();
            }

            
            sky.Draw(camera.ViewMatrix, camera.ProjectionMatrix);
            #endregion
            /*
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;
            spriteBatch.Begin();
            string message1 = "Player Score :" + PlayerScore.ToString();
            string message2 = "Enemy Score : " + enemyScore.ToString();
            Vector2 position = new Vector2(100, 480);
            spriteBatch.DrawString(font, message1, position, Color.Red);
            position.Y += 27;
            spriteBatch.DrawString(font, message2, position, Color.Blue);
            spriteBatch.End();
            */
            //hud.Draw(spriteBatch, font);
            base.Draw(gameTime);

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha);
        }
        #endregion
    }
}
