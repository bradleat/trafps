#region File Description
//-----------------------------------------------------------------------------
// LobbyScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.GamerServices;

using EGGEngine.Audio;
using EGGEngine.Networking;
#endregion

namespace TRA_Game
{
    /// <summary>
    /// The lobby screen provides a place for gamers to congregate before starting
    /// the actual gameplay. It displays a list of all the gamers in the session,
    /// and indicates which ones are currently talking. Each gamer can press a button
    /// to mark themselves as ready: gameplay will begin after everyone has done this.
    /// </summary>
    class LobbyScreen : GameScreen
    {
        #region Fields

        NetworkSession networkSession;

        Texture2D isReadyTexture;
        Texture2D hasVoiceTexture;
        Texture2D isTalkingTexture;
        Texture2D voiceMutedTexture;

        AudioManager audioManager;

        Player localPlayer;
        KeyboardState oldKeyboardState = new KeyboardState();

        NetworkSessionComponent.Level level;
        NetworkSessionComponent.GameMode gameModeType;
        NetworkSessionComponent.NoOfBots noOfBots;
        NetworkSessionComponent.ScoreToWin scoreToWinType;
        NetworkSessionComponent.Weapons weapons;

        NetworkHelper networkHelper;

        // This will hold a string of messages until
        // a breakpoint ("[END]") is reached. Uses a
        // dictionary to maintain a string for each
        // gamertag in the game.
        Dictionary<string, string> incomingMessages = new Dictionary<string, string>();

        // A simple list of messages to render them
        // each frame.
        List<string[]> messages = new List<string[]>();


        #endregion

        #region Initialization


        /// <summary>
        /// Constructs a new lobby screen.
        /// </summary>
        public LobbyScreen(NetworkSession networkSession, AudioManager audioManager //Audio audioHelper
            , bool audio_on)
        {
            this.networkSession = networkSession;

            networkSession.GamerJoined += GamerJoined;

            networkHelper = new NetworkHelper();

            GetVariables();

            if (networkSession.LocalGamers.Count > 0)
            {
                localPlayer = networkSession.LocalGamers[0].Tag as Player;
            }

            // Adds a simple message to tell the user what to do.
            // Since we will be using the guide to get commands,
            // we need to tell them how to open it up!
            messages.Add(new string[] { "System", "Press [Tab] to send a message " });


            this.audioManager = audioManager;
            /*
            if (audioHelper == null)
                this.audioHelper = new Audio("Content\\TRA_Game.xgs");
            else
                this.audioHelper = audioHelper;
            if (audio_on == false)
            {
                mystery = this.audioHelper.GetCue("mystery");
                this.audioHelper.Play(mystery, false, new AudioListener(), new AudioEmitter());
            }
            else
                mystery = this.audioHelper.GetCue("mystery");

            this.audioHelper.Update();*/
            TransitionOnTime = TimeSpan.FromSeconds(0.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);
        }


        /// <summary>
        /// Loads graphics content used by the lobby screen.
        /// </summary>
        public override void LoadContent()
        {
            ContentManager content = ScreenManager.Game.Content;

            isReadyTexture = content.Load<Texture2D>("chat_ready");
            hasVoiceTexture = content.Load<Texture2D>("chat_able");
            isTalkingTexture = content.Load<Texture2D>("chat_talking");
            voiceMutedTexture = content.Load<Texture2D>("chat_mute");
        }



        #endregion

        #region Update


