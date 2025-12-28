using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
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

        public static Rectangle? CameraBounds => CurrentTilemap.CameraBounds;

        public static bool IsValid(Rectangle Bounds) => CurrentTilemap.IsValid(Bounds);
        
        public static void Clear()
        {
            ActiveTilemaps.Clear();
            CurrentTilemapName = Tilemaps.None;
        }

        public static void AddTilemap(Tilemaps newTilemap)
        {
            Tilemap tilemap = new Tilemap();
            string loc = AssetManager.GetTilemapFileLocation(newTilemap);
            tilemap.Load(loc);
            ActiveTilemaps.Add(newTilemap, tilemap);
        }

        
        public static void ChangeTilemap(Tilemaps tilemap)
        {
            CurrentTilemapName = tilemap;
        }

        public static void Draw()
        {
            CurrentTilemap.Draw();
        }


        
    }
}
