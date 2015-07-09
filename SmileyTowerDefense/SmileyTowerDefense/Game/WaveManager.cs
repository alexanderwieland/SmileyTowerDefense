using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SmileyTowerDefense
{
    public class WaveManager
    {
        private int numberOfWaves; // How many waves the game will have
        public int NumberOfWaves { set { numberOfWaves = value; } get { return numberOfWaves; } }
        private float timeSinceLastWave; // How long since the last wave ended
        public float TimeSinceLastWave { set { timeSinceLastWave = value; } get { return timeSinceLastWave; } }

        private Queue<Wave> waves = new Queue<Wave>(); // A queue of all our waves
        public Queue<Wave> Waves { get { return waves; } set { waves = value; } }

        private bool waveFinished = false; // Is the current wave over?
        public bool WaveFinished { set { waveFinished = value; } get { return waveFinished; } }

        private Level level; // A reference to our level class

        public Wave CurrentWave // Get the wave at the front of the queue
        {
            get
            {
                if (waves.Count > 0) return waves.Peek();
                else return null;
            }
            
        }
        public List<Enemy> Enemies // Get a list of the current enemeies
        {
            get
            {
                if (CurrentWave != null) return CurrentWave.Enemies;
                else return null;
            }
            set { CurrentWave.Enemies = value; }
        }
        public int Round // Returns the wave number
        {
            get { return CurrentWave.RoundNumber + 1; }
        }

        public WaveManager(Level level, int numberOfWaves, Player player, List<Texture2D> textures, Pathfinder pathFinder)
        {
            this.numberOfWaves = numberOfWaves;

            this.level = level;

            for (int i = 0; i < numberOfWaves; i++)
            {
                int initialNumerOfEnemies = 6;
                int numberModifier = (i / 6) + 1;

                Wave wave = new Wave(i, initialNumerOfEnemies *
                   numberModifier, level, player, textures, pathFinder);

                waves.Enqueue(wave);
            }

            StartNextWave();
        }

        private void StartNextWave()
        {
            if (waves.Count > 0) // If there are still waves left
            {
                waves.Peek().Start(); // Start the next one

                timeSinceLastWave = 0; // Reset timer
                waveFinished = false;
            }
        }

        public void Update(GameTime gameTime)
        {
            CurrentWave.Update(gameTime); // Update the wave

            if (CurrentWave.RoundOver) // Check if it has finished
            {
                waveFinished = true;
            }

            if (waveFinished) // If it has finished
            {
                timeSinceLastWave += (float)gameTime.ElapsedGameTime.TotalSeconds; // Start the timer
            }

            if (timeSinceLastWave > 10.0f) // If 30 seconds has passed
            {
                waves.Dequeue(); // Remove the finished wave
                StartNextWave(); // Start the next wave
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            CurrentWave.Draw(spriteBatch);
        }
    }
}
