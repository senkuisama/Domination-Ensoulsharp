using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Events;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.Prediction;
using EnsoulSharp.SDK.Utility;
using System.Runtime.Remoting.Messaging;
using System.Runtime.CompilerServices;
using System.Drawing;
using SharpDX.Multimedia;
using DominationAIO.Champions.Helper;
using System.Diagnostics;

namespace DominationAIO.Champions
{
    public static class RivenHelp
    {
        public static void DelayTargetCast(this Spell spell, AIBaseClient target = null, int time = 0)
        {
            if (spell.IsReady())
                DelayAction.Add(time, () =>
                {
                    spell.Cast(target);
                });
        }

        public static void DelayPosCast(this Spell spell, Vector3 pos, int time = 0)
        {
            if (spell.IsReady())
                DelayAction.Add(time, () =>
                {
                    spell.Cast(pos);
                });
        }
    }
    internal class MenuRiven
    {
        public static MenuKeyBind BurstCombo = new MenuKeyBind("BurstCombo", "Burst", System.Windows.Forms.Keys.T, KeyBindType.Press);
        public class QSettings
        {
            public static MenuBool Qcombo = new MenuBool("Qcombo", "Q Combo");
            public static MenuBool QafterAA = new MenuBool("QafterAA", "Q After AA");
            public static MenuBool FastQ = new MenuBool("FaseQ", "Fast Q");
            public static MenuBool QGapcloser = new MenuBool("QGapcloser", "Q Gapcloser");
        }

        public class WSettings
        {
            public static MenuBool Wcombo = new MenuBool("Wcombo", "W Combo");
            public static MenuBool WafterAA = new MenuBool("WafterAA", "W after AA");
            public static MenuBool WDash = new MenuBool("WDash", "W on Dash");
            public static MenuBool Winterupt = new MenuBool("Winterupt", "W interupt");

        }

        public class ESettings
        {
            public static MenuBool Ecombo = new MenuBool("Ecombo", "E Combo");
            public static MenuBool Egapcloser = new MenuBool("Egapcloser", "E Gapcloser");
            public static MenuBool EafterAA = new MenuBool("EafterAA", "E after AA");
        }

        public class RSettings
        {
            public static MenuBool Rcombo = new MenuBool("Rcombo", "R Combo");
            public static MenuSlider target = new MenuSlider("target", "Target", 70);
            public static MenuBool R1 = new MenuBool("R1", "----> R1");
            public static MenuBool R2 = new MenuBool("R2", "----> R2");
            public static MenuBool R2Q = new MenuBool("R2Q", "R2 ---> Q");
            public static MenuBool tiamatR = new MenuBool("tiamatR", "Tiamat <---> R2");
            public static MenuBool R2AA = new MenuBool("R2AA", "R2 ---> AA");
            public static MenuBool ER2 = new MenuBool("ER2", "E ---> R2");
            public static MenuBool ER1Q = new MenuBool("ER1Q", "E ---> R1 / R2 ---> Q");
        }

        public class RangeSettings
        {
            public static MenuSlider QRange = new MenuSlider("QRange", "Q Range", 350, (int)ObjectManager.Player.GetRealAutoAttackRange(), 500);
            public static MenuSlider WRange = new MenuSlider("WRange", "W Range", 265, (int)ObjectManager.Player.GetRealAutoAttackRange(), 330);
            public static MenuSlider ERange = new MenuSlider("ERange", "E Range", 300, (int)ObjectManager.Player.GetRealAutoAttackRange(), 600);
            public static MenuSlider R2Range = new MenuSlider("R2Range", "R2 Range", 900, (int)ObjectManager.Player.GetRealAutoAttackRange(), 900);
        }

        public class ClearSettings
        {
            public static MenuBool useQ = new MenuBool("useQ", "Use Q");
            public static MenuBool useW = new MenuBool("useW", "Use W");
            public static MenuBool useE = new MenuBool("useE", "Use E");
        }
    }

    internal class Rupdate
    {
        public static AIHeroClient Player { get { return ObjectManager.Player; } }
        public static Spell Q, W, E, R, Item, fl, ig;
        public static SpellSlot flash = Player.GetSpellSlot("summonerflash");
        public static SpellSlot ignite = ObjectManager.Player.GetSpellSlot("summonerdot");
        private static Menu RivenMenu;

        public static bool beforeaa = false;
        public static bool afteraa = false;
        public static bool onaa = false;

