using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsoulSharp;
using EnsoulSharp.SDK;

namespace DominationAIO.Champions.Helper
{
    public class MinionManager
    {
        public static IEnumerable<AIMinionClient> GetEnemyMinions(float range)
        {
            return GameObjects.EnemyMinions.Where(x => x.IsValidTarget(range) && x.IsEnemy && x.IsMinion());
        }
        public static IEnumerable<AIMinionClient> GetJungleMobs(float range)
        {
            return GameObjects.Jungle.Where(x => x.IsValidTarget(range) && !x.IsAlly);
        }

        public static IEnumerable<AIMinionClient> GetAllyMinions(float range)
        {
            return GameObjects.AllyMinions.Where(x => x.IsValidTarget(range) && x.IsAlly);
        }

        public static IEnumerable<AIMinionClient> GetLegendaryMobs(float range)
        {
            return GameObjects.Jungle.Where(x => x.IsValidTarget(range) && x.GetJungleType() == JungleType.Legendary);
        }
        public static IEnumerable<AIMinionClient> GetLargeMobs(float range)
        {
            return GameObjects.Jungle.Where(x => x.IsValidTarget(range) && x.GetJungleType() == JungleType.Large);
        }
    }
}
