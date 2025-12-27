using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;

namespace TheDungeonGame
{
    public class GameScene : Scene
    {
        public const SceneName Name = SceneName.Game;
        Player player;
        UIRect healthBar;
        int healthBarMaxLength = Camera.ScreenWidth - 10 * 2;
        Enemy enemy;
        Tilemap tilemap;

        public GameScene(ContentManager Content)
        {
            Texture2D entityTexture = Content.Load<Texture2D>("PointedCircle");

            int playerMaxHealth = 100;
            int playerHealth = 100;
            float playerDamage = 5f;
            float playerSpeed = 5f;
            AnimationManager playerAnimationManager = new AnimationManager(entityTexture);
            Animation playerIdle = new Animation(new Vector2(100f), 0, 1, 1);
            playerIdle.Pause();
            playerAnimationManager.AddAnimation(AnimationNames.Idle, playerIdle);
            playerAnimationManager.ChangeAnimation(AnimationNames.Idle);
            player = new Player(playerAnimationManager, Vector2.Zero, 0f, playerMaxHealth, playerHealth, playerDamage, playerSpeed, Classes.Berserker);
            healthBar = new UIRect(
                new Rectangle(10, 10, healthBarMaxLength, 25),
                Color.Red
                );

            AnimationManager enemyAnimationManager = new AnimationManager(entityTexture);
            Animation enemyIdle = new Animation(new Vector2(100f), 0, 1, 1);
            enemyIdle.Pause();
            enemyAnimationManager.AddAnimation(AnimationNames.Idle, enemyIdle);
            enemyAnimationManager.ChangeAnimation(AnimationNames.Idle);
            enemy = new Enemy(enemyAnimationManager, Vector2.Zero, 0f, 100, 100, 1f, 1f);
            EnemyManager.AddEnemy(enemy);

            tilemap = new Tilemap(@"Maps/Lobby.json");

        }

        public override void OnSwitch()
        { }

        public override void Update()
        {
            Camera.MoveCamera(player.Position, 0.3f, tilemap.CameraBounds);
            player.Update(tilemap);
            healthBar.ChangeSize(new Vector2((player.Health / player.MaxHealth) * healthBarMaxLength, healthBar.Rectangle.Height));
            EnemyManager.Update();
            if (InputManager.IsPressed(Input.Escape))
                SceneManager.BackScene();
        }

        public override void Draw()
        {
            tilemap.Draw();
            player.Draw();
            EnemyManager.Draw();

            healthBar.Draw(); 
        }
    }
}
