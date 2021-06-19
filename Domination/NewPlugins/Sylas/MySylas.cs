using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominationAIO.NewPlugins
{
    public class MySylas
    {
        private static Spell Q, W, E, R;
        private static Menu SMenu = null;
        protected static void LoadMenu()
        {
            SMenu = new Menu("Sylas", "FunnySlayer Sylas", true);

            Menu Qmenu = new Menu("Qmenu", "Q Settings");
            Menu Wmenu = new Menu("Wmenu", "W Settings");
            Menu Emenu = new Menu("Emenu", "E Settings");

            Qmenu.Add(SylasMenu.QMenu.UseQCombo);
            Qmenu.Add(SylasMenu.QMenu.UseQHarass);
            /*Qmenu.Add(SylasMenu.QMenu.UseQClear);
            Qmenu.Add(SylasMenu.QMenu.QClearHit);
            Qmenu.Add(SylasMenu.QMenu.QManaClear);*/

            Wmenu.Add(SylasMenu.WMenu.UseWCombo);
            Wmenu.Add(SylasMenu.WMenu.OnlyWWhenp);
            Wmenu.Add(SylasMenu.WMenu.OnlyWWhent);
            Wmenu.Add(SylasMenu.WMenu.UseWHarass);

            Emenu.Add(SylasMenu.EMenu.UseECombo);
            Emenu.Add(SylasMenu.EMenu.UseEHarass);
            Emenu.Add(SylasMenu.EMenu.AutoE);



            SMenu.Add(Qmenu);
            SMenu.Add(Wmenu);
            SMenu.Add(Emenu);
            SMenu.Attach();
        }

        private static class SylasMenu
        {
            public static class QMenu
            {
                public static MenuBool UseQCombo = new MenuBool("UseQCombo", "Use Q in Combo");
                public static MenuBool UseQHarass = new MenuBool("UseQHarass", "Use Q in Harass");
                public static MenuBool UseQClear = new MenuBool("UseQClear", "Use Q in Clear");
                public static MenuSlider QClearHit = new MenuSlider("QClearHit", "---> When hit >= x", 3, 1, 6);
                public static MenuSlider QManaClear = new MenuSlider("QManaClear", "---> Clear mana >= x %", 50);
            }
            public static class WMenu
            {
                public static MenuBool UseWCombo = new MenuBool("UseWCombo", "Use W in Combo");
                public static MenuSlider OnlyWWhenp = new MenuSlider("Only When player", "Only When Player Heath < x% ", 50, 0, 101);
                public static MenuSlider OnlyWWhent = new MenuSlider("Only When target", "Only When Target Heath < x% ", 70, 0, 101);
                public static MenuBool UseWHarass = new MenuBool("UseWHarass", "Use W in Harass");
            }
            public static class EMenu
            {
                public static MenuBool UseECombo = new MenuBool("UseECombo", "Use E in Combo");
                public static MenuBool UseEHarass = new MenuBool("UseEHarass", "Use E in Harass");
                public static MenuBool AutoE = new MenuBool("AutoE", "Auto E on best target");
            }                    
        }

        public static void LoadSylas()
        {
            LoadMenu();
            Q = new Spell(SpellSlot.Q, 775);
            Q.SetSkillshot(0.4f, 50, float.MaxValue, false, SpellType.Circle);
            W = new Spell(SpellSlot.W, 400);
            W.SetTargetted(0.4f, 2000);
            E = new Spell(SpellSlot.E, 800);
            E.SetSkillshot(0.25f, 50f, 1500f, true, SpellType.Line);
            R = new Spell(SpellSlot.R, 950);

            Game.OnUpdate += Game_OnUpdate;
            AIBaseClient.OnProcessSpellCast += AIBaseClient_OnProcessSpellCast;
            Orbwalker.OnAfterAttack += Orbwalker_OnAfterAttack;
        }

        private static void Orbwalker_OnAfterAttack(object sender, AfterAttackEventArgs e)
        {
            LastCasted = 0;
        }

        private static int LastCasted = 0;
        private static int LastE = 0;


        private static void AIBaseClient_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
                return;

            if (Orbwalker.IsAutoAttack(args.SData.Name) && args.Target is AIHeroClient)
            {
                LastCasted = 0;
            }

            if(args.Slot <= SpellSlot.E && args.Slot > SpellSlot.Unknown)
            {
                LastCasted = Variables.GameTimeTickCount;
                if(args.Slot == SpellSlot.E)
                {
                    LastE = Variables.GameTimeTickCount;
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
                return;


            if(Orbwalker.ActiveMode == OrbwalkerMode.Combo)
            {
                Combo();
            }
        }

        private static void Combo()
        {          
            
            if(R.IsReady() && R.Name.ToString().ToLower().Contains("pyke"))
            {
                R.SetSkillshot(0.4f, 100f, float.MaxValue, false, SpellType.Circle);
                var targets = ObjectManager.Get<AIHeroClient>().Where(i => !i.IsDead && i.IsEnemy && !i.IsAlly && !i.IsMinion() && !i.IsJungle() && i.Type == GameObjectType.AIHeroClient).OrderBy(i => i.DistanceToPlayer()).ThenBy(i => i.Health);
                if (targets != null)
                {
                    foreach (var target in targets)
                    {
                        if (target != null && !target.IsDead && target.HealthPercent <= 30)
                        {
                            var pred = FSpred.Prediction.Prediction.GetPrediction(R, target);
                            if (pred.Hitchance >= FSpred.Prediction.HitChance.High)
                            {
                                if (R.Cast(pred.CastPosition))
                                    return;
                            }
                        }
                    }
                }
            }
            if (E.IsReady() && SylasMenu.EMenu.UseECombo.Enabled)
            {
                if (E.Name.ToString().Contains("2"))
                {
                    goto E2Combo;
                }
                else
                {
                    var target = TargetSelector.GetTarget(E.Range, DamageType.Physical);
                    if(target != null)
                    {
                        E.Cast(target.Position);
                        return;
                    }
                }
            }

            E2Combo:
            {
                var targets = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget()
                    && i.DistanceToPlayer() <= Q.Range && !i.IsDead
                    && FSpred.Prediction.Prediction.GetPrediction(Q, i).Hitchance >= FSpred.Prediction.HitChance.High)
                    .OrderBy(i => i.Health);

                if(targets.Count() >= 1 && Q.IsReady() && SylasMenu.QMenu.UseQCombo.Enabled)
                {
                    var target = targets.FirstOrDefault();

                    var pred = FSpred.Prediction.Prediction.GetPrediction(Q, target);
                    if(pred.Hitchance >= FSpred.Prediction.HitChance.High)
                    {
                        if (LastCasted + 1000 >= Variables.GameTimeTickCount)
                            return;

                        Q.Cast(pred.CastPosition);
                        return;
                    }
                }
            }

            {               
                if (E.Name.ToString().Contains("2"))
                {
                    var targets = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget()
                    && i.DistanceToPlayer() <= E.Range && !i.IsDead
                    && FSpred.Prediction.Prediction.GetPrediction(E, i).Hitchance >= FSpred.Prediction.HitChance.High)
                    .OrderBy(i => i.Health);


                    if (targets.Count() >= 1 && E.IsReady() && SylasMenu.EMenu.UseECombo.Enabled)
                    {
                        var target = targets.FirstOrDefault();

                        var pred = FSpred.Prediction.Prediction.GetPrediction(E, target);
                        if (pred.Hitchance >= FSpred.Prediction.HitChance.High)
                        {
                            if (LastCasted + 1000 >= Variables.GameTimeTickCount && LastE + 2750 >= Variables.GameTimeTickCount)
                                return; 

                            E.Cast(pred.CastPosition);
                            return;
                        }
                    }
                }
            }
            {
                if (LastCasted + 1750 <= Variables.GameTimeTickCount && W.IsReady() && SylasMenu.WMenu.UseWCombo.Enabled)
                {
                    var target = TargetSelector.GetTarget(400, DamageType.Physical);
                    if (target != null)
                    {
                        W.Cast(target);
                        return;
                    }
                }               
            }
        }
    }
}
