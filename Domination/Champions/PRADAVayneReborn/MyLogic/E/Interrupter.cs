using EnsoulSharp.SDK;
using EnsoulSharp;
using System;

namespace PRADA_Vayne.MyLogic.E
{
    public static partial class Events
    {
        public static void OnPossibleToInterrupt(AIHeroClient sender, Interrupter.InterruptSpellArgs arg)
        {
            if (sender == null) return;
            if (Program.E.IsReady() &&
                Program.E.IsInRange(sender) && sender.CharacterName != "Shyvana" &&
                sender.CharacterName != "Vayne")
            {
                Console.WriteLine("Interrupter");
                Program.E.Cast(sender);
            }
        }
    }
}