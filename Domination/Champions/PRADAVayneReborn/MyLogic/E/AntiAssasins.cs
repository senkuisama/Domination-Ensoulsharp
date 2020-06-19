using EnsoulSharp;
using EnsoulSharp.SDK;
using PRADA_Vayne.MyUtils;
using System;
using System.Linq;

namespace PRADA_Vayne.MyLogic.E
{
    public static class AntiAssasins
    {
        public static void OnCreateGameObject(GameObject sender, EventArgs args)
        {
            if (sender.Name.ToLower().Contains("leapsound.troy"))
            {
                var rengo = Heroes.EnemyHeroes.FirstOrDefault(h => h.CharacterName == "Rengar");
                if (rengo.IsValidTarget(545) && Program.E.IsReady()) Program.E.Cast(rengo);
            }
        }
    }
}