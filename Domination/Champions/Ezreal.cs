using EnsoulSharp;
using EnsoulSharp.SDK.MenuUI.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPredictionMash;
using EnsoulSharp.SDK;
using System.Drawing;
using EnsoulSharp.SDK.MenuUI;
using SharpDX;
using EnsoulSharp.SDK.Utility;

namespace DominationAIO.Champions
{
    #region EzrealMenu
    public class EzKeysSetting
    {
        public static readonly MenuKeyBind SemiRKey = new MenuKeyBind("SemiRKey", "R Using Key [Type : Press]", System.Windows.Forms.Keys.T, KeyBindType.Press);
        public static readonly MenuKeyBind FarmKey = new MenuKeyBind("FarmKey", "Spell Farming [Type : Toggle]", System.Windows.Forms.Keys.A, KeyBindType.Toggle);
        public static readonly MenuKeyBind HarassKey = new MenuKeyBind("HarassKey", "Harass When Farm [Type : Toggle]", System.Windows.Forms.Keys.A, KeyBindType.Toggle);
        public static readonly MenuKeyBind Stacks = new MenuKeyBind("Stacks", "Stacks Q [Type : Toggle]", System.Windows.Forms.Keys.M, KeyBindType.Toggle);
        public static readonly MenuKeyBind Flee = new MenuKeyBind("Flee", "Flee [Type : Press]", System.Windows.Forms.Keys.Z, KeyBindType.Press);
    }
    public class QEzSettings
    {
        public static readonly MenuBool Qcombo = new MenuBool("Qcombo", "Q Combo");
        public static readonly MenuBool QinAA = new MenuBool("QinAA", "Use Q when player on Auto Attack", false);
        public static readonly MenuBool Qharass = new MenuBool("Qharass", "Q Harass");
        public static readonly MenuSlider HarassMana = new MenuSlider("HarassMana", "(Harass) Min Mana : ", 60);
        public static readonly MenuBool QClear = new MenuBool("QClear", "Q Clear");
        public static readonly MenuSlider ClearMana = new MenuSlider("ClearMana", "(Clear) Min Mana : ", 40);
    }

    public class WEzSettings
    {
        public static readonly MenuBool Wcombo = new MenuBool("Wcombo", "W Combo");
        public static readonly MenuBool WClear = new MenuBool("WClear", "W Clear");
        public static readonly MenuBool BeforeAA = new MenuBool("BeforeAA", "Before AA");
    }

    public class EEzSettings
    {
        public static readonly MenuBool Ecombo = new MenuBool("Ecombo", "E Combo");
        public static readonly MenuSlider TargetHeath = new MenuSlider("TargetHeath", "Target Heath <= ", 40);
        public static readonly MenuSlider TargetCount = new MenuSlider("TargetCount", "Target Count <= ", 2, 1, 5);
    }

    public class REzSettings
    {
        public static readonly MenuBool Rcombo = new MenuBool("Rcombo", "R Combo");
        public static readonly MenuSlider MinR = new MenuSlider("MinR", "Min Range : ", 1000, 0, 3000);
        public static readonly MenuSlider MaxR = new MenuSlider("MaxR", "Max Range : ", 3000, 0, 5000);

        public static readonly MenuBool BaseUlt = new MenuBool("BaseUlt", "Use Base Ult");
    }

    public class DrawEzSettings
    {
        public static readonly MenuBool DrawQ = new MenuBool("DrawQ", "Draw Q && AA Range", false);
    }
    #endregion

    internal class Ezreal
    {     
        public static AIHeroClient Player = ObjectManager.Player;
        public static Menu EzrealMenu;
        public static Menu EzQmenu, EzWmenu, EzEmenu, EzRmenu, EzSpredictionmenu;
        public static Spell Q, W, E, R;

