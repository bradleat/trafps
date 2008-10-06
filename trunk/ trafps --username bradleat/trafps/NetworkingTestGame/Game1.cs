#region License
//=============================================================================
// System  : Networking Game Loop
// File    : Game1.cs
// Author  : Evan
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
#endregion

#region Revision Number
// Revision Number: 0.1.0.0
//
// Revision Number: Major.Minor.Build.Bug
#endregion

namespace NetworkingTestGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        #region (Properties)
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        int gameMode = 0;
        int gamerIndex = 0;

        private SpriteFont gameFont;

        private Texture2D meteorTexture;

        private int lastTickCount, lifeTime = 0;
        Player player;
        private KeyboardState keyboard;
        private int bulletCount;
        private const int STARTBULLETCOUNT = 50;
        private const int ADDBULLETTIME = 1000;

        const int maxGamers = 16;
        const int maxLocalGamers = 4;
        NetworkSession networkSession;
        string errorMessage;

        const int screenHeight = 600;
        const int screenWidth = 900;

        PacketWriter packetWriter = new PacketWriter();
        PacketReader packetReader = new PacketReader();

        #endregion

        /// <summary>
        /// Prepares the graphics device
        /// Adds LIVE services
        /// </summary>
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Components.Add(new GamerServicesComponent(this));
        }

        /// <summary>
        /// Changes Title of window
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();

            this.Window.Title = "Network Demo";
        }

        /// <summary>
        /// Loads a spritebatch, textures and a font.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            Services.AddService(typeof(SpriteBatch), spriteBatch);

            meteorTexture = Content.Load<Texture2D>("RockRain");

            // Load game font
            gameFont = Content.Load<SpriteFont>("font");


            // TODO: use this.Content to load your game content here
        }


        /// <summary>
        /// Starts the Single Player game.
        /// </summary>
        private void Start()
        {
            //RemoveAllBullets();
            for (int i = 0; i < STARTBULLETCOUNT; i++)
            {
                Components.Add(new Bullet(this, ref meteorTexture));
            }

            // Create (if necessary) and put the player in start position
            if (player == null)
            {
                // Add the player component
                player = new Player(this, ref meteorTexture, gamerIndex);
                Components.Add(player);
                player.PutinStartPosition();
            }


            // Initialize a counter
            lastTickCount = System.Environment.TickCount;
            lifeTime = System.Environment.TickCount;
            bulletCount = STARTBULLETCOUNT;
        }
        /// <summary>
        /// Starts the Network Game
        /// </summary>
        private void StartNetworkGame()
        {
            // Create (if necessary) and put the player in start position
            if (player == null)
            {
                // Add the player component
                player = new Player(this, ref meteorTexture, gamerIndex);
                Components.Add(player);
                player.PutinStartPosition();
            }
        }


        /// <summary>
        /// Checks for input and changes Gamemode accordingly.
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Update(GameTime gameTime)
        {

            keyboard = Keyboard.GetState();

            if (keyboard.IsKeyDown(Keys.Escape))
            {
                Exit();
            }

            if (gameMode == 0)
            {
                // If we are not in a network session, update the
                // menu screen that will let us create or join one.
                UpdateMenuScreen();
            }
            else
                if (gameMode == 1)
                {

                    if (player == null)
                    {
                        Start();
                    }




                    player.Update(keyboard);

                    // Check collisions
                    bool hasColision = false;
                    Rectangle shipRectangle = player.GetBounds();
                    foreach (GameComponent gc in Components)
                    {
                        if (gc is Bullet)
                        {
                            hasColision = ((Bullet)gc).CheckCollision(shipRectangle);
                            if (hasColision)
                            {

                                RemoveAllBullets();
                                // Let's start again
                                gameMode = 0;

                                lifeTime = System.Environment.TickCount - lifeTime;
                                break;
                            }
                        }
                    }

                    CheckforNewBullet();
                }

                else if (gameMode == 2)
                {
                    UpdateNetworkSession(gameTime);
                }

            base.Update(gameTime);
        }
        /// <summary>
        /// Menu screen provides options to create or join network sessions.
        /// </summary>
        void UpdateMenuScreen()
        {
            if (IsActive)
            {
                if (Gamer.SignedInGamers.Count == 0)
                {
                    // If there are no profiles signed in, we cannot proceed.
                    // Show the Guide so the user can sign in.
                    Guide.ShowSignIn(maxLocalGamers, false);
                }
                else if (keyboard.IsKeyDown(Keys.A))
                {
                    // Create a new session?
                    CreateSession();
                    gameMode = 2;
                }
                else if (keyboard.IsKeyDown(Keys.B))
                {
                    // Join an existing session?
                    JoinSession();
                }
                else if (keyboard.IsKeyDown(Keys.Enter))
                {
                    Start();
                    gameMode = 1;
                }
            }
        }

        /// <summary>
        /// Starts hosting a new network session.
        /// </summary>
        void CreateSession()
        {
            DrawMessage("Creating session...");

            try
            {
                networkSession = NetworkSession.Create(NetworkSessionType.SystemLink,
                                                       maxLocalGamers, maxGamers);

                HookSessionEvents();
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
            }
        }

        /// <summary>
        /// Joins an existing network session.
        /// </summary>
        void JoinSession()
        {
            DrawMessage("Joining session...");

            try
            {
                // Search for sessions.
                using (AvailableNetworkSessionCollection availableSessions =
                            NetworkSession.Find(NetworkSessionType.SystemLink,
                                                maxLocalGamers, null))
                {
                    if (availableSessions.Count == 0)
                    {
                        errorMessage = "No network sessions found.";
                        return;
                    }

                    // Join the first session we found.
                    networkSession = NetworkSession.Join(availableSessions[0]);
                    DrawMessage("Connected to Session!!");
                    gameMode = 2;
                    HookSessionEvents();
                }
            }
            catch (Exception e)
            {
                errorMessage = e.Message;
            }
        }

        /// <summary>
        /// After creating or joining a network session, we must subscribe to
        /// some events so we will be notified when the session changes state.
        /// </summary>
        void HookSessionEvents()
        {
            networkSession.GamerJoined += GamerJoinedEventHandler;
            networkSession.SessionEnded += SessionEndedEventHandler;
        }


        /// <summary>
        /// This event handler will be called whenever a new gamer joins the session.
        /// We use it to allocate a Tank object, and associate it with the new gamer.
        /// </summary>
        void GamerJoinedEventHandler(object sender, GamerJoinedEventArgs e)
        {
            int gamerIndex = networkSession.AllGamers.IndexOf(e.Gamer);

            e.Gamer.Tag = new NetworkPlayer(gamerIndex, Content, screenWidth, screenHeight);

        }


        /// <summary>
        /// Event handler notifies us when the network session has ended.
        /// </summary>
        void SessionEndedEventHandler(object sender, NetworkSessionEndedEventArgs e)
        {
            errorMessage = e.EndReason.ToString();

            networkSession.Dispose();
            networkSession = null;
            gameMode = 0;
        }

        /// <summary>
        /// Updates the state of the network session, moving the tanks
        /// around and synchronizing their state over the network.
        /// </summary>
        void UpdateNetworkSession(GameTime gameTime)
        {
            // Read inputs for locally controlled tanks, and send them to the server.
            foreach (LocalNetworkGamer gamer in networkSession.LocalGamers)
            {
                UpdateLocalGamer(gamer);
            }

            // If we are the server, update all the tanks and transmit
            // their latest positions back out over the network.
            if (networkSession.IsHost)
            {
                UpdateServer(gameTime);
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
                    ServerReadInputFromClients(gamer);
                }
                else
                {
                    ClientReadGameStateFromServer(gamer);
                }
            }
        }


        /// <summary>
        /// Helper for updating a locally controlled gamer.
        /// </summary>
        void UpdateLocalGamer(LocalNetworkGamer gamer)
        {
            // Look up what tank is associated with this local player,
            // and read the latest user inputs for it. The server will
            // later use these values to control the tank movement.
            NetworkPlayer localPlayer = gamer.Tag as NetworkPlayer;

            localPlayer.Update();
            // Only send if we are not the server. There is no point sending packets
            // to ourselves, because we already know what they will contain!
            if (!networkSession.IsHost)
            {
                // Write our latest input state into a network packet.
                packetWriter.Write(localPlayer.Position);

                // Send our input data to the server.
                gamer.SendData(packetWriter,
                               SendDataOptions.InOrder, networkSession.Host);
            }
        }


        /// <summary>
        /// This method only runs on the server. It calls Update on all the
        /// tank instances, both local and remote, using inputs that have
        /// been received over the network. It then sends the resulting
        /// tank position data to everyone in the session.
        /// </summary>
        void UpdateServer(GameTime gameTime)
        {
            // First off, our packet will indicate how many tanks it has data for.
            packetWriter.Write(networkSession.AllGamers.Count);

            // Loop over all the players in the session, not just the local ones!
            foreach (NetworkGamer gamer in networkSession.AllGamers)
            {
                // Look up what tank is associated with this player.
                NetworkPlayer player = gamer.Tag as NetworkPlayer;

                // Update the tank.
                player.Update();

                // Write the tank state into the output network packet.
                packetWriter.Write(player.Position);
            }

            // Send the combined data for all tanks to everyone in the session.
            LocalNetworkGamer server = (LocalNetworkGamer)networkSession.Host;

            server.SendData(packetWriter, SendDataOptions.InOrder);
        }


        /// <summary>
        /// This method only runs on the server. It reads tank inputs that
        /// have been sent over the network by a client machine, storing
        /// them for later use by the UpdateServer method.
        /// </summary>
        void ServerReadInputFromClients(LocalNetworkGamer gamer)
        {
            // Keep reading as long as incoming packets are available.
            while (gamer.IsDataAvailable)
            {
                NetworkGamer sender;

                // Read a single packet from the network.
                gamer.ReceiveData(packetReader, out sender);

                if (!sender.IsLocal)
                {
                    // Look up the tank associated with whoever sent this packet.
                    NetworkPlayer remotePlayer = sender.Tag as NetworkPlayer;

                    // Read the latest inputs controlling this tank.
                    remotePlayer.Position = packetReader.ReadVector2();
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

                // Read a single packet from the network.
                gamer.ReceiveData(packetReader, out sender);

                // If a player has recently joined or left, it is possible the server
                // might have sent information about a different number of players
                // than the client currently knows about. If so, we will be unable
                // to match up which data refers to which player. The solution is
                // just to ignore the packet for now: this situation will resolve
                // itself as soon as the client gets the join/leave notification.
                if (networkSession.AllGamers.Count != packetReader.ReadInt32())
                    continue;

                // This packet contains data about all the players in the session.
                foreach (NetworkGamer remoteGamer in networkSession.AllGamers)
                {
                    NetworkPlayer ship = remoteGamer.Tag as NetworkPlayer;

                    // Read the state of this tank from the network packet.
                    ship.Position = packetReader.ReadVector2();
                }
            }
        }


        private void CheckforNewBullet()
        {
            // Add a rock each ADDMETEORTIME
            if ((System.Environment.TickCount - lastTickCount) > ADDBULLETTIME)
            {
                lastTickCount = System.Environment.TickCount;
                Components.Add(new Bullet(this, ref meteorTexture));

                bulletCount++;
            }
        }



        /// <summary>
        /// Draws the screen based on the Gamemode
        /// 0 = MenuScreen
        /// 1 = Single PLayer
        /// 2 = Multiplayer
        /// </summary>
        /// <param name="gameTime"></param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.Black);

            switch (gameMode)
            {
                case 1:


                    // Start rendering sprites
                    spriteBatch.Begin(SpriteBlendMode.AlphaBlend);

                    base.Draw(gameTime);

                    // End rendering sprites
                    spriteBatch.End();

                    spriteBatch.Begin();

                    float surviveTime;
                    if (gameMode == 0.0f) surviveTime = lifeTime / 1000.0f;
                    else surviveTime = (float)(System.Environment.TickCount - lifeTime) / 1000.0f;

                    spriteBatch.DrawString(gameFont, "Number of Bullets: " + bulletCount.ToString(), new Vector2(10, 10), Color.White);
                    spriteBatch.DrawString(gameFont, "Time Survived: " + surviveTime.ToString() + " Seconds", new Vector2(10, 30), Color.White);

                    spriteBatch.End();
                    break;

                case 2:

                    DrawNetworkGame(gameTime);
                    break;


                case 0:
                    string message = string.Empty;

                    if (!string.IsNullOrEmpty(errorMessage))
                        message += "Error:\n" + errorMessage.Replace(". ", ".\n") + "\n\n";

                    message += "Enter: Start Game\n" +
                               "A: Create Session\n" +
                               "B: Join Session\n" +
                               "Esc: Quit Game";

                    spriteBatch.Begin();

                    spriteBatch.DrawString(gameFont, message, new Vector2(120, 100), Color.White);

                    spriteBatch.End();
                    break;



                default:
                    break;

            }
        }
        /// <summary>
        /// Draws the Network Game
        /// </summary>
        /// <param name="gameTime"></param>
        void DrawNetworkGame(GameTime gameTime)
        {
            // Start rendering sprites
            spriteBatch.Begin(SpriteBlendMode.AlphaBlend);

            //base.Draw(gameTime);

            // For each person in the session...
            foreach (NetworkGamer gamer in networkSession.AllGamers)
            {
                // Look up the tank object belonging to this network gamer.
                NetworkPlayer ship = gamer.Tag as NetworkPlayer;

                // Draw the tank.
                ship.Draw(spriteBatch);

                // Draw a gamertag label.
                string label = gamer.Gamertag;
                Color labelColor = Color.White;
                Vector2 labelOffset = new Vector2(100, 150);

                if (gamer.IsHost)
                    label += " (server)";

                // Flash the gamertag to yellow when the player is talking.
                if (gamer.IsTalking)
                    labelColor = Color.Yellow;

                spriteBatch.DrawString(gameFont, label, ship.Position, labelColor, 0,
                                       labelOffset, 0.6f, SpriteEffects.None, 0);
            }

            spriteBatch.End();

        }


        /// <summary>
        /// Helper draws notification messages before calling blocking network methods.
        /// </summary>
        void DrawMessage(string message)
        {
            if (!BeginDraw())
                return;

            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            spriteBatch.DrawString(gameFont, message, new Vector2(160, 160), Color.White);

            spriteBatch.End();

            EndDraw();
        }

        /// <summary>
        /// Reads input data from keyboard and gamepad, and stores
        /// it into the specified player object.
        /// </summary>
        void ReadShipInputs(NetworkPlayer ship, PlayerIndex playerIndex)
        {
            // Read the gamepad.
            GamePadState gamePad = GamePad.GetState(playerIndex);

            Vector2 tankInput = gamePad.ThumbSticks.Left;
            Vector2 turretInput = gamePad.ThumbSticks.Right;

            // Read the keyboard.
            KeyboardState keyboard = Keyboard.GetState(playerIndex);

            if (keyboard.IsKeyDown(Keys.Left))
                tankInput.X = -1;
            else if (keyboard.IsKeyDown(Keys.Right))
                tankInput.X = 1;

            if (keyboard.IsKeyDown(Keys.Up))
                tankInput.Y = 1;
            else if (keyboard.IsKeyDown(Keys.Down))
                tankInput.Y = -1;

            if (keyboard.IsKeyDown(Keys.A))
                turretInput.X = -1;
            else if (keyboard.IsKeyDown(Keys.D))
                turretInput.X = 1;

            if (keyboard.IsKeyDown(Keys.W))
                turretInput.Y = 1;
            else if (keyboard.IsKeyDown(Keys.S))
                turretInput.Y = -1;

            // Normalize the input vectors.
            if (tankInput.Length() > 1)
                tankInput.Normalize();

            if (turretInput.Length() > 1)
                turretInput.Normalize();

            // Store these input values into the tank object.
            ship.TankInput = tankInput;
            ship.TurretInput = turretInput;
        }
        /// <summary>
        /// Removes all bullets when exiting the Single Player game.
        /// </summary>
        private void RemoveAllBullets()
        {
            for (int i = 0; i < Components.Count; i++)
            {
                if (Components[i] is Bullet)
                {
                    Components.RemoveAt(i);
                    i--;
                }
            }
        }
    }
}

