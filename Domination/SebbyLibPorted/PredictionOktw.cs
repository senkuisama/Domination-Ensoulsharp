using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;
using EnsoulSharp;
using EnsoulSharp.SDK;

namespace SebbyLibPorted.Prediction
{
    public enum HitChance
    {
        Immobile = 5,
        Dashing = 6,
        VeryHigh = 4,
        High = 3,
        Medium = 2,
        Low = 1,
        Impossible = -2,
        OutOfRange = 0,
        Collision = -1
    }

    public enum SkillshotType
    {
        SkillshotLine,
        SkillshotCircle,
        SkillshotCone
    }

    public enum CollisionableObjects
    {
        Minions,
        Heroes,
        YasuoWall,
        Walls
    }

    public class PredictionInput
    {

        private Vector3 _from;
        private Vector3 _rangeCheckFrom;

        /// <summary>
        ///     Set to true make the prediction hit as many enemy heroes as posible.
        /// </summary>
        public bool Aoe = false;

        /// <summary>
        ///     Set to true if the unit collides with units.
        /// </summary>
        public bool Collision = false;

        /// <summary>
        ///     Array that contains the unit types that the skillshot can collide with.
        /// </summary>
        public CollisionableObjects[] CollisionObjects =
        {
            CollisionableObjects.Minions, CollisionableObjects.YasuoWall
        };

        /// <summary>
        ///     The skillshot delay in seconds.
        /// </summary>
        public float Delay;

        /// <summary>
        ///     The skillshot width's radius or the angle in case of the cone skillshots.
        /// </summary>
        public float Radius = 1f;

        /// <summary>
        ///     The skillshot range in units.
        /// </summary>
        public float Range = float.MaxValue;

        /// <summary>
        ///     The skillshot speed in units per second.
        /// </summary>
        public float Speed = float.MaxValue;

        /// <summary>
        ///     The skillshot type.
        /// </summary>
        public SkillshotType Type = SkillshotType.SkillshotLine;

        /// <summary>
        ///     The unit that the prediction will made for.
        /// </summary>
        public AIBaseClient Unit = ObjectManager.Player;

        /// <summary>
        ///     Source unit for the prediction 
        /// </summary>
        public AIBaseClient Source = ObjectManager.Player;

        /// <summary>
        ///     Set to true to increase the prediction radius by the unit bounding radius.
        /// </summary>
        public bool UseBoundingRadius = true;

        /// <summary>
        ///     The position from where the skillshot missile gets fired.
        /// </summary>
        public Vector3 From
        {
            get { return _from.ToVector2().IsValid() ? _from : ObjectManager.Player.Position; }
            set { _from = value; }
        }

        /// <summary>
        ///     The position from where the range is checked.
        /// </summary>
        public Vector3 RangeCheckFrom
        {
            get
            {
                return _rangeCheckFrom.ToVector2().IsValid()
                    ? _rangeCheckFrom
                    : (From.ToVector2().IsValid() ? From : ObjectManager.Player.Position);
            }
            set { _rangeCheckFrom = value; }
        }

        internal float RealRadius
        {
            get { return UseBoundingRadius ? Radius + Unit.BoundingRadius : Radius; }
        }
    }

    public class PredictionOutput
    {
        internal int _aoeTargetsHitCount;
        private Vector3 _castPosition;
        private Vector3 _unitPosition;

        /// <summary>
        ///     The list of the targets that the spell will hit (only if aoe was enabled).
        /// </summary>
        public List<AIHeroClient> AoeTargetsHit = new List<AIHeroClient>();

        /// <summary>
        ///     The list of the units that the skillshot will collide with.
        /// </summary>
        public List<AIBaseClient> CollisionObjects = new List<AIBaseClient>();

        /// <summary>
        ///     Returns the hitchance.
        /// </summary>
        public HitChance Hitchance = HitChance.Impossible;

        internal PredictionInput Input;

        /// <summary>
        ///     The position where the skillshot should be casted to increase the accuracy.
        /// </summary>
        public Vector3 CastPosition
        {
            get
            {
                return _castPosition.IsValid() && _castPosition.ToVector2().IsValid()
                    ? _castPosition.SetZ()
                    : Input.Unit.Position;
            }
            set { _castPosition = value; }
        }

        /// <summary>
        ///     The number of targets the skillshot will hit (only if aoe was enabled).
        /// </summary>
        public int AoeTargetsHitCount
        {
            get { return Math.Max(_aoeTargetsHitCount, AoeTargetsHit.Count); }
        }

        /// <summary>
        ///     The position where the unit is going to be when the skillshot reaches his position.
        /// </summary>
        public Vector3 UnitPosition
        {
            get { return _unitPosition.ToVector2().IsValid() ? _unitPosition.SetZ() : Input.Unit.Position; }
            set { _unitPosition = value; }
        }
    }

    /// <summary>
    ///     Class used for calculating the position of the given unit after a delay.
    /// </summary>
    public static class Prediction
    {
        public static PredictionOutput GetPrediction(AIBaseClient unit, float delay)
        {
            return GetPrediction(new PredictionInput { Unit = unit, Delay = delay });
        }

        public static PredictionOutput GetPrediction(AIBaseClient unit, float delay, float radius)
        {
            return GetPrediction(new PredictionInput { Unit = unit, Delay = delay, Radius = radius });
        }

        public static PredictionOutput GetPrediction(AIBaseClient unit, float delay, float radius, float speed)
        {
            return GetPrediction(new PredictionInput { Unit = unit, Delay = delay, Radius = radius, Speed = speed });
        }

        public static PredictionOutput GetPrediction(AIBaseClient unit,
            float delay,
            float radius,
            float speed,
            CollisionableObjects[] collisionable)
        {
            return
                GetPrediction(
                    new PredictionInput
                    {
                        Unit = unit,
                        Delay = delay,
                        Radius = radius,
                        Speed = speed,
                        CollisionObjects = collisionable
                    });
        }
        public static PredictionOutput GetPrediction(this Spell spell,
            AIBaseClient unit,
            bool Aoe = false,
            float overrideRange = -1f,
            CollisionableObjects[] CollisionObjects = null
            )
        {
            var SpellInput = new PredictionInput
            {
                Aoe = ((spell.Type == SpellType.Circle || Aoe) ? true : false),
                Unit = unit,
                Collision = spell.Collision,
                CollisionObjects = (CollisionObjects ?? new CollisionableObjects[1]),
                Delay = spell.Delay,
                Radius = spell.Width,
                Range = ((overrideRange > 150f) ? overrideRange : spell.Range),
                Speed = spell.Speed,
                Type = (SkillshotType)spell.Type,
                RangeCheckFrom = spell.RangeCheckFrom,
                From = spell.From
            };

            if(SpellInput.Aoe == true)
            {
                return AoePrediction.GetPrediction(SpellInput);
            }
            else
            {
                return GetPrediction(SpellInput);
            }
        }

