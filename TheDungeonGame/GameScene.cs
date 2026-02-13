using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
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

        private bool PlayingMaze;
        private bool PlayingTower;

        private Dictionary<int, int?> Maze;
        private int MazeCurrentLoc;

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
            TrackedPlayerId = -1;
            TargetPlayer = -1;
            ShowingTabList = false;

            PlayingMaze = false;
            PlayingTower = false;

            UI.SetGUI(GUINames.Game);
        }

        public override void Update()
        {
            Debug.WriteLine($"Tilemap Id in Update: {Dungeon.CurrentTilemapName}");
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

        public void GenerateMaze(int size)
        {
            Random rand = new Random((int)DateTime.Now.Ticks);
            HashSet<int> Visited = new HashSet<int>();
            Dictionary<int, int?> Paths = new Dictionary<int, int?>();
            Visited.Add(0);
            Paths.Add(0, null);

            int currentTile = 0;
            while (Visited.Count < size * size)
            {
                List<int> choices = new List<int>();

                int row = currentTile / size;
                int col = currentTile % size;
                // left
                if (col != 0 && !Visited.Contains(currentTile - 1))
                    choices.Add(currentTile - 1);
                // right
                if (col != size - 1 && !Visited.Contains(currentTile + 1))
                    choices.Add(currentTile + 1);
                // up
                if (row != 0 && !Visited.Contains(currentTile - size))
                    choices.Add(currentTile - size);
                // down
                if (row != size - 1 && !Visited.Contains(currentTile + size))
                    choices.Add(currentTile + size);

                if (choices.Count == 0)
                {
                    currentTile = Paths[currentTile].Value;
                    continue;
                }

                int nextTile = choices[rand.Next(0, choices.Count)];
                Paths.Add(nextTile, currentTile);
                Visited.Add(nextTile);
                if (nextTile == size * size - 1) // so that redundant routes aren't created from the ending 
                    currentTile = Paths[currentTile].Value;
                else
                    currentTile = nextTile;
            }

            Maze = Paths;

            Debug.WriteLine("Generated Maze: ");
            foreach ((int tile, int? prev) in Paths)
            {
                Debug.WriteLine($"({tile}) <== ({prev.ToString()})");
            }
        }

        public void StartMaze()
        {
            PlayingMaze = true;
            GenerateMaze(size: 10);
            MazeCurrentLoc = 0;
        }
        public bool IsConnected(int a, int b)
        {
            return (Maze.ContainsKey(a) && Maze[a] == b) || (Maze.ContainsKey(b) && Maze[b] == a);
        }

        public void UpdateMaze()
        {
            // handle exit
            if (InputManager.IsPressed(Input.Escape))
            {
                PlayingMaze = false;
                return;
            }
            // handle inputs to traverse the maze
            if(InputManager.IsPressed(Input.MoveLeft) && IsConnected(MazeCurrentLoc, MazeCurrentLoc - 1))
            {
                    MazeCurrentLoc -= 1; 
            }
            if(InputManager.IsPressed(Input.MoveRight) && IsConnected(MazeCurrentLoc, MazeCurrentLoc + 1))
            {
                MazeCurrentLoc += 1;
            }
            if(InputManager.IsPressed(Input.MoveUp) && IsConnected(MazeCurrentLoc, MazeCurrentLoc - 10))
            {
                MazeCurrentLoc -= 10;
            }
            if(InputManager.IsPressed(Input.MoveDown) && IsConnected(MazeCurrentLoc, MazeCurrentLoc + 10)) // magic numbers :(
            {
                MazeCurrentLoc += 10;
            }

            if(MazeCurrentLoc == 99) // win condition
            {
                PlayingMaze = false;
                Dungeon.CompletedPuzzle(Network.GetConnections[Network.LocalId], Dungeon.CurrentTilemapId);
            }
            
        }

        public void DrawMaze()
        {
            // dark gray background
            UI.DrawRect(Camera.GetScaledRect(1f, 1f, 22f, 22f, (1f / 23f)), Color.DarkGray);
            for(int i = 0; i < 10*10; i++)
            {
                int row = i / 10;
                int col = i % 10;
                Color color = Color.White;
                if (i == 0)
                    color = Color.LightGreen;
                if (i == 99)
                    color = Color.Crimson;

                UI.DrawRect(Camera.GetScaledRect(2 * col + 2f, 2 * row + 2f, 1f, 1f, 1f / 23f), color);
                if(Maze.ContainsKey(i) && Maze[i] != null)
                {
                    int dx, dy;
                    dx = (i % 10) - (Maze[i].Value % 10);
                    dy = (i / 10) - (Maze[i].Value / 10);
                    UI.DrawRect(Camera.GetScaledRect(2 * col + 2f - dx, 2 * row + 2f - dy, 1f, 1f, 1f / 23f), Color.White);
                }
                if(i == MazeCurrentLoc)
                {
                    UI.DrawRect(Camera.GetScaledRect(2 * col + 2f + 1 / 3f, 2 * row + 2f + 1 / 3f, 1 / 3f, 1 / 3f, 1 / 23f), Color.LightBlue);
                }
            }
        }
        // create func for updating the two different puzzles
        // if a maze is opened generate a new one fixed size random seed 
        // add basic movement for solving the maze

        public void StartTower()
        {
            PlayingTower = true;
        }
        
        public void UpdateTower()
        {
            if (InputManager.IsPressed(Input.Escape))
            {
                PlayingTower = false;
                return;
            }

        }

        public void DrawTower()
        {

        }
        // tower of hanoi is always 3 tall
        // move with mouse

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

        private void DrawTabList()
        {
            // other players
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
            // minimap
            Dungeon.DrawMinimap();

        }

        private void ClearTabList()
        {
            TabList.Clear();
            MuteButtons.Clear();
            PMButtons.Clear();
        }


        public void UpdateGame()
        {
            #region tablist
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
            #endregion

            #region chat
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
            #endregion

            #region camera
            // move camera to player
            if (PlayerManager.Contains(Network.LocalId))
            {
                TrackedPlayerId = Network.LocalId;
            }
            else 
            {
                TrackedPlayerId = PlayerManager.GetClosestPlayer(new Vector2(0f));
            }

            if(PlayerManager.Count > 0)
                Camera.MoveCamera(PlayerManager.GetPlayerPosition(TrackedPlayerId));
            #endregion

            // update players
            PlayerManager.UpdatePlayers(!Chat.IsFocused && !PlayingMaze && !PlayingTower);


            // update the dungeon
            Dungeon.Update();

            // handle pausing 
            // handle changing tilemap
            int id = -1;
            if (PlayerManager.Contains(Network.LocalId))
                id = Network.LocalId;
            Dungeon.CheckIfChangingTilemap(id);

            #region puzzles
            if (PlayingMaze)
            {
                UpdateMaze();
                return;
            }
            else if (PlayingTower)
            {
                UpdateTower();
                return;
            } // wont handle interactions when doing puzzles (can click accidentally on puzzle tile)
            #endregion

            // handle interactions
            Point tilemapPos = InputManager.GetTilemapMousePos();
            if (Dungeon.IsPuzzle(Tilemap.GetLoc(tilemapPos)) && InputManager.IsPressed(Input.LMB))
            {
                if (Dungeon.CurrentTilemapName == Tilemaps.MazePuzzle)
                {
                    StartMaze();
                }
                else if (Dungeon.CurrentTilemapName == Tilemaps.TowerOfHanoiPuzzle)
                {
                    StartTower();
                }
            }
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
                DrawTabList();
            }

            if (PlayingMaze)
                DrawMaze();
            if (PlayingTower)
                DrawTower();

            string localIdText = $"Local id: {Network.LocalId}";
            UI.DrawText(localIdText, new Vector2(0f), Color.White);
            UI.DrawText(Network.GetTranslatedMessage(), new Vector2(0f, Camera.MeasureString(localIdText).Y + 2), Color.White);
            
        }
    }
}
