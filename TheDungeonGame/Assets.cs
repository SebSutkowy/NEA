
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics.Tracing;

namespace TheDungeonGame
{
    public enum SpriteSheets
    {
        Player,
        Enemy,
        Weapons
    }

    public static class AssetManager
    {
        private static Texture2D UnimplementedTexture;
        private static Dictionary<ItemNames, Texture2D> ItemTextures = new Dictionary<ItemNames, Texture2D>();
        private static Dictionary<TileType, Texture2D> TileTextures = new Dictionary<TileType, Texture2D>();
        private static Dictionary<SpriteSheets, Texture2D> SpriteSheets = new Dictionary<SpriteSheets, Texture2D>();

        private static Dictionary<Tilemaps, string> TilemapFileLocations = new Dictionary<Tilemaps, string>()
        {
            {Tilemaps.None, @"" },
            {Tilemaps.Lobby, @"Maps/Lobby.json" },
            {Tilemaps.Hallway1, @"Maps/" }  // add later
        };

        public static void LoadUnimplementedTexture(ContentManager Content, string path)
        {
            UnimplementedTexture = Content.Load<Texture2D>(path);
        }

        public static void LoadItemTexture(ContentManager Content, ItemNames itemName, string fileName)
        {
            ItemTextures.Add(itemName, Content.Load<Texture2D>(fileName));
        }

        public static void LoadTileTexture(ContentManager Content, TileType type, string fileName)
        {
            TileTextures.Add(type, Content.Load<Texture2D>(fileName));
        }

        public static void LoadSpriteSheet(ContentManager Content, SpriteSheets spriteSheet, string fileName)
        {
            SpriteSheets.Add(spriteSheet, Content.Load<Texture2D>(fileName));
        }

        public static Texture2D GetItemTexture(ItemNames itemName)
        {
            if (ItemTextures.ContainsKey(itemName)) return ItemTextures[itemName];
            return UnimplementedTexture;
        }

        public static Texture2D GetTileTexture(TileType type)
        {
            if (TileTextures.ContainsKey(type)) return TileTextures[type];
            return UnimplementedTexture;
        }

        public static Texture2D GetSpriteSheet(SpriteSheets spriteSheet)
        {
            if (SpriteSheets.ContainsKey(spriteSheet)) return SpriteSheets[spriteSheet];
            return UnimplementedTexture;
        }

        public static string GetTilemapFileLocation(Tilemaps tilemap)
        {
            if (TilemapFileLocations.ContainsKey(tilemap)) return TilemapFileLocations[tilemap];
            return "";
        }

    }
}