        public static PredictionOutput GetPrediction(PredictionInput input)
        {
            return GetPrediction(input, true, true);
        }

        internal static PredictionOutput GetPrediction(PredictionInput input, bool ft, bool checkCollision)
        {
            PredictionOutput result = null;

            if (!input.Unit.IsValidTarget(float.MaxValue, false))
            {
                return new PredictionOutput();
            }

            if (ft)
            {
                //Increase the delay due to the latency and server tick:
                input.Delay += Game.Ping / 1000f + 0.075f;

                /*if (input.Aoe)
                {
                    return AoePrediction.GetPrediction(input);
                }*/
            }

            //Target too far away.
            /*if (Math.Abs(input.Range - float.MaxValue) > float.Epsilon &&
                input.Unit.DistanceSquared(input.RangeCheckFrom) > Math.Pow(input.Range * 1.5, 2))
            {
                return new PredictionOutput { Input = input };
            }*/
            if(float.MaxValue - input.Radius > float.Epsilon && input.RangeCheckFrom.Distance(input.Unit.Position) > input.Range * 3f)
            {
                return new PredictionOutput { Input = input };
            }

            //Unit is dashing.
            if (input.Unit.IsDashing())
            {
                result = GetDashingPrediction(input);
            }
            else
            {
                //Unit is immobile.
                if (!input.Unit.CanMove)
                {
                    var remainingImmobileT = UnitIsImmobileUntil(input.Unit);
                    if (remainingImmobileT >= 0d)
                    {
                        result = GetImmobilePrediction(input, remainingImmobileT);
                    }
                }
            }

            //Normal prediction
            if (result == null)
            {
                result = GetPositionOnPath(input, input.Unit.GetWaypoints(), input.Unit.MoveSpeed);
            }

            if (input.Unit is AIHeroClient && input.Radius > 1 && result.Hitchance <= HitChance.VeryHigh)
            {
                var moveOutWall = input.Unit.BoundingRadius + input.Radius / 2 + 10;
                if (input.Type == SkillshotType.SkillshotCircle)
                    moveOutWall = input.Unit.BoundingRadius;

                var wallPoint = GetWallPoint(result.CastPosition, moveOutWall);
                if (!wallPoint.IsZero)
                {
                    result.Hitchance = HitChance.High;
                    result.CastPosition = wallPoint.Extend(result.CastPosition, moveOutWall);
                }
            }

            //Check if the unit position is in range
            if (Math.Abs(input.Range - float.MaxValue) > float.Epsilon)
            {
                if (result.Hitchance >= HitChance.High &&
                    input.RangeCheckFrom.DistanceSquared(input.Unit.Position) >
                    Math.Pow(input.Range + input.RealRadius * 3 / 4, 2))
                {
                    result.Hitchance = HitChance.Medium;
                }

                if (input.RangeCheckFrom.DistanceSquared(result.UnitPosition) >
                    Math.Pow(input.Range + (input.Type == SkillshotType.SkillshotCircle ? input.RealRadius : 0), 2))
                {
                    result.Hitchance = HitChance.OutOfRange;
                }

                /* This does not need to be handled for the updated predictions, but left as a reference.*/
                if (input.RangeCheckFrom.DistanceSquared(result.CastPosition) > Math.Pow(input.Range, 2))
                {
                    if (result.Hitchance != HitChance.OutOfRange)
                    {
                        result.CastPosition = input.RangeCheckFrom +
                                              input.Range *
                                              (result.UnitPosition - input.RangeCheckFrom).ToVector2().Normalized().ToVector3();
                    }
                    else
                    {
                        result.Hitchance = HitChance.OutOfRange;
                    }
                }
            }

            //Set hit chance
            if (result.Hitchance == HitChance.High)
            {
                result = WayPointAnalysis(result, input);
            }

            //Check for collision
            if (checkCollision && input.Collision && result.Hitchance > HitChance.Impossible)
            {
                var positions = new List<Vector3> { result.CastPosition, result.UnitPosition };
                var originalUnit = input.Unit;
                if (Collision.GetCollision(positions, input))
                    result.Hitchance = HitChance.Collision;
            }

            return result;
        }

        public static bool PointInLineSegment(Vector2 segmentStart, Vector2 segmentEnd, Vector2 point)
        {
            var distanceStartEnd = segmentStart.DistanceSquared(segmentEnd);
            var distanceStartPoint = segmentStart.DistanceSquared(point);
            var distanceEndPoint = segmentEnd.DistanceSquared(point);
            return !(distanceEndPoint > distanceStartEnd || distanceStartPoint > distanceStartEnd);
        }

