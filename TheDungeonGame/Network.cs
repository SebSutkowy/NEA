
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

        public static string LastMessage { get; private set; } = ""; 

        private static HashSet<int> ConnectedClients = new HashSet<int>();
        public static HashSet<int> GetConnections => ConnectedClients;

        public static ConnectionType GetMode() => NetworkMode;

        public static void SetMessage(string message)
        {
            LastMessage = message;
        }

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
                    ConnectedClients.Add(0);
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

        public static void AddClient(int id)
        {
            if (!ConnectedClients.Contains(id)) ConnectedClients.Add(id);
        }


        public static void OnClientJoin(int id)
        {
            if (NetworkMode == ConnectionType.Client && _localId == -1)
                _localId = id;
            ConnectedClients.Add(id);
            LastMessage = ($"[NETWORK] Client {id} Joined.");
        }

        public static void OnClientDisconnect(int id)
        {
            ConnectedClients.Remove(id);
            LastMessage = ($"[NETWORK] Client {id} Disconnected.");
        }

        public static void SendMessage(string message)
        {
            if (NetworkMode == ConnectionType.Client)
            {
                Client.SendMessage(message);
            }
        }


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




    }
}
