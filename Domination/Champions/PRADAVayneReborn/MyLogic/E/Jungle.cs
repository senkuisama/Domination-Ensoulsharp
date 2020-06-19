using DominationAIO.Champions.Helper;
using EnsoulSharp;
using EnsoulSharp.SDK;
using PRADA_Vayne.MyUtils;
using System;
using System.Linq;

namespace PRADA_Vayne.MyLogic.E
{
    public static partial class Events
    {
        private static readonly string[] _jungleMobs =
        {
            "SRU_Blue", "SRU_Red", "SRU_Krug", "SRU_Gromp", "SRU_Murkwolf", "SRU_Razorbeak", "TT_Spiderboss",
            "TTNGolem",
            "TTNWraith", "TTNWolf"
        };

        public static void JungleUsage(EventArgs args)
        {
            var obj = GameObjects.Get<AIMinionClient>().Where(i => i.IsValidTarget() && _jungleMobs.Contains(i.CharacterName) && i.IsValidTarget(700));
            if(obj.Count() > 0)
            {
                foreach(var minion in obj)
                {
                    if (Program.MainMenu.GetMenuBool("Laneclear Settings", "EJungleMobs") && Program.E.IsReady())
                        for (var i = 40; i < 425; i += 141)
                        {
                            var flags = NavMesh.GetCollisionFlags(minion.Position.ToVector2()
                                .Extend(Heroes.Player.Position.ToVector2(), -i).ToVector3());
                            if (flags.HasFlag(CollisionFlags.Wall) || flags.HasFlag(CollisionFlags.Building))
                            {
                                Program.E.CastOnUnit(minion);
                                return;
                            }
                        }
                }
            }           
        }

        /*var target = Orbwalker.GetTarget();
            if (Program.MainMenu.GetMenuBool("Laneclear Settings", "EJungleMobs") && Program.E.IsReady())
            {               
                if (target != null && target.Type.Equals(GameObjectType.AIMinionClient) && target.IsValidTarget())
                {
                    var minion = (AIMinionClient)target;
                    if (_jungleMobs.Contains(minion.CharacterName))
                        for (var i = 40; i < 425; i += 141)
                        {
                            var flags = NavMesh.GetCollisionFlags(minion.Position.ToVector2()
                                .Extend(Heroes.Player.Position.ToVector2(), -i).ToVector3());
                            if (flags.HasFlag(CollisionFlags.Wall) || flags.HasFlag(CollisionFlags.Building))
                            {
                                Program.E.CastOnUnit(minion);
                                return;
                            }
                        }
                }
            }*/
    }
}