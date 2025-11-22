using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;

namespace TheDungeonGame
{
    public class MainMenuScene : Scene
    {
        public const SceneName Name = SceneName.MainMenu;

        private UIRect switchSceneRect = new UIRect(new Rectangle(100, 100, 200, 75), Color.Red);
        private UIRect quitGameRect = new UIRect(new Rectangle(100, 200, 200, 75), Color.Red);

        public MainMenuScene(ContentManager content)
        { }

        public override void Update()
        {
            if (switchSceneRect.Contains(InputManager.GetMousePos()))
                switchSceneRect.ChangeColor(Color.Green);
            else
                switchSceneRect.ChangeColor(Color.Red);
        }

        public override void Draw()
        {
            Camera.DrawString("Hello, this is main menu", Vector2.Zero, Color.White);
            switchSceneRect.Draw();
            quitGameRect.Draw();
        }
    }
}
