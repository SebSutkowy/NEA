using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Diagnostics;
using System.Collections.Generic;
using System;

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
            _graphics.PreferredBackBufferWidth = Camera.ScreenWidth;
            _graphics.PreferredBackBufferHeight = Camera.ScreenHeight;
            _graphics.IsFullScreen = false;
            _graphics.ApplyChanges();


            base.Initialize();
        }

        public bool IsFocused() => this.IsActive;

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // TODO: use this.Content to load your game content here
            //InputManager.LoadBinds(@"Keybinds\Keybinds.json");
            _cameraFont = Content.Load<SpriteFont>("CameraFont");
            AssetManager.LoadUnimplementedTexture(Content, "ErrorTexture");
            AssetManager.LoadItemTexture(Content, ItemNames.BasicSword, "Sword");
            AssetManager.LoadTileTexture(Content, TileType.Floor, "FloorTile");
            AssetManager.LoadTileTexture(Content, TileType.Wall, "WallTile");
            AssetManager.LoadTileTexture(Content, TileType.Door, "DoorTile");
            AssetManager.LoadSpriteSheet(Content, SpriteSheets.Weapons, "WeaponSpriteSheet");
            AssetManager.LoadSpriteSheet(Content, SpriteSheets.Entity, "PointedCircle");

            AssetManager.LoadDialogue(NPCId.Shopkeep, "Shopkeep.json");
            AssetManager.LoadNPC(NPCId.Shopkeep, new NPC(0f, NPCId.Shopkeep));

            Camera.Initialize(_spriteBatch, _cameraFont, Window);
            UI.LoadUI(GraphicsDevice);
            SceneManager.LoadScenes(Content);

            GUI gameGUI = new GUI(GUINames.Game);
            UIRect healthBar = new UIRect(new Rectangle((int) (0.01f * Camera.ScreenWidth), (int) (0.01f * Camera.ScreenHeight), (int) (0.98f * Camera.ScreenWidth), (int) (0.02f * Camera.ScreenHeight)), Color.Red);
            gameGUI.AddElement((int)GameGUIElements.HealthBar, healthBar);

            GUI shopGUI = new GUI(GUINames.Shop);
            UIRect background = new UIRect(new Rectangle((int) (0.1f * Camera.ScreenWidth), (int) (0.1f * Camera.ScreenHeight), (int) (0.8f * Camera.ScreenWidth), (int) (0.8f * Camera.ScreenHeight)), new Color(50, 50, 50, 200));
            UIRect HealButton = new UIRect(new Rectangle((int)(0.2f * Camera.ScreenWidth), (int)(0.7f * Camera.ScreenHeight), (int)(0.1f * Camera.ScreenWidth), (int)(0.1f * Camera.ScreenHeight)), Color.Green, "Heal");
            UIRect upgradeNormalAttackRect = new UIRect(new Rectangle((int) (0.4f * Camera.ScreenWidth), (int) (0.7f * Camera.ScreenHeight), (int) (0.15f * Camera.ScreenWidth), (int) (0.1f * Camera.ScreenHeight)), Color.Green, "Upgrade\nNormal\nAttack");
            UIRect upgradeSpecialAttackRect = new UIRect(new Rectangle((int)(0.65f * Camera.ScreenWidth), (int)(0.7f * Camera.ScreenHeight), (int)(0.15f * Camera.ScreenWidth), (int)(0.1f * Camera.ScreenHeight)), Color.Green, "Upgrade\nSpecial\nAttack");
            UIRect healthBarShop = new UIRect(new Rectangle((int) (0.01f * Camera.ScreenWidth), (int) (0.01f * Camera.ScreenHeight), (int) (0.98f * Camera.ScreenWidth), (int) (0.02f * Camera.ScreenHeight)), Color.Red);
            shopGUI.AddElement((int)ShopGUIElements.Background, background);
            shopGUI.AddElement((int)ShopGUIElements.UpgradeNormalAttack, upgradeNormalAttackRect);
            shopGUI.AddElement((int)ShopGUIElements.UpgradeSpecialAttack, upgradeSpecialAttackRect);
            shopGUI.AddElement((int)ShopGUIElements.HealButton, HealButton);
            shopGUI.AddElement((int)ShopGUIElements.HealthBar, healthBarShop);

            GUI dialogueGUI = new GUI(GUINames.Dialogue);
            DialogueBox dialogueBox = new DialogueBox(new Rectangle((int)(0.1f * Camera.ScreenWidth), (int)(0.5f * Camera.ScreenHeight), (int)(0.8f * Camera.ScreenWidth), (int)(0.4f * Camera.ScreenHeight)), new Color(20, 20, 20, 200), new List<string>());
            dialogueGUI.AddElement((int)DialogueGUIElements.DialogueBox, dialogueBox);

            UI.AddGUI(GUINames.Game, gameGUI);
            UI.AddGUI(GUINames.Shop, shopGUI);
            UI.AddGUI(GUINames.Dialogue, dialogueGUI);
        }

        protected override void Update(GameTime gameTime)
        {
            Camera.SetFocus(this.IsActive);
            
            SceneManager.Update();
            UI.Update();
            Network.Update(gameTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            SceneManager.Draw();
            UI.Draw();

            /*
            float fps = (float)gameTime.ElapsedGameTime.TotalSeconds;
            fps = 1 / fps;
            Camera.DrawString($"{fps}FPS", new Vector2(0, 50), Color.White);
            */

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}
