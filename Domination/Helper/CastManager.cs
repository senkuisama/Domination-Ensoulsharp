using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsoulSharp;
using EnsoulSharp.SDK;

namespace DominationAIO.Champions.Helper
{
    public static class CastManager
    {
        public static void CastSkillShot(this Spell spell, HitChance hitChance, AIBaseClient target, bool aoe)
        {
            var pred = spell.GetPrediction(target, aoe);
            if (pred.Hitchance >= hitChance)
            {
                spell.Cast(pred.CastPosition);
            }
        }
        public static void CastTargeted(this Spell spell, AIBaseClient target, bool aoe = false)
        {
            spell.CastOnUnit(target);
        }
    }
}
