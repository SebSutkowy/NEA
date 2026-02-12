using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using System.Diagnostics;


namespace TheDungeonGame
{
    public class LobbyScene : Scene // where the players will join before loading into a game.
    {
        private UIRect BackButton;
        private UIRect Chat;
        private TextBox ChatTextBox;
        private List<UIRect> PlayerList;
        private UIRect StartButton;
        string receivedMessages;

        private Random seedRandomiser;

        public LobbyScene(ContentManager Content)
        {
            float div = (1f / 16f);
            BackButton = new UIRect(Camera.GetScaledRect(0f, 0f, 2f, 1f, div), Color.Red, "<- Back");
            Chat = new UIRect(Camera.GetScaledRect(0f, 2f, 6f, 7f, div), new Color(0, 0, 0, 160));
            ChatTextBox = new TextBox(Camera.GetWindow(), Camera.GetScaledRect(0, 9f, 6f, 1f, div), new Color(20, 20, 20, 160), "Click here to Chat");
            PlayerList = new List<UIRect>();


        }

        public override void OnSwitch()
        {
            string startButtonText = "";
            Color startButtonColor = Color.Black;
            switch(Network.GetMode())
            {
                case ConnectionType.Host:
                    startButtonText = "Start Game";
                    startButtonColor = Color.Green;
                    break;
                case ConnectionType.Client:
                    startButtonText = "Only host can start";
                    startButtonColor = Color.LightGreen;
                    break;
                default:
                    break;
            }
            StartButton = new UIRect(Camera.GetScaledRect(6f, 10f, 4f, 1f, (1f/16f)), startButtonColor, startButtonText);
        }
        public void SendMessage()
        {
            string message = Message.CreateSendMessage(Network.LocalId, ChatTextBox.Text);
            Network.AddMessage($"[{Network.GetConnections[Network.LocalId]}] {ChatTextBox.Text}");
            Network.SendMessage(message);
            ChatTextBox.Reset();
        }

        public override void Update() 
        {
            Point mpos = InputManager.GetMousePos();
            bool Clicked = InputManager.IsPressed(Input.LMB);

            receivedMessages = "";
            foreach(string message in Network.GetMessages())
            {
                receivedMessages += $"{message}\n";
            }
            Chat.ChangeText(receivedMessages);

            float height = 0f;
            PlayerList.Clear();
            foreach(string name in Network.GetConnections.Values)
            {
                UIRect player = new UIRect(Camera.GetScaledRect(10f, 2f + height, 6f, 1f, (1f / 16f)), new Color(0, 0, 0, 160), name);
                PlayerList.Add(player);
                height++;
            }

            if(StartButton.Contains(mpos) && Clicked)
            {
                if(Network.GetMode() == ConnectionType.Host)
                {
                    seedRandomiser = new Random();
                    int seed = seedRandomiser.Next();
                    Dungeon.GenerateMap(5, seed);
                    string message = Message.CreateGenerateWorldMessage(5, seed);
                    Network.SendMessage(message);
                    SceneManager.SwitchScene(SceneName.Game);
                }
                else
                {
                    Debug.WriteLine("YOU CANNOT START AS A CLIENT");
                }
            }

            if(BackButton.Contains(mpos) && Clicked)
            {
                Network.Stop();
                SceneManager.SwitchScene(SceneName.MainMenu);
            }

            if(Clicked)
            {
                ChatTextBox.OnClick(mpos);
            }

            if (ChatTextBox.Entered)
            {
                SendMessage();
                ChatTextBox.Reset();
            }
        }

        public override void Draw()
        {
            UI.DrawRect(Camera.GetScaledRect(0f, 0f, 1f, 1f), Color.Gray);

            BackButton.Draw();
            Chat.Draw();
            ChatTextBox.Draw();
            foreach (UIRect rect in PlayerList)
            {
                rect.Draw();
            }
            StartButton.Draw();

        }
    }
}