        internal static PredictionOutput WayPointAnalysis(PredictionOutput result, PredictionInput input)
        {
            if (!(input.Unit is AIHeroClient) || input.Radius == 1)
            {
                result.Hitchance = HitChance.VeryHigh;
                return result;
            }
            // CAN'T MOVE SPELLS ///////////////////////////////////////////////////////////////////////////////////

            if (//UnitTracker.GetSpecialSpellEndTime(input.Unit) > 100 || 
                input.Unit.HasBuff("Recall") || (UnitTracker.GetLastStopMoveTime(input.Unit) < 100))
            {
                //OktwCommon.debug("CAN'T MOVE SPELLS");
                result.Hitchance = HitChance.VeryHigh;
                result.CastPosition = input.Unit.Position;
                return result;
            }

            // LOW HP DETECTION ///////////////////////////////////////////////////////////////////////////////////

            if (input.Unit.HealthPercent < 20 || ObjectManager.Player.HealthPercent < 20)
            {
                result.Hitchance = HitChance.VeryHigh;
                //OktwCommon.debug("PRED VH: LOW HP");
                return result;
            }

            // NEW VISABLE ///////////////////////////////////////////////////////////////////////////////////

            if (UnitTracker.GetLastVisableTime(input.Unit) < 100)
            {
                //OktwCommon.debug("PRED M: NEW VISABLE");
                result.Hitchance = HitChance.Medium;
                return result;
            }

            // PREPARE MATH ///////////////////////////////////////////////////////////////////////////////////
            var path = input.Unit.GetWaypoints();


            var lastWaypiont = path.Last().ToVector3();

            var distanceUnitToWaypoint = lastWaypiont.Distance(input.Unit.Position);
            var distanceFromToUnit = input.From.Distance(input.Unit.Position);
            var distanceFromToWaypoint = lastWaypiont.Distance(input.From);



            float speedDelay = distanceFromToUnit / input.Speed;

            if (Math.Abs(input.Speed - float.MaxValue) < float.Epsilon)
                speedDelay = 0;

            float totalDelay = speedDelay + input.Delay;
            float moveArea = input.Unit.MoveSpeed * totalDelay;
            float fixRange = moveArea * 0.27f;
            float pathMinLen = 800;

            if (input.Type == SkillshotType.SkillshotCircle)
            {
                fixRange -= input.Radius / 1.5f;
            }

            // FIX RANGE ///////////////////////////////////////////////////////////////////////////////////
            if (distanceFromToWaypoint <= distanceFromToUnit && distanceFromToUnit > input.Range - fixRange)
            {
                result.Hitchance = HitChance.Medium;
                return result;
            }

            // SPAM POSITION ///////////////////////////////////////////////////////////////////////////////////

            if (UnitTracker.SpamSamePlace(input.Unit))
            {
                result.Hitchance = HitChance.VeryHigh;
                return result;
            }

            // SHORT CLICK DETECTION ///////////////////////////////////////////////////////////////////////////////////

            if (distanceUnitToWaypoint > 0 && distanceUnitToWaypoint < 50 + input.Radius)
            {
                //OktwCommon.debug("PRED M: SHORT CLICK DETECTION");
                result.Hitchance = HitChance.Medium;
                return result;
            }

            if (input.Unit.IsWindingUp)
            {
                if (totalDelay > 0.6)
                {
                    //OktwCommon.debug("PRED M: winding block");
                    result.Hitchance = HitChance.Medium;
                }
                else
                {
                    //OktwCommon.debug("PRED VH: winding accept");
                    result.Hitchance = HitChance.VeryHigh;
                }
                return result;
            }
            else if (!input.Unit.CanMove)
            {
                //OktwCommon.debug("PRED VH: After CC detection " + totalDelay);
                result.Hitchance = HitChance.VeryHigh;
                return result;
            }

            if (distanceUnitToWaypoint > 0)
            {
                Vector2 pos1 = lastWaypiont.ToVector2() - input.Unit.Position.ToVector2();
                Vector2 pos2 = input.From.ToVector2() - input.Unit.Position.ToVector2();
                var getAngle = pos1.AngleBetween(pos2);

                // RUN IN LANE DETECTION /////////////////////////////////////////////////////////////////////////////////// 
                if (getAngle < 20 || getAngle > 160 || ((getAngle > 150 || getAngle < 30) && SebbyLibPorted.OktwCommon.IsMovingInSameDirection(ObjectManager.Player, input.Unit)))
                {
                    //OktwCommon.debug("PRED VH: ANGLE " + getAngle);
                    result.Hitchance = HitChance.VeryHigh;
                    return result;
                }

                // WALL LOGIC  ///////////////////////////////////////////////////////////////////////////////////

                var points = OktwCommon.CirclePoints(10, 200, input.Unit.Position).Where(x => x.IsWall());

                if (points.Count() > 2)
                {
                    var runOutWall = true;
                    foreach (var point in points)
                    {
                        if (input.Unit.Position.Distance(point) > lastWaypiont.Distance(point))
                        {
                            runOutWall = false;
                        }
                    }
                    if (runOutWall)
                    {
                        OktwCommon.debug("PRED VH: RUN OUT WALL");
                        result.Hitchance = HitChance.VeryHigh;
                        return result;
                    }
                }
                else if (UnitTracker.GetLastNewPathTime(input.Unit) > 250 && input.Delay < 0.3)
                {
                    // LONG TIME ///////////////////////////////////////////////////////////////////////////////////
                    OktwCommon.debug("PRED VH: LONG TIME");
                    result.Hitchance = HitChance.VeryHigh;
                    return result;
                }
            }



            if (input.Unit.GetWaypoints().Count == 1)
            {
                if (UnitTracker.GetLastAutoAttackTime(input.Unit) < 0.1d && totalDelay < 0.7)
                {
                    OktwCommon.debug("PRED VH: AA try");
                    result.Hitchance = HitChance.VeryHigh;
                    return result;
                }
                if (input.Unit.IsWindingUp)
                {
                    result.Hitchance = HitChance.High;
                    return result;
                }
                else if (UnitTracker.GetLastStopMoveTime(input.Unit) < 800)
                {
                    //OktwCommon.debug("PRED: STOP HIGH");
                    result.Hitchance = HitChance.High;
                    return result;
                }
                else
                {
                    OktwCommon.debug("PRED VH: STOP LOGIC");
                    result.Hitchance = HitChance.VeryHigh;
                    return result;
                }
            }



            // SPECIAL CASES ///////////////////////////////////////////////////////////////////////////////////

            if (distanceFromToUnit < 250)
            {
                OktwCommon.debug("PRED VH: SPECIAL CASES NEAR");
                result.Hitchance = HitChance.VeryHigh;
                return result;
            }
            else if (input.Unit.MoveSpeed < 250)
            {
                OktwCommon.debug("PRED VH: SPECIAL CASES SLOW");
                result.Hitchance = HitChance.VeryHigh;
                return result;
            }
            else if (distanceFromToWaypoint < 250)
            {
                OktwCommon.debug("PRED VH: SPECIAL CASES ON WAY");
                result.Hitchance = HitChance.VeryHigh;
                return result;
            }

            // LONG CLICK DETECTION ///////////////////////////////////////////////////////////////////////////////////

            if (distanceUnitToWaypoint > pathMinLen)
            {
                OktwCommon.debug("PRED VH: LONG CLICK DETECTION");
                result.Hitchance = HitChance.VeryHigh;
                return result;
            }

            // NEW PATH ///////////////////////////////////////////////////////////////////////////////////

            if (UnitTracker.GetLastNewPathTime(input.Unit) < 80 && totalDelay < 0.6)
            {
                OktwCommon.debug("PRED VH: NEW PATH");
                result.Hitchance = HitChance.VeryHigh;
            }

            //Program.debug("PRED: NO DETECTION");
            return result;
        }

