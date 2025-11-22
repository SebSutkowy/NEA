using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheDungeonGame
{
    public static class UI
    {
        private static Texture2D pixelRect;
        
        public static void LoadUI(GraphicsDevice graphicsDevice)
        {
            pixelRect = new Texture2D(graphicsDevice, 1, 1);
            pixelRect.SetData(new[] { Color.White });
        }
        public static void DrawRect(Rectangle rect, Color color)
        {
            Camera.Draw(pixelRect, rect, color);
        }

        public static void DrawText(string text, Vector2 position, Color color)
        {
            Camera.DrawString(text, position, color);
        }
    }

    public class UIRect
    {
        public Rectangle Rectangle { get; private set; }
        public string Text { get; private set; }
        public Color Color { get; private set; }

        public UIRect(Rectangle rect, Color color, string text="")
        {
            Rectangle = rect;
            Color = color;
            Text = text;
        }

        public bool Contains(Point pos) => Rectangle.Contains(pos);

        public void ChangePos(Vector2 pos)
        {
            Rectangle = new Rectangle((int)pos.X, (int)pos.Y, Rectangle.Width, Rectangle.Height);
        }
        public void ChangeSize(Vector2 size)
        {
            Rectangle = new Rectangle(Rectangle.X, Rectangle.Y, (int)size.X, (int)size.Y);
        }

        public void ChangeText(string text)
        {
            Text = text;
        }

        public void ChangeColor(Color newColor)
        {
            Color = newColor;
        }

        public void Draw()
        {
            UI.DrawRect(Rectangle, Color);
            Vector2 textSize = Camera.MeasureString(Text);
            Vector2 Position = new Vector2(Rectangle.Center.X - textSize.X / 2, Rectangle.Center.Y - textSize.Y / 2);
            Camera.DrawString(Text, Position, Color.White);
        }
    }
}