        public static float lastQcast;
        public static float lastWcast;
        public static float lastEcast;
        public static float lastRcast;
        public static float lastTiamat;

        public static bool R1()
        {
            return R.Name == "RivenFengShuiEngine" && R.IsReady();
        }
        public static bool R2()
        {
            return R.Name == "RivenIzunaBlade";
        }
        public static void OnLoaded()
        {
            Game.Print("FunnySlayer Riven || Still under test");
            Q = new Spell(SpellSlot.Q, 350);
            W = new Spell(SpellSlot.W, 200 + Player.BoundingRadius);
            E = new Spell(SpellSlot.E, 250);
            R = new Spell(SpellSlot.R, 900);
            fl = new Spell(flash, 400f);
            ig = new Spell(ignite, 400);
            Q.SetTargetted(0.1f, float.MaxValue);
            R.SetSkillshot(0.25f, 45, 1600, false, false, SkillshotType.Line);

            RivenMenu = new Menu("RivenMenu", "FunnySlayer Riven", true);

            var Qmenu = new Menu("Qmenu", "Q Settings");
            var Wmenu = new Menu("Wmenu", "W Settings");
            var Emenu = new Menu("Emenu", "E Settings");
            var Rmenu = new Menu("Rmenu", "R Settings");
            var range = new Menu("range", "Range Settings");
            var clear = new Menu("clear", "Clear Settings");

            Qmenu.Add(MenuRiven.QSettings.Qcombo);
            Qmenu.Add(MenuRiven.QSettings.QafterAA);
            Qmenu.Add(MenuRiven.QSettings.FastQ);
            Qmenu.Add(MenuRiven.QSettings.QGapcloser);

            Wmenu.Add(MenuRiven.WSettings.Wcombo);
            Wmenu.Add(MenuRiven.WSettings.WDash);
            Wmenu.Add(MenuRiven.WSettings.Winterupt);
            Wmenu.Add(MenuRiven.WSettings.WafterAA);

            Emenu.Add(MenuRiven.ESettings.Ecombo);
            Emenu.Add(MenuRiven.ESettings.EafterAA);
            Emenu.Add(MenuRiven.ESettings.Egapcloser);

            Rmenu.Add(MenuRiven.RSettings.Rcombo);
            Rmenu.Add(MenuRiven.RSettings.target);
            Rmenu.Add(MenuRiven.RSettings.R1);
            Rmenu.Add(MenuRiven.RSettings.R2);
            Rmenu.Add(MenuRiven.RSettings.ER1Q);
            Rmenu.Add(MenuRiven.RSettings.ER2);
            Rmenu.Add(MenuRiven.RSettings.R2AA);
            Rmenu.Add(MenuRiven.RSettings.R2Q);
            Rmenu.Add(MenuRiven.RSettings.tiamatR);

            range.Add(MenuRiven.RangeSettings.QRange);
            range.Add(MenuRiven.RangeSettings.WRange);
            range.Add(MenuRiven.RangeSettings.ERange);
            range.Add(MenuRiven.RangeSettings.R2Range);

            clear.Add(MenuRiven.ClearSettings.useQ);
            clear.Add(MenuRiven.ClearSettings.useW);
            clear.Add(MenuRiven.ClearSettings.useE);

            RivenMenu.Add(MenuRiven.BurstCombo).Permashow();
            RivenMenu.Add(Qmenu);
            RivenMenu.Add(Wmenu);
            RivenMenu.Add(Emenu);
            RivenMenu.Add(Rmenu);
            RivenMenu.Add(range);
            RivenMenu.Add(clear);

            RivenMenu.Attach();

            Game.OnUpdate += Game_OnUpdate;
            Orbwalker.OnAction += Orbwalker_OnAction;
            AIBaseClient.OnProcessSpellCast += AIBaseClient_OnProcessSpellCast;
            Interrupter.OnInterrupterSpell += Interrupter_OnInterrupterSpell;
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
            Dash.OnDash += Dash_OnDash;

            Drawing.OnDraw += Drawing_OnDraw;


        }


        private static Vector3 MovePos(Vector3 Startpos)
        {
            return ObjectManager.Player.Position.Extend(Startpos, -300);
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!Player.IsDead)
            {
                Drawing.DrawCircle(Player.Position, W.Range, System.Drawing.Color.Red);
                Drawing.DrawCircle(Player.Position, 400, System.Drawing.Color.Yellow);
                Drawing.DrawCircle(Player.Position, 900, System.Drawing.Color.Blue);
            }
        }

