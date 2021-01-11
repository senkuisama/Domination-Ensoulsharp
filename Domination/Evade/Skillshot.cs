// Copyright 2014 - 2014 Esk0r
// Skillshot.cs is part of Evade.
// 
// Evade is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Evade is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Evade. If not, see <http://www.gnu.org/licenses/>.

// GitHub: https://github.com/Esk0r/LeagueSharp/blob/master/Evade


namespace DaoHungAIO.Evade
{
    #region

    using EnsoulSharp;
    using EnsoulSharp.SDK;
    using EnsoulSharp.SDK.MenuUI;
    using SharpDX;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    #endregion

    public class Skillshot
    {
        public Geometry.Polygon Polygon;
        public Geometry.Polygon DrawingPolygon;
        public Geometry.Polygon.Arc Arc;
        public Geometry.Polygon.Ring Ring;
        public Geometry.Polygon.Sector Sector;
        public Geometry.Polygon.Circle Circle;
        public Geometry.Polygon.Rectangle Rectangle;

        public DetectionType DetectionType;

        public Vector2 End;
        public Vector2 Start;
        public Vector2 Direction;
        public Vector2 OriginalEnd;
        public Vector2 MissilePosition;

        public bool ForceDisabled;

        public SpellData SpellData;

        public int StartTick;

        private int _helperTick;
        private int _cachedValueTick;
        private int _lastCollisionCalc;

        private bool _cachedValue;

        private Vector2 _collisionEnd;
        private IOrderedEnumerable<AIHeroClient> bestAllies;

        public Skillshot(DetectionType detectionType, SpellData spellData, int startT, Vector2 start, Vector2 end, AIBaseClient unit)
        {
            DetectionType = detectionType;
            SpellData = spellData;
            StartTick = startT;
            Start = start;
            End = end;
            MissilePosition = start;
           Direction = (Vector2)(end - start).ToVector3().Normalized();

            Unit = unit;
            //if (ObjectManager.Player.CharacterName == "Janna" ||
            //    ObjectManager.Player.CharacterName == "Rakan" || ObjectManager.Player.CharacterName == "Ivern" ||
            //    ObjectManager.Player.CharacterName == "Lulu" ||
            //    ObjectManager.Player.CharacterName == "Karma")
            //{
            //    bestAllies = GameObjects.AllyHeroes
            //        .Where(t =>
            //            t.Distance(ObjectManager.Player) < Helper.Spells[Helper.E].Range)
            //        .OrderBy(x => x.Health);
            //}
            //if (ObjectManager.Player.CharacterName == "Lux" ||
            //    ObjectManager.Player.CharacterName == "Sona" ||
            //    ObjectManager.Player.CharacterName == "Taric")

            //{
            //    bestAllies = GameObjects.AllyHeroes
            //        .Where(t =>
            //            t.Distance(ObjectManager.Player) < Helper.Spells[Helper.W].Range)
            //        .OrderBy(x => x.Health);
            //}
            foreach (var ally in bestAllies)
            {

                switch (spellData.Type)
                {
                    case SkillShotType.SkillshotCircle:
                        Circle = new Geometry.Polygon.Circle(CollisionEnd, spellData.Radius, 22);
                        break;
                    case SkillShotType.SkillshotLine:
                        Rectangle = new Geometry.Polygon.Rectangle(Start, CollisionEnd, spellData.Radius);
                        break;
                    case SkillShotType.SkillshotMissileLine:
                        Rectangle = new Geometry.Polygon.Rectangle(Start, CollisionEnd, spellData.Radius);
                        break;
                    case SkillShotType.SkillshotCone:
                        Sector = new Geometry.Polygon.Sector(
                            start, CollisionEnd - start, spellData.Radius * (float) Math.PI / 180, spellData.Range, 22);
                        break;
                    case SkillShotType.SkillshotRing:
                        Ring = new Geometry.Polygon.Ring(CollisionEnd, spellData.Radius, spellData.RingRadius, 22);
                        break;
                    case SkillShotType.SkillshotArc:
                        Arc = new Geometry.Polygon.Arc(start, end,
                            EvadeManager.SkillShotsExtraRadius + (int) ally.BoundingRadius, 22);
                        break;
                }
            }
            UpdatePolygon();
        }

     

        public Vector2 Perpendicular => Direction.Perpendicular();

