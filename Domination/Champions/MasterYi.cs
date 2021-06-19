using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp;
using DaoHungAIO.Evade;

namespace DominationAIO.Champions
{
    public class MasterYi
    {
        private static AIHeroClient Player = GameObjects.Player;
        private static Spell Q, E, R;
        private static Menu YiMenu;
        public static void YiLoad()
        {
            if (Player == null)
                return;

            YiMenu = new Menu("Yi Load", "Yi Load", true);
            EvadeManager.Attach(YiMenu);
            //FunnySlayerCommon.MenuClass.AddTargetSelectorMenu(YiMenu);
            YiMenu.Attach();

            Q = new Spell(SpellSlot.Q, 625f);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R);

            Q.SetTargetted(0f, float.MaxValue);

            Game.OnUpdate += Game_OnUpdate;
            Game.OnUpdate += Game_OnUpdate1;
            Game.OnUpdate += Game_OnUpdate2;
            AIBaseClient.OnProcessSpellCast += AIBaseClient_OnProcessSpellCast;
        }

        private static void Game_OnUpdate2(EventArgs args)
        {
            if (Player.IsDead)
                return;

            if (Q.IsReady())
            {
                var target = TargetSelector.GetTargets(Q.Range, DamageType.Physical).Where(i => i.Health < Q.GetDamage(i)).OrderBy(i => i.Health).FirstOrDefault();
                if(target != null)
                {
                    if (Q.Cast(target) == CastStates.SuccessfullyCasted || Q.CastOnUnit(target))
                    {
                        return;
                    }
                }
            }
        }

        private static void Game_OnUpdate1(EventArgs args)
        {
            if (Player.IsDead)
                return;

            if (Q.IsReady())
            {
                var NotSafe = EvadeManager.DetectedSkillshots.Any(i => !i.IsSafePoint(Player.Position.ToVector2()));

                if (NotSafe)
                {
                    if(Orbwalker.ActiveMode <= OrbwalkerMode.Harass)
                    {
                        var target = TargetSelector.GetTargets(Q.Range, DamageType.Physical).OrderBy(i => i.Health).FirstOrDefault(i => !Yasuo_LogicHelper.Logichelper.UnderTower(i.Position));
                        if(target != null)
                        {
                            if(Q.Cast(target) == CastStates.SuccessfullyCasted || Q.CastOnUnit(target))
                            {
                                return;
                            }
                        }
                    }
                    else
                    {
                        var obj = ObjectManager.Get<AIMinionClient>().Where(i => i.IsValidTarget(Q.Range) && !i.IsAlly && !i.IsDead).OrderBy(i => i.Health).LastOrDefault(i => !Yasuo_LogicHelper.Logichelper.UnderTower(i.Position));
                        if (obj != null)
                        {
                            if (Q.Cast(obj) == CastStates.SuccessfullyCasted || Q.CastOnUnit(obj))
                            {
                                return;
                            }
                        }
                    }
                }
            }
        }

        private static void AIBaseClient_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
                return;

            if (args.Target.IsMe && sender is AITurretClient)
            {
                if (Orbwalker.ActiveMode <= OrbwalkerMode.Harass)
                {
                    var target = TargetSelector.GetTargets(Q.Range, DamageType.Physical).OrderBy(i => i.Health).FirstOrDefault();
                    if (target != null)
                    {
                        if (Q.Cast(target) == CastStates.SuccessfullyCasted || Q.CastOnUnit(target))
                        {
                            return;
                        }
                    }
                }
                else
                {
                    var obj = ObjectManager.Get<AIMinionClient>().Where(i => i.IsValidTarget(Q.Range) && !i.IsAlly && !i.IsDead).OrderBy(i => i.Health).LastOrDefault(i => !Yasuo_LogicHelper.Logichelper.UnderTower(i.Position));
                    if (obj != null)
                    {
                        if (Q.Cast(obj) == CastStates.SuccessfullyCasted || Q.CastOnUnit(obj))
                        {
                            return;
                        }
                    }
                    var target = TargetSelector.GetTargets(Q.Range, DamageType.Physical).OrderBy(i => i.Health).FirstOrDefault(i => !Yasuo_LogicHelper.Logichelper.UnderTower(i.Position));
                    if (target != null)
                    {
                        if (Q.Cast(target) == CastStates.SuccessfullyCasted || Q.CastOnUnit(target))
                        {
                            return;
                        }
                    }
                }
            }
            if (args.Target.IsMe && sender is AIHeroClient && args.Slot <= SpellSlot.R)
            {
                if (Orbwalker.ActiveMode <= OrbwalkerMode.Harass)
                {
                    var target = TargetSelector.GetTargets(Q.Range, DamageType.Physical).OrderBy(i => i.Health).FirstOrDefault(i => !Yasuo_LogicHelper.Logichelper.UnderTower(i.Position));
                    if (target != null)
                    {
                        if (Q.Cast(target) == CastStates.SuccessfullyCasted || Q.CastOnUnit(target))
                        {
                            return;
                        }
                    }
                }
                else
                {
                    var obj = ObjectManager.Get<AIMinionClient>().Where(i => i.IsValidTarget(Q.Range) && !i.IsAlly && !i.IsDead).OrderBy(i => i.Health).LastOrDefault(i => !Yasuo_LogicHelper.Logichelper.UnderTower(i.Position));
                    if (obj != null)
                    {
                        if (Q.Cast(obj) == CastStates.SuccessfullyCasted || Q.CastOnUnit(obj))
                        {
                            return;
                        }
                    }
                    var target = TargetSelector.GetTargets(Q.Range, DamageType.Physical).OrderBy(i => i.Health).FirstOrDefault(i => !Yasuo_LogicHelper.Logichelper.UnderTower(i.Position));
                    if (target != null)
                    {
                        if (Q.Cast(target) == CastStates.SuccessfullyCasted || Q.CastOnUnit(target))
                        {
                            return;
                        }
                    }
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;

            if (E.IsReady())
            {
                if(Orbwalker.GetTarget() != null && Orbwalker.ActiveMode <= OrbwalkerMode.Harass)
                {
                    if (E.Cast())
                    {
                        return;
                    }
                }
            }

            if (Q.IsReady())
            {
                var target = FunnySlayerCommon.FSTargetSelector.GetFSTarget(Q.Range);
                if(target != null && Orbwalker.ActiveMode <= OrbwalkerMode.Harass)
                {
                    if (target.IsDashing())
                    {
                        if(Q.Cast(target) == CastStates.SuccessfullyCasted || Q.CastOnUnit(target))
                        {
                            return;
                        }
                    }
                    if(target.DistanceToPlayer() > 550)
                    {
                        if (Q.Cast(target) == CastStates.SuccessfullyCasted || Q.CastOnUnit(target))
                        {
                            return;
                        }
                    }
                }
            }
            if (R.IsReady() && Orbwalker.ActiveMode <= OrbwalkerMode.Harass)
            {
                if(Player.CountEnemyHeroesInRange(625) > 2)
                {
                    if (R.Cast())
                    {
                        return;
                    }
                }
            }
        }
    }
}
