using System.Collections.Generic;
using System.Security.Cryptography;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text.Json;
using System.Diagnostics;

namespace TheDungeonGame
{
    public enum TileType 
    {
        Wall,
        Floor
    }

    public class Tilemap
    {
        private Dictionary<string, TileType> Map { get; set; }
        public Rectangle? CameraBounds { get; private set; }
        public int TileSize { get; private set; }

        public Tilemap()
        {
            Map = new Dictionary<string, TileType>();
            CameraBounds = null;
            TileSize = 100;
        }

        public void Save(string path = "")
        {
            JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true };
            string text = JsonSerializer.Serialize(Map, options);
            Debug.WriteLine(text);
        }

        public string GetLoc(Point point) => $"{point.X};{point.Y}";
        public Point GetPoint(string loc) => new Point(int.Parse(loc.Split(';')[0]), int.Parse(loc.Split(';')[1]));
        public Point GetPos(Point point) => new Point(point.X * TileSize, point.Y * TileSize);
        public Point GetPos(string loc) => GetPos(GetPoint(loc)); 

        public void Add(string loc, TileType tileType) => Map.Add(loc, tileType);
        public void Add(Point point, TileType tileType) => Map.Add(GetLoc(point), tileType);

        public void Remove(string loc) => Map.Remove(loc);
        public void Remove(Point point) => Map.Remove(GetLoc(point));

        public TileType this[string loc]
        {
            get => Map[loc];
        }
        public TileType this[Point point]
        {
            get => Map[GetLoc(point)];
        }

        public void Draw()
        {
            foreach ((string loc, TileType tileType) in Map)
            {
                Texture2D texture = AssetManager.GetTileTexture(tileType);
                Rectangle rect = new Rectangle(GetPos(loc), texture.Bounds.Size);
                Camera.Draw(AssetManager.GetTileTexture(tileType), rect, Color.White);
            }
        }

    }
}
