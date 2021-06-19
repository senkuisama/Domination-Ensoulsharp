using EnsoulSharp;

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
using FunnySlayerCommon;

namespace DominationAIO.Champions
{
    #region EzrealMenu
    public class EzKeysSetting
    {
        public static readonly MenuKeyBind SemiRKey = new MenuKeyBind("SemiRKey", "R Using Key [Type : Press]", Keys.T, KeyBindType.Press);
        public static readonly MenuKeyBind FarmKey = new MenuKeyBind("FarmKey", "Spell Farming [Type : Toggle]", Keys.A, KeyBindType.Toggle);
        public static readonly MenuKeyBind HarassKey = new MenuKeyBind("HarassKey", "Harass When Farm [Type : Toggle]", Keys.A, KeyBindType.Toggle);
        public static readonly MenuKeyBind Stacks = new MenuKeyBind("Stacks", "Stacks Q [Type : Toggle]", Keys.M, KeyBindType.Toggle);
        public static readonly MenuKeyBind Flee = new MenuKeyBind("Flee", "Flee [Type : Press]", Keys.Z, KeyBindType.Press);
    }
    public class QEzSettings
    {
        public static readonly MenuBool Qcombo = new MenuBool("Qcombo", "Q Combo");
        public static readonly MenuBool QinAA = new MenuBool("QinAA", "Use Q when player on Auto Attack", false);
        public static readonly MenuBool Qharass = new MenuBool("Qharass", "Q Harass");
        public static readonly MenuSlider HarassMana = new MenuSlider("HarassMana", "(Harass) Min Mana : ", 60);
        public static readonly MenuBool QClear = new MenuBool("QClear", "Q Clear");
        public static readonly MenuSlider ClearMana = new MenuSlider("ClearMana", "(Clear) Min Mana : ", 40);
        public static readonly MenuBool LastQOnly = new MenuBool("QLastHitOnly", "Only Q last hit", false);
    }

    public class WEzSettings
    {
        public static readonly MenuBool Wcombo = new MenuBool("Wcombo", "W Combo");
        public static readonly MenuBool Wonly = new MenuBool("W only", "Only W if can Q or AA");
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
        public static MenuBool PacketCast = new MenuBool("Packet Cast", "Packet Cast");
        public static AIHeroClient Player = ObjectManager.Player;
        public static Menu EzrealMenu;
        public static Menu EzQmenu, EzWmenu, EzEmenu, EzRmenu, EzSpredictionmenu;
        public static Menu SebbyLibMenuAttack;
        public static Spell Q, W, E, R;
      
        public static void Ezreal_Load()
        {
            Game.Print("SPrediction Ezreal");

            EzQmenu = new Menu("Qmenu", "Q Settings");
            EzWmenu = new Menu("Wmenu", "W Settings");
            EzEmenu = new Menu("Emenu", "E Settings");
            EzRmenu = new Menu("Rmenu", "R Settings");

            EzSpredictionmenu = new Menu("EzSpredictionmenu", "(Sprediction)");
            SebbyLibMenuAttack = new Menu("OrbWalking@@", "OrbWalking @@");
            //new SebbyLibPorted.Orbwalking.Orbwalker(SebbyLibMenuAttack);
            EzSpredictionmenu.Add(SebbyLibMenuAttack);
            SPredictionMash.Prediction.Initialize(EzSpredictionmenu);

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
            EzQmenu.Add(QEzSettings.LastQOnly);
            EzQmenu.Add(QEzSettings.ClearMana);

            EzWmenu.Add(WEzSettings.Wcombo);
            EzWmenu.Add(WEzSettings.Wonly);
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
            //var TargetFS = new Menu("Target FS", "Target");
            //TargetFS.AddTargetSelectorMenu();
            //EzrealMenu.Add(TargetFS);
            EzrealMenu.Add(keys);
            EzrealMenu.Add(EzQmenu);
            EzrealMenu.Add(EzWmenu);
            EzrealMenu.Add(EzEmenu);
            EzrealMenu.Add(EzRmenu);

            EzrealMenu.Add(EzSpredictionmenu);
            EzrealMenu.Add(DrawEzSettings.DrawQ);
            EzrealMenu.Add(PacketCast);
            EzrealMenu.PermasShowText = "(Sprediction) Ezreal";
            EzrealMenu.Attach();
            

            Drawing.OnDraw += Drawing_OnDraw;


            Q = new Spell(SpellSlot.Q, 1150f);
            Q.SetSkillshot(0.25f, 50f, 2000f, true, SpellType.Line);
            W = new Spell(SpellSlot.W, 1150f);
            W.SetSkillshot(0.3f, 60f, 1200f, false, SpellType.Line);
            E = new Spell(SpellSlot.E, 475f) { Delay = 0.65f };
            R = new Spell(SpellSlot.R, 20000f);
            R.SetSkillshot(1.10f, 160f, 2000f, false, SpellType.Line);

            Game.OnUpdate += Game_OnUpdate;
            //Orbwalker.OnAction += Orbwalker_OnAction;
            AIHeroClient.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
            //Teleport.OnTeleport += Teleport_OnTeleport;
            //Teleport.OnTeleport += Teleport_OnTeleport1;
        }

