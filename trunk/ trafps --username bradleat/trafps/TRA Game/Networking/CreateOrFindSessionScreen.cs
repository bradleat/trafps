#region File Description
//-----------------------------------------------------------------------------
// CreateOrFindSessionScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Audio;

using EGGEngine.Audio;
using EGGEngine.Networking;
#endregion

namespace TRA_Game
{
    /// <summary>
    /// This menu screen lets the user choose whether to create a new
    /// network session, or search for an existing session to join.
    /// </summary>
    class CreateOrFindSessionScreen : MenuScreen
    {
        #region Fields

        NetworkSessionType sessionType;
        Audio audioHelper;
        Cue mystery;

        NetworkHelper networkHelper;
        NetworkInterface networkInterface;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public CreateOrFindSessionScreen(ScreenManager screenManager,NetworkSessionType sessionType, Audio audioHelper, Cue mystery)
            : base(GetMenuTitle(sessionType), false)
        {
            
            networkHelper = new NetworkHelper();
            networkInterface = new NetworkInterface();
            networkInterface.InitNetwork(screenManager.Game);
            this.audioHelper = audioHelper;
            this.mystery = mystery;
            this.sessionType = sessionType;

            // Create our menu entries.
            MenuEntry createSessionMenuEntry = new MenuEntry(Resources.CreateSession);
            MenuEntry findSessionsMenuEntry = new MenuEntry(Resources.FindSessions);
            MenuEntry backMenuEntry = new MenuEntry(Resources.Back);

            // Hook up menu event handlers.
            createSessionMenuEntry.Selected += CreateSessionMenuEntrySelected;
            findSessionsMenuEntry.Selected += FindSessionsMenuEntrySelected;
            backMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(createSessionMenuEntry);
            MenuEntries.Add(findSessionsMenuEntry);
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

                default:
                    throw new NotSupportedException();
            }
        }


        #endregion

        #region Event Handlers


        /// <summary>
        /// Event handler for when the Create Session menu entry is selected.
        /// </summary>
        void CreateSessionMenuEntrySelected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new SessionPropertiesScreen(ScreenManager,sessionType,audioHelper, mystery, true, networkHelper));
            /*
            try
            {
                IAsyncResult asyncResult = networkInterface.CreateNetwork(ScreenManager.Game, sessionType, NetworkSessionComponent.MaxLocalGamers, NetworkSessionComponent.MaxGamers, 0, null, true, true);
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
            }*/
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

                // Create a component that will manage the session we just created.
                NetworkSessionComponent.Create(ScreenManager, networkHelper.NetworkGameSession);

                // Go to the lobby screen.
                ScreenManager.AddScreen(new LobbyScreen(networkHelper.NetworkGameSession, audioHelper,true));
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
        void FindSessionsMenuEntrySelected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new SessionPropertiesScreen(ScreenManager,sessionType,audioHelper, mystery, false, networkHelper));
            /*
            try
            {

                IAsyncResult asyncResult = networkInterface.JoinNetwork(ScreenManager.Game, sessionType, NetworkSessionComponent.MaxLocalGamers, null);
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
            }*/
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
    }
}
