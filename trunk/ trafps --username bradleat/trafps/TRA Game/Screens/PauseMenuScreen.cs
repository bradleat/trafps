#region File Description
//-----------------------------------------------------------------------------
// PauseMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Net;
#endregion

namespace TRA_Game
{
    /// <summary>
    /// The pause menu comes up over the top of the game,
    /// giving the player options to resume or quit.
    /// </summary>
    class PauseMenuScreen : MenuScreen
    {
        

        NetworkSession networkSession;
        

        /// <summary>
        /// Constructor.
        /// </summary>
        public PauseMenuScreen(NetworkSession networkSession)
            : base(Resources.Paused, false)
        {
            this.networkSession = networkSession;

            // Flag that there is no need for the game to transition
            // off when the pause menu is on top of it.
            IsPopup = true;

            // Add the Resume Game menu entry.
            MenuEntry resumeGameMenuEntry = new MenuEntry(Resources.ResumeGame);
            resumeGameMenuEntry.Selected += OnCancel;
            MenuEntry optionsMenuEntry = new MenuEntry(Resources.Options);
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            MenuEntries.Add(resumeGameMenuEntry);
            MenuEntries.Add(optionsMenuEntry);

            if (networkSession == null)
            {
                // If this is a single player game, add the Quit menu entry.
                MenuEntry quitGameMenuEntry = new MenuEntry(Resources.QuitGame);
                quitGameMenuEntry.Selected += QuitGameMenuEntrySelected;
                MenuEntries.Add(quitGameMenuEntry);
            }
            else
            {
                // If we are hosting a network game, add the Return to Lobby menu entry.
                if (networkSession.IsHost)
                {
                    MenuEntry lobbyMenuEntry = new MenuEntry(Resources.ReturnToLobby);
                    lobbyMenuEntry.Selected += ReturnToLobbyMenuEntrySelected;
                    MenuEntries.Add(lobbyMenuEntry);
                }

                // Add the End/Leave Session menu entry.
                string leaveEntryText = networkSession.IsHost ? Resources.EndSession :
                                                                Resources.LeaveSession;

                MenuEntry leaveSessionMenuEntry = new MenuEntry(leaveEntryText);
                leaveSessionMenuEntry.Selected += LeaveSessionMenuEntrySelected;
                MenuEntries.Add(leaveSessionMenuEntry);
            }
        }


        

        

        /// <summary>
        /// Event handler for when the Quit Game menu entry is selected.
        /// </summary>
        void QuitGameMenuEntrySelected(object sender, EventArgs e)
        {
            MessageBoxScreen confirmQuitMessageBox =
                                    new MessageBoxScreen(Resources.ConfirmQuitGame);

            confirmQuitMessageBox.Accepted += ConfirmQuitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmQuitMessageBox);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to quit" message box. This uses the loading screen to
        /// transition from the game back to the main menu screen.
        /// </summary>
        void ConfirmQuitMessageBoxAccepted(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, new BackgroundScreen(false),
                                                     new MainMenuScreen(false, null));
        }


        /// <summary>
        /// Event handler for when the Return to Lobby menu entry is selected.
        /// </summary>
        void ReturnToLobbyMenuEntrySelected(object sender, EventArgs e)
        {
            if (networkSession.SessionState == NetworkSessionState.Playing)
            {
                networkSession.EndGame();
            }
        }


        /// <summary>
        /// Event handler for when the End/Leave Session menu entry is selected.
        /// </summary>
        void LeaveSessionMenuEntrySelected(object sender, EventArgs e)
        {
            NetworkSessionComponent.LeaveSession(ScreenManager);
        }


        

        


        /// <summary>
        /// Draws the pause menu screen. This darkens down the gameplay screen
        /// that is underneath us, and then chains to the base MenuScreen.Draw.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

            base.Draw(gameTime);
        }


        
    }
}
