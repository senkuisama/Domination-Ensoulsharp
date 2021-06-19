using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;

using FSpred.Prediction;

namespace DominationAIO.Champions
{
    internal static class Brand
    {
        internal static AIHeroClient ME = GameObjects.Player;
        internal static Menu BrandMenu;
        internal static Spell Q, W, E, R;
        internal static Spell EBurn, RBurn;
        public static FSpred.Prediction.PredictionInput QPred;
        public static FSpred.Prediction.PredictionInput WPred;
        internal static void BrandLoad()
        {
            if (ME == null)
                return;

            BrandMenu = new Menu("FS_Brand", "Brand Simple", true);
            Menu Blaze = new Menu("Blaze Settings", "Blaze Settings");
            Blaze.Add(BrandSettings.LogicBlaze);
            Blaze.Add(BrandSettings.AcceptQ);
            Blaze.Add(BrandSettings.AcceptW);
            Blaze.Add(BrandSettings.AcceptE);
            Blaze.Add(BrandSettings.AcceptR);
            Menu Qmenu = new Menu("Q Settings", "Q Settings");
            Qmenu.Add(BrandSettings.Qcombo);
            Qmenu.Add(BrandSettings.OnlyStun);
            Menu Wmenu = new Menu("W Settings", "W Settings");
            Wmenu.Add(BrandSettings.Wcombo);
            Menu Emenu = new Menu("E Settings", "E Settings");
            Emenu.Add(BrandSettings.Ecombo);
            Menu Rmenu = new Menu("R Settings", "R Settings");
            Rmenu.Add(BrandSettings.Rcombo);
            Rmenu.Add(BrandSettings.Hit);
            Rmenu.Add(BrandSettings.Heath);

            Menu BrandAuto = new Menu("Auto Brand", "Auto Settings");
            BrandAuto.Add(BrandSettings.EarlyCombo);
            BrandAuto.Add(BrandSettings.QStun);
            BrandAuto.Add(BrandSettings.Whit);
            BrandAuto.Add(BrandSettings.AutoR);

            BrandMenu.Add(Blaze);
            BrandMenu.Add(Qmenu);
            BrandMenu.Add(Wmenu);
            BrandMenu.Add(Emenu);
            BrandMenu.Add(Rmenu);
            BrandMenu.Add(BrandAuto);
            BrandMenu.Attach();

            Q = new Spell(SpellSlot.Q, 1000f);
            W = new Spell(SpellSlot.W, 900f);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R);

            Q.SetSkillshot(0.25f, 55f, 1600f, true, SpellType.Line);
            //Q.Range = 1000f; Q.Delay = 0.25f; Q.Width = 55f; Q.Speed = 1600f; Q.Collision = true; Q.Type = SpellType.Line;
            //Q
            W.SetSkillshot(0.75f, 200f, float.MaxValue, false, SpellType.Circle);
            //  W.Range = 900f; W.Delay = 0.75f; W.Width = 200f; W.Speed = float.MaxValue; W.Collision = false; W.Type = SpellType.Circle;
            //W
            E.Range = 675f; E.SetTargetted(0.25f, float.MaxValue);
            //E
            R.Range = 750f; R.SetTargetted(0.25f, 1000f);
            //R

            QPred = new FSpred.Prediction.PredictionInput
            {
                Aoe = false,
                CollisionYasuoWall = true,
                Collision = true,
                CollisionObjects = new FSpred.Prediction.CollisionableObjects[] { FSpred.Prediction.CollisionableObjects.Minions, FSpred.Prediction.CollisionableObjects.YasuoWall },
                Delay = 0.25f,
                Radius = 55f,
                Range = 1000f,
                Speed = 1600f,
                Type = FSpred.Prediction.SkillshotType.SkillshotLine,
                From = ME.Position,
                RangeCheckFrom = ME.Position
            };

            WPred = new FSpred.Prediction.PredictionInput
            {
                Aoe = true,
                CollisionYasuoWall = false,
                Collision = false,
                CollisionObjects = null,
                Delay = 0.75f,
                Radius = 200f,
                Range = 900f,
                Speed = float.MaxValue,
                Type = FSpred.Prediction.SkillshotType.SkillshotCircle,
                From = ME.Position,
                RangeCheckFrom = ME.Position
            };
            Game.OnUpdate += Game_OnUpdate;

