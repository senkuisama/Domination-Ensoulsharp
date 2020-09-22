using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;
using SPredictionMash;

namespace DominationAIO.Champions
{
    public class Samira
    {
        private static Menu SamiraMenu = new Menu("Samira Menu", "Simple Samira", true);
        private static MenuBool QCombo = new MenuBool("Q Combo Harass", "Q Combo Harass");
        private static MenuBool WCombo = new MenuBool("W Combo", "W Combo");
        private static MenuBool ECombo = new MenuBool("E Combo", "E Combo");
        private static MenuBool RCombo = new MenuBool("R Combo", "R Combo");

        private static AIHeroClient Player => ObjectManager.Player;
        private static Spell Q, W, E, R;

        public static void SamiraLoad()
        {
            Q = new Spell(SpellSlot.Q, 900f);
            Q.SetSkillshot(0.25f, 120f, 2600, true, EnsoulSharp.SDK.Prediction.SkillshotType.Line);
            W = new Spell(SpellSlot.W, 325f);
            E = new Spell(SpellSlot.E, 600f);
            R = new Spell(SpellSlot.R, 600f);

            var Helper = new Menu("Helper", "Helper");
            SPredictionMash.ConfigMenu.Initialize(Helper, "Helper");
            FunnySlayerCommon.MenuClass.AddTargetSelectorMenu(Helper);
            new SebbyLibPorted.Orbwalking.Orbwalker(Helper);
            SamiraMenu.Add(QCombo);
            SamiraMenu.Add(WCombo);
            SamiraMenu.Add(ECombo);
            SamiraMenu.Add(RCombo);
            SamiraMenu.Add(Helper);
            SamiraMenu.Attach();

            Game.OnUpdate += Game_OnUpdate;
            Orbwalker.OnAction += Orbwalker_OnAction;
            AIBaseClient.OnProcessSpellCast += AIBaseClient_OnProcessSpellCast;
        }

        private static void AIBaseClient_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (!sender.IsAlly && sender is AIHeroClient && sender.IsRanged && sender.IsValidTarget(W.Range + E.Range))
            {
                if(args.Target.IsMe || args.Target.NetworkId == Player.NetworkId)
                {
                    if(!BeforeAA && !OnAA && !Orbwalker.CanAttack())
                    {
                        if(W.IsReady() && WCombo.Enabled)
                        {
                            if (W.Cast())
                                return;
                        }
                    }
                }
            }
        }

        private static bool AfterAA = false;
        private static bool BeforeAA = false;
        private static bool OnAA = false;
        private static void Orbwalker_OnAction(object sender, OrbwalkerActionArgs args)
        {
            if(args.Type == OrbwalkerType.AfterAttack)
            {
                AfterAA = true;
                OnAA = false;
                BeforeAA = false;
            }
            else
            {
                AfterAA = false;
            }

            if (args.Type == OrbwalkerType.OnAttack)
            {
                AfterAA = false;
                OnAA = true;
                BeforeAA = false;
            }
            else
            {
                OnAA = false;
            }

            if (args.Type == OrbwalkerType.BeforeAttack)
            {
                AfterAA = false;
                OnAA = false;
                BeforeAA = true;
            }
            else
            {
                BeforeAA = false;
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Player.IsDead || OnAA || Player.IsDashing())
                return;

            if (Player.HasBuff("SamiraR"))
                Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            if (Player.HasBuff("SamiraW"))
                Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            var fstarget = FunnySlayerCommon.FSTargetSelector.GetFSTarget(Q.Range);

            if(Orbwalker.ActiveMode <= OrbwalkerMode.Harass)
            {
                if (Orbwalker.GetTarget() != null)
                {
                    if (ECombo.Enabled && E.IsReady())
                    {
                        if (!Orbwalker.CanAttack() || AfterAA)
                        {
                            if (Orbwalker.GetTarget().IsValidTarget(E.Range))
                            {
                                if (E.Cast(Orbwalker.GetTarget() as AIHeroClient) == CastStates.SuccessfullyCasted || E.CastOnUnit(Orbwalker.GetTarget() as AIHeroClient))
                                {
                                    if (Q.IsReady())
                                        if (Q.Cast(new SharpDX.Vector3(5154954f, 5641561f, 45115f), true))
                                            return;
                                    return;
                                }
                            }
                        }
                    }
                }

                if (QCombo.Enabled && Q.IsReady() && !Player.IsDashing())
                {
                    var targets = GameObjects.EnemyHeroes.Where(i => !i.IsDead && i.IsValidTarget(Q.Range / 3)).OrderBy(i => i.Health);
                    if(targets != null)
                    {
                        foreach(var target in targets)
                        {
                            if (AfterAA)
                            {
                                if (Q.Cast(target.Position))
                                    return;
                            }
                        }
                    }
                    if (fstarget != null)
                    {
                        var pred = SebbyLibPorted.Prediction.Prediction.GetPrediction(Q, fstarget);
                        if(pred.Hitchance >= SebbyLibPorted.Prediction.HitChance.High)
                        {
                            if(!Orbwalker.CanAttack() || !fstarget.InAutoAttackRange() || AfterAA)
                            {
                                if (Q.SPredictionCast(fstarget, EnsoulSharp.SDK.Prediction.HitChance.High))
                                    return;
                            }
                        }
                    }
                }
               
                if (R.IsReady() && RCombo.Enabled)
                {
                    var targets = GameObjects.EnemyHeroes.Where(i => !i.IsDead && i.IsValidTarget(R.Range)).OrderBy(i => i.Health);
                    if(targets != null)
                    {
                        foreach(var target in targets)
                        {
                            if(FSpred.Prediction.Prediction.PredictUnitPosition(target, 700).DistanceToPlayer() < R.Range)
                            {
                                if (E.IsReady())
                                {
                                    if(E.Cast(target) == CastStates.SuccessfullyCasted || E.Cast(target.Position) || E.CastOnUnit(target))
                                    {
                                        if (R.Cast(target) == CastStates.SuccessfullyCasted || R.Cast())
                                        {
                                            return;
                                        }
                                        return;
                                    }
                                }
                                else
                                {
                                    if (R.Cast(target) == CastStates.SuccessfullyCasted || R.Cast())
                                    {
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
}
