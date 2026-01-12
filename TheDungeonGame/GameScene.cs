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
            // update players
            // update the dungeon
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
