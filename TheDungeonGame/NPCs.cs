using Microsoft.VisualBasic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

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
        private int CurrentDialogue { get; set; }
        private bool IsTalking { get; set; }

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
            DialogueBox = new DialogueBox(new Rectangle(10, Camera.ScreenHeight / 2 + 10, Camera.ScreenWidth - 20, Camera.ScreenHeight / 2 - 20), new Color(20, 20, 20, 200), AssetManager.GetDialogue(id));
            Dialogue = AssetManager.GetDialogue(id);
            
            CurrentDialogue = 0;
            IsTalking = false;
        }

        public void Update()
        {
        }

        public void Interact()
        {
            UI.SetGUI(GUINames.Dialogue);
            UI.SetDialogue(Dialogue);
        }

        public void Draw(Vector2 pos)
        {
            AnimationManager.Draw(pos, Rotation, Origin);
            if (IsTalking)
                DialogueBox.Draw();
        }
    }
}
