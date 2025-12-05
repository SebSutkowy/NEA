using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TheDungeonGame
{
    public class Entity : Sprite
    {
        public int MaxHealth { get; private set; }
        public int Health { get; private set; }
        public float Damage { get; private set; }
        public float Speed { get; private set; } = 5.0f;
        protected int FramesSinceDamage { get; set; }


        public Entity(Texture2D texture, Vector2 position, float rotation, int maxHealth, int health, float damage, float speed) : base(texture, position, rotation)
        {
            MaxHealth = maxHealth;
            Health = health;
            Damage = damage;
            Speed = speed;
            FramesSinceDamage = 0;
        }

        public void TakeDamage(int damage)
        {
            Health = Math.Clamp(0, Health-damage, MaxHealth);
            FramesSinceDamage = 0;
        }

        public void Heal(int healAmount)
        {
            TakeDamage(-healAmount);
        }
    }
}
