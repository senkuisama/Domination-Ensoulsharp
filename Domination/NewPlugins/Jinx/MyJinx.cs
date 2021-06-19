using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsoulSharp;

namespace DominationAIO.NewPlugins
{
    public static class MyJinx
    {
        private static class JinxMenu
        {
            public static class Qmenu
            {
                public static MenuBool useQ = new MenuBool("-useQ", "- Use Q");
                public static MenuBool autoQ = new MenuBool("autoQ", "- Auto Q");
            }
            public static class Wmenu
            {
                public static MenuBool useW = new MenuBool("-useW", "- Use W");
                public static MenuBool outrange = new MenuBool("Out Range", "When target not in range");
                public static MenuSlider range = new MenuSlider("-range", "- ", 650, 0, 1300);
            }
            public static class Emenu
            {
                public static MenuBool useE = new MenuBool("-useE", "- Use E");
                public static MenuBool antigap = new MenuBool("-anti gap", "Anti Gapcloser with E");
            }
            public static class Rmenu
            {
                public static MenuSeparator baseult = new MenuSeparator("Base Ult", "Default Base Ult");
            }
        }
        private static Menu Jmenu = null;
        private static Spell _Q, _W, _E, _R;
        private static double[] BonusQRange = new double[] { 0 , 100 , 125 , 150 , 175 , 200 };
        public static void LoadJinx()
        {
            Jmenu = new Menu("FunnySlayer Jinx", "FunnySlayer Jinx", true);
            Menu Q = new Menu("Qmenu", "Q Settings");
            Menu W = new Menu("Wmenu", "W Settings");
            Menu E = new Menu("Emenu", "E Settings");
            Menu R = new Menu("Rmenu", "R Settings");

            Q.Add(JinxMenu.Qmenu.useQ);
            Q.Add(JinxMenu.Qmenu.autoQ);
            W.Add(JinxMenu.Wmenu.useW);
            W.Add(JinxMenu.Wmenu.outrange);
            W.Add(JinxMenu.Wmenu.range);
            E.Add(JinxMenu.Emenu.useE);
            E.Add(JinxMenu.Emenu.antigap);
            R.Add(JinxMenu.Rmenu.baseult);
            Jmenu.Add(Q);
            Jmenu.Add(W);
            Jmenu.Add(E);
            Jmenu.Add(R);
            Jmenu.Attach();


            _Q = new Spell(SpellSlot.Q);
            _W = new Spell(SpellSlot.W, 1500);
            _E = new Spell(SpellSlot.E, 900);
            _W.SetSkillshot(0.5f, 60f, 3300f, true, SpellType.Line);
            _E.SetSkillshot(0.75f, 70f, 1750f, false, SpellType.Circle);

            Game.OnUpdate += Game_OnUpdate;
            Game.OnUpdate += Game_OnUpdate1;
        }

        private static void Game_OnUpdate1(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
                return;
            bool HaveQBuff = ObjectManager.Player.HasBuff("JinxQ");
            if (FunnySlayerCommon.OnAction.OnAA || FunnySlayerCommon.OnAction.BeforeAA)
                return;
            if (HaveQBuff && _Q.IsReady() && JinxMenu.Qmenu.autoQ.Enabled)
            {
                if(TargetSelector.GetTarget(1300, DamageType.Physical) == null)
                {
                    if (_Q.Cast())
                        return;
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            _W.Delay = Math.Max(0.4f, 0.6f - (ObjectManager.Player.PercentAttackSpeedMod - 100) / 25 * (0.02f));

            if (ObjectManager.Player.IsDead || Orbwalker.ActiveMode > OrbwalkerMode.Harass)
                return;

            bool HaveQBuff = ObjectManager.Player.HasBuff("JinxQ");           
            if (FunnySlayerCommon.OnAction.OnAA || FunnySlayerCommon.OnAction.BeforeAA)
                return;

            if (_W.IsReady() && JinxMenu.Wmenu.useW.Enabled)
            {
                var targets = TargetSelector.GetTargets(1500, DamageType.Physical);

                if(targets != null)
                {
                    AIHeroClient gettarget = targets.Where(i => _W.GetPrediction(i).Hitchance >= HitChance.High).OrderBy(i => i.Health).FirstOrDefault();
                    if(Orbwalker.GetTarget() == null && gettarget != null)
                    {
                        if((gettarget.DistanceToPlayer() >= JinxMenu.Wmenu.range.Value || !JinxMenu.Wmenu.outrange.Enabled))
                        {
                            var pred = _W.GetPrediction(gettarget);

                            if (pred.CastPosition.IsValid() && pred.Hitchance >= HitChance.High)
                            {
                                if (_W.Cast(pred.CastPosition))
                                    return;
                            }
                        }                       
                    }
                }
            }

            if (_E.IsReady() && JinxMenu.Emenu.useE.Enabled)
            {
                var targets = TargetSelector.GetTargets(900, DamageType.Physical);

                if (targets != null)
                {
                    AIHeroClient gettarget = targets.Where(i => _E.GetPrediction(i).Hitchance >= HitChance.High).OrderBy(i => i.Health).FirstOrDefault();
                    if (Orbwalker.GetTarget() != null && gettarget != null)
                    {
                        var pred = _E.GetPrediction(gettarget);

                        if (pred.CastPosition.IsValid() && pred.Hitchance >= HitChance.High)
                        {
                            if (_E.Cast(pred.CastPosition))
                                return;
                        }
                    }
                }
            }

            if (_Q.IsReady() && JinxMenu.Qmenu.useQ.Enabled)
            {
                if (HaveQBuff)
                {
                    if(Orbwalker.GetTarget() != null)
                    {
                        var target = TargetSelector.GetTarget((float)(ObjectManager.Player.GetCurrentAutoAttackRange() - BonusQRange[_Q.Level]), DamageType.Physical);
                        if(target != null)
                        {
                            if(target.NetworkId == Orbwalker.GetTarget().NetworkId)
                            {
                                if (_Q.Cast())
                                    return;
                            }
                        }
                    }
                }
                else
                {
                    if(Orbwalker.GetTarget() == null)
                    {
                        var target = TargetSelector.GetTarget((float)(ObjectManager.Player.GetCurrentAutoAttackRange() + BonusQRange[_Q.Level]), DamageType.Physical);                    
                        if(target != null)
                        {
                            if (_Q.Cast())
                                return;
                        }
                    }
                }
            }            
        }
    }
}
