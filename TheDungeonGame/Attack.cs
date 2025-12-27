using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

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

    public enum NormalAttacks
    {
        Swing,
        BowShot,
        Orb
    }

    public enum SpecialAttacks
    {
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

    public class Skillset 
    {
        public NormalAttacks NormalAttack;
        public SpecialAttacks SpecialAttack;
        public Utilities Utility;

        public int NormalAttackLevel;
        public int SpecialAttack1Level;
        public int SpecialAttack2Level;

        public Skillset()
        {
            NormalAttackLevel = 1;
            SpecialAttack1Level = 1;
            SpecialAttack2Level = 1;
        }

        public Skillset(Classes playerClass)
        {
            switch (playerClass)
            {
                case Classes.Berserker:
                    NormalAttack = NormalAttacks.Swing;
                    SpecialAttack = SpecialAttacks.Sweep;
                    Utility = Utilities.Rage;
                    break;
                case Classes.Archer:
                    NormalAttack = NormalAttacks.BowShot;
                    SpecialAttack = SpecialAttacks.PowerShot;
                    Utility = Utilities.Stealth;
                    break;
                case Classes.Mage:
                    NormalAttack = NormalAttacks.Orb;
                    SpecialAttack = SpecialAttacks.Beam;
                    Utility = Utilities.Drain;
                    break;
            }
        }

        public Rectangle GetHitbox(Vector2 position, float rotation, AttackType attackType)
        {
            // create a table for all the functions of the new attack position given these parameters f(x, y, θ) = rect

            return new Rectangle((int)(position.X + Math.Sin(rotation)), (int)(position.Y + Math.Cos(rotation)), 50, 50);
        }

        public float GetDamage(AttackType attackType)
        {
            /* in notepad, create a table / data set for damage of all attacks and level scaling e.g. swing = 5 (+2 per level) */

            return 0f;
        }



    }
}