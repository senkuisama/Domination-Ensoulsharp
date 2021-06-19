using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;

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
            public static MenuBool WonlyBlockIncombo = new MenuBool("WonlyBlockIncombo", "Block AA in Combo only");
            public static MenuBool WCantAA = new MenuBool("WCantAA", "W if cant AA");
            public static MenuSlider EnemyCount = new MenuSlider("Enemy Count", "Targets Count >= ", 1, 1, 5);
        }
        public static class ESettings
        {
            public static MenuBool ECombo = new MenuBool("ECombo", "E Combo");
            public static MenuBool EQ = new MenuBool("EQ", "Cast Q when E", false);
            public static MenuBool EW = new MenuBool("EW", "EW Combo");
            public static MenuKeyBind EMinions = new MenuKeyBind("EMinions", "Accept E on Minion", Keys.A, KeyBindType.Toggle);
            public static MenuSeparator Eonly = new MenuSeparator("Eonly", "But Only When");
            public static MenuSlider Eheath = new MenuSlider("E Heath", "Target Heath % <= ", 50, 0, 100);
            public static MenuBool ER = new MenuBool("E R", "R Is In Ready");
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
            public static MenuSlider AATimer = new MenuSlider("AATimer", "ÄA Timer", 1500, 0, 2500);
            public static MenuBool PacketCast = new MenuBool("PacketCast", "Packet Cast");
            public static MenuBool DrawQAARange = new MenuBool("DrawQAA Range", "Draw AA & Q Range");
        }
        public static class KeysSettings
        {
            public static MenuKeyBind QMixed = new MenuKeyBind("QMixed", "Q Harass When Clear ", Keys.H, KeyBindType.Toggle) { Active = true, };
            public static MenuKeyBind QClear = new MenuKeyBind("QClear", "Q Clear Minions ", Keys.H, KeyBindType.Toggle) { Active = true, };
            public static MenuKeyBind AllowTurret = new MenuKeyBind("AllowTurret", "Allow Turret Key [ Toggle ] ", Keys.T, KeyBindType.Toggle);
            public static MenuKeyBind TurboFast = new MenuKeyBind("TurboFast", "Turbo Fastly", Keys.Z, KeyBindType.Toggle);
        }
        public static MenuBool reset = new MenuBool("reset", "Reset Samira");
        public static void AddSamiraMenu(this Menu menu)
        {
            var QSamira = new Menu("Q Samira Settings", "Q Settings");
            QSamira.Add(QSettings.QGunCombo);
            QSamira.Add(QSettings.QBladeCombo);
            QSamira.Add(QSettings.QHarass);
            QSamira.Add(QSettings.QManaCheck);
            var WSamira = new Menu("W Samira Settings", "W Settings");
            WSamira.Add(WSettings.WBlock);
            WSamira.Add(WSettings.WonlyBlockIncombo);
            WSamira.Add(WSettings.WCantAA);
            WSamira.Add(WSettings.EnemyCount);
            var ESamira = new Menu("E Samira Settings", "E Settings");
            ESamira.Add(ESettings.ECombo);
            ESamira.Add(ESettings.EQ);
            ESamira.Add(ESettings.EW);
            ESamira.Add(ESettings.EMinions).Permashow();
            ESamira.Add(ESettings.Eonly);
            ESamira.Add(ESettings.Eheath);
            ESamira.Add(ESettings.ER);
            ESamira.Add(ESettings.EKs);
            var RSamira = new Menu("R Samira Settings", "R Settings");
            RSamira.Add(RSettings.RCombo);
            RSamira.Add(RSettings.RCount);
            RSamira.Add(RSettings.AutoE);
            var MiscSamira = new Menu("Misc Samira Settings", "Misc Settings");
            MiscSamira.Add(Misc.WaitForAA);
            MiscSamira.Add(Misc.AATimer);
            MiscSamira.Add(Misc.PacketCast);
            MiscSamira.Add(Misc.DrawQAARange);
            var KeysSamira = new Menu("Keys Samira Settings", "Keys Settings");
            KeysSamira.Add(KeysSettings.QMixed).Permashow();
            KeysSamira.Add(KeysSettings.QClear).Permashow();
            KeysSamira.Add(KeysSettings.AllowTurret).Permashow();
            KeysSamira.Add(KeysSettings.TurboFast).Permashow();

            menu.Add(QSamira);
            menu.Add(WSamira);
            menu.Add(ESamira);
            menu.Add(RSamira);
            menu.Add(MiscSamira);
            menu.Add(KeysSamira);
            menu.Add(reset);

            Game.OnUpdate += (a) =>
            {
                if (reset.Enabled)
                {
                    //Q
                    QSettings.QBladeCombo.Enabled = true;
                    QSettings.QBladeCombo.Enabled = true;
                    QSettings.QManaCheck.Value = 30;
                    //W
                    WSettings.WCantAA.Enabled = false;
                    WSettings.WBlock.Enabled = true;
                    WSettings.EnemyCount.Value = 1;
                    //E
                    ESettings.ECombo.Enabled = true;
                    ESettings.EQ.Enabled = false;
                    ESettings.EW.Enabled = true;
                    ESettings.EMinions.Active = false;
                    ESettings.Eheath.Value = 70;
                    ESettings.ER.Enabled = true;
                    ESettings.EKs.Enabled = true;
                    //ESettings.Eonly.Permashow(true);
                    //R
                    RSettings.RCombo.Enabled = true;
                    RSettings.RCount.Value = 1;
                    RSettings.AutoE.Enabled = true;
                    //Misc
                    Misc.WaitForAA.Enabled = true;
                    Misc.AATimer.Value = 1500;
                    Misc.PacketCast.Enabled = false;
                    Misc.DrawQAARange.Enabled = false;

                    reset.Enabled = false;
                }
            };
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
            Q.SetSkillshot(0.25f, 50f, 2600, true, SpellType.Line);
            W = new Spell(SpellSlot.W, 325f);
            E = new Spell(SpellSlot.E, 600f);
            E.SetTargetted(0f, 2000);
            R = new Spell(SpellSlot.R, 600f);

            var Helper = new Menu("Helper", "Helper");
            SPredictionMash.ConfigMenu.Initialize(Helper, "Helper");
            FunnySlayerCommon.MenuClass.AddTargetSelectorMenu(Helper);
            //new SebbyLibPorted.Orbwalking.Orbwalker(Helper);
            SamiraMenu.Add(Helper);

            SamiraMenu.AddSamiraMenu();
            SamiraMenu.Attach();

            Game.OnUpdate += Check;
            Game.OnUpdate += LogicCombo;
            //Orbwalker.OnAction += Orbwalker_OnAction;
            Orbwalker.OnAfterAttack += Orbwalker_OnAfterAttack;
            AIBaseClient.OnProcessSpellCast += AIBaseClient_OnProcessSpellCast;
            Game.OnUpdate += Game_OnUpdate;
            Game.OnUpdate += JungleClear;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Orbwalker_OnAfterAttack(object sender, AfterAttackEventArgs e)
        {
            LastCasted = 0;
        }

        private static void JungleClear(EventArgs args)
        {
            if (Player.IsDead)
                return;

            if (Orbwalker.ActiveMode != OrbwalkerMode.LaneClear)
                return;

            var minions = GameObjects.Jungle.Where(i => i != null &&!i.IsDead && i.IsValidTarget(Q.Range));
            if(minions != null)
            {
                foreach(var min in minions)
                {
                    if (Q.IsReady() && SamiraSetMenu.QSettings.QManaCheck.Value < Player.ManaPercent)
                    {
                        Q.Cast(min.Position);
                    }
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!Player.IsDead)
            {
                if (SamiraSetMenu.Misc.DrawQAARange.Enabled)
                {
                    Drawing.DrawCircle(Player.Position, Player.GetRealAutoAttackRange(), System.Drawing.Color.Red);
                    Drawing.DrawCircle(Player.Position, 900, System.Drawing.Color.Yellow);
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Player.IsDead)
                return;

            if(Orbwalker.ActiveMode == OrbwalkerMode.Harass && SamiraSetMenu.KeysSettings.QMixed.Active)
            {
                if(SamiraSetMenu.QSettings.QManaCheck.Value < Player.ManaPercent)
                {
                    if (Q.IsReady())
                    {
                        var targets = GameObjects.EnemyHeroes.Where(i => !i.IsDead && i.IsValidTarget(Q.Range)).OrderBy(i => i.Health).ToArray();
                        if(targets != null)
                        {
                            foreach(var target in targets)
                            {
                                if (Q.SPredictionCast(target, EnsoulSharp.SDK.HitChance.High))
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
                        var minions = ObjectManager.Get<AIBaseClient>().Where(i => !i.IsAlly && (i.Type == GameObjectType.AIHeroClient || i.Type == GameObjectType.AIMinionClient) && !i.IsDead && i.IsValidTarget(Q.Range) && !i.Position.IsBuilding()).OrderBy(i => i.Health);
                        if(minions != null && Q.IsReady())
                        {
                            foreach(var min in minions)
                            {
                                if (SamiraSetMenu.QSettings.QManaCheck.Value < Player.ManaPercent)
                                {
                                    if (Q.Cast(min) == CastStates.SuccessfullyCasted)
                                        return;
                                }
                            }
                        }
                    }

                    if(Orbwalker.ActiveMode == OrbwalkerMode.LastHit)
                    {
                        var minions = ObjectManager.Get<AIBaseClient>().Where(i => !i.IsAlly && (i.Type == GameObjectType.AIHeroClient || i.Type == GameObjectType.AIMinionClient) && !i.IsDead && i.IsValidTarget(Q.Range) && !i.Position.IsBuilding() && i.Health < Player.GetSpellDamage(i, SpellSlot.Q)).OrderBy(i => i.Health);
                        if (minions != null && Q.IsReady())
                        {
                            foreach (var min in minions)
                            {
                                if (SamiraSetMenu.QSettings.QManaCheck.Value < Player.ManaPercent)
                                {
                                    if (Q.Cast(min) == CastStates.SuccessfullyCasted)
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
            if (Player.IsDead || FunnySlayerCommon.OnAction.OnAA || FunnySlayerCommon.OnAction.BeforeAA || Orbwalker.ActiveMode != OrbwalkerMode.Combo)
                return;

            if (SamiraSetMenu.KeysSettings.TurboFast.Active && Q.IsReady() && W.IsReady() && E.IsReady())
            {
                var target = FunnySlayerCommon.FSTargetSelector.GetFSTarget(E.Range);
                if(target != null)
                {
                    if (FunnySlayerCommon.OnAction.AfterAA)
                    {
                        W.Cast();
                        E.Cast(target);
                        if (target.IsValidTarget(300))
                        {
                            Q.Cast(target.Position);
                        }
                        else
                        {
                            if (Q.SPredictionCast(target, EnsoulSharp.SDK.HitChance.High))
                            {
                                if (E.Cast(target) == CastStates.SuccessfullyCasted)
                                    if (W.Cast())
                                        return;
                            }
                            else
                            {
                                if (E.Cast(target) == CastStates.SuccessfullyCasted)
                                    if (W.Cast())
                                        return;
                            }
                        }
                    }
                    if (R.IsReady() && SamiraSetMenu.RSettings.RCombo.Enabled)
                    {
                        if (R.Cast())
                        {
                            if (W.Cast())
                            {
                                if (E.Cast(target) == CastStates.SuccessfullyCasted)
                                {
                                    if (Q.Cast())
                                    {
                                        /*if (*/return;/*)*/
                                    }
                                }
                            }
                        }
                    }
                    if (E.IsReady() && SamiraSetMenu.ESettings.ECombo.Enabled)
                    {
                        if (SamiraSetMenu.KeysSettings.AllowTurret.Active || !UnderTower(Player.Position.Extend(target.Position, E.Range)))
                        {
                            if (target.IsValidTarget(300))
                            {
                                Q.Cast(target.Position);
                                E.Cast(target);
                                EnsoulSharp.SDK.Utility.DelayAction.Add(300, () => { W.Cast(); });
                            }
                            else
                            {
                                if(Q.SPredictionCast(target, EnsoulSharp.SDK.HitChance.High))
                                {
                                    if (W.Cast())
                                    {
                                        if (E.Cast(target) == CastStates.SuccessfullyCasted)
                                            return;
                                    }
                                }
                                else
                                {
                                    if(W.Cast())
                                    {
                                        if (E.Cast(target) == CastStates.SuccessfullyCasted)
                                            return;
                                    }
                                }
                            }
                        }
                    }
                }               
            }
            else
            {
                if (R.IsReady() && SamiraSetMenu.RSettings.RCombo.Enabled)
                {
                    if (Player.CountEnemyHeroesInRange(R.Range) >= SamiraSetMenu.RSettings.RCount.Value)
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
                                            if (E.Cast(t) == CastStates.SuccessfullyCasted)
                                            {
                                                if (R.Cast())
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
                            if (Pos.DistanceToPlayer() > R.Range)
                            {
                                if (E.IsReady() && SamiraSetMenu.RSettings.AutoE.Enabled)
                                {
                                    if (SamiraSetMenu.KeysSettings.AllowTurret.Active || !UnderTower(Player.Position.Extend(target.Position, E.Range)))
                                    {
                                        if (E.Cast(target) == CastStates.SuccessfullyCasted)
                                        {
                                            if (R.Cast())
                                                return;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                if (R.Cast())
                                    return;
                            }
                        }
                    }
                }
                if (E.IsReady() && SamiraSetMenu.ESettings.ECombo.Enabled)
                {
                    //E on Minions
                    {
                        var fstarget = FunnySlayerCommon.FSTargetSelector.GetFSTarget(E.Range + Q.Range);
                        if (fstarget != null)
                        {
                            var minion = ObjectManager.Get<AIMinionClient>().Where(i => !i.IsDead && i.IsValid() && !i.IsAlly && i.IsValidTarget(E.Range) && Player.Position.Extend(i.Position, E.Range).Distance(fstarget) < Player.Distance(fstarget)).OrderBy(i => Player.Position.Extend(i.Position, E.Range).Distance(fstarget));
                            if (minion != null)
                            {
                                if (SamiraSetMenu.ESettings.EMinions.Active)
                                {
                                    foreach (var min in minion)
                                    {
                                        if (min != null)
                                        {
                                            if (SamiraSetMenu.KeysSettings.AllowTurret.Active || !UnderTower(Player.Position.Extend(min.Position, E.Range)))
                                            {
                                                if (fstarget.HealthPercent <= SamiraSetMenu.ESettings.Eheath.Value)
                                                {
                                                    if (SamiraSetMenu.ESettings.ER.Enabled)
                                                    {
                                                        if (R.IsReady())
                                                        {
                                                            if (E.Cast(min) == CastStates.SuccessfullyCasted)
                                                                return;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (E.Cast(min) == CastStates.SuccessfullyCasted)
                                                            return;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    var targets = GameObjects.EnemyHeroes.Where(i => !i.IsDead && i.IsValidTarget(E.Range)).OrderBy(i => i.Health);
                    if (targets != null)
                    {
                        foreach (var target in targets)
                        {
                            if (FSpred.Prediction.Prediction.PredictUnitPosition(target, 700).DistanceToPlayer() > E.Range)
                            {
                                if (SamiraSetMenu.KeysSettings.AllowTurret.Active || !UnderTower(Player.Position.Extend(target.Position, E.Range)))
                                    if (E.Cast(target) == CastStates.SuccessfullyCasted)
                                    {
                                        return;
                                    }
                            }
                            if (target.Health < GetEDmg(target) && SamiraSetMenu.ESettings.EKs.Enabled)
                            {
                                if (SamiraSetMenu.KeysSettings.AllowTurret.Active || !UnderTower(Player.Position.Extend(target.Position, E.Range)))
                                    if (E.Cast(target) == CastStates.SuccessfullyCasted)
                                    {
                                        return;
                                    }
                            }
                            if (Player.HasBuff("SamiraR") && SamiraSetMenu.RSettings.AutoE.Enabled)
                            {
                                if (Player.Position.Extend(target.Position, +E.Range).CountEnemyHeroesInRange(R.Range) > Player.CountEnemyHeroesInRange(R.Range))
                                {
                                    if (SamiraSetMenu.KeysSettings.AllowTurret.Active || !UnderTower(Player.Position.Extend(target.Position, E.Range)))
                                        if (E.Cast(target) == CastStates.SuccessfullyCasted)
                                        {
                                            return;
                                        }
                                }
                                if (FunnySlayerCommon.FSTargetSelector.GetFSTarget(R.Range + 300) != null)
                                {
                                    var Pos = FSpred.Prediction.Prediction.PredictUnitPosition(FunnySlayerCommon.FSTargetSelector.GetFSTarget(R.Range + 300), 700);
                                    var enemy = ObjectManager.Get<AIBaseClient>().Where(i => !i.IsAlly && (i.Type == GameObjectType.AIHeroClient || i.Type == GameObjectType.AIMinionClient) && !i.IsDead && i.IsValidTarget(E.Range) && !i.Position.IsBuilding()).OrderBy(i => i.Health).OrderBy(i => i is AIHeroClient);
                                    if (enemy != null)
                                    {
                                        foreach (var Next in enemy)
                                        {
                                            if (SamiraSetMenu.KeysSettings.AllowTurret.Active || !UnderTower(Player.Position.Extend(Next.Position, E.Range)))
                                            {
                                                if (Player.Position.Extend(Next.Position, E.Range).Distance(Pos) < Player.Distance(Pos))
                                                {
                                                    if (E.Cast(Next) == CastStates.SuccessfullyCasted)
                                                        return;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                            if (Player.HasBuff("SamiraW") && SamiraSetMenu.ESettings.EW.Enabled)
                            {
                                if (Variables.TickCount > LastW + 750)
                                {
                                    if (SamiraSetMenu.KeysSettings.AllowTurret.Active || !UnderTower(Player.Position.Extend(target.Position, E.Range)))
                                        if (E.Cast(target) == CastStates.SuccessfullyCasted)
                                        {
                                            return;
                                        }
                                }
                            }
                            if (Q.IsReady() && SamiraSetMenu.ESettings.EQ.Enabled)
                            {
                                if (SamiraSetMenu.KeysSettings.AllowTurret.Active || !UnderTower(Player.Position.Extend(target.Position, E.Range)))
                                    if (E.Cast(target) == CastStates.SuccessfullyCasted)
                                    {
                                        if (Q.Cast(new SharpDX.Vector3(45646, 546416, 45462)))
                                            return;
                                        return;
                                    }
                            }
                        }
                    }
                }
                if (LastCasted + SamiraSetMenu.Misc.AATimer.Value <= Variables.TickCount)
                {
                    if (W.IsReady() && SamiraSetMenu.WSettings.WCantAA.Enabled && LastCasted + SamiraSetMenu.Misc.AATimer.Value < Variables.TickCount)
                    {
                        if (!Player.CanAttack || !Orbwalker.CanAttack())
                        {
                            var targets = GameObjects.EnemyHeroes.Where(i => !i.IsDead && i.IsValidTarget(E.IsReady() ? E.Range : W.Range)).OrderBy(i => i.Health);
                            if (targets != null)
                            {
                                if (W.Cast())
                                    return;
                            }
                        }
                        if (E.IsReady() && SamiraSetMenu.ESettings.EW.Enabled)
                        {
                            var targets = GameObjects.EnemyHeroes.Where(i => !i.IsDead && i.IsValidTarget(E.Range)).OrderBy(i => i.Health).ToArray();
                            if (targets != null)
                            {
                                foreach (var target in targets)
                                {
                                    if (SamiraSetMenu.KeysSettings.AllowTurret.Active || !UnderTower(Player.Position.Extend(target.Position, E.Range)))
                                    {
                                        if (target.HealthPercent < 60)
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
                    }

                    if (Q.IsReady())
                    {
                        if (SamiraSetMenu.QSettings.QGunCombo.Enabled || SamiraSetMenu.QSettings.QBladeCombo.Enabled)
                        {
                            var target = FunnySlayerCommon.FSTargetSelector.GetFSTarget(Q.Range);
                            if (target != null)
                            {
                                var pred = SebbyLibPorted.Prediction.Prediction.GetPrediction(Q, target);
                                if ((Player.IsDashing() || LastE + 700 > Variables.TickCount) && SamiraSetMenu.ESettings.EQ.Enabled)
                                {

                                }
                                if ((!Player.IsDashing() && (LastE + 700 < Variables.TickCount || SamiraSetMenu.ESettings.EQ.Enabled)))
                                {
                                    if (pred.CastPosition.DistanceToPlayer() > Q.Range / 3)
                                    {
                                        if (SamiraSetMenu.QSettings.QGunCombo.Enabled)
                                        {
                                            if (LastCasted + SamiraSetMenu.Misc.AATimer.Value > Variables.TickCount)
                                                return;
                                            else
                                            if (Q.SPredictionCast(target, EnsoulSharp.SDK.HitChance.High))
                                                return;
                                        }
                                        else
                                        {
                                            var targets = GameObjects.EnemyHeroes.Where(i => !i.IsDead && i.IsValidTarget(Q.Range / 3)).OrderBy(i => i.Health);
                                            if (targets != null)
                                            {
                                                foreach (var t in targets)
                                                {
                                                    if (LastCasted + SamiraSetMenu.Misc.AATimer.Value > Variables.TickCount)
                                                        return;
                                                    else
                                                    if (Q.Cast(t.Position))
                                                        return;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (!SamiraSetMenu.QSettings.QBladeCombo.Enabled)
                                        {
                                            if (pred.Hitchance != SebbyLibPorted.Prediction.HitChance.Collision)
                                            {
                                                if (LastCasted + SamiraSetMenu.Misc.AATimer.Value > Variables.TickCount)
                                                    return;
                                                else
                                                if (Q.Cast(Player.Position.Extend(pred.CastPosition, Q.Range)))
                                                    return;
                                            }
                                        }
                                        else
                                        {
                                            if (LastCasted + SamiraSetMenu.Misc.AATimer.Value > Variables.TickCount)
                                                return;
                                            else
                                            if (Q.Cast(pred.CastPosition))
                                                return;
                                        }
                                    }
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
            var Rlist = new List<int>{
                0, 10, 20, 30
            };
            var EGetDmg = Player.CalculateMagicDamage(target, Elist[E.Level]);

            EGetDmg += 0.2f * Player.GetBonusPhysicalDamage();

            EGetDmg += Player.GetAutoAttackDamage(target);

            if (Player.HasBuff("SamiraR") || R.IsReady())
                EGetDmg += Player.CalculatePhysicalDamage(target, (Rlist[R.Level] + 0.6f * Player.TotalAttackDamage) * 2);

            return (float)EGetDmg;
        }
        private static int LastCasted = 0;
        private static int LastE = 0;
        private static int LastW = 0;
        private static int LastAttack = 0;
        private static void AIBaseClient_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (sender == null || !sender.IsValid() || args == null)
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
                    Orbwalker.ResetAutoAttackTimer();
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

            if (!sender.IsAlly && (args.Slot <= SpellSlot.R && sender.Type == GameObjectType.AIHeroClient))
            {
                if(args.Target != null)
                {
                    if (args.Target.IsMe || args.Target.NetworkId == ObjectManager.Player.NetworkId)
                    {
                        if (TargetSelector.GetTargets(W.Range + E.Range, DamageType.Physical) != null && TargetSelector.GetTargets(W.Range + E.Range, DamageType.Physical).Count() >= SamiraSetMenu.WSettings.EnemyCount.Value)
                        {
                            if (SamiraSetMenu.WSettings.WBlock.Enabled && (Orbwalker.ActiveMode == OrbwalkerMode.Combo || !SamiraSetMenu.WSettings.WonlyBlockIncombo.Enabled) && !FunnySlayerCommon.OnAction.BeforeAA && !FunnySlayerCommon.OnAction.OnAA)
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
        }

        /*private static bool AfterAA = false;
        private static bool BeforeAA = false;
        private static bool OnAA = false;*/
        /*private static void Orbwalker_OnAction(object sender, OrbwalkerActionArgs args)
        {
            if(args.Type == OrbwalkerType.AfterAttack)
            {
                LastCasted = 0;
                AfterAA = true;
                OnAA = false;
                BeforeAA = false;                
            }
            else
            {
                AfterAA = false;
            }

            if (args.Type == OrbwalkerType.OnAttack)
            {
                LastCasted = 0;
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
        }*/

        private static void Check(EventArgs args)
        {
            if (Player.IsDead)
                return;

            if (FunnySlayerCommon.OnAction.AfterAA)
                LastCasted = 0;

            if (Player.HasBuff("SamiraR"))
                Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            if (Player.HasBuff("SamiraW") && LastE < LastW)
                Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            if (SamiraSetMenu.ESettings.EKs.Enabled)
            {
                var targets = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(E.Range) && !i.IsDead).OrderBy(i => i.Health);
                if(targets != null)
                    foreach(var target in targets)
                    {
                        if(target != null)
                        {
                            if(target.Health <= GetEDmg(target))
                            {
                                if(!UnderTower(Player.Position.Extend(target.Position, E.Range)) || SamiraSetMenu.KeysSettings.AllowTurret.Active){
                                    if (E.Cast(target) == CastStates.SuccessfullyCasted)
                                        return;
                                }
                            }
                        }
                    }
            }
        }
    }
}