        public static void Ezreal_Load()
        {
            Game.Print("SPrediction Ezreal");

            EzQmenu = new Menu("Qmenu", "Q Settings");
            EzWmenu = new Menu("Wmenu", "W Settings");
            EzEmenu = new Menu("Emenu", "E Settings");
            EzRmenu = new Menu("Rmenu", "R Settings");

            EzSpredictionmenu = new Menu("EzSpredictionmenu", "(Sprediction)");

            Prediction.Initialize(EzSpredictionmenu);

            var keys = new Menu("Keys", "Keys");
            keys.Add(EzKeysSetting.SemiRKey).Permashow();
            keys.Add(EzKeysSetting.FarmKey).Permashow();
            keys.Add(EzKeysSetting.HarassKey).Permashow();
            keys.Add(EzKeysSetting.Stacks).Permashow();
            keys.Add(EzKeysSetting.Flee).Permashow();

            EzQmenu.Add(QEzSettings.Qcombo);
            EzQmenu.Add(QEzSettings.QinAA);
            EzQmenu.Add(QEzSettings.Qharass);
            EzQmenu.Add(QEzSettings.HarassMana);
            EzQmenu.Add(QEzSettings.QClear);
            EzQmenu.Add(QEzSettings.ClearMana);

            EzWmenu.Add(WEzSettings.Wcombo);
            EzWmenu.Add(WEzSettings.WClear);
            EzWmenu.Add(WEzSettings.BeforeAA);

            EzEmenu.Add(EEzSettings.Ecombo);
            EzEmenu.Add(EEzSettings.TargetHeath);
            EzEmenu.Add(EEzSettings.TargetCount);

            EzRmenu.Add(REzSettings.Rcombo);
            EzRmenu.Add(REzSettings.MinR);
            EzRmenu.Add(REzSettings.MaxR);
            EzRmenu.Add(REzSettings.BaseUlt);

            EzrealMenu = new Menu("EzrealMenu", "Ezreal Menu", true);
            EzrealMenu.Add(keys);
            EzrealMenu.Add(EzQmenu);
            EzrealMenu.Add(EzWmenu);
            EzrealMenu.Add(EzEmenu);
            EzrealMenu.Add(EzRmenu);

            EzrealMenu.Add(EzSpredictionmenu);
            EzrealMenu.Add(DrawEzSettings.DrawQ);
            EzrealMenu.PermaShowText = "(Sprediction) Ezreal";
            EzrealMenu.Attach();

            Drawing.OnDraw += Drawing_OnDraw;


            Q = new Spell(SpellSlot.Q, 1150f);
            Q.SetSkillshot(0.3f, 60f, 2000f, true, EnsoulSharp.SDK.Prediction.SkillshotType.Line);
            W = new Spell(SpellSlot.W, 1150f);
            W.SetSkillshot(0.3f, 60f, 1200f, false, EnsoulSharp.SDK.Prediction.SkillshotType.Line);
            E = new Spell(SpellSlot.E, 475f) { Delay = 0.65f };
            R = new Spell(SpellSlot.R, 20000f);
            R.SetSkillshot(1.10f, 160f, 2000f, false, EnsoulSharp.SDK.Prediction.SkillshotType.Line);

            Game.OnUpdate += Game_OnUpdate;
            Orbwalker.OnAction += Orbwalker_OnAction;

            Teleport.OnTeleport += Teleport_OnTeleport;
        }
       
        private static bool BeforeAA, OnAA, AfterAA;
        private static void Orbwalker_OnAction(object sender, OrbwalkerActionArgs args)
        {
            if(args.Type == OrbwalkerType.OnAttack)
            {
                OnAA = true;
            }
            else { OnAA = false; }

            if (args.Type == OrbwalkerType.BeforeAttack)
            {
                BeforeAA = true;
            }
            else { BeforeAA = false; }

            if (args.Type == OrbwalkerType.AfterAttack)
            {
                AfterAA = true;
            }
            else { AfterAA = false; }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Player.IsDead) return;

            BU();

            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    EzCombo();
                    break;
                case OrbwalkerMode.Harass:
                    EzHarass();
                    break;
                case OrbwalkerMode.LaneClear:
                    EzClear();
                    break;
                case OrbwalkerMode.LastHit:
                    break;


