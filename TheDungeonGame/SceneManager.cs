using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;

namespace TheDungeonGame
{
    public enum SceneName
    {
        MainMenu,
        Game,
        OnlineTesting,
        Login,
        Lobby
    }

    public static class SceneManager
    {
        private static SceneName CurrentSceneName = SceneName.MainMenu;
        private static SceneName? PreviousSceneName = null;
        private static SceneName? NewScene = null;

        public static Scene CurrentScene => Scenes[CurrentSceneName];

        private static Dictionary<SceneName, Scene> Scenes = new Dictionary<SceneName, Scene>();

        public static void LoadScenes(ContentManager Content)
        {
            CurrentSceneName = SceneName.MainMenu;
            Scenes.Add(SceneName.MainMenu, new MainMenuScene(Content));
            Scenes.Add(SceneName.Game, new GameScene(Content));
            Scenes.Add(SceneName.OnlineTesting, new OnlineTestingScene(Content));
            Scenes.Add(SceneName.Login, new LoginScene(Content));
            Scenes.Add(SceneName.Lobby, new LobbyScene(Content));
        }

        public static void Update()
        {
            InputManager.Update();
            CurrentScene.Update();

            if (NewScene != null)
            {
                if (!Scenes.ContainsKey(NewScene.Value))
                    throw new NotImplementedException();
                PreviousSceneName = CurrentSceneName;
                CurrentSceneName = NewScene.Value;
                NewScene = null;
                CurrentScene.OnSwitch();
            }

        }

        public static void Draw()
        {
            CurrentScene.Draw();
        }

        public static void SwitchScene(SceneName newScene)
        {
            NewScene = newScene;
        }

        public static void BackScene()
        {
            if (PreviousSceneName == null) return;
            SwitchScene((SceneName)PreviousSceneName);
        }
    }

    public abstract class Scene
    {
        public abstract void OnSwitch();
        public abstract void Update();
        public abstract void Draw();
    }
}