        private static void Dash_OnDash(AIBaseClient sender, Dash.DashArgs args)
        {
            if (sender.IsMe) return;
            if (W.IsReady() && sender.IsValidTarget(W.Range))
            {
                if (Player.CanUseItem((int)ItemId.Tiamat)
                    || Player.CanUseItem((int)ItemId.Ravenous_Hydra)
                    || Player.CanUseItem((int)ItemId.Tiamat_Melee_Only)
                    || Player.CanUseItem((int)ItemId.Ravenous_Hydra_Melee_Only)
                    )
                {
                    Player.UseItem((int)ItemId.Tiamat);
                    Player.UseItem((int)ItemId.Ravenous_Hydra_Melee_Only);
                    Player.UseItem((int)ItemId.Ravenous_Hydra);
                    Player.UseItem((int)ItemId.Tiamat_Melee_Only);
                    DelayAction.Add(1, () => { W.Cast(sender); });
                }
                else DelayAction.Add(1, () => { W.Cast(sender); });
            }
        }

        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserArgs args)
        {
            if (sender.IsMe) return;
            if (W.IsReady() && sender.IsValidTarget(W.Range))
            {
                if (Player.CanUseItem((int)ItemId.Tiamat)
                    || Player.CanUseItem((int)ItemId.Ravenous_Hydra)
                    || Player.CanUseItem((int)ItemId.Tiamat_Melee_Only)
                    || Player.CanUseItem((int)ItemId.Ravenous_Hydra_Melee_Only)
                    )
                {
                    Player.UseItem((int)ItemId.Tiamat);
                    Player.UseItem((int)ItemId.Ravenous_Hydra_Melee_Only);
                    Player.UseItem((int)ItemId.Ravenous_Hydra);
                    Player.UseItem((int)ItemId.Tiamat_Melee_Only);
                    DelayAction.Add(1, () => { W.Cast(sender); });
                }
                else DelayAction.Add(1, () => { W.Cast(sender); });
            }
        }

        private static void Interrupter_OnInterrupterSpell(AIHeroClient sender, Interrupter.InterruptSpellArgs args)
        {
            if (sender.IsMe) return;
            if (args.DangerLevel >= Interrupter.DangerLevel.Low && W.IsReady() && sender.IsValidTarget(W.Range))
            {
                if (Player.CanUseItem((int)ItemId.Tiamat)
                    || Player.CanUseItem((int)ItemId.Ravenous_Hydra)
                    || Player.CanUseItem((int)ItemId.Tiamat_Melee_Only)
                    || Player.CanUseItem((int)ItemId.Ravenous_Hydra_Melee_Only)
                    )
                {
                    Player.UseItem((int)ItemId.Tiamat);
                    Player.UseItem((int)ItemId.Ravenous_Hydra_Melee_Only);
                    Player.UseItem((int)ItemId.Ravenous_Hydra);
                    Player.UseItem((int)ItemId.Tiamat_Melee_Only);
                    DelayAction.Add(1, () => { W.Cast(sender); });
                }
                else DelayAction.Add(1, () => { W.Cast(sender); });
            }
        }
        private static void AIBaseClient_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name.Contains("ItemTiamatCleave") && W.IsReady())
                {
                    var targets = TargetSelector.GetTargets(W.Range);
                    if (targets.Any())
                        W.Cast();
                }

                if (args.SData.Name == "RivenTriCleave")
                {
                    lastQcast = Variables.TickCount;
                    Orbwalker.ResetAutoAttackTimer();
                }
            }
               

            if (!sender.IsMe)
            {
                if (args.SData.Name.Contains("TalonCutthroat"))
                {
                    if (args.Target.NetworkId == Player.NetworkId)
                    {
                        if (W.IsReady() && sender.IsValidTarget(W.Range)) W.Cast(sender.Position);
                    }
                }
                if (args.SData.Name.Contains("RenektonPreExecute"))
                {
                    if (args.Target.NetworkId == Player.NetworkId)
                    {
                        if (W.IsReady()) W.Cast(sender.Position);
                    }
                }
                if (args.SData.Name.Contains("GarenRPreCast"))
                {
                    if (args.Target.NetworkId == Player.NetworkId)
                    {
                        if (E.IsReady()) E.Cast(Player.Position.Extend(sender.Position, -300));
                    }
                }
                if (args.SData.Name.Contains("GarenQAttack"))
                {
                    if (args.Target.NetworkId == Player.NetworkId)
                    {
                        if (E.IsReady()) E.Cast(sender.Position);
                    }
                }
                if (args.SData.Name.Contains("XenZhaoThrust3"))
                {
                    if (args.Target.NetworkId == Player.NetworkId)
                    {
                        if (W.IsReady() && sender.IsValidTarget(W.Range)) W.Cast(sender.Position);
                    }
                }
                if (args.SData.Name.Contains("RengarQ"))
                {
                    if (args.Target.NetworkId == Player.NetworkId)
                    {
                        if (E.IsReady()) E.Cast(sender.Position);
                    }
                }
                if (args.SData.Name.Contains("RengarPassiveBuffDash"))
                {
                    if (args.Target.NetworkId == Player.NetworkId)
                    {
                        if (E.IsReady()) E.Cast(sender.Position);
                    }
                }
                if (args.SData.Name.Contains("RengarPassiveBuffDashAADummy"))
                {
                    if (args.Target.NetworkId == Player.NetworkId)
                    {
                        if (E.IsReady()) E.Cast(sender.Position);
                    }
                }
                if (args.SData.Name.Contains("TwitchEParticle"))
                {
                    if (args.Target.NetworkId == Player.NetworkId)
                    {
                        if (E.IsReady()) E.Cast(sender.Position);
                    }
                }
                if (args.SData.Name.Contains("FizzPiercingStrike"))
                {
                    if (args.Target.NetworkId == Player.NetworkId)
                    {
                        if (E.IsReady()) E.Cast(sender.Position);
                    }
                }
                if (args.SData.Name.Contains("HungeringStrike"))
                {
                    if (args.Target.NetworkId == Player.NetworkId)
                    {
                        if (E.IsReady()) E.Cast(sender.Position);
                    }
                }
                if (args.SData.Name.Contains("YasuoDash"))
                {
                    if (args.Target.NetworkId == Player.NetworkId)
                    {
                        if (E.IsReady()) E.Cast(sender.GetDashInfo().EndPos.Extend(Player.Position, -300));
                    }
                }
                if (args.SData.Name.Contains("KatarinaRTrigger"))
                {
                    if (args.Target.NetworkId == Player.NetworkId)
                    {
                        if (W.IsReady() && sender.IsValidTarget(W.Range)) W.Cast();
                        else if (E.IsReady()) E.Cast(Player.Position.Extend(sender.Position, -300));
                    }
                }
                if (args.SData.Name.Contains("YasuoDash"))
                {
                    if (args.Target.NetworkId == Player.NetworkId)
                    {
                        if (E.IsReady()) E.Cast(sender.GetDashInfo().EndPos.Extend(Player.Position, -300));
                    }
                }
                if (args.SData.Name.Contains("KatarinaE"))
                {
                    if (args.Target.NetworkId == Player.NetworkId)
                    {
                        if (W.IsReady()) W.Cast(sender.Position);
                    }
                }
                if (args.SData.Name.Contains("MonkeyKingQAttack"))
                {
                    if (args.Target.NetworkId == Player.NetworkId)
                    {
                        if (E.IsReady()) E.Cast(sender.Position);
                    }
                }
                if (args.SData.Name.Contains("MonkeyKingSpinToWin"))
                {
                    if (args.Target.NetworkId == Player.NetworkId)
                    {
                        if (E.IsReady()) E.Cast(sender.Position);
                        else if (W.IsReady()) W.Cast(sender.Position);
                    }
                }
                if (args.SData.Name.Contains("MonkeyKingQAttack"))
                {
                    if (args.Target.NetworkId == Player.NetworkId)
                    {
                        if (E.IsReady()) E.Cast(sender.Position);
                    }
                }
                if (args.SData.Name.Contains("MonkeyKingQAttack"))
                {
                    if (args.Target.NetworkId == Player.NetworkId)
                    {
                        if (E.IsReady()) E.Cast(sender.Position);
                    }
                }
                if (args.SData.Name.Contains("MonkeyKingQAttack"))
                {
                    if (args.Target.NetworkId == Player.NetworkId)
                    {
                        if (E.IsReady()) E.Cast(sender.Position);
                    }
                }
            }
        }

        public static void DoMove()
        {
            if (Orbwalker.ActiveMode == OrbwalkerMode.Combo || Orbwalker.ActiveMode == OrbwalkerMode.Harass || Orbwalker.ActiveMode == OrbwalkerMode.LaneClear || Orbwalker.ActiveMode == OrbwalkerMode.LastHit || MenuRiven.BurstCombo.Active)
            {
                if (Variables.TickCount - lastQcast >= 150 && Variables.TickCount - lastQcast <= 260 && MenuRiven.QSettings.FastQ.Enabled)
                {
                    Player.IssueOrder(GameObjectOrder.MoveTo, MovePos(Game.CursorPos));
                }
            }
        }
        private static void Orbwalker_OnAction(object sender, OrbwalkerActionArgs args)
        {
            DoMove();
            if (args.Type == OrbwalkerType.AfterAttack)
            {
                afteraa = true;
                if(!args.Target.IsAlly && args.Target.Position.IsBuilding() && Q.IsReady() && (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear || Orbwalker.ActiveMode == OrbwalkerMode.Harass))
                {
                    Q.Cast(args.Target.Position);
                    DoMove();
                }

                if ((args.Target.IsJungle() || args.Target.IsMinion()) && Orbwalker.ActiveMode == OrbwalkerMode.LaneClear)
                {                   
                    if (W.IsReady() && MenuRiven.ClearSettings.useW.Enabled && args.Target.IsValidTarget(W.Range))
                    {
                        W.Cast();
                    }
                    else
                    {                    
                        if (E.IsReady() && MenuRiven.ClearSettings.useE.Enabled && !args.Target.IsMinion())
                        {
                            E.Cast(args.Target.Position);
                        }
                        else
                        {
                            if (Q.IsReady() && MenuRiven.ClearSettings.useQ.Enabled)
                            {
                                Q.CastOnUnit(args.Target);
                            }
                        }
                    }                    
                }
            }
            else afteraa = false;
            if (args.Type == OrbwalkerType.BeforeAttack)
            {
                beforeaa = true;
            }
            else beforeaa = false;
            if (args.Type == OrbwalkerType.OnAttack)
            {
                onaa = true;
            }
            else onaa = false;
        }

        public static void Burst()
        {
            var target = TargetSelector.SelectedTarget;
            if (target != null && target.IsValidTarget())
            {
                if (Player.IsDashing() && HaveTiamat() && target.IsValidTarget(W.Range + 200))
                {
                    Player.UseItem((int)ItemId.Tiamat);
                    Player.UseItem((int)ItemId.Ravenous_Hydra_Melee_Only);
                    Player.UseItem((int)ItemId.Ravenous_Hydra);
                    Player.UseItem((int)ItemId.Tiamat_Melee_Only);
                }
                if (R.IsReady() && W.IsReady() && E.IsReady() && Player.Distance(target.Position) <= 250 + 70 + Player.AttackRange)
                {
                    E.Cast(target.Position);
                    R.DelayPosCast(target.Position, 1);
                    W.DelayPosCast(target.Position, 100);
                    if (Q.IsReady())
                    {
                        Q.DelayTargetCast(target, 205);
                    }
                }
                else if (R.IsReady() && E.IsReady() && W.IsReady() && Q.IsReady() &&
                         Player.Distance(target.Position) <= 400 + 70 + Player.AttackRange)
                {
                    E.Cast(target.Position);
                    R.DelayPosCast(target.Position, 1);
                    Q.DelayTargetCast(target, 150);
                    W.DelayPosCast(target.Position, 160);
                }
                else if (fl.IsReady()
                    && R.IsReady() && (Player.Distance(target.Position) <= 800) && E.IsReady() && W.IsReady())
                {
                    E.Cast(target.Position);
                    R.DelayPosCast(target.Position, 1);
                    DelayAction.Add(170, () => {
                        if (HaveTiamat())
                        {
                            Player.UseItem((int)ItemId.Tiamat);
                            Player.UseItem((int)ItemId.Ravenous_Hydra_Melee_Only);
                            Player.UseItem((int)ItemId.Ravenous_Hydra);
                            Player.UseItem((int)ItemId.Tiamat_Melee_Only);
                        }
                        W.DelayPosCast(target.Position, 10);
                        fl.DelayPosCast(target.Position, 20);
                    });

                    if (Q.IsReady())
                    {
                        Q.DelayTargetCast(target, 285);
                    }
                }
                else if (fl.IsReady()
                    && R.IsReady() && E.IsReady() && W.IsReady() && (Player.Distance(target.Position) <= 800))
                {
                    E.Cast(target.Position);
                    R.DelayPosCast(target.Position, 1);
                    DelayAction.Add(170, () => {
                        if (HaveTiamat())
                        {
                            Player.UseItem((int)ItemId.Tiamat);
                            Player.UseItem((int)ItemId.Ravenous_Hydra_Melee_Only);
                            Player.UseItem((int)ItemId.Ravenous_Hydra);
                            Player.UseItem((int)ItemId.Tiamat_Melee_Only);
                        }
                        W.DelayPosCast(target.Position, 10);
                        fl.DelayPosCast(target.Position, 20);
                    });
                    if (Q.IsReady())
                    {
                        Q.DelayTargetCast(target, 285);
                    }
                }
            }
        }
        private static void Game_OnUpdate(EventArgs args)
        {
            
            if (Player.HasBuff("RivenFengShuiEngine"))
            {
                W.Range = MenuRiven.RangeSettings.WRange.Value + 65;
                Q.Range = MenuRiven.RangeSettings.QRange.Value + 65;
            }
            else
            {
                W.Range = MenuRiven.RangeSettings.WRange.Value;
                Q.Range = MenuRiven.RangeSettings.QRange.Value;
            }
            
            E.Range = MenuRiven.RangeSettings.ERange.Value;
            R.Range = MenuRiven.RangeSettings.R2Range.Value;
            if (Player.IsDead) return;

            DoMove();


            

            if (MenuRiven.BurstCombo.Active && E.IsReady() && R.IsReady() && W.IsReady())
            {
                Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                Burst();

            }
            else
            {
                if (Orbwalker.ActiveMode == OrbwalkerMode.Combo)
                    combo();
            }           
        }

        public static void harass()
        {

        }
        public static void combo()
        {
            DoMove();

            var target = TargetSelector.GetTarget(400 + Player.GetRealAutoAttackRange(), DamageType.Physical);
            
            if(target != null)
            {
                /*if (Player.IsDashing() && HaveTiamat() && target.IsValidTarget(W.Range  + 200))
                {
                    Player.UseItem((int)ItemId.Tiamat);
                    Player.UseItem((int)ItemId.Ravenous_Hydra_Melee_Only);
                    Player.UseItem((int)ItemId.Ravenous_Hydra);
                    Player.UseItem((int)ItemId.Tiamat_Melee_Only);
                }*/

                if (R.IsReady() && target.HealthPercent <= MenuRiven.RSettings.target && MenuRiven.RSettings.Rcombo.Enabled)
                {
                    if (R1() && MenuRiven.RSettings.R1.Enabled)
                    {
                        if (Q.IsReady() && target.IsValidTarget(Q.Range) && MenuRiven.QSettings.Qcombo.Enabled)
                        {
                            if (afteraa)
                            {
                                Q.DelayTargetCast(target);
                            }
                            if (!target.InAutoAttackRange() && Variables.TickCount - lastQcast > 700)
                            {
                                Q.DelayTargetCast(target);
                            }
                        }

                        if (W.IsReady() && target.IsValidTarget(W.Range) && MenuRiven.WSettings.Wcombo.Enabled)
                        {
                            if (!Orbwalker.CanAttack())
                            {
                                W.DelayPosCast(target.Position);
                            }

                            if (Variables.TickCount - lastQcast > 500 && Player.IsDashing())
                            {
                                W.DelayPosCast(target.Position);
                            }

                            if (target.InAutoAttackRange() && afteraa)
                            {
                                W.DelayPosCast(target.Position);
                            }

                            if (target.IsDashing() && target.IsValidTarget(W.Range))
                            {
                                W.DelayPosCast(target.Position);
                            }
                        }

                        if (E.IsReady() && target.IsValidTarget(E.Range))
                        {
                            if (W.IsReady() ? !target.IsValidTarget(W.Range) : !target.InAutoAttackRange())
                            {
                                E.DelayPosCast(target.Position);
                            }
                        }

                        if (E.IsReady() && Q.IsReady())
                        {
                            E.Cast(target.Position);
                            R.DelayPosCast(target.Position, 1);
                            Q.DelayTargetCast(target, 200);
                        }

                        if(!E.IsReady() && !Q.IsReady())
                        {
                            R.Cast(target.Position);
                        }
                    }
                    if (R2() && MenuRiven.RSettings.R2.Enabled)
                    {
                        if (Q.IsReady() && target.IsValidTarget(Q.Range) && MenuRiven.QSettings.Qcombo.Enabled)
                        {
                            if (afteraa)
                            {
                                Q.DelayTargetCast(target);
                            }
                            if (!target.InAutoAttackRange() && Variables.TickCount - lastQcast > 700)
                            {
                                Q.DelayTargetCast(target);
                            }
                        }

                        if (W.IsReady() && target.IsValidTarget(W.Range) && MenuRiven.WSettings.Wcombo.Enabled)
                        {
                            if (!Orbwalker.CanAttack())
                            {
                                W.DelayPosCast(target.Position);
                            }

                            if (Variables.TickCount - lastQcast > 500 && Player.IsDashing())
                            {
                                W.DelayPosCast(target.Position);
                            }

                            if (target.InAutoAttackRange() && afteraa)
                            {
                                W.DelayPosCast(target.Position);
                            }

                            if (target.IsDashing() && target.IsValidTarget(W.Range))
                            {
                                W.DelayPosCast(target.Position);
                            }
                        }

                        if (E.IsReady() && target.IsValidTarget(E.Range))
                        {
                            if (W.IsReady() ? !target.IsValidTarget(W.Range) : !target.InAutoAttackRange())
                            {
                                E.DelayPosCast(target.Position);
                            }
                        }

                        if (target.Health <= R.GetDamage(target) && R.GetPrediction(target).Hitchance >= HitChance.High)
                        {
                            R.DelayPosCast(R.GetPrediction(target).CastPosition);
                        }

                        if (Q.IsReady() && target.InAutoAttackRange())
                        {
                            if (target.Health <= R.GetDamage(target) + Q.GetDamage(target) + Player.GetAutoAttackDamage(target))
                            {
                                if (E.IsReady())
                                    E.Cast(target.Position);
                                R.DelayPosCast(R.GetPrediction(target).CastPosition, 1);
                                Q.DelayTargetCast(target, 150);
                            }
                        }

                        if(target.InAutoAttackRange() && beforeaa && !Q.IsReady())
                        {
                            if (target.Health <= R.GetDamage(target) + Player.GetAutoAttackDamage(target))
                            {
                                R.DelayPosCast(R.GetPrediction(target).CastPosition);
                            }
                        }

                        if (target.IsValidTarget(W.Range) && W.IsReady())
                        {
                            if (target.Health <= R.GetDamage(target) + W.GetDamage(target))
                            {
                                R.DelayPosCast(R.GetPrediction(target).CastPosition);
                            }
                        }
                    }
                }
                else
                {
                    if (Q.IsReady() && target.IsValidTarget(Q.Range) && MenuRiven.QSettings.Qcombo.Enabled)
                    {
                        if (afteraa)
                        {
                            Q.DelayTargetCast(target);
                        }
                        if (!target.InAutoAttackRange() && Variables.TickCount - lastQcast > 700)
                        {
                            Q.DelayTargetCast(target);
                        }
                    }

                    if(W.IsReady() && target.IsValidTarget(W.Range) && MenuRiven.WSettings.Wcombo.Enabled)
                    {
                        if (!Orbwalker.CanAttack())
                        {
                            W.DelayPosCast(target.Position);
                        }

                        if(Variables.TickCount - lastQcast > 500 && Player.IsDashing())
                        {
                            W.DelayPosCast(target.Position);
                        }

                        if(target.InAutoAttackRange() && afteraa)
                        {
                            W.DelayPosCast(target.Position);
                        }

                        if (target.IsDashing() && target.IsValidTarget(W.Range))
                        {
                            W.DelayPosCast(target.Position);
                        }
                    }

                    if(E.IsReady() && target.IsValidTarget(E.Range))
                    {
                        if (W.IsReady() ? !target.IsValidTarget(W.Range) : !target.InAutoAttackRange())
                        {
                            E.DelayPosCast(target.Position);
                        }
                    }
                }
            }
        }

        private static bool HaveTiamat()
        {
            return Player.CanUseItem((int)ItemId.Tiamat)
                    || Player.CanUseItem((int)ItemId.Ravenous_Hydra)
                    || Player.CanUseItem((int)ItemId.Tiamat_Melee_Only)
                    || Player.CanUseItem((int)ItemId.Ravenous_Hydra_Melee_Only);
        }       
    }
}
