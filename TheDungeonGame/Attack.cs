using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace TheDungeonGame
{
    public enum AttackType
    {
        Melee,
        Projectile,
        Beam
    }

    public class Attack : Sprite
    {
        public Attack(Texture2D texture, Vector2 position, float rotation) : base(texture, position, rotation) { }

        public void ChangeWeapon(Texture2D newTexture)
        {
            Texture = newTexture;
        }
    }

    public class MeleeAttack : Attack
    {
        public readonly float Damage = 10f;
        public readonly float ManaCost = 0f;
        public readonly AttackType Type = AttackType.Melee;

        public MeleeAttack(Texture2D texture, Vector2 position, float rotation) : base(texture, position, rotation)
        { }
    }

    public class ProjectileAttack : Attack 
    {
        public readonly float Damage = 5f;
        public readonly float ManaCost = 0f;
        public readonly AttackType Type = AttackType.Projectile;

        public ProjectileAttack(Texture2D texture, Vector2 position, float rotation) : base(texture, position, rotation)
        { }
    }

    public class BeamAttack : Attack
    {
        public readonly float Damage = 3f;
        public readonly float ManaCost = 5f;
        public readonly AttackType Type = AttackType.Beam;

        public BeamAttack(Texture2D texture, Vector2 position, float rotation) : base(texture, position, rotation)
        { }
    }
}