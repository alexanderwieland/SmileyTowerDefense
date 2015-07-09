using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.IO;

namespace SmileyTowerDefense
{
    public class Player
    {
        public Level level = new Level();
        public WaveManager waveManager;
        Button arrowButton;
        Toolbar toolBar;

        Texture2D bulletTexture;
        Texture2D enemyTexture;
        Texture2D towerTexture;

      

        List<Texture2D> enemyTextures;

        // Player state.
        private int money;
        private int lives;
        private int score = 1;
        private string name;

        // A list of the players towers
        private List<Tower> towers = new List<Tower>();
        public List<Tower> Towers { get { return towers; } }

        // Mouse state for the current frame.
        private MouseState mouseState;
        // Mouse state for the previous frame.
        private MouseState oldState;

        Texture2D arrowNormal;
        Texture2D arrowHover;
        Texture2D arrowPressed;

        // Tower placement.
        private int cellX;
        private int cellY;

        private int tileX;
        private int tileY;

        // The type of tower to add.
        private string newTowerType;

        // Pathfinder
        Pathfinder pathFinder;


        public int Money
        {
            set { money = value; }
            get { return money; }
        }
        public int Lives
        {
            set { lives = value; }
            get { return lives; }
        }
        public int Score
        {
            set { score = value; }
            get { return score; }
        }
        public string Name
        {
            set { name = value; }
            get { return name; }
        }
        public string NewTowerType
        {
            set { newTowerType = value; }
        }

        /// <summary>
        /// Construct a new player.
        /// </summary>
        /// 


        public Player(ContentManager Content, string name)
        {
            pathFinder = new Pathfinder(level);
            this.name = name;
            this.money = 100;
            this.lives = 10;
            arrowNormal = Content.Load<Texture2D>("Game/arrow button");
            arrowHover = Content.Load<Texture2D>("Game/arrow hover");
            arrowPressed = Content.Load<Texture2D>("Game/arrow pressed");

            //Initialize the arrow button.
            
                arrowButton = new Button(arrowNormal, arrowHover,
                    arrowPressed, new Vector2(0, level.Height * 32));
            
                arrowButton.Clicked += new EventHandler(arrowButton_Clicked);
            

            SpriteFont font = Content.Load<SpriteFont>("Arial");

            Texture2D topBar = Content.Load<Texture2D>("Game/toolbar");
            enemyTextures = new List<Texture2D>();
            toolBar = new Toolbar(topBar, font, new Vector2(0, (level.Height) * 32));

            Texture2D grass = Content.Load<Texture2D>("Game/grass");
            Texture2D path = Content.Load<Texture2D>("Game/path");


            level.AddTexture(grass);
            level.AddTexture(path);

            enemyTextures.Add(Content.Load<Texture2D>("Game/enemy"));
            enemyTextures.Add(Content.Load<Texture2D>("Game/enemy50"));
            enemyTextures.Add(Content.Load<Texture2D>("Game/enemy25"));
            enemyTextures.Add(Content.Load<Texture2D>("Game/enemy00"));

            Texture2D towerTexture = Content.Load<Texture2D>("Game/arrowtower");
            Texture2D bulletTexture = Content.Load<Texture2D>("Game/bullet");

            waveManager = new WaveManager(level, 10, this, enemyTextures, pathFinder);

            this.towerTexture = towerTexture;
            this.bulletTexture = bulletTexture;
        }

        private void arrowButton_Clicked(object sender, EventArgs e)
        {
            this.NewTowerType = "ArrowTower";
        }

        /// <summary>
        /// Returns wether the current cell is clear
        /// </summary>

