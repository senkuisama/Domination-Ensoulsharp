using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.Prediction;
using FunnySlayerCommon;
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
        public class QSettings
        {
            public static MenuBool Qcombo = new MenuBool("Qcombo", "Q in Combo [Gap_closer | KillSteal]");
            public static MenuBool QStacks = new MenuBool("QStacks", "Use Q Stack Passive Logic");
            public static MenuBool QDancing = new MenuBool("QDancing", "----> Q Dancing logic");
        }
        public class WSettings
        {
            public static MenuBool Wcombo = new MenuBool("Wcombo", "W in Combo", false);
            public static MenuSliderButton Wdelay = new MenuSliderButton("WDelay", "Recast W", 0, 0, 1000);
        }

        public class ESettings
        {
            public static MenuBool Ecombo = new MenuBool("Ecombo", "E in Combo");
            public static MenuBool ImproveE = new MenuBool("ImproveE", "Improve E prediction", false);
            public static MenuSeparator Efeedback = new MenuSeparator("Efeedback", "E logic if not good Feedback it to FunnySlayer#0348");
        }

        public class RSettings
        {
            public static MenuBool Rcombo = new MenuBool("Rcombo", "R in Combo [Calculator Dmg]");
            public static MenuSlider Rheath = new MenuSlider("Rheath", "Target Heath", 60);
            public static MenuSlider Rhit = new MenuSlider("Rhit", "Hit Count", 2, 2, 5);
        }

        public class ClearSettings
        {
            public static MenuBool DrawMinions = new MenuBool("DrawMinions", "Draw On Minions");
            public static MenuBool QireClear = new MenuBool("QireClear", "Q Clear");
            public static MenuSlider QireMana = new MenuSlider("QireMana", "Min Mana to use : ", 40);
        }
        public class KeysSettings
        {
            public static MenuKeyBind TurretKey = new MenuKeyBind("TurretKey", "Turret Key", System.Windows.Forms.Keys.A, KeyBindType.Toggle);
            public static MenuKeyBind SemiE = new MenuKeyBind("SemiE", "E Using Key", System.Windows.Forms.Keys.G, KeyBindType.Press);
            public static MenuKeyBind SemiR = new MenuKeyBind("SemiR", "R Using Key", System.Windows.Forms.Keys.T, KeyBindType.Press);
            public static MenuKeyBind FleeKey = new MenuKeyBind("FleeKey", "Flee Key", System.Windows.Forms.Keys.Z, KeyBindType.Press);

            public static MenuKeyBind AutoClearMinions = new MenuKeyBind("Auto Clear Minions", "Clear Minions AUTO", System.Windows.Forms.Keys.N, KeyBindType.Toggle);
        }
    }
    public class loaded
    {
        private static AIHeroClient objPlayer = ObjectManager.Player;
        private static Menu myMenu;
        public static Spell Q;
        public static Spell W;
        public static Spell E, E1, E2;
        public static Spell R, R2;

        public static void OnLoad()
        {
            Q = new Spell(SpellSlot.Q, 600f);
            Q.SetTargetted(0f, Qspeed());

            W = new Spell(SpellSlot.W, 825f);
            W.SetCharged("IreliaW", "ireliawdefense", 800, 800, 0);

            E = new Spell(SpellSlot.E, 775f);
            E.SetSkillshot(0.25f, 5f, 1800f, false, false, SkillshotType.Line);

            E1 = new Spell(SpellSlot.Unknown, 775f);
            E1.SetSkillshot(0.15f + (0.25f * 2), 5f, 1800f, false, false, SkillshotType.Circle);

            E2 = new Spell(SpellSlot.Unknown, 775f);
            E2.SetSkillshot(0.25f, 5f, 1800f, false, false, SkillshotType.Line);

            R = new Spell(SpellSlot.R, 1000);
            R.SetSkillshot(2f, 100, 1000f, true, SkillshotType.Line);

            R2 = new Spell(SpellSlot.Unknown, 1000);
            R.SetSkillshot(0.25f, 300, 1500, false, SkillshotType.Line);

            myMenu = new Menu(objPlayer.CharacterName, "Irelia The Flash", true);
            Menu FStarget = new Menu("FS Target", "Target Selector");
            FStarget.AddTargetSelectorMenu();
            myMenu.Add(FStarget);

            Menu Qmenu = new Menu("Qmenu", "Q Settings")
            {
                MenuSettings.QSettings.Qcombo,
                MenuSettings.QSettings.QStacks,
                MenuSettings.QSettings.QDancing,
            };

            Menu Wmenu = new Menu("Wmenu", "W Settings")
            {
                MenuSettings.WSettings.Wcombo,
                MenuSettings.WSettings.Wdelay,
            };

            Menu Emenu = new Menu("Emenu", "E Settings")
            {
                MenuSettings.ESettings.Ecombo,
                MenuSettings.ESettings.ImproveE,
                MenuSettings.ESettings.Efeedback,
            };

            Menu Rmenu = new Menu("Rmenu", "R Settings")
            {
                MenuSettings.RSettings.Rcombo,
                MenuSettings.RSettings.Rheath,
                MenuSettings.RSettings.Rhit,
            };

            Menu ClearMenu = new Menu("ClearMenu", "Clear Settings")
            {
                MenuSettings.ClearSettings.DrawMinions,
                MenuSettings.ClearSettings.QireClear,
                MenuSettings.ClearSettings.QireMana,
            };

            Menu Keys = new Menu("Keys", "Keys");
            Keys.Add(MenuSettings.KeysSettings.TurretKey).Permashow();
            Keys.Add(MenuSettings.KeysSettings.FleeKey).Permashow();
            Keys.Add(MenuSettings.KeysSettings.SemiE).Permashow();
            Keys.Add(MenuSettings.KeysSettings.SemiR).Permashow();
            Keys.Add(MenuSettings.KeysSettings.AutoClearMinions).Permashow();

            myMenu.Add(Qmenu);
            myMenu.Add(Wmenu);
            myMenu.Add(Emenu);
            myMenu.Add(Rmenu);
            myMenu.Add(ClearMenu);
            myMenu.Add(Keys);

            myMenu.Attach();

            Game.OnUpdate += OnUpdate;
            AIBaseClient.OnProcessSpellCast += AIBaseClient_OnProcessSpellCast;
            AIHeroClient.OnBuffLose += AIHeroClient_OnBuffLose;
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnDraw += DRAW_RPOS;
            Game.OnUpdate += IRELIA_LANECLEAR;
            Game.OnUpdate += IRELIA_KS;
            Game.OnUpdate += IRELIA_ECOMBO;
            Game.OnUpdate += IRELIA_RCOMBO;
            Game.OnUpdate += IRELIA_QCOMBO;
        }

        private static void IRELIA_QCOMBO(EventArgs args)
        {
            if (objPlayer.IsDead)
                return;

            if (Orbwalker.ActiveMode != OrbwalkerMode.Combo)
                return;

            if (!Q.IsReady() || !MenuSettings.QSettings.Qcombo.Enabled)
                return;

            var targets = TargetSelector.GetTargets(3000);
            if (targets == null)
                return;

            var target = FSTargetSelector.GetFSTarget(3000);
            if (target == null)
                return;

            var MaxObjs = ObjectManager.Get<AIBaseClient>().Where(i => !i.IsAlly && i.IsValidTarget(600) && CanQ(i) && Q.CanCast(i)).OrderByDescending(i => i.Distance(target));
            var MinObjs = ObjectManager.Get<AIBaseClient>().Where(i => !i.IsAlly && i.IsValidTarget(600) && CanQ(i) && Q.CanCast(i)).OrderBy(i => i.Distance(target));

            var Heallist = new List<float>
            {
                0f, 12f , 14f, 16f, 18f, 20f
            };

            var Heal = Heallist[Q.Level] * 0.2f * objPlayer.TotalAttackDamage;

            GapCloserTargetCanKillable();

            if (CanQ(target))
            {
                if (objPlayer.Health <= objPlayer.MaxHealth - Heal * MaxObjs.Count() && MenuSettings.QSettings.QDancing.Enabled)
                {
                    if (MaxObjs != null && MinObjs != null && MaxObjs.Count() >= 2 && MinObjs.Count() >= 2)
                    {
                        AIBaseClient Gap = null;
                        AIBaseClient Flee = null;

                        
                        foreach (var max in MaxObjs)
                        {
                            foreach (var min in MinObjs)
                            {
                                if (Gap != null && Flee != null && Vector3.Distance(Gap.Position, Flee.Position) <= Q.Range)
                                {
                                    if(Gap.NetworkId != Flee.NetworkId)
                                    {
                                        break;
                                    }
                                }

                                if (Gap != null)
                                {
                                    if (max.Distance(Gap) <= Q.Range)
                                    {
                                        Flee = max;
                                        break;
                                    }
                                }
                                else
                                {
                                    Flee = max;
                                }

                                if (Flee != null)
                                {
                                    if (min.Distance(Flee) <= Q.Range)
                                    {
                                        Gap = min;
                                        break;
                                    }
                                }
                                else
                                {
                                    Gap = min;
                                }                               
                            }

                            if (Gap != null && Flee != null && Vector3.Distance(Gap.Position, Flee.Position) <= Q.Range)
                            {
                                if (Gap.NetworkId != Flee.NetworkId)
                                {
                                    break;
                                }
                            }
                        }

                        if (Flee != null && Gap != null && Gap.NetworkId != Flee.NetworkId && Vector3.Distance(Gap.Position, Flee.Position) <= Q.Range)
                        {
                            if (Q.Cast(Flee) == CastStates.SuccessfullyCasted || Q.CastOnUnit(Flee))
                            {
                                return;
                            }
                        }
                        else
                        {
                            QGapCloserPos(target.Position);
                        }

                    }
                    else
                    {
                        QGapCloserPos(target.Position);
                    }
                }
                else
                {
                    QGapCloserPos(target.Position);
                }
            }
            else
            {
                foreach (var t in targets.OrderBy(i => i.Health))
                {
                    if (CanQ(t))
                    {
                        if (objPlayer.Health <= objPlayer.MaxHealth - Heal * MaxObjs.Count() && MenuSettings.QSettings.QDancing.Enabled)
                        {
                            if (MaxObjs != null && MinObjs != null && MaxObjs.Count() >= 2 && MinObjs.Count() >= 2)
                            {
                                AIBaseClient Gap = null;
                                AIBaseClient Flee = null;


                                foreach (var max in MaxObjs)
                                {
                                    foreach (var min in MinObjs)
                                    {
                                        if (Gap != null)
                                        {
                                            if (max.Distance(Gap) <= Q.Range)
                                            {
                                                Flee = max;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            Flee = max;
                                        }

                                        if (Flee != null)
                                        {
                                            if (min.Distance(Flee) <= Q.Range)
                                            {
                                                Gap = min;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            Gap = min;
                                        }
                                    }

                                    if (Gap != null && Flee != null && Vector3.Distance(Gap.Position, Flee.Position) <= Q.Range)
                                    {
                                        break;
                                    }
                                }

                                if (Flee != null && Gap != null)
                                {
                                    if (Q.Cast(Flee) == CastStates.SuccessfullyCasted || Q.CastOnUnit(Flee))
                                    {
                                        return;
                                    }
                                }
                                else
                                {
                                    QGapCloserPos(t.Position);
                                }

                            }
                            else
                            {
                                QGapCloserPos(t.Position);
                            }
                        }
                        else
                        {
                            QGapCloserPos(t.Position);
                        }
                    }
                    else
                    {
                        QGapCloserPos(target.Position);
                    }
                }
            }
        }

        private static void DRAW_RPOS(EventArgs args)
        {
            if (objPlayer.IsDead)
                return;

            if (GetRPos1 != null)
            {
                GetRPos1.Draw(System.Drawing.Color.Red, 1);
            }
            if (GetRPos2 != null)
            {
                GetRPos2.Draw(System.Drawing.Color.Red, 1);
            }
        }

        public static Geometry.Rectangle GetRPos1;
        public static Geometry.Rectangle GetRPos2;
        private static void IRELIA_RCOMBO(EventArgs args)
        {
            if (objPlayer.IsDead)
            {
                GetRPos1 = null;
                GetRPos2 = null;
                return;
            }

            if (Orbwalker.ActiveMode != OrbwalkerMode.Combo)
            {
                GetRPos1 = null;
                GetRPos2 = null;
                return;
            }

            if (!R.IsReady() || !MenuSettings.RSettings.Rcombo.Enabled)
            {
                GetRPos1 = null;
                GetRPos2 = null;
                return;
            }

            var target = FSTargetSelector.GetFSTarget(1000);
            if (target == null || target.HasBuff("ireliamark"))
                return;
            var pred = FSpred.Prediction.Prediction.PredictUnitPosition(target, 600);
            if (pred != null)
            {
                if (target.HealthPercent <= MenuSettings.RSettings.Rheath.Value)
                {
                    if (R.Cast(pred))
                        return;
                }
                GetRPos1 = new Geometry.Rectangle(objPlayer.Position, pred.ToVector3(), 110);
                GetRPos2 = new Geometry.Rectangle(pred, pred.Extend(objPlayer.Position, -450), 300);

                var TargetCount1 = GameObjects.EnemyHeroes.Where(i => GetRPos1.IsInside(i.Position)).Count();
                var TargetCount2 = GameObjects.EnemyHeroes.Where(i => GetRPos2.IsInside(i.Position) && !GameObjects.EnemyHeroes.Where(a => GetRPos1.IsInside(a.Position)).Any(h => i.NetworkId == h.NetworkId)).Count();

                if (TargetCount1 >= MenuSettings.RSettings.Rhit.Value
                    || TargetCount2 >= MenuSettings.RSettings.Rhit.Value
                    || TargetCount1 + TargetCount2 >= MenuSettings.RSettings.Rhit.Value
                    )
                {
                    if (R.Cast(pred))
                        return;
                }
            }
            else
            {
                GetRPos1 = null;
                GetRPos2 = null;
            }
        }

        private static void IRELIA_ECOMBO(EventArgs args)
        {
            if (objPlayer.IsDead)
                return;

            if (Orbwalker.ActiveMode != OrbwalkerMode.Combo)
                return;

            if (!E.IsReady())
                return;

            if (MenuSettings.ESettings.Ecombo.Enabled)
            {
                NewEPred();
            }
        }

        private static void IRELIA_KS(EventArgs args)
        {
            if (objPlayer.IsDead)
                return;

            if (!Q.IsReady())
                return;

            var minions = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(600)).Where(i => i.Health <= GetQDmg(i)).OrderBy(i => i.DistanceToPlayer());

            if (minions == null)
                return;

            foreach (var min in minions)
            {
                if (!UnderTower(min.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                {
                    if (Q.CastOnUnit(min))
                        return;
                }
            }
        }

        private static void IRELIA_LANECLEAR(EventArgs args)
        {
            if (objPlayer.IsDead)
                return;

            if (!Q.IsReady())
                return;

            if (!MenuSettings.ClearSettings.QireClear.Enabled) return;
            if (objPlayer.ManaPercent <= MenuSettings.ClearSettings.QireMana.Value) return;

            if (Orbwalker.ActiveMode != OrbwalkerMode.LaneClear && Orbwalker.ActiveMode != OrbwalkerMode.LastHit && !MenuSettings.KeysSettings.AutoClearMinions.Active)
                return;

            var minions = GameObjects.EnemyMinions.Where(i => i.IsValidTarget(600)).OrderBy(i => i.Health);

            if (minions == null)
                return;

            foreach (var min in minions)
            {
                if (CanQ(min))
                {
                    if (!UnderTower(min.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                    {
                        if (Q.Cast(min) == CastStates.SuccessfullyCasted || Q.CastOnUnit(min))
                            return;
                    }
                }
            }
        }

        public static float SheenTimer = 0;
        private static void AIHeroClient_OnBuffLose(AIBaseClient sender, AIBaseClientBuffLoseEventArgs args)
        {
            if (sender.MemoryAddress == objPlayer.MemoryAddress || sender.NetworkId == objPlayer.NetworkId || sender.IsMe)
            {
                if (args.Buff.Name == "sheen"
                    || args.Buff.Name == "trinityforce"
                    || args.Buff.Name.Contains("sheen")
                    || args.Buff.Name.Contains("trinity")
                    || args.Buff.Name.Contains("iceborn")
                    || args.Buff.Name.Contains("Lich"))
                {
                    SheenTimer = Environment.TickCount + 1700;
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!MenuSettings.ClearSettings.DrawMinions.Enabled) return;

            var minions = GameObjects.GetMinions(600).Where(i => CanQ(i)).ToList();

            if (minions == null) return;

            foreach (var min in minions)
            {
                Drawing.DrawCircle(min.Position, 70, System.Drawing.Color.Red);
            }
        }

        public static Vector3 ECatPos;

        private static void AIBaseClient_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "IreliaEMissile")
            {
                ECatPos = args.End;
            }
        }
        private static void OnUpdate(EventArgs args)
        {
            Q.Speed = Qspeed();
            if (objPlayer.IsDead || objPlayer.IsRecalling())
                return;

            if (MenuSettings.KeysSettings.FleeKey.Active)
            {
                objPlayer.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                QGapCloserPos(Game.CursorPos);
            }

            if (MenuSettings.KeysSettings.SemiR.Active || MenuSettings.KeysSettings.SemiE.Active)
            {
                CastManual();
            }
        }

        private static void CastManual()
        {
            if (MenuSettings.KeysSettings.SemiE.Active && E.IsReady())
            {

                #region New E pred
                var targets = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(2000) && !i.IsDead);
                var target = TargetSelector.GetTarget(775);
                {
                    if (target == null) return;

                    float ereal = 0.3f + Game.Ping / 1000;

                    if (E.IsReady(0))
                    {
                        if (!objPlayer.HasBuff("IreliaE"))
                        {
                            if (E1.GetPrediction(target).CastPosition.DistanceToPlayer() < 775)
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
                        if (E.IsReady() && E.Name != "IreliaE" && ECatPos.IsValid())
                        {
                            Vector2 vector2 = FSpred.Prediction.Prediction.PredictUnitPosition(target, 600);
                            if (vector2.Distance(ObjectManager.Player.Position) < E.Range)
                            {
                                Vector2 v3 = vector2;
                                int slider = 775;
                                for (int j = 50; j <= slider; j += 50)
                                {
                                    Vector2 vector3 = vector2.Extend(ECatPos.ToVector2(), (float)(-(float)j));
                                    if (vector3.Distance(ObjectManager.Player) >= E.Range)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        v3 = vector3;
                                        continue;
                                    }
                                }
                                if (E.Cast(v3.ToVector3()))
                                {
                                    return;
                                }
                            }
                        }
                    }
                }
                #endregion



            }
            if (MenuSettings.KeysSettings.SemiR.Active && R.IsReady())
            {
                #region R
                try
                {
                    var targets = TargetSelector.GetTargets(900);
                    Vector3 Rpos = Vector3.Zero;

                    if (!targets.Any()) return;
                    foreach (var Rprediction in targets.Select(i => R.GetPrediction(i)).Where(i => i.Hitchance >= HitChance.High || (i.Hitchance >= HitChance.Medium && i.AoeTargetsHitCount > 1)).OrderByDescending(i => i.AoeTargetsHitCount))
                    {
                        Rpos = Rprediction.CastPosition;
                    }
                    if (Rpos != Vector3.Zero)
                    {
                        R.Cast(Rpos);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("R.cast Error" + ex);
                }
                #endregion
            }
        }
             
        public static bool UnderTower(Vector3 pos, float bonusrange = 0)
        {
            return
                ObjectManager.Get<AITurretClient>()
                    .Any(i => i.IsEnemy && !i.IsDead && (i.Distance(pos) < 850 + ObjectManager.Player.BoundingRadius + bonusrange));
        }
        public static void GapCloserTargetCanKillable()
        {
            var target = GameObjects.EnemyHeroes.Where(i => !i.IsDead && i.IsValidTarget(2000) && CanQ(i)).OrderBy(i => i.DistanceToPlayer()).FirstOrDefault();
            if (target == null || !Q.IsReady()) return;

            if (target.IsValidTarget(Q.Range))
            {
                if (!UnderTower(target.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                {
                    Q.Cast(target);
                }
            }

            var gapobjs = ObjectManager.Get<AIBaseClient>().Where(i => !i.IsAlly && !i.IsDead && i.IsValidTarget(2000) && CanQ(i)).ToArray();
            if (gapobjs.Any())
            {
                foreach (var obj1 in gapobjs)
                {
                    if (obj1.Distance(target) < Q.Range && obj1.IsValidTarget(Q.Range))
                    {
                        if (!UnderTower(obj1.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                        {
                            Q.Cast(obj1);
                        }
                    }

                    if (gapobjs.Count() >= 2)
                    {
                        foreach (var obj2 in gapobjs.Where(i => i.IsValidTarget(Q.Range)))
                        {
                            if (obj1.Distance(target) < Q.Range)
                            {
                                if (obj2.Distance(obj1) < Q.Range)
                                {
                                    if (!UnderTower(obj2.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                                    {
                                        Q.Cast(obj2);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public static void QGapCloserPos(Vector3 pos)
        {
            var obj = GameObjects.Get<AIBaseClient>().Where(i => i.IsValidTarget(Q.Range)
            && !i.IsDead
            && !i.IsAlly
            && CanQ(i)
            && (i.Position.Distance(pos) <= objPlayer.Distance(pos) + objPlayer.GetRealAutoAttackRange() || i.Position.Distance(pos) <= objPlayer.GetRealAutoAttackRange() + 50)
            ).OrderBy(i => i.DistanceToPlayer()).FirstOrDefault();

            if (Q.IsReady() && obj != null)
            {
                if (!UnderTower(obj.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                    if (Q.Cast(obj) == CastStates.SuccessfullyCasted || Q.CastOnUnit(obj))
                        return;
            }
        }
        public static bool CanQ(AIBaseClient target)
        {
            if (target.Health > GetQDmg(target) && !target.HasBuff("ireliamark"))
            {
                return false;
            }
            else return true;
        }
        public static void NewEPred()
        {
            var targets = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(2000) && !i.IsDead);
            var target = FSTargetSelector.GetFSTarget(2000);
            {
                if (target == null || target.HasBuff("ireliamark")) return;

                if (target != null && !target.HasBuff("ireliamark"))
                {
                    float ereal = 0.275f + Game.Ping / 1000;

                    if (E.IsReady(0))
                    {
                        if (E.IsReady() && E.Name == "IreliaE")
                        {
                            if (E1.GetPrediction(target).CastPosition.DistanceToPlayer() < 825)
                            {
                                Geometry.Circle circle = new Geometry.Circle(objPlayer.Position, 600, 50);

                                {
                                    foreach (var onecircle in circle.Points)
                                    {
                                        if (onecircle.Distance(target) > 600)
                                        {
                                            if (E.Cast(onecircle))
                                                return;
                                        }
                                    }
                                }
                            }
                            if (objPlayer.IsDashing())
                            {
                                if (objPlayer.GetDashInfo().EndPos.Distance(target) < 775)
                                {
                                    Geometry.Circle circle = new Geometry.Circle(objPlayer.Position, 775, 50);

                                    {
                                        foreach (var onecircle in circle.Points.Where(i => i.Distance(target) > 775))
                                        {
                                            if (E.Cast(onecircle))
                                                return;
                                        }
                                    }
                                }
                            }
                        }
                        if (MenuSettings.ESettings.ImproveE.Enabled)
                        {
                            if (E.IsReady() && E.Name != "IreliaE" && ECatPos.IsValid())
                            {
                                var vector2 = FSpred.Prediction.Prediction.PredictUnitPosition(target, 700);
                                var v3 = vector2;
                                for (int j = 50; j <= 900; j += 20)
                                {
                                    var vector3 = vector2.Extend(ECatPos.ToVector2(), -j);
                                    if (vector3.Distance(ObjectManager.Player) >= E.Range)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        v3 = vector3;
                                        continue;
                                    }
                                }
                                if (E.Cast(v3.ToVector3()))
                                {
                                    return;
                                }
                            }
                        }
                        else
                        {
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

                                    if (castepos != Vector2.Zero)
                                    {
                                        if (E.Cast(castepos))
                                            return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static float[] QBaseDamage = { 0f, 5f, 25f, 45f, 65f, 85f };
        public static float[] QBonusDamage = { 0f, 55f, 75f, 95f, 115f, 135f };

        private static float PassiveDamage(AIBaseClient target)
        {
            var ireliaPassiveDamage = 15f;
            ireliaPassiveDamage += (objPlayer.Level - 1) * 3;
            ireliaPassiveDamage += 25 / 100 * objPlayer.GetBonusPhysicalDamage();


            return ireliaPassiveDamage;
        }

        public static float Sheen(AIBaseClient target)
        {
            float damage = 0;
            bool sheen = false;
            bool trinity = false;

            if (ObjectManager.Player.CanUseItem((int)ItemId.Trinity_Force))
            {
                trinity = true;
            }
            else { trinity = false; }
            if (ObjectManager.Player.CanUseItem((int)ItemId.Sheen))
            {
                sheen = true;
            }
            else { sheen = false; }

            var Sheen = new Items.Item(ItemId.Sheen, 0f);
            if (
                objPlayer.CanUseItem((int)ItemId.Sheen) ||
                (Sheen.IsOwned() && Sheen.IsReady) ||
                sheen
                )
            {
                damage = objPlayer.BaseAttackDamage;
            }

            var Trinity = new Items.Item(ItemId.Trinity_Force, 0f);
            if (
                objPlayer.CanUseItem((int)ItemId.Trinity_Force) ||
                (Trinity.IsOwned() && Trinity.IsReady) ||
                trinity
                )
            {
                damage = objPlayer.BaseAttackDamage * 2;
            }

            if (Environment.TickCount < SheenTimer)
            {
                damage = 0;
            }

            return damage;
        }

        public static float GetQDmg(AIBaseClient target)
        {
            if (objPlayer.HasBuff("ireliapassivestacksmax"))
            {
                return Q.GetDamage(target) + (float)objPlayer.CalculatePhysicalDamage(target, Sheen(target)) + (float)objPlayer.CalculateMagicDamage(target, PassiveDamage(target));
            }
            else
            {
                return Q.GetDamage(target) + (float)objPlayer.CalculatePhysicalDamage(target, Sheen(target));
            }
        }

        /*private static double GetQDmg(AIBaseClient target)
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
            if (target.IsMinion && !target.IsJungle())
            {
                Alldmg = dmgMinions + (ObjectManager.Player.TotalAttackDamage * 60 / 100) + dmgSheen;
            }
            return Q.GetDamage(target) + ObjectManager.Player.CalculateMagicDamage(target, passive) + objPlayer.CalculateDamage(target, DamageType.Physical, dmgSheen);
        }*/
        public static float Qspeed()
        {
            return 1500 + objPlayer.MoveSpeed;
        }
        public static List<AIBaseClient> Q_ListAIBaseClient(float minvalue = 0, float maxvalue = 0, AIBaseClient target = null)
        {
            if (target == null)
                return GameObjects.Get<AIBaseClient>().Where(i => !i.IsDead && i.IsValidTarget(maxvalue) && (minvalue != 0 ? i.DistanceToPlayer() >= minvalue : i.IsValidTarget(maxvalue)) && !i.IsAlly && CanQ(i)).ToList();
            else
                return GameObjects.Get<AIBaseClient>().Where(i => !i.IsDead && i.IsValidTarget(maxvalue) && (minvalue != 0 ? i.DistanceToPlayer() >= minvalue : i.IsValidTarget(maxvalue)) && !i.IsAlly && CanQ(i) && i.NetworkId != target.NetworkId).ToList();
        }
    }
}
