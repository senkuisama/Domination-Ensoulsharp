using EnsoulSharp;
using EnsoulSharp.SDK;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Remoting;
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
        public static bool SheenReady()
        {
            if (Variables.GameTimeTickCount > Irelia.SheenTimer + 1700 + Game.Ping 
                && ((ObjectManager.Player.CanUseItem((int)ItemId.Divine_Sunderer))
                || (ObjectManager.Player.CanUseItem((int)ItemId.Trinity_Force))
                || (ObjectManager.Player.CanUseItem((int)ItemId.Sheen)))
                )
            {
                return true;
            }

            return false;
        }
        public static float Sheen(AIBaseClient target = null)
        {
            if (Variables.GameTimeTickCount < Irelia.SheenTimer + 1700 + Game.Ping)
                return 0f;

            if (ObjectManager.Player.HasItem(ItemId.Divine_Sunderer) && ObjectManager.Player.CanUseItem((int)ItemId.Divine_Sunderer))
            {
                return target.MaxHealth * 0.1f;
            }
            
            else
            if (ObjectManager.Player.HasItem(ItemId.Trinity_Force) && ObjectManager.Player.CanUseItem((int)ItemId.Trinity_Force))
            {
                return ObjectManager.Player.BaseAttackDamage * 2;
            }
            
            else
            if (ObjectManager.Player.HasItem(ItemId.Sheen) && ObjectManager.Player.CanUseItem((int)ItemId.Sheen))
            {
                return ObjectManager.Player.BaseAttackDamage;
            }

            return 0f;
        }
        public static double GetQDmg(AIBaseClient target, bool CheckItem = true)
        {
            if (target == null)
                return 0f;

            var num1 = new double[5]{5, 25, 45, 65, 85}[Irelia.Q.Level - 1] 
                + 0.6 * (double)ObjectManager.Player.TotalAttackDamage;
            var num2 = new double[5] { 55, 75, 95, 115, 135 }[Irelia.Q.Level - 1];
            var qdmg = (target.Type != GameObjectType.AIMinionClient || !target.IsMinion() || target.IsJungle()) ? num1 : num1 + num2;

            if (CheckItem == true && SheenReady())
            {
                qdmg += Sheen(target);
            }
            var alldmg = ObjectManager.Player.CalculatePhysicalDamage(target, qdmg);
            if (ObjectManager.Player.HasBuff("ireliapassivestacksmax"))
            {
                var PassiveDmg = 15.0f + (ObjectManager.Player.Level - 1) * 3.0f + ObjectManager.Player.GetBonusPhysicalDamage() * 0.25f;
                alldmg += ObjectManager.Player.CalculateMagicDamage(target, PassiveDmg);
            }

            return alldmg;
        }

        public static bool CanQ(AIBaseClient target, bool CheckItems = true)
        {
            /*if (Variables.GameTimeTickCount - Irelia.lastQ <= MenuSettings.QSettings.Qdelay.Value)
                return false;
            */
            if (target.HasBuff("ireliamark"))
                return true;

            CheckItems = MenuSettings.QSettings.CheckQDmgITems.Enabled;
            
            if(target.Type == GameObjectType.AIHeroClient)
            {
                if (target.Health <= GetQDmg(target, CheckItems) + GetQDmg(target, CheckItems) * 0.08f + (ObjectManager.Player.HasItem((int)ItemId.The_Collector) ? 0.05f * target.MaxHealth : 0))
                {
                    return true;
                }
            }
            else
            {
                if (target.Health < GetQDmg(target, CheckItems))
                {
                    return true;
                }
            }

            return false;    
        }
    }
}
