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

namespace DominationAIO.Champions
{
    public class RMenu
    {
        //simple combo
        public static MenuBool useq = new MenuBool("useq", "Use Combo Q");
        public static MenuBool usew = new MenuBool("usew", "Use Combo W");
        public static MenuBool usee = new MenuBool("usee", "Use Combo E");
        public static MenuBool user = new MenuBool("user", "Use Combo R");
        //logic combo
        public static MenuSlider rh = new MenuSlider("rh", "R target heath <= x%", 60);
        public static MenuKeyBind flash = new MenuKeyBind("flash", "Use Combo Flash", System.Windows.Forms.Keys.A, KeyBindType.Toggle) { Active = false };
        public static MenuBool useqgap = new MenuBool("useqgap", "Use Q gap");
        public static MenuSlider qgap = new MenuSlider("qgap", "Q gapcloser if target distance player <= ", (int)ObjectManager.Player.GetRealAutoAttackRange() + 200, (int)ObjectManager.Player.GetRealAutoAttackRange(), (int)ObjectManager.Player.GetRealAutoAttackRange() + 400);
        public static MenuSlider egap = new MenuSlider("egap", "E gap distance", 400, 200, 700);
        public static MenuKeyBind fastcombo = new MenuKeyBind("fastcombo", "Fast Combo", System.Windows.Forms.Keys.A, KeyBindType.Toggle);
        public static MenuBool tiamatW = new MenuBool("tiamatW", "Tiamat W");
        public static MenuBool RQ = new MenuBool("RQ", "R2-Q");
        public static MenuBool ER1Q = new MenuBool("ER1Q", "E-R1-Q");

        //simple harass
        public static MenuBool useqh = new MenuBool("useqh", "Use Harass Q");
        public static MenuBool usewh = new MenuBool("usewh", "Use Harass W");
        public static MenuBool useeh = new MenuBool("useeh", "Use Harass E");

        //simple farm
        public static MenuBool useqf = new MenuBool("useqf", "Use Farm Q");
        public static MenuBool usewf = new MenuBool("usewf", "Use Farm W");
        public static MenuBool useef = new MenuBool("useef", "Use Farm E");

        //simple events
        public static MenuBool itr = new MenuBool("itr", "Interrupter with W");
        public static MenuBool antigap = new MenuBool("antigap", "Anti Gap with W");
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
            return R.Name == "RivenIzunaBlade" && R.IsReady();
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
            R.SetSkillshot(0.25f, 45, 1600, false, false, SkillshotType.Cone); 

            RivenMenu = new Menu("RivenMenu", "FunnySlayer Riven", true);
            var combom = new Menu("combom", "Combo Menu");
            combom.Add(RMenu.useq);
            combom.Add(RMenu.usew);
            combom.Add(RMenu.usee);
            combom.Add(RMenu.user);
            combom.Add(RMenu.rh).Permashow(true, "R Combo <= x%", SharpDX.Color.DarkGreen);

            combom.Add(RMenu.flash).Permashow(true, "Flash In Combo", SharpDX.Color.Yellow);
            combom.Add(RMenu.useqgap);
            combom.Add(RMenu.qgap);
            combom.Add(RMenu.egap);
            combom.Add(RMenu.fastcombo).Permashow(true, "Fast Combo in level 3", SharpDX.Color.Red);
            combom.Add(RMenu.tiamatW);
            combom.Add(RMenu.RQ);
            combom.Add(RMenu.ER1Q);
            RivenMenu.Add(combom);

            var harassm = new Menu("harassm", "Harass Menu");
            harassm.Add(RMenu.useqh);
            harassm.Add(RMenu.usewh);
            harassm.Add(RMenu.useeh);
            RivenMenu.Add(harassm);

            var farmm = new Menu("farmm", "Farm Menu");
            farmm.Add(RMenu.useqf);
            farmm.Add(RMenu.usewf);
            farmm.Add(RMenu.useef);
            RivenMenu.Add(farmm);

            var miscm = new Menu("miscm", "Misc Menu");
            miscm.Add(RMenu.itr);
            miscm.Add(RMenu.antigap);
            RivenMenu.Add(miscm);

            RivenMenu.Attach();

            Game.OnUpdate += Game_OnUpdate;
            Orbwalker.OnAction += Orbwalker_OnAction;
            AIBaseClient.OnProcessSpellCast += AIBaseClient_OnProcessSpellCast;
            Interrupter.OnInterrupterSpell += Interrupter_OnInterrupterSpell;
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
            Dash.OnDash += Dash_OnDash;

            Drawing.OnDraw += Drawing_OnDraw;

            Orbwalker.OnAction += FarmAction;
        }        

