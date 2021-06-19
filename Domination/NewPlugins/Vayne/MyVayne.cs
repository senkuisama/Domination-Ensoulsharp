using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominationAIO.NewPlugins
{
    public static class MyVayne
    {
        private static class VayneMenu
        {
            public static class QCombo
            {
                public static MenuBool UseQ = new MenuBool("_UseQ", "Use Combo Q (After AA)");
                public static MenuBool QOnlyCombo = new MenuBool("_QOnlyCombo", "Only Combo", false);
                public static MenuBool QBack = new MenuBool("_QBack", "Q Turn Around Logic");
                public static MenuBool QCursor = new MenuBool("_QCursor", "Q Follow Mouse");
            } 

            public static class ECombo
            {
                public static MenuBool UseE = new MenuBool("_UseE", "Use Combo E");
                public static MenuBool FastE = new MenuBool("_FastE", "Turn On Fast E", false);
                public static MenuBool EOnlyCombo = new MenuBool("_EOnlyCombo", "Only Combo", false);
                public static MenuBool EAnti = new MenuBool("_EAnti", "E Anti Someone Want Kill U");
            }

            public static class RCombo
            {
                public static MenuBool UseR = new MenuBool("_UseR", "Use Combo R");
                public static MenuBool AutoEnable = new MenuBool("_   Auto Enable R", "Auto Enable R");
                public static MenuSlider Hp = new MenuSlider("_Hp", "My Hp : ", 60);
                public static MenuSlider Target = new MenuSlider("_Target", "Target Count >= ", 3, 1, 5);
                public static MenuSlider TargetHp = new MenuSlider("_Target Hp", "Target Hp <= % ", 60);
                public static MenuBool CheckTurret = new MenuBool("_Check Turret", "Check Turret Around");
            }

            public static class Misc
            {
                public static class QSettings
                {
                    public static MenuSeparator QBackSettings = new MenuSeparator("_QBackSettings", "Q Back Settings");
                    public static MenuBool EnemyClose = new MenuBool("_EnemyClose", "Use When Enemy Too Close");
                    public static MenuBool Mode1vs2 = new MenuBool("_Mode1vs21vs2", "When 1 vs 2");
                    public static MenuBool OutPlayMotion = new MenuBool("_OutPlay", "Make High Light");
                    public static MenuBool OnlyWhenRActive = new MenuBool("_OnlyR", "Only When R Active", false);
                }

                public static class ESettings
                {
                    public static MenuSeparator ELogic = new MenuSeparator("_ELogicSettings", "E Logic Settings");
                    public static MenuSlider KnockBackRange = new MenuSlider("_KnockBackRange", "Max Knock Back Range", 425, 375, 475);
                    public static MenuBool CheckAction = new MenuBool("_CheckAction", "Checking AA Action");
                    public static MenuBool AutoFindBestTarget = new MenuBool("_BestTarget", "Auto E On Best Target");
                }

                public static class RSettings
                {
                    public static MenuSeparator RLogicSettings = new MenuSeparator("_RLogicSettings", "R Logic Settings");
                    public static MenuBool RQ = new MenuBool("_RQ", "Use RQ If Can");
                    public static MenuBool FindOut = new MenuBool("_FindOut", "R When Find Out danger");
                }

                public static class OrbWalk
                {
                    public static MenuSeparator OrbWalkerForVayne = new MenuSeparator("_OrbWalkerForVayne", "Vayne OrbWalker (Coming Soon :))");
                }
            }

            public static void LoadMenuVayne()
            {
                VMenu = new Menu("MyVayne", "FunnySlayer Vayne", true);
                var QC = new Menu("_QC", "Q Combo");
                QC.Add(QCombo.UseQ);
                QC.Add(QCombo.QOnlyCombo);
                QC.Add(QCombo.QBack);
                QC.Add(QCombo.QCursor);

                var EC = new Menu("_EC", "E Combo");
                EC.Add(ECombo.UseE);
                EC.Add(ECombo.EOnlyCombo);
                EC.Add(ECombo.FastE);
                EC.Add(ECombo.EAnti);

                var RC = new Menu("_RC", "R Combo");
                RC.Add(RCombo.UseR);
                RC.Add(RCombo.AutoEnable);
                RC.Add(RCombo.Hp);
                RC.Add(RCombo.Target);
                RC.Add(RCombo.TargetHp);
                RC.Add(RCombo.CheckTurret);

                var AllMisc = new Menu("_Misc", "Misc")
                {
                    new Menu("_QSet", "Q Settings")
                    {
                        Misc.QSettings.QBackSettings,
                        Misc.QSettings.EnemyClose,
                        Misc.QSettings.Mode1vs2,
                        Misc.QSettings.OutPlayMotion,
                        Misc.QSettings.OnlyWhenRActive
                    },
                    new Menu("_ESet", "E Settings")
                    {
                        Misc.ESettings.ELogic,
                        Misc.ESettings.KnockBackRange,
                        Misc.ESettings.CheckAction,
                        Misc.ESettings.AutoFindBestTarget
                    },
                    new Menu("_RSet", "R Settings")
                    {
                        Misc.RSettings.RLogicSettings,
                        Misc.RSettings.RQ,
                        Misc.RSettings.FindOut
                    },
                    new Menu("_OrbSet", "OrbWalker Settings")
                    {
                        Misc.OrbWalk.OrbWalkerForVayne,
                        //Orbwalker.Menu
                    },
                };

                VMenu.Add(QC);
                VMenu.Add(EC);
                VMenu.Add(RC);
                VMenu.Add(AllMisc);

                VMenu.Attach();
            }
        }
        private static Menu VMenu = null;
        private static Spell Q, E, R;
        public static void MyVayneLoad()
        {
            Q = new Spell(SpellSlot.Q, 300);
            E = new Spell(SpellSlot.E, 550);
            E.SetTargetted(0.25f, 1200f);
            R = new Spell(SpellSlot.R, float.MaxValue);

            VayneMenu.LoadMenuVayne();
            //Orbwalker.OnAction += Orbwalker_OnAction;
            Game.OnUpdate += Game_OnUpdate;
            AntiGapcloser.OnGapcloser += Gapcloser_OnGapcloser;

            Orbwalker.OnAfterAttack += Orbwalker_OnAfterAttack;

            Game.OnUpdate += Game_OnUpdate1;
        }
        private static Vector3 GetQCastPos(this AIHeroClient target)
        {
            if (target.CombatType != GameObjectCombatType.Melee && !Game.CursorPos.IsWall()) return Game.CursorPos;
            var allcastpos = new List<EnsoulSharp.SDK.Clipper.IntPoint>();

            if (ObjectManager.Player.CountEnemyHeroesInRange(800) > 1)
            {
                var circel = new Geometry.Circle(ObjectManager.Player.Position, 300).ToClipperPath();
                allcastpos.AddRange(circel.Where(c => !Helper.UnderTower(new Vector3(c.X, c.Y, 0)) && (target.IsFacing(ObjectManager.Player)) ? (new Vector3(c.X, c.Y, 0)).Distance(target) <= 530 : (new Vector3(c.X, c.Y, 0)).Distance(target) <= 555 - 0.2 * target.MoveSpeed).ToList());
            }
            if(allcastpos != null && allcastpos.Count > 1)
            {
                var getpos = allcastpos.OrderBy(p => new Vector3(p.X, p.Y, 0).Distance(Game.CursorPos)).FirstOrDefault();
                return new Vector3(getpos.X, getpos.Y, 0);
            }

            return Game.CursorPos;
        }
        private static void Orbwalker_OnAfterAttack(object sender, AfterAttackEventArgs e)
        {
            if (ObjectManager.Player.IsDead)
                return;

            var targets = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(660) && !i.IsDead).OrderBy(k => k.MaxHealth).ThenBy(k => k.Health);


            if (Orbwalker.ActiveMode < OrbwalkerMode.Harass || !VayneMenu.QCombo.QOnlyCombo.Enabled)
            {
                if(e.Target.Type == GameObjectType.AIHeroClient)
                {
                    if (targets != null)
                    {
                        targets.ForEach(i =>
                        {
                            if (i.IsValidTarget(660) && i.CanMove && VayneMenu.QCombo.QBack.Enabled && (R.IsReady() || ObjectManager.Player.HasBuff("VayneInquisition") || !VayneMenu.Misc.QSettings.OnlyWhenRActive.Enabled))
                            {
                                if (i.DistanceToPlayer() > i.GetWaypoints().LastOrDefault().DistanceToPlayer())
                                {
                                    if (i.DistanceToPlayer() <= ObjectManager.Player.GetWaypoints().LastOrDefault().Distance(i))
                                    {
                                        var poswillcast = GetQCastPos(i);
                                        if(poswillcast != Vector3.Zero)
                                        {
                                            if (!UnderTower(poswillcast))
                                            {
                                                if (R.IsReady() && VayneMenu.RCombo.UseR.Enabled && (Orbwalker.ActiveMode < OrbwalkerMode.Harass || VayneMenu.RCombo.AutoEnable.Enabled))
                                                {
                                                    if (R.Cast())
                                                        if (Q.Cast(poswillcast))
                                                            return;
                                                }
                                                else
                                                {
                                                    if (Q.Cast(poswillcast))
                                                        return;
                                                }
                                            }
                                            else
                                            {
                                                if (Q.Cast(Game.CursorPos))
                                                    return;
                                            }
                                        }
                                        else
                                        {
                                            if (Q.Cast(Game.CursorPos))
                                                return;
                                        }
                                    }
                                    else
                                    {
                                        if (Q.Cast(Game.CursorPos))
                                            return;
                                    }
                                }
                                else
                                {
                                    if (Q.Cast(Game.CursorPos))
                                        return;
                                }
                            }
                            else
                            {
                                if (Q.Cast(Game.CursorPos))
                                    return;
                            }
                        });
                    }
                }
            }
        }

        private static void Gapcloser_OnGapcloser(AIHeroClient sender, AntiGapcloser.GapcloserArgs args)
        {
            if (sender.IsAlly)
                return;

            if (args.SpellName == "ZedR")
                return;

            if(args.EndPosition.DistanceToPlayer() < args.StartPosition.DistanceToPlayer())
            {
                if(args.EndPosition.DistanceToPlayer() <= 300 && sender.IsValidTarget(550))
                {
                    if (E.Cast(sender) == CastStates.SuccessfullyCasted)
                        return;
                }
                else
                {
                    return;
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            //VayneInquisition

            if (ObjectManager.Player.IsDead)
                return;

            var targets = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(650) && !i.IsDead).OrderBy(k => k.MaxHealth).ThenBy(k => k.Health);

            if (E.IsReady() && VayneMenu.ECombo.UseE.Enabled)
            {
                if ((!VayneMenu.ECombo.EOnlyCombo.Enabled || Orbwalker.ActiveMode < OrbwalkerMode.Harass) && (!UnderTower(ObjectManager.Player.Position)))
                {
                    if (VayneMenu.ECombo.FastE.Enabled)
                    {
                        //Game.OnUpdate += Game_OnUpdate1;
                    }
                    else
                    {

                        if (!FunnySlayerCommon.OnAction.OnAA && !FunnySlayerCommon.OnAction.BeforeAA)
                        {
                            if (targets != null)
                            {
                                foreach (var target in targets)
                                {
                                    var check = false;
                                    for (int i = 0; i < VayneMenu.Misc.ESettings.KnockBackRange.Value; i++)
                                    {
                                        var t_pos = FSpred.Prediction.Prediction.PredictUnitPosition(target, 300);
                                        var flag = NavMesh.GetCollisionFlags(t_pos.Extend(ObjectManager.Player.Position.ToVector2(), -i).ToVector3());

                                        if (flag.HasFlag(CollisionFlags.Building) || flag.HasFlag(CollisionFlags.Wall))
                                        {
                                            check = true;
                                            if (E.Cast(target) == CastStates.SuccessfullyCasted)
                                                return;
                                        }
                                        else
                                        {
                                            check = false;
                                        }
                                    }

                                    if (!check)
                                        continue;
                                }
                            }
                        }                        
                    }
                }
            }

            if(R.IsReady() && VayneMenu.RCombo.UseR.Enabled)
            {
                if (VayneMenu.RCombo.AutoEnable.Enabled || Orbwalker.ActiveMode == OrbwalkerMode.Combo)
                {
                    if (!FunnySlayerCommon.OnAction.OnAA)
                    {
                        if (targets.Any(i => i.HealthPercent <= VayneMenu.RCombo.TargetHp.Value) || ObjectManager.Player.HealthPercent <= VayneMenu.RCombo.Hp.Value || ObjectManager.Player.CountEnemyHeroesInRange(600) >= VayneMenu.RCombo.Target.Value)
                        {
                            if (FunnySlayerCommon.OnAction.BeforeAA)
                            {
                                if (R.Cast())
                                    return;
                            }
                            if (FunnySlayerCommon.OnAction.AfterAA)
                            {
                                if (Q.IsReady())
                                {
                                    if (R.Cast())
                                        if (Q.Cast(Game.CursorPos))
                                            return;
                                }
                            }
                        }
                    }
                }
            }

            if(Q.IsReady() && VayneMenu.QCombo.UseQ.Enabled)
            {
                if(!VayneMenu.QCombo.QOnlyCombo.Enabled || Orbwalker.ActiveMode <= OrbwalkerMode.Harass)
                {
                    if (FunnySlayerCommon.OnAction.AfterAA && Orbwalker.LastTarget.Type == GameObjectType.AIHeroClient)
                    {
                        if(targets != null)
                        {
                            var melee = targets.Where(i => i.CombatType == GameObjectCombatType.Melee);
                            if(melee != null)
                            {
                                melee.ForEach(i =>
                                {
                                    if(i.IsValidTarget(660) && i.CanMove && VayneMenu.QCombo.QBack.Enabled && (R.IsReady() || ObjectManager.Player.HasBuff("VayneInquisition") || !VayneMenu.Misc.QSettings.OnlyWhenRActive.Enabled))
                                    {
                                        if(i.DistanceToPlayer() > i.GetWaypoints().LastOrDefault().DistanceToPlayer())
                                        {
                                            if(i.DistanceToPlayer() <= ObjectManager.Player.GetWaypoints().LastOrDefault().Distance(i))
                                            {
                                                var poswillcast = GetQCastPos(i);
                                                if(poswillcast != Vector3.Zero)
                                                {
                                                    if (!UnderTower(poswillcast))
                                                    {
                                                        if (R.IsReady() && VayneMenu.RCombo.UseR.Enabled && (Orbwalker.ActiveMode < OrbwalkerMode.Harass || VayneMenu.RCombo.AutoEnable.Enabled))
                                                        {
                                                            if (R.Cast())
                                                                if (Q.Cast(poswillcast))
                                                                    return;
                                                        }
                                                        else
                                                        {
                                                            if (Q.Cast(poswillcast))
                                                                return;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        if (Q.Cast(Game.CursorPos))
                                                            return;
                                                    }
                                                }
                                                else
                                                {
                                                    if (Q.Cast(Game.CursorPos))
                                                        return;
                                                }
                                            }
                                            else
                                            {
                                                if (Q.Cast(Game.CursorPos))
                                                    return;
                                            }
                                        }
                                        else
                                        {
                                            if (Q.Cast(Game.CursorPos))
                                                return;
                                        }
                                    }
                                    else
                                    {
                                        if (Q.Cast(Game.CursorPos))
                                            return;
                                    }
                                });
                            }
                            else
                            {
                                if(targets.Any(i => i.HealthPercent <= VayneMenu.RCombo.TargetHp.Value) || ObjectManager.Player.HealthPercent <= VayneMenu.RCombo.Hp.Value || ObjectManager.Player.CountEnemyHeroesInRange(600) >= VayneMenu.RCombo.Target.Value)
                                {
                                    if (R.IsReady())
                                    {
                                        if (R.Cast())
                                            if (Q.Cast(Game.CursorPos))
                                                return;
                                    }
                                    else
                                    {
                                        if (Q.Cast(Game.CursorPos))
                                            return;
                                    }
                                }
                                else
                                {
                                    if (Q.Cast(Game.CursorPos))
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
        private static void Game_OnUpdate1(EventArgs args)
        {
            if (ObjectManager.Player.IsDead) return;

            if(R.IsReady() && VayneMenu.Misc.RSettings.FindOut.Enabled)
            {
                if(TargetSelector.GetTargets(750, DamageType.Physical) != null && !FunnySlayerCommon.OnAction.OnAA)
                {
                    var target = TargetSelector.GetTarget(750, DamageType.Physical);
                    if(TargetSelector.GetTargets(750, DamageType.Physical).Count() >= 2 && ObjectManager.Player.CountAllyHeroesInRange(750) <= 1)
                    {
                        if (FunnySlayerCommon.OnAction.BeforeAA)
                        {
                            if(target.DistanceToPlayer() <= ObjectManager.Player.GetCurrentAutoAttackRange())
                            {
                                if(target.GetWaypoints()[3] != Vector2.Zero && target.DistanceToPlayer() <= target.GetWaypoints()[3].DistanceToPlayer())
                                {
                                    var i = target.GetWaypoints()[3];

                                    var poswillcast = ObjectManager.Player.Position.Extend(i, 300);
                                    if (!UnderTower(poswillcast))
                                    {
                                        if (R.IsReady() && VayneMenu.RCombo.UseR.Enabled && (Orbwalker.ActiveMode < OrbwalkerMode.Harass || VayneMenu.RCombo.AutoEnable.Enabled))
                                        {
                                            if (R.Cast())
                                                    return;
                                        }                                        
                                    }
                                }
                            }
                        }

                        if(FunnySlayerCommon.OnAction.AfterAA && Q.IsReady())
                        {
                            if (target.GetWaypoints()[3] != Vector2.Zero && target.DistanceToPlayer() <= target.GetWaypoints()[3].DistanceToPlayer())
                            {
                                var i = target.GetWaypoints()[3];

                                var poswillcast = ObjectManager.Player.Position.Extend(i, 300);
                                if (!UnderTower(poswillcast))
                                {
                                    if (R.IsReady() && VayneMenu.RCombo.UseR.Enabled && (Orbwalker.ActiveMode < OrbwalkerMode.Harass || VayneMenu.RCombo.AutoEnable.Enabled))
                                    {
                                        if (R.Cast())
                                            if (Q.Cast(poswillcast))
                                                return;
                                    }
                                    else
                                    {
                                        if (Q.Cast(poswillcast))
                                            return;
                                    }
                                }
                                else
                                {
                                    if (Q.Cast(Game.CursorPos))
                                        return;
                                }
                            }
                            else
                            {
                                if (Q.Cast(Game.CursorPos))
                                    return;
                            }
                        }
                    }

                    if(TargetSelector.GetTargets(750, DamageType.Physical).Any(i => i.HealthPercent >= 40 && (i.MaxHealth <= 2000 || i.CombatType == GameObjectCombatType.Melee || i.TotalAttackDamage >= 200 || i.TotalMagicalDamage >= 250)))
                    {
                        if (target.DistanceToPlayer() <= ObjectManager.Player.GetCurrentAutoAttackRange())
                        {
                            if (target.GetWaypoints()[3] != Vector2.Zero && target.DistanceToPlayer() <= target.GetWaypoints()[3].DistanceToPlayer())
                            {
                                var i = target.GetWaypoints()[3];

                                var poswillcast = ObjectManager.Player.Position.Extend(i, 300);
                                if (!UnderTower(poswillcast))
                                {
                                    if (R.IsReady() && VayneMenu.RCombo.UseR.Enabled && (Orbwalker.ActiveMode < OrbwalkerMode.Harass || VayneMenu.RCombo.AutoEnable.Enabled))
                                    {
                                        if (R.Cast())
                                            if (Q.Cast(poswillcast))
                                                return;
                                    }
                                    else
                                    {
                                        if (Q.Cast(poswillcast))
                                            return;
                                    }
                                }
                                else
                                {
                                    if (Q.Cast(Game.CursorPos))
                                        return;
                                }
                            }

                            if (target.GetWaypoints()[3] != Vector2.Zero && target.DistanceToPlayer() >= target.GetWaypoints()[3].DistanceToPlayer())
                            {
                                if (R.IsReady())
                                {
                                    if (R.Cast())
                                        if (Q.Cast(Game.CursorPos))
                                            return;
                                }
                            }
                        }
                    }
                }                
            }


            if (E.IsReady() && VayneMenu.ECombo.UseE.Enabled)
            {
                if ((!VayneMenu.ECombo.EOnlyCombo.Enabled || Orbwalker.ActiveMode <= OrbwalkerMode.Harass) && (!UnderTower(ObjectManager.Player.Position) || Orbwalker.ActiveMode <= OrbwalkerMode.Combo))
                {
                    if (VayneMenu.ECombo.FastE.Enabled)
                    {
                        var targets = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(580) && !i.IsDead)
                            .OrderBy(k => k.MaxHealth)
                            .ThenBy(k => k.Health);

                        if (targets != null)
                        {
                            foreach (var target in targets)
                            {
                                var check = false;
                                for (int i = 0; i < VayneMenu.Misc.ESettings.KnockBackRange.Value; i++)
                                {
                                    var t_pos = FSpred.Prediction.Prediction.PredictUnitPosition(target, 300);
                                    var flag = NavMesh.GetCollisionFlags(t_pos.Extend(ObjectManager.Player.Position.ToVector2(), -i).ToVector3());

                                    if (flag.HasFlag(CollisionFlags.Building) || flag.HasFlag(CollisionFlags.Wall))
                                    {
                                        check = true;
                                        if (E.Cast(target) == CastStates.SuccessfullyCasted)
                                            return;
                                    }
                                    else
                                    {
                                        check = false;
                                    }
                                }

                                if (!check)
                                    continue;
                            }
                        }
                    }
                    else
                    {
                        if (FunnySlayerCommon.OnAction.OnAA || FunnySlayerCommon.OnAction.BeforeAA)
                            return;

                        var targets = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(580) && !i.IsDead).OrderBy(k => k.MaxHealth).ThenBy(k => k.Health);

                        if(targets != null)
                        {
                            foreach(var target in targets)
                            {
                                var check = false;
                                for (int i = 0; i < VayneMenu.Misc.ESettings.KnockBackRange.Value; i++)
                                {
                                    var t_pos = FSpred.Prediction.Prediction.PredictUnitPosition(target, 300);
                                    var flag = NavMesh.GetCollisionFlags(t_pos.Extend(ObjectManager.Player.Position.ToVector2(), -i).ToVector3());

                                    if (flag.HasFlag(CollisionFlags.Building) || flag.HasFlag(CollisionFlags.Wall))
                                    {
                                        check = true;
                                        if (E.Cast(target) == CastStates.SuccessfullyCasted)
                                            return;
                                    }
                                    else
                                    {
                                        check = false;
                                    }
                                }

                                if (!check)
                                    continue;
                            }
                        }
                    }
                }
            }
        }

        //private static bool BA, OA, AA = false;
        /*private static void Orbwalker_OnAction(object sender, OrbwalkerActionArgs args)
        {
            if (args.Type == OrbwalkerType.AfterAttack)
            {
                AA = true;
                BA = false;
                OA = false;
            }
            else
            {
                AA = false;
            }

            if (args.Type == OrbwalkerType.BeforeAttack)
            {
                BA = true;
                AA = false;
                OA = false;
            }
            else
            {
                BA = false;
            }

            if (args.Type == OrbwalkerType.OnAttack)
            {
                OA = true;
                BA = false;
                AA = false;
            }
            else
            {
                OA = false;
            }
        }*/
    }
}
