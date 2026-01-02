using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TheDungeonGame
{

    public enum NPCId
    {
        Shopkeep
    }

    public class NPC : Sprite 
    {
        public  NPCId Id { get; private set; }

        private DialogueBox DialogueBox { get; set; }
        private List<string> Dialogue { get; set; }

        public NPC() : base()
        { }

        public NPC(float rotation, NPCId id) : base(null, Vector2.Zero, rotation)
        {
            AnimationManager = new AnimationManager(AssetManager.GetSpriteSheet(SpriteSheets.Player));
            Animation anim = new Animation(new Vector2(100f), 0, 1, 0);
            anim.Pause();
            AnimationManager.AddAnimation(AnimationNames.Idle, anim);
            AnimationManager.ChangeAnimation(AnimationNames.Idle);

            Id = id;
            DialogueBox = new DialogueBox();
            Dialogue = new List<string>();
        }

        public void Interact()
        {
            // Do something

        }

        public void Draw(Vector2 pos)
        {
            AnimationManager.Draw(pos, Rotation, Origin);
        }
    }
}