        private bool IsCellClear(List<Enemy> enemies)
        {
            // Make sure tower is within limits
            bool spaceInBounds = cellX >= 0 && cellY >= 0 &&
                cellX < level.Width && cellY < level.Height;

            bool spaceClearOfEnemies = false;
            bool noEnemyLock = false;
            bool spaceClearOfTowers = false;

            if (spaceInBounds && !(cellX == 4 && cellY == 0) && !(cellX == 4 && cellY == 8))       // FLAGged pete
            {
                // Check for enemies on the spot
                Level lvlSave = new Level();
                lvlSave.Map = (int[,])level.Map.Clone();
                lvlSave.Map[cellX, cellY] = 1;  // maybe it does with x and y corrected.. SetWayPoint(cellX, cellY);// not changing anything oO?!.Map[cellY, cellX] = 1;
                Pathfinder hypoPath = new Pathfinder(lvlSave);

                if (enemies.Count != 0)
                {
                    foreach (Enemy e in enemies)
                    {
                        // TODOne somewhere in the code: Enemy setWaypoint and update; Player..well here /notsomuch: Update
                        // TODOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOne

                        // if enemy is not on the cell
                        spaceClearOfEnemies = (Math.Abs(e.Position.X - tileX) >= 32/*8*/ || Math.Abs(e.Position.Y - tileY) >= 32/*8*/) && (e.Waypoints.Peek().X != cellX/*tileX*/ || e.Waypoints.Peek().Y != cellY/*tileY*/) ? true : false;   // TODOne min 32 pixel dazwischen!
                        // ftodone when enemy would not move towards mouse potential new tower position, it should still be allowed

                        if (!spaceClearOfEnemies)
                        {
                            break;
                        }

                        if (e.Waypoints.Count > 0)
                        {
                            // enemies don't get locked in
                            if (hypoPath.FindPath(new Point((int)e.Waypoints.Peek().X / 32, (int)e.Waypoints.Peek().Y / 32), new Point(4, 8)).Count > 0)    // FLAGged
                            {
                                noEnemyLock = true;
                            }
                        }

                        if (!noEnemyLock)
                        {
                            break;
                        }
                    }

                }
                else
                {
                    spaceClearOfEnemies = true;
                    noEnemyLock = true;
                }
                // start to endpoint flow check
                if (hypoPath.FindPath(new Point(4, 0), new Point(4, 8)).Count < 1)
                    noEnemyLock = false;

                //ob ma durchkann oda nicht

                spaceClearOfTowers = (level.GetIndex(cellX, cellY) != 1);
            }
            //string s = "0";
            //if (spaceInBounds && spaceClearOfEnemies && spaceClearOfTowers && noEnemyLock)
            //    s = "";
            return spaceInBounds && spaceClearOfEnemies && spaceClearOfTowers && noEnemyLock; // If both checks are true return true
        }  

        /// <summary>
        /// Adds a tower to the player's collection.
        /// </summary>
        public void AddTower(List<Enemy> enemies)
        {
            Tower towerToAdd = null;

            switch (newTowerType)
            {
                case /*"Arrow Tower"*/"ArrowTower":
                    {
                        towerToAdd = new ArrowTower(towerTexture,
                            bulletTexture, new Vector2(tileX, tileY));
                        break;
                    }
            }

            // Only add the tower if there is a space, it doesn't lock enemies from getting to the goal, and if the player can afford it.
            if (IsCellClear(enemies) == true && towerToAdd.Cost <= money)
            {
                towers.Add(towerToAdd);
                money -= towerToAdd.Cost;
                level.Map[cellY, cellX] = 1;

                // Reset the newTowerType field.
                newTowerType = string.Empty;

                // TODOne SetWaypoints of enemies!
                pathFinder.ResetSearchNodes();
                pathFinder.InitializeSearchNodes();
                foreach (Enemy e in enemies)
                {
                    List<Vector2> path = pathFinder.FindPath(new Point((int)e.Waypoints.Peek().X / 32, (int)e.Waypoints.Peek().Y / 32), new Point(4,8));

                    // Path is found
                    if (path.Count > 0)
                    {
                        e.Waypoints.Clear();
                        foreach (Vector2 waypoint in path)
                            e.Waypoints.Enqueue(waypoint);

                        // not needed: this.position = this.waypoints.Dequeue();
                    }

                    // no path possible
                    else
                        ;   // debug error msg optional
                }
            }
        }

