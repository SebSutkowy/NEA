using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
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
        private UIRect ReceivedMessageButton { get; set; }
        private UIRect ConnectedClientsList { get; set; }

        public OnlineTestingScene() { }
        
        public OnlineTestingScene(ContentManager Content)
        {
            ClientModeButton = new UIRect(Camera.GetScaledRect(0.125f, 0.375f, 0.25f, 0.25f), Color.Yellow, Color.Black, "Switch To\nClient");
            ServerModeButton = new UIRect(Camera.GetScaledRect(0.625f, 0.375f, 0.25f, 0.25f), Color.Green, "Switch To\nServer");
            ExitNetworkButton = new UIRect(Camera.GetScaledRect(0.125f, 0.125f, 0.125f, 0.125f), Color.Red, "Exit");
            SendMessageButton = new UIRect(Camera.GetScaledRect(0.125f, 0.375f, 0.25f, 0.125f), Color.Green, "Send");
            ReceivedMessageButton = new UIRect(Camera.GetScaledRect(0.375f, 0.375f, 0.5f, 0.125f), Color.White, Color.Black);
            ConnectedClientsList = new UIRect(Camera.GetScaledRect(0.375f, 0.55f, 0.5f, 0.375f), Color.Gray);
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

            string clientsList = "";
            foreach (int client in Network.GetConnections)
            {
                clientsList += $"Client {client}\n"; 
            }
            ConnectedClientsList.ChangeText(clientsList);
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
            
        }

        public void UpdateServer(Point mpos)
        {
            ReceivedMessageButton.ChangeText(Network.LastMessage);
        }

        public override void Update()
        {
            Debug.WriteLine(Network.LastMessage);
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
                ReceivedMessageButton.Draw();
                ConnectedClientsList.Draw();
            }
        }
    }
}