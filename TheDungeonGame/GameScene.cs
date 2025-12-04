using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace TheDungeonGame
{
    public class GameScene : Scene
    {
        public const SceneName Name = SceneName.Game;
        Player player;
        UIRect healthBar;
        int healthBarMaxLength = Camera.ScreenWidth - 10 * 2;

        public GameScene(ContentManager Content)
        {
            player = new Player(Content.Load<Texture2D>("PointedCircle"), Vector2.Zero, 0f, 1, 1, 5f, 5f);
            healthBar = new UIRect(
                new Rectangle(10, 10, healthBarMaxLength, 25),
                Color.Red
                );
        }

        public override void OnSwitch()
        { }

        public override void Update()
        {
            Camera.MoveCamera(player.Position, 0.3f, new Rectangle(-500, -500, 2000, 2000));
            player.Update();
            if (InputManager.IsPressed(Input.Escape))
                SceneManager.BackScene();
            healthBar.ChangeSize(new Vector2((player.Health / player.MaxHealth) * healthBarMaxLength, healthBar.Rectangle.Height));
        }

        public override void Draw()
        {
            Camera.DrawString("Hello, this is the Game Scene", Vector2.Zero, Color.White);
            player.Draw();
            healthBar.Draw();
            
        }
    }
}
