using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TheDungeonGame
{
    public static class Dungeon
    {
        private static Dictionary<Tilemaps, Tilemap> ActiveTilemaps = new Dictionary<Tilemaps, Tilemap>();
        public static Tilemaps CurrentTilemapName { get; private set; } = Tilemaps.None;
        private static Tilemap CurrentTilemap => ActiveTilemaps[CurrentTilemapName];
        public static int TileSize => CurrentTilemap.TileSize;
        public static bool IsActive => CurrentTilemapName != Tilemaps.None;
        public static Rectangle? CameraBounds => CurrentTilemap.CameraBounds;
        public static Dictionary<(Tilemaps, string), (Tilemaps, string)> Doors = new Dictionary<(Tilemaps, string), (Tilemaps, string)>(); // door -> destination
        private static bool ChangingTilemap = false;
        private static Tilemaps newTilemap;
        private static string newLoc;
        private static Dictionary<Tilemaps, EnemyManager> EnemyManagers = new Dictionary<Tilemaps, EnemyManager>();
        private static Stack<(Rectangle, float)> Attacks = new Stack<(Rectangle, float)>();
        private static EnemyManager EnemyManager => EnemyManagers[CurrentTilemapName];

        public static bool IsValid(Rectangle Bounds) => CurrentTilemap.IsValid(Bounds);
        
        public static void Clear()
        {
            ActiveTilemaps.Clear();
            CurrentTilemapName = Tilemaps.None;
            Doors.Clear();
            ChangingTilemap = false;
            EnemyManagers.Clear();
            Attacks.Clear();
        }

        public static Point GetTilemapPos(Point pos)
        {
            Point res = pos;
            res.X = (int)Math.Floor((float)res.X / (float)TileSize);
            res.Y = (int)Math.Floor((float)res.Y / (float)TileSize);
            return res;
        }

        public static void AddTilemap(Tilemaps newTilemap)
        {
            Tilemap tilemap = new Tilemap();
            string loc  = AssetManager.GetTilemapFileLocation(newTilemap);
            tilemap.Load(loc);
            ActiveTilemaps.Add(newTilemap, tilemap);
            EnemyManagers.Add(newTilemap, new EnemyManager());
        }

        public static void UseDoor(Tilemaps tilemap, string loc)
        {
            if (!Doors.ContainsKey((tilemap, loc)))
            {
                Debug.WriteLine("Couldn't find door");
                return;
            }
            ChangingTilemap = true;
            (newTilemap, newLoc) = Doors[(tilemap, loc)];
            newLoc = ActiveTilemaps[newTilemap].GetDoorLoc(newLoc); 
        }

        public static void Update()
        {
            EnemyManager.Update();
            while (Attacks.Count > 0)
            {
                (Rectangle hitbox, float damage) = Attacks.Pop();
                PlayerManager.Attack(hitbox, damage);
            }

            foreach (NPCId id in CurrentTilemap.NPCs.Values)
            {
                AssetManager.GetNPC(id).Update();
            }
        }

        public static bool IsInteractive(string loc)
        {
            if (CurrentTilemap.NPCs.ContainsKey(loc)) return true;
            return false;
        }

        public static bool IsTraversable(string loc) => CurrentTilemap.IsTraversable(loc);
        public static bool IsTraversable(Point point) => CurrentTilemap.IsTraversable(point);

        public static void Interact(string loc)
        {
            CurrentTilemap.Interact(loc);
        }


        public static void CheckIfChangingTilemap(Player player)
        {
            if (ChangingTilemap)
            {
                CurrentTilemapName = newTilemap;
                Point newPos = CurrentTilemap.GetPos(newLoc);
                player.SetPos(new Vector2(newPos.X + CurrentTilemap.TileSize/2, newPos.Y + CurrentTilemap.TileSize/2));
                
                ChangingTilemap = false;
                newTilemap = Tilemaps.None;
                newLoc = "";
            }
        }


        public static void ConnectDoors(Tilemaps tilemap1, string loc1, Tilemaps tilemap2, string loc2)
        {
            Doors[(tilemap1, loc1)] = (tilemap2, loc2);
            Doors[(tilemap2, loc2)] = (tilemap1, loc1);
        }
        
        public static void ChangeTilemap(Tilemaps tilemap)
        {
            CurrentTilemapName = tilemap;
        }

        public static void GenerateMap(int size, int seed) // size must be an odd integer
        {
            Random rng = new Random(seed);
            HashSet<int> visited = new HashSet<int>();
            Dictionary<int, int> path = new Dictionary<int, int>();
            HashSet<int> neighbours = new HashSet<int>
            {
                -1,
                1,
                -size,
                size,
            };

            int start;

            // getters
            (int, int) GetArrayPos(int pos) => ((pos / size), (pos % size));
            int GetOverallPos(int row, int col) => (row * size + col);
            

            // create size x size grid
            int[,] grid = new int[size, size];
            // place in the special rooms
            int n = (size - 1) / 2;
            int nextNum = n;
            int row, col;
            // create list of choices for special rooms locations
            List<int> order = Enumerable.Range(0, n * n).ToList<int>();

            // apply fisher-yates algorithm to shuffle elements for as many special rooms
            int temp;
            int i = 0;
            for (; i < Tilemap.SpecialRooms.Count; i++)
            {
                nextNum = rng.Next(i, n * n);
                temp = order[nextNum];
                order[nextNum] = order[i];
                order[i] = temp;
            }

            i = 0;
            foreach (Tilemaps tilemap in Tilemap.SpecialRooms)
            {
                row = 2 * (order[i] % (n + 1));
                col = 2 * (order[i] / (n + 1));
                grid[row, col] = (int)tilemap;
                if (tilemap == Tilemaps.Spawn)
                    start = GetOverallPos(row, col);
                visited.Add(GetOverallPos(row, col));
                i++;
            }



            // do recursive maze gen to create paths
            int current = 0;
            int next;
            List<int> choices = new List<int>();
            while (visited.Count < size*size)
            {
                choices.Clear();
                visited.Add(current);
                (row, col) = GetArrayPos(current);
                grid[row, col] = -1;
                // get choices
                // check if on right edge
                if (current % size != size - 1)
                {
                    if(!visited.Contains(current + 1))
                        choices.Add(current + 1);
                }
                // check if on left edge
                if (current % size != 0)
                {
                    if(!visited.Contains(current - 1))
                        choices.Add(current - 1);
                }
                // check if on top edge
                if ((int)(current / size) != 0)
                {
                    if (!visited.Contains(current - size))
                        choices.Add(current - size);
                }
                // check if on bottom edge
                if ((int)(current / size) != size - 1)
                {
                    if (!visited.Contains(current + size))
                        choices.Add(current + size);
                }

                // check if there where any free tiles
                if (choices.Count == 0)
                {
                    current = path[current]; 
                    continue;
                }

                // pick a random one

                next = choices[rng.Next(0, choices.Count)];
                Debug.WriteLine($"{GetArrayPos(current)} --> {GetArrayPos(next)}");
                path.Add(next, current);
                current = next;
            }




            // go through the maze paths and place rooms where you can

            // any left alone spots will be 1 x 1 rooms

            // Prints the maze
            Debug.WriteLine("#########");
            for (i = 0; i < size; i++)
            {
                Debug.Write("#");
                for (int j = 0; j < size; j++)
                {
                    Debug.Write(grid[i, j]);
                }
                Debug.Write("#\n");
            }
            Debug.WriteLine("#########\n");
        }

        #region Enemy Manager Methods
        public static void Attack(int playerId, Rectangle hitbox, float damage)
        {
            EnemyManager.Attack(playerId, hitbox, damage);
        }

        public static void EnemyKilled(int playerId)
        {

        }

        public static void AttackPlayer(Rectangle hitbox, float damage)
        {
            // replace with player manager when online
            Attacks.Push((hitbox, damage));
        }


        public static void AddEnemy(Enemy enemy)
        {
            EnemyManager.AddEnemy(enemy);
        }
        
        public static void AddEnemy(Tilemaps tilemap, Enemy enemy)
        {
            if (EnemyManagers.ContainsKey(tilemap)) EnemyManagers[tilemap].AddEnemy(enemy);
        }

        #endregion

        public static void Draw()
        {
            CurrentTilemap.Draw();
            EnemyManager.Draw();
        }


        
    }
}
