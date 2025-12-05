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
        Enemy enemy;

        public GameScene(ContentManager Content)
        {
            Texture2D entityTexture = Content.Load<Texture2D>("PointedCircle");
            int playerMaxHealth = 100;
            int playerHealth = 100;
            float playerDamage = 5f;
            float playerSpeed = 5f;
            player = new Player(entityTexture, Vector2.Zero, 0f, playerMaxHealth, playerHealth, playerDamage, playerSpeed);
            healthBar = new UIRect(
                new Rectangle(10, 10, healthBarMaxLength, 25),
                Color.Red
                );
            enemy = new Enemy(entityTexture, Vector2.Zero, 0f, 100, 100, 1f, 1f);
            EnemyManager.AddEnemy(enemy);
        }

        public override void OnSwitch()
        { }

        public override void Update()
        {
            Camera.MoveCamera(player.Position, 0.3f, new Rectangle(-500, -500, 2000, 2000));
            player.Update();
            healthBar.ChangeSize(new Vector2((player.Health / player.MaxHealth) * healthBarMaxLength, healthBar.Rectangle.Height));
            EnemyManager.Update();
            if (InputManager.IsPressed(Input.Escape))
                SceneManager.BackScene();
        }

        public override void Draw()
        {
            player.Draw();
            EnemyManager.Draw();

            healthBar.Draw(); 
        }
    }
}
