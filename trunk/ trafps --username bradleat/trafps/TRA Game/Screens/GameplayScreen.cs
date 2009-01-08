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
#endregion

namespace TRA_Game
{
    public struct GameVariables
    {
        public float bulletSpeed;
        public float bulletDamage;
        public float playerHealth;
        public float playerSpeed;
        public float gravity;
    }
    /// <summary>
    /// This screen implements the actual game logic. It is just a
    /// placeholder to get the idea across: you'll probably want to
    /// put some more interesting gameplay in here!
    /// </summary>
    class GameplayScreen : GameScreen
    {
        NetworkSession networkSession;
        GameTime gameTime;
        ContentManager Content;
        SpriteFont gameFont;

        Vector2 playerPosition = new Vector2(100, 100);
        Vector2 enemyPosition = new Vector2(100, 100);

        Random random = new Random();
        
        
        GraphicsDeviceManager graphics;

        //Debugging Stuff
        bool FPS_Counter_On;

        Vector3 initalPos1 = new Vector3(0, 15, 0);

        //Config File Stuff
        ConfigFile config = new ConfigFile("content\\config.ini");


        DrawableModel person1;
        DrawableModel bullet;
        Model terrain;
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

        List<DrawableModel> enemyBulletList = new List<DrawableModel>();
        List<BoundingSphere> enemyBulletSpheres = new List<BoundingSphere>();

        List<DrawableModel> bulletList = new List<DrawableModel>();
        List<BoundingSphere> bulletspheres = new List<BoundingSphere>();
        BoundingSphere bulletSphere, enemySphere, enemybulletSphere, playerSphere;
        double lastBulletTime = 0;
        double lastEnemyBulletTime = 0;
        float gameSpeed = 1.0f;
        int bulletAmount = 10;
        float bulletDamage;
        float bulletSpeed;
        float gravity;
        int PlayerScore;
        int enemyScore;

        


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
            input = new InputHelper();
            audioHelper = new Audio("Content\\TRA_Game.xgs");
            famas_1 = audioHelper.GetCue("famas-1");
            famas_forearm = audioHelper.GetCue("famas_forearm");
            camera = new FPSCamera(ScreenManager.Game.GraphicsDevice.Viewport);
            FPS_Counter_On = config.SettingGroups["DebugFeatures"].Settings["FPSCounterOn"].GetValueAsBool();

            string name = config.SettingGroups["Filenames"].Settings["person"].GetValueAsString();
            person1 = new DrawableModel(Content.Load<Model>(name), Matrix.Identity);

            person2 = new DrawableModel(Content.Load<Model>(name), Matrix.Identity);

            name = config.SettingGroups["Filenames"].Settings["terrain"].GetValueAsString();
            terrain = new Model();
            terrain = Content.Load<Model>(name);

            string filename = Environment.CurrentDirectory + "GameVariables";
            OpenFile(filename);
            sky = Content.Load<Sky>("Models\\sky1");
            gameFont = Content.Load<SpriteFont>("gamefont");
            // Comment this to remove the framerate counter
            if (FPS_Counter_On == true)
            {
                ScreenManager.Game.Components.Add(new FrameRateCounter(ScreenManager.Game));
            }
            person2.Position = new Vector3(0, 15, -30);
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
            Content.Unload();
        }



       
        


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

                if (input.KeyDown(Keys.CapsLock))
                {
                    string filename = Environment.CurrentDirectory.ToString() + "GameVariables";
                    SaveVariables(filename);
                }

