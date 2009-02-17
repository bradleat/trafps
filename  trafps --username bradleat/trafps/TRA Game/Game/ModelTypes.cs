using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
            shipMap
        }
        public static string[] levelModelFileName = { "ship_map" };

    }
}
