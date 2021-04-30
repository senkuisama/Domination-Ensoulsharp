using EnsoulSharp;
using EnsoulSharp.SDK;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominationAIO.NewPlugins.Yasuo
{
    public static class YasuoHelper
    {
        public static bool CheckDashingTick()
        {
            if (!ObjectManager.Player.IsDashing())
                return false;

            return ObjectManager.Player.GetDashInfo().EndTick - Variables.GameTimeTickCount > Yasuo.MyYS.YasuoMenu.Ecombo.DashingTick.Value;
        }
        public static Vector3 Eprediction(AIBaseClient aIBaseClient = null)
        {
            if (aIBaseClient == null)
                return Vector3.Zero;

            var Epred = aIBaseClient.Position;

            var E = new Spell(SpellSlot.E, 475);
            E.SetSkillshot(0.3f, 175, 750f + 0.6f * ObjectManager.Player.MoveSpeed, false, SpellType.Line);

            if (MyYS.CheckImDashing)
            {
                Epred = aIBaseClient.Position;
            }
            else
            {
                Epred = FSpred.Prediction.Prediction.GetPrediction(E, aIBaseClient).CastPosition;
            }

            return Epred;
        }

        public static Vector3 Eprediction(Spell thespell, AIBaseClient aIBaseClient = null)
        {
            if (aIBaseClient == null)
                return Vector3.Zero;

            var Epred = aIBaseClient.Position;

            if (MyYS.CheckImDashing)
            {
                Epred = aIBaseClient.Position;
            }
            else
            {
                Epred = FSpred.Prediction.Prediction.GetPrediction(thespell, aIBaseClient).CastPosition;
            }

            return Epred;
        }
        public static Vector3 PosExploit(this AIBaseClient target)
        {
            if (!MyYS.YasuoMenu.UseExploit.Enabled) return (ObjectManager.Player.Position);
            else
            {
                return (new Vector3(5000000, 5000000, 5000000));
            }
        }

        public static Vector3 PosAfterE(this AIBaseClient target)
        {
            if (target == null)
                return Vector3.Zero;

            if (target.DistanceToPlayer() > 410)
            {
                return target.Position.Extend(ObjectManager.Player.Position, -50);
            }
            return ObjectManager.Player.Position.Extend(
                target.Position, 475);
        }

        public static bool UnderTower(this Vector3 pos)
        {
            if (pos == Vector3.Zero)
                return false;
            return
                ObjectManager.Get<AITurretClient>()
                    .Any(i => i.IsEnemy && !i.IsDead && (i.Distance(pos) < 850 + ObjectManager.Player.BoundingRadius)) || GameObjects.EnemySpawnPoints.Any(i => i.Position.Distance(pos) < 850 + ObjectManager.Player.BoundingRadius);
        }

        public static bool CanE(this AIBaseClient t)
        {
            if (t == null)
                return false;

            return !t.HasBuff("YasuoE");
        }
        public static bool HaveQ1
        {
            get
            {
                return !ObjectManager.Player.IsDead && (new Spell(SpellSlot.Q)).Name == "YasuoQ1Wrapper";
            }
        }
        public static bool HaveQ2
        {
            get
            {
                return !ObjectManager.Player.IsDead && (new Spell(SpellSlot.Q)).Name == "YasuoQ2Wrapper";
            }
        }
        public static bool HaveQ3
        {
            get
            {
                return !ObjectManager.Player.IsDead && (new Spell(SpellSlot.Q)).Name == "YasuoQ3Wrapper";
            }
        }

        public static AIBaseClient GetNearObj(this AIBaseClient target)
        {
            if (target == null)
                return null;

            var Et = new Spell(SpellSlot.E, 475);
            Et.SetSkillshot(0.3f, 175, (float)(750f + 0.6f * ObjectManager.Player.MoveSpeed), false, SpellType.Line);

            Vector3 pos = Eprediction(Et, target);

            switch (MyYS.YasuoMenu.Ecombo.Yasuo_EMode.SelectedValue)
            {
                case "Target Pos":
                    pos = target.Position;
                    break;
                case "Cursor Pos":
                    pos = Game.CursorPos;
                    break;
            }

            var obj = ObjectManager.Get<AIBaseClient>().Where(i => !i.IsAlly && !i.IsDead && i.IsValidTarget(475) && i.DistanceToPlayer() <= 475 && CanE(i));

            if(obj == null)
            {
                return null;
            }

            var gotobj = obj.Where(i => (pos.Distance(PosAfterE(i)) <= pos.DistanceToPlayer())
            || (pos.DistanceSquared(PosAfterE(i)) <= Math.Pow(MyYS.YasuoMenu.RangeCheck.EQrange.Value + MyYS.YasuoMenu.EQCombo.EBonusRange.Value, 2))
            && (MyYS.YasuoMenu.Yasuo_Keys.TurretKey.Active || !UnderTower(PosAfterE(i))));

            var getobj = gotobj.OrderBy(i => PosAfterE(i).Distance(pos))
                .ThenBy(i => i.Type == GameObjectType.AIMinionClient)
                .FirstOrDefault();

            switch (MyYS.YasuoMenu.Ecombo.Yasuo_ERange.ActiveValue)
            {
                case 1:
                getobj = gotobj.OrderByDescending(i => PosAfterE(i).Distance(pos))
                        .ThenBy(i => i.Type == GameObjectType.AIMinionClient)
                        .FirstOrDefault();
                    break;
                case 2:
               getobj = gotobj.OrderBy(i => PosAfterE(i).Distance(pos))
                        .ThenBy(i => i.Type == GameObjectType.AIMinionClient)
                        .FirstOrDefault();
                    break;
                default:
                    goto case 2;
            }
            return getobj;
        }
    }
}
