using LiteNetLib;
using LiteNetLib.Utils;
using System.Collections.Generic;
using Microsoft.Data.Sqlite;
using System.Security.Cryptography;
using System.Diagnostics;
using System.Text;
using System;

namespace TheDungeonGame
{
    public class Server
    {
        private int CurrentTick { get; set; }
        public bool IsRunning { get; private set; }
        private bool IsStopped { get; set; }
        private float Timer;

        private const int MAX_CONNECTIONS = 10;
        private const int MAX_STRING_LENGTH = 100;
        private const string KEY = "gameKey";

        private EventBasedNetListener Listener { get; set; }
        private NetManager _Server { get; set; }
        private string DatabaseConnectionString { get; set; }

        public Dictionary<int, NetPeer> ConnectedClients { get; private set; }

        public Server()
        {
            CurrentTick = 0;
            Timer = 0f;
            IsRunning = false;
            IsStopped = true;
            ConnectedClients = new Dictionary<int, NetPeer>();
        }

        public bool IsOnline() => _Server.IsRunning;

        public void StartServer(int port)
        {
            Network.SetLocalId(0);

            IsRunning = true;

            Listener = new EventBasedNetListener();
            _Server = new NetManager(Listener);

            _Server.Start(port);
            Debug.WriteLine($"Started on port {port}");

            Listener.ConnectionRequestEvent += request =>
            {
                if (_Server.ConnectedPeersCount < MAX_CONNECTIONS)
                    request.AcceptIfKey(KEY);
                else
                    request.Reject();
            };

            Listener.PeerConnectedEvent += peer =>
            {
                Debug.WriteLine($"Connection at {peer}");

                int playerId = GetNextAvailableId();
                ConnectedClients.Add(playerId, peer);
                string message = Message.CreateClientIdMessage(playerId);
                SendMessage(peer, message);
            };

            Listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod, channel) =>
            {
                string message = dataReader.GetString(MAX_STRING_LENGTH);
                Message.Decode(message);
                dataReader.Recycle();
            };

            Listener.PeerDisconnectedEvent += (peer, disconnectInfo) =>
            {
                Debug.WriteLine($"{peer.Address} Disconnected from server: {disconnectInfo.ToString()}");
                int id = GetClientId(peer);
                string message = Message.CreateClientDisconnectMessage(id);
                SendGlobalMessage(message);
                RemoveId(id);
                Network.OnClientDisconnect(id);
            };

            DatabaseConnectionString = $"Data Source={FileManager.GetDatabasePath()}";
            using SqliteConnection connection = new SqliteConnection(DatabaseConnectionString);
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = """
                CREATE TABLE IF NOT EXISTS players (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    name TEXT NOT NULL UNIQUE,
                    password TEXT NOT NULL,
                    salt TEXT NOT NULL
                );
                """;
            command.ExecuteNonQuery();
        }

        public void Update()
        {
            if (!IsRunning)
            {
                if (!IsStopped)
                {
                    _Server.Stop();
                    IsStopped = true;
            }
                return;
            }
            _Server.PollEvents();

            Timer += Network.GetDelta();
            while (Timer > Network.TIME_BETWEEN_TICKS)
            {
                Timer -= Network.TIME_BETWEEN_TICKS;
                // HandleTick();
                CurrentTick++;
            }
        }

        public void Stop() => IsRunning = false;

        #region Database

        public string Hash(string input, byte[] salt)
        {
            byte[] hash = Rfc2898DeriveBytes.Pbkdf2(
                input,
                salt,
                iterations: 100_000,
                hashAlgorithm: HashAlgorithmName.SHA256,
                outputLength: 32);
            return Convert.ToHexString(hash);
        }

