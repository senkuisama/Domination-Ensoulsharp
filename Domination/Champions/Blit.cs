using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;

using SPredictionMash;

namespace DominationAIO.Champions
{
    public static class Blit
    {
        private static Menu BlitMenu = new Menu("Blit Menu", "Simple Blitzcrank", true);
        private static MenuBool QComboHarass = new MenuBool("QComboHarass", "Q Combo Harass");
        private static MenuBool WCombo = new MenuBool("WCombo", "W Combo");
        private static MenuBool EComboHarass = new MenuBool("EComboHarass", "E Combo Harass");
        private static MenuBool RCombo = new MenuBool("RCombo", "R Combo");
        private static MenuBool RKs = new MenuBool("RKs", "R KS");

        private static MenuSlider QMin = new MenuSlider("QMin", "Q min range", 0, 250, 500);
        private static MenuSlider RMin = new MenuSlider("RMin", "R Min Hit", 1, 2, 5);

        private static AIHeroClient Player => ObjectManager.Player;
        private static Spell Q, W, E, R;
        public static void Load()
        {
            if (Player == null)
                return;

            BlitMenu.Add(new MenuSeparator("Simple Blit", "Simple Blit"));
            

            var HelperMenu = new Menu("Helper", "Helper");
            //FunnySlayerCommon.MenuClass.AddTargetSelectorMenu(HelperMenu);
            SPredictionMash.ConfigMenu.Initialize(HelperMenu, "Get Prediction");
            //new SebbyLibPorted.Orbwalking.Orbwalker(HelperMenu);

            BlitMenu.Add(HelperMenu);
            BlitMenu.Add(QComboHarass);
            BlitMenu.Add(WCombo);
            BlitMenu.Add(EComboHarass);
            BlitMenu.Add(RCombo);
            BlitMenu.Add(RKs);
            BlitMenu.Add(QMin);
            BlitMenu.Add(RMin);

            BlitMenu.Attach();

            Q = new Spell(SpellSlot.Q, 1150f);
            Q.SetSkillshot(0.25f, 100f, 2000, true, SpellType.Line);
            W = new Spell(SpellSlot.W, Player.GetRealAutoAttackRange());
            E = new Spell(SpellSlot.E, Player.GetRealAutoAttackRange());
            R = new Spell(SpellSlot.R, 600);
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;

            if(Orbwalker.ActiveMode <= OrbwalkerMode.Harass)
            {
                BlitDoCombo();
            }
        }

        private static void BlitDoCombo()
        {
            if(QComboHarass.Enabled && Q.IsReady())
            {
                var Target = FunnySlayerCommon.FSTargetSelector.GetFSTarget(Q.Range);
                if(Target != null)
                {
                    var Pred = SebbyLibPorted.Prediction.Prediction.GetPrediction(Q, Target);
                    if(Target.DistanceToPlayer() > QMin.Value && Pred.Hitchance >= SebbyLibPorted.Prediction.HitChance.High)
                    {
                        if (Q.SPredictionCast(Target, EnsoulSharp.SDK.HitChance.High))
                            return;
                    }
                }              
            }

            if(EComboHarass.Enabled && E.IsReady())
            {
                if(Orbwalker.GetTarget() != null)
                {
                    if (E.Cast())
                    {
                        Orbwalker.ResetAutoAttackTimer();
                        return;
                    }
                }
            }

            if(Orbwalker.ActiveMode == OrbwalkerMode.Combo)
            {
                if(WCombo.Enabled && W.IsReady())
                {
                    if (Orbwalker.GetTarget() != null)
                    {
                        if (W.Cast())
                        {
                            return;
                        }
                    }
                }

                if(RCombo.Enabled && R.IsReady())
                {
                    var targets = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(R.Range));
                    if (RKs.Enabled)
                    {
                        
                        if(targets != null)
                        {
                            foreach(var target in targets.OrderBy(i => i.Health))
                            {
                                if(target.Health < Player.GetSpellDamage(target, SpellSlot.R))
                                {
                                    if (R.Cast())
                                        return;
                                }
                            }
                        }
                    }

                    if(targets != null)
                    {
                        if(targets.Count() >= RMin.Value)
                        {
                            if (R.Cast())
                                return;
                        }
                    }
                }
            }
        }
    }
}
