#region Using Statements
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;

using EGGEngine.Audio;
using EGGEngine.Networking;
#endregion

namespace TRA_Game
{
    class SessionPropertiesScreen : MenuScreen
    {
        #region Fields

        NetworkSessionType sessionType;
        Audio audioHelper;
        Cue mystery;
        Cue famas_1;

        NetworkHelper networkHelper;
        NetworkInterface networkInterface;

        NetworkSessionComponent.GameMode gameModeType;
        NetworkSessionComponent.Weapons weaponsType;
        NetworkSessionComponent.ScoreToWin scoreToWinType;
        NetworkSessionComponent.NoOfBots noOfBots;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public SessionPropertiesScreen(ScreenManager screenManager,NetworkSessionType sessionType, Audio audioHelper, Cue mystery, bool createSession, NetworkHelper networkHelper)
            : base(Resources.SessionProperties + GetMenuTitle(sessionType), false)
        {
         
            if (networkHelper != null)
                this.networkHelper = networkHelper;
            else
                this.networkHelper = new NetworkHelper();
            networkInterface = new NetworkInterface();
            networkInterface.InitNetwork(screenManager.Game);
            this.audioHelper = audioHelper;
            this.mystery = mystery;
            this.sessionType = sessionType;
            famas_1 = audioHelper.GetCue("famas-1");

            // Create our menu entries.
            MenuEntry gameModeMenuEntry = new MenuEntry(Resources.GameMode);
            gameModeType = NetworkSessionComponent.GameMode.DeathMatch;
            MenuEntry weaponsMenuEntry = new MenuEntry(Resources.Weapons);
            weaponsType = NetworkSessionComponent.Weapons.Normal;
            MenuEntry scoreToWinMenuEntry = new MenuEntry(Resources.ScoreToWin);
            scoreToWinType = NetworkSessionComponent.ScoreToWin.One;
            MenuEntry createSessionMenuEntry = new MenuEntry(Resources.CreateSession);
            MenuEntry searchSessionMenuEntry = new MenuEntry(Resources.SearchSessions);
            MenuEntry StartGameMenuEntry = new MenuEntry(Resources.StartGame);
            MenuEntry noofbotsMenuEntry = new MenuEntry(Resources.NumberOfBots);
            noOfBots = NetworkSessionComponent.NoOfBots.Ten;

            /*
            MenuEntry createSessionMenuEntry = new MenuEntry(Resources.CreateSession);
            MenuEntry findSessionsMenuEntry = new MenuEntry(Resources.FindSessions);*/
            MenuEntry backMenuEntry = new MenuEntry(Resources.Back);

            // Hook up menu event handlers.
            //createSessionMenuEntry.Selected += CreateSessionMenuEntrySelected;
            //findSessionsMenuEntry.Selected += FindSessionsMenuEntrySelected;
            gameModeMenuEntry.Selected += GameModeMenuEntrySelected;
            weaponsMenuEntry.Selected += WeaponsMenuEntrySelected;
            scoreToWinMenuEntry.Selected += ScoreToWinMenuEntrySelected;
            createSessionMenuEntry.Selected += CreateSessionMenuEntrySelected;
            searchSessionMenuEntry.Selected += SearchSessionsMenuEntrySelected;
            StartGameMenuEntry.Selected += StartGameMenuEntrySelected;
            noofbotsMenuEntry.Selected += NoOfBotsMenuEntry;
            backMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            //MenuEntries.Add(createSessionMenuEntry);
            //MenuEntries.Add(findSessionsMenuEntry);
            MenuEntries.Add(gameModeMenuEntry);
            MenuEntries.Add(weaponsMenuEntry);
            MenuEntries.Add(scoreToWinMenuEntry);
            MenuEntries.Add(noofbotsMenuEntry);

            if (sessionType != NetworkSessionType.Local)
                if(createSession)
                    MenuEntries.Add(createSessionMenuEntry);
                else
                    MenuEntries.Add(searchSessionMenuEntry);
            else
                MenuEntries.Add(StartGameMenuEntry);
            MenuEntries.Add(backMenuEntry);
        }