        internal static PredictionOutput GetDashingPrediction(PredictionInput input)
        {
            var dashData = input.Unit.GetDashInfo();
            var result = new PredictionOutput { Input = input };
            //Normal dashes.
            if (dashData != null && !(dashData.Speed > 10000))
            {
                //Mid air:
                var endP = dashData.Path.Last();
                var dashPred = GetPositionOnPath(
                    input, new List<Vector2> { input.Unit.Position.ToVector2(), endP }, dashData.Speed);
                if (dashPred.Hitchance >= HitChance.High && dashPred.UnitPosition.ToVector2().Distance(input.Unit.Position.ToVector2(), endP, true) < 200)
                {
                    dashPred.CastPosition = dashPred.UnitPosition;
                    dashPred.Hitchance = HitChance.Dashing;
                    return dashPred;
                }

                //At the end of the dash:
                if (dashData.Path.PathLength() > 200)
                {
                    var timeToPoint = input.Delay / 2f + input.From.ToVector2().Distance(endP) / input.Speed - 0.25f;
                    if (timeToPoint <=
                        input.Unit.Distance(endP) / dashData.Speed + input.RealRadius / input.Unit.MoveSpeed)
                    {
                        return new PredictionOutput
                        {
                            CastPosition = endP.ToVector3(),
                            UnitPosition = endP.ToVector3(),
                            Hitchance = HitChance.Dashing
                        };
                    }
                }
                result.CastPosition = dashData.Path.Last().ToVector3();
                result.UnitPosition = result.CastPosition;

                //Figure out where the unit is going.
            }

            return result;
        }

        internal static PredictionOutput GetImmobilePrediction(PredictionInput input, double remainingImmobileT)
        {
            var timeToReachTargetPosition = input.Delay + input.Unit.Distance(input.From) / input.Speed;

            if (timeToReachTargetPosition <= remainingImmobileT + input.RealRadius / input.Unit.MoveSpeed)
            {
                return new PredictionOutput
                {
                    CastPosition = input.Unit.Position,
                    UnitPosition = input.Unit.Position,
                    Hitchance = HitChance.Immobile
                };
            }

            return new PredictionOutput
            {
                Input = input,
                CastPosition = input.Unit.Position,
                UnitPosition = input.Unit.Position,
                Hitchance = HitChance.High
                /*timeToReachTargetPosition - remainingImmobileT + input.RealRadius / input.Unit.MoveSpeed < 0.4d ? HitChance.High : HitChance.Medium*/
            };
        }

        internal static Vector3 GetWallPoint(Vector3 from, float range)
        {
            var count = 30;
            var points = OktwCommon.CirclePoints(count, range, from);
            Vector3 first = Vector3.Zero, last = Vector3.Zero;

            for (int i = 0; i < count; i++)
            {
                if (points[i].IsWall())
                {
                    if (first.IsZero)
                    {
                        if (i == count - 1)
                        {
                            if (!points[0].IsWall())
                                first = points[i];
                        }
                        else
                        {
                            if (!points[i + 1].IsWall())
                                first = points[i];
                        }
                    }
                    if (last.IsZero)
                    {
                        if (i == 0)
                        {
                            if (!points[count - 1].IsWall())
                                last = points[i];
                        }
                        else
                        {
                            if (!points[i - 1].IsWall())
                                last = points[i];
                        }
                    }
                }
            }
            if (!first.IsZero && !last.IsZero)
            {
                var finnaly = new Vector3((last.X + first.X) / 2, (last.Y + first.Y) / 2, (last.Z + first.Z) / 2);
                return finnaly;
            }
            else
                return Vector3.Zero;
        }

        internal static double UnitIsImmobileUntil(AIBaseClient unit)
        {
            var result =
                unit.Buffs.Where(
                    buff =>
                        buff.IsActive && Game.Time <= buff.EndTime &&
                        (buff.Type == BuffType.Charm || buff.Type == BuffType.Knockup || buff.Type == BuffType.Stun ||
                         buff.Type == BuffType.Suppression || buff.Type == BuffType.Snare || buff.Type == BuffType.Fear
                         || buff.Type == BuffType.Taunt || buff.Type == BuffType.Knockback) || buff.Type == BuffType.Asleep)
                    .Aggregate(0d, (current, buff) => Math.Max(current, buff.EndTime));
            return (result - Game.Time);
        }

