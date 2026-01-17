using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;

namespace TheDungeonGame
{
    public class MainMenuScene : Scene
    {
        public const SceneName Name = SceneName.MainMenu;

        private UIRect startServerRect = new UIRect(new Rectangle(100, 100, 200, 75), Color.Red, "Start Server"); 
        private UIRect joinServerRect = new UIRect(new Rectangle(100, 200, 200, 75), Color.Red, "Join Server");
        private UIRect quitGameRect = new UIRect(new Rectangle(100, 300, 200, 75), Color.Red, "Quit");

        public MainMenuScene(ContentManager content)
        { }

        public override void OnSwitch()
        {
            Camera.ResetCamera();
        }

        public override void Update()
        {
            if (startServerRect.Contains(InputManager.GetMousePos()))
                startServerRect.ChangeColor(Color.DarkGray);
            else
                startServerRect.ChangeColor(Color.Gray);

            if (InputManager.IsPressed(Input.LMB) && startServerRect.Color == Color.DarkGray)
            {
                SceneManager.SwitchScene(SceneName.Game);
                Network.ChangeNetworkMode(ConnectionType.Host);
            }

            if (joinServerRect.Contains(InputManager.GetMousePos()))
                joinServerRect.ChangeColor(Color.DarkGray);
            else
                joinServerRect.ChangeColor(Color.Gray);

            if (InputManager.IsPressed(Input.LMB) && joinServerRect.Color == Color.DarkGray)
            {
                SceneManager.SwitchScene(SceneName.Game);
                Network.ChangeNetworkMode(ConnectionType.Client);
            }


            if (quitGameRect.Contains(InputManager.GetMousePos()))
                quitGameRect.ChangeColor(Color.DarkGray);
            else
                quitGameRect.ChangeColor(Color.Gray);

            if (InputManager.IsPressed(Input.LMB) && quitGameRect.Color == Color.DarkGray)
                System.Environment.Exit(0);
                

        }

        public override void Draw()
        {
            Camera.DrawString("Hello, this is main menu", Vector2.Zero, Color.White);
            startServerRect.Draw();
            joinServerRect.Draw();
            quitGameRect.Draw();
        }
    }
}
