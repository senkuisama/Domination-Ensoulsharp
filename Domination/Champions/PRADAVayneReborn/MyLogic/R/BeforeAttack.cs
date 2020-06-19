using DominationAIO.Champions.Helper;
using EnsoulSharp;
using EnsoulSharp.SDK;
using PRADA_Vayne.MyUtils;
using System.Linq;

namespace PRADA_Vayne.MyLogic.R
{
    public static partial class Events
    {
        public static void BeforeAttack(object sender, OrbwalkerActionArgs args)
        {
            if (args.Type != OrbwalkerType.BeforeAttack) return;
            if (args.Sender.IsMe || Program.Q.IsReady() || Program.MainMenu.GetMenuBool("Combo Settings", "QCombo"))
                if (ObjectManager.Player.HasBuff("vaynetumblefade") &&
                    Program.MainMenu.GetMenuBool("EscapeMenu", "QUlt") && Heroes.EnemyHeroes.Any(h =>
                        h.IsMelee && h.Distance(Heroes.Player) < h.AttackRange + h.BoundingRadius))
                    args.Process = false;
        }
    }
}