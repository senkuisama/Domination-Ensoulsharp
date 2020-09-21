using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;
using FunnySlayerCommon;
using FSpred.Prediction;

namespace Pyke_Ryū
{
    public class Program
    {       
        public static AIHeroClient Player = GameObjects.Player;
        public static Spell Q = new Spell(SpellSlot.Q, 400f);
        public static Spell E = new Spell(SpellSlot.E, 550f);
        public static Spell R = new Spell(SpellSlot.R, 750f);
        public static Menu RootPyke = new Menu("Pyke_Ryū", "Pyke_Ryū", true);
        public static Menu Selector = new Menu("Selector", "Target Menu");
        public static MenuBool Rpyke = new MenuBool("R Pyke", "R KS");
        public static MenuBool Qpyke = new MenuBool("Q Combo", "Q Combo|Harass");
        public static MenuBool Wpyke = new MenuBool("W Combo", "W Combo|Harass");
        public static MenuBool Epyke = new MenuBool("E Combo", "E Combo|Harass");

        public static void GameEvent_OnGameLoad()
        {
            if(Player.CharacterName != "Pyke" || Player == null)
            {
                return;
            }
            R.SetSkillshot(0.5f, 100f, float.MaxValue, false, EnsoulSharp.SDK.Prediction.SkillshotType.Circle);
            Game.OnUpdate += RKS;

            Q.SetSkillshot(0.25f, 55f, 2000, true, EnsoulSharp.SDK.Prediction.SkillshotType.Line);
            E.SetSkillshot(0.25f, 70f, 2000, false, EnsoulSharp.SDK.Prediction.SkillshotType.Line);
            
            Q.SetCharged("PykeQ", "PykeQ", 400, 1100, 1.15f);

            Selector.AddTargetSelectorMenu();
            RootPyke.Add(Selector);
            RootPyke.Add(Rpyke);
            RootPyke.Add(Qpyke);
            RootPyke.Add(Wpyke);
            RootPyke.Add(Epyke);           
            RootPyke.Attach();

            Game.OnUpdate += Game_OnUpdate;
            Game.OnUpdate += Game_OnUpdate1;
            Drawing.OnDraw += Drawing_OnDraw;            
        }

        private static void Game_OnUpdate1(EventArgs args)
        {
            if (Q.IsCharging)
            {
                Orbwalker.AttackState = false;
            }
            else
            {
                Orbwalker.AttackState = true;
            }
        }

        private static void RKS(EventArgs args)
        {
            if (Player.IsDead) return;

            if (!Rpyke.Enabled) return;

            var target = FSTargetSelector.GetFSTarget(R.Range);
            if(target != null)
            {
                if(target.Health < R.GetDamage(target, DamageStage.Empowered) && R.IsReadyToCastOn(target))
                {
                    var pred = FSpred.Prediction.Prediction.GetPrediction(R, target);
                    if(pred.Hitchance >= HitChance.High)
                    {
                        if (R.Cast(pred.CastPosition))
                            return;
                        else
                        {
                            R.Cast(pred.CastPosition);
                        }                       
                    }
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead) return;

            Drawing.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.White);
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Player.IsDead) return;

            if(Orbwalker.ActiveMode == OrbwalkerMode.Combo || Orbwalker.ActiveMode == OrbwalkerMode.Harass)
            {
                CASTQ();
                CASTW();
                CASTE();
            }
        }

        private static void CASTQ()
        {
            var targets = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(Q.ChargedMaxRange)).ToList();
            if (targets == null) return;

            if (!Qpyke.Enabled) return;

            if (Q.IsReady())
            {
                if (Q.IsCharging)
                {
                    var pred = FSpred.Prediction.Prediction.GetPrediction(Q, targets.OrderBy(i => i.Health).FirstOrDefault(i => Prediction.GetPrediction(Q, i).Hitchance >= HitChance.High));
                    if(pred.Hitchance >= HitChance.High)
                    {
                        Q.Cast(pred.CastPosition);
                    }
                }
                else
                {
                    var target = targets.FirstOrDefault(i => Prediction.GetPrediction(Q, i).Hitchance >= HitChance.High);
                    if (target.IsValidTarget(400))
                    {
                        Q.Cast(target);
                    }
                    var pred = Prediction.GetPrediction(Q, targets.OrderBy(i => i.Health).FirstOrDefault(i => Prediction.GetPrediction(Q, i).Hitchance >= HitChance.High));
                    if (pred.Hitchance >= HitChance.High)
                    {
                        Q.StartCharging();
                    }
                }
            }
        }
        private static void CASTW()
        {
            if (!Wpyke.Enabled)
                return;
            var target = FSTargetSelector.GetFSTarget(2000);
            if (target == null) return;

            if (Q.IsCharging) return;

            if((new Spell(SpellSlot.W)).IsReady())
            {
                if (!target.IsValidTarget(E.Range + Q.Range))
                {
                    (new Spell(SpellSlot.W)).Cast();
                }
            }
        }
        private static void CASTE()
        {
            var targets = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(E.Range)).ToList();
            if (targets == null) return;

            if (!Epyke.Enabled) return;

            if (Q.IsCharging) return;

            if (E.IsReady())
            {
                var pred = SebbyLibPorted.Prediction.Prediction.GetPrediction(E, targets.FirstOrDefault(i => Prediction.GetPrediction(E, i).Hitchance >= HitChance.High));
                if (pred.Hitchance >= SebbyLibPorted.Prediction.HitChance.High)
                {
                    E.Cast(pred.CastPosition);
                }
            }
        }
    }
}