        internal static PredictionOutput GetPositionOnPath(PredictionInput input, List<Vector2> path, float speed = -1)
        {
            if (input.Unit.DistanceSquared(input.From) < 200 * 200)
            {
                speed /= 1.5f;
            }

            speed = (Math.Abs(speed - (-1)) < float.Epsilon) ? input.Unit.MoveSpeed : speed;

            if (path.Count <= 1 || (input.Unit.IsWindingUp && !input.Unit.IsDashing()))
            {
                return new PredictionOutput
                {
                    Input = input,
                    UnitPosition = input.Unit.Position,
                    CastPosition = input.Unit.Position,
                    Hitchance = HitChance.High
                };
            }

            var pLength = path.PathLength();

            //Skillshots with only a delay
            var tDistance = input.Delay * speed - input.RealRadius;
            if (pLength >= tDistance && Math.Abs(input.Speed - float.MaxValue) < float.Epsilon)
            {
                for (var i = 0; i < path.Count - 1; i++)
                {
                    var a = path[i];
                    var b = path[i + 1];
                    var d = a.Distance(b);

                    if (d >= tDistance)
                    {
                        var direction = (b - a).Normalized();

                        var cp = a + direction * tDistance;
                        var p = a +
                                direction *
                                ((i == path.Count - 2)
                                    ? Math.Min(tDistance + input.RealRadius, d)
                                    : (tDistance + input.RealRadius));

                        return new PredictionOutput
                        {
                            Input = input,
                            CastPosition = cp.ToVector3(),
                            UnitPosition = p.ToVector3(),
                            Hitchance = HitChance.High
                        };
                    }

                    tDistance -= d;
                }
            }

            //Skillshot with a delay and speed.
            if (pLength >= tDistance && Math.Abs(input.Speed - float.MaxValue) > float.Epsilon)
            {
                var d = tDistance;
                if (input.Type == SkillshotType.SkillshotLine || input.Type == SkillshotType.SkillshotCone)
                {
                    if (input.From.DistanceSquared(input.Unit.Position) < 200 * 200)
                    {
                        d = input.Delay * speed;
                    }
                }

                path = path.CutPath(d);
                var tT = 0f;
                for (var i = 0; i < path.Count - 1; i++)
                {
                    var a = path[i];
                    var b = path[i + 1];
                    var tB = a.Distance(b) / speed;
                    var direction = (b - a).Normalized();
                    a = a - speed * tT * direction;
                    var sol = Geometry.VectorMovementCollision(a, b, speed, input.From.ToVector2(), input.Speed, tT);
                    var t = (float)sol[0];
                    var pos = (Vector2)sol[1];

                    if (pos.IsValid() && t >= tT && t <= tT + tB)
                    {
                        if (pos.DistanceSquared(b) < 20)
                            break;
                        var p = pos + input.RealRadius * direction;

                        if (input.Type == SkillshotType.SkillshotLine && false)
                        {
                            var alpha = (input.From.ToVector2() - p).AngleBetween(a - b);
                            if (alpha > 30 && alpha < 180 - 30)
                            {
                                var beta = (float)Math.Asin(input.RealRadius / p.Distance(input.From));
                                var cp1 = input.From.ToVector2() + (p - input.From.ToVector2()).Rotated(beta);
                                var cp2 = input.From.ToVector2() + (p - input.From.ToVector2()).Rotated(-beta);

                                pos = cp1.DistanceSquared(pos) < cp2.DistanceSquared(pos) ? cp1 : cp2;
                            }
                        }

                        return new PredictionOutput
                        {
                            Input = input,
                            CastPosition = pos.ToVector3(),
                            UnitPosition = p.ToVector3(),
                            Hitchance = HitChance.High
                        };
                    }
                    tT += tB;
                }
            }

            var position = path.Last();
            return new PredictionOutput
            {
                Input = input,
                CastPosition = position.ToVector3(),
                UnitPosition = position.ToVector3(),
                Hitchance = HitChance.Medium
            };
        }


    }

    public static class AoePrediction
    {
        public static PredictionOutput GetPrediction(PredictionInput input)
        {
            switch (input.Type)
            {
                case SkillshotType.SkillshotCircle:
                    return Circle.GetPrediction(input);
                case SkillshotType.SkillshotCone:
                    return Cone.GetPrediction(input);
                case SkillshotType.SkillshotLine:
                    return Line.GetPrediction(input);
            }
            return new PredictionOutput();
        }

        internal static List<PossibleTarget> GetPossibleTargets(PredictionInput input)
        {
            var result = new List<PossibleTarget>();
            var originalUnit = input.Unit;
            foreach (var enemy in
                ObjectManager.Get<AIHeroClient>().Where(
                    h =>
                        !h.IsAlly &&
                        h.NetworkId != originalUnit.NetworkId &&
                        h.IsValidTarget((input.Range + 200 + input.RealRadius), true, input.RangeCheckFrom)))
            {
                input.Unit = enemy;
                var prediction = Prediction.GetPrediction(input, false, false);
                if (prediction.Hitchance >= HitChance.High)
                {
                    result.Add(new PossibleTarget { Position = prediction.UnitPosition.ToVector2(), Unit = enemy });
                }
            }
            return result;
        }

        public static class Circle
        {
            public static PredictionOutput GetPrediction(PredictionInput input)
            {
                var mainTargetPrediction = Prediction.GetPrediction(input, false, true);
                var posibleTargets = new List<PossibleTarget>
                {
                    new PossibleTarget { Position = mainTargetPrediction.UnitPosition.ToVector2(), Unit = input.Unit }
                };

                if (mainTargetPrediction.Hitchance >= HitChance.Medium)
                {
                    //Add the posible targets  in range:
                    posibleTargets.AddRange(GetPossibleTargets(input));
                }

                while (posibleTargets.Count > 1)
                {
                    var mecCircle = MEC.GetMec(posibleTargets.Select(h => h.Position).ToList());

                    if (mecCircle.Radius <= input.RealRadius - 10 &&
                        Vector2.DistanceSquared(mecCircle.Center, input.RangeCheckFrom.ToVector2()) <
                        input.Range * input.Range)
                    {
                        return new PredictionOutput
                        {
                            AoeTargetsHit = posibleTargets.Select(h => (AIHeroClient)h.Unit).ToList(),
                            CastPosition = mecCircle.Center.ToVector3(),
                            UnitPosition = mainTargetPrediction.UnitPosition,
                            Hitchance = mainTargetPrediction.Hitchance,
                            Input = input,
                            _aoeTargetsHitCount = posibleTargets.Count
                        };
                    }

                    float maxdist = -1;
                    var maxdistindex = 1;
                    for (var i = 1; i < posibleTargets.Count; i++)
                    {
                        var distance = Vector2.DistanceSquared(posibleTargets[i].Position, posibleTargets[0].Position);
                        if (distance > maxdist || maxdist.CompareTo(-1) == 0)
                        {
                            maxdistindex = i;
                            maxdist = distance;
                        }
                    }
                    posibleTargets.RemoveAt(maxdistindex);
                }

                return mainTargetPrediction;
            }
        }

        public static class Cone
        {
            internal static int GetHits(Vector2 end, double range, float angle, List<Vector2> points)
            {
                return (from point in points
                        let edge1 = end.Rotated(-angle / 2)
                        let edge2 = edge1.Rotated(angle)
                        where
                            point.DistanceSquared(new Vector2()) < range * range && edge1.CrossProduct(point) > 0 &&
                            point.CrossProduct(edge2) > 0
                        select point).Count();
            }

