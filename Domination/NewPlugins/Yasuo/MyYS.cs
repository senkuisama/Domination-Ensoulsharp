using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.Utility;
using FunnySlayerCommon;
using SharpDX;
using SPredictionMash;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominationAIO.NewPlugins.Yasuo
{
    public class MyYS
    {
        public static Menu themenu = new Menu("FunnySlayer Yasuo", "FunnySlayer Yasuo", true);

        #region Menu
        public class YasuoMenu
        {
            //public static MenuKeyBind DrawObjPlayerPos = new MenuKeyBind("DrawObjPlayerPos", "DrawObjPlayerPos", Keys.Z, KeyBindType.Press);
            public static MenuBool ResetConfig = new MenuBool("Reset menu", "Reset menu", false);
            public class RangeCheck
            {
                public static MenuSlider Qrange = new MenuSlider("Qrange", "Q Range", 475, 0, 475);
                public static MenuSlider Q3range = new MenuSlider("Q3range", "Wind Range", 1000, 0, 1100);
                public static MenuSlider EQrange = new MenuSlider("EQrange", "EQ Range", 200, 100, 250);
                public static MenuSlider Erange = new MenuSlider("Erange", "E Range", 475, 0, 475);
                public static MenuSlider Egaprange = new MenuSlider("Egaprange", "E Gapcloser Range", 2000, 0, 3000);
                public static MenuSlider Rrange = new MenuSlider("Rrange", "R Range", 1400, 0, 1400);
            }
            public class Yasuo_target
            {
                public static MenuBool Yasuo_Target_lock = new MenuBool("Yasuo_Target_lock", "Livestream");
            }

            public class Qcombo
            {
                public static MenuBool Yasuo_Qcombo = new MenuBool(",Yasuo_Qcombo", "Yasuo Q in Combo");
                public static MenuBool Yasuo_Windcombo = new MenuBool(",Yasuo_Windcombo", "Yasuo Wind in Combo");
                public static MenuBool Yasuo_Qaa = new MenuBool(",Yasuo_Qaa", "----> Q After AA", false);
                public static MenuBool Yasuo_Qba = new MenuBool(",Yasuo_Qba", "----> Q Before AA", false);
                public static MenuBool Yasuo_Qalways = new MenuBool(",Yasuo_Qalwaays", "----> Q always in combo");
                public static MenuSlider Yasuo_Qoa = new MenuSlider(",Yasuo_Qoa", "----> Q Cancel aa", 30, 0, 100);
            }

            public class EQCombo
            {
                public static MenuBool Yasuo_EQcombo = new MenuBool(",Yasuo_EQcombo", "Yasuo EQ in Combo");
                public static MenuBool Yasuo_EWindcombo = new MenuBool(",Yasuo_EWindcombo", "Yasuo EQ Wind in Combo");
                public static MenuSlider EBonusRange = new MenuSlider(",Yasuo_EBonusRange", "Yasuo Bonus E Range", 50, 0, 150);
            }

            public class Wcombo
            {
                public static MenuSeparator Yasuo_Wcombo = new MenuSeparator(",Yasuo_Wcombo", "Yasuo W in Combo");
                public static MenuBool LoadWEvade = new MenuBool("LoadWEvade", "Load W evade (F5)");
            }

            public class Ecombo
            {
                public static MenuSliderButton Yasuo_ERange = new MenuSliderButton(",Yasuo_ERange", "Yasuo E in Combo", 1, 1, 2);
                public static MenuSlider DashingTick = new MenuSlider("DashingTick", "Dashing Tick", 400, 0, 500);
                public static MenuBool Yasuo_Eziczac = new MenuBool(",Yasuo_Eziczac", "----> E zic zac");
                public static MenuSlider Yasuo_zizzacRange = new MenuSlider(",Yasuo_zizzacRange", "Target is Valid", 850, 0, 2000);
                public static MenuBool Yasuo_Eziczac_Qready = new MenuBool(",Yasuo_Eziczac_Qready", "----> E zic zac only when Q not ready", true);
                public static MenuList Yasuo_EziczacMode = new MenuList(",Yasuo_EziczacMode", "Yasuo E ZZ", new string[] { "Gap obj is validable", "Target Count > 1", "Solo Mode", "Flower Dashing" }, 0);
                public static MenuList Yasuo_EMode = new MenuList(",Yasuo_EMode", "Yasuo E Mode", new string[] { "Target Pos", "Cursor Pos", "Logic Target Gapcloser" }, 2);
                public static MenuBool ddtest = new MenuBool("ddtest", "Disable move When dash (Not recommand)", false);
            }

            public class Rcombo
            {
                public static MenuBool Yasuo_Rcombo = new MenuBool(",Yasuo_Rcombo", "Yasuo R in Combo");
                public static MenuBool Yasuo_Rcombo_EQR = new MenuBool(",Yasuo_Rcombo_EQR", "Logic EQ R");
                public static MenuBool AlwaysEQR = new MenuBool("...AlwaysEQR", "R Tornado always");
                public static MenuSlider RtargetHeath = new MenuSlider("RtargetHeath", "---> Target heath ", 70, 0, 101);
                public static MenuSlider RDelayTime = new MenuSlider("R Delay Time (ms)", "R Delay Cast (ms)", 10);
            }

            public class Yasuo_Clear
            {
                public static MenuBool Yasuo_Qclear = new MenuBool(",Yasuo_Qclear", "Q in Clear");
                public static MenuBool Yasuo_Qclear_aa = new MenuBool(",Yasuo_Qclear_aa", "----> Q clear after aa", false);
                public static MenuBool Yasuo_Qclear_ba = new MenuBool(",Yasuo_Qclear_ba", "----> Q clear before aa", false);
                public static MenuBool Yasuo_Qclear_always = new MenuBool(",Yasuo_Qclear_always", "----> Q clear always");
                public static MenuBool Yasuo_Windclear = new MenuBool(",Yasuo_Windclear", "Wind in Clear");
                public static MenuSlider Yasuo_Windclear_ifhit = new MenuSlider(",Yasuo_Windclear_ifhit", "----> minions count ", 1, 1, 5);
                public static MenuBool Yasuo_Eclear = new MenuBool(",Yasuo_Eclear", "E in clear");
                public static MenuBool Yasuo_Eclear_ks = new MenuBool(",Yasuo_Eclear_ks", "----> Ks minions", false);
            }

            public class Yasuo_Keys
            {
                public static MenuKeyBind Yasuo_Flee = new MenuKeyBind(",Yasuo_Flee", "Flee Key", Keys.E, KeyBindType.Press);
                public static MenuKeyBind Yasuo_AutoQharass = new MenuKeyBind(",Yasuo_AutoQ", "Auto Q harass Key", Keys.A, KeyBindType.Toggle);
                public static MenuKeyBind Yasuo_AutoStacks = new MenuKeyBind(",Yasuo_AutoStacks", "Auto Stacks Q", Keys.A, KeyBindType.Toggle);
                public static MenuKeyBind AutoQifDashOnTarget = new MenuKeyBind(",AutoQifDashOnTarget", "Auto use Q if dash [Flee , Dashing on target, Exploit]", Keys.N, KeyBindType.Toggle) { Active = true };
                public static MenuKeyBind TurretKey = new MenuKeyBind(",TurretKey", "Accept E turret", Keys.T, KeyBindType.Toggle);
                public static MenuKeyBind EQFlashKey = new MenuKeyBind(",EQFlashKey", "EQ flash key", Keys.G, KeyBindType.Press);

                public static MenuKeyBind ComboEQ3Flash = new MenuKeyBind(",EQ3 Flash when Combo", "EQ3 Flash when Combo [Toggle]", Keys.H, KeyBindType.Toggle);

            }

            public static MenuBool UseExploit = new MenuBool("Use Exploit Q", "Bug Q When Dash");

            public static void AddMenuYasuo()
            {

                themenu.Add(ResetConfig);
                themenu.Add(YasuoMenu.Yasuo_target.Yasuo_Target_lock);
                SPredictionMash.ConfigMenu.Initialize(themenu, "Prediction Helper");
                var SkillRange = new Menu("SkillRange", "Yasuo Skill Range");
                var Qcombo = new Menu("YasuoQincombo", "Yasuo_Q Combo");
                var Ecombo = new Menu("YasuoEincombo", "Yasuo_E Combo");
                var EQcombo = new Menu("YasuoEQincombo", "Yasuo_EQ Combo");
                var Rcombo = new Menu("YasuoRincombo", "Yasuo_R Combo");
                var Wcombo = new Menu("YasuoWincombo", "Yasuo_W Combo");
                var ysClear = new Menu("ysClear", "Clear Settings");
                var yskeys = new Menu("YasuoKeys", "All Key Settings");

                SkillRange.Add(YasuoMenu.RangeCheck.Qrange);
                SkillRange.Add(YasuoMenu.RangeCheck.Q3range);
                SkillRange.Add(YasuoMenu.RangeCheck.EQrange);
                SkillRange.Add(YasuoMenu.RangeCheck.Erange);
                SkillRange.Add(YasuoMenu.RangeCheck.Egaprange);
                SkillRange.Add(YasuoMenu.RangeCheck.Rrange);

                yskeys.Add(YasuoMenu.Yasuo_Keys.Yasuo_Flee).Permashow();
                yskeys.Add(YasuoMenu.Yasuo_Keys.ComboEQ3Flash).Permashow();
                yskeys.Add(YasuoMenu.Yasuo_Keys.EQFlashKey).Permashow();
                yskeys.Add(YasuoMenu.Yasuo_Keys.TurretKey).Permashow();
                yskeys.Add(YasuoMenu.Yasuo_Keys.Yasuo_AutoQharass).Permashow();
                yskeys.Add(YasuoMenu.Yasuo_Keys.Yasuo_AutoStacks).Permashow();
                yskeys.Add(YasuoMenu.Yasuo_Keys.AutoQifDashOnTarget).Permashow(true, "Auto Q in Dash", SharpDX.Color.Azure);


                ysClear.Add(YasuoMenu.Yasuo_Clear.Yasuo_Qclear);
                ysClear.Add(YasuoMenu.Yasuo_Clear.Yasuo_Qclear_always);
                ysClear.Add(YasuoMenu.Yasuo_Clear.Yasuo_Qclear_aa);
                ysClear.Add(YasuoMenu.Yasuo_Clear.Yasuo_Qclear_ba);
                ysClear.Add(YasuoMenu.Yasuo_Clear.Yasuo_Windclear);
                ysClear.Add(YasuoMenu.Yasuo_Clear.Yasuo_Windclear_ifhit);
                ysClear.Add(YasuoMenu.Yasuo_Clear.Yasuo_Eclear);
                ysClear.Add(YasuoMenu.Yasuo_Clear.Yasuo_Eclear_ks);

                Qcombo.Add(YasuoMenu.Qcombo.Yasuo_Qcombo);
                Qcombo.Add(YasuoMenu.Qcombo.Yasuo_Windcombo);
                Qcombo.Add(YasuoMenu.Qcombo.Yasuo_Qalways);
                Qcombo.Add(YasuoMenu.Qcombo.Yasuo_Qaa);
                Qcombo.Add(YasuoMenu.Qcombo.Yasuo_Qba);
                Qcombo.Add(YasuoMenu.Qcombo.Yasuo_Qoa);

                Ecombo.Add(YasuoMenu.Ecombo.Yasuo_ERange);
                Ecombo.Add(YasuoMenu.Ecombo.DashingTick);
                Ecombo.Add(YasuoMenu.Ecombo.Yasuo_EMode).Permashow();
                Ecombo.Add(YasuoMenu.Ecombo.Yasuo_Eziczac);
                Ecombo.Add(YasuoMenu.Ecombo.Yasuo_zizzacRange);
                Ecombo.Add(YasuoMenu.Ecombo.Yasuo_Eziczac_Qready);
                Ecombo.Add(YasuoMenu.Ecombo.Yasuo_EziczacMode).Permashow(true, YasuoMenu.Ecombo.Yasuo_Eziczac.Enabled ? YasuoMenu.Ecombo.Yasuo_EziczacMode.SelectedValue : "Disabled", SharpDX.Color.Yellow);
                Ecombo.Add(YasuoMenu.Ecombo.ddtest);

                EQcombo.Add(YasuoMenu.EQCombo.Yasuo_EQcombo);
                EQcombo.Add(YasuoMenu.EQCombo.Yasuo_EWindcombo);
                EQcombo.Add(YasuoMenu.EQCombo.EBonusRange);

                Rcombo.Add(YasuoMenu.Rcombo.Yasuo_Rcombo);
                Rcombo.Add(YasuoMenu.Rcombo.RDelayTime);
                Rcombo.Add(YasuoMenu.Rcombo.Yasuo_Rcombo_EQR);
                Rcombo.Add(YasuoMenu.Rcombo.AlwaysEQR);
                Rcombo.Add(YasuoMenu.Rcombo.RtargetHeath);

                Wcombo.Add(YasuoMenu.Wcombo.Yasuo_Wcombo);
                Wcombo.Add(YasuoMenu.Wcombo.LoadWEvade);

                /*var fstarget = new Menu("fs target", "FS Target Selector");
                fstarget.AddTargetSelectorMenu();*/

                //themenu.Add(fstarget);

                themenu.Add(Qcombo);
                themenu.Add(Ecombo);
                themenu.Add(EQcombo);
                themenu.Add(Rcombo);
                themenu.Add(Wcombo);
                themenu.Add(ysClear);
                themenu.Add(SkillRange);
                themenu.Add(yskeys);

                themenu.Add(YasuoMenu.UseExploit).Permashow();
                //themenu.Add(YasuoMenu.DrawObjPlayerPos);

                if (YasuoMenu.Wcombo.LoadWEvade.Enabled)
                    EvadeTarget.Init(Wcombo);

                themenu.Attach();

                ResetConfig.ValueChanged += ResetConfig_ValueChanged;

                Drawing.OnEndScene += Drawing_OnEndScene;
            }

            private static void Drawing_OnEndScene(EventArgs args)
            {
                if (ObjectManager.Player.IsDead)
                    return;

                if (CheckImDashing)
                {
                    Render.Circle.DrawCircle(ObjectManager.Player.Position, RangeCheck.EQrange.Value, System.Drawing.Color.Red);
                }
            }

            private static void ResetConfig_ValueChanged(MenuBool menuItem, EventArgs args)
            {
                if (ResetConfig.Enabled)
                    Game.Print("Reset Yasuo Config");

                ResetConfig.Enabled = false;

                RangeCheck.Qrange.Value = 475;
                RangeCheck.Q3range.Value = 1080;
                RangeCheck.EQrange.Value = 200;
                RangeCheck.Erange.Value = 475;
                RangeCheck.Egaprange.Value = 2000;
                RangeCheck.Rrange.Value = 1400;

                Yasuo_Keys.Yasuo_Flee.Active = false;
                Yasuo_Keys.ComboEQ3Flash.Active = true;
                Yasuo_Keys.EQFlashKey.Active = false;
                Yasuo_Keys.TurretKey.Active = false;
                Yasuo_Keys.Yasuo_AutoQharass.Active = true;
                Yasuo_Keys.Yasuo_AutoStacks.Active = true;
                Yasuo_Keys.AutoQifDashOnTarget.Active = true;

                Yasuo_Clear.Yasuo_Qclear.Enabled = true;
                Yasuo_Clear.Yasuo_Qclear_always.Enabled = true;
                Yasuo_Clear.Yasuo_Qclear_aa.Enabled = false;
                Yasuo_Clear.Yasuo_Qclear_ba.Enabled = false;
                Yasuo_Clear.Yasuo_Windclear.Enabled = true;
                Yasuo_Clear.Yasuo_Windclear_ifhit.Value = 1;
                Yasuo_Clear.Yasuo_Eclear.Enabled = true;
                Yasuo_Clear.Yasuo_Eclear_ks.Enabled = true;

                Qcombo.Yasuo_Qcombo.Enabled = true;
                Qcombo.Yasuo_Windcombo.Enabled = true;
                Qcombo.Yasuo_Qalways.Enabled = true;
                Ecombo.Yasuo_ERange.Enabled = true;
                Ecombo.Yasuo_ERange.Value = 2;
                Ecombo.Yasuo_EMode.Index = 2;
                Ecombo.DashingTick.Value = 150;
                Ecombo.Yasuo_Eziczac.Enabled = false;
                Ecombo.ddtest.Enabled = false;

                EQCombo.EBonusRange.Value = 0;
                Rcombo.Yasuo_Rcombo.Enabled = true;
                Rcombo.RDelayTime.Value = 4;
                Rcombo.Yasuo_Rcombo_EQR.Enabled = true;
                Rcombo.AlwaysEQR.Enabled = false;
                Rcombo.RtargetHeath.Value = 70;
            }
        }

        #endregion

        public static bool baa = false;
        public static bool oaa = false;
        public static bool aaa = false;
        public static Spell Q, Q3, W, E, E1, R;
        public static Spell EQFlash;
        public static SpellSlot Flash;

        public static void YasuoLoad()
        {
            Q = new Spell(SpellSlot.Q, 475);
            Q3 = new Spell(SpellSlot.Q, 1100);
            W = new Spell(SpellSlot.W, 100);
            E = new Spell(SpellSlot.E, 475);
            E1 = new Spell(SpellSlot.E, 475);
            R = new Spell(SpellSlot.R, 1400);

            Q.SetSkillshot(0.25f, 55, 20000, false, SpellType.Line);
            Q3.SetSkillshot(0.275f, 80, 1200, false, SpellType.Line);
            E1.SetTargetted(0f, 1000f);
            E.SetSkillshot(0.3f, 175, 1000f, false, SpellType.Line);

            Flash = ObjectManager.Player.GetSpellSlot("summonerflash");
            EQFlash = new Spell(Flash, 850f);
            EQFlash.SetSkillshot(0, 175f, float.MaxValue, false, SpellType.Circle);
            E.Speed = 800;
            YasuoMenu.AddMenuYasuo();

            //Game.OnUpdate += Game_OnUpdate;
            Game.OnUpdate += Game_OnUpdate1;
            //Game.OnUpdate += Game_OnUpdate2;
            Game.OnUpdate += Game_OnUpdate3;
            //AIBaseClient.OnProcessSpellCast += AIBaseClient_OnProcessSpellCast;
            //Game.OnUpdate += Game_OnUpdate4;
            //Orbwalker.OnBeforeAttack += OrbwalkerOnOnBeforeAttack;
            //  Orbwalker.OnBeforeMove += Orbwalker_OnBeforeMove;
            Dash.OnDash += Dash_OnDash;

            //AIBaseClient.OnProcessSpellCast += AIBaseClient_OnProcessSpellCast;
        }

        private static void Dash_OnDash(AIBaseClient sender, Dash.DashArgs args)
        {
            if (sender.IsMe)
            {
                CheckDashingEndTime = args.EndTick + YasuoMenu.Ecombo.DashingTick.Value;
            }
        }
        private static int lastE = 0;
        private static void AIBaseClient_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.Slot == SpellSlot.E)
                {
                    lastE = Variables.GameTimeTickCount;
                    CheckImDashing = true;
                }
            }
        }

        /*private static void AIBaseClient_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if(sender.IsMe && args.Slot == SpellSlot.Q)
            {
                //Game.SendEmote(EmoteId.Dance);
                //ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }
        }*/

        #region Onupdate 3
        private static int CheckDashingEndTime = 0;
        public static bool CheckImDashing = false;
        private static void Game_OnUpdate3(EventArgs args)
        {
            if (!CheckImDashing && CheckDashingEndTime - Variables.GameTimeTickCount > 0)
                CheckImDashing = true;
            else 
                if(CheckImDashing)
                    CheckImDashing = false;



            if (CheckImDashing && Q.IsReady() && !YasuoHelper.HaveQ3)
            {
                if (YasuoMenu.Yasuo_Keys.Yasuo_AutoStacks.Active && YasuoHelper.HaveQ1)
                {
                    var targets = TargetSelector.GetTargets(250, DamageType.Physical);
                    if (targets == null)
                        return;

                    if (targets.Where(i => YasuoHelper.Eprediction(i).Distance(ObjectManager.Player.GetDashInfo().EndPos) <= YasuoMenu.RangeCheck.EQrange.Value).FirstOrDefault() != null)
                    {
                        Q.Cast(YasuoHelper.PosExploit(null));
                        return;
                    }
                    else
                    {
                        if (targets.Where(i => i.Distance(ObjectManager.Player.GetDashInfo().EndPos) < 350).Count() < 1)
                        {
                            Q.Cast(YasuoHelper.PosExploit(null));
                        }
                        return;
                    }
                }
                else
                {
                    if (!YasuoHelper.HaveQ3)
                    {
                        if (YasuoMenu.Yasuo_Keys.AutoQifDashOnTarget.Active)
                        {
                            var target = GameObjects.EnemyHeroes.Where(i => !i.IsDead && !i.IsAlly && i.DistanceToPlayer() <= 600).OrderBy(i => i.DistanceToPlayer()).FirstOrDefault();
                            if (target != null && YasuoHelper.Eprediction(target).Distance(ObjectManager.Player.GetDashInfo().EndPos) <= YasuoMenu.RangeCheck.EQrange.Value && ObjectManager.Player.GetDashInfo().EndPos.DistanceToPlayer() < 100)
                            {
                                Q.Cast(YasuoHelper.PosExploit(target));
                            }
                            if (YasuoMenu.Yasuo_Keys.Yasuo_Flee.Active || YasuoMenu.Yasuo_Keys.EQFlashKey.Active)
                            {
                                Q.Cast(YasuoHelper.PosExploit(target));
                            }
                            if (YasuoMenu.Yasuo_Keys.EQFlashKey.Active)
                            {
                                Q.Cast(YasuoHelper.PosExploit(target));
                            }

                            if (YasuoHelper.HaveQ2)
                            {
                                Q.Cast(YasuoHelper.PosExploit(target));
                            }
                        }
                    }                   
                }
            }
            else
            {
                return;
            }
        }
        #endregion

        #region Onupdate 2
        #endregion

        #region Combo
        private static void EQFlashInCombo()
        {
            if (Flash == SpellSlot.Unknown || !EQFlash.IsReady() || !Flash.IsReady()) return;
            var targets = TargetSelector.GetTargets(850, DamageType.Physical);
            Vector3 FlashPos = Vector3.Zero;

            if (!targets.Any() || targets == null) return;

            var target = FSTargetSelector.GetFSTarget(850);

            //foreach (var EQprediction in )
            //{
            var EQprediction = targets.Select(i => FSpred.Prediction.Prediction.GetPrediction(EQFlash, i)).Where(i => i.Hitchance >= FSpred.Prediction.HitChance.High && i.AoeTargetsHitCount >= 2).OrderByDescending(i => i.AoeTargetsHitCount).FirstOrDefault();
            if (EQprediction == null)
                return;
            FlashPos = EQprediction.CastPosition;

            if (ObjectManager.Player.IsDashing() && YasuoHelper.HaveQ3 && Q.IsReady() && FlashPos != Vector3.Zero)
            {
                if (FlashPos.Distance(ObjectManager.Player.Position) <= 400 + 175 && FlashPos.Distance(ObjectManager.Player.Position) >= 175)
                {
                    if (Q3.Cast(YasuoHelper.PosExploit(target)))
                    {
                        if (EQFlash.Cast(FlashPos)) return;
                    }
                }
            }

            if (!ObjectManager.Player.IsDashing() && YasuoHelper.HaveQ3 && Q.IsReady() && FlashPos != Vector3.Zero)
            {
                if (FlashPos.Distance(ObjectManager.Player.Position) <= 400 + 175 && FlashPos.Distance(ObjectManager.Player.Position) >= 175)
                {
                    var obj = ObjectManager.Get<AIBaseClient>().Where(i => !i.IsAlly && !i.IsDead && !i.Position.IsBuilding() && YasuoHelper.CanE(i) && i.IsValidTarget(E.Range)).FirstOrDefault();

                    if (obj != null)
                        if (E1.Cast(obj) == CastStates.SuccessfullyCasted || E1.CastOnUnit(obj))
                            return;
                }
            }
            //}

            return;
        }

        private static void Yasuo_DoCombo()
        {
            var heroes = ObjectManager.Get<AIHeroClient>().Where(i => !i.IsDead && !i.IsAlly && i.DistanceToPlayer() <= 1400);

            if (heroes.FirstOrDefault() != null)
            {
                var target = heroes.Where(i => i.IsValidTarget(R.Range) && YasuoMenu.Rcombo.RtargetHeath.Value >= i.HealthPercent && (i.HasBuffOfType(BuffType.Knockup) || i.HasBuffOfType(BuffType.Knockback))).OrderBy(i => i.Health).FirstOrDefault();

                if (R.IsReady() && YasuoMenu.Rcombo.Yasuo_Rcombo.Enabled && target != null)
                {
                    var buff = target.Buffs.FirstOrDefault(i => i.Type == BuffType.Knockback || i.Type == BuffType.Knockup);
                    if ((buff.EndTime - Game.Time) * 1000 <= YasuoMenu.Rcombo.RDelayTime.Value && !YasuoMenu.Rcombo.AlwaysEQR.Enabled)
                    {
                        {
                            R.Cast(target.Position);
                            Game.Print("R accepted");
                            return;
                        }
                    }
                    if (Q.IsReady() && E.IsReady())
                    {
                        var EQR = GameObjects.Get<AIBaseClient>().Where(i => i.IsValidTarget(E.Range) && !i.IsDead && !i.IsAlly && !i.HasBuff("YasuoE"));

                        if (EQR != null)
                        {
                            //foreach (var obj in EQR)
                            //{
                            var obj = EQR.FirstOrDefault();
                            if (E1.Cast(obj) == CastStates.SuccessfullyCasted)
                            {
                                {
                                    Q.Cast(YasuoHelper.PosExploit(null));
                                    R.Cast(target.Position);
                                }
                            }
                            //}
                        }
                    }
                    if (Q.IsReady() && ObjectManager.Player.IsDashing())
                    {
                        {
                            Q.Cast(YasuoHelper.PosExploit(null));
                            R.Cast(target.Position);
                        }
                    }
                }
            }

            QcastTarget();

            if (!FunnySlayerCommon.OnAction.BeforeAA && !FunnySlayerCommon.OnAction.OnAA)
                Egaptarget();

            #region hmm
            /*foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(2000)))
            {
                if (target == null)
                    return;
                if (target != null || target1 != null)
                {
                    if ((target.HasBuffOfType(BuffType.Knockup) || target.HasBuffOfType(BuffType.Knockback)) && R.IsReady() && target.IsValidTarget(R.Range) && YasuoMenu.Rcombo.RtargetHeath.Value >= target.HealthPercent && YasuoMenu.Rcombo.Yasuo_Rcombo.Enabled)
                    {
                        var buff = target.Buffs.FirstOrDefault(i => i.Type == BuffType.Knockback || i.Type == BuffType.Knockup);
                        if ((buff.EndTime - Game.Time) * 1000 <= YasuoMenu.Rcombo.RDelayTime.Value && !YasuoMenu.Rcombo.AlwaysEQR.Enabled)
                        {
                            Game.Print("R accepted");
                            if (R.Cast(target.Position))
                                return;
                        }
                        if (Q.IsReady() && E.IsReady())
                        {
                            var EQR = GameObjects.Get<AIBaseClient>().Where(i => i.IsValidTarget(E.Range) && !i.IsDead && !i.IsAlly && !i.HasBuff("YasuoE"));

                            if (EQR.Any())
                            {
                                foreach (var obj in EQR)
                                {
                                    if(E1.Cast(obj) == CastStates.SuccessfullyCasted)
                                    {
                                        if (Q.Cast(YasuoHelper.PosExploit(null)))
                                        {
                                            if (R.Cast(target.Position))
                                                return;
                                        }
                                    }
                                }
                            }
                        }
                        if (Q.IsReady() && ObjectManager.Player.IsDashing())
                        {
                            if (Q.Cast(YasuoHelper.PosExploit(null)))
                            {
                                if (R.Cast(target.Position))
                                    return;
                            }
                        }
                    }
                    else
                    {
                        Egaptarget();
                        if (YasuoHelper.HaveQ3)
                        {
                            if (Q3.GetPrediction(target1).CastPosition.DistanceToPlayer() <= YasuoMenu.RangeCheck.Q3range.Value)
                            {
                                QcastTarget();
                            }
                            else
                            {
                                QcastTarget();
                            }
                        }
                        else
                        {
                            if (Q.GetPrediction(target1).CastPosition.DistanceToPlayer() <= YasuoMenu.RangeCheck.Qrange.Value)
                            {
                                QcastTarget();
                            }
                            else
                            {
                                QcastTarget();
                            }
                        }
                    }
                }
            }*/
            #endregion
        }
        private static void YasuoJungle()
        {
            if (OnAction.OnAA)
                return;

            //jungleclear
            var jungles = GameObjects.Jungle.Where(i => i.DistanceToPlayer() <= (YasuoHelper.HaveQ3 ? 1100 : 475) && !i.IsDead);
            if (jungles.FirstOrDefault() == null)
                return;

            if (E.IsReady())
            {
                var jungleEon = jungles.Where(i => i.DistanceToPlayer() < 475 && i.DistanceToPlayer() > 240 && YasuoHelper.CanE(i)).FirstOrDefault();
                if (jungleEon != null)
                    if (E1.Cast(jungleEon) == CastStates.SuccessfullyCasted)
                        return;
            }

            if (Q.IsReady())
            {
                if (ObjectManager.Player.IsDashing())
                {
                    if (YasuoHelper.HaveQ2)
                    {
                        if (Q.Cast(YasuoHelper.PosExploit(null)))
                            return;
                    }
                    else
                    {
                        if (jungles.Where(i => i.Distance(ObjectManager.Player.GetDashInfo().EndPos) < 185).FirstOrDefault() != null)
                        {
                            if (Q.Cast(YasuoHelper.PosExploit(null)))
                                return;
                        }
                    }
                }


                {
                    var getjungleQ = jungles.FirstOrDefault();
                    if (getjungleQ != null)
                    {
                        if (YasuoHelper.HaveQ3)
                        {
                            var pout = FSpred.Prediction.Prediction.GetPrediction(Q3, getjungleQ);
                            if (pout.Hitchance >= FSpred.Prediction.HitChance.High && pout.UnitPosition.DistanceToPlayer() <= 1080)
                                Q3.Cast(pout.CastPosition);
                            return;
                        }
                        else
                        {
                            var pout = FSpred.Prediction.Prediction.GetPrediction(Q, getjungleQ);
                            if (pout.Hitchance >= FSpred.Prediction.HitChance.High && pout.UnitPosition.DistanceToPlayer() <= 475)
                                Q.Cast(pout.CastPosition);
                            return;
                        }
                    }
                }
            }
        }
        private static void Yasuo_DoClear()
        {
            //var Obj = new ObjectManager();

            if (FunnySlayerCommon.OnAction.OnAA)
                return;
            //laneclear
            var Qminions = ObjectManager.Get<AIBaseClient>().Where(i => !i.IsAlly && !i.IsDead && (i.Type == GameObjectType.AIHeroClient || i.IsMinion() || i.IsJungle()) && i.IsValidTarget(YasuoHelper.HaveQ3 ? YasuoMenu.RangeCheck.Q3range.Value : YasuoMenu.RangeCheck.Qrange.Value) && !i.Position.IsBuilding());
            var Eminions = GameObjects.EnemyMinions.Where(i => i.IsValidTarget(E.Range) && YasuoHelper.CanE(i));
            if (Qminions != null && YasuoMenu.Yasuo_Clear.Yasuo_Qclear.Enabled && Q.IsReady())
            {
                var min = Qminions.OrderBy(i => i.Health).ThenBy(i => i.Type == GameObjectType.AIHeroClient).FirstOrDefault();
                if (min != null)
                {
                    if (ObjectManager.Player.IsDashing())
                    {
                        YasuoMenu.Yasuo_Keys.AutoQifDashOnTarget.Active = true;
                    }
                    else
                    {
                        //YasuoMenu.Yasuo_Keys.AutoQifDashOnTarget.Active = false;
                        if (!min.IsMinion())
                        {
                            if (YasuoHelper.UnderTower(ObjectManager.Player.Position) && min.HealthPercent >= 30) return;

                            else
                            {
                                if (YasuoHelper.HaveQ3)
                                {
                                    if (!YasuoMenu.Yasuo_Clear.Yasuo_Windclear.Enabled) return;

                                    else
                                    {
                                        var pout = FSpred.Prediction.Prediction.GetPrediction(Q3, min);

                                        Q3.Cast(pout.CastPosition);
                                        return;
                                    }
                                }
                                else
                                {
                                    var pout = FSpred.Prediction.Prediction.GetPrediction(Q, min);

                                    Q3.Cast(pout.CastPosition);
                                    return;
                                }
                            }
                        }
                        else
                        {
                            if (YasuoHelper.HaveQ3)
                            {
                                if (!YasuoMenu.Yasuo_Clear.Yasuo_Windclear.Enabled) return;

                                else
                                {
                                    var qFarm = Q3.GetLineFarmLocation(Qminions.ToList());

                                    if (qFarm.MinionsHit >= YasuoMenu.Yasuo_Clear.Yasuo_Windclear_ifhit.Value)
                                    {
                                        Q3.Cast(qFarm.Position);
                                    }
                                }
                            }
                            else
                            {
                                var qFarm = Q.GetLineFarmLocation(Qminions.ToList());

                                if (qFarm.MinionsHit >= 1)
                                {
                                    Q.Cast(qFarm.Position);
                                }
                            }
                        }
                    }
                }
            }
            if (Eminions != null && YasuoMenu.Yasuo_Clear.Yasuo_Eclear.Enabled && E.IsReady())
            {
                var min = Eminions.OrderBy(i => i.Health).FirstOrDefault();
                if (min != null)
                {
                    if (min.Health <= E.GetDamage(min))
                    {
                        if (YasuoMenu.Yasuo_Keys.TurretKey.Active)
                        {
                            if (E1.Cast(min) == CastStates.SuccessfullyCasted || E1.CastOnUnit(min))
                                return;
                        }
                        else
                        {
                            if (YasuoHelper.UnderTower(YasuoHelper.PosAfterE(min))) return;

                            else
                            {
                                if (E1.Cast(min) == CastStates.SuccessfullyCasted || E1.CastOnUnit(min))
                                    return;
                            }
                        }
                    }
                }

                if (Eminions.All(i => !i.IsValidTarget(350)) && !YasuoMenu.Yasuo_Clear.Yasuo_Eclear_ks.Enabled)
                {
                    if (ObjectManager.Player.IsDashing())
                    {
                        YasuoMenu.Yasuo_Keys.AutoQifDashOnTarget.Active = true;
                    }

                    if (min != null)
                    {
                        if (YasuoMenu.Yasuo_Keys.TurretKey.Active)
                        {
                            if (E1.Cast(min) == CastStates.SuccessfullyCasted || E1.CastOnUnit(min))
                                return;
                        }
                        else
                        {
                            if (YasuoHelper.UnderTower(YasuoHelper.PosAfterE(min))) return;

                            else
                            {
                                if (E1.Cast(min) == CastStates.SuccessfullyCasted || E1.CastOnUnit(min))
                                    return;
                            }
                        }
                    }
                }
            }
        }

        private static void Yasuo_DoHarass()
        {
            var targets = ObjectManager.Get<AIHeroClient>().Where(i => !i.IsAlly && !i.IsDead && i.IsValidTarget(YasuoHelper.HaveQ3 ? Q3.Range : Q.Range));

            if (targets != null)
            {
                //foreach (var target in targets)
                //{
                var target = targets.OrderBy(i => i.Health).ThenBy(i => (YasuoHelper.HaveQ3 ? Q3 : Q).FSPredCastPos(i).DistanceToPlayer()).FirstOrDefault();
                var obj = YasuoHelper.GetNearObj(target);
                if (target != null)
                {
                    if (obj != null)
                    {
                        if (E.IsReady() && Q.IsReady() && (YasuoHelper.HaveQ2 || YasuoHelper.HaveQ3) &&
                                                YasuoHelper.PosAfterE(obj).Distance(YasuoHelper.Eprediction(target)) <= YasuoMenu.RangeCheck.EQrange.Value && !YasuoHelper.UnderTower(YasuoHelper.PosAfterE(obj))
                                                )
                        {
                            if (E1.Cast(obj) == CastStates.SuccessfullyCasted)
                            {
                                YasuoMenu.Yasuo_Keys.AutoQifDashOnTarget.Active = true;
                                return;
                            }
                        }
                        else
                        {
                            if (target.IsValidTarget(YasuoHelper.HaveQ3 ? Q3.Range : Q.Range) && Q.IsReady() && !ObjectManager.Player.IsDashing())
                            {
                                if (YasuoHelper.HaveQ3)
                                {
                                    var pout = FSpred.Prediction.Prediction.GetPrediction(Q3, target);
                                    if (pout.Hitchance >= FSpred.Prediction.HitChance.High && pout.UnitPosition.DistanceToPlayer() <= 1080)
                                        Q3.Cast(pout.CastPosition);
                                    return;
                                }
                                else
                                {
                                    var pout = FSpred.Prediction.Prediction.GetPrediction(Q, target);
                                    if (pout.Hitchance >= FSpred.Prediction.HitChance.High && pout.UnitPosition.DistanceToPlayer() <= 475)
                                        Q.Cast(pout.CastPosition);
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (target.IsValidTarget(YasuoHelper.HaveQ3 ? Q3.Range : Q.Range) && Q.IsReady() && !ObjectManager.Player.IsDashing())
                        {
                            if (YasuoHelper.HaveQ3)
                            {
                                var pout = FSpred.Prediction.Prediction.GetPrediction(Q3, target);
                                if (pout.Hitchance >= FSpred.Prediction.HitChance.High && pout.UnitPosition.DistanceToPlayer() <= 1080)
                                    Q3.Cast(pout.CastPosition);
                                return;
                            }
                            else
                            {
                                var pout = FSpred.Prediction.Prediction.GetPrediction(Q, target);
                                if (pout.Hitchance >= FSpred.Prediction.HitChance.High && pout.UnitPosition.DistanceToPlayer() <= 475)
                                    Q.Cast(pout.CastPosition);
                                return;
                            }
                        }
                    }
                }

                //}
            }

            var minions = ObjectManager.Get<AIMinionClient>().Where(i => !i.IsAlly && !i.IsDead && i.DistanceToPlayer() <= Q.Range && i.MaxHealth > 5 && i.Health < Q.GetDamage(i)).FirstOrDefault();
            if (minions != null)
            {
                var min = minions;
                if (min != null && Q.IsReady() && !YasuoHelper.HaveQ3)
                {
                    if (Q.Cast(min.Position))
                        return;
                }
                //foreach (var min in minions)
                //{

                //}
            }
        }

        private static void Egaptarget()
        {
            var target = FSTargetSelector.GetFSTarget(YasuoMenu.RangeCheck.Egaprange.Value);
            if (target == null || OnAction.OnAA || OnAction.BeforeAA) return;
            if (!YasuoMenu.Ecombo.Yasuo_ERange.Enabled)
                return;
            var obj = YasuoHelper.GetNearObj(target);


            if (obj != null && E1.Level >= 1)
            {
                if (YasuoMenu.Ecombo.Yasuo_Eziczac.Enabled && E.Level >= 1)
                {
                    LogicEZicZac(target);
                }
                if (!YasuoHelper.UnderTower(YasuoHelper.PosAfterE(obj)) || YasuoMenu.Yasuo_Keys.TurretKey.Active)
                {
                    if (obj.NetworkId == target.NetworkId)
                    {
                        if (obj.DistanceToPlayer() >= 250)
                        {
                            E1.Cast(obj);
                            return;
                        }
                    }
                    else
                    {
                        E1.Cast(obj);
                        return;
                    }
                }
            }
        }

        private static void LogicEZicZac(AIBaseClient target)
        {
            // return if on auto attack
            if (FunnySlayerCommon.OnAction.OnAA || FunnySlayerCommon.OnAction.BeforeAA || !E.IsReady()) return;

            if (target == null || !target.IsValidTarget(YasuoMenu.Ecombo.Yasuo_zizzacRange.Value)) return;

            if (YasuoHelper.UnderTower(ObjectManager.Player.Position)) return;

            if (!YasuoMenu.Ecombo.Yasuo_Eziczac.Enabled) return;

            if (YasuoMenu.Ecombo.Yasuo_Eziczac_Qready.Enabled && Q.IsReady()) return;

            AIBaseClient obj1 = null;
            AIBaseClient obj2 = null;
            bool ready = false;

            //search
            var AllObj = ObjectManager.Get<AIBaseClient>().Where(i => !i.IsDead && !i.IsAlly && E1.CanCast(i) && YasuoHelper.CanE(i));

            //MenuChecker
            if (YasuoMenu.Ecombo.Yasuo_zizzacRange.Value < target.DistanceToPlayer()) return;

            if (YasuoMenu.Ecombo.Yasuo_EziczacMode.Index == 0)
            {
                if (YasuoHelper.GetNearObj(target) == null) return;
            }
            if (YasuoMenu.Ecombo.Yasuo_EziczacMode.Index == 1)
            {
                if (ObjectManager.Player.CountEnemyHeroesInRange(YasuoMenu.Ecombo.Yasuo_zizzacRange.Value) <= 1) return;
            }
            if (YasuoMenu.Ecombo.Yasuo_EziczacMode.Index == 2)
            {
                if (
                    !target.CanMove
                    || !target.CanAttack
                    ) return;

                if (
                    target.Armor < ObjectManager.Player.Armor
                     || target.IsRanged
                    ) return;

                //Some Champion
                if (
                    target.CharacterName == "Shaco"
                    || target.CharacterName == "Fizz"
                    || target.CharacterName == "Akali"
                    || target.CharacterName == "Camille"
                    || target.CharacterName == "Gangplank"
                    || target.CharacterName == "Graves"
                    || target.CharacterName == "Hecarim"
                    || target.CharacterName == "Katarina"
                    || target.CharacterName == "Poppy"
                    ) return;
            }
            if (YasuoMenu.Ecombo.Yasuo_EziczacMode.Index == 3)
            {
                //nothing happaned
            }

            //set
            if (AllObj.Count() < 2) return;

            foreach (var aobj in AllObj)
            {
                obj1 = aobj;


                obj2 = AllObj.Where(i => (YasuoHelper.PosAfterE(obj1).Distance(i) < 475 && obj1.IsValidTarget(E.Range)) || (YasuoHelper.PosAfterE(i).Distance(obj1) < 475 && i.IsValidTarget(E.Range))
                && obj1.NetworkId != i.NetworkId).OrderBy(i => i.DistanceToPlayer()).FirstOrDefault();

                //ready ?
                if (obj1 != null && obj2 != null)
                {
                    ready = true;
                }
                else
                {
                    ready = false;
                }

                //logic
                if (ready == true)
                {
                    var ziczacpos1 = YasuoHelper.PosAfterE(obj1).Extend(YasuoHelper.Eprediction(obj2), 475);
                    var ziczacpos2 = YasuoHelper.PosAfterE(obj2).Extend(YasuoHelper.Eprediction(obj1), 475);

                    if (ziczacpos1.Distance(YasuoHelper.Eprediction(target)) <= YasuoMenu.RangeCheck.EQrange.Value || ziczacpos1.Distance(YasuoHelper.Eprediction(target)) <= YasuoHelper.Eprediction(target).DistanceToPlayer())
                    {
                        if (YasuoMenu.Yasuo_Keys.TurretKey.Active || !YasuoHelper.UnderTower(YasuoHelper.PosAfterE(obj1)))
                            if (obj1.IsValidTarget(E.Range))
                                if (E1.Cast(obj1) == CastStates.SuccessfullyCasted || E1.CastOnUnit(obj1))
                                    return;
                                else return;
                            else return;
                    }
                    else
                    if (ziczacpos2.Distance(YasuoHelper.Eprediction(target)) <= YasuoMenu.RangeCheck.EQrange.Value || ziczacpos2.Distance(YasuoHelper.Eprediction(target)) <= YasuoHelper.Eprediction(target).DistanceToPlayer())
                    {
                        if (YasuoMenu.Yasuo_Keys.TurretKey.Active || !YasuoHelper.UnderTower(YasuoHelper.PosAfterE(obj2)))
                            if (obj2.IsValidTarget(E.Range))
                                if (E1.Cast(obj2) == CastStates.SuccessfullyCasted || E1.CastOnUnit(obj2))
                                    return;
                                else return;
                            else return;
                    }
                }
            }
        }

        private static void QcastTarget()
        {
            var target = FSTargetSelector.GetFSTarget(YasuoHelper.HaveQ3 ? Q3.Range : Q.Range);
            if (target == null || !Q.IsReady() || FunnySlayerCommon.OnAction.OnAA) return;

            var obj = YasuoHelper.GetNearObj(target);


            if (obj != null && E1.Level >= 1 && (!YasuoHelper.UnderTower(YasuoHelper.PosAfterE(obj)) || YasuoMenu.Yasuo_Keys.TurretKey.Active) && (obj.DistanceToPlayer() >= 250 || obj.Type != GameObjectType.AIHeroClient))
            {

            }
            else
            {
                if (!CheckImDashing && !ObjectManager.Player.IsDashing())
                {
                    CastQNormal(target);
                }
                else
                {
                    var EQtarget = GameObjects.EnemyHeroes.Where(i => !i.IsDead && !i.IsAlly && i.DistanceToPlayer() <= 200).OrderBy(i => i.DistanceToPlayer()).FirstOrDefault();
                    if (EQtarget != null)
                        if (ObjectManager.Player.GetDashInfo().EndTick - Variables.GameTimeTickCount < 200)
                            CastQcircle(EQtarget);
                }
            }
        }

        private static void CastQNormal(AIBaseClient target)
        {
            if (CheckImDashing || ObjectManager.Player.IsDashing())
                return;

            if (YasuoHelper.HaveQ3)
            {
                var targetpred = FSpred.Prediction.Prediction.GetPrediction(Q3, target);
                if(target != null && targetpred.Hitchance >= FSpred.Prediction.HitChance.High)
                {
                    Q3.Cast(targetpred.CastPosition);
                }
                else
                {
                    var findqtarget = ObjectManager.Get<AIHeroClient>().Where(i =>
                    i.IsValidTarget(YasuoMenu.RangeCheck.Q3range.Value)
                    && !i.IsDead
                    && !i.IsAlly
                    && FSpred.Prediction.Prediction.GetPrediction(Q3, i).Hitchance
                    >= FSpred.Prediction.HitChance.High)
                        .OrderBy(i => i.Health)
                        .FirstOrDefault();


                    //var newtarget = 
                    var pout = FSpred.Prediction.Prediction.GetPrediction(Q3, findqtarget);
                    if (pout.Hitchance >= FSpred.Prediction.HitChance.High && pout.UnitPosition.DistanceToPlayer() <= 1080)
                        Q3.Cast(pout.CastPosition);
                    return;
                }                 
            }
            else
            {
                if (!YasuoMenu.Qcombo.Yasuo_Qcombo.Enabled) return;

                var targetpred = FSpred.Prediction.Prediction.GetPrediction(Q, target);
                if (target != null && targetpred.Hitchance >= FSpred.Prediction.HitChance.High)
                {
                    Q.Cast(targetpred.CastPosition);
                }
                else
                {
                    var findqtarget = ObjectManager.Get<AIHeroClient>().Where(i =>
                    i.IsValidTarget(YasuoMenu.RangeCheck.Q3range.Value)
                    && !i.IsDead
                    && !i.IsAlly
                    && FSpred.Prediction.Prediction.GetPrediction(Q, i).Hitchance
                    >= FSpred.Prediction.HitChance.High)
                        .OrderBy(i => i.Health)
                        .FirstOrDefault();


                    //var newtarget = 
                    var pout = FSpred.Prediction.Prediction.GetPrediction(Q, findqtarget);
                    if (pout.Hitchance >= FSpred.Prediction.HitChance.High && pout.UnitPosition.DistanceToPlayer() <= 1080)
                        Q.Cast(pout.CastPosition);
                    return;
                }
            }
        }

        private static void CastQcircle(AIBaseClient target)
        {
            if (CheckImDashing == false)
                return;

            if (YasuoHelper.HaveQ3)
            {
                if (!YasuoMenu.EQCombo.Yasuo_EWindcombo.Enabled) return;

                if (ObjectManager.Player.GetDashInfo().EndPos.Distance(ObjectManager.Player.Position) < 200)
                {
                    if (ObjectManager.Player.GetDashInfo().EndPos.Distance(YasuoHelper.Eprediction(target)) <= YasuoMenu.RangeCheck.EQrange.Value)
                    {
                        Q.Cast(YasuoHelper.PosExploit(target));
                        return;
                    }
                }
            }
            else
            {
                if (!YasuoMenu.EQCombo.Yasuo_EQcombo.Enabled) return;
                if (YasuoHelper.HaveQ2)
                {
                    Q.Cast(YasuoHelper.PosExploit(target));
                    return;
                }
                else
                {
                    var Eobjs = new List<AIBaseClient>();
                    Eobjs.AddRange(ObjectManager.Get<AIBaseClient>().Where(i => i.Distance(ObjectManager.Player.GetDashInfo().EndPos) <= YasuoMenu.RangeCheck.EQrange.Value && !i.IsAlly));

                    if (target.DistanceToPlayer() > 450 && Eobjs != null)
                    {
                        Q.Cast(YasuoHelper.PosExploit(target));
                        return;
                    }
                    if (target.Distance(ObjectManager.Player.GetDashInfo().EndPos) < YasuoMenu.RangeCheck.EQrange.Value && ObjectManager.Player.GetDashInfo().EndPos.DistanceToPlayer() < 200)
                    {
                        Q.Cast(YasuoHelper.PosExploit(target));
                        return;
                    }
                }
            }
        }
        #endregion

        #region Onupdate 1
        private static void Game_OnUpdate1(EventArgs args)
        {

            if (ObjectManager.Player.IsDead) return;

            /*if (ObjectManager.Player.IsDashing())
            {
                Game.Print("Dashing Tick : " + ObjectManager.Player.GetDashInfo().StartTick + " in Game tick : " + Variables.GameTimeTickCount);Game.Print("Dashing Tick : " + ObjectManager.Player.GetDashInfo().StartTick + " in Game tick : " + Variables.GameTimeTickCount);
                Game.Print("Dashing End Tick : " + CheckDashingEndTime);
            }*/

            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    if (YasuoMenu.Yasuo_Keys.ComboEQ3Flash.Active)
                    {
                        EQFlashInCombo();
                    }
                    Yasuo_DoCombo();
                    return;
                case OrbwalkerMode.LaneClear:
                    Yasuo_DoClear();
                    YasuoJungle();
                    return;
                case OrbwalkerMode.Harass:
                    Yasuo_DoHarass();
                    return;
            }

            //E.Speed = 750f + 0.6f * ObjectManager.Player.MoveSpeed;
            //if(Orbwalker.ActiveMode == OrbwalkerMode.None)
            {
                if (YasuoMenu.Yasuo_Keys.Yasuo_Flee.Active || YasuoMenu.Yasuo_Keys.EQFlashKey.Active)
                {
                    //Orbwalker.AttackEnabled = false;
                    ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                }
                //else Orbwalker.AttackEnabled = true;

                /*if (YasuoMenu.Ecombo.ddtest.Enabled)
                {
                    if (ObjectManager.Player.IsDashing())
                    {
                        Orbwalker.MoveEnabled = false;
                        DelayAction.Add((int)(ObjectManager.Player.GetDashInfo().EndPos.DistanceToPlayer() / E.Speed * 1000 - 75), () =>
                        {
                            Orbwalker.MoveEnabled = true;
                        });
                    }
                    else
                    {
                        Orbwalker.MoveEnabled = true;
                    }
                }
                else
                {
                    Orbwalker.MoveEnabled = true;
                }*/

                if (YasuoMenu.Yasuo_Keys.Yasuo_Flee.Active && E.IsReady(0) && !FunnySlayerCommon.OnAction.OnAA)
                {
                    var obj = ObjectManager.Get<AIBaseClient>().Where(i => i.IsValidTarget(E.Range) && !i.IsAlly && YasuoHelper.CanE(i));
                    if (E.Level >= 1)
                    {
                        //foreach (AIBaseClient getobj in obj)
                        //{
                        var getobj = obj.FirstOrDefault();
                        if (getobj != null && YasuoHelper.CanE(getobj))
                        {
                            if (YasuoHelper.PosAfterE(getobj).DistanceToCursor() <= ObjectManager.Player.DistanceToCursor())
                            {
                                if (E1.Cast(getobj) == CastStates.SuccessfullyCasted || E1.CastOnUnit(getobj))
                                    return;
                            }
                            if (getobj.DistanceToCursor() <= 50)
                            {
                                if (E1.Cast(getobj) == CastStates.SuccessfullyCasted || E1.CastOnUnit(getobj))
                                    return;
                            }
                            if (getobj.DistanceToCursor() <= ObjectManager.Player.DistanceToCursor())
                            {
                                if (E1.Cast(getobj) == CastStates.SuccessfullyCasted || E1.CastOnUnit(getobj))
                                    return;
                            }
                        }
                        //}
                    }

                    return;
                }

                if (YasuoMenu.Yasuo_Keys.EQFlashKey.Active && Flash.IsReady() && E.IsReady())
                {
                    YasuoEQFlash();

                    var obj = ObjectManager.Get<AIBaseClient>().Where(i => i.IsValidTarget(E.Range) && !i.IsAlly && YasuoHelper.CanE(i));
                    if (E.Level >= 1)
                    {
                        //foreach (AIBaseClient getobj in obj)
                        //{
                        var getobj = obj.FirstOrDefault();
                        if (getobj != null && YasuoHelper.CanE(getobj))
                        {
                            if (YasuoHelper.PosAfterE(getobj).DistanceToCursor() <= ObjectManager.Player.DistanceToCursor())
                            {
                                if (E1.Cast(getobj) == CastStates.SuccessfullyCasted || E1.CastOnUnit(getobj))
                                    return;
                            }
                            if (getobj.DistanceToCursor() <= 50)
                            {
                                if (E1.Cast(getobj) == CastStates.SuccessfullyCasted || E1.CastOnUnit(getobj))
                                    return;
                            }
                            if (getobj.DistanceToCursor() <= ObjectManager.Player.DistanceToCursor())
                            {
                                if (E1.Cast(getobj) == CastStates.SuccessfullyCasted || E1.CastOnUnit(getobj))
                                    return;
                            }
                        }
                        //}
                    }

                    return;
                }
            }
        }
        #endregion

        #region EQ flash
        private static void YasuoEQFlash()
        {
            if (ObjectManager.Player.IsDead)
                return;

            if (Flash == SpellSlot.Unknown || !EQFlash.IsReady() || !Flash.IsReady()) return;
            var targets = TargetSelector.GetTargets(850, DamageType.Magical);
            Vector3 FlashPos = Vector3.Zero;

            if (!targets.Any()) return;

            var target = TargetSelector.GetTarget(850, DamageType.Magical);

            if (target == null)
                return;

            if (targets.Count == 1)
            {
                FlashPos = target.Position;
                if (ObjectManager.Player.IsDashing() && YasuoHelper.HaveQ3 && Q.IsReady() && FlashPos != Vector3.Zero)
                {
                    if (FlashPos.Distance(ObjectManager.Player.Position) <= 400 + 175 && FlashPos.Distance(ObjectManager.Player.Position) > 150)
                    {
                        if (Q3.Cast(YasuoHelper.PosExploit(target)))
                        {
                            if (EQFlash.Cast(FlashPos))
                                return;
                        }
                    }
                }
            }

            if (targets.Count() >= 2)
            {
                var i = targets.OrderByDescending(e => EQFlash.GetPrediction(e, true).AoeTargetsHitCount).FirstOrDefault();
                //i =>
                if (i != null)
                {
                    FlashPos = EQFlash.GetPrediction(i, true).CastPosition;

                    if (ObjectManager.Player.IsDashing() && YasuoHelper.HaveQ3 && Q.IsReady() && FlashPos != Vector3.Zero)
                    {
                        if (FlashPos.Distance(ObjectManager.Player.Position) <= 400 + 175 && FlashPos.Distance(ObjectManager.Player.Position) > 150)
                        {
                            Q3.Cast(YasuoHelper.PosExploit(target));
                            DelayAction.Add(1, () => { EQFlash.Cast(FlashPos); });
                        }
                    }

                    if (!ObjectManager.Player.IsDashing() && target != null)
                    {
                        var obj = YasuoHelper.GetNearObj(target);
                        if (obj != null)
                        {
                            if (obj.NetworkId == target.NetworkId)
                            {
                                if (YasuoHelper.Eprediction(obj).DistanceToPlayer() > 300)
                                {
                                    if (E1.Cast(obj) == CastStates.SuccessfullyCasted || E1.CastOnUnit(obj))
                                        return;
                                }
                            }
                            else
                            {
                                if (E1.Cast(obj) == CastStates.SuccessfullyCasted || E1.CastOnUnit(obj))
                                    return;
                            }

                            if (YasuoHelper.Eprediction(target).Distance(YasuoHelper.PosAfterE(obj)) <= YasuoMenu.RangeCheck.EQrange.Value)
                            {
                                if (E1.Cast(obj) == CastStates.SuccessfullyCasted || E1.CastOnUnit(obj))
                                    return;
                            }
                        }
                    }
                }
            }
            else
            {
                FlashPos = target.Position;
                if (!ObjectManager.Player.IsDashing() && target != null)
                {
                    var obj = YasuoHelper.GetNearObj(target);
                    if (obj != null)
                    {
                        if (obj.NetworkId == target.NetworkId)
                        {
                            if (YasuoHelper.Eprediction(obj).DistanceToPlayer() > 300)
                            {
                                if (E1.Cast(obj) == CastStates.SuccessfullyCasted || E1.CastOnUnit(obj))
                                    return;
                            }
                        }
                        else
                        {
                            if (E1.Cast(obj) == CastStates.SuccessfullyCasted || E1.CastOnUnit(obj))
                                return;
                        }

                        if (YasuoHelper.Eprediction(target).Distance(YasuoHelper.PosAfterE(obj)) <= YasuoMenu.RangeCheck.EQrange.Value)
                        {
                            if (E1.Cast(obj) == CastStates.SuccessfullyCasted || E1.CastOnUnit(obj))
                                return;
                        }
                    }
                }

                if (ObjectManager.Player.IsDashing() && YasuoHelper.HaveQ3 && Q.IsReady() && FlashPos != Vector3.Zero)
                {
                    if (FlashPos.Distance(ObjectManager.Player.Position) <= 600 && FlashPos.Distance(ObjectManager.Player.Position) > 125)
                    {
                        if (Q3.Cast(YasuoHelper.PosExploit(target)))
                        {
                            if (EQFlash.Cast(FlashPos))
                                return;
                        }
                    }
                }
            }
        }
        #endregion

        #region Onupdate
        #endregion

        #region Evade
        public class EvadeTarget
        {
            #region Static Fields

            private static readonly List<Targets> DetectedTargets = new List<Targets>();

            private static readonly List<SpellData> Spells = new List<SpellData>();

            private static Vector2 wallCastedPos;

            #endregion

            #region Properties

            private static GameObject Wall
            {
                get
                {
                    return
                        ObjectManager.Get<GameObject>()
                            .FirstOrDefault(
                                i => i.IsValid && i.Name.Contains("_w_windwall"));
                }
            }

            #endregion

            #region Public Methods and Operators
            private static Menu evadeMenu;
            public static void Init(Menu menu)
            {
                LoadSpellData();
                evadeMenu = new Menu("EvadeTarget", "Evade Target");
                {
                    evadeMenu.Add(new MenuBool("W", "Use W"));
                    evadeMenu.Add(new MenuBool("E", "Use E (To Dash Behind WindWall)"));
                    evadeMenu.Add(new MenuBool("ETower", "-> Under Tower", false));
                    evadeMenu.Add(new MenuBool("BAttack", "Basic Attack"));
                    evadeMenu.Add(new MenuSlider("BAttackHpU", "-> If Hp <", 35));
                    evadeMenu.Add(new MenuBool("CAttack", "Crit Attack"));
                    evadeMenu.Add(new MenuSlider("CAttackHpU", "-> If Hp <", 40));
                    foreach (var hero in
                        GameObjects.EnemyHeroes.Where(i => Spells.Any(a => a.CharacterName == i.CharacterName)))
                    {
                        evadeMenu.Add(new Menu("ET_" + hero.CharacterName, "-> " + hero.CharacterName));
                    }
                    foreach (
                        var spell in Spells.Where(i => GameObjects.EnemyHeroes.Any(a => a.CharacterName == i.CharacterName)))
                    {

                        ((Menu)evadeMenu["ET_" + spell.CharacterName]).Add(new MenuBool(
                        spell.MissileName,
                        spell.MissileName + " (" + spell.Slot + ")"));
                    }
                }
                menu.Add(evadeMenu);
                Game.OnUpdate += OnUpdateTarget;
                GameObject.OnCreate += ObjSpellMissileOnCreate;
                GameObject.OnDelete += ObjSpellMissileOnDelete;
                //AIBaseClient.OnDoCast += OnProcessSpellCast;
            }

            #endregion

            #region Methods

            private static bool GoThroughWall(Vector2 pos1, Vector2 pos2)
            {
                /*if (Wall == null)
                {
                    return false;
                }
                var wallWidth = 300 + 50 * Convert.ToInt32(Wall.Name.Substring(Wall.Name.Length - 6, 1));
                var wallDirection = (Wall.Position.ToVector2() - wallCastedPos).Normalized().Perpendicular();
                var wallStart = Wall.Position.ToVector2() + wallWidth / 2f * wallDirection;
                var wallEnd = wallStart - wallWidth * wallDirection;
                var wallPolygon = new DaoHungAIO.Evade.Geometry.Polygon.Rectangle(wallStart, wallEnd, 75);
                var intersections = new List<Vector2>();
                for (var i = 0; i < wallPolygon.Points.Count; i++)
                {
                    var inter =
                        wallPolygon.Points[i].Intersection(
                            wallPolygon.Points[i != wallPolygon.Points.Count - 1 ? i + 1 : 0],
                            pos1,
                            pos2);
                    if (inter.Intersects)
                    {
                        intersections.Add(inter.Point);
                    }
                }*/
                return false;
            }

            private static void LoadSpellData()
            {
                Spells.Add(
                    new SpellData
                    { CharacterName = "Ahri", SpellNames = new[] { "ahrifoxfiremissiletwo" }, Slot = SpellSlot.W });
                Spells.Add(
                    new SpellData
                    { CharacterName = "Ahri", SpellNames = new[] { "ahritumblemissile" }, Slot = SpellSlot.R });
                Spells.Add(
                    new SpellData { CharacterName = "Anivia", SpellNames = new[] { "frostbite" }, Slot = SpellSlot.E });
                Spells.Add(
                    new SpellData { CharacterName = "Annie", SpellNames = new[] { "annieq" }, Slot = SpellSlot.Q });
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Brand",
                        SpellNames = new[] { "brandconflagrationmissile", "brandemissile" },
                        Slot = SpellSlot.E
                    });
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Brand",
                        SpellNames = new[] { "brandwildfire", "brandwildfiremissile", "brandr", "brandrmissile" },
                        Slot = SpellSlot.R
                    });
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Caitlyn",
                        SpellNames = new[] { "caitlynaceintheholemissile" },
                        Slot = SpellSlot.R
                    });
                Spells.Add(
                    new SpellData
                    { CharacterName = "Cassiopeia", SpellNames = new[] { "cassiopeiatwinfang", "cassiopeiae" }, Slot = SpellSlot.E });
                Spells.Add(
                    new SpellData { CharacterName = "Elise", SpellNames = new[] { "elisehumanq" }, Slot = SpellSlot.Q });
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Ezreal",
                        SpellNames = new[] { "ezrealarcaneshiftmissile" },
                        Slot = SpellSlot.E
                    });
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "FiddleSticks",
                        SpellNames = new[] { "fiddlesticksdarkwind", "fiddlesticksdarkwindmissile" },
                        Slot = SpellSlot.E
                    });
                Spells.Add(
                    new SpellData { CharacterName = "Gangplank", SpellNames = new[] { "parley", "gangplankqproceed" }, Slot = SpellSlot.Q });
                Spells.Add(
                    new SpellData { CharacterName = "Jhin", SpellNames = new[] { "jhinq", "jhinqmisbounce" }, Slot = SpellSlot.Q });
                Spells.Add(
                    new SpellData { CharacterName = "Janna", SpellNames = new[] { "sowthewind" }, Slot = SpellSlot.W });
                Spells.Add(
                    new SpellData { CharacterName = "Kassadin", SpellNames = new[] { "nulllance" }, Slot = SpellSlot.Q });
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Katarina",
                        SpellNames = new[] { "katarinaq", "katarinaqmis" },
                        Slot = SpellSlot.Q
                    });
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Katarina",
                        SpellNames = new[] { "katarinar", "katarinarmis" },
                        Slot = SpellSlot.R
                    });
                Spells.Add(
                    new SpellData
                    { CharacterName = "Kayle", SpellNames = new[] { "judicatorreckoning" }, Slot = SpellSlot.Q });
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Leblanc",
                        SpellNames = new[] { "leblancchaosorb", "leblancchaosorbm", "leblancq" },
                        Slot = SpellSlot.Q
                    });
                Spells.Add(new SpellData { CharacterName = "Lulu", SpellNames = new[] { "luluw", "luluwtwo" }, Slot = SpellSlot.W });
                Spells.Add(
                    new SpellData
                    { CharacterName = "Malphite", SpellNames = new[] { "seismicshard" }, Slot = SpellSlot.Q });
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "MissFortune",
                        SpellNames = new[] { "missfortunericochetshot", "missFortunershotextra" },
                        Slot = SpellSlot.Q
                    });
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Nami",
                        SpellNames = new[] { "namiwenemy", "namiwmissileenemy" },
                        Slot = SpellSlot.W
                    });
                Spells.Add(
                    new SpellData { CharacterName = "Nunu", SpellNames = new[] { "iceblast" }, Slot = SpellSlot.E });
                Spells.Add(
                    new SpellData { CharacterName = "Pantheon", SpellNames = new[] { "pantheonq" }, Slot = SpellSlot.Q });
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Ryze",
                        SpellNames = new[] { "spellflux", "spellfluxmissile", "ryzee" },
                        Slot = SpellSlot.E
                    });
                Spells.Add(
                    new SpellData { CharacterName = "Shaco", SpellNames = new[] { "twoshivpoison" }, Slot = SpellSlot.E });
                Spells.Add(
                    new SpellData { CharacterName = "Sona", SpellNames = new[] { "sonaqmissile" }, Slot = SpellSlot.Q });
                Spells.Add(
                    new SpellData { CharacterName = "Swain", SpellNames = new[] { "swaintorment" }, Slot = SpellSlot.E });
                Spells.Add(
                    new SpellData { CharacterName = "Syndra", SpellNames = new[] { "syndrar", "syndrarcasttime" }, Slot = SpellSlot.R });
                Spells.Add(
                    new SpellData { CharacterName = "Taric", SpellNames = new[] { "dazzle" }, Slot = SpellSlot.E });
                Spells.Add(
                    new SpellData { CharacterName = "Teemo", SpellNames = new[] { "blindingdart" }, Slot = SpellSlot.Q });
                Spells.Add(
                    new SpellData
                    { CharacterName = "Tristana", SpellNames = new[] { "detonatingshot" }, Slot = SpellSlot.E });
                Spells.Add(
                    new SpellData
                    { CharacterName = "Tristana", SpellNames = new[] { "tristanar" }, Slot = SpellSlot.R });
                Spells.Add(
                    new SpellData
                    { CharacterName = "TwistedFate", SpellNames = new[] { "bluecardattack" }, Slot = SpellSlot.W });
                Spells.Add(
                    new SpellData
                    { CharacterName = "TwistedFate", SpellNames = new[] { "goldcardattack" }, Slot = SpellSlot.W });
                Spells.Add(
                    new SpellData
                    { CharacterName = "TwistedFate", SpellNames = new[] { "redcardattack" }, Slot = SpellSlot.W });
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Urgot",
                        SpellNames = new[] { "urgotheatseekinghomemissile" },
                        Slot = SpellSlot.Q
                    });
                Spells.Add(
                    new SpellData { CharacterName = "Vayne", SpellNames = new[] { "vaynecondemn", "vaynecondemnmissile" }, Slot = SpellSlot.E });
                Spells.Add(
                    new SpellData
                    { CharacterName = "Veigar", SpellNames = new[] { "veigarprimordialburst", "veigarr" }, Slot = SpellSlot.R });
                Spells.Add(
                    new SpellData
                    { CharacterName = "Viktor", SpellNames = new[] { "viktorpowertransfer" }, Slot = SpellSlot.Q });
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Vladimir",
                        SpellNames = new[] { "vladimirtidesofbloodnuke" },
                        Slot = SpellSlot.E
                    });
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Lillia",
                        SpellNames = new[] { "Nothing" },
                        Slot = SpellSlot.R
                    });
            }

            private static void ObjSpellMissileOnCreate(GameObject sender, EventArgs args)
            {
                if (!(sender is MissileClient))
                {
                    return;
                }
                var missile = (MissileClient)sender;
                if (!(missile.SpellCaster is AIHeroClient) || missile.SpellCaster.IsAlly)
                {
                    return;
                }
                var unit = (AIHeroClient)missile.SpellCaster;

                var spellData =
                    Spells.FirstOrDefault(
                        i =>
                        {
                            return i.SpellNames.Contains(missile.SData.Name.ToLower())
                        && evadeMenu["ET_" + i.CharacterName][i.MissileName] != null
                        && evadeMenu["ET_" + i.CharacterName].GetValue<MenuBool>(i.MissileName).Enabled;
                        }
                        );
                if (spellData == null && Orbwalker.IsAutoAttack(missile.SData.Name)
                    && (!missile.SData.Name.ToLower().Contains("crit")
                            ? evadeMenu.GetValue<MenuBool>("BAttack").Enabled
                              && ObjectManager.Player.HealthPercent < evadeMenu.GetValue<MenuSlider>("BAttackHpU").Value
                            : evadeMenu.GetValue<MenuBool>("CAttack").Enabled
                              && ObjectManager.Player.HealthPercent < evadeMenu.GetValue<MenuSlider>("CAttackHpU").Value))
                {
                    spellData = new SpellData
                    { CharacterName = unit.CharacterName, SpellNames = new[] { missile.SData.Name } };
                }

                if (spellData == null || !missile.CastInfo.Target.IsMe)
                {
                    return;
                }
                DetectedTargets.Add(new Targets { Start = unit.Position, Obj = missile });
            }

            private static void ObjSpellMissileOnDelete(GameObject sender, EventArgs args)
            {
                if (!(sender is MissileClient))
                {
                    return;
                }
                var missile = (MissileClient)sender;
                if (missile.SpellCaster is AIHeroClient && !missile.SpellCaster.IsAlly)
                {
                    DetectedTargets.RemoveAll(i => i.Obj.NetworkId == missile.NetworkId);
                }
            }

            private static void OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
            {
                //if(sender.IsMe && args.Slot == SpellSlot.E)
                //{
                //    Q.CastOnUnit(Player);
                //}
                if (!sender.IsValid || sender.Team != ObjectManager.Player.Team || args.SData.Name != "YasuoWMovingWall")
                {
                    return;
                }
                wallCastedPos = sender.Position.ToVector2();
            }

            private static void OnUpdateTarget(EventArgs args)
            {
                if (ObjectManager.Player.IsDead)
                {
                    return;
                }

                /*if (ObjectManager.Player.HasBuffOfType(BuffType.SpellImmunity) || ObjectManager.Player.HasBuffOfType(BuffType.SpellShield))
                {
                    return;
                }*/

                if (!W.IsReady() && (Wall == null || !E.IsReady()))
                {
                    return;
                }

                foreach (var target in
                    DetectedTargets.Where(i => ObjectManager.Player.Distance(i.Obj.Position) < 700))
                {
                    if (W.IsReady() && evadeMenu.GetValue<MenuBool>("W").Enabled && W.IsInRange(target.Obj, 500)
                            && W.Cast(ObjectManager.Player.Position.Extend(target.Start, 100)))
                    {
                        return;
                    }
                }


                /*foreach (var target in
                    DetectedTargets.Where(i => ObjectManager.Player.Distance(i.Obj.Position) < 700))
                {
                    


                    if (E.IsReady() && evadeMenu.GetValue<MenuBool>("E").Enabled && Wall != null
                        && Variables.TickCount - W.LastCastAttemptTime > 1000
                        && !GoThroughWall(ObjectManager.Player.Position.ToVector2(), target.Obj.Position.ToVector2())
                        && W.IsInRange(target.Obj, 250))
                    {
                        var obj = new List<AIBaseClient>();
                        obj.AddRange(GameObjects.GetMinions(ObjectManager.Player.Position, E.Range, MinionTypes.All, MinionTeam.Enemy));
                        obj.AddRange(GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(E.Range)));
                        if (
                            obj.Where(
                                i =>
                                !i.HasBuff("YasuoE")
                                && (!YasuoHelper.UnderTower(YasuoHelper.PosAfterE(i)) || evadeMenu.GetValue<MenuBool>("ETower").Enabled)
                                && GoThroughWall(ObjectManager.Player.Position.ToVector2(), YasuoHelper.PosAfterE(i).ToVector2()))
                                .OrderBy(i => YasuoHelper.PosAfterE(i).Distance(Game.CursorPos))
                                .Any(i => E1.CastOnUnit(i)))
                        {
                            return;
                        }
                    }                    
                }*/
            }

            #endregion

            private class SpellData
            {
                #region Fields

                public string CharacterName;

                public SpellSlot Slot;

                public string[] SpellNames = { };

                #endregion

                #region Public Properties

                public string MissileName
                {
                    get
                    {
                        return this.SpellNames.First();
                    }
                }

                #endregion
            }

            private class Targets
            {
                #region Fields

                public MissileClient Obj;

                public Vector3 Start;

                #endregion
            }
        }
        #endregion
    }
}
