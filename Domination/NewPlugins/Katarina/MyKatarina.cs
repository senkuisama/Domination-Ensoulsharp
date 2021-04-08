using EnsoulSharp.SDK.MenuUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsoulSharp;
using EnsoulSharp.SDK;
using SharpDX;
using Color = System.Drawing.Color;
using Prediction = FSpred.Prediction.Prediction;

namespace DominationAIO.NewPlugins.Katarina
{
    public static class MyKatarina
    {
        private static class KataMenu
        {
            public static MenuList combomode = new MenuList("KataComboMode", "Combo Mode : ", new []{"E first then Q", "Q first then E", "Logic Swap Combo"}, 0);
            public static MenuKeyBind turret = new MenuKeyBind("Turret", "Combo under Turret", Keys.T, KeyBindType.Toggle);
            public static class Qmenu
            {
                public static MenuBool fintbesttarget = new MenuBool("FindBestTarget", "Find Best Target");
                public static MenuBool autoQ = new MenuBool("Auto Q", "Auto Q");
                public static MenuBool useQkillsteal = new MenuBool("useQKS", "Use Q KS");
            }

            public static class Wmenu
            {
                public static MenuSlider Wrange = new MenuSlider("W Range", "W Range", 300, 200, 340);
                public static MenuBool Wgapcloser = new MenuBool("W Gapcloser", "W Gapcloser");
                public static MenuBool WOrb = new MenuBool("WOrb", "Orbwalker to Dagger");
            }

            public static class Emenu
            {
                public static MenuBool useELogic = new MenuBool("Use E logic", "Use E Logic", false);
                public static MenuBool EKS = new MenuBool("EKs", "Use E KS");
                public static MenuKeyBind SaveE = new MenuKeyBind("SaveEIfNoDaggers", "Save E", Keys.H, KeyBindType.Toggle, true);
            }

            public static class Rmenu
            {
                public static MenuKeyBind RCombo = new MenuKeyBind("RCombo", "R Combo toggle Key", Keys.A, KeyBindType.Toggle);
                public static MenuBool UseRifKS = new MenuBool("UseRIfKs", "Accept R combo if target Can kill");
                public static MenuSlider Rcount = new MenuSlider("RCount", "R Target in range", 3, 1, 5);
            }
        }

        private static List<Daggers> _daggersList = new List<Daggers>();
        private static double Rdmg(AIHeroClient target)
        {
            if (target == null)
                return 0;

            if (target.IsValidTarget())
            {
                if (target.DistanceToPlayer() <= 550 - 200)
                {
                    return (double)(new Spell(SpellSlot.R).GetDamage(target));
                }
            }
            
            return (double)(new Spell(SpellSlot.R).GetDamage(target) / 10 * 6);
        }
        private static Menu KMenu = null;
        private static Spell Q = null;
        private static Spell W = null;
        private static Spell E = null;
        private static Spell R = null;
        public static void LoadKata()
        {
            KMenu = new Menu("FunnySlayerKata", "FunnySlayer Katarina", true);
            KMenu.Add(KataMenu.combomode).Permashow();

            var Qstg = new Menu("Qstg", "Q Settings");
            var Wstg = new Menu("Wstg", "W Settings");
            var Estg = new Menu("Estg", "E Settings");
            var Rstg = new Menu("Rstg", "R Settings");
            Qstg.Add(KataMenu.Qmenu.fintbesttarget);
            Qstg.Add(KataMenu.Qmenu.autoQ);
            Qstg.Add(KataMenu.Qmenu.useQkillsteal);

            Wstg.Add(KataMenu.Wmenu.Wrange);
            Wstg.Add(KataMenu.Wmenu.Wgapcloser);
            Wstg.Add(KataMenu.Wmenu.WOrb);

            Estg.Add(KataMenu.Emenu.useELogic);
            Estg.Add(KataMenu.Emenu.EKS);
            Estg.Add(KataMenu.Emenu.SaveE).Permashow();

            Rstg.Add(KataMenu.Rmenu.RCombo).Permashow();
            Rstg.Add(KataMenu.Rmenu.UseRifKS);
            Rstg.Add(KataMenu.Rmenu.Rcount);

            KMenu.Add(KataMenu.turret).Permashow();
            KMenu.Add(Qstg);
            KMenu.Add(Wstg);
            KMenu.Add(Estg);
            KMenu.Add(Rstg);
            KMenu.Attach();

            Q = new Spell(SpellSlot.Q, 625f);
            Q.SetTargetted(0.25f, 2000);

            W = new Spell(SpellSlot.W, 340f);

            E = new Spell(SpellSlot.E, 775f);

            R = new Spell(SpellSlot.R, 550f);
            
            Game.OnUpdate += GameOnOnUpdate; 
            Drawing.OnDraw += DrawingOnOnDraw;
            AIMinionClient.OnCreate += AIMinionClientOnOnCreate;
            AIMinionClient.OnDelete += AIMinionClientOnOnDelete;
            Game.OnUpdate += GameOnOnUpdate1;
            Game.OnUpdate += Game_OnUpdate;
            Game.OnUpdate += Game_OnUpdate1;
        }

