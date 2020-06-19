using System;
using System.Linq;
using SharpDX;
using System.Collections.Generic;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Events;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.Prediction;
using EnsoulSharp.SDK.Utility;

namespace DominationAIO.Champions.Aphelios
{
    internal class Program
    {
        /*private static void Main(string[] args)
        {
            GameEvent.OnGameLoad += OnGameLoad;
        }*/
        private static void OnGameLoad()
        {
            if (ObjectManager.Player.CharacterName != "Aphelios")
                return;

            loaded.OnLoad();
        }
    }
    internal class MenuSettings
    {
        public static MenuSeparator secsec = new MenuSeparator("secsec", "________________________________________________");
        public class Combo
        {
            public static MenuSeparator comboSeparator = new MenuSeparator("comboSeparator", "Combo Settings");
            public static MenuBool Qcombo = new MenuBool("Qcombo", "Use Q in combo");
            public static MenuBool Calibrum = new MenuBool("Calibrum", "^  Use Q Calibrum in combo");
            public static MenuBool Severum = new MenuBool("Severum", "^  Use Q Severum in combo");
            public static MenuBool Gravitum = new MenuBool("Gravitum", "^  Use Q Gravitum in combo");
            public static MenuBool Infernum = new MenuBool("Infernum", "^  Use Q Infernum in combo");
            public static MenuBool Crescendum = new MenuBool("Crescendum", "^  Use Q Crescendum in combo");
            public static MenuBool Wcombo = new MenuBool("Wcombo", "Use W in combo");
            public static MenuBool Rcombo = new MenuBool("Rcombo", "Use R on combo");
            public static MenuSlider Rheath = new MenuSlider("Rheath", "^  If target heath <= %", 40);
            public static MenuSlider Rhit = new MenuSlider("Rhit", "^  If target hit count >=", 3, 0, 5);
        }
        public class Harass
        {
            public static MenuSeparator harassSeparator = new MenuSeparator("harassSeparator", "Harass Settings");
            public static MenuBool Qharass = new MenuBool("Qharass", "Use Q in harass");
            public static MenuBool Calibrum = new MenuBool("Qharass", "^  Use Q Calibrum in harass");
            public static MenuBool Severum = new MenuBool("Qharass", "^  Use Q Severum in harass");
            public static MenuBool Gravitum = new MenuBool("Qharass", "^  Use Q Gravitum in harass");
            public static MenuBool Infernum = new MenuBool("Qharass", "^  Use Q Infernum in harass");
            public static MenuBool Crescendum = new MenuBool("Qharass", "^  Use Q Crescendum in harass");
            public static MenuBool Wharass = new MenuBool("Wharass", "Use W in harass");
            public static MenuSlider ManaHarass = new MenuSlider("ManaHarass", "|| Only 'Harass' if mana >=", 30);
        }
        public class LaneClear
        {
            public static MenuSeparator laneClearSeperator = new MenuSeparator("laneClearSeperator", "Lane Clear Settings");
            public static MenuBool QLaneClear = new MenuBool("Qharass", "Use Q in LaneClear");
            public static MenuBool Calibrum = new MenuBool("Qharass", "^  Use Q Calibrum in LaneClear");
            public static MenuBool Severum = new MenuBool("Qharass", "^  Use Q Severum in LaneClear");
            public static MenuBool Gravitum = new MenuBool("Qharass", "^  Use Q Gravitum in LaneClear");
            public static MenuBool Infernum = new MenuBool("Qharass", "^  Use Q Infernum in LaneClear");
            public static MenuBool Crescendum = new MenuBool("Qharass", "^  Use Q Crescendum in LaneClear");
            public static MenuBool WLaneClear = new MenuBool("Wharass", "Use W in LaneClear");
            public static MenuSlider ManaLaneClear = new MenuSlider("ManaLaneClear", "|| Only 'Lane Clear' if mana >=", 30);
        }

