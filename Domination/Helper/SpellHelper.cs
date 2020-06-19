using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsoulSharp.SDK;

namespace DominationAIO.Champions.Helper
{
    public static class SpellHelper
    {
        public static bool SpellIsReadyAndActive(this Spell spell, bool menuSpellActive)
        {
            return spell.IsReady() && menuSpellActive;
        }
    }
}
