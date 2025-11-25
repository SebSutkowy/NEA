using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace TheDungeonGame
{
    public class Entity : Sprite
    {
        private int MaxHealth { get; set; }
        public int Health { get; private set; }
        public float Damage { get; private set; }
        public float Speed { get; private set; } = 5.0f;

        public Entity(Texture2D texture, Vector2 position, float rotation, int maxHealth, int health, float damage, float speed) : base(texture, position, rotation)
        {
            Health = health;
            Damage = damage;
            Speed = speed;
        }

        public void TakeDamage(int damage)
        {
            Health = Math.Clamp(0, Health-damage, MaxHealth);
        }

        public void Heal(int healAmount)
        {
            TakeDamage(-healAmount);
        }
    }
}
