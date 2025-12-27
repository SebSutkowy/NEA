
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace TheDungeonGame
{
    public class Sprite
    {

        public AnimationManager AnimationManager;
        public Vector2 Origin => new Vector2(AnimationManager.CurrentFrameRect.Width / 2, AnimationManager.CurrentFrameRect.Height / 2);
        public Vector2 Position { get; set; }
        public float Rotation { get; set; }
        public Rectangle Hitbox => new Rectangle((int)Position.X - AnimationManager.CurrentFrameRect.Width/2, (int)Position.Y - AnimationManager.CurrentFrameRect.Height/2, AnimationManager.CurrentFrameRect.Width, AnimationManager.CurrentFrameRect.Height);

        public Sprite() { }

        public Sprite(AnimationManager animationManager, Vector2 position, float rotation)
        {
            AnimationManager = animationManager; 
            Position = position;
            Rotation = rotation;
        }

        public void Draw(Color color)
        {
            AnimationManager.Draw(Position, Rotation, Origin);
        }

        public void Draw() => Draw(Color.White);
    }
}
