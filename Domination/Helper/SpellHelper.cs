using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsoulSharp.SDK;
using EnsoulSharp;
using SharpDX;

namespace DominationAIO.Champions.Helper
{
    public static class SpellHelper
    {
        public static bool SpellIsReadyAndActive(this Spell spell, bool menuSpellActive)
        {
            return spell.IsReady() && menuSpellActive;
        }
    }
    class SummonerItems
    {
        private AIHeroClient player;
        private Spellbook sumBook;
        private SpellSlot ignite;
        private SpellSlot smite;


        public enum ItemIds
        {
            //MuramanaDe = 3043,
            Muramana = 3042,
            Tiamat = 3077,
            Hydra = 3074,
            MercScim = 3139,
            Hextech = 3146,
            SwordOD = 3131,
            Ghostblade = 3142,
            BotRK = 3153,
            Cutlass = 3144,

            Omen = 3143
        }

        public SummonerItems(AIHeroClient myHero)
        {
            player = myHero;
            sumBook = player.Spellbook;
            ignite = player.GetSpellSlot("summonerdot");
            smite = player.GetSpellSlot("SummonerSmite");
        }

        public void castIgnite(AIHeroClient target)
        {
            if (ignite != SpellSlot.Unknown && sumBook.CanUseSpell(ignite) == SpellState.Ready)
                sumBook.CastSpell(ignite, target);
        }

        public void castSmite(AIHeroClient target)
        {
            if (smite != SpellSlot.Unknown && sumBook.CanUseSpell(smite) == SpellState.Ready)
                sumBook.CastSpell(smite, target);
        }

        public void cast(ItemIds item)
        {
            var itemId = (int)item;
            if (Items.CanUseItem(ObjectManager.Player, itemId))
                Items.UseItem(ObjectManager.Player, itemId);
        }

        public void cast(ItemIds item, Vector3 target)
        {
            var itemId = (int)item;
            if (Items.CanUseItem(ObjectManager.Player, itemId))
                player.Spellbook.CastSpell(getInvSlot(itemId).SpellSlot, target);

        }

        public void cast(ItemIds item, AIBaseClient target)
        {
            var itemId = (int)item;
            if (Items.CanUseItem(ObjectManager.Player, itemId))
                Items.UseItem(ObjectManager.Player, itemId, target);
        }

        private InventorySlot getInvSlot(int id)
        {
            return player.InventoryItems.FirstOrDefault(iSlot => (int)iSlot.Id == id);
        }
    }
}