            Game.OnUpdate += AUTOQSTUN;
            Game.OnUpdate += AUTOWHIT;
            Game.OnUpdate += AUTOEHIT;
            Game.OnUpdate += AUTORHIT;

            Game.OnUpdate += COMBOHARASS;

            Drawing.OnDraw += BRANDDRAWING;

            //Orbwalker.OnAction += Orbwalker_OnAction;
        }

        /*private static bool AfterAA = false;
        private static void Orbwalker_OnAction(object sender, OrbwalkerActionArgs args)
        {
            if(args.Type == OrbwalkerType.AfterAttack)
            {
                AfterAA = true;
            }
            else
            {
                AfterAA = false;
            }
        }*/

        private static void Game_OnUpdate(EventArgs args)
        {
            if(TargetSelector.GetTarget(1100f, DamageType.Magical) == null)
            {
                //if(WPoly != null)
                    WPoly = null;
                //if (EPoly != null)
                    EPoly = null;
                //if (RPoly != null)
                    RPoly = null;
            }
            //Poly           
        }

        private static void BRANDDRAWING(EventArgs args)
        {
            if (ME.IsDead)
                return;

            if(WPoly != null)
            {
                WPoly.Draw(System.Drawing.Color.Red);
            }

            if(EPoly != null)
            {
                EPoly.Draw(System.Drawing.Color.Yellow);
            }

            if(RPoly != null)
            {
                RPoly.Draw(System.Drawing.Color.Green);
            }
        }

        public static bool Burning(this AIBaseClient target)
        {
            return target.HasBuff("brandablaze");
        }

        private static void COMBOHARASS(EventArgs args)
        {
            if (ME.IsDead)
                return;

            if (Orbwalker.ActiveMode != OrbwalkerMode.Harass && Orbwalker.ActiveMode != OrbwalkerMode.Combo)
                return;

            if (E.IsReady() && BrandSettings.Ecombo.Enabled)
            {
                DO_BRAND_E();
            }
            if (R.IsReady() && BrandSettings.Rcombo.Enabled)
            {
                DO_BRAND_R();
            }
            if (W.IsReady() && BrandSettings.Wcombo.Enabled)
            {
                DO_BRAND_W();
            }
            if (Q.IsReady() && BrandSettings.Qcombo.Enabled)
            {
                DO_BRAND_Q();
            }         
        }

        private static void DO_BRAND_Q()
        {
            if (!Q.IsReady())
                return;

            var target = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
            if (target == null)
                return;
            
            if (target.Burning())
            {
                if (BrandSettings.AcceptQ.Enabled)
                {
                    var pred = SebbyLibPorted.Prediction.Prediction.GetPrediction(Q, target);
                    if(pred != null && pred.Hitchance >= SebbyLibPorted.Prediction.HitChance.High)
                    {
                        if (Q.Cast(pred.CastPosition))
                        {
                            if(E.IsReady() && target.IsValidTarget(E.Range))
                            {
                                if (E.Cast(target) == CastStates.SuccessfullyCasted || E.CastOnUnit(target))
                                    return;
                            }
                        }
                    }
                }
            }
            else
            {
                if (!BrandSettings.OnlyStun.Enabled)
                {
                    var pred = FSpred.Prediction.Prediction.GetPrediction(Q, target);
                    if (pred != null && pred.Hitchance >= FSpred.Prediction.HitChance.High)
                    {
                        if (Q.Cast(pred.CastPosition))
                        {
                            if (E.IsReady() && target.IsValidTarget(E.Range))
                            {
                                if (E.Cast(target) == CastStates.SuccessfullyCasted || E.CastOnUnit(target))
                                    return;
                            }
                        }
                    }
                }
            }
        }
        private static void DO_BRAND_W()
        {
            if (!W.IsReady())
                return;

            var target = TargetSelector.GetTarget(W.Range, DamageType.Magical);
            if (target == null)
                return;

            if((ObjectManager.Player.Level < 3 || Q.State == SpellState.NotLearned) && BrandSettings.EarlyCombo.Enabled)
            {
                if (FunnySlayerCommon.OnAction.AfterAA)
                {
                    var pred = W.GetPrediction(target);
                    if(pred.Hitchance >= EnsoulSharp.SDK.HitChance.High)
                    {
                        if (W.Cast(pred.CastPosition))
                            return;
                    }
                }
            }
            else
            {
                if (target.Burning())
                {
                    if (BrandSettings.AcceptW.Enabled)
                    {
                        var pred = W.GetPrediction(target);
                        if (pred.Hitchance >= EnsoulSharp.SDK.HitChance.High)
                        {
                            if (W.Cast(pred.CastPosition))
                                return;
                        }
                    }
                }
                else
                {
                    var pred = W.GetPrediction(target);
                    if (pred.Hitchance >= EnsoulSharp.SDK.HitChance.High)
                    {
                        if (W.Cast(pred.CastPosition))
                            return;
                    }
                }
            }           
        }
        private static void DO_BRAND_E()
        {
            if (!E.IsReady())
                return;

            var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);
            if (target == null)
                return;

