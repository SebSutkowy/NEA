using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework;

namespace TheDungeonGame
{
    public class DungeonFinishScene : Scene
    {
        UIRect TimeTakenRect;
        UIRect CompletedPuzzlesRect;
        UIRect BackToLobbyRect;
        

        public DungeonFinishScene(ContentManager Content)
        {
            float div = (1f / 16f);
            TimeTakenRect = new UIRect(Camera.GetScaledRect(6f, 4f, 4f, 1f, div), new Color(0, 0, 0, 0), "");
            CompletedPuzzlesRect = new UIRect(Camera.GetScaledRect(6f, 5f, 4f, 1f, div), new Color(0, 0, 0, 0), "");
            BackToLobbyRect = new UIRect(Camera.GetScaledRect(6f, 8f, 4f, 1f, div), Color.Green, "Click here to go back to lobby");
        
        }


        public override void OnSwitch() { }

        public override void Update() 
        {
            bool clicked = InputManager.IsPressed(Input.LMB);
            Point mpos = InputManager.GetMousePos();

            TimeTakenRect.ChangeText($"Time Taken: {Dungeon.GetTimeTaken()}");
            CompletedPuzzlesRect.ChangeText($"Puzzles completed: {Dungeon.PuzzlesCompleted}");
            
            if(BackToLobbyRect.Contains(mpos))
            {
                BackToLobbyRect.ChangeColor(Color.LightGreen);
                if (clicked)
                    SceneManager.SwitchScene(SceneName.Lobby);
            }
            else
            {
                BackToLobbyRect.ChangeColor(Color.Green);
            }
        }

        public override void Draw()
        {
            TimeTakenRect.Draw();
            CompletedPuzzlesRect.Draw();
            BackToLobbyRect.Draw();
        }
    }
}