using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace TheDungeonGame
{
    enum Input
    {
        MoveRight, MoveLeft, MoveUp, MoveDown, // Orthogonal movement binds
        
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


    class InputManager
    {
        private Dictionary<Input, KeyBind> Binds; // hard coded for now --> read from json file on launch

        private KeyboardState currentKeyboardState = new KeyboardState();
        private KeyboardState prevKeyboardState;

        private MouseState currentMouseState = new MouseState();
        private MouseState prevMouseState;

        public void Update()
        {
            prevKeyboardState = currentKeyboardState;
            currentKeyboardState = Keyboard.GetState();

            prevMouseState = currentMouseState;
            currentMouseState = Mouse.GetState();
        }

        public Point GetMousePos() => currentMouseState.Position;

        public bool IsPressed(Input input)
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
        public bool IsHeld(Input input)
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
