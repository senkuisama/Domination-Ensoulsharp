using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.Utility;
using EnsoulSharp.SDK.Prediction;
using EnsoulSharp.SDK.MenuUI.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominationAIO.Champions
{
    internal static class AkaliHelper
    {
        public static bool HavePassive(this AIBaseClient target)
        {
            return target.GetRealAutoAttackRange() > 250;
        }
        /*public static bool Invisible(this AIBaseClient target)
        {
            return target.Invisible();
        }*/
    }
    internal class AkaliMenu
    {
        public class QSettings
        {
            public static MenuBool Qcombo = new MenuBool("Qcombo", "Q Combo");
            public static MenuBool QPassive = new MenuBool("QPassive", "----> Use Q When have Passive", false);
            public static MenuBool Qks = new MenuBool("Qks", "Use Q in KS");
        }
        public class WSettings
        {
            public static MenuBool Wcombo = new MenuBool("Wcombo", "W Combo");
            public static MenuSliderButton TargetCount = new MenuSliderButton("TargetCount", "Count Enemy Heroes In Range > 1", 600, 400, 800);
            public static MenuSliderButton Wmana = new MenuSliderButton("Wmana", "Energy < [0 = off]", 100, 0, 200);
        }

        public class ESettings
        {
            public static MenuBool Ecombo = new MenuBool("Ecombo", "E Combo");

            public static MenuBool E2 = new MenuBool("E2", "----> E2");
            public class E2Use
            {
                public static MenuKeyBind TurretE2 = new MenuKeyBind("TurretE2", "Use on turret [Type: Toggle]", System.Windows.Forms.Keys.T, KeyBindType.Toggle) { Active = false };
                public static MenuSlider TargetHeath = new MenuSlider("TargetHeath", "Target Heath <= %", 50, 0, 101);
                public static MenuSlider CountAlly = new MenuSlider("CountAlly", "Ally in 600 form target Count", 1, 0, 4);
                public static MenuSlider E2mana = new MenuSlider("E2mana", "Energy > ", (int)ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).ManaCost, 0, (int)ObjectManager.Player.MaxMana);
                public static MenuBool WIsReady = new MenuBool("WIsReady", "W Is Ready");
            }
        }

        public class RSettings
        {
            public static MenuKeyBind Rcombo = new MenuKeyBind("Rcombo", "R Combo [Type: Toggle]", System.Windows.Forms.Keys.G, KeyBindType.Toggle);
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

        public static float Last_Q = -500;
        public static float Last_W = -500;
        public static float Last_E = -500;
        public static float Last_R = -500;

        public static void OnLoad()
        {
            //
            // Check Spell
            //
            Q = new Spell(SpellSlot.Q, 500);
            Q.SetSkillshot(0.25f, 70f, 1200f, false, SkillshotType.Cone);

            W = new Spell(SpellSlot.W, 250);
            W.SetSkillshot(0.3f, 350f, 1200f, false, SkillshotType.Circle);

            E = new Spell(SpellSlot.E, 825);
            E.SetSkillshot(0.25f, 70f, 1200, true, SkillshotType.Line);

            R = new Spell(SpellSlot.R, 675);
            R.SetTargetted(0.3f, float.MaxValue);

            R2 = new Spell(SpellSlot.R, 750);
            R2.SetSkillshot(0.125f, 80f, float.MaxValue, false, SkillshotType.Line);

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

            AIHeroClient.OnAggro += AIHeroClient_OnAggro;
        }

        public static bool CanUseQNow = false;
        public static bool CastedAndHit = false;
        public static float TimeCast = 0;
        public static bool isQ = false;
        public static bool isW = false;
        public static bool isE = false;
        public static bool isR = false;

        private static void AIHeroClient_OnAggro(AIBaseClient sender, AIBaseClientAggroEventArgs args)
        {
            
            if (sender.IsAlly && sender.IsMelee && sender.Name == "TwilightShroud" && isQ)
            {
                TimeCast = Variables.TickCount;
                CastedAndHit = true;
            }           
        }

        public static void CheckAll()
        {
            if (Variables.TickCount - Last_Q <= 20)
            {
                isQ = true;

                isW = false;
                isE = false;
                isR = false;
            }
            if(Variables.TickCount - Last_Q > 2000)
            {
                isQ = false;
            }
            if (Variables.TickCount - Last_W <= 20)
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

            if (Variables.TickCount - Last_E <= 20)
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

            if (Variables.TickCount - Last_R <= 20)
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


            if(Variables.TickCount - TimeCast > 2000)
            {
                CastedAndHit = false;
            }

            if (Variables.TickCount - TimeCast < 2000 && Player.HavePassive())
            {
                CastedAndHit = false;
            }

            if (CastedAndHit == false && !Player.HavePassive())
            {
                CanUseQNow = true;
            }
            else
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
            if (Player.IsDead) return;

            CheckAll();


           

            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    if (Player.HasBuff("akaliwstealth"))
                    {
                        if(!Player.HavePassive())
                            Orbwalker.AttackState = false;
                        else
                            Orbwalker.AttackState = true;
                    }
                    else
                    {
                        Orbwalker.AttackState = true;
                    }
                    Combo();
                    break;
                case OrbwalkerMode.LaneClear:
                    Clear();
                    break;
                default:
                    Orbwalker.AttackState = true;
                    Orbwalker.MovementState = true;
                    break;
            }
        }

        private static void Combo()
        {
            var target = TargetSelector.GetTarget(Q.Range);

            if (target != null)
            {                
                if (Q.IsReady() && CanUseQNow == true)
                {
                    if (!W.IsReady())
                    {
                        if (!Player.IsDashing() && Variables.TickCount - Last_E > 700 && Variables.TickCount - Last_R > 700)
                            Q.Cast(target);
                    }
                    else
                    {
                        if(Player.Mana - Q.Mana <= Q.Mana - 1.5f * 5)
                        {
                            if (target.IsValidTarget(Q.Range) && !Player.IsDashing() && Variables.TickCount - Last_E > 700 && Variables.TickCount - Last_R > 700)
                            {
                                Q.Cast(target);
                                W.Cast(Player.Position);
                            }
                                
                        }
                    }
                    
                }

                if(W.IsReady() && AkaliMenu.WSettings.TargetCount.Enabled && Player.CountEnemyHeroesInRange(AkaliMenu.WSettings.TargetCount.Value) > 1)
                {
                    W.Cast(Player.Position);
                }

                if(Q.IsReady() && Player.Mana > 150 && Player.HealthPercent > 50)
                {

                }
                else
                {
                    if (W.IsReady() && E.IsReady() && Player.Mana - E.Mana <= Q.Mana)
                    {
                        if(E.Name == "AkaliE")
                        {
                            if (Player.HavePassive()) return;

                            if (CanUseQNow == true)
                            {
                                E.Cast(E.GetPrediction(target).CastPosition);
                                DelayAction.Add(1, () => { W.Cast(E.GetPrediction(target).CastPosition); });
                            }                               
                        }
                        
                        if(E.Name == "AkaliEb")
                        {
                            E.Cast(E.GetPrediction(target).CastPosition);

                            if (target.IsValidTarget(300))
                            {
                                W.Cast(Player.Position);
                            }
                        }
                    }
                    else
                    {
                        if (W.IsReady())
                        {
                            if(Player.Mana <= AkaliMenu.WSettings.Wmana.Value && target.Position.DistanceToPlayer() < 500)
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
                                        E.Cast(E.GetPrediction(target).CastPosition);
                                    }
                            }
                        }
                        if (E.Name == "AkaliEb")
                        {
                            if (E.IsReady())
                            {
                                if (Variables.TickCount - Last_Q > 700)
                                {
                                    E.Cast(E.GetPrediction(target).CastPosition);
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

                    if(R.Name == "AkaliR" && CanUseQNow && Variables.TickCount - Last_Q > 700)
                    {
                        R.Cast(target);
                    }
                }

                if (R.IsReady())
                {
                    if(target.Health <= R.GetDamage(target) + (39 + 15 * Player.Level))
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
                if (Q.IsReady())
                {
                    if(target.Health <= Q.GetDamage(target) + (39 + 15 * Player.Level))
                    {
                        Q.Cast(target);
                    }
                }
            }           
        }

        private static void Clear()
        {

        }
    }
}