        /// <summary>
        /// Updates the lobby screen.
        /// </summary>
        public override void Update(GameTime gameTime, bool otherScreenHasFocus,
                                                       bool coveredByOtherScreen)
        {
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

            if (IsActive)
            {
                UpdateNetwork();

                KeyboardState keyboardState = Keyboard.GetState();
                if (keyboardState.IsKeyDown(Keys.F1) && !oldKeyboardState.IsKeyDown(Keys.T) && networkSession.IsHost)
                {
                    ScreenManager.AddScreen(new SessionPropertiesPopUpScreen(networkSession));
                }
                if (keyboardState.IsKeyDown(Keys.T) && !oldKeyboardState.IsKeyDown(Keys.T) && !networkSession.LocalGamers[0].IsReady)
                {
                    if (localPlayer.TeamID == 0)
                    {
                        localPlayer.TeamID = 1;
                    }
                    else
                        localPlayer.TeamID = 0;

                    //Write the team packet
                    networkHelper.ClientPacketWriter.Write('T');
                    networkHelper.ClientPacketWriter.Write(localPlayer.TeamID);
                    networkHelper.SendToAll(networkSession, networkHelper.ClientPacketWriter, SendDataOptions.None);

                    networkSession.Update();

                }
                oldKeyboardState = keyboardState;
            }


            GetVariables();

            if (!IsExiting)
            {

                if (networkSession.SessionState == NetworkSessionState.Playing)
                {
                    //audioHelper.Stop(mystery);
                    // Check if we should leave the lobby and begin gameplay.
                    LoadingScreen.Load(ScreenManager, true,
                                       new GameplayScreen(networkSession, audioManager));
                }
                else if (networkSession.IsHost && networkSession.IsEveryoneReady)
                {
                    //audioHelper.Stop(mystery);
                    // The host checks whether everyone has marked themselves
                    // as ready, and starts the game in response.
                    networkSession.StartGame();
                }
            }
        }

        /// <summary>
        /// Event handler called when a gamer joins the session.
        /// Displays a notification message.
        /// </summary>
        void GamerJoined(object sender, GamerJoinedEventArgs e)
        {
            if (e.Gamer != networkSession.LocalGamers[0])
            {
                networkHelper.ClientPacketWriter.Write('T');
                networkHelper.ClientPacketWriter.Write(localPlayer.TeamID);
                networkHelper.SendToAll(networkSession, networkHelper.ClientPacketWriter, SendDataOptions.ReliableInOrder);

            }
        }

        void GetVariables()
        {
            level = (NetworkSessionComponent.Level)networkSession.SessionProperties[(int)NetworkSessionComponent.SessionProperties.Level];
            gameModeType = (NetworkSessionComponent.GameMode)networkSession.SessionProperties[(int)NetworkSessionComponent.SessionProperties.GameMode];
            weapons = (NetworkSessionComponent.Weapons)networkSession.SessionProperties[(int)NetworkSessionComponent.SessionProperties.Weapons];
            scoreToWinType = (NetworkSessionComponent.ScoreToWin)networkSession.SessionProperties[(int)NetworkSessionComponent.SessionProperties.ScoreToWin];
            noOfBots = (NetworkSessionComponent.NoOfBots)networkSession.SessionProperties[(int)NetworkSessionComponent.SessionProperties.NoOfBots];
        }

        void UpdateNetwork()
        {
            // If the session isn't null and we are actually in it,
            // we need to handle a few things.
            if (networkSession != null && networkSession.LocalGamers.Count > 0)
            {
                try
                {
                    // First we need to update the session.
                    networkSession.Update();

                    // If there is data to be received,
                    // we should probably grab it.
                    if (networkSession.LocalGamers[0].IsDataAvailable)
                    {

                        // This will allow us to decode a string.
                        System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();

                        NetworkGamer sender;

                        sender = networkHelper.ReadClientData(networkSession.LocalGamers[0]);
                        try
                        {
                            while (networkHelper.ClientPacketReader.PeekChar() != -1)
                            {
                                char header = networkHelper.ClientPacketReader.ReadChar();
                                if (header == 'C')
                                {
                                    string packetString = encoding.GetString(networkHelper.ClientPacketReader.ReadBytes(networkHelper.ClientPacketReader.Length));// networkHelper.ClientPacketReader.ReadInt32();

                                    // If the gamertag isn't logged already, add it to
                                    // the dictionary. Otherwise add the string to the
                                    // current string.
                                    if (incomingMessages.ContainsKey(sender.Gamertag))
                                        incomingMessages[sender.Gamertag] += packetString;
                                    else
                                        incomingMessages.Add(sender.Gamertag, packetString);

                                    // If there is a breakpoint in the string, we need to
                                    // take care of the message!
                                    if (incomingMessages[sender.Gamertag].Contains("[END]"))
                                    {
                                        messages.Add(
                                            new string[] { sender.Gamertag,
                                    incomingMessages[sender.Gamertag].Substring(0, incomingMessages[sender.Gamertag].IndexOf("[END]")) }
                                            );

                                        // Wipe out the string!
                                        incomingMessages[sender.Gamertag] = string.Empty;
                                    }
                                }
                                else if (header == 'T')
                                {
                                    Player remotePlayer = sender.Tag as Player;
                                    remotePlayer.TeamID = networkHelper.ClientPacketReader.ReadInt32();
                                }
                            }
                        }
                        catch (Exception e)
                        {
                            return;
                        }
                    }
                }
                // Catch and show the message.
                catch (NullReferenceException nre)
                {
                    messages.Add(new string[] { "[ERROR]", nre.Message });
                }
            }
        }
        /// <summary>
        /// Handles user input for all the local gamers in the session. Unlike most
        /// screens, which use the InputState class to combine input data from all
        /// gamepads, the lobby needs to individually mark specific players as ready,
        /// so it loops over all the local gamers and reads their inputs individually.
        /// </summary>
        public override void HandleInput(InputState input)
        {
            foreach (LocalNetworkGamer gamer in networkSession.LocalGamers)
            {
                PlayerIndex playerIndex = gamer.SignedInGamer.PlayerIndex;

                if (input.IsMenuSelect(playerIndex))
                {
                    HandleMenuSelect(gamer);
                }
                else if (input.IsMenuCancel(playerIndex))
                {
                    HandleMenuCancel(gamer);
                }
                if (input.IsNewKeyPress(Keys.Tab))
                {
                    // Get some message from the user to send.
                    Guide.BeginShowKeyboardInput(playerIndex, "Send Message",
                        "Type the message you wish to broadcast.", "", SendMessage, null);
                }
            }
        }


