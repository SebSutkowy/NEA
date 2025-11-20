using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace TheDungeonGame
{
    internal class Player : Sprite
    {
        const float Speed = 5.0f;

        public Player(Texture2D texture, Vector2 position, float rotation): base(texture, position, rotation)
        { }

        public void Update()
        {
            Move();

            if (InputManager.IsPressed(Input.SaveBinds))
            {
                InputManager.SaveBinds(@"Data\Keybinds");
            }

        }


        public void Move()
        {
            Vector2 vels = new Vector2();

            vels.X = InputManager.IsHeld(Input.MoveLeft) ? -Speed : 0;
            vels.X += InputManager.IsHeld(Input.MoveRight) ? Speed : 0;

            vels.Y = InputManager.IsHeld(Input.MoveUp) ? -Speed : 0;
            vels.Y += InputManager.IsHeld(Input.MoveDown) ? Speed : 0;

            Position += new Vector2(vels.X, vels.Y);

            Point mpos = InputManager.GetMousePos();
            Rotation = (float)Math.Atan2(mpos.Y - Position.Y, mpos.X - Position.X);
            Rotation = (Rotation + MathHelper.Pi / 2) % (MathHelper.Pi * 2);
        }
    }
}
