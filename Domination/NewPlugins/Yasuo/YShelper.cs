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
            E.SetSkillshot(0.3f, 175, 1000f, false, SpellType.Line);

            if (ObjectManager.Player.IsDashing())
            {
                Epred = aIBaseClient.Position;
            }
            else
            {
                Epred = E.GetPrediction(aIBaseClient).CastPosition;
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

            var Epred = target.Position;

            var E = new Spell(SpellSlot.E, 475);
            E.SetSkillshot(0.3f, 175, 1000f, false, SpellType.Line);

            if (ObjectManager.Player.IsDashing())
            {
                Epred = target.Position;
            }
            else
            {
                Epred = E.GetPrediction(target).CastPosition;
            }

            var pos = Epred;

            switch (MyYS.YasuoMenu.Ecombo.Yasuo_EMode.SelectedValue)
            {
                case "Target Pos":
                    pos = target.Position;
                    break;
                case "Cursor Pos":
                    pos = Game.CursorPos;
                    break;
                case "Logic Target Gapcloser":
                    pos = Epred;
                    break;
            }


            var obj = new List<AIBaseClient>();
            obj.AddRange(ObjectManager.Get<AIMinionClient>().Where(i => i.IsValidTarget(E.Range) && !i.IsDead && !i.IsAlly && CanE(i)));
            obj.AddRange(ObjectManager.Get<AIHeroClient>().Where(i => i.IsValidTarget(E.Range) && !i.IsDead && !i.IsAlly && CanE(i)));
            obj.AddRange(ObjectManager.Get<AIBaseClient>().Where(i => i.IsValidTarget(E.Range) && !i.IsDead && !i.IsAlly && CanE(i)));
            if (CanE(target) && E.IsReady() && (!(new Spell(SpellSlot.Q)).IsReady() || HaveQ2))
            {
                if (MyYS.YasuoMenu.Ecombo.Yasuo_Eziczac.Enabled)
                {
                    return
                obj.Where(
                    i =>
                    CanE(i)
                    && (pos.Distance(PosAfterE(i)) <= pos.DistanceToPlayer()
                        || ((new Spell(SpellSlot.Q)).IsReady() ? pos.Distance(PosAfterE(i)) <= MyYS.YasuoMenu.RangeCheck.EQrange.Value + 15
                        : pos.Distance(PosAfterE(i)) <= pos.DistanceToPlayer())
                        || pos.Distance(PosAfterE(i)) <= 410
                        )
                    )
                    .OrderByDescending(i => pos.Distance(PosAfterE(i))).FirstOrDefault();
                }
                else
                {
                    return
                obj.Where(
                    i =>
                    CanE(i)
                    && (pos.Distance(PosAfterE(i)) <= pos.DistanceToPlayer()
                        || ((new Spell(SpellSlot.Q)).IsReady() ? pos.Distance(PosAfterE(i)) <= MyYS.YasuoMenu.RangeCheck.EQrange.Value + 15
                        : pos.Distance(PosAfterE(i)) <= pos.DistanceToPlayer())
                        || pos.Distance(PosAfterE(i)) <= 410
                        )
                    )
                    .OrderBy(i => pos.Distance(PosAfterE(i))).FirstOrDefault();
                }
            }
            else
            {
                if (MyYS.YasuoMenu.Ecombo.Yasuo_Eziczac.Enabled)
                {
                    return
                    obj.Where(
                        i =>
                        CanE(i)
                        && (pos.Distance(PosAfterE(i)) <= pos.DistanceToPlayer()
                        || ((new Spell(SpellSlot.Q)).IsReady() ? pos.Distance(PosAfterE(i)) <= MyYS.YasuoMenu.RangeCheck.EQrange.Value + 15
                        : pos.Distance(PosAfterE(i)) <= pos.DistanceToPlayer()))
                        )
                        .OrderByDescending(i => pos.Distance(PosAfterE(i))).FirstOrDefault();
                }
                else
                {
                    return
                    obj.Where(
                        i =>
                        CanE(i)
                        && (pos.Distance(PosAfterE(i)) <= pos.DistanceToPlayer()
                        || ((new Spell(SpellSlot.Q)).IsReady() ? pos.Distance(PosAfterE(i)) <= 230
                        : pos.Distance(PosAfterE(i)) <= pos.DistanceToPlayer()))
                        )
                        .OrderBy(i => pos.Distance(PosAfterE(i))).FirstOrDefault();
                }
            }
        }
    }
}
