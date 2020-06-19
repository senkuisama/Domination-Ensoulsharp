using EnsoulSharp;
using EnsoulSharp.SDK;
using System.Linq;

namespace PRADA_Vayne.MyUtils
{
    public static class MyWizard
    {
        public static bool UltActive()
        {
            return Heroes.Player.HasBuff("VayneInquisition");
        }

        public static bool TumbleActive()
        {
            return Heroes.Player.HasBuff("vaynetumblebonus");
        }

        public static bool ShouldSaveCondemn()
        {
            var katarina =
                GameObjects.EnemyHeroes.FirstOrDefault(h => h.CharacterName == "Katarina" && h.IsValidTarget(1400));
            if (katarina != null)
            {
                var kataR = katarina.GetSpell(SpellSlot.R);
                return kataR.IsReady() || katarina.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Ready;
            }

            var galio = GameObjects.EnemyHeroes.FirstOrDefault(h => h.CharacterName == "Galio" && h.IsValidTarget(1400));
            if (galio != null)
            {
                var galioR = galio.GetSpell(SpellSlot.R);
                return galioR.IsReady() || galio.Spellbook.CanUseSpell(SpellSlot.R) == SpellState.Ready;
            }

            return false;
        }
    }
}