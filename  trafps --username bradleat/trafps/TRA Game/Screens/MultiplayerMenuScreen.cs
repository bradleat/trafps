using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Audio;

using EGGEngine.Audio;

namespace TRA_Game
{
    class MultiplayerMenuScreen : MenuScreen
    {
        // We need this to keep the sound loop going
        Audio audioHelper;
        Cue mystery;

        // <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MultiplayerMenuScreen(Audio audioHelper, Cue mystery)
            : base(Resources.MultiplayerMenu)
        {
            this.audioHelper = audioHelper;
            this.mystery = mystery;
            // Create our menu entries.
            MenuEntry liveMenuEntry = new MenuEntry(Resources.PlayerMatch);
            MenuEntry systemlinkMenuEntry = new MenuEntry(Resources.SystemLink);
            MenuEntry backMenuEntry = new MenuEntry(Resources.Back);
            

            // Hook up menu event handlers.
            liveMenuEntry.Selected += LiveMenuEntrySelected;
            systemlinkMenuEntry.Selected += SystemLinkMenuEntrySelected;
            backMenuEntry.Selected += BackMenuEntrySelected;

            // Add entries to the menu.
            MenuEntries.Add(liveMenuEntry);
            MenuEntries.Add(systemlinkMenuEntry);
            MenuEntries.Add(backMenuEntry);

            this.audioHelper.Update();
        }

        /// <summary>
        /// 
        /// </summary>
        void BackMenuEntrySelected(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, new BackgroundScreen(false),
                                                     new MainMenuScreen(true, this.audioHelper));
        }
        /// <summary>
        /// Event handler for when the Live menu entry is selected.
        /// </summary>
        void LiveMenuEntrySelected(object sender, EventArgs e)
        {
            CreateOrFindSession(NetworkSessionType.PlayerMatch);
        }


        /// <summary>
        /// Event handler for when the System Link menu entry is selected.
        /// </summary>
        void SystemLinkMenuEntrySelected(object sender, EventArgs e)
        {
            CreateOrFindSession(NetworkSessionType.SystemLink);
        }

        
        /// <summary>
        /// Helper method shared by the Live and System Link menu event handlers.
        /// </summary>
        void CreateOrFindSession(NetworkSessionType sessionType)
        {
            // First, we need to make sure a suitable gamer profile is signed in.
            ProfileSignInScreen profileSignIn = new ProfileSignInScreen(sessionType);

            // Hook up an event so once the ProfileSignInScreen is happy,
            // it will activate the CreateOrFindSessionScreen.
            profileSignIn.ProfileSignedIn += delegate
            {
                ScreenManager.AddScreen(new CreateOrFindSessionScreen(ScreenManager, sessionType, this.audioHelper, this.mystery));
            };

            // Activate the ProfileSignInScreen.
            ScreenManager.AddScreen(profileSignIn);
        }


        /// <summary>
        /// When the user cancels the main menu, ask if they want to exit the sample.
        /// </summary>
        protected override void OnCancel()
        {
            MessageBoxScreen confirmExitMessageBox =
                                    new MessageBoxScreen(Resources.ConfirmExitGame);

            confirmExitMessageBox.Accepted += ConfirmExitMessageBoxAccepted;

            ScreenManager.AddScreen(confirmExitMessageBox);
        }


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to exit" message box.
        /// </summary>
        void ConfirmExitMessageBoxAccepted(object sender, EventArgs e)
        {
            ScreenManager.Game.Exit();
        }

    }
}
