
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace TheDungeonGame
{
    public class Sprite
    {
        public Texture2D Texture { get; set; }
        public Vector2 Origin => new Vector2(Texture.Width / 2, Texture.Height / 2);
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Rectangle Hitbox => new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);

        public Sprite(Texture2D texture, Vector2 position, float rotation)
        {
            if (texture == null)
                throw new Exception("Texture is not implemented");
            Texture = texture;
            Position = position;
            Rotation = rotation;
        }

        public void Draw(Color color)
        {
            Camera.Draw(Texture, Position, null, color, Rotation, Origin, 1.0f, SpriteEffects.None, 0.0f); // will add animations source rectangle will no longer be null
        }

        public void Draw() => Draw(Color.White);
    }
}
