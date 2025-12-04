using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TheDungeonGame
{
    public class GameScene : Scene
    {
        public const SceneName Name = SceneName.Game;
        Player player;
        Texture2D _attackTexture;

        public GameScene(ContentManager Content)
        {
            _attackTexture = Content.Load<Texture2D>("sword");
            player = new Player(Content.Load<Texture2D>("PointedCircle"), Vector2.Zero, 0f, 1, 1, 5f, 5f);
        }

        public override void OnSwitch()
        { }

        public override void Update()
        {
            Camera.MoveCamera(player.Position, 0.3f, new Rectangle(-500, -500, 2000, 2000));
            player.Update();
            if (InputManager.IsPressed(Input.Escape))
                SceneManager.BackScene();
        }

        public override void Draw()
        {
            Camera.DrawString("Hello, this is the Game Scene", Vector2.Zero, Color.White);
            player.Draw();
        }
    }
}