            public static PredictionOutput GetPrediction(PredictionInput input)
            {
                var mainTargetPrediction = Prediction.GetPrediction(input, false, true);
                var posibleTargets = new List<PossibleTarget>
                {
                    new PossibleTarget { Position = mainTargetPrediction.UnitPosition.ToVector2(), Unit = input.Unit }
                };

                if (mainTargetPrediction.Hitchance >= HitChance.Medium)
                {
                    //Add the posible targets  in range:
                    posibleTargets.AddRange(GetPossibleTargets(input));
                }

                if (posibleTargets.Count > 1)
                {
                    var candidates = new List<Vector2>();

                    foreach (var target in posibleTargets)
                    {
                        target.Position = target.Position - input.From.ToVector2();
                    }

                    for (var i = 0; i < posibleTargets.Count; i++)
                    {
                        for (var j = 0; j < posibleTargets.Count; j++)
                        {
                            if (i != j)
                            {
                                var p = (posibleTargets[i].Position + posibleTargets[j].Position) * 0.5f;
                                if (!candidates.Contains(p))
                                {
                                    candidates.Add(p);
                                }
                            }
                        }
                    }

                    var bestCandidateHits = -1;
                    var bestCandidate = new Vector2();
                    var positionsList = posibleTargets.Select(t => t.Position).ToList();

                    foreach (var candidate in candidates)
                    {
                        var hits = GetHits(candidate, input.Range, input.Radius, positionsList);
                        if (hits > bestCandidateHits)
                        {
                            bestCandidate = candidate;
                            bestCandidateHits = hits;
                        }
                    }

                    bestCandidate = bestCandidate + input.From.ToVector2();

                    if (bestCandidateHits > 1 && input.From.ToVector2().DistanceSquared(bestCandidate) > 50 * 50)
                    {
                        return new PredictionOutput
                        {
                            Hitchance = mainTargetPrediction.Hitchance,
                            _aoeTargetsHitCount = bestCandidateHits,
                            UnitPosition = mainTargetPrediction.UnitPosition,
                            CastPosition = bestCandidate.ToVector3(),
                            Input = input
                        };
                    }
                }
                return mainTargetPrediction;
            }
        }

        public static class Line
        {
            internal static IEnumerable<Vector2> GetHits(Vector2 start, Vector2 end, double radius, List<Vector2> points)
            {
                return points.Where(p => p.DistanceSquared(start, end, true) <= radius * radius);
            }

            internal static Vector2[] GetCandidates(Vector2 from, Vector2 to, float radius, float range)
            {
                var middlePoint = (from + to) / 2;
                var intersections = Geometry.CircleCircleIntersection(
                    from, middlePoint, radius, from.Distance(middlePoint));

                if (intersections.Length > 1)
                {
                    var c1 = intersections[0];
                    var c2 = intersections[1];

                    c1 = from + range * (to - c1).Normalized();
                    c2 = from + range * (to - c2).Normalized();

                    return new[] { c1, c2 };
                }

                return new Vector2[] { };
            }

            public static PredictionOutput GetPrediction(PredictionInput input)
            {
                var mainTargetPrediction = Prediction.GetPrediction(input, false, true);
                var posibleTargets = new List<PossibleTarget>
                {
                    new PossibleTarget { Position = mainTargetPrediction.UnitPosition.ToVector2(), Unit = input.Unit }
                };
                if (mainTargetPrediction.Hitchance >= HitChance.Medium)
                {
                    //Add the posible targets  in range:
                    posibleTargets.AddRange(GetPossibleTargets(input));
                }

                if (posibleTargets.Count > 1)
                {
                    var candidates = new List<Vector2>();
                    foreach (var target in posibleTargets)
                    {
                        var targetCandidates = GetCandidates(
                            input.From.ToVector2(), target.Position, (input.Radius), input.Range);
                        candidates.AddRange(targetCandidates);
                    }

                    var bestCandidateHits = -1;
                    var bestCandidate = new Vector2();
                    var bestCandidateHitPoints = new List<Vector2>();
                    var positionsList = posibleTargets.Select(t => t.Position).ToList();

                    foreach (var candidate in candidates)
                    {
                        if (
                            GetHits(
                                input.From.ToVector2(), candidate, (input.Radius + input.Unit.BoundingRadius / 3 - 10),
                                new List<Vector2> { posibleTargets[0].Position }).Count() == 1)
                        {
                            var hits = GetHits(input.From.ToVector2(), candidate, input.Radius, positionsList).ToList();
                            var hitsCount = hits.Count;
                            if (hitsCount >= bestCandidateHits)
                            {
                                bestCandidateHits = hitsCount;
                                bestCandidate = candidate;
                                bestCandidateHitPoints = hits.ToList();
                            }
                        }
                    }

                    if (bestCandidateHits > 1)
                    {
                        float maxDistance = -1;
                        Vector2 p1 = new Vector2(), p2 = new Vector2();

                        //Center the position
                        for (var i = 0; i < bestCandidateHitPoints.Count; i++)
                        {
                            for (var j = 0; j < bestCandidateHitPoints.Count; j++)
                            {
                                var startP = input.From.ToVector2();
                                var endP = bestCandidate;
                                var proj1 = positionsList[i].ProjectOn(startP, endP);
                                var proj2 = positionsList[j].ProjectOn(startP, endP);
                                var dist = Vector2.DistanceSquared(bestCandidateHitPoints[i], proj1.LinePoint) +
                                           Vector2.DistanceSquared(bestCandidateHitPoints[j], proj2.LinePoint);
                                if (dist >= maxDistance &&
                                    (proj1.LinePoint - positionsList[i]).AngleBetween(
                                        proj2.LinePoint - positionsList[j]) > 90)
                                {
                                    maxDistance = dist;
                                    p1 = positionsList[i];
                                    p2 = positionsList[j];
                                }
                            }
                        }

                        return new PredictionOutput
                        {
                            Hitchance = mainTargetPrediction.Hitchance,
                            _aoeTargetsHitCount = bestCandidateHits,
                            UnitPosition = mainTargetPrediction.UnitPosition,
                            CastPosition = ((p1 + p2) * 0.5f).ToVector3(),
                            Input = input
                        };
                    }
                }

                return mainTargetPrediction;
            }
        }

        internal class PossibleTarget
        {
            public Vector2 Position;
            public AIBaseClient Unit;
        }
    }

    public static class Collision
    {
        static Collision()
        {

        }

