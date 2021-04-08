using System;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;

namespace DaoHungAIO.Champions.Gangplank
{
    public static class BadaoGangplankHarass
    {
        public static void BadaoActivate ()
        {
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveMode != OrbwalkerMode.Harass)
                return;
            if (BadaoMainVariables.Q.IsReady() && BadaoGangplankVariables.HarassQ.GetValue<MenuBool>().Enabled)
            {
                var target = TargetSelector.GetTarget(BadaoMainVariables.Q.Range, DamageType.Physical);
                if (target.IsValidTarget())
                {
                    BadaoMainVariables.Q.Cast(target);
                }
            }
        }
    }
}
