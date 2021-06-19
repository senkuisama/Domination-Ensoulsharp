using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using SharpDX;
using System.Collections.Generic;
using System.Linq;

namespace DominationAIO.NewPlugins
{
    public static class MyYone
    {
        private static Spell Q1, Q3, W, E, R;
        private static Menu YMenu = null;
        private static AIHeroClient objPlayer = null;
        public static void YoneLoad()
        {
            objPlayer = ObjectManager.Player;
            LoadMenu();

            Q1 = new Spell(SpellSlot.Q, 475);
            Q3 = new Spell(SpellSlot.Q, 900);
            W = new Spell(SpellSlot.W, 600);
            E = new Spell(SpellSlot.E, 300);
            R = new Spell(SpellSlot.R, 1000);

            Q1.SetSkillshot(0.2f, 20, float.MaxValue, false, SpellType.Line);
            Q3.SetSkillshot(0.25f, 50, 1500, false, SpellType.Line);
            W.SetSkillshot(0.25f, 100, float.MaxValue, false, SpellType.Line);
            R.SetSkillshot(1f, 100, float.MaxValue, false, SpellType.Line);

            Game.OnUpdate += Game_OnUpdate;
            Game.OnUpdate += Game_OnUpdate1;
            AIBaseClient.OnBuffAdd += AIBaseClient_OnBuffAdd;
            AIBaseClient.OnBuffRemove += AIBaseClient_OnBuffRemove;

            foreach (var item in GameObjects.EnemyHeroes)
            {
                var target = item;
                ListDmg.Add(new DmgOnTarget(target.NetworkId, 0));
            }
            AIBaseClient.OnDoCast += AIBaseClient_OnDoCast;
            Drawing.OnDraw += Drawing_OnDraw;
        }
      
        private static void Drawing_OnDraw(System.EventArgs args)
        {
            if(pos != Vector3.Zero)
                Render.Circle.DrawCircle(pos, 50, System.Drawing.Color.Red);
        }
        private static Vector3 pos = Vector3.Zero;

