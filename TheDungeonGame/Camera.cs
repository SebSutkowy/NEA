using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;

namespace TheDungeonGame
{
    public static class Camera
    {
        private static Vector2 Position;
        private static SpriteBatch SpriteBatch;
        private static SpriteFont Font;
        public const int ScreenWidth = 1000;
        public const int ScreenHeight = 1000;
        public static Vector2 ScreenDimensions => new Vector2(ScreenWidth, ScreenHeight);

        public static void Initialize(SpriteBatch spriteBatch, SpriteFont font)
        {
            Position = Vector2.Zero;
            SpriteBatch = spriteBatch;
            Font = font;
        }

        public static Rectangle OffsetRect(Rectangle rect) => new Rectangle(rect.X - (int)Position.X, rect.Y - (int)Position.Y, rect.Width, rect.Height);
        public static Vector2 OffsetPos(Vector2 pos) => pos - Position;
        public static Point OffsetPoint(Point pos) => pos - new Point((int)Position.X, (int)Position.Y);

        #region Draw overloads

        public static void Draw(Texture2D texture, Rectangle rect, Color color, bool AbsolutePos=false)
        {
            if (AbsolutePos)
                SpriteBatch.Draw(texture, rect, color);
            else
                SpriteBatch.Draw(texture, OffsetRect(rect), color);
        }
        public static void Draw(Texture2D texture, Rectangle rect, Rectangle? sourceRect, Color color)
        {
            SpriteBatch.Draw(texture, OffsetRect(rect), sourceRect, color);
        }
        public static void Draw(Texture2D texture, Rectangle rect,
            Rectangle? sourceRect, Color color,
            float rotation, Vector2 Origin,
            SpriteEffects spriteEffects, float layerDepth)
        {
            SpriteBatch.Draw(texture, OffsetRect(rect), sourceRect, color, rotation, Origin, spriteEffects, layerDepth);
        }

        public static void Draw(Texture2D texture, Vector2 position, Color color)
        {
            SpriteBatch.Draw(texture, OffsetPos(position), color);
        }

        public static void Draw(Texture2D texture, Vector2 position, Rectangle? sourceRect, Color color)
        {
            SpriteBatch.Draw(texture, OffsetPos(position), sourceRect, color);
        }

        public static void Draw(Texture2D texture, Vector2 position,
            Rectangle? sourceRect, Color color, float rotation,
            Vector2 origin, Vector2 scale, SpriteEffects spriteEffects, float layerDepth)
        {
            SpriteBatch.Draw(texture, OffsetPos(position), sourceRect, color, rotation, origin, scale, spriteEffects, layerDepth);
        }

        public static void Draw(Texture2D texture, Vector2 position,
            Rectangle? sourceRect, Color color, float rotation,
            Vector2 origin, float scale, SpriteEffects spriteEffects, float layerDepth)
        {
            SpriteBatch.Draw(texture, OffsetPos(position), sourceRect, color, rotation, origin, scale, spriteEffects, layerDepth);
        }

        public static void DrawString(string text, Vector2 position, Color color)
        {
            SpriteBatch.DrawString(Font, text, position, color);
        }

        #endregion

        public static Vector2 MeasureString(string text) => Font.MeasureString(text);

        public static void ResetCamera()
        {
            Position = Vector2.Zero;
        }


        public static void MoveCamera(Vector2 newPosition, float LerpConstant = 0.3f, Rectangle? CameraBounds=null)
        {
            Position += ScreenDimensions/2;
            Position = Vector2.Lerp(Position, newPosition, LerpConstant);
            Position -= ScreenDimensions / 2;
            if (CameraBounds != null)
            {
                Position.X = Math.Clamp(Position.X, (float)CameraBounds?.X, (float)CameraBounds?.X + (float)CameraBounds?.Width - (float)ScreenWidth);
                Position.Y = Math.Clamp(Position.Y, (float)CameraBounds?.Y, (float)CameraBounds?.Y + (float)CameraBounds?.Height- (float)ScreenHeight);
            }
        }

    }
}
