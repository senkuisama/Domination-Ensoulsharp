using EnsoulSharp;
using EnsoulSharp.SDK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunnySlayerCommon
{
    public static class FSspell
    {
        public static bool IsReadyToCastOn(this Spell spell, AIHeroClient target, int maxToggleState = 1)
        {
            return spell.IsReady() && cancast(spell, maxToggleState) && spell.CanCast(target) && target.IsValidTarget(spell.Range) && !target.IsDead;
        }
        public static bool cancast(Spell spell, int maxToggleState = 1)
        {
            if (ObjectManager.Player.CharacterName.ToLowerInvariant() == "vladimir")
            {
                return true;
            }
            else
            {
                return ObjectManager.Player.Mana > spell.Mana && (int)spell.ToggleState <= maxToggleState;
            }
        }
    }
}