        /// <summary>
        /// Helper chooses an appropriate menu title for the specified session type.
        /// </summary>
        static string GetMenuTitle(NetworkSessionType sessionType)
        {
            switch (sessionType)
            {
                case NetworkSessionType.PlayerMatch:
                    return Resources.PlayerMatch;

                case NetworkSessionType.SystemLink:
                    return Resources.SystemLink;

                case NetworkSessionType.Local:
                    return Resources.Training;

                default:
                    throw new NotSupportedException();
            }
        }


        #endregion

        #region Event Handlers

        void StartGameMenuEntrySelected(object sender, EventArgs e)
        {
            try
            {
                NetworkSessionProperties searchProperties = GetSessionProperties();

                IAsyncResult asyncResult = networkInterface.CreateNetwork(ScreenManager.Game, sessionType, NetworkSessionComponent.MaxLocalGamers, NetworkSessionComponent.MaxGamers, 0, searchProperties, true, true);

                NetworkBusyScreen busyScreen = new NetworkBusyScreen(asyncResult);

                busyScreen.OperationCompleted += CreateSinglePlayerOperationCompleted;

                ScreenManager.AddScreen(busyScreen);
            }
            catch (NetworkException exception)
            {
                ScreenManager.AddScreen(new NetworkErrorScreen(exception));
            }
            catch (GamerPrivilegeException exception)
            {
                ScreenManager.AddScreen(new NetworkErrorScreen(exception));
            }
        }

        void CreateSinglePlayerOperationCompleted(object sender, OperationCompletedEventArgs e)
        {
            try
            {
                // End the asynchronous create network session operation.
                networkHelper.NetworkGameSession = Microsoft.Xna.Framework.Net.NetworkSession.EndCreate(e.AsyncResult);

                // Create a component that will manage the session we just created.
                NetworkSessionComponent.Create(ScreenManager, networkHelper.NetworkGameSession);

                networkHelper.NetworkGameSession.StartGame();

                audioHelper.Stop(mystery);
                audioHelper.Play(famas_1, false, new AudioListener(), new AudioEmitter());

                // Go to the lobby screen.
                LoadingScreen.Load(ScreenManager, true, new GameplayScreen(networkHelper.NetworkGameSession, ModelTypes.Levels.shipMap));

            }
            catch (NetworkException exception)
            {
                ScreenManager.AddScreen(new NetworkErrorScreen(exception));
            }
            catch (GamerPrivilegeException exception)
            {
                ScreenManager.AddScreen(new NetworkErrorScreen(exception));
            }
        }

