using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.Diagnostics.Contracts;

namespace TheDungeonGame
{
    public class GameScene : Scene
    {
        public const SceneName Name = SceneName.Game;
        public int TrackedPlayerId;

        public GameScene(ContentManager Content)
        {

        }

        public override void OnSwitch()
        {
            Dungeon.Clear();

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
            // move camera to player
            TrackedPlayerId = -1;
            if (PlayerManager.Contains(Network.LocalId))
            {
                TrackedPlayerId = Network.LocalId;
            }
            else
            {
                if (InputManager.IsPressed(Input.Space))
                {
                    string message = Message.CreateSpawnPlayerMessage(TrackedPlayerId);
                    Network.SendMessage(message);
                    if (Network.GetMode() == ConnectionType.Host)
                    {
                        PlayerManager.AddPlayer(Network.LocalId);
                    }
                }

                TrackedPlayerId = PlayerManager.GetRandomId();
            }

            Camera.MoveCamera(PlayerManager.GetPlayerPosition(TrackedPlayerId));

            // update players
            PlayerManager.UpdatePlayers();

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
        }
    }
}
