using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominationAIO.Champions
{
    internal class Sion
    {
        public static MenuBool Activated = new MenuBool("Activated", "R Sion Activated", true);
        public static void SionLoad()
        {
            Game.OnUpdate += Game_OnUpdate;

            var menu = new Menu("Sion", "SionR", true);
          
            menu.Add(Activated).Permashow();
            menu.Attach();

        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead) return;

            if (ObjectManager.Player.HasBuff("SionR"))
            {
                var target = TargetSelector.SelectedTarget;

                if(target == null || target.IsVisible)
                {
                    target = TargetSelector.GetTarget(2000, DamageType.Physical);
                }

                if (target == null) return;


                if (!Activated.Enabled && Orbwalker.ActiveMode != OrbwalkerMode.Combo) return;
                else
                ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, target);


            }
        }
    }
}
