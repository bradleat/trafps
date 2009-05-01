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

        AudioManager audioManager;

        NetworkHelper networkHelper;
        NetworkInterface networkInterface;

        #endregion

        #region Initialization


        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public CreateOrFindSessionScreen(ScreenManager screenManager, NetworkSessionType sessionType, AudioManager audioManager)//Audio audioHelper, Cue mystery)
            : base(GetMenuTitle(sessionType), false)
        {

            networkHelper = new NetworkHelper();
            networkInterface = new NetworkInterface();
            networkInterface.InitNetwork(screenManager.Game);
            this.audioManager = audioManager;
            //this.audioHelper = audioHelper;
            //this.mystery = mystery;
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
            ScreenManager.AddScreen(new SessionPropertiesScreen(ScreenManager, sessionType,
                 true, networkHelper));

        }

        /// <summary>
        /// Event handler for when the Find Sessions menu entry is selected.
        /// </summary>
        void FindSessionsMenuEntrySelected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new SessionPropertiesScreen(ScreenManager, sessionType,
                 false, networkHelper));

        }
        #endregion
    }
}
