using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;

namespace TheDungeonGame
{
    public enum Animations
    {
        Idle,
        Aura
    }

    public class AnimationManager
    {
        private Texture2D SpriteSheet { get; set; }
        private Dictionary<Animations, Animation> Animations { get; set; }
        public Animations CurrentAnimation { get; private set; }
        public Rectangle CurrentFrameRect => Animations[CurrentAnimation].GetSourceRect();

        public AnimationManager()
        {
            Animations = new Dictionary<Animations, Animation>();
        }

        public AnimationManager(Texture2D spriteSheet)
        {
            Animations = new Dictionary<Animations, Animation>();
            SpriteSheet = spriteSheet;
        }

        public void ChangeAnimation(Animations newState) => CurrentAnimation = newState;

        public void AddAnimation(Animations state, Animation animation)
        {
            Animations[state] = animation;
        }

        public void Draw(Vector2 position, float rotation, Vector2 origin)
        {
            Animations[CurrentAnimation].Draw(SpriteSheet, position, rotation, origin, 1.0f);
        }

    }
}
