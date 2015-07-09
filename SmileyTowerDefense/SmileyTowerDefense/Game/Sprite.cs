using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;

using Microsoft.Xna.Framework.Graphics;


namespace SmileyTowerDefense
{
   public class Sprite
    {
        protected Texture2D texture;
        public Texture2D Texture { set { texture = value; } get { return texture; } }

        protected Vector2 position;
        public Vector2 Position { set { position = value; } get { return position; } }
        protected Vector2 velocity;
        public Vector2 Velocity { get { return velocity; } }

        protected Vector2 center;
        public Vector2 Center { get { return center; } }
        protected Vector2 origin;
        public Vector2 Origin { get { return origin; } }

        protected float rotation;
        public float Rotation { set { rotation = value; } get { return rotation; } }

        public Sprite(Texture2D tex, Vector2 pos)
        {
            texture = tex;
            position = pos;

            velocity = Vector2.Zero;

            center = new Vector2(position.X + texture.Width /2,
                position.Y + texture.Height / 2);
            origin = new Vector2(texture.Width / 2,
                texture.Height / 2);
        }

        public virtual void Update(GameTime gameTime)
        {
            this.center = new Vector2(position.X + texture.Width / 2,
            position.Y + texture.Height / 2);
        }

        public virtual void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, center, null, Color.White,
                rotation, origin, 1.0f, SpriteEffects.None, 0);
        }

        public virtual void Draw(SpriteBatch spriteBatch, Color color)
        {
            spriteBatch.Draw(texture, center, null, color, rotation,
                origin, 1.0f, SpriteEffects.None, 0);
        }
    }
}
