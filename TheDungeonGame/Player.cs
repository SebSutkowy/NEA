using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;
using System.Runtime.Serialization;
using System.Diagnostics;

namespace TheDungeonGame
{
    public class Player : Entity
    {
        private int AttackCooldown = 0; // so that you can't spam attacks
        private const int MaxAttackCooldown = 30;
        private bool IsAttacking = false;
        private Skillset Skills;
        private Classes Class;

        public Player() : base()
        { }

        public Player(AnimationManager animationManager, Vector2 position, float rotation, int maxHealth, int health, float damage, float speed, Classes @class) 
            : base(animationManager, position, rotation, maxHealth, health, damage, speed)
        {
            Class = @class;
            Skills = new Skillset(@class);

        }

        public void Update(Tilemap tilemap)
        {
            Move(tilemap);

            if (InputManager.IsPressed(Input.SaveBinds))
            {
                InputManager.SaveBinds();
            }
            if (InputManager.IsPressed(Input.LMB) && AttackCooldown >= MaxAttackCooldown) // for attacking
            {
                Attack(AttackType.NormalAttack);
                IsAttacking = true;
                AttackCooldown = 0;
            }
            if (IsAttacking && Skills.AnimationManager.CurrentAnimation != AnimationNames.None)
            {
                Skills.Update(Position, Rotation); // just updates the animation
            }
            else if (IsAttacking && Skills.AnimationManager.CurrentAnimation == AnimationNames.None)
            {
                IsAttacking = false;
            }
            else
            {
                AttackCooldown = Math.Min(++AttackCooldown, MaxAttackCooldown);
            }
            Debug.WriteLine(AttackCooldown);
        }

        public Vector2 GetPolarPos(float radius, float angle)
        {
            Vector2 result = Vector2.Zero;
            result.X = radius * (float)Math.Cos(angle);
            result.Y = radius * (float)Math.Sin(angle);
            result += Position;
            return result;
        }

        public void Move(Tilemap tilemap)
        {
            Vector2 vels = Vector2.Zero;

            // horizontal velocities
            vels.X = InputManager.IsHeld(Input.MoveLeft) ? -Speed : 0;
            vels.X += InputManager.IsHeld(Input.MoveRight) ? Speed : 0; 

            // vertical velocities
            vels.Y = InputManager.IsHeld(Input.MoveUp) ? -Speed : 0;
            vels.Y += InputManager.IsHeld(Input.MoveDown) ? Speed : 0;

            // checking for collisions with the tilemap
            Rectangle collisionBox = new Rectangle((int)(Hitbox.X + vels.X), Hitbox.Y, Hitbox.Width, Hitbox.Height);
            if (tilemap.IsValid(collisionBox)) Position = new Vector2(Position.X + vels.X, Position.Y);
            collisionBox = new Rectangle(Hitbox.X, (int)(Hitbox.Y + vels.Y), Hitbox.Width, Hitbox.Height);
            if (tilemap.IsValid(collisionBox)) Position = new Vector2(Position.X, Position.Y + vels.Y);

            // calculating player rotation based on mouse pos
            Point mpos = InputManager.GetMousePos();
            mpos = Camera.OffsetPoint(new Point(-1 * mpos.X, -1 * mpos.Y)); // the offset method subtracts the offset from the point when addition is needed here
            mpos = new Point(-1 * mpos.X, -1 * mpos.Y); // -(-mpos-offset) = mpos + offset
            Rotation = (float)Math.Atan2(mpos.Y - Position.Y, mpos.X - Position.X);
            Rotation = (Rotation + MathHelper.Pi / 2) % (MathHelper.Pi * 2);
        }

        public void Attack(AttackType attackType)
        {
            Skills.Attack(attackType);
            EnemyManager.Attack(Skills.Hitbox, Skills.GetDamage(attackType));

        }

        public new void Draw()
        {
            UI.DrawRect(Hitbox, Color.Red, false);
            base.Draw();
            if (IsAttacking && Skills.AnimationManager.CurrentAnimation != AnimationNames.None)
                Skills.Draw(); 
        }
   }
}
