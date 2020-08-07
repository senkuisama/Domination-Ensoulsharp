using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.Prediction;
using EnsoulSharp.SDK.Utility;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Template
{
    internal class Program
    {
        /*private static void Main(string[] args)
        {
            GameEvent.OnGameLoad += OnGameLoad;
        }*/
        private static void OnGameLoad()
        {
            if (ObjectManager.Player.CharacterName != "Irelia")
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
            public static MenuBool useQ = new MenuBool("useQ", "Use Q > Only if E or R hit || Gapcloser logic");
            public static MenuSlider Qmd = new MenuSlider("Qmd", "^ Use Q More Dash > MoreDash if objplayer Heath <= %", 80);
            public static MenuBool Qmaxstacks = new MenuBool("Qmaxstacks", "^ Use Q Max Stacks > Active more Dash if have max stacks", false);
            public static MenuSlider Qat = new MenuSlider("Qat", "^ Use Q Max Stacks > Auto turn on if objplayer Heath <= %", 40);
            public static MenuBool useW = new MenuBool("useW", "Use W > in start Combo");
            public static MenuBool useE = new MenuBool("useE", "Use E > Logic E cast");
            public static MenuBool useR = new MenuBool("useR", "Use R > if will hit x enermy || if target heath <= y%");
            public static MenuSlider x = new MenuSlider("x", "x value", 3, 1, 5);
            public static MenuSlider y = new MenuSlider("y", "y value", 60);
        }
        public class Harass
        {
            public static MenuSeparator harassSeparator = new MenuSeparator("harassSeparator", "Harass Settings");
            public static MenuBool useQ = new MenuBool("useQ", "Use Q > only E hit || R hit");
            public static MenuBool useW = new MenuBool("useW", "Use W > Auto changed");
            public static MenuBool useE = new MenuBool("useE", "Use E > Logic E cast");
        }
        public class LaneClear
        {
            public static MenuSeparator laneClearSeperator = new MenuSeparator("laneClearSeperator", "Lane Clear Settings");
            public static MenuBool useQ = new MenuBool("useQ", "Use Q > only can kill minion");
            public static MenuBool mana = new MenuBool("mana", "Wont use Q if mana < x");
            public static MenuSlider x = new MenuSlider("x", "X value", 40);
        }

        public class JungleClear
        {
            public static MenuSeparator jungleClearSeparator    = new MenuSeparator("jungleClearSeparator", "Jungle Clear Settings");
            public static MenuBool useQ = new MenuBool("useQ", "Use Q > when E hit");
            public static MenuBool useE = new MenuBool("useE", "Use E > Simple E cast");
        }
        public class LastHit
        {
            public static MenuSeparator lastHitSeparator        = new MenuSeparator("lastHitSeparator", "Last Hit Settings");
            public static MenuBool useQ = new MenuBool("useQ", "Use Q if can kill minion");
        }
        public class Misc
        {
            public static MenuSeparator miscSeparator           = new MenuSeparator("miscSeparator", "Misc Settings");
        }
        public class Drawing
        {
            public static MenuSeparator drawingSeparator        = new MenuSeparator("drawingSeparator", "Drawings");
            public static MenuBool disableDrawings              = new MenuBool("disableDrawings", "Disable", false);
            public static MenuBool drawDmg                      = new MenuBool("drawDmg", "Damage Indicator");
            public static MenuSeparator rangesSeperator         = new MenuSeparator("rangesSeperator", "Spell Ranges");
            public static MenuBool drawQ                        = new MenuBool("drawQ", "Q Range");
            public static MenuBool drawW                        = new MenuBool("drawW", "W Range");
            public static MenuBool drawE                        = new MenuBool("drawE", "E Range");
            public static MenuBool drawR                        = new MenuBool("drawR", "R Range");
        }
    }
    public class loaded
    {
        private static AIHeroClient objPlayer = ObjectManager.Player;
        private static Menu myMenu;
        public static Spell Q, Q1, Q2, Q3, Q4;
        public static Spell W, W1, W2, W3, W4;
        public static Spell E, E1, E2, E3, E4;
        public static Spell R, R1, R2, R3, R4;

        public static void OnLoad()
        {
            Q = new Spell(SpellSlot.Q, 600f);
            Q.SetTargetted(0f, Qspeed());

            W = new Spell(SpellSlot.W, 825f);
            W.SetCharged("IreliaW", "ireliawdefense", 800, 800, 0);

            E = new Spell(SpellSlot.E, 775f);
            E.SetSkillshot(0.25f, 80f, 1800f, false, false, SkillshotType.Line);

            E1 = new Spell(SpellSlot.Unknown, 775f);
            E1.SetSkillshot(0.15f + (0.25f * 2), 80, 1800f, false, false, SkillshotType.Circle);

            E2 = new Spell(SpellSlot.Unknown, 775f);
            E2.SetSkillshot(0.25f, 80f, 1800f, false, false, SkillshotType.Line);

            R = new Spell(SpellSlot.R, 800f);
            R.SetSkillshot(0.25f, 300, 1500, true, SkillshotType.Line);

            myMenu = new Menu(objPlayer.CharacterName, "Irelia The Flash", true);

            var comboMenu = new Menu("comboMenu", "Combo")
            {
                MenuSettings.Combo.comboSeparator,
                MenuSettings.secsec,
                MenuSettings.Combo.useQ,
                MenuSettings.Combo.Qmd,
                MenuSettings.Combo.useW,
                MenuSettings.Combo.useE,
                MenuSettings.Combo.useR,
                MenuSettings.Combo.x,
                MenuSettings.Combo.y,
            };
            comboMenu.Add(MenuSettings.Combo.Qmaxstacks).Permashow(true, "Q Max Stack", SharpDX.Color.Yellow);
            comboMenu.Add(MenuSettings.Combo.Qat);
            myMenu.Add(comboMenu);

            var harassMenu = new Menu("harassMenu", "Harass")
            {
                MenuSettings.Harass.harassSeparator,
                MenuSettings.secsec,
                MenuSettings.Harass.useQ,
                MenuSettings.Harass.useW,
                MenuSettings.Harass.useE,
            };
            //myMenu.Add(harassMenu);

            var laneClearMenu = new Menu("laneClearMenu", "Lane Clear")
            {
                MenuSettings.LaneClear.laneClearSeperator,
                MenuSettings.secsec,
                MenuSettings.LaneClear.useQ,
                MenuSettings.LaneClear.mana,
                MenuSettings.LaneClear.x,
            };
            myMenu.Add(laneClearMenu);

            var jungleClearMenu = new Menu("jungleClearMenu", "Jungle Clear")
            {
                MenuSettings.JungleClear.jungleClearSeparator,
                MenuSettings.secsec,
                MenuSettings.JungleClear.useQ,
                MenuSettings.JungleClear.useE,
            };
            myMenu.Add(jungleClearMenu);

            var lastHitMenu = new Menu("lastHitMenu", "Last Hit")
            {
                MenuSettings.LastHit.lastHitSeparator,
                MenuSettings.secsec,
                MenuSettings.LastHit.useQ,
            };
            myMenu.Add(lastHitMenu);

            var miscMenu = new Menu("miscMenu", "Misc")
            {
                MenuSettings.Misc.miscSeparator,
                MenuSettings.secsec,
            };
            //myMenu.Add(miscMenu);

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

            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += OnDraw;
            Drawing.OnEndScene += OnEndScene;
            Orbwalker.OnAction += OnAction;
            Interrupter.OnInterrupterSpell += OnInterrupterSpell;
            Gapcloser.OnGapcloser += OnGapcloser;
            AIBaseClient.OnProcessSpellCast += AIBaseClient_OnProcessSpellCast;
        }
        private static void AIBaseClient_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "IreliaEMissile")
            {
                ECatPos = args.End;
            }
            if (sender.IsMe && args.SData.Name == "IreliaR")
            {
                rtime = Variables.GameTimeTickCount;
            }
            if (sender.IsMe && (args.SData.Name == "IreliaE2" || args.SData.Name == "IreliaE"))
            {
                etime = Variables.GameTimeTickCount;
            }
        }
        private static void OnUpdate(EventArgs args)
        {
            Q.Speed = Qspeed();
            if (objPlayer.IsDead || objPlayer.IsRecalling())
                return;
            if (MenuGUI.IsChatOpen || MenuGUI.IsShopOpen)
                return;
            if (MenuSettings.Combo.Qat.Value >= objPlayer.HealthPercent)
            {
                MenuSettings.Combo.Qmaxstacks.Enabled = true;
            }
            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    Combo();
                    KillSteal();
                    foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(600) && GetQDmg(x) >= x.Health))
                    {
                        if (target.Health <= GetQDmg(target)) Q.Cast(target);
                    }
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

        private static void Combo()
        {
            var obj = new List<AIBaseClient>();
            obj.AddRange(ObjectManager.Get<AIMinionClient>().Where(i => i.IsValidTarget(Q.Range) && !i.IsAlly));
            obj.AddRange(ObjectManager.Get<AIHeroClient>().Where(i => i.IsValidTarget(Q.Range) && !i.IsAlly));
            obj.AddRange(ObjectManager.Get<AIBaseClient>().Where(i => i.IsValidTarget(Q.Range) && !i.IsAlly));
            var targets = TargetSelector.GetTargets(2000);
            if (targets != null)
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(2000)))
                {
                    /*Game.Print(GetQDmg(target) + " cal");
                    Game.Print(Q.GetDamage(target) + " Get dmg");
                    Game.Print(objPlayer.GetBonusPhysicalDamage());*/
                    if (MenuSettings.Combo.useQ.Enabled && Q.IsReady())
                    {
                        AIBaseClient gapobj = GetGapObj(target);
                        if (target.HasBuff("ireliamark") || target.Health < GetQDmg(target))
                        {
                            foreach (AIBaseClient aIBaseClient in obj.Where(i => i.HasBuff("ireliamark") || i.Health <= GetQDmg(i)))
                            {
                                if (ObjectManager.Player.HealthPercent < MenuSettings.Combo.Qmd.Value && aIBaseClient != null && aIBaseClient.NetworkId != target.NetworkId)
                                {
                                    if (MenuSettings.Combo.Qmaxstacks.Enabled)
                                    {
                                        if (Extensions.Distance(target.Position, aIBaseClient.Position) < 600 && aIBaseClient.Health < GetQDmg(aIBaseClient))
                                        {
                                            if (objPlayer.ManaPercent >= 15)
                                            {
                                                if (aIBaseClient.DistanceToPlayer() < Q.Range)
                                                    Q.Cast(aIBaseClient);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (!objPlayer.HasBuff("ireliapassivestacksmax"))
                                        {
                                            if (Extensions.Distance(target.Position, aIBaseClient.Position) < 600 && aIBaseClient.Health < GetQDmg(aIBaseClient))
                                            {
                                                if (objPlayer.ManaPercent >= 15)
                                                {
                                                    if (aIBaseClient.DistanceToPlayer() < Q.Range)
                                                        Q.Cast(aIBaseClient);
                                                }
                                            }
                                        }
                                    }

                                }
                                else
                                {
                                    if (target.DistanceToPlayer() < Q.Range)
                                        Q.Cast(target);
                                }
                            }
                        }
                        else
                        {
                            QgapObj(target);
                        }
                    }
                    if (MenuSettings.Combo.useW.Enabled)
                    {
                        if (W.IsCharging)
                        {
                            DelayAction.Add(300 - Game.Ping, () =>
                            {
                                if (!target.IsValidTarget(800))
                                {
                                    if (GetGapObj(target) != null)
                                    {
                                        W.ShootChargedSpell(GetGapObj(target).Position);
                                    }
                                }
                            });
                        }
                        if (GetGapObj(target) != null)
                        {
                            if (GetGapObj(target).NetworkId != target.NetworkId)
                            {
                                if (GetGapObj(target).Health <= Q.GetDamage(GetGapObj(target)) + W.GetDamage(GetGapObj(target)) && GetGapObj(target).Health >= GetQDmg(GetGapObj(target)) && Extensions.Distance(target.Position, GetGapObj(target).Position) < 500)
                                {
                                    if (objPlayer.ManaPercent > 20)
                                    {
                                        W.StartCharging();
                                        DelayAction.Add(300 - Game.Ping, () =>
                                        {
                                            W.ShootChargedSpell(GetGapObj(target).Position);
                                        });
                                    }
                                }
                            }
                            else
                            {
                                if (objPlayer.ManaPercent > 20)
                                {
                                    W.StartCharging();
                                    DelayAction.Add(300 - Game.Ping, () =>
                                    {
                                        W.ShootChargedSpell(GetGapObj(target).Position);
                                    });
                                }
                            }
                        }
                    }
                    if (MenuSettings.Combo.useE.Enabled && E.IsReady() && !target.HasBuff("ireliamark"))
                    {
                        /*if(MenuSettings.Combo.useQ.Enabled && Q.IsReady())
                        {
                            if(objPlayer.HasBuff("IreliaE"))
                            {
                                var PosCanE = ECatPos.Extend(target.Position, 300);
                                foreach (AIBaseClient aIBaseClient in obj.Where(i => i.HasBuff("ireliamark") || i.Health <= GetQDmg(i)))
                                {
                                    if(aIBaseClient.Position.Distance(PosCanE) < 700 && aIBaseClient.Position.Distance(target) < 500)
                                    {
                                        if(objPlayer.ManaPercent > 20 && aIBaseClient.Health < GetQDmg(aIBaseClient))
                                        {
                                            if (aIBaseClient.DistanceToPlayer() < Q.Range)
                                                Q.Cast(aIBaseClient);
                                        }
                                    }
                                }
                            }
                        }*/
                        //EcomboCastPostion(target);
                        if (Variables.GameTimeTickCount - (rtime + 700) > 0)
                        {
                            NewEPred();
                        }
                    }
                    if (MenuSettings.Combo.useR.Enabled && R.IsReady() && !target.HasBuff("ireliamark"))
                    {
                        var Rpred = R.GetPrediction(target, false, -1, CollisionObjects.YasuoWall);
                        if (Variables.GameTimeTickCount - (etime + 500) > 0)
                            CastRx();
                        if (Variables.GameTimeTickCount - (etime + 500) > 0)
                            if (objPlayer.Position.CountEnemyHeroesInRange(800) < 2 && target.Distance(objPlayer) < 600)
                            {
                                if (target.Health <= GetQDmg(target) * 3 + (W.IsReady() ? W.GetDamage(target) : 0) + (E.IsReady() ? E.GetDamage(target) : 0) + R.GetDamage(target) + 100)
                                {
                                    if (Rpred.Hitchance == HitChance.VeryHigh && Rpred.CastPosition.DistanceToPlayer() < R.Range - 200)
                                        if (Variables.GameTimeTickCount - (etime + 500) > 0 && Rpred.CastPosition.DistanceToPlayer() < R.Range)
                                            R.Cast(Rpred.CastPosition);
                                }
                            }

                        if (Variables.GameTimeTickCount - (etime + 500) > 0)
                            if (target.HealthPercent <= MenuSettings.Combo.y.Value)
                            {
                                if (Rpred.Hitchance == HitChance.VeryHigh && Rpred.CastPosition.DistanceToPlayer() < R.Range - 75)
                                    if (Q.IsReady())
                                    {
                                        if (target.Health > GetQDmg(target))
                                            if (Variables.GameTimeTickCount - (etime + 500) > 0 && Rpred.CastPosition.DistanceToPlayer() < R.Range)
                                                R.Cast(Rpred.CastPosition);
                                    }
                                    else
                                    {
                                        if (Variables.GameTimeTickCount - (etime + 500) > 0 && Rpred.CastPosition.DistanceToPlayer() < R.Range)
                                            R.Cast(Rpred.CastPosition);
                                    }
                            }

                        if (Variables.GameTimeTickCount - (etime + 500) > 0 && Rpred.CastPosition.DistanceToPlayer() < R.Range)
                            R.CastIfWillHit(target, MenuSettings.Combo.x);
                    }
                }
            }
        }

        private static void QgapObj(AIBaseClient target)
        {
            foreach (AIBaseClient Objects in GameObjects.EnemyMinions.Where(i => (i.Health < GetQDmg(i) || i.HasBuff("ireliamark"))
             && i.IsValidTarget(600)
             && (i.Distance(target) <= objPlayer.Distance(target) + 150 || i.Distance(target) <= objPlayer.GetRealAutoAttackRange())
            ))
            {
                if (Objects != null && Objects.Health < GetQDmg(Objects))
                {
                    if (Objects.DistanceToPlayer() < Q.Range && (Objects.Distance(target) < objPlayer.Distance(target) || Objects.Distance(target) < objPlayer.GetRealAutoAttackRange()))
                        Q.Cast(Objects);
                    if (Objects.Position.Distance(target) < 600 && MenuSettings.Combo.useE.Enabled && !objPlayer.HasBuff("IreliaE"))
                    {
                        E.Cast(objPlayer.Position - 800);
                    }
                }
            }
            foreach (AIBaseClient Objects in GameObjects.EnemyHeroes.Where(i => (i.Health < GetQDmg(i) || i.HasBuff("ireliamark"))
             && i.IsValidTarget(600)
             && (i.Distance(target) <= objPlayer.Distance(target) + 150 || i.Distance(target) <= objPlayer.GetRealAutoAttackRange())
            ))
            {
                if (Objects != null && Objects.Health < GetQDmg(Objects))
                {
                    if (Objects.DistanceToPlayer() < Q.Range && (Objects.Distance(target) < objPlayer.Distance(target) || Objects.Distance(target) < objPlayer.GetRealAutoAttackRange()))
                        Q.Cast(Objects);
                    if (Objects.Position.Distance(target) < 600 && MenuSettings.Combo.useE.Enabled && !objPlayer.HasBuff("IreliaE"))
                    {
                        E.Cast(objPlayer.Position - 800);
                    }
                }
            }
            foreach (AIBaseClient Objects in GameObjects.Jungle.Where(i => (i.Health < GetQDmg(i) || i.HasBuff("ireliamark"))
             && i.IsValidTarget(600)
             && (i.Distance(target) <= objPlayer.Distance(target) + 150 || i.Distance(target) <= objPlayer.GetRealAutoAttackRange())
            ))
            {
                if (Objects != null && Objects.Health < GetQDmg(Objects))
                {
                    if (Objects.DistanceToPlayer() < Q.Range && (Objects.Distance(target) < objPlayer.Distance(target) || Objects.Distance(target) < objPlayer.GetRealAutoAttackRange()))
                        Q.Cast(Objects);
                    if (Objects.Position.Distance(target) < 600 && MenuSettings.Combo.useE.Enabled && !objPlayer.HasBuff("IreliaE"))
                    {
                        E.Cast(objPlayer.Position - 800);
                    }
                }
            }
        }
        public static void CastRx() //Made by Brian(Valve Sharp)
        {
            try
            {
                var targets = GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(R.Range)).ToArray();
                var castPos = Vector3.Zero;

                if (!targets.Any())
                {
                    return;
                }

                foreach (var pred in
                    targets.Select(i => R.GetPrediction(i, false, -1, CollisionObjects.YasuoWall))
                        .Where(
                            i => i.Hitchance >= HitChance.Medium && i.AoeTargetsHitCount >= MenuSettings.Combo.x.Value)
                        .OrderByDescending(i => i.AoeTargetsHitCount))
                {
                    castPos = pred.CastPosition;
                    break;
                }

                if (castPos != Vector3.Zero && castPos.DistanceToPlayer() < R.Range)
                {
                    R.Cast(castPos);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("R.cast Error" + ex);
            }
        }

        private static void NewEPred()
        {
            var targets = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(2000) && !i.IsDead);
            var target = TargetSelector.GetTarget(2000);
            {
                if (target == null || target.HasBuff("ireliamark")) return;

                if (target != null && !target.HasBuff("ireliamark"))
                {
                    float ereal = 0.25f + 0.025f + Game.Ping / 1000;

                    if (E.IsReady(0))
                    {
                        if (!objPlayer.HasBuff("IreliaE"))
                        {
                            if (E1.GetPrediction(target).CastPosition.DistanceToPlayer() < 975)
                            {
                                Geometry.Circle circle = new Geometry.Circle(objPlayer.Position, 600, 50);

                                {
                                    foreach (var onecircle in circle.Points)
                                    {
                                        if (onecircle.Distance(target) > 450)
                                        {
                                            E.Cast(onecircle);
                                        }
                                    }
                                }
                            }
                        }
                        if (objPlayer.HasBuff("IreliaE"))
                        {                           
                            var castepos = Vector2.Zero;
                            Geometry.Circle onecircle = new Geometry.Circle(objPlayer.Position, 725);
                            foreach (var circle in onecircle.Points)
                            {
                                E.Delay = (725 - E2.GetPrediction(target).CastPosition.Distance(objPlayer)) / E.Speed + 0.25f + ereal;
                            }

                            var epred = E.GetPrediction(target);

                            for (int i = 10000; i > 55; i -= 50)
                            {
                                if (epred.CastPosition.Extend(ECatPos, -i).ToVector2().DistanceToPlayer() <= 775)
                                    castepos = epred.CastPosition.Extend(ECatPos, -i).ToVector2();
                            }
                            if (castepos != Vector2.Zero)
                            {
                                E.Cast(castepos);
                            }
                        }
                    }
                }
            }
        }
        private static double GetQDmg(AIBaseClient target)
        {
            double dmgQ = Q.GetDamage(target);
            double dmgSheen = 0;
            double dmgMinions = 60;
            float passive = 0;

            bool sheen = false;
            bool trinity = false;

            if (ObjectManager.Player.CanUseItem((int)ItemId.Trinity_Force))
            {
                trinity = false;
                DelayAction.Add(10, () => { trinity = true; });
            }
            else { trinity = false; }
            if (ObjectManager.Player.CanUseItem((int)ItemId.Sheen))
            {
                sheen = false;
                DelayAction.Add(10, () => { sheen = true; });
            }
            else { sheen = false; }

            if (ObjectManager.Player.Level > 0 && ObjectManager.Player.HasBuff("ireliapassivestacksmax"))
            {
                passive = (objPlayer.Level - 1) * (ObjectManager.Player.Level == 1 ? 0 : 3) + 15;
            }
            if (ObjectManager.Player.HasBuff("Sheen") || sheen)
            {
                dmgSheen = ObjectManager.Player.TotalAttackDamage - 5;
            }
            if (trinity)
            {
                dmgSheen = (ObjectManager.Player.TotalAttackDamage - 30) * 2;
            }
            switch (Q.Level)
            {
                case 1:
                    dmgQ = 5f + ObjectManager.Player.TotalAttackDamage * 60 / 100;
                    dmgMinions = 60;
                    break;
                case 2:
                    dmgQ = 25f + ObjectManager.Player.TotalAttackDamage * 60 / 100;
                    dmgMinions = 100;
                    break;
                case 3:
                    dmgQ = 45f + ObjectManager.Player.TotalAttackDamage * 60 / 100;
                    dmgMinions = 140;
                    break;
                case 4:
                    dmgQ = 65f + ObjectManager.Player.TotalAttackDamage * 60 / 100;
                    dmgMinions = 180;
                    break;
                case 5:
                    dmgQ = 85f + ObjectManager.Player.TotalAttackDamage * 60 / 100;
                    dmgMinions = 220;
                    break;
            }
            double Alldmg = dmgQ + dmgSheen;
            if (target.IsMinion)
            {
                Alldmg = dmgMinions + (ObjectManager.Player.TotalAttackDamage * 60 / 100) + dmgSheen;
            }
            return ObjectManager.Player.CalculatePhysicalDamage(target, Alldmg) + ObjectManager.Player.CalculateMagicDamage(target, passive);
        }

        public static AIBaseClient GetGapObj(AIHeroClient aIHeroClient)
        {
            var obj = new List<AIBaseClient>();
            obj.AddRange(ObjectManager.Get<AIMinionClient>().Where(i => i.IsValidTarget(Q.Range) && !i.IsAlly));
            obj.AddRange(ObjectManager.Get<AIHeroClient>().Where(i => i.IsValidTarget(Q.Range) && !i.IsAlly));
            obj.AddRange(ObjectManager.Get<AIBaseClient>().Where(i => i.IsValidTarget(Q.Range) && !i.IsAlly));
            return obj.Where(
                    i => (i.Position.Distance(aIHeroClient.Position) <= objPlayer.Distance(aIHeroClient.Position) + 100
                    || i.Position.Distance(aIHeroClient.Position) <= objPlayer.GetRealAutoAttackRange()))
                .MinOrDefault(i => i.Distance(objPlayer.Position));
        }

        public static Vector3 ECatPos;
        public static float rtime;
        public static float etime;
        public static float Qspeed()
        {
            return 1500 + objPlayer.MoveSpeed;
        }
        private static void KillSteal()
        {
            var target = TargetSelector.GetTarget(Q.Range);
            if (target != null && target.Health <= GetQDmg(target))
            {
                Q.Cast(target);
                return;
            }

        }
        private static void Harass()
        {

        }
        public static List<AIMinionClient> GetEnemyLaneMinionsTargetsInRange(float range)
        {
            return GameObjects.EnemyMinions.Where(x => x.IsValidTarget(range) && x.IsMinion()).Cast<AIMinionClient>().ToList();
        }
        private static void LaneClear()
        {
            foreach (var min in GetEnemyLaneMinionsTargetsInRange(Q.Range))
            {
                if (min != null)
                {
                    /*Game.Print(GetQDmg(min) + " cal");
                    Game.Print(Q.GetDamage(min) + " Get dmg");
                    Game.Print(objPlayer.GetBonusPhysicalDamage());*/
                }
                if (MenuSettings.LaneClear.useQ.Enabled)
                    if (min != null && min.Health <= GetQDmg(min) && Q.IsReady())
                    {
                        if (MenuSettings.LaneClear.mana.Enabled)
                        {
                            if (objPlayer.ManaPercent >= MenuSettings.LaneClear.x.Value)
                            {
                                Q.Cast(min);
                            }
                            else
                                return;
                        }
                        else Q.Cast(min);
                    }
            }
        }
        public static List<AIMinionClient> GetJungleTargetsInRange(float range)
        {
            return GameObjects.Jungle.Where(x => x.IsValidTarget(range) && x.IsMonster).Cast<AIMinionClient>().ToList();
        }
        private static void JungleClear()
        {
            foreach (var mod in GetJungleTargetsInRange(R.Range))
            {
                if (mod != null)
                {
                    if (MenuSettings.JungleClear.useQ.Enabled && Q.IsReady() && mod.DistanceToPlayer() <= Q.Range)
                    {
                        if (mod.HasBuff("ireliamark") || mod.Health <= GetQDmg(mod))
                        {
                            Q.Cast(mod);
                        }
                        if (mod.Health <= GetQDmg(mod))
                        {
                            Q.Cast(mod);
                        }
                    }
                    if (MenuSettings.JungleClear.useE.Enabled && E.IsReady() && !mod.HasBuff("ireliamark"))
                    {
                        var Epos = objPlayer.Position.Extend(mod.Position, -775);
                        var Epred = E.GetPrediction(mod, false);
                        var Ecast = Epred.CastPosition.Extend(Epos, -50);
                        if (Ecast.DistanceToPlayer() <= 775)
                        {
                            if (!objPlayer.HasBuff("IreliaE"))
                            {
                                E.Cast(Epos);
                            }
                            else
                            {
                                E.Cast(Ecast);
                                return;
                            }
                        }
                    }
                }
            }
        }
        private static void LastHit()
        {
            foreach (var min in GetEnemyLaneMinionsTargetsInRange(Q.Range))
            {
                if (min != null && min.Health <= GetQDmg(min) && Q.IsReady())
                {
                    if (MenuSettings.LastHit.useQ.Enabled)
                    {
                        Q.Cast(min);
                    }
                }
            }
        }
        private static void OnDraw(EventArgs args)
        {
            if (MenuSettings.Drawing.disableDrawings.Enabled)
                return;

            if (MenuSettings.Drawing.drawQ.Enabled && Q.IsReady())
            {
                Render.Circle.DrawCircle(objPlayer.Position, Q.Range, System.Drawing.Color.Red);
            }
            if (MenuSettings.Drawing.drawW.Enabled && W.IsReady())
            {
                Render.Circle.DrawCircle(objPlayer.Position, W.Range, System.Drawing.Color.Beige);
            }
            if (MenuSettings.Drawing.drawE.Enabled && E.IsReady())
            {
                Render.Circle.DrawCircle(objPlayer.Position, W.Range, System.Drawing.Color.DodgerBlue);
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

            if (objPlayer.GetBuffCount("itemmagicshankcharge") == 100) // oktw sebby
                damage += (float)objPlayer.CalculateMagicDamage(target, 100 + 0.1 * objPlayer.TotalMagicalDamage);

            if (target.HasBuff("ManaBarrier") && target.HasBuff("BlitzcrankManaBarrierCO"))
                damage += target.Mana / 2f;
            if (target.HasBuff("GarenW"))
                damage = damage * 0.7f;

            return damage;
        }
        private static void OnAction(object sender, OrbwalkerActionArgs args)
        {

        }
        private static void OnInterrupterSpell(AIHeroClient sender, Interrupter.InterruptSpellArgs arg)
        {

        }
        private static void OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserArgs args)
        {

        }
    }
}
