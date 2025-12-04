
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

namespace TheDungeonGame
{
    public static class AssetManager
    {
        private static Texture2D UnimplementedTexture;
        private static Dictionary<ItemNames, Texture2D> ItemTextures = new Dictionary<ItemNames, Texture2D>();

        public static void LoadUnimplementedTexture(ContentManager Content, string path)
        {
            Content.Load<Texture2D>(path);
        }

        public static void LoadItemTexture(ContentManager Content, ItemNames itemName, string fileName)
        {
            ItemTextures.Add(itemName, Content.Load<Texture2D>(fileName));
        }

        public static Texture2D GetItemTexture(ItemNames itemName)
        {
            if(ItemTextures.ContainsKey(itemName)) return ItemTextures[itemName];
            return UnimplementedTexture;
        }
    }
}
