using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using SharpDX;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Events;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.Prediction;
using EnsoulSharp.SDK.Utility;
using SPrediction;

using Yasuo_LogicHelper;
using DominationAIO.DominationAIO.Common.YasuoSpellDatabase;

using static SPrediction.MinionManager;
using MinionTypes = SPrediction.MinionManager.MinionTypes;
using Color = System.Drawing.Color;
using System.Text.RegularExpressions;
using SpellDatabase = DaoHungAIO.Evade.SpellDatabase;
using SafePathResult = DaoHungAIO.Evade.SafePathResult;
using SkillshotDetector = DaoHungAIO.Evade.SkillshotDetector;
using Skillshot = DaoHungAIO.Evade.Skillshot;
using FoundIntersection = DaoHungAIO.Evade.FoundIntersection;
using EvadeManager = DaoHungAIO.Evade.EvadeManager;
using DetectionType = DaoHungAIO.Evade.DetectionType;
using CollisionObjectTypes = DaoHungAIO.Evade.CollisionObjectTypes;
using MinionTeam = SPrediction.MinionManager.MinionTeam;
using MinionOrderTypes = SPrediction.MinionManager.MinionOrderTypes;

namespace Yasuo
{
    internal class MenuSettings
    {
        #region Menu settings
        public static MenuBool usetargetselect = new MenuBool("usetargetselect", "Use New target select mode");
        public static MenuBool soloyasuo = new MenuBool("soloyasuo", "Turn On <Solo Yasuo> Mode") { Enabled = false };
        public static MenuKeyBind fleekey = new MenuKeyBind("fleekey", "Flee Key", System.Windows.Forms.Keys.E, KeyBindType.Press);
        public static MenuKeyBind EQ3Flash = new MenuKeyBind("Eq3Flash", "EQ Flash Key [: :Still under test ]", System.Windows.Forms.Keys.A, KeyBindType.Press);
        public static MenuBool resetautosetting = new MenuBool("resetautosetting", "Auto Setting by FunnySlayer(Recommend) ", false);
        public class Combo
        {
            public static MenuSeparator comboSeparator = new MenuSeparator("comboSeparator", "Combo Settings");
            public static MenuSeparator secsec = new MenuSeparator("secsec", "______________");
            public static MenuBool useQ = new MenuBool("useQ", "Use Q");
            public static MenuBool useW = new MenuBool("useW", "Use W");
            public static MenuBool useE = new MenuBool("useE", "Use E");
            public static MenuBool useEZicZac = new MenuBool("useEZizZac", "[Beta test] Combo E Zic Zac", false);
            public static MenuKeyBind Eturret = new MenuKeyBind("Eturret", "^ Use E gap when target in turret", System.Windows.Forms.Keys.T, KeyBindType.Toggle) { Active = false };
            public static MenuBool useR = new MenuBool("useR", "Use R");
            public static MenuSlider Rtarget = new MenuSlider("Rtarget", "^ Only use R if target hp < x%", 60, 0, 100);
            public static MenuSlider Rhit = new MenuSlider("Rhit", "^ Auto R if hit >= x enermy ", 3, 0, 5);
            public static MenuBool useIgnite = new MenuBool("useIgnite", "Use Ignite");
            public static MenuBool useEQ = new MenuBool("UseEQ", "^ Use EQ");
            public static MenuBool useEQ3 = new MenuBool("UseEQ3", "^ Use EQ3 logic");
            public static MenuBool Wblocklist = new MenuBool("Wblocklist", "^ Use W Evade");
            public static MenuBool Rdelay = new MenuBool("Rdelay", "^ Use R delay");
            public static MenuBool Rdelaylogic = new MenuBool("Rdelaylogic", "Use Combo EQ R");
            public static MenuBool qhextech = new MenuBool("qhextech", "Q đai lưng Hextech");
        }
        public class Harass
        {
            public static MenuSeparator harassSeparator = new MenuSeparator("harassSeparator", "Harass Settings");
            public static MenuSeparator secsec = new MenuSeparator("secsec", "______________");
            public static MenuBool useQ = new MenuBool("useQ", "Use Q");
            public static MenuBool useE = new MenuBool("useE", "Use E", false);
            public static MenuBool UseEQ = new MenuBool("UseEQ", "Use EQ");
            public static MenuSeparator Autoharass = new MenuSeparator("AutoHarass", "Auto Harass");
            public static MenuBool AutoHarassQ = new MenuBool("AutoHarassQ", "Auto Harass with Q", false);

        }
        public class LaneClear
        {
            public static MenuSeparator laneClearSeperator = new MenuSeparator("laneClearSeperator", "Lane Clear Settings");
            public static MenuSeparator secsec = new MenuSeparator("secsec", "______________");
            public static MenuBool useQ = new MenuBool("useQ", "Use Q");
            public static MenuSlider Qonly = new MenuSlider("Qonly", "Only use  if hit >= x minions", 1, 1, 5);
            public static MenuBool useE = new MenuBool("useE", "Use E");
            public static MenuList eMode = new MenuList("eMode", "Select Mode:", new[] { "Logic clear", "Only killable", "Fast clear" }, 1);
            public static MenuBool autoQ = new MenuBool("autoQ", "Auto Stack Q");
            public static MenuBool fastclearE = new MenuBool("fastclearE", "Use E fast clear", false);
            public static MenuBool Eturret = new MenuBool("Eturret", "Use E LaneClear under turret", false);
        }
        public class JungleClear
        {
            public static MenuSeparator jungleClearSeparator = new MenuSeparator("jungleClearSeparator", "Jungle Clear Settings");
            public static MenuSeparator secsec = new MenuSeparator("secsec", "______________");
            public static MenuBool useQ = new MenuBool("useQ", "Use Q");
            public static MenuBool useE = new MenuBool("useE", "Use E");


        }
        public class LastHit
        {
            public static MenuSeparator lastHitSeparator = new MenuSeparator("lastHitSeparator", "Last Hit Settings");
            public static MenuSeparator secsec = new MenuSeparator("secsec", "______________");
            public static MenuBool useQ = new MenuBool("useQ", "Use Q");
            public static MenuBool autoQ = new MenuBool("autoQ", "Auto Q");
            public static MenuBool useE = new MenuBool("useE", "Use E", false);

        }
        public class Misc
        {
            public static MenuSeparator miscSeparator = new MenuSeparator("miscSeparator", "Misc Settings");

            public static MenuKeyBind flee = new MenuKeyBind("flee", "Flee key", System.Windows.Forms.Keys.Space, KeyBindType.Press);
            public static MenuBool autoQ = new MenuBool("autoQ", "Use Auto Q");
            public static MenuBool interrupter = new MenuBool("interrupter", "Interrupter");
            public static MenuBool gapcloser = new MenuBool("gapcloser", "Gapcloser");
            public static MenuSeparator killStealSeparator = new MenuSeparator("killStealSeparator", "KillSteal Settings");
            public static MenuBool killstealEnable = new MenuBool("killstealEnable", "Enable");
            public static MenuBool killstealQ = new MenuBool("killstealQ", "Use Q");
            public static MenuBool killstealW = new MenuBool("killstealW", "Use W");
            public static MenuBool killstealE = new MenuBool("killstealE", "Use E");
            public static MenuBool killstealR = new MenuBool("killstealR", "Use R");
            public static MenuBool killstealIgnite = new MenuBool("killstealIgnite", "Use Ignite");

            public static MenuSeparator exploit = new MenuSeparator("Exploit", "Exploit Q2 and Q3");
            public static MenuBool eexploit = new MenuBool("eexploit", "Use BUG");
        }
        public class Drawing
        {
            public static MenuSeparator drawingSeparator = new MenuSeparator("drawingSeparator", "Drawings");
            public static MenuBool disableDrawings = new MenuBool("disableDrawings", "Disable", false);
            public static MenuBool drawAutoStack = new MenuBool("drawAutoStack", "Auto Stack Text");
            public static MenuBool drawDmg = new MenuBool("drawDmg", "Damage Indicator");
            public static MenuSeparator rangesSeperator = new MenuSeparator("rangesSeperator", "Spell Ranges");
            public static MenuBool drawQ = new MenuBool("drawQ", "Q Range");
            public static MenuBool drawW = new MenuBool("drawW", "W Range", false);
            public static MenuBool drawE = new MenuBool("drawE", "E Range", false);
            public static MenuBool drawR = new MenuBool("drawR", "R Range", false);
        }
        #endregion
    }
    internal class Yasuo : Logichelper
    {
        private static SpellSlot summonerIgnite;
        private static SpellSlot Flash;
        private static AIHeroClient objPlayer = ObjectManager.Player;
        private static Menu myMenu;
        public static float lastEtime, lastE;
        public static Menu EvadeSkillshotMenu = new Menu("EvadeSkillshot", "Evade Skillshot");
        
