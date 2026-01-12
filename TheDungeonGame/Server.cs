using LiteNetLib;
using LiteNetLib.Utils;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

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

        public Dictionary<int, NetPeer> ConnectedClients { get; private set; }

        public Server()
        {
            CurrentTick = 0;
            Timer = 0f;
            IsRunning = false;
            IsStopped = true;
            ConnectedClients = new Dictionary<int, NetPeer>();
        }

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
                string message = Message.CreateClientJoinMessage(playerId, CurrentTick);
                SendMessage(peer, message);
                Network.OnClientJoin(playerId);
                foreach (int Id in ConnectedClients.Keys)
                {
                    message = Message.CreateListClientsMessage(Id);
                    SendGlobalMessage(message);
                }
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

        #region Ids
        public void RemoveId(int id)
        {
            ConnectedClients.Remove(id);
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
