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
    }

    public static class SceneManager
    {
        private static SceneName CurrentSceneName = SceneName.MainMenu;
        private static SceneName? PreviousSceneName = null;

        public static Scene CurrentScene => Scenes[CurrentSceneName];

        private static Dictionary<SceneName, Scene> Scenes = new Dictionary<SceneName, Scene>();

        public static void LoadScenes(ContentManager Content)
        {
            CurrentSceneName = SceneName.Game;
            Scenes.Add(SceneName.MainMenu, new MainMenuScene(Content));
            Scenes.Add(SceneName.Game, new GameScene(Content));
        }

        public static void Update()
        {
            InputManager.Update();
            CurrentScene.Update();
        }

        public static void Draw()
        {
            CurrentScene.Draw();
        }
    }

    public abstract class Scene
    {
        public abstract void Update();
        public abstract void Draw();
    }
}