        private static void AIBaseClient_OnDoCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && isE2())
            {
                var target = args.Target;
                if(target != null)
                {
                    var FindinList = ListDmg.Where(i => i.UID == target.NetworkId);
                    if (FindinList.Count() >= 1)
                    {
                        var dmgtarget = FindinList.FirstOrDefault();
                        dmgtarget.dmg += ObjectManager.Player.GetAutoAttackDamage(target as AIHeroClient);
                    }
                }
                else
                {
                    pos = args.To;
                    List<AIHeroClient> gettarget = new List<AIHeroClient>();
                    if (args.Slot == SpellSlot.Q)
                    {
                        var poly = isQ3() ? new Geometry.Rectangle(ObjectManager.Player.Position, ObjectManager.Player.Position.Extend(pos, Q3.Range), 80) : new Geometry.Rectangle(ObjectManager.Player.Position, ObjectManager.Player.Position.Extend(pos, Q1.Range), 50);
                        gettarget.AddRange(GameObjects.EnemyHeroes.Where(i => !i.IsDead && poly.IsInside(i.Position.ToVector2())));

                        foreach (var item in gettarget)
                        {
                            var Qtarget = item;

                            var FindinList = ListDmg.Where(i => i.UID == Qtarget.NetworkId);
                            if (FindinList.Count() >= 1)
                            {
                                var dmgtarget = FindinList.FirstOrDefault();
                                dmgtarget.dmg += Q1.GetDamage(Qtarget);
                            }
                        }
                    }

                    if (args.Slot == SpellSlot.W)
                    {
                        var poly = new Geometry.Line(ObjectManager.Player.Position, ObjectManager.Player.Position.Extend(pos, W.Range), 200);
                        gettarget.AddRange(GameObjects.EnemyHeroes.Where(i => !i.IsDead && poly.IsInside(i.Position.ToVector2())));

                        foreach (var item in gettarget)
                        {
                            var Qtarget = item;

                            var FindinList = ListDmg.Where(i => i.UID == Qtarget.NetworkId);
                            if (FindinList.Count() >= 1)
                            {
                                var dmgtarget = FindinList.FirstOrDefault();
                                dmgtarget.dmg += W.GetDamage(Qtarget);
                            }
                        }
                    }

                    if (args.Slot == SpellSlot.R)
                    {
                        var poly = new Geometry.Line(ObjectManager.Player.Position, ObjectManager.Player.Position.Extend(pos, R.Range), 150);
                        gettarget.AddRange(GameObjects.EnemyHeroes.Where(i => !i.IsDead && poly.IsInside(i.Position.ToVector2())));

                        foreach (var item in gettarget)
                        {
                            var Qtarget = item;

                            var FindinList = ListDmg.Where(i => i.UID == Qtarget.NetworkId);
                            if (FindinList.Count() >= 1)
                            {
                                var dmgtarget = FindinList.FirstOrDefault();
                                dmgtarget.dmg += R.GetDamage(Qtarget);
                            }
                        }
                    }
                }                
            }
        }

        private static void PrintLine(object a)
        {
            Game.Print(a.ToString());
        }
        private static List<DmgOnTarget> ListDmg = new List<DmgOnTarget>();
        class DmgOnTarget

        {
            public int UID;
            public double dmg;
            public double LastHeath;
            public DmgOnTarget(int ID, double Dmg)
            {
                UID = ID;
                dmg = Dmg;
                LastHeath = ObjectManager.GetUnitByNetworkId<AIHeroClient>(ID).Health;
            }
        }

        private static string YoneEDeath = "yoneedeathmark";    
        private static void AIBaseClient_OnBuffAdd(AIBaseClient sender, AIBaseClientBuffAddEventArgs args)
        {
            if (args.Buff.Name != "YoneE")
                return;

            foreach (var item in ListDmg)
            {
                var target = item;

                //Start dmg
                target.dmg = 0;
                target.LastHeath = sender.Health;
            }
        }

        private static void AIBaseClient_OnBuffRemove(AIBaseClient sender, AIBaseClientBuffRemoveEventArgs args)
        {
            if (args.Buff.Name != "YoneE")
                return;

            foreach (var item in ListDmg)
            {
                var target = item;

                //End dmg
                target.dmg = 0;
                target.LastHeath = sender.Health;
            }
        }

        private static double GetEDmg(AIBaseClient target)
        {
            if(!isE2() || !target.HasBuff(YoneEDeath))
                return 0;

            var findinlist = ListDmg.Where(i => i.UID == target.NetworkId);
            if (findinlist.Count() < 1)
                return 0;

            double dmg = 0;
            var list = new double[]
            {
                0.25, 0.275, 0.3, 0.325, 0.35
            };
            var xtarget = findinlist.FirstOrDefault();
            dmg += list[E.Level - 1] * xtarget.dmg;

            return dmg;
        }
        private static void Game_OnUpdate1(System.EventArgs args)
        {
            if (YoneMenu.Keys.SemiR.Active && R.IsReady())
            {
                var hit = 0;
                var target = SelectedRTarget(GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(R.Range) && !i.IsDead).ToList(), out hit);
                if (target != null && hit >= 1)
                {
                    var pred = FSpred.Prediction.Prediction.GetPrediction(R, target, true);

                    if(pred != null)
                        R.Cast(pred.CastPosition);
                    return;
                }
            }
        }

        private static AIHeroClient SelectedRTarget(List<AIBaseClient> Targets)
        {
            var returnTarget = Targets.Where(i => CheckRTarget(i as AIHeroClient)).OrderByDescending(i => TargetRPredHit(i as AIHeroClient));
            return returnTarget.FirstOrDefault() as AIHeroClient;
        }
        private static AIHeroClient SelectedRTarget(List<AIHeroClient> Targets, out int hitcount)
        {
            var returnTarget = Targets.Where(i => CheckRTarget(i)).OrderByDescending(i => TargetRPredHit(i));
            var target = returnTarget.FirstOrDefault();
            hitcount = TargetRPredHit(target);
            return target;
        }



        private static bool CheckRTarget(AIHeroClient target)
        {
            var pred = FSpred.Prediction.Prediction.GetPrediction(R, target, true);
            return target.IsValidTarget(R.Range) && (pred.Hitchance >= FSpred.Prediction.HitChance.High || pred.Hitchance >= FSpred.Prediction.HitChance.Medium && pred.AoeTargetsHitCount > 1);
        }

        private static int TargetRPredHit(AIHeroClient target)
        {
            var pred = FSpred.Prediction.Prediction.GetPrediction(R, target, true);
            return pred.AoeTargetsHitCount;
        }

        private static void Game_OnUpdate(System.EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
                return;


            if(Orbwalker.ActiveMode == OrbwalkerMode.Combo)
            {
                //Game.Print("In Combo");
                YoneCombo();
            }

            if(Orbwalker.ActiveMode == OrbwalkerMode.LaneClear && !FunnySlayerCommon.OnAction.OnAA)
            {
                var Qminions = ObjectManager.Get<AIBaseClient>().Where(i => !i.IsDead && !i.IsAlly && i.IsValidTarget(isQ3() ? 900 : 475) && !i.Position.IsBuilding()).OrderByDescending(i => i.Health);
                if (Qminions != null && Q1.IsReady())
                {
                    foreach (var min in Qminions)
                    {
                        if (!min.IsMinion())
                        {
                            if (isQ3())
                            {
                                var qFarm = Q3.GetPrediction(min);

                                if (qFarm.Hitchance >= HitChance.High && qFarm.CastPosition.DistanceToPlayer() <= 900 && !Yasuo.YasuoHelper.UnderTower(objPlayer.Position.Extend(qFarm.CastPosition, 500)))
                                    Q3.Cast(qFarm.CastPosition);
                            }
                            else
                            {
                                var qFarm = Q1.GetPrediction(min);

                                if (qFarm.Hitchance >= HitChance.High && qFarm.CastPosition.DistanceToPlayer() <= 475)
                                    Q1.Cast(qFarm.CastPosition);
                            }
                        }
                        else
                        {
                            if (isQ3())
                            {
                                if (Orbwalker.ActiveMode == OrbwalkerMode.Harass)
                                {
                                    return;
                                }
                                else
                                {
                                    var qFarm = Q3.GetLineFarmLocation(Qminions.ToList());

                                    if (qFarm.MinionsHit >= 1 && !Yasuo.YasuoHelper.UnderTower(objPlayer.Position.Extend(qFarm.Position, 500)))
                                    {
                                        Q3.Cast(qFarm.Position);
                                    }
                                }
                            }
                            else
                            {
                                if (Orbwalker.ActiveMode == OrbwalkerMode.Harass)
                                {
                                    if (min.Health < objPlayer.GetSpellDamage(min, SpellSlot.Q) && min.IsValidTarget(Q1.Range))
                                    {
                                        Q1.Cast(min.Position);
                                    }
                                }
                                else
                                {
                                    var qFarm = Q1.GetLineFarmLocation(Qminions.ToList());

                                    if (qFarm.MinionsHit >= 1)
                                    {
                                        Q1.Cast(qFarm.Position);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        private static bool isE2()
        {
            if (ObjectManager.Player.Mana > 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private static bool isQ3()
        {
            if (Q1.Name == "YoneQ3")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static void YoneCombo()
        {
            if (FunnySlayerCommon.OnAction.OnAA)
                return;
            {
                var target = FunnySlayerCommon.FSTargetSelector.GetFSTarget(isQ3() ? 910 : 700);
                if (target != null && E.IsReady() && !isE2() && YoneMenu.Ecombo.Combo_Ecombo.Enabled)
                {
                    if (YoneMenu.Ecombo.Combo_Edashturret.Enabled && YoneMenu.Keys.TurretKey.Active)
                    {
                        E.Cast(target.Position);
                        return;
                    }

                    if (!Yasuo.YasuoHelper.UnderTower(ObjectManager.Player.Position.Extend(target.Position, 300)))
                    {
                        E.Cast(target.Position);
                        return;
                    }
                }
            }

            {
                if (E.IsReady() && isE2() && ObjectManager.Player.CountEnemyHeroesInRange(R.Range) <= 1)
                {
                    if(GameObjects.EnemyHeroes.Any(
                        i => !i.IsDead && (
                        i.Health - GetEDmg(i) <= 0 
                        || (ObjectManager.Player.HasItem(ItemId.The_Collector) && 100 * (i.Health - GetEDmg(i)) <= 5))
                        )
                        )
                    {
                        E.Cast(ObjectManager.Player.Position);
                        return;
                    }
                }
            }

            {
                var target = FunnySlayerCommon.FSTargetSelector.GetFSTarget(R.Range);
                if(target != null && R.IsReady() && YoneMenu.Rcombo.Combo_Rcombo.Enabled)
                {
                    var pred = FSpred.Prediction.Prediction.GetPrediction(R, target, true);

                    if(pred.Hitchance >= FSpred.Prediction.HitChance.Medium && pred.AoeTargetsHitCount >= YoneMenu.Rcombo.Combo_Rhitcount.Value)
                    {
                        R.Cast(pred.CastPosition);
                        return;
                    }
                }
            }

            {
                var target = FunnySlayerCommon.FSTargetSelector.GetFSTarget(isQ3() ? Q3.Range : Q1.Range);
                if (target != null && Q3.IsReady())
                {
                    if (isQ3())
                    {
                        var pred = FSpred.Prediction.Prediction.GetPrediction(Q3, target);

                        if(pred.Hitchance >= FSpred.Prediction.HitChance.High)
                        {
                            if (!Yasuo.YasuoHelper.UnderTower(objPlayer.Position.Extend(pred.CastPosition, 500)) || YoneMenu.Keys.TurretKey.Active)
                            {
                                Q3.Cast(pred.CastPosition);
                                return;
                            }
                        }
                    }
                    else
                    {
                        var pred = FSpred.Prediction.Prediction.GetPrediction(Q1, target);

                        if (pred.Hitchance >= FSpred.Prediction.HitChance.High)
                        {
                            Q1.Cast(pred.CastPosition);
                            return;
                        }
                    }
                }
            }

            {
                var target = FunnySlayerCommon.FSTargetSelector.GetFSTarget(W.Range);
                if(target != null && W.IsReady() && YoneMenu.Wcombo.Combo_Wcombo.Enabled)
                {
                    var pred = FSpred.Prediction.Prediction.GetPrediction(W, target);

                    if (pred.Hitchance >= FSpred.Prediction.HitChance.High)
                    {
                        W.Cast(pred.CastPosition);
                        return;
                    }
                }                
            }
        }

        private static class YoneMenu
        {
            public class Cancelaa
            {
                public static MenuBool Q_cancel = new MenuBool("Q", "Accept Q Cancel AA", false);
                public static MenuBool W_cancel = new MenuBool("W", "Accept W Cancel AA", false);
                public static MenuBool E_cancel = new MenuBool("E", "Accept E Cancel AA", false);
                public static MenuBool R_cancel = new MenuBool("R", "Accept R Cancel AA", false);
            }
            public class Qcombo
            {
                public static MenuBool Combo_Qcombo = new MenuBool("Qcombo", "Q in combo");
                public static MenuBool Combo_Qwindcombo = new MenuBool("Qwindcombo", "_Wind in combo");
                public static MenuBool Combo_Qbeforeaa = new MenuBool("Qbeforeaa", "_ Only use Q [Before aa]", false);
                public static MenuBool Combo_Qafteraa = new MenuBool("Qafteraa", "_ Only use Q [After aa]", false);

                public static MenuBool Combo_Qauto = new MenuBool("Comb_Qauto", "Auto use Q", false);
                public static MenuBool AcceptQ3 = new MenuBool("AcceptQ3", "---> Wind", false);
            }
            public class Wcombo
            {
                public static MenuBool Combo_Wcombo = new MenuBool("Wcombo", "W in combo");
                public static MenuBool Combo_Wafteraa = new MenuBool("Wafteraa", "_ Only use W [After aa]", true);
                public static MenuBool Combo_Woutaarange = new MenuBool("Woutaarange", "_ Only use W [Out aa range]", true);
                public static MenuBool Combo_Wifhavewind = new MenuBool("Wifhavewind", "_ Only use W [Player have Wind]", false);
                public static MenuSliderButton Combo_Whit = new MenuSliderButton("Whit", ": : Use W if hit x target", 1, 0, 5);
            }
            public class Ecombo
            {
                public static MenuBool Combo_Ecombo = new MenuBool("Ecombo", "E in combo");
                public static MenuBool Combo_Edashturret = new MenuBool("Edashturret", "_ E dash turret");
                public static MenuSliderButton Combo_Etargetheath = new MenuSliderButton("Etargetheath", ": : Use E if target heath <=", 100);
                public static MenuSliderButton Combo_Eplayerheath = new MenuSliderButton("Eplayerheath", ": : Use E if player heath <=", 80);
                public static MenuBool Combo_Ereturn = new MenuBool("Ereturn", "_ E return [gap closer]");
                public static MenuBool Combo_Eoutaarange = new MenuBool("Eoutaarange", "_ only use E [Out aa range]");
                public static MenuBool Combo_Eifhavewind = new MenuBool("Eifhavewind", "_ Only use E [Player have Wind]", false);
                public static MenuBool Combo_Etargetcount = new MenuBool("Etargetcount", ": : Use E if target count > 1");
            }
            public class Rcombo
            {
                public static MenuBool Combo_Rcombo = new MenuBool("Rcombo", "R in combo");
                public static MenuSliderButton Combo_Rtargetheath = new MenuSliderButton("Rtargetheath", ": : Use R if target heath <= ", 40);
                public static MenuSliderButton Combo_Rhitcount = new MenuSliderButton("Rhitcount", "Use R if can hit >= ", 3, 1, 5);

            }

            public class EQwind
            {
                public static MenuBool Combo_EQWind = new MenuBool("Combo_EQWind", "EWind logic");
            }
            public class Keys
            {
                public static MenuKeyBind TurretKey = new MenuKeyBind("TurretKey", "Allow combo in Turret", EnsoulSharp.SDK.MenuUI.Keys.A, KeyBindType.Toggle);
                public static MenuKeyBind SemiR = new MenuKeyBind("SemiR", "R Using Key", EnsoulSharp.SDK.MenuUI.Keys.T, KeyBindType.Press);
                public static MenuKeyBind SemiE = new MenuKeyBind("SemiE", "E Using Key", EnsoulSharp.SDK.MenuUI.Keys.A, KeyBindType.Toggle);
            }
        }

        private static void LoadMenu()
        {
            YMenu = new Menu("YoneTheMenu", "FunnySlayer Yone", true);
            var combomenu = new MenuSeparator("combomenu", "Combo Settings");

            Menu Qcb = new Menu("Qcombo", "Q Settings") { YoneMenu.Qcombo.Combo_Qcombo,
                YoneMenu.Qcombo.Combo_Qwindcombo,
                YoneMenu.Qcombo.Combo_Qbeforeaa,
                YoneMenu.Qcombo.Combo_Qafteraa,
                YoneMenu.Qcombo.Combo_Qauto,
                YoneMenu.Qcombo.AcceptQ3,};
            Menu Wcb = new Menu("Wcombo", "W Settings") { YoneMenu.Wcombo.Combo_Wcombo,
                YoneMenu.Wcombo.Combo_Wafteraa,
                YoneMenu.Wcombo.Combo_Woutaarange,
                YoneMenu.Wcombo.Combo_Wifhavewind,
                YoneMenu.Wcombo.Combo_Whit,};
            Menu Ecb = new Menu("Ecombo", "E Settings") { YoneMenu.Ecombo.Combo_Ecombo,
                YoneMenu.Ecombo.Combo_Edashturret,
                YoneMenu.Ecombo.Combo_Etargetheath,
                YoneMenu.Ecombo.Combo_Eplayerheath,
                YoneMenu.Ecombo.Combo_Ereturn,
                YoneMenu.Ecombo.Combo_Eoutaarange,
                YoneMenu.Ecombo.Combo_Eifhavewind,
                YoneMenu.Ecombo.Combo_Etargetcount,};
            Menu Rcb = new Menu("Rcombo", "R Settings") { YoneMenu.Rcombo.Combo_Rcombo,
                YoneMenu.Rcombo.Combo_Rhitcount,
                YoneMenu.Rcombo.Combo_Rtargetheath,};

            Menu AA = new Menu("AA", "AA Settings") { YoneMenu.Cancelaa.Q_cancel,
                YoneMenu.Cancelaa.W_cancel,
                YoneMenu.Cancelaa.E_cancel,
                YoneMenu.Cancelaa.R_cancel,};

            Menu Keys = new Menu("Keys", "Keys Settings");
            Keys.Add(YoneMenu.Keys.TurretKey).Permashow();
            Keys.Add(YoneMenu.Keys.SemiE).Permashow();
            Keys.Add(YoneMenu.Keys.SemiR).Permashow();

            //FunnySlayerCommon.MenuClass.AddTargetSelectorMenu(YMenu);

            YMenu.Add(combomenu);
            YMenu.Add(Qcb);
            YMenu.Add(Wcb);
            YMenu.Add(Ecb);
            YMenu.Add(Rcb);
            YMenu.Add(AA);
            YMenu.Add(Keys);
            YMenu.Attach();
        }
    }
}
