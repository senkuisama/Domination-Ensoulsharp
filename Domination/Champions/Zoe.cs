using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp;
using EnsoulSharp.SDK;
using SebbyLibPorted.Prediction;
using SharpDX;
using EnsoulSharp.SDK.Utility;

namespace DominationAIO.Champions
{
    public static class Zoe
    {
        private static Menu ZoeMenu;
        private static Spell Q, E, R;
        private static Spell SpellCheckQ1;
        public static void Load()
        {
            if (GameObjects.Player == null)
                return;

            Q = new Spell(SpellSlot.Q, 800);
            Q.SetSkillshot(0.3f, 150, 2000, true, EnsoulSharp.SDK.Prediction.SkillshotType.Line);
            E = new Spell(SpellSlot.E, 800);
            E.SetSkillshot(0.25f, 100, 800, true, EnsoulSharp.SDK.Prediction.SkillshotType.Line);
            R = new Spell(SpellSlot.R, 575);

            SpellCheckQ1 = new Spell(SpellSlot.Unknown, 800f);
            SpellCheckQ1.SetSkillshot(0.25f, 80, 2000, true, EnsoulSharp.SDK.Prediction.SkillshotType.Line);

            ZoeMenu = new Menu("ZoeMenu", "FunnySlayer Zoe", true);
            var ZoeHelper = new Menu("Zoe_Helper", "Helper");
            new SebbyLibPorted.Orbwalking.Orbwalker(ZoeHelper);
            ZoeMenu.Add(ZoeHelper);
            ZoeMenu.Add(Qcombo);
            ZoeMenu.Add(Ecombo);
            ZoeMenu.Add(Rcombo);
            ZoeMenu.Attach();

            Game.Print("Zoe Beta");

            Game.OnUpdate += Checker;
            Game.OnUpdate += DoOrb;
            Game.OnUpdate += Game_OnUpdate1;
            Game.OnUpdate += Game_OnUpdate;
            AIHeroClient.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            Drawing.DrawCircle(GameObjects.Player.Position, MoveRange, System.Drawing.Color.White);
        }

        private static int QTimer = 0;
        private static Vector3 QVector = Vector3.Zero;
        private static bool Q2Now = false;
        private static void AIHeroClient_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if(sender.IsMe || sender.NetworkId == GameObjects.Player.NetworkId)
            {
                if (args.SData.Name == "ZoeQ" || args.Slot == SpellSlot.Q)
                {
                    QTimer = Variables.TickCount;
                }
                if (args.SData.Name == "ZoeQMissile")
                {                    
                    QVector = args.End;
                }              
            }
        }

        private static void Checker(EventArgs args)
        {
            if (Q2())
                Q.Range = 1000f;
            else
                Q.Range = 800f;

            if(QTimer + QFlyingTime < Variables.TickCount)
            {
                Q2Now = true;
            }
            else
            {
                Q2Now = false;
            }

            if (R.IsReady())
                MoveRange = GameObjects.Player.MoveSpeed * 0.5f + R.Range;
            else
                MoveRange = GameObjects.Player.MoveSpeed * 0.5f;


            if (QClient() != null)
            {               
                Q.UpdateSourcePosition(QClient().Position, GameObjects.Player.Position);
                
                if (Q2())
                {
                    var QGeometry = new Geometry.Rectangle(QClient().Position, QVector, Q.Width + 50);

                    if (QGeometry != null)
                    {
                        var CheckCollision = GameObjects.EnemyMinions.Any(i => i.IsValid() && !i.IsDead && QGeometry.IsInside(i));
                        if(CheckCollision == true)
                        {
                            Q2Now = true;
                        }
                    }
                }
            }
            else
            {
                Q.UpdateSourcePosition(GameObjects.Player.Position, GameObjects.Player.Position);
            }
        }

