using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.Utility;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominationAIO.NewPlugins
{
    public static class MyRiven
    {
        private static class RivenMenu
        {
            public static class Qmenu
            {
                public static MenuBool UseQ = new MenuBool("Use Q", "Use Q");
                public static MenuBool OnlyAfterAA = new MenuBool("QOnlyAfterAA", "👁 Only Q After AA", false);
                public static MenuBool QForKS = new MenuBool("QForKS", "👁 Q For KS");
                public static MenuList QFocusMode = new MenuList("QFocusMode", "👁 Q Focus On", new string[] { "Target", "Cursor" }, 0);
            }

            public static class Wmenu
            {
                public static MenuBool UseW = new MenuBool("Use W", "Use W");
                public static MenuBool OnlyAfterAA = new MenuBool("WOnlyAfterAA", "👁 Only After AA", false);
                public static MenuBool WForKS = new MenuBool("WForKs", "👁 W For KS");
            }

            public static class Emenu
            {
                public static MenuBool UseE = new MenuBool("Use E", "Use E");
                public static MenuBool OnlyAfterAA = new MenuBool("EOnlyAfterAA", "👁 Only After AA", false);
                public static MenuBool EForGapCloser = new MenuBool("EForGapCloser", "👁 E For GapCloser");
                public static MenuSlider EGapRange = new MenuSlider("EGapRange", "👁 E Gap Range", 400, 250, 400);
            }

            public static class Rmenu
            {
                public static MenuBool UseR1 = new MenuBool("Use R1", "Use R1");
                public static MenuBool UseR2 = new MenuBool("Use R2", "Use R2");
                public static MenuKeyBind ToggleKey = new MenuKeyBind("Accept R1", "Accept R1", EnsoulSharp.SDK.MenuUI.Keys.A, KeyBindType.Toggle);
            }

            public static class Keys
            {
                public static MenuKeyBind BurstCombo = new MenuKeyBind("BurstCombo", "Burst Key", EnsoulSharp.SDK.MenuUI.Keys.U, KeyBindType.Press);
                public static MenuBool AutoSelect = new MenuBool("AutoSelect", "Auto Select Target to Burst", false);
                public static MenuKeyBind FlashW = new MenuKeyBind("FlashW", "Flash W keys", EnsoulSharp.SDK.MenuUI.Keys.T, KeyBindType.Press);
                public static MenuKeyBind Flee = new MenuKeyBind("Flee", "Flee Keys", EnsoulSharp.SDK.MenuUI.Keys.Z, KeyBindType.Press);
            }

            public static class Moving
            {
                public static string[] GetPos = new string[]
                {
                    "Focus On Target",
                    "Focus On Cursor",
                    "Go Behind",
                    "Find Logic Pos",
                };

                public static MenuBool ActiveMove = new MenuBool("ActiveMove", "Active Move");
                public static MenuBool CheckForTarget = new MenuBool("CheckForTarget", "Checking For Target");
                public static MenuSlider MoveTime = new MenuSlider("MoveTime", "Move Time", 260, 100, 300);
                public static MenuList MovePos = new MenuList("MovePos", "Move Pos", GetPos, 0);
            }
        }

        private static bool IsQ1
            => ObjectManager.Player.GetBuffCount(Q.Name) <= 0;

        private static bool IsQ2
            => ObjectManager.Player.GetBuffCount(Q.Name) == 1;

        private static bool IsQ3
            => ObjectManager.Player.GetBuffCount(Q.Name) == 2;

        private static bool IsR1
            => R.Name == "RivenFengShuiEngine";

        private static bool IsR2
            => R.Name == "RivenIzunaBlade";

        private static Spell Q = new Spell(SpellSlot.Q, 250f);
        private static Spell W = new Spell(SpellSlot.W, 250f);
        private static Spell E = new Spell(SpellSlot.E, 250f);
        private static Spell R = new Spell(SpellSlot.R, 900f);
        private static SpellSlot flash = ObjectManager.Player.GetSpellSlot("summonerflash");
        private static SpellSlot ignite = ObjectManager.Player.GetSpellSlot("summonerdot");
        private static Spell Flash, Ignite;
        private static Menu RMenu = new Menu("Riven Menu", "FunnySlayer Riven", true);

        public static void LoadRiven()
        {
            R.SetSkillshot(0.25f, 200f, 1600, false, SpellType.Line);

            if(flash != SpellSlot.Unknown)
            {
                Flash = new Spell(flash, 400f);
            }

            if(ignite != SpellSlot.Unknown)
            {
                Ignite = new Spell(ignite, 600f);
            }

            try
            {
                var Qcombo = new Menu("QCombo", "Q Settings");
                Qcombo.Add(RivenMenu.Qmenu.UseQ);
                Qcombo.Add(RivenMenu.Qmenu.OnlyAfterAA);
                Qcombo.Add(RivenMenu.Qmenu.QForKS);
                Qcombo.Add(RivenMenu.Qmenu.QFocusMode);

                var Wcombo = new Menu("WCombo", "W Settings");
                Wcombo.Add(RivenMenu.Wmenu.UseW);
                Wcombo.Add(RivenMenu.Wmenu.OnlyAfterAA);
                Wcombo.Add(RivenMenu.Wmenu.WForKS);


                var Ecombo = new Menu("Ecombo", "E Settings");
                Ecombo.Add(RivenMenu.Emenu.UseE);
                Ecombo.Add(RivenMenu.Emenu.OnlyAfterAA);
                Ecombo.Add(RivenMenu.Emenu.EForGapCloser);
                Ecombo.Add(RivenMenu.Emenu.EGapRange);


                var rcombo = new Menu("Rcombo", "R Settings");
                rcombo.Add(RivenMenu.Rmenu.ToggleKey).Permashow();
                rcombo.Add(RivenMenu.Rmenu.UseR1);
                rcombo.Add(RivenMenu.Rmenu.UseR2);


                var key = new Menu("key", "Keys Settings");
                key.Add(RivenMenu.Keys.BurstCombo).Permashow();
                key.Add(RivenMenu.Keys.AutoSelect).Permashow();
                key.Add(RivenMenu.Keys.FlashW).Permashow();
                key.Add(RivenMenu.Keys.Flee).Permashow();

                var move = new Menu("move", "Moving Settings");
                move.Add(RivenMenu.Moving.ActiveMove);
                move.Add(RivenMenu.Moving.CheckForTarget);
                move.Add(RivenMenu.Moving.MovePos);
                move.Add(RivenMenu.Moving.MoveTime);


                RMenu.Add(Qcombo);
                RMenu.Add(Wcombo);
                RMenu.Add(Ecombo);
                RMenu.Add(rcombo);
                RMenu.Add(key);
                RMenu.Add(move);
            }
            catch
            {
                Game.Print("FunnySlayer Riven Menu Error");
            } 

            RMenu.Attach();
            
            AIBaseClient.OnProcessSpellCast += AIBaseClient_OnProcessSpellCast;
            Game.OnUpdate += Game_OnUpdate;

            //Check
            //Game.OnUpdate += Check;

            //DoCombo
            Orbwalker.OnAfterAttack += Orbwalker_OnAfterAttack;
            Game.OnUpdate += Game_OnUpdate1;

            //DoKeys
            Game.OnUpdate += Game_OnUpdate3;

            //Interrupt
            Dash.OnDash += Dash_OnDash;
            AntiGapcloser.OnGapcloser += AntiGapcloser_OnGapcloser;
            Interrupter.OnInterrupterSpell += Interrupter_OnInterrupterSpell;
            Game.OnUpdate += Game_OnUpdate2;

            //Drawing
            Drawing.OnEndScene += Drawing_OnEndScene;
        }

        private static void Game_OnUpdate3(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
                return;

            //Logic Burst
            if (RivenMenu.Keys.BurstCombo.Active)
            {
                /*if (Flash.IsReady())
                    ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);*/

                var target = TargetSelector.SelectedTarget;

                if (target == null || target.IsValidTarget(1000))
                {
                    target = TargetSelector.GetTarget(1000, DamageType.Physical);
                }

                if (target != null && target.IsValidTarget(800))
                {
                    if (Flash.IsReady()
                    && R.IsReady() && ObjectManager.Player.Distance(target) >= E.Range + W.Range && (ObjectManager.Player.Distance(target.Position) <= 800) && E.IsReady() && W.IsReady())
                    {                        
                        var poscast = target.Position;

                        if (Q.IsReady())
                            poscast = target.Position.Extend(ObjectManager.Player.Position, 125);

                        E.Cast(poscast);
                        R.DelayPosCast(poscast, 1);
                        DelayAction.Add(210, () =>
                        {
                            W.DelayPosCast(ObjectManager.Player.Position);
                            Flash.DelayPosCast(poscast, 1);
                        });
                     
                        if (Q.IsReady())
                        {
                            Q.DelayTargetCast(target, 285);
                        }

                        return;
                    }
                }
            }

            if (RivenMenu.Keys.FlashW.Active)
            {
                if (W.IsReady() && Flash.IsReady())
                {
                    ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

                    var target = FunnySlayerCommon.FSTargetSelector.GetFSTarget(Flash.Range + W.Range - 100);

                    if(target != null)
                    {
                        W.DelayPosCast(target.Position);
                        Flash.DelayPosCast(FSpred.Prediction.Prediction.PredictUnitPosition(target, 100).ToVector3(), 1);
                    }
                }
            }

            if (RivenMenu.Keys.Flee.Active)
            {
                ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

                if (E.IsReady() && (IsQ1 || IsQ3))
                {
                    E.Cast(Game.CursorPos);
                }
                if (Q.IsReady())
                {
                    Q.Cast(Game.CursorPos);
                }              
            }
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
                return;

            Render.Circle.DrawCircle(ObjectManager.Player.Position, Q.Range, System.Drawing.Color.Red);
        }

        private static void Check(EventArgs args)
        {
            if (IsQ1)
                Game.Print("Q1");

            if (IsQ2)
                Game.Print("Q2");

            if (IsQ3)
                Game.Print("Q3");
        }

        private static bool CastQ(AIBaseClient target)
        {
            if(RivenMenu.Qmenu.QFocusMode.Index == 0)
            {
                Q.DelayTargetCast(target);
                return true;
            }
            else
            {
                Q.DelayPosCast(target.Position);
                return true;
            }

            return false;
        }

        private static void Orbwalker_OnAfterAttack(object sender, AfterAttackEventArgs e)
        {
            if (Orbwalker.ActiveMode <= OrbwalkerMode.Harass && RivenMenu.Qmenu.UseQ.Enabled)
            {
                if(Q.IsReady() && e.Target.Type == GameObjectType.AIHeroClient && e.Target.IsValidTarget(Q.Range))
                {
                    CastQ((AIBaseClient)e.Target);
                    UpdateMovePos(e.Target.Position);
                    return;
                }
            }

            if(Orbwalker.ActiveMode == OrbwalkerMode.LaneClear)
            {
                if(Q.IsReady() && e.Target != null && e.Target.Health >= Q.GetDamage(e.Target as AIBaseClient) && e.Target.IsValidTarget(Q.Range) && e.Target.IsJungle())
                {
                    CastQ((AIBaseClient)e.Target);
                    UpdateMovePos(e.Target.Position);
                    return;
                }
            }
        }

        private static void Game_OnUpdate2(EventArgs args)
        {
            var targets = GameObjects.EnemyHeroes.Where(i => !i.IsDead && i.IsValidTarget(W.Range));
            if (W.IsReady())
            {
                if (targets.Any(i => i.IsCastingImporantSpell()))
                    WCastOnTarget(targets.FirstOrDefault(i => i.IsCastingImporantSpell()));

                if (targets.Any(i => i.IsDashing()))
                    WCastOnTarget(targets.FirstOrDefault(i => i.IsDashing()));

                if (targets.Any(i => i.GetGapcloserInfo().EndPosition != Vector3.Zero))
                    WCastOnTarget(targets.FirstOrDefault(i => i.GetGapcloserInfo() != null));

                var Wtarget = targets.Where(i => i.Health <= W.GetDamage(i) + (Orbwalker.ActiveMode == OrbwalkerMode.Combo ? (ObjectManager.Player.GetAutoAttackDamage(i) + (Q.IsReady() ? Q.GetDamage(i) * 2 : 0)) : 0));
                if (Wtarget.FirstOrDefault() != null)
                {
                    WCastOnTarget(Wtarget.FirstOrDefault());
                }            
            }
        }

        private static void Interrupter_OnInterrupterSpell(AIHeroClient sender, Interrupter.InterruptSpellArgs args)
        {
            WCastOnTarget(sender);
        }

        private static void AntiGapcloser_OnGapcloser(AIHeroClient sender, AntiGapcloser.GapcloserArgs args)
        {
            WCastOnTarget(sender);
        }

        private static void Dash_OnDash(AIBaseClient sender, Dash.DashArgs args)
        {
            WCastOnTarget(sender);
        }
        private static void WCastOnTarget(AIBaseClient sender)
        {
            if (W.IsReady() && sender.IsValidTarget() && sender.IsEnemy)
            {
                if (sender.DistanceToPlayer() <= W.Range)
                {
                    W.Cast();
                    return;
                }
            }
        }

        private static void Game_OnUpdate1(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    DoCombo();
                    return;
                case OrbwalkerMode.Harass:
                    return;
                case OrbwalkerMode.LaneClear:
                    return;
                case OrbwalkerMode.LastHit:
                    return;
                default:
                    return;
            }
        }

        private static void DoCombo()
        {
            var target = FunnySlayerCommon.FSTargetSelector.GetFSTarget(R.Range);

            if (FunnySlayerCommon.OnAction.BeforeAA  && target.InCurrentAutoAttackRange() && R.IsReady() && IsR2 && RivenMenu.Rmenu.UseR2.Enabled)
            {
                if(target.Health <= GetRDmg(target) + ObjectManager.Player.GetAutoAttackDamage(target))
                {
                    var pred = FSpred.Prediction.Prediction.GetPrediction(R, target);

                    if (pred.Hitchance >= FSpred.Prediction.HitChance.High)
                    {
                        R.Cast(pred.CastPosition);
                        return;
                    }
                }
            }

            if ((!FunnySlayerCommon.OnAction.BeforeAA && !FunnySlayerCommon.OnAction.OnAA))
            {
                if (E.IsReady() && RivenMenu.Emenu.UseE.Enabled)
                {
                    if(RivenMenu.Emenu.EForGapCloser.Enabled && target.DistanceToPlayer() <= RivenMenu.Emenu.EGapRange.Value)
                    {
                        DoE(target);
                        return;
                    }

                    if (!RivenMenu.Emenu.OnlyAfterAA.Enabled || FunnySlayerCommon.OnAction.AfterAA)
                    {
                        DoE(target);
                        return;
                    }
                }

                if (R.IsReady() && (RivenMenu.Rmenu.ToggleKey.Active || target.Health <= CalculatorComboDmg(target)))
                {
                    if (IsR1 && RivenMenu.Rmenu.UseR1.Enabled)
                    {
                        R.Cast(target.Position);
                        return;
                    }

                    if (IsR2 && target.Health <= GetRDmg(target) && RivenMenu.Rmenu.UseR2.Enabled)
                    {
                        var pred = FSpred.Prediction.Prediction.GetPrediction(R, target);

                        if(pred.Hitchance >= FSpred.Prediction.HitChance.High)
                        {
                            R.Cast(pred.CastPosition);
                            return;
                        }
                    }
                }

                if (RivenMenu.Qmenu.UseQ.Enabled)
                {
                    if (IsQ1 || IsQ2)
                    {
                        goto CastQ12;
                    }
                    else
                    {
                        goto CastQ3;
                    }
                }

                CastQ12:
                DoQ(); 
                
                CastQ3:
                if (W.IsReady() && RivenMenu.Wmenu.UseW.Enabled && (Variables.GameTimeTickCount - LastQCast >= 1000 || !IsQ2))
                {
                    if(!RivenMenu.Wmenu.OnlyAfterAA.Enabled || FunnySlayerCommon.OnAction.AfterAA)
                    {
                        var targets = GameObjects.EnemyHeroes.Where(i => !i.IsDead && i.IsValidTarget(W.Range));
                        if (targets.FirstOrDefault() != null)
                        {
                            WCastOnTarget(targets.FirstOrDefault());
                            return;
                        }
                    }
                }

                DoQ();
                return;
            }
        }

        private static bool DoQ()
        {
            if (IsQ1 && !Q.IsReady())
                return false;

            if(FunnySlayerCommon.OnAction.AfterAA || !RivenMenu.Qmenu.OnlyAfterAA.Enabled)
            {
                var targets = GameObjects.EnemyHeroes.Where(i => !i.IsDead && i.IsValidTarget(Q.Range)).OrderBy(i => i.Health + i.PhysicalShield - ObjectManager.Player.GetAutoAttackDamage(i));
                if (targets.FirstOrDefault() != null)
                {
                    return CastQ(targets.FirstOrDefault());
                }
            }

            return false;
        }

        private static bool DoE(AIBaseClient target)
        {
            if(target != null && E.IsReady() && !R.IsReady() ? target.IsValidTarget(RivenMenu.Emenu.EGapRange.Value) : target.IsValidTarget(900))
            {
                E.Cast(target.Position);

                if(R.IsReady() && IsR1 && target.Health <= CalculatorComboDmg(target))
                {
                    R.DelayPosCast(target.Position, 1);

                    if (Q.IsReady())
                        Q.DelayPosCast(target.Position, 205);
                }

                return true;
            }

            return false;
        }

        private static double CalculatorComboDmg(AIBaseClient target)
        {
            double thisdmg = 0;

            if (Q.IsReady())
            {
                if (IsQ1)
                {
                    thisdmg += 3 * Q.GetDamage(target) + ObjectManager.Player.GetAutoAttackDamage(target);
                }

                if (IsQ2)
                {
                    thisdmg += 2 * Q.GetDamage(target) + ObjectManager.Player.GetAutoAttackDamage(target);
                }

                if (IsQ3)
                {
                    thisdmg += Q.GetDamage(target) + ObjectManager.Player.GetAutoAttackDamage(target);
                }
            }

            if (W.IsReady())
                thisdmg += W.GetDamage(target);

            if(E.IsReady())
                thisdmg += ObjectManager.Player.GetAutoAttackDamage(target);

            if (R.IsReady())
                thisdmg += GetRDmg(target, RGetDmgState.Max);

            return thisdmg;
        }
        private enum RGetDmgState
        {
            Normal,
            Min,
            Max
        }

        private static double GetRDmg(AIBaseClient target, RGetDmgState state)
        {
            double thisdmg = 0;
            var baserdmg = new double[]
            {
                0, 100, 150, 200,
            };
            switch (state)
            {
                case RGetDmgState.Max:
                    thisdmg = 1.8 * ObjectManager.Player.GetBonusPhysicalDamage() + baserdmg[R.Level] * 3;
                    return ObjectManager.Player.CalculatePhysicalDamage(target, thisdmg);
                case RGetDmgState.Min:
                    thisdmg = 0.6 * ObjectManager.Player.GetBonusPhysicalDamage() + baserdmg[R.Level];
                    return ObjectManager.Player.CalculatePhysicalDamage(target, thisdmg);
                default:
                    return GetRDmg(target);
            }
        }

        private static double GetRDmg(AIBaseClient target)
        {
            double thisdmg = 0;

            var baserdmg = new double[]
            {
                0, 100, 150, 200, 
            };

            var bonusdmg = 0.6 * ObjectManager.Player.GetBonusPhysicalDamage();

            var missingheath = 100 * ((target.MaxHealth - target.Health) / target.MaxHealth);

            var baseheath = new double[]
            {
                0, 9.38, 18.75, 28.13, 37.5, 46.88, 56.25, 65.63, 75, 101
            };

            var min = baseheath.Where(i => i - missingheath >= 0).ToArray().Min();

            var index = Array.IndexOf(baseheath, min);

            var baseheathdmg = new double[]
            {
                0, 0.25, 0.5, 0.75, 1, 0.125, 0.15, 0.175, 2, 2
            };

            thisdmg = (bonusdmg + baserdmg[R.Level]) * (1 + baseheathdmg[index]);


            return ObjectManager.Player.CalculatePhysicalDamage(target, thisdmg);
        }
        private static void Game_OnUpdate(EventArgs args)
        {
            UpdateSkillRange();

            if(Variables.GameTimeTickCount - LastDisableMove > RivenMenu.Moving.MoveTime.Value)
            {
                Orbwalker.MoveEnabled = true;
            }
        }

        private static void AIBaseClient_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if(sender.NetworkId == ObjectManager.Player.NetworkId)
            {
                if(args.Slot == SpellSlot.Q)
                {
                    Orbwalker.ResetAutoAttackTimer();
                    LastQCast = Variables.GameTimeTickCount - 5;
                    UpdateMovePos(ObjectManager.Player.Position.Extend(args.To, 300));
                    SetMove(RivenMenu.Moving.MoveTime.Value);

                    return;
                }

                if(args.Slot == SpellSlot.R && Orbwalker.ActiveMode == OrbwalkerMode.Combo)
                {
                    if (Q.IsReady())
                        Q.Cast(args.To);
                }
            }
        }

        private static void UpdateSkillRange()
        {           
            if (IsR2)
            {
                W.Range = 300;

                if (IsQ1 || IsQ2)
                {
                    Q.Range = 250f + ObjectManager.Player.GetRealAutoAttackRange() - 75;
                }
                else
                {
                    Q.Range = 300f + ObjectManager.Player.GetRealAutoAttackRange() - 75;
                }
            }
            else
            {
                W.Range = 150f + ObjectManager.Player.GetRealAutoAttackRange() - 75;

                if (IsQ1 || IsQ2)
                {
                    Q.Range = 150f + ObjectManager.Player.GetRealAutoAttackRange() - 75;
                }
                else
                {
                    Q.Range = 250f + ObjectManager.Player.GetRealAutoAttackRange() - 75;
                }
            }
        }

        private static Vector3 MovePos = Vector3.Zero;
        private static void UpdateMovePos(Vector3 pos)
        {
            var target = FunnySlayerCommon.FSTargetSelector.GetFSTarget(Q.Range);
            if(RivenMenu.Moving.MovePos.Index == 0)
            {
                if(target != null)
                {
                    MovePos = pos;
                }
                else
                {
                    MovePos = ObjectManager.Player.Position.Extend(pos, -250);
                }
            }

            if (RivenMenu.Moving.MovePos.Index == 1)
            {
                MovePos = Game.CursorPos;
            }

            if (RivenMenu.Moving.MovePos.Index == 2)
            {
                MovePos = ObjectManager.Player.Position.Extend(pos, -250);
            }

            if (RivenMenu.Moving.MovePos.Index == 3)
            {
                if(target != null)
                {
                    var getpos = RotatedVector(ObjectManager.Player.Position.ToVector2(), pos.ToVector2(), 70).ToVector3();
                    MovePos = ObjectManager.Player.Position.Extend(getpos, 250);
                }
                else
                {
                    var getpos = RotatedVector(ObjectManager.Player.Position.ToVector2(), Game.CursorPos.ToVector2(), 70).ToVector3();
                    MovePos = ObjectManager.Player.Position.Extend(getpos, 250);
                }
            }
        }

        private static Vector2 RotatedVector(Vector2 start, Vector2 end, int alpha)
        {
            float beta = alpha * (float)Math.PI / 180;
            var cp = start +
                     (end - start).Rotated
                         (beta);

            return cp;
        }

        private static void SetMove(int Maxtime = 260)
        {
            if(Orbwalker.ActiveMode == OrbwalkerMode.Combo || Orbwalker.ActiveMode == OrbwalkerMode.Harass || Orbwalker.ActiveMode == OrbwalkerMode.LaneClear)
            {
                if(Variables.GameTimeTickCount - LastQCast >= 0 || Variables.GameTimeTickCount - LastQCast <= Maxtime)
                {
                    DoMove(Maxtime);
                    return;
                }
            }
        }

        private static int LastDisableMove = 0;
        private static int LastMove = 0;
        private static void DoMove(int MoveTime = 260)
        {
            if (!RivenMenu.Moving.ActiveMove.Enabled)
                return;

            if (RivenMenu.Moving.CheckForTarget.Enabled)
            {
                if(Orbwalker.ActiveMode <= OrbwalkerMode.Harass)
                {
                    if (ObjectManager.Get<AIHeroClient>().Where(i => i.IsValidTarget(Q.Range) && !i.IsDead && !i.IsAlly).FirstOrDefault() == null)
                        return;
                }

                if(Orbwalker.ActiveMode <= OrbwalkerMode.LastHit && Orbwalker.ActiveMode > OrbwalkerMode.Harass)
                {
                    if (ObjectManager.Get<AIMinionClient>().Where(i => i.IsValidTarget(Q.Range) && !i.IsDead && !i.IsAlly).FirstOrDefault() == null)
                        return;
                }
            }

            if (Orbwalker.MoveEnabled)
            {
                Orbwalker.MoveEnabled = false;
                LastDisableMove = Variables.GameTimeTickCount;
            }

            LastMove = Variables.GameTimeTickCount;

            Orbwalker.AttackEnabled = false;
            DelayAction.Add(MoveTime, () =>
            {
                Orbwalker.AttackEnabled = true;
                Orbwalker.ResetAutoAttackTimer();
            });

            for (int i = 0; i <= MoveTime; i++)
            {
                DelayAction.Add(i, () =>
                {
                    Orbwalker.MoveEnabled = false;
                    ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, MovePos);
                });
            }

            return;
        }

        private static int LastQCast = 0;
    }

    public static class RivenHelp
    {
        public static void DelayTargetCast(this Spell spell, AIBaseClient target = null, int time = 0)
        {
            if (spell.IsReady())
                DelayAction.Add(time, () =>
                {
                    spell.CastOnUnit(target);
                });
        }

        public static void DelayPosCast(this Spell spell, Vector3 pos, int time = 0)
        {
            if (spell.IsReady())
                DelayAction.Add(time, () =>
                {
                    spell.Cast(pos);
                });
        }
    }
}
