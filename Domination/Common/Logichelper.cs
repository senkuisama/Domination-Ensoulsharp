using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EnsoulSharp;
using EnsoulSharp.SDK;

using EnsoulSharp.SDK.Utility;
using SharpDX;
using System.Drawing;

namespace Yasuo_LogicHelper
{
    public class Logichelper
    {


        public static Spell Q, Q2, W, E, E1, R;
        public static Spell Eq3flash;
        public const int QRange = 475, Q2Range = 1000, QCirWidth = 275, QCirWidthMin = 250, RWidth = 400;
        public static bool beforeaa = false;
        public static bool afteraa = false;
        public static bool onaa = false;
        public static bool flashready = false;
        public static bool comboactive = false;
        public static int exploitpos;

        public static float Espeeds()
        {
            return 750f + 0.6f * ObjectManager.Player.MoveSpeed;
        }
        public static bool inQ13cc()
        {
            return ObjectManager.Player.GetDashInfo().EndPos.DistanceToPlayer() <= 50;
        }
        public static bool HaveQ2
        {
            get
            {
                return ObjectManager.Player.HasBuff("YasuoQ1");
            }
        }
        public static bool HaveQ3
        {
            get { return Q.Name == "YasuoQ3Wrapper"; }
        }
        public static float GetQDelay
        {
            get
            {
                return 0.4f * (1 - Math.Min((ObjectManager.Player.AttackSpeedMod - 1) * 0.58f, 0.66f));
            }
        }
        public static float GetQ2Delay
        {
            get
            {
                return 0.5f * (1 - Math.Min((ObjectManager.Player.AttackSpeedMod - 1) * 0.58f, 0.66f));
            }
        }
        public static float Qdelay
        {
            get
            {
                return Q.Delay;
            }
        }
        public static float Qspeed
        {
            get
            {
                return Q.Speed;
            }
        }
        public static Vector3 PosAfterE(AIBaseClient target)
        {
            if(target.DistanceToPlayer() > 410)
            {
                return target.Position.Extend(ObjectManager.Player.Position, -50);
            }
            else
            return ObjectManager.Player.Position.Extend(
                target.Position, 475);
        }

        public static bool UnderTower(Vector3 pos)
        {
            return
                ObjectManager.Get<AITurretClient>()
                    .Any(i => i.IsEnemy && !i.IsDead && (i.Distance(pos) < 850 + ObjectManager.Player.BoundingRadius));
        }
        public static float TimeLeftR(AIHeroClient target)
        {
            var buff = target.Buffs.FirstOrDefault(i => i.Type == BuffType.Knockback || i.Type == BuffType.Knockup);
            return buff != null ? buff.EndTime - Game.Time : -1;
        }
        public static bool UnderTowerR(Vector3 pos)
        {
            return
                ObjectManager.Get<AITurretClient>()
                    .Any(i => i.IsEnemy && !i.IsDead && i.Distance(pos) < 750);
        }
        public static bool CanCastR(AIHeroClient target)
        {
            return target.HasBuffOfType(BuffType.Knockup) || target.HasBuffOfType(BuffType.Knockback);
        }
        public static Vector3 PosExploit(AIBaseClient target)
        {
            return target.Position.Extend(ObjectManager.Player.Position, exploitpos);
        }
        public static List<AIMinionClient> GetEnemyLaneMinionsTargetsInRange(float range)
        {
            return GameObjects.EnemyMinions.Where(x => x.IsValidTarget(range) && x.IsMinion()).Cast<AIMinionClient>().ToList();
        }
        public static List<AIHeroClient> GetBestEnemyHeroesTargetsInRange(float range)
        {
            return TargetSelector.GetTargets(range, DamageType.Physical);
        }
        public static List<AIMinionClient> GetJungleTargetsInRange(float range)
        {
            return GameObjects.Jungle.Where(x => x.IsValidTarget(2000) && x.IsJungle()).Cast<AIMinionClient>().ToList();
        }
        public static Vector3 Epred(AIBaseClient target)
        {
            var getPrediction = E1.GetPrediction(target);
            return getPrediction.CastPosition;
        }
        public static IEnumerable<Vector3> FlashPoints()
        {
            var points = new List<Vector3>();

            for (var i = 1; i <= 360; i++)
            {
                var angle = i * 2 * Math.PI / 360;
                var point = new Vector3(ObjectManager.Player.Position.X + 425f * (float)Math.Cos(angle),
                    ObjectManager.Player.Position.Y + 425f * (float)Math.Sin(angle), ObjectManager.Player.Position.Z);

                points.Add(point);
            }

            return points;
        }
    }
}
