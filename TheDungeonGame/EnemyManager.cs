using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;

namespace TheDungeonGame
{
    public static class EnemyManager
    {
        public static List<Enemy> Enemies = new List<Enemy>();

        public static void AddEnemy(Enemy enemy)
        {
            Enemies.Add(enemy);
        }

        public static void Update()
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

        public static void Attack(Rectangle hitbox, float damage)
        {
            foreach (Enemy enemy in Enemies)
            {
                if (enemy.Hitbox.Intersects(hitbox))
                {
                    enemy.TakeDamage((int)damage);
                }
            }
        }

        public static void Draw()
        {
            foreach (Enemy enemy in Enemies)
            {
                enemy.Draw();
            }
        }
    }
    
}
