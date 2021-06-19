using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunnySlayerCommon
{
    public static class FSTargetSelector
    {
        /// <summary>
        /*public static string[] TargetWeight = new string[]
        {
                "Heath", //0
                "Armor", //1
                "Mana", //2
                "Auto Attack to die", //3
                "Ranged", //4
                "Melee", //5
        };*/
        ///</summary>
        /// <param name="range"></param>
        /// <returns></returns>
    public static AIHeroClient GetFSTarget(float range = 0)
        {

            if (range == 0)
                range = ObjectManager.Player.GetCurrentAutoAttackRange();

            AIHeroClient ThisTarget = null;
            List<AIHeroClient> ThisTargetList = null;
            if (TargetSelector.SelectedTarget != null && TargetSelector.SelectedTarget.IsValidTarget(range))
            {
                return TargetSelector.SelectedTarget;
            }
            else
            {
                if (MenuClass.SecondMenu.Index == 0)
                {
                    return TargetSelector.SelectedTarget;
                }
                if (MenuClass.SecondMenu.Index == 1)
                {
                    switch (MenuClass.GetWeight.Index)
                    {
                        case 0:
                            ThisTargetList = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).OrderBy(i => i.Health).ToList();
                            break;
                        case 1:
                            ThisTargetList = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).OrderBy(i => i.Armor).ToList();
                            break;
                        case 2:
                            ThisTargetList = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).OrderBy(i => i.Mana).ToList();
                            break;
                        case 3:
                            ThisTargetList = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).OrderBy(i => i.Health / ObjectManager.Player.GetAutoAttackDamage(i)).ToList();
                            break;
                        case 4:
                            ThisTargetList = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).OrderBy(i => i.CombatType == GameObjectCombatType.Ranged).ToList();
                            break;
                        case 5:
                            ThisTargetList = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).OrderBy(i => i.CombatType == GameObjectCombatType.Melee).ToList();
                            break;
                        default:
                            return TargetSelector.GetTarget(range, DamageType.Physical);
                    }
                }
            }
          
            if(MenuClass.SecondMenu.Index == 2)
            {
                ThisTargetList = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).OrderByDescending(i => MenuClass.GetSPriority(i)).ToList();
            }

            if (MenuClass.OrderByNearMouse.Enabled)
            {
                var thetargets = ThisTargetList.OrderBy(i => i.Position.Distance(Game.CursorPos)).ToList();

                if (MenuClass.OrderByValue.Enabled)
                {
                    ThisTarget = thetargets.FirstOrDefault();
                }
                else
                {
                    ThisTarget = thetargets.LastOrDefault();
                }
            }
            else
            {
                if (MenuClass.OrderByValue.Enabled)
                {
                    ThisTarget = ThisTargetList.FirstOrDefault();
                }
                else
                {
                    ThisTarget = ThisTargetList.LastOrDefault();
                }
            }

            if(ThisTarget != null)
            return ThisTarget;

            return TargetSelector.GetTarget(range, DamageType.Physical);
        }
    }
}
