using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.Utility;
using SharpDX;
using SPredictionMash;
using System;
using System.Linq;

namespace DominationAIO.NewPlugins
{
    public static class MyXerath
    {
        private  static class XerathMenu
        {
            public static class QMenu
            {
                public static MenuBool UseQCombo = new MenuBool("UseQCombo", "Use Q in Combo");
                public static MenuBool UseQHarass = new MenuBool("UseQHarass", "Use Q in Harass");
                public static MenuBool UseQClear = new MenuBool("UseQClear", "Use Q in Clear");
                public static MenuSlider QClearHit = new MenuSlider("QClearHit", "---> When hit >= x", 3, 1, 6);
                public static MenuSlider QManaClear = new MenuSlider("QManaClear", "---> Clear mana >= x %", 50);
            }
            public static class WMenu
            {
                public static MenuBool UseWCombo = new MenuBool("UseWCombo", "Use W in Combo");
                public static MenuBool UseWHarass = new MenuBool("UseWHarass", "Use W in Harass");
                public static MenuBool UseWClear = new MenuBool("UseWClear", "Use W in Clear");
                public static MenuSlider WClearHit = new MenuSlider("WClearHit", "---> When hit >= x", 3, 1, 6);
                public static MenuSlider WManaClear = new MenuSlider("WManaClear", "---> Clear mana >= x %", 50);
            }
            public static class EMenu
            {
                public static MenuBool UseECombo = new MenuBool("UseECombo", "Use E in Combo");
                public static MenuBool UseEHarass = new MenuBool("UseEHarass", "Use E in Harass");
                public static MenuBool AutoE = new MenuBool("AutoE", "Auto E on best target");
            }
            public static class RMenu
            {
                public static MenuKeyBind RKey = new MenuKeyBind("RKey", "R Key", Keys.T, KeyBindType.Press);
                public static MenuSlider CursorRange = new MenuSlider("Cursor Range", "Cursor Range", 400, 0, 2000);

                public static MenuList Select = new MenuList("Select", "Select target in Cursor Range",
                    new[] {"Low Hp", "AOE prediction", "LOGIC"}, 0);
                
            }
            
            public static class PredictionMenu
            {
                public static class Q
                {
                    public static MenuList QPred = new MenuList("QPred", "Using Prediction ",
                        new[] {"SDK Prediction", "FunnySlayer Prediction", "Exory Prediction"}, 0);

                    public static MenuList QHitchance = new MenuList("QHitchance", "Min Hitchance ",
                        new[] {"Medium", "High", "Very High"}, 1);

                    /*public static MenuSeparator QRange = new MenuSeparator("QRange", "Q Range: " + MyXerath.Q.Range);
                    public static MenuSeparator QDelay = new MenuSeparator("QDelay", "Q Delay: " + MyXerath.Q.Delay);
                    public static MenuSeparator QWidth = new MenuSeparator("QWidth", "Q Width: " + MyXerath.Q.Width);
                    public static MenuSeparator QSpeed = new MenuSeparator("QSpeed", "Q Speed: " + MyXerath.Q.Speed);*/
                }
                public static class W
                {
                    public static MenuList WPred = new MenuList("WPred", "Using Prediction ",
                        new[] {"SDK Prediction", "FunnySlayer Prediction", "Exory Prediction"}, 0);
                    
                    public static MenuList WHitchance = new MenuList("WHitchance", "Min Hitchance ",
                        new[] {"Medium", "High", "Very High"}, 1);
                    
                    /*public static MenuSeparator WRange = new MenuSeparator("WRange", "W Range: " + MyXerath.W.Range);
                    public static MenuSeparator WDelay = new MenuSeparator("WDelay", "W Delay: " + MyXerath.W.Delay);
                    public static MenuSeparator WWidth = new MenuSeparator("WWidth", "W Width: " + MyXerath.W.Width);
                    public static MenuSeparator WSpeed = new MenuSeparator("WSpeed", "W Speed: " + MyXerath.W.Speed);*/
                }
                public static class E
                {
                    public static MenuList EPred = new MenuList("EPred", "Using Prediction ",
                        new[] {"SDK Prediction", "FunnySlayer Prediction", "Exory Prediction"}, 0);
                    
                    public static MenuList EHitchance = new MenuList("EHitchance", "Min Hitchance ",
                        new[] {"Medium", "High", "Very High"}, 1);
                    
