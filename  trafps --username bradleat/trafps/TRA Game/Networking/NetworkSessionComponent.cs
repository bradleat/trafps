using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework;

using EGGEngine.Audio;

namespace TRA_Game
{
    /// <summary>
    /// Component in charge of owning and updating the current NetworkSession object.
    /// This is responsible for calling NetworkSession.Update at regular intervals,
    /// and also exposes the NetworkSession as a game service which can easily be
    /// looked up by any other code that needs to access it.
    /// </summary>
    class NetworkSessionComponent : GameComponent
    {
        #region Fields

        public const int MaxGamers = 16;
        public const int MaxLocalGamers = 4;

        public enum SessionProperties
        {
            GameMode, Weapons, ScoreToWin
        }
        public enum GameMode
        {
            DeathMatch, TeamDeathmatch, CaptureTheFlag
        }
        public enum Weapons
        {
            Light,Normal,Heavy
        }
        public enum ScoreToWin
        {
            One, Three, Five, TwentyFive, Fifty
        }

        ScreenManager screenManager;

        NetworkSession networkSession;
        IMessageDisplay messageDisplay;

        bool notifyWhenPlayersJoinOrLeave;
        bool hostChanged;

        string sessionEndMessage;

        #endregion

        #region Initialization


        /// <summary>
        /// The constructor is private: external callers should use the Create method.
        /// </summary>
        NetworkSessionComponent(ScreenManager screenManager,
                                NetworkSession networkSession)
            : base(screenManager.Game)
        {
            this.screenManager = screenManager;
            this.networkSession = networkSession;
            

            // Hook up our session event handlers.
            networkSession.GamerJoined += GamerJoined;
            networkSession.GamerLeft += GamerLeft;
            networkSession.SessionEnded += NetworkSessionEnded;
            networkSession.HostChanged += HostChanged;
        }


        /// <summary>
        /// Creates a new NetworkSessionComponent.
        /// </summary>
        public static void Create(ScreenManager screenManager,
                                  NetworkSession networkSession)
        {
            Game game = screenManager.Game;

            // Register this network session as a service.
            game.Services.AddService(typeof(NetworkSession), networkSession);

            // Create a NetworkSessionComponent, and add it to the Game.
            game.Components.Add(new NetworkSessionComponent(screenManager,
                                                            networkSession));
        }


        /// <summary>
        /// Initializes the component.
        /// </summary>
        public override void Initialize()
        {
            base.Initialize();

            // Look up the IMessageDisplay service, which will
            // be used to report gamer join/leave notifications.
            messageDisplay = (IMessageDisplay)Game.Services.GetService(
                                                              typeof(IMessageDisplay));

            if (messageDisplay != null)
                notifyWhenPlayersJoinOrLeave = true;
        }


        #endregion

        #region Update


        /// <summary>
        /// Updates the network session.
        /// </summary>
        public override void Update(GameTime gameTime)
        {
            if (networkSession == null)
                return;

            try
            {
                networkSession.Update();
                // Has the session ended?
                if (networkSession.SessionState == NetworkSessionState.Ended)
                {
                    LeaveSession();
                }
            }
            catch (NetworkException exception)
            {
                // Handle any errors from the network session update.
                Trace.WriteLine(string.Format("NetworkSession.Update threw {0}: {1}",
                                              exception, exception.Message));

                sessionEndMessage = Resources.ErrorNetwork;

                LeaveSession();
            }
        }


        #endregion

        #region Event Handlers


        /// <summary>
        /// Event handler called when a gamer joins the session.
        /// Displays a notification message.
        /// </summary>
        void GamerJoined(object sender, GamerJoinedEventArgs e)
        {
            if (notifyWhenPlayersJoinOrLeave)
            {
                messageDisplay.ShowMessage(Resources.MessageGamerJoined,
                                           e.Gamer.Gamertag);
            }
        }


        /// <summary>
        /// Event handler called when a gamer leaves the session.
        /// Displays a notification message.
        /// </summary>
        void GamerLeft(object sender, GamerLeftEventArgs e)
        {
            if (notifyWhenPlayersJoinOrLeave)
            {
                messageDisplay.ShowMessage(Resources.MessageGamerLeft,
                                           e.Gamer.Gamertag);
            }
        }

