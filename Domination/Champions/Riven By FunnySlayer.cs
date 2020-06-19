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

namespace DominationAIO.Champions
{
    public class RMenu
    {
        //simple combo
        public static MenuBool ComboQ = new MenuBool("ComboQ", "Combo Q");
        public static MenuBool ComboW = new MenuBool("ComboW", "Combo W");
        public static MenuBool ComboE = new MenuBool("ComboE", "Combo E");
        public static MenuBool ComboR = new MenuBool("ComboR", "Combo R");
        public static MenuSlider R1target = new MenuSlider("Rtarget", "^ Use R1 if target heath =< x %", 40);
        public static MenuSlider R2hit = new MenuSlider("R2hit", "^ Use R2 if hit count >= x", 3 , 1 , 5);
        public static MenuBool TiamatW = new MenuBool("TiamatW", "^ Tiamat W");
        public static MenuBool TiamatR2 = new MenuBool("TiamatR", "^ Tiamat R2");
        public static MenuBool RAA = new MenuBool("RAA", "^ R2 AA");
        public static MenuBool ER1Q = new MenuBool("ER1Q", "^ E R1 Q");
        public static MenuBool R2Q = new MenuBool("R2Q", "^ R2 Q");
        public static MenuKeyBind ComboFlash = new MenuKeyBind("ComboFlash", "Use Flash in combo", System.Windows.Forms.Keys.A, KeyBindType.Toggle) { Active = false };
        public static MenuBool WaitforQ = new MenuBool("WaitQ", "^ Wait for Q is ready");
        public static MenuBool WaitforW = new MenuBool("WaitW", "^ Wait for W is ready", false);
        public static MenuBool WaitforE = new MenuBool("WaitE", "^ Wait for E is ready");
        public static MenuBool WaitforR = new MenuBool("WaitE", "^ Wait for R is ready");

        public static MenuSlider Qdelayslider = new MenuSlider("Qdelayslider", "Q set Delay", 300, 0, 600);
        public static MenuBool Qdelay = new MenuBool("Qdelay", "Q delay");
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
            return R.Name == "RivenFengShuiEngine";
        }
        public static bool R2()
        {
            return R.Name == "RivenIzunaBlade";
        }
        public static bool Q1()
        {
            return !Player.HasBuff("riventricleavesoundone") || !Player.HasBuff("riventricleavesoundtwo");
        }
        public static bool Q2()
        {
            return Player.HasBuff("riventricleavesoundone") || !Player.HasBuff("riventricleavesoundtwo");
        }
        public static bool Q3()
        {
            return Player.HasBuff("riventricleavesoundtwo");
        }
        public static void OnLoaded()
        {
            Game.Print("FunnySlayer Riven || Still under test");
            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 200f);
            E = new Spell(SpellSlot.E, 250);
            R = new Spell(SpellSlot.R, 900);
            fl = new Spell(flash, 400f);
            ig = new Spell(ignite, 400);
            R.SetSkillshot(0.25f, 45, 1600, false, false, SkillshotType.Cone); 

            RivenMenu = new Menu("RivenMenu", "FunnySlayer Riven", true);
            var combom = new Menu("combom", "Combo Menu");
            combom.Add(RMenu.ComboQ);
            combom.Add(RMenu.ComboW);
            combom.Add(RMenu.ComboE);
            combom.Add(RMenu.ComboR);
            combom.Add(RMenu.R1target);
            combom.Add(RMenu.R2hit);
            combom.Add(RMenu.RAA);
            combom.Add(RMenu.R2Q);
            combom.Add(RMenu.ER1Q);
            combom.Add(RMenu.TiamatR2);
            combom.Add(RMenu.TiamatW);
            combom.Add(RMenu.ComboFlash).Permashow();
            combom.Add(RMenu.Qdelayslider);
            combom.Add(RMenu.Qdelay).Permashow();
            RivenMenu.Add(combom);
            RivenMenu.Attach();

            Game.OnUpdate += Game_OnUpdate;
            Orbwalker.OnAction += Orbwalker_OnAction;
            AIBaseClient.OnProcessSpellCast += AIBaseClient_OnProcessSpellCast;
            AIBaseClient.OnPlayAnimation += AIBaseClient_OnPlayAnimation;
            Interrupter.OnInterrupterSpell += Interrupter_OnInterrupterSpell;
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {

        }

        private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserArgs args)
        {

        }

        private static void Interrupter_OnInterrupterSpell(AIHeroClient sender, Interrupter.InterruptSpellArgs args)
        {

        }

        private static void AIBaseClient_OnPlayAnimation(AIBaseClient sender, AIBaseClientPlayAnimationEventArgs args)
        {

        }

        private static void AIBaseClient_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            var spell = args.SData;
            if (sender.IsMe)
            {
                if (spell.Name.Contains("ItemTiamatCleave"))
                {
                    lastTiamat = Variables.GameTimeTickCount;
                }
                if (spell.Name.Contains("RivenTriCleave"))
                {
                    Orbwalker.ResetAutoAttackTimer();
                    lastQcast = Variables.GameTimeTickCount;
                }
                if (spell.Name.Contains("RivenFient"))
                {
                    lastEcast = Variables.GameTimeTickCount;
                }
            }
        }

        private static void Orbwalker_OnAction(object sender, OrbwalkerActionArgs args)
        {
            if (args.Type == OrbwalkerType.BeforeAttack)
            {
                beforeaa = true;
            }
            else beforeaa = false;

            if (args.Type == OrbwalkerType.AfterAttack)
            {
                afteraa = true;
            }
            else afteraa = false;

            if (args.Type == OrbwalkerType.OnAttack)
            {
                onaa = true;
            }
            else onaa = false;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if(RMenu.Qdelay.Enabled)
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
            }
            
            if (!Player.IsDead)
            {
                switch (Orbwalker.ActiveMode)
                {
                    case OrbwalkerMode.Combo:
                        RivenCombo();
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
        public static void RivenCombo()
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

        }
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