                default:
                    break;
            }
        }

        private static void BU()
        {
            if (!R.IsReady() || !REzSettings.BaseUlt.Enabled || RecalFinished == true) return;

            if (Orbwalker.ActiveMode == OrbwalkerMode.Combo) return;

            if(RecallTarget.Health + RecallTarget.AllShield < RDamage(RecallTarget))
            {

                RHitTime = GameObjects.EnemySpawnPoints.FirstOrDefault(i => i.IsValid).Position.DistanceToPlayer() / 2 + 1000;

                if(RHitTime >= RecalEndTime - (Variables.TickCount - RecalStartTime) && RHitTime <= RecalEndTime - (Variables.TickCount - RecalStartTime) + 1000)
                {
                    R.Cast(GameObjects.EnemySpawnPoints.FirstOrDefault(i => i.IsValid).Position);
                }
            }
        }
        private static void EzCombo()
        {
            EzWCombo(); 

            if(!Player.IsDashing())
                EzQCombo();

            if (EEzSettings.Ecombo.Enabled && E.IsReady())
                EzECombo();
        }

        private static void EzQCombo()
        {
            var target = TargetSelector.GetTarget(Q.Range);

            if (target == null) return;

            if (target == null) return;

            if ((!OnAA || QEzSettings.QinAA.Enabled) && QEzSettings.Qcombo.Enabled)
            {
                if (Q.IsReady())
                {
                    Q.SPredictionCast(target, EnsoulSharp.SDK.Prediction.HitChance.High);
                }
            }
        }

        private static void EzWCombo()
        {
            var target = TargetSelector.SelectedTarget;

            if (target == null ||
                !target.IsValidTarget(W.Range) /*|| 
                LinePrediction.GetPrediction(target, Q.Width, Q.Delay, Q.Speed, Q.Range, Q.Collision, waypoints, avgt, movt, avgp, target.LastAngleDiff(), Q.From.ToVector2(), Player.Position.ToVector2()).HitChance == HitChance.Collision ||
                LinePrediction.GetPrediction(target, Q.Width, Q.Delay, Q.Speed, Q.Range, Q.Collision, waypoints, avgt, movt, avgp, target.LastAngleDiff(), Q.From.ToVector2(), Player.Position.ToVector2()).HitChance == HitChance.OutOfRange ||
                LinePrediction.GetPrediction(target, Q.Width, Q.Delay, Q.Speed, Q.Range, Q.Collision, waypoints, avgt, movt, avgp, target.LastAngleDiff(), Q.From.ToVector2(), Player.Position.ToVector2()).HitChance == HitChance.Medium ||
                LinePrediction.GetPrediction(target, Q.Width, Q.Delay, Q.Speed, Q.Range, Q.Collision, waypoints, avgt, movt, avgp, target.LastAngleDiff(), Q.From.ToVector2(), Player.Position.ToVector2()).HitChance == HitChance.Low ||
                LinePrediction.GetPrediction(target, Q.Width, Q.Delay, Q.Speed, Q.Range, Q.Collision, waypoints, avgt, movt, avgp, target.LastAngleDiff(), Q.From.ToVector2(), Player.Position.ToVector2()).HitChance == HitChance.None ||
                LinePrediction.GetPrediction(target, Q.Width, Q.Delay, Q.Speed, Q.Range, Q.Collision, waypoints, avgt, movt, avgp, target.LastAngleDiff(), Q.From.ToVector2(), Player.Position.ToVector2()).CastPosition.DistanceToPlayer() > Q.Range
                */
                )
            {
                target = TargetSelector.GetTarget(W.Range);
            }

            if (target == null) return;


            if (OnAA) return;

            if (!WEzSettings.Wcombo.Enabled || !W.IsReady()) return;

            /*if (WEzSettings.Wcombo.Enabled && W.IsReady())
            { 
                W.SPredictionCast(target, EnsoulSharp.SDK.Prediction.HitChance.High);
            }    */
            float avgt = target.AvgMovChangeTime();
            float movt = target.LastMovChangeTime();
            float avgp = target.AvgPathLenght();
            var waypoints = target.GetWaypoints();

            if (LinePrediction.GetPrediction(target, Q.Width, Q.Delay, Q.Speed, Q.Range, Q.Collision, waypoints, avgt, movt, avgp, target.LastAngleDiff(), Q.From.ToVector2(), Player.Position.ToVector2()).HitChance >= EnsoulSharp.SDK.Prediction.HitChance.High)
            {
                W.SPredictionCast(target, EnsoulSharp.SDK.Prediction.HitChance.High);
            }

            if (target.InAutoAttackRange())
            {
                W.SPredictionCast(target, EnsoulSharp.SDK.Prediction.HitChance.High);
            }
        }

        private static void EzECombo()
        {
            var target = TargetSelector.SelectedTarget;

            /*float avgt = target.AvgMovChangeTime();
            float movt = target.LastMovChangeTime();
            float avgp = target.AvgPathLenght();
            var waypoints = target.GetWaypoints();*/


            if (target == null ||
                !target.IsValidTarget(Q.Range) /*|| 
                LinePrediction.GetPrediction(target, Q.Width, Q.Delay, Q.Speed, Q.Range, Q.Collision, waypoints, avgt, movt, avgp, target.LastAngleDiff(), Q.From.ToVector2(), Player.Position.ToVector2()).HitChance == HitChance.Collision ||
                LinePrediction.GetPrediction(target, Q.Width, Q.Delay, Q.Speed, Q.Range, Q.Collision, waypoints, avgt, movt, avgp, target.LastAngleDiff(), Q.From.ToVector2(), Player.Position.ToVector2()).HitChance == HitChance.OutOfRange ||
                LinePrediction.GetPrediction(target, Q.Width, Q.Delay, Q.Speed, Q.Range, Q.Collision, waypoints, avgt, movt, avgp, target.LastAngleDiff(), Q.From.ToVector2(), Player.Position.ToVector2()).HitChance == HitChance.Medium ||
                LinePrediction.GetPrediction(target, Q.Width, Q.Delay, Q.Speed, Q.Range, Q.Collision, waypoints, avgt, movt, avgp, target.LastAngleDiff(), Q.From.ToVector2(), Player.Position.ToVector2()).HitChance == HitChance.Low ||
                LinePrediction.GetPrediction(target, Q.Width, Q.Delay, Q.Speed, Q.Range, Q.Collision, waypoints, avgt, movt, avgp, target.LastAngleDiff(), Q.From.ToVector2(), Player.Position.ToVector2()).HitChance == HitChance.None ||
                LinePrediction.GetPrediction(target, Q.Width, Q.Delay, Q.Speed, Q.Range, Q.Collision, waypoints, avgt, movt, avgp, target.LastAngleDiff(), Q.From.ToVector2(), Player.Position.ToVector2()).CastPosition.DistanceToPlayer() > Q.Range
                */
                )
            {
                target = TargetSelector.GetTarget(E.Range + Q.Range);
            }

            if (target == null || !(target is AIHeroClient)) return;

            if (target == null || target.HealthPercent > EEzSettings.TargetHeath.Value + 1) return;

            if (OnAA) return;

            EnsoulSharp.SDK.Geometry.Circle EPoints = new EnsoulSharp.SDK.Geometry.Circle(Player.Position, E.Range);

            foreach(var EPoint in EPoints.Points)
            {
                float avgt = target.AvgMovChangeTime();
                float movt = target.LastMovChangeTime();
                float avgp = target.AvgPathLenght();
                var waypoints = target.GetWaypoints();

                if (EPoint != Vector2.Zero && EPoint.CountEnemyHeroesInRange(600) <= EEzSettings.TargetCount &&
                    !ObjectManager.Get<AITurretClient>()
                    .Any(i => i.IsEnemy && !i.IsDead && (i.Distance(EPoint) <= 850 + ObjectManager.Player.BoundingRadius))
                    )
                {
                    if (LinePrediction.GetPrediction(target, Q.Width, Q.Delay, Q.Speed, Q.Range, Q.Collision, waypoints, avgt, movt, avgp, target.LastAngleDiff(), Q.From.ToVector2(), Player.Position.ToVector2()).HitChance == EnsoulSharp.SDK.Prediction.HitChance.Collision)
                        return;

                    if (EPoint.Distance(target) < target.DistanceToPlayer() && EEzSettings.Ecombo.Enabled)
                    {
                        E.Cast(EPoint);
                    }
                }
            }
        }

        private static void EzHarass()
        {
            var target = TargetSelector.SelectedTarget;

            if (target == null || !target.IsValidTarget(Q.Range))
            {
                target = TargetSelector.GetTarget(Q.Range);
            }

            if (target == null || target.InAutoAttackRange())
            {
                target = Orbwalker.GetTarget() as AIHeroClient;
            }

            if (target == null || !(target is AIHeroClient)) return;

            if (Player.ManaPercent >= QEzSettings.HarassMana.Value)
                EzQCombo();
        }

        private static void EzClear()
        {
            if (OnAA || !QEzSettings.QClear.Enabled) return;

            var minions = GameObjects.Enemy.Where(i => i.IsValidTarget(Q.Range) && !i.IsDead && !i.Position.IsBuilding());
            if (minions.Any())
            {
                foreach(var minion in minions)
                {
                    if(minion is AIHeroClient)
                    {
                        if (Player.ManaPercent >= QEzSettings.HarassMana.Value)
                            Q.SPredictionCast(minion as AIHeroClient, EnsoulSharp.SDK.Prediction.HitChance.High);
                    }
                    else
                    {
                        if(minion is AIMinionClient)
                        {
                            if(minion.Health <= Q.GetDamage(minion) || minion.Health >= Q.GetDamage(minion) + Player.GetAutoAttackDamage(minion))
                            {
                                if(Player.ManaPercent >= QEzSettings.ClearMana.Value)
                                    Q.Cast(minion);
                            }

                            var AllyMinions = GameObjects.GetMinions(400, MinionTypes.All, MinionTeam.Ally);
                            if (AllyMinions == null)
                            {
                                if (Player.ManaPercent >= QEzSettings.ClearMana.Value)
                                    Q.Cast(minion);
                            }
                        }
                    }
                }
            }
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!Player.IsDead && DrawEzSettings.DrawQ.Enabled)
            {
                Drawing.DrawCircle(Player.Position, Q.Range, System.Drawing.Color.Blue);
                Drawing.DrawCircle(Player.Position, Player.GetRealAutoAttackRange(), System.Drawing.Color.White);
            }
        }



        private static readonly float[] RBaseDamage = { 0f, 350f, 500f, 650f };

        public static float RDamage(AIBaseClient target)
        {
            var rLevel = R.Level;

            var rBaseDamage = RBaseDamage[rLevel]             +
                              Player.TotalAttackDamage        +
                              0.9f * Player.TotalMagicalDamage;

            return (float)Player.CalculateDamage(target, DamageType.Magical, rBaseDamage);
        }

        public static float RHitTime;
        public static float RecalStartTime;
        public static float RecalEndTime;
        public static AIBaseClient RecallTarget;
        public static bool RecalFinished = true;

        private static void Teleport_OnTeleport(AIBaseClient sender, Teleport.TeleportEventArgs args)
        {
            if (sender.IsValid && sender is AIHeroClient)
            {

                if (args.Type == Teleport.TeleportType.Recall)
                {
                    if(args.Status == Teleport.TeleportStatus.Start)
                    {
                        RecalFinished = false;
                        RecalStartTime = Variables.TickCount;
                        RecalEndTime = args.Duration;
                        RecallTarget = args.Source;
                    }

                    if(args.Status == Teleport.TeleportStatus.Abort)
                    {
                        RecalFinished = true;
                    }

                    if (args.Status == Teleport.TeleportStatus.Finish)
                    {
                        RecalFinished = true;
                    }
                }
            }
        }
    }
}