                    /*public static MenuSeparator ERange = new MenuSeparator("ERange", "E Range: " + MyXerath.E.Range);
                    public static MenuSeparator EDelay = new MenuSeparator("EDelay", "E Delay: " + MyXerath.E.Delay);
                    public static MenuSeparator EWidth = new MenuSeparator("EWidth", "E Width: " + MyXerath.E.Width);
                    public static MenuSeparator ESpeed = new MenuSeparator("ESpeed", "E Speed: " + MyXerath.E.Speed);*/
                }
                public static class R
                {
                    public static MenuList RPred = new MenuList("RPred", "Using Prediction ",
                        new[] {"SDK Prediction", "FunnySlayer Prediction", "Exory Prediction"}, 0);
                    
                    public static MenuList RHitchance = new MenuList("RHitchance", "Min Hitchance ",
                        new[] {"Medium", "High", "Very High"}, 1);
                }
            }
        }
        private static Spell Q, W, E, R;
        private static Menu XMenu = null;
        public static void XerathLoad()
        {
            Q = new Spell(SpellSlot.Q, 750f);
            W = new Spell(SpellSlot.W, 950f);
            E = new Spell(SpellSlot.E, 1050f);
            R = new Spell(SpellSlot.R, 5000f);
            
            W.SetSkillshot(0.75f, 125f, float.MaxValue, false, SpellType.Circle);
            E.SetSkillshot(0.25f, 60f, 1400f, true, SpellType.Line);
            R.SetSkillshot(0.627f, 68f, float.MaxValue, false, SpellType.Circle);
            Q.SetSkillshot(0.6f, 65f, 20000, false, SpellType.Line);
            Q.SetCharged("XerathArcanopulseChargeUp", "XerathArcanopulseChargeUp", 750, 1550, 1.5f);

            XMenu = new Menu("FunnySlayerXerath", "FunnySlayer Xerath", true);
            Menu Qmenu = new Menu("Qmenu", "Q Settings");
            Menu Wmenu = new Menu("Wmenu", "W Settings");
            Menu Emenu = new Menu("Emenu", "E Settings");
            Menu Rmenu = new Menu("Rmenu", "R Settings");

            Qmenu.Add(XerathMenu.QMenu.UseQCombo);
            Qmenu.Add(XerathMenu.QMenu.UseQHarass);
            Qmenu.Add(XerathMenu.QMenu.UseQClear);
            Qmenu.Add(XerathMenu.QMenu.QClearHit);
            Qmenu.Add(XerathMenu.QMenu.QManaClear);
            
            Wmenu.Add(XerathMenu.WMenu.UseWCombo);
            Wmenu.Add(XerathMenu.WMenu.UseWHarass);
            Wmenu.Add(XerathMenu.WMenu.UseWClear);
            Wmenu.Add(XerathMenu.WMenu.WClearHit);
            Wmenu.Add(XerathMenu.WMenu.WManaClear);

            Emenu.Add(XerathMenu.EMenu.UseECombo);
            Emenu.Add(XerathMenu.EMenu.UseEHarass);
            Emenu.Add(XerathMenu.EMenu.AutoE);
            AntiGapcloser.Attach(Emenu);

            Rmenu.Add(XerathMenu.RMenu.RKey).Permashow();
            Rmenu.Add(XerathMenu.RMenu.CursorRange);
            Rmenu.Add(XerathMenu.RMenu.Select).Permashow();

            var SpellInfor = new Menu("SpellInfor", "Spell");
            SpellInfor.Add(
                new Menu("Qinfor", "Q")
                {
                    XerathMenu.PredictionMenu.Q.QPred,
                    XerathMenu.PredictionMenu.Q.QHitchance,
                    /*XerathMenu.PredictionMenu.Q.QRange,
                    XerathMenu.PredictionMenu.Q.QDelay,
                    XerathMenu.PredictionMenu.Q.QWidth,
                    XerathMenu.PredictionMenu.Q.QSpeed,*/
                }
            );
            
            SpellInfor.Add(
                new Menu("Winfor", "W")
                {
                    XerathMenu.PredictionMenu.W.WPred,
                    XerathMenu.PredictionMenu.W.WHitchance,
                    /*XerathMenu.PredictionMenu.W.WRange,
                    XerathMenu.PredictionMenu.W.WDelay,
                    XerathMenu.PredictionMenu.W.WWidth,
                    XerathMenu.PredictionMenu.W.WSpeed,*/
                }
            );
            
            SpellInfor.Add(
                new Menu("Einfor", "E")
                {
                    XerathMenu.PredictionMenu.E.EPred,
                    XerathMenu.PredictionMenu.E.EHitchance,
                    /*XerathMenu.PredictionMenu.E.ERange,
                    XerathMenu.PredictionMenu.E.EDelay,
                    XerathMenu.PredictionMenu.E.EWidth,
                    XerathMenu.PredictionMenu.E.ESpeed,*/
                }
            );
            
            SpellInfor.Add(
                new Menu("Rinfor", "R")
                {
                    XerathMenu.PredictionMenu.R.RPred,
                    XerathMenu.PredictionMenu.R.RHitchance,
                }
            );

            XMenu.Add(Qmenu);
            XMenu.Add(Wmenu);
            XMenu.Add(Emenu);
            XMenu.Add(Rmenu);
            XMenu.Add(SpellInfor);

            XMenu.Attach();
            
            Game.OnUpdate += GameOnOnUpdate;
            AntiGapcloser.OnAllGapcloser += AntiGapcloserOnOnAllGapcloser;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            Render.Circle.DrawCircle(Game.CursorPos, 200f, CheckTarget ? System.Drawing.Color.Red : System.Drawing.Color.Green);
        }