        public static void OnLoad()
        {
            Hacks.DisableAntiDisconnect = true;
            Console.OutputEncoding = Encoding.Unicode;
            Console.WriteLine("VinhKevin Yasuo loaded");
            #region TheSpell
            Q = new Spell(SpellSlot.Q, 475);
            Q2 = new Spell(SpellSlot.Q, 1070f);
            W = new Spell(SpellSlot.W, 100);
            E = new Spell(SpellSlot.E, 475);
            E1 = new Spell(SpellSlot.E, 475);
            R = new Spell(SpellSlot.R, 1400);
            R.SetSkillshot(0.70f, 125f, float.MaxValue, false, false, SkillshotType.Circle);

            summonerIgnite = ObjectManager.Player.GetSpellSlot("summonerdot");
            Flash = ObjectManager.Player.GetSpellSlot("summonerflash");
            Q.SetSkillshot(0.21f, 20, float.MaxValue, false, false, SkillshotType.Line);
            Q2.SetSkillshot(0.31f, 90, 1200, false, false, SkillshotType.Line);
            E.SetTargetted(0.05f, Espeeds());
            E1.SetSkillshot(0.5f, 175, Espeeds(), false, SkillshotType.Circle);
            Eq3flash = new Spell(Flash, 600f);
            #endregion

            Game.Print("FunnySlayer Yasuo, Good Luck");


            #region The Menu

            myMenu = new Menu(objPlayer.CharacterName, "FunnySlayer Yasuo", true);
            myMenu.Add(MenuSettings.soloyasuo).Permashow();
            myMenu.Add(MenuSettings.resetautosetting);
            var comboMenu = new Menu("comboMenu", "Combo");
            comboMenu.Add(MenuSettings.Combo.comboSeparator);
            comboMenu.Add(MenuSettings.usetargetselect).Permashow();
            comboMenu.Add(MenuSettings.Combo.secsec);
            comboMenu.Add(MenuSettings.Combo.useQ);
            comboMenu.Add(MenuSettings.Combo.useEQ);
            comboMenu.Add(MenuSettings.Combo.useEQ3);
            comboMenu.Add(MenuSettings.Combo.useW);
            comboMenu.Add(MenuSettings.Combo.Wblocklist);
            comboMenu.Add(MenuSettings.Combo.useE);
            comboMenu.Add(MenuSettings.Combo.useEZicZac);
            comboMenu.Add(MenuSettings.Combo.Eturret).Permashow();
            comboMenu.Add(MenuSettings.Combo.useR);
            comboMenu.Add(MenuSettings.Combo.Rtarget);
            comboMenu.Add(MenuSettings.Combo.Rhit);
            comboMenu.Add(MenuSettings.Combo.Rdelay);
            comboMenu.Add(MenuSettings.Combo.Rdelaylogic);
            comboMenu.Add(MenuSettings.Combo.useIgnite);
            myMenu.Add(comboMenu);

            var harassMenu = new Menu("harassMenu", "Harass");
            harassMenu.Add(MenuSettings.Harass.harassSeparator);
            harassMenu.Add(MenuSettings.Harass.secsec);
            harassMenu.Add(MenuSettings.Harass.useQ);
            harassMenu.Add(MenuSettings.Harass.useE);
            harassMenu.Add(MenuSettings.Harass.Autoharass);
            harassMenu.Add(MenuSettings.Harass.AutoHarassQ).Permashow();

            myMenu.Add(harassMenu);

            var laneClearMenu = new Menu("laneClearMenu", "Lane Clear");
            laneClearMenu.Add(MenuSettings.LaneClear.laneClearSeperator);
            laneClearMenu.Add(MenuSettings.LaneClear.secsec);
            laneClearMenu.Add(MenuSettings.LaneClear.useQ);
            laneClearMenu.Add(MenuSettings.LaneClear.useE);
            laneClearMenu.Add(MenuSettings.LaneClear.Eturret).Permashow();
            laneClearMenu.Add(MenuSettings.LaneClear.fastclearE);
            myMenu.Add(laneClearMenu);

            var jungleClearMenu = new Menu("jungleClearMenu", "Jungle Clear")
            {
                MenuSettings.JungleClear.jungleClearSeparator,
                MenuSettings.JungleClear.secsec,
                MenuSettings.JungleClear.useQ,
                MenuSettings.JungleClear.useE,
            };
            myMenu.Add(jungleClearMenu);

            var lastHitMenu = new Menu("lastHitMenu", "Last Hit")
            {
                MenuSettings.LastHit.lastHitSeparator,
                MenuSettings.LastHit.secsec,
                MenuSettings.LastHit.useQ,
                //MenuSettings.LastHit.autoQ,
                MenuSettings.LastHit.useE,
            };
            myMenu.Add(lastHitMenu);

            var miscMenu = new Menu("miscMenu", "Misc");
            var ksmenu = new Menu("ksmenu", "Kill Steal");
            ksmenu.Add(MenuSettings.Misc.killStealSeparator);
            ksmenu.Add(MenuSettings.Misc.killstealEnable);
            ksmenu.Add(MenuSettings.Misc.killstealQ);
            ksmenu.Add(MenuSettings.Misc.killstealW);
            ksmenu.Add(MenuSettings.Misc.killstealE);
            ksmenu.Add(MenuSettings.Misc.killstealR);
            ksmenu.Add(MenuSettings.Misc.killstealIgnite);
            miscMenu.Add(ksmenu);
            EvadeSkillshot.Init(miscMenu);
            EvadeTarget.Init(miscMenu);
            miscMenu.Add(MenuSettings.Misc.autoQ);
            miscMenu.Add(MenuSettings.fleekey).Permashow();
            miscMenu.Add(MenuSettings.EQ3Flash).Permashow();
            miscMenu.Add(MenuSettings.Combo.qhextech);
            miscMenu.Add(MenuSettings.Misc.miscSeparator);
            miscMenu.Add(MenuSettings.Misc.interrupter);
            miscMenu.Add(MenuSettings.Misc.gapcloser);
            miscMenu.Add(MenuSettings.Misc.exploit);
            miscMenu.Add(MenuSettings.Misc.eexploit).Permashow();
            myMenu.Add(miscMenu);

            var drawingMenu = new Menu("drawingMenu", "Drawings")
            {
                MenuSettings.Drawing.drawingSeparator,
                MenuSettings.Drawing.disableDrawings,
                MenuSettings.Drawing.drawDmg,
                MenuSettings.Drawing.rangesSeperator,
                MenuSettings.Drawing.drawQ,
                MenuSettings.Drawing.drawW,
                MenuSettings.Drawing.drawE,
                MenuSettings.Drawing.drawR,
            };
            myMenu.Add(drawingMenu);
            myMenu.Attach();

            #endregion

            #region The Events
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Drawing.OnEndScene += OnEndScene;
            Orbwalker.OnAction += OnAction;
            Interrupter.OnInterrupterSpell += OnInterrupterSpell;
            Gapcloser.OnGapcloser += OnGapcloser;
            AIBaseClient.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
            AIHeroClient.OnPlayAnimation += AIHeroClient_OnPlayAnimation;
            #endregion
        }
        #region Check Wall
        #endregion
        #region The AIHeroClient_OnPlayAnimation
        private static void AIHeroClient_OnPlayAnimation(AIBaseClient sender, AIBaseClientPlayAnimationEventArgs args)
        {
            try
            {
                /*if (sender.IsMe)
                {
                    if (args.Animation == "Spell3")
                    {
                        lastE = Variables.TickCount;
                    }
                    if (args.Animation == "Spell1_Dash")
                    {
                        Orbwalker.AttackState = false;
                        DelayAction.Add(300 + (Game.Ping / 2 + 10), () =>
                        {
                            Orbwalker.ResetAutoAttackTimer();
                            Orbwalker.AttackState = true;
                        });
                    }
                }*/
            }
            catch (Exception ex)
            {
                Game.Print(ex);
            }
        }
        #endregion

        #region The AIHeroClient_OnProcessSpellCast
        private static void AIHeroClient_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (sender.IsEnemy && !sender.IsMinion && sender.Distance(ObjectManager.Player.Position) <= Q2Range)
            {
                if (WBlockSpellDatabase.DangerousList.Any(spell => spell.Contains(args.SData.Name)))
                {
                    W.Cast(ObjectManager.Player.Position.Extend(args.Start, 100));
                }
                #region THE WSPELL
                if (args.SData.Name == "JavelinToss" ||
                    args.SData.Name ==  "AhriSeduce" ||
                    args.SData.Name == "BandageToss" ||
                    args.SData.Name ==  "FlashFrost" ||
                    args.SData.Name == "Disintegrate" ||
                    args.SData.Name == "Volley" ||
                    args.SData.Name == "EnchantedCrystalArrow"||
                    args.SData.Name == "BardQ" ||
                    args.SData.Name == "RocketGrab" ||
                    args.SData.Name == "BraumQ" ||
                    args.SData.Name == "BraumRWrapper" ||
                    args.SData.Name == "CaitlynPiltoverPeacemaker"||
                    args.SData.Name == "CaitlynEntrapment"||
                    args.SData.Name == "CaitlynAceintheHole"||
                    args.SData.Name == "PhosphorusBomb"||
                    args.SData.Name == "DianaArc"||
                    args.SData.Name == "InfectedCleaverMissileCast"||
                    args.SData.Name == "DravenDoubleShot"||
                    args.SData.Name == "DravenRCast"||
                    args.SData.Name == "EkkoQ"||
                    args.SData.Name == "EliseHumanE"||
                    args.SData.Name == "EzrealTrueshotBarrage"||
                    args.SData.Name == "FioraW"||
                    args.SData.Name == "FizzMarinerDoom"||
                    args.SData.Name == "GalioRighteousGust"||
                    args.SData.Name == "GnarQ"||
                    args.SData.Name == "GnarBigQ"||
                    args.SData.Name == "GragasR"||
                    args.SData.Name == "GravesQLineSpell"||
                    args.SData.Name == "GravesChargeShot"||
                    args.SData.Name == "HeimerdingerW"||
                    args.SData.Name == "HeimerdingerE"||
                    args.SData.Name == "IllaoiE"||
                    args.SData.Name == "IreliaTranscendentBlades"||
                    args.SData.Name == "HowlingGale"||
                    args.SData.Name == "JayceShockBlast"||
                    args.SData.Name == "JhinW"||
                    args.SData.Name == "JhinRShot"||
                    args.SData.Name == "JinxW"||
                    args.SData.Name == "SkillshotMissileLine"||
                    args.SData.Name == "KalistaMysticShot"||
                    args.SData.Name == "KarmaQMantra"||
                    args.SData.Name == "KennenShurikenHurlMissile1"||
                    args.SData.Name == "KhazixW"||
                    args.SData.Name == "LeblancSoulShackle"||
                    args.SData.Name == "BlindMonkQOne"||
                    args.SData.Name == "LissandraQ"||
                    args.SData.Name == "LuluQ"||
                    args.SData.Name == "LuxLightBinding"||
                    args.SData.Name == "DarkBindingMissile"||
                    args.SData.Name == "NamiR"||
                    args.SData.Name == "NautilusAnchorDrag"||
                    args.SData.Name == "OlafAxeThrowCast"||
                    args.SData.Name == "QuinnQ"||
                    args.SData.Name == "PoppyRSpell"||
                    args.SData.Name == "RengarE"||
                    args.SData.Name == "RumbleGrenade"||
                    args.SData.Name == "SejuaniGlacialPrisonStart"||
                    args.SData.Name == "SorakaE"||
                    args.SData.Name == "SonaR"||
                    args.SData.Name == "TahmKenchQ"||
                    args.SData.Name == "ThreshQ"||
                    args.SData.Name == "VarusQ"||
                    args.SData.Name == "VarusR"||
                    args.SData.Name == "ViktorDeathRay"||
                    args.SData.Name == "XerathMageSpear"||
                    args.SData.Name == "ZedQ"||
                    args.SData.Name == "ZileanQ"||
                    args.SData.Name == "ZyraGraspingRoots"||
                    args.SData.Name == "annieq"||
                    args.SData.Name == "frostbite"||
                    args.SData.Name == "brandwildfire"||
                    args.SData.Name == "brandwildfiremissile" ||
                    args.SData.Name == "caitlynaceintheholemissile"||
                    args.SData.Name == "katarinaqmis"||
                    args.SData.Name == "seismicshard"||
                    args.SData.Name == "syndrar"||
                    args.SData.Name == "blindingdart"||
                    args.SData.Name == "detonatingshot"||
                    args.SData.Name == "goldcardattack"||
                    args.SData.Name == "vaynecondemn"||
                    args.SData.Name == "veigarprimordialburst"||
                    args.SData.Name == "ZyraBrambleZone")
                {
                    W.Cast(ObjectManager.Player.Position.Extend(args.Start, 100));
                }
                #endregion
            }
            if(sender.IsMe)
            {
                if(args.SData.Name == "YasuoE")
                {
                    lastEtime = Variables.GameTimeTickCount;
                }
            }
        }
        #endregion