        private static void Teleport_OnTeleport1(AttackableUnit sender, Teleport.TeleportEventArgs args)
        {
            if (sender.IsValid && sender is AIHeroClient)
            {

                if (args.Type == Teleport.TeleportType.Recall)
                {
                    if (args.Status == Teleport.TeleportStatus.Start)
                    {
                        RecalFinished = false;
                        RecalStartTime = Variables.TickCount;
                        RecalEndTime = args.Duration;
                        RecallTarget = args.Source;
                    }

                    if (args.Status == Teleport.TeleportStatus.Abort)
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

        private static void AIHeroClient_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (Orbwalker.IsAutoAttack(args.SData.Name) && !Orbwalker.IsAutoAttackReset(args.SData.Name))
                {
                    LastAfterAA = Environment.TickCount;
                }
            }
        }
        private static int LastAfterAA = 0;
        /*private static bool BeforeAA, OnAA, AfterAA;
        
        private static void Orbwalker_OnAction(object sender, OrbwalkerActionArgs args)
        {
            if(args.Type == OrbwalkerType.OnAttack)
            {
                OnAA = true;
                BeforeAA = false;
                AfterAA = false;
            }
            else { OnAA = false; }

            if (args.Type == OrbwalkerType.BeforeAttack)
            {
                BeforeAA = true;
                OnAA = false;
                AfterAA = false;
            }
            else { BeforeAA = false; }

            if (args.Type == OrbwalkerType.AfterAttack)
            {
                AfterAA = true;
                OnAA = false;
                BeforeAA = false;
            }
            else { AfterAA = false; }
        }*/
        private static void Game_OnUpdate(EventArgs args)
        {
            if (Player.IsDead) return;

            //BU();
            if(Orbwalker.ActiveMode == OrbwalkerMode.Combo)
            {
                EzCombo();
            }
            if (Orbwalker.ActiveMode == OrbwalkerMode.Harass)
            {
                EzHarass();
            }
            if (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear)
            {
                EzClear();
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
            if (Player.IsDashing())
                return;

            if (Variables.GameTimeTickCount - OnAction.LastAttack < ObjectManager.Player.AttackCastDelay * 1000 || FunnySlayerCommon.OnAction.OnAA || FunnySlayerCommon.OnAction.BeforeAA)
                return;

            {
                var targets = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget() 
                && i.DistanceToPlayer() <= W.Range && !i.IsDead
                && FSpred.Prediction.Prediction.GetPrediction(W, i).Hitchance >= FSpred.Prediction.HitChance.High)
                    .OrderBy(i => i.Health);

                if(WEzSettings.Wcombo.Enabled && W.IsReady() && targets.Count() >= 1)
                {
                    var target = targets.FirstOrDefault();
                    var pred = FSpred.Prediction.Prediction.GetPrediction(W, target);
                    if(pred.Hitchance >= FSpred.Prediction.HitChance.High)
                    {
                        if (WEzSettings.Wonly.Enabled)
                        {
                            if (target.DistanceToPlayer() < Player.GetRealAutoAttackRange() - 60)
                            {
                                if (Environment.TickCount - LastAfterAA >= (Player.AttackDelay - target.DistanceToPlayer() / W.Speed) * 1000 - W.Delay * 100 || Orbwalker.CanAttack())
                                {
                                    W.Cast(pred.CastPosition);
                                    return;
                                }
                            }

                            if (Q.IsReady())
                            {
                                var qpred = FSpred.Prediction.Prediction.GetPrediction(Q, target);

                                if(qpred.Hitchance >= FSpred.Prediction.HitChance.High)
                                {
                                    if (W.Cast(pred.CastPosition))
                                    {
                                        Q.Cast(qpred.CastPosition);
                                            return;
                                    }
                                    return;
                                }
                            }
                        }
                        else
                        {

                        }
                    }
                }
            }

            {
                var targets = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget()
                && i.DistanceToPlayer() <= Q.Range && !i.IsDead
                && FSpred.Prediction.Prediction.GetPrediction(Q, i).Hitchance >= FSpred.Prediction.HitChance.High)
                    .OrderBy(i => i.Health);

                if (QEzSettings.Qcombo.Enabled && Q.IsReady() && targets.Count() >= 1)
                {
                    var target = targets.FirstOrDefault();
                    var pred = FSpred.Prediction.Prediction.GetPrediction(Q, target);
                    if (pred.Hitchance >= FSpred.Prediction.HitChance.High)
                    {
                        Q.Cast(pred.CastPosition);
                        return;
                    }
                }
            }

            if (EEzSettings.Ecombo.Enabled && E.IsReady())
                EzECombo();

            /*var targetss = ObjectManager.Get<AIHeroClient>().Where(i => 
            i != null
            && !i.IsDead
            && !i.IsAlly            
            && SebbyLibPorted.Prediction.Prediction.GetPrediction(W, i).Hitchance >= SebbyLibPorted.Prediction.HitChance.High).OrderBy(i => i.Health);
            if (W.IsReady() && targetss != null && targetss.Any())
            {
                foreach(var target in targetss)
                {
                    if(target != null)
                    {
                        var Qpred = SebbyLibPorted.Prediction.Prediction.GetPrediction(Q, target);
                        if(Q.IsReady() && Qpred.Hitchance >= SebbyLibPorted.Prediction.HitChance.High)
                        {
                            EzQCombo();
                        }
                        else
                        {
                            EzWCombo();
                        }
                    }
                }
            }
            else
            {
                EzQCombo();

                
            }    */
        }

