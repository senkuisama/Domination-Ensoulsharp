using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.Utility;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using System.Drawing;

namespace DominationAIO.Champions
{
    internal static class AkaliHelper
    {
        public static bool HavePassive(this AIBaseClient target)
        {
            return target.GetCurrentAutoAttackRange() > 250;
        }
    }
    internal class AkaliMenu
    {
        public class QSettings
        {
            public static MenuBool Qcombo = new MenuBool("Qcombo", "Q Combo");
            public static MenuBool QPassive = new MenuBool("QPassive", "----> Use Q When have Passive", false);
            public static MenuBool Qks = new MenuBool("Qks", "Use Q in KS");
            public static MenuSliderButton MoveQ = new MenuSliderButton("MoveQ", "Move After Q (ms)", 1000, 0, 2000);
        }
        public class WSettings
        {
            public static MenuBool Wcombo = new MenuBool("Wcombo", "W Combo");
            public static MenuSliderButton TargetCount = new MenuSliderButton("TargetCount", "Count Enemy Heroes In Range > 1", 600, 400, 800);
            public static MenuSliderButton Wmana = new MenuSliderButton("Wmana", "Energy < [0 = off]", 100, 0, 300);
        }

        public class ESettings
        {
            public static MenuBool Ecombo = new MenuBool("Ecombo", "E Combo");

            public static MenuBool E2 = new MenuBool("E2", "----> E2");
            public class E2Use
            {
                public static MenuKeyBind TurretE2 = new MenuKeyBind("TurretE2", "Use on turret [Type: Toggle]", Keys.T, KeyBindType.Toggle) { Active = false };
                public static MenuSlider TargetHeath = new MenuSlider("TargetHeath", "Target Heath <= %", 50, 0, 101);
                public static MenuSlider CountAlly = new MenuSlider("CountAlly", "Ally in 600 form target Count", 1, 0, 4);
                public static MenuSlider E2mana = new MenuSlider("E2mana", "Energy > ", (int)ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).ManaCost, 0, (int)ObjectManager.Player.MaxMana);
                public static MenuBool WIsReady = new MenuBool("WIsReady", "W Is Ready");
            }
        }

        public class RSettings
        {
            public static MenuKeyBind Rcombo = new MenuKeyBind("Rcombo", "R Combo [Type: Toggle]", Keys.G, KeyBindType.Toggle);
            public static MenuBool RPassive = new MenuBool("RPassive", "----> Use R When have Passive", false);
        }