        public Vector2 CollisionEnd
        {
            get
            {
                if (_collisionEnd.IsValid())
                {
                    return _collisionEnd;
                }

                if (IsGlobal)
                {
                    //if (ObjectManager.Player.CharacterName == "Janna" ||
                    //    ObjectManager.Player.CharacterName == "Rakan" ||
                    //    ObjectManager.Player.CharacterName == "Lulu" || ObjectManager.Player.CharacterName == "Ivern" ||
                    //    ObjectManager.Player.CharacterName == "Karma")
                    //{
                    //    bestAllies = GameObjects.AllyHeroes
                    //        .Where(t =>
                    //            t.Distance(ObjectManager.Player) < Helper.Spells[Helper.E].Range)
                    //        .OrderBy(x => x.Health);
                    //}
                    //if (ObjectManager.Player.CharacterName == "Lux" || ObjectManager.Player.CharacterName == "Sona" ||
                    //    ObjectManager.Player.CharacterName == "Taric")

                    //{
                    //    bestAllies = GameObjects.AllyHeroes
                    //        .Where(t =>
                    //            t.Distance(ObjectManager.Player) < Helper.Spells[Helper.W].Range)
                    //        .OrderBy(x => x.Health);
                    //}
                    foreach (var ally in bestAllies)
                    {

                        return GlobalGetMissilePosition(0) +
                               Direction * SpellData.MissileSpeed *
                               (0.5f + SpellData.Radius * 2 /
                               ally.MoveSpeed);
                    }
                }

                return End;
            }
        }

        public bool IsGlobal => SpellData.RawRange == 20000;

        public Geometry.Polygon EvadePolygon { get; set; }

        public Geometry.Polygon PathFindingPolygon { get; set; }

        public Geometry.Polygon PathFindingInnerPolygon { get; set; }

        public AIBaseClient Unit { get; set; }

        public int GetSlider(string name)
        {
            return EvadeManager.SkillShotMenu["Evade" + SpellData.ChampionName.ToLower()][name + SpellData.MenuItemName].GetValue<MenuSlider>().Value;
        }

        public bool GetBool(string name)
        {
            return EvadeManager.SkillShotMenu["Evade" + SpellData.ChampionName.ToLower()][name + SpellData.MenuItemName].GetValue<MenuBool>().Enabled;
        }

        public bool IsActive()
        {
            if (SpellData.MissileAccel != 0)
            {
                return Utils.GameTimeTickCount <= StartTick + 5000;
            }

            return Utils.GameTimeTickCount <=
                   StartTick + SpellData.Delay + SpellData.ExtraDuration +
                   1000 * (Start.Distance(End) / SpellData.MissileSpeed);
        }

        public bool Evade()
        {
            if (ForceDisabled)
            {
                return false;
            }

            if (Utils.GameTimeTickCount - _cachedValueTick < 100)
            {
                return _cachedValue;
            }

            _cachedValue = GetBool("Enabled");
            _cachedValueTick = Utils.GameTimeTickCount;

            return _cachedValue;
        }

        public void OnUpdate()
        {
            if (SpellData.CollisionObjects.Length > 0 && SpellData.CollisionObjects != null 
                && Utils.GameTimeTickCount - _lastCollisionCalc > 50)
            {
                _lastCollisionCalc = Utils.GameTimeTickCount;
                _collisionEnd = Collision.GetCollisionPoint(this);
            }

            if (SpellData.Type == SkillShotType.SkillshotMissileLine)
            {
                Rectangle = new Geometry.Polygon.Rectangle(GetMissilePosition(0), CollisionEnd, SpellData.Radius);
                UpdatePolygon();
            }

            if (SpellData.MissileFollowsUnit)
            {
                if (Unit.IsVisible)
                {
                    End = Unit.Position.ToVector2();
                    Direction = (End - Start).Normalized();
                    UpdatePolygon();
                }
            }

            if (SpellData.SpellName == "TaricE")
            {
                Start = Unit.Position.ToVector2();
                End = Start + Direction * SpellData.Range;
                Rectangle = new Geometry.Polygon.Rectangle(Start, End, SpellData.Radius);
                UpdatePolygon();
            }


            if (SpellData.FollowCaster)
            {
                Circle.Center = Unit.Position.ToVector2();
                UpdatePolygon();
            }
        }

        public void UpdatePolygon()
        {
            switch (SpellData.Type)
            {
                case SkillShotType.SkillshotCircle:
                    Circle.UpdatePolygon();
                    Polygon = Circle;
                    break;
                case SkillShotType.SkillshotLine:
                    Rectangle.UpdatePolygon();
                    Polygon = Rectangle;
                    break;
                case SkillShotType.SkillshotMissileLine:
                    Rectangle.UpdatePolygon();
                    Polygon = Rectangle;
                    break;
                case SkillShotType.SkillshotCone:
                    Sector.UpdatePolygon();
                    Polygon = Sector;
                    break;
                case SkillShotType.SkillshotRing:
                    Ring.UpdatePolygon();
                    Polygon = Ring;
                    break;
                case SkillShotType.SkillshotArc:
                    Arc.UpdatePolygon();
                    Polygon = Arc;
                    break;
            }
        }

