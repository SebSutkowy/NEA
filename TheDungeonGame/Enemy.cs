
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using System;
using System.Collections.Generic;

namespace TheDungeonGame
{

    public class Enemy : Entity
    {
        private UIRect HealthBar;
        private int HealthBarMaxLength;
        private int FrameDamageTime;
        private Skillset attacks = new Skillset(Classes.Berserker);
        private int AttackCooldown;
        private const int MaxAttackCooldown = 30;
        private bool IsAttacking;
        public int TrackedPlayerId { get; private set; } 

        private const float MinSquareDistToPlayer = 10000f; 

        public Enemy(AnimationManager animationManager, Vector2 position, float rotation, int maxHealth, int health, float damage, float speed) : base(animationManager, position, rotation, maxHealth, health, damage, speed)
        {
            HealthBarMaxLength = AnimationManager.CurrentFrameRect.Width;
            HealthBar = new UIRect(
                    new Rectangle((int)(Position.X - AnimationManager.CurrentFrameRect.Width / 2), (int)(Position.Y - AnimationManager.CurrentFrameRect.Height / 2 - 12), (Health / MaxHealth) * HealthBarMaxLength, 10),
                    Color.Red
                    );
            FrameDamageTime = 15;
            FramesSinceDamage = FrameDamageTime;
            AttackCooldown = 0;
            IsAttacking = false;
            TrackedPlayerId = -1;
        }

        public void LockOnPlayer(int id) => TrackedPlayerId = id;

        public void Update(Point? dest)
        {
            AttackCooldown = Math.Min(++AttackCooldown, MaxAttackCooldown);
            if (IsAttacking && attacks.IsAttacking)
                attacks.Update(Position, Rotation);
            else if (IsAttacking && !attacks.IsAttacking)
                IsAttacking = false;

            // follow player if there is a path and if there is a player
            if (TrackedPlayerId != -1)
            {
                Vector2 playerPos = PlayerManager.GetPlayerPosition(TrackedPlayerId);
                float distanceToPlayer = Vector2.DistanceSquared(Position, playerPos);
                bool IsCloseToPlayer = distanceToPlayer < MinSquareDistToPlayer;
                if (dest != null && !IsCloseToPlayer)
                    Move(dest.Value);

                if (IsCloseToPlayer && AttackCooldown >= MaxAttackCooldown)
                {
                    attacks.Attack(AttackType.NormalAttack);
                    IsAttacking = true;
                    AttackCooldown = 0;
                    Dungeon.AttackPlayer(attacks.Hitbox, attacks.GetDamage(AttackType.NormalAttack));
                }

                Rotation = (float)Math.Atan2(playerPos.Y - Position.Y, playerPos.X - Position.X);
                Rotation = (Rotation + MathHelper.Pi / 2) % (MathHelper.Pi * 2);
            }
            
            FramesSinceDamage = MathHelper.Min(FrameDamageTime, FramesSinceDamage+1);
            
            HealthBar.ChangeSize(new Vector2((float)Health * (float)MaxHealth / (float)HealthBarMaxLength, 10));
            HealthBar.ChangePos(new Vector2(Position.X - AnimationManager.CurrentFrameRect.Width / 2, Position.Y - AnimationManager.CurrentFrameRect.Height / 2 - 12));
        }

        public void Move(Point dest)
        {
            dest = new Point(Dungeon.TileSize * dest.X + Dungeon.TileSize/2, Dungeon.TileSize * dest.Y + Dungeon.TileSize/2);

            Vector2 vel = Vector2.Zero;
            vel.X = Math.Sign(dest.X - Position.X) * Speed;
            vel.Y = Math.Sign(dest.Y - Position.Y) * Speed;

            // horizontal collisions
            Rectangle collisionBox = new Rectangle((int)(Hitbox.X + vel.X), Hitbox.Y, Hitbox.Width, Hitbox.Height);
            if (Dungeon.IsValid(collisionBox)) Position = new Vector2(Position.X + vel.X, Position.Y);
            // vertical collisions
            collisionBox = new Rectangle(Hitbox.X, (int)(Hitbox.Y + vel.Y), Hitbox.Width, Hitbox.Height);
            if (Dungeon.IsValid(collisionBox)) Position = new Vector2(Position.X, Position.Y + vel.Y);

        }

        public new void Draw()
        {
            if (FramesSinceDamage < 15)
                base.Draw(Color.Red);
            else
                base.Draw();
            if(IsAttacking && attacks.IsAttacking)
                attacks.Draw();
            HealthBar.Draw(false);
        }
    }
}
