
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;

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
        private static Dictionary<NormalAttacks, Animation> NormalAttackAnimations = new Dictionary<NormalAttacks, Animation>();
        private static Dictionary<SpecialAttacks, Animation> SpecialAttackAnimations = new Dictionary<SpecialAttacks, Animation>();

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

        public static void LoadNormalAttackAnimation(NormalAttacks attack, Animation animation)
        {
            NormalAttackAnimations.Add(attack, animation);
        }
        public static void LoadSpecialAttackAnimation(SpecialAttacks attack, Animation animation)
        {
            SpecialAttackAnimations.Add(attack, animation);
        }

        public static Texture2D GetItemTexture(ItemNames itemName)
        {
            if(ItemTextures.ContainsKey(itemName)) return ItemTextures[itemName];
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

        public static Animation GetNormalAttackAnimation(NormalAttacks normalAttack) => NormalAttackAnimations[normalAttack]; // lacking unimplemented error

        public static Animation GetSpecialAttackAnimation(SpecialAttacks specialAttacks) => SpecialAttackAnimations[specialAttacks];
    }
}
