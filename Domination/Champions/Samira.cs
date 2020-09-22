using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;
using SPredictionMash;

namespace DominationAIO.Champions
{
    internal static class SamiraSetMenu
    {
        public static class QSettings
        {
            public static MenuBool QGunCombo = new MenuBool("Q Gun Combo", "Q Gun Combo");
            public static MenuBool QBladeCombo = new MenuBool("Q Blade Combo", "Q Blade Combo");
            public static MenuSeparator QHarass = new MenuSeparator("Q_Harass", "Q Harass | Clear");
            public static MenuSlider QManaCheck = new MenuSlider("Q Mana Check", "If Mana > ", 30, 0, 100);
        }
        public static class WSettings
        {
            public static MenuBool WBlock = new MenuBool("WBlock", "W Block Attack");
            public static MenuBool WCantAA = new MenuBool("WCantAA", "W if cant AA");
            public static MenuSlider EnemyCount = new MenuSlider("Enemy Count", "Targets Count >= ", 1, 1, 5);
        }
        public static class ESettings
        {
            public static MenuBool ECombo = new MenuBool("ECombo", "E Combo");
            public static MenuBool EQ = new MenuBool("EQ", "Cast Q when E", false);
            public static MenuBool EW = new MenuBool("EW", "EW Combo");
            public static MenuBool EKs = new MenuBool("E Ks", "E Ks");
        }
        public static class RSettings
        {
            public static MenuBool RCombo = new MenuBool("R Combo", "R Combo");
            public static MenuSlider RCount = new MenuSlider("R Count", "Target Hit >= ", 1, 1, 5);
            public static MenuBool AutoE = new MenuBool("Auto E", "Auto E When Using R");
        }
        public static class Misc
        {
            public static MenuBool WaitForAA = new MenuBool("Waitting For AA", "Waiting For AA");
            public static MenuSlider AATimer = new MenuSlider("AATimer", "ÄA Timer", 1000, 0, 2000);
            public static MenuBool PacketCast = new MenuBool("PacketCast", "Packet Cast");
        }
        public static class KeysSettings
        {
            public static MenuKeyBind QMixed = new MenuKeyBind("QMixed", "Q Harass When Clear ", System.Windows.Forms.Keys.H, KeyBindType.Toggle) { Active = true, };
            public static MenuKeyBind QClear = new MenuKeyBind("QClear", "Q Clear Minions ", System.Windows.Forms.Keys.H, KeyBindType.Toggle) { Active = true, };
            public static MenuKeyBind AllowTurret = new MenuKeyBind("AllowTurret", "Allow Turret Key [ Toggle ] ", System.Windows.Forms.Keys.T, KeyBindType.Toggle);
        }
        public static void AddSamiraMenu(this Menu menu)
        {
            var QSamira = new Menu("Q Samira Settings", "Q Settings");
            QSamira.Add(QSettings.QGunCombo);
            QSamira.Add(QSettings.QBladeCombo);
            QSamira.Add(QSettings.QHarass);
            QSamira.Add(QSettings.QManaCheck);
            var WSamira = new Menu("W Samira Settings", "W Settings");
            WSamira.Add(WSettings.WBlock);
            WSamira.Add(WSettings.WCantAA);
            WSamira.Add(WSettings.EnemyCount);
            var ESamira = new Menu("E Samira Settings", "E Settings");
            ESamira.Add(ESettings.ECombo);
            ESamira.Add(ESettings.EQ);
            ESamira.Add(ESettings.EW);
            ESamira.Add(ESettings.EKs);
            var RSamira = new Menu("R Samira Settings", "R Settings");
            RSamira.Add(RSettings.RCombo);
            RSamira.Add(RSettings.RCount);
            RSamira.Add(RSettings.AutoE);
            var MiscSamira = new Menu("Misc Samira Settings", "Misc Settings");
            MiscSamira.Add(Misc.WaitForAA);
            MiscSamira.Add(Misc.AATimer);
            MiscSamira.Add(Misc.PacketCast);
            var KeysSamira = new Menu("Keys Samira Settings", "Keys Settings");
            KeysSamira.Add(KeysSettings.QMixed).Permashow();
            KeysSamira.Add(KeysSettings.QClear).Permashow();
            KeysSamira.Add(KeysSettings.AllowTurret).Permashow();

            menu.Add(QSamira);
            menu.Add(WSamira);
            menu.Add(ESamira);
            menu.Add(RSamira);
            menu.Add(MiscSamira);
            menu.Add(KeysSamira);
        }
    }
    public class Samira
    {
        private static Menu SamiraMenu = new Menu("FunnySlayer Menu", "FunnySlayer Samira", true);

