using DominationAIO.Champions.Helper;
using EnsoulSharp;
using EnsoulSharp.SDK;
using PRADA_Vayne.MyUtils;
using System.Linq;

namespace PRADA_Vayne.MyLogic.Q
{
    public static partial class Events
    {
        public static void BeforeAttack(object sender, OrbwalkerActionArgs args)
        {
            if (args.Type != OrbwalkerType.BeforeAttack) return;
            if (Program.Q.IsReady() && Program.MainMenu.GetMenuBool("Combo Settings", "QCombo") == true)
                if (args.Target.IsValid)
                {
                    var targets = TargetSelector.GetTargets(700);
                    if(targets.Count() > 0)
                    foreach(var target in targets)
                    {
                        if (target != null)
                        {
                            if (Program.MainMenu.GetMenuBool("Combo Settings", "RCombo") && Program.R.IsReady() &&
                               Orbwalker.ActiveMode == OrbwalkerMode.Combo)
                                Program.R.Cast();
                            if (target.IsMelee && target.IsFacing(Heroes.Player))
                                if (target.Distance(Heroes.Player.Position) < 325)
                                {
                                    var tumblePosition = target.GetTumblePos();
                                    args.Process = false;
                                    Tumble.Cast(tumblePosition);
                                }

                            var closestJ4Wall = ObjectManager.Get<AIMinionClient>().FirstOrDefault(m =>
                                m.CharacterName == "jarvanivwall" && ObjectManager.Player.Position.Distance(m.Position) < 100);
                            if (closestJ4Wall != null)
                            {
                                args.Process = false;
                                Program.Q.Cast(ObjectManager.Player.Position.Extend(closestJ4Wall.Position, 300));
                            }

                        }
                    }                                       
                }
        }
    }
}