                if (input.KeyDown(Keys.Space))
                {
                    //Check if we have bullets remaining
                    if (bulletAmount != 0)
                    {
                        double currentTime = gameTime.TotalGameTime.TotalMilliseconds;
                        if (currentTime - lastBulletTime > 1000)
                        {
                            DrawableModel newBullet = new DrawableModel(Content.Load<Model>("cube"), Matrix.Identity);
                            newBullet.Position = person1.Position;
                            newBullet.startingPosition = person1.Position;
                            newBullet.Rotation = camera.CameraRotation;
                            bulletList.Add(newBullet);
                            bulletSphere = new BoundingSphere(newBullet.Position, 1.0f);
                            bulletspheres.Add(bulletSphere);

                            bulletAmount -= 1;
                            lastBulletTime = currentTime;
                            audioHelper.Play(famas_1, false, listener, emitter);
                        }

                    }
                    else
                    {
                        //Reload
                        bulletAmount = 10;
                        audioHelper.Play(famas_forearm, false, listener, emitter);
                    }
                }
                UpdateEnemy(gameTime);
                UpdateEnemyBulletPositions(bulletSpeed);
                // Move the bullets at a speed
                UpdateBulletPositions(bulletSpeed);
                if (bulletList.Count > 0)
                {
                    //Check collision between each bullet and the enemy
                    for (int i = 0; i < bulletList.Count; i++)
                    {
                        DrawableModel bullet = bulletList[i];
                        bulletSphere = new BoundingSphere(bullet.Position, 1.5f);
                        int result = CheckEnemyCollision(bulletSphere);
                        if (result == 1)
                        {
                            person2.EnemyRecieveDamage(bulletDamage);
                            PlayerScore += 1;
                            bulletspheres.RemoveAt(i);
                            bulletList.RemoveAt(i);
                            i--;
                        }
                        else
                            bulletList[i] = bullet;

                    }
                }
                

                camera.AddToCameraPosition(moveDirection, forwardReq, ref initalPos1, gameTime);
                person1.Position = initalPos1;

                //person2.Position = initialPos2;
                person2.WorldMatrix = Matrix.CreateScale(2.0f);


                camera.Update(mouseState, person1.Position);