        void HostChanged(object sender, HostChangedEventArgs e)
        {
            if (notifyWhenPlayersJoinOrLeave)
            {
                if (networkSession.IsHost)
                    messageDisplay.ShowMessage(Resources.MessageYouAreHost, null);
                else
                    messageDisplay.ShowMessage(Resources.MessageNewHost, e.NewHost.Gamertag);
                
            }
        }


        /// <summary>
        /// Event handler called when the network session ends.
        /// Stores the end reason, so this can later be displayed to the user.
        /// </summary>
        void NetworkSessionEnded(object sender, NetworkSessionEndedEventArgs e)
        {
            switch (e.EndReason)
            {
                case NetworkSessionEndReason.ClientSignedOut:
                    sessionEndMessage = null;
                    break;

                case NetworkSessionEndReason.HostEndedSession:
                    sessionEndMessage = Resources.ErrorHostEndedSession;
                    break;

                case NetworkSessionEndReason.RemovedByHost:
                    sessionEndMessage = Resources.ErrorRemovedByHost;
                    break;

                case NetworkSessionEndReason.Disconnected:
                default:
                    sessionEndMessage = Resources.ErrorDisconnected;
                    break;
            }

            notifyWhenPlayersJoinOrLeave = false;
        }


        #endregion

        #region Methods


        /// <summary>
        /// Public method called when the user wants to leave the network session.
        /// Displays a confirmation message box, then disposes the session, removes
        /// the NetworkSessionComponent, and returns them to the main menu screen.
        /// </summary>
        public static void LeaveSession(ScreenManager screenManager)
        {
            // Search through Game.Components to find the NetworkSessionComponent.
            foreach (IGameComponent component in screenManager.Game.Components)
            {
                NetworkSessionComponent self = component as NetworkSessionComponent;

                if (self != null)
                {
                    // Display a message box to confirm the user really wants to leave.
                    string message;

                    if (self.networkSession.IsHost)
                        message = Resources.ConfirmEndSession;
                    else
                        message = Resources.ConfirmLeaveSession;

                    MessageBoxScreen confirmMessageBox = new MessageBoxScreen(message);

                    // Hook the messge box ok event to actually leave the session.
                    confirmMessageBox.Accepted += delegate
                    {
                        self.LeaveSession();
                    };

                    screenManager.AddScreen(confirmMessageBox);

                    break;
                }
            }
        }
        public static void LeaveSessionFromGame(ScreenManager screenManager, Audio audioHelper)
        {
            // Search through Game.Components to find the NetworkSessionComponent.
            foreach (IGameComponent component in screenManager.Game.Components)
            {
                NetworkSessionComponent self = component as NetworkSessionComponent;

                if (self != null)
                {
                    // Display a message box to confirm the user really wants to leave.
                    string message;

                    if (self.networkSession.IsHost)
                        message = Resources.ConfirmEndSession;
                    else
                        message = Resources.ConfirmLeaveSession;

                    MessageBoxScreen confirmMessageBox = new MessageBoxScreen(message);

                    // Hook the messge box ok event to actually leave the session.
                    confirmMessageBox.Accepted += delegate
                    {
                        self.LeaveSessionFromGame(audioHelper);
                    };

                    screenManager.AddScreen(confirmMessageBox);

                    break;
                }
            }
        }

