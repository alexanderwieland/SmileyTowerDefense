using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace SmileyTowerDefense
{
    class HighScore
    {
        List<string> scorelist = new List<string>();
        List<string> namelist = new List<string>();

        public MenuButton highscorebutton;

        public HighScore(Texture2D texture, GraphicsDevice graphics)
        {
            GetData();
            highscorebutton = new MenuButton(texture, graphics);
            highscorebutton.SetPosition(new Vector2(120, 225));
        }
        private void GetData()
        {
            FileStream stream = new FileStream("highscores.txt", FileMode.Open);
            StreamReader sr = new StreamReader(stream);

            while (sr.Peek() != -1)
            {
                string s = sr.ReadLine();
                namelist.Add(s.Split(';')[0]);
                scorelist.Add(s.Split(';')[1]);
            }
            sr.Close();
        }

        public void Update(MouseState mouse)
        {
            highscorebutton.Update(mouse);
        }
       

        public void Draw(SpriteBatch spritebatch, SpriteFont font)
        {
            int nameX=80;
            int nameY=80;

            int scoreX=180;
            int scoreY=80;

            for (int i = 0; i < 5; i++)
			{
                spritebatch.DrawString(font, namelist[i], new Vector2(nameX, nameY), Color.Black);
                spritebatch.DrawString(font, scorelist[i], new Vector2(scoreX, scoreY), Color.Black);
                nameY += 30;
                scoreY += 30;
			}
            highscorebutton.Draw(spritebatch);
        }
    }
}
