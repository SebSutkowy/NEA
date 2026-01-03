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
        private static EnemyManager EnemyManager => EnemyManagers[CurrentTilemapName];

        public static bool IsValid(Rectangle Bounds) => CurrentTilemap.IsValid(Bounds);
        
        public static void Clear()
        {
            ActiveTilemaps.Clear();
            CurrentTilemapName = Tilemaps.None;
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

        #region Enemy Manager Methods
        public static void Attack(Rectangle hitbox, float damage)
        {
            EnemyManager.Attack(hitbox, damage);
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
