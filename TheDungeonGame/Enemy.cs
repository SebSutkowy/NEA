
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;

namespace TheDungeonGame
{
    public class Enemy : Entity
    {
        private UIRect HealthBar;
        private int HealthBarMaxLength;
        private int FrameDamageTime;
        public Enemy(Texture2D texture, Vector2 position, float rotation, int maxHealth, int health, float damage, float speed) : base(texture, position, rotation, maxHealth, health, damage, speed)
        {
            HealthBarMaxLength = Texture.Width;
            HealthBar = new UIRect(
                    new Rectangle((int)(Position.X - Texture.Width / 2), (int)(Position.Y - Texture.Height / 2 - 12), (Health / MaxHealth) * HealthBarMaxLength, 10),
                    Color.Red
                    );
            FrameDamageTime = 15;
            FramesSinceDamage = FrameDamageTime;
        }

        public void Update()
        {
            HealthBar.ChangeSize(new Vector2((float)Health * (float)MaxHealth / (float)HealthBarMaxLength, 10));
            FramesSinceDamage = MathHelper.Min(FrameDamageTime, FramesSinceDamage+1);
        }

        public new void Draw()
        {
            if (FramesSinceDamage < 15)
                base.Draw(Color.Red);
            else
                base.Draw();
            HealthBar.Draw(false);
        }
    }
}
