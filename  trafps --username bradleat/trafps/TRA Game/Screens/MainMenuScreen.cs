#region File Description
//-----------------------------------------------------------------------------
// MainMenuScreen.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

#region Using Statements
using System;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Audio;

using EGGEngine.Audio;
using EGGEngine.Debug;
#endregion

namespace TRA_Game
{
    /// <summary>
    /// The main menu screen is the first thing displayed when the game starts up.
    /// </summary>
    class MainMenuScreen : MenuScreen
    {
        Audio audioHelper;
        Cue mystery;
        Cue famas_1;
        bool audio_on = false;
        bool gamePlayed;

        

        /// <summary>
        /// Constructor fills in the menu contents.
        /// </summary>
        public MainMenuScreen(bool audio_on, Audio audioHelper)
            : base(Resources.MainMenu)
        {
            
            // Create our menu entries.
            MenuEntry trainingMenuEntry = new MenuEntry(Resources.Training);
            MenuEntry multiplayerMenuEntry = new MenuEntry(Resources.Multiplayer);
            MenuEntry theatherMenuEntry = new MenuEntry(Resources.Theather);
            MenuEntry assemblerMenuEntry = new MenuEntry(Resources.Assembler);
            MenuEntry optionsMenuEntry = new MenuEntry(Resources.Options);
            MenuEntry exitMenuEntry = new MenuEntry(Resources.Exit);

            // Hook up menu event handlers.
            trainingMenuEntry.Selected += TrainingMenuEntrySelected;
            multiplayerMenuEntry.Selected += MultiplayerMenuEntrySelected;
            theatherMenuEntry.Selected += TheatherMenuEntrySelected;
            assemblerMenuEntry.Selected += AssemblerMenuEntrySelected;
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            exitMenuEntry.Selected += OnCancel;

            // Add entries to the menu.
            MenuEntries.Add(trainingMenuEntry);
            MenuEntries.Add(multiplayerMenuEntry);
            MenuEntries.Add(theatherMenuEntry);
            MenuEntries.Add(assemblerMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
            MenuEntries.Add(exitMenuEntry);

            if (audioHelper == null)
                this.audioHelper = new Audio("Content\\TRA_Game.xgs");
            else
                this.audioHelper = audioHelper;
            if (audio_on == false)
            {
                mystery = this.audioHelper.GetCue("mystery");
                famas_1 = this.audioHelper.GetCue("famas-1");
                this.audioHelper.Play(mystery, false, new AudioListener(), new AudioEmitter());
            }
            else
            {
                mystery = this.audioHelper.GetCue("mystery");
                famas_1 = this.audioHelper.GetCue("famas-1");
            }

            this.audioHelper.Update();
        }

        


        /// <summary>
        /// Event handler for when the Single Player menu entry is selected.
        /// </summary>
        void OptionsMenuEntrySelected(object sender, EventArgs e)
        {
            //LoadingScreen.Load(ScreenManager, true, new GameplayScreen(null));
        }

        /// <summary>
        /// Event handler for when the Single Player menu entry is selected.
        /// </summary>
        void AssemblerMenuEntrySelected(object sender, EventArgs e)
        {
            //LoadingScreen.Load(ScreenManager, true, new GameplayScreen(null));
        }

        /// <summary>
        /// Event handler for when the Single Player menu entry is selected.
        /// </summary>
        void TheatherMenuEntrySelected(object sender, EventArgs e)
        {
            //LoadingScreen.Load(ScreenManager, true, new GameplayScreen(null));
        }

        /// <summary>
        /// Event handler for when the Single Player menu entry is selected.
        /// </summary>
        void MultiplayerMenuEntrySelected(object sender, EventArgs e)
        {
            LoadingScreen.Load(ScreenManager, false, new BackgroundScreen(true), new MultiplayerMenuScreen(this.audioHelper, mystery));
        }

        /// <summary>
        /// Event handler for when the Single Player menu entry is selected.
        /// </summary>
        void TrainingMenuEntrySelected(object sender, EventArgs e)
        {
            ScreenManager.AddScreen(new SessionPropertiesScreen(ScreenManager, NetworkSessionType.Local,audioHelper, mystery,false,null));
            /*audioHelper.Stop(mystery);
            audioHelper.Play(famas_1, false, new AudioListener(), new AudioEmitter());
            LoadingScreen.Load(ScreenManager, true, new GameplayScreen(null));*/
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
