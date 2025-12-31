using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;

namespace TheDungeonGame
{
    public class EnemyManager
    {
        private List<Enemy> Enemies { get; set; }

        public EnemyManager()
        {
            Enemies = new List<Enemy>();
        }

        public void AddEnemy(Enemy enemy)
        {
            Enemies.Add(enemy);
        }

        public void Update()
        {
            Enemy enemy;
            for (int i = Enemies.Count - 1; i >= 0; i--)
            {
                enemy = Enemies[i];
                if (enemy.Health <= 0)
                {
                    Debug.WriteLine($"Enemy died at {enemy.Position}, spawn basic loot");
                    Enemies.RemoveAt(i);
                }
                else
                    enemy.Update();
                    
            }
        }

        public void Attack(Rectangle hitbox, float damage)
        {
            foreach (Enemy enemy in Enemies)
            {
                if (enemy.Hitbox.Intersects(hitbox))
                {
                    enemy.TakeDamage((int)damage);
                }
            }
        }

        public void Draw()
        {
            foreach (Enemy enemy in Enemies)
            {
                enemy.Draw();
            }
        }
    }
    
}
