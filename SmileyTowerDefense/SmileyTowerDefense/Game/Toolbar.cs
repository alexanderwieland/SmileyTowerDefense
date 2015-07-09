using System;
using System.Collections.Generic;
using System.Text;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SmileyTowerDefense
{
    public class Toolbar
    {
        private Texture2D texture;
        // A class to access the font we created
        private SpriteFont font;

        // The position of the toolbar
        private Vector2 position;
        // The position of the text
        private Vector2 textPosition;

        public Toolbar(Texture2D texture, SpriteFont font, Vector2 position)
        {
            this.texture = texture;
            this.font = font;

            this.position = position;
            // Offset the text to the bottom right corner
            textPosition = new Vector2(140, position.Y + 10);
        }

        public void Draw(SpriteBatch spriteBatch, Player player)
        {
            spriteBatch.Draw(texture,new Rectangle(0,player.level.Height*32,player.level.Width*32,32), Color.White);

            string text = string.Format("Gold : {0} Lives : {1}", player.Money, player.Lives);
            spriteBatch.DrawString(font, text, textPosition, Color.White);
        }
    }
}
