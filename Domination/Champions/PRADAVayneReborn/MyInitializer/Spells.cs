using EnsoulSharp;
using EnsoulSharp.SDK;

namespace PRADA_Vayne.MyInitializer
{
    public static partial class PRADALoader
    {
        public static void LoadSpells()
        {
            Program.Q = new Spell(SpellSlot.Q, 300f);
            Program.W = new Spell(SpellSlot.W);
            Program.E = new Spell(SpellSlot.E, 550f);
            Program.R = new Spell(SpellSlot.R);
        }
    }
}