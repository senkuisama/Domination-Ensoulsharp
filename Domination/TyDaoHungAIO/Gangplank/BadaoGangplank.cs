using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsoulSharp;
using EnsoulSharp.SDK;
using SharpDX;
using Color = System.Drawing.Color;

namespace DaoHungAIO.Champions.Gangplank
{
    using static BadaoMainVariables;
    using static BadaoGangplankVariables;
    public static class BadaoGangplank
    {
        public static void BadaoActivate()
        {
            BadaoGangplankConfig.BadaoActivate();
            BadaoGangplankBarrels.BadaoActivate();
            BadaoGangplankCombo.BadaoActivate();
            BadaoGangplankHarass.BadaoActivate();
            BadaoGangplankLaneClear.BadaoActivate();
            BadaoGangplankJungleClear.BadaoActivate();
            BadaoGangplankAuto.BadaoActivate();
            //Game.OnUpdate += Game_OnUpdate;
        }
        private static void Game_OnUpdate(EventArgs args)
        {

        }
    }
}
