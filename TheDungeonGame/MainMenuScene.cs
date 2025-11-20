using Microsoft.Xna.Framework;

namespace TheDungeonGame
{
    public class MainMenuScene : Scene
    {
        public const SceneName Name = SceneName.MainMenu;

        public MainMenuScene()
        { }

        public override void Update()
        { }

        public override void Draw()
        {
            Camera.DrawString("Hello, this is main menu", Vector2.Zero, Color.White);
        }
    }
}
