using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace TheDungeonGame
{
    public class Player : Entity 
    {
        private MeleeAttack PlayerAttack { get; set; }
        private int AttackFrame = 0; // place holder for the animation of the attacks
        private int AttackTimer = 0; // so that you can't spam attacks
        private bool IsAttacking = false; 

        public Player(Texture2D texture, Vector2 position, float rotation, int maxHealth, int health, float damage, float speed,
                      Texture2D attackTexture): base(texture, position, rotation, maxHealth, health, damage, speed)
        {
            PlayerAttack = new MeleeAttack(attackTexture, Vector2.Zero, 0f);

        }

        public void Update()
        {
            Move();

            if (InputManager.IsPressed(Input.SaveBinds))
            {
                InputManager.SaveBinds();
            }
            if (InputManager.IsPressed(Input.LMB)) // for attacking
            {
                Attack();
                IsAttacking = true;
            }
            if (IsAttacking && AttackFrame < 10)
                AttackFrame++;
            else if (IsAttacking && AttackFrame >= 10)
            {
                AttackFrame = 0;
                IsAttacking = false;
            }
            else
            {
                AttackTimer = Math.Min(AttackTimer++, 30);
            }



        }

        public Vector2 GetPolarPos(float radius, float angle)
        {
            Vector2 result = Vector2.Zero;
            result.X = radius * (float)Math.Cos(angle);
            result.Y = radius * (float)Math.Sin(angle);
            result += Position;
            return result;
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
            PlayerAttack.Position = GetPolarPos(75, Rotation);
            EnemyManager.Attack(PlayerAttack.Hitbox, PlayerAttack.Damage * Damage); 
        }

        public new void Draw()
        {
            base.Draw();
            if (IsAttacking)
                PlayerAttack.Draw();
        }
    }
}
