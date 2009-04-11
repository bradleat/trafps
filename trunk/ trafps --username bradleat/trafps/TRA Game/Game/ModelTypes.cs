using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Xna.Framework;

namespace TRA_Game
{
    public static class ModelTypes
    {
        // Player
        // ---------------------------------------------------------------------------
        public enum PlayerType
        {
            TankGirl
        }
        public static string[] PlayerModelFileName = { "Models//model" };
        public static int[] PlayerHealth = { 100 };
        public static int[] ShieldHealth = { 100 };

        // Player Weapons
        // ---------------------------------------------------------------------------
        public enum WeaponType
        {
            Pistol,
            SniperRifle,
            BullpupAssaultRifle,
            Grenade
        }
        public static string[] WeaponModelFileName = { "Models//pistol(1)",
                                                       "Models//sniper_rifle",
                                                       "Models//bullpup",
                                                       "Models//grenade"};
        public static int[] BulletDamage = { 10,
                                             50,
                                             20,
                                             100 };
        public static int[] BulletsCount = { 20,
                                             20,
                                             100,
                                             2   };
        // Levels
        // ---------------------------------------------------------------------------
        public enum Levels
        {
            shipMap,
            Level_1
        }
        public static string[] levelModelFileName = { "ship_map",
                                                      "level_1"   };

        public static Vector3[] BlueTeamSpawnPoint = { new Vector3((float)52.238, (float)-4.86833572, (float)-5.327177),
                                                         new Vector3(0,-3,0)};
        public static Vector3[] RedTeamSpawnPoint = { new Vector3((float)-32.7520447, (float)-4.86833572, (float)-5.2758193),
                                                        new Vector3(0,-3,0)};

        public static float[] BlueTeamSpawnRotation = {(float)-1.56999528,
                                                          0f               };
        public static float[] RedTeamSpawnRotation = {(float)-4.729999,
                                                         0f            };              

    }
}