        public class JungleClear
        {
            public static MenuSeparator jungleClearSeparator    = new MenuSeparator("jungleClearSeparator", "Jungle Clear Settings");
            public static MenuBool QJungleClear = new MenuBool("Qharass", "Use Q in JungleClear");
            public static MenuBool Calibrum = new MenuBool("Qharass", "^  Use Q Calibrum in JungleClear");
            public static MenuBool Severum = new MenuBool("Qharass", "^  Use Q Severum in JungleClear");
            public static MenuBool Gravitum = new MenuBool("Qharass", "^  Use Q Gravitum in JungleClear");
            public static MenuBool Infernum = new MenuBool("Qharass", "^  Use Q Infernum in JungleClear");
            public static MenuBool Crescendum = new MenuBool("Qharass", "^  Use Q Crescendum in JungleClear");
            public static MenuBool WJungleClear = new MenuBool("Wharass", "Use W in JungleClear");
            public static MenuSlider ManaJungleClear = new MenuSlider("ManaJungleClear", "|| Only 'Jungle Clear' is mana >=", 30);
        }
    }
    public class loaded
    {
        private static AIHeroClient objPlayer = ObjectManager.Player;
        private static AIHeroClient Player = ObjectManager.Player;
        private static Menu Amenu;
        private static Spell W, R;
        private static Spell Calibrum, Severum, Gravitum, Infernum, Crescendum;


        private static bool Q1isready, Q2isready, Q3isready, Q4isready, Q5isready;
        private static bool R1isready, R2isready, R3isready, R4isready, R5isready;

        private static float lastCalibrum, lastSeverum, lastGravitum, lastInfernum, lastCrescendum;
        private static float lastW, lastR;

        private static bool beforeaa, onaa, afteraa;

        private static SpellSlot summonerIgnite;

        public static void OnLoad()
        {
            Calibrum = new Spell(SpellSlot.Q, 1450f);
            Severum = new Spell(SpellSlot.Q, 550f);
            Gravitum = new Spell(SpellSlot.Q);
            Infernum = new Spell(SpellSlot.Q, 650f);
            Crescendum = new Spell(SpellSlot.Q, 475f);

            W = new Spell(SpellSlot.W);
            R = new Spell(SpellSlot.R, 1300f);

            Calibrum.SetSkillshot(0.3f, 90f, 1850f, true, SkillshotType.Line);
            Severum.SetTargetted(0.3f, float.MaxValue);

            Infernum.SetSkillshot(0.3f, 1.35f, float.MaxValue, true, SkillshotType.Cone);
            Crescendum.SetSkillshot(0.3f, 575, float.MaxValue, false, SkillshotType.Circle);

            R.SetSkillshot(0.5f, 110f, 2050f, true, SkillshotType.Line);

            Game.Print("<font color='#1dff00' size='25'>Aphelios The MachineGun loaded</font>");

            #region Menu Init

            Amenu = new Menu(objPlayer.CharacterName, "Aphelios The MachineGun", true);

            var comboMenu = new Menu("comboMenu", "Combo")
            {
                MenuSettings.Combo.comboSeparator,
                MenuSettings.secsec,
                MenuSettings.Combo.Qcombo,
                MenuSettings.Combo.Calibrum,
                MenuSettings.Combo.Severum,
                MenuSettings.Combo.Gravitum,
                MenuSettings.Combo.Infernum,
                MenuSettings.Combo.Crescendum,
                MenuSettings.Combo.Wcombo,
                MenuSettings.Combo.Rcombo,
                MenuSettings.Combo.Rheath,
                MenuSettings.Combo.Rhit
            };
            Amenu.Add(comboMenu);

            /*var harassMenu = new Menu("harassMenu", "Harass")
            {
                MenuSettings.Harass.harassSeparator,
                MenuSettings.secsec,
                MenuSettings.Harass.Qharass,
                MenuSettings.Harass.Calibrum,
                MenuSettings.Harass.Severum,
                MenuSettings.Harass.Gravitum,
                MenuSettings.Harass.Infernum,
                MenuSettings.Harass.Crescendum,
                MenuSettings.Harass.Wharass,
                MenuSettings.Harass.ManaHarass
            };
            Amenu.Add(harassMenu);

            var laneClearMenu = new Menu("laneClearMenu", "Lane Clear")
            {
                MenuSettings.LaneClear.laneClearSeperator,
                MenuSettings.secsec,
                MenuSettings.LaneClear.QLaneClear,
                MenuSettings.LaneClear.Calibrum,
                MenuSettings.LaneClear.Severum,
                MenuSettings.LaneClear.Gravitum,
                MenuSettings.LaneClear.Infernum,
                MenuSettings.LaneClear.Crescendum,
                MenuSettings.LaneClear.WLaneClear,
                MenuSettings.LaneClear.ManaLaneClear
            };
            Amenu.Add(laneClearMenu);

            var jungleClearMenu = new Menu("jungleClearMenu", "Jungle Clear")
            {
                MenuSettings.JungleClear.jungleClearSeparator,
                MenuSettings.secsec,
                MenuSettings.JungleClear.QJungleClear,
                MenuSettings.JungleClear.Calibrum,
                MenuSettings.JungleClear.Severum,
                MenuSettings.JungleClear.Gravitum,
                MenuSettings.JungleClear.Infernum,
                MenuSettings.JungleClear.Crescendum,
                MenuSettings.JungleClear.WJungleClear,
                MenuSettings.JungleClear.ManaJungleClear
            };
            Amenu.Add(jungleClearMenu);*/

            Amenu.Attach();

            #endregion

            Game.OnUpdate                    += OnUpdate;
            Orbwalker.OnAction              += OnAction;
            AIBaseClient.OnProcessSpellCast += AIBaseClient_OnProcessSpellCast;
        }
              
