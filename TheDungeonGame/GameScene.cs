using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheDungeonGame
{
    public class GameScene : Scene
    {
        public const SceneName Name = SceneName.Game;
        Player player;

        public GameScene()
        {
        }

        public override void Update()
        {
        }

        public override void Draw()
        {
            Camera.DrawString("Hello, this is the Game Scene", Vector2.Zero, Color.White);
        }
    }
}
