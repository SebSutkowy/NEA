using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;

namespace TheDungeonGame
{
    enum Input
    {
        MoveRight, MoveLeft, MoveUp, MoveDown, // Orthogonal movement binds
        SaveBinds, // misc
    }

    enum InputMethod
    {
        Mouse,
        Keyboard
    }
    enum MouseButtons
    {
        LeftButton,
        MiddleButton,
        RightButton
    }

    struct KeyBind
    {
        public InputMethod Method;
        public Keys Key;
        public MouseButtons Button;
    }


    static class InputManager
    {
        private static Dictionary<Input, KeyBind> Binds = new Dictionary<Input, KeyBind>() // hard coded for now --> read from json file on launch
        {
            {Input.MoveLeft, new KeyBind(){ Method = InputMethod.Keyboard, Key = Keys.A} },
            {Input.MoveRight, new KeyBind(){ Method = InputMethod.Keyboard, Key = Keys.D} },
            {Input.MoveUp, new KeyBind(){ Method = InputMethod.Keyboard, Key = Keys.W } },
            {Input.MoveDown, new KeyBind(){ Method = InputMethod.Keyboard, Key = Keys.S} },
            {Input.SaveBinds, new KeyBind(){ Method = InputMethod.Keyboard, Key = Keys.P } },
        };
        private static KeyboardState currentKeyboardState = new KeyboardState();
        private static KeyboardState prevKeyboardState;

        private static MouseState currentMouseState = new MouseState();
        private static MouseState prevMouseState;

        public static void Update()
        {
            prevKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            prevMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
        }

        public static void SaveBinds(string path)
        {
            JsonSerializerOptions options = new JsonSerializerOptions();
            options.WriteIndented = true;
            string jsonText = JsonSerializer.Serialize(Binds, options);
            Debug.WriteLine(jsonText);
            Debug.WriteLine("^^^^^^ JSON ^^^^");
            FileManager.SaveData(path, "Keybinds.json", jsonText);
        }
        
        public static void LoadBinds(string path)
        {
            //JsonSerializerOptions options = new JsonSerializerOptions();
            Binds = JsonSerializer.Deserialize<Dictionary<Input, KeyBind>>(path);
        }


        public static Point GetMousePos() => currentMouseState.Position;

        public static bool IsPressed(Input input)
        {
            if (Binds[input].Method == InputMethod.Keyboard)
                return currentKeyboardState.IsKeyDown(Binds[input].Key) && !prevKeyboardState.IsKeyDown(Binds[input].Key);
            if (Binds[input].Button == MouseButtons.LeftButton)
                return currentMouseState.LeftButton == ButtonState.Pressed && prevMouseState.LeftButton != ButtonState.Pressed;
            if (Binds[input].Button == MouseButtons.MiddleButton)
                return currentMouseState.MiddleButton == ButtonState.Pressed && prevMouseState.MiddleButton != ButtonState.Pressed;
            if (Binds[input].Button == MouseButtons.RightButton)
                return currentMouseState.RightButton == ButtonState.Pressed && prevMouseState.RightButton != ButtonState.Pressed;
            return false;
        }
        public static bool IsHeld(Input input)
        {
            if (Binds[input].Method == InputMethod.Keyboard)
                return currentKeyboardState.IsKeyDown(Binds[input].Key);
            if (Binds[input].Button == MouseButtons.LeftButton)
                return currentMouseState.LeftButton == ButtonState.Pressed;
            if (Binds[input].Button == MouseButtons.MiddleButton)
                return currentMouseState.MiddleButton == ButtonState.Pressed;
            if (Binds[input].Button == MouseButtons.RightButton)
                return currentMouseState.RightButton == ButtonState.Pressed;
            return false;
        }
        
    }
}
