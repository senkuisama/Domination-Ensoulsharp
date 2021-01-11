using EnsoulSharp;
using EnsoulSharp.SDK;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominationAIO.NewPlugins
{
    public static class Helper
    {
        public static bool UnderTower(Vector3 pos, float bonusrange = 0)
        {
            return
                ObjectManager.Get<AITurretClient>()
                    .Any(i => i.IsEnemy && !i.IsDead && (i.Distance(pos) < 850 + ObjectManager.Player.BoundingRadius + bonusrange));
        }
        public static float Sheen()
        {
            /*if (Variables.TickCount < Irelia.SheenTimer + 1550)
                return 0f;*/

            if (ObjectManager.Player.CanUseItem((int)ItemId.Trinity_Force) || ObjectManager.Player.HasBuff("3078trinityforce") || ObjectManager.Player.HasBuff("trinityforce"))
            {
                return ObjectManager.Player.BaseAttackDamage * 2;
            }
            if (ObjectManager.Player.CanUseItem((int)ItemId.Sheen) || ObjectManager.Player.HasBuff("sheen"))
            {
                return ObjectManager.Player.BaseAttackDamage;
            }

            return 0f;
        }
        public static float GetQDmg(AIBaseClient target, bool CheckItem = true)
        {
            if (target == null)
                return 0f;

            int level = ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level;
            if (level == 0)
            {
                return 0f;
            }
            var normaldmg = 5f + (level - 1) * 20f + ObjectManager.Player.TotalAttackDamage * 0.6f;
            if (target.IsMinion() && !target.IsJungle())
            {
                normaldmg += 55f + (float)(level - 1) * 20f;
            }
            var passivedmg = 0f;
            if (ObjectManager.Player.HasBuff("ireliapassivestacksmax"))
            {
                var PassiveDmg = 15f + (ObjectManager.Player.Level - 1) * 3f + ObjectManager.Player.GetBonusPhysicalDamage() * 0.25f;
                passivedmg = (float)EnsoulSharp.SDK.Damage.CalculateMagicDamage(ObjectManager.Player, target, (double)PassiveDmg);
            }
            if (CheckItem == true)
                normaldmg += Sheen();

            return (float)EnsoulSharp.SDK.Damage.CalculatePhysicalDamage(ObjectManager.Player, target, normaldmg) + passivedmg;
        }

        public static bool CanQ(AIBaseClient target, bool CheckItems = true)
        {
            if(target.Type == GameObjectType.AIHeroClient)
            {
                if (target.Health <= GetQDmg(target, CheckItems) + GetQDmg(target, CheckItems) * 0.08f || target.HasBuff("ireliamark"))
                {
                    return true;
                }
                else
                    return false;
            }
            else
            {
                if (target.Health <= GetQDmg(target, CheckItems) || target.HasBuff("ireliamark"))
                {
                    return true;
                }
                else
                    return false;
            }           
        }
    }
}
