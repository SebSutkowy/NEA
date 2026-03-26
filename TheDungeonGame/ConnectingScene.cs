using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace TheDungeonGame
{
    public class ConnectingScene : Scene
    {
        private UIRect ExitButton;
        private UIRect ConnectingText;
        private UIRect RetryButton;
        private bool connecting;
        private float timer;

        public ConnectingScene(ContentManager Content) 
        {
            ExitButton = new UIRect(Camera.GetScaledRect(1f, 1f, 2f, 1f, (1 / 16f)), Color.Crimson, "Exit");
            ConnectingText = new UIRect(Camera.GetScaledRect(1f, 1f, 2f, 1f, (1/4f)), Color.Transparent, "Not Connected");
            RetryButton = new UIRect(Camera.GetScaledRect(2f, 2f, 1f, 1f, (1 / 5f)), Color.Crimson, "Retry");
        } 

        public override void OnSwitch()
        {
            connecting = false;
            timer = -1.0f;
        }

        public override void Update()
        {
            Debug.WriteLine($"{timer}");
            Debug.WriteLine($"{connecting}");
            if(Network.IsOnline())
            {
                ConnectingText.ChangeText("Connected");
                if(!connecting)
                {
                    connecting = true;
                    timer = 2.0f;
                }
            }
            else if(Network.GetMode() == ConnectionType.Client && Network.IsConnecting())
            {
                ConnectingText.ChangeText("Connecting...");
            }
            else if(Network.DidConnectionFail())
            {
                ConnectingText.ChangeText("Connection Failed");
            }

            if(RetryButton.Contains(InputManager.GetMousePos()) && InputManager.IsPressed(Input.LMB))
            {
                Network.RestartClient();
            }

            if(connecting)
            {
                timer -= Network.GetDelta();
            }
            if(timer <= 0 && connecting)
            {
                SceneManager.SwitchScene(SceneName.Login);
            }

        }

        public override void Draw()
        {
            ConnectingText.Draw();
            RetryButton.Draw();
        }
    }
}
