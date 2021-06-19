using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.Utility;
using FunnySlayerCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luian
{
    public class URF_Lucian
    {
        private static AIHeroClient Player => ObjectManager.Player;
        private static Spell Q, W, E;
        private static Spell Qextend;
        private static Menu LucianMain = new Menu("URF_Lucian", "URF Lucian", true);
        public static void LoadLucian()
        {
            Game.Print("Simple Lucian For URF");
            Q = new Spell(SpellSlot.Q, 650f);
            Q.SetTargetted(0.25f, float.MaxValue);

            Qextend = new Spell(SpellSlot.Unknown, 900f);
            Qextend.SetSkillshot(0.25f, 55f, float.MaxValue, false, SpellType.Line);

            W = new Spell(SpellSlot.W, 1000f);
            W.SetSkillshot(0.25f, 80f, 1600f, false, SpellType.Line);

            E = new Spell(SpellSlot.E, 425f);

            LMenu.AddMenu();
            DelayAction.Add(2000, () =>
            {
                LucianMain.Attach();
                Game.Print("Loaded Menu");
            });

            AIBaseClient.OnPlayAnimation += AIBaseClient_OnPlayAnimation;
            AIBaseClient.OnProcessSpellCast += AIBaseClient_OnProcessSpellCast;
            //Game.OnUpdate += AttackType;
            //Game.OnUpdate += Qx;
            //Game.OnUpdate += Wx;
            //Game.OnUpdate += Ex;
            Game.OnUpdate += Rx;
            Game.OnUpdate += Game_OnUpdate;
            Orbwalker.OnAfterAttack += Orbwalker_OnAfterAttack;
        }

        private static void Orbwalker_OnAfterAttack(object sender, AfterAttackEventArgs e)
        {
            LastQWE = 0;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            /*if(Orbwalker.GetTarget() != null)
            {
                if (Passive())
                    Player.IssueOrder(GameObjectOrder.AttackUnit, Orbwalker.GetTarget());
            }*/
            if (Orbwalker.ActiveMode != OrbwalkerMode.Combo)
                return;

            if (FunnySlayerCommon.OnAction.BeforeAA || FunnySlayerCommon.OnAction.OnAA)
                return;

            if (Player.IsDead)
                return;

            if (Variables.GameTimeTickCount - LastQWE <= 1000)
                return;

            var dashPos = Player.Position.Extend(Game.CursorPos, LMenu.ESettings.ECombo.ActiveValue);

            var target = FSTargetSelector.GetFSTarget(ObjectManager.Player.GetCurrentAutoAttackRange());
            if(target != null)
            {
                if (E.IsReady() && LMenu.ESettings.ECombo.Enabled)
                {
                    if (E.Cast(dashPos))
                        return;
                }
                else
                {
                    if (Q.IsReady() && LMenu.QSettings.QCombo.Enabled)
                    {
                        if (Q.Cast(target) == CastStates.SuccessfullyCasted)
                            return;
                    }
                    else
                    {
                        if (W.IsReady() && LMenu.WSettings.WCombo.Enabled)
                        {
                            if (W.Cast(target.Position))
                                return;
                        }
                    }
                }
            }           
        }

        private static int LastQWE = 0;
        private static void AIBaseClient_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs Args)
        {
            if (!sender.IsMe)
            {
                return;
            }

            if(Args.Slot <= SpellSlot.E && Args.Slot != SpellSlot.Unknown)
            {
                LastQWE = Variables.GameTimeTickCount;
            }


            if(Args.Slot <= SpellSlot.R && Args.Slot != SpellSlot.Unknown)
            {
                //Orbwalker.ResetAutoAttackTimer();
            }
            if(Args.Slot <= SpellSlot.W && Args.Slot != SpellSlot.Unknown)
            {
                Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }
        }

        private static void AIBaseClient_OnPlayAnimation(AIBaseClient sender, AIBaseClientPlayAnimationEventArgs Args)
        {
            if (!sender.IsMe || Orbwalker.ActiveMode == OrbwalkerMode.None)
            {
                return;
            }

            if (Args.Animation == "Spell1" || Args.Animation == "Spell2")
            {
                Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }
        }

        private static bool BAA = false;
        private static bool OAA = false;
        private static bool AAA = false;

        private static void AttackType(EventArgs args)
        {
            BAA = FunnySlayerCommon.OnAction.BeforeAA;
            OAA = FunnySlayerCommon.OnAction.OnAA;
            AAA = FunnySlayerCommon.OnAction.AfterAA;


            if (AAA)
            {
                if(Orbwalker.LastTarget != null && !Passive())
                {
                    if(Orbwalker.LastTarget.Type == GameObjectType.AITurretClient || Orbwalker.LastTarget.Type == GameObjectType.BuildingClient)
                    {
                        if (E.IsReady())
                        {
                            if (E.Cast(Game.CursorPos))
                                return;
                        }
                        else
                        {
                            if (W.IsReady())
                            {
                                if (W.Cast(Game.CursorPos))
                                    return;
                            }
                        }
                    }
                }
            }
        }
        private static bool Passive()
        {
            return Player.HasBuff("lucianPassiveBuff") || Variables.GameTimeTickCount - LastQWE <= 2000;
        }

        private static void Rx(EventArgs args)
        {
            if (Player.HasBuff("LucianR"))
            {
                Orbwalker.AttackEnabled = false;
            }
            else
                Orbwalker.AttackEnabled = true;
        }

        private static void Ex(EventArgs args)
        {
            if (Player.IsDead)
                return;

            if (!E.IsReady())
                return;

            var target = Orbwalker.GetTarget() as AIHeroClient;
            if (target != null)
            {
                var dashRange = (Player.PreviousPosition.DistanceToCursor() > Player.GetRealAutoAttackRange(target) ? E.Range : 130);

                var dashPos = Player.PreviousPosition.Extend(Game.CursorPos, E.Range);

                if (Orbwalker.ActiveMode == OrbwalkerMode.Harass && LMenu.ESettings.EHarass.Enabled)
                {
                    if (Passive())
                    {
                        if (AAA)
                        {
                            if (E.Cast(dashPos))
                                return;
                        }
                    }
                    else
                    {
                        if (OAA || BAA)
                            return;

                        if (E.Cast(dashPos))
                            return;
                    }
                }
            }
            var minions = ObjectManager.Get<AIMinionClient>().Where(i => i != null && !i.IsDead && !i.IsAlly && (i.IsMinion() || i.GetJungleType() != JungleType.Unknown));
            if(minions != null || minions.Any())
            {
                if (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear && LMenu.ESettings.EClear.Enabled)
                {
                    if (Passive())
                    {
                        if (AAA)
                        {
                            if (E.Cast(Game.CursorPos))
                                return;
                        }
                    }
                    else
                    {
                        if (OAA || BAA)
                            return;

                        if (E.Cast(Game.CursorPos))
                            return;
                    }
                }
            }            
        }

        private static void Wx(EventArgs args)
        {
            if (Player.IsDead)
                return;

            if (!W.IsReady())
                return;

            var target = TargetSelector.GetTarget(W.Range, DamageType.Physical);
            if(target != null)
            {
                if (Orbwalker.ActiveMode == OrbwalkerMode.Harass && LMenu.WSettings.WHarass.Enabled)
                {
                    if (Passive())
                    {
                        if (AAA)
                        {
                            if (W.Cast(target.Position))
                                return;
                        }
                    }
                    else
                    {
                        if (OAA || BAA)
                            return;

                        if (W.Cast(target.Position))
                            return;
                    }
                }
            }

            var minions = ObjectManager.Get<AIMinionClient>().Where(i => i != null && !i.IsDead && !i.IsAlly && i.IsValidTarget(W.Range) && (i.IsMinion() || i.GetJungleType() != JungleType.Unknown));
            if (minions != null || minions.Any())
            {
                if (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear && LMenu.WSettings.WClear.Enabled)
                {
                    if (Passive())
                    {
                        if (AAA)
                        {
                            if (W.Cast(minions.FirstOrDefault().Position))
                                return;
                        }
                    }
                    else
                    {
                        if (BAA)
                            return;

                        if (W.Cast(minions.FirstOrDefault().Position))
                            return;
                    }
                }
            }               
        }

        private static void Qx(EventArgs args)
        {
            if (Player.IsDead)
                return;

            if (!Q.IsReady())
                return;
            var target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
            if(target != null)
            {
                if (Orbwalker.ActiveMode == OrbwalkerMode.Harass && LMenu.QSettings.QHarass.Enabled)
                {
                    if (Passive())
                    {
                        if (AAA)
                        {
                            if (Q.Cast(target) == CastStates.SuccessfullyCasted)
                                return;
                        }
                    }
                    else
                    {
                        if (OAA || BAA)
                            return;

                        if (Q.Cast(target) == CastStates.SuccessfullyCasted)
                            return;
                    }
                }
            }
            var minions = ObjectManager.Get<AIMinionClient>().Where(i => i != null && !i.IsDead && !i.IsAlly && i.IsValidTarget(Q.Range) && (i.IsMinion() || i.GetJungleType() != JungleType.Unknown));
            if (minions != null || minions.Any())
            {
                if (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear && LMenu.QSettings.QClear.Enabled)
                {
                    if (Passive())
                    {
                        if (AAA)
                        {
                            if (Q.Cast(minions.FirstOrDefault()) == CastStates.SuccessfullyCasted)
                                return;
                        }
                    }
                    else
                    {
                        if (OAA || BAA)
                            return;

                        if (Q.Cast(minions.FirstOrDefault()) == CastStates.SuccessfullyCasted)
                            return;
                    }
                }
            }               
        }

        private class LMenu
        {
            public class QSettings
            {
                public static MenuBool QCombo = new MenuBool("Q_Combo", "Q Combo");
                public static MenuBool QHarass = new MenuBool("Q_Harass", "Q Harass");
                public static MenuBool QClear = new MenuBool("Q_Clear", "Q Clear");
            }
            public class WSettings
            {
                public static MenuBool WCombo = new MenuBool("W_Combo", "W Combo");
                public static MenuBool WHarass = new MenuBool("W_Harass", "W Harass");
                public static MenuBool WClear = new MenuBool("W_Clear", "W Clear");
            }
            public class ESettings
            {
                public static MenuSliderButton ECombo = new MenuSliderButton("E_Combo", "E Combo", 130, 130, 425);
                public static MenuBool EHarass = new MenuBool("E_Harass", "E Harass");
                public static MenuBool EClear = new MenuBool("E_Clear", "E Clear");
            }

            public static void AddMenu()
            {
                var Q = new Menu("Q Settings", "Q Settings")
                {
                    QSettings.QCombo,
                    QSettings.QHarass,
                    QSettings.QClear
                };
                LucianMain.Add(Q);

                var W = new Menu("W Settings", "W Settings")
                {
                    WSettings.WCombo,
                    WSettings.WHarass,
                    WSettings.WClear
                };
                LucianMain.Add(W);

                var E = new Menu("E Settings", "E Settings")
                {
                    ESettings.ECombo,
                    ESettings.EHarass,
                    ESettings.EClear
                };
                LucianMain.Add(E);
                
                var newmenu = new Menu("Hmm", "Hmm");
                FunnySlayerCommon.OnAction.CheckOnAction(false, newmenu);
                FSpred.Prediction.Prediction.Initialize();
                LucianMain.Add(newmenu);
            }
        }
    }
}
