using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System.Collections.Generic;
using System.Diagnostics;

namespace TheDungeonGame
{
    public class OnlineTestingScene : Scene
    {
        public readonly SceneName Name = SceneName.OnlineTesting;

        private Queue<string> Messages;

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
            Messages = new Queue<string>();

            float div = 0.0625f; // size of a pixel in the ui grid ( for simpler designing)
            ClientModeButton = new UIRect(Camera.GetScaledRect(2f * div, 6f * div, 4f * div, 2.4f * div), Color.Yellow, Color.Black, "Switch To\nClient");
            ServerModeButton = new UIRect(Camera.GetScaledRect(10f * div, 6f * div, 4f * div, 2.4f * div), Color.Green, "Switch To\nServer");
            ExitNetworkButton = new UIRect(Camera.GetScaledRect(1f * div, 1f * div, 1f * div, 1f * div), Color.Red, "Exit");
            SendMessageButton = new UIRect(Camera.GetScaledRect(3f * div, 6f * div, 4f * div, 2f * div), Color.Green, "Send");
            MessageTextBox = new TextBox(Camera.GetWindow(), Camera.GetScaledRect(8f * div, 3f * div, 8f * div, 2f * div), Color.Gray);
            ReceivedMessages = new UIRect(Camera.GetScaledRect(8f * div, 8f * div, 8f * div, 8f * div), Color.Gray, Color.Black);
            ConnectedClientsList = new UIRect(Camera.GetScaledRect(3f * div, 8f * div, 2f * div, 8f * div), Color.Gray);
            LocalIdButton = new UIRect(Camera.GetScaledRect(8f * div, 1f * div, 2f * div, 1f * div), Color.Gray, Color.Black);
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

        public void UpdateClient(Point mpos)
        {
            ReceivedMessageButton.ChangeText(Network.LastMessage);
            if (InputManager.IsPressed(Input.LMB) && ExitNetworkButton.Contains(mpos))
            {
                // exit network
            }
            if (SendMessageButton.Contains(mpos))
            {
                SendMessageButton.ChangeColor(Color.LightGreen);
                if (InputManager.IsPressed(Input.LMB))
                {
                    string message = Message.CreateSendMessage();
                    Network.SendMessage(message);
                }
            }
            else
            {
                SendMessageButton.ChangeColor(Color.Green);
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
                    UpdateServer(mpos);
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