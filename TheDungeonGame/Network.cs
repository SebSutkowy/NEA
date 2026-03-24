using LiteNetLib;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;

namespace TheDungeonGame
{
    public enum ConnectionType
    {
        None,
        Client,
        Host
    }

    public static class Network
    {
        private static ConnectionType NetworkMode = ConnectionType.None;
        public const float TICK_RATE = 60.0f;
        public const float TIME_BETWEEN_TICKS = 1f / TICK_RATE;
        private static float deltaTime;
        public static float GetDelta() => deltaTime;

        private static int _localId = -1;
        public static int LocalId => _localId;

        private static int serverPort = 9050;
        private static string serverIp = "localhost";
        private static string serverKey = "gameKey";

        private static Client Client = new Client();
        private static Server Server = new Server();

        private static Queue<string> Messages = new Queue<string>();
        private static string LastMessage = string.Empty;

        private static string LastTranslatedMessage = string.Empty;
        public static void SetTranslatedMessage(string s) => LastTranslatedMessage = s;
        public static string GetTranslatedMessage() => LastTranslatedMessage;

        private static Dictionary<int, string> ConnectedClients = new Dictionary<int, string>();
        public static Dictionary<int, string> GetConnections => ConnectedClients;
        private static HashSet<int> MutedConnections = new HashSet<int>();
        public static bool IsMuted(int id) => MutedConnections.Contains(id);
        public static void Mute(int id)
        {
            MutedConnections.Add(id);
            AddMessage($"[CHAT] Muted {id}");
        }
        public static void Unmute(int id)
        {
            MutedConnections.Remove(id);
            AddMessage($"[CHAT] Unmuted {id}");
        }

        public static ConnectionType GetMode() => NetworkMode;

        public static void AddMessage(string message)
        {
            LastMessage = message;
            Messages.Enqueue(message);
            while (Messages.Count > 10)
            {
                Messages.Dequeue();
            }
            Debug.WriteLine(message);
        }

        public static Queue<string> GetMessages() => Messages;
        public static string GetLastMessage() => LastMessage;

        public static void SetLocalId(int localId)
        {
            _localId = localId;
        }

        public static void ChangeNetworkMode(ConnectionType mode)
        {
            if (NetworkMode != ConnectionType.None) return;
            switch (mode)
            {
                case ConnectionType.Client:
                    NetworkMode = ConnectionType.Client;
                    Client.StartClient(serverIp, serverPort, serverKey);
                    break;
                case ConnectionType.Host:
                    NetworkMode = ConnectionType.Host;
                    Server.StartServer(serverPort);
                    break;
                case ConnectionType.None:
                    ConnectedClients.Clear();
                    Server = new Server();
                    Client = new Client();
                    _localId = -1;
                    break;
            }

        }

        public static void SetTick(int tick)
        {
            Client.SetTick(tick);
        }

        public static void AddClient(int id, string username)
        {
            if (!ConnectedClients.ContainsKey(id)) ConnectedClients.Add(id, username);
        }

        public static void OnClientJoin(int id, string username)
        {
            if (NetworkMode == ConnectionType.Client && _localId == -1)
                _localId = id;
            if (ConnectedClients.ContainsKey(id))
                return;
            ConnectedClients.Add(id, username);
            AddMessage($"[NETWORK] {username} Joined.");
        }

        public static void OnClientDisconnect(int id)
        {
            ConnectedClients.Remove(id);
            AddMessage($"[NETWORK] Client {id} Disconnected.");
        }

        public static void PlayerAttacked(int id)
        {
            string message = Message.CreatePlayerAttackMessage(id);
            if (NetworkMode == ConnectionType.Host)
            {
                Server.SendGlobalMessage(message);
            }
            else if (NetworkMode == ConnectionType.Client)
            {
                Client.SendMessage(message);
            }
        }

        public static void SendMessage(string message, int id=-1)
        {
            switch (NetworkMode)
            {
                case ConnectionType.Client:
                    Client.SendMessage(message);
                    break;
                case ConnectionType.Host:
                    if (id == -1)
                        Server.SendGlobalMessage(message);
                    else
                        Server.SendMessage(id, message);
                    break;
            }
        }

        public static void SendExclusiveMessage(string message, HashSet<int> exlcudedPeers)
        {
            Server.SendExclusiveMessage(message, exlcudedPeers);
        }

        public static void SendExclusiveMessage(string message, HashSet<NetPeer> exlcudedPeers)
        {
            Server.SendExclusiveMessage(message, exlcudedPeers);
        }

        public static void CreatePlayer(int id, string username, string password, string salt) => Server.CreatePlayer(id, username, password, salt);

        public static void VerifyLogin(int id, string username, string password) => Server.VerifyLogin(id, username, password); 

        public static void Update(GameTime gameTime)
        {
            deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            switch (NetworkMode)
            {
                case ConnectionType.None:
                    break;
                case ConnectionType.Client:
                    Client.Update();
                    break;
                case ConnectionType.Host:
                    Server.Update();
                    break;
            }
        }

        public static void Stop()
        {
            if (NetworkMode == ConnectionType.Host)
                Server.Stop();
            else if (NetworkMode == ConnectionType.Client)
                Client.Stop();
            ChangeNetworkMode(ConnectionType.None);
        }
    }
}
