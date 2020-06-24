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
        private static SpellSlot summonerIgnite;
        private static Spell Q, W, E, R;
        private static AIHeroClient objPlayer = ObjectManager.Player;
        private static Menu myMenu;

        public static void OnLoad()
        {
            Game.Print("<font color='#b756c5' size='25'> It just a Beta version, Dont forget to feedback </font>");
            Q = new Spell(SpellSlot.Q, 600f);
            Q.SetTargetted(0f, Qspeed());

            W = new Spell(SpellSlot.W, 825f);
            W.SetCharged("IreliaW", "ireliawdefense", 800, 800, 0);

            E = new Spell(SpellSlot.E, 775f);
            E.SetSkillshot(0.25f, 5f, 2000f, false, false, SkillshotType.Line, HitChance.High, default(Vector3), default(Vector3));

            R = new Spell(SpellSlot.R, 1000f);
            R.SetSkillshot(0.25f, 300, 1500, true, SkillshotType.Line);

            #region Menu Init

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
            comboMenu.Add(MenuSettings.Combo.Qmaxstacks).Permashow();
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
            };
            //myMenu.Add(jungleClearMenu);

            var lastHitMenu = new Menu("lastHitMenu", "Last Hit")
            {
                MenuSettings.LastHit.lastHitSeparator,
                MenuSettings.secsec,
            };
            //myMenu.Add(lastHitMenu);

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

            #endregion

            Game.OnUpdate                    += OnUpdate;
            Drawing.OnDraw                  += OnDraw;
            Drawing.OnEndScene              += OnEndScene;
            Orbwalker.OnAction              += OnAction;
            Interrupter.OnInterrupterSpell  += OnInterrupterSpell;
            Gapcloser.OnGapcloser           += OnGapcloser;
            AIBaseClient.OnProcessSpellCast += AIBaseClient_OnProcessSpellCast;
        }
        public static Vector3 ECatPos;
        private static void AIBaseClient_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "IreliaEMissile")
            {
                ECatPos = args.End;
            }
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
        public static List<AIMinionClient> GetEnemyLaneMinionsTargetsInRange(float range)
        {
            return GameObjects.EnemyMinions.Where(x => x.IsValidTarget(range) && x.IsMinion() && x.Health < Q.GetDamage(x)).Cast<AIMinionClient>().ToList();
        }
        public static List<AIMinionClient> GetJungleTargetsInRange(float range)
        {
            return GameObjects.Jungle.Where(x => x.IsValidTarget(range) && x.IsMonster).Cast<AIMinionClient>().ToList();
        }
        public static List<AIHeroClient> GetTargets(float range)
        {
            return GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(range) && x.IsEnemy).Cast<AIHeroClient>().ToList();
        }
        private static void OnUpdate(EventArgs args)
        {
            if (objPlayer.IsDead || objPlayer.IsRecalling())
                return;
            if (MenuGUI.IsChatOpen || MenuGUI.IsShopOpen)
                return;            
            if(MenuSettings.Combo.Qat.Value >= objPlayer.HealthPercent)
            {
                MenuSettings.Combo.Qmaxstacks.Enabled = true;
            }
            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    Combo();
                    KillSteal();
                    foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(600)))
                    {
                        if (target.Health <= GetQDmg(target) && Q.CastOnUnit(target))
                            return;
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

        #region Orbwalker Modes
        public static float Qspeed()
        {
            return 1500 + objPlayer.MoveSpeed;
        }
        private static void Combo()
        {
            var obj = new List<AIBaseClient>();
            obj.AddRange(ObjectManager.Get<AIMinionClient>().Where(i => i.IsValidTarget(Q.Range) && !i.IsAlly));
            obj.AddRange(ObjectManager.Get<AIHeroClient>().Where(i => i.IsValidTarget(Q.Range) && !i.IsAlly));
            obj.AddRange(ObjectManager.Get<AIBaseClient>().Where(i => i.IsValidTarget(Q.Range) && !i.IsAlly));
            var targets = TargetSelector.GetTargets(2000);
            if(targets != null)
            {
                foreach (var target in GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(2000)))
                {
                    if(MenuSettings.Combo.useQ.Enabled && Q.IsReady())
                    {
                        AIBaseClient gapobj = GetGapObj(target);
                        if(target.HasBuff("ireliamark") || target.Health < GetQDmg(target))
                        {
                            foreach(AIBaseClient aIBaseClient in obj.Where(i => i.HasBuff("ireliamark") || i.Health <= GetQDmg(i)))
                            {
                                if(MenuSettings.Combo.Qmd.Value >= objPlayer.HealthPercent && aIBaseClient.NetworkId != target.NetworkId)
                                {
                                    if (objPlayer.HasBuff("ireliapassivestacksmax"))
                                    {
                                        if(MenuSettings.Combo.Qmaxstacks.Enabled)
                                        {
                                            if(Extensions.Distance(target.Position, aIBaseClient.Position) < 600 && aIBaseClient.Health < GetQDmg(aIBaseClient))
                                            {
                                                if(objPlayer.ManaPercent >= 15)
                                                {
                                                    Q.Cast(aIBaseClient);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            Q.Cast(target);
                                        }
                                    }
                                    else
                                    {
                                        if (Extensions.Distance(target.Position, aIBaseClient.Position) < 600)
                                        {
                                            if (objPlayer.ManaPercent >= 15 && aIBaseClient.Health < GetQDmg(aIBaseClient))
                                            {
                                                Q.Cast(aIBaseClient);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    Q.Cast(target);
                                }
                            }
                        }
                        else
                        {
                            QgapObj(target);
                            if(gapobj != null && (gapobj.HasBuff("ireliamark") || gapobj.Health <= GetQDmg(gapobj)) && Q.CanCast(gapobj))
                            {
                                if(target.DistanceToPlayer() <= objPlayer.GetRealAutoAttackRange() + 200 && objPlayer.ManaPercent > 20)
                                {
                                    foreach (AIBaseClient aIBaseClient in obj.Where(i => i.HasBuff("ireliamark") || i.Health <= GetQDmg(i)))
                                    {
                                        if(Extensions.Distance(gapobj.Position, aIBaseClient.Position) < 600 && aIBaseClient.Health < GetQDmg(aIBaseClient))
                                        {
                                            Q.Cast(aIBaseClient);
                                        }
                                    }
                                }
                                else
                                {
                                    if(gapobj.Health < GetQDmg(gapobj))
                                    Q.Cast(gapobj);
                                }
                            }
                        }
                    }
                    if (MenuSettings.Combo.useW.Enabled)
                    {
                        if(W.IsCharging)
                        {
                            DelayAction.Add(300 - Game.Ping, () =>
                            {
                                if(!target.IsValidTarget(800))
                                {
                                    if (GetGapObj(target) != null)
                                    {
                                        W.ShootChargedSpell(GetGapObj(target).Position);
                                    }
                                }
                            });
                        }
                        if(GetGapObj(target) != null)
                        {
                            if(GetGapObj(target).NetworkId != target.NetworkId)
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
                        if(MenuSettings.Combo.useQ.Enabled && Q.IsReady())
                        {
                            if(objPlayer.HasBuff("IreliaE"))
                            {
                                var PosCanE = ECatPos.Extend(target.Position, 300);
                                foreach (AIBaseClient aIBaseClient in obj.Where(i => i.HasBuff("ireliamark") || i.Health <= Q.GetDamage(i)))
                                {
                                    if(aIBaseClient.Position.Distance(PosCanE) < 700 && aIBaseClient.Position.Distance(target) < 500)
                                    {
                                        if(objPlayer.ManaPercent > 20 && aIBaseClient.Health < GetQDmg(aIBaseClient))
                                        {
                                            Q.Cast(aIBaseClient);
                                        }
                                    }
                                }
                            }
                        }
                        NewEPred();
                    }
                    if (MenuSettings.Combo.useR.Enabled && R.IsReady() && !target.HasBuff("ireliamark"))
                    {
                        var Rpred = R.GetPrediction(target, false, -1, CollisionObjects.YasuoWall);
                        CastRx();
                        if(objPlayer.Position.CountEnemyHeroesInRange(800) < 2 && target.Distance(objPlayer) < 600)
                        {
                            if(target.Health <= GetQDmg(target) * 3 + (W.IsReady() ? W.GetDamage(target) : 0) + (E.IsReady() ? E.GetDamage(target) : 0) + R.GetDamage(target) + 100)
                            {
                                if (Rpred.Hitchance == HitChance.VeryHigh && Rpred.CastPosition.DistanceToPlayer() < R.Range - 200)
                                    R.Cast(Rpred.CastPosition);
                            }
                        }
                        if (target.HealthPercent <= MenuSettings.Combo.y.Value)
                        {
                            if (Rpred.Hitchance == HitChance.VeryHigh && Rpred.CastPosition.DistanceToPlayer() < R.Range - 75)
                                if (Q.IsReady())
                                {
                                    if (target.Health > GetQDmg(target))
                                        R.Cast(Rpred.CastPosition);
                                }
                                else
                                {
                                    R.Cast(Rpred.CastPosition);
                                }
                        }
                        R.CastIfWillHit(target, MenuSettings.Combo.x);
                    }
                }
            }                      
        }
        private static void QgapObj(AIBaseClient target)
        {
            foreach(AIBaseClient Objects in GameObjects.EnemyMinions.Where(i => (i.Health < GetQDmg(i) || i.HasBuff("ireliamark"))
            && i.IsValidTarget(600)
            && (i.Distance(target) <= objPlayer.Distance(target) + 150 || i.Distance(target) <= objPlayer.GetRealAutoAttackRange())
            ))
            {
                if(Objects != null && Objects.Health < GetQDmg(Objects))
                {
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
                    Q.Cast(Objects);
                    if (Objects.Position.Distance(target) < 600 && MenuSettings.Combo.useE.Enabled && !objPlayer.HasBuff("IreliaE"))
                    {
                        E.Cast(objPlayer.Position - 800);
                    }
                }
            }
        }
        private static double GetQDmg(AIBaseClient target)
        {
            double dmgQ = Q.GetDamage(target);
            double dmgSheen = 0;
            double dmgMinions = 55;
            if(objPlayer.HasBuff("Sheen") || (objPlayer.HasItem((int)ItemId.Sheen) && objPlayer.CanUseItem((int)ItemId.Sheen)))
            {
                dmgSheen = objPlayer.GetAutoAttackDamage(target);
            }
            if (Q.Level == 1)
            {
                dmgQ = 5f + objPlayer.TotalAttackDamage;
                dmgMinions = 55;
            }
            if (Q.Level == 2)
            {
                dmgQ = 25f + objPlayer.TotalAttackDamage;
                dmgMinions = 75;
            }
            if (Q.Level == 3)
            {
                dmgQ = 45f + objPlayer.TotalAttackDamage;
                dmgMinions = 95;
            }
            if (Q.Level == 4)
            {
                dmgQ = 65f + objPlayer.TotalAttackDamage;
                dmgMinions = 115;
            }
            if (Q.Level == 5)
            {
                dmgQ = 85f + objPlayer.TotalAttackDamage;
                dmgMinions = 135;
            }
            double Alldmg = dmgQ + dmgSheen;
            if(target.IsMinion)
            {
                Alldmg = dmgQ + dmgSheen + dmgMinions + objPlayer.GetAutoAttackDamage(target) * 25 /100;
            }
            return objPlayer.CalculateDamage(target, DamageType.Physical, Alldmg);
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
                            i => i.Hitchance >= HitChance.High && i.AoeTargetsHitCount >= MenuSettings.Combo.x.Value)
                        .OrderByDescending(i => i.AoeTargetsHitCount))
                {
                    castPos = pred.CastPosition;
                    break;
                }

                if (castPos != Vector3.Zero)
                {
                    R.Cast(castPos);
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine("R.cast Error" + ex);
            }
        }
        private static void EcomboCastPostion(AIHeroClient aiheroClient_0)
        {
            Vector3 vector = Vector3.Zero;
            Vector3 vector2 = Vector3.Zero;
            if (!Extensions.IsValidTarget(aiheroClient_0, E.Range, true))
            {
                aiheroClient_0 = TargetSelector.GetTarget(E.Range, false);
            }
            if (aiheroClient_0 != null && !aiheroClient_0.HasBuffOfType(BuffType.SpellImmunity) && !aiheroClient_0.HasBuffOfType(BuffType.SpellShield))
            {
                SpellPrediction.PredictionOutput prediction = E.GetPrediction(aiheroClient_0);
                if (prediction.Hitchance >= HitChance.High)
                {
                    if(objPlayer.HasBuff("IreliaE"))
                    {
                        Geometry.Circle circle = new Geometry.Circle(ObjectManager.Player.Position, 730);
                        foreach (Vector2 vector3 in circle.Points.Where(i => i.DistanceToPlayer() < 775 
                        //&& (
                        //((aiheroClient_0.DistanceToPlayer() > 264 && aiheroClient_0.DistanceToPlayer() < 424) ? i.DistanceToPlayer() / 2000 <= prediction.CastPosition.DistanceToPlayer() / 1600 + 0.2 : i.DistanceToPlayer() / 2000 <= prediction.CastPosition.DistanceToPlayer() / 1600 + 0.4)
                        //&& ((aiheroClient_0.DistanceToPlayer() < 504 && aiheroClient_0.DistanceToPlayer() > 424) ? i.DistanceToPlayer() / 2000 <= prediction.CastPosition.DistanceToPlayer() / 1600 + 0.1 : i.DistanceToPlayer() / 2000 <= prediction.CastPosition.DistanceToPlayer() / 1600 + 0.4)
                        //&& ((aiheroClient_0.DistanceToPlayer() > 504) ? i.DistanceToPlayer() / 2000 <= prediction.CastPosition.DistanceToPlayer() / 1600 + 0.05 : i.DistanceToPlayer() / 2000 <= prediction.CastPosition.DistanceToPlayer() / 1600 + 0.4)
                        //)
                        ))
                        {
                            var projectionInfo = prediction.CastPosition.ProjectOn(vector3.ToVector3(), ECatPos);
                            if (projectionInfo.IsOnSegment && projectionInfo.LinePoint.Distance(aiheroClient_0.Position) <= 120 && projectionInfo.LinePoint.Distance(prediction.CastPosition) <= 120)
                            {
                                vector = vector3.ToVector3();
                                vector2 = ECatPos;
                            }
                        }
                        if (vector.DistanceToPlayer() <= E.Range)
                        {
                            if (vector != Vector3.Zero && vector2 != Vector3.Zero && Extensions.Distance(vector, vector2) > 300)
                            {
                                if (objPlayer.HasBuff("IreliaE"))
                                {
                                    E.Cast(vector, false);
                                }
                                else
                                {
                                    return;
                                }
                            }else
                            {
                                return;
                            }
                        }
                    }
                    else
                    {
                        Geometry.Circle circle = new Geometry.Circle(ObjectManager.Player.Position, 730);
                        foreach (Vector2 vector3 in circle.Points)
                        {
                            foreach (Vector2 vector4 in circle.Points)
                            {
                                var projectionInfo = prediction.CastPosition.ProjectOn(vector3.ToVector3(), vector4.ToVector3());
                                if (projectionInfo.IsOnSegment && projectionInfo.LinePoint.Distance(aiheroClient_0.Position) <= 120 && projectionInfo.LinePoint.Distance(prediction.CastPosition) <= 120)
                                {
                                    vector = Extensions.ToVector3(vector3, 0f);
                                    vector2 = Extensions.ToVector3(vector4, 0f);
                                }
                            }
                        }
                        if (Extensions.DistanceToPlayer(vector) <= E.Range && Extensions.DistanceToPlayer(vector2) <= E.Range)
                        {
                            if (vector.Distance(vector2) > 300 && vector != Vector3.Zero && vector2 != Vector3.Zero)
                            {
                                if (!objPlayer.HasBuff("IreliaE") && vector2.Distance(aiheroClient_0) > 200)
                                {
                                    E.Cast(vector2, false);
                                }
                                else
                                {
                                    return;
                                }
                            }
                        }
                    }                    
                }
            }
        }  
        
        private static void NewEPred()
        {
            var targets = GameObjects.EnemyHeroes.Where(heroes => heroes.IsValidTarget(700) && !heroes.IsDead);
            var getetarget = GameObjects.EnemyHeroes.Where(heroes => heroes.IsValidTarget(700) && !heroes.IsDead && heroes.HasBuff("ireliamark"));
            var count = targets.Count();
            var buffcount = getetarget.Count();

            if (count == 0 || !E.IsReady(0) || count == buffcount) return;

            foreach(var target in targets.Where(hero => !hero.HasBuff("ireliamark")))
            {
                if (target != null) return;

                if (objPlayer.HasBuff("IreliaE"))
                {
                    E.UpdateSourcePosition(ECatPos, objPlayer.Position);
                }
                else
                {
                    E.UpdateSourcePosition(objPlayer.Position, objPlayer.Position);
                }

                var checkepred = E.GetPrediction(target, false, -1, CollisionObjects.YasuoWall);

                if (count >= 2)
                {
                    if (count - buffcount > 1)
                    {
                        if (objPlayer.HasBuff("IreliaE"))
                        {
                            int j = 600;
                            for (int i = 1; i < 11; i++, j -= 50)
                            {
                                if (checkepred.Hitchance >= HitChance.High && checkepred.CastPosition.Extend(ECatPos, -j).DistanceToPlayer() < 775 && checkepred.CastPosition.Extend(ECatPos, -j).Distance(target) > 100)
                                    E.Cast(checkepred.CastPosition.Extend(ECatPos, -400));
                            }                           
                        }
                        else
                        {
                            var gete1pos = new Geometry.Circle(objPlayer.Position, 730, 100);
                            foreach(var e1pos in gete1pos.Points.Where(e => !e.IsWall() && !e.IsZero))
                            {
                                E.UpdateSourcePosition(e1pos.ToVector3(), objPlayer.Position);
                                if (E.GetPrediction(target).CastPosition.IsZero) return;
                                else { E.Cast(e1pos); DelayAction.Add(1, () => { return; }); }
                            }
                        }
                    }
                }
                else
                {
                    if (objPlayer.HasBuff("IreliaE"))
                    {
                        int j = 600;
                        for (int i = 1; i < 11; i++, j -= 50)
                        {
                            if (checkepred.Hitchance >= HitChance.High && checkepred.CastPosition.Extend(ECatPos, -j).DistanceToPlayer() < 775 && checkepred.CastPosition.Extend(ECatPos, -j).Distance(target) > 100)
                                E.Cast(checkepred.CastPosition.Extend(ECatPos, -400));
                        }
                    }
                    else
                    {
                        var gete1pos = new Geometry.Circle(objPlayer.Position, 730, 100);
                        foreach (var e1pos in gete1pos.Points.Where(e => !e.IsWall() && !e.IsZero))
                        {
                            E.UpdateSourcePosition(e1pos.ToVector3(), objPlayer.Position);
                            if (E.GetPrediction(target).CastPosition.IsZero) return;
                            else { E.Cast(e1pos); DelayAction.Add(1, () => { return; }); }
                        }
                    }
                }
            }           
        }
        private static void Harass()
        {
            
        }
        private static void LaneClear()
        {
            foreach(var min in GetEnemyLaneMinionsTargetsInRange(Q.Range))
            {
                if(min != null && min.Health <= GetQDmg(min) && Q.IsReady())
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
        private static void JungleClear()
        {
            foreach(var mod in GetJungleTargetsInRange(R.Range))
            {
                if(mod != null)
                {
                    if(MenuSettings.JungleClear.useQ.Enabled && Q.IsReady() && mod.DistanceToPlayer() <= Q.Range)
                    {
                        if(mod.HasBuff("ireliamark"))
                        {
                            Q.CastOnUnit(mod);
                        }
                        if(mod.Health <= Q.GetDamage(mod))
                        {
                            Q.CastOnUnit(mod);
                        }
                    }
                    if(MenuSettings.JungleClear.useE.Enabled && E.IsReady() && !mod.HasBuff("ireliamark"))
                    {
                        var Epos = objPlayer.Position.Extend(mod.Position, -775);
                        var Epred = E.GetPrediction(mod, false);
                        var Ecast = Epred.CastPosition.Extend(Epos, -50);
                        if(Ecast.DistanceToPlayer() <= 775)
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
                if (min != null && min.Health <= Q.GetDamage(min) && Q.IsReady())
                {
                    if(MenuSettings.LastHit.useQ.Enabled)
                    {
                        Q.CastOnUnit(min);
                    }
                }
            }
        }

        #endregion

        #region Events

        private static void OnAction(object sender, OrbwalkerActionArgs args)
        {

        }
        private static void OnInterrupterSpell(AIHeroClient sender, Interrupter.InterruptSpellArgs arg)
        {

        }
        private static void OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserArgs args)
        {

        }

        #endregion

        #region Drawings

        private static void OnDraw(EventArgs args)
        {
            if (MenuSettings.Drawing.disableDrawings.Enabled)
                return;

            if (MenuSettings.Drawing.drawQ.Enabled && Q.IsReady())
            {
                Render.Circle.DrawCircle(objPlayer.Position, Q.Range, System.Drawing.Color.AliceBlue);
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

        #endregion

        #region Misc
        public static bool E2()
        {
            return objPlayer.HasBuff("IreliaE");
        }
        public static bool E1()
        {
            return !objPlayer.HasBuff("IreliaE");
        }

        private static void KillSteal()
        {
            var target = TargetSelector.GetTarget(Q.Range);
            if(target != null && target.Health <= Q.GetDamage(target))
            {
                Q.CastOnUnit(target);
                return;
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
    }
}