        private static void Game_OnUpdate1(EventArgs args)
        {
            if (GameObjects.Player.IsDead)
                return;

            //Game.Print(QTimer + QFlyingTime + "  ----  " + Variables.TickCount);

            if(Orbwalker.GetTarget() != null)
            {
                if((Orbwalker.GetTarget() as AIBaseClient).HasBuff("zoeesleepstun") && Q.IsReady())
                {
                    Orbwalker.AttackState = false;
                }
                else
                {
                    Orbwalker.AttackState = true;
                }
            }

            if (Orbwalker.ActiveMode > OrbwalkerMode.Harass)
                return;

            var EclientPos = Vector3.Zero;
            if (EClient() != null || EStunClient() != null)
            {                
                if(EClient() != null)
                {
                    EclientPos = EClient().Position;
                }
                if(EStunClient() != null)
                {
                    EclientPos = EStunClient().Position;
                }

                if (EclientPos.DistanceToPlayer() < Q.Range + MoveRange)
                {
                    if (Q.IsReady())
                    {
                        if (Q2())
                        {
                            if (EclientPos.DistanceToPlayer() > Q.Range && QTimer + QFlyingTime < Variables.TickCount)
                            {
                                if (Q.Cast(EclientPos, true))
                                {
                                    DelayAction.Add(200, () =>
                                    {
                                        if (R.Cast(EclientPos, true))
                                            return;
                                    });                                    
                                }
                            }
                            else
                            {
                                if(QTimer + QFlyingTime < Variables.TickCount)
                                    if (Q.Cast(EclientPos, true))
                                        return;
                            }
                        }

                        if (!Q2())
                        {
                            var GetCastPos = Vector3.Zero;
                            var QGetPoss = new Geometry.Circle(GameObjects.Player.Position, 800f);
                            foreach (var QGetPos in QGetPoss.Points.OrderByDescending(i => i.Distance(EclientPos)))
                            {
                                QCollision = new Geometry.Rectangle(GameObjects.Player.Position.ToVector2(), QGetPos, Q.Width + 50);
                                var CheckCollision = GameObjects.EnemyMinions.Any(i => i.IsValid() && !i.IsDead && QCollision.IsInside(i));
                                if (CheckCollision == false)
                                {
                                    //Check For Collision
                                    {
                                        var QGeometry = new Geometry.Rectangle(QGetPos.ToVector3(), EclientPos, Q.Width * 2);

                                        if (QGeometry != null)
                                        {
                                            var CheckForCollision = GameObjects.EnemyMinions.Any(i => i.IsValid() && !i.IsDead && QGeometry.IsInside(i));
                                            if (CheckForCollision == false)
                                            {
                                                //Add Pos
                                                GetCastPos = QGetPos.ToVector3();
                                                GetBool = false;
                                                if (Q.Cast(GetCastPos))
                                                    break;
                                            }
                                            else
                                            {
                                                GetBool = true;
                                                continue;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private static Geometry.Polygon QCollision = null;
        private static bool GetBool = true;
        private static void DoOrb(EventArgs args)
        {
            if (GameObjects.Player.IsDead)
                return;

            if (Orbwalker.ActiveMode > OrbwalkerMode.Harass)
                return;

            var target = TargetSelector.GetTarget(Q.Range + MoveRange, DamageType.Magical);
            if (target == null)
                return;

            var Epred = Prediction.GetPrediction(E, target);
            if (E.IsReady() && Epred.Hitchance >= HitChance.High && Ecombo.Enabled)
            {
                if (E.Cast(Epred.CastPosition))
                    return;
            }
            else
            {
                if (Q.IsReady() && Qcombo.Enabled)
                {
                    var Qpred = Prediction.GetPrediction(Q, target);
                    if (Q2())
                    {
                        if(Q2Now)
                        {
                            var CastPos = Qpred.CastPosition;
                            if(CastPos.DistanceToPlayer() > Q.Range)
                            {
                                if (R.IsReady())
                                {
                                    if (Q.Cast(CastPos, true))
                                    {
                                        DelayAction.Add(200, () =>
                                        {
                                            if (R.Cast(CastPos, true))
                                                return;
                                        });
                                    }
                                }
                            }
                            else
                            {
                                if (Q.Cast(CastPos, true))
                                    return;
                            }
                        }
                    }
                    else
                    {
                        if(EDownClient() == null || EDownClient().DistanceToPlayer() > MoveRange + Q.Range)
                        {
                            var GetCastPos = Vector3.Zero;
                            var QGetPoss = new Geometry.Circle(GameObjects.Player.Position, 800f);
                            foreach (var QGetPos in QGetPoss.Points.OrderByDescending(i => i.Distance(Qpred.CastPosition)))
                            {
                                QCollision = new Geometry.Rectangle(GameObjects.Player.Position.ToVector2(), QGetPos, Q.Width + 50);
                                var CheckCollision = GameObjects.EnemyMinions.Any(i => i.IsValid() && !i.IsDead && QCollision.IsInside(i));
                                if (CheckCollision == false)
                                {
                                    //Check For Collision
                                    {
                                        var QGeometry = new Geometry.Rectangle(QGetPos.ToVector3(), Qpred.CastPosition, Q.Width + 50);

                                        if (QGeometry != null)
                                        {
                                            var CheckForCollision = GameObjects.EnemyMinions.Any(i => i.IsValid() && !i.IsDead && QGeometry.IsInside(i));
                                            if (CheckForCollision == false)
                                            {
                                                //Add Pos
                                                GetCastPos = QGetPos.ToVector3();
                                                GetBool = false;
                                                if (Q.Cast(GetCastPos))
                                                    break;
                                            }
                                            else
                                            {
                                                GetBool = true;
                                                continue;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }                
            }
        }

        private static bool Q2()
        {
            return Q.Name == "ZoeQRecast";
        }
        private static MissileClient QClient()
        {       
            var Qc = GameObjects.Get<MissileClient>().Where(i => i.Position.IsValid() && i.Name == QMissile);
            MissileClient Get = null;
            foreach (var hmm in Qc)
            {
                Get = hmm;
                return hmm;
            }

            if (Get != null)
                return Get;
            else
                return null;
        }
        private static AIMinionClient EClient()
        {       
            var Ec = GameObjects.Get<AIMinionClient>().Where(i => i.Position.IsValid() && i.Name == "TestCubeRender");
            AIMinionClient Get = null;
            foreach (var hmm in Ec)
            {
                Get = hmm;
                return hmm;
            }

            if (Get != null)
                return Get;
            else
                return null;
        }

        private static AIHeroClient EDownClient()
        {
            var Edc = GameObjects.Get<AIHeroClient>().Where(i => i.Position.IsValid() && !i.IsAlly && !i.IsDead && (i.HasBuff("zoeesleepcount") || i.HasBuff("zoeesleepcountdown") || i.HasBuff("zoeesleepcountdownslow")));
            AIHeroClient Get = null;
            foreach (var hmm in Edc)
            {
                Get = hmm;
                return hmm;
            }

            if (Get != null)
                return Get;
            else
                return null;
        }
        private static AIHeroClient EStunClient()
        {
            var Edc = GameObjects.Get<AIHeroClient>().Where(i => i.Position.IsValid() && !i.IsAlly && !i.IsDead && i.HasBuff("zoeesleepstun"));
            AIHeroClient Get = null;
            foreach (var hmm in Edc)
            {
                Get = hmm;
                return hmm;
            }

            if (Get != null)
                return Get;
            else
                return null;
        }
        private static float MoveRange = 0;
        private static string QMissile = "ZoeQInterstitial";
        private static int QFlyingTime = 1000;
        private static MenuBool Qcombo = new MenuBool("Q_combo", "Q Combo");
        private static MenuBool Ecombo = new MenuBool("E_combo", "E Combo");
        private static MenuBool Rcombo = new MenuBool("R_combo", "R Combo");
    }
}