                person1.WorldMatrix = Matrix.CreateScale(2.0f) * Matrix.CreateRotationY(4.05f);
                audioHelper.Update();
                if (person1.isRespawning == true)
                {
                    ScreenManager.AddScreen(new RespawnScreen(networkSession));
                    person1.isRespawning = false;
                }
               
                    }
            

            // If we are in a network game, check if we should return to the lobby.
            if ((networkSession != null) && !IsExiting)
            {
                
                if (networkSession.SessionState == NetworkSessionState.Lobby)
                {
                    LoadingScreen.Load(ScreenManager, true,
                                       new BackgroundScreen(true),
                                       new LobbyScreen(networkSession, null, false));
                }
            }
        }
        private void UpdateEnemy(GameTime gameTime)
        {
            //TODO: Fix bullet to come at player. Rotation problem.


            double currentTime = gameTime.TotalGameTime.TotalMilliseconds;
            if (currentTime - lastEnemyBulletTime > 1000)
            {
                DrawableModel newBullet = new DrawableModel(Content.Load<Model>("cube"), Matrix.Identity);
                newBullet.Position = person2.Position;
                newBullet.Rotation = person2.Rotation;
                newBullet.startingPosition = person2.Position;
                newBullet.targetPosition = person1.Position;
                enemyBulletList.Add(newBullet);
                enemybulletSphere = new BoundingSphere(newBullet.Position, 1.0f);
                enemyBulletSpheres.Add(enemybulletSphere);
                audioHelper.Play(famas_1, false, new AudioListener(), new AudioEmitter());

                lastEnemyBulletTime = currentTime;
            }
            
            float moveSpeed = gameTime.ElapsedGameTime.Milliseconds / 500.0f * gameSpeed;
            UpdateEnemyBulletPositions(bulletSpeed);

            if (enemyBulletList.Count > 0)
            {
                // Checking for collision between the bullets and the player
                for (int i = 0; i < enemyBulletList.Count; i++)
                {
                    DrawableModel bullet = enemyBulletList[i];
                    bulletSphere = new BoundingSphere(bullet.Position, 1.5f);
                    int result = CheckPlayerCollision(bulletSphere);
                    if (result == 1)
                    {
                        initalPos1 = person1.PlayerRecieveDamage(bulletDamage, initalPos1);
                        enemyScore += 1;
                        enemyBulletSpheres.RemoveAt(i);
                        enemyBulletList.RemoveAt(i);
                        i--;
                    }
                    else
                        enemyBulletList[i] = bullet;

                }
            }
        }
        private void UpdateEnemyBulletPositions(float moveSpeed)
        {
            float maxDistance = 200f;
            float bulletDistance;

            
            for (int i = 0; i < enemyBulletList.Count; i++)
            {
                DrawableModel currentBullet = enemyBulletList[i];
                //currentBullet.Position = MoveForward(currentBullet.Position,
                //Problem here, bullets not moving?? AddVector returns zero? rotation problem
                currentBullet.Position = MoveEnemyBullets(currentBullet.Position,currentBullet.startingPosition, currentBullet.targetPosition, bulletSpeed);
                Vector3.Distance(ref currentBullet.startingPosition, ref currentBullet.position, out bulletDistance);
                if (bulletDistance > maxDistance)
                {
                    enemyBulletSpheres.RemoveAt(i);
                    enemyBulletList.RemoveAt(i);
                    i--;
                }
                else if (currentBullet.position == currentBullet.targetPosition)
                {
                    enemyBulletSpheres.RemoveAt(i);
                    enemyBulletList.RemoveAt(i);
                    i--;
                }
                else
                    enemyBulletList[i] = currentBullet;
            }
        }
        /// <summary>
        /// move each bullet.
        /// </summary>
        /// <param name="moveSpeed">Speed at which each bullet moves</param>
        private void UpdateBulletPositions(float moveSpeed)
        {

            float maxDistance = 200f;
            float bulletDistance;
            for (int i = 0; i < bulletList.Count; i++)
            {
                DrawableModel currentBullet = bulletList[i];
                currentBullet.Position = MoveForward(currentBullet.Position, currentBullet.Rotation, moveSpeed * 2.0f);
                Vector3.Distance(ref camera.cameraPosition, ref currentBullet.position, out bulletDistance);
                if (bulletDistance > maxDistance)
                {
                    bulletspheres.RemoveAt(i);
                    bulletList.RemoveAt(i);
                    i--;
                }
                else
                    bulletList[i] = currentBullet;
            }

        }
        private Vector3 MoveEnemyBullets(Vector3 currentPosition, Vector3 startPosition, Vector3 targetPos, float bulletSpeed)
        {
            Vector3 addVector = Vector3.Normalize(targetPos - startPosition);
            
            currentPosition += addVector * bulletSpeed;
            return currentPosition;
        }
        private Vector3 MoveForward(Vector3 position, Matrix rotation, float speed)
        {
            Vector3 addVector = Vector3.Transform(new Vector3(0, 0, -1), rotation);
            //addVector returns zero for enemy ?/?
            position += addVector * speed;
            return position;
        }

        /// <summary>
        /// Checks if bullet sphere is in player sphere
        /// </summary>
        /// <param name="sphere">the bullet's bounding sphere</param>
        /// <returns>1 if colission
        /// 0 if no collision</returns>
        int CheckPlayerCollision(BoundingSphere sphere)
        {

            //Create the bounding sphere for the player
            playerSphere = new BoundingSphere();
            playerSphere.Center = new Vector3(person1.Position.X, person1.Position.Y + 5f, person1.Position.Z);
            playerSphere.Radius = 5.0f;

            if (playerSphere.Contains(sphere) != ContainmentType.Disjoint)
                return 1;
            else
                return 0;

        }
        int CheckEnemyCollision(BoundingSphere sphere)
        {
            enemySphere = new BoundingSphere();
            // put the center of the sphere in the centre of the model
            enemySphere.Center = new Vector3(person2.Position.X, person2.Position.Y + 5f, person2.Position.Z);
            enemySphere.Radius = 5.0f;

            if (enemySphere.Contains(sphere) != ContainmentType.Disjoint)
                return 1;
            else
                return 0;

        }

        /// <summary>
        /// Lets the game respond to player input. Unlike the Update method,
        /// this will only be called when the gameplay screen is active.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            if (input == null)
                throw new ArgumentNullException("input");

            if (input.PauseGame)
            {
                // If they pressed pause, bring up the pause menu screen.
                ScreenManager.AddScreen(new PauseMenuScreen(networkSession));
            }
            else
            {
                float forwardReq = 0;

                Vector3 moveDirection = new Vector3(0, 0, 0);

                if (input.IsNewKeyPress(Keys.S))
                {
                    forwardReq += 5.0f;
                    moveDirection = new Vector3(0, 0, 1);  //Backward
                }
                if (input.IsNewKeyPress(Keys.W))
                {
                    forwardReq += 5.0f;
                    moveDirection = new Vector3(0, 0, -1);  //Forward
                }
                if (input.IsNewKeyPress(Keys.A))
                {
                    forwardReq += 5.0f;
                    moveDirection = new Vector3(-1, 0, 0);  //Left
                }
                if (input.IsNewKeyPress(Keys.D))
                {
                    forwardReq += 5.0f;
                    moveDirection = new Vector3(1, 0, 0);   //Right
                }
                if (input.IsNewKeyPress(Keys.Space))
                {
                    //Check if we have bullets remaining
                    if (bulletAmount != 0)
                    {
                        double currentTime = gameTime.TotalGameTime.TotalMilliseconds;
                        if (currentTime - lastBulletTime > 1000)
                        {
                            DrawableModel newBullet = new DrawableModel(Content.Load<Model>("cube"), Matrix.Identity);
                            newBullet.Position = person1.Position;
                            newBullet.Rotation = camera.CameraRotation;
                            bulletList.Add(newBullet);
                            bulletSphere = new BoundingSphere(newBullet.Position, 1.0f);
                            bulletspheres.Add(bulletSphere);

                            bulletAmount -= 1;
                            lastBulletTime = currentTime;
                            audioHelper.Play(famas_1, false, listener, emitter);
                        }

                    }
                    else
                    {
                        //Reload
                        bulletAmount = 10;
                        audioHelper.Play(famas_forearm, false, listener, emitter);
                    }
                }/*
                // Otherwise move the player position.
                Vector2 movement = Vector2.Zero;

                for (int i = 0; i < InputState.MaxInputs; i++)
                {
                    if (input.CurrentKeyboardStates[i].IsKeyDown(Keys.Left))
                        movement.X--;

                    if (input.CurrentKeyboardStates[i].IsKeyDown(Keys.Right))
                        movement.X++;

                    if (input.CurrentKeyboardStates[i].IsKeyDown(Keys.Up))
                        movement.Y--;

                    if (input.CurrentKeyboardStates[i].IsKeyDown(Keys.Down))
                        movement.Y++;

                    Vector2 thumbstick = input.CurrentGamePadStates[i].ThumbSticks.Left;

                    movement.X += thumbstick.X;
                    movement.Y -= thumbstick.Y;
                }

                if (movement.Length() > 1)
                    movement.Normalize();

                playerPosition += movement * 2;*/
            }
        }


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

            
                if (bulletList.Count > 0)
                {
                    for (int i = 0; i < bulletList.Count; i++)
                    {
                        DrawableModel currentBullet = bulletList[i];
                        currentBullet.Model.Bones[0].Transform = person1.OriginalTransforms[0] * Matrix.CreateRotationX(camera.UpDownRot)
                        * Matrix.CreateRotationY(camera.LeftRightRot);
                        currentBullet.Draw(camera);
                        bulletList[i] = currentBullet;
                    }
                }
                if (enemyBulletList.Count > 0)
                {
                    for (int i = 0; i < enemyBulletList.Count; i++)
                    {

                        DrawableModel currentBullet = enemyBulletList[i];
                        
                        currentBullet.Model.Bones[0].Transform = person1.OriginalTransforms[0] * Matrix.CreateRotationX(camera.UpDownRot)
                        * Matrix.CreateRotationY(camera.LeftRightRot);
                        currentBullet.Draw(camera);
                        enemyBulletList[i] = currentBullet;
                    }
                }

                //enemy bullets draw fine, just commented out code until bug is fixed

                
                
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

            if (networkSession != null)
            {
                spriteBatch = ScreenManager.SpriteBatch;
                spriteBatch.Begin();
                string message = "Players: " + networkSession.AllGamers.Count;
                Vector2 messagePosition = new Vector2(100, 480);
                spriteBatch.DrawString(gameFont, message, messagePosition, Color.White);
                spriteBatch.End();
            }

           
            

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha);
        }


        
    }
}
