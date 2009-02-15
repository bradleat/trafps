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


namespace EGGEngine
{
    /// <summary>
    /// This is a game component that implements IUpdateable.
    /// </summary>
    public class Weapon : DrawableGameComponent
    {
        private ModelTypes.WeaponType currentWeapon;
        private int maxBullets;
        private int bulletsCount;
        private int bulletDamage;
        private Vector3 firePosition;
        private Vector3 targetDirection;
        private Model model;
        private Game game;

        #region Properties
        public int BulletDamage
        {
            get
            {
                return bulletDamage;
            }
            set
            {
                bulletDamage = value;
            }
        }

        public int MaxBullets
        {
            get
            {
                return maxBullets;
            }
            set
            {
                maxBullets = value;
            }
        }

        public int BulletsCount
        {
            get
            {
                return bulletsCount;
            }
            set
            {
                bulletsCount = value;
            }
        }

        public Vector3 FirePosition
        {
            get
            {
                return firePosition;
            }
        }

        public Vector3 TargetDirection
        {
            get
            {
                return targetDirection;
            }
            set
            {
                targetDirection = value;
            }
        }

        #endregion


        public Weapon(Game game, ModelTypes.WeaponType weaponType)
            : base(game)
        {
            this.game = game;
            this.currentWeapon = weaponType;

            bulletDamage = ModelTypes.BulletDamage[(int)currentWeapon];
            bulletsCount = ModelTypes.BulletsCount[(int)currentWeapon];
            maxBullets = bulletsCount;
        }

        protected override void LoadContent()
        {
            this.model = game.Content.Load<Model>(ModelTypes.WeaponModelFileName[(int)currentWeapon]);
        }

        /// <summary>
        /// Allows the game component to perform any initialization it needs to before starting
        /// to run.  This is where it can query for any required services and load content.
        /// </summary>
        public override void Initialize()
        {
            // TODO: Add your initialization code here

            base.Initialize();
        }

        /// <summary>
        /// Allows the game component to update itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        public override void Update(GameTime gameTime)
        {
            //Update Animations
            // TODO: Add your update code here

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime)
        {
            //Draw the current weapon
            base.Draw(gameTime);
        }
    }
}