        #region The OnUpdate
        private static void OnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveMode == OrbwalkerMode.Combo)
            {
                comboactive = true;
            }
            else comboactive = false;
            if (MenuSettings.Misc.eexploit.Enabled)
            {
                exploitpos = -500000;
            }
            else
                exploitpos = 1;
            if(MenuSettings.resetautosetting.Enabled)
            {
                //combo
                MenuSettings.resetautosetting.Enabled = false;
                MenuSettings.Combo.useQ.Enabled = true;
                MenuSettings.Combo.useW.Enabled = true;
                MenuSettings.Combo.Wblocklist.Enabled = true;
                MenuSettings.Combo.useE.Enabled = true;
                MenuSettings.Combo.useEQ.Enabled = true;
                MenuSettings.Combo.useEQ3.Enabled = true;
                MenuSettings.Combo.useEZicZac.Enabled = false;
                MenuSettings.Combo.Eturret.Active = false;
                MenuSettings.Combo.useR.Enabled = true;
                MenuSettings.Combo.Rtarget.Value = 60;
                MenuSettings.Combo.Rhit.Value = 2;
                MenuSettings.Combo.Rdelay.Enabled = true;
                MenuSettings.Combo.Rdelaylogic.Enabled = true;
                MenuSettings.Combo.useIgnite.Enabled = true;
                //harass
                //laneclear
                //jungleclear
                //lasthit
                MenuSettings.Misc.eexploit.Enabled = true;
            }
            if (!objPlayer.IsDead)
            {
                if (MenuSettings.soloyasuo.Enabled)
                {
                    SoloYasuoMode();
                }
                if (MenuSettings.fleekey.Active)
                {
                    if (!beforeaa || !onaa)
                    {
                        fleeActive();
                    }
                }
                if(MenuSettings.EQ3Flash.Active && Flash != SpellSlot.Unknown)
                {
                    EQFlashEvent();
                }
                if (MenuSettings.Misc.killstealEnable.Enabled)
                    KillSteal();
                if (MenuSettings.Misc.autoQ.Enabled)
                    autoQ();
                if (MenuSettings.Harass.AutoHarassQ.Enabled)
                {
                    autoharassQ();
                }
                switch (Orbwalker.ActiveMode)
                {
                    case OrbwalkerMode.Combo:
                        Combo();
                        break;
                    case OrbwalkerMode.Harass:
                        Harass();
                        break;
                    case OrbwalkerMode.LaneClear:
                        LaneClear();
                        JungleClear();
                        break;
                    case OrbwalkerMode.LastHit:
                        LastHit();
                        break;
                }
            }
        }
        #endregion

        #region The EQ3 flash
        private static void EQFlashEvent()
        {
            try
            {
                if (Orbwalker.ActiveMode == OrbwalkerMode.None && Flash != SpellSlot.Unknown && Flash.IsReady())
                {
                    objPlayer.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

                    if (!HaveQ3)
                    {
                        if (Q.IsReady())
                        {
                            var minion = GameObjects.EnemyMinions.FirstOrDefault(x => x.IsValidTarget(Q.Range) && x.MaxHealth > 5);

                            if (minion != null && minion.IsValidTarget(Q.Range))
                            {
                                var pred = Q.GetPrediction(minion);
                                if (pred.Hitchance >= HitChance.Medium)
                                {
                                    Q.Cast(pred.CastPosition);
                                }
                            }
                        }
                    }
                    else if (HaveQ3 && Q.IsReady())
                    {
                        if (objPlayer.IsDashing() && Flash != SpellSlot.Unknown && Flash.IsReady())
                        {
                            var bestPos =
                                FlashPoints().ToArray()
                                    .Where(x => GameObjects.EnemyHeroes.Count(a => a.IsValidTarget(600f, true, x)) > 0)
                                    .OrderByDescending(x => GameObjects.EnemyHeroes.Count(i => i.Distance(x) <= 220))
                                    .FirstOrDefault();

                            if (bestPos != Vector3.Zero && bestPos.CountEnemyHeroesInRange(220) > 0 && Q.Cast(new Vector3(50000f, 50000f, 50000f)))
                            {
                                DelayAction.Add(300 + (Game.Ping / 2 - 5), () =>
                                {
                                    Eq3flash.Cast(bestPos);
                                });
                            }
                        }

                        if (E.IsReady())
                        {
                            var allTargets = new List<AIBaseClient>();

                            allTargets.AddRange(GameObjects.EnemyMinions.Where(x => x.IsValidTarget(E.Range) && x.MaxHealth > 5));
                            allTargets.AddRange(GameObjects.EnemyHeroes.Where(x => !x.IsDead && x.IsValidTarget(E.Range)));

                            if (allTargets.Any())
                            {
                                var eTarget =
                                    allTargets.Where(x => x.IsValidTarget(E.Range) && (!x.HasBuff("YasuoE") || !x.HasBuff("YasuoDashWrapper")))
                                        .OrderByDescending(
                                            x =>
                                                GameObjects.EnemyHeroes.Count(
                                                    t => t.IsValidTarget(600f, true, PosAfterE(x))))
                                        .FirstOrDefault();

                                if (eTarget != null)
                                {
                                    E.CastOnUnit(eTarget);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in MyEventManager.EQFlashEvent." + ex);
            }
        }
        public static void TheEQ3Flash()
        {
            if (Orbwalker.ActiveMode == OrbwalkerMode.None)
            {
                objPlayer.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }
            AIHeroClient target = TargetSelector.SelectedTarget;
            if (target.IsValidTarget(1000) && target != null)
            {
                if(objPlayer.IsDashing() && objPlayer.GetDashInfo().EndPos.DistanceToPlayer() < 40 && objPlayer.GetDashInfo().EndPos.DistanceToPlayer() > 10)
                {
                    if(target.DistanceToPlayer() < 600 && HaveQ3 && Q2.IsReady() && Flash.IsReady())
                    {
                        Q2.Cast(PosExploit(target));
                        Eq3flash.Cast(target.Position);
                    }
                }
                else
                {
                    if (!objPlayer.IsDashing())
                    {
                        var obj = new List<AIBaseClient>();
                        obj.AddRange(ObjectManager.Get<AIMinionClient>().Where(i => i.IsValidTarget(E.Range) && !i.IsAlly));
                        obj.AddRange(ObjectManager.Get<AIHeroClient>().Where(i => i.IsValidTarget(E.Range) && !i.IsAlly));
                        obj.AddRange(ObjectManager.Get<AIBaseClient>().Where(i => i.IsValidTarget(E.Range) && !i.IsAlly));
                        foreach(AIBaseClient objbase in obj.Where(i => !i.HasBuff("YasuoE")))
                        {
                            if(PosAfterE(objbase).Distance(target) < 475)
                            {
                                E.Cast(objbase);
                            }
                        }
                    }
                        
                }
            }           
        }
        #endregion

        #region The SoloYasuoMode xD
        private static void SoloYasuoMode()
        {
            AIHeroClient aIHeroClient = TargetSelector.GetTarget(1000);
            if (aIHeroClient == null || aIHeroClient.CharacterName != "Yasuo")
                return;           
            
            var obj = new List<AIBaseClient>();
            obj.AddRange(ObjectManager.Get<AIMinionClient>().Where(i => i.IsValidTarget(E.Range) && !i.IsAlly));
            obj.AddRange(ObjectManager.Get<AIHeroClient>().Where(i => i.IsValidTarget(E.Range) && !i.IsAlly));
            obj.AddRange(ObjectManager.Get<AIBaseClient>().Where(i => i.IsValidTarget(E.Range) && !i.IsAlly));
            Vector2 tdash = aIHeroClient.GetDashInfo().EndPos;
            foreach(AIBaseClient ObjBase in obj.Where(i => !i.HasBuff("YasuoE")))
            {
                if (E.IsReady())
                {
                    if (!UnderTower(PosAfterE(ObjBase)))
                    {
                        if (aIHeroClient.IsDashing())
                        {
                            if (tdash.DistanceToPlayer() <= QCirWidthMin && PosAfterE(ObjBase).Distance(tdash) > QCirWidthMin)
                            {
                                E.CastOnUnit(ObjBase);
                            }
                        }
                    }
                }
                foreach (AIBaseClient ObjBase2 in obj.Where(i => !i.HasBuff("YasuoE")))
                {
                    if (E.IsReady())
                    {

                            if (aIHeroClient.IsDashing())
                            {
                                if (tdash.DistanceToPlayer() <= QCirWidth && PosAfterE(ObjBase).Distance(tdash) > 200 && !UnderTower(PosAfterE(ObjBase)))
                                {
                                    E.CastOnUnit(ObjBase);
                                }
                                if (tdash.DistanceToPlayer() <= QCirWidth && PosAfterE(ObjBase2).Distance(tdash) > 200 && !UnderTower(PosAfterE(ObjBase2)))
                                {
                                    E.CastOnUnit(ObjBase);
                                }
                            }
                            else
                            {
                                if (comboactive && !onaa && !beforeaa)
                                {
                                    if (PosAfterE(ObjBase).Distance(Eprediction(aIHeroClient)) < QCirWidthMin - aIHeroClient.BoundingRadius)
                                    {
                                        E.CastOnUnit(ObjBase);
                                    }
                                    if (!aIHeroClient.HasBuff("YasuoE") && PosAfterE(ObjBase).Distance(Eprediction(aIHeroClient)) <= 410)
                                    {
                                        E.CastOnUnit(ObjBase);
                                    }
                                    if(ObjBase.NetworkId != ObjBase2.NetworkId)
                                    {
                                        Vector3 base1 = PosAfterE(ObjBase).Extend(ObjBase2.Position, 475);
                                        Vector3 base2 = PosAfterE(ObjBase2).Extend(ObjBase.Position, 475);
                                        var b1 = true;
                                        var b2 = true;
                                        if(PosAfterE(ObjBase).Distance(ObjBase2) > 475)
                                        {
                                            b1 = false;
                                        }
                                        if (PosAfterE(ObjBase2).Distance(ObjBase) > 475)
                                        {
                                            b2 = false;
                                        }

                                        if (base1.Distance(Eprediction(aIHeroClient)) < QCirWidthMin - aIHeroClient.BoundingRadius && b1)
                                        {
                                            E.CastOnUnit(ObjBase);
                                        }
                                        if (base2.Distance(Eprediction(aIHeroClient)) < QCirWidthMin - aIHeroClient.BoundingRadius && b2)
                                        {
                                            E.CastOnUnit(ObjBase);
                                        }
                                    }
                                }
                            }
                        
                    }
                }                
            }
            /*if (E.IsReady())
            {                
                if (aIHeroClient.IsDashing() && aIHeroClient.GetDashInfo().EndPos.DistanceToPlayer() < QCirWidthMin)
                {
                    foreach (AIBaseClient aIBaseClient in obj.Where(i => !i.HasBuff("YasuoDashWrapper") && !UnderTower(PosAfterE(i)) && PosAfterE(i).Distance(aIHeroClient.GetDashInfo().EndPos) > QCirWidth))
                    {
                        E.CastOnUnit(aIBaseClient);
                    }
                    foreach (AIBaseClient aIBaseClient1 in obj.Where(i => !i.HasBuff("YasuoDashWrapper") && !UnderTower(PosAfterE(i)) && PosAfterE(i).Distance(aIHeroClient.Position) < 150))
                    {
                        if (UnderTower(PosAfterE(aIBaseClient1)))
                            return;
                        E.CastOnUnit(aIBaseClient1);
                    }
                    if (aIHeroClient.HasBuff("YasuoDashWrapper"))
                    {
                        foreach (AIBaseClient aIBaseClient2 in obj.Where(i => !i.HasBuff("YasuoDashWrapper") && !UnderTower(PosAfterE(i)) && PosAfterE(i).Distance(aIHeroClient.Position) < 410))
                        {
                            if (UnderTower(PosAfterE(aIBaseClient2)))
                                return;
                            E.CastOnUnit(aIBaseClient2);
                        }
                    }
                    foreach (AIBaseClient aIBaseClient3 in obj.Where(i => !i.HasBuff("YasuoDashWrapper") && i.Distance(aIHeroClient.Position) < aIHeroClient.DistanceToPlayer()))
                    {
                        foreach (AIBaseClient aIBaseClient4 in obj.Where(i => !i.HasBuff("YasuoDashWrapper") && !UnderTower(PosAfterE(i)) && i.Distance(aIBaseClient3.Position) < 410))
                        {
                            if (UnderTower(PosAfterE(aIBaseClient4)) && !comboactive)
                                return;
                            E.CastOnUnit(aIBaseClient4);
                        }
                    }
                }
            }*/
        }
        #endregion

        #region The Flee
        public static void fleeActive()
        {
                objPlayer.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                         
            var obj = new List<AIBaseClient>();
            obj.AddRange(ObjectManager.Get<AIMinionClient>().Where(i => i.IsValidTarget(E.Range) && !i.IsAlly));
            obj.AddRange(ObjectManager.Get<AIHeroClient>().Where(i => i.IsValidTarget(E.Range) && !i.IsAlly));
            obj.AddRange(ObjectManager.Get<AIBaseClient>().Where(i => i.IsValidTarget(E.Range) && !i.IsAlly));
            if (E.IsReady())
            {
                foreach (AIBaseClient getobj in obj)
                {
                    if (getobj != null && !getobj.HasBuff("YasuoE"))
                    {
                        if (PosAfterE(getobj).DistanceToCursor() <= objPlayer.DistanceToCursor())
                        {
                            E.CastOnUnit(getobj);
                        }
                    }
                }
            }                       
        }
        #endregion        

        #region The OnUpdate Orb Modes

        private static void Combo()
        {
            if(!objPlayer.IsDashing())
            {
                Orbwalker.MovementState = true;
                Orbwalker.AttackState = true;
            }
            else
            {
                Orbwalker.MovementState = true;
                Orbwalker.AttackState = true;
            }
            if(MenuSettings.usetargetselect.Enabled)
            {
                var target1 = TargetSelector.SelectedTarget;
                if (target1 == null || !target1.IsValidTarget(2000))
                {
                    target1 = TargetSelector.GetTarget(2000);
                }
                foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(2000)))
                {
                    if (target == null || target.HasBuffOfType(BuffType.Invulnerability) || target.HasBuffOfType(BuffType.SpellImmunity) || target1 == null)
                        return;
                    if (target != null || target1 != null)
                    {
                        if(MenuSettings.Combo.qhextech.Enabled && (objPlayer.HasItem(((int)ItemId.Will_of_the_Ancients)) || objPlayer.HasItem((int)ItemId.Hextech_Gunblade)))
                        {
                            if (objPlayer.CanUseItem((int)ItemId.Hextech_Gunblade) && target.Distance(objPlayer) < 700)
                            {
                                objPlayer.UseItem(3146, target);
                            }

                            if(HaveQ3)
                            if (target1.Distance(objPlayer) < 475 + 100 && objPlayer.CanUseItem((int)ItemId.Will_of_the_Ancients))
                            {
                                    Orbwalker.MovementState = true;
                                    Orbwalker.AttackState = true;
                                    if (Edashing() == true)
                                {
                                    if (Q.IsReady())
                                    {
                                        Q.Cast(target1.Position);
                                            DelayAction.Add(200, () =>
                                            {
                                                Q.Cast(target1.Position);
                                                Q.Cast(target1.Position);
                                                Q.Cast(target1.Position);
                                                objPlayer.UseItem(3152, target1.Position);
                                            });
                                    }
                                }
                                else
                                {
                                    var getallobj = new List<AIBaseClient>().Where(i => !i.IsAlly && !i.IsDead && i.HasBuff("YasuoE") && i.IsValidTarget(475));
                                    if(E.IsReady(0) && getallobj.Count() > 0)
                                    {
                                        foreach(var getobj in getallobj)
                                        {
                                            E.CastOnUnit(
                                                getobj);
                                        }
                                    }
                                }
                                /*if (PosAfterE(getobj).Distance(target1) - 100 < 275)
                                    if (E.IsReady() && E.CastOnUnit(getobj)) if (Q.IsReady() && Q.Cast(PosExploit(target1)))
                                            objPlayer.UseItem(3152, target1.Position);
                                if(Q.IsReady() && Q.Cast(PosExploit(target)))
                                {
                                    objPlayer.UseItem(3152, target.Position);
                                }     */
                            }       
                            else
                            {
                                /*if (Edashing() == true)
                                {
                                    Orbwalker.MovementState = false;
                                    Orbwalker.AttackState = false;
                                }
                                else
                                {
                                    Orbwalker.MovementState = true;
                                    Orbwalker.AttackState = true;
                                }   */                             
                            }
                        }
                        if (CanCastR(target) && R.IsReady() && target.IsValidTarget(R.Range) && MenuSettings.Combo.Rtarget.Value >= target.HealthPercent && MenuSettings.Combo.useR.Enabled)
                        {
                            var buff = target.Buffs.FirstOrDefault(i => i.Type == BuffType.Knockback || i.Type == BuffType.Knockup);
                            if (MenuSettings.Combo.Rdelay.Enabled)
                            {
                                var EQR = new List<AIBaseClient>();
                                EQR.AddRange(ObjectManager.Get<AIMinionClient>().Where(i => i.IsValidTarget(E.Range) && !i.IsAlly));
                                EQR.AddRange(ObjectManager.Get<AIHeroClient>().Where(i => i.IsValidTarget(E.Range) && !i.IsAlly));
                                EQR.AddRange(ObjectManager.Get<AIBaseClient>().Where(i => i.IsValidTarget(E.Range) && !i.IsAlly));                               

                                if (buff.EndTime * 100 < 150 + Game.Ping)
                                {
                                    R.Cast(target);
                                }
                                if (!EQR.Any())
                                {
                                    CastE(target1);

                                    if (HaveQ3 ? target1.IsValidTarget(1070) : target1.IsValidTarget(475))
                                    {
                                        Qcombo(target1);
                                    }
                                    else
                                    {
                                        Qcombo(target);
                                    }
                                }
                                if (MenuSettings.Combo.Rdelaylogic.Enabled)
                                {
                                    if (objPlayer.IsDashing() && Q.IsReady() && ObjectManager.Player.GetDashInfo().EndPos.DistanceToPlayer() > 10)
                                    {
                                        Q.Cast(PosExploit(target1));
                                        R.Cast(target.Position);
                                    }
                                    foreach (AIBaseClient EQRBase in EQR.Where(i => !i.HasBuff("YasuoE") && PosAfterE(i).Distance(target) < R.Range))
                                    {
                                        if (E.IsReady() && Q.IsReady())
                                        {
                                            E.Cast(EQRBase);
                                            Q.Cast(PosExploit(target1));
                                            R.Cast(target.Position);
                                        }
                                        if (EQRBase == null || EQRBase.HasBuff("YasuoE") || !UnderTower(PosAfterE(EQRBase)))
                                        {
                                            if (Q.IsReady())
                                            {
                                                if (HaveQ3 ? target1.IsValidTarget(1070) : target1.IsValidTarget(475))
                                                {
                                                    Qcombo(target1);
                                                }
                                                else
                                                {
                                                    Qcombo(target);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            else R.Cast(target.Position);
                        }
                        else
                        {
                            CastE(target1);
                            if (HaveQ3 ? target1.IsValidTarget(1070) : target1.IsValidTarget(475))
                            {
                                Qcombo(target1);
                            }
                            else
                            {
                                Qcombo(target);
                            }
                        }
                    }
                }
            } 
            else
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(2000)))
                {
                    if (target == null || target.HasBuffOfType(BuffType.Invulnerability) || target.HasBuffOfType(BuffType.SpellImmunity))
                        return;
                    if (target != null)
                    {
                        if (CanCastR(target) && R.IsReady() && target.IsValidTarget(R.Range) && MenuSettings.Combo.Rtarget.Value >= target.HealthPercent && MenuSettings.Combo.useR.Enabled)
                        {
                            if (MenuSettings.Combo.Rdelay.Enabled)
                            {
                                var EQR = new List<AIBaseClient>();
                                EQR.AddRange(ObjectManager.Get<AIMinionClient>().Where(i => i.IsValidTarget(E.Range) && !i.IsAlly));
                                EQR.AddRange(ObjectManager.Get<AIHeroClient>().Where(i => i.IsValidTarget(E.Range) && !i.IsAlly));
                                EQR.AddRange(ObjectManager.Get<AIBaseClient>().Where(i => i.IsValidTarget(E.Range) && !i.IsAlly));
                                var buff = target.Buffs.FirstOrDefault(i => i.Type == BuffType.Knockback || i.Type == BuffType.Knockup);

                                if (buff.EndTime * 100 < 20 + Game.Ping && Game.Time - buff.StartTime >= 0.85 * (buff.EndTime - buff.StartTime))
                                {
                                    R.Cast(target.Position);
                                }
                                if (!EQR.Any())
                                {
                                    CastE(target);

                                    Qcombo(target);
                                }
                                if (MenuSettings.Combo.Rdelaylogic.Enabled)
                                {
                                    if (objPlayer.IsDashing() && Q.IsReady() && ObjectManager.Player.GetDashInfo().EndPos.DistanceToPlayer() > 10)
                                    {
                                        Q.Cast(PosExploit(target));
                                        R.Cast(target.Position);
                                    }
                                    foreach (AIBaseClient EQRBase in EQR.Where(i => !i.HasBuff("YasuoE") && PosAfterE(i).Distance(target) < R.Range))
                                    {
                                        if (E.IsReady() && Q.IsReady())
                                        {
                                            E.Cast(EQRBase);
                                            Q.Cast(PosExploit(target));
                                            R.Cast(target.Position);
                                        }
                                        if (EQRBase == null || EQRBase.HasBuff("YasuoE") || !UnderTower(PosAfterE(EQRBase)))
                                        {
                                            if (Q.IsReady())
                                            {
                                                Qcombo(target);
                                            }
                                        }
                                    }
                                }
                            }
                            else R.Cast(target.Position);
                        }
                        else
                        {
                            CastE(target);
                            Qcombo(target);
                        }
                    }
                }
            }
        }
        public static void Qcombo(AIHeroClient target)
        {
            if(target != null && HaveQ3 ? target.IsValidTarget(1070) : target.IsValidTarget(475))
            {
                var obj = GetNearObj(target);
                if (Q.IsReady())
                {
                    if (!objPlayer.IsDashing() && !Edashing())
                    {
                        var qpred = Q.GetPrediction(target);
                        var q2pred = Q2.GetPrediction(target, false, -1, CollisionObjects.YasuoWall);
                        if (HaveQ3)
                        {
                            if (obj != null && !obj.HasBuff("YasuoE") && obj.IsValidTarget(475) && Eprediction(target).Distance(PosAfterE(obj)) < QCirWidthMin - target.BoundingRadius)
                            {
                                if (MenuSettings.Combo.Eturret.Active)
                                {
                                    E.Cast(GetNearObj(target));
                                }
                                else
                                {
                                    if (!UnderTower(PosAfterE(GetNearObj(target))))
                                    {
                                        E.Cast(GetNearObj(target));
                                    }
                                }
                            }
                            if (obj == null || obj.HasBuff("YasuoE") || Eprediction(target).Distance(PosAfterE(obj)) > QCirWidthMin - target.BoundingRadius || !obj.IsValidTarget(475))
                            {
                                if (!beforeaa)
                                    CastQ3(target);
                            }
                        }
                        else
                        {
                            if (GetNearObj(target) != null && GetNearObj(target).HasBuff("YasuoE") && Eprediction(target).Distance(PosAfterE(GetNearObj(target))) < QCirWidthMin - target.BoundingRadius && obj.IsValidTarget(475))
                            {
                                if (MenuSettings.Combo.Eturret.Active)
                                {
                                    E.Cast(GetNearObj(target));
                                }
                                else
                                {
                                    if (!UnderTower(PosAfterE(GetNearObj(target))))
                                    {
                                        E.Cast(GetNearObj(target));
                                    }
                                }
                            }
                            if (obj == null || obj.HasBuff("YasuoE") || Eprediction(target).Distance(PosAfterE(obj)) > QCirWidthMin - target.BoundingRadius || !obj.IsValidTarget(475))
                            {
                                CastQ(target);
                            }
                        }

                    }
                    else
                    {
                        if (MenuSettings.Combo.useEQ.Enabled && objPlayer.IsDashing())
                        {
                            if (target.IsValidTarget(475) && inQ13cc() && objPlayer.GetDashInfo().EndPos.Distance(Eprediction(target)) < objPlayer.GetRealAutoAttackRange() + 20)
                            {
                                Q.Cast(PosExploit(target));
                            }
                            if (HaveQ2)
                            {
                                Q.Cast(PosExploit(target));
                            }
                            if (!HaveQ2 && !HaveQ3)
                            {
                                if (objPlayer.GetDashInfo().EndPos.Distance(Eprediction(target)) > 475)
                                {
                                    Q.Cast(PosExploit(target));
                                }
                            }
                        }
                    }
                }
            }            
        }
        public static void CastE(AIHeroClient target)
        {
            if(target != null)
            if (MenuSettings.Combo.useE.Enabled && E.IsReady())
            {
                var GetNewObj = GameObjects.EnemyMinions.Where(x => x.IsValidTarget(475) && x.IsMinion() && !x.HasBuff("YasuoE"));
                var obj = GetNearObj(target);
                if(!GetNewObj.Any())
                {
                    foreach(AIMinionClient obj2 in GetNewObj)
                    {
                        if(obj.NetworkId != obj2.NetworkId)
                        {
                            Vector3 base1 = PosAfterE(obj).Extend(obj2.Position, 475);
                            Vector3 base2 = PosAfterE(obj2).Extend(obj.Position, 475);
                            var b1 = true;
                            var b2 = true;
                            if (PosAfterE(obj).Distance(obj2) > 475)
                            {
                                b1 = false;
                            }
                            if (PosAfterE(obj2).Distance(obj) > 475)
                            {
                                b2 = false;
                            }
                            if(MenuSettings.Combo.useEZicZac)
                            {
                                if (base1.Distance(Eprediction(target)) < QCirWidthMin - target.BoundingRadius && b1)
                                {
                                    E.CastOnUnit(obj);
                                }
                                if (base2.Distance(Eprediction(target)) < QCirWidthMin - target.BoundingRadius && b2)
                                {
                                    E.CastOnUnit(obj2);
                                }
                            }
                        }
                    }
                }
                if (obj != null && (!beforeaa || !onaa))
                {
                    if (obj.NetworkId == target.NetworkId)
                    {
                        try
                        {
                            if(MenuSettings.Combo.Eturret.Active)
                            {
                                 if (Eprediction(target).Distance(PosAfterE(target)) < objPlayer.GetRealAutoAttackRange())
                                 {
                                        E.CastOnUnit(target);
                                 }
                                 if (target.DistanceToPlayer() > 300)
                                 {
                                        E.Cast(target);
                                 }
                                                              
                            }
                            else
                            {
                                if(!UnderTower(PosAfterE(target)))
                                {
                                        if (Eprediction(target).Distance(PosAfterE(target)) < 150)
                                        {
                                            E.CastOnUnit(target);
                                        }
                                        if (target.DistanceToPlayer() > 300)
                                        {
                                            E.Cast(target);
                                        }
                                                                      
                                }
                            }                                
                        }
                        catch (Exception exception)
                        {
                            Game.Print(exception);
                        }
                    }
                    else
                    {
                        if (MenuSettings.Combo.Eturret.Active)
                        {
                            E.Cast(obj);
                            foreach (var min in GetEnemyLaneMinionsTargetsInRange(Q.Range))
                            {
                                if (PosAfterE(min).CountEnemyHeroesInRange(QCirWidth) >= 2 && min != null)
                                    E.CastOnUnit(min);
                            }
                            foreach (var t in GetBestEnemyHeroesTargetsInRange(Q.Range))
                            {
                                if (PosAfterE(t).CountEnemyHeroesInRange(QCirWidth) >= 2 && t != null)
                                    E.CastOnUnit(t);
                            }
                        }
                        if (!MenuSettings.Combo.Eturret.Active)
                        {
                            if (UnderTower(PosAfterE(obj)))
                            {
                                return;
                            }
                            if (!UnderTower(PosAfterE(obj)))
                            {
                                E.CastOnUnit(obj);
                            }
                            foreach (var min in GetEnemyLaneMinionsTargetsInRange(Q.Range))
                            {
                                if (min != null && PosAfterE(min).CountEnemyHeroesInRange(QCirWidth) >= 2 && !UnderTower(PosAfterE(min)))
                                    E.CastOnUnit(min);
                            }
                            foreach (var t in GetBestEnemyHeroesTargetsInRange(Q.Range))
                            {
                                if (t != null && PosAfterE(t).CountEnemyHeroesInRange(QCirWidthMin) >= 2 && !UnderTower(PosAfterE(t)))
                                    E.CastOnUnit(t);
                            }
                        }
                    }                    
                }
            }
        }
        public static void CastR(AIHeroClient target)
        {
            if (MenuSettings.Combo.useR.Enabled && CanCastR(target) && R.IsReady() && target.IsValidTarget(R.Range))
            {
                if (target.HealthPercent <= MenuSettings.Combo.Rtarget.Value)
                {
                    if (MenuSettings.Combo.Rdelay.Enabled)
                    {
                        if (TimeLeftR(target) * 1000 < 150 + Game.Ping * 2)
                            R.Cast(target);
                    }
                    if (!MenuSettings.Combo.Rdelay.Enabled)
                    {
                        R.Cast(target);
                    }
                    if (objPlayer.IsDashing() && Q.IsReady())
                    {
                        Q.Cast(PosExploit(target));
                        R.Cast();
                    }
                    if (!objPlayer.IsDashing() && Q.IsReady() && E.IsReady())
                    {

                            var EQR = new List<AIBaseClient>();
                            EQR.AddRange(ObjectManager.Get<AIMinionClient>().Where(i => i.IsValidTarget(E.Range) && !i.IsAlly));
                            EQR.AddRange(ObjectManager.Get<AIHeroClient>().Where(i => i.IsValidTarget(E.Range) && !i.IsAlly));
                            EQR.AddRange(ObjectManager.Get<AIBaseClient>().Where(i => i.IsValidTarget(E.Range) && !i.IsAlly));
                            if (EQR != null)
                            {
                                foreach (AIBaseClient getEQRobj in EQR)
                                {
                                    if (PosAfterE(getEQRobj).Distance(target) < R.Range)
                                    {
                                        E.Cast(getEQRobj);
                                        Q.Cast(PosExploit(target));
                                        R.Cast();
                                    }
                                }
                            }
                        
                    }
                }
            }
        }
        public static void CastQ3(AIBaseClient target) //Made by Brian(Valve Sharp)
        {
            try
            {
                if (MenuSettings.Combo.useQ.Enabled)
                    if (!objPlayer.IsDashing() && !Edashing() && target.IsValidTarget(1080) && Q2.GetPrediction(target).CastPosition.DistanceToPlayer() < 1070)
                        Q2.Cast(Q2.GetPrediction(target).CastPosition);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        public static void CastQ(AIBaseClient target)
        {
            try
            {
                if(MenuSettings.Combo.useQ.Enabled)
                    if (!objPlayer.IsDashing() && !Edashing() && target.IsValidTarget(475) && Q.GetPrediction(target).CastPosition.DistanceToPlayer() < 475)
                        Q.Cast(Q.GetPrediction(target).CastPosition);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        static void autoQ()
        {
            foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(700)))
            {
                if(target != null)
                {
                    if (Q.IsReady())
                    {                        
                        if (objPlayer.IsDashing() && ObjectManager.Player.GetDashInfo().EndPos.DistanceToPlayer() > 5)
                        {
                            if (HaveQ3 && inQ13cc() && target.IsValidTarget(objPlayer.GetRealAutoAttackRange()))
                            {
                                if (MenuSettings.Combo.useEQ3.Enabled)
                                    Q2.Cast(PosExploit(target));
                            }
                            if (HaveQ2)
                            {
                                if (MenuSettings.Combo.useEQ.Enabled)
                                    Q.Cast(PosExploit(target));
                            }
                            else
                            {
                                if(!HaveQ3)
                                {
                                    if (inQ13cc() && target.IsValidTarget(QCirWidthMin - target.BoundingRadius))
                                    {
                                        if (MenuSettings.Combo.useEQ.Enabled)
                                            Q.Cast(PosExploit(target));
                                    }
                                    if (target.Distance(objPlayer.GetDashInfo().EndPos) > Q.Range && inQ13cc())
                                    {
                                        var AQ = new List<AIBaseClient>();
                                        AQ.AddRange(ObjectManager.Get<AIMinionClient>().Where(i => i.IsValidTarget(QCirWidthMin) && !i.IsAlly));
                                        AQ.AddRange(ObjectManager.Get<AIHeroClient>().Where(i => i.IsValidTarget(QCirWidthMin) && !i.IsAlly));
                                        AQ.AddRange(ObjectManager.Get<AIBaseClient>().Where(i => i.IsValidTarget(QCirWidthMin) && !i.IsAlly));
                                        if (AQ.Any())
                                        {
                                            Q.Cast(PosExploit(target));
                                        }
                                    }
                                }                                
                            }
                        }
                    }
                }         
                else
                {
                    if (objPlayer.IsDashing() && ObjectManager.Player.GetDashInfo().EndPos.DistanceToPlayer() > 5 && target == null)
                    {
                        if (MenuSettings.Misc.eexploit.Enabled)
                        {
                            Q.Cast(new Vector3(50000f, 50000f, 50000f));
                        }
                        else
                        {
                            Q.Cast(objPlayer.PreviousPosition);
                        }
                    }
                    if (objPlayer.IsDashing() && ObjectManager.Player.GetDashInfo().EndPos.DistanceToPlayer() > 5 && ObjectManager.Player.GetDashInfo().EndPos.DistanceToPlayer() < 50 && target == null)
                    {
                        if (MenuSettings.Misc.eexploit.Enabled)
                        {
                            Q.Cast(new Vector3(50000f, 50000f, 50000f));
                        }
                        else
                        {
                            Q.Cast(objPlayer.PreviousPosition);
                        }
                    }
                }
            }                      
        }

        private static void autoharassQ()
        {
            AIHeroClient target = TargetSelector.GetTarget(E.Range);

            if(target == null || target.IsDead || !target.IsValidTarget(Q.Range))
                return;

            var getPrediction = Q.GetPrediction(target);
            if(!objPlayer.IsDashing())
            if (getPrediction.CollisionObjects.Count() < 2 && getPrediction.Hitchance >= HitChance.High)
            {
                Q.Cast(getPrediction.CastPosition);
            }
        }
        private static void Harass()
        {
            AIHeroClient target = TargetSelector.GetTarget(E.Range);

            if (target == null || target.IsDead)
                return;

            if(objPlayer.IsDashing() && ObjectManager.Player.GetDashInfo().EndPos.DistanceToPlayer() > 5 && MenuSettings.Harass.UseEQ.Enabled)
            {
                if(ObjectManager.Player.GetDashInfo().EndPos.Distance(target) < QCirWidth)
                {
                    Q.Cast(PosExploit(target));
                    return;
                }
            }
            if (MenuSettings.Harass.useE.Enabled && E.IsReady() && !UnderTower(PosAfterE(target)))
            {
                if (!target.IsValidTarget(E.Range))
                    return;
                E.CastOnUnit(target);
            }
            if (MenuSettings.Harass.useQ.Enabled && Q.IsReady())
            {
                if (!target.IsValidTarget(Q2.Range))
                    return;

                if (HaveQ3 && !objPlayer.IsDashing())
                {
                    Q2.CastIfHitchanceMinimum(target, HitChance.High);
                    return;
                }
                else Q.Cast(PosExploit(target));
            }
        }
        private static void LaneClear()
        {
            var allMinions = GameObjects.EnemyMinions.Where(x => x.IsMinion() && !x.IsDead).OrderBy(x => x.Distance(objPlayer.Position));

            if (allMinions.Count() == 0 || onaa)
                return;
            if (MenuSettings.Harass.UseEQ.Enabled && Q.IsReady() && objPlayer.IsDashing() && ObjectManager.Player.GetDashInfo().EndPos.DistanceToPlayer() > 5)
            {
                foreach (var min in allMinions.Where(x => x.IsValidTarget(QCirWidthMin)))
                {
                    if (min.Distance(objPlayer.GetDashInfo().EndPos) < QCirWidthMin && inQ13cc())
                    {
                        Q.Cast(PosExploit(min));
                        return;
                    }
                }
            }
            if(MenuSettings.LaneClear.fastclearE.Enabled)
            {
                foreach (var min in allMinions.Where(x => x.IsValidTarget(E.Range)))
                {
                    if (min.DistanceToPlayer() > objPlayer.GetRealAutoAttackRange() 
                        && MenuSettings.LaneClear.Eturret.Enabled 
                        ? E.CastOnUnit(min)
                        : !UnderTower(PosAfterE(min)))
                    {
                        E.CastOnUnit(min);
                        return;
                    }
                }
            }
            if (MenuSettings.LaneClear.useE.Enabled && E.IsReady())
            {
                foreach (var min in allMinions.Where(x => x.IsValidTarget(E.Range)))
                {
                    if (min.Health < E.GetDamage(min))
                    {
                        if(UnderTower(PosAfterE(min)))
                        {
                            if (MenuSettings.LaneClear.Eturret.Enabled)
                            {
                                E.CastOnUnit(min);
                            }
                            else return;
                        }
                        if(!UnderTower(PosAfterE(min)))
                        {
                            E.Cast(min);
                            return;
                        }
                    }
                }
            }
            
            if (MenuSettings.LaneClear.useQ.Enabled && Q.IsReady())
            {
                foreach (var min in allMinions.Where(x => x.IsValidTarget(Q2.Range)))
                {
                    if (min.DistanceToPlayer() < objPlayer.GetRealAutoAttackRange() - 40)
                    {
                        if(beforeaa || afteraa)
                        {
                            if(HaveQ3 && !objPlayer.IsDashing())
                            {
                                Q2.CastIfHitchanceMinimum(min, HitChance.High);
                                return;
                            }
                            if(HaveQ2 && !objPlayer.IsDashing())
                            {
                                Q.CastIfHitchanceMinimum(min, HitChance.High);
                            }
                            else
                                if (!objPlayer.IsDashing())
                                Q.CastIfHitchanceMinimum(min, HitChance.High);
                        }
                    }
				   if(min.DistanceToPlayer() > objPlayer.GetRealAutoAttackRange() && !objPlayer.IsDashing())
                   {
                        if (HaveQ3)
                        {
                            Q2.CastIfHitchanceMinimum(min, HitChance.High);
                            return;
                        }
                        if (HaveQ2)
                        {
                            Q.CastIfHitchanceMinimum(min, HitChance.High);
                        }
                        else 
                            if(!HaveQ3)
                            Q.CastIfHitchanceMinimum(min, HitChance.High);
                   }                    
                }
            }
        }
        private static void JungleClear()
        {
            foreach (var jungle in GetJungleTargetsInRange(2000))
            {
                if(jungle != null && jungle.DistanceToPlayer() <= E.Range)
                {
                    if (MenuSettings.JungleClear.useE.Enabled && E.IsReady() && !jungle.HasBuff("YasuoE") && PosAfterE(jungle).Distance(jungle) < QCirWidth && E.CastOnUnit(jungle))
                    {
                        return;
                    }
                    if(MenuSettings.JungleClear.useQ.Enabled && Q.IsReady())
                    {
                        if(objPlayer.IsDashing() && ObjectManager.Player.GetDashInfo().EndPos.DistanceToPlayer() > 5)
                        {
                            Q.Cast(PosExploit(jungle));
                        }
                        if(!objPlayer.IsDashing())
                        {
                            if(HaveQ3)
                            {
                                Q2.CastIfHitchanceMinimum(jungle, HitChance.High);
                            }
                            else
                            {
                                Q.CastIfHitchanceMinimum(jungle, HitChance.High);
                            }
                        }
                    }
                }
            }
        }
        private static void LastHit()
        {
            var allMinions = GameObjects.EnemyMinions.Where(x => x.IsMinion() && !x.IsDead).OrderBy(x => x.Distance(objPlayer.Position));

            if (MenuSettings.LastHit.useQ.Enabled && Q.IsReady())
            {
                foreach (var min in allMinions.Where(x => x.IsValidTarget(Q.Range) && x.Health < Q.GetDamage(x)))
                {
                    if(!objPlayer.IsDashing())
                    Q.Cast(PosExploit(min));

                    if (objPlayer.IsDashing() && ObjectManager.Player.GetDashInfo().EndPos.DistanceToPlayer() > 5)
                    {
                        if(min.IsValidTarget(175) && inQ13cc())
                        {
                            Q.Cast(PosExploit(min));
                        }
                    }
                }
            }
            if (MenuSettings.LastHit.useE.Enabled && E.IsReady())
            {
                foreach (var min in allMinions.Where(x => x.IsValidTarget(Q.Range) && x.Health < E.GetDamage(x)))
                {
                    E.CastOnUnit(min);
                }
            }
        }

        #endregion

        #region The bool Events

        private static void OnAction(object sender, OrbwalkerActionArgs args)
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

        private static void OnInterrupterSpell(AIHeroClient sender, Interrupter.InterruptSpellArgs arg)
        {
            if (!MenuSettings.Misc.interrupter.Enabled)
                return;
            if (!HaveQ3)
                return;

            if (sender.IsEnemy && sender.DistanceToPlayer() < 550 && !objPlayer.IsDashing())
            {
                Q2.Cast(sender.PreviousPosition);
            }
        }
        private static void OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserArgs args)
        {
            if (!MenuSettings.Misc.gapcloser.Enabled)
                return;
            if (!HaveQ3)
                return;

            if (args != null && args.EndPosition.DistanceToPlayer() < 500 && !objPlayer.IsDashing())
                Q2.Cast(args.EndPosition);
        }

        #endregion

        #region The Drawings

        private static void OnDraw(EventArgs args)
        {
            if (MenuSettings.Drawing.disableDrawings.Enabled)
                return;
            if (MenuSettings.Drawing.drawQ.Enabled && Q.IsReady())
            {
                Render.Circle.DrawCircle(objPlayer.Position, HaveQ3 ? Q2.Range : Q.Range, System.Drawing.Color.DarkBlue);
            }
            if (MenuSettings.Drawing.drawW.Enabled && W.IsReady())
            {
                Render.Circle.DrawCircle(objPlayer.Position, W.Range, System.Drawing.Color.Beige);
            }
            if (MenuSettings.Drawing.drawE.Enabled && E.IsReady())
            {
                Render.Circle.DrawCircle(objPlayer.Position, E.Range, System.Drawing.Color.DodgerBlue);
            }
            if (MenuSettings.Drawing.drawR.Enabled && R.IsReady())
            {
                Drawing.DrawCircle(objPlayer.Position, R.Range, System.Drawing.Color.DarkBlue);
            }
        }
        private static void OnEndScene(EventArgs args)
        {
            if (MenuSettings.Drawing.disableDrawings.Enabled)
                return;
            if (!MenuSettings.Drawing.drawDmg.Enabled)
                return;

            foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(2000) && !x.IsDead && x.IsHPBarRendered))
            {
                Vector2 pos = Drawing.WorldToScreen(target.Position);

                if (!pos.IsOnScreen())
                    return;

                var damage = getDamage(target, true, true, true, true);

                var hpBar = target.HPBarPosition;

                if (damage > target.Health)
                {
                    Drawing.DrawText(hpBar.X + 69, hpBar.Y - 45, System.Drawing.Color.White, "KILLABLE");
                }

                var damagePercentage = ((target.Health - damage) > 0 ? (target.Health - damage) : 0) / target.MaxHealth;
                var currentHealthPercentage = target.Health / target.MaxHealth;

                var startPoint = new Vector2(hpBar.X - 45 + damagePercentage * 104, hpBar.Y - 18);
                var endPoint = new Vector2(hpBar.X - 45 + currentHealthPercentage * 104, hpBar.Y - 18);

                Drawing.DrawLine(startPoint, endPoint, 12, System.Drawing.Color.Yellow);
            }
        }

        #endregion

        #region The Prediction
        public static Vector3 Eprediction(AIBaseClient target)
        {
            var getPrediction = E1.GetPrediction(target);
            return getPrediction.CastPosition;
        }
        public static bool Edashing()
            => Game.Time * 1000 - lastEtime >= -1f && Game.Time * 1000 - lastEtime < 475/Espeeds()
            || objPlayer.IsDashing();
        public static AIBaseClient GetNearObj(AIBaseClient target = null)
        {
            var pos = Epred(target);
            var obj = new List<AIBaseClient>();
            obj.AddRange(ObjectManager.Get<AIMinionClient>().Where(i => i.IsValidTarget(E.Range) && !i.IsAlly));
            obj.AddRange(ObjectManager.Get<AIHeroClient>().Where(i => i.IsValidTarget(E.Range) && !i.IsAlly));
            obj.AddRange(ObjectManager.Get<AIBaseClient>().Where(i => i.IsValidTarget(E.Range) && !i.IsAlly));
            if (!target.HasBuff("YasuoE") && E.IsReady() && target.DistanceToPlayer() <= 700 && !Q.IsReady() && MenuSettings.Combo.useEZicZac.Enabled)
            {
                return
                obj.Where(
                    i =>
                    !i.HasBuff("YasuoE")
                    && pos.Distance(PosAfterE(i)) < 375
                    )
                    .MinOrDefault(i => pos.Distance(PosAfterE(i)));
            }
            else
            return
                obj.Where(
                    i =>
                    !i.HasBuff("YasuoE")
                    && (pos.Distance(PosAfterE(i)) <= pos.DistanceToPlayer()
                    || (Q.IsReady() ? pos.Distance(PosAfterE(i)) <= QCirWidth
                    : pos.Distance(PosAfterE(i)) <= pos.DistanceToPlayer()))
                    )
                    .MinOrDefault(i => pos.Distance(PosAfterE(i)));
        }
        #endregion

        #region Misc

        private static void KillSteal()
        {
            foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(2000)))
            {
                if (target == null || target.HasBuffOfType(BuffType.Invulnerability) || target.HasBuffOfType(BuffType.SpellImmunity))
                    return;

                if (MenuSettings.Misc.killstealR.Enabled && R.IsReady() && target.IsValidTarget(R.Range))
                {
                    if (target.Health < R.GetDamage(target))
                        R.Cast(target);
                }
                if (MenuSettings.Misc.killstealW.Enabled && W.IsReady() && target.IsValidTarget(W.Range))
                {
                    var getPrediction = W.GetPrediction(target);

                    if (target.Health + target.MagicalShield < W.GetDamage(target) && getPrediction.Hitchance >= HitChance.High)
                    {
                        W.Cast(getPrediction.CastPosition);
                    }
                }
                if (MenuSettings.Misc.killstealE.Enabled && E.IsReady() && target.IsValidTarget(E.Range))
                {
                    if (target.Health + target.MagicalShield < (Q.IsReady() ?
                        E.GetDamage(target) + Q.GetDamage(target) : 
                        E.GetDamage(target)))
                    {
                        E.CastOnUnit(target);
                    }
                }
                if (MenuSettings.Misc.killstealIgnite.Enabled && summonerIgnite.IsReady() && target.IsValidTarget(600))
                {
                    if (target.Health + target.MagicalShield < objPlayer.GetSummonerSpellDamage(target, SummonerSpell.Ignite))
                    {
                        objPlayer.Spellbook.CastSpell(summonerIgnite, target);
                    }
                }
            }
        }

        #region Extensions

        private static float getDamage(AIBaseClient target, bool q = false, bool w = false, bool r = false, bool ignite = false)
        {
            float damage = 0;

            if (target == null || target.IsDead)
                return 0;
            if (target.HasBuffOfType(BuffType.Invulnerability))
                return 0;

            if (q && Q.IsReady())
                damage += (float)Damage.GetSpellDamage(objPlayer, target, SpellSlot.Q);
            if (w && W.IsReady())
                damage += (float)Damage.GetSpellDamage(objPlayer, target, SpellSlot.W);
            if (r && R.IsReady())
                damage += (float)Damage.GetSpellDamage(objPlayer, target, SpellSlot.R);

            if (ignite && summonerIgnite.IsReady())
                damage += (float)objPlayer.GetSummonerSpellDamage(target, SummonerSpell.Ignite);

            if (objPlayer.GetBuffCount("itemmagicshankcharge") == 100) // oktw sebby
                damage += (float)objPlayer.CalculateMagicDamage(target, 100 + 0.1 * objPlayer.TotalMagicalDamage);

            if (target.HasBuff("ManaBarrier") && target.HasBuff("BlitzcrankManaBarrierCO"))
                damage += target.Mana / 2f;
            if (target.HasBuff("GarenW"))
                damage = damage * 0.7f;

            return damage;
        }

        #endregion

        #endregion

        public class EvadeSkillshot
        {
            #region Public Methods and Operators
            public static void Init(Menu menu)
            {
                {
                    EvadeSkillshotMenu.Add(new MenuBool("Credit", "Credit: Evade#"));
                    var evadeSpells = new Menu("Spells", "Spells");
                    {
                        foreach (var spell in EvadeSpellDatabase.Spells)
                        {
                            try
                            {
                                var sub = new Menu("ESSS_" + spell.Name, spell.Name + " (" + spell.Slot + ")");
                                {
                                    if (spell.Name == "YasuoDashWrapper")
                                    {
                                        sub.Add(new MenuBool("ETower", "Under Tower", false));
                                    }
                                    else if (spell.Name == "YasuoWMovingWall")
                                    {
                                        sub.Add(new MenuSlider("WDelay", "Extra Delay", 100, 0, 150));
                                    }
                                    sub.Add(new MenuSlider("DangerLevel", "If Danger Level >=", spell.DangerLevel, 1, 5));
                                    sub.Add(new MenuBool("Enabled", "Enabled", false));
                                    evadeSpells.Add(sub);
                                }
                            }
                            catch { }
                        }
                        EvadeSkillshotMenu.Add(evadeSpells);
                    }
                    foreach (var hero in
                        HeroManager.Enemies.Where(i => SpellDatabase.Spells.Any(a => a.ChampionName == i.CharacterName)))
                    {
                        try
                        {
                            EvadeSkillshotMenu.Add(new Menu("EvadeSS_" + hero.CharacterName, "-> " + hero.CharacterName));
                        }
                        catch { }
                    }
                    foreach (var spell in
                        SpellDatabase.Spells.Where(i => HeroManager.Enemies.Any(a => a.CharacterName == i.ChampionName)))
                    {
                        try
                        {
                            var sub = new Menu("ESS_" + spell.MenuItemName, spell.SpellName + " (" + spell.Slot + ")");
                            {
                                sub.Add(new MenuSlider("DangerLevel", "Danger Level", spell.DangerValue, 1, 5));
                                sub.Add(new MenuBool("Enabled", "Enabled", !spell.DisabledByDefault));
                                ((Menu)EvadeSkillshotMenu["EvadeSS_" + spell.ChampionName]).Add(sub);
                            }
                        }
                        catch { }
                    }
                }
                menu.Add(EvadeSkillshotMenu);
                DaoHungAIO.Evade.Collision.Init();
                Game.OnUpdate += OnUpdateEvade;
                SkillshotDetector.OnDetectSkillshot += OnDetectSkillshot;
                SkillshotDetector.OnDeleteMissile += OnDeleteMissile;
            }

            public static IsSafeResult IsSafePoint(Vector2 point)
            {
                var result = new IsSafeResult { SkillshotList = new List<Skillshot>() };
                foreach (var skillshot in
                    EvadeManager.DetectedSkillshots.Where(i => i.Evade() && !i.IsSafePoint(point)))
                {
                    result.SkillshotList.Add(skillshot);
                }
                result.IsSafe = result.SkillshotList.Count == 0;
                return result;
            }

            #endregion

            #region Methods

            private static bool IsWard(AIMinionClient obj)
            {
                return obj.Team != GameObjectTeam.Neutral && !MinionManager.IsMinion(obj) && !IsPet(obj)
                       && MinionManager.IsMinion(obj);
            }
            public static bool IsPet(AIMinionClient obj)
            {
                var pets = new[]
                               {
                               "annietibbers", "elisespiderling", "heimertyellow", "heimertblue", "leblanc",
                               "malzaharvoidling", "shacobox", "shaco", "yorickspectralghoul", "yorickdecayedghoul",
                               "yorickravenousghoul", "zyrathornplant", "zyragraspingplant"
                           };
                return pets.Contains(obj.CharacterName.ToLower());
            }
            private static IEnumerable<AIBaseClient> GetEvadeTargets(
                EvadeSpellData spell,
                bool onlyGood = false,
                bool dontCheckForSafety = false)
            {
                var badTargets = new List<AIBaseClient>();
                var goodTargets = new List<AIBaseClient>();
                var allTargets = new List<AIBaseClient>();
                foreach (var targetType in spell.ValidTargets)
                {
                    switch (targetType)
                    {
                        case SpellTargets.AllyChampions:
                            allTargets.AddRange(
                                GameObjects.AllyHeroes.Where(i => i.IsValidTarget(spell.MaxRange, false) && !i.IsMe));
                            break;
                        case SpellTargets.AllyMinions:
                            allTargets.AddRange(GetMinions(spell.MaxRange, MinionManager.MinionTypes.All, MinionTeam.Ally));
                            break;
                        case SpellTargets.AllyWards:
                            allTargets.AddRange(
                                ObjectManager.Get<AIMinionClient>()
                                    .Where(
                                        i =>
                                        IsWard(i) && i.IsValidTarget(spell.MaxRange, false) && i.Team == objPlayer.Team));
                            break;
                        case SpellTargets.EnemyChampions:
                            allTargets.AddRange(HeroManager.Enemies.Where(i => i.IsValidTarget(spell.MaxRange)));
                            break;
                        case SpellTargets.EnemyMinions:
                            allTargets.AddRange(GetMinions(spell.MaxRange, MinionManager.MinionTypes.All, MinionTeam.NotAlly));
                            break;
                        case SpellTargets.EnemyWards:
                            allTargets.AddRange(
                                ObjectManager.Get<AIMinionClient>()
                                    .Where(i => IsWard(i) && i.IsValidTarget(spell.MaxRange)));
                            break;
                    }
                }
                foreach (var target in
                    allTargets.Where(i => dontCheckForSafety || IsSafePoint(i.Position.ToVector2()).IsSafe))
                {
                    if (spell.Name == "YasuoDashWrapper" && target.HasBuff("YasuoDashWrapper"))
                    {
                        continue;
                    }
                    var pathToTarget = new List<Vector2> { objPlayer.Position.ToVector2(), target.Position.ToVector2() };
                    if (IsSafePath(pathToTarget, Configs.EvadingFirstTimeOffset, spell.Speed, spell.Delay).IsSafe)
                    {
                        goodTargets.Add(target);
                    }
                    if (IsSafePath(pathToTarget, Configs.EvadingSecondTimeOffset, spell.Speed, spell.Delay).IsSafe)
                    {
                        badTargets.Add(target);
                    }
                }
                return goodTargets.Any() ? goodTargets : (onlyGood ? new List<AIBaseClient>() : badTargets);
            }

            private static SafePathResult IsSafePath(List<Vector2> path, int timeOffset, int speed = -1, int delay = 0)
            {
                var isSafe = false;
                var intersections = new List<FoundIntersection>();
                var intersection = new FoundIntersection();
                foreach (SafePathResult sResult in
                    EvadeManager.DetectedSkillshots.Where(i => i.Evade())
                        .Select(i => i.IsSafePath(path, timeOffset, speed, delay)))
                {
                    isSafe = sResult.IsSafe;
                    if (sResult.Intersection.Valid)
                    {
                        intersections.Add(sResult.Intersection);
                    }

                }
                return isSafe
                           ? new SafePathResult(true, intersection)
                           : new SafePathResult(
                                 false,
                                 intersections.Count > 0 ? intersections.MinOrDefault(i => i.Distance) : intersection);
            }

            private static void OnDeleteMissile(Skillshot skillshot, MissileClient missile)
            {
                if (skillshot.SpellData.SpellName != "VelkozQ"
                    || EvadeManager.DetectedSkillshots.Count(i => i.SpellData.SpellName == "VelkozQSplit") != 0)
                {
                    return;
                }
                var spellData = SpellDatabase.GetByName("VelkozQSplit");
                for (var i = -1; i <= 1; i = i + 2)
                {
                    EvadeManager.DetectedSkillshots.Add(
                        new Skillshot(
                            DetectionType.ProcessSpell,
                            spellData,
                            Variables.GameTimeTickCount,
                            missile.Position.ToVector2(),
                            missile.Position.ToVector2() + i * skillshot.Perpendicular * spellData.Range,
                            skillshot.Unit));
                }
            }

            private static void OnDetectSkillshot(Skillshot skillshot)
            {
                Game.Print("detected:");
                var alreadyAdded =
                    EvadeManager.DetectedSkillshots.Any(
                        i =>
                        i.SpellData.SpellName == skillshot.SpellData.SpellName
                        && i.Unit.NetworkId == skillshot.Unit.NetworkId
                        && skillshot.Direction.AngleBetween(i.Direction) < 5
                        && (skillshot.Start.Distance(i.Start) < 100 || skillshot.SpellData.FromObjects.Length == 0));
                if (skillshot.Unit.Team == objPlayer.Team)
                {
                    return;
                }
                if (skillshot.Start.Distance(objPlayer.Position.ToVector2())
                    > (skillshot.SpellData.Range + skillshot.SpellData.Radius + 1000) * 1.5)
                {
                    return;
                }
                if (alreadyAdded && !skillshot.SpellData.DontCheckForDuplicates)
                {
                    return;
                }
                if (skillshot.DetectionType == DetectionType.ProcessSpell)
                {
                    if (skillshot.SpellData.MultipleNumber != -1)
                    {
                        var originalDirection = skillshot.Direction;
                        for (var i = -(skillshot.SpellData.MultipleNumber - 1) / 2;
                             i <= (skillshot.SpellData.MultipleNumber - 1) / 2;
                             i++)
                        {
                            EvadeManager.DetectedSkillshots.Add(
                                new Skillshot(
                                    skillshot.DetectionType,
                                    skillshot.SpellData,
                                    skillshot.StartTick,
                                    skillshot.Start,
                                    skillshot.Start
                                    + skillshot.SpellData.Range
                                    * originalDirection.Rotated(skillshot.SpellData.MultipleAngle * i),
                                    skillshot.Unit));
                        }
                        return;
                    }
                    if (skillshot.SpellData.SpellName == "UFSlash")
                    {
                        skillshot.SpellData.MissileSpeed = 1600 + (int)skillshot.Unit.MoveSpeed;
                    }
                    if (skillshot.SpellData.SpellName == "SionR")
                    {
                        skillshot.SpellData.MissileSpeed = (int)skillshot.Unit.MoveSpeed;
                    }
                    if (skillshot.SpellData.Invert)
                    {
                        EvadeManager.DetectedSkillshots.Add(
                            new Skillshot(
                                skillshot.DetectionType,
                                skillshot.SpellData,
                                skillshot.StartTick,
                                skillshot.Start,
                                skillshot.Start
                                + -(skillshot.End - skillshot.Start).Normalized()
                                * skillshot.Start.Distance(skillshot.End),
                                skillshot.Unit));
                        return;
                    }
                    if (skillshot.SpellData.Centered)
                    {
                        EvadeManager.DetectedSkillshots.Add(
                            new Skillshot(
                                skillshot.DetectionType,
                                skillshot.SpellData,
                                skillshot.StartTick,
                                skillshot.Start - skillshot.Direction * skillshot.SpellData.Range,
                                skillshot.Start + skillshot.Direction * skillshot.SpellData.Range,
                                skillshot.Unit));
                        return;
                    }
                    if (skillshot.SpellData.SpellName == "SyndraE" || skillshot.SpellData.SpellName == "syndrae5")
                    {
                        const int Angle = 60;
                        var edge1 =
                            (skillshot.End - skillshot.Unit.Position.ToVector2()).Rotated(
                                -Angle / 2f * (float)Math.PI / 180);
                        var edge2 = edge1.Rotated(Angle * (float)Math.PI / 180);
                        foreach (var skillshotToAdd in from minion in ObjectManager.Get<AIMinionClient>()
                                                       let v =
                                                           (minion.Position - skillshot.Unit.Position).ToVector2(
                                                               )
                                                       where
                                                           minion.Name == "Seed" && edge1.CrossProduct(v) > 0
                                                           && v.CrossProduct(edge2) > 0
                                                           && minion.Distance(skillshot.Unit) < 800
                                                           && minion.Team != objPlayer.Team
                                                       let start = minion.Position.ToVector2()
                                                       let end =
                                                           skillshot.Unit.Position.Extend(
                                                               minion.Position,
                                                               skillshot.Unit.Distance(minion) > 200 ? 1300 : 1000)
                                                           .ToVector2()
                                                       select
                                                           new Skillshot(
                                                           skillshot.DetectionType,
                                                           skillshot.SpellData,
                                                           skillshot.StartTick,
                                                           start,
                                                           end,
                                                           skillshot.Unit))
                        {
                            EvadeManager.DetectedSkillshots.Add(skillshotToAdd);
                        }
                        return;
                    }
                    if (skillshot.SpellData.SpellName == "AlZaharCalloftheVoid")
                    {
                        EvadeManager.DetectedSkillshots.Add(
                            new Skillshot(
                                skillshot.DetectionType,
                                skillshot.SpellData,
                                skillshot.StartTick,
                                skillshot.End - skillshot.Perpendicular * 400,
                                skillshot.End + skillshot.Perpendicular * 400,
                                skillshot.Unit));
                        return;
                    }
                    if (skillshot.SpellData.SpellName == "DianaArc")
                    {
                        EvadeManager.DetectedSkillshots.Add(
                            new Skillshot(
                                skillshot.DetectionType,
                                SpellDatabase.GetByName("DianaArcArc"),
                                skillshot.StartTick,
                                skillshot.Start,
                                skillshot.End,
                                skillshot.Unit));
                    }
                    if (skillshot.SpellData.SpellName == "ZiggsQ")
                    {
                        var d1 = skillshot.Start.Distance(skillshot.End);
                        var d2 = d1 * 0.4f;
                        var d3 = d2 * 0.69f;
                        var bounce1SpellData = SpellDatabase.GetByName("ZiggsQBounce1");
                        var bounce2SpellData = SpellDatabase.GetByName("ZiggsQBounce2");
                        var bounce1Pos = skillshot.End + skillshot.Direction * d2;
                        var bounce2Pos = bounce1Pos + skillshot.Direction * d3;
                        bounce1SpellData.Delay =
                            (int)(skillshot.SpellData.Delay + d1 * 1000f / skillshot.SpellData.MissileSpeed + 500);
                        bounce2SpellData.Delay =
                            (int)(bounce1SpellData.Delay + d2 * 1000f / bounce1SpellData.MissileSpeed + 500);
                        EvadeManager.DetectedSkillshots.Add(
                            new Skillshot(
                                skillshot.DetectionType,
                                bounce1SpellData,
                                skillshot.StartTick,
                                skillshot.End,
                                bounce1Pos,
                                skillshot.Unit));
                        EvadeManager.DetectedSkillshots.Add(
                            new Skillshot(
                                skillshot.DetectionType,
                                bounce2SpellData,
                                skillshot.StartTick,
                                bounce1Pos,
                                bounce2Pos,
                                skillshot.Unit));
                    }
                    if (skillshot.SpellData.SpellName == "ZiggsR")
                    {
                        skillshot.SpellData.Delay =
                            (int)(1500 + 1500 * skillshot.End.Distance(skillshot.Start) / skillshot.SpellData.Range);
                    }
                    if (skillshot.SpellData.SpellName == "JarvanIVDragonStrike")
                    {
                        var endPos = new Vector2();
                        foreach (var s in EvadeManager.DetectedSkillshots)
                        {
                            if (s.Unit.NetworkId == skillshot.Unit.NetworkId && s.SpellData.Slot == SpellSlot.E)
                            {
                                var extendedE = new Skillshot(
                                    skillshot.DetectionType,
                                    skillshot.SpellData,
                                    skillshot.StartTick,
                                    skillshot.Start,
                                    skillshot.End + skillshot.Direction * 100,
                                    skillshot.Unit);
                                if (!extendedE.IsSafePoint(s.End))
                                {
                                    endPos = s.End;
                                }
                                break;
                            }
                        }
                        foreach (var m in ObjectManager.Get<AIMinionClient>())
                        {
                            if (m.CharacterName == "jarvanivstandard" && m.Team == skillshot.Unit.Team)
                            {
                                var extendedE = new Skillshot(
                                    skillshot.DetectionType,
                                    skillshot.SpellData,
                                    skillshot.StartTick,
                                    skillshot.Start,
                                    skillshot.End + skillshot.Direction * 100,
                                    skillshot.Unit);
                                if (!extendedE.IsSafePoint(m.Position.ToVector2()))
                                {
                                    endPos = m.Position.ToVector2();
                                }
                                break;
                            }
                        }
                        if (endPos.IsValid())
                        {
                            skillshot = new Skillshot(
                                DetectionType.ProcessSpell,
                                SpellDatabase.GetByName("JarvanIVEQ"),
                                Variables.GameTimeTickCount,
                                skillshot.Start,
                                endPos + 200 * (endPos - skillshot.Start).Normalized(),
                                skillshot.Unit);
                        }
                    }
                }
                if (skillshot.SpellData.SpellName == "OriannasQ")
                {
                    EvadeManager.DetectedSkillshots.Add(
                        new Skillshot(
                            skillshot.DetectionType,
                            SpellDatabase.GetByName("OriannaQend"),
                            skillshot.StartTick,
                            skillshot.Start,
                            skillshot.End,
                            skillshot.Unit));
                }
                if (skillshot.SpellData.DisableFowDetection && skillshot.DetectionType == DetectionType.RecvPacket)
                {
                    return;
                }
                EvadeManager.DetectedSkillshots.Add(skillshot);
            }

            private static void OnUpdateEvade(EventArgs args)
            {
                //Game.Print("Evade:" + EvadeManager.DetectedSkillshots.Count());
                EvadeManager.DetectedSkillshots.RemoveAll(i => !i.IsActive());
                foreach (var skillshot in EvadeManager.DetectedSkillshots)
                {
                    skillshot.OnUpdate();
                }
                if (objPlayer.IsDead)
                {
                    return;
                }
                if (objPlayer.HasBuffOfType(BuffType.SpellImmunity) || objPlayer.HasBuffOfType(BuffType.SpellShield))
                {
                    return;
                }
                var safePoint = IsSafePoint(objPlayer.Position.ToVector2());
                var safePath = IsSafePath(objPlayer.GetWaypoints(), 100);
                if (!safePath.IsSafe && !safePoint.IsSafe)
                {
                    TryToEvade(safePoint.SkillshotList, Game.CursorPos.ToVector2());
                }
            }

            private static void TryToEvade(List<Skillshot> hitBy, Vector2 to)
            {
                var dangerLevel =
                    hitBy.Select(i => Yasuo.EvadeSkillshotMenu["Spells"]["ESS_" + i.SpellData.MenuItemName].GetValue<MenuSlider>("DangerLevel").Value)
                        .Concat(new[] { 0 })
                        .Max();
                foreach (var evadeSpell in
                    EvadeSpellDatabase.Spells.Where(i => i.Enabled && i.DangerLevel <= dangerLevel && i.IsReady)
                        .OrderBy(i => i.DangerLevel))
                {
                    if (evadeSpell.EvadeType == EvadeTypes.Dash && evadeSpell.CastType == CastTypes.Target)
                    {
                        var targets =
                            GetEvadeTargets(evadeSpell)
                                .Where(
                                    i =>
                                    IsSafePoint(PosAfterE(i).ToVector2()).IsSafe
                                    && (!UnderTower(PosAfterE(i)) || Yasuo.EvadeSkillshotMenu["Spells"]["ESSS_" + evadeSpell.Name].GetValue<MenuBool>("ETower")))
                                .ToList();
                        if (targets.Count > 0)
                        {
                            var closestTarget = targets.MinOrDefault(i => PosAfterE(i).ToVector2().Distance(to));
                            objPlayer.Spellbook.CastSpell(evadeSpell.Slot, closestTarget);
                            return;
                        }
                    }
                    if (evadeSpell.EvadeType == EvadeTypes.WindWall
                        && hitBy.Where(
                            i =>
                            i.SpellData.CollisionObjects.Contains(CollisionObjectTypes.YasuoWall)
                            && i.IsAboutToHit(
                                150 + evadeSpell.Delay - Yasuo.EvadeSkillshotMenu["Spells"]["ESSS_" + evadeSpell.Name].GetValue<MenuSlider>("WDelay").Value,
                                objPlayer))
                               .OrderByDescending(
                                   i => Yasuo.EvadeSkillshotMenu["Spells"]["ESS_" + i.SpellData.MenuItemName].GetValue<MenuSlider>("DangerLevel").Value)
                               .Any(
                                   i =>
                                   objPlayer.Spellbook.CastSpell(
                                       evadeSpell.Slot,
                                       objPlayer.Position.Extend(i.Start.ToVector3(), 100))))
                    {
                        return;
                    }
                }
            }

            #endregion

            internal struct IsSafeResult
            {
                #region Fields

                public bool IsSafe;

                public List<Skillshot> SkillshotList;

                #endregion
            }
        }

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
                        HeroManager.Enemies.Where(i => Spells.Any(a => a.CharacterName == i.CharacterName)))
                    {
                        evadeMenu.Add(new Menu("ET_" + hero.CharacterName, "-> " + hero.CharacterName));
                    }
                    foreach (
                        var spell in Spells.Where(i => HeroManager.Enemies.Any(a => a.CharacterName == i.CharacterName)))
                    {

                        ((Menu)evadeMenu["ET_" + spell.CharacterName]).Add(new MenuBool(
                        spell.MissileName,
                        spell.MissileName + " (" + spell.Slot + ")"));
                    }
                }
                menu.Add(evadeMenu);
                Game.OnUpdate += OnUpdateTarget;
                GameObject.OnMissileCreate += ObjSpellMissileOnCreate;
                GameObject.OnDelete += ObjSpellMissileOnDelete;
                AIBaseClient.OnDoCast += OnProcessSpellCast;
            }

            #endregion

            #region Methods

            private static bool GoThroughWall(Vector2 pos1, Vector2 pos2)
            {
                if (Wall == null)
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
                }
                return intersections.Any();
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
                        SpellNames = new[] { "brandconflagrationmissile", "brandemissile"},
                        Slot = SpellSlot.E
                    });
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Brand",
                        SpellNames = new[] { "brandwildfire", "brandwildfiremissile", "brandr", "brandrmissile"},
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
                        SpellNames = new[] { "ezrealarcaneshiftmissile"},
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
            }

            private static void ObjSpellMissileOnCreate(GameObject sender, EventArgs args)
            {
                if (!(sender is MissileClient))
                {
                    return;
                }
                var missile = (MissileClient)sender;
                if (!(missile.SpellCaster is AIHeroClient) || missile.SpellCaster.Team == objPlayer.Team)
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
                        && evadeMenu["ET_" + i.CharacterName].GetValue<MenuBool>(i.MissileName);
                        }
                        );
                if (spellData == null && Orbwalker.IsAutoAttack(missile.SData.Name)
                    && (!missile.SData.Name.ToLower().Contains("crit")
                            ? evadeMenu.GetValue<MenuBool>("BAttack")
                              && objPlayer.HealthPercent < evadeMenu.GetValue<MenuSlider>("BAttackHpU").Value
                            : evadeMenu.GetValue<MenuBool>("CAttack")
                              && objPlayer.HealthPercent < evadeMenu.GetValue<MenuSlider>("CAttackHpU").Value))
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
                if (missile.SpellCaster is AIHeroClient && missile.SpellCaster.Team != objPlayer.Team)
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
                if (objPlayer.IsDead)
                {
                    return;
                }
                if (objPlayer.HasBuffOfType(BuffType.SpellImmunity) || objPlayer.HasBuffOfType(BuffType.SpellShield))
                {
                    return;
                }
                if (!W.IsReady(300) && (Wall == null || !E.IsReady(200)))
                {
                    return;
                }
                foreach (var target in
                    DetectedTargets.Where(i => objPlayer.Distance(i.Obj.Position) < 700))
                {
                    if (E.IsReady() && evadeMenu.GetValue<MenuBool>("E") && Wall != null
                        && Variables.TickCount - W.LastCastAttemptT > 1000
                        && !GoThroughWall(objPlayer.Position.ToVector2(), target.Obj.Position.ToVector2())
                        && W.IsInRange(target.Obj, 250))
                    {
                        var obj = new List<AIBaseClient>();
                        obj.AddRange(GetMinions(E.Range, MinionManager.MinionTypes.All, MinionTeam.NotAlly));
                        obj.AddRange(HeroManager.Enemies.Where(i => i.IsValidTarget(E.Range)));
                        if (
                            obj.Where(
                                i =>
                                !i.HasBuff("YasuoE") && EvadeSkillshot.IsSafePoint(i.Position.ToVector2()).IsSafe
                                && EvadeSkillshot.IsSafePoint(PosAfterE(i).ToVector2()).IsSafe
                                && (!UnderTower(PosAfterE(i)) || evadeMenu.GetValue<MenuBool>("ETower"))
                                && GoThroughWall(objPlayer.Position.ToVector2(), PosAfterE(i).ToVector2()))
                                .OrderBy(i => PosAfterE(i).Distance(Game.CursorPos))
                                .Any(i => E.CastOnUnit(i)))
                        {
                            return;
                        }
                    }
                    if (W.IsReady() && evadeMenu.GetValue<MenuBool>("W") && W.IsInRange(target.Obj, 500)
                        && W.Cast(objPlayer.Position.Extend(target.Start, 100)))
                    {
                        return;
                    }
                }
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
    }
    internal class EvadeSpellDatabase
    {
        #region Static Fields

        public static List<EvadeSpellData> Spells = new List<EvadeSpellData>();

        #endregion

        #region Constructors and Destructors

        static EvadeSpellDatabase()
        {
            if (ObjectManager.Player.CharacterName != "Yasuo")
            {
                return;
            }
            Spells.Add(
                new EvadeSpellData
                {
                    Name = "YasuoDashWrapper",
                    DangerLevel = 2,
                    Slot = SpellSlot.E,
                    EvadeType = EvadeTypes.Dash,
                    CastType = CastTypes.Target,
                    MaxRange = 475,
                    Speed = 1000,
                    Delay = 50,
                    FixedRange = true,
                    ValidTargets = new[] { SpellTargets.EnemyChampions, SpellTargets.EnemyMinions }
                });
            Spells.Add(
                new EvadeSpellData
                {
                    Name = "YasuoWMovingWall",
                    DangerLevel = 3,
                    Slot = SpellSlot.W,
                    EvadeType = EvadeTypes.WindWall,
                    CastType = CastTypes.Position,
                    MaxRange = 400,
                    Speed = int.MaxValue,
                    Delay = 250
                });
        }

        #endregion
    }
    internal static class Configs
    {
        #region Constants

        public const int EvadingFirstTimeOffset = 250;

        public const int EvadingSecondTimeOffset = 80;

        public const int GridSize = 10;

        public const int SkillShotsExtraRadius = 9;

        public const int SkillShotsExtraRange = 20;

        #endregion
    }
    public enum CastTypes
    {
        Position,

        Target,

        Self
    }

    public enum SpellTargets
    {
        AllyMinions,

        EnemyMinions,

        AllyWards,

        EnemyWards,

        AllyChampions,

        EnemyChampions
    }

    public enum EvadeTypes
    {
        Blink,

        Dash,

        Invulnerability,

        MovementSpeedBuff,

        Shield,

        SpellShield,

        WindWall
    }
    internal class HeroManager
    {
        public static IEnumerable<AIHeroClient> Enemies
        {
            get { return GameObjects.EnemyHeroes; }
        }
    }

    internal class EvadeSpellData
    {
        #region Fields

        public CastTypes CastType;

        public string CheckSpellName = "";

        public int Delay;

        public EvadeTypes EvadeType;

        public bool FixedRange;

        public float MaxRange;

        public string Name;

        public SpellSlot Slot;

        public int Speed;

        public SpellTargets[] ValidTargets;

        private int dangerLevel;

        #endregion

        #region Public Properties

        public int DangerLevel
        {
            get
            {
                try
                {
                    return Yasuo.EvadeSkillshotMenu["Spells"]["ESSS_" + this.Name]["DangerLevel"] != null
                               ? Yasuo.EvadeSkillshotMenu["Spells"]["ESSS_" + this.Name].GetValue<MenuSlider>("DangerLevel").Value
                    : this.dangerLevel;
                }
                catch
                {
                    return this.dangerLevel;
                }
                //return this.dangerLevel;
            }
            set
            {
                this.dangerLevel = value;
            }
        }

        public bool Enabled
        {
            get
            {
                return Yasuo.EvadeSkillshotMenu["Spells"]["ESSS_" + this.Name].GetValue<MenuBool>("Enabled");
            }
        }

        public bool IsReady
        {
            get
            {
                return (this.CheckSpellName == ""
                        || ObjectManager.Player.Spellbook.GetSpell(this.Slot).Name == this.CheckSpellName)
                       && this.Slot.IsReady();
            }
        }
        #endregion
    }
}
