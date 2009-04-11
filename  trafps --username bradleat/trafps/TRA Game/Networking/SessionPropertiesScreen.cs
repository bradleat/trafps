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

        AudioManager audioManager;

        NetworkHelper networkHelper;
        NetworkInterface networkInterface;

        NetworkSessionComponent.Level level;
        NetworkSessionComponent.GameMode gameModeType;
        NetworkSessionComponent.Weapons weaponsType;
        NetworkSessionComponent.ScoreToWin scoreToWinType;
        NetworkSessionComponent.NoOfBots noOfBots;

        bool createSession;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public SessionPropertiesScreen(ScreenManager screenManager, NetworkSessionType sessionType, AudioManager audioManager, //Audio audioHelper, Cue mystery
             bool createSession, NetworkHelper networkHelper)
            : base(Resources.SessionProperties + GetMenuTitle(sessionType), false)
        {

            if (networkHelper != null)
                this.networkHelper = networkHelper;
            else
                this.networkHelper = new NetworkHelper();
            networkInterface = new NetworkInterface();
            networkInterface.InitNetwork(screenManager.Game);
            this.audioManager = audioManager;
            //this.audioHelper = audioHelper;
            //this.mystery = mystery;
            this.sessionType = sessionType;
            //famas_1 = audioHelper.GetCue("famas-1");

            this.createSession = createSession;

            // Create our menu entries.
            MenuEntry levelMenuEntry = new MenuEntry(Resources.Level);
            MenuEntry gameModeMenuEntry = new MenuEntry(Resources.GameMode);
            MenuEntry weaponsMenuEntry = new MenuEntry(Resources.Weapons);
            MenuEntry scoreToWinMenuEntry = new MenuEntry(Resources.ScoreToWin);
            MenuEntry createSessionMenuEntry = new MenuEntry(Resources.CreateSession);
            MenuEntry searchSessionMenuEntry = new MenuEntry(Resources.SearchSessions);
            MenuEntry StartGameMenuEntry = new MenuEntry(Resources.StartGame);
            MenuEntry noofbotsMenuEntry = new MenuEntry(Resources.NumberOfBots);
            MenuEntry backMenuEntry = new MenuEntry(Resources.Back);


            if (createSession)
            {
                level = NetworkSessionComponent.Level.shipMap;
                gameModeType = NetworkSessionComponent.GameMode.DeathMatch;
                weaponsType = NetworkSessionComponent.Weapons.Normal;
                scoreToWinType = NetworkSessionComponent.ScoreToWin.One;
                noOfBots = NetworkSessionComponent.NoOfBots.Ten;
            }
            else
            {
                level = NetworkSessionComponent.Level.Any;
                gameModeType = NetworkSessionComponent.GameMode.Any;
                weaponsType = NetworkSessionComponent.Weapons.Any;
                scoreToWinType = NetworkSessionComponent.ScoreToWin.Any;
                noOfBots = NetworkSessionComponent.NoOfBots.Any;
            }


            // Hook up menu event handlers.
            levelMenuEntry.Selected += LevelMenuEntrySelected;
            gameModeMenuEntry.Selected += GameModeMenuEntrySelected;
            weaponsMenuEntry.Selected += WeaponsMenuEntrySelected;
            scoreToWinMenuEntry.Selected += ScoreToWinMenuEntrySelected;
            createSessionMenuEntry.Selected += CreateSessionMenuEntrySelected;
            searchSessionMenuEntry.Selected += SearchSessionsMenuEntrySelected;
            StartGameMenuEntry.Selected += StartGameMenuEntrySelected;
            noofbotsMenuEntry.Selected += NoOfBotsMenuEntry;
            backMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(levelMenuEntry);
            MenuEntries.Add(gameModeMenuEntry);
            MenuEntries.Add(weaponsMenuEntry);
            MenuEntries.Add(scoreToWinMenuEntry);
            MenuEntries.Add(noofbotsMenuEntry);

            if (sessionType != NetworkSessionType.Local)
                if (createSession)
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


                //audioHelper.Play(famas_1, false, new AudioListener(), new AudioEmitter());

                LoadingScreen.Load(ScreenManager, false, new BackgroundScreen(true, NetworkSessionComponent.Level.shipMap), new LobbyScreen(networkHelper.NetworkGameSession, audioManager,//audioHelper
                    true));

                // Go to the lobby screen.
                //ScreenManager.AddScreen(new LobbyScreen(networkHelper.NetworkGameSession, audioHelper, true));
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
            else if (noOfBots == NetworkSessionComponent.NoOfBots.Twenty && !createSession)
                noOfBots = NetworkSessionComponent.NoOfBots.Any;
            else if (noOfBots == NetworkSessionComponent.NoOfBots.Twenty)
                noOfBots = NetworkSessionComponent.NoOfBots.Ten;
            else if (noOfBots == NetworkSessionComponent.NoOfBots.Any)
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
                case NetworkSessionComponent.NoOfBots.Any:
                    return Resources.Any;
                default:
                    throw new NotSupportedException();
            }
        }

        void LevelMenuEntrySelected(object sender, EventArgs e)
        {
            ChangeLevel();
        }

        void ChangeLevel()
        {
            if (level == NetworkSessionComponent.Level.shipMap)
                if (!createSession)
                    level = NetworkSessionComponent.Level.Any;
                else
                    level = NetworkSessionComponent.Level.Level_1;
            else if (level == NetworkSessionComponent.Level.Level_1)
                level = NetworkSessionComponent.Level.shipMap;
            else if (level == NetworkSessionComponent.Level.Any)
                level = NetworkSessionComponent.Level.shipMap;
        }

        string GetLevel()
        {
           
            switch (level)
            {
                case NetworkSessionComponent.Level.shipMap:
                    return Resources.Level_ShipMap;
                case NetworkSessionComponent.Level.Level_1:
                    return Resources.Level_1;
                case NetworkSessionComponent.Level.Any:
                    return Resources.Any;
                default:
                    throw new Exception("Level Not found");
            }
        }

        void GameModeMenuEntrySelected(object sender, EventArgs e)
        {
            ChangeGameModeType();
        }
        void ChangeGameModeType()
        {
            if (gameModeType == NetworkSessionComponent.GameMode.Any)
                gameModeType = NetworkSessionComponent.GameMode.DeathMatch;
            else if (gameModeType == NetworkSessionComponent.GameMode.DeathMatch)
                gameModeType = NetworkSessionComponent.GameMode.TeamDeathmatch;
            else if (gameModeType == NetworkSessionComponent.GameMode.TeamDeathmatch)
                gameModeType = NetworkSessionComponent.GameMode.CaptureTheFlag;
            else if (gameModeType == NetworkSessionComponent.GameMode.CaptureTheFlag)
                if (createSession)
                    gameModeType = NetworkSessionComponent.GameMode.DeathMatch;
                else
                    gameModeType = NetworkSessionComponent.GameMode.Any;
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
                case NetworkSessionComponent.GameMode.Any:
                    return Resources.Any;
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
            if (weaponsType == NetworkSessionComponent.Weapons.Any)
                weaponsType = NetworkSessionComponent.Weapons.Normal;
            else if (weaponsType == NetworkSessionComponent.Weapons.Normal)
                weaponsType = NetworkSessionComponent.Weapons.Heavy;
            else if (weaponsType == NetworkSessionComponent.Weapons.Heavy)
                weaponsType = NetworkSessionComponent.Weapons.Light;
            else if (weaponsType == NetworkSessionComponent.Weapons.Light)
                if (createSession)
                    weaponsType = NetworkSessionComponent.Weapons.Normal;
                else
                    weaponsType = NetworkSessionComponent.Weapons.Any;
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
                case NetworkSessionComponent.Weapons.Any:
                    return Resources.Any;
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
            if (scoreToWinType == NetworkSessionComponent.ScoreToWin.Any)
                scoreToWinType = NetworkSessionComponent.ScoreToWin.One;
            else if (scoreToWinType == NetworkSessionComponent.ScoreToWin.One)
                scoreToWinType = NetworkSessionComponent.ScoreToWin.Three;
            else if (scoreToWinType == NetworkSessionComponent.ScoreToWin.Three)
                scoreToWinType = NetworkSessionComponent.ScoreToWin.Five;
            else if (scoreToWinType == NetworkSessionComponent.ScoreToWin.Five)
                scoreToWinType = NetworkSessionComponent.ScoreToWin.TwentyFive;
            else if (scoreToWinType == NetworkSessionComponent.ScoreToWin.TwentyFive)
                scoreToWinType = NetworkSessionComponent.ScoreToWin.Fifty;
            else if (scoreToWinType == NetworkSessionComponent.ScoreToWin.Fifty)
                if (createSession)
                    scoreToWinType = NetworkSessionComponent.ScoreToWin.One;
                else
                    scoreToWinType = NetworkSessionComponent.ScoreToWin.Any;

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
                case NetworkSessionComponent.ScoreToWin.Any:
                    return Resources.Any;
                default:
                    throw new NotSupportedException();
            }
        }

        NetworkSessionProperties GetSessionProperties()
        {
            NetworkSessionProperties sessionProperties = new NetworkSessionProperties();

            if (level == NetworkSessionComponent.Level.Any)
                sessionProperties[(int)NetworkSessionComponent.SessionProperties.Level] = null;
            else
                sessionProperties[(int)NetworkSessionComponent.SessionProperties.Level] = (int)level;

            if (gameModeType == NetworkSessionComponent.GameMode.Any)
                sessionProperties[(int)NetworkSessionComponent.SessionProperties.GameMode] = null;
            else
                sessionProperties[(int)NetworkSessionComponent.SessionProperties.GameMode] = (int)gameModeType;

            if (weaponsType == NetworkSessionComponent.Weapons.Any)
                sessionProperties[(int)NetworkSessionComponent.SessionProperties.Weapons] = null;
            else
                sessionProperties[(int)NetworkSessionComponent.SessionProperties.Weapons] = (int)weaponsType;

            if (scoreToWinType == NetworkSessionComponent.ScoreToWin.Any)
                sessionProperties[(int)NetworkSessionComponent.SessionProperties.ScoreToWin] = null;
            else
                sessionProperties[(int)NetworkSessionComponent.SessionProperties.ScoreToWin] = (int)scoreToWinType;

            if (noOfBots == NetworkSessionComponent.NoOfBots.Any)
                sessionProperties[(int)NetworkSessionComponent.SessionProperties.NoOfBots] = null;
            else
                sessionProperties[(int)NetworkSessionComponent.SessionProperties.NoOfBots] = (int)noOfBots;

            return sessionProperties;
        }



        /// <summary>
        /// Event handler for when the Create Session menu entry is selected.
        /// </summary>
        void CreateSessionMenuEntrySelected(object sender, EventArgs e)
        {
            NetworkSessionProperties searchProperties = GetSessionProperties();
            try
            {
                IAsyncResult asyncResult = networkInterface.CreateNetwork(ScreenManager.Game, sessionType, NetworkSessionComponent.MaxLocalGamers, NetworkSessionComponent.MaxGamers, 0, searchProperties, true, true);

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
                ScreenManager.AddScreen(new LobbyScreen(networkHelper.NetworkGameSession, audioManager //audioHelper
                    , true));
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

                IAsyncResult asyncResult = networkInterface.JoinNetwork(ScreenManager.Game, sessionType, NetworkSessionComponent.MaxLocalGamers, searchProperties);
                // Begin an asynchronous find network sessions operation.


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
                    ScreenManager.AddScreen(new JoinSessionScreen(availableSessions, audioManager));//audioHelper
                    // , mystery));
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

            Vector2 position = new Vector2(400, 387);

            spriteBatch.Begin();
            spriteBatch.DrawString(font, GetLevel(), position, Color.Yellow);
            position.Y += 27;
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
