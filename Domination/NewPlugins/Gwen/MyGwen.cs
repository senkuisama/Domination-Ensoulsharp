﻿using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominationAIO.NewPlugins
{
    public static class MyGwen
    {
        private static class GwenMenu
        {
            public static class QMenu
            {
                public static MenuBool UseQCombo = new MenuBool("UseQCombo", "Use Q Combo");
                public static MenuBool MinStacks = new MenuBool("MinStacks", "Min Stacks");
                public static MenuSlider Stacks = new MenuSlider("Stacks", "Stacks", 2, 0, 4);
                public static MenuBool QResetAA = new MenuBool("QResetAA", "Reset AA timer");
            }
            public static class WMenu
            {
                public static MenuBool UseW = new MenuBool("UseW", "Use W");
                public static MenuBool AutoW = new MenuBool("Auto W", "Auto W");
                public static MenuBool WAfterDash = new MenuBool("WAfterDash", "W After E");
            }
            public static class EMenu
            {
                public static MenuBool UseE = new MenuBool("UseE", "Use E");
                public static MenuSlider ERange = new MenuSlider("ERange", "E Range", 500, 350, 700);
                public static MenuBool EFollowCursor = new MenuBool("FollowCursor", "Follow Mouse");
                public static MenuBool EResetAA = new MenuBool("EResetAA", "Reset AA timer");

            }
            public static class RMenu
            {
                public static MenuBool useR = new MenuBool("UseR", "Use R");
                public static MenuKeyBind AcceptAlwaysR = new MenuKeyBind("Accept Always R", "Always R", Keys.T, KeyBindType.Toggle);
                public static MenuSlider TargetHeath = new MenuSlider("TargetHeath", "Target Heath <= x% ", 70, 0, 101);
            }
        }
        private static Menu GMenu = new Menu("GMenu", "FunnySlayer Gwen", true);
        private static void LoadMenu()
        {
            var Qs = new Menu("Qs", "Q Settings");
            var Ws = new Menu("Ws", "W Settings");
            var Es = new Menu("Es", "E Settings");
            var Rs = new Menu("Rs", "R Settings");

            Qs.Add(GwenMenu.QMenu.UseQCombo);
            Qs.Add(GwenMenu.QMenu.MinStacks);
            Qs.Add(GwenMenu.QMenu.Stacks).Permashow();
            Qs.Add(GwenMenu.QMenu.QResetAA);

            Ws.Add(GwenMenu.WMenu.UseW);
            Ws.Add(GwenMenu.WMenu.WAfterDash);
            Ws.Add(GwenMenu.WMenu.AutoW);

            Es.Add(GwenMenu.EMenu.UseE);
            Es.Add(GwenMenu.EMenu.ERange);
            Es.Add(GwenMenu.EMenu.EFollowCursor);
            Es.Add(GwenMenu.EMenu.EResetAA);

            Rs.Add(GwenMenu.RMenu.useR);
            Rs.Add(GwenMenu.RMenu.TargetHeath);
            Rs.Add(GwenMenu.RMenu.AcceptAlwaysR).Permashow();

            GMenu.Add(Qs);
            GMenu.Add(Ws);
            GMenu.Add(Es);
            GMenu.Add(Rs);

            GMenu.Attach();

        }
        private static int QBuffCount()
        {
            var buffs = ObjectManager.Player.Buffs;

            if (!ObjectManager.Player.HasBuff("GwenQ") && buffs.Where(i => i.Name == "GwenQ").Count() == 0)
                return 0;

            var count = 0;
            for (var i = 0; i < buffs.Count(); i +=1)
            {
                if(buffs[i].Name == "GwenQ")
                {
                    count = buffs[i].Count;
                }
            }
            return count;
        }


        private static Spell Q, W, E, R;
        public static void GwenLoad()
        {
            Q = new Spell(SpellSlot.Q, 475f);
            Q.SetSkillshot(0.25f, 55f, float.MaxValue, false, SpellType.Line);
            W = new Spell(SpellSlot.W);
            E = new Spell(SpellSlot.E, 350f);
            R = new Spell(SpellSlot.R, 1000f);
            R.SetSkillshot(0.25f, 35f, 1500f, false, SpellType.Line);
            Game.OnUpdate += Game_OnUpdate;
            AIBaseClient.OnProcessSpellCast += AIBaseClient_OnProcessSpellCast;
            AIBaseClient.OnDoCast += AIBaseClient_OnDoCast;
        }

        private static void AIBaseClient_OnDoCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name == "GwenW")
                {
                    LastW = Variables.GameTimeTickCount;
                }
            }
            else
            {
                if (sender.IsEnemy && GwenMenu.WMenu.AutoW.Enabled && !args.SData.Name.IsAutoAttack())
                {
                    if (args.Target != null)
                    {
                        if ((args.Target.IsMe || args.To.DistanceToPlayer() <= 200 || args.Start.DistanceToPlayer() + args.To.DistanceToPlayer() == args.Start.Distance(args.To)))
                        {
                            W.Cast();
                            return;
                        }

                    }
                    else
                    {
                        if ((args.To.DistanceToPlayer() <= 200 || args.Start.DistanceToPlayer() + args.To.DistanceToPlayer() == args.Start.Distance(args.To)))
                        {
                            W.Cast();
                            return;

                        }
                    }
                }
            }
        }

        private static int LastW = 0;
        private static void AIBaseClient_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if(args.SData.Name == "GwenW")
                {
                    LastW = Variables.GameTimeTickCount;
                }
                if (GwenMenu.QMenu.QResetAA.Enabled)
                {
                    if (args.Slot == SpellSlot.Q)
                    {
                        Orbwalker.ResetAutoAttackTimer();
                    }
                }
                if (GwenMenu.EMenu.EResetAA.Enabled)
                {
                    if (args.Slot == SpellSlot.Q)
                    {
                        Orbwalker.ResetAutoAttackTimer();
                    }
                }                
            }
            else
            {
                if (sender.IsEnemy && GwenMenu.WMenu.AutoW.Enabled && !args.SData.Name.IsAutoAttack())
                {
                    if (args.Target != null)
                    {
                        if ((args.Target.IsMe || args.To.DistanceToPlayer() <= 200 || args.Start.DistanceToPlayer() + args.To.DistanceToPlayer() == args.Start.Distance(args.To)))
                        {
                            W.Cast();
                            return;
                        }

                    }
                    else
                    {
                        if ((args.To.DistanceToPlayer() <= 200 || args.Start.DistanceToPlayer() + args.To.DistanceToPlayer() == args.Start.Distance(args.To)))
                        {
                            W.Cast();
                            return;

                        }
                    }
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
                return;

            if(Orbwalker.ActiveMode == OrbwalkerMode.Combo)
            {
                GwenCombo();
            }
        }

        private static void GwenCombo()
        {
            if (FunnySlayerCommon.OnAction.BeforeAA || FunnySlayerCommon.OnAction.OnAA)
                return;
            {
                var target = TargetSelector.GetTarget(GwenMenu.EMenu.ERange.Value);
                if(target != null)
                {
                    if (E.IsReady() && GwenMenu.EMenu.UseE.Enabled)
                    {
                        if (GwenMenu.EMenu.EFollowCursor.Enabled)
                        {
                            var posafterE = ObjectManager.Player.Position.Extend(Game.CursorPos, 350);

                            if(posafterE.Distance(target.Position) <= Math.Abs(GwenMenu.EMenu.ERange.Value - 350) + 100 + ObjectManager.Player.GetCurrentAutoAttackRange())
                            {
                                if (FunnySlayerCommon.OnAction.BeforeAA || FunnySlayerCommon.OnAction.OnAA)
                                    return;

                                E.Cast(Game.CursorPos);
                                if(GwenMenu.WMenu.UseW.Enabled && GwenMenu.WMenu.WAfterDash.Enabled && (W.Name == "GwenW" || Variables.GameTimeTickCount - LastW >= 4000))
                                {
                                    W.Cast();
                                    return;
                                }
                                return;
                            }
                        }
                        else
                        {
                            if (FunnySlayerCommon.OnAction.BeforeAA || FunnySlayerCommon.OnAction.OnAA)
                                return;

                            E.Cast(target.Position);
                            if (GwenMenu.WMenu.UseW.Enabled && GwenMenu.WMenu.WAfterDash.Enabled && (W.Name == "GwenW" || Variables.GameTimeTickCount - LastW >= 4000))
                            {
                                W.Cast();
                                return;
                            }
                            return;
                        }
                    }
                }
            }

            {
                var target = TargetSelector.GetTarget(1000);
                if (target != null)
                {
                    var pred = FSpred.Prediction.Prediction.GetPrediction(R, target);
                    if (pred.Hitchance >= FSpred.Prediction.HitChance.High && R.IsReady())
                    {
                        var Qtarget = TargetSelector.GetTarget(475);
                        var orbtarget = Orbwalker.GetTarget();
                        
                        if (GwenMenu.RMenu.useR.Enabled)
                        {
                            if (R.Name != "GwenR")
                            {
                                if(Qtarget != null && orbtarget != null)
                                {
                                    if (Q.IsReady())
                                    {
                                        var Qpred = FSpred.Prediction.Prediction.GetPrediction(Q, target);
                                        if (Qpred.Hitchance >= FSpred.Prediction.HitChance.High)
                                        {
                                            if (QBuffCount() >= 4)
                                            {
                                                Q.Cast(Qpred.CastPosition);
                                                return;
                                            }
                                            else
                                            {
                                                if (orbtarget != null)
                                                {
                                                    var isaiheroclient = orbtarget.Type == GameObjectType.AIHeroClient;
                                                    if (GwenMenu.WMenu.UseW.Enabled && GwenMenu.WMenu.AutoW.Enabled && (W.Name == "GwenW" || Variables.GameTimeTickCount - LastW >= 4000))
                                                    {
                                                        W.Cast();
                                                        return;
                                                    }
                                                    return;
                                                }
                                                else
                                                {
                                                    if (target.DistanceToPlayer() >= ObjectManager.Player.GetCurrentAutoAttackRange() + 100)
                                                    {
                                                        Q.Cast(Qpred.CastPosition);
                                                        return;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (GwenMenu.RMenu.AcceptAlwaysR.Active || target.HealthPercent <= GwenMenu.RMenu.TargetHeath.Value)
                                            {
                                                if (FunnySlayerCommon.OnAction.BeforeAA || FunnySlayerCommon.OnAction.OnAA)
                                                    return;

                                                R.Cast(pred.CastPosition);
                                                return;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (GwenMenu.RMenu.AcceptAlwaysR.Active || target.HealthPercent <= GwenMenu.RMenu.TargetHeath.Value)
                                        {
                                            if (FunnySlayerCommon.OnAction.BeforeAA || FunnySlayerCommon.OnAction.OnAA)
                                                return;

                                            R.Cast(pred.CastPosition);
                                            return;
                                        }
                                    }
                                }
                                else
                                {
                                    if (GwenMenu.RMenu.AcceptAlwaysR.Active || target.HealthPercent <= GwenMenu.RMenu.TargetHeath.Value)
                                    {
                                        if (FunnySlayerCommon.OnAction.BeforeAA || FunnySlayerCommon.OnAction.OnAA)
                                            return;

                                        R.Cast(pred.CastPosition);
                                        return;
                                    }
                                }
                            }
                            else
                            {

                                if (GwenMenu.RMenu.AcceptAlwaysR.Active || target.HealthPercent <= GwenMenu.RMenu.TargetHeath.Value)
                                {
                                    if (FunnySlayerCommon.OnAction.BeforeAA || FunnySlayerCommon.OnAction.OnAA)
                                        return;

                                    R.Cast(pred.CastPosition);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            {
                var target = TargetSelector.GetTarget(475);
                if(target != null)
                {
                    if(W.IsReady() && GwenMenu.WMenu.UseW.Enabled && GwenMenu.WMenu.AutoW.Enabled)
                    {
                        if(Variables.GameTimeTickCount - LastW >= 4000)
                        {
                            W.Cast();
                            return;
                        }
                    }
                }
            }
            {
                var target = TargetSelector.GetTarget(475);
                var orbtarget = Orbwalker.GetTarget();


                if(target != null)
                {
                    var pred = FSpred.Prediction.Prediction.GetPrediction(Q, target);
                    if(pred.Hitchance >= FSpred.Prediction.HitChance.High)
                    {
                        if(QBuffCount() >= 4)
                        {
                            Q.Cast(pred.CastPosition);
                            return;
                        }
                        else
                        {
                            if(orbtarget != null)
                            {
                                var isaiheroclient = orbtarget.Type == GameObjectType.AIHeroClient;
                                if (GwenMenu.WMenu.UseW.Enabled && GwenMenu.WMenu.AutoW.Enabled && (W.Name == "GwenW" || Variables.GameTimeTickCount - LastW >= 4000))
                                {
                                    W.Cast();
                                    return;
                                }
                                return;
                            }
                            else
                            {
                                if (target.DistanceToPlayer() >= ObjectManager.Player.GetCurrentAutoAttackRange() + 150 && QBuffCount() >= (GwenMenu.QMenu.MinStacks.Enabled ? GwenMenu.QMenu.Stacks.Value : 0))
                                {
                                    Q.Cast(pred.CastPosition);
                                    return;
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}