        public Vector2 GlobalGetMissilePosition(int time)
        {
            var t = Math.Max(0, Utils.GameTimeTickCount + time - StartTick - SpellData.Delay);

            t = (int) Math.Max(0, Math.Min(End.Distance(Start), t * SpellData.MissileSpeed / 1000));

            return Start + Direction * t;
        }

        public Vector2 GetMissilePosition(int time)
        {
            var t = Math.Max(0, Utils.GameTimeTickCount + time - StartTick - SpellData.Delay);
            var x = 0;

            if (SpellData.MissileAccel == 0)
            {
                x = t * SpellData.MissileSpeed / 1000;
            }
            else
            {
                var t1 = (SpellData.MissileAccel > 0
                    ? SpellData.MissileMaxSpeed
                    : SpellData.MissileMinSpeed - SpellData.MissileSpeed) * 1000f / SpellData.MissileAccel;

                if (t <= t1)
                {
                    x =
                        (int)
                            (t * SpellData.MissileSpeed / 1000d + 0.5d * SpellData.MissileAccel * Math.Pow(t / 1000d, 2));
                }
                else
                {
                    x =
                        (int)
                            (t1 * SpellData.MissileSpeed / 1000d +
                             0.5d * SpellData.MissileAccel * Math.Pow(t1 / 1000d, 2) +
                             (t - t1) / 1000d *
                             (SpellData.MissileAccel < 0 ? SpellData.MissileMaxSpeed : SpellData.MissileMinSpeed));
                }
            }

            t = (int) Math.Max(0, Math.Min(CollisionEnd.Distance(Start), x));

            return Start + Direction * t;
        }
        public bool IsSafePoint(Vector2 point)
        {
            return this.Polygon.IsOutside(point);
        }
        public SafePathResult IsSafePath(List<Vector2> path, int timeOffset, int speed = -1, int delay = 0,
            AIBaseClient unit = null)
        {
            
                var Distance = 0f;
                //if (ObjectManager.Player.CharacterName == "Janna" ||
                //    ObjectManager.Player.CharacterName == "Rakan" || ObjectManager.Player.CharacterName == "Ivern" ||
                //    ObjectManager.Player.CharacterName == "Lulu" ||
                //    ObjectManager.Player.CharacterName == "Karma")
                //{
                //    bestAllies = GameObjects.AllyHeroes
                //        .Where(t =>
                //            t.Distance(ObjectManager.Player) < Helper.Spells[Helper.E].Range)
                //        .OrderBy(x => x.Health);
                //}
                //if (ObjectManager.Player.CharacterName == "Lux" || ObjectManager.Player.CharacterName == "Sona" ||
                //    ObjectManager.Player.CharacterName == "Taric")

                //{
                //    bestAllies = GameObjects.AllyHeroes
                //        .Where(t =>
                //            t.Distance(ObjectManager.Player) < Helper.Spells[Helper.W].Range)
                //        .OrderBy(x => x.Health);
                //}
                foreach (var ally in bestAllies)
                {

                    timeOffset += Game.Ping / 2;
                    speed = speed == -1
                        ? (int) ally.MoveSpeed
                        : speed;
                }

                if (unit == null)
                {
                    foreach (var ally in bestAllies)
                    {
                        unit = ally;
                    }
                }

                var allIntersections = new List<FoundIntersection>();

                for (var i = 0; i <= path.Count - 2; i++)
                {
                    var from = path[i];
                    var to = path[i + 1];
                    var segmentIntersections = new List<FoundIntersection>();

                    for (var j = 0; j <= Polygon.Points.Count - 1; j++)
                    {
                        var sideStart = Polygon.Points[j];
                        var sideEnd = Polygon.Points[j == (Polygon.Points.Count - 1) ? 0 : j + 1];
                        var intersection = from.Intersection(to, sideStart, sideEnd);

                        if (intersection.Intersects)
                        {
                            segmentIntersections.Add(
                                new FoundIntersection(
                                    Distance + intersection.Point.Distance(from),
                                    (int) ((Distance + intersection.Point.Distance(from)) * 1000 / speed),
                                    intersection.Point, from));
                        }
                    }

                    var sortedList = segmentIntersections.OrderBy(o => o.Distance).ToList();
                    allIntersections.AddRange(sortedList);

                    Distance += from.Distance(to);
                }

                if (SpellData.Type == SkillShotType.SkillshotMissileLine ||
                    SpellData.Type == SkillShotType.SkillshotMissileCone ||
                    SpellData.Type == SkillShotType.SkillshotArc)
                {

                    foreach (var ally in bestAllies)
                    {

                        if (IsSafe(ally.Position.ToVector2()))
                        {

                            if (allIntersections.Count == 0)
                            {
                                return new SafePathResult(true, new FoundIntersection());
                            }

                            if (SpellData.DontCross)
                            {
                                return new SafePathResult(false, allIntersections[0]);
                            }

                            for (var i = 0; i <= allIntersections.Count - 1; i = i + 2)
                            {
                                var enterIntersection = allIntersections[i];
                                var enterIntersectionProjection =
                                    enterIntersection.Point.ProjectOn(Start, End).SegmentPoint;

                                if (i == allIntersections.Count - 1)
                                {
                                    var missilePositionOnIntersection =
                                        GetMissilePosition(enterIntersection.Time - timeOffset);
                                    return
                                        new SafePathResult(
                                            End.Distance(missilePositionOnIntersection) + 50 <=
                                            End.Distance(enterIntersectionProjection) &&
                                            ally.MoveSpeed <
                                            SpellData.MissileSpeed, allIntersections[0]);
                                }

                                var exitIntersection = allIntersections[i + 1];
                                var exitIntersectionProjection =
                                    exitIntersection.Point.ProjectOn(Start, End).SegmentPoint;
                                var missilePosOnEnter = GetMissilePosition(enterIntersection.Time - timeOffset);
                                var missilePosOnExit = GetMissilePosition(exitIntersection.Time + timeOffset);

                                if (missilePosOnEnter.Distance(End) + 50 > enterIntersectionProjection.Distance(End))
                                {
                                    if (missilePosOnExit.Distance(End) <= exitIntersectionProjection.Distance(End))
                                    {
                                        return new SafePathResult(false, allIntersections[0]);
                                    }
                                }
                            }
                        }
                        return new SafePathResult(true, allIntersections[0]);
                    }

                    if (allIntersections.Count == 0)
                    {
                        return new SafePathResult(false, new FoundIntersection());
                    }

                    if (allIntersections.Count > 0)
                    {
                        var exitIntersection = allIntersections[0];
                        var exitIntersectionProjection = exitIntersection.Point.ProjectOn(Start, End).SegmentPoint;
                        var missilePosOnExit = GetMissilePosition(exitIntersection.Time + timeOffset);

                        if (missilePosOnExit.Distance(End) <= exitIntersectionProjection.Distance(End))
                        {
                            return new SafePathResult(false, allIntersections[0]);
                        }
                    }
                }

                foreach (var ally in bestAllies)
                {
                    if (IsSafe(ally.Position.ToVector2()))
                    {
                        if (allIntersections.Count == 0)
                        {
                            return new SafePathResult(true, new FoundIntersection());
                        }

                        if (SpellData.DontCross)
                        {
                            return new SafePathResult(false, allIntersections[0]);
                        }
                    }

                    else
                    {
                        if (allIntersections.Count == 0)
                        {
                            return new SafePathResult(false, new FoundIntersection());
                        }
                    }
                }
            
            var timeToExplode = (SpellData.DontAddExtraDuration ? 0 : SpellData.ExtraDuration) + SpellData.Delay +
                                (int) (1000 * Start.Distance(End) / SpellData.MissileSpeed) -
                                (Utils.GameTimeTickCount - StartTick);
            var myPositionWhenExplodes = path.PositionAfter(timeToExplode, speed, delay);

            if (!IsSafe(myPositionWhenExplodes))
            {
                return new SafePathResult(false, allIntersections[0]);
            }

            var myPositionWhenExplodesWithOffset = path.PositionAfter(timeToExplode, speed, timeOffset);

            return new SafePathResult(IsSafe(myPositionWhenExplodesWithOffset), allIntersections[0]);

        }


        public bool IsSafe(Vector2 point)
        {
            return Polygon.IsOutside(point);
        }

        public bool IsDanger(Vector2 point)
        {
            return !IsSafe(point);
        }

        public bool IsAboutToHit(int time, AIBaseClient unit)
        {

            if (SpellData.Type == SkillShotType.SkillshotMissileLine)
            {
                var missilePos = GetMissilePosition(0);
                var missilePosAfterT = GetMissilePosition(time);
                var projection = unit.Position.ToVector2().ProjectOn(missilePos, missilePosAfterT);

                return projection.IsOnSegment &&
                       projection.SegmentPoint.Distance(unit.Position) < SpellData.Radius;
            }

            if (!IsSafe(unit.Position.ToVector2()))
            {
                var timeToExplode = SpellData.ExtraDuration + SpellData.Delay +
                                    (int) (1000*Start.Distance(End)/SpellData.MissileSpeed) -
                                    (Utils.GameTimeTickCount - StartTick);

                if (timeToExplode <= time)
                {
                    return true;
                }
            }

            return false;
        }
    }
}