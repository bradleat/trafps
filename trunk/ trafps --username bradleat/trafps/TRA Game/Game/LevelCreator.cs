using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Net;
using Microsoft.Xna.Framework.Storage;
using EGGEngine.Rendering;


namespace TRA_Game
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public static class LevelCreator
    {
        public static GameLevel CreateLevel(Game game, NetworkSessionComponent.Level level)
        {
            switch (level)
            {
                case NetworkSessionComponent.Level.shipMap:
                    return CreateShipMapLevel(game);
                case NetworkSessionComponent.Level.Level_1:
                    return CreateLevel_1Level(game);
                default:
                    throw new ArgumentException("Invalid game level");
            }
        }

        private static GameLevel CreateLevel_1Level(Game game)
        {
            ContentManager Content = game.Content;
            GameLevel gameLevel = new GameLevel();
            // Terrain
            gameLevel.level = new Level(game, ModelTypes.levelModelFileName[(int)ModelTypes.Levels.Level_1]);
            // Sky
            gameLevel.sky = Content.Load<Sky>("Models\\sky1");

            gameLevel.world = new EGGEngine.Physics.World(gameLevel.level.Model);
            // Player
            gameLevel.weapon = new Weapon(game, ModelTypes.WeaponType.Pistol);
            //gameLevel.player = new Player(game);//, ModelTypes.PlayerType.TankGirl, new Vector3(0, -3, -5), 0, new Vector3(0, 5, 0), gameLevel.world, gameLevel.weapon);

            return gameLevel;
        }

        private static GameLevel CreateShipMapLevel(Game game)
        {
            ContentManager Content = game.Content;
            GameLevel gameLevel = new GameLevel();
            // Terrain
            gameLevel.level  = new Level(game, ModelTypes.levelModelFileName[(int)ModelTypes.Levels.shipMap]);            
            // Sky
            gameLevel.sky = Content.Load<Sky>("Models\\sky1");

            gameLevel.world = new EGGEngine.Physics.World(gameLevel.level.Model);
            // Player
            gameLevel.weapon = new Weapon(game, ModelTypes.WeaponType.Pistol);
            //gameLevel.player = new Player(game);//, ModelTypes.PlayerType.TankGirl, new Vector3(0, -3, -5), 0, new Vector3(0, 5, 0), gameLevel.world, gameLevel.weapon);
           
            return gameLevel;
        }
    }
}