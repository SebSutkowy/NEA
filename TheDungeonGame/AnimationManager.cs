using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;

namespace TheDungeonGame
{
    public enum AnimationNames
    {
        None,
        Idle,
        Aura,
        SwingAttack,
        BowShotAttack,
        OrbAttack
    }

    public class AnimationManager
    {
        private Texture2D SpriteSheet { get; set; }
        private Dictionary<AnimationNames, Animation> Animations { get; set; }
        public AnimationNames CurrentAnimation { get; private set; }
        public Rectangle CurrentFrameRect => CurrentAnimation != AnimationNames.None ? Animations[CurrentAnimation].GetSourceRect() : new Rectangle(0, 0, 0, 0);

        public AnimationManager()
        {
            Animations = new Dictionary<AnimationNames, Animation>();
        }

        public AnimationManager(Texture2D spriteSheet)
        {
            Animations = new Dictionary<AnimationNames, Animation>();
            SpriteSheet = spriteSheet;
        }

        public AnimationManager(AnimationManager animationManager)
        {
            this.Animations = animationManager.Animations;
            this.SpriteSheet = animationManager.SpriteSheet;
            this.CurrentAnimation= animationManager.CurrentAnimation;
        }

        public void Play()
        {
            Animations[CurrentAnimation].Play();
        }

        public void Play(AnimationNames animation)
        {
            CurrentAnimation = animation;
            Play();
        }

        public void Update()
        {
            if (Animations[CurrentAnimation].Update())
            {
                CurrentAnimation = AnimationNames.None; 
            }
        }

        public void ChangeAnimation(AnimationNames newState) => CurrentAnimation = newState;

        public void AddAnimation(AnimationNames state, Animation animation)
        {
            Animations[state] = animation;
        }

        public void Draw(Vector2 position, float rotation, Vector2 origin)
        {
            Animations[CurrentAnimation].Draw(SpriteSheet, position, rotation, origin, 1.0f);
        }

    }
}
