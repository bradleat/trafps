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

namespace TRA_Game
{
    class SessionPropertiesPopUpScreen : MenuScreen
    {

        NetworkSession networkSession;

        NetworkSessionComponent.GameMode gameModeType;
        NetworkSessionComponent.Weapons weaponsType;
        NetworkSessionComponent.ScoreToWin scoreToWinType;
        NetworkSessionComponent.NoOfBots noOfBots;

        /// <summary>
        /// Constructor.
        /// </summary>
        public SessionPropertiesPopUpScreen(NetworkSession networkSession)
            : base(Resources.Paused, false)
        {
            this.networkSession = networkSession;
            GetVariables();

            // Flag that there is no need for the game to transition
            // off when the pause menu is on top of it.
            IsPopup = true;

            MenuEntry gameModeMenuEntry = new MenuEntry(Resources.GameMode);
            MenuEntry weaponsMenuEntry = new MenuEntry(Resources.Weapons);
            MenuEntry scoreToWinMenuEntry = new MenuEntry(Resources.ScoreToWin);
            MenuEntry noofbotsMenuEntry = new MenuEntry(Resources.NumberOfBots);
            MenuEntry lobbyMenuEntry = new MenuEntry(Resources.ReturnToLobby);

            gameModeMenuEntry.Selected += GameModeMenuEntrySelected;
            weaponsMenuEntry.Selected += WeaponsMenuEntrySelected;
            scoreToWinMenuEntry.Selected += ScoreToWinMenuEntrySelected;
            noofbotsMenuEntry.Selected += NoOfBotsMenuEntrySelected;
            lobbyMenuEntry.Selected += ReturnToLobbyMenuEntrySelected;

            MenuEntries.Add(gameModeMenuEntry);
            MenuEntries.Add(weaponsMenuEntry);
            MenuEntries.Add(scoreToWinMenuEntry);
            MenuEntries.Add(noofbotsMenuEntry);
            //MenuEntries.Add(lobbyMenuEntry);

            // Add the Resume Game menu entry.
            MenuEntry resumeGameMenuEntry = new MenuEntry(Resources.ResumeGame);
            resumeGameMenuEntry.Selected += OnCancel;
            MenuEntry optionsMenuEntry = new MenuEntry(Resources.Options);
            optionsMenuEntry.Selected += OptionsMenuEntrySelected;
            MenuEntries.Add(resumeGameMenuEntry);
            MenuEntries.Add(optionsMenuEntry);
        }

        void GetVariables()
        {
            gameModeType = (NetworkSessionComponent.GameMode)networkSession.SessionProperties[(int)NetworkSessionComponent.SessionProperties.GameMode];
            weaponsType = (NetworkSessionComponent.Weapons)networkSession.SessionProperties[(int)NetworkSessionComponent.SessionProperties.Weapons];
            scoreToWinType = (NetworkSessionComponent.ScoreToWin)networkSession.SessionProperties[(int)NetworkSessionComponent.SessionProperties.ScoreToWin];
            noOfBots = (NetworkSessionComponent.NoOfBots)networkSession.SessionProperties[(int)NetworkSessionComponent.SessionProperties.NoOfBots];
        }
        
        /// <summary>
        /// Event handler for when the Quit Game menu entry is selected.
        /// </summary>
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


        /// <summary>
        /// Event handler for when the user selects ok on the "are you sure
        /// you want to quit" message box. This uses the loading screen to
        /// transition from the game back to the main menu screen.
        /// </summary>
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

       
        void NoOfBotsMenuEntrySelected(object sender, EventArgs e)
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

        /// <summary>
        /// Draws the pause menu screen. This darkens down the gameplay screen
        /// that is underneath us, and then chains to the base MenuScreen.Draw.
        /// </summary>
        public override void Draw(GameTime gameTime)
        {
            ScreenManager.FadeBackBufferToBlack(TransitionAlpha * 2 / 3);

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

        void SetSessionProperties()
        {
            networkSession.SessionProperties[(int)NetworkSessionComponent.SessionProperties.GameMode] = (int)gameModeType;
            networkSession.SessionProperties[(int)NetworkSessionComponent.SessionProperties.Weapons] = (int)weaponsType;
            networkSession.SessionProperties[(int)NetworkSessionComponent.SessionProperties.ScoreToWin] = (int)scoreToWinType;
            networkSession.SessionProperties[(int)NetworkSessionComponent.SessionProperties.NoOfBots] = (int)noOfBots;
        }

        protected override void OnCancel()
        {
            SetSessionProperties();
            
            base.OnCancel();
        }

    }
}

