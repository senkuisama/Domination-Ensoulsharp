using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsoulSharp;
using EnsoulSharp.SDK;
using SharpDX;

namespace SebbyLibPorted
{
    public class HealthPrediction
    {
        /// <summary>
        /// The active attacks
        /// </summary>
        private static readonly Dictionary<uint, PredictedDamage> ActiveAttacks = new Dictionary<uint, PredictedDamage>();

        /// <summary>
        /// Initializes static members of the <see cref="HealthPrediction"/> class. 
        /// </summary>
        static HealthPrediction()
        {
            AIBaseClient.OnProcessSpellCast += ObjAiBaseOnOnProcessSpellCast;
            Game.OnUpdate += Game_OnGameUpdate;
            Spellbook.OnStopCast += SpellbookOnStopCast;
            MissileClient.OnDelete += MissileClient_OnDelete;
            AIBaseClient.OnDoCast += AIBaseClient_OnDoCast;
        }

        /// <summary>
        /// Fired when a unit does an auto attack.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="AIBaseClientProcessSpellCastEventArgs"/> instance containing the event data.</param>
        private static void AIBaseClient_OnDoCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (ActiveAttacks.ContainsKey((uint)sender.NetworkId) && sender.IsMelee)
            {
                ActiveAttacks[(uint)sender.NetworkId].Processed = true;
            }
        }

        static void MissileClient_OnDelete(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;
            if (missile != null && missile.SpellCaster != null)
            {
                var casterNetworkId = missile.SpellCaster.NetworkId;
                foreach (var activeAttack in ActiveAttacks)
                {
                    if (activeAttack.Key == casterNetworkId)
                    {
                        ActiveAttacks[(uint)casterNetworkId].Processed = true;
                    }
                }
            }
        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            ActiveAttacks.ToList()
                .Where(pair => pair.Value.StartTick < Variables.GameTimeTickCount - 3000)
                .ToList()
                .ForEach(pair => ActiveAttacks.Remove(pair.Key));
        }

        /// <summary>
        /// Fired when the spellbooks stops a cast.
        /// </summary>
        /// <param name="spellbook">The spellbook.</param>
        /// <param name="args">The <see cref="SpellbookStopCastEventArgs"/> instance containing the event data.</param>
        private static void SpellbookOnStopCast(Spellbook spellbook, SpellbookStopCastEventArgs args)
        {
            if (spellbook.Owner.IsValid && args.SpellStopCancelled)
            {
                if (ActiveAttacks.ContainsKey((uint)spellbook.Owner.NetworkId))
                {
                    ActiveAttacks.Remove((uint)spellbook.Owner.NetworkId);
                }
            }
        }

        /// <summary>
        /// Fired when the game processes a spell cast.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="args">The <see cref="AIBaseClientProcessSpellCastEventArgs"/> instance containing the event data.</param>
        private static void ObjAiBaseOnOnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if ( sender.Team != ObjectManager.Player.Team || !sender.IsValidTarget(3000, false) || !Orbwalking.IsAutoAttack(args.SData.Name) || !(args.Target is AIBaseClient))
            {
                return;
            }

            var target = (AIBaseClient)args.Target;
            ActiveAttacks.Remove((uint)sender.NetworkId);

