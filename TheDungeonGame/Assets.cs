
using Microsoft.VisualBasic.FileIO;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Tracing;
using System.Net.NetworkInformation;
using System.Text.Json;

namespace TheDungeonGame
{
    public enum SpriteSheets
    {
        Entity,
        Player,
        Enemy,
        Weapons
    }

    public enum FileTypes
    {
        Tilemap,
        Dialogues
    }

    public static class AssetManager
    {
        private static readonly Dictionary<FileTypes, string> Prefixes = new Dictionary<FileTypes, string>()
        {
            {FileTypes.Tilemap, @"Maps/" },
            {FileTypes.Dialogues, @"Dialogue/" }
        };

        private static string AddPrefix(FileTypes fileType, string loc) => $"{Prefixes[fileType]}{loc}";

        private static Texture2D UnimplementedTexture;
        private static Dictionary<ItemNames, Texture2D> ItemTextures = new Dictionary<ItemNames, Texture2D>();
        private static Dictionary<TileType, Texture2D> TileTextures = new Dictionary<TileType, Texture2D>();
        private static Dictionary<SpriteSheets, Texture2D> SpriteSheets = new Dictionary<SpriteSheets, Texture2D>();
        private static Dictionary<NPCId, List<string>> Dialogues = new Dictionary<NPCId, List<string>>();
        private static Dictionary<NPCId, NPC> NPCs = new Dictionary<NPCId, NPC>();

        private static readonly Dictionary<Tilemaps, string> TilemapFileLocations = new Dictionary<Tilemaps, string>()
        {
            {Tilemaps.None, AddPrefix(FileTypes.Tilemap, "") },
            {Tilemaps.Lobby, AddPrefix(FileTypes.Tilemap, "Lobby.json") },
            {Tilemaps.Hallway1, AddPrefix(FileTypes.Tilemap, "Hallway1.json") }  
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

        public static void LoadDialogue(NPCId id, string fileName)
        {
            string text = FileManager.ReadData(AddPrefix(FileTypes.Dialogues, fileName));
            List<string> dialogue = JsonSerializer.Deserialize<List<string>>(text);
            Dialogues.Add(id, dialogue);
        }

        public static void LoadNPC(NPCId id, NPC npc)
        {
            NPCs.Add(id, npc);
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

        public static List<string> GetDialogue(NPCId id)
        {
            Debug.WriteLine(Dialogues.ContainsKey(id));
            if (Dialogues.ContainsKey(id)) return Dialogues[id];
            return new List<string>();
        }

        public static NPC GetNPC(NPCId id)
        {
            if (NPCs.ContainsKey(id)) return NPCs[id];
            return null;
        }

    }
}