        public void CreatePlayer(int id, string username, string password, string salt)
        {
            string hashedPassword = Hash(password, Convert.FromBase64String(salt));

            using SqliteConnection connection = new SqliteConnection(DatabaseConnectionString);
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = @"
                INSERT INTO Players (name, password, salt)
                VALUES (@name, @password, @salt)
                ON CONFLICT(name) DO NOTHING;
            ";

            command.Parameters.AddWithValue("@name", username);
            command.Parameters.AddWithValue("@password", Hash(password, Convert.FromBase64String(salt)));
            command.Parameters.AddWithValue("@salt", salt);

            int rowsAffected = command.ExecuteNonQuery();

            string message;
            if (rowsAffected > 0)
            {
                message = Message.CreateAccountCreationSuccess();
            }
            else
            {
                message = Message.CreateAccountCreationFail();
            }
            if (id == 0)
                Message.Decode(message);
            else
                SendMessage(id, message);
        }


        
        public void VerifyLogin(int id, string username, string password)
        {
            // compare to database password
            using SqliteConnection connection = new SqliteConnection(DatabaseConnectionString);
            connection.Open();
            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = "SELECT salt, password FROM players WHERE name = $n;";
            command.Parameters.AddWithValue("$n", username);

            SqliteDataReader reader = command.ExecuteReader();
            string message, hashedPassword;

            if (reader.Read())
            {
                string dpSalt = reader.GetString(0);
                hashedPassword = Hash(password, Convert.FromBase64String(dpSalt));
                string dbPassword = reader.GetString(1);
                if (dbPassword == hashedPassword)
                {
                    message = Message.CreateLoginSuccessMessage(username);
                    foreach ((int Id, string Name) in Network.GetConnections)
                    {
                        string msg = Message.CreateClientJoinMessage(Id, Name);
                        SendGlobalMessage(msg);
                        if(Id != 0)
                        {
                            msg = Message.CreateClientJoinMessage(id, username);
                            SendMessage(Id, msg);
                        }
                    }
                    foreach (string msg in PlayerManager.OnClientJoin())
                    {
                        SendGlobalMessage(msg);
                    }
                    Network.OnClientJoin(id, username);
                }
                else
                {
                    message = Message.CreateLoginFailMessage();
                }
            }
            else
            {
                message = Message.CreateLoginFailMessage();
            }
            if (id == 0)
                Message.Decode(message);
            else
                SendMessage(id, message);
        }

        #endregion

        #region Ids
        public void RemoveId(int id)
        {
            ConnectedClients.Remove(id);
            if(PlayerManager.Contains(id))
                PlayerManager.RemovePlayer(id);
        }

        public int GetClientId(NetPeer peer)
        {
            foreach (int id in ConnectedClients.Keys)
            {
                if (ConnectedClients[id] == peer)
                    return id;
            }
            return -1;

        }

        public int GetNextAvailableId()
        {
            int id = 1;
            while (ConnectedClients.ContainsKey(id))
            {
                id++;
            }
            return id;
        }
        #endregion

        #region Sending Messages

        public void SendGlobalMessage(string message)
        {
            foreach (int clientId in ConnectedClients.Keys)
            {
                SendMessage(clientId, message);
            }
        }

        public void SendExclusiveMessage(string message, HashSet<NetPeer> excludedPeers)
        {
            foreach (NetPeer peer in ConnectedClients.Values)
            {
                if (!excludedPeers.Contains(peer))
                    SendMessage(peer, message);
            }
        }

        public void SendExclusiveMessage(string message, HashSet<int> excludedPeers)
        {
            foreach (int peerId in ConnectedClients.Keys)
            {
                if (!excludedPeers.Contains(peerId))
                    SendMessage(peerId, message);
            }
        }

        public void SendMessage(NetPeer client, string message)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put(message);
            client.Send(writer, DeliveryMethod.ReliableOrdered);
            Debug.WriteLine($"Sent message \"{message}\" to {client.Address}");
        }

        public void SendMessage(int clientId, string message) => SendMessage(ConnectedClients[clientId], message);
        #endregion

        #region Handling Ticks
        private void HandleTick()
        {
            if (CurrentTick % (Network.TICK_RATE * 1f /* change freq of syncing */) == 0)
            {
                string message = Message.CreateSyncMessage(CurrentTick);
                SendGlobalMessage(message);
            }
            PlayerManager.UpdatePlayers();
        }

        public void SetTick(int tick)
        {
            CurrentTick = tick;
        }

        public int GetTick() => CurrentTick;
#endregion
    }
}