        private static AIHeroClient Player => ObjectManager.Player;
        private static Spell Q, W, E, R;

        public static void SamiraLoad()
        {
            Q = new Spell(SpellSlot.Q, 900f);
            Q.SetSkillshot(0.25f, 50f, 2600, true, EnsoulSharp.SDK.Prediction.SkillshotType.Line);
            W = new Spell(SpellSlot.W, 325f);
            E = new Spell(SpellSlot.E, 600f);
            E.SetTargetted(0f, 2000);
            R = new Spell(SpellSlot.R, 600f);

            var Helper = new Menu("Helper", "Helper");
            SPredictionMash.ConfigMenu.Initialize(Helper, "Helper");
            FunnySlayerCommon.MenuClass.AddTargetSelectorMenu(Helper);
            new SebbyLibPorted.Orbwalking.Orbwalker(Helper);
            SamiraMenu.Add(Helper);

            SamiraMenu.AddSamiraMenu();
            SamiraMenu.Attach();

            Game.OnUpdate += Check;
            Game.OnUpdate += LogicCombo;
            Orbwalker.OnAction += Orbwalker_OnAction;
            AIBaseClient.OnProcessSpellCast += AIBaseClient_OnProcessSpellCast;
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;

            if(Orbwalker.ActiveMode == OrbwalkerMode.Harass && SamiraSetMenu.KeysSettings.QMixed.Active)
            {
                if(SamiraSetMenu.QSettings.QManaCheck < Player.ManaPercent)
                {
                    if (Q.IsReady())
                    {
                        var targets = GameObjects.EnemyHeroes.Where(i => !i.IsDead && i.IsValidTarget(Q.Range)).OrderBy(i => i.Health).ToArray();
                        if(targets != null)
                        {
                            foreach(var target in targets)
                            {
                                if (Q.SPredictionCast(target, EnsoulSharp.SDK.Prediction.HitChance.High, SamiraSetMenu.Misc.PacketCast.Enabled))
                                    return;
                            }
                        }
                    }
                }
            }
            else
            {
                if(Orbwalker.ActiveMode == OrbwalkerMode.LaneClear || Orbwalker.ActiveMode == OrbwalkerMode.LastHit)
                {
                    if(Orbwalker.ActiveMode == OrbwalkerMode.LaneClear && SamiraSetMenu.KeysSettings.QClear.Active)
                    {
                        var minions = GameObjects.Enemy.Where(i => !i.IsDead && i.IsValidTarget(Q.Range) && !i.Position.IsBuilding()).OrderBy(i => i.Health);
                        if(minions != null && Q.IsReady())
                        {
                            foreach(var min in minions)
                            {
                                if (SamiraSetMenu.QSettings.QManaCheck < Player.ManaPercent)
                                {
                                    if (Q.Cast(min, SamiraSetMenu.Misc.PacketCast) == CastStates.SuccessfullyCasted)
                                        return;
                                }
                            }
                        }
                    }

                    if(Orbwalker.ActiveMode == OrbwalkerMode.LastHit)
                    {
                        var minions = GameObjects.Enemy.Where(i => !i.IsDead && i.IsValidTarget(Q.Range) && !i.Position.IsBuilding() && i.Health < Player.GetSpellDamage(i, SpellSlot.Q)).OrderBy(i => i.Health);
                        if (minions != null && Q.IsReady())
                        {
                            foreach (var min in minions)
                            {
                                if (SamiraSetMenu.QSettings.QManaCheck < Player.ManaPercent)
                                {
                                    if (Q.Cast(min, SamiraSetMenu.Misc.PacketCast) == CastStates.SuccessfullyCasted)
                                        return;
                                }
                            }
                        }
                    }
                }
            }
        }

