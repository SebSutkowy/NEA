using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System.Diagnostics;

namespace TheDungeonGame
{
    public class OnlineTestingScene : Scene
    {
        public readonly SceneName Name = SceneName.OnlineTesting;

        // UI elements
        private UIRect ClientModeButton { get; set; }
        private UIRect ServerModeButton { get; set; }
        private UIRect ExitNetworkButton { get; set; }
        private UIRect SendMessageButton { get; set; }
        private UIRect ReceivedMessages { get; set; }
        private TextBox MessageTextBox { get; set; }
        private UIRect ConnectedClientsList { get; set; }
        private UIRect LocalIdButton { get; set; }

        public OnlineTestingScene() { }
        
        public OnlineTestingScene(ContentManager Content)
        {
            float div = (1f/16f); // size of a pixel in the ui grid ( for simpler designing)
            ClientModeButton = new UIRect(Camera.GetScaledRect(2f, 6f, 4f, 2.4f, div), Color.Yellow, Color.Black, "Switch To\nClient");
            ServerModeButton = new UIRect(Camera.GetScaledRect(10f, 6f, 4f, 2.4f, div), Color.Green, "Switch To\nServer");
            ExitNetworkButton = new UIRect(Camera.GetScaledRect(1f, 1f, 1f, 1f, div), Color.Red, "Exit");
            SendMessageButton = new UIRect(Camera.GetScaledRect(2f, 5f, 4f, 2f, div), Color.Green, "Send");
            MessageTextBox = new TextBox(Camera.GetWindow(), Camera.GetScaledRect(6f, 5f, 8f, 2f, div), Color.Gray);
            ReceivedMessages = new UIRect(Camera.GetScaledRect(7f, 8f, 7f, 7f, div), Color.Gray, Color.Black);
            ConnectedClientsList = new UIRect(Camera.GetScaledRect(2f, 8f, 4f, 7f, div), Color.Gray);
            LocalIdButton = new UIRect(Camera.GetScaledRect(8f, 1f, 2f, 1f, div), Color.Gray, Color.Black);
        }


        public override void OnSwitch()
        {
            
        }

        public void UpdateOffline(Point mpos)
        {
            if (ClientModeButton.Contains(mpos))
            {
                ClientModeButton.ChangeColor(Color.LightYellow);
                if (InputManager.IsPressed(Input.LMB))
                {
                    Network.ChangeNetworkMode(ConnectionType.Client);
                }
            }
            else
            {
                ClientModeButton.ChangeColor(Color.Yellow);
            }
            if (ServerModeButton.Contains(mpos))
            {
                ServerModeButton.ChangeColor(Color.LightGreen);
                if (InputManager.IsPressed(Input.LMB))
                {
                    Network.ChangeNetworkMode(ConnectionType.Host);
                }
            }
            else
            {
                ServerModeButton.ChangeColor(Color.Green);
            }

        }

        public void SendMessage()
        {
            string message = Message.CreateSendMessage(Network.LocalId, MessageTextBox.Text);
            Network.AddMessage($"[{Network.LocalId}] {MessageTextBox.Text}");
            Network.SendMessage(message);
            MessageTextBox.Reset();
        }

        public void UpdateClient(Point mpos)
        {

            string receivedMessages = "";
            foreach (string message in Network.GetMessages())
            {
                receivedMessages += $"{message}\n";
            }
            ReceivedMessages.ChangeText(receivedMessages);

            if (InputManager.IsPressed(Input.LMB) && ExitNetworkButton.Contains(mpos))
            {
                // exit network
            }
            if (MessageTextBox.Entered && MessageTextBox.Text != string.Empty)
            {
                SendMessage();
            }
            if (SendMessageButton.Contains(mpos))
            {
                SendMessageButton.ChangeColor(Color.LightGreen);
                if (InputManager.IsPressed(Input.LMB) && MessageTextBox.Text != string.Empty)
                {
                    SendMessage();
                }
            }
            else
            {
                SendMessageButton.ChangeColor(Color.Green);
            }
            if (InputManager.IsPressed(Input.LMB))
            {
                MessageTextBox.OnClick(mpos);
            }

            string clientsList = "";
            foreach (int client in Network.GetConnections)
            {
                clientsList += $"Client {client}\n"; 
            }
            Debug.WriteLine(clientsList);
            ConnectedClientsList.ChangeText(clientsList);
            LocalIdButton.ChangeText($"Local Id: {Network.LocalId}");
        }

        public void UpdateServer(Point mpos)
        {
            //ReceivedMessageButton.ChangeText(Network.LastMessage);

            string clientsList = "";
            foreach (int client in Network.GetConnections)
            {
                clientsList += $"Client {client}\n"; 
            }
            Debug.WriteLine(clientsList);
            ConnectedClientsList.ChangeText(clientsList);
            LocalIdButton.ChangeText($"Local Id: {Network.LocalId}");
        }

        public override void Update()
        {
            Point mpos = InputManager.GetMousePos();
            switch (Network.GetMode())
            {
                case ConnectionType.None:
                    UpdateOffline(mpos);
                    break;
                case ConnectionType.Client:
                    UpdateClient(mpos);
                    break;
                case ConnectionType.Host:
                    UpdateClient(mpos);
                    break;
            }

        }

        public override void Draw()
        {
            if (Network.GetMode() == ConnectionType.None)
            {
                ClientModeButton.Draw();
                ServerModeButton.Draw();
            }
            else
            {
                ExitNetworkButton.Draw();
                SendMessageButton.Draw();
                ReceivedMessages.Draw();
                MessageTextBox.Draw();
                ConnectedClientsList.Draw();
                LocalIdButton.Draw();
            }
        }
    }
}