            var attackData = new PredictedDamage(
                sender,
                target,
                sender.Position,
                Variables.GameTimeTickCount - Game.Ping / 2,
                sender.AttackCastDelay * 1000,
                sender.AttackDelay * 1000 - (sender is AITurretClient ? 70 : 0),
                sender.IsMelee ? int.MaxValue : (int)args.SData.MissileSpeed,
                (float)sender.GetAutoAttackDamage(target));
            ActiveAttacks.Add((uint)sender.NetworkId, attackData);
        }

        public static float GetHealthPrediction(AIBaseClient unit, int time, int delay = 70)
        {
            var predictedDamage = 0f;

            foreach (var attack in ActiveAttacks.Values)
            {
                var attackDamage = 0f;
                if (!attack.Processed &&
                    attack.Target.IsValidTarget(float.MaxValue, false) && attack.Target.NetworkId == unit.NetworkId)
                {

                    float bonding = Math.Max(attack.Target.BoundingRadius, unit.Distance(attack.StartPos) - attack.Source.BoundingRadius);
                    if(attack.Source.IsMelee )
                    {
                        bonding = 0;
                    }

                    var landTime = attack.StartTick + attack.Delay + 1000 *  bonding / attack.ProjectileSpeed + delay;

                    if (/*Variables.GameTimeTickCount < landTime - delay &&*/ landTime < Variables.GameTimeTickCount + time)
                    {
                        attackDamage = attack.Damage;
                    }
                }

                predictedDamage += attackDamage;
            }

            return unit.Health - predictedDamage;
        }

        public static float LaneClearHealthPrediction(AIBaseClient unit, int time, int delay = 70)
        {
            var predictedDamage = 0f;

            foreach (var attack in ActiveAttacks.Values)
            {
                var n = 0;
                if (Variables.GameTimeTickCount - 100 <= attack.StartTick + attack.AnimationTime &&
                    attack.Target.IsValidTarget(float.MaxValue, false) &&
                    attack.Source.IsValidTarget(float.MaxValue, false) && attack.Target.NetworkId == unit.NetworkId)
                {
                    var fromT = attack.StartTick;
                    var toT = Variables.GameTimeTickCount + time;

                    while (fromT < toT)
                    {
                        if (fromT >= Variables.GameTimeTickCount &&
                            (fromT + attack.Delay + Math.Max(0, unit.Distance(attack.Source) - attack.Source.BoundingRadius / 2) / attack.ProjectileSpeed < toT))
                        {
                            n++;
                        }
                        fromT += (int)attack.AnimationTime;
                    }
                }
                predictedDamage += n * attack.Damage;
            }

            return unit.Health - predictedDamage;
        }

        /// <summary>
        /// Determines whether the specified minion has minion aggro.
        /// </summary>
        /// <param name="minion">The minion.</param>
        /// <returns></returns>
        public static bool HasMinionAggro(AIMinionClient minion)
        {
            return ActiveAttacks.Values.Any(m => (m.Source is AIMinionClient) && m.Target.NetworkId == minion.NetworkId);
        }
        /// <summary>
        /// Determines whether the specified minion has turret aggro.
        /// </summary>
        /// <param name="minion">The minion</param>
        /// <returns></returns>
        public static bool HasTurretAggro(AIMinionClient minion)
        {
            return ActiveAttacks.Values.Any(m => (m.Source is AITurretClient) && m.Target.NetworkId == minion.NetworkId);
        }
        /// <summary>
        /// Return the starttick of the attacking turret.
        /// </summary>
        /// <param name="minion"></param>
        /// <returns></returns>
        public static int TurretAggroStartTick(AIMinionClient minion)
        {
            var ActiveTurret = ActiveAttacks.Values
                .FirstOrDefault(m => (m.Source is AITurretClient) && m.Target.NetworkId == minion.NetworkId);
            return ActiveTurret != null ? ActiveTurret.StartTick : 0;
        }
        /// <summary>
        /// Return the Attacking turret.
        /// </summary>
        /// <param name="minion"></param>
        /// <returns></returns>
        public static AIBaseClient GetAggroTurret(AIMinionClient minion)
        {
            var ActiveTurret = ActiveAttacks.Values
                .FirstOrDefault(m => (m.Source is AITurretClient) && m.Target.NetworkId == minion.NetworkId);
            return ActiveTurret != null ? ActiveTurret.Source : null;
        }
        /// <summary>
        /// Represetns predicted damage.
        /// </summary>
        private class PredictedDamage
        {
            /// <summary>
            /// The animation time
            /// </summary>
            public readonly float AnimationTime;

            /// <summary>
            /// Gets or sets the damage.
            /// </summary>
            /// <value>
            /// The damage.
            /// </value>
            public float Damage { get; private set; }

            /// <summary>
            /// Gets or sets the delay.
            /// </summary>
            /// <value>
            /// The delay.
            /// </value>
            public float Delay { get; private set; }

            /// <summary>
            /// Gets or sets the projectile speed.
            /// </summary>
            /// <value>
            /// The projectile speed.
            /// </value>
            public int ProjectileSpeed { get; private set; }

            /// <summary>
            /// Gets or sets the source.
            /// </summary>
            /// <value>
            /// The source.
            /// </value>
            public AIBaseClient Source { get; private set; }

            /// <summary>
            /// Gets or sets the start tick.
            /// </summary>
            /// <value>
            /// The start tick.
            /// </value>
            /// 
            public Vector3 StartPos { get; private set; }

            /// <summary>
            /// Gets or sets the start tick.
            /// </summary>
            /// <value>
            /// The start tick.
            /// </value>
            /// 
            public int StartTick { get; internal set; }

            /// <summary>
            /// Gets or sets the target.
            /// </summary>
            /// <value>
            /// The target.
            /// </value>
            public AIBaseClient Target { get; private set; }

            /// <summary>
            /// Gets or sets a value indicating whether this <see cref="PredictedDamage"/> is processed.
            /// </summary>
            /// <value>
            ///   <c>true</c> if processed; otherwise, <c>false</c>.
            /// </value>
            public bool Processed { get; internal set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="PredictedDamage"/> class.
            /// </summary>
            /// <param name="source">The source.</param>
            /// <param name="target">The target.</param>
            /// <param name="startTick">The start tick.</param>
            /// <param name="delay">The delay.</param>
            /// <param name="animationTime">The animation time.</param>
            /// <param name="projectileSpeed">The projectile speed.</param>
            /// <param name="damage">The damage.</param>
            public PredictedDamage(AIBaseClient source,
                AIBaseClient target,
                Vector3 startPos,
                int startTick,
                float delay,
                float animationTime,
                int projectileSpeed,
                float damage)
            {
                Source = source;
                StartPos = startPos;
                Target = target;
                StartTick = startTick;
                Delay = delay;
                ProjectileSpeed = projectileSpeed;
                Damage = damage;
                AnimationTime = animationTime;
            }
        }
    }
}