using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Windows.Markup;

namespace TheDungeonGame
{
    public enum Classes
    {
        Berserker,
        Archer,
        Mage
    }

    public enum AttackType
    {
        NormalAttack,
        SpecialAttack
    }

    public enum Attacks
    {
        Swing,
        BowShot,
        Orb,
        Sweep,
        PowerShot,
        Beam
    }

    public enum Utilities
    {
        Rage,
        Stealth,
        Drain
    }

    public class Skillset : Sprite
    {
        public bool IsAttacking => AnimationManager.CurrentAnimation != AnimationNames.None;

        public Attacks NormalAttack { get; set; }
        public Attacks SpecialAttack { get; set; }
        public Utilities Utility { get; set; }

        public int NormalAttackLevel { get; set; }
        public int SpecialAttackLevel { get; set; }
        public int UtilityLevel { get; set; }

        public readonly Dictionary<Attacks, AnimationNames> AttackAnimations = new Dictionary<Attacks, AnimationNames>()
        {
            {Attacks.Swing, AnimationNames.SwingAttack },
            {Attacks.BowShot, AnimationNames.BowShotAttack },
            {Attacks.Orb, AnimationNames.OrbAttack }
        };

        public Skillset()
        {
            NormalAttackLevel = 1;
            SpecialAttackLevel = 1;
            UtilityLevel = 1;
        }

        public Skillset(Classes playerClass)
        {
            AnimationManager = new AnimationManager(AssetManager.GetSpriteSheet(SpriteSheets.Weapons));
            Animation swingAnimation = new Animation(frameSize: new Vector2(100f), spriteSheetRow: 0, frameCount: 5, frameInterval: 3, looping: false);
            AnimationManager.AddAnimation(AnimationNames.SwingAttack, swingAnimation);

            switch (playerClass)
            {
                case Classes.Berserker:
                    NormalAttack = Attacks.Swing;
                    SpecialAttack = Attacks.Sweep;
                    Utility = Utilities.Rage;
                    break;
                case Classes.Archer:
                    NormalAttack = Attacks.BowShot;
                    SpecialAttack = Attacks.PowerShot;
                    Utility = Utilities.Stealth;
                    break;
                case Classes.Mage:
                    NormalAttack = Attacks.Orb;
                    SpecialAttack = Attacks.Beam;
                    Utility = Utilities.Drain;
                    break;
            }
            NormalAttackLevel = 1;
            SpecialAttackLevel = 1;
            UtilityLevel = 1;
        }

        public void Update(Vector2 position, float rotation)
        {
            Position = new Vector2(position.X + 75 * (float)Math.Sin(rotation), position.Y - 75 * (float)Math.Cos(rotation));
            Rotation = rotation;
            AnimationManager.Update();
        }

        public void Attack(AttackType attackType)
        {
            AnimationNames name;
            switch (attackType)
            {
                case AttackType.NormalAttack:
                    name = AttackAnimations[NormalAttack];
                    break;
                case AttackType.SpecialAttack:
                    name = AttackAnimations[SpecialAttack];
                    break;
                default:
                    name = AnimationNames.None;
                    break;
            }
            AnimationManager.Play(name);
        }

        public float GetDamage(AttackType attackType)
        {
            /* in notepad, create a table / data set for damage of all attacks and level scaling e.g. swing = 5 (+2 per level) */

            return 5f;
        }

    }
}