        public class ClearSettings
        {
            public static MenuSlider ClearMana = new MenuSlider("ClearMana", "Mana >= [200 = off]", 100, 0, 200);
            public static MenuBool useQ = new MenuBool("useQ", "Use Q");
        }
    }
    internal class Akali
    {
        public static Menu akalimenu, Qmenu, Wmenu, Emenu, Rmenu, Clearmenu;
        public static AIHeroClient Player = ObjectManager.Player;

        public static Spell Q, W, E, R, R2;

        public static float Last_Q = 0;
        public static float Last_W = 0;
        public static float Last_E = 0;
        public static float Last_R = 0;

        public static void OnLoad()
        {
            //
            // Check Spell
            //
            Q = new Spell(SpellSlot.Q, 500);
            Q.SetSkillshot(0.25f, 70f, 1200f, false, SpellType.Cone);

            W = new Spell(SpellSlot.W, 250);
            W.SetSkillshot(0.3f, 350f, 1200f, false, SpellType.Circle);

            E = new Spell(SpellSlot.E, 800);
            E.SetSkillshot(0.25f, 100f, 800, true, SpellType.Line, HitChance.High);

            R = new Spell(SpellSlot.R, 675);
            R.SetTargetted(0.3f, float.MaxValue);

            R2 = new Spell(SpellSlot.R, 750);
            R2.SetSkillshot(0.125f, 80f, float.MaxValue, false, SpellType.Line);

            //
            // Check Menu
            //

            Qmenu = new Menu("Qmenu", "Q Settings");
            Wmenu = new Menu("Wmenu", "W Settings");
            Emenu = new Menu("Emenu", "E Settings");
            Rmenu = new Menu("Rmenu", "R Settings");
            Clearmenu = new Menu("Clearmenu", "Clear Settings");

            Qmenu.Add(AkaliMenu.QSettings.Qcombo);
            Qmenu.Add(AkaliMenu.QSettings.QPassive);
            Qmenu.Add(AkaliMenu.QSettings.Qks);
            Qmenu.Add(AkaliMenu.QSettings.MoveQ);

            Wmenu.Add(AkaliMenu.WSettings.Wcombo);
            Wmenu.Add(AkaliMenu.WSettings.Wmana);
            Wmenu.Add(AkaliMenu.WSettings.TargetCount);

            Emenu.Add(AkaliMenu.ESettings.Ecombo);
            Emenu.Add(AkaliMenu.ESettings.E2);
            Menu E2Menu = new Menu("E2Menu", "E2 Settings");
            E2Menu.Add(AkaliMenu.ESettings.E2Use.WIsReady);
            E2Menu.Add(AkaliMenu.ESettings.E2Use.TargetHeath);
            E2Menu.Add(AkaliMenu.ESettings.E2Use.E2mana);
            E2Menu.Add(AkaliMenu.ESettings.E2Use.CountAlly);
            E2Menu.Add(AkaliMenu.ESettings.E2Use.TurretE2).Permashow();
            Emenu.Add(E2Menu);


            Rmenu.Add(AkaliMenu.RSettings.Rcombo).Permashow();
            Rmenu.Add(AkaliMenu.RSettings.RPassive);

            Clearmenu.Add(AkaliMenu.ClearSettings.useQ);
            Clearmenu.Add(AkaliMenu.ClearSettings.ClearMana);

            akalimenu = new Menu("AkaliMenu", "Akali Menu", true);
            akalimenu.Add(Qmenu);
            akalimenu.Add(Wmenu);
            akalimenu.Add(Emenu);
            akalimenu.Add(Rmenu);

            akalimenu.Add(Clearmenu);

            akalimenu.Attach();

            //
            // Do Events
            //
            Game.OnUpdate += Game_OnUpdate;
            AIHeroClient.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
            //Orbwalker.OnAction += Orbwalker_OnAction;

            AIHeroClient.OnAggro += AIHeroClient_OnAggro;

            Drawing.OnDraw += Drawing_OnDraw;

            Orbwalker.OnAfterAttack += Orbwalker_OnAfterAttack;
            Orbwalker.OnBeforeAttack += Orbwalker_OnBeforeAttack;
            Orbwalker.OnAttack += Orbwalker_OnAttack;

            if (ObjectManager.Player.PercentCooldownMod < -0.7)
                isURF = true;
        }

        private static void Orbwalker_OnAttack(object sender, AttackingEventArgs e)
        {
            if (Player.HavePassive() && Variables.TickCount - Last_E >= 1000)
            {
                CanUseQNow = true;
            }
        }

        private static void Orbwalker_OnBeforeAttack(object sender, BeforeAttackEventArgs e)
        {
            if (Player.HavePassive() && Variables.TickCount - Last_E >= 1000)
            {
                CanUseQNow = true;
            }
        }

        private static void Orbwalker_OnAfterAttack(object sender, AfterAttackEventArgs e)
        {
            if (Player.HavePassive() && Variables.TickCount - Last_E >= 1000)
            {
                CanUseQNow = true;
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.HavePassive())
            {
                Render.Circle.DrawCircle(Player.Position, Player.GetCurrentAutoAttackRange(), System.Drawing.Color.White);
            }
            else
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.Blue);
            }

            var target = Orbwalker.GetTarget() as AIHeroClient;

            if(target != null && target is AIHeroClient && target.IsVisibleOnScreen)
            {
                Render.Circle.DrawCircle(target.Position, 70, System.Drawing.Color.Gold);
            }
        }

        /*public static bool BeforeAttack = false;
        private static void Orbwalker_OnAction(object sender, OrbwalkerActionArgs args)
        {
            if(args.Type == OrbwalkerType.BeforeAttack)
            {
                BeforeAttack = true;
            }
            else
            {
                BeforeAttack = false;
            }
        }*/

        public static bool CanUseQNow = false;
        public static bool CastedAndHit = false;
        public static float TimeCast = 0;
        public static bool isQ = false;
        public static bool isW = false;
        public static bool isE = false;
        public static bool isR = false;
        public static bool isURF = false;
        private static void AIHeroClient_OnAggro(AIBaseClient sender, AIBaseClientAggroEventArgs args)
        {
            if (sender.CharacterName == "TestCubeRender" && isQ)
            {
                TimeCast = Variables.TickCount;
                CastedAndHit = true;
            }           
        }

        public static void CheckAll()
        {
            if (Variables.TickCount - Last_Q <= 100)
            {
                isQ = true;

                isW = false;
                isE = false;
                isR = false;
            }
            if(Variables.TickCount - Last_Q > 2500)
            {
                isQ = false;
            }

            if (Variables.TickCount - Last_W <= 100)
            {
                isW = true;

                isQ = false;
                isE = false;
                isR = false;
            }
            if (Variables.TickCount - Last_W > 2000)
            {
                isW = false;
            }

            if (Variables.TickCount - Last_E <= 100)
            {
                isE = true;

                isW = false;
                isQ = false;
                isR = false;
            }
            if (Variables.TickCount - Last_E > 2000)
            {
                isE = false;
            }

            if (Variables.TickCount - Last_R <= 100)
            {
                isR = true;

                isW = false;
                isE = false;
                isQ = false;
            }
            if (Variables.TickCount - Last_R > 2000)
            {
                isR = false;
            }


            if(Variables.TickCount - TimeCast > 2500)
            {
                CastedAndHit = false;
            }

            if (Player.HavePassive())
            {
                CastedAndHit = false;
            }

            if (CastedAndHit == false && !Player.HavePassive() && Variables.TickCount - Last_E >= 1000)
            {
                CanUseQNow = true;
            }
            else
            {
                CanUseQNow = false;
            }

            if (Player.HavePassive() && FunnySlayerCommon.OnAction.BeforeAA && Variables.TickCount - Last_E >= 1000)
            {
                CanUseQNow = true;
            }


            if (ObjectManager.Player.IsDashing() || Variables.TickCount - Last_E <= 700 || Variables.TickCount - Last_R <= 700)
            {
                CanUseQNow = false;
            }
        }

        private static void AIHeroClient_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {            
            if (sender.IsMe)
            {
                if(args.SData.Name == "AkaliQ" || args.SData.Name.ToString().Contains("AkaliQ"))
                {
                    Last_Q = Variables.TickCount;
                }
                if (args.SData.Name == W.SData.Name || args.SData.Name.ToString().Contains(W.SData.Name))
                {
                    Last_W = Variables.TickCount;
                }
                if (args.SData.Name == E.SData.Name || args.SData.Name.ToString().Contains(E.SData.Name))
                {
                    Last_E = Variables.TickCount;
                }
                if (args.SData.Name == R.SData.Name || args.SData.Name.ToString().Contains(R.SData.Name))
                {
                    Last_R = Variables.TickCount;
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if(CanUseQNow == false && AkaliMenu.QSettings.QPassive.Enabled || isURF)
            {
                CanUseQNow = true;
            }

            if (Player.IsDead) {
                Orbwalker.AttackEnabled = true;
                Orbwalker.MoveEnabled = true;

                Orbwalker.SetOrbwalkerPosition(Vector3.Zero);

                return;
            }
            if (Variables.TickCount - TimeCast > AkaliMenu.QSettings.MoveQ.Value)
            {
                Orbwalker.SetOrbwalkerPosition(Vector3.Zero);
            }

            CheckAll();

            KS();

            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    if (Player.HasBuff("akaliwstealth"))
                    {
                        if(!Player.HavePassive())
                            Orbwalker.AttackEnabled = false;
                        else
                            Orbwalker.AttackEnabled = true;
                    }
                    else
                    {
                        Orbwalker.AttackEnabled = true;
                    }
                    Combo();
                    break;
                case OrbwalkerMode.LaneClear:
                    Clear();
                    break;
                default:
                    Orbwalker.AttackEnabled = true;
                    Orbwalker.MoveEnabled = true;
                    break;
            }
        }

        private static void KS()
        {
            var targets = TargetSelector.GetTargets(R.IsReady() ? 750 : 500, DamageType.Magical);

            if (targets == null) return;

            foreach(var target in targets)
            {
                if (target == null) return;

                if(target.Health <= Q.GetDamage(target) && Q.IsReady() && !Player.IsDashing() && target.IsValidTarget(500))
                {
                    Q.Cast(target);
                }
                if(target.Health <= R.GetDamage(target) && R.IsReady() && target.IsValidTarget(R.Name == "AkaliRb" ? R2.Range : R.Range))
                {
                    if (R.Name == "AkaliRb")
                    {
                        R2.Cast(target);
                    }
                    else
                    {
                        R2.Cast(target);
                    }
                }
            }
        }

        private static void Combo()
        {
            var target = TargetSelector.SelectedTarget;

            if(target == null || !target.IsValidTarget(750))
            {
                target = TargetSelector.GetTarget(R.IsReady() ? 750 : 600, DamageType.Magical);
            }

            if(target == null || target.InAutoAttackRange())
            {
                target = Orbwalker.GetTarget() as AIHeroClient;
            }

            if (target != null)
            {
                if (Variables.TickCount - TimeCast <= AkaliMenu.QSettings.MoveQ.Value && target.IsValidTarget(570) && !Player.HavePassive() && AkaliMenu.QSettings.MoveQ.Enabled &&

                !ObjectManager.Get<AITurretClient>()
                    .Any(i => i.IsEnemy && !i.IsDead && (i.Distance(target.Position.Extend(Player.Position, +570)) < 850 + ObjectManager.Player.BoundingRadius))
                    
                    && !target.Position.Extend(Player.Position, +570).IsWall()
                    && !target.Position.Extend(Player.Position, +200).IsBuilding()
                    && !target.Position.Extend(Player.Position, +570).IsBuilding()
                    && target.Position.Extend(Player.Position, +570).IsValid()
                    )
                {
                    Orbwalker.AttackEnabled = false;
                    Orbwalker.SetOrbwalkerPosition(target.Position.Extend(Player.Position, +600));
                }
                else
                {
                    Orbwalker.SetOrbwalkerPosition(Vector3.Zero);
                    Orbwalker.AttackEnabled = true;
                }

                if (Q.IsReady() && CanUseQNow == true && Q.GetPrediction(target).Hitchance >= HitChance.High && Q.GetPrediction(target).CastPosition.DistanceToPlayer() <= Q.Range)
                {
                    if (!W.IsReady())
                    {
                        if (Variables.TickCount - Last_E > 700 && Variables.TickCount - Last_R > 700)
                            if (Q.Cast(target.Position)) return;                   
                    }
                    else
                    {
                        if(Player.Mana - Q.Mana <= Q.Mana - 1.5f * 5 || isURF)
                        {
                            if (target.IsValidTarget(Q.Range))
                            {
                                Q.Cast(target.Position);
                                W.Cast(target.Position);
                            }

                        }
                        else
                        {
                            if (Q.Cast(target.Position)) return;
                        }
                    }
                    
                }

                if(W.IsReady() && AkaliMenu.WSettings.TargetCount.Enabled && Player.CountEnemyHeroesInRange(AkaliMenu.WSettings.TargetCount.Value) > 1)
                {
                    if(!Player.IsDashing())
                        W.Cast(Player.Position);
                }

                if(Q.IsReady() && Player.Mana > 150 && Player.HealthPercent > 80)
                {

                }
                else
                {
                    var Epred = E.GetPrediction(target, false, -1, new CollisionObjects[] { CollisionObjects.Minions, CollisionObjects.YasuoWall});
                    if (W.IsReady() && E.IsReady() && (Player.Mana - E.Mana <= Q.Mana) || isURF)
                    {
                        if(E.Name == "AkaliE")
                        {
                            if (Player.HavePassive()) return;

                            if (CanUseQNow == true)
                            {
                                E.Cast(Epred.CastPosition);
                                DelayAction.Add(1, () => { W.Cast(Epred.CastPosition); });
                            }                               
                        }
                        
                        if(E.Name == "AkaliEb" && GameObjects.EnemyHeroes.Any(i => i.HasBuff("AkaliEMis")))
                        {
                            E.Cast();

                            if (target.IsValidTarget(400))
                            {
                                W.Cast(target.Position);
                            }
                        }
                    }
                    else
                    {
                        if (W.IsReady())
                        {
                            if(Player.Mana <= AkaliMenu.WSettings.Wmana.Value || isURF && target.Position.DistanceToPlayer() < 500)
                            {
                                W.Cast(Player.Position);
                            }
                        }
                        if(E.Name == "AkaliE")
                        {
                            if (E.IsReady())
                            {
                                if (Player.HavePassive() && target.IsValidTarget(650)) return;

                                if (CanUseQNow == true)
                                    if (Variables.TickCount - Last_Q > 700)
                                    {
                                        E.Cast(Epred.CastPosition);
                                    }
                            }
                        }
                        if (E.Name == "AkaliEb")
                        {
                            if (E.IsReady())
                            {
                                if (Variables.TickCount - Last_Q > 700)
                                {
                                    E.Cast(Epred.CastPosition);
                                }
                            }
                        }
                    }
                }

                if (!Player.HavePassive() && target.IsValidTarget(R.Name == "AkaliRb" ? R2.Range : R.Range) && AkaliMenu.RSettings.Rcombo.Active)
                {
                    if(target.Health <= R.GetDamage(target) + Player.GetAutoAttackDamage(target))
                    {
                        if(R.Name == "AkaliRb")
                        {
                            R2.Cast(target);
                        }
                        else
                        {
                            R.Cast(target);
                        }                       
                    }

                    if(R.Name == "AkaliR" && Variables.TickCount - Last_Q > 700)
                    {
                        R.Cast(target);
                    }
                }

                if (R.IsReady())
                {
                    if(target.Health <= R.GetDamage(target) + (39 + 15 * Player.Level) + (Q.IsReady() ? Q.GetDamage(target) : 0))
                    {
                        if(R.Name == "AkaliRb")
                        {
                            R2.Cast(target);
                        }
                        else
                        {
                            R.Cast(target);
                        }
                    }
                }
                if (Q.IsReady() && Player.IsDashing() && Q.GetPrediction(target).Hitchance >= HitChance.High && Q.GetPrediction(target).CastPosition.DistanceToPlayer() <= Q.Range)
                {
                    if(target.Health <= Q.GetDamage(target) + (39 + 15 * Player.Level))
                    {
                        Q.Cast(target.Position);
                    }
                }
            }           
        }

        private static void Clear()
        {
            var target = TargetSelector.GetTarget(500, DamageType.Magical);

            if(target != null && Q.IsReady() && !Player.HavePassive() &&

                !ObjectManager.Get<AITurretClient>()
                    .Any(i => i.IsEnemy && !i.IsDead && (i.Distance(Player.Position) < 850 + ObjectManager.Player.BoundingRadius))
                )
            {
                Q.Cast(target.Position);
            }

            var minions = GameObjects.GetMinions(500);

            if (minions.Any())
            {
                var Qfarm = Q.GetLineFarmLocation(minions.ToList());

                if (Qfarm.Position.IsValid() && AkaliMenu.ClearSettings.useQ.Enabled && AkaliMenu.ClearSettings.ClearMana.Value < Player.Mana)
                {
                    Q.Cast(Qfarm.Position);
                }
            }
        }
    }
}
