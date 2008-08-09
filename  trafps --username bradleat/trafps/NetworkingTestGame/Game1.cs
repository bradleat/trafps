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
using EGGEngine.Networking;

namespace NetworkingTestGame
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class NetworkingTestGame : Microsoft.Xna.Framework.Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        SignIn signIn;
        HandleEvents eventHandler;
        ReadNetworkData readNetworkData;
        SendNetworkData sendNetworkData;
        Session gameSession;
        PlayerReady playerReady;
        NetworkHelper networkHelper;

        // The Game session
        private NetworkSession session = null;
        // Only two players
        private int maximumGamers = 2;
        // no split-screen, only remote players
        private int maximumLocalPlayers = 1;
        // Private slots
        int privateSlots = 0;
        // Properties for custom searching (placeholder text)
        NetworkSessionProperties properties;
        // SpriteFont
        SpriteFont Arial;

        // Message regarding the sessions current state
        private String message = "Waiting for user command...";
        public String Message
        {
            get { return message; }
        }

        public NetworkingTestGame()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            Components.Add(new GamerServicesComponent(this));
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            eventHandler = new HandleEvents();
            readNetworkData = new ReadNetworkData();
            sendNetworkData = new SendNetworkData();
            gameSession = new Session();
            playerReady = new PlayerReady();
            signIn = new SignIn();
            networkHelper = new NetworkHelper();


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

            Arial = Content.Load<SpriteFont>("Arial");
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
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
                this.Exit();

            networkHelper.Update();

            if (Keyboard.GetState().IsKeyDown(Keys.F2))
            {
                gameSession.CreateSession(NetworkSessionType.SystemLink,
                    maximumLocalPlayers, maximumGamers, privateSlots, properties);
            }

            if (Keyboard.GetState().IsKeyDown(Keys.F1))
            {
                signIn.SignInGamer(1, false);
            }

            base.Update(gameTime);
        }


        public void HookSessionEvents()
        {
            networkHelper.session.GamerJoined += new EventHandler<GamerJoinedEventArgs>(session_GamerJoined);
            networkHelper.session.GamerLeft += new EventHandler<GamerLeftEventArgs>(session_GamerLeft);
            networkHelper.session.GameStarted += new EventHandler<GameStartedEventArgs>(session_GameStarted);
            networkHelper.session.GameEnded += new EventHandler<GameEndedEventArgs>(session_GameEnded);
            networkHelper.session.SessionEnded += new EventHandler<NetworkSessionEndedEventArgs>(session_SessionEnded);
            networkHelper.session.HostChanged += new EventHandler<HostChangedEventArgs>(session_HostChanged);

        }

        void session_GamerJoined(object sender, GamerJoinedEventArgs e)
        {
            if (e.Gamer.IsHost)
            {
                message = "The Host started the session";
            }
            else
                message = "Gamer " + e.Gamer.Tag + " joined the session";
        }

        void session_GamerLeft(object sender, GamerLeftEventArgs e)
        {
            message = "Gamer " + e.Gamer.Tag + " left the session";
        }

        void session_GameStarted(object sender, GameStartedEventArgs e)
        {
            message = "Game Started";
        }

        void session_HostChanged(object sender, HostChangedEventArgs e)
        {
            message = "Host changed from " + e.OldHost.Tag + " to " + e.NewHost.Tag;
        }

        void session_SessionEnded(object sender, NetworkSessionEndedEventArgs e)
        {
            message = "The session has ended";
        }

        void session_GameEnded(object sender, GameEndedEventArgs e)
        {
            message = "Game Over";
        }





        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            graphics.GraphicsDevice.Clear(Color.CornflowerBlue);

            // Show the current session state
            spriteBatch.Begin();
            spriteBatch.DrawString(Arial, "Game State: " + Message, new Vector2(20, 20), Color.Black);
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
