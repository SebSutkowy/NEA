
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

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
            Texture = texture;
            Position = position;
            Rotation = rotation;
        }

        public void Draw()
        {
            Camera.Draw(Texture, Position, null, Color.White, Rotation, Origin, 1.0f, SpriteEffects.None, 0.0f); // will add animations source rectangle will no longer be null
        }
    }
}
