using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sebby
{
    public class GetIPrediction : IPrediction
    {
        public GetIPrediction()
        {
            var menu = new Menu("      P", "Exory Prediction Loaded", true);
            menu.Attach();
        }

        public EnsoulSharp.SDK.PredictionOutput GetPrediction(EnsoulSharp.SDK.PredictionInput input)
        {
            var ChangeInput = new Sebby.PredictionInput()
            {
                From = input.From,
                RangeCheckFrom = input.RangeCheckFrom,
                Aoe = input.Aoe,
                Collision = input.Collision,
                CollisionObjects = input.CollisionObjects ?? new CollisionObjects[1],
                Delay = input.Delay,
                Radius = input.Radius,
                Range = input.Range,
                Speed = input.Speed,
                Type = input.Type,
                Unit = input.Unit,
                Source = ObjectManager.Player,
            };

            var GetPred = Sebby.Prediction.GetPrediction(ChangeInput);

            var OutPut = new EnsoulSharp.SDK.PredictionOutput()
            {
                AoeTargetsHitCount = GetPred.AoeTargetsHitCount,
                AoeTargetsHit = GetPred.AoeTargetsHit,
                Hitchance = (EnsoulSharp.SDK.HitChance)GetPred.Hitchance,
                CastPosition = GetPred.CastPosition,
                UnitPosition = GetPred.UnitPosition,
                CollisionObjects = GetPred.CollisionObjects,
            };

            return OutPut;
        }
        public EnsoulSharp.SDK.PredictionOutput GetPrediction(EnsoulSharp.SDK.PredictionInput input, bool ft, bool checkCollision)
        {
            var ChangeInput = new Sebby.PredictionInput()
            {
                From = input.From,
                RangeCheckFrom = input.RangeCheckFrom,
                Aoe = input.Aoe,
                Collision = input.Collision,
                CollisionObjects = input.CollisionObjects ?? new CollisionObjects[1],
                Delay = input.Delay,
                Radius = input.Radius,
                Range = input.Range,
                Speed = input.Speed,
                Type = input.Type,
                Unit = input.Unit,
                Source = ObjectManager.Player,
            };

            var GetPred = Sebby.Prediction.GetPrediction(ChangeInput, ft, checkCollision);

            var OutPut = new EnsoulSharp.SDK.PredictionOutput()
            {
                AoeTargetsHitCount = GetPred.AoeTargetsHitCount,
                AoeTargetsHit = GetPred.AoeTargetsHit,
                Hitchance = (EnsoulSharp.SDK.HitChance)GetPred.Hitchance,
                CastPosition = GetPred.CastPosition,
                UnitPosition = GetPred.UnitPosition,
                CollisionObjects = GetPred.CollisionObjects,
            };

            return OutPut;
        }
    }
}
