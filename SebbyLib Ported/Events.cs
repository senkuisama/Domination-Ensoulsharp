using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsoulSharp;
using EnsoulSharp.SDK;

namespace SebbyLib
{
    public static class Events
    {
        public static bool IsCastingInterruptableSpell(this AIBaseClient target, bool checkMovementInterruption = false)
        {
            var interruptableTargetData = GetInterruptableTargetData(target);
            return interruptableTargetData != null && (!checkMovementInterruption);
        }

        public static Interrupter.InterruptSpellDB GetInterruptableTargetData(AIBaseClient target)
        {
            if (Interrupter.CastingSpell.ContainsKey(target.NetworkId))
            {
                return Interrupter.CastingSpell[target.NetworkId];
            }
            if (Interrupter.CastingSpell.ContainsKey((uint)target.MemoryAddress))
            {
                return Interrupter.CastingSpell[(uint)target.MemoryAddress];
            }
            return null;
        }
    }
}
