using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;

namespace DominationAIO.Champions
{
    public static class Qiyana
    {
        private static class QSettings
        {
            public static MenuBool QCH = new MenuBool("Q Combo / Harass", "Q Combo / Harass");
            public static MenuBool QClear = new MenuBool("Q Clear", "Q Clear");
            public static MenuSlider QMana = new MenuSlider("QMana", "Mana Check ", 60);
        }
        private static class WSettings
        {
            public static MenuBool WCH = new MenuBool("W Combo / Harass", "W Combo / Harass");
            public static MenuSlider WWater = new MenuSlider("WWater", "Water", 3, 1, 3);
            public static MenuSlider WRock = new MenuSlider("WRock", "Rock", 2, 1, 3);
            public static MenuSlider WGrass = new MenuSlider("WGrass", "Grass", 1, 1, 3);
        }
        private static class ESettings
        {
            public static MenuBool ECombo = new MenuBool("E Combo", "E Combo");
            public static MenuBool EHarass = new MenuBool("E Harass", "E Harass", false);
        }
        private static class RSettings
        {
            public static MenuBool RCombo = new MenuBool("R Combo", "R Combo");
            public static MenuSlider RHeath = new MenuSlider("R Heath", "When Target Heath % <= ", 80, 0, 100);
            public static MenuBool RWall = new MenuBool("RWall", "R Wall");
            public static MenuBool RWater = new MenuBool("RWater", "R Water");
            public static MenuBool RGrass = new MenuBool("R Grass", "R Grass", false);
            public static MenuSlider RBack = new MenuSlider("R Back", "R Knoc kback", 350, 0, 450);
        }

        private static AIHeroClient Player = ObjectManager.Player;
        private static Spell Q1, Q2;
        private static Spell W, E, R;
        private static Menu QiyanaMenu = new Menu("QiyanaMenu", "FunnySlayer Qiyana", true);

