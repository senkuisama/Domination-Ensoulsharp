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
            public static MenuSeparator Efeedback = new MenuSeparator("Efeedback", "E logic if not good Feedback it to FunnySlayer#0348");
        }

        public class RSettings
        {
            public static MenuBool Rcombo = new MenuBool("Rcombo", "R in Combo [Calculator Dmg]");
            public static MenuSlider Rheath = new MenuSlider("Rheath", "Target Heath", 60);
            public static MenuSlider Rhit = new MenuSlider("Rhit", "Hit Count", 3, 2, 5);
        }
        public class KeysSettings
        {
            public static MenuKeyBind TurretKey = new MenuKeyBind("TurretKey", "Turret Key", System.Windows.Forms.Keys.A, KeyBindType.Toggle);
            public static MenuKeyBind SemiE = new MenuKeyBind("SemiE", "E Using Key", System.Windows.Forms.Keys.G, KeyBindType.Press);
            public static MenuKeyBind SemiR = new MenuKeyBind("SemiR", "R Using Key", System.Windows.Forms.Keys.T, KeyBindType.Toggle);
            public static MenuKeyBind FleeKey = new MenuKeyBind("FleeKey", "Flee Key", System.Windows.Forms.Keys.Z, KeyBindType.Press);
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
            E.SetSkillshot(0.25f, 80f, 1800f, false, false, SkillshotType.Line);

            E1 = new Spell(SpellSlot.Unknown, 775f);
            E1.SetSkillshot(0.15f + (0.25f * 2), 80, 1800f, false, false, SkillshotType.Circle);

            E2 = new Spell(SpellSlot.Unknown, 775f);
            E2.SetSkillshot(0.25f, 80f, 1800f, false, false, SkillshotType.Line);

            R = new Spell(SpellSlot.R, 1000);
            R.SetSkillshot(0.5f, 100, 1500, false, SkillshotType.Line);

            R2 = new Spell(SpellSlot.Unknown, 1000);
            R.SetSkillshot(0.25f, 300, 1500, false, SkillshotType.Line);

            myMenu = new Menu(objPlayer.CharacterName, "Irelia The Flash", true);

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
                MenuSettings.ESettings.Efeedback,
            };

            Menu Rmenu = new Menu("Rmenu", "R Settings")
            {
                MenuSettings.RSettings.Rcombo,
                MenuSettings.RSettings.Rheath,
                MenuSettings.RSettings.Rhit,
            };

            Menu Keys = new Menu("Keys", "Keys");
            Keys.Add(MenuSettings.KeysSettings.TurretKey).Permashow();
            Keys.Add(MenuSettings.KeysSettings.FleeKey).Permashow();
            Keys.Add(MenuSettings.KeysSettings.SemiE).Permashow();
            Keys.Add(MenuSettings.KeysSettings.SemiR).Permashow();

            myMenu.Add(Qmenu);
            myMenu.Add(Wmenu);
            myMenu.Add(Emenu);
            myMenu.Add(Rmenu);
            myMenu.Add(Keys);

            myMenu.Attach();

            Game.OnUpdate += OnUpdate;
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
        private static void OnUpdate(EventArgs args)
        {
            Q.Speed = Qspeed();
            if (objPlayer.IsDead || objPlayer.IsRecalling())
                return;

            if(MenuSettings.KeysSettings.FleeKey.Active)
            {
                objPlayer.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                QGapCloserPos(Game.CursorPos);
            }

            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    Irelia_Combo();
                    break;
                case OrbwalkerMode.LaneClear:
                    Irelia_Clear();
                    break;
            }
        }

        public static void Irelia_Combo()
        {
            foreach(AIBaseClient target in GameObjects.Get<AIHeroClient>().Where(i => !i.IsDead && i.IsValidTarget(2000) && !i.IsAlly))
            {
                if (target == null) return;

                var OutAARange = Q_ListAIBaseClient(300, Q.Range + 1, target);
                var InAARange = Q_ListAIBaseClient(0, 300, target);
                var QRange = Q_ListAIBaseClient(0, Q.Range + 1);
                //ireliapassivestacksmax             Passive Stacks
                //ireliamark                         E Buffs
                if (Q.IsReady() && MenuSettings.QSettings.Qcombo.Enabled)
                {
                    if (CanQ(target))
                    {
                        if (!objPlayer.HasBuff("ireliapassivestacksmax") || objPlayer.HealthPercent <= 70)
                        {
                            if (OutAARange.Any())
                            {
                                foreach (AIBaseClient obj in OutAARange)
                                {
                                    if (obj.Distance(target) <= 600)
                                        if (!UnderTower(obj.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                                            Q.Cast(obj);
                                }
                            }
                            else
                            {
                                if (InAARange.Any())
                                {
                                    foreach (AIBaseClient obj in InAARange)
                                    {
                                        if (obj.Distance(target) <= 600)
                                            if (!UnderTower(obj.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                                                Q.Cast(obj);
                                    }
                                }
                                else
                                {
                                    #region Use Same Fuction
                                    if (!objPlayer.HasBuff("ireliapassivestacksmax") || objPlayer.HealthPercent <= 70)
                                    {
                                        if (QRange.Any() && QRange.Count >= 2)
                                        {
                                            foreach (AIBaseClient one in QRange)
                                            {
                                                foreach (AIBaseClient two in QRange)
                                                {
                                                    if (one.NetworkId != two.NetworkId)
                                                    {
                                                        AIBaseClient follow = null;
                                                        if ((one.Position.Distance(target.Position) <= objPlayer.Position.Distance(target.Position) || one.Position.Distance(target.Position) <= objPlayer.GetRealAutoAttackRange()) && one.Position.Distance(target.Position) < two.Position.Distance(target.Position) && one.Position.Distance(two.Position) <= Q.Range)
                                                        {
                                                            follow = two;
                                                        }
                                                        else
                                                        {
                                                            if ((two.Position.Distance(target.Position) <= objPlayer.Position.Distance(target.Position) || two.Position.Distance(target.Position) <= objPlayer.GetRealAutoAttackRange()) && two.Position.Distance(target.Position) < one.Position.Distance(target.Position) && one.Position.Distance(two.Position) <= Q.Range)
                                                            {
                                                                follow = one;
                                                            }
                                                            else
                                                            {
                                                                QGapCloserPos(target.Position);
                                                            }
                                                        }

                                                        if (follow == null)
                                                        {
                                                            QGapCloserPos(target.Position);
                                                        }
                                                        else
                                                        {
                                                            if (objPlayer.ManaPercent >= 15)
                                                            {
                                                                if (!UnderTower(follow.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                                                                    Q.Cast(follow);
                                                            }
                                                            else
                                                            {
                                                                QGapCloserPos(target.Position);
                                                            }
                                                        }
                                                    }
                                                }
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

                                    #endregion
                                }
                            }
                        }
                        else
                        {
                            if (target.DistanceToPlayer() <= Q.Range + 1)
                            {
                                if (!UnderTower(target.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                                    Q.Cast(target);
                            }
                        }
                    }

                    if (!CanQ(target))
                    {
                        if (!objPlayer.HasBuff("ireliapassivestacksmax") || objPlayer.HealthPercent <= 70)
                        {
                            if (QRange.Any() && QRange.Count >= 2)
                            {
                                foreach(AIBaseClient one in QRange)
                                {
                                    foreach(AIBaseClient two in QRange)
                                    {
                                        if(one.NetworkId != two.NetworkId)
                                        {
                                            AIBaseClient follow = null;
                                            if ((one.Position.Distance(target.Position) <= objPlayer.Position.Distance(target.Position) || one.Position.Distance(target.Position) <= objPlayer.GetRealAutoAttackRange()) && one.Position.Distance(target.Position) < two.Position.Distance(target.Position) && one.Position.Distance(two.Position) <= Q.Range)
                                            {
                                                follow = two;
                                            }
                                            else
                                            {
                                                if ((two.Position.Distance(target.Position) <= objPlayer.Position.Distance(target.Position) || two.Position.Distance(target.Position) <= objPlayer.GetRealAutoAttackRange()) && two.Position.Distance(target.Position) < one.Position.Distance(target.Position) && one.Position.Distance(two.Position) <= Q.Range)
                                                {
                                                    follow = one;
                                                }
                                                else
                                                {
                                                    QGapCloserPos(target.Position);
                                                }
                                            }

                                            if(follow == null)
                                            {
                                                QGapCloserPos(target.Position);
                                            }
                                            else
                                            {
                                                if(objPlayer.ManaPercent >= 15)
                                                {
                                                    if (!UnderTower(follow.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                                                        Q.Cast(follow);
                                                }
                                                else
                                                {
                                                    QGapCloserPos(target.Position);
                                                }
                                            }
                                        }
                                    }
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
                }

                if(E.IsReady() && MenuSettings.ESettings.Ecombo.Enabled)
                {
                    NewEPred();
                }
                if (R.IsReady() && MenuSettings.RSettings.Rcombo.Enabled && !target.HasBuff("ireliamark"))
                {
                    var Rpred = R.GetPrediction(target, false, -1, CollisionObjects.YasuoWall);

                    if (objPlayer.Position.CountEnemyHeroesInRange(800) < 2 && target.Distance(objPlayer) < 600)
                    {
                        if (target.Health <= GetQDmg(target) * 3 + (W.IsReady() ? W.GetDamage(target) : 0) + (E.IsReady() ? E.GetDamage(target) : 0) + R.GetDamage(target) + 100)
                        {
                            if (Rpred.Hitchance >= HitChance.High && Rpred.CastPosition.DistanceToPlayer() < R.Range - 200)
                                    R.Cast(Rpred.CastPosition);
                        }
                    }

                    try
                    {
                        var targets = GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(900)).ToArray();
                        var castPos = Vector3.Zero;

                        if (!targets.Any())
                        {
                            return;
                        }

                        foreach (var pred in
                            targets.Select(i => R2.GetPrediction(i, false, -1, CollisionObjects.YasuoWall))
                                .Where(
                                    i => i.Hitchance >= HitChance.Medium && i.AoeTargetsHitCount >= MenuSettings.RSettings.Rhit)
                                .OrderByDescending(i => i.AoeTargetsHitCount))
                        {
                            castPos = pred.CastPosition;
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
            }           
        }
        public static void Irelia_Clear()
        {
            var mins = GameObjects.Get<AIMinionClient>().Where(i => i.Position.Distance(objPlayer.Position) <= 600 && !i.IsAlly && !i.IsDead && CanQ(i));
            if (!mins.Any()) return;

            foreach(AIMinionClient min in mins)
            {
                if (!UnderTower(min.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                    Q.Cast(min);
            }
        }
        public static bool UnderTower(Vector3 pos, float bonusrange = 0)
        {
            return
                ObjectManager.Get<AITurretClient>()
                    .Any(i => i.IsEnemy && !i.IsDead && (i.Distance(pos) < 850 + ObjectManager.Player.BoundingRadius + bonusrange));
        }
        public static void QGapCloserPos(Vector3 pos)
        {
            var objs = new List<AIBaseClient>();
            objs.AddRange(ObjectManager.Get<AIBaseClient>().Where(i => i.IsValidTarget(Q.Range) && !i.IsDead && !i.IsAlly && CanQ(i)));

            if (Q.IsReady())
            {
                if (!objs.Any() || objs != null)
                {
                    foreach (AIBaseClient obj in objs)
                    {
                        if (obj.Position.Distance(pos) <= objPlayer.Distance(pos) || obj.Position.Distance(pos) <= objPlayer.GetRealAutoAttackRange() + 20)
                        {
                            if(Orbwalker.ActiveMode != OrbwalkerMode.Combo || !UnderTower(obj.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                            Q.Cast(obj);
                        }
                    }
                }
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
            var target = TargetSelector.GetTarget(2000);
            {
                if (target == null || target.HasBuff("ireliamark")) return;

                if (target != null && !target.HasBuff("ireliamark"))
                {
                    float ereal = 0.25f + 0.025f + Game.Ping / 1000 - 0.1f;

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
        public static float Qspeed()
        {
            return 1500 + objPlayer.MoveSpeed;
        }        
        public static List<AIBaseClient> Q_ListAIBaseClient(float minvalue = 0, float maxvalue = 0, AIBaseClient target = null)
        {
            if(target == null)
                return GameObjects.Get<AIBaseClient>().Where(i => !i.IsDead && i.DistanceToPlayer() <= maxvalue && (minvalue != 0 ? i.DistanceToPlayer() >= minvalue : i.IsValidTarget(maxvalue)) && !i.IsAlly && CanQ(i)).ToList();
            else
                return GameObjects.Get<AIBaseClient>().Where(i => !i.IsDead && i.DistanceToPlayer() <= maxvalue && (minvalue != 0 ? i.DistanceToPlayer() >= minvalue : i.IsValidTarget(maxvalue)) && !i.IsAlly && CanQ(i) && i.NetworkId != target.NetworkId).ToList();
        }
    }
}
