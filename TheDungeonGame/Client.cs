using LiteNetLib;
using LiteNetLib.Utils;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TheDungeonGame
{
    public class Client
    {
        private int CurrentTick;
        private float Timer;

        private EventBasedNetListener Listener;
        private NetManager _Client;
        private NetPeer Server;

        public bool IsRunning { get; private set; }
        public bool IsStopped { get; private set; }
        private Stack<string> Messages = new Stack<string>();

        public Client()
        {
            Timer = 0f;
            Messages = new Stack<string>();
            IsRunning = false;
            IsStopped = false;
        }

        public void StartClient(string ip, int port, string key)
        {
            IsRunning = true;

            Listener = new EventBasedNetListener();
            _Client = new NetManager(Listener);
            _Client.Start();
            Server = _Client.Connect(ip, port, key);


            Listener.NetworkReceiveEvent += (fromPeer, dataReader, deliveryMethod, channel) =>
            {
                int maxMessageLength = 100;
                string message = dataReader.GetString(maxMessageLength);
                if (GetMostRecentMessage() != message)
                    Messages.Push(message);
                Message.Decode(message);
                dataReader.Recycle();
            };
        }

        public void Stop() => IsRunning = false;

        public void Update()
        {
            if (!IsRunning && !IsStopped)
            {
                _Client.Stop();
                IsStopped = true;
                return;
            }
            else if (IsStopped)
            {
                return;
            }



            _Client.PollEvents();

            bool Connection = CheckServerConnection();
            if (Connection)
            {
                TickTimer();
            }
        }

        private void TickTimer()
        {
            Timer += Network.GetDelta();
            while (Timer >= Network.TIME_BETWEEN_TICKS)
            {
                Timer -= Network.TIME_BETWEEN_TICKS;
                // handle tick
                CurrentTick++;
            }
        }

        public void SetTick(int tick)
        {
            CurrentTick = tick; 
        }

        public string GetMostRecentMessage() => Messages.Count > 0 ? Messages.Peek() : "";

        public void SendMessage(string message)
        {
            NetDataWriter writer = new NetDataWriter();
            writer.Put(message);
            Server.Send(writer, DeliveryMethod.ReliableOrdered);
        }

        public bool CheckServerConnection()
        {
            string message;
            switch (Server.ConnectionState)
            {
                case ConnectionState.Outgoing:
                    message = "Connection to server...";
                    break;
                case ConnectionState.Disconnected:
                    message = "Failed to connect to server";
                    break;
                default:
                    return true;
            }
            if (GetMostRecentMessage() != message)
                Messages.Push(message);
            return false;
        }
    }
}
