using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace TheDungeonGame
{
    public static class UI
    {
        private static Texture2D pixelRect;
        private static Point CursorPos;
        private static Color CursorColor = Color.Green;
        public static GUINames CurrentGUIName { get; private set; }
        public static GUI CurrentGUI => GUIs[CurrentGUIName];
        private static GUINames? NewGUI = null;
        private static Dictionary<GUINames, GUI> GUIs = new Dictionary<GUINames, GUI>();
        
        public static void LoadUI(GraphicsDevice graphicsDevice)
        {
            pixelRect = new Texture2D(graphicsDevice, 1, 1);
            pixelRect.SetData(new[] { Color.White });
        }

        public static void AddGUI(GUINames name, GUI gui)
        {
            GUIs.Add(name, gui);
        }

        public static GUI GetGUI(GUINames name) => GUIs[name];

        public static void SetGUI(GUINames name)
        {
            NewGUI = name;
        }

        public static void SetDialogue(List<string> texts)
        {
            GUIs[GUINames.Dialogue][(int)DialogueGUIElements.DialogueBox].SetTexts(texts);
        }

        public static void Update()
        {
            CursorPos = InputManager.GetMousePos();
            if (!Dungeon.IsActive) return;
            Point tilemapPos = Dungeon.GetTilemapPos(Camera.InverseOffset(CursorPos));
            string loc = Tilemap.GetLoc(tilemapPos);
            if (Dungeon.IsInteractive(loc))
            {
                CursorColor = Color.Red;
            }
            else
            {
                CursorColor = Color.Green;
            }
            if (NewGUI != null)
            {
                CurrentGUIName = NewGUI.Value;
                NewGUI = null;
            }
        }

        public static void DrawRect(Rectangle rect, Color color, bool drawAbsolute=true)
        {
            Camera.Draw(pixelRect, rect, color, drawAbsolute);
        }

        public static void DrawText(string text, Vector2 position, Color color)
        {
            Camera.DrawString(text, position, color);
        }

        public static void Draw()
        {
            DrawRect(new Rectangle(CursorPos, new Point(10)), CursorColor);
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

        public virtual void Interact() { }
        public virtual void SetTexts(List<string> text) { }
        public virtual void Update() { }

        public void Draw(bool DrawAbsolute=true)
        {
            UI.DrawRect(Rectangle, Color, DrawAbsolute);
            Vector2 textSize = Camera.MeasureString(Text);
            Vector2 Position = new Vector2(Rectangle.Center.X - textSize.X / 2, Rectangle.Center.Y - textSize.Y / 2);
            Camera.DrawString(Text, Position, TextColor);
        }
    }
    public class DialogueBox : UIRect
    {
        private List<string> Texts { get; set; }
        private int CurrentTextNum { get; set; }
        private string CurrentText => Texts.Count > 0 ? Texts[CurrentTextNum] : ""; 

        public bool IsFinished => CurrentText.Length <= Text.Length;
        public bool IsCompleted => (CurrentTextNum == Texts.Count - 1) && IsFinished;

        public bool IsPaused { get; protected set; }
        public bool IsVisible { get; set; }

        public DialogueBox() : base()
        {
            Texts = new List<string>();
            CurrentTextNum = 0;
        }

        public DialogueBox(Rectangle rect, Color color, List<string> texts) : base(rect, color, "")
        {
            CurrentTextNum = 0;
            Texts = texts;
        }

        public DialogueBox(Rectangle rect, Color color, List<string> texts, Color textColor) : base(rect, color, textColor, "")
        {
            CurrentTextNum = 0;
            Texts = texts;
        }

        public override void SetTexts(List<string> texts)
        {
            Texts = texts;
            CurrentTextNum = 0;
            Text = "";
        }

        public void IncrementText()
        {
            if (Texts.Count <= 0)
                return;
            CurrentTextNum = ++CurrentTextNum % Texts.Count;
            Text = "";
        }

        public override void Update()
        {
            if(!IsFinished && !IsPaused)
                Text += CurrentText[Text.Length];
        }

        public void Reset()
        {
            CurrentTextNum = 0;
            Text = "";
            UnPause();
            IsVisible = true;
        }

        public override void Interact()
        {
            if (!IsFinished)
                Text = CurrentText;
            else if (IsFinished && !IsCompleted)
            {
                IncrementText();
            }
            else if (IsCompleted)
            {
                IsVisible = false;
                IsPaused = true;
                Reset();
                UI.SetGUI(GUINames.Shop);
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

    public class TextBox : UIRect
    {
        private GameWindow Window { get; set; }
        private bool IsFocused { get; set; }
        private StringBuilder Input { get; set; }
        public bool Entered { get; set; }

        public TextBox() : base()
        {
            IsFocused = false;
        }

        public TextBox(GameWindow window, Rectangle rect, Color color) : base(rect, color)
        {
            Window = window;
            IsFocused = false;
            Entered = false;
        }

        private void OnTextInput(object sender, TextInputEventArgs e)
        {
            char c = e.Character;

            Entered = false;
            switch (c)
            {
                case '\b':
                    if (Text.Length > 0)
                        Text = Text.Remove(Text.Length - 1);
                    break;
                case '\r':
                case '\n':
                    Entered = true;
                    break;
                default:
                    Text += c;
                    break;
            }
        }

        public void Reset()
        {
            Text = string.Empty;
        }

        public void OnClick(Point mpos)
        {
            if (Contains(mpos))
            {
                IsFocused = true;
                RegisterTextInput(OnTextInput); 
            }
            else
            {
                IsFocused = false;
                UnRegisterTextInput(OnTextInput);
            }
        }

        private void RegisterTextInput(EventHandler<TextInputEventArgs> handler) => Window.TextInput += handler; 
        private void UnRegisterTextInput(EventHandler<TextInputEventArgs> handler) => Window.TextInput -= handler;
    }
}
