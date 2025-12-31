
namespace TheDungeonGame
{
    public class Door
    {
        public string Loc { get; private set; }
        public Tilemaps Tilemap { get; private set; }
        public Tilemaps DestinationTilemap { get; private set; }
        public string DestinationLoc { get; private set; }
    }
}
