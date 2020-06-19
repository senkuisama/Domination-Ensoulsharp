using DominationAIO.Champions.Helper;
using EnsoulSharp;
using EnsoulSharp.SDK;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PRADA_Vayne.MyUtils
{
    public static class Extensions
    {
        private static readonly AIHeroClient Player = ObjectManager.Player;

        public static bool IsCondemnable(this AIHeroClient hero)
        {
            if (hero == null) return false;
            if (!hero.IsValidTarget(550f) || hero.HasBuffOfType(BuffType.SpellShield) ||
                hero.HasBuffOfType(BuffType.SpellImmunity) || hero.IsDashing()) return false;

            //values for pred calc pP = player position; p = enemy position; pD = push distance
            var pP = Heroes.Player.Position;
            var p = hero.Position;
            var pD = Program.MainMenu.GetMenuSlider("Combo Settings", "EPushDist");
            var mode = Program.MainMenu.GetMenuList("Combo Settings", "EMode");

            if (mode == "PRADASMART" && (p.Extend(pP, -pD).IsCollisionable() ||
                                         p.Extend(pP, -pD / 2f).IsCollisionable() ||
                                         p.Extend(pP, -pD / 3f).IsCollisionable()))
            {
                if (!hero.CanMove || hero.Spellbook.IsAutoAttack)
                    return true;

                var enemiesCount = ObjectManager.Player.CountEnemyHeroesInRange(1200);
                if (enemiesCount > 1 && enemiesCount <= 3)
                {
                    var prediction = Program.E.GetPrediction(hero);
                    for (var i = 15; i < pD; i += 75)
                    {
                        var posFlags =
                            NavMesh.GetCollisionFlags(prediction.UnitPosition.ToVector2().Extend(pP.ToVector2(), -i).ToVector3());
                        if (posFlags.HasFlag(CollisionFlags.Wall) || posFlags.HasFlag(CollisionFlags.Building))
                            return true;
                    }

                    return false;
                }

                var hitchance = Program.MainMenu.GetMenuSlider("Combo Settings", "EHitchance");
                var angle = 0.20 * hitchance;
                const float travelDistance = 0.5f;
                var alpha = new Vector2((float)(p.X + travelDistance * Math.Cos(Math.PI / 180 * angle)),
                    (float)(p.X + travelDistance * Math.Sin(Math.PI / 180 * angle)));
                var beta = new Vector2((float)(p.X - travelDistance * Math.Cos(Math.PI / 180 * angle)),
                    (float)(p.X - travelDistance * Math.Sin(Math.PI / 180 * angle)));

                for (var i = 15; i < pD; i += 100)
                    if (pP.ToVector2().Extend(alpha, i).ToVector3().IsCollisionable() &&
                        pP.ToVector2().Extend(beta, i).ToVector3().IsCollisionable())
                        return true;
                return false;
            }

            if (mode == "PRADAPERFECT" && (p.Extend(pP, -pD).IsCollisionable() ||
                                           p.Extend(pP, -pD / 2f).IsCollisionable() ||
                                           p.Extend(pP, -pD / 3f).IsCollisionable()))
            {
                if (!hero.CanMove || hero.Spellbook.IsAutoAttack)
                    return true;

                var hitchance = Program.MainMenu.GetMenuSlider("Combo Settings", "EHitchance");
                var angle = 0.20 * hitchance;
                const float travelDistance = 0.5f;
                var alpha = new Vector2((float)(p.X + travelDistance * Math.Cos(Math.PI / 180 * angle)),
                    (float)(p.X + travelDistance * Math.Sin(Math.PI / 180 * angle)));
                var beta = new Vector2((float)(p.X - travelDistance * Math.Cos(Math.PI / 180 * angle)),
                    (float)(p.X - travelDistance * Math.Sin(Math.PI / 180 * angle)));

                for (var i = 15; i < pD; i += 100)
                    if (pP.ToVector2().Extend(alpha, i).ToVector3().IsCollisionable() &&
                        pP.ToVector2().Extend(beta, i).ToVector3().IsCollisionable())
                        return true;
                return false;
            }

            if (mode == "OLDPRADA")
            {
                if (!hero.CanMove || hero.Spellbook.IsAutoAttack)
                    return true;

                var hitchance = Program.MainMenu.GetMenuSlider("Combo Settings", "EHitchance");
                var angle = 0.20 * hitchance;
                const float travelDistance = 0.5f;
                var alpha = new Vector2((float)(p.X + travelDistance * Math.Cos(Math.PI / 180 * angle)),
                    (float)(p.X + travelDistance * Math.Sin(Math.PI / 180 * angle)));
                var beta = new Vector2((float)(p.X - travelDistance * Math.Cos(Math.PI / 180 * angle)),
                    (float)(p.X - travelDistance * Math.Sin(Math.PI / 180 * angle)));

                for (var i = 15; i < pD; i += 100)
                    if (pP.ToVector2().Extend(alpha, i).ToVector3().IsCollisionable() ||
                        pP.ToVector2().Extend(beta, i).ToVector3().IsCollisionable())
                        return true;
                return false;
            }

            if (mode == "MARKSMAN")
            {
                var prediction = Program.E.GetPrediction(hero);
                return NavMesh.GetCollisionFlags(prediction.UnitPosition.ToVector2().Extend(pP.ToVector2(), -pD).ToVector3())
                           .HasFlag(CollisionFlags.Wall) || NavMesh
                           .GetCollisionFlags(prediction.UnitPosition.ToVector2().Extend(pP.ToVector2(), -pD / 2f).ToVector3())
                           .HasFlag(CollisionFlags.Wall);
            }

            if (mode == "SHARPSHOOTER")
            {
                var prediction = Program.E.GetPrediction(hero);
                for (var i = 15; i < pD; i += 100)
                {
                    var posCF = NavMesh.GetCollisionFlags(prediction.UnitPosition.ToVector2().Extend(pP.ToVector2(), -i).ToVector3());
                    if (posCF.HasFlag(CollisionFlags.Wall) || posCF.HasFlag(CollisionFlags.Building))
                        return true;
                }

                return false;
            }

            if (mode == "GOSU")
            {
                var prediction = Program.E.GetPrediction(hero);
                for (var i = 15; i < pD; i += 75)
                {
                    var posCF = NavMesh.GetCollisionFlags(prediction.UnitPosition.ToVector2().Extend(pP.ToVector2(), -i).ToVector3());
                    if (posCF.HasFlag(CollisionFlags.Wall) || posCF.HasFlag(CollisionFlags.Building))
                        return true;
                }

                return false;
            }

            if (mode == "VHR")
            {
                var prediction = Program.E.GetPrediction(hero);
                for (var i = 15; i < pD; i += (int)hero.BoundingRadius) //:frosty:
                {
                    var posCF = NavMesh.GetCollisionFlags(prediction.UnitPosition.ToVector2().Extend(pP.ToVector2(), -i).ToVector3());
                    if (posCF.HasFlag(CollisionFlags.Wall) || posCF.HasFlag(CollisionFlags.Building))
                        return true;
                }

                return false;
            }

            if (mode == "PRADALEGACY")
            {
                var prediction = Program.E.GetPrediction(hero);
                for (var i = 15; i < pD; i += 75)
                {
                    var posCF = NavMesh.GetCollisionFlags(prediction.UnitPosition.ToVector2().Extend(pP.ToVector2(), -i).ToVector3());
                    if (posCF.HasFlag(CollisionFlags.Wall) || posCF.HasFlag(CollisionFlags.Building))
                        return true;
                }

                return false;
            }

            if (mode == "FASTEST" && (p.Extend(pP, -pD).IsCollisionable() || p.Extend(pP, -pD / 2f).IsCollisionable() ||
                                      p.Extend(pP, -pD / 3f).IsCollisionable()))
                return true;

            var j4 = ObjectManager.Get<AIHeroClient>().FirstOrDefault(h =>
                h.IsEnemy && h.CharacterName == "JarvanIV" && h.Distance(ObjectManager.Player) < 800);
            if (j4 != null)
            {
                if (ObjectManager.Get<AIMinionClient>().Any(m =>
                    m.CharacterName == "jarvanivwall" && m.Distance((Vector2)pP.Extend(p, 425)) < 100))
                    return true;
                if (!j4.IsDead && ObjectManager.Get<AIMinionClient>().Any(m =>
                        m.CharacterName == "jarvanivwall" && m.Distance(j4.Position.Extend(p, 425)) < 100))
                    Program.E.Cast(j4);
            }

            return false;
        }

        public static Vector3 GetTumblePos(this AIBaseClient target)
        {
            if (target == null) return Vector3.Zero;
            //if the target is not a melee and he's alone he's not really a danger to us, proceed to 1v1 him :^ )
            if (!target.IsMelee && Heroes.Player.CountEnemyHeroesInRange(800) == 1) return Game.CursorPos;

            var aRC = new Geometry.Circle(Heroes.Player.Position.ToVector2(), 300).ToPolygon().ToClipperPath();
            var cursorPos = Game.CursorPos;
            var targetPosition = target.Position;
            var pList = new List<Vector3>();
            var additionalDistance = (0.106 + Game.Ping / 2000f) * target.MoveSpeed;

            if (!cursorPos.IsDangerousPosition()) return cursorPos;

            foreach (var p in aRC)
            {
                var v3 = new Vector2(p.X, p.Y).ToVector3();

                if (target.IsFacing(Heroes.Player))
                {
                    if (!v3.IsDangerousPosition() && v3.Distance(targetPosition) < 530) pList.Add(v3);
                }
                else
                {
                    if (!v3.IsDangerousPosition() && v3.Distance(targetPosition) < 530 - additionalDistance)
                        pList.Add(v3);
                }
            }

            if (Heroes.Player.CountEnemyHeroesInRange(800) == 1)
                return pList.Count > 1 ? pList.OrderBy(el => el.Distance(cursorPos)).FirstOrDefault() : Vector3.Zero;
            if (Program.MainMenu.GetMenuList("Combo Settings", "QOrderBy") == "CLOSETOTARGET")
                return pList.Count > 1
                    ? pList.OrderBy(el => el.Distance(targetPosition)).FirstOrDefault()
                    : Vector3.Zero;
            return pList.Count > 1
                ? pList.OrderByDescending(el => el.Distance(cursorPos)).FirstOrDefault()
                : Vector3.Zero;
        }

        public static int VayneWStacks(this AIBaseClient o)
        {
            if (o == null) return 0;
            var buffs = o.Buffs;
            if (buffs == null) return 0;
            if (!buffs.Any(b => b.Name.ToLower().Equals("vaynesilvereddebuff"))) return 0;
            if (buffs.FirstOrDefault(b => b.Name.ToLower().Equals("vaynesilvereddebuff")) == null) return 0;
            return buffs.FirstOrDefault(b => b.Name.ToLower().Equals("vaynesilvereddebuff")).Count;
        }

        public static Vector3 Randomize(this Vector3 pos)
        {
            var r = new Random(Environment.TickCount);
            return new Vector2(pos.X + r.Next(-150, 150), pos.Y + r.Next(-150, 150)).ToVector3();
        }

        public static bool IsDangerousPosition(this Vector3 pos)
        {
            return GameObjects.EnemyHeroes.Any(e =>
                       e.IsValidTarget(1000) && e.IsVisible &&
                       e.Distance(pos) < Program.MainMenu.GetMenuSlider("Combo Settings", "QMinDist")) ||
                   Traps.EnemyTraps.Any(t => pos.Distance(t.Position) < 125) && pos.IsWall();
        }

        public static bool IsKillable(this AIHeroClient hero)
        {
            return Player.GetAutoAttackDamage(hero) * 2 < hero.Health;
        }

        public static bool IsCollisionable(this Vector3 pos)
        {
            return NavMesh.GetCollisionFlags(pos).HasFlag(CollisionFlags.Wall) ||
                   Orbwalker.ActiveMode == OrbwalkerMode.Combo &&
                   NavMesh.GetCollisionFlags(pos).HasFlag(CollisionFlags.Building);
        }

        public static bool IsValidState(this AIHeroClient target)
        {
            return !target.HasBuffOfType(BuffType.SpellShield) && !target.HasBuffOfType(BuffType.SpellImmunity) &&
                   !target.HasBuffOfType(BuffType.Invulnerability);
        }

        public static int CountHerosInRange(this AIHeroClient target, bool checkteam, float range = 1200f)
        {
            var objListTeam = ObjectManager.Get<AIHeroClient>().Where(x => x.IsValidTarget(range, false));
            return objListTeam.Count(hero => checkteam ? hero.Team != target.Team : hero.Team == target.Team);
        }
    }
}