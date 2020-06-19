using EnsoulSharp;
using EnsoulSharp.SDK;
using PRADA_Vayne.MyUtils;
using System;

namespace PRADA_Vayne.MyLogic.Others
{
    public static partial class Events
    {
        public static void OnUpdate(EventArgs args)
        {
            //ObjectManager.Player
            if (Heroes.Player.HasBuff("rengarralertsound"))
            {
                if (ObjectManager.Player.HasItem((int)ItemId.Oracle_Alteration) &&
                    ObjectManager.Player.CanUseItem((int)ItemId.Oracle_Alteration))
                    ObjectManager.Player.UseItem((int)ItemId.Oracle_Alteration, Heroes.Player.Position);
                else if (ObjectManager.Player.HasItem((int)ItemId.Control_Ward))
                    ObjectManager.Player.UseItem((int)ItemId.Control_Ward, Heroes.Player.Position.Randomize());
            }
        }
    }
}