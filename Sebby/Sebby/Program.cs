using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sebby
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            GameEvent.OnGameLoad += GameEvent_OnGameLoad;
        }

        private static void GameEvent_OnGameLoad()
        {
            string PredictionName = "Exory Prediction";
            EnsoulSharp.SDK.Prediction.AddPrediction(PredictionName, new GetIPrediction());
            DelayAction.Add(5000, () =>
            {
                EnsoulSharp.SDK.Prediction.SetPrediction(PredictionName);
            });
        }
    }
}
