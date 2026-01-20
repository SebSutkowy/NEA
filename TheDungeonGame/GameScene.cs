using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace TheDungeonGame
{
    public class GameScene : Scene
    {
        public const SceneName Name = SceneName.Game;
        public int TrackedPlayerId;

        private UIRect ChatHistory;
        private TextBox Chat;
        private Queue<string> Messages;

        private List<UIRect> TabList;
        private bool ShowingTabList;

        public GameScene(ContentManager Content)
        {
            // 03 25
            ChatHistory = new UIRect(Camera.GetScaledRect(0f, 3f, 3f, 2f, 0.1f), new Color(0, 0, 0, 150));
            Chat = new TextBox(Camera.GetWindow(), Camera.GetScaledRect(0f, 5f, 3f, 1f, 0.1f), new Color(0, 0, 0, 150));
            TabList = new List<UIRect>();
        }

        public override void OnSwitch()
        {
            Dungeon.Clear();
            Dungeon.AddTilemap(Tilemaps.Lobby);
            Dungeon.ChangeTilemap(Tilemaps.Lobby);

            TrackedPlayerId = -1;
            ShowingTabList = false;

            UI.SetGUI(GUINames.Game);
        }

        public override void Update()
        {
            switch (UI.CurrentGUIName)
            {
                case GUINames.Game:
                    UpdateGame();
                    break;
                case GUINames.Shop:
                    UpdateShop();
                    break;
                case GUINames.Dialogue:
                    UpdateDialogue();
                    break;
                default:
                    break;

            }

        }

        public void UpdateShop()
        {

        }

        public void UpdateDialogue()
        {
        }

        public void UpdateGame()
        {
            //tablist
            if (InputManager.IsPressed(Input.Tab))
            {
                ShowingTabList = true;
                HashSet<int> connections = Network.GetConnections;
                //Debug.WriteLine(connections.Count)
                UIRect rect;
                string playerStatus;
                int health;
                foreach (int connection in connections)
                {
                    playerStatus = $"Client {connection}   ";
                    health = PlayerManager.GetPlayerHealth(connection);
                    switch (health)
                    {
                        case -1:
                            playerStatus += "Dead";
                            break;
                        default:
                            playerStatus += $"♥ {health}";
                            break;
                    }
                    if (connection == Network.LocalId)
                    {
                        rect = new UIRect(Camera.GetScaledRect(3f, (float)(1 + TabList.Count), 4f, 1f, 0.1f), new Color(0, 0, 0, 150), playerStatus);
                    }
                    else
                    {
                        rect = new UIRect();
                    }

                    TabList.Add(rect);
                }
            }
            else if (InputManager.IsHeld(Input.Tab))
            {
                ShowingTabList = true;
            }
            else
            {
                ShowingTabList = false;
                TabList.Clear();
            }



            // chat
            if (InputManager.IsPressed(Input.LMB))
                Chat.OnClick(InputManager.GetMousePos());
            if (Chat.Entered && Chat.Text != string.Empty)
            {
                string message = Message.CreateSendMessage(Network.LocalId, Chat.Text);
                Network.AddMessage($"[{Network.LocalId}] {Chat.Text}");
                Network.SendMessage(message);
                Chat.Reset();
            }
            if (Chat.IsFocused)
            {
                Chat.ChangeColor(new Color(100, 100, 100, 150));
            }
            else
            {
                Chat.ChangeColor(new Color(0, 0, 0, 150));
            }

            Messages = new Queue<string>(Network.GetMessages());


            // move camera to player
            if (PlayerManager.Contains(Network.LocalId))
            {
                TrackedPlayerId = Network.LocalId;
            }
            else if (InputManager.IsPressed(Input.Space))
            {
                string message = Message.CreateSpawnPlayerMessage(Network.LocalId);
                Network.SendMessage(message);
                if (Network.GetMode() == ConnectionType.Host)
                {
                    PlayerManager.AddPlayer(Network.LocalId);
                }
            }
            else 
            {
                TrackedPlayerId = PlayerManager.GetClosestPlayer(new Vector2(0f));
            }
            

            if(PlayerManager.Count > 0)
                Camera.MoveCamera(PlayerManager.GetPlayerPosition(TrackedPlayerId));

            // update players
            PlayerManager.UpdatePlayers(!Chat.IsFocused);

            // update the dungeon
            Dungeon.Update();

            // handle pausing 
            // handle changing tilemap
            // handle interactions
        }

        public override void Draw()
        {
            Dungeon.Draw();
            PlayerManager.Draw();

            UI.CurrentGUI.Draw();
            if (UI.CurrentGUIName == GUINames.Game)
            {
                ChatHistory.Draw();
                Chat.Draw();
            }
            int height = 0;
            while (Messages != null && Messages.Count > 0)
            {
                string message = Messages.Dequeue();
                Camera.DrawString(message, new Vector2(0, 300 + height), Color.White);
                height += (int)Camera.MeasureString(message).Y;
            }

            if (ShowingTabList)
            {
                foreach (UIRect rect in TabList)
                {
                    rect.Draw();
                }
            }

            string localIdText = $"Local id: {Network.LocalId}";
            UI.DrawText(localIdText, new Vector2(0f), Color.White);
            UI.DrawText(Network.GetTranslatedMessage(), new Vector2(0f, Camera.MeasureString(localIdText).Y + 2), Color.White);
            
        }
    }
}
