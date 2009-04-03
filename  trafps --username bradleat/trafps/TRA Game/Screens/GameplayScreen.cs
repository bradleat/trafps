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
using EGGEngine;
using EGGEngine.Cameras;
using EGGEngine.Debug;
using EGGEngine.Rendering;
using EGGEngine.Rendering.Shaders;
using EGGEngine.Helpers;
using EGGEngine.Networking;
using EGGEngine.Utils;
using EGGEngine.Audio;
using EGGEngine.Awards;
using EGGEngine.Physics;
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
        ModelTypes.Levels currentLevel;
        GameLevel level;
        Player localPlayer;
        Weapon weapon;

        NetworkSessionComponent.GameMode gameMode;
        NetworkSessionComponent.ScoreToWin scoreToWin;
        NetworkSessionComponent.NoOfBots noOfBots;
        NetworkSessionComponent.Weapons Weapons;
        

        NetworkHelper networkHelper;



        #region Fields

        public static FrameRateCounter fpsCounter;

        //Classes
        Microsoft.Xna.Framework.Net.NetworkSession networkSession;
        ContentManager Content;
        ConsoleMenu console;
        DrawableModel person1;
        DrawableModel person2;
        EGGEngine.Rendering.Shaders.PostProcessing postProc;
        DrawableModel pistol;
        InputHelper input;
        FirstPersonCamera camera;

        // Audio
        Audio audioHelper;
        Cue mystery;
        Cue famas_1;
        Cue famas_forearm;
        AudioEmitter emitter = new AudioEmitter();
        AudioListener listener = new AudioListener();

        //Physics
        World world;
        Model ship_Map;
        Sky sky;
        Matrix[] boneTransforms;

        // Vector3
        Vector3 initialPos2 = new Vector3(0, 15, -15); 
        Vector3 pistolOffset = new Vector3(1, 1, -10);
        Vector3 initalPos1 = new Vector3(0, 15, -2);
        Vector3 translate = Vector3.Zero;
        Vector3 modelPosition = new Vector3(0, -3, -5);
        Vector3 levelPos = new Vector3(0, 0, 0);
        Vector3 avatarOffset = new Vector3(0, 3, 0);

        //Debugging Stuff
        bool FPS_Counter_On;
        GameVariables gameVariables;
        Random random = new Random();
        ConfigFile config = new ConfigFile("content\\config.ini");
         
        //double / floats / ints
        double lastBulletTime = 0;
        double lastEnemyBulletTime = 0;
        const int maxBullets = 20;
        float modelRotation = 0f;
        float gameSpeed = 1.0f;
        float levelRot = 0.0f;
        float forwardReq = 0;
        float bulletSpeed;
        float bulletDamage;
        float gravity;
        int bulletAmount;
        int PlayerScore;
        int enemyScore;
        
        //Hud
        HUD hud;
        HUD.message playerMessage = new HUD.message();
        HUD.message enemyMessage = new HUD.message();
        HUD.message bulletAmountMessage = new HUD.message();       
        List<HUD.message> messageList;

        //Awards
        AwardsComponent awards;
        Award shootAward;

        #region bool IsActive
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
            //person1.Life = gameVariables.Data.playerHealth;
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
        public GameplayScreen(Microsoft.Xna.Framework.Net.NetworkSession networkSession, ModelTypes.Levels currentLevel)
        {
            networkHelper = new NetworkHelper();
            this.networkSession = networkSession;
            this.currentLevel = currentLevel;
            networkHelper.NetworkGameSession = networkSession;

            GetVariables();

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


            if (gameMode == null)
                throw new Exception("No gameMode selected");
            //Classes
            input = new InputHelper();
            console = new ConsoleMenu(ScreenManager.Game);
            ScreenManager.Game.Components.Add(console);
            //FPS            
            fpsCounter = new FrameRateCounter(ScreenManager.Game);
            ScreenManager.Game.Components.Add(fpsCounter);

            camera = new FirstPersonCamera(ScreenManager.GraphicsDevice.Viewport);
            level = LevelCreator.CreateLevel(ScreenManager.Game, currentLevel);
            //player = level.player;
            weapon = level.weapon;

            foreach (NetworkGamer gamer in networkSession.AllGamers)
            {
                Player player = gamer.Tag as Player;

                player.Initialize(ModelTypes.PlayerType.TankGirl, new Vector3(0, -3, -5), 0, new Vector3(0, 5, 0), level.world, level.weapon);
            }

            if (networkSession.LocalGamers.Count > 0)
            {
                localPlayer = networkSession.LocalGamers[0].Tag as Player;
            }

            #region HUD
            //HUD
            hud = new HUD(ScreenManager.Game, localPlayer, weapon.BulletsCount, weapon.MaxBullets, ScreenManager.Game.Content, ScreenManager.SpriteBatch);
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
            bulletAmountMessage.text = "Pistol :" + weapon.BulletsCount + "/" + weapon.MaxBullets;
            messageList.Add(bulletAmountMessage);
            #endregion

            string filename = Environment.CurrentDirectory + "GameVariables";
            OpenFile(filename);

            
     
            #region unused - speed up loading time
            //person2.Position = new Vector3(0, 15, -30);
            //bulletAmount = maxBullets;
            //ScreenManager.Game.Components.Add(awards = new AwardsComponent(ScreenManager.Game));
            //audioHelper = new Audio("Content\\TRA_Game.xgs");
            //famas_1 = audioHelper.GetCue("famas-1");
            //famas_forearm = audioHelper.GetCue("famas_forearm");
            //person2 = new DrawableModel(Content.Load<Model>("Models//model"), Matrix.Identity);
            //pistol = new DrawableModel(Content.Load<Model>("Models//pistol(1)"), Matrix.Identity);
            //awards = new AwardsComponent(ScreenManager.Game);
            //shootAward = new Award { Name = "Shoot!", TextureAssetName = "award-1", ProgressNeeded = 10 };
            //shootAward.LoadTexture(Content);
            //awards.Awards.Add(shootAward);
            #endregion

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

        void GetVariables()
        {
            gameMode = (NetworkSessionComponent.GameMode)networkSession.SessionProperties[(int)NetworkSessionComponent.SessionProperties.GameMode];
            Weapons = (NetworkSessionComponent.Weapons)networkSession.SessionProperties[(int)NetworkSessionComponent.SessionProperties.Weapons];
            scoreToWin = (NetworkSessionComponent.ScoreToWin)networkSession.SessionProperties[(int)NetworkSessionComponent.SessionProperties.ScoreToWin];
            noOfBots = (NetworkSessionComponent.NoOfBots)networkSession.SessionProperties[(int)NetworkSessionComponent.SessionProperties.NoOfBots];
        }


        #region Update
        /// <summary>
        /// Updates the state of the game.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);


            switch (gameMode)
            {
                case NetworkSessionComponent.GameMode.DeathMatch:
                    UpdateDeathMatch();
                    break;
                case NetworkSessionComponent.GameMode.TeamDeathmatch:
                    UpdateTeamDeathMatch();
                    break;
                case NetworkSessionComponent.GameMode.CaptureTheFlag:
                    UpdateCTF();
                    break;
                default:
                    break;
            }

            if (IsActive)
            {
                
                MouseState current_Mouse = Mouse.GetState();
                KeyboardState KeyState = Keyboard.GetState();

                localPlayer.Update(gameTime, current_Mouse, KeyState);

                camera.Position = localPlayer.Position + avatarOffset;

                camera.Update(localPlayer.Rotation, current_Mouse);

                Mouse.SetPosition(0, 0);
            
                // If we are in a network session, update it.
                if (networkSession.SessionType != NetworkSessionType.Local)
                {
                    UpdateNetworkSession();
                }
                /*
                MouseState current_Mouse = Mouse.GetState();
                KeyboardState KeyState = Keyboard.GetState();

                player.Update(gameTime,current_Mouse, KeyState);

                camera.Position = player.Position + avatarOffset;

                camera.Update(player.Rotation, current_Mouse);

                Mouse.SetPosition(0, 0);
                */
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
                                       new BackgroundScreen(false, ModelTypes.Levels.shipMap),
                                                   
                                       new LobbyScreen(networkSession, null, false));

        }
        #endregion

        void UpdateNetworkSession()
        {
            SendLocalPlayer();

            // If we are the server, update all the tanks and transmit
            // their latest positions back out over the network.
            if (networkSession.IsHost)
            {
                UpdateServer();
            }

            // Pump the underlying session object.
            networkSession.Update();

            // Make sure the session has not ended.
            if (networkSession == null)
                return;

            // Read any incoming network packets.
            foreach (LocalNetworkGamer gamer in networkSession.LocalGamers)
            {
                if (gamer.IsHost)
                {
                    ServerReadPlayerFromClients(gamer);
                }
                else
                {
                    ClientReadGameStateFromServer(gamer);
                }
            }
        }

        void SendLocalPlayer()
        {
            
            // Only send if we are not the server. There is no point sending packets
            // to ourselves, because we already know what they will contain!
            if (!networkSession.IsHost)
            {
                // Write our latest input state into a network packet.
                networkHelper.ClientPacketWriter.Write(localPlayer.Position); //packetWriter.Write(localTank.TankInput);
                networkHelper.ClientPacketWriter.Write(localPlayer.Rotation); //packetWriter.Write(localTank.TurretInput);

                networkHelper.SendClientData();
                // Send our input data to the server.
                //gamer.SendData(packetWriter,
                  //             SendDataOptions.InOrder, networkSession.Host);
            }
        }

        /// <summary>
        /// This method only runs on the server. It calls Update on all the
        /// tank instances, both local and remote, using inputs that have
        /// been received over the network. It then sends the resulting
        /// tank position data to everyone in the session.
        /// </summary>
        void UpdateServer()
        {
            networkHelper.ServerPacketWriter.Write(networkSession.AllGamers.Count);

            // First off, our packet will indicate how many tanks it has data for.
            //packetWriter.Write(networkSession.AllGamers.Count);

            // Loop over all the players in the session, not just the local ones!
            foreach (NetworkGamer gamer in networkSession.AllGamers)
            {
                // Look up what tank is associated with this player.
                Player player = gamer.Tag as Player;

                // Update the tank.
                //tank.Update();

                //Some smoothing predfiction code here....

                // Write the tank state into the output network packet.
                networkHelper.ServerPacketWriter.Write(player.Position);
                networkHelper.ServerPacketWriter.Write(player.Rotation);

                //packetWriter.Write(tank.Position);
                //packetWriter.Write(tank.TankRotation);
                //packetWriter.Write(tank.TurretRotation);
            }

            networkHelper.SendServerData();

            //server.SendData(packetWriter, SendDataOptions.InOrder);
        }

        /// <summary>
        /// This method only runs on the server. It reads tank inputs that
        /// have been sent over the network by a client machine, storing
        /// them for later use by the UpdateServer method.
        /// </summary>
        void ServerReadPlayerFromClients(LocalNetworkGamer gamer)
        {
            // Keep reading as long as incoming packets are available.
            while (gamer.IsDataAvailable)
            {
                NetworkGamer sender;

                sender = networkHelper.ReadServerData(gamer);

                // Read a single packet from the network.
                //gamer.ReceiveData(packetReader, out sender);

                if (!sender.IsLocal)
                {
                    // Look up the tank associated with whoever sent this packet.
                    Player remotePlayer = sender.Tag as Player;

                    // Read the latest inputs controlling this tank.
                    remotePlayer.Position = networkHelper.ServerPacketReader.ReadVector3();
                    remotePlayer.Rotation = networkHelper.ServerPacketReader.ReadSingle();

                    //remoteTank.TankInput = packetReader.ReadVector2();
                    //remoteTank.TurretInput = packetReader.ReadVector2();
                }
            }
        }

        /// <summary>
        /// This method only runs on client machines. It reads
        /// tank position data that has been computed by the server.
        /// </summary>
        void ClientReadGameStateFromServer(LocalNetworkGamer gamer)
        {
            // Keep reading as long as incoming packets are available.
            while (gamer.IsDataAvailable)
            {
                NetworkGamer sender;

                sender = networkHelper.ReadClientData(gamer);
                // Read a single packet from the network.
                //gamer.ReceiveData(packetReader, out sender);

                // If a player has recently joined or left, it is possible the server
                // might have sent information about a different number of players
                // than the client currently knows about. If so, we will be unable
                // to match up which data refers to which player. The solution is
                // just to ignore the packet for now: this situation will resolve
                // itself as soon as the client gets the join/leave notification.
                if (networkSession.AllGamers.Count != networkHelper.ClientPacketReader.ReadInt32())
                    continue;

                // This packet contains data about all the players in the session.
                foreach (NetworkGamer remoteGamer in networkSession.AllGamers)
                {
                    Player player = remoteGamer.Tag as Player;

                    // Read the state of this tank from the network packet.
                    player.Position = networkHelper.ClientPacketReader.ReadVector3();
                    player.Rotation = networkHelper.ClientPacketReader.ReadSingle();

                    //tank.Position = packetReader.ReadVector2();
                    //tank.TankRotation = packetReader.ReadSingle();
                    //tank.TurretRotation = packetReader.ReadSingle();
                }
            }
        }

        void UpdateDeathMatch()
        {

        }

        void UpdateTeamDeathMatch()
        {

        }

        void UpdateCTF()
        {

        }

        #region  UpdateEnemy / UpdateWeapon
        private void UpdateEnemy(GameTime gameTime)
        { }/*
            double currentTime = gameTime.TotalGameTime.TotalMilliseconds;
            if (currentTime - lastEnemyBulletTime > 1000)
            {
                float? result;
                Matrix enemyRotation = Matrix.Invert(camera.CameraRotation);
                Vector3 AI = new Vector3(2);
                Vector3 direction = Vector3.Normalize(person1.position * AI - person2.position);
                Ray ray = new Ray(person2.Position, direction);
                CheckPlayerCollision(ray, out result);
                if (result != null)
                {
                    person1.PlayerRecieveDamage(bulletDamage, initalPos1);
                    enemyScore += 1;
                }
                lastEnemyBulletTime = currentTime;
                audioHelper.Play(famas_1, false, listener, emitter);
            }
        }

        private void UpdateWeapon(GameTime gameTime)
        {
            pistol.position.X = person1.position.X + pistolOffset.X;
            pistol.position.Y = person1.position.Y + pistolOffset.Y;
            pistol.position.Z = person1.position.Z + pistolOffset.Z;
        }*/
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
        {/*
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
                        {
                            person2.EnemyRecieveDamage(bulletDamage);
                            PlayerScore += 1;
                        } 

                        
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
            */}
        

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

            level.sky.Draw(camera.ViewMatrix, camera.ProjectionMatrix);
            level.level.Draw(camera);

            
            foreach (NetworkGamer gamer in networkSession.AllGamers)
            {
                Player networkPlayer = gamer.Tag as Player;
                networkPlayer.Draw(gameTime, camera);
            } 
            
            

            
            base.Draw(gameTime);

            // If the game is transitioning on or off, fade it out to black.
            if (TransitionPosition > 0)
                ScreenManager.FadeBackBufferToBlack(255 - TransitionAlpha);
        }
        #endregion
    }
}
