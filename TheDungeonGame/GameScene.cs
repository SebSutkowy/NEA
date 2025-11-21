using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TheDungeonGame
{
    public class GameScene : Scene
    {
        public const SceneName Name = SceneName.Game;
        Player player;

        public GameScene(ContentManager Content)
        {
            player = new Player(Content.Load<Texture2D>("PointedCircle"), Vector2.Zero, 0f);
        }

        public override void Update()
        {
            if (InputManager.IsPressed(Input.Escape))
                SceneManager.BackScene();
            Camera.MoveCamera(player.Position);
            player.Update();
        }

        public override void Draw()
        {
            Camera.DrawString("Hello, this is the Game Scene", Vector2.Zero, Color.White);
            player.Draw();
        }
    }
}