        void LeaveSessionFromGame(Audio audioHelper)
        {
            // Remove the NetworkSessionComponent.
            Game.Components.Remove(this);

            // Remove the NetworkSession service.
            Game.Services.RemoveService(typeof(NetworkSession));

            // Dispose the NetworkSession.
            networkSession.Dispose();
            networkSession = null;

            // If we have a sessionEndMessage string explaining why the session has
            // ended (maybe this was a network disconnect, or perhaps the host kicked
            // us out?) create a message box to display this reason to the user.
            MessageBoxScreen messageBox;

            if (!string.IsNullOrEmpty(sessionEndMessage))
                messageBox = new MessageBoxScreen(sessionEndMessage, false);
            else
                messageBox = null;

            // At this point we normally want to return the user all the way to the
            // main menu screen. But what if they just joined a session? In that case
            // they went through this flow of screens:
            //
            //  - MainMenuScreen
            //  - CreateOrFindSessionsScreen
            //  - JoinSessionScreen (if joining, skipped if creating a new session)
            //  - LobbyScreeen
            //
            // If we have these previous screens on the history stack, and the user
            // backs out of the LobbyScreen, the right thing is just to pop off the
            // LobbyScreen and JoinSessionScreen, returning them to the
            // CreateOrFindSessionsScreen (we cannot just back up to the
            // JoinSessionScreen, because it contains search results that will no
            // longer be valid). But if the user is in gameplay, or has been in
            // gameplay and then returned to the lobby, the screen stack will have
            // been emptied.
            //
            // To do the right thing in both cases, we scan through the screen history
            // stack looking for a CreateOrFindSessionScreen. If we find one, we pop
            // any subsequent screens so as to return back to it, while if we don't
            // find it, we just reset everything and go back to the main menu.

            GameScreen[] screens = screenManager.GetScreens();

            // Look for the CreateOrFindSessionsScreen.
            for (int i = 0; i < screens.Length; i++)
            {
                if (screens[i] is CreateOrFindSessionScreen)
                {
                    // If we found one, pop everything since then to return back to it.
                    for (int j = i + 1; j < screens.Length; j++)
                        screens[j].ExitScreen();

                    // Display the why-did-the-session-end message box.
                    if (messageBox != null)
                        screenManager.AddScreen(messageBox);

                    return;
                }
            }

            // If we didn't find a CreateOrFindSessionsScreen, reset everything and
            // go back to the main menu. The why-did-the-session-end message box
            // will be displayed after the loading screen has completed.
            LoadingScreen.Load(screenManager, false, new BackgroundScreen(false),
                                                     new MainMenuScreen(true, audioHelper),
                                                     messageBox);
        }

        /// <summary>
        /// Internal method for leaving the network session. This disposes the 
        /// session, removes the NetworkSessionComponent, and returns the user
        /// to the main menu screen.
        /// </summary>
        void LeaveSession()
        {
            // Remove the NetworkSessionComponent.
            Game.Components.Remove(this);

            // Remove the NetworkSession service.
            Game.Services.RemoveService(typeof(NetworkSession));

            // Dispose the NetworkSession.
            networkSession.Dispose();
            networkSession = null;

            // If we have a sessionEndMessage string explaining why the session has
            // ended (maybe this was a network disconnect, or perhaps the host kicked
            // us out?) create a message box to display this reason to the user.
            MessageBoxScreen messageBox;

            if (!string.IsNullOrEmpty(sessionEndMessage))
                messageBox = new MessageBoxScreen(sessionEndMessage, false);
            else
                messageBox = null;

            // At this point we normally want to return the user all the way to the
            // main menu screen. But what if they just joined a session? In that case
            // they went through this flow of screens:
            //
            //  - MainMenuScreen
            //  - CreateOrFindSessionsScreen
            //  - JoinSessionScreen (if joining, skipped if creating a new session)
            //  - LobbyScreeen
            //
            // If we have these previous screens on the history stack, and the user
            // backs out of the LobbyScreen, the right thing is just to pop off the
            // LobbyScreen and JoinSessionScreen, returning them to the
            // CreateOrFindSessionsScreen (we cannot just back up to the
            // JoinSessionScreen, because it contains search results that will no
            // longer be valid). But if the user is in gameplay, or has been in
            // gameplay and then returned to the lobby, the screen stack will have
            // been emptied.
            //
            // To do the right thing in both cases, we scan through the screen history
            // stack looking for a CreateOrFindSessionScreen. If we find one, we pop
            // any subsequent screens so as to return back to it, while if we don't
            // find it, we just reset everything and go back to the main menu.

            GameScreen[] screens = screenManager.GetScreens();

            // Look for the CreateOrFindSessionsScreen.
            for (int i = 0; i < screens.Length; i++)
            {
                if (screens[i] is CreateOrFindSessionScreen)
                {
                    // If we found one, pop everything since then to return back to it.
                    for (int j = i + 1; j < screens.Length; j++)
                        screens[j].ExitScreen();

                    // Display the why-did-the-session-end message box.
                    if (messageBox != null)
                        screenManager.AddScreen(messageBox);

                    return;
                }
            }

            // If we didn't find a CreateOrFindSessionsScreen, reset everything and
            // go back to the main menu. The why-did-the-session-end message box
            // will be displayed after the loading screen has completed.
            LoadingScreen.Load(screenManager, false, new BackgroundScreen(false),
                                                     new MainMenuScreen(false,null),
                                                     messageBox);
        }


        #endregion
    }
}
