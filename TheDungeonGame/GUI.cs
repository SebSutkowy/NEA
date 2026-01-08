using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;

namespace TheDungeonGame
{
    public enum GUINames
    {
        None,
        Game,
        Shop,
        Dialogue
    }

    public enum ShopGUIElements : int
    {
        Background = 0,
        UpgradeNormalAttack = 1,
        UpgradeSpecialAttack = 2,
        HealButton = 3,
        HealthBar = 4,
    }

    public enum DialogueGUIElements : int
    {
        DialogueBox = 0
    }

    public enum GameGUIElements : int
    {
        HealthBar = 0
    }

    public class GUI
    {
        private GUINames Name { get; set; }
        private Dictionary<int, UIRect> Elements { get; set; }
        
        public GUI()
        {
            Name = GUINames.None;
            Elements = new Dictionary<int, UIRect>();
        }

        public GUI(GUINames name)
        {
            Name = name;
            Elements = new Dictionary<int, UIRect>();
        }

        public void AddElement(int name, UIRect rect)
        {
            Elements.Add(name, rect);
        }

        public UIRect this[int name]
        {
            get => Elements[name];
        }


        public void Draw()
        {
            foreach (UIRect element in Elements.Values)
            {
                element.Draw();
            }
        }

    }
}
