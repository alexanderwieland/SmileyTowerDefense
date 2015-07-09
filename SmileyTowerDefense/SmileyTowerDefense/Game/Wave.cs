using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SmileyTowerDefense
{
    
    public class Wave
    {  
        private int numOfEnemies; // Number of enemies to spawn
        public int NumOfEnemies { set { numOfEnemies = value; } get { return numOfEnemies; } }
        private int waveNumber; // What wave is this?
        public int WaveNumber { set { waveNumber = value; } get { return waveNumber; } }

        private float spawnTimer = 0; // When should we spawn an enemy
        public float SpawnTimer { set { spawnTimer = value; } get { return spawnTimer; } }
        private int enemiesSpawned = 0; // How mant enemies have spawned
        public int EnemiesSpawned { set { enemiesSpawned = value; } get { return enemiesSpawned; } }

        private bool enemyAtEnd; // Has an enemy reached the end of the path?
        private bool spawningEnemies; // Are we still spawing enemies?

        private Level level; // A reference of the level

        private Texture2D enemyTexture; // A texture for the enemies

        private List<Enemy> enemies = new List<Enemy>(); // List of enemies
        private List<Texture2D> textures; //textures the enemies use
        Player player;
        public bool RoundOver
        {
            get
            {
                return enemies.Count == 0 && enemiesSpawned == numOfEnemies;
            }
        }
        public int RoundNumber
        {
            get { return waveNumber; }
        }

        public bool EnemyAtEnd
        {
            get { return enemyAtEnd; }
            set { enemyAtEnd = value; }
        }
        public List<Enemy> Enemies
        {
            get { return enemies; }
            set { enemies = value; }
        }

        private Pathfinder pathFinder;

        public Wave(int waveNumber, int numOfEnemies,
            Level level, Player player, List<Texture2D> textures, Pathfinder pathFinder)
        {
            this.waveNumber = waveNumber;
            this.numOfEnemies = numOfEnemies;

            this.level = level;
            this.enemyTexture = textures[0];

            this.player = player;

            this.textures = textures;

            this.pathFinder = pathFinder;
        }

        private void AddEnemy()
        {
            Enemy enemy = new Enemy(/*level.Waypoints.Peek()*/ new Vector2(4 * 32f, 0 * 32f/*0f, 0f*/), 100, 10, 0.5f, player, this.textures, pathFinder);       // FLAGged
            // TODOne SetWaypoints for each individual enmey!
            enemy.SetWaypoints();

            enemies.Add(enemy);

            spawnTimer = 0;
            enemiesSpawned++;
        }

        public void Start()
        {
            spawningEnemies = true;
        }

        public void Update(GameTime gameTime)
        {
            if (enemiesSpawned == numOfEnemies)
                spawningEnemies = false; // We have spawned enough enemies

            if (spawningEnemies)
            {
                spawnTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

                if (spawnTimer > 2)
                    AddEnemy(); // Time to add a new enemey
            }

            for (int i = 0; i < enemies.Count; i++)
            {
                Enemy enemy = enemies[i];
                enemy.Update(gameTime);

                if (enemy.IsDead)
                {
                    if (enemy.CurrentHealth > 0) // Enemy is at the end
                    {
                        enemyAtEnd = true;
                    }

                    enemies.Remove(enemy);
                    i--;
                }
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (Enemy enemy in enemies)
                enemy.Draw(spriteBatch);
        }
    }
}
