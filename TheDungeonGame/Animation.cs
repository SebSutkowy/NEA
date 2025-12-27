using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace TheDungeonGame
{
    public class Animation
    {
        public int FrameCount { get; set; }
        public int CurrentFrame { get; set; }
        public int AnimationFrameInterval { get; set; }
        public int FrameTimer { get; set; }
        private bool Paused;
        public bool IsPaused => Paused;
        private bool Looping;
        public bool IsLooping => Looping;
        public Vector2 FrameSize { get; set; }
        public int SpriteSheetRow { get; set; }

        public Animation(Vector2 frameSize, int spriteSheetRow, int frameCount, int frameInterval, bool looping = false)
        {
            FrameSize = frameSize;
            SpriteSheetRow = spriteSheetRow;
            FrameCount = frameCount;
            AnimationFrameInterval = frameInterval;
            CurrentFrame = 0;
            FrameTimer = 0;
            Paused = true;
            Looping = looping;
        }

        public Animation(Animation animation)
        {
            this.FrameSize = animation.FrameSize;
            this.SpriteSheetRow = animation.SpriteSheetRow;
            this.FrameCount = animation.FrameCount;
            this.AnimationFrameInterval = animation.AnimationFrameInterval;
            this.CurrentFrame = 0;
            this.FrameTimer = 0;
            this.Paused = true;
            this.Looping = animation.Looping;
        }

        public void Reset()
        {
            FrameTimer = 0;
            CurrentFrame = 0;
            Paused = true;
        }

        public void Play()
        {
            Reset();
            Paused = false;
        }

        public void Pause()
        {
            Paused = true;
        }

        public void UnPause()
        {
            Paused = false;
        }

        public void Update()
        {
            if (Paused) return;
            if (FrameTimer++ < AnimationFrameInterval)
                return;
            FrameTimer = 0;
            CurrentFrame = ++CurrentFrame % FrameCount;
            if (!Looping && CurrentFrame == 0)
                Paused = true;
        }

        public Rectangle GetSourceRect() => new Rectangle((int)FrameSize.X * CurrentFrame, (int)FrameSize.Y * SpriteSheetRow, (int)FrameSize.X, (int)FrameSize.Y);

        public void Draw(Texture2D spriteSheet, Vector2 position, float rotation, Vector2 origin, float scale)
        {
            Camera.Draw(spriteSheet, position, GetSourceRect(), Color.White, rotation, origin, scale, SpriteEffects.None, 1.0f);
        }



    }
}
