using System.Collections.Generic;
using System.Drawing;

namespace TheDungeonGame
{
    public static class PlayerManager
    {
        private static Dictionary<int, Player> Players = new Dictionary<int, Player>();

        private static Point PlayerSize = new Point(100);
        private static float PlayerSpeed = 5f;

        public static List<string> GetPlayerHealth()
        {
            List<string> data = new List<string>();
            foreach (int playerId in Players.Keys)
            {
                data.Add($"{playerId} {Players[playerId]}");
            }
            return data;
        }

        public static void RemovePlayer(int playerId)
        {
            Players.Remove(playerId);
        }

        public static bool Contains(int playerId) => Players.ContainsKey(playerId);

        public static void UpdatePlayers()
        {
            foreach (int playerId in Players.Keys)
            {
                Players[playerId].Update();
            }

        }

        public static void Reset()
        {
            Players = new Dictionary<int, Player>();
        }

        public static void Draw()
        {
            foreach (Player player in Players.Values)
            {
                player.Draw();
            }
        }
    }
}
