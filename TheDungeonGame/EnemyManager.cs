using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Transactions;

namespace TheDungeonGame
{
    public enum PathfindDirection
    {
        Up,
        Down,
        Left,
        Right,
    }

    public class PathfindingPath
    {
        private Dictionary<Point, Point?> path { get; set; } 
        private HashSet<Point> Visited { get; set; }
        public int Size => Visited.Count;

        public PathfindingPath()
        {
            path = new Dictionary<Point, Point?>();
            Visited = new HashSet<Point>();
        }

        public PathfindingPath(Point start)
        {
            path = new Dictionary<Point, Point?>();
            path.Add(start, null);
            Visited = new HashSet<Point>();
            Visited.Add(start);
        }

        public bool Contains(Point tile) => Visited.Contains(tile);

        public void Add(Point tile, Point prev)
        {
            path.Add(tile, prev);
            Visited.Add(tile);
            if (!Visited.Contains(prev))
                Visited.Add(prev);
        }

        public void Clear() => path.Clear();

        public Queue<Point> GetPath(Point start)
        {
            Queue<Point> result = new Queue<Point>();
            if (!path.ContainsKey(start)) return result;
            Point? current = start;
            Point? next(Point? cur) => path[cur.Value];
            while (next(current) != null)
            {
                result.Enqueue((Point)next(current));
                current = next(current);
            }
            return result;
        }

        public Point? GetNextPoint(Point start)
        {
            if (Contains(start)) return path[start];
            return null;
        }
    }

    public class EnemyManager
    {
        private List<Enemy> Enemies { get; set; }
        private Dictionary<Point, PathfindingPath> PathfindCache { get; set; }

        private readonly Dictionary<PathfindDirection, Point> Neighbours = new Dictionary<PathfindDirection, Point>()
        {
            {PathfindDirection.Left,  new Point(-1,  0) },
            {PathfindDirection.Right, new Point( 1,  0) },
            {PathfindDirection.Up,    new Point( 0, -1) },
            {PathfindDirection.Down,  new Point( 0,  1) },

        };

        public EnemyManager()
        {
            Enemies = new List<Enemy>();
            PathfindCache = new Dictionary<Point, PathfindingPath>();
        }

        public void AddEnemy(Enemy enemy)
        {
            Enemies.Add(enemy);
        }

        public void Update()
        {
            Enemy enemy;
            
            //Pathfind(player);
            //Point playerPos = new Point((int)player.Position.X, (int)player.Position.Y);
            //playerPos = Dungeon.GetTilemapPos(playerPos);
            for (int i = Enemies.Count - 1; i >= 0; i--)
            {
                enemy = Enemies[i];
                if (!enemy.IsAlive)
                {
                    Debug.WriteLine($"Enemy died at {enemy.Position}, spawn basic loot");
                    Enemies.RemoveAt(i);
                }
                else
                {
                    // check if enemies tracked player exists
                    int id = enemy.TrackedPlayerId;
                    if(!PlayerManager.Contains(id))
                    {
                        // if not then find the next closest player
                        id = PlayerManager.GetClosestPlayer(enemy.Position);
                    }
                    // make the enemy track that player
                    enemy.LockOnPlayer(id);
                    Point? dest = GetNextPoint(enemy);
                    enemy.Update(dest);
                }

                    
            }
        }

        public Point? GetNextPoint(Enemy enemy)
        {
            // get enemy position
            Point enemyPos = new Point((int)enemy.Position.X, (int)enemy.Position.Y);
            enemyPos = Dungeon.GetTilemapPos(enemyPos);
            // get player pos
            Vector2 playerPos = PlayerManager.GetPlayerPosition(enemy.TrackedPlayerId);
            Point playerTilemapPos = Dungeon.GetTilemapPos(new Point((int)playerPos.X, (int)playerPos.Y));
            // check if it exists in the pathfind cache
            Point? dest = null;
            if (!PathfindCache.ContainsKey(playerTilemapPos))
            {
                Pathfind(playerTilemapPos);
            }
            dest = PathfindCache[playerTilemapPos].GetNextPoint(enemyPos);
            // if it exists update the enemy with it
            // if not update the player with null destination
            return dest;
        }

        public void Pathfind(Point start)
        {
            // check if in cache
            if (PathfindCache.ContainsKey(start))
            {
                return;
            }

            PathfindingPath path = new PathfindingPath(start);
            Point currentTile = start;
            Queue<Point> next = new Queue<Point>();

            do
            {
                foreach (Point dir in Neighbours.Values)
                {
                    Point nextTile = dir + currentTile;
                    if (!path.Contains(nextTile) && Dungeon.IsTraversable(Tilemap.GetLoc(nextTile)))
                    {
                        next.Enqueue(nextTile);
                        path.Add(nextTile, currentTile);
                    }
                }
                currentTile = next.Dequeue();
            } while (path.Size < 100 && next.Count > 0);
            PathfindCache[start] = path;
        }

        public void Attack(int playerId, Rectangle hitbox, float damage)
        {
            foreach (Enemy enemy in Enemies)
            {
                if (enemy.Hitbox.Intersects(hitbox))
                {
                    enemy.TakeDamage((int)damage);
                    if (enemy.Health <= 0)
                    {
                        Dungeon.EnemyKilled(playerId);
                    }

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
