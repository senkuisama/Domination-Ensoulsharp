using System;
using System.Linq;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;

namespace DaoHungAIO.Champions.Gangplank
{
    public static class BadaoGangplankLaneClear
    {
        public static void BadaoActivate()
        {
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveMode != OrbwalkerMode.LaneClear)
                return;
            if (!BadaoGangplankVariables.LaneQ.GetValue<MenuBool>().Enabled)
                return;
            foreach (AIMinionClient minion in GameObjects.GetMinions(BadaoMainVariables.Q.Range).OrderBy(x => x.Health))
            {
                if (minion.IsValidTarget() && BadaoMainVariables.Q.GetDamage(minion) >= minion.Health && !(minion.InAutoAttackRange()))
                {
                    BadaoMainVariables.Q.Cast(minion);
                }
            }
        }
    }
}
