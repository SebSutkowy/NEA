using Microsoft.Xna.Framework;
using System.Security.Cryptography;
using System;
using Microsoft.Xna.Framework.Content;
using System.Diagnostics;

namespace TheDungeonGame
{
    public enum LoginState
    {
        Login,
        Register
    }

    public class LoginScene : Scene
    {
        private Color SubmitColor;

        private UIRect ExitRect;
        private TextBox UsernameBox;
        private TextBox PasswordBox;
        private UIRect SubmitRect;
        private UIRect SwapLoginMenuRect;

        private LoginState LoginState;

        public LoginScene(ContentManager Content)
        {
            SubmitColor = new Color(11, 218, 81);
            LoginState = LoginState.Login;

            float div = 1.0f / 20.0f;
            ExitRect = new UIRect(Camera.GetScaledRect(1f, 1f, 2f, 1f, div), Color.Crimson, "Exit");
            UsernameBox = new TextBox(Camera.GetWindow(), Camera.GetScaledRect(5f, 5f, 10f, 2f, div), Color.Gray, "Username", new Color(100, 100, 100));
            PasswordBox = new TextBox(Camera.GetWindow(), Camera.GetScaledRect(5f, 8f, 10f, 2f, div), Color.Gray, "Password", new Color(100, 100, 100));
            SubmitRect = new UIRect(Camera.GetScaledRect(8f, 11f, 4f, 2f, div), SubmitColor, "Submit");
            SwapLoginMenuRect = new UIRect(Camera.GetScaledRect(17f, 1f, 2f, 1f, div), Color.Gray, "Register");
        }

        public override void OnSwitch()
        {
            LoginState = LoginState.Login;
        }

        public void UpdateLogin(Point mpos)
        {
            // login success / fail handling
            if(SubmitRect.Contains(mpos) && InputManager.IsPressed(Input.LMB))
            {
                if (UsernameBox.Text != string.Empty && PasswordBox.Text.Length >= 8)
                {
                    string message = Message.CreateRequestLoginMessage(Network.LocalId, UsernameBox.Text, PasswordBox.Text);
                    Debug.WriteLine(message);
                    if (Network.GetMode() == ConnectionType.Host)
                    {
                        Message.Decode(message);
                    }
                    else
                    {
                        Network.SendMessage(message);
                    }
                    UsernameBox.Reset();
                    PasswordBox.Reset();
                }
                else if (PasswordBox.Text.Length < 8)
                {
                    // password must be 8 characters or more 
                    Debug.WriteLine("Password must be 8 characters or longer");
                    PasswordBox.Reset();
                }

            }
        }

        public void UpdateRegister(Point mpos)
        {
            // handling account creation / failure to create
            if(SubmitRect.Contains(mpos) && InputManager.IsPressed(Input.LMB))
            {
                if (UsernameBox.Text != string.Empty && PasswordBox.Text.Length >= 8)
                {
                    string salt = Convert.ToBase64String(RandomNumberGenerator.GetBytes(16));
                    string message = Message.CreateRegisterMessage(Network.LocalId, UsernameBox.Text, PasswordBox.Text, salt);
                    if (Network.GetMode() == ConnectionType.Host)
                    {
                        Message.Decode(message);
                    }
                    else
                    {
                        Network.SendMessage(message);
                    }
                    UsernameBox.Reset();
                    PasswordBox.Reset();
                }
                else if (PasswordBox.Text.Length < 8)
                {
                    // password must be 8 characters or more 
                    Debug.WriteLine("Password must be 8 characters or longer");
                    PasswordBox.Reset();
                }
            }
        }

        public override void Update()
        {
            switch (LoginState)
            {
                case LoginState.Login:
                    SwapLoginMenuRect.ChangeText(">Register");
                    break;
                case LoginState.Register:
                    SwapLoginMenuRect.ChangeText(">Login");
                    break;
            }
            Point mpos = InputManager.GetMousePos();
            if(ExitRect.Contains(mpos))
            {
                ExitRect.ChangeColor(Color.Crimson * 1.1f);
                // disconnect from sever
                if (InputManager.IsPressed(Input.LMB))
                {
                    Network.Stop();
                    SceneManager.SwitchScene(SceneName.MainMenu);
                }

            }
            else
            {
                ExitRect.ChangeColor(Color.Crimson);
            }

            if(SubmitRect.Contains(mpos))
            {
                SubmitRect.ChangeColor(SubmitColor * 1.1f);
            }
            else
            {
                SubmitRect.ChangeColor(SubmitColor);
            }

            if (SwapLoginMenuRect.Contains(mpos))
            {
                SwapLoginMenuRect.ChangeColor(Color.Gray * 1.1f);
                if(InputManager.IsPressed(Input.LMB))
                {
                    if (LoginState == LoginState.Register)
                        LoginState = LoginState.Login;
                    else
                        LoginState = LoginState.Register;
                    UsernameBox.Reset();
                    PasswordBox.Reset();
                }
                // clear both username and password boxes

            }
            else
            {
                SwapLoginMenuRect.ChangeColor(Color.Gray);
            }

            if(InputManager.IsPressed(Input.LMB))
            {
                UsernameBox.OnClick(mpos);
                PasswordBox.OnClick(mpos);
            }

            if (LoginState == LoginState.Login)
            {
                UpdateLogin(mpos);
            }
            else if (LoginState == LoginState.Register)
            {
                UpdateRegister(mpos);
            }

        }

        public override void Draw() 
        {
            ExitRect.Draw();
            UsernameBox.Draw();
            PasswordBox.Draw();
            SubmitRect.Draw();
            SwapLoginMenuRect.Draw();
        }
    }
}