        private static void Drawing_OnDraw(EventArgs args)
        {
            if(!Player.IsDead)
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
            if(W.IsReady() && sender.IsValidTarget(W.Range))
            {
                if(Player.CanUseItem((int)ItemId.Tiamat) 
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
                }else DelayAction.Add(1, () => { W.Cast(sender); });
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
            SpellData sdataw = Player.Spellbook.GetSpell(SpellSlot.W).SData;
            var spell = args.SData;
            var targets = GameObjects.EnemyHeroes.Where(heroes => heroes.IsValidTarget(Player.GetRealAutoAttackRange())); 
            if (sender.IsMe)
            {
                if(spell.Name == sdataw.Name && Orbwalker.ActiveMode == OrbwalkerMode.Combo && Q.SpellIsReadyAndActive(RMenu.useq))
                {
                    if(!Player.HasBuff("riventricleavesoundtwo"))
                    Q.Cast(Game.CursorPos);
                    DelayAction.Add(1, () => { return; });
                }
                if (spell.Name.Contains("ItemTiamatCleave"))
                {
                    if(Orbwalker.ActiveMode == OrbwalkerMode.Combo || Orbwalker.ActiveMode == OrbwalkerMode.Harass)
                    {
                        if(targets.Count() > 0 || targets != null)
                            W.Cast();
                    }
                    lastTiamat = Variables.GameTimeTickCount;
                }
                if (spell.Name.Contains("RivenTriCleave"))
                {
                    Orbwalker.ResetAutoAttackTimer();
                    lastQcast = Variables.GameTimeTickCount;
                }
                if(spell.Name == "RivenTriCleave")
                {
                    Orbwalker.ResetAutoAttackTimer();
                    lastQcast = Variables.GameTimeTickCount;
                }
                if (spell.Name.Contains("RivenFient"))
                {
                    lastEcast = Variables.GameTimeTickCount;
                }
                if (spell.Name == ("RivenFient"))
                {
                    lastEcast = Variables.GameTimeTickCount;
                }
            }
        }

        private static void Orbwalker_OnAction(object sender, OrbwalkerActionArgs args)
        {
            int time = 340;
            if (Orbwalker.AttackSpeed < 0.7f)
            {
                time = 340;
            }
            else
            {
                if (Orbwalker.AttackSpeed < 0.8f)
                {
                    time = 320;
                }
                if (Orbwalker.AttackSpeed > 0.9f && Orbwalker.AttackSpeed < 1.2f)
                {
                    time = 295;
                }
                if (Orbwalker.AttackSpeed > 1.2f)
                {
                    time = 245;
                }
            }
            if (args.Type == OrbwalkerType.AfterAttack)
            {
                if (args.Type == OrbwalkerType.AfterAttack)
                {
                    afteraa = true;
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
                //DelayAction.Add(time, () => { onaa = false; afteraa = true; });
            }
            else onaa = false;
            /*if (args.Type == OrbwalkerType.AfterAttack)
                        {
                            afteraa = true;
                        }
                        else afteraa = false;

             if (args.Type == OrbwalkerType.OnAttack)
                        {
                            onaa = true;
                        }
                        else onaa = false;*/
        }

        private static void FarmAction(object sender, OrbwalkerActionArgs args)
        {
            if(Orbwalker.ActiveMode == OrbwalkerMode.LaneClear)
            {
                if (args.Type == OrbwalkerType.AfterAttack)
                {
                    if(args.Target.IsJungle())
                    {
                        if(Q.IsReady() && RMenu.useqf.Enabled)
                        {
                            Q.CastOnUnit(args.Target);
                        }
                        if(W.IsReady() && RMenu.usewf.Enabled && args.Target.IsValidTarget(W.Range))
                        {
                            W.Cast();
                        }
                        if (E.IsReady() && RMenu.useef.Enabled)
                        {
                            E.Cast(args.Target.Position);
                        }
                    }
                }
            }           
        }

        private static void Game_OnUpdate(EventArgs args)
        {        
            #region old
            /*if (RMenu.Qdelay.Enabled)
            {
                if(Orbwalker.AttackSpeed < 0.7f)
                {
                    RMenu.Qdelayslider.Value = 300;
                }
                else
                {
                    if(Orbwalker.AttackSpeed < 1f)
                    {
                        RMenu.Qdelayslider.Value = 280;
                    }
                    if (Orbwalker.AttackSpeed > 1f && Orbwalker.AttackSpeed < 1.3f)
                    {
                        RMenu.Qdelayslider.Value = 250;
                    }
                    if (Orbwalker.AttackSpeed > 1.3f)
                    {
                        RMenu.Qdelayslider.Value = 200;
                    }
                }
            }*/

            #endregion
            var target = GameObjects.EnemyHeroes.FirstOrDefault(i => i.IsValidTarget(900) && i.Health < R.GetDamage(i) && i.DistanceToPlayer() < R.GetPrediction(i, false, -1, CollisionObjects.YasuoWall).CastPosition.DistanceToPlayer());          
            if (!Player.IsDead)
            {
                switch (Orbwalker.ActiveMode)
                {
                    case OrbwalkerMode.Combo:
                        combo();
                        if(RMenu.user.Enabled && R2())
                        {
                            if(!onaa && R.GetPrediction(target, false, -1, CollisionObjects.YasuoWall).Hitchance >= HitChance.High)
                            {
                                if(target.Health <= R.GetDamage(target))
                                R.Cast(R.GetPrediction(target, false, -1, CollisionObjects.YasuoWall).CastPosition);
                            }
                        }
                        break;
                    case OrbwalkerMode.Harass:
                        break;
                    case OrbwalkerMode.LaneClear:
                        break;
                    case OrbwalkerMode.LastHit:
                        break;
                }
            }
        }

        public static void harass()
        {

        }
        public static void combo()
        {
            var target = TargetSelector.SelectedTarget;
            if(target == null || !target.IsValidTarget(700))
            target = TargetSelector.GetTarget(1300);

            var qready = Q.IsReady();
            var wready = W.IsReady();
            var eready = E.IsReady();
            var rready = R.IsReady();

            var qenabled = RMenu.useq.Enabled;
            var wenabled = RMenu.usew.Enabled;
            var eenabled = RMenu.usee.Enabled;
            var renabled = RMenu.user.Enabled;

            var rheath = RMenu.rh.Value;

            var checkqgap = RMenu.useqgap.Enabled ? RMenu.qgap.Value : 0;
            var checkegap = RMenu.egap.Value;

            var rpreddmg = R.GetPrediction(target, false, -1, CollisionObjects.YasuoWall);

            if (!qenabled || !wenabled || !eenabled || !renabled) return;

            if (target == null) return;

            #region normal
            if(Player.Level <= 3 || !RMenu.fastcombo.Active)
            {
                if (target.Health < 40)
                {
                    if (qready && !wready && !eready)
                    {
                        if(target.IsValidTarget(checkqgap) && target.DistanceToPlayer() >= Player.GetRealAutoAttackRange())
                        {
                            if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                                Q.Cast(target, true);
                        }else
                        {
                            if(target.DistanceToPlayer() <= Player.GetRealAutoAttackRange())
                            {
                                if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                                if (afteraa) Q.Cast(target, true);
                            }
                        }
                    }
                    if (!qready && wready && !eready)
                    {
                        if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                        if (afteraa && target.IsValidTarget(W.Range)) W.Cast(target);
                        if (W.IsInRange(target) && target.DistanceToPlayer() > Player.GetRealAutoAttackRange() || target.IsFleeing || !target.IsFacing(Player)) W.Cast(target);
                    }
                    if (!qready && !wready && eready)
                    {
                        if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                        if (afteraa && target.IsValidTarget(W.Range)) E.Cast(target);
                        if (!target.IsValidTarget(W.Range) && target.IsValidTarget(checkegap)) E.Cast(target, true);
                    }
                    if (qready && wready)
                    {
                        if (target.IsValidTarget(checkqgap) && !onaa && !beforeaa)
                            if (target.DistanceToPlayer() < W.Range)
                            {
                                if (Player.HasBuff("riventricleavesoundtwo"))
                                {
                                    if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                                    if (afteraa || target.IsFleeing || !target.IsFacing(Player)) W.Cast(target);
                                }
                                else
                                {
                                    if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                                    if (afteraa) Q.Cast(target);
                                }
                            }
                            else
                            {
                                if (target.IsValidTarget(checkqgap)) { Q.Cast(target, true); }
                            }
                    }
                    if (qready && eready)
                    {
                        if(target.IsValidTarget(E.Range + 200) && !onaa && !beforeaa)
                        {
                            if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                            if (Player.HasBuff("riventricleavesoundtwo"))
                            {
                                Geometry.Circle circle = new Geometry.Circle(target.Position, Player.GetRealAutoAttackRange());
                                foreach (var c in circle.Points.Where(c => c.DistanceToPlayer() <= E.Range))
                                {
                                    if (c != Vector2.Zero)
                                        E.Cast(c);
                                }
                            }
                            if (afteraa) E.Cast(target);
                        }
                    }
                    if (eready && wready)
                    {
                        if(target.IsValidTarget(checkegap) && !onaa && !beforeaa)
                        {
                            if(target.DistanceToPlayer() > Player.GetRealAutoAttackRange())
                            {
                                if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                                E.Cast(target);
                            }else
                            {
                                if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                                if (afteraa || target.IsFleeing || !target.IsFacing(Player)) W.Cast(target);
                            }
                        }
                    }
                }
                else
                {
                    if (qready && !wready && !eready)
                    {
                        if (target.DistanceToPlayer() < checkqgap && target.DistanceToPlayer() > Player.GetRealAutoAttackRange())
                        {
                            if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                            if (!onaa && !beforeaa) Q.Cast(target, true);
                        }
                        if (target.DistanceToPlayer() < Player.GetRealAutoAttackRange())
                        {
                            if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                            if (!onaa && !beforeaa) if (afteraa)
                                    Q.Cast(target, true);
                        }
                    }
                    if (!qready && wready && !eready)
                    {
                        if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                        if (afteraa || target.IsFleeing || !target.IsFacing(Player) && target.DistanceToPlayer() <= W.Range)
                            W.Cast(target);
                    }
                    if (!qready && !wready && eready)
                    {
                        if (!onaa && !beforeaa) if (target.DistanceToPlayer() < checkegap && target.DistanceToPlayer() > Player.GetRealAutoAttackRange())
                                E.Cast(target.Position);
                            else if (afteraa && target.DistanceToPlayer() < Player.GetRealAutoAttackRange()) E.Cast(target);
                    }
                    if (qready && wready)
                    {
                        if (target.IsValidTarget(checkqgap) && !onaa && !beforeaa)
                            if (target.DistanceToPlayer() < W.Range)
                            {
                                if (Player.HasBuff("riventricleavesoundtwo"))
                                {
                                    if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                                    if (afteraa || target.IsFleeing || !target.IsFacing(Player)) W.Cast(target);
                                }
                                else
                                {
                                    if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                                    if (afteraa) Q.Cast(target);
                                }
                            }
                            else
                            {
                                if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                                if (target.IsValidTarget(checkqgap)) { Q.Cast(target, true); }
                            }
                    }
                    if (qready && eready)
                    {
                        if (target.IsValidTarget(checkegap) && !onaa && !beforeaa)
                        {
                            if (target.DistanceToPlayer() < Player.GetRealAutoAttackRange())
                            {
                                if (!Player.HasBuff("riventricleavesoundtwo"))
                                {
                                    if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                                    if (afteraa)
                                        Q.Cast(target, true);
                                }
                                else
                                {
                                    if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                                    if (afteraa)
                                        E.Cast(target, true);
                                }
                            }
                        }
                    }
                    if (eready && wready)
                    {
                        if (target.IsValidTarget(checkegap) && !onaa && !beforeaa)
                        {
                            if (target.DistanceToPlayer() < Player.GetRealAutoAttackRange())
                            {
                                if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                                if (afteraa || target.IsFleeing || !target.IsFacing(Player)) W.Cast(target);
                                if (afteraa) E.Cast(target);
                            }
                        }
                    }                   
                }               
            }
            else
            {
                if (qready && !wready && !eready && qenabled && ((!rready || R2()) || !renabled))
                {
                    if (target.DistanceToPlayer() <= Player.GetRealAutoAttackRange() + 100)
                    {
                        if (onaa) return;
                        if (afteraa)
                        {
                            if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                            Q.Cast(target);
                            DelayAction.Add(10, () => { return; });
                        }
                    }
                    else
                    {
                        if (target.DistanceToPlayer() <= checkqgap + Player.GetRealAutoAttackRange() && target.DistanceToPlayer() >= Player.GetRealAutoAttackRange() + 100)
                        {
                            if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                            Q.Cast(target);
                            DelayAction.Add(10, () => { return; });
                        }
                    }
                }
                if (!qready && wready && !eready && wenabled && ((!rready || R2()) || !renabled))
                {
                    if (afteraa || target.IsFleeing || !target.IsFacing(Player))
                    {
                        if (target.IsValidTarget(W.Range))
                        {
                            if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                            W.Cast(target);
                        }
                        DelayAction.Add(10, () => { return; });
                    }
                }
                if (!qready && !wready && eready && eenabled && ((!rready || R2()) || !renabled))
                {
                    if (onaa) return;
                    if (target.DistanceToPlayer() >= Player.GetRealAutoAttackRange() && !afteraa && !beforeaa)
                    {
                        if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                        Geometry.Circle circle = new Geometry.Circle(target.Position, Player.GetRealAutoAttackRange());
                        foreach (var c in circle.Points.Where(c => c.DistanceToPlayer() < E.Range))
                        {
                            if (c != Vector2.Zero)
                                E.Cast(c);
                        }
                        if(target.IsValidTarget(checkegap))
                        E.Cast(target);
                        DelayAction.Add(10, () => { return; });
                    }
                    if (afteraa)
                    {
                        if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                        if (Player.HasBuff("riventricleavesoundtwo"))
                        {
                            Geometry.Circle circle = new Geometry.Circle(target.Position, Player.GetRealAutoAttackRange() - 20);
                            foreach (var c in circle.Points.Where(c => c.DistanceToPlayer() < E.Range))
                            {
                                E.Cast(c);
                            }
                            DelayAction.Add(10, () => { return; });
                        }
                    }
                }
                if (rready && renabled)
                {
                    if (R1() && target.DistanceToPlayer() < 700)
                    {
                        if(target.HealthPercent <= rheath)
                        {
                            if(Variables.GameTimeTickCount - lastEcast < 300 && Variables.GameTimeTickCount - lastEcast > 99)
                            {
                                R.Cast(target);
                                DelayAction.Add(5, () => { Q.Cast(target); });
                            }
                        }
                        if (target.HealthPercent < 40)
                        {
                            DelayAction.Add(1, () => { R.Cast(rpreddmg.CastPosition); });
                        }
                        if (target.HealthPercent <= rheath)
                        {
                            if (eready)
                            {
                                if (qready)
                                {
                                        E.Cast(target.Position);
                                    DelayAction.Add(110 - Game.Ping, () => { R.Cast(target); });
                                    DelayAction.Add(120 - Game.Ping, () => { Q.Cast(target); });
                                }
                            }
                            else
                            {
                                if (qready)
                                {
                                        E.Cast(target.Position);
                                    DelayAction.Add(110 - Game.Ping, () => { R.Cast(target); });
                                    DelayAction.Add(120 - Game.Ping, () => { Q.Cast(target); });
                                }
                            }
                        }
                        if (Player.CountEnemyHeroesInRange(700) > 1)
                        {
                            if (eready)
                            {
                                if (qready)
                                {
                                    E.Cast(target.Position);
                                    DelayAction.Add(110 - Game.Ping, () => { R.Cast(target); });
                                    DelayAction.Add(120 - Game.Ping, () => { Q.Cast(target); });
                                }
                            }
                            else
                            {
                                if (qready)
                                {
                                    E.Cast(target.Position);
                                    DelayAction.Add(110 - Game.Ping, () => { R.Cast(target); });
                                    DelayAction.Add(120 - Game.Ping, () => { Q.Cast(target); });
                                }
                            }
                        }
                    }
                    if (R2())
                    {
                        if (target.IsValidTarget(900))
                        {
                            if (target.Health < R.GetDamage(target) + Player.TotalAttackDamage)
                            {
                                if (rpreddmg.Hitchance >= HitChance.High)
                                {
                                    R.Cast(rpreddmg.CastPosition);
                                }
                            }
                            if (beforeaa && target.Health + target.BonusHealth <= R.GetDamage(target) + Player.TotalAttackDamage * 2)
                            {
                                if (rpreddmg.Hitchance >= HitChance.High && rpreddmg.CastPosition != Vector3.Zero)
                                {
                                    R.Cast(rpreddmg.CastPosition);
                                    DelayAction.Add(1, () => { Orbwalker.Attack(target); });
                                }
                            }
                            if (HaveTiamat())
                            {
                                if (afteraa)
                                {
                                    Player.UseItem((int)ItemId.Tiamat);
                                    Player.UseItem((int)ItemId.Ravenous_Hydra_Melee_Only);
                                    Player.UseItem((int)ItemId.Ravenous_Hydra);
                                    Player.UseItem((int)ItemId.Tiamat_Melee_Only);
                                }
                                if (target.Health <= R.GetDamage(target) + Q.GetDamage(target) * 2 + W.GetDamage(target))
                                    if (qready || wready || eready)
                                    {
                                        if (qready && wready && eready)
                                        {
                                            if (rpreddmg.Hitchance >= HitChance.High && rpreddmg.CastPosition != Vector3.Zero)
                                            {
                                                //DelayAction.Add(1, () => { W.Cast(rpreddmg.CastPosition); });
                                                DelayAction.Add(110, () => { R.Cast(rpreddmg.CastPosition); });
                                                DelayAction.Add(120, () => { Q.Cast(target); });
                                            }
                                        }
                                        else
                                        {
                                            if (qready && wready)
                                            {
                                                //DelayAction.Add(1, () => { W.Cast(rpreddmg.CastPosition); });
                                                DelayAction.Add(110, () => { R.Cast(rpreddmg.CastPosition); });
                                                DelayAction.Add(120, () => { Q.Cast(target); });
                                            }
                                            else
                                            {
                                                if (eready && qready)
                                                {
                                                    E.Cast(Player.Position.Extend(rpreddmg.CastPosition, -10));
                                                    DelayAction.Add(110, () => { R.Cast(rpreddmg.CastPosition); });
                                                    DelayAction.Add(120, () => { Q.Cast(target); });
                                                }
                                            }
                                            if (qready && eready)
                                            {
                                                DelayAction.Add(110, () => { R.Cast(rpreddmg.CastPosition); });
                                                DelayAction.Add(120, () => { Q.Cast(target); });
                                            }
                                            else
                                            {
                                                if (wready)
                                                {
                                                    //DelayAction.Add(1, () => { W.Cast(rpreddmg.CastPosition); });
                                                    DelayAction.Add(110, () => { R.Cast(rpreddmg.CastPosition); });
                                                }
                                            }
                                            if (wready && eready)
                                            {
                                                //DelayAction.Add(1, () => { W.Cast(rpreddmg.CastPosition); });
                                                DelayAction.Add(110, () => { R.Cast(rpreddmg.CastPosition); });
                                            }
                                            else
                                            {
                                                if (qready)
                                                {
                                                    DelayAction.Add(110, () => { R.Cast(rpreddmg.CastPosition); });
                                                    DelayAction.Add(120, () => { Q.Cast(target); });
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var rpredheath = R.GetHealthPrediction(target);
                                        if (target.Health - rpredheath <= R.GetDamage(target))
                                        {
                                            if (rpreddmg.Hitchance >= HitChance.High)
                                            {
                                                R.Cast(rpreddmg.CastPosition);
                                            }
                                        }
                                        if (target.Health <= R.GetDamage(target))
                                        {
                                            if (rpreddmg.Hitchance >= HitChance.High)
                                            {
                                                R.Cast(rpreddmg.CastPosition);
                                            }
                                        }
                                    }
                            }
                            else
                            {
                                if (target.Health <= R.GetDamage(target) + Q.GetDamage(target) * 2 + W.GetDamage(target))
                                    if (qready || wready || eready)
                                    {
                                        if (qready && wready && eready)
                                        {
                                            if (rpreddmg.Hitchance >= HitChance.High && rpreddmg.CastPosition != Vector3.Zero)
                                            {
                                                //DelayAction.Add(1, () => { W.Cast(rpreddmg.CastPosition); });
                                                DelayAction.Add(110, () => { R.Cast(rpreddmg.CastPosition); });
                                                DelayAction.Add(120, () => { Q.Cast(target); });
                                            }
                                        }
                                        else
                                        {
                                            if (qready && wready)
                                            {
                                                //DelayAction.Add(1, () => { W.Cast(rpreddmg.CastPosition); });
                                                DelayAction.Add(110, () => { R.Cast(rpreddmg.CastPosition); });
                                                DelayAction.Add(120, () => { Q.Cast(target); });
                                            }
                                            else
                                            {
                                                if (eready)
                                                {

                                                }
                                            }
                                            if (qready && eready)
                                            {
                                                DelayAction.Add(110, () => { R.Cast(rpreddmg.CastPosition); });
                                                DelayAction.Add(120, () => { Q.Cast(target); });
                                            }
                                            else
                                            {
                                                if (wready)
                                                {
                                                    //DelayAction.Add(1, () => { W.Cast(rpreddmg.CastPosition); });
                                                    DelayAction.Add(110, () => { R.Cast(rpreddmg.CastPosition); });
                                                }
                                            }
                                            if (wready && eready)
                                            {
                                                //DelayAction.Add(1, () => { W.Cast(rpreddmg.CastPosition); });
                                                DelayAction.Add(110, () => { R.Cast(rpreddmg.CastPosition); });
                                            }
                                            else
                                            {
                                                if (qready)
                                                {
                                                    DelayAction.Add(1, () => { R.Cast(rpreddmg.CastPosition); });
                                                    DelayAction.Add(110, () => { Q.Cast(target); });
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var rpredheath = R.GetHealthPrediction(target);
                                        if (target.Health - rpredheath <= R.GetDamage(target))
                                        {
                                            if (rpreddmg.Hitchance >= HitChance.High)
                                            {
                                                R.Cast(rpreddmg.CastPosition);
                                            }
                                        }
                                        if (target.Health <= R.GetDamage(target))
                                        {
                                            if (rpreddmg.Hitchance >= HitChance.High)
                                            {
                                                R.Cast(rpreddmg.CastPosition);
                                            }
                                        }
                                    }
                            }
                        }
                        if (target.IsValidTarget(E.Range + Q.Range))
                        {
                            if (target.Health <= R.GetDamage(target) + Q.GetDamage(target) * 2)
                            {
                                if (qready && eready)
                                {
                                    DelayAction.Add(1, () => { E.Cast(rpreddmg.CastPosition); });
                                    DelayAction.Add(110, () => { R.Cast(rpreddmg.CastPosition); });
                                    DelayAction.Add(120, () => { Q.Cast(target); });
                                }
                                else
                                {
                                    if (qready)
                                    {
                                        DelayAction.Add(110, () => { R.Cast(rpreddmg.CastPosition); });
                                        DelayAction.Add(120, () => { Q.Cast(target); });
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (target.Health <= R.GetDamage(target))
                            {
                                if (rpreddmg.Hitchance >= HitChance.High)
                                {
                                    R.Cast(rpreddmg.CastPosition);
                                }
                            }
                        }
                    }
                }
                
                if (RMenu.flash.Active && flash.IsReady() && target.DistanceToPlayer() >= 425 && target.IsValidTarget(950))
                {
                    if (target.IsValidTarget(1000))
                    {
                        var cancancelanimation = false;
                        if (wready && qready && eready) cancancelanimation = true; else cancancelanimation = false;
                        if (Player.IsDashing() && target.Distance(Player) < W.Range) W.Cast(target);
                        if(R1())
                        {
                            if(cancancelanimation)
                            {
                                E.Cast(target);
                                DelayAction.Add(110, () =>
                                {
                                    R.Cast(target);
                                }
                                );
                                DelayAction.Add(140, () =>
                                {
                                    fl.Cast(target.Position);
                                }
                                );
                                DelayAction.Add(180, () =>
                                {
                                    Q.Cast(target);
                                }
                                );
                            }

                            {
                                if(Variables.GameTimeTickCount - lastEcast < 99)
                                {
                                    if(wready || qready)
                                    {
                                        R.Cast(target);
                                        DelayAction.Add(80, () =>
                                        {
                                            fl.Cast(target.Position);
                                        }
                                        );
                                        DelayAction.Add(120, () =>
                                        {
                                            Q.Cast(target);
                                        }
                                        );
                                    }
                                }
                            }
                        }
                        if(R2())
                        {
                            if (cancancelanimation)
                            {
                                E.Cast(target);
                                DelayAction.Add(130, () =>
                                {
                                    R.Cast(target);
                                }
                                );
                                DelayAction.Add(460, () =>
                                {
                                    fl.Cast(target.Position);
                                }
                                );
                                DelayAction.Add(490, () =>
                                {
                                    Q.Cast(target);
                                }
                                );
                            }

                            {
                                if (Variables.GameTimeTickCount - lastEcast < 99)
                                {
                                    if (wready || qready)
                                    {
                                        R.Cast(target);
                                        DelayAction.Add(330, () =>
                                        {
                                            fl.Cast(target.Position);
                                        }
                                        );
                                        DelayAction.Add(360, () =>
                                        {
                                            Q.Cast(target);
                                        }
                                        );

                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (target.IsValidTarget(checkqgap + checkegap + 400))
                    {
                        if (rready && renabled && target.DistanceToPlayer() < 700 && (target.HealthPercent < rheath || Player.CountEnemyHeroesInRange(600) > 1))
                        {
                            if (R1() && target.DistanceToPlayer() < 700)
                            {
                                if (target.HealthPercent < 40)
                                {
                                    DelayAction.Add(1, () => { R.Cast(rpreddmg.CastPosition); });
                                }
                                if (target.HealthPercent < rheath)
                                {
                                    if (eready)
                                    {
                                        if (qready)
                                        {
                                            E.Cast(target.Position);
                                            DelayAction.Add(112, () => { R.Cast(target); });
                                            DelayAction.Add(120, () => { Q.Cast(target); });
                                        }
                                    }
                                    else
                                    {
                                        if (qready)
                                        {
                                            E.Cast(target.Position);
                                            DelayAction.Add(110, () => { R.Cast(target); });
                                            DelayAction.Add(120, () => { Q.Cast(target); });
                                        }
                                    }
                                }
                                if (Player.CountEnemyHeroesInRange(600) > 1)
                                {
                                    if (eready)
                                    {
                                        if (qready)
                                        {
                                            E.Cast(target.Position);
                                            DelayAction.Add(110, () => { R.Cast(target); });
                                            DelayAction.Add(120, () => { Q.Cast(target); });
                                        }
                                    }
                                    else
                                    {
                                        if (qready)
                                        {
                                            E.Cast(target.Position);
                                            DelayAction.Add(110, () => { R.Cast(target); });
                                            DelayAction.Add(120, () => { Q.Cast(target); });
                                        }
                                    }
                                }
                            }
                            if (R2())
                            {
                                if (target.IsValidTarget(W.Range))
                                {
                                    if (beforeaa && target.Health + target.BonusHealth <= R.GetDamage(target) + Player.TotalAttackDamage * 2)
                                    {
                                        if (rpreddmg.Hitchance >= HitChance.High && rpreddmg.CastPosition != Vector3.Zero)
                                        {
                                            R.Cast(rpreddmg.CastPosition);
                                            DelayAction.Add(120, () => { Orbwalker.Attack(target); });
                                        }
                                    }
                                    if (HaveTiamat())
                                    {
                                        if (!onaa)
                                        {
                                            Player.UseItem((int)ItemId.Tiamat);
                                            Player.UseItem((int)ItemId.Ravenous_Hydra_Melee_Only);
                                            Player.UseItem((int)ItemId.Ravenous_Hydra);
                                            Player.UseItem((int)ItemId.Tiamat_Melee_Only);
                                        }
                                        if (target.Health <= R.GetDamage(target) + Q.GetDamage(target) * 3 + W.GetDamage(target))
                                            if (qready || wready || eready)
                                            {
                                                if (qready && wready && eready)
                                                {
                                                    if (rpreddmg.Hitchance >= HitChance.High && rpreddmg.CastPosition != Vector3.Zero)
                                                    {
                                                        //DelayAction.Add(1, () => { W.Cast(rpreddmg.CastPosition); });
                                                        DelayAction.Add(110, () => { R.Cast(rpreddmg.CastPosition); });
                                                        DelayAction.Add(120, () => { Q.Cast(target); });
                                                    }
                                                }
                                                else
                                                {
                                                    if (qready && wready)
                                                    {
                                                        //DelayAction.Add(1, () => { W.Cast(rpreddmg.CastPosition); });
                                                        DelayAction.Add(110, () => { R.Cast(rpreddmg.CastPosition); });
                                                        DelayAction.Add(120, () => { Q.Cast(target); });
                                                    }
                                                    else
                                                    {
                                                        if (eready && qready)
                                                        {
                                                            DelayAction.Add(1, () => { E.Cast(Player.Position.Extend(rpreddmg.CastPosition, -10)); });
                                                            DelayAction.Add(110, () => { R.Cast(rpreddmg.CastPosition); });
                                                            DelayAction.Add(120, () => { Q.Cast(target); });
                                                        }
                                                    }
                                                    if (qready && eready)
                                                    {
                                                        DelayAction.Add(110, () => { R.Cast(rpreddmg.CastPosition); });
                                                        DelayAction.Add(120, () => { Q.Cast(target); });
                                                    }
                                                    else
                                                    {
                                                        if (wready)
                                                        {
                                                            //DelayAction.Add(1, () => { W.Cast(rpreddmg.CastPosition); });
                                                            DelayAction.Add(110, () => { R.Cast(rpreddmg.CastPosition); });
                                                        }
                                                    }
                                                    if (wready && eready)
                                                    {
                                                        DelayAction.Add(1, () => { W.Cast(rpreddmg.CastPosition); });
                                                        DelayAction.Add(110, () => { R.Cast(rpreddmg.CastPosition); });
                                                    }
                                                    else
                                                    {
                                                        if (qready)
                                                        {
                                                            DelayAction.Add(110, () => { R.Cast(rpreddmg.CastPosition); });
                                                            DelayAction.Add(120, () => { Q.Cast(target); });
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                var rpredheath = R.GetHealthPrediction(target);
                                                if (target.Health - rpredheath <= R.GetDamage(target))
                                                {
                                                    if (rpreddmg.Hitchance >= HitChance.High)
                                                    {
                                                        R.Cast(rpreddmg.CastPosition);
                                                    }
                                                }
                                                if (target.Health <= R.GetDamage(target))
                                                {
                                                    if (rpreddmg.Hitchance >= HitChance.High)
                                                    {
                                                        R.Cast(rpreddmg.CastPosition);
                                                    }
                                                }
                                            }
                                    }
                                    else
                                    {
                                        if (target.Health <= R.GetDamage(target) + Q.GetDamage(target) * 2 + W.GetDamage(target))
                                            if (qready || wready || eready)
                                            {
                                                if (qready && wready && eready)
                                                {
                                                    if (rpreddmg.Hitchance >= HitChance.High && rpreddmg.CastPosition != Vector3.Zero)
                                                    {
                                                        //DelayAction.Add(1, () => { W.Cast(rpreddmg.CastPosition); });
                                                        DelayAction.Add(110, () => { R.Cast(rpreddmg.CastPosition); });
                                                        DelayAction.Add(120, () => { Q.Cast(target); });
                                                    }
                                                }
                                                else
                                                {
                                                    if (qready && wready)
                                                    {
                                                        //DelayAction.Add(1, () => { W.Cast(rpreddmg.CastPosition); });
                                                        DelayAction.Add(110, () => { R.Cast(rpreddmg.CastPosition); });
                                                        DelayAction.Add(120, () => { Q.Cast(target); });
                                                    }
                                                    else
                                                    {
                                                        if (eready)
                                                        {

                                                        }
                                                    }
                                                    if (qready && eready)
                                                    {
                                                        DelayAction.Add(110, () => { R.Cast(rpreddmg.CastPosition); });
                                                        DelayAction.Add(120, () => { Q.Cast(target); });
                                                    }
                                                    else
                                                    {
                                                        if (wready)
                                                        {
                                                            //DelayAction.Add(1, () => { W.Cast(rpreddmg.CastPosition); });
                                                            DelayAction.Add(110, () => { R.Cast(rpreddmg.CastPosition); });
                                                        }
                                                    }
                                                    if (wready && eready)
                                                    {
                                                        //DelayAction.Add(1, () => { W.Cast(rpreddmg.CastPosition); });
                                                        DelayAction.Add(110, () => { R.Cast(rpreddmg.CastPosition); });
                                                    }
                                                    else
                                                    {
                                                        if (qready)
                                                        {
                                                            DelayAction.Add(110, () => { R.Cast(rpreddmg.CastPosition); });
                                                            DelayAction.Add(120, () => { Q.Cast(target); });
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                var rpredheath = R.GetHealthPrediction(target);
                                                if (target.Health - rpredheath <= R.GetDamage(target))
                                                {
                                                    if (rpreddmg.Hitchance >= HitChance.High)
                                                    {
                                                        R.Cast(rpreddmg.CastPosition);
                                                    }
                                                }
                                                if (target.Health <= R.GetDamage(target))
                                                {
                                                    if (rpreddmg.Hitchance >= HitChance.High)
                                                    {
                                                        R.Cast(rpreddmg.CastPosition);
                                                    }
                                                }
                                            }
                                    }
                                }
                                else
                                {
                                    if (target.IsValidTarget(E.Range + Q.Range))
                                    {
                                        if (target.Health <= R.GetDamage(target) + Q.GetDamage(target) * 2)
                                        {
                                            if (qready && eready)
                                            {
                                                DelayAction.Add(1, () => { E.Cast(rpreddmg.CastPosition); });
                                                DelayAction.Add(110, () => { R.Cast(rpreddmg.CastPosition); });
                                                DelayAction.Add(120, () => { Q.Cast(target); });
                                            }
                                            else
                                            {
                                                if (qready)
                                                {
                                                    DelayAction.Add(110, () => { R.Cast(rpreddmg.CastPosition); });
                                                    DelayAction.Add(120, () => { Q.Cast(target); });
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (target.Health <= R.GetDamage(target))
                                        {
                                            if (rpreddmg.Hitchance >= HitChance.High)
                                            {
                                                R.Cast(rpreddmg.CastPosition);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if ((qready && qenabled) || (wready && wenabled) || (eready && eenabled))
                            {
                                if (HaveTiamat())
                                {
                                    if ((qready && qenabled) && (wready && wenabled) && (eready && eenabled))
                                    {
                                        if (target.DistanceToPlayer() <= Player.GetRealAutoAttackRange() + 100)
                                        {
                                            if (onaa) return;
                                            if (afteraa)
                                            {
                                                if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                                                Q.CastOnUnit(target);
                                                DelayAction.Add(10, () => { return; });
                                            }
                                            if (target.DistanceToPlayer() >= Player.GetRealAutoAttackRange())
                                            {
                                                if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                                                Q.CastOnUnit(target);
                                                DelayAction.Add(10, () => { return; });
                                            }
                                            if (afteraa)
                                            {
                                                if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                                                if (Player.HasBuff("riventricleavesoundtwo"))
                                                {
                                                    Geometry.Circle circle = new Geometry.Circle(target.Position, Player.GetRealAutoAttackRange() - 20);
                                                    foreach (var c in circle.Points.Where(c => c.DistanceToPlayer() < E.Range))
                                                    {
                                                        E.Cast(c);
                                                    }
                                                    DelayAction.Add(10, () => { return; });
                                                }
                                            }

                                        }
                                        else
                                        {
                                            if (target.DistanceToPlayer() <= checkqgap + checkegap + Player.GetRealAutoAttackRange())
                                            {
                                                if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                                                Geometry.Circle circle = new Geometry.Circle(target.Position, Player.GetRealAutoAttackRange() - 20);
                                                foreach (var c in circle.Points.Where(c => c.DistanceToPlayer() < E.Range))
                                                {
                                                    if (c != Vector2.Zero)
                                                        E.Cast(c);
                                                }
                                                if (target.IsValidTarget(checkegap))
                                                    E.Cast(target.Position);
                                                DelayAction.Add(120, () => { Q.CastOnUnit(target); });
                                                DelayAction.Add(200, () => { return; });
                                            }
                                            else
                                            {
                                                /*if (target.DistanceToPlayer() >= checkegap + Player.GetRealAutoAttackRange())
                                                {
                                                    E.Cast(target.Position);
                                                    DelayAction.Add(10, () => { return; });
                                                }
                                                else
                                                {
                                                    E.Cast(target.Position);
                                                    DelayAction.Add(1, () => { Q.CastOnUnit(target); });
                                                    DelayAction.Add(10, () => { return; });
                                                }*/
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (qready && qenabled && wready && wenabled)
                                        {
                                            if (target.DistanceToPlayer() <= Player.GetRealAutoAttackRange() + 100)
                                            {
                                                if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                                                if (onaa) return;
                                                if (target.DistanceToPlayer() <= Player.GetRealAutoAttackRange())
                                                {
                                                    if (afteraa)
                                                    {
                                                        Q.CastOnUnit(target);
                                                        DelayAction.Add(10, () => { return; });
                                                    }
                                                }
                                                else
                                                {
                                                    Q.CastOnUnit(target);
                                                    DelayAction.Add(10, () => { return; });
                                                }
                                            }
                                            else
                                            {
                                                if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                                                if (onaa && !target.IsValidTarget(checkqgap)) return;
                                                Q.CastOnUnit(target);
                                                DelayAction.Add(10, () => { return; });
                                            }
                                        }
                                        else
                                        {
                                            if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                                            if (eready && eenabled)
                                            {
                                                if (onaa) return;
                                                if (target.DistanceToPlayer() <= checkegap)
                                                {
                                                    E.Cast(target.Position);
                                                    DelayAction.Add(10, () => { return; });
                                                }
                                            }
                                        }
                                        if (qready && qenabled && eready && eenabled)
                                        {
                                            if (target.IsValidTarget(checkegap + checkqgap))
                                            {
                                                if (target.DistanceToPlayer() <= Player.GetRealAutoAttackRange() + 100)
                                                {
                                                    if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                                                    if (onaa) return;
                                                    if (target.DistanceToPlayer() <= Player.GetRealAutoAttackRange())
                                                    {
                                                        if (afteraa)
                                                        {
                                                            if (Player.HasBuff("riventricleavesoundtwo"))
                                                            {
                                                                Geometry.Circle circle = new Geometry.Circle(target.Position, Player.GetRealAutoAttackRange() - 20);
                                                                foreach (var c in circle.Points.Where(c => c.DistanceToPlayer() < E.Range))
                                                                {
                                                                    if (c != Vector2.Zero)
                                                                        E.Cast(c);
                                                                }
                                                            }
                                                            DelayAction.Add(120, () => { Q.Cast(target); });                                                         
                                                            DelayAction.Add(200, () => { return; });
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                                                        Q.Cast(target);
                                                        DelayAction.Add(10, () => { return; });
                                                    }
                                                }
                                                else
                                                {
                                                    if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                                                    if (onaa) return;
                                                    if (target.IsValidTarget(checkegap))
                                                        E.Cast(target.Position);
                                                    DelayAction.Add(120, () => { Q.Cast(target); });
                                                    DelayAction.Add(200, () => { return; });
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (wready && wenabled)
                                            {

                                            }
                                        }
                                        if (eready && eenabled && wready && wenabled)
                                        {
                                            if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                                            if (onaa) return;
                                            if (target.DistanceToPlayer() <= checkegap)
                                            {
                                                E.Cast(target.Position);
                                                DelayAction.Add(10, () => { return; });
                                            }
                                        }
                                        else
                                        {
                                            if (qready && qenabled)
                                            {
                                                if (target.DistanceToPlayer() <= Player.GetRealAutoAttackRange() + 100)
                                                {
                                                    if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                                                    if (onaa) return;
                                                    if (target.DistanceToPlayer() <= Player.GetRealAutoAttackRange())
                                                    {
                                                        if (afteraa)
                                                        {
                                                            Q.Cast(target);
                                                            DelayAction.Add(10, () => { return; });
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                                                        Q.Cast(target);
                                                        DelayAction.Add(10, () => { return; });
                                                    }
                                                }
                                                else
                                                {
                                                    if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                                                    if (onaa && !target.IsValidTarget(checkqgap)) return;
                                                    Q.Cast(target);
                                                    DelayAction.Add(10, () => { return; });
                                                }
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if ((qready && qenabled) && (wready && wenabled) && (eready && eenabled))
                                    {
                                        if (target.DistanceToPlayer() <= Player.GetRealAutoAttackRange() + 100)
                                        {
                                            if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                                            if (onaa) return;
                                            if (target.DistanceToPlayer() <= Player.GetRealAutoAttackRange())
                                            {
                                                if (afteraa)
                                                {
                                                    Q.Cast(target);                                                    
                                                    DelayAction.Add(10, () => { return; });
                                                }
                                                if(afteraa  || target.IsFleeing || !target.IsFacing(Player))
                                                {
                                                    if (target.IsValidTarget(W.Range))
                                                    {
                                                        W.Cast(target);
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                                                Q.Cast(target);
                                                DelayAction.Add(10, () => { return; });
                                            }
                                            if (target.DistanceToPlayer() >= Player.GetRealAutoAttackRange() && !afteraa && !beforeaa)
                                            {
                                                if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                                                Geometry.Circle circle = new Geometry.Circle(target.Position, Player.GetRealAutoAttackRange() - 20);
                                                foreach (var c in circle.Points.Where(c => c.DistanceToPlayer() < E.Range))
                                                {
                                                    if (c != Vector2.Zero)
                                                        E.Cast(c);
                                                }
                                                if (target.IsValidTarget(checkegap))
                                                    E.Cast(target.Position);
                                                DelayAction.Add(120, () => { Q.Cast(target); });
                                                DelayAction.Add(200, () => { return; });
                                            }
                                            if (afteraa)
                                            {
                                                if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                                                if (Player.HasBuff("riventricleavesoundtwo"))
                                                {
                                                    Geometry.Circle circle = new Geometry.Circle(target.Position, Player.GetRealAutoAttackRange() - 20);
                                                    foreach (var c in circle.Points.Where(c => c.DistanceToPlayer() < E.Range))
                                                    {
                                                        E.Cast(c);
                                                    }
                                                    DelayAction.Add(10, () => { return; });
                                                }
                                            }

                                        }
                                        else
                                        {
                                            if (target.DistanceToPlayer() <= checkqgap + checkegap + Player.GetRealAutoAttackRange())
                                            {
                                                if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                                                if (target.IsValidTarget(checkegap))
                                                    E.Cast(target.Position);
                                                DelayAction.Add(120, () => { Q.Cast(target); });
                                                DelayAction.Add(200, () => { return; });
                                            }
                                            else
                                            {
                                                /*if (target.DistanceToPlayer() <= checkegap + Player.GetRealAutoAttackRange())
                                                {
                                                    E.Cast(target.Position);
                                                    DelayAction.Add(10, () => { return; });
                                                }
                                                else
                                                {
                                                    E.Cast(target.Position);
                                                    DelayAction.Add(1, () => { Q.CastOnUnit(target); });
                                                    DelayAction.Add(10, () => { return; });
                                                }*/
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (qready && qenabled && wready && wenabled)
                                        {
                                            if (target.DistanceToPlayer() <= Player.GetRealAutoAttackRange() + 100)
                                            {
                                                if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                                                if (onaa) return;
                                                if (target.DistanceToPlayer() <= Player.GetRealAutoAttackRange())
                                                {
                                                    if (afteraa)
                                                    {
                                                        Q.Cast(target);                                                       
                                                        DelayAction.Add(10, () => { return; });
                                                    }
                                                    if(afteraa || target.IsFleeing || !target.IsFacing(Player))
                                                    {
                                                        if (target.IsValidTarget(W.Range + 75))
                                                        {
                                                            W.Cast(target.Position);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                                                    Q.Cast(target);
                                                    DelayAction.Add(10, () => { return; });
                                                }
                                            }
                                            else
                                            {
                                                if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                                                if (onaa && !target.IsValidTarget(checkqgap)) return;
                                                Q.Cast(target);
                                                DelayAction.Add(10, () => { return; });
                                            }
                                        }
                                        else
                                        {
                                            if (eready && eenabled)
                                            {
                                                if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                                                if (onaa) return;
                                                if (target.DistanceToPlayer() <= checkegap)
                                                {
                                                    E.Cast(target.Position);
                                                    DelayAction.Add(10, () => { return; });
                                                }
                                                if (afteraa && Player.HasBuff("riventricleavesoundtwo"))
                                                {
                                                    Geometry.Circle circle = new Geometry.Circle(target.Position, Q.Range);
                                                    foreach (var c in circle.Points.Where(c => c.DistanceToPlayer() < E.Range))
                                                    {
                                                        E.Cast(c);
                                                    }
                                                    DelayAction.Add(10, () => { return; });
                                                }
                                            }
                                        }
                                        if (qready && qenabled && eready && eenabled)
                                        {
                                            if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                                            if (target.IsValidTarget(checkegap + checkqgap))
                                            {
                                                if (target.DistanceToPlayer() <= Player.GetRealAutoAttackRange())
                                                {
                                                    if (onaa) return;
                                                    if (afteraa)
                                                    {
                                                        Q.Cast(target);
                                                        if (target.IsValidTarget(E.Range))
                                                        {
                                                            if (Player.HasBuff("riventricleavesoundtwo"))
                                                            {
                                                                Geometry.Circle circle = new Geometry.Circle(target.Position, Q.Range);
                                                                foreach (var c in circle.Points.Where(c => c.DistanceToPlayer() < E.Range))
                                                                {
                                                                    E.Cast(c);
                                                                }
                                                            }
                                                        }
                                                        DelayAction.Add(10, () => { return; });
                                                    }
                                                }
                                                else
                                                {
                                                    if (onaa) return;
                                                    Geometry.Circle circle = new Geometry.Circle(target.Position, Q.Range);
                                                    foreach (var c in circle.Points.Where(c => c.DistanceToPlayer() < E.Range))
                                                    {
                                                        E.Cast(c);
                                                    }
                                                    DelayAction.Add(120, () => { Q.Cast(target); });
                                                    DelayAction.Add(200, () => { return; });
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                                            if (wready && wenabled)
                                            {
                                                if (afteraa || target.IsFleeing || !target.IsFacing(Player) && target.IsValidTarget(W.Range))
                                                {
                                                    W.Cast(target.Position);
                                                    DelayAction.Add(10, () => { return; });
                                                }
                                            }
                                        }
                                        if (eready && eenabled && wready && wenabled)
                                        {
                                            if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                                            if (onaa) return;
                                            if (target.DistanceToPlayer() <= Player.GetRealAutoAttackRange())
                                            {
                                                if (afteraa || target.IsFleeing || !target.IsFacing(Player) && target.IsValidTarget(W.Range))
                                                {
                                                    W.Cast(target);
                                                    if (Player.HasBuff("riventricleavesoundtwo"))
                                                    {
                                                        Geometry.Circle circle = new Geometry.Circle(target.Position, Q.Range);
                                                        foreach (var c in circle.Points.Where(c => c.DistanceToPlayer() < E.Range))
                                                        {
                                                            E.Cast(c);
                                                        }
                                                    }
                                                    DelayAction.Add(10, () => { return; });
                                                }
                                            }
                                            else
                                            {
                                                if (target.DistanceToPlayer() <= checkegap)
                                                {
                                                    E.Cast(target);
                                                    DelayAction.Add(10, () => { return; });
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (Variables.GameTimeTickCount - lastEcast < 98 && Variables.GameTimeTickCount - lastEcast > 0) return;
                                            if (qready && qenabled)
                                            {
                                                if (target.DistanceToPlayer() <= Player.GetRealAutoAttackRange())
                                                {
                                                    if (onaa) return;
                                                    if (afteraa)
                                                    {
                                                        Q.Cast(target);
                                                    }
                                                    DelayAction.Add(10, () => { return; });
                                                }
                                                else
                                                {
                                                    if (onaa && !target.IsValidTarget(checkqgap)) return;
                                                    if (target.DistanceToPlayer() > Player.GetRealAutoAttackRange() + 100)
                                                    {
                                                        Q.Cast(target);
                                                    }
                                                    else
                                                    {
                                                        Q.Cast(target);
                                                    }
                                                    DelayAction.Add(10, () => { return; });
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
            #endregion
        }

        private static bool HaveTiamat()
        {
            return Player.CanUseItem((int)ItemId.Tiamat)
                    || Player.CanUseItem((int)ItemId.Ravenous_Hydra)
                    || Player.CanUseItem((int)ItemId.Tiamat_Melee_Only)
                    || Player.CanUseItem((int)ItemId.Ravenous_Hydra_Melee_Only);
        }
        
        #region old
        /*public static void RivenCombo()
        {
            AIHeroClient target = TargetSelector.GetTarget(900f);
            if (Q.IsReady())
            {
                CanQ = true;
            }
            else CanQ = false;
            if (W.IsReady())
            {
                CanW = true;
            }
            else CanW = false;
            if (E.IsReady())
            {
                CanE = true;
            }
            else CanE = false;
            if (R.IsReady())
            {
                CanR = true;
                if (Player.CanUseItem((int)ItemId.Tiamat_Melee_Only) && R2())
                {
                    CanTiamatR2 = true;
                }
                else CanTiamatR2 = false;
                if (Q.IsReady() && R2())
                {
                    CanR2Q = true;
                }
                else CanR2Q = false;
                if (E.IsReady() && Q.IsReady() && R1())
                {
                    CanER1Q = true;
                }
                else CanER1Q = false;
            }
            else CanR = false;
            if (Player.CanUseItem((int)ItemId.Tiamat_Melee_Only))
            {
                CanTiamat = true;
                if (W.IsReady())
                {
                    CanTiamatW = true;
                }
                else CanTiamatW = false;
            }
            else CanTiamat = false;
            if(target != null)
            {
                if(RMenu.ComboFlash.Active)
                {
                    FastCombo(target);
                }
                else
                {
                    NorCombo(target);
                }
            }
        }
        public static void NorCombo(AIHeroClient target)
        {
            if(CanR && RMenu.ComboR.Enabled)
            {
                if(R1())
                {
                    if(CanER1Q && target.HealthPercent <= RMenu.R1target.Value && target.IsValidTarget(Player.GetRealAutoAttackRange() + 250 + 200 + 100))
                    {
                        E.Cast(target.Position);
                        DelayAction.Add(150 - Game.Ping, () => { R.Cast(target);});
                        DelayAction.Add(160 - Game.Ping, () => { Q.Cast(target); });
                    }
                }
                if(R2())
                {
                    if(CanR2AA && target.Health <= Player.GetAutoAttackDamage(target) + R.GetDamage(target) && target.InAutoAttackRange() && beforeaa)
                    {
                        R.Cast(target.Position);
                    }
                    else
                    {
                        if (target.Health < R.GetDamage(target) && R2())
                        {
                            R.Cast(R.GetPrediction(target).CastPosition);
                        }
                    }
                    if(CanTiamatR2 && target.Health <= Player.GetAutoAttackDamage(target) + R.GetDamage(target) && target.IsValidTarget(Player.GetRealAutoAttackRange() + 100) && CanTiamat)
                    {
                        Player.UseItem((int)ItemId.Tiamat_Melee_Only);
                        R.Cast(target.Position);
                    }
                    else
                    {
                        if (target.Health < R.GetDamage(target) && R2())
                        {
                            R.Cast(R.GetPrediction(target).CastPosition);
                        }
                    }

                }
            }
            if(CanE && RMenu.ComboE.Enabled)
            {
                if (target.IsValidTarget(Player.GetRealAutoAttackRange() + 250 + 200))
                {
                    if (target.DistanceToPlayer() > 250 && (afteraa || onaa))
                    {
                        if (onaa)
                        {
                            DelayAction.Add(RMenu.Qdelayslider.Value - Game.Ping, () => {
                                E.CastOnUnit(target);
                            });
                        }
                        else
                            E.CastOnUnit(target);
                    }
                }
            }
            if(CanW && RMenu.ComboW.Enabled && target.IsValidTarget(W.Range))
            {
                if(CanTiamatW && !beforeaa && target.IsValidTarget(W.Range))
                {
                    Player.UseItem((int)ItemId.Tiamat_Melee_Only);
                    W.Cast(target);
                }
                if (onaa)
                {
                    DelayAction.Add(RMenu.Qdelayslider.Value - Game.Ping, () => {
                        W.Cast(target);
                    });
                }
            }
            if(CanE && RMenu.ComboE.Enabled)
            {
                if (target.DistanceToPlayer() < Player.GetRealAutoAttackRange() + 200 + 250)
                {
                    if (target.DistanceToPlayer() > Player.GetRealAutoAttackRange())
                    {
                        DelayAction.Add(RMenu.Qdelayslider.Value *2 - Game.Ping, () => {
                            E.Cast(target.Position);
                        });
                    }
                }
            }
            if(CanQ && RMenu.ComboQ.Enabled)
            {
                if (target.DistanceToPlayer() < Player.GetRealAutoAttackRange() + 200)
                {
                    if (afteraa || target.DistanceToPlayer() > Player.GetRealAutoAttackRange() + 30 || onaa)
                    {
                        if (onaa)
                        {
                            DelayAction.Add(RMenu.Qdelayslider.Value - Game.Ping, () => {
                                Q.CastOnUnit(target);
                                if (target.InAutoAttackRange())
                                {
                                    Orbwalker.Attack(target);
                                }
                            });
                        }
                        if (target.DistanceToPlayer() > Player.GetRealAutoAttackRange() + 30)
                        {
                            Q.CastOnUnit(target);
                            if (target.InAutoAttackRange())
                            {
                                Orbwalker.Attack(target);
                            }
                        }
                        if (afteraa)
                        {
                            Q.CastOnUnit(target);
                            if (target.InAutoAttackRange())
                            {
                                Orbwalker.Attack(target);
                            }
                        }
                    }
                }
            }
        }
        public static void FastCombo(AIHeroClient target)
        {

        }*/
        #endregion
        public static bool CanQ = false;
        public static bool CanW = false;
        public static bool CanE = false;
        public static bool CanR = false;
        public static bool CanTiamat = false;
        public static bool CanTiamatW = false;
        public static bool CanTiamatR2 = false;
        public static bool CanER1Q = false;
        public static bool CanR2Q = false;
        public static bool CanR2AA = false;
    }
}
