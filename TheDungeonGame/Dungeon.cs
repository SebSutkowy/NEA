using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Remoting;
using System.Text;
using System.Threading.Tasks;

namespace TheDungeonGame
{
    public static class Dungeon
    {
        private static Dictionary<int, Tilemap> ActiveTilemaps = new Dictionary<int, Tilemap>();
        public static int CurrentTilemapId { get; private set; } = 0;
        private static Tilemap CurrentTilemap => ActiveTilemaps[CurrentTilemapId];
        public static int TileSize => CurrentTilemap.TileSize;
        public static bool IsActive => CurrentTilemapId != 0;
        public static Rectangle? CameraBounds => CurrentTilemap.CameraBounds;
        public static Dictionary<(int, string), (int, string)> Doors = new Dictionary<(int, string), (int, string)>(); // door -> destination
        private static bool ChangingTilemap = false;
        private static int newTilemapId;
        private static string newLoc;
        private static Dictionary<int, EnemyManager> EnemyManagers = new Dictionary<int, EnemyManager>();
        private static Stack<(Rectangle, float)> Attacks = new Stack<(Rectangle, float)>();
        private static EnemyManager EnemyManager => EnemyManagers[CurrentTilemapId];

        public static bool IsValid(Rectangle Bounds) => CurrentTilemap.IsValid(Bounds);
        
        public static void Clear()
        {
            ActiveTilemaps.Clear();
            CurrentTilemapId = 0;
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

        public static void AddTilemap(int newTilemapId, Tilemaps newTilemap)
        {
            string loc  = AssetManager.GetTilemapFileLocation(newTilemap);
            Tilemap tilemap = new Tilemap(loc, newTilemapId);
            ActiveTilemaps.Add(newTilemapId, tilemap);
            EnemyManagers.Add(newTilemapId, new EnemyManager());
        }

        public static void UseDoor(int tilemapId, string loc)
        {
            Debug.WriteLine($"Trying door {(tilemapId, loc)}");
            if (!Doors.ContainsKey((tilemapId, loc)))
            {
                Debug.WriteLine("Couldn't find door");
                return;
            }
            ChangingTilemap = true;
            (newTilemapId, newLoc) = Doors[(tilemapId, loc)];
            newLoc = ActiveTilemaps[newTilemapId].GetDoorLoc(newLoc); 
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
                CurrentTilemapId = newTilemapId;
                Point newPos = CurrentTilemap.GetPos(newLoc);
                player.SetPos(new Vector2(newPos.X + CurrentTilemap.TileSize/2, newPos.Y + CurrentTilemap.TileSize/2));
                
                ChangingTilemap = false;
                newTilemapId = 0;
                newLoc = "";
            }
        }


        public static void ConnectDoors(int tilemap1, string loc1, int tilemap2, string loc2)
        {
            Doors[(tilemap1, loc1)] = (tilemap2, loc2);
            Doors[(tilemap2, loc2)] = (tilemap1, loc1);
            Debug.WriteLine($"{(tilemap1, loc1)} <=> {(tilemap2, loc2)}");
        }
        
        public static void ChangeTilemap(int tilemapId)
        {
            CurrentTilemapId = tilemapId;
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
            List<int> order = Enumerable.Range(1, n * n).ToList<int>();

            // apply fisher-yates algorithm to shuffle elements for as many special rooms
            // remove 0 so no special rooms can appear there -> makes linking doors avoid special case where a special room already is connected
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
                {
                    AddTilemap(GetOverallPos(row, col), tilemap);
                    CurrentTilemapId = GetOverallPos(row, col);
                    Debug.WriteLine($"Current tilemap: {CurrentTilemapId}");
                }
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
                if(!visited.Contains(current))
                {
                    visited.Add(current);
                    AddTilemap(current, Tilemaps.BasicRoom);
                }
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

            // put in the rooms from the maze and link the doors 
            int index;
            for(row = 0; row < size; row++)
            {
                for(col = 0; col < size; col++)
                {
                    index = GetOverallPos(row, col);
                    if (path.ContainsKey(index))
                    {
                        // all rooms here are basic rooms so door positions are known
                        // link the doors from the path to this door
                        if(index - path[index] == -1) // right 
                        {
                            ConnectDoors(index, "10;0", path[index], "-10;0");
                        }
                        else if (index - path[index] == 1) // left 
                        {
                            ConnectDoors(index, "-10;0", path[index], "10;0");

                        }
                        else if (index - path[index] == size) // top 
                        {
                            ConnectDoors(index, "0;-10", path[index], "0;10");
                        }
                        else if (index - path[index] == -size) // bottom
                        {
                            ConnectDoors(index, "0;10", path[index], "0;-10");
                        }
                    }
                    else
                    {
                        // not found so must be a special room
                        // link door from room to a random adjacent room 
                        choices.Clear();
                        if (index % size != size - 1)
                        {
                            choices.Add(index + 1);
                        }
                        // check if on left edge
                        if (index % size != 0)
                        {
                            choices.Add(index - 1);
                        }
                        // check if on top edge
                        if ((int)(index / size) != 0)
                        {
                            choices.Add(index - size);
                        }
                        // check if on bottom edge
                        if ((int)(index / size) != size - 1)
                        {
                            choices.Add(index + size);
                        } 
                        next = choices[rng.Next(choices.Count)];
                        (int id, string loc) neighbour = (0, "");

                        if(index - next == -1) // right 
                        {
                            neighbour = (next, "-10;0");
                            if (grid[row, col] == (int)Tilemaps.Spawn)
                            {
                                ConnectDoors(index, "5;0", neighbour.id, neighbour.loc);
                            }
                        }
                        else if (index - next == 1) // left 
                        {
                            neighbour = (next, "10;0");
                            if (grid[row, col] == (int)Tilemaps.Spawn)
                            {
                                ConnectDoors(index, "-5;0", neighbour.id, neighbour.loc);
                            }
                        }
                        else if (index - next == size) // top 
                        {
                            neighbour = (next, "0;10");
                            if (grid[row, col] == (int)Tilemaps.Spawn)
                            {
                                ConnectDoors(index, "0;-5", neighbour.id, neighbour.loc);
                            }
                        }
                        else if (index - next == -size) // bottom
                        {
                            neighbour = (next, "0;-10");
                            if (grid[row, col] == (int)Tilemaps.Spawn)
                            {
                                ConnectDoors(index, "0;5", neighbour.id, neighbour.loc);
                            }
                        }
                        switch(grid[row, col])
                        {
                            case (int)Tilemaps.Boss:
                                ConnectDoors(index, "0;5", neighbour.id, neighbour.loc);
                                break;
                            case (int)Tilemaps.MazePuzzle:
                                ConnectDoors(index, "0;6", neighbour.id, neighbour.loc);
                                break;
                            case (int)Tilemaps.TowerOfHanoiPuzzle:
                                ConnectDoors(index, "0;6", neighbour.id, neighbour.loc);
                                break;
                            default:
                                break;
                        }

                        
                    }
                }
            }


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
        
        public static void AddEnemy(int tilemapId, Enemy enemy)
        {
            if (EnemyManagers.ContainsKey(tilemapId)) 
                EnemyManagers[tilemapId].AddEnemy(enemy);
        }

        #endregion

        public static void Draw()
        {
            CurrentTilemap.Draw();
            EnemyManager.Draw();
        }


        
    }
}
