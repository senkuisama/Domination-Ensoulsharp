using EnsoulSharp;
using EnsoulSharp.SDK;
using FunnySlayerCommon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominationAIO.NewPlugins
{
    public static class LogicR
    {
        public static Geometry.Rectangle GetRPos1;
        public static Geometry.Rectangle GetRPos2;
        public static bool IRELIA_RCOMBO()
        {
            if (ObjectManager.Player.IsDead)
            {
                GetRPos1 = null;
                GetRPos2 = null;
                return false;
            }
           
            if (!Irelia.R.IsReady() || !MenuSettings.RSettings.Rcombo.Enabled)
            {
                GetRPos1 = null;
                GetRPos2 = null;
                return false;
            }

            var target = TargetSelector.GetTarget(1000, DamageType.Physical);
            if (target == null || target.HasBuff("ireliamark"))
            {
                GetRPos1 = null;
                GetRPos2 = null;
                return false;
            }
            var pred = FSpred.Prediction.Prediction.PredictUnitPosition(target, 600);
            if (pred.DistanceToPlayer() <= Irelia.R.Range)
            {
                if (target.HealthPercent <= MenuSettings.RSettings.Rheath.Value && pred.DistanceToPlayer() < Irelia.R.Range && target.IsValidTarget(Irelia.R.Range))
                {
                    if (Irelia.R.Cast(pred))
                        return true;
                }
                GetRPos1 = new Geometry.Rectangle(ObjectManager.Player.Position, pred.ToVector3(), 110);
                GetRPos2 = new Geometry.Rectangle(pred, pred.Extend(ObjectManager.Player.Position, -450), 300);

                var TargetCount1 = GameObjects.EnemyHeroes.Where(i => GetRPos1.IsInside(i.Position)).Count();
                var TargetCount2 = GameObjects.EnemyHeroes.Where(i => GetRPos2.IsInside(i.Position) && !GameObjects.EnemyHeroes.Where(a => GetRPos1.IsInside(a.Position)).Any(h => i.NetworkId == h.NetworkId)).Count();

                if (TargetCount1 >= MenuSettings.RSettings.Rhit.Value
                    || TargetCount2 >= MenuSettings.RSettings.Rhit.Value
                    || TargetCount1 + TargetCount2 >= MenuSettings.RSettings.Rhit.Value
                    )
                {
                    if (pred.DistanceToPlayer() <= 980)
                        if (Irelia.R.Cast(pred))
                            return true;
                }
            }
            else
            {
                GetRPos1 = null;
                GetRPos2 = null;
                return false;
            }

            return false;
        }
    }
}
