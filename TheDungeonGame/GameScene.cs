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
            Dungeon.AddTilemap(Tilemaps.Lobby);
            Dungeon.ChangeTilemap(Tilemaps.Lobby);

            TrackedPlayerId = -1;

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
            PlayerManager.WriteIds();
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
            string localIdText = $"Local id: {Network.LocalId}";
            UI.DrawText(localIdText, new Vector2(0f), Color.White);
            UI.DrawText(Network.GetTranslatedMessage(), new Vector2(0f, Camera.MeasureString(localIdText).Y + 2), Color.White);
            
        }
    }
}
