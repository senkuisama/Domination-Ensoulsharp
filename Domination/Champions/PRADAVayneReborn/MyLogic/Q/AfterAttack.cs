using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Prediction;
using PRADA_Vayne.MyUtils;
using System;
using System.Linq;
using Troll_Chat_xD.Helper;

namespace PRADA_Vayne.MyLogic.Q
{
    public static partial class Events
    {
        public static void AfterAttack(object sender, OrbwalkerActionArgs args)
        {
            if (args.Type != OrbwalkerType.AfterAttack) return;
            //var targets = GameObjects.AllGameObjects.Where(i => !i.IsAlly && !i.IsMe && !i.IsDead);
            //foreach(var target in targets)
            //{
                if (!Program.Q.IsReady()) return;
                if (args.Target.IsValidTarget(800) &&
                    (Orbwalker.ActiveMode == OrbwalkerMode.Combo ||
                     !Program.MainMenu.GetMenuBool("Combo Settings", "OnlyQinCombo") == true))
                {
                    var tg = args.Target as AIHeroClient;
                    if (tg == null) return;
                    var mode = Program.MainMenu.GetMenuList("Combo Settings", "QMode");
                    var tumblePosition = Game.CursorPos;
                    switch (mode)
                    {
                        case "PRADA":
                            tumblePosition = tg.GetTumblePos();
                            break;

                        default:
                            tumblePosition = Game.CursorPos;
                            break;
                    }

                    Tumble.Cast(tumblePosition);
                }

            var ms = ObjectManager.Get<AIMinionClient>().Where(minion => minion.NetworkId != minion.NetworkId && minion.IsEnemy && minion.IsValidTarget(615));
            if(ms != null)
            foreach (var m in ms)
            {
                    if (!m.IsValidTarget(800)) return;
                if (m != null && Program.MainMenu.GetMenuBool("Laneclear Settings", "QLastHit") == true &&
                ObjectManager.Player.ManaPercent >=
                Program.MainMenu.GetMenuSlider("Laneclear Settings", "QLastHitMana") &&
                Orbwalker.ActiveMode == OrbwalkerMode.LastHit ||
                Orbwalker.ActiveMode == OrbwalkerMode.LaneClear)
                {
                    var dashPosition = Game.CursorPos;
                    var mode = Program.MainMenu.GetMenuList("Combo Settings", "QMode");
                    switch (mode)
                    {
                        case "PRADA":
                            dashPosition = m.GetTumblePos();
                            break;

                        default:
                            dashPosition = Game.CursorPos;
                            break;
                    }

                    if (m.Team == GameObjectTeam.Neutral) Program.Q.Cast(dashPosition);
                    foreach (var minion in ObjectManager.Get<AIMinionClient>().Where(minion =>
                        m.NetworkId != minion.NetworkId && minion.IsEnemy && minion.IsValidTarget(615)))
                    {
                        if (minion == null)
                            break;
                        var time = (int)(ObjectManager.Player.AttackCastDelay * 1000) + Game.Ping / 2 + 1000 *
                                   (int)Math.Max(0,
                                       ObjectManager.Player.Distance(minion) - ObjectManager.Player.BoundingRadius) /
                                   (int)ObjectManager.Player.BasicAttack.MissileSpeed;
                        var predHealth = HealthPrediction.GetPrediction(minion, time);
                        if (predHealth < ObjectManager.Player.GetAutoAttackDamage(minion) + Program.Q.GetDamage(minion) &&
                            predHealth > 0)
                            Program.Q.Cast(dashPosition, true);
                    }
                }
            }
        }
    }
}