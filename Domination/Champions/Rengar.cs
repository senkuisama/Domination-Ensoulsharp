using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsoulSharp.SDK;
using EnsoulSharp;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.Utility;

namespace DominationAIO.Champions
{
    public class Rengar
    {
        private static AIHeroClient Player = GameObjects.Player;
        private static Spell Q, W, E, R;
        private static bool OnRengarDashing;
        private static float DashTimer = 0;

        public static void RengarLoader()
        {
            AddMenu();

            Q = new Spell(SpellSlot.Q);
            W = new Spell(SpellSlot.W, 450);
            E = new Spell(SpellSlot.E, 1000);
            R = new Spell(SpellSlot.R);
            E.SetSkillshot(0.25f, 140f, 1500f, true, EnsoulSharp.SDK.Prediction.SkillshotType.Line);
                       
            Dash.OnDash += Dash_OnDash;
            Game.OnUpdate += ClearCC;
            Game.OnUpdate += RCheck;
            Game.OnUpdate += Combo;
            Game.OnUpdate += Clear;
            AIHeroClient.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
            Orbwalker.OnAction += CheckAA;
            Game.OnUpdate += WKS;
        }
        private static bool OnAttack(AIBaseClient target)
        {
            return Orbwalker.Attack(target) || Player.IssueOrder(GameObjectOrder.AttackUnit, target);
        }
        private static bool OnItem()
        {
            if (Player.HasItem(ItemId.Ravenous_Hydra_Melee_Only) && Player.CanUseItem((int)ItemId.Ravenous_Hydra_Melee_Only))
            {
                    return Player.UseItem((int)ItemId.Ravenous_Hydra_Melee_Only);
            }
            if (Player.HasItem(ItemId.Ravenous_Hydra) && Player.CanUseItem((int)ItemId.Ravenous_Hydra))
            {
                    return Player.UseItem((int)ItemId.Ravenous_Hydra);
            }
            if (Player.HasItem(ItemId.Tiamat) && Player.CanUseItem((int)ItemId.Tiamat))
            {
                    return Player.UseItem((int)ItemId.Tiamat);
            }
            if (Player.HasItem(ItemId.Tiamat_Melee_Only) && Player.CanUseItem((int)ItemId.Tiamat_Melee_Only))
            {
                    return Player.UseItem((int)ItemId.Tiamat_Melee_Only);
            }
            if (Player.HasItem(ItemId.Titanic_Hydra) && Player.CanUseItem((int)ItemId.Titanic_Hydra))
            {
                    return Player.UseItem((int)ItemId.Titanic_Hydra);
            }

            return false;
        }
        private static bool QEmpCast()
        {
            var target = Orbwalker.GetTarget();
            if (target == null)
                return false;

            if (!Q.IsReady() || Q.Name == "RengarQ")
                return false;

            if (Q.Cast())
            {
                if (OnAttack((AIBaseClient)target))
                {
                    if (OnItem())
                        return true;
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        private static void Clear(EventArgs args)
        {
            if (Player.IsDead) return;

            if (Orbwalker.ActiveMode != OrbwalkerMode.LaneClear && Orbwalker.ActiveMode != OrbwalkerMode.LastHit) return;

            var minions = GameObjects.Get<AIMinionClient>().Where(i => !i.IsDead && !i.IsAlly && i.DistanceToPlayer() < 1000).OrderByDescending(i => i.Health);
            if (minions == null)
                return;

            if (OnRengarDashing)
                return;

            foreach(var min in minions)
            {
                if (Q.Name != "RengarQ")
                {
                    if(min.IsJungle() && MenuRengar.Empowerd.JungleClear.Enabled && min.InAutoAttackRange())
                    {
                        if (Q.Cast() || Q.Cast(min) != CastStates.NotCasted || Q.CastOnUnit(min))
                        {
                            if (Player.HasItem(ItemId.Ravenous_Hydra_Melee_Only) && Player.CanUseItem((int)ItemId.Ravenous_Hydra_Melee_Only))
                            {
                                if (Player.UseItem((int)ItemId.Ravenous_Hydra_Melee_Only))
                                    return;
                            }
                            if (Player.HasItem(ItemId.Ravenous_Hydra) && Player.CanUseItem((int)ItemId.Ravenous_Hydra))
                            {
                                if (Player.UseItem((int)ItemId.Ravenous_Hydra))
                                    return;
                            }
                            if (Player.HasItem(ItemId.Tiamat) && Player.CanUseItem((int)ItemId.Tiamat))
                            {
                                if (Player.UseItem((int)ItemId.Tiamat))
                                    return;
                            }
                            if (Player.HasItem(ItemId.Tiamat_Melee_Only) && Player.CanUseItem((int)ItemId.Tiamat_Melee_Only))
                            {
                                if (Player.UseItem((int)ItemId.Tiamat_Melee_Only))
                                    return;
                            }
                            if (Player.HasItem(ItemId.Titanic_Hydra) && Player.CanUseItem((int)ItemId.Titanic_Hydra))
                            {
                                if (Player.UseItem((int)ItemId.Titanic_Hydra))
                                    return;
                            }

                            return;
                        }
                    }
                    if (min.IsMinion && MenuRengar.Empowerd.LaneClear.Enabled && min.InAutoAttackRange())
                    {
                        if (Q.Cast() || Q.Cast(min) != CastStates.NotCasted || Q.CastOnUnit(min))
                        {
                            if (Player.HasItem(ItemId.Ravenous_Hydra_Melee_Only) && Player.CanUseItem((int)ItemId.Ravenous_Hydra_Melee_Only))
                            {
                                if (Player.UseItem((int)ItemId.Ravenous_Hydra_Melee_Only))
                                    return;
                            }
                            if (Player.HasItem(ItemId.Ravenous_Hydra) && Player.CanUseItem((int)ItemId.Ravenous_Hydra))
                            {
                                if (Player.UseItem((int)ItemId.Ravenous_Hydra))
                                    return;
                            }
                            if (Player.HasItem(ItemId.Tiamat) && Player.CanUseItem((int)ItemId.Tiamat))
                            {
                                if (Player.UseItem((int)ItemId.Tiamat))
                                    return;
                            }
                            if (Player.HasItem(ItemId.Tiamat_Melee_Only) && Player.CanUseItem((int)ItemId.Tiamat_Melee_Only))
                            {
                                if (Player.UseItem((int)ItemId.Tiamat_Melee_Only))
                                    return;
                            }
                            if (Player.HasItem(ItemId.Titanic_Hydra) && Player.CanUseItem((int)ItemId.Titanic_Hydra))
                            {
                                if (Player.UseItem((int)ItemId.Titanic_Hydra))
                                    return;
                            }

                            return;
                        }
                    }
                }
                else
                {
                    if (Q.IsReady())
                    {
                        if (afteraa)
                        {
                            if (Q.Cast() || Q.Cast(min) != CastStates.NotCasted || Q.CastOnUnit(min))
                            {
                                if (Player.HasItem(ItemId.Ravenous_Hydra_Melee_Only) && Player.CanUseItem((int)ItemId.Ravenous_Hydra_Melee_Only))
                                {
                                    if (Player.UseItem((int)ItemId.Ravenous_Hydra_Melee_Only))
                                        return;
                                }
                                if (Player.HasItem(ItemId.Ravenous_Hydra) && Player.CanUseItem((int)ItemId.Ravenous_Hydra))
                                {
                                    if (Player.UseItem((int)ItemId.Ravenous_Hydra))
                                        return;
                                }
                                if (Player.HasItem(ItemId.Tiamat) && Player.CanUseItem((int)ItemId.Tiamat))
                                {
                                    if (Player.UseItem((int)ItemId.Tiamat))
                                        return;
                                }
                                if (Player.HasItem(ItemId.Tiamat_Melee_Only) && Player.CanUseItem((int)ItemId.Tiamat_Melee_Only))
                                {
                                    if (Player.UseItem((int)ItemId.Tiamat_Melee_Only))
                                        return;
                                }
                                if (Player.HasItem(ItemId.Titanic_Hydra) && Player.CanUseItem((int)ItemId.Titanic_Hydra))
                                {
                                    if (Player.UseItem((int)ItemId.Titanic_Hydra))
                                        return;
                                }

                                return;
                            }
                        }
                    }

                    if (W.IsReady() && min.IsJungle())
                    {
                        if(Player.Health < Player.MaxHealth - 100)
                        {
                            if (min.IsValidTarget(450))
                            {
                                if(W.Cast() || W.Cast(min) != CastStates.NotCasted || W.CastOnUnit(min))
                                {
                                    if (Player.HasItem(ItemId.Ravenous_Hydra_Melee_Only) && Player.CanUseItem((int)ItemId.Ravenous_Hydra_Melee_Only))
                                    {
                                        if (Player.UseItem((int)ItemId.Ravenous_Hydra_Melee_Only))
                                            return;
                                    }
                                    if (Player.HasItem(ItemId.Ravenous_Hydra) && Player.CanUseItem((int)ItemId.Ravenous_Hydra))
                                    {
                                        if (Player.UseItem((int)ItemId.Ravenous_Hydra))
                                            return;
                                    }
                                    if (Player.HasItem(ItemId.Tiamat) && Player.CanUseItem((int)ItemId.Tiamat))
                                    {
                                        if (Player.UseItem((int)ItemId.Tiamat))
                                            return;
                                    }
                                    if (Player.HasItem(ItemId.Tiamat_Melee_Only) && Player.CanUseItem((int)ItemId.Tiamat_Melee_Only))
                                    {
                                        if (Player.UseItem((int)ItemId.Tiamat_Melee_Only))
                                            return;
                                    }
                                    if (Player.HasItem(ItemId.Titanic_Hydra) && Player.CanUseItem((int)ItemId.Titanic_Hydra))
                                    {
                                        if (Player.UseItem((int)ItemId.Titanic_Hydra))
                                            return;
                                    }

                                    return;
                                }
                            }
                        }
                    }

                    if (E.IsReady())
                    {
                        if (min.IsValidTarget(1000))
                        {
                            if (E.Cast(min.Position) || E.Cast(min) != CastStates.NotCasted || E.CastOnUnit(min))
                                return;
                        }
                    }
                }
            }
            
        }

        private static void WKS(EventArgs args)
        {
            if (W.IsReady())
            {
                if(GameObjects.EnemyHeroes.Any(i => !i.IsDead && i.DistanceToPlayer() < 400 && i.Health <= W.GetDamage(i)))
                {
                    if (W.Cast())
                    {
                        if (Player.HasItem(ItemId.Ravenous_Hydra_Melee_Only) && Player.CanUseItem((int)ItemId.Ravenous_Hydra_Melee_Only))
                        {
                            if (Player.UseItem((int)ItemId.Ravenous_Hydra_Melee_Only))
                                return;
                        }
                        if (Player.HasItem(ItemId.Ravenous_Hydra) && Player.CanUseItem((int)ItemId.Ravenous_Hydra))
                        {
                            if (Player.UseItem((int)ItemId.Ravenous_Hydra))
                                return;
                        }
                        if (Player.HasItem(ItemId.Tiamat) && Player.CanUseItem((int)ItemId.Tiamat))
                        {
                            if (Player.UseItem((int)ItemId.Tiamat))
                                return;
                        }
                        if (Player.HasItem(ItemId.Tiamat_Melee_Only) && Player.CanUseItem((int)ItemId.Tiamat_Melee_Only))
                        {
                            if (Player.UseItem((int)ItemId.Tiamat_Melee_Only))
                                return;
                        }
                        if (Player.HasItem(ItemId.Titanic_Hydra) && Player.CanUseItem((int)ItemId.Titanic_Hydra))
                        {
                            if (Player.UseItem((int)ItemId.Titanic_Hydra))
                                return;
                        }

                        return;
                    }
                }
            }
        }

        private static bool CheckR;
        private static void RCheck(EventArgs args)
        {
            if(Environment.TickCount - RSetTime < 2000 && Player.GetCurrentAutoAttackRange() < 400)
            {
                Orbwalker.AttackState = false;
                CheckR = false;
            }
            else
            {
                Orbwalker.AttackState = true;
                CheckR = true;
            }
        }

        private static void Combo(EventArgs args)
        {
            if (Player.IsDead) return;

            if (Orbwalker.ActiveMode != OrbwalkerMode.Combo && Orbwalker.ActiveMode != OrbwalkerMode.Harass) return;

            var target = FunnySlayerCommon.FSTargetSelector.GetFSTarget(1000);
            if (target == null) return;

            var OrbTarget = (AIBaseClient)Orbwalker.GetTarget();

            if(OrbTarget != null && !Player.IsDashing() && CheckR)
            {
                if (QEmpCast())
                    return;
            }

            if((Player.HasBuff("RengarQ") || Player.HasBuff("RengarQEmp")) && (beforeaa || onaa))
            {
                if (Player.HasItem(ItemId.Ravenous_Hydra_Melee_Only) && Player.CanUseItem((int)ItemId.Ravenous_Hydra_Melee_Only))
                {
                    if (Player.UseItem((int)ItemId.Ravenous_Hydra_Melee_Only))
                        return;
                }
                if (Player.HasItem(ItemId.Ravenous_Hydra) && Player.CanUseItem((int)ItemId.Ravenous_Hydra))
                {
                    if (Player.UseItem((int)ItemId.Ravenous_Hydra))
                        return;
                }
                if (Player.HasItem(ItemId.Tiamat) && Player.CanUseItem((int)ItemId.Tiamat))
                {
                    if (Player.UseItem((int)ItemId.Tiamat))
                        return;
                }
                if (Player.HasItem(ItemId.Tiamat_Melee_Only) && Player.CanUseItem((int)ItemId.Tiamat_Melee_Only))
                {
                    if (Player.UseItem((int)ItemId.Tiamat_Melee_Only))
                        return;
                }
                if (Player.HasItem(ItemId.Titanic_Hydra) && Player.CanUseItem((int)ItemId.Titanic_Hydra))
                {
                    if (Player.UseItem((int)ItemId.Titanic_Hydra))
                        return;
                }

                return;
            }
            if(Q.Name == "RengarQEmp" && Player.Level < 3)
            {
                if (Q.IsReady() && MenuRengar.QSettings.UseQCombo.Enabled && target.IsValidTarget(300))
                {
                    if (Q.Cast())
                    {
                        Orbwalker.ResetAutoAttackTimer();
                        OnAttack(OrbTarget);
                        return;
                    }
                }
                else
                {
                    if (E.IsReady() && MenuRengar.ESettings.UseECombo.Enabled)
                    {
                        if (target.DistanceToPlayer() > 300)
                        {
                            var pred1 = FSpred.Prediction.Prediction.GetPrediction(E, target);
                            if (pred1.Hitchance >= FSpred.Prediction.HitChance.High && E.Cast(pred1.CastPosition))
                            {
                                return;
                            }

                            var pred2 = E.GetPrediction(target);
                            if (pred2.Hitchance >= EnsoulSharp.SDK.Prediction.HitChance.High && E.Cast(pred2.CastPosition))
                            {
                                return;
                            }
                        }
                    }
                    else
                    {
                        if (W.IsReady() && MenuRengar.WSettings.UseWCombo.Enabled)
                        {

                        }
                    }
                }               
            }

            if (Player.HasBuff("RengarR"))
            {
                if (Player.IsDashing() && CheckR)
                {
                    if(Q.Name == "RengarQEmp")
                    {
                        if (Q.IsReady() && MenuRengar.QSettings.UseQCombo.Enabled)
                        {
                            DelayAction.Add((int)DashTimer, () =>
                            {
                                if (Q.Cast())
                                {
                                    Orbwalker.ResetAutoAttackTimer();
                                    OnAttack(OrbTarget);
                                    return;
                                }
                            });
                        }
                        if (E.IsReady() && MenuRengar.ESettings.UseECombo.Enabled)
                        {
                            if(target.DistanceToPlayer() > 350)
                            {
                                var pred1 = FSpred.Prediction.Prediction.GetPrediction(E, target);
                                if (pred1.Hitchance >= FSpred.Prediction.HitChance.High && E.Cast(pred1.CastPosition))
                                {
                                    return;
                                }

                                var pred2 = E.GetPrediction(target);
                                if (pred2.Hitchance >= EnsoulSharp.SDK.Prediction.HitChance.High && E.Cast(pred2.CastPosition))
                                {
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (Q.IsReady() && MenuRengar.QSettings.UseQCombo.Enabled && OrbTarget != null)
                        {
                            DelayAction.Add((int)DashTimer, () =>
                            {
                                if (Q.Cast())
                                {
                                    Orbwalker.ResetAutoAttackTimer();
                                    OnAttack(OrbTarget);
                                    return;
                                }
                            });
                        }

                        if (W.IsReady() && MenuRengar.WSettings.UseWCombo.Enabled)
                        {
                            if (Player.Mana != 4 && GameObjects.EnemyHeroes.Any(i => i.DistanceToPlayer() <= 350) && W.Cast())
                            {
                                if (Player.HasItem(ItemId.Ravenous_Hydra_Melee_Only) && Player.CanUseItem((int)ItemId.Ravenous_Hydra_Melee_Only))
                                {
                                    if (Player.UseItem((int)ItemId.Ravenous_Hydra_Melee_Only))
                                        return;
                                }
                                if (Player.HasItem(ItemId.Ravenous_Hydra) && Player.CanUseItem((int)ItemId.Ravenous_Hydra))
                                {
                                    if (Player.UseItem((int)ItemId.Ravenous_Hydra))
                                        return;
                                }
                                if (Player.HasItem(ItemId.Tiamat) && Player.CanUseItem((int)ItemId.Tiamat))
                                {
                                    if (Player.UseItem((int)ItemId.Tiamat))
                                        return;
                                }
                                if (Player.HasItem(ItemId.Tiamat_Melee_Only) && Player.CanUseItem((int)ItemId.Tiamat_Melee_Only))
                                {
                                    if (Player.UseItem((int)ItemId.Tiamat_Melee_Only))
                                        return;
                                }
                                if (Player.HasItem(ItemId.Titanic_Hydra) && Player.CanUseItem((int)ItemId.Titanic_Hydra))
                                {
                                    if (Player.UseItem((int)ItemId.Titanic_Hydra))
                                        return;
                                }

                                return;
                            }
                        }

                        if (E.IsReady() && MenuRengar.ESettings.UseECombo.Enabled)
                        {
                            var pred1 = FSpred.Prediction.Prediction.GetPrediction(E, target);
                            if (pred1.Hitchance >= FSpred.Prediction.HitChance.High && E.Cast(pred1.CastPosition))
                            {
                                return;
                            }

                            var pred2 = E.GetPrediction(target);
                            if (pred2.Hitchance >= EnsoulSharp.SDK.Prediction.HitChance.High && E.Cast(pred2.CastPosition))
                            {
                                return;
                            }
                        }
                    }                          
                }
            }
            else
            {
                if ((Player.HasBuff("RengarQ") || Player.HasBuff("RengarQEmp")) && (onaa || beforeaa))
                {
                    if(OrbTarget != null)
                    {
                        if (Player.HasItem(ItemId.Ravenous_Hydra_Melee_Only) && Player.CanUseItem((int)ItemId.Ravenous_Hydra_Melee_Only))
                        {
                            if (Player.UseItem((int)ItemId.Tiamat))
                                return;
                        }
                        if (Player.HasItem(ItemId.Ravenous_Hydra) && Player.CanUseItem((int)ItemId.Ravenous_Hydra))
                        {
                            if (Player.UseItem((int)ItemId.Tiamat))
                                return;
                        }
                        if (Player.HasItem(ItemId.Tiamat) && Player.CanUseItem((int)ItemId.Tiamat))
                        {
                            if (Player.UseItem((int)ItemId.Tiamat))
                                return;
                        }
                        if (Player.HasItem(ItemId.Tiamat_Melee_Only) && Player.CanUseItem((int)ItemId.Tiamat_Melee_Only))
                        {
                            if (Player.UseItem((int)ItemId.Tiamat_Melee_Only))
                                return;
                        }
                        if (Player.HasItem(ItemId.Titanic_Hydra) && Player.CanUseItem((int)ItemId.Titanic_Hydra))
                        {
                            if (Player.UseItem((int)ItemId.Tiamat_Melee_Only))
                                return;
                        }

                        return;
                    }
                }

                if (Player.IsDashing())
                {
                    if (Q.Name == "RengarQEmp")
                    {
                        if (Q.IsReady() && MenuRengar.QSettings.UseQCombo.Enabled && OrbTarget != null)
                        {
                            DelayAction.Add((int)DashTimer, () =>
                            {
                                if (Q.Cast())
                                {
                                    Orbwalker.ResetAutoAttackTimer();
                                    return;
                                }
                            });
                        }
                        if (E.IsReady() && MenuRengar.ESettings.UseECombo.Enabled)
                        {
                            if (target.DistanceToPlayer() > 300)
                            {
                                var pred1 = FSpred.Prediction.Prediction.GetPrediction(E, target);
                                if (pred1.Hitchance >= FSpred.Prediction.HitChance.High && E.Cast(pred1.CastPosition))
                                {
                                    return;
                                }

                                var pred2 = E.GetPrediction(target);
                                if (pred2.Hitchance >= EnsoulSharp.SDK.Prediction.HitChance.High && E.Cast(pred2.CastPosition))
                                {
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (Q.IsReady() && MenuRengar.QSettings.UseQCombo.Enabled && OrbTarget != null)
                        {
                            DelayAction.Add((int)DashTimer, () =>
                            {
                                if (Q.Cast())
                                {
                                    Orbwalker.ResetAutoAttackTimer();
                                    return;
                                }
                            });
                        }

                        if (W.IsReady() && MenuRengar.WSettings.UseWCombo.Enabled)
                        {
                            if (Player.Mana != 4 && GameObjects.EnemyHeroes.Any(i => i.DistanceToPlayer() <= 450) && W.Cast())
                            {
                                if (Player.HasItem(ItemId.Ravenous_Hydra_Melee_Only) && Player.CanUseItem((int)ItemId.Ravenous_Hydra_Melee_Only))
                                {
                                    if (Player.UseItem((int)ItemId.Ravenous_Hydra_Melee_Only))
                                        return;
                                }
                                if (Player.HasItem(ItemId.Ravenous_Hydra) && Player.CanUseItem((int)ItemId.Ravenous_Hydra))
                                {
                                    if (Player.UseItem((int)ItemId.Ravenous_Hydra))
                                        return;
                                }
                                if (Player.HasItem(ItemId.Tiamat) && Player.CanUseItem((int)ItemId.Tiamat))
                                {
                                    if (Player.UseItem((int)ItemId.Tiamat))
                                        return;
                                }
                                if (Player.HasItem(ItemId.Tiamat_Melee_Only) && Player.CanUseItem((int)ItemId.Tiamat_Melee_Only))
                                {
                                    if (Player.UseItem((int)ItemId.Tiamat_Melee_Only))
                                        return;
                                }
                                if (Player.HasItem(ItemId.Titanic_Hydra) && Player.CanUseItem((int)ItemId.Titanic_Hydra))
                                {
                                    if (Player.UseItem((int)ItemId.Titanic_Hydra))
                                        return;
                                }

                                return;
                            }
                        }

                        if (E.IsReady() && MenuRengar.ESettings.UseECombo.Enabled)
                        {
                            var pred1 = FSpred.Prediction.Prediction.GetPrediction(E, target);
                            if (pred1.Hitchance >= FSpred.Prediction.HitChance.High && E.Cast(pred1.CastPosition))
                            {
                                return;
                            }

                            var pred2 = E.GetPrediction(target);
                            if (pred2.Hitchance >= EnsoulSharp.SDK.Prediction.HitChance.High && E.Cast(pred2.CastPosition))
                            {
                                return;
                            }
                        }
                    }
                }
                else
                {
                    if (Q.Name == "RengarQEmp")
                    {
                        if (Q.IsReady() && MenuRengar.QSettings.UseQCombo.Enabled && GameObjects.EnemyHeroes.Any(i => i.IsValidTarget(300) && !i.IsDead))
                        {
                            if (!onaa && (Player.GetRealAutoAttackRange() < 300 || TargetSelector.GetTargets(200) != null))
                            {
                                if(Q.Cast())
                                    Orbwalker.ResetAutoAttackTimer();
                                return;
                            }
                        }
                        if (E.IsReady() && MenuRengar.ESettings.UseECombo.Enabled)
                        {
                            if (target.DistanceToPlayer() > 350)
                            {
                                var pred1 = FSpred.Prediction.Prediction.GetPrediction(E, target);
                                if (pred1.Hitchance >= FSpred.Prediction.HitChance.High && E.Cast(pred1.CastPosition))
                                {
                                    return;
                                }

                                var pred2 = E.GetPrediction(target);
                                if (pred2.Hitchance >= EnsoulSharp.SDK.Prediction.HitChance.High && E.Cast(pred2.CastPosition))
                                {
                                    return;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (Q.IsReady() && MenuRengar.QSettings.UseQCombo.Enabled && OrbTarget != null && MenuRengar.QSettings.QAfterAA.Enabled ? afteraa : (!beforeaa && !onaa))
                        {
                            if ((Player.GetRealAutoAttackRange() < 300 || TargetSelector.GetTargets(200) != null))
                            {
                                if(Q.Cast())
                                    Orbwalker.ResetAutoAttackTimer();
                                return;
                            }
                        }

                        if (W.IsReady() && MenuRengar.WSettings.UseWCombo.Enabled)
                        {
                            if (W.Name == "RengarW" && GameObjects.EnemyHeroes.Any(i => i.DistanceToPlayer() <= 400) && W.Cast())
                            {
                                if (Player.HasItem(ItemId.Ravenous_Hydra_Melee_Only) && Player.CanUseItem((int)ItemId.Ravenous_Hydra_Melee_Only))
                                {
                                    if (Player.UseItem((int)ItemId.Ravenous_Hydra_Melee_Only))
                                        return;
                                }
                                if (Player.HasItem(ItemId.Ravenous_Hydra) && Player.CanUseItem((int)ItemId.Ravenous_Hydra))
                                {
                                    if (Player.UseItem((int)ItemId.Ravenous_Hydra))
                                        return;
                                }
                                if (Player.HasItem(ItemId.Tiamat) && Player.CanUseItem((int)ItemId.Tiamat))
                                {
                                    if (Player.UseItem((int)ItemId.Tiamat))
                                        return;
                                }
                                if (Player.HasItem(ItemId.Tiamat_Melee_Only) && Player.CanUseItem((int)ItemId.Tiamat_Melee_Only))
                                {
                                    if (Player.UseItem((int)ItemId.Tiamat_Melee_Only))
                                        return;
                                }
                                if (Player.HasItem(ItemId.Titanic_Hydra) && Player.CanUseItem((int)ItemId.Titanic_Hydra))
                                {
                                    if (Player.UseItem((int)ItemId.Titanic_Hydra))
                                        return;
                                }

                                return;
                            }
                        }

                        if (E.IsReady() && MenuRengar.ESettings.UseECombo.Enabled)
                        {
                            var pred1 = FSpred.Prediction.Prediction.GetPrediction(E, target);                           
                            if (pred1.Hitchance >= FSpred.Prediction.HitChance.High && E.Cast(pred1.CastPosition))
                            {
                                return;
                            }

                            var pred2 = E.GetPrediction(target);
                            if (pred2.Hitchance >= EnsoulSharp.SDK.Prediction.HitChance.High && E.Cast(pred2.CastPosition))
                            {
                                return;
                            }
                        }
                    }
                }
            }
        }
        private static bool afteraa, onaa, beforeaa;
        //private static AIBaseClient OrbwalkerTarget = null;
        private static void CheckAA(object sender, OrbwalkerActionArgs args)
        {
            if(args.Type == OrbwalkerType.AfterAttack)
            {
                afteraa = true;
                onaa = false;
                beforeaa = false;
            }
            else
            {
                afteraa = false;
            }
            if (args.Type == OrbwalkerType.OnAttack)
            {
                onaa = true;
                afteraa = false;
                beforeaa = false;
            }
            else
            {
                onaa = false;
            }
            if (args.Type == OrbwalkerType.BeforeAttack)
            {
                beforeaa = true;
                onaa = false;
                afteraa = false;
            }
            else
            {
                beforeaa = false;
            }

            //OrbwalkerTarget = (AIBaseClient)args.Target;
        }
        private static void ClearCC(EventArgs args)
        {
            if (!MenuRengar.WSettings.AutoClearCC.Enabled) return;

            if(buffTypes.Any(i => Player.HasBuffOfType(i)))
            {
                if(W.IsReady() && Q.Name == "RengarQEmp")
                {
                    if (W.Cast())
                        return;
                }
            }
        }

        private static void Dash_OnDash(AIBaseClient sender, Dash.DashArgs args)
        {
            if (sender.IsMe || sender.NetworkId == Player.NetworkId || sender.MemoryAddress == Player.MemoryAddress)
            {
                OnRengarDashing = true;
                DashTimer = args.Duration;
                DelayAction.Add(args.Duration, () => OnRengarDashing = false);
            }
            else
            {
                OnRengarDashing = false;
            }
        }
        private static float RSetTime = 0;
        private static void AIHeroClient_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if(sender.IsMe || sender.NetworkId == Player.NetworkId || sender.MemoryAddress == Player.MemoryAddress)
            {
                if(args.Slot == SpellSlot.Q)
                {
                    Orbwalker.ResetAutoAttackTimer();
                }
                if(args.Slot == SpellSlot.R)
                {
                    RSetTime = Environment.TickCount;
                }
            }
        }

        private static List<BuffType> buffTypes = new List<BuffType>
        {
            BuffType.Stun,
            BuffType.Silence,
            BuffType.Taunt,
            BuffType.Polymorph,
            BuffType.Slow,
            BuffType.Snare,
            BuffType.Sleep,
            BuffType.Fear,
            BuffType.Charm,
            BuffType.Suppression,
            BuffType.Blind,
            BuffType.Flee,
            BuffType.Knockup,
            BuffType.Knockback,
            BuffType.Drowsy,
            BuffType.Asleep
        };

        private static Menu RengarMenu = new Menu("FunnySlayer Renger", "Renger Simple", true);
        private static void AddMenu()
        {
            Menu EmpowerdMenu = new Menu("Empowerd Menu", "Empowerd Settings");
            Menu QMenu = new Menu("Q Menu", "Q Settingss");
            Menu WMenu = new Menu("W Menu", "W Settings");
            Menu EMenu = new Menu("E Menu", "E Settings");
            Menu RMenu = new Menu("R Menu", "R Settings");

            EmpowerdMenu.Add(MenuRengar.Empowerd.InCombo);
            EmpowerdMenu.Add(MenuRengar.Empowerd.InHaras);
            MenuRengar.Empowerd.InClear.Add(MenuRengar.Empowerd.LaneClear);
            MenuRengar.Empowerd.InClear.Add(MenuRengar.Empowerd.JungleClear);
            EmpowerdMenu.Add(MenuRengar.Empowerd.InClear);

            QMenu.Add(MenuRengar.QSettings.UseQCombo);
            QMenu.Add(MenuRengar.QSettings.QAfterAA);
            QMenu.Add(MenuRengar.QSettings.UseQHarass);
            QMenu.Add(MenuRengar.QSettings.UseQClear);

            WMenu.Add(MenuRengar.WSettings.UseWCombo);
            WMenu.Add(MenuRengar.WSettings.UseWHarass);
            WMenu.Add(MenuRengar.WSettings.UseWClear);
            WMenu.Add(MenuRengar.WSettings.AutoClearCC);

            EMenu.Add(MenuRengar.ESettings.UseECombo);
            EMenu.Add(MenuRengar.ESettings.UseEHarass);
            EMenu.Add(MenuRengar.ESettings.UseEClear);

            RMenu.Add(MenuRengar.RSettings.Rmanual).Permashow();
            RMenu.Add(MenuRengar.RSettings.RHp);
            RMenu.Add(MenuRengar.RSettings.RRangeMax);
            RMenu.Add(MenuRengar.RSettings.RRangeMin);

            RengarMenu.Add(EmpowerdMenu);
            RengarMenu.Add(QMenu);
            RengarMenu.Add(WMenu);
            RengarMenu.Add(EMenu);
            RengarMenu.Add(RMenu);

            var target = new Menu("target", "Target");
            FunnySlayerCommon.MenuClass.AddTargetSelectorMenu(target);
            RengarMenu.Add(target);
            RengarMenu.Attach();
        }
    }
    internal class MenuRengar
    {       
        public static class Empowerd
        {
            public static MenuBool InCombo = new MenuBool("In Combo", "In Combo");
            public static MenuBool InHaras = new MenuBool("In Harass", "In Harass");

            public static Menu InClear = new Menu("In Clear", "In Clear");
            public static MenuBool LaneClear = new MenuBool("Lane Clear", "Lane Clear", false);
            public static MenuBool JungleClear = new MenuBool("Jungle Clear", "Jungle Clear", false);

        }
        public static class QSettings
        {
            public static MenuBool UseQCombo = new MenuBool("Use Q in Combo", "Q in Combo");
            public static MenuBool QAfterAA = new MenuBool("Q After AA", "Q After AA");
            public static MenuBool UseQHarass = new MenuBool("Use Q in Harass", "Q in Harass");
            public static MenuBool UseQClear = new MenuBool("Use Q in Clear", "Q in Clear");
        }
        public static class WSettings
        {
            public static MenuBool UseWCombo = new MenuBool("Use W in Combo", "W in Combo");
            public static MenuBool UseWHarass = new MenuBool("Use W in Harass", "W in Harass");
            public static MenuBool UseWClear = new MenuBool("Use W in Clear", "W in Clear");

            public static MenuBool AutoClearCC = new MenuBool("Auto Clear CC", "W Empowerd Clear CC");
        }
        public static class ESettings
        {
            public static MenuBool UseECombo = new MenuBool("Use E in Combo", "E in Combo");
            public static MenuBool UseEHarass = new MenuBool("Use E in Harass", "E in Harass");
            public static MenuBool UseEClear = new MenuBool("Use E in Clear", "E in Clear");
        }
        public static class RSettings
        {
            public static MenuBool Rmanual = new MenuBool("R Manual", "R Manual");
            public static MenuSlider RHp = new MenuSlider("R HP", "R HP <= %", 50, 0, 101);
            public static MenuSlider RRangeMax = new MenuSlider("R Range Max", "Max Range", 1500, 0, 2000);
            public static MenuSlider RRangeMin = new MenuSlider("R Range Min", "Min Range", 1000, 0, 1500);
        }
    }
}
