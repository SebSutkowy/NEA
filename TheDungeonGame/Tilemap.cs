using System.Collections.Generic;
using Microsoft.Xna.Framework;

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
        public Rectangle CameraBounds { get; private set; }

        public Tilemap()
        {
            
        }

        public TileType this[string loc]
        {
            get => Map[loc];
        }
    }
}