            if (target.Burning())
            {
                if (BrandSettings.AcceptE.Enabled)
                {
                    if (E.Cast(target) == CastStates.SuccessfullyCasted || E.CastOnUnit(target))
                        return;
                }
            }
            else
            {
                if (E.Cast(target) == CastStates.SuccessfullyCasted || E.CastOnUnit(target))
                    return;
            }
        }
        private static void DO_BRAND_R()
        {
            if (!R.IsReady())
                return;

            var target = TargetSelector.GetTarget(R.Range, DamageType.Magical);
            if (target == null)
                return;

            if (target.Burning())
            {
                if (BrandSettings.AcceptR.Enabled)
                {
                    if(target.HealthPercent <= BrandSettings.Heath.Value)
                    {
                        if (R.Cast(target) == CastStates.SuccessfullyCasted || R.CastOnUnit(target))
                            return;
                    }
                    if(target.CountEnemyHeroesInRange(600) >= BrandSettings.Hit.Value)
                    {
                        if (R.Cast(target) == CastStates.SuccessfullyCasted || R.CastOnUnit(target))
                            return;
                    }
                }
            }
            else
            {
                if (target.HealthPercent <= BrandSettings.Heath.Value)
                {
                    if (R.Cast(target) == CastStates.SuccessfullyCasted || R.CastOnUnit(target))
                        return;
                }
                if (target.CountEnemyHeroesInRange(600) >= BrandSettings.Hit.Value)
                {
                    if (R.Cast(target) == CastStates.SuccessfullyCasted || R.CastOnUnit(target))
                        return;
                }
            }
        }

        public static Geometry.Circle RPoly;
        private static void AUTORHIT(EventArgs args)
        {
            if (ME.IsDead || !BrandSettings.AutoR.Enabled)
                return;

            if (!R.IsReady())
                return;

            var targets = TargetSelector.GetTargets(R.Range + 600, DamageType.Magical).OrderBy(i => i.DistanceToPlayer());
            if (targets == null)
                return;

            foreach (var target in targets)
            {
                if (target == null)
                    return;

                RPoly = new Geometry.Circle(target.Position, 600f);
                var targethit = GameObjects.EnemyHeroes.Where(i => !i.IsDead && RPoly.IsInside(i));
                var targethitcount = targethit.Count();

                if (targethitcount >= BrandSettings.AutoR.ActiveValue && target.IsValidTarget(R.Range))
                {
                    if (R.Cast(target) == CastStates.SuccessfullyCasted || R.CastOnUnit(target))
                        return;
                }
            }
        }
      
        public static Geometry.Circle EPoly;
        private static void AUTOEHIT(EventArgs args)
        {
            if (ME.IsDead)
                return;

            if (!E.IsReady())
                return;

            var targets = TargetSelector.GetTargets(E.Range + 375f, DamageType.Magical).OrderBy(i => i.DistanceToPlayer());

            if (targets == null)
                return;

            foreach(var target in targets)
            {
                if (target == null)
                    return;

                EPoly = new Geometry.Circle(target.Position, 375f);
                var targethit = GameObjects.EnemyHeroes.Where(i => EPoly.IsInside(i.Position));
                var targethitcount = targethit.Count();

                if(targethitcount >= 2 && target.IsValidTarget(E.Range) && target.Burning())
                {
                    if(E.Cast(target) == CastStates.SuccessfullyCasted || E.CastOnUnit(target))
                    {
                        return;
                    }
                }
            }
        }

        public static Geometry.Circle WPoly;
        private static void AUTOWHIT(EventArgs args)
        {
            if (ME.IsDead || !BrandSettings.Whit.Enabled)
                return;

            if (!W.IsReady())
                return;

            var targets = TargetSelector.GetTargets(W.Range, DamageType.Magical).OrderBy(i => i.DistanceToPlayer());
            if (targets == null)
                return;

            foreach(var target in targets)
            {
                if (target == null)
                    return;

                var pred = FSpred.Prediction.Prediction.GetPrediction(W, target);
                if(pred != null && pred.Hitchance >= FSpred.Prediction.HitChance.High)
                {
                    WPoly = new Geometry.Circle(pred.CastPosition, 260f);
                    var targethit = GameObjects.EnemyHeroes.Where(i => WPoly.IsInside(i.Position));
                    var targethitcount = targethit.Count();

                    if(targethitcount >= BrandSettings.Whit.ActiveValue && pred.CastPosition.DistanceToPlayer() <= W.Range)
                    {
                        if (W.Cast(pred.CastPosition))
                            return;
                    }
                }
            }
        }

        private static void AUTOQSTUN(EventArgs args)
        {
            if (ME.IsDead || !BrandSettings.QStun.Enabled)
                return;

            if (!Q.IsReady())
                return;

            var targets = TargetSelector.GetTargets(Q.Range, DamageType.Magical).Where(i => i.Burning()).OrderBy(i => i.Health);
            if (targets == null)
                return;

            var target = targets.FirstOrDefault(i => FSpred.Prediction.Prediction.GetPrediction(Q, i).Hitchance >= FSpred.Prediction.HitChance.High);
            if (target == null)
                return;

            var pred = Q.GetPrediction(target);
            if(pred != null && pred.Hitchance >= EnsoulSharp.SDK.HitChance.High)
            {
                if (Q.Cast(pred.CastPosition))
                    return;
            }
        }
    }

    internal class BrandSettings
    {
        //Blaze Settings
        internal static MenuBool LogicBlaze = new MenuBool("Logic Blaze", "Logic Blaze");
        internal static MenuBool AcceptQ = new MenuBool("Accept Q", "Accept Q");
        internal static MenuBool AcceptW = new MenuBool("Accept W", "Accept W");
        internal static MenuBool AcceptE = new MenuBool("Accept E", "Accept E");
        internal static MenuBool AcceptR = new MenuBool("Accept R", "Accept R");
        //Q Settings
        internal static MenuBool Qcombo = new MenuBool("Q combo", "Q Combo");
        internal static MenuBool OnlyStun = new MenuBool("Stun Only", "Only when target is FIRE", false);
        //W Settings
        internal static MenuBool Wcombo = new MenuBool("W combo", "W Combo");
        //E Settings
        internal static MenuBool Ecombo = new MenuBool("E Combo", "E COmbo");
        //R Settings
        internal static MenuBool Rcombo = new MenuBool("R combo", "R Combo");
        internal static MenuSlider Hit = new MenuSlider("Hit target", "Target Hit Count >= ", 2, 1, 5);
        internal static MenuSlider Heath = new MenuSlider("target Heath", "Target Heath % <= ", 60, 0, 100);
        //Auto Settings
        internal static MenuBool EarlyCombo = new MenuBool("Early Combo", "Early Combo");
        internal static MenuBool QStun = new MenuBool("Q Stun", "Auto Q if can stun");
        internal static MenuSliderButton Whit = new MenuSliderButton("W Hit Count", "Auto W if can hit", 2, 1, 5);
        internal static MenuSliderButton AutoR = new MenuSliderButton("R auto hit", "Auto R if target hit count >=", 3, 1, 5);

    }
}
