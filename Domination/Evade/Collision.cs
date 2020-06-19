// Copyright 2014 - 2014 Esk0r
// Collision.cs is part of Evade.
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
    using SharpDX;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    #endregion

    internal static class Collision
    {

        private static int WallCastT;
        private static Vector2 YasuoWallCastedPos;

        public static void Init()
        {
            AIBaseClient.OnProcessSpellCast += OnProcessSpellCast;
        }

        private static void OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (sender.IsValid && sender.Team == ObjectManager.Player.Team && args.SData.Name == "YasuoWMovingWall")
            {
                WallCastT = Utils.GameTimeTickCount;
                YasuoWallCastedPos = sender.Position.ToVector2();
            }
        }

        public static FastPredResult FastPrediction(Vector2 from, AIBaseClient unit, int delay, int speed)
        {
            var tDelay = delay / 1000f + from.Distance(unit) / speed;
            var d = tDelay * unit.MoveSpeed;
            var path = unit.GetWaypoints();

            if (path.PathLength() > d)
            {
                return new FastPredResult
                {
                    IsMoving = true,
                    CurrentPos = unit.Position.ToVector2(),
                    PredictedPos = path.CutPath((int) d)[0]
                };
            }

            if (path.Count == 0)
            {
                return new FastPredResult
                {
                    IsMoving = false,
                    CurrentPos = unit.Position.ToVector2(),
                    PredictedPos = unit.Position.ToVector2()
                };
            }

            return new FastPredResult
            {
                IsMoving = false,
                CurrentPos = path[path.Count - 1],
                PredictedPos = path[path.Count - 1]
            };
        }

        public static Vector2 GetCollisionPoint(Skillshot skillshot)
        {
            var collisions = new List<DetectedCollision>();
            var from = skillshot.GetMissilePosition(0);

            skillshot.ForceDisabled = false;

            foreach (var cObject in skillshot.SpellData.CollisionObjects)
            {
                switch (cObject)
                {
                    case CollisionObjectTypes.Minion:
                        collisions.AddRange(
                            from minion in
                            GameObjects.EnemyMinions.Where(
                                x => x.IsValidTarget(1200, false, @from.ToVector3()) && x.MaxHealth > 5)
                            let pred =
                            FastPrediction(@from, minion,
                                Math.Max(0, skillshot.SpellData.Delay - (Utils.GameTimeTickCount - skillshot.StartTick)),
                                skillshot.SpellData.MissileSpeed)
                            let pos = pred.PredictedPos
                            let w =
                            skillshot.SpellData.RawRadius + (!pred.IsMoving ? minion.BoundingRadius - 15 : 0) -
                            pos.Distance(@from, skillshot.End, true)
                            where w > 0
                            select new DetectedCollision
                            {
                                Position =
                                    pos.ProjectOn(skillshot.End, skillshot.Start).LinePoint + skillshot.Direction * 30,
                                Unit = minion,
                                Type = CollisionObjectTypes.Minion,
                                Distance = pos.Distance(@from),
                                Diff = w,
                            });
                        break;
                    case CollisionObjectTypes.Champions:
                        collisions.AddRange(
                            from hero in GameObjects.AllyHeroes.Where(x => x.IsValidTarget(1200))
                            let pred =
                            FastPrediction(@from, hero,
                                Math.Max(0, skillshot.SpellData.Delay - (Utils.GameTimeTickCount - skillshot.StartTick)),
                                skillshot.SpellData.MissileSpeed)
                            let pos = pred.PredictedPos
                            let w = skillshot.SpellData.RawRadius + 30 - pos.Distance(@from, skillshot.End, true)
                            where w > 0
                            select new DetectedCollision
                            {
                                Position =
                                    pos.ProjectOn(skillshot.End, skillshot.Start).LinePoint + skillshot.Direction * 30,
                                Unit = hero,
                                Type = CollisionObjectTypes.Minion,
                                Distance = pos.Distance(@from),
                                Diff = w,
                            });
                        break;

                    case CollisionObjectTypes.YasuoWall:
                        if (GameObjects.AllyHeroes.All(x => x.CharacterName != "Yasuo"))
                        {
                            break;
                        }

                        GameObject wall = null;

                        foreach (var gameObject in ObjectManager.Get<GameObject>())
                        {
                            if (gameObject.IsValid &&
                                System.Text.RegularExpressions.Regex.IsMatch(
                                    gameObject.Name, "_w_windwall.\\.troy",
                                    System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                            {
                                wall = gameObject;
                            }
                        }

                        if (wall == null)
                        {
                            break;
                        }

                        var level = wall.Name.Substring(wall.Name.Length - 6, 1);
                        var wallWidth = 300 + 50 * Convert.ToInt32(level);
                        var wallDirection = (wall.Position.ToVector2() - YasuoWallCastedPos).Normalized().Perpendicular();
                        var wallStart = wall.Position.ToVector2() + wallWidth / 2 * wallDirection;
                        var wallEnd = wallStart - wallWidth * wallDirection;
                        var wallPolygon = new DaoHungAIO.Evade.Geometry.Polygon.Rectangle(wallStart, wallEnd, 75);
                        var intersection = new Vector2();
                        var intersections = new List<Vector2>();

                        for (var i = 0; i < wallPolygon.Points.Count; i++)
                        {
                            var inter =
                                wallPolygon.Points[i].Intersection(
                                    wallPolygon.Points[i != wallPolygon.Points.Count - 1 ? i + 1 : 0], from,
                                    skillshot.End);
                            if (inter.Intersects)
                            {
                                intersections.Add(inter.Point);
                            }
                        }

                        if (intersections.Count > 0)
                        {
                            intersection = intersections.OrderBy(item => item.Distance(from)).ToList()[0];

                            var collisionT = Utils.GameTimeTickCount +
                                             Math.Max(
                                                 0,
                                                 skillshot.SpellData.Delay -
                                                 (Utils.GameTimeTickCount - skillshot.StartTick)) + 100 +
                                             1000 * intersection.Distance(from) / skillshot.SpellData.MissileSpeed;

                            if (collisionT - WallCastT < 4000)
                            {
                                if (skillshot.SpellData.Type != SkillShotType.SkillshotMissileLine)
                                {
                                    skillshot.ForceDisabled = true;
                                }

                                return intersection;
                            }
                        }
                        break;
                }
            }

            return collisions.Count > 0 ? collisions.OrderBy(c => c.Distance).ToList()[0].Position : new Vector2();
        }

    }
}