        private static void EzQCombo()
        {
            if (Player.IsDashing())
                return;

            if (FunnySlayerCommon.OnAction.OnAA || FunnySlayerCommon.OnAction.BeforeAA)
                return;

            {
                var targets = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget()
                && i.DistanceToPlayer() <= W.Range && !i.IsDead
                && FSpred.Prediction.Prediction.GetPrediction(W, i).Hitchance >= FSpred.Prediction.HitChance.High)
                    .OrderBy(i => i.Health);

                if (WEzSettings.Wcombo.Enabled && W.IsReady() && targets.Count() >= 1)
                {
                    var target = targets.FirstOrDefault();
                    var pred = FSpred.Prediction.Prediction.GetPrediction(W, target);
                    if (pred.Hitchance >= FSpred.Prediction.HitChance.High)
                    {
                        if (WEzSettings.Wonly.Enabled)
                        {
                            if (target.DistanceToPlayer() < Player.GetRealAutoAttackRange() - 60)
                            {
                                if (Environment.TickCount - LastAfterAA >= (Player.AttackDelay - target.DistanceToPlayer() / W.Speed) * 1000 - W.Delay * 100 || Orbwalker.CanAttack())
                                {
                                    W.Cast(pred.CastPosition);
                                    return;
                                }
                            }

                            if (Q.IsReady())
                            {
                                var qpred = FSpred.Prediction.Prediction.GetPrediction(Q, target);

                                if (qpred.Hitchance >= FSpred.Prediction.HitChance.High)
                                {
                                    if (W.Cast(pred.CastPosition))
                                    {
                                        Q.Cast(qpred.CastPosition);
                                        return;
                                    }
                                    return;
                                }
                            }
                        }
                        else
                        {

                        }
                    }
                }
            }

            {
                var targets = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget()
                && i.DistanceToPlayer() <= Q.Range && !i.IsDead
                && FSpred.Prediction.Prediction.GetPrediction(Q, i).Hitchance >= FSpred.Prediction.HitChance.High)
                    .OrderBy(i => i.Health);

