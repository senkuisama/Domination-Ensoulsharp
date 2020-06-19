using EnsoulSharp;
using PRADA_Vayne.MyUtils;
using SharpDX;

namespace PRADA_Vayne.MyLogic.Q
{
    public static partial class Events
    {
        public static void OnCastSpell(Spellbook spellbook, SpellbookCastSpellEventArgs args)
        {
            if (spellbook.Owner.IsMe)
                if (args.Slot == SpellSlot.Q)
                    if (Tumble.TumbleOrderPos != Vector3.Zero)
                    {
                        if (Tumble.TumbleOrderPos.IsDangerousPosition())
                        {
                            Tumble.TumbleOrderPos = Vector3.Zero;
                            args.Process = false;
                        }
                        else
                        {
                            Tumble.TumbleOrderPos = Vector3.Zero;
                        }
                    }
        }
    }
}