        public void AddMultiplayerTower(Vector2 position)
        {
            Tower towerToAdd = null;

            switch (newTowerType)
            {
                case /*"Arrow Tower"*/"ArrowTower":
                    {
                        towerToAdd = new ArrowTower(towerTexture,
                            bulletTexture, position);
                        break;
                    }
            }

            towers.Add(towerToAdd);
            newTowerType = string.Empty;
            level.Map[(int)position.X / 32, (int)position.Y / 32] = 1;

            pathFinder.ResetSearchNodes();
            pathFinder.InitializeSearchNodes();
            foreach (Enemy e in waveManager.CurrentWave.Enemies)
            {
                List<Vector2> path = pathFinder.FindPath(new Point((int)e.Waypoints.Peek().X / 32, (int)e.Waypoints.Peek().Y / 32), new Point(4, 8));

                // Path is found
                if (path.Count > 0)
                {
                    e.Waypoints.Clear();
                    foreach (Vector2 waypoint in path)
                        e.Waypoints.Enqueue(waypoint);

                    // not needed: this.position = this.waypoints.Dequeue();
                }

                // no path possible
                else
                    ;   // debug error msg optional
            }
        }

        public void Update(GameTime gameTime,bool islocal)
        {
            mouseState = Mouse.GetState();

            //Update the arrow button.
            if(islocal)
            arrowButton.Update(gameTime);

            waveManager.Update(gameTime);

            cellX = (int)(mouseState.X / 32); // Convert the position of the mouse
            cellY = (int)(mouseState.Y / 32); // from array space to level space

            tileX = cellX * 32; // Convert from array space to level space
            tileY = cellY * 32; // Convert from array space to level space

            if (mouseState.LeftButton == ButtonState.Released
                && oldState.LeftButton == ButtonState.Pressed)
            {
                if (string.IsNullOrEmpty(newTowerType) == false)
                {
                    // TODOne AddTower();
                    AddTower(waveManager.CurrentWave.Enemies);
                }
            }

            foreach (Tower tower in towers)
            {
                if (tower.Target == null)
                {
                    tower.GetClosestEnemy(waveManager.Enemies);
                }

                tower.Update(gameTime);
            }

            oldState = mouseState; // Set the oldState so it becomes the state of the previous frame.
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            level.Draw(spriteBatch);

            waveManager.Draw(spriteBatch);

            // Draw the tool bar first,
            toolBar.Draw(spriteBatch, this);
            // and then our buttons.
            
            arrowButton.Draw(spriteBatch);

            foreach (Tower tower in towers)
            {
                tower.Draw(spriteBatch);
            }
        }

        public void RaiseStats(int statsToAdd)
        {
            this.money += statsToAdd;
            this.score += statsToAdd;
        }

        public void Lost()
        {
            FileStream stream = new FileStream("highscores.txt", FileMode.Open);
            StreamReader sr = new StreamReader(stream);

            List<string> wholeFile = new List<string>();

            while (sr.Peek() != -1)
            {
                wholeFile.Add(sr.ReadLine());
            }
            sr.Close();
            stream.Close();

            stream = new FileStream("highscores.txt", FileMode.Open);
            StreamWriter sw = new StreamWriter(stream);

            string[] splitArray;
            for (int i = 0; i < wholeFile.Count; i++)
            {
                splitArray = wholeFile[i].Split(';');
                if (Int32.Parse(splitArray[1]) <= score)
                {
                    for (int j = 4; j > i; j--)
                    {
                        wholeFile[j] = wholeFile[j - 1];
                    }
                    wholeFile[i] = name + ";" + score;
                }
                sw.WriteLine(wholeFile[i]);
            }
            sw.Flush();
            sw.Close();
            stream.Close();
        }
    }
}
