using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunnySlayerCommon
{
    public static class MenuClass
    {
        public static string[] TargetWeight = new string[]
        {
                "Min Heath",
                "Min Armor",
                "Min Mana",
                "Min Auto Attack to die",

                "Max Heath",
                "Max Armor",
                "Max Mana",
                "Max Auto Attack to die",

                "Cant Move",
                "Can Move",

                "Cant Attack",
                "Can Attack",

                "Ranged",
                "Melee",
        };

        public static Menu ThisMenu;
        public static MenuList SecondMenu;
        public static MenuList GetWeight = new MenuList("Get Weight", "Get Weight", TargetWeight, 0);
        public static Menu GetPriority = new Menu("Get Priority", "Get Priority");

        public static MenuSliderButton DrawTarget = new MenuSliderButton("Draw Target", "Draw Target", 1000, 0, 2000, false);
     
        public static void AddTargetSelectorMenu(this Menu menu)
        {
            ThisMenu = menu;
            var FirstMenu = new MenuSeparator("FS Target Selector", "FS Target Selector");
            menu.Add(FirstMenu);
            SecondMenu = new MenuList("FS Select Mode", "FS Select Mode", new string[] { "Selected", "Weight", "Priority" }, 1);
            menu.Add(SecondMenu);

            var Selected = new MenuSeparator("Selected", "Selected");
            var Weight = new MenuSeparator("Weight", "Weight");
            var Priority = new MenuSeparator("Priority", "Priority");

            menu.Add(Selected);
                      
            foreach(var t in GameObjects.AllGameObjects.ToArray())
            {
                if(t is AIHeroClient && t.IsEnemy)
                {
                    GetPriority.Add(new MenuSlider("FS Priority " + t.Name, (t as AIHeroClient).CharacterName, (t as AIHeroClient).IsRanged ? 5 : 1, 0, 10));
                }
            }

            menu.Add(Weight);
            menu.Add(GetWeight);

            menu.Add(Priority);
            menu.Add(GetPriority);

            menu.Add(DrawTarget);

            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (DrawTarget.Enabled  && FSTargetSelector.GetFSTarget(DrawTarget.ActiveValue) != null)
            {
                var targetpos = Drawing.WorldToScreen(FSTargetSelector.GetFSTarget(DrawTarget.ActiveValue).Position);
                Drawing.DrawLine(new Vector2(targetpos.X, targetpos.Y), new Vector2(targetpos.X + 25, targetpos.Y - 25), 5f, System.Drawing.Color.Red);
                Drawing.DrawLine(new Vector2(targetpos.X, targetpos.Y), new Vector2(targetpos.X - 25, targetpos.Y - 25), 5f, System.Drawing.Color.Red);

                Drawing.DrawCircle(FSTargetSelector.GetFSTarget(DrawTarget.ActiveValue).Position, 100, System.Drawing.Color.Red);
            }
        }
    }
}
