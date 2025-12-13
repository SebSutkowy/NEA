using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheDungeonGame
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private SpriteFont _cameraFont;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            // TODO: Add your initialization logic here

            base.Initialize();
            _graphics.PreferredBackBufferWidth = Camera.ScreenWidth;
            _graphics.PreferredBackBufferHeight = Camera.ScreenHeight;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();

        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            //InputManager.LoadBinds(@"Keybinds\Keybinds.json");
            _cameraFont = Content.Load<SpriteFont>("CameraFont");
            AssetManager.LoadUnimplementedTexture(Content, "ErrorTexture");
            AssetManager.LoadItemTexture(Content, ItemNames.BasicSword, "Sword");
            AssetManager.LoadTileTexture(Content, TileType.Floor, "BrickFloor");
            AssetManager.LoadTileTexture(Content, TileType.Wall, "TempWall");
            Camera.Initialize(_spriteBatch, _cameraFont);
            UI.LoadUI(GraphicsDevice);
            SceneManager.LoadScenes(Content);
            
        }

        protected override void Update(GameTime gameTime)
        {

            SceneManager.Update();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            SceneManager.Draw();

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
