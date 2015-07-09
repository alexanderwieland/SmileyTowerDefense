using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SmileyTowerDefense
{
    public class Tower : Sprite
    {
        protected int cost; // How much will the tower cost to make
        protected int damage; // The damage done to enemy's

        protected float radius; // How far the tower can shoot

        protected Enemy target;
        public Enemy Target { set { target = value; } get { return target; } }


        protected float bulletTimer; // How long ago was a bullet fired
        protected Texture2D bulletTexture;

        protected List<Bullet> bulletList = new List<Bullet>();
        public List<Bullet> BulletList { set { bulletList = value; } get { return bulletList; } }

        public int Cost
        {
            get { return cost; }
        }
        public int Damage
        {
            get { return damage; }
        }

        public float Radius
        {
            get { return radius; }
        }
       

        public Tower(Texture2D texture, Texture2D bulletTexture, Vector2 position)
            : base(texture, position)
        {
            this.bulletTexture = bulletTexture;
        }

        protected void FaceTarget()
        {
            Vector2 direction = center - target.Center;
            direction.Normalize();

            rotation = (float)Math.Atan2(-direction.X, direction.Y);
        }

        public bool IsInRange(Vector2 position)
        {
            return Vector2.Distance(center, position) <= radius;
        }

        public void GetClosestEnemy(List<Enemy> enemies)
        {
            target = null;
            float smallestRange = radius;

            foreach (Enemy enemy in enemies)
            {
                if (Vector2.Distance(center, enemy.Center) < smallestRange)
                {
                    smallestRange = Vector2.Distance(center, enemy.Center);
                    target = enemy;
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            bulletTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (target != null)
            {
                FaceTarget();

                if (!IsInRange(target.Center) || target.IsDead)
                {
                    target = null;
                    bulletTimer = 0;

                    foreach (Bullet bullet in bulletList)
                    {
                        bullet.Kill();
                    }
                }
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);

            foreach (Bullet bullet in bulletList)
                bullet.Draw(spriteBatch);
        }
    }
}