        private static bool CheckTarget = false;
        private static void AntiGapcloserOnOnAllGapcloser(AIHeroClient sender, AntiGapcloser.GapcloserArgs args)
        {

        }

        private static void GameOnOnUpdate(EventArgs args)
        {
            if(ObjectManager.Player.IsDead)
                return;
            
            if (ObjectManager.Player.HasBuff("XerathLocusOfPower2"))
            {
                Orbwalker.AttackEnabled = false;
                Orbwalker.MoveEnabled = false;

                if (XerathMenu.RMenu.RKey.Active)
                {
                    var targets = GameObjects.EnemyHeroes.Where(i => i.Distance(Game.CursorPos) <= XerathMenu.RMenu.CursorRange.Value && !i.IsDead).OrderBy(i => i.Health);

                    if (targets != null)
                    {
                        CheckTarget = false;

                        if(targets != null)
                        {
                            var target = targets.Find(i => i.DistanceToCursor() <= XerathMenu.RMenu.CursorRange.Value);
                            if (target != null)
                            {
                                if (target.RPredictionCasted())
                                    return;
                            }
                            else
                                return;
                        }
                    }
                    else
                    {
                        CheckTarget = true;
                    }
                }
            }
            else
            {
                CheckTarget = false;

                if (Orbwalker.ActiveMode > OrbwalkerMode.Harass)
                    return;

                if (Q.IsCharging)
                    Orbwalker.AttackEnabled = false;
                else
                    Orbwalker.AttackEnabled = true;
                
                Orbwalker.MoveEnabled = true;
                
                var targets = GameObjects.EnemyHeroes.Where(i => !i.IsDead && i.IsValidTarget(Q.Range));
                if (targets != null)
                {
                    var target = TargetSelector.GetTarget(targets, DamageType.Magical);

                    if (target != null)
                    {
                        if(target.EPredictionCasted())
                            return;
                        else
                        {
                            if(target.WPredictionCasted())
                                return;
                            else
                            {
                                if (Q.IsReady())
                                {
                                    if (Q.IsCharging)
                                    {
                                        if (target.QPredictionCasted())
                                        {
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        var Qtemp = new Spell(SpellSlot.Unknown, 1400f);
                                        Qtemp.SetSkillshot(0.5f, 100f, float.MaxValue, false, SpellType.Line);
                                        var pred = Qtemp.GetPrediction(target);

                                        if (pred.Hitchance >= HitChance.High)
                                        {
                                            if(Q.StartCharging())
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


        private static bool QPredictionCasted(this AIBaseClient target)
        {
            if (target == null || !Q.IsReady())
                return false;

            if (Orbwalker.ActiveMode == OrbwalkerMode.Combo && !XerathMenu.QMenu.UseQCombo.Enabled)
                return false;
            if (Orbwalker.ActiveMode == OrbwalkerMode.Harass && !XerathMenu.QMenu.UseQHarass.Enabled)
                return false;

            int hitchange = 3;
            switch (XerathMenu.PredictionMenu.Q.QHitchance.Index)
            {
                case 0:
                    hitchange = 2;
                    break;
                case 1:
                    hitchange = 3;
                    break;
                case 2:
                    hitchange = 4;
                    break;
            }
            
            switch (XerathMenu.PredictionMenu.Q.QPred.Index)
            {
                case 0:
                    var Prediction0 = Q.GetPrediction(target);
                    if ((int) Prediction0.Hitchance >= hitchange)
                    {
                        return Q.ShootChargedSpell(Prediction0.CastPosition);
                    }
                    break;
                case 1:
                    var Prediction1 = FSpred.Prediction.Prediction.GetPrediction(Q, target);
                    if ((int) Prediction1.Hitchance >= hitchange)
                    {
                        return Q.ShootChargedSpell(Prediction1.CastPosition);
                    }
                    break;
                case 2:
                    var Prediction2 = SebbyLibPorted.Prediction.Prediction.GetPrediction(Q, target);
                    if ((int) Prediction2.Hitchance >= hitchange)
                    {
                        return Q.ShootChargedSpell(Prediction2.CastPosition);
                    }
                    break;
            }
            return false;
        }
        
        private static bool WPredictionCasted(this AIBaseClient target)
        {
            if (target == null || !W.IsReady())
                return false;

            if (Orbwalker.ActiveMode == OrbwalkerMode.Combo && !XerathMenu.WMenu.UseWCombo.Enabled)
                return false;
            if (Orbwalker.ActiveMode == OrbwalkerMode.Harass && !XerathMenu.WMenu.UseWHarass.Enabled)
                return false;

            int hitchange = 3;
            switch (XerathMenu.PredictionMenu.W.WHitchance.Index)
            {
                case 0:
                    hitchange = 2;
                    break;
                case 1:
                    hitchange = 3;
                    break;
                case 2:
                    hitchange = 4;
                    break;
            }
            
            switch (XerathMenu.PredictionMenu.W.WPred.Index)
            {
                case 0:
                    var Prediction0 = W.GetPrediction(target, true);
                    if ((int) Prediction0.Hitchance >= hitchange)
                    {
                        return W.Cast(Prediction0.CastPosition);
                    }
                    break;
                case 1:
                    var Prediction1 = FSpred.Prediction.Prediction.GetPrediction(W, target, true);
                    if ((int) Prediction1.Hitchance >= hitchange)
                    {
                        return W.Cast(Prediction1.CastPosition);
                    }
                    break;
                case 2:
                    var Prediction2 = SebbyLibPorted.Prediction.Prediction.GetPrediction(W, target, true);
                    if ((int) Prediction2.Hitchance >= hitchange)
                    {
                        return W.Cast(Prediction2.CastPosition);
                    }
                    break;
            }
            return false;
        }
        
        private static bool EPredictionCasted(this AIBaseClient target)
        {
            if (target == null || !E.IsReady())
                return false;

            if (Orbwalker.ActiveMode == OrbwalkerMode.Combo && !XerathMenu.EMenu.UseECombo.Enabled)
                return false;
            if (Orbwalker.ActiveMode == OrbwalkerMode.Harass && !XerathMenu.EMenu.UseEHarass.Enabled)
                return false;

            int hitchange = 3;
            switch (XerathMenu.PredictionMenu.E.EHitchance.Index)
            {
                case 0:
                    hitchange = 2;
                    break;
                case 1:
                    hitchange = 3;
                    break;
                case 2:
                    hitchange = 4;
                    break;
            }
            
            switch (XerathMenu.PredictionMenu.E.EPred.Index)
            {
                case 0:
                    var Prediction0 = E.GetPrediction(target);
                    if ((int) Prediction0.Hitchance >= hitchange)
                    {
                        return E.Cast(Prediction0.CastPosition);
                    }
                    break;
                case 1:
                    var Prediction1 = FSpred.Prediction.Prediction.GetPrediction(E, target);
                    if ((int) Prediction1.Hitchance >= hitchange)
                    {
                        return E.Cast(Prediction1.CastPosition);
                    }
                    break;
                case 2:
                    var Prediction2 = SebbyLibPorted.Prediction.Prediction.GetPrediction(E, target);
                    if ((int) Prediction2.Hitchance >= hitchange)
                    {
                        return E.Cast(Prediction2.CastPosition);
                    }
                    break;
            }
            
            return false;
        }
        
        private static bool RPredictionCasted(this AIBaseClient target)
        {
            if (target == null)
                return false;

            int hitchange = 3;
            switch (XerathMenu.PredictionMenu.R.RHitchance.Index)
            {
                case 0:
                    hitchange = 2;
                    break;
                case 1:
                    hitchange = 3;
                    break;
                case 2:
                    hitchange = 4;
                    break;
            }
            
            switch (XerathMenu.PredictionMenu.R.RPred.Index)
            {
                case 0:
                    var Prediction0 = R.GetPrediction(target);
                    if ((int) Prediction0.Hitchance >= hitchange)
                    {
                        return R.Cast(Prediction0.CastPosition);
                    }
                    break;
                case 1:
                    var Prediction1 = FSpred.Prediction.Prediction.GetPrediction(R, target);
                    if ((int) Prediction1.Hitchance >= hitchange)
                    {
                        return R.Cast(Prediction1.CastPosition);
                    }
                    break;
                case 2:
                    var Prediction2 = SebbyLibPorted.Prediction.Prediction.GetPrediction(R, target);
                    if ((int) Prediction2.Hitchance >= hitchange)
                    {
                        return R.Cast(Prediction2.CastPosition);
                    }
                    break;
            }
            
            return false;
        }
    }
}