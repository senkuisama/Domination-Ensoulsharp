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
        public static AIHeroClient GetFSTarget(float range = 0)
        {
            if(TargetSelector.SelectedTarget != null && TargetSelector.SelectedTarget.IsValidTarget(range))
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
                    #region INDEX
                    if (MenuClass.GetWeight.Index == 0)
                    {
                        var x = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).OrderBy(i => i.Health).FirstOrDefault();
                        if (x != null)
                        {
                            return x;
                        }
                        else
                        {
                            var aI = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).OrderBy(i => MenuClass.GetSPriority(i).Value).LastOrDefault();
                            if (aI != null)
                            {
                                return x;
                            }
                            else
                            {
                                return TargetSelector.GetTarget(range);
                            }
                        }
                    }
                    if (MenuClass.GetWeight.Index == 01)
                    {
                        var x = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).OrderBy(i => i.Armor).FirstOrDefault();
                        if (x != null)
                        {
                            return x;
                        }
                        else
                        {
                            var aI = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).OrderBy(i => MenuClass.GetSPriority(i).Value).LastOrDefault();
                            if (aI != null)
                            {
                                return x;
                            }
                            else
                            {
                                return TargetSelector.GetTarget(range);
                            }
                        }
                    }
                    if (MenuClass.GetWeight.Index == 02)
                    {
                        var x = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).OrderBy(i => i.Mana).FirstOrDefault();
                        if (x != null)
                        {
                            return x;
                        }
                        else
                        {
                            var aI = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).OrderBy(i => MenuClass.GetSPriority(i).Value).LastOrDefault();
                            if (aI != null)
                            {
                                return x;
                            }
                            else
                            {
                                return TargetSelector.GetTarget(range);
                            }
                        }
                    }
                    if (MenuClass.GetWeight.Index == 03)
                    {
                        var x = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).OrderBy(i => i.Health / ObjectManager.Player.GetAutoAttackDamage(i)).FirstOrDefault();
                        if (x != null)
                        {
                            return x;
                        }
                        else
                        {
                            var aI = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).OrderBy(i => MenuClass.GetSPriority(i).Value).LastOrDefault();
                            if (aI != null)
                            {
                                return x;
                            }
                            else
                            {
                                return TargetSelector.GetTarget(range);
                            }
                        }
                    }
                    if (MenuClass.GetWeight.Index == 04)
                    {
                        var x = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).MaxOrDefault(i => i.Health);
                        if (x != null)
                        {
                            return x;
                        }
                        else
                        {
                            var aI = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).OrderBy(i => MenuClass.GetSPriority(i).Value).LastOrDefault();
                            if (aI != null)
                            {
                                return x;
                            }
                            else
                            {
                                return TargetSelector.GetTarget(range);
                            }
                        }
                    }
                    if (MenuClass.GetWeight.Index == 05)
                    {
                        var x = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).MaxOrDefault(i => i.Armor);
                        if (x != null)
                        {
                            return x;
                        }
                        else
                        {
                            var aI = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).OrderBy(i => MenuClass.GetSPriority(i).Value).LastOrDefault();
                            if (aI != null)
                            {
                                return x;
                            }
                            else
                            {
                                return TargetSelector.GetTarget(range);
                            }
                        }
                    }
                    if (MenuClass.GetWeight.Index == 06)
                    {
                        var x = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).MaxOrDefault(i => i.Mana);
                        if (x != null)
                        {
                            return x;
                        }
                        else
                        {
                            var aI = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).OrderBy(i => MenuClass.GetSPriority(i).Value).LastOrDefault();
                            if (aI != null)
                            {
                                return x;
                            }
                            else
                            {
                                return TargetSelector.GetTarget(range);
                            }
                        }
                    }
                    if (MenuClass.GetWeight.Index == 07)
                    {
                        var x = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).MaxOrDefault(i => i.Health / ObjectManager.Player.GetAutoAttackDamage(i));
                        if (x != null)
                        {
                            return x;
                        }
                        else
                        {
                            var aI = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).OrderBy(i => MenuClass.GetSPriority(i).Value).LastOrDefault();
                            if (aI != null)
                            {
                                return x;
                            }
                            else
                            {
                                return TargetSelector.GetTarget(range);
                            }
                        }
                    }
                    if (MenuClass.GetWeight.Index == 08)
                    {
                        var x = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).FirstOrDefault(i => !i.CanMove);
                        if (x != null)
                        {
                            return x;
                        }
                        else
                        {
                            var aI = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).OrderBy(i => MenuClass.GetSPriority(i).Value).LastOrDefault();
                            if (aI != null)
                            {
                                return x;
                            }
                            else
                            {
                                return TargetSelector.GetTarget(range);
                            }
                        }
                    }
                    if (MenuClass.GetWeight.Index == 09)
                    {
                        var x = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).FirstOrDefault(i => i.CanMove);
                        if (x != null)
                        {
                            return x;
                        }
                        else
                        {
                            var aI = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).OrderBy(i => MenuClass.GetSPriority(i).Value).LastOrDefault();
                            if (aI != null)
                            {
                                return x;
                            }
                            else
                            {
                                return TargetSelector.GetTarget(range);
                            }
                        }
                    }
                    if (MenuClass.GetWeight.Index == 10)
                    {
                        var x = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).FirstOrDefault(i => !i.CanAttack);
                        if (x != null)
                        {
                            return x;
                        }
                        else
                        {
                            var aI = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).OrderBy(i => MenuClass.GetSPriority(i).Value).LastOrDefault();
                            if (aI != null)
                            {
                                return x;
                            }
                            else
                            {
                                return TargetSelector.GetTarget(range);
                            }
                        }
                    }
                    if (MenuClass.GetWeight.Index == 11)
                    {
                        var x = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).FirstOrDefault(i => i.CanAttack);
                        if (x != null)
                        {
                            return x;
                        }
                        else
                        {
                            var aI = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).OrderBy(i => MenuClass.GetSPriority(i).Value).LastOrDefault();
                            if (aI != null)
                            {
                                return x;
                            }
                            else
                            {
                                return TargetSelector.GetTarget(range);
                            }
                        }
                    }
                    if (MenuClass.GetWeight.Index == 12)
                    {
                        var x = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).FirstOrDefault(i => i.IsRanged);
                        if (x != null)
                        {
                            return x;
                        }
                        else
                        {
                            var aI = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).OrderBy(i => MenuClass.GetSPriority(i).Value).LastOrDefault();
                            if (aI != null)
                            {
                                return x;
                            }
                            else
                            {
                                return TargetSelector.GetTarget(range);
                            }
                        }
                    }
                    if (MenuClass.GetWeight.Index == 13)
                    {
                        var x = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).FirstOrDefault(i => i.IsMelee);
                        if (x != null)
                        {
                            return x;
                        }
                        else
                        {
                            var aI = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).OrderBy(i => MenuClass.GetSPriority(i).Value).LastOrDefault();
                            if (aI != null)
                            {
                                return x;
                            }
                            else
                            {
                                return TargetSelector.GetTarget(range);
                            }
                        }
                    }
                    #endregion
                }
                if (MenuClass.SecondMenu.Index == 2)
                {
                    var x = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(range)).OrderBy(i => MenuClass.GetSPriority(i).Value).LastOrDefault();
                    if (x != null)
                    {
                        return x;
                    }
                    else
                    {
                        return TargetSelector.GetTarget(range);
                    }
                }
            }        
            
            return null;
        }
    }
}
