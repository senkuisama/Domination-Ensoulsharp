using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominationAIO.NewPlugins
{
    public static class MyViego
    {
        private static class ViegoMenu
        {
            public static class QCombo
            {
                public static MenuBool useQCombo = new MenuBool("UseQCombo", "Use Q Combo and Harass");
                public static MenuBool QCancelAA = new MenuBool("QcancelAA", "Accept Q cancel AA");
                public static MenuBool QCheckQPassive = new MenuBool("CheckQPassive", "Check Q Passive");
            }
            public static class WCombo
            {
                public static MenuBool useWCombo = new MenuBool("UseWCombo", "Use W Combo and Harass");
                public static MenuBool WCancelAA = new MenuBool("WcancelAA", "Accept W cancel AA");
                public static MenuBool WCheckQPassive = new MenuBool("WCheckQPassive", "Check Q Passive");
            }
            public static class ECombo
            {
                public static MenuBool useECombo = new MenuBool("UseECombo", "Use E Combo", false);
                public static MenuBool CheckWall = new MenuBool("CheckWall", "Check Wall");
            }
            public static class RCombo
            {
                public static MenuBool useR = new MenuBool("UseR", "R in Combo");
                public static MenuBool TakeSoul = new MenuBool("TakeSoul", "Take Soul");

                public static MenuSlider TargetHeath = new MenuSlider("TargetHeath", "Target Heath <= x% ", 40, 0, 101);
                public static MenuKeyBind useRinturret = new MenuKeyBind("useRinturret", "Accept R Turret", Keys.T, KeyBindType.Toggle);
            }
        }

        private static Menu ViegoRoot = null;
        private static string Soul = "ViegoSoul";
        private static string Mark = "viegoqmark";
        private static Spell Q, W, E, Esoul, R = null;
        public static void ViegoLoad()
        {
            Q = new Spell(SpellSlot.Q, 600f);
            W = new Spell(SpellSlot.W, 400f);
            E = new Spell(SpellSlot.E, 700f);
            Esoul = new Spell(SpellSlot.Unknown, 400f);
            R = new Spell(SpellSlot.R, 500f);

            Q.SetSkillshot(0.35f, 60f, float.MaxValue, false, SpellType.Line);
            W.SetSkillshot(0.25f, 60f, 800f, true, SpellType.Line);
            W.SetCharged("ViegoW", "ViegoW", 400, 900, 1f);
            R.SetSkillshot(1f, 100f, float.MaxValue, false, SpellType.Circle);
            Game.Print("Viego Loaded. Not support Passive champions");

            ViegoRoot = new Menu("FunnySlayerViego", "FunnySlayer Viego", true);
            var Qmenu = new Menu("Qmenu", "Q Settings");
            var Wmenu = new Menu("Wmenu", "W Settings");
            var Emenu = new Menu("Emenu", "E Settings");
            var Rmenu = new Menu("Rmenu", "R Settings");

            Qmenu.Add(ViegoMenu.QCombo.useQCombo);
            Qmenu.Add(ViegoMenu.QCombo.QCancelAA);
            Qmenu.Add(ViegoMenu.QCombo.QCheckQPassive);

            Wmenu.Add(ViegoMenu.WCombo.useWCombo);
            Wmenu.Add(ViegoMenu.WCombo.WCancelAA);
            Wmenu.Add(ViegoMenu.WCombo.WCheckQPassive);

            Emenu.Add(ViegoMenu.ECombo.useECombo);
            Emenu.Add(ViegoMenu.ECombo.CheckWall);

            Rmenu.Add(ViegoMenu.RCombo.useR);
            Rmenu.Add(ViegoMenu.RCombo.useRinturret).Permashow();
            Rmenu.Add(ViegoMenu.RCombo.TargetHeath);
            Rmenu.Add(ViegoMenu.RCombo.TakeSoul).Permashow();
            var GetEnemy = ObjectManager.Get<AIHeroClient>().Where(i => !i.IsAlly);
            foreach (var enemy in GetEnemy.Where(i => i.CharacterName != ObjectManager.Player.CharacterName))
            {
                Rmenu.Add(new MenuSeparator(enemy.NetworkId + enemy.CharacterName + enemy.Name, enemy.CharacterName + " Not support !"));
            }

            ViegoRoot.Add(Qmenu);
            ViegoRoot.Add(Wmenu);
            ViegoRoot.Add(Emenu);
            ViegoRoot.Add(Rmenu);
            ViegoRoot.Attach();

            Game.OnUpdate += Game_OnUpdate;
            Game.OnUpdate += Game_OnUpdate1;
            Game.OnUpdate += Game_OnUpdate2;
        }

        private static void Game_OnUpdate2(EventArgs args)
        {
            if (ObjectManager.Player.IsDead || !Q.IsReady())
                return;

            if (Orbwalker.ActiveMode != OrbwalkerMode.LaneClear)
                return;

            if (FunnySlayerCommon.OnAction.OnAA || FunnySlayerCommon.OnAction.BeforeAA)
                return;

            var Qminions = ObjectManager.Get<AIBaseClient>().Where(i => !i.IsDead && i.Health > 0 && i.IsEnemy && i.IsValidTarget(600f) && (i.Type == GameObjectType.AIMinionClient || i.Type == GameObjectType.AIHeroClient || i.IsMinion() || i.IsJungle())).OrderBy(i => i.Type == GameObjectType.AIHeroClient);
            if(Qminions != null)
            {
                foreach(var minion in Qminions)
                {
                    if (minion == null)
                        return;

                    if (minion.Type == GameObjectType.AIHeroClient)
                    {
                        if (!Helper.UnderTower(ObjectManager.Player.Position))
                        {
                            var pred = Q.GetPrediction(minion);
                            if(pred.Hitchance >= HitChance.Medium)
                            {
                                if (Q.Cast(pred.CastPosition))
                                    return;
                            }
                        }
                    }
                    else
                    {
                        var farmpred = Q.GetLineFarmLocation(Qminions.ToList());
                        if(farmpred.MinionsHit >= 1)
                        {
                            if (Q.Cast(farmpred.Position))
                                return;
                        }
                    }
                }
            }
        }

        private static bool OnAA, BAA = false;
        private static void Game_OnUpdate1(EventArgs args)
        {
            OnAA = FunnySlayerCommon.OnAction.OnAA;
            BAA = FunnySlayerCommon.OnAction.BeforeAA;

            if (W.IsCharging)
            {
                Orbwalker.AttackEnabled = false;
            }
            else
            {
                Orbwalker.AttackEnabled = true;
            }
        }

        private static bool CanUseR(AIBaseClient target = null, bool checkhp = true)
        {
            if (target == null)
                return false;
            var pred = R.GetPrediction(target);

            return (!OnAA && !BAA) && ViegoMenu.RCombo.useR.Enabled && (!Helper.UnderTower(target.Position) || ViegoMenu.RCombo.useRinturret.Active) && pred.Hitchance >= HitChance.High && (target.HealthPercent <= ViegoMenu.RCombo.TargetHeath.Value || !checkhp);
        }

        private static bool CanUseQ(AIBaseClient target = null)
        {
            if (target == null)
                return false;
            var pred = Q.GetPrediction(target);

            return !ObjectManager.Player.IsDashing() && (!target.HasBuff(Mark) || !target.IsValidTarget(150 + ObjectManager.Player.GetCurrentAutoAttackRange())) && ViegoMenu.QCombo.useQCombo.Enabled && pred.Hitchance >= HitChance.High && (ViegoMenu.QCombo.QCancelAA.Enabled || (!OnAA && !BAA));
        }

        private static bool CanUseW(AIBaseClient target = null)
        {
            if (target == null)
                return false;
            var tempw = new Spell(SpellSlot.Unknown, 800f);
            tempw.SetSkillshot(0.25f, 60f, 800f, true, SpellType.Line);

            var pred = SebbyLibPorted.Prediction.Prediction.GetPrediction(tempw, target); ;

            return !ObjectManager.Player.IsDashing() && (!target.HasBuff(Mark) || !target.IsValidTarget(150 + ObjectManager.Player.GetCurrentAutoAttackRange())) && ViegoMenu.WCombo.useWCombo.Enabled && pred.Hitchance >= SebbyLibPorted.Prediction.HitChance.High && (ViegoMenu.WCombo.WCancelAA.Enabled || (!OnAA && !BAA));
        }

        private static bool CanUseE(AIBaseClient target = null)
        {
            if (target == null)
                return false;

            var circles = new Geometry.Circle(target.Position, 400f).Points.OrderBy(c => c.DistanceToPlayer());
            var check = false;
            foreach (var circle in circles)
            {
                for (int i = 0; i <= 700; i++)
                {
                    var flag = NavMesh.GetCollisionFlags((ObjectManager.Player.Position.ToVector2().Extend(circle, i)).ToVector3());

                    if (flag.HasFlag(CollisionFlags.Building) || flag.HasFlag(CollisionFlags.Wall))
                    {
                        check = true;
                        i += 800;
                    }
                    else
                    {
                        check = false;
                    }
                }

                if (!check)
                    continue;
            }

            return (!OnAA && !BAA) && ViegoMenu.ECombo.useECombo.Enabled && (check || !ViegoMenu.ECombo.CheckWall.Enabled);
        }
        private static Vector3 EcastPos(AIBaseClient target = null)
        {
            if (target == null)
                return Vector3.Zero;

            var circles = new Geometry.Circle(target.Position, 400f).Points.OrderBy(c => c.DistanceToPlayer());
            var check = false;
            var pos = Vector3.Zero;
            foreach (var circle in circles)
            {
                for (int i = 0; i <= 700; i++)
                {
                    var flag = NavMesh.GetCollisionFlags((ObjectManager.Player.Position.ToVector2().Extend(circle, i)).ToVector3());

                    if (flag.HasFlag(CollisionFlags.Building) || flag.HasFlag(CollisionFlags.Wall))
                    {
                        check = true;
                        i += 800;
                        pos = circle.ToVector3();
                    }
                    else
                    {
                        check = false;
                    }
                }

                if (!check)
                    continue;
            }

            if (check)
            {
                return pos;
            } else
                return Vector3.Zero;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
                return;

            if (Orbwalker.ActiveMode > OrbwalkerMode.Harass) 
                return;
            var target = TargetSelector.GetTarget(900, DamageType.Physical);
            if (target == null)
                return;

            if (Q.Name == "ViegoQ")
            {               
                if (W.IsCharging)
                {
                    var Wpred = SebbyLibPorted.Prediction.Prediction.GetPrediction(W, target);

                    if (Wpred.Hitchance >= SebbyLibPorted.Prediction.HitChance.High)
                    {
                        if (W.Cast(Wpred.CastPosition))
                            return;
                    }
                }
                else
                {

                    var Rtarget = TargetSelector.GetTarget(R.Range, DamageType.Physical);
                    if (Rtarget != null)
                    {
                        if (R.IsReady() && ViegoMenu.RCombo.useR.Enabled && CanUseR(Rtarget))
                        {
                            var Rpred = R.GetPrediction(Rtarget);
                            if (Rpred.Hitchance >= HitChance.High)
                            {
                                if (R.Cast(Rpred.CastPosition))
                                    return;
                            }
                        }
                        else
                        {
                            if (!R.IsReady() && R.Level >= 1 && ViegoMenu.RCombo.TakeSoul.Enabled)
                            {
                                var gettargetsoul = ObjectManager.Get<AIBaseClient>().Where(i => !i.IsAlly && i.Health <= 0 && i.DistanceToPlayer() <= ObjectManager.Player.GetCurrentAutoAttackRange() + 200).OrderByDescending(i => i.MaxHealth).FirstOrDefault();
                                if (gettargetsoul != null)
                                {
                                    ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, gettargetsoul);
                                }
                            }

                            {
                                var Etarget = TargetSelector.GetTarget(E.Range, DamageType.Physical);
                                if (Etarget != null)
                                {
                                    if (E.IsReady() && CanUseE(Etarget))
                                    {
                                        if(EcastPos(Etarget) != Vector3.Zero)
                                        {
                                            if (E.Cast(EcastPos(Etarget)))
                                                return;
                                        }
                                        else
                                        {
                                            if (Q.IsReady() && CanUseQ(target))
                                            {
                                                var Qtarget = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                                                if (target != null)
                                                {
                                                    var Qpred = Q.GetPrediction(Qtarget);
                                                    if (Qpred.Hitchance >= HitChance.High)
                                                    {
                                                        if (Q.Cast(Qpred.CastPosition))
                                                            return;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (!W.IsCharging && W.IsReady() && CanUseW(target) && Orbwalker.ActiveMode == OrbwalkerMode.Combo)
                                                {
                                                    if (W.StartCharging())
                                                        return;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (Q.IsReady() && CanUseQ(target))
                                        {
                                            var Qtarget = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                                            if (target != null)
                                            {
                                                var Qpred = Q.GetPrediction(Qtarget);
                                                if (Qpred.Hitchance >= HitChance.High)
                                                {
                                                    if (Q.Cast(Qpred.CastPosition))
                                                        return;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (W.IsReady() && CanUseW(target) && Orbwalker.ActiveMode == OrbwalkerMode.Combo)
                                            {
                                                if (W.StartCharging())
                                                    return;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    var Qtarget = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                                    if (Qtarget != null)
                                    {
                                        if (Q.IsReady() && CanUseQ(Qtarget))
                                        {
                                            var Qpred = Q.GetPrediction(Qtarget);
                                            if (Qpred.Hitchance >= HitChance.High)
                                            {
                                                if (Q.Cast(Qpred.CastPosition))
                                                    return;
                                            }
                                        }
                                        else
                                        {
                                            if (W.IsReady() && CanUseW(target) && Orbwalker.ActiveMode == OrbwalkerMode.Combo)
                                            {
                                                if (W.StartCharging())
                                                    return;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (W.IsReady() && CanUseW(target) && Orbwalker.ActiveMode == OrbwalkerMode.Combo)
                                        {
                                            if (W.StartCharging())
                                                return;
                                        }
                                    }
                                }

                            }
                        }                        
                    }
                    else
                    {
                        {
                            if (!R.IsReady() && R.Level >= 1 && CanUseR(target) && ViegoMenu.RCombo.TakeSoul.Enabled)
                            {
                                var gettargetsoul = ObjectManager.Get<AIBaseClient>().Where(i => !i.IsAlly  && i.DistanceToPlayer() <= ObjectManager.Player.GetCurrentAutoAttackRange() + 200 && i.Health <= 0).OrderByDescending(i => i.MaxHealth);
                                if (gettargetsoul != null)
                                {
                                    foreach (var targetsoul in gettargetsoul)
                                    {
                                        ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, targetsoul);
                                    }
                                }
                            }

                            {
                                var Etarget = TargetSelector.GetTarget(800, DamageType.Physical);
                                if (Etarget != null)
                                {
                                    if (E.IsReady() && CanUseE(Etarget))
                                    {
                                        if (EcastPos(Etarget) != Vector3.Zero)
                                        {
                                            if (E.Cast(EcastPos(Etarget)))
                                                return;
                                        }
                                        else
                                        {
                                            if (Q.IsReady() && CanUseQ(target))
                                            {
                                                var Qtarget = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                                                if (target != null)
                                                {
                                                    var Qpred = Q.GetPrediction(Qtarget);
                                                    if (Qpred.Hitchance >= HitChance.High)
                                                    {
                                                        if (Q.Cast(Qpred.CastPosition))
                                                            return;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (W.IsReady() && CanUseW(target) && Orbwalker.ActiveMode == OrbwalkerMode.Combo)
                                                {
                                                    if (W.StartCharging())
                                                        return;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (Q.IsReady() && CanUseQ(target))
                                        {
                                            var Qtarget = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                                            if (target != null)
                                            {
                                                var Qpred = Q.GetPrediction(Qtarget);
                                                if (Qpred.Hitchance >= HitChance.High)
                                                {
                                                    if (Q.Cast(Qpred.CastPosition))
                                                        return;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (W.IsReady() && CanUseW(target) && Orbwalker.ActiveMode == OrbwalkerMode.Combo)
                                            {
                                                if (W.StartCharging())
                                                    return;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    var Qtarget = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                                    if (Qtarget != null)
                                    {
                                        if (Q.IsReady() && CanUseQ(Qtarget))
                                        {
                                            var Qpred = Q.GetPrediction(Qtarget);
                                            if (Qpred.Hitchance >= HitChance.High)
                                            {
                                                if (Q.Cast(Qpred.CastPosition))
                                                    return;
                                            }
                                        }
                                        else
                                        {
                                            if (W.IsReady() && CanUseW(target) && Orbwalker.ActiveMode == OrbwalkerMode.Combo)
                                            {
                                                if (W.StartCharging())
                                                    return;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (W.IsReady() && CanUseW(target) && Orbwalker.ActiveMode == OrbwalkerMode.Combo)
                                        {
                                            if (W.StartCharging())
                                                return;
                                        }
                                    }
                                }

                            }
                        }
                    }
                }                           
            }
            else
            {
                if (R.IsReady() && CanUseR(target, false))
                {
                    var Rpred = R.GetPrediction(target);
                    if(Rpred.Hitchance >= HitChance.High)
                    {
                        if (R.Cast(Rpred.CastPosition))
                            return;
                    }
                }
            }
        }
    }
}
