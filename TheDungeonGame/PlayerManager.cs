using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;

namespace TheDungeonGame
{
    public static class PlayerManager
    {
        private static Dictionary<int, Player> Players = new Dictionary<int, Player>();

        private static int MaxHealth = 100;
        private static Point PlayerSize = new Point(100);
        private static float PlayerSpeed = 5f;
        private static float PlayerDamage = 5f;

        public static int GetPlayerHealth(int id)
        {
            if (Contains(id)) return Players[id].Health;
            return -1;
        }
            

        public static List<string> GetPlayersHealth()
        {
            List<string> data = new List<string>();
            foreach (int playerId in Players.Keys)
            {
                data.Add($"{playerId} {Players[playerId]}");
            }
            return data;
        }

        public static void AddPlayer(int playerId)
        {
            AnimationManager animationManager = new AnimationManager(AssetManager.GetSpriteSheet(SpriteSheets.Player));
            Animation idleAnimation = new Animation(new Vector2(100f), 0, 1, 0);
            idleAnimation.Pause();
            animationManager.AddAnimation(AnimationNames.Idle, idleAnimation);
            animationManager.ChangeAnimation(AnimationNames.Idle);
            // all class berserker for now
            Player player = new Player(animationManager, new Vector2(0f), 0f, MaxHealth, MaxHealth, PlayerDamage, PlayerSpeed, Classes.Berserker);
            Players.Add(playerId, player);
        }

        public static List<string> OnClientJoin()
        {
            List<string> messages = new List<string>();
            foreach ((int id, Player player) in Players)
            {
                messages.Add(Message.CreateSpawnPlayerMessage(id));
                messages.Add(Message.CreateUpdatePlayerPosMessage(id, player.Position.X, player.Position.Y, player.Rotation));
            }
            return messages;
        }

        public static void UpdatePos(int playerId, Vector2 newPos, float rotation)
        {
            if (Players.ContainsKey(playerId))
            {
                Players[playerId].Position = newPos;
                Players[playerId].Rotation = rotation;
            }

        }

        public static void RemovePlayer(int playerId)
        {
            Players.Remove(playerId);
        }

        public static int Count => Players.Count;
        public static bool Contains(int playerId) => Players.ContainsKey(playerId);

        public static void WriteIds()
        {
            foreach (int id in PlayerManager.Players.Keys)
            {
                Debug.WriteLine(id);
            }
        }

        public static void UpdatePlayers(bool updateLocal=true)
        {
            foreach (int playerId in Players.Keys)
            {
                if (updateLocal && playerId == Network.LocalId || Network.LocalId == -1)
                    Players[playerId].LocalUpdate();

                Players[playerId].Update();
            }
        }

        public static Vector2 GetPlayerPosition(int id) => PlayerManager.Contains(id) ? Players[id].Position : new Vector2(0f);

        public static int GetClosestPlayer(Vector2 pos)
        {
            int closestId = -1;
            float minDist = -1f;
            foreach ((int id, Player player) in Players)
            {
                if (closestId == -1)
                {
                    closestId = id;
                    minDist = Vector2.DistanceSquared(pos, player.Position);
                }
                else if (Vector2.DistanceSquared(pos, player.Position) < minDist)
                {
                    closestId = id;
                    minDist = Vector2.DistanceSquared(pos, player.Position);
                }
            }
            return closestId;
        }

        public static void PlayerAttacked(int id)
        {
            Players[id].Attack(AttackType.NormalAttack);
        }

        public static void Attack(Rectangle hitbox, float damage)
        {
            foreach (Player player in Players.Values)
            {
                if (player.Hitbox.Intersects(hitbox))
                {
                    player.TakeDamage((int)damage);
                }

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
