using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace TheDungeonGame
{
    public class MainMenuScene : Scene
    {
        public const SceneName Name = SceneName.MainMenu;

        public MainMenuScene(ContentManager content)
        { }

        public override void Update()
        {
            if (InputManager.IsPressed(Input.LMB))
                SceneManager.SwitchScene(SceneName.Game);
        }

        public override void Draw()
        {
            Camera.DrawString("Hello, this is main menu", Vector2.Zero, Color.White);
        }
    }
}
