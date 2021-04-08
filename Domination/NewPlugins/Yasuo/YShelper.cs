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
        public static Vector3 Eprediction(AIBaseClient aIBaseClient = null)
        {
            if (aIBaseClient == null)
                return Vector3.Zero;

            var Epred = aIBaseClient.Position;

            var E = new Spell(SpellSlot.E, 475);
            E.SetSkillshot(0.3f, 175, 750f + 0.6f * ObjectManager.Player.MoveSpeed, false, SpellType.Line);

            if (ObjectManager.Player.IsDashing())
            {
                Epred = aIBaseClient.Position;
            }
            else
            {
                Epred = FSpred.Prediction.Prediction.GetPrediction(E, aIBaseClient).CastPosition;
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

        public static bool HaveQ2
        {
            get
            {
                return !ObjectManager.Player.IsDead && ObjectManager.Player.HasBuff("YasuoQ1");
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

            var pos = FSpred.Prediction.Prediction.GetPrediction(Et, target).CastPosition;

            switch (MyYS.YasuoMenu.Ecombo.Yasuo_EMode.SelectedValue)
            {
                case "Target Pos":
                    pos = target.Position;
                    break;
                case "Cursor Pos":
                    pos = Game.CursorPos;
                    break;
            }

            var obj = ObjectManager.Get<AIBaseClient>().Where(i => !i.IsAlly && !i.IsDead && i.DistanceToPlayer() <= 475 && CanE(i));

            if(obj == null)
            {
                return null;
            }
            switch (MyYS.YasuoMenu.Ecombo.Yasuo_ERange.ActiveValue)
            {
                case 1:
                    return


                obj.Where(
                    i => (
                        pos.Distance(PosAfterE(i)) <= pos.DistanceToPlayer() + 50
                         )
                         ||
                         (
                        pos.Distance(PosAfterE(i)) <= MyYS.YasuoMenu.RangeCheck.EQrange.Value + MyYS.YasuoMenu.EQCombo.EBonusRange.Value
                         )
                         && (MyYS.YasuoMenu.Yasuo_Keys.TurretKey.Active || !UnderTower(PosAfterE(i)))
                ).OrderByDescending(i => PosAfterE(i).Distance(pos)).ThenBy(i => i.Type == GameObjectType.AIMinionClient).FirstOrDefault();
                case 2:
                    return


                obj.Where(
                    i => (
                        pos.Distance(PosAfterE(i)) <= pos.DistanceToPlayer() + 50
                         )
                         ||
                         (
                        pos.Distance(PosAfterE(i)) <= MyYS.YasuoMenu.RangeCheck.EQrange.Value + MyYS.YasuoMenu.EQCombo.EBonusRange.Value
                         )
                         && (MyYS.YasuoMenu.Yasuo_Keys.TurretKey.Active || !UnderTower(PosAfterE(i)))
                ).OrderBy(i => PosAfterE(i).Distance(pos)).ThenBy(i => i.Type == GameObjectType.AIMinionClient).FirstOrDefault();
            }
            return


                obj.Where(
                    i => (
                        pos.Distance(PosAfterE(i)) <= pos.DistanceToPlayer() + 50
                         )
                         ||
                         (
                        pos.Distance(PosAfterE(i)) <= MyYS.YasuoMenu.RangeCheck.EQrange.Value + MyYS.YasuoMenu.EQCombo.EBonusRange.Value
                         )
                         && (MyYS.YasuoMenu.Yasuo_Keys.TurretKey.Active || !UnderTower(PosAfterE(i)))
                ).OrderByDescending(i => PosAfterE(i).Distance(pos)).ThenBy(i => i.Type == GameObjectType.AIMinionClient).FirstOrDefault();
        }
    }
}
