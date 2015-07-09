using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SmileyTowerDefense
{
    public class Enemy : Sprite
    {
        private List<Texture2D> textures;

        public delegate void EnemyDiedEventHandler(int bounty);
        public event EnemyDiedEventHandler EnemyDied;

        protected int nr;
        public int NR { set { value = nr; } get { return nr; } }

        private Queue<Vector2> waypoints = new Queue<Vector2>();
        public Queue<Vector2> Waypoints { get { return waypoints; } set { waypoints = value; } }

        protected float startHealth;
        public float StartHealth { set { startHealth = value; } get { return startHealth; } }
        protected float currentHealth;

        protected bool alive = true;
        public bool Alive { set { alive = value; } get { return alive; } }

        protected float speed = 0.5f;
        protected int bountyGiven;

        private Pathfinder pathFinder;

        public float CurrentHealth
        {
            get { return currentHealth; }
            set { currentHealth = value; }
        }

        public bool IsDead
        {
            get { return !alive; }
        }
        public int BountyGiven
        {
            get { return bountyGiven; }
        }

        public float DistanceToDestination
        {
            get { return Vector2.Distance(position, waypoints.Peek()); }
        }
        Player player;
        public Enemy(Vector2 position, float health, int bountyGiven, float speed, Player player, List<Texture2D> textures, Pathfinder pathFinder)
            : base(textures[0], position)
        {
            this.startHealth = health;
            this.currentHealth = startHealth;
            this.player = player;
            this.bountyGiven = bountyGiven;
            this.speed = speed;
            EnemyDied += new EnemyDiedEventHandler(player.RaiseStats);


            this.textures = textures;

            this.pathFinder = pathFinder;
        }


            public bool SetWaypoints()
            {
            // TODOne HAND PATHFINDER REFERENCE ALL THE WAY DOWN TO ENEMYOR AT LEAST SOMETHING ACCESSABLE HERE!!!
            // TODOne set them with pathfinder
            pathFinder.ResetSearchNodes();
            pathFinder.InitializeSearchNodes();
            List<Vector2> path = pathFinder.FindPath(new Point(4, 0), new Point(4, 8));     // FLAGged 

            // if a path is found
            if (path.Count > 0)
            {
                this.waypoints.Enqueue(new Vector2(4, 0)*32);  // TODOne FLAGged
                foreach (Vector2 waypoint in path)
                    this.waypoints.Enqueue(waypoint);

                // this.position = this.waypoints.Dequeue();   // TODOne what do I do here oO? doing it here or when enemy is first placed?

                return true;
            }

            // no path possible
            else
                return false;

            //foreach (Vector2 waypoint in waypoints)
            //    this.waypoints.Enqueue(waypoint);

            //this.position = this.waypoints.Dequeue();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            if (waypoints.Count > 0)
            {
                if (DistanceToDestination < 1f)
                {
                    position = waypoints.Peek();
                    waypoints.Dequeue();
                }

                else
                {
                    Vector2 direction = waypoints.Peek() - position;
                    direction.Normalize();

                    velocity = Vector2.Multiply(direction, speed);

                    position += velocity;
                }
            }

            else
            {
                alive = false;
                player.Lives--;
            }
            if (currentHealth <= 0)
            {
                EnemyDied(bountyGiven);
                alive = false;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (alive)
            {
                float healthPercentage = (float)currentHealth / (float)startHealth;

                if (healthPercentage > 0.75)
                {
                    base.Texture = textures[0];
                }
                else
                {
                    if (healthPercentage > 0.5)
                    {
                        base.Texture = textures[1];
                    }
                    else
                    {
                        if (healthPercentage > 0.25)
                        {
                            base.Texture = textures[2];
                        }
                        else
                        {
                            base.Texture = textures[3];
                        }
                    }
                }

                base.Draw(spriteBatch);
            }
        }
    }
}
