using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Text.Json;
using System.Diagnostics;
using System.Text.Json.Serialization;
using System.IO;
using TheDungeonGame;
using System;

namespace TheDungeonGame 
{
    public class RectangleData
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public RectangleData()
        {

        }

        public RectangleData(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        public RectangleData(Rectangle rect)
        {
            X = rect.X;
            Y = rect.Y;
            Width = rect.Width;
            Height = rect.Height;
        }

        public Rectangle getRect() => new Rectangle(X, Y, Width, Height);
    }

    public enum TileType 
    {
        None,
        Wall,
        Floor,
    }

    public class Tilemap
    {
        public readonly HashSet<TileType> TraversableTiles = new HashSet<TileType>()
        {
            TileType.Floor
        };

        [JsonInclude]
        private Dictionary<string, TileType> Map { get; set; }
        public Rectangle? CameraBounds { get; private set; }
        public int TileSize { get; private set; }
        [JsonInclude]
        private RectangleData savedBounds { get; set; }

        public Tilemap()
        {
            Map = new Dictionary<string, TileType>();
            CameraBounds = null;
            TileSize = 100;
        }

        public Tilemap(string path)
        {
            Load(path);
        }

        public void Load(string path)
        {
            JsonSerializerOptions options = new JsonSerializerOptions { Converters = { new JsonStringEnumConverter() } };
            string text = FileManager.ReadData(path);
            Tilemap newTilemap = (Tilemap)JsonSerializer.Deserialize<Tilemap>(text, options);
            Debug.WriteLine(newTilemap.savedBounds);
            Map = newTilemap.Map;
            TileSize = newTilemap.TileSize;
            CameraBounds = newTilemap.savedBounds.getRect();
        }

        public void Save(string path = "")
        {
            JsonSerializerOptions options = new JsonSerializerOptions { WriteIndented = true, Converters = { new JsonStringEnumConverter() } };
            string text = JsonSerializer.Serialize(this, options);
            if(path != "")
                File.WriteAllText(path, text);
            Debug.WriteLine(text);
        }

        public string GetLoc(Point point) => $"{point.X};{point.Y}";
        public Point GetPoint(string loc) => new Point(int.Parse(loc.Split(';')[0]), int.Parse(loc.Split(';')[1]));
        public Point GetPos(Point point) => new Point(point.X * TileSize, point.Y * TileSize);
        public Point GetPos(string loc) => GetPos(GetPoint(loc)); 

        public void Add(string loc, TileType tileType) => Map[loc] = tileType;
        public void Add(Point point, TileType tileType) => Map[GetLoc(point)] = tileType;

        public void Remove(string loc) => Map.Remove(loc);
        public void Remove(Point point) => Map.Remove(GetLoc(point));

        public bool Contains(string loc) => Map.ContainsKey(loc);
        public bool Contains(Point pos) => Map.ContainsKey(GetLoc(pos));

        public void SetBounds(Rectangle? bounds)
        {
            CameraBounds = bounds;
        }

        public TileType this[string loc]
        {
            get => Map.ContainsKey(loc) ? Map[loc] : TileType.None;
        }
        public TileType this[Point point]
        {
            get => Map.ContainsKey(GetLoc(point)) ? Map[GetLoc(point)] : TileType.None;
        }

        public bool IsValid(Rectangle bounds)
        {
            // works only for tileSize x tileSize entities
            int left = bounds.Left,
                right = bounds.Right - 1,
                top = bounds.Top,
                bottom = bounds.Bottom - 1;

            int tileLeft = (int)Math.Floor(left / 100f),
                tileRight = (int)Math.Floor(right / 100f),
                tileTop = (int)Math.Floor(top / 100f),
                tileBottom = (int)Math.Floor(bottom / 100f);

            // top left
            if (!TraversableTiles.Contains(this[new Point(tileLeft, tileTop)])) return false;
            // top right
            if (!TraversableTiles.Contains(this[new Point(tileRight, tileTop)])) return false;
            // bottom left
            if (!TraversableTiles.Contains(this[new Point(tileLeft, tileBottom)])) return false;
            // bottom right
            if (!TraversableTiles.Contains(this[new Point(tileRight, tileBottom)])) return false;
            return true;
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
