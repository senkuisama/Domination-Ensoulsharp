using DominationAIO.Champions.Helper;
using EnsoulSharp.SDK;
using EnsoulSharp;
using PRADA_Vayne.MyUtils;

namespace PRADA_Vayne.MyLogic.Q
{
    public static partial class Events
    {
        public static void OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserArgs args)
        {
            if (Heroes.Player.Distance(args.EndPosition) < 400)
                Tumble.Cast(Heroes.Player.Position.Extend(args.EndPosition, -300));
        }
    }
}