        public static void Load()
        {
            if (Player == null)
                return;

            Q1 = new Spell(SpellSlot.Q, 460f);
            Q1.SetSkillshot(0.25f, 55, 3000f, false, EnsoulSharp.SDK.Prediction.SkillshotType.Line);
            Q2 = new Spell(SpellSlot.Q, 710f);
            Q2.SetSkillshot(0.25f, 60f, 2000f, false, EnsoulSharp.SDK.Prediction.SkillshotType.Line);
            W = new Spell(SpellSlot.W, 1100f);
            E = new Spell(SpellSlot.E, 650f);
            E.SetTargetted(0.25f, 2000f);
            R = new Spell(SpellSlot.R, 850f);
            R.SetSkillshot(0.7f, 200f, 2000f, false, EnsoulSharp.SDK.Prediction.SkillshotType.Line);
            var QMenu = new Menu("Q Qiyana", "Q Settings");
            QMenu.Add(QSettings.QCH);
            QMenu.Add(QSettings.QClear);
            QMenu.Add(QSettings.QMana);
            var WMenu = new Menu("W Qiyana", "W Settings");
            WMenu.Add(WSettings.WCH);
            WMenu.Add(WSettings.WWater);
            WMenu.Add(WSettings.WRock);
            WMenu.Add(WSettings.WGrass);
            var EMenu = new Menu("E Qiyana", "E Settings");
            EMenu.Add(ESettings.ECombo);
            EMenu.Add(ESettings.EHarass);
            var RMenu = new Menu("R Qiyana", "R Settings");
            RMenu.Add(RSettings.RCombo);
            RMenu.Add(RSettings.RHeath);
            RMenu.Add(RSettings.RBack);
            RMenu.Add(RSettings.RWall);
            RMenu.Add(RSettings.RWater);
            RMenu.Add(RSettings.RGrass);


            QiyanaMenu.Add(QMenu);
            QiyanaMenu.Add(WMenu);
            QiyanaMenu.Add(EMenu);
            QiyanaMenu.Add(RMenu);
            QiyanaMenu.Attach();

            Game.OnUpdate += Game_OnUpdate;
            AIBaseClient.OnProcessSpellCast += AIBaseClient_OnProcessSpellCast;
            Game.OnUpdate += Game_OnUpdate1;
            //Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Game_OnUpdate1(EventArgs args)
        {
            if (Player.IsDead)
                return;

            var target = TargetSelector.GetTarget(R.Range);
            if (target == null)
                return;
            if (target.HealthPercent > RSettings.RHeath.Value)
                return;

            if (Orbwalker.ActiveMode > OrbwalkerMode.Combo)
                return;

            if(R.IsReady() && RSettings.RCombo.Enabled)
            {
                var pred = SebbyLibPorted.Prediction.Prediction.GetPrediction(R, target);
                if(pred.Hitchance >= SebbyLibPorted.Prediction.HitChance.High)
                {
                    if(pred.CastPosition.DistanceToPlayer() < R.Range - RSettings.RBack.Value)
                    {
                        var BackPos = pred.CastPosition.Extend(Player.Position, -RSettings.RBack.Value);
                        var FlagGet = NavMesh.GetCollisionFlags(BackPos);
                        if (FlagGet.HasFlag(CollisionFlags.Building) || FlagGet.HasFlag(CollisionFlags.Grass) || FlagGet.HasFlag(CollisionFlags.Wall))
                        {
                            if (R.Cast(pred.CastPosition))
                                return;
                        }
                        if (NavMesh.IsWater((uint)BackPos.X, (uint)BackPos.Y))
                        {
                            if (R.Cast(pred.CastPosition))
                                return;
                        }
                    }
                    else
                    {
                        var BackPos = Player.Position.Extend(pred.CastPosition, R.Range);
                        var FlagGet = NavMesh.GetCollisionFlags(BackPos);
                        if (FlagGet.HasFlag(CollisionFlags.Building) || FlagGet.HasFlag(CollisionFlags.Grass) || FlagGet.HasFlag(CollisionFlags.Wall))
                        {
                            if (R.Cast(pred.CastPosition))
                                return;
                        }

                        if (NavMesh.IsWater((uint)BackPos.X, (uint)BackPos.Y))
                        {
                            if (R.Cast(pred.CastPosition))
                                return;
                        }
                    }
                }
            }
        }

        private static int LastE = 0;
        private static void AIBaseClient_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (sender == null)
                return;

            if (sender.IsMe)
            {
                if(args.Slot == SpellSlot.W)
                {
                    LastE = Variables.TickCount;
                }
            }
        }
        private static Geometry.Circle AllPos = null;
        private static void Game_OnUpdate(EventArgs args)
        {
            if (Player.IsDead) 
                return;

            var target = TargetSelector.GetTarget(R.Range + E.Range);
            if (target == null)
                return;

            if(Orbwalker.ActiveMode <= OrbwalkerMode.Harass)
            {
                if(Orbwalker.ActiveMode == OrbwalkerMode.Harass)
                {
                    if(E.IsReady() && ESettings.EHarass.Enabled)
                    {
                        if (target.IsValidTarget(E.Range))
                        {
                            if (E.Cast(target) == CastStates.SuccessfullyCasted)
                                return;
                        }
                    }
                }
                else
                {
                    if (E.IsReady() && ESettings.ECombo.Enabled)
                    {
                        if (target.IsValidTarget(E.Range))
                        {
                            if (E.Cast(target) == CastStates.SuccessfullyCasted)
                                return;
                        }
                    }
                }

                if(W.IsReady() && WSettings.WCH.Enabled)
                {
                    if(!QEmp() && Variables.TickCount > LastE + 500 && !Player.IsDashing())
                    {
                        AllPos = new Geometry.Circle(Player.Position, 1000);

                        var canwater = false;
                        var canrock = false;
                        var cangrass = false;
                        if (AllPos != null)
                        {
                            if (AllPos.Points.Any(i => i != target.Position.ToVector2() && NavMesh.IsWater((uint)i.X, (uint)i.Y)))
                                canwater = true;
                            else
                                canwater = false;

                            if (AllPos.Points.Any(i => i != target.Position.ToVector2() && (i.IsWall() || i.IsBuilding())))
                                canrock = true;
                            else
                                canrock = false;

                            if (AllPos.Points.Any(i => i != target.Position.ToVector2() && ObjectManager.Get<GrassObject>() != null && ObjectManager.Get<GrassObject>().Any(a => a.Distance(i) < 375)))
                                cangrass = true;
                            else
                                cangrass = false;

                            if (WSettings.WWater.Value >= WSettings.WRock.Value && WSettings.WWater.Value >= WSettings.WGrass.Value && canwater)
                            {
                                if (W.Cast(AllPos.Points.OrderBy(i => i.Distance(target)).FirstOrDefault(i => i != target.Position.ToVector2() && NavMesh.IsWater((uint)i.X, (uint)i.Y))))
                                    return;
                            }
                            else
                            if (WSettings.WRock.Value >= WSettings.WRock.Value && WSettings.WRock.Value >= WSettings.WGrass.Value && canrock)
                            {
                                if (W.Cast(AllPos.Points.OrderBy(i => i.Distance(target)).FirstOrDefault(i => i != target.Position.ToVector2() && (i.IsWall() || i.IsBuilding()))))
                                    return;
                            }
                            else
                            if (WSettings.WGrass.Value >= WSettings.WGrass.Value && WSettings.WGrass.Value >= WSettings.WRock.Value && cangrass)
                            {
                                if (W.Cast(AllPos.Points.OrderBy(i => i.Distance(target)).FirstOrDefault(i => i != target.Position.ToVector2() && ObjectManager.Get<GrassObject>() != null && ObjectManager.Get<GrassObject>().Any(a => a.Distance(i) < 375))))
                                    return;
                            }
                        }
                        else
                        {
                            canwater = false;
                            canrock = false;
                            cangrass = false;
                        }                      
                    }
                }

                if (Q1.IsReady() && QSettings.QCH.Enabled)
                {
                    if (QEmp())
                    {
                        var pred = SebbyLibPorted.Prediction.Prediction.GetPrediction(Q2, target);
                        if(pred.Hitchance >= SebbyLibPorted.Prediction.HitChance.High)
                        {
                            if (Q2.Cast(pred.CastPosition))
                                return;
                        }
                    }
                    else
                    {
                        var pred = SebbyLibPorted.Prediction.Prediction.GetPrediction(Q1, target);
                        if (pred.Hitchance >= SebbyLibPorted.Prediction.HitChance.High)
                        {
                            if (Q1.Cast(pred.CastPosition))
                                return;
                        }
                    }
                }
            }
        }

        private static bool QEmp()
        {
            return Q1.Name != "QiyanaQ";
        }
    }
}