        public static bool UnderTower(SharpDX.Vector3 pos)
        {
            return
                ObjectManager.Get<AITurretClient>()
                    .Any(i => i.IsEnemy && !i.IsDead && (i.Distance(pos) < 850 + ObjectManager.Player.BoundingRadius)) || GameObjects.EnemySpawnPoints.Any(i => i.Position.Distance(pos) < 850 + ObjectManager.Player.BoundingRadius);
        }
        private static void LogicCombo(EventArgs args)
        {
            if (Player.IsDead || OnAA || BeforeAA || Orbwalker.ActiveMode != OrbwalkerMode.Combo)
                return;
            if(W.IsReady() && SamiraSetMenu.WSettings.WCantAA.Enabled && LastCasted + SamiraSetMenu.Misc.AATimer.Value < Variables.TickCount)
            {
                if (!Player.CanAttack)
                {
                    var targets = GameObjects.EnemyHeroes.Where(i => !i.IsDead && i.IsValidTarget(E.Range)).OrderBy(i => i.Health);
                    if(targets != null)
                    {
                        if (W.Cast(SamiraSetMenu.Misc.PacketCast.Enabled))
                            return;
                    }
                }
                if (E.IsReady() && SamiraSetMenu.ESettings.ECombo.Enabled)
                {
                    var targets = GameObjects.EnemyHeroes.Where(i => !i.IsDead && i.IsValidTarget(E.Range)).OrderBy(i => i.Health).ToArray();
                    if(targets != null)
                    {
                        foreach(var target in targets)
                        {
                            if (SamiraSetMenu.KeysSettings.AllowTurret.Active || !UnderTower(Player.Position.Extend(target.Position, E.Range)))
                            {
                                if (target.HealthPercent < 60)
                                {
                                    if (W.Cast())
                                    {
                                        EnsoulSharp.SDK.Utility.DelayAction.Add(700, () => { E.Cast(target, SamiraSetMenu.Misc.PacketCast.Enabled); });
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if(R.IsReady() && SamiraSetMenu.RSettings.RCombo.Enabled)
            {
                if(Player.CountEnemyHeroesInRange(R.Range) >= SamiraSetMenu.RSettings.RCount.Value)
                {
                    var targets = GameObjects.EnemyHeroes.Where(i => !i.IsDead && i.IsValidTarget(E.Range)).OrderBy(i => i.Health);
                    if (targets != null)
                    {
                        foreach (var t in targets)
                        {
                            if (SamiraSetMenu.RSettings.AutoE.Enabled)
                            {
                                if (Player.Position.Extend(t.Position, +E.Range).CountEnemyHeroesInRange(R.Range) > SamiraSetMenu.RSettings.RCount.Value)
                                {
                                    if (SamiraSetMenu.KeysSettings.AllowTurret.Active || !UnderTower(Player.Position.Extend(t.Position, E.Range)))
                                        if (E.Cast(t, SamiraSetMenu.Misc.PacketCast.Enabled) == CastStates.SuccessfullyCasted)
                                        {
                                            if (R.Cast(SamiraSetMenu.Misc.PacketCast.Enabled))
                                                return;
                                        }
                                }
                            }
                        }
                    }

                    var target = FunnySlayerCommon.FSTargetSelector.GetFSTarget(R.Range);
                    if (target != null)
                    {
                        var Pos = FSpred.Prediction.Prediction.PredictUnitPosition(target, 1000);
                        if(Pos.DistanceToPlayer() > R.Range)
                        {
                            if(E.IsReady() && SamiraSetMenu.RSettings.AutoE.Enabled)
                            {
                                if (SamiraSetMenu.KeysSettings.AllowTurret.Active || !UnderTower(Player.Position.Extend(target.Position, E.Range)))
                                {
                                    if(E.Cast(target, SamiraSetMenu.Misc.PacketCast) == CastStates.SuccessfullyCasted)
                                    {
                                        if (R.Cast(SamiraSetMenu.Misc.PacketCast.Enabled))
                                            return;
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (R.Cast(SamiraSetMenu.Misc.PacketCast.Enabled))
                                return;
                        }
                    }
                }
            }
            if (E.IsReady() && SamiraSetMenu.ESettings.ECombo.Enabled)
            {
                var targets = GameObjects.EnemyHeroes.Where(i => !i.IsDead && i.IsValidTarget(E.Range)).OrderBy(i => i.Health);
                if(targets != null)
                {
                    foreach(var target in targets)
                    {
                        if(FSpred.Prediction.Prediction.PredictUnitPosition(target, 700).DistanceToPlayer() > E.Range)
                        {
                            if (SamiraSetMenu.KeysSettings.AllowTurret.Active || !UnderTower(Player.Position.Extend(target.Position, E.Range)))
                                if (E.Cast(target, SamiraSetMenu.Misc.PacketCast.Enabled) == CastStates.SuccessfullyCasted)
                                {
                                    return;
                                }
                        }
                        if(target.Health < GetEDmg(target) && SamiraSetMenu.ESettings.EKs.Enabled)
                        {
                            if (SamiraSetMenu.KeysSettings.AllowTurret.Active || !UnderTower(Player.Position.Extend(target.Position, E.Range)))
                                if (E.Cast(target, SamiraSetMenu.Misc.PacketCast.Enabled) == CastStates.SuccessfullyCasted)
                                {
                                    return;
                                }
                        }
                        if (Player.HasBuff("SamiraR") && SamiraSetMenu.RSettings.AutoE.Enabled)
                        {
                            if(Player.Position.Extend(target.Position, + E.Range).CountEnemyHeroesInRange(R.Range) > Player.CountEnemyHeroesInRange(R.Range))
                            {
                                if (SamiraSetMenu.KeysSettings.AllowTurret.Active || !UnderTower(Player.Position.Extend(target.Position, E.Range)))
                                    if (E.Cast(target, SamiraSetMenu.Misc.PacketCast.Enabled) == CastStates.SuccessfullyCasted)
                                    {
                                        return;
                                    }
                            }
                            if (FunnySlayerCommon.FSTargetSelector.GetFSTarget(R.Range + 300) != null)
                            {
                                var Pos = FSpred.Prediction.Prediction.PredictUnitPosition(FunnySlayerCommon.FSTargetSelector.GetFSTarget(R.Range + 300), 700);
                                var enemy = GameObjects.Enemy.Where(i => !i.IsDead && i.IsValidTarget(E.Range) && !i.Position.IsBuilding()).OrderBy(i => i.Health).OrderBy(i => i is AIHeroClient);
                                if (enemy != null)
                                {
                                    foreach (var Next in enemy)
                                    {
                                        if (SamiraSetMenu.KeysSettings.AllowTurret.Active || !UnderTower(Player.Position.Extend(Next.Position, E.Range)))
                                        {
                                            if (Player.Position.Extend(Next.Position, E.Range).Distance(Pos) < Player.Distance(Pos))
                                            {
                                                if (E.Cast(Next, SamiraSetMenu.Misc.PacketCast.Enabled) == CastStates.SuccessfullyCasted)
                                                    return;
                                            }
                                        }   
                                    }
                                }
                            }                         
                        }
                        if (Player.HasBuff("SamiraW") && SamiraSetMenu.ESettings.EW.Enabled)
                        {
                            if(Variables.TickCount > LastW + 600)
                            {
                                if (SamiraSetMenu.KeysSettings.AllowTurret.Active || !UnderTower(Player.Position.Extend(target.Position, E.Range)))
                                    if (E.Cast(target, SamiraSetMenu.Misc.PacketCast.Enabled) == CastStates.SuccessfullyCasted)
                                    {
                                        return;
                                    }
                            }
                        }
                        if (Q.IsReady() && SamiraSetMenu.ESettings.EQ.Enabled)
                        {
                            if (SamiraSetMenu.KeysSettings.AllowTurret.Active || !UnderTower(Player.Position.Extend(target.Position, E.Range)))
                                if (E.Cast(target, SamiraSetMenu.Misc.PacketCast.Enabled) == CastStates.SuccessfullyCasted)
                                {
                                    if (Q.Cast(new SharpDX.Vector3(45646, 546416, 45462), SamiraSetMenu.Misc.PacketCast.Enabled))
                                        return;
                                    return;
                                }
                        }
                    }
                }
            }

            if (Q.IsReady())
            {
                if(SamiraSetMenu.QSettings.QGunCombo.Enabled || SamiraSetMenu.QSettings.QBladeCombo.Enabled)
                {
                    var target = FunnySlayerCommon.FSTargetSelector.GetFSTarget(Q.Range);
                    if(target != null)
                    {
                        var pred = SebbyLibPorted.Prediction.Prediction.GetPrediction(Q, target);
                        if ((Player.IsDashing() || LastE + 700 > Variables.TickCount) && SamiraSetMenu.ESettings.EQ.Enabled)
                        {

                        }
                        if ((!Player.IsDashing() && LastE + 700 < Variables.TickCount))
                        {
                            if(pred.CastPosition.DistanceToPlayer() > Q.Range / 3)
                            {
                                if (SamiraSetMenu.QSettings.QGunCombo.Enabled)
                                {
                                    if (LastCasted + SamiraSetMenu.Misc.AATimer.Value > Variables.TickCount)
                                        return;
                                    else
                                    if (Q.SPredictionCast(target, EnsoulSharp.SDK.Prediction.HitChance.High, SamiraSetMenu.Misc.PacketCast.Enabled))
                                        return;
                                }
                                else
                                {
                                    var targets = GameObjects.EnemyHeroes.Where(i => !i.IsDead && i.IsValidTarget(Q.Range / 3)).OrderBy(i => i.Health);
                                    if (targets != null)
                                    {
                                        foreach(var t in targets)
                                        {
                                            if (LastCasted + SamiraSetMenu.Misc.AATimer.Value > Variables.TickCount)
                                                return;
                                            else
                                            if (Q.Cast(t.Position, SamiraSetMenu.Misc.PacketCast.Enabled))
                                                return;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (!SamiraSetMenu.QSettings.QBladeCombo.Enabled)
                                {
                                    if(pred.Hitchance != SebbyLibPorted.Prediction.HitChance.Collision)
                                    {
                                        if (LastCasted + SamiraSetMenu.Misc.AATimer.Value > Variables.TickCount)
                                            return;
                                        else
                                        if (Q.Cast(Player.Position.Extend(pred.CastPosition, Q.Range), SamiraSetMenu.Misc.PacketCast.Enabled))
                                            return;
                                    }
                                }
                                else
                                {
                                    if (LastCasted + SamiraSetMenu.Misc.AATimer.Value > Variables.TickCount)
                                        return;
                                    else
                                    if (Q.Cast(pred.CastPosition, SamiraSetMenu.Misc.PacketCast.Enabled))
                                        return;
                                }
                            }
                        }
                    }
                 
                }
            }
        }
        private static float GetEDmg(AIBaseClient target)
        {
            var Elist = new List<int>{
                0, 50 , 60 , 70 , 80 , 90
            };
            var EGetDmg = Player.CalculateMagicDamage(target, Elist[E.Level]);
            return (float)EGetDmg;
        }
        private static int LastCasted = 0;
        private static int LastE = 0;
        private static int LastW = 0;
        private static int LastAttack = 0;
        private static void AIBaseClient_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (sender == null || !sender.IsValid() || !(sender is AIHeroClient) || args == null)
                return;

            if (sender.IsMe)
            {
                if(args.Slot <= SpellSlot.R && args.Slot != SpellSlot.Unknown)
                {
                    LastCasted = Variables.TickCount;
                }
                if(args.Slot == SpellSlot.E)
                {
                    LastE = Variables.TickCount;
                }
                if (args.Slot == SpellSlot.W)
                {
                    LastW = Variables.TickCount;
                }
                if (Orbwalker.IsAutoAttack(args.SData.Name) && args.Target is AIHeroClient)
                {
                    LastCasted = 0;
                    LastAttack = Variables.TickCount;
                }
            }
            else
            {
                if (args.Target != null && args.Target.IsMe && sender != null && sender.IsRanged && args.Slot >= SpellSlot.R)
                {
                    if(TargetSelector.GetTargets(W.Range + E.Range).Count() >= SamiraSetMenu.WSettings.EnemyCount.Value)
                    {
                        if (SamiraSetMenu.WSettings.WBlock.Enabled && Orbwalker.ActiveMode == OrbwalkerMode.Combo)
                        {
                            if (W.Cast())
                            {
                                return;
                            }
                        }
                    }                   
                }
            }
        }

        private static bool AfterAA = false;
        private static bool BeforeAA = false;
        private static bool OnAA = false;
        private static void Orbwalker_OnAction(object sender, OrbwalkerActionArgs args)
        {
            if(args.Type == OrbwalkerType.AfterAttack)
            {
                AfterAA = true;
                OnAA = false;
                BeforeAA = false;
                LastCasted = 0;
            }
            else
            {
                AfterAA = false;
            }

            if (args.Type == OrbwalkerType.OnAttack)
            {
                AfterAA = false;
                OnAA = true;
                BeforeAA = false;
            }
            else
            {
                OnAA = false;
            }

            if (args.Type == OrbwalkerType.BeforeAttack)
            {
                AfterAA = false;
                OnAA = false;
                BeforeAA = true;
            }
            else
            {
                BeforeAA = false;
            }
        }

        private static void Check(EventArgs args)
        {
            if (Player.IsDead)
                return;

            if (Player.HasBuff("SamiraR"))
                Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            if (Player.HasBuff("SamiraW"))
                Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
        }
    }
}
