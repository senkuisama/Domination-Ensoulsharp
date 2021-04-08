using System;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;

namespace DaoHungAIO.Champions.Gangplank
{
    public static class BadaoGangplankJungleClear
    {
        public static void BadaoActivate ()
        {
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveMode != OrbwalkerMode.LaneClear)
                return;
            if (!BadaoGangplankVariables.JungleQ.GetValue<MenuBool>().Enabled)
                return;
            foreach (AIMinionClient minion in GameObjects.GetJungles(ObjectManager.Player.Position, BadaoMainVariables.Q.Range))
            {
                if (minion.IsValidTarget() && BadaoMainVariables.Q.GetDamage(minion) >= minion.Health)
                {
                    BadaoMainVariables.Q.Cast(minion);
                }
            }
        }
    }
}