        /// <summary>
        /// Handle MenuSelect inputs by marking ourselves as ready.
        /// </summary>
        void HandleMenuSelect(LocalNetworkGamer gamer)
        {
            if (!gamer.IsReady)
            {
                gamer.IsReady = true;
            }
            else if (gamer.IsHost)
            {
                // The host has an option to force starting the game, even if not
                // everyone has marked themselves ready. If they press select twice
                // in a row, the first time marks the host ready, then the second
                // time we ask if they want to force start.
                MessageBoxScreen messageBox = new MessageBoxScreen(
                                                    Resources.ConfirmForceStartGame);

                messageBox.Accepted += ConfirmStartGameMessageBoxAccepted;

                ScreenManager.AddScreen(messageBox);
            }
        }


        /// <summary>
        /// Event handler for when the host selects ok on the "are you sure
        /// you want to start even though not everyone is ready" message box.
        /// </summary>
        void ConfirmStartGameMessageBoxAccepted(object sender, EventArgs e)
        {
            if (networkSession.SessionState == NetworkSessionState.Lobby)
            {
                networkSession.StartGame();
            }
        }


        /// <summary>
        /// Handle MenuCancel inputs by clearing our ready status, or if it is
        /// already clear, prompting if the user wants to leave the session.
        /// </summary>
        void HandleMenuCancel(LocalNetworkGamer gamer)
        {
            if (gamer.IsReady)
            {
                gamer.IsReady = false;
            }
            else
            {
                NetworkSessionComponent.LeaveSessionFromGame(ScreenManager, audioManager); //audioHelper);
            }
        }

        /// <summary>
        /// Handles when the user wishes to send a message.
        /// </summary>
        /// <param name="result"></param>
        protected void SendMessage(IAsyncResult result)
        {
            // Get the string that they entered.
            string input = Guide.EndShowKeyboardInput(result);

            // If it is empty, just return.
            if (string.IsNullOrEmpty(input))
                return;

            // First add it locally.
            messages.Add(new string[] { Gamer.SignedInGamers[0].Gamertag, input });

            // If the session is null, don't send it.
            if (networkSession == null)
                return;

            // Encode the message into an array of bytes.
            System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
            byte[] bytes = encoding.GetBytes(input + "[END]");

            // Write it to a construct that helps
            // with sending messages.
            //PacketWriter writer = new PacketWriter();
            //writer.Write(bytes);

            // C for chat
            networkHelper.ClientPacketWriter.Write('C');
            networkHelper.ClientPacketWriter.Write(bytes);
            networkHelper.SendToAll(networkSession, networkHelper.ClientPacketWriter, SendDataOptions.Chat);

            networkSession.Update();
        }



