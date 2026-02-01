using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

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
        private Dictionary<int, UIRect> MuteButtons;
        private Dictionary<int, UIRect> PMButtons;
        private int TargetPlayer;
        private bool ShowingTabList;

        public GameScene(ContentManager Content)
        {
            // 03 25
            ChatHistory = new UIRect(Camera.GetScaledRect(0f, 3f, 3f, 2f, 0.1f), new Color(0, 0, 0, 150));
            Chat = new TextBox(Camera.GetWindow(), Camera.GetScaledRect(0f, 5f, 3f, 1f, 0.1f), new Color(0, 0, 0, 150), "Type something to chat", new Color(20, 20, 20, 150));
            TabList = new List<UIRect>();
            MuteButtons = new Dictionary<int, UIRect>();
            PMButtons = new Dictionary<int, UIRect>();
        }

        public override void OnSwitch()
        {
            Dungeon.Clear();
            Dungeon.GenerateMap(7, 1067);

            TrackedPlayerId = -1;
            TargetPlayer = -1;
            ShowingTabList = false;

            UI.SetGUI(GUINames.Game);
        }

        public override void Update()
        {
            Debug.WriteLine($"Tilemap Id in Update: {Dungeon.CurrentTilemapId}");
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

        public void CreateTabList()
        {
            //Debug.WriteLine(connections.Count)
            UIRect rect;
            UIRect muteRect;
            UIRect pmRect;
            string playerStatus;
            int health;
            foreach ((int connection, string username) in Network.GetConnections)
            {
                playerStatus = $"{username}   ";
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
                    rect = new UIRect(Camera.GetScaledRect(3f, (float)(1 + TabList.Count), 2f, 1f, 0.1f), new Color(0, 0, 0, 150), playerStatus);
                    muteRect = new UIRect(Camera.GetScaledRect(5f, (float)(1 + TabList.Count), 1f, 1f, 0.1f), new Color(0, 0, 0, 150), "Mute");
                    pmRect = new UIRect(Camera.GetScaledRect(6f, (float)(1 + TabList.Count), 1f, 1f, 0.1f), new Color(0, 0, 0, 150), "Msg");
                    MuteButtons.Add(connection, muteRect);
                    PMButtons.Add(connection, pmRect);
                }

                TabList.Add(rect);
            }
        }

        private void CheckTabList()
        {
            Point mpos = InputManager.GetMousePos();
            // MUTE
            foreach ((int id, UIRect rect) in MuteButtons)
            {
                if (Network.IsMuted(id))
                {
                    if (rect.Contains(mpos))
                    {
                        rect.ChangeColor(new Color(150, 150, 150, 150));
                        if (InputManager.IsPressed(Input.LMB))
                            Network.Unmute(id);
                    }
                    else
                        rect.ChangeColor(new Color(100, 100, 100, 150));
                }
                else
                {
                    if (rect.Contains(mpos))
                    {
                        rect.ChangeColor(new Color(50, 50, 50, 150));
                        if (InputManager.IsPressed(Input.LMB))
                            Network.Mute(id);
                    }
                    else
                        rect.ChangeColor(new Color(0, 0, 0, 150));
                }

            }

            // PRIVATE MESSAGE
            foreach ((int id, UIRect rect) in PMButtons)
            {
                if (id == TargetPlayer)
                {
                    if (rect.Contains(mpos))
                    {
                        rect.ChangeColor(new Color(150, 150, 150, 150));
                        if (InputManager.IsPressed(Input.LMB))
                            TargetPlayer = -1;
                    }
                    else
                        rect.ChangeColor(new Color(100, 100, 100, 150));
                }
                else
                {
                    if (rect.Contains(mpos))
                    {
                        rect.ChangeColor(new Color(50, 50, 50, 150));
                        if (InputManager.IsPressed(Input.LMB))
                            TargetPlayer = id;
                    }
                    else
                        rect.ChangeColor(new Color(0, 0, 0, 150));
                }
            }
        }

        private void ClearTabList()
        {
            TabList.Clear();
            MuteButtons.Clear();
            PMButtons.Clear();
        }


        public void UpdateGame()
        {
            //tablist
            if (!Network.GetConnections.ContainsKey(TargetPlayer))
                TargetPlayer = -1;
            if (InputManager.IsPressed(Input.Tab))
            {
                ShowingTabList = true;
                CreateTabList();
            }
            if (InputManager.IsHeld(Input.Tab))
            {
                CheckTabList();
                ShowingTabList = true;
            }
            else
            {
                ShowingTabList = false;
                ClearTabList();
            }



            // chat
            if (InputManager.IsPressed(Input.LMB))
                Chat.OnClick(InputManager.GetMousePos());
            if (Chat.Entered && Chat.Text != string.Empty)
            {
                string username = Network.GetConnections[Network.LocalId];
                if (TargetPlayer == -1)
                {
                    string message = Message.CreateSendMessage(Network.LocalId, Chat.Text);
                    Network.AddMessage($"[{username}] {Chat.Text}");
                    Network.SendMessage(message);
                }
                else
                {
                    string targetUsername = Network.GetConnections[TargetPlayer];
                    string message = Message.CreateSendPrivateMessage(Network.LocalId, TargetPlayer, Chat.Text);
                    Network.AddMessage($"[{username}->{targetUsername}] {Chat.Text}");
                    Network.SendMessage(message);
                }

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
            Dungeon.CheckIfChangingTilemap();
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
                foreach (UIRect rect in MuteButtons.Values)
                {
                    rect.Draw();
                }
                foreach (UIRect rect in PMButtons.Values)
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
