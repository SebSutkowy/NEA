using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Globalization;

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
        public static void DrawRect(Rectangle rect, Color color, bool drawAbsolute=true)
        {
            Camera.Draw(pixelRect, rect, color, drawAbsolute);
        }

        public static void DrawText(string text, Vector2 position, Color color)
        {
            Camera.DrawString(text, position, color);
        }
    }

    public class UIRect
    {
        public Rectangle Rectangle { get; protected set; }
        public string Text { get; protected set; }
        public Color Color { get; protected set; }
        public Color TextColor { get; protected set; }

        public UIRect() { }

        public UIRect(Rectangle rect, Color color, string text = "") : this(rect, color, Color.White, text)
        { }

        public UIRect(Rectangle rect, Color color, Color textColor, string text = "")
        {
            Rectangle = rect;
            Color = color;
            Text = text;
            TextColor = textColor;
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

        public void Draw(bool DrawAbsolute=true)
        {
            UI.DrawRect(Rectangle, Color, DrawAbsolute);
            Vector2 textSize = Camera.MeasureString(Text);
            Vector2 Position = new Vector2(Rectangle.Center.X - textSize.X / 2, Rectangle.Center.Y - textSize.Y / 2);
            Camera.DrawString(Text, Position, Color.White);
        }
    }

    public class DialogueBox : UIRect
    {

        private string FinalText { get; set; }

        public bool IsFinished => FinalText.Length <= Text.Length;

        public bool IsPaused { get; protected set; }
        public bool IsVisible { get; set; }

        public DialogueBox() : base()
        {
            FinalText = string.Empty;
        }

        public DialogueBox(Rectangle rect, Color color, string finalText) : base(rect, color, "")
        {
            FinalText = finalText;
        }

        public DialogueBox(Rectangle rect, Color color, string finalText, Color textColor) : base(rect, color, textColor, "")
        {
            FinalText = finalText;
        }

        public void Update()
        {
            if(!IsFinished && !IsPaused)
                Text += FinalText[Text.Length];
                
        }

        public void SkipDialogue()
        {
            if(!IsFinished)
                Text = FinalText;
            else
            {
                IsVisible = false;
                IsPaused = true;
            }
        }

        public void Pause()
        {
            IsPaused = true;
        }

        public void UnPause()
        {
            IsPaused = false;
        }

        public void Draw()
        {
            if (IsVisible)
                base.Draw();
        }

    }
}
