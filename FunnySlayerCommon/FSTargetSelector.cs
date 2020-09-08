using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunnySlayerCommon
{
    public static class FSTargetSelector
    {
        public static AIHeroClient GetFSTarget(float range = 0)
        {
            if(MenuClass.SecondMenu.Index == 0)
            {
                return TargetSelector.SelectedTarget;
            }
            if (MenuClass.SecondMenu.Index == 1)
            {
                #region INDEX
                if (MenuClass.GetWeight.Index == 0)
                {
                    return GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).OrderBy(i => i.Health).FirstOrDefault();
                }
                if (MenuClass.GetWeight.Index == 01)
                {
                    return GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).OrderBy(i => i.Armor).FirstOrDefault();
                }
                if (MenuClass.GetWeight.Index == 02)
                {
                    return GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).OrderBy(i => i.Mana).FirstOrDefault();
                }
                if (MenuClass.GetWeight.Index == 03)
                {
                    return GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).OrderBy(i => i.Health / ObjectManager.Player.GetAutoAttackDamage(i)).FirstOrDefault();
                }
                if (MenuClass.GetWeight.Index == 04)
                {
                    return GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).MaxOrDefault(i => i.Health);
                }
                if (MenuClass.GetWeight.Index == 05)
                {
                    return GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).MaxOrDefault(i => i.Armor);
                }
                if (MenuClass.GetWeight.Index == 06)
                {
                    return GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).MaxOrDefault(i => i.Mana);
                }
                if (MenuClass.GetWeight.Index == 07)
                {
                    return GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).MaxOrDefault(i => i.Health / ObjectManager.Player.GetAutoAttackDamage(i));
                }
                if (MenuClass.GetWeight.Index == 08)
                {
                    return GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).FirstOrDefault(i => !i.CanMove);
                }
                if (MenuClass.GetWeight.Index == 09)
                {
                    return GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).FirstOrDefault(i => i.CanMove);
                }
                if (MenuClass.GetWeight.Index == 10)
                {
                    return GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).FirstOrDefault(i => !i.CanAttack);
                }
                if (MenuClass.GetWeight.Index == 11)
                {
                    return GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).FirstOrDefault(i => i.CanAttack);
                }
                if (MenuClass.GetWeight.Index == 12)
                {
                    return GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).FirstOrDefault(i => i.IsRanged);
                }
                if (MenuClass.GetWeight.Index == 13)
                {
                    return GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).FirstOrDefault(i => i.IsMelee);
                }
                #endregion
            }
            if (MenuClass.SecondMenu.Index == 2)
            {
                /*return GameObjects.EnemyHeroes
                    .Where(i => i.IsValidTarget(range))
                    .MaxOrDefault(i => MenuClass.ThisMenu["GetPriority"][i.CharacterName].GetValue<MenuSlider>().Value);*/
                return TargetSelector.GetTarget(range);
            }

            return null;
        }
    }
}