        #endregion

        #region Draw


        /// <summary>
        /// Draws the lobby screen.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            Vector2 position = new Vector2(100, 150);

            // Make the lobby slide into place during transitions.
            float transitionOffset = (float)Math.Pow(TransitionPosition, 2);

            if (ScreenState == ScreenState.TransitionOn)
                position.X -= transitionOffset * 256;
            else
                position.X += transitionOffset * 512;

            spriteBatch.Begin();

            // Draw all the gamers in the session.
            int gamerCount = 0;

            foreach (NetworkGamer gamer in networkSession.AllGamers)
            {
                DrawGamer(gamer, position);

                // Advance to the next screen position, wrapping into two
                // columns if there are more than 8 gamers in the session.
                if (++gamerCount == 8)
                {
                    position.X += 433;
                    position.Y = 150;
                }
                else
                    position.Y += ScreenManager.Font.LineSpacing;
            }

            // Draw the screen title.
            string title = Resources.Lobby;

            Vector2 titlePosition = new Vector2(533, 80);
            Vector2 titleOrigin = font.MeasureString(title) / 2;
            Color titleColor = new Color(192, 192, 192, TransitionAlpha);
            float titleScale = 1.25f;

            titlePosition.Y -= transitionOffset * 100;

            spriteBatch.DrawString(font, title, titlePosition, titleColor, 0,
                                   titleOrigin, titleScale, SpriteEffects.None, 0);

            spriteBatch.End();

            DrawChat();

            DrawVariables();
        }

        void DrawVariables()
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont spriteFont = ScreenManager.Font;

            Vector2 position = new Vector2(50, 400);
            spriteBatch.Begin();

            string text = Resources.Level;

            //string text = GetGameModeType();
            spriteBatch.DrawString(spriteFont, text, position, Color.White);

            text = Resources.GameMode;
            position.Y += 27;
            spriteBatch.DrawString(spriteFont, text, position, Color.White);

            text = Resources.Weapons;
            position.Y += 27;
            spriteBatch.DrawString(spriteFont, text, position, Color.White);

            text = Resources.ScoreToWin;
            position.Y += 27;
            spriteBatch.DrawString(spriteFont, text, position, Color.White);

            text = Resources.NumberOfBots;
            position.Y += 27;
            spriteBatch.DrawString(spriteFont, text, position, Color.White);


            // Varibles---------------------------------

            text = GetLevel();
            position = new Vector2(200, 400);
            spriteBatch.DrawString(spriteFont, text, position, Color.Yellow);

            text = GetGameModeType();
            position.Y += 27;
            spriteBatch.DrawString(spriteFont, text, position, Color.Yellow);

            text = GetWeaponType();
            position.Y += 27;
            spriteBatch.DrawString(spriteFont, text, position, Color.Yellow);

            text = GetScoreToWinType();
            position.Y += 27;
            spriteBatch.DrawString(spriteFont, text, position, Color.Yellow);

            text = GetNoOfBots();
            position.Y += 27;
            spriteBatch.DrawString(spriteFont, text, position, Color.Yellow);