        void NoOfBotsMenuEntry(object sender, EventArgs e)
        {
            ChangeNoOfBots();
        }
        void ChangeNoOfBots()
        {
            if (noOfBots == NetworkSessionComponent.NoOfBots.Ten)
                noOfBots = NetworkSessionComponent.NoOfBots.Twenty;
            else if (noOfBots == NetworkSessionComponent.NoOfBots.Twenty)
                noOfBots = NetworkSessionComponent.NoOfBots.Ten;
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

        void GameModeMenuEntrySelected(object sender, EventArgs e)
        {
            ChangeGameModeType();
        }
        void ChangeGameModeType()
        {
            if (gameModeType == NetworkSessionComponent.GameMode.DeathMatch)
                gameModeType = NetworkSessionComponent.GameMode.TeamDeathmatch;
            else if (gameModeType == NetworkSessionComponent.GameMode.TeamDeathmatch)
                gameModeType = NetworkSessionComponent.GameMode.CaptureTheFlag;
            else if (gameModeType == NetworkSessionComponent.GameMode.CaptureTheFlag)
                gameModeType = NetworkSessionComponent.GameMode.DeathMatch;
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
        void WeaponsMenuEntrySelected(object sender, EventArgs e)
        {
            ChangeWeaponType();
        }
        void ChangeWeaponType()
        {
            if (weaponsType == NetworkSessionComponent.Weapons.Normal)
                weaponsType = NetworkSessionComponent.Weapons.Heavy;
            else if (weaponsType == NetworkSessionComponent.Weapons.Heavy)
                weaponsType = NetworkSessionComponent.Weapons.Light;
            else if (weaponsType == NetworkSessionComponent.Weapons.Light)
                weaponsType = NetworkSessionComponent.Weapons.Normal;
        }
        string GetWeaponType()
        {
            switch (weaponsType)
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

        void ScoreToWinMenuEntrySelected(object sender, EventArgs e)
        {
            ChangeScoreToWinType();
        }
        void ChangeScoreToWinType()
        {
            if (scoreToWinType == NetworkSessionComponent.ScoreToWin.One)
                scoreToWinType = NetworkSessionComponent.ScoreToWin.Three;
            else if (scoreToWinType == NetworkSessionComponent.ScoreToWin.Three)
                scoreToWinType = NetworkSessionComponent.ScoreToWin.Five;
            else if (scoreToWinType == NetworkSessionComponent.ScoreToWin.Five)
                scoreToWinType = NetworkSessionComponent.ScoreToWin.TwentyFive;
            else if (scoreToWinType == NetworkSessionComponent.ScoreToWin.TwentyFive)
                scoreToWinType = NetworkSessionComponent.ScoreToWin.Fifty;
            else if (scoreToWinType == NetworkSessionComponent.ScoreToWin.Fifty)
                scoreToWinType = NetworkSessionComponent.ScoreToWin.One;
            
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

        NetworkSessionProperties GetSessionProperties()
        {
            NetworkSessionProperties searchProperties = new NetworkSessionProperties();
            searchProperties[(int)NetworkSessionComponent.SessionProperties.GameMode] = (int)gameModeType;
            searchProperties[(int)NetworkSessionComponent.SessionProperties.Weapons] = (int)weaponsType;
            searchProperties[(int)NetworkSessionComponent.SessionProperties.ScoreToWin] = (int)scoreToWinType;
            searchProperties[(int)NetworkSessionComponent.SessionProperties.NoOfBots] = (int)noOfBots;
            return searchProperties;
        }

        
        
        /// <summary>
        /// Event handler for when the Create Session menu entry is selected.
        /// </summary>
        void CreateSessionMenuEntrySelected(object sender, EventArgs e)
        {
            NetworkSessionProperties searchProperties = GetSessionProperties();
            try
            {
                IAsyncResult asyncResult = networkInterface.CreateNetwork(ScreenManager.Game, sessionType, NetworkSessionComponent.MaxLocalGamers, NetworkSessionComponent.MaxGamers, 0,searchProperties,  true, true);
                // Begin an asynchronous create network session operation.
                //IAsyncResult asyncResult = Microsoft.Xna.Framework.Net.NetworkSession.BeginCreate(sessionType,
                                                //NetworkSessionComponent.MaxLocalGamers,
                                                //NetworkSessionComponent.MaxGamers,
                                                //null, null);

                // Activate the network busy screen, which will display
                // an animation until this operation has completed.
                NetworkBusyScreen busyScreen = new NetworkBusyScreen(asyncResult);

                busyScreen.OperationCompleted += CreateSessionOperationCompleted;

                ScreenManager.AddScreen(busyScreen);
            }
            catch (NetworkException exception)
            {
                ScreenManager.AddScreen(new NetworkErrorScreen(exception));
            }
            catch (GamerPrivilegeException exception)
            {
                ScreenManager.AddScreen(new NetworkErrorScreen(exception));
            }
        }


        /// <summary>
        /// Event handler for when the asynchronous create network session
        /// operation has completed.
        /// </summary>
        void CreateSessionOperationCompleted(object sender,
                                             OperationCompletedEventArgs e)
        {
            try
            {
                // End the asynchronous create network session operation.
                networkHelper.NetworkGameSession = Microsoft.Xna.Framework.Net.NetworkSession.EndCreate(e.AsyncResult);
                //networkHelper.NetworkGameSession.AllowHostMigration = true;
                //networkHelper.NetworkGameSession.AllowJoinInProgress = true;

                // Create a component that will manage the session we just created.
                NetworkSessionComponent.Create(ScreenManager, networkHelper.NetworkGameSession);

                // Go to the lobby screen.
                ScreenManager.AddScreen(new LobbyScreen(networkHelper.NetworkGameSession, audioHelper, true));
            }
            catch (NetworkException exception)
            {
                ScreenManager.AddScreen(new NetworkErrorScreen(exception));
            }
            catch (GamerPrivilegeException exception)
            {
                ScreenManager.AddScreen(new NetworkErrorScreen(exception));
            }
        }

        
        /// <summary>
        /// Event handler for when the Find Sessions menu entry is selected.
        /// </summary>
        void SearchSessionsMenuEntrySelected(object sender, EventArgs e)
        {
            try
            {
                NetworkSessionProperties searchProperties = GetSessionProperties();

                IAsyncResult asyncResult = networkInterface.JoinNetwork(ScreenManager.Game, sessionType, NetworkSessionComponent.MaxLocalGamers,searchProperties);
                // Begin an asynchronous find network sessions operation.
                //IAsyncResult asyncResult = Microsoft.Xna.Framework.Net.NetworkSession.BeginFind(sessionType,
                                               // NetworkSessionComponent.MaxLocalGamers,
                                                //null, null, null);

                // Activate the network busy screen, which will display
                // an animation until this operation has completed.
                NetworkBusyScreen busyScreen = new NetworkBusyScreen(asyncResult);

                busyScreen.OperationCompleted += FindSessionsOperationCompleted;

                ScreenManager.AddScreen(busyScreen);
            }
            catch (NetworkException exception)
            {
                ScreenManager.AddScreen(new NetworkErrorScreen(exception));
            }
            catch (GamerPrivilegeException exception)
            {
                ScreenManager.AddScreen(new NetworkErrorScreen(exception));
            }
        }


        /// <summary>
        /// Event handler for when the asynchronous find network sessions
        /// operation has completed.
        /// </summary>
        void FindSessionsOperationCompleted(object sender,
                                            OperationCompletedEventArgs e)
        {
            try
            {
                // End the asynchronous find network sessions operation.
                AvailableNetworkSessionCollection availableSessions =
                                                Microsoft.Xna.Framework.Net.NetworkSession.EndFind(e.AsyncResult);

                if (availableSessions.Count == 0)
                {
                    // If we didn't find any sessions, display an error.
                    availableSessions.Dispose();

                    ScreenManager.AddScreen(
                            new MessageBoxScreen(Resources.NoSessionsFound, false));
                }
                else
                {

                    // If we did find some sessions, proceed to the JoinSessionScreen.
                    ScreenManager.AddScreen(new JoinSessionScreen(availableSessions, audioHelper, mystery));
                }
            }
            catch (NetworkException exception)
            {
                ScreenManager.AddScreen(new NetworkErrorScreen(exception));
            }
            catch (GamerPrivilegeException exception)
            {
                ScreenManager.AddScreen(new NetworkErrorScreen(exception));
            }
        }

        
        #endregion

        #region Draw
        public override void Draw(GameTime gameTime)
        {
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;
            SpriteFont font = ScreenManager.Font;

            Vector2 position = new Vector2(300, 387);

            spriteBatch.Begin();
            spriteBatch.DrawString(font, GetGameModeType(), position, Color.Yellow);
            position.Y += 27;
            spriteBatch.DrawString(font, GetWeaponType(), position, Color.Yellow);
            position.Y += 27;
            spriteBatch.DrawString(font, GetScoreToWinType(), position, Color.Yellow);
            position.Y += 27;
            spriteBatch.DrawString(font, GetNoOfBots(), position, Color.Yellow);
            spriteBatch.End();
            base.Draw(gameTime);
        }
        #endregion
    }
}