        private static void OnUpdate(EventArgs args)
        {
            if (objPlayer.IsDead || objPlayer.IsRecalling())
                return;
            if (MenuGUI.IsChatOpen || MenuGUI.IsShopOpen)
                return;
            CheckAll();
            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                        SimpleCombo();
                    break;
                case OrbwalkerMode.Harass:
                    if (MenuSettings.Harass.ManaHarass.Value < Player.Mana)
                        SimpleCombo();
                    break;
                case OrbwalkerMode.LaneClear:
                    if (MenuSettings.LaneClear.ManaLaneClear.Value < Player.Mana)
                        SimpleLaneClear();
                    break;
                case OrbwalkerMode.LastHit:
                    break;
            }
        }

        #region Orbwalker Modes
        private static void CheckAll()
        {
            if (R.IsReady())
            {
                if (Player.HasBuff("ApheliosOffHandBuffCalibrum") || Player.HasBuff("ApheliosCalibrumManager"))
                {
                    R1isready = true;
                }
                else
                {
                    R1isready = false;
                }

                if (Player.HasBuff("ApheliosOffHandBuffSevernum") || Player.HasBuff("ApheliosSevernumManager"))
                {
                    R2isready = true;
                }
                else
                {
                    R2isready = false;
                }

                if (Player.HasBuff("ApheliosOffHandBuffGravitum") || Player.HasBuff("ApheliosGravitumManager"))
                {
                    R3isready = true;
                }
                else
                {
                    R3isready = false;
                }

                if (Player.HasBuff("ApheliosOffHandBuffInfernum") || Player.HasBuff("ApheliosInfernumManager"))
                {
                    R4isready = true;
                }
                else
                {
                    R4isready = false;
                }

                if (Player.HasBuff("ApheliosOffHandBuffCrescendum") || Player.HasBuff("ApheliosCrescendumManager"))
                {
                    R5isready = true;
                }
                else
                {
                    R5isready = false;
                }
            }

            //Check Q
                if (Game.Time * 1000 - lastCalibrum >= Q1Delay() * 1000)
                {
                    Q1isready = true;
                }
                else
                {
                    Q1isready = false;
                }

                if (Game.Time * 1000 - lastSeverum >= Q2Delay() * 1000)
                {
                    Q2isready = true;
                }
                else
                {
                    Q2isready = false;
                }

                if (Game.Time * 1000 - lastGravitum >= Q3Delay() * 1000)
                {
                    Q3isready = true;
                }
                else
                {
                    Q3isready = false;
                }

                if (Game.Time * 1000 - lastInfernum >= Q4Delay() * 1000)
                {
                    Q4isready = true;
                }
                else
                {
                    Q4isready = false;
                }

                if (Game.Time * 1000 - lastCrescendum >= Q4Delay() * 1000)
                {
                    Q5isready = true;
                }
                else
                {
                    Q5isready = false;
                }
        }
        private static float Q1Delay()
        {
            var Delay = 10F;
            if (Player.Level > 1 && Player.Level < 3)
            {
                Delay = 10F;
            }
            if (Player.Level >= 3 && Player.Level < 5)
            {
                Delay = 9.6f;
            }
            if (Player.Level >= 5 && Player.Level < 7)
            {
                Delay = 9.3f;
            }
            if (Player.Level >= 7 && Player.Level < 9)
            {
                Delay = 9f;
            }
            if (Player.Level >= 9 && Player.Level < 11)
            {
                Delay = 8.6f;
            }
            if (Player.Level >= 11 && Player.Level < 13)
            {
                Delay = 8.3f;
            }
            if (Player.Level >= 13)
            {
                Delay = 8f;
            }
            var RealDelay = Delay + Delay * Player.PercentCooldownMod - Game.Ping / 1000;
            return RealDelay;
        }
        private static float Q2Delay()
        {
            var Delay = 10F;
            if (Player.Level > 1 && Player.Level < 3)
            {
                Delay = 10F;
            }
            if (Player.Level >= 3 && Player.Level < 5)
            {
                Delay = 10F;
            }
            if (Player.Level >= 5 && Player.Level < 7)
            {
                Delay = 9;
            }
            if (Player.Level >= 7 && Player.Level < 9)
            {
                Delay = 9;
            }
            if (Player.Level >= 9 && Player.Level < 11)
            {
                Delay = 8;
            }
            if (Player.Level >= 11 && Player.Level < 13)
            {
                Delay = 8;
            }
            if (Player.Level >= 13)
            {
                Delay = 7;
            }
            var RealDelay = Delay + Delay * Player.PercentCooldownMod - Game.Ping / 1000;
            return RealDelay;
        }
        private static float Q3Delay()
        {
            var Delay = 12f;
            if (Player.Level > 1 && Player.Level < 3)
            {
                Delay = 12f;
            }
            if (Player.Level >= 3 && Player.Level < 5)
            {
                Delay = 11.5f;
            }
            if (Player.Level >= 5 && Player.Level < 7)
            {
                Delay = 11.2f;
            }
            if (Player.Level >= 7 && Player.Level < 9)
            {
                Delay = 10.9f;
            }
            if (Player.Level >= 9 && Player.Level < 11)
            {
                Delay = 10.5f;
            }
            if (Player.Level >= 11 && Player.Level < 13)
            {
                Delay = 10.2f;
            }
            if (Player.Level >= 13)
            {
                Delay = 9.9f;
            }
            var RealDelay = Delay + Delay * Player.PercentCooldownMod - Game.Ping / 1000;
            return RealDelay;
        }
        private static float Q4Delay()
        {
            var Delay = 9f;
            if (Player.Level > 1 && Player.Level < 3)
            {
                Delay = 9f;
            }
            if (Player.Level >= 3 && Player.Level < 5)
            {
                Delay = 8.6f;
            }
            if (Player.Level >= 5 && Player.Level < 7)
            {
                Delay = 8.3f;
            }
            if (Player.Level >= 7 && Player.Level < 9)
            {
                Delay = 8f;
            }
            if (Player.Level >= 9 && Player.Level < 11)
            {
                Delay = 7.6f;
            }
            if (Player.Level >= 11 && Player.Level < 13)
            {
                Delay = 7.3f;
            }
            if (Player.Level >= 13)
            {
                Delay = 7f;
            }
            var RealDelay = Delay + Delay * Player.PercentCooldownMod - Game.Ping / 1000;
            return RealDelay;
        }
        private static float Q5Delay()
        {
            var Delay = 9f;
            if (Player.Level > 1 && Player.Level < 3)
            {
                Delay = 9f;
            }
            if (Player.Level >= 3 && Player.Level < 5)
            {
                Delay = 8.5f;
            }
            if (Player.Level >= 5 && Player.Level < 7)
            {
                Delay = 8.2f;
            }
            if (Player.Level >= 7 && Player.Level < 9)
            {
                Delay = 7.9f;
            }
            if (Player.Level >= 9 && Player.Level < 11)
            {
                Delay = 7.5f;
            }
            if (Player.Level >= 11 && Player.Level < 13)
            {
                Delay = 7.2f;
            }
            if (Player.Level >= 13)
            {
                Delay = 6.9f;
            }
            var RealDelay = Delay + Delay * Player.PercentCooldownMod - Game.Ping / 1000;
            return RealDelay;
        }
        private static void SimpleCombo()
        {
            foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(100000)))
            {
                var target1 = TargetSelector.GetTarget(100000);
                if (target != null || target1 != null)
                {
                    if (MenuSettings.Combo.Qcombo.Enabled)
                    {
                        if (Calibrum.IsReady())
                        {
                            if (Player.HasBuff("ApheliosCalibrumManager"))
                            {
                                CastQCalibrum(target);
                            }
                            if (Player.HasBuff("ApheliosSeverumManager"))
                            {
                                CastQSeverum(target);
                            }
                            if (Player.HasBuff("ApheliosGravitumManager"))
                            {
                                CastQGravitum(target);
                            }
                            if (Player.HasBuff("ApheliosInfernumManager"))
                            {
                                CastQInfernum(target);
                            }
                            if (Player.HasBuff("ApheliosCrescendumManager"))
                            {
                                CastQCrescendum(target);
                            }
                        }
                    }
                    if (Orbwalker.ActiveMode == OrbwalkerMode.Combo && MenuSettings.Combo.Rcombo.Enabled)
                    {
                        if (R4isready)
                        {
                            if (R.IsReady())
                                if (Player.HasBuff("ApheliosInfernumManager"))
                                {
                                    CastR(target1);
                                }
                                else
                                {
                                    if (!onaa)
                                        W.Cast(target);
                                    CastR(target1);
                                }
                        }
                        else
                        {
                            if (R.IsReady())
                                CastR(target1);
                        }
                    }
                    if (MenuSettings.Combo.Wcombo.Enabled)
                    {
                        if (Player.HasBuff("ApheliosOffHandBuffCalibrum") && target1.DistanceToPlayer() > 550)
                        {
                            if (!onaa)
                                W.Cast(target1);
                        }
                        if (Player.HasBuff("ApheliosOffHandBuffGravitum") && target1.DistanceToPlayer() > 550)
                        {
                            if (!Player.HasBuff("ApheliosCalibrumManager") || !Player.HasBuff("ApheliosOffHandBuffCalibrum"))
                            {
                                if (!onaa)
                                    W.Cast(target1);
                            }
                        }
                    }
                    if (target.HasBuff("aphelioscalibrumbonusrangebuff"))
                    {
                        Orbwalker.Attack(target);
                    }

                    if (MenuSettings.Combo.Qcombo.Enabled)
                    {
                        if (Q1isready && target1.IsValidTarget(Calibrum.Range))
                        {
                            if (Player.HasBuff("ApheliosOffHandBuffCalibrum"))
                            {
                                if (!Calibrum.IsReady() && !onaa)
                                    W.Cast(target);
                                CastQCalibrum(target1);
                            }
                            if (Player.HasBuff("ApheliosCalibrumManager"))
                            {
                                CastQCalibrum(target1);
                            }
                        }

                        if (Q2isready && target1.IsValidTarget(objPlayer.GetRealAutoAttackRange()))
                        {
                            if (Player.HasBuff("ApheliosOffHandBuffSeverum"))
                            {
                                if (!Calibrum.IsReady() && !onaa)
                                    W.Cast(target);
                                CastQSeverum(target1);
                            }
                            if (Player.HasBuff("ApheliosSeverumManager"))
                            {
                                CastQSeverum(target1);
                            }
                        }

                        if (Q3isready)
                        {
                            if (target1.IsValidTarget(550) || target1.HasBuff("ApheliosGravitumDebuff"))
                            {
                                if (Player.HasBuff("ApheliosOffHandBuffGravitum"))
                                {
                                    if (!Calibrum.IsReady() && !onaa)
                                        W.Cast(target);
                                    CastQGravitum(target);
                                }
                                if (Player.HasBuff("ApheliosGravitumManager"))
                                {
                                    CastQGravitum(target);
                                }
                            }
                        }

                        if (Q4isready && target1.IsValidTarget(Infernum.Range))
                        {
                            if (Player.HasBuff("ApheliosOffHandBuffInfernum"))
                            {
                                if (!Calibrum.IsReady() && !onaa)
                                    W.Cast(target);
                                CastQInfernum(target1);
                            }
                            if (Player.HasBuff("ApheliosInfernumManager"))
                            {
                                CastQInfernum(target1);
                            }
                        }

                        if (Q5isready && target1.IsValidTarget(Crescendum.Range + 100))
                        {
                            if (Player.HasBuff("ApheliosOffHandBuffCrescendum"))
                            {
                                if (!Calibrum.IsReady() && !onaa)
                                    W.Cast(target);
                                CastQCrescendum(target1);
                            }
                            if (Player.HasBuff("ApheliosCrescendumManager"))
                            {
                                CastQCrescendum(target1);
                            }
                        }
                    }
                }
            }
        }

        private static void SimpleLaneClear()
        {
            foreach (var target in GameObjects.Jungle.Where(x => x.IsValidTarget(100000)))
            {
                if (target != null)
                {
                    if (MenuSettings.Combo.Qcombo.Enabled)
                    {
                        if (Calibrum.IsReady())
                        {
                            if (Player.HasBuff("ApheliosCalibrumManager"))
                            {
                                CastQCalibrum(target);
                            }
                            if (Player.HasBuff("ApheliosSeverummManager"))
                            {
                                CastQSeverum(target);
                            }
                            if (Player.HasBuff("ApheliosGravitumManager"))
                            {
                                CastQGravitum(target);
                            }
                            if (Player.HasBuff("ApheliosInfernumManager"))
                            {
                                CastQInfernum(target);
                            }
                            if (Player.HasBuff("ApheliosCrescendumManager"))
                            {
                                CastQCrescendum(target);
                            }
                        }
                    }
                    if (Orbwalker.ActiveMode == OrbwalkerMode.Combo && MenuSettings.Combo.Rcombo.Enabled)
                    {
                        if (R4isready)
                        {
                            if (R.IsReady())
                                if (Player.HasBuff("ApheliosInfernumManager"))
                                {
                                    CastR(target);
                                }
                                else
                                {
                                    W.Cast(target);
                                    CastR(target);
                                }
                        }
                        else
                        {
                            if (R.IsReady())
                                CastR(target);
                        }
                    }
                    if (MenuSettings.Combo.Wcombo.Enabled)
                    {
                        if (Player.HasBuff("ApheliosOffHandBuffCalibrum") && target.DistanceToPlayer() > 550)
                        {
                            W.Cast(target);
                        }
                        if (Player.HasBuff("ApheliosOffHandBuffGravitum") && target.DistanceToPlayer() > 550)
                        {
                            if (!Player.HasBuff("ApheliosCalibrumManager") || !Player.HasBuff("ApheliosOffHandBuffCalibrum"))
                            {
                                W.Cast(target);
                            }
                        }
                    }
                    if (target.HasBuff("aphelioscalibrumbonusrangebuff"))
                    {
                        Orbwalker.Attack(target);
                    }

                    if (MenuSettings.Combo.Qcombo.Enabled)
                    {
                        if (Q1isready && target.IsValidTarget(Calibrum.Range))
                        {
                            if (Player.HasBuff("ApheliosOffHandBuffCalibrum"))
                            {
                                if (!Calibrum.IsReady())
                                    W.Cast(target);
                                CastQCalibrum(target);
                            }
                            if (Player.HasBuff("ApheliosCalibrumManager"))
                            {
                                CastQCalibrum(target);
                            }
                        }

                        if (Q2isready && target.IsValidTarget(objPlayer.GetRealAutoAttackRange()))
                        {
                            if (Player.HasBuff("ApheliosOffHandBuffSeverum"))
                            {
                                if (!Calibrum.IsReady())
                                    W.Cast(target);
                                CastQSeverum(target);
                            }
                            if (Player.HasBuff("ApheliosSeverumManager"))
                            {
                                CastQSeverum(target);
                            }
                        }

                        if (Q3isready)
                        {
                            if (target.IsValidTarget(550) || target.HasBuff("ApheliosGravitumDebuff"))
                            {
                                if (Player.HasBuff("ApheliosOffHandBuffGravitum"))
                                {
                                    if (!Calibrum.IsReady())
                                        W.Cast(target);
                                    CastQGravitum(target);
                                }
                                if (Player.HasBuff("ApheliosGravitumManager"))
                                {
                                    CastQGravitum(target);
                                }
                            }
                        }

                        if (Q4isready && target.IsValidTarget(Infernum.Range))
                        {
                            if (Player.HasBuff("ApheliosOffHandBuffInfernum"))
                            {
                                if (!Calibrum.IsReady())
                                    W.Cast(target);
                                CastQInfernum(target);
                            }
                            if (Player.HasBuff("ApheliosInfernumManager"))
                            {
                                CastQInfernum(target);
                            }
                        }

                        if (Q5isready && target.IsValidTarget(Crescendum.Range + 100))
                        {
                            if (Player.HasBuff("ApheliosOffHandBuffCrescendum"))
                            {
                                if (!Calibrum.IsReady())
                                    W.Cast(target);
                                CastQCrescendum(target);
                            }
                            if (Player.HasBuff("ApheliosCrescendumManager"))
                            {
                                CastQCrescendum(target);
                            }
                        }
                    }
                }
            }
        }
        private static void CastQCalibrum(AIBaseClient target)
        {
            var Qpred = Calibrum.GetPrediction(target, false, -1, CollisionObjects.YasuoWall
                                                                | CollisionObjects.Minions);
            if (Calibrum.IsReady())
                if (Q1isready && target != null && target.IsValidTarget(Calibrum.Range))
                {
                    if (Qpred.Hitchance >= HitChance.High && Qpred.CastPosition.DistanceToPlayer() < Calibrum.Range)
                    {
                        if (!onaa || !beforeaa)
                            if (MenuSettings.Combo.Calibrum.Enabled)
                                Calibrum.Cast(Qpred.CastPosition);
                    }
                }
        }
        private static void CastQSeverum(AIBaseClient target)
        {
            if (target.IsValidTarget(550) && Severum.IsReady())
            {
                if (MenuSettings.Combo.Severum.Enabled)
                    Severum.Cast(target);
            }
        }
        private static void CastQGravitum(AIBaseClient target)
        {
            if (Gravitum.IsReady())
                if (Q3isready && target != null && target.IsValidTarget(R.Range))
                {
                    if (target.HasBuff("ApheliosGravitumDebuff"))
                    {
                        if (!onaa || !beforeaa)
                            if (MenuSettings.Combo.Gravitum.Enabled)
                                Gravitum.Cast(target);
                    }
                }
        }
        private static void CastQInfernum(AIBaseClient target)
        {
            var Qpred = Infernum.GetPrediction(target, false, -1, CollisionObjects.YasuoWall);
            if (Infernum.IsReady())
                if (Q4isready && target != null && target.IsValidTarget(Infernum.Range))
                {
                    if (Qpred.Hitchance >= HitChance.High && Qpred.CastPosition.DistanceToPlayer() < Infernum.Range)
                    {
                        if (!onaa || !beforeaa)
                            if (MenuSettings.Combo.Infernum.Enabled)
                                Infernum.Cast(Qpred.CastPosition);
                    }
                }
        }
        private static void CastQCrescendum(AIBaseClient target)
        {
            if (Crescendum.IsReady())
                if (Q5isready && target != null && target.IsValidTarget(700))
                {
                    if (!onaa || !beforeaa)
                    {
                        if (MenuSettings.Combo.Crescendum.Enabled)
                            Crescendum.Cast(target.Position);
                    }
                }
        }
        private static void CastR(AIBaseClient target)
        {
            var Rpred = R.GetPrediction(target, false, -1, CollisionObjects.YasuoWall);
            if (R.IsReady() && target != null && target.IsValidTarget(R.Range))
            {
                if (Rpred.Hitchance >= HitChance.High && Rpred.CastPosition.DistanceToPlayer() < R.Range)
                {
                    if (!onaa || !beforeaa)
                    {
                        if (target.HealthPercent <= MenuSettings.Combo.Rheath.Value)
                        {
                            R.Cast(Rpred.CastPosition);
                        }
                        try
                        {
                            var targets = GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(R.Range)).ToArray();
                            var castPos = Vector3.Zero;

                            if (!targets.Any())
                            {
                                return;
                            }

                            foreach (var pred in
                                targets.Select(i => R.GetPrediction(i, false, -1, CollisionObjects.Heroes
                                                         | CollisionObjects.YasuoWall))
                                    .Where(
                                        i => i.Hitchance >= HitChance.Medium && i.AoeTargetsHitCount >= MenuSettings.Combo.Rhit.Value)
                                    .OrderByDescending(i => i.AoeTargetsHitCount))
                            {
                                castPos = pred.CastPosition;
                                break;
                            }

                            if (castPos != Vector3.Zero && castPos.DistanceToPlayer() <= R.Range)
                            {
                                R.Cast(castPos);
                            }

                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("R hit : " + ex);
                        }
                    }
                }
            }
        }

        #endregion

        #region Events

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

        private static void AIBaseClient_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.SData.Name == "ApheliosCalibrumQ")
                {
                    lastCalibrum = Variables.GameTimeTickCount;
                }
                if (args.SData.Name == "ApheliosSeverumQ")
                {
                    lastSeverum = Variables.GameTimeTickCount;
                }
                if (args.SData.Name == "ApheliosGravitumQ")
                {
                    lastGravitum = Variables.GameTimeTickCount;
                }
                if (args.SData.Name == "ApheliosInfernumQ")
                {
                    lastInfernum = Variables.GameTimeTickCount;
                }
                if (args.SData.Name == "ApheliosCrescendumQ")
                {
                    lastCrescendum = Variables.GameTimeTickCount;
                }
            }
        }
        #endregion
    }
}