        /// <summary>
        ///     Returns the list of the units that the skillshot will hit before reaching the set positions.
        /// </summary>
        /// 
        private static bool MinionIsDead(PredictionInput input, AIBaseClient minion, float distance)
        {
            float delay = (distance / input.Speed) + input.Delay;

            if (Math.Abs(input.Speed - float.MaxValue) < float.Epsilon)
                delay = input.Delay;

            int convert = (int)(delay * 1000) - Game.Ping;

            if (HealthPrediction.LaneClearHealthPrediction(minion, convert, 0) <= 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool GetCollision(List<Vector3> positions, PredictionInput input)
        {

            foreach (var position in positions)
            {
                foreach (var objectType in input.CollisionObjects)
                {
                    switch (objectType)
                    {
                        case CollisionableObjects.Minions:
                            foreach (var minion in Cache.GetMinions(input.From, Math.Max(input.Range + 500, 2000)))
                            {

                                var distanceFromToUnit = minion.Position.Distance(input.From);
                                var bOffset = minion.BoundingRadius + input.Unit.BoundingRadius;
                                if (distanceFromToUnit < bOffset)
                                {
                                    return true;
                                }

                                if (minion.Position.Distance(position) < bOffset)
                                {
                                    return true;
                                }

                                if (minion.Position.Distance(input.Unit.Position) < bOffset)
                                {
                                    return true;
                                }

                                {
                                    var minionPos = minion.Position;
                                    int bonusRadius = 15;
                                    if (minion.IsMoving)
                                    {
                                        var predInput2 = new PredictionInput
                                        {
                                            Collision = false,
                                            Speed = input.Speed,
                                            Delay = input.Delay,
                                            Range = input.Range,
                                            From = input.From,
                                            Radius = input.Radius,
                                            Unit = minion,
                                            Type = input.Type
                                        };
                                        minionPos = Prediction.GetPrediction(predInput2).CastPosition;
                                        bonusRadius = 50 + (int)input.Radius;
                                    }

                                    if (minionPos.ToVector2().Distance(input.From.ToVector2(), position.ToVector2(), false) <= input.Radius + bonusRadius + minion.BoundingRadius)
                                    {
                                        return true;
                                    }
                                }
                            }
                            break;
                        case CollisionableObjects.Heroes:
                            foreach (var hero in
                                ObjectManager.Get<AIHeroClient>().Where(
                                    hero =>
                                        !hero.IsAlly &&
                                        hero.IsValidTarget(
                                            Math.Min(input.Range + input.Radius + 100, 2000), true, input.RangeCheckFrom))
                                )
                            {
                                input.Unit = hero;
                                var prediction = Prediction.GetPrediction(input, false, false);
                                if (
                                    prediction.UnitPosition.ToVector2()
                                        .DistanceSquared(input.From.ToVector2(), position.ToVector2(), true) <=
                                    Math.Pow((input.Radius + 50 + hero.BoundingRadius), 2))
                                {
                                    return true;
                                }
                            }
                            break;

                        case CollisionableObjects.Walls:
                            var step = position.Distance(input.From) / 20;
                            for (var i = 0; i < 20; i++)
                            {
                                var p = input.From.ToVector2().Extend(position.ToVector2(), step * i);
                                if (NavMesh.GetCollisionFlags(p.X, p.Y).HasFlag(CollisionFlags.Wall))
                                {
                                    return true;
                                }
                            }
                            break;
                    }
                }
            }
            return false;
        }
    }

    internal class PathInfo
    {
        public Vector2 Position { get; set; }
        public float Time { get; set; }
    }

    internal class Spells
    {
        public string name { get; set; }
        public double duration { get; set; }
    }

    internal class UnitTrackerInfo
    {
        public int NetworkId { get; set; }
        public int AaTick { get; set; }
        public int NewPathTick { get; set; }
        public int StopMoveTick { get; set; }
        public int LastInvisableTick { get; set; }
        public int SpecialSpellFinishTick { get; set; }
        public List<PathInfo> PathBank = new List<PathInfo>();
    }

    internal static class UnitTracker
    {
        public static List<UnitTrackerInfo> UnitTrackerInfoList = new List<UnitTrackerInfo>();
        private static List<AIHeroClient> Champion = new List<AIHeroClient>();
        private static List<Spells> spells = new List<Spells>();
        private static List<PathInfo> PathBank = new List<PathInfo>();
        static UnitTracker()
        {
            spells.Add(new Spells() { name = "katarinar", duration = 1 }); //Katarinas R
            spells.Add(new Spells() { name = "drain", duration = 1 }); //Fiddle W
            spells.Add(new Spells() { name = "crowstorm", duration = 1 }); //Fiddle R
            spells.Add(new Spells() { name = "consume", duration = 0.5 }); //Nunu Q
            spells.Add(new Spells() { name = "absolutezero", duration = 1 }); //Nunu R
            spells.Add(new Spells() { name = "staticfield", duration = 0.5 }); //Blitzcrank R
            spells.Add(new Spells() { name = "cassiopeiapetrifyinggaze", duration = 0.5 }); //Cassio's R
            spells.Add(new Spells() { name = "ezrealtrueshotbarrage", duration = 1 }); //Ezreal's R
            spells.Add(new Spells() { name = "galioidolofdurand", duration = 1 }); //Ezreal's R                                                                   
            spells.Add(new Spells() { name = "luxmalicecannon", duration = 1 }); //Lux R
            spells.Add(new Spells() { name = "reapthewhirlwind", duration = 1 }); //Jannas R
            spells.Add(new Spells() { name = "jinxw", duration = 0.6 }); //jinxW
            spells.Add(new Spells() { name = "jinxr", duration = 0.6 }); //jinxR
            spells.Add(new Spells() { name = "missfortunebullettime", duration = 1 }); //MissFortuneR
            spells.Add(new Spells() { name = "shenstandunited", duration = 1 }); //ShenR
            spells.Add(new Spells() { name = "threshe", duration = 0.4 }); //ThreshE
            spells.Add(new Spells() { name = "threshrpenta", duration = 0.75 }); //ThreshR
            spells.Add(new Spells() { name = "threshq", duration = 0.75 }); //ThreshQ
            spells.Add(new Spells() { name = "infiniteduress", duration = 1 }); //Warwick R
            spells.Add(new Spells() { name = "meditate", duration = 1 }); //yi W
            spells.Add(new Spells() { name = "alzaharnethergrasp", duration = 1 }); //Malza R
            spells.Add(new Spells() { name = "lucianq", duration = 0.5 }); //Lucian Q
            spells.Add(new Spells() { name = "caitlynpiltoverpeacemaker", duration = 0.5 }); //Caitlyn Q
            spells.Add(new Spells() { name = "velkozr", duration = 0.5 }); //Velkoz R 
            spells.Add(new Spells() { name = "jhinr", duration = 2 }); //Velkoz R 

            foreach (var hero in ObjectManager.Get<AIHeroClient>())
            {
                Champion.Add(hero);
                UnitTrackerInfoList.Add(new UnitTrackerInfo() { NetworkId = (int)hero.NetworkId, AaTick = Variables.TickCount, StopMoveTick = Variables.TickCount, NewPathTick = Variables.TickCount, SpecialSpellFinishTick = Variables.TickCount, LastInvisableTick = Variables.TickCount });
            }

            AIBaseClient.OnProcessSpellCast += AIBaseClient_OnProcessSpellCast;
            AIBaseClient.OnNewPath += AIHeroClient_OnNewPath;
            //AttackableUnit.vi += AIBaseClient_OnEnterLocalVisiblityClient;
        }

        private static void AIBaseClient_OnEnterLocalVisiblityClient(AttackableUnit sender, EventArgs args)
        {
            if (sender is AIHeroClient)
                UnitTrackerInfoList.Find(x => x.NetworkId == sender.NetworkId).LastInvisableTick = Variables.TickCount;
        }

        private static void AIHeroClient_OnNewPath(AIBaseClient sender, AIBaseClientNewPathEventArgs args)
        {
            if (sender is AIHeroClient)
            {               
                var item = UnitTrackerInfoList.Find(x => x.NetworkId == sender.NetworkId);

                if (UnitTrackerInfoList == null || item == null || args.Path.Last() == Vector3.Zero)
                {
                    /*if(args.Path.First() != Vector3.Zero)
                        item.PathBank.Add(new PathInfo() { Position = args.Path.First().ToVector2(), Time = Variables.TickCount });*/
                    return;
                }

                if (args.Path.Count() == 1) // STOP MOVE DETECTION
                    item.StopMoveTick = Variables.TickCount;

                item.NewPathTick = Variables.TickCount;
                item.PathBank.Add(new PathInfo() { Position = args.Path.Last().ToVector2(), Time = Variables.TickCount });

                if (item.PathBank.Count > 3)
                    item.PathBank.RemoveAt(0);
            }
        }

        private static void AIBaseClient_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (UnitTrackerInfoList == null)
                return;

            if (sender is AIHeroClient)
            {
                if (Orbwalker.IsAutoAttack(args.SData.Name))
                    UnitTrackerInfoList.Find(x => x.NetworkId == sender.NetworkId).AaTick = Variables.TickCount;
                else
                {

                    var foundSpell = spells.Find(x => args.SData.Name.ToLower() == x.name.ToLower());
                    if (foundSpell != null)
                    {
                        UnitTrackerInfoList.Find(x => x.NetworkId == sender.NetworkId).SpecialSpellFinishTick = Variables.TickCount + (int)(foundSpell.duration * 1000);
                    }
                    else if (sender.IsWindingUp || !sender.CanMove)
                    {
                        UnitTrackerInfoList.Find(x => x.NetworkId == sender.NetworkId).SpecialSpellFinishTick = Variables.TickCount + 100;
                    }
                }
            }
        }

        public static bool SpamSamePlace(AIBaseClient unit)
        {
            var TrackerUnit = UnitTrackerInfoList.Find(x => x.NetworkId == unit.NetworkId);
            if (TrackerUnit.PathBank.Count < 3)
                return false;

            if (TrackerUnit.PathBank[2].Time - TrackerUnit.PathBank[1].Time < 180 && Variables.TickCount - TrackerUnit.PathBank[2].Time < 90)
            {
                if (TrackerUnit.PathBank[1].Position.Distance(TrackerUnit.PathBank[2].Position) < 50)
                {
                    Console.WriteLine("PRED VH: SPAM PLACE");
                    return true;
                }
                else
                {
                    var C = TrackerUnit.PathBank[1].Position;
                    var A = TrackerUnit.PathBank[2].Position;

                    var B = unit.Position.ToVector2();

                    var AB = Math.Pow(A.X - B.X, 2) + Math.Pow(A.Y - B.Y, 2);
                    var BC = Math.Pow(B.X - C.X, 2) + Math.Pow(B.Y - C.Y, 2);
                    var AC = Math.Pow(A.X - C.X, 2) + Math.Pow(A.Y - C.Y, 2);

                    if (Math.Cos((AB + BC - AC) / (2 * Math.Sqrt(AB) * Math.Sqrt(BC))) * 180 / Math.PI < 31)
                    {
                        //Console.WriteLine("PRED VH: SPAM ANGLE");
                        return true;
                    }
                }
            }
            return false;
        }

        public static List<Vector2> GetPathWayCalc(AIBaseClient unit)
        {
            var TrackerUnit = UnitTrackerInfoList.Find(x => x.NetworkId == unit.NetworkId);
            List<Vector2> points = new List<Vector2>();
            points.Add(unit.Position.ToVector2());
            return points;
        }

        public static double GetSpecialSpellEndTime(AIBaseClient unit)
        {
            var TrackerUnit = UnitTrackerInfoList.Find(x => x.NetworkId == unit.NetworkId);

            if (TrackerUnit == null)
                return Variables.TickCount;

            return TrackerUnit.SpecialSpellFinishTick - Variables.TickCount;
        }

        public static double GetLastAutoAttackTime(AIBaseClient unit)
        {
            var TrackerUnit = UnitTrackerInfoList.Find(x => x.NetworkId == unit.NetworkId);

            if (TrackerUnit == null)
                return Variables.TickCount;

            return Variables.TickCount - TrackerUnit.AaTick;
        }

        public static double GetLastNewPathTime(AIBaseClient unit)
        {
            var TrackerUnit = UnitTrackerInfoList.Find(x => x.NetworkId == unit.NetworkId);

            if (TrackerUnit == null)
                return Variables.TickCount;

            return Variables.TickCount - TrackerUnit.NewPathTick;
        }

        public static double GetLastVisableTime(AIBaseClient unit)
        {
            var TrackerUnit = UnitTrackerInfoList.Find(x => x.NetworkId == unit.NetworkId);

            if (TrackerUnit == null)
                return Variables.TickCount;

            return Variables.TickCount - TrackerUnit.LastInvisableTick;
        }

        public static double GetLastStopMoveTime(AIBaseClient unit)
        {
            var TrackerUnit = UnitTrackerInfoList.Find(x => x.NetworkId == unit.NetworkId);

            if (TrackerUnit == null)
                return Variables.TickCount;

            return Variables.TickCount - TrackerUnit.StopMoveTick;
        }
    }

}