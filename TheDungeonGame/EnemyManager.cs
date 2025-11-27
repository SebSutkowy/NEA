using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace TheDungeonGame
{
    public static class EnemyManager
    {
        public static List<Entity> Enemies = new List<Entity>();

        public static void Attack(Rectangle hitbox, float damage)
        {
            foreach (Entity enemy in Enemies)
            {
                if (enemy.Hitbox.Intersects(hitbox))
                {
                    enemy.TakeDamage((int)damage);
                }
            }
        }
    }
    
}
