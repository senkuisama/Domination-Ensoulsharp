using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsoulSharp;
using EnsoulSharp.SDK;

namespace DominationAIO.Champions.Helper
{
    public class HeroManager
    {
        public static IEnumerable<AIHeroClient> GetEnemyHeroes(float range)
        {
            return GameObjects.EnemyHeroes.Where(x => x.IsValidTarget(range) && x.IsEnemy && !x.IsDead && x.IsVisible);
        }
        public static IEnumerable<AIHeroClient> GetAllyHeroes(float range)
        {
            return GameObjects.AllyHeroes.Where(x => x.IsValidTarget(range) && x.IsAlly && !x.IsDead && x.IsVisible);
        }
    }
}