        private static void Game_OnUpdate1(EventArgs args)
        {
            var Etarget = E.GetTarget();
            if(Etarget != null)
            {
                if (_daggersList != null && booldagger)
                {
                    foreach (var dagger in _daggersList.Where(i => Variables.GameTimeTickCount - i.CreateTime >= 1000 && i.Position.DistanceToPlayer() <= 775).OrderBy(i => i.CreateTime))
                    {
                        if (dagger == null)
                            return;
                        var targets = TargetSelector.GetTargets(775 + 340);
                        if (targets == null)
                            return;

                        if (targets.Any(i => i.Distance(dagger.Position) <= 390 && i.Health <= PassiveDmg(i)))
                        {
                            var target = targets.Where(i => i.Distance(dagger.Position) <= 390 && i.Health <= PassiveDmg(i))
                                .FirstOrDefault();

                            if (target == null)
                                return;

                            var poscast = dagger.Dagger.Position.Extend(target.Position, 100);
                            if (ObjectManager.Player.Distance(poscast) <= 775)
                            {
                                if (E.Cast(poscast))
                                    return;
                            }
                            else
                            {
                                if (Etarget.Health <= E.GetDamage(Etarget))
                                {
                                    if (E.Cast(Etarget.Position))
                                        return;
                                }
                            }
                        }
                        else
                        {
                            if (Etarget.Health <= E.GetDamage(Etarget))
                            {
                                if (E.Cast(Etarget.Position))
                                    return;
                            }
                        }
                    }
                }
                else
                {
                    if (Etarget.Health <= E.GetDamage(Etarget))
                    {
                        if (E.Cast(Etarget.Position))
                            return;
                    }
                }                
            }
            var Qtarget = Q.GetTarget();
            if(Qtarget != null)
            {
                if(Qtarget.Health <= Q.GetDamage(Qtarget))
                {
                    if (Q.Cast(Qtarget) == CastStates.SuccessfullyCasted)
                        return;
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {           
            if (Orbwalker.ActiveMode >= OrbwalkerMode.LaneClear || !KataMenu.Wmenu.WOrb.Enabled || !booldagger || !_daggersList.Any(i => i.Dagger.Position.DistanceToPlayer() < 300) || _daggersList.Where(i => i.Dagger.Position.DistanceToPlayer() < 100).FirstOrDefault() == null)
            {
                Orbwalker.SetOrbwalkerPosition(Vector3.Zero);
            }
            if (ObjectManager.Player.HasBuff("katarinarsound"))
            {
                Orbwalker.MoveEnabled = false;
                Orbwalker.AttackEnabled = false;
            }
            else
            {
                Orbwalker.MoveEnabled = true;
                if (_daggersList != null && booldagger)
                {
                    if (_daggersList.Where(i => i.Dagger.Position.DistanceToPlayer() < 300 && Variables.GameTimeTickCount - i.CreateTime >= 750).FirstOrDefault() != null)
                    {
                        Orbwalker.AttackEnabled = false;

                        var target = TargetSelector.GetTargets(340);
                        if(target != null)
                        {
                            if (Orbwalker.ActiveMode == OrbwalkerMode.Combo && KataMenu.Wmenu.WOrb.Enabled && _daggersList.Where(i => i.Dagger.Position.DistanceToPlayer() < 100).FirstOrDefault() != null)
                            {
                                Orbwalker.SetOrbwalkerPosition(_daggersList.Where(i => i.Dagger.Position.DistanceToPlayer() < 300).OrderBy(i => i.CreateTime).ThenBy(i => i.Dagger.Position.DistanceToPlayer()).FirstOrDefault().Position.Extend(target.FirstOrDefault().Position, 100));
                            }
                            else
                            {
                                Orbwalker.SetOrbwalkerPosition(Vector3.Zero);
                            }
                        }
                        else
                        {
                            Orbwalker.SetOrbwalkerPosition(Vector3.Zero);
                        }
                    }
                    else
                    {
                        Orbwalker.AttackEnabled = true;
                    }
                }
                else
                {
                    Orbwalker.AttackEnabled = true;
                    Orbwalker.SetOrbwalkerPosition(Vector3.Zero);
                }             
            }            
        }

        private static List<float> BasePassiveDmg = new List<float>()
        {
            0, 68, 72, 77, 82, 89, 96, 103, 112, 121, 131, 142, 154, 166, 180, 194, 208, 225, 240
        };

        private static double PassiveDmg(AIBaseClient target)
        {
            double dmg = 0;
            if (ObjectManager.Player.Level >= 1 && ObjectManager.Player.Level < 6)
            {
                dmg = 0.55;
            }
            if (ObjectManager.Player.Level >= 6 && ObjectManager.Player.Level < 11)
            {
                dmg = 0.7;
            }
            if (ObjectManager.Player.Level >= 11 && ObjectManager.Player.Level < 16)
            {
                dmg = 0.85;
            }
            if (ObjectManager.Player.Level >= 16)
            {
                dmg = 1;
            }

            return ObjectManager.Player.CalculateMagicDamage(target,
                dmg * ObjectManager.Player.TotalMagicalDamage + 0.75 * ObjectManager.Player.GetBonusPhysicalDamage() +
                BasePassiveDmg[ObjectManager.Player.Level]);
        }
        private static void GameOnOnUpdate1(EventArgs args)
        {
            if(ObjectManager.Player.IsDead)
                return;

            if(!E.IsReady())
                return;
           
            if (_daggersList != null && booldagger)
            {
                foreach (var dagger in _daggersList.Where(i => Variables.GameTimeTickCount - i.CreateTime >= 1000 && i.Position.DistanceToPlayer() <= 775).OrderBy(i => i.CreateTime))
                {
                    if(dagger == null)
                        return;
                    var targets = TargetSelector.GetTargets(775 + 340);
                    if(targets == null)
                        return;

                    if (targets.Any(i => i.Distance(dagger.Position) <= 390 && i.Health <= PassiveDmg(i)))
                    {
                        var target = targets.Where(i => i.Distance(dagger.Position) <= 390 && i.Health <= PassiveDmg(i))
                            .FirstOrDefault();
                        
                        if(target == null)
                            return;

                        var poscast = dagger.Dagger.Position.Extend(target.Position, 100);
                        if (ObjectManager.Player.Distance(poscast) <= 775)
                        {
                            if(E.Cast(poscast))
                                return;
                        }
                    }
                }
            }
        }

        private static void AIMinionClientOnOnDelete(GameObject sender, EventArgs args)
        {
            if (sender.Type == GameObjectType.AIMinionClient)
            {
                if (sender.Name == "HiddenMinion")
                {
                    _daggersList.RemoveAll(i => i.Dagger.NetworkId == sender.NetworkId);
                }
            }
        }

        private static void AIMinionClientOnOnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Type == GameObjectType.AIMinionClient)
            {
                if (sender.Name == "HiddenMinion")
                {
                    _daggersList.Add(new Daggers(sender as AIMinionClient, Variables.GameTimeTickCount));
                }
            }
        }

        private static void DrawingOnOnDraw(EventArgs args)
        {
            if (_daggersList != null)
            {
                foreach (var dagger in _daggersList)
                {
                    if(dagger.Dagger.Team == GameObjectTeam.Chaos)
                    {
                        Drawing.DrawCircle(dagger.Dagger.Position, 340, Color.DarkRed);
                        Render.Circle.DrawCircle(dagger.Dagger.Position, 150, Color.DarkRed);
                    }
                    else
                    {
                        Drawing.DrawCircle(dagger.Dagger.Position, 340, Color.Gold);
                        Render.Circle.DrawCircle(dagger.Dagger.Position, 150, Color.Chartreuse);
                    }                   
                    var pos = Drawing.WorldToScreen(dagger.Position);
                    if (Variables.GameTimeTickCount - dagger.CreateTime >= 1000)
                    {
                        Drawing.DrawText(pos.X, pos.Y - 50, Color.Yellow, "Ready");
                    }
                }
            }

            if (Game.Ping >= 100)
            {
                var pos = Drawing.WorldToScreen(ObjectManager.Player.Position);
                Drawing.DrawText(pos.X - 20, pos.Y + 20, Color.Yellow, "High Ping " + Game.Ping.To<string>());
            }
        }
        public static bool booldagger = false;
        private static void GameOnOnUpdate(EventArgs args)
        {

            {
                var target = E.GetTarget();
                if (target != null && E.IsReady() && KataMenu.Emenu.EKS.Enabled)
                {
                    if (target.Health <= E.GetDamage(target) + (!ObjectManager.Player.HasBuff("katarinarsound") ? ObjectManager.Player.GetAutoAttackDamage(target) : 0))
                    {
                        if (E.Cast(target.Position))
                            return;
                    }
                }
            }


            if (_daggersList != null)
            {
                if (_daggersList.FirstOrDefault() == null || !_daggersList.Any(i => i.Dagger.Team == ObjectManager.Player.Team))
                    booldagger = false;
                else
                    booldagger = true;
            }


            if (_daggersList != null)
            {              
                _daggersList.RemoveAll(i => Variables.GameTimeTickCount - i.CreateTime >= 5000 || i.Dagger.IsDead || !i.Dagger.IsValid || i.Dagger.Position == Vector3.Zero);
            }

            /**/

            if (ObjectManager.Player.IsDead) 
                return;

            if (Orbwalker.ActiveMode == OrbwalkerMode.Combo)
            {
                switch (KataMenu.combomode.Index)
                {
                    case 0:
                        EQ();
                        break;
                    case 1:
                        QE();
                        break;
                    case 2:
                        QE();
                        //LogicQWER();
                        EQ();
                        break;
                }
            }

            if(Orbwalker.ActiveMode == OrbwalkerMode.Harass)
            {
                var Qtarget = Q.GetTarget();
                if (Q.IsReady() && Qtarget != null)
                {
                    if (Q.Cast(Qtarget) == CastStates.SuccessfullyCasted)
                        return;
                }
            }
        }

        private static void EQ()
        {        
            var Wtarget = W.GetTarget();
            if(Wtarget != null && W.IsReady())
            {
                if(Wtarget.DistanceToPlayer() <= KataMenu.Wmenu.Wrange.Value)
                {
                    if (W.Cast())
                        return;
                }
            }
           
            if (!KataMenu.Emenu.SaveE.Active || booldagger || (W.IsReady() && W.Level >= 1) || (Q.IsReady() && Q.Level >= 1))
            {
                if (E.IsReady())
                {
                    if (DoComboE())
                        return;
                }
            }

            var Qtarget = Q.GetTarget();
            if (Q.IsReady() && Qtarget != null)
            {
                if (Q.Cast(Qtarget) == CastStates.SuccessfullyCasted)
                    return;
            }

            if (R.IsReady())
            {
                var target = R.GetTarget();
                if (target != null)
                {
                    if(Prediction.PredictUnitPosition(target, 500).DistanceToPlayer() <= R.Range)
                    {
                        if (target.Health <= Rdmg(target))
                        {
                            if (R.Cast())
                                return;
                        }
                    }
                    if (KataMenu.Rmenu.RCombo.Active)
                    {
                        if (R.Cast())
                            return;
                    }

                    var targets = TargetSelector.GetTargets(R.Range);
                    if(targets != null)
                    {
                        if(targets.Count() >= KataMenu.Rmenu.Rcount.Value)
                        {
                            if (R.Cast())
                                return;
                        }
                    }
                }                   
            }
        }
        private static void QE()
        {
            var Wtarget = W.GetTarget();
            if (Wtarget != null && W.IsReady())
            {
                if (Wtarget.DistanceToPlayer() <= KataMenu.Wmenu.Wrange.Value)
                {
                    if (W.Cast())
                        return;
                }
            }

            var Qtarget = Q.GetTarget();

            if (Q.IsReady() && Qtarget != null)
            {
                if (Q.Cast(Qtarget) == CastStates.SuccessfullyCasted)
                    return;
            }

            if (E.IsReady() && !Q.IsReady())
            {
                if (!KataMenu.Emenu.SaveE.Active || booldagger || (W.IsReady() && W.Level >= 1))
                {
                    if (DoComboE())
                        return;
                }                
            }
            
            if (R.IsReady())
            {
                var target = R.GetTarget();
                if (target != null)
                {
                    if (Prediction.PredictUnitPosition(target, 500).DistanceToPlayer() <= R.Range)
                    {
                        if (target.Health <= Rdmg(target))
                        {
                            if (R.Cast())
                                return;
                        }
                    }
                    if (KataMenu.Rmenu.RCombo.Active)
                    {
                        if (R.Cast())
                            return;
                    }

                    var targets = TargetSelector.GetTargets(R.Range);
                    if (targets != null)
                    {
                        if (targets.Count() >= KataMenu.Rmenu.Rcount.Value)
                        {
                            if (R.Cast())
                                return;
                        }
                    }
                }
            }
        }
        private static void LogicQWER()
        {

        }

        public static bool DoComboE()
        {
            var target = E.GetTarget(400);
            if(target != null)
            {
                var posontarget = target.Position;

                if (target.Position.DistanceToPlayer()
                        < FSpred.Prediction.Prediction.PredictUnitPosition(target, 200)
                        .DistanceToPlayer())
                {
                    posontarget = target.Position.Extend(ObjectManager.Player.Position, -50);
                }
                else
                {
                    posontarget = target.Position.Extend(ObjectManager.Player.Position, -50);
                }

                if (_daggersList != null && booldagger)
                {
                    var bestdagger = _daggersList.Where(i => i.Dagger.Position.Distance(target) <= 390).OrderBy(i => i.CreateTime).OrderBy(i => i.Dagger.Position.DistanceToPlayer()).ThenByDescending(i => i.Dagger.Position.Distance(target)).FirstOrDefault();
                    if(bestdagger != null)
                    {
                        goto CastEOnDagger;
                    }
                    
                    if(posontarget != Vector3.Zero)
                    {
                        if (posontarget.DistanceToPlayer() <= 725 && (!Helper.UnderTower(posontarget) || KataMenu.turret.Active))
                            return (E.Cast(posontarget));
                    }
                    

                    CastEOnDagger:
                    if(bestdagger != null)
                    {
                        var castpos = bestdagger.Dagger.Position.Extend(target.Position, 150);
                        if(castpos.DistanceToPlayer() <= E.Range && (!Helper.UnderTower(castpos) || KataMenu.turret.Active))
                        {
                            return (E.Cast(castpos));
                        }
                    }
                }
                else
                {
                    if(posontarget != Vector3.Zero)
                    {
                        if (posontarget.DistanceToPlayer() <= 725 && (!Helper.UnderTower(posontarget) || KataMenu.turret.Active))
                            return E.Cast(posontarget);
                    }                    
                }
            }

            return false;
        }
    }

    public class Daggers
    {
        public Vector3 Position { get; set; }
        public int CreateTime { get; set; }
        public int uid { get; set; }
        public AIMinionClient Dagger;

        public Daggers(AIMinionClient dagger, int timecreate)
        {
            Position = dagger.Position;
            CreateTime = timecreate;
            Dagger = dagger;
            uid = dagger.NetworkId;
        }
    }
}
