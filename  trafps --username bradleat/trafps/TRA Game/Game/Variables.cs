using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TRA_Game
{
    class Variables
    {
        private NetworkSessionComponent.SessionProperties sessionProperties;
        private NetworkSessionComponent.GameMode gameMode;
        private NetworkSessionComponent.Weapons weaponsType;
        private NetworkSessionComponent.ScoreToWin scoreToWin;
        private NetworkSessionComponent.NoOfBots noOfBots;

        public NetworkSessionComponent.SessionProperties SessionProperties
        {
            get { return sessionProperties; }
            set { sessionProperties = value; }
        }
        public NetworkSessionComponent.GameMode GameMode
        {
            get { return gameMode; }
            set { gameMode = value; }
        }
        public NetworkSessionComponent.Weapons WeaponsType
        {
            get { return weaponsType; }
            set { weaponsType = value; }
        }
        public NetworkSessionComponent.ScoreToWin ScoreToWin
        {
            get { return scoreToWin; }
            set { scoreToWin = value; }
        }
        public NetworkSessionComponent.NoOfBots NoOfBots
        {
            get { return noOfBots; }
            set { noOfBots = value; }
        }

    }
}
