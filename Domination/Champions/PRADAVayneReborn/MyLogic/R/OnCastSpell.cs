using DominationAIO.Champions.Helper;
using EnsoulSharp;
using EnsoulSharp.SDK;
using PRADA_Vayne.MyLogic.Q;
using PRADA_Vayne.MyUtils;

namespace PRADA_Vayne.MyLogic.R
{
    public static partial class Events
    {
        public static void OnCastSpell(Spellbook spellbook, SpellbookCastSpellEventArgs args)
        {            
            if (spellbook.Owner.IsMe)
                if (args.Slot == SpellSlot.R && Program.MainMenu.GetMenuBool("Combo Settings", "QR"))
                {
                    var target = TargetSelector.GetTarget(300);
                    var tumblePos = target != null ? target.GetTumblePos() : Game.CursorPos;
                    Tumble.Cast(tumblePos);
                }
        }
    }
}