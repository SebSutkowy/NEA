using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Diagnostics;
using System.Security.Cryptography.X509Certificates;

namespace TheDungeonGame
{
    public static class Camera
    {
        private static Vector2 Position;
        private static SpriteBatch SpriteBatch;
        private static SpriteFont Font;
        private static GameWindow Window;
        public static GameWindow GetWindow() => Window;


        public static void SetFocus(bool focus) => _Focus = focus;
        private static bool _Focus;
        public static bool IsFocused => _Focus;

        public const int ScreenWidth = 1000;
        public const int ScreenHeight = 1000;
        public static Vector2 ScreenDimensions => new Vector2(ScreenWidth, ScreenHeight);

        public static void Initialize(SpriteBatch spriteBatch, SpriteFont font, GameWindow window)
        {
            Position = Vector2.Zero;
            SpriteBatch = spriteBatch;
            Font = font;
            Window = window;
        }


        public static Rectangle GetScaledRect(float x, float y, float width, float height, float division) => GetScaledRect(x * division, y * division, width * division, height * division);
        public static Rectangle GetScaledRect(float x, float y, float width, float height) => new Rectangle((int)(x * ScreenWidth), (int)(y * ScreenHeight), (int)(width * ScreenWidth), (int)(height * ScreenHeight));
        public static Rectangle OffsetRect(Rectangle rect) => new Rectangle(rect.X - (int)Position.X, rect.Y - (int)Position.Y, rect.Width, rect.Height);  
        public static Vector2 OffsetPos(Vector2 pos) => pos - Position; // draw overloads require this way, Maps in game pos -> screen pos
        public static Point OffsetPoint(Point pos) => pos - new Point((int)Position.X, (int)Position.Y);

        public static Vector2 InverseOffset(Vector2 pos) => pos + Position; // screen pos -> in game pos 
        public static Point InverseOffset(Point pos) => new Point((int)Position.X + pos.X, (int)Position.Y + pos.Y);

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
                float horizontalSide1 = (float)CameraBounds?.X;
                float horizontalSide2 = (float)CameraBounds?.X + (float)CameraBounds?.Width - ScreenWidth;
                float verticalSide1 = (float)CameraBounds?.Y;
                float verticalSide2 = (float)CameraBounds?.Y + (float)CameraBounds?.Height - ScreenHeight;
                Position.X = Math.Clamp(Position.X, Math.Min(horizontalSide1, horizontalSide2), Math.Max(horizontalSide1, horizontalSide2));
                Position.Y = Math.Clamp(Position.Y, Math.Min(verticalSide1, verticalSide2), Math.Max(verticalSide1, verticalSide2));
            }
        }

    }
}
