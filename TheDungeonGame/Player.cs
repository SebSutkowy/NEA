using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace TheDungeonGame
{
    public class Player : Entity 
    {

        public Player(Texture2D texture, Vector2 position, float rotation, int maxHealth, int health, float damage, float speed): base(texture, position, rotation, maxHealth, health, damage, speed)
        { }

        public void Update()
        {
            Move();

            if (InputManager.IsPressed(Input.SaveBinds))
            {
                InputManager.SaveBinds();
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
            mpos = Camera.OffsetPoint(new Point(-1 * mpos.X, -1 * mpos.Y)); // the offset method subtracts the offset from the point when addition is needed here
            mpos = new Point(-1 * mpos.X, -1 * mpos.Y); // -(-mpos-offset) = mpos + offset
            Rotation = (float)Math.Atan2(mpos.Y - Position.Y, mpos.X - Position.X);
            Rotation = (Rotation + MathHelper.Pi / 2) % (MathHelper.Pi * 2);
        }

        public void Attack()
        {
            
        }
    }
}