                if (QEzSettings.Qcombo.Enabled && Q.IsReady() && targets.Count() >= 1)
                {
                    var target = targets.FirstOrDefault();
                    var pred = FSpred.Prediction.Prediction.GetPrediction(Q, target);
                    if (pred.Hitchance >= FSpred.Prediction.HitChance.High)
                    {
                        Q.Cast(pred.CastPosition);
                        return;
                    }
                }
            }
        }

        private static void EzWCombo()
        {
            var target = FSTargetSelector.GetFSTarget(W.Range);

            if (target == null) return;

            if (FunnySlayerCommon.OnAction.OnAA || FunnySlayerCommon.OnAction.BeforeAA) return;

            if (!WEzSettings.Wcombo.Enabled || !W.IsReady()) return;

            if (WEzSettings.Wonly.Enabled)
            {
                if(target.DistanceToPlayer() < Player.GetRealAutoAttackRange() - 60)
                {
                    if(Environment.TickCount - LastAfterAA >= (Player.AttackDelay - target.DistanceToPlayer() / W.Speed) * 1000 - W.Delay * 100 || Orbwalker.CanAttack())
                    {
                        if (W.SPredictionCast(target, EnsoulSharp.SDK.HitChance.High))
                        {
                            if (Q.IsReady())
                            {
                                var Fspred = FSpred.Prediction.Prediction.GetPrediction(Q, target);

                                if (Fspred.Hitchance >= FSpred.Prediction.HitChance.High)
                                {
                                    if (Q.SPredictionCast(target, EnsoulSharp.SDK.HitChance.High))
                                    {
                                        return;
                                    }
                                }
                            }
                            return;
                        }
                    }
                }

                if (Q.IsReady())
                {
                    var Fspred = FSpred.Prediction.Prediction.GetPrediction(Q, target);

                    if (Fspred.Hitchance >= FSpred.Prediction.HitChance.High)
                    {
                        if (W.SPredictionCast(target, EnsoulSharp.SDK.HitChance.High))
                        {
                            if (Q.SPredictionCast(target, EnsoulSharp.SDK.HitChance.High))
                            {
                                return;
                            }
                        }
                    }
                }               
            }
            else
            {
                if (W.SPredictionCast(target, EnsoulSharp.SDK.HitChance.High))
                {
                    var Fspred = FSpred.Prediction.Prediction.GetPrediction(Q, target);

                    if (Q.IsReady() && QEzSettings.Qcombo.Enabled)
                        if (Fspred.Hitchance >= FSpred.Prediction.HitChance.High)
                        {
                            if (Q.SPredictionCast(target, EnsoulSharp.SDK.HitChance.High))
                            {
                                return;
                            }
                        }
                    return;
                }
            }
        }

        private static void EzECombo()
        {
            var target = TargetSelector.GetTargets(Q.Range + E.Range, DamageType.Physical).OrderBy(i => i.Health).FirstOrDefault();

            if (target == null || !(target is AIHeroClient)) return;

            if (target.HealthPercent > EEzSettings.TargetHeath.Value + 1) return;

            if (FunnySlayerCommon.OnAction.OnAA || OnAction.BeforeAA) return;

            EnsoulSharp.SDK.Geometry.Circle EPoints = new EnsoulSharp.SDK.Geometry.Circle(Player.Position, E.Range);
            var Echeck = new Spell(SpellSlot.Unknown, 1000f);
            Echeck.SetSkillshot(0.3f, 80f, 2000f, true, SpellType.Line);
            foreach (var EPoint in EPoints.Points.OrderBy(i => i.Distance(target)))
            {
                if (Yasuo_LogicHelper.Logichelper.UnderTower(EPoint.ToVector3()))
                    return;

                Echeck.UpdateSourcePosition(EPoint.ToVector3(), EPoint.ToVector3());

                if (FSpred.Prediction.Prediction.GetPrediction(Echeck, target).Hitchance >= FSpred.Prediction.HitChance.High && target.Distance(EPoint) < Q.Range)
                {
                    if (EPoint.CountEnemyHeroesInRange(Player.GetRealAutoAttackRange() + 200) <= EEzSettings.TargetCount.Value)
                    {
                        E.Cast(EPoint);
                            return;
                    }
                }
            }
        }

        private static void EzHarass()
        {
            if (!EzKeysSetting.HarassKey.Active)
                return;

            var target = TargetSelector.SelectedTarget;

            if (target == null || !target.IsValidTarget(Q.Range))
            {
                target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
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
            if (FunnySlayerCommon.OnAction.OnAA || FunnySlayerCommon.OnAction.BeforeAA || !QEzSettings.QClear.Enabled) return;

            if (!EzKeysSetting.FarmKey.Active)
                return;

            var Minions = ObjectManager.Get<AIBaseClient>().Where(i => !i.IsAlly && (i.Type == GameObjectType.AIHeroClient || i.Type == GameObjectType.AIMinionClient) && i.IsValidTarget(Q.Range) && !i.IsAlly && !i.IsDead && !i.Position.IsBuilding()).OrderByDescending(i => i.Health);
            if (Minions == null)
                return;

            foreach(var min in Minions)
            {
                if (min.IsMinion())
                {
                    if (!EzKeysSetting.FarmKey.Active)
                        return;

                    if (QEzSettings.ClearMana.Value <= Player.ManaPercent)
                    {
                        if(min.Health >= Player.GetAutoAttackDamage(min) + 17 + Damage.GetSpellDamage(Player, min, SpellSlot.Q) || min.Health < Damage.GetSpellDamage(Player, min, SpellSlot.Q))
                        {
                            if (QEzSettings.LastQOnly.Enabled)
                            {
                                if(min.Health < Damage.GetSpellDamage(Player, min, SpellSlot.Q))
                                {
                                    Q.Cast(min);
                                }
                                else
                                {
                                    var target = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(Q.Range) && !i.IsDead && SebbyLibPorted.Prediction.Prediction.GetPrediction(Q, i).Hitchance >= SebbyLibPorted.Prediction.HitChance.High).OrderBy(i => i.Health).FirstOrDefault();
                                    if (target != null)
                                    {
                                        var pred = FSpred.Prediction.Prediction.GetPrediction(Q, target);
                                        if(pred.Hitchance >= FSpred.Prediction.HitChance.High)
                                        {
                                            Q.Cast(pred.CastPosition);
                                            return;
                                        }
                                    }                                       
                                }
                            }
                            else
                            {
                                Q.Cast(min);
                            }
                        }
                        else
                        {
                            var target = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(Q.Range) && !i.IsDead && SebbyLibPorted.Prediction.Prediction.GetPrediction(Q, i).Hitchance >= SebbyLibPorted.Prediction.HitChance.High).OrderBy(i => i.Health).FirstOrDefault();
                            if (target != null)
                            {
                                var pred = FSpred.Prediction.Prediction.GetPrediction(Q, target);
                                if (pred.Hitchance >= FSpred.Prediction.HitChance.High)
                                {
                                    Q.Cast(pred.CastPosition);
                                    return;
                                }
                            }                               
                        }
                    }
                }
                else
                {
                    if (!EzKeysSetting.HarassKey.Active)
                        return;

                    if (QEzSettings.Qharass.Enabled)
                    {
                        if(QEzSettings.HarassMana.Value <= Player.ManaPercent)
                        {
                            var target = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(Q.Range) && !i.IsDead && SebbyLibPorted.Prediction.Prediction.GetPrediction(Q, i).Hitchance >= SebbyLibPorted.Prediction.HitChance.High).OrderBy(i => i.Health).FirstOrDefault();
                            if (target != null)
                            {
                                var pred = FSpred.Prediction.Prediction.GetPrediction(Q, target);
                                if (pred.Hitchance >= FSpred.Prediction.HitChance.High)
                                {
                                    Q.Cast(pred.CastPosition);
                                    return;
                                }
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
                //Render.Circle.DrawCircle(Player.Position, Player.GetRealAutoAttackRange(), System.Drawing.Color.White);
                /*if(FSTargetSelector.GetFSTarget(Q.Range) != null)
                {
                    Render.Circle.DrawCircle(FSpred.Prediction.Prediction.GetPrediction(Q, FSTargetSelector.GetFSTarget(2000)).CastPosition, 20, System.Drawing.Color.White);
                }*/
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