            spriteBatch.End();
        }

        void DrawChat()
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;
            if (messages.Count > 0)
            {
                // If there are more than 20 messages,
                // trim the array.
                if (messages.Count > 10)
                    messages.RemoveAt(0);

                Vector2 pos = new Vector2(600, 150);

                spriteBatch.Begin();

                for (int i = 0; i < messages.Count; i++)
                {
                    if (messages[i].Length < 2)
                        continue;

                    Vector2 measure = font.MeasureString(messages[i][0] + ": ");
                    float height = measure.Y + 5;
                    measure.Y = 0;

                    spriteBatch.DrawString(font, messages[i][0] + ": ", pos, Color.DarkOrange);
                    spriteBatch.DrawString(font, messages[i][1], pos + measure, Color.Yellow);

                    pos.Y += height;
                }

                spriteBatch.End();
            }

        }
        /// <summary>
        /// Helper draws the gamertag and status icons for a single NetworkGamer.
        /// </summary>
        void DrawGamer(NetworkGamer gamer, Vector2 position)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            Player networkPlayer = gamer.Tag as Player;

            Vector2 iconWidth = new Vector2(34, 0);
            Vector2 iconOffset = new Vector2(0, 0);

            Vector2 iconPosition = position + iconOffset;

            // Draw the "is ready" icon.
            if (gamer.IsReady)
            {
                spriteBatch.Draw(isReadyTexture, iconPosition,
                                 FadeAlphaDuringTransition(Color.Lime));
            }

            iconPosition += iconWidth;

            // Draw the "is muted", "is talking", or "has voice" icon.
            if (gamer.IsMutedByLocalUser)
            {
                spriteBatch.Draw(voiceMutedTexture, iconPosition,
                                 FadeAlphaDuringTransition(Color.Red));
            }
            else if (gamer.IsTalking)
            {
                spriteBatch.Draw(isTalkingTexture, iconPosition,
                                 FadeAlphaDuringTransition(Color.Yellow));
            }
            else if (gamer.HasVoice)
            {
                spriteBatch.Draw(hasVoiceTexture, iconPosition,
                                 FadeAlphaDuringTransition(Color.White));
            }

            // Draw the gamertag, normally in white, but yellow for local players.
            string text = gamer.Gamertag;

            if (gamer.IsHost)
                text += Resources.HostSuffix;

            Color color; //= (gamer.IsLocal) ? Color.Yellow : Color.White;

            if (networkPlayer.TeamID == 1)
                color = Color.Red;
            else
                color = Color.Blue;

            spriteBatch.DrawString(font, text, position + iconWidth * 2,
                                   FadeAlphaDuringTransition(color));
        }




        /// <summary>
        /// Helper modifies a color to fade its alpha value during screen transitions.
        /// </summary>
        Color FadeAlphaDuringTransition(Color color)
        {
            return new Color(color.R, color.G, color.B, TransitionAlpha);
        }

        string GetLevel()
        {
            switch (level)
            {
                case NetworkSessionComponent.Level.shipMap:
                    return Resources.Level_ShipMap;
                case NetworkSessionComponent.Level.Level_1:
                    return Resources.Level_1;
                default:
                    throw new Exception();
            }
        }

        string GetNoOfBots()
        {
            switch (noOfBots)
            {
                case NetworkSessionComponent.NoOfBots.Ten:
                    return Resources.NumberOfBots10;
                case NetworkSessionComponent.NoOfBots.Twenty:
                    return Resources.NumerOfBots20;
                default:
                    throw new NotSupportedException();
            }
        }

        string GetGameModeType()
        {
            switch (gameModeType)
            {
                case NetworkSessionComponent.GameMode.DeathMatch:
                    return Resources.GameModeTypeDM;
                case NetworkSessionComponent.GameMode.TeamDeathmatch:
                    return Resources.GameModeTypeTDM;
                case NetworkSessionComponent.GameMode.CaptureTheFlag:
                    return Resources.GameModeTypeCTF;
                default:
                    throw new NotSupportedException();
            }
        }

        string GetWeaponType()
        {
            switch (weapons)
            {
                case NetworkSessionComponent.Weapons.Light:
                    return Resources.WeaponsTypeLight;
                case NetworkSessionComponent.Weapons.Normal:
                    return Resources.WeaponsTypeNormal;
                case NetworkSessionComponent.Weapons.Heavy:
                    return Resources.WeaponsTypeHeavy;
                default:
                    throw new NotSupportedException();
            }
        }

        string GetScoreToWinType()
        {
            switch (scoreToWinType)
            {
                case NetworkSessionComponent.ScoreToWin.One:
                    return Resources.ScoreToWinType1;
                case NetworkSessionComponent.ScoreToWin.Three:
                    return Resources.ScoreToWinType3;
                case NetworkSessionComponent.ScoreToWin.Five:
                    return Resources.ScoreToWinType5;
                case NetworkSessionComponent.ScoreToWin.TwentyFive:
                    return Resources.ScoreToWinTypeTwentyFive;
                case NetworkSessionComponent.ScoreToWin.Fifty:
                    return Resources.ScoreToWinTypeFifty;
                default:
                    throw new NotSupportedException();
            }
        }

        #endregion
    }
}
