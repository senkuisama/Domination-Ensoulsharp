using System;
using System.Collections.Generic;
using System.Linq;
using EnsoulSharp;
using EnsoulSharp.SDK;
using SharpDX;

namespace SebbyLibPorted
{
    public class OktwCommon
    {
        private static AIHeroClient Player { get { return ObjectManager.Player; } }

        private static int LastAATick = Variables.GameTimeTickCount;
        public static bool YasuoInGame = false;
        public static bool Thunderlord = false;

        public static bool
            blockMove = false,
            blockAttack = false,
            blockSpells = false;

        private static List<UnitIncomingDamage> IncomingDamageList = new List<UnitIncomingDamage>();
        private static List<AIHeroClient> ChampionList = new List<AIHeroClient>();
        private static YasuoWall yasuoWall = new YasuoWall();

        static OktwCommon()
        {
            foreach (var hero in ObjectManager.Get<AIHeroClient>())
            {
                ChampionList.Add(hero);
                if (hero.IsEnemy && hero.CharacterName == "Yasuo")
                    YasuoInGame = true;
            }
            AIBaseClient.OnProcessSpellCast += AIBaseClient_OnProcessSpellCast;
            AIBaseClient.OnIssueOrder += AIBaseClient_OnIssueOrder;
            Spellbook.OnCastSpell += Spellbook_OnCastSpell;
            //AIBaseClient.OnDamage += AIBaseClient_OnDamage;
            AIBaseClient.OnDoCast += AIBaseClient_OnDoCast;
            Game.OnWndProc += Game_OnWndProc;
        }

        /*private static void AIBaseClient_OnDamage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        {
            if (sender is AIHeroClient)
            {
                float time = Game.Time - 2;
                IncomingDamageList.RemoveAll(damage => time < damage.Time || ((int)damage.Damage == (int)args.Damage && damage.TargetNetworkId == sender.NetworkId));
            }
        }*/

        public static void debug(string msg)
        {
            if (true)
            {
                //Console.WriteLine(msg);
            }
            /*if (false)
            {
                Game.Print(msg);
            }*/
        }

        public static double GetIncomingDamage(AIHeroClient target, float time = 0.5f, bool skillshots = true)
        {
            double totalDamage = 0;

            foreach (var damage in IncomingDamageList.Where(damage => damage.TargetNetworkId == target.NetworkId && Game.Time - time < damage.Time))
            {
                if (skillshots)
                {
                    totalDamage += damage.Damage;
                }
                else
                {
                    if (!damage.Skillshot)
                        totalDamage += damage.Damage;
                }
            }

            return totalDamage;
        }

        public static bool CanHarras()
        {
            if (!Player.IsWindingUp && !Player.IsUnderEnemyTurret() && Orbwalking.CanMove(50))
                return true;
            else
                return false;
        }
        public static bool ShouldWait()
        {
            var attackCalc = (int)(Player.AttackDelay * 1000);
            return
                Cache.GetMinions(Player.Position, 0).Any(
                    minion => HealthPrediction.LaneClearHealthPrediction(minion, attackCalc, 30) <= Player.GetAutoAttackDamage(minion));
        }


        public static float GetEchoLudenDamage(AIHeroClient target)
        {
            float totalDamage = 0;

            if (Player.HasBuff("itemmagicshankcharge"))
            {
                if (Player.GetBuff("itemmagicshankcharge").Count == 100)
                {
                    totalDamage += (float)Player.CalculateMagicDamage(target, 100 + 0.1 * Player.FlatMagicDamageMod);
                }
            }
            return totalDamage;
        }

        public static bool IsSpellHeroCollision(AIHeroClient t, Spell QWER, int extraWith = 50)
        {
            foreach (var hero in GameObjects.EnemyHeroes.Where(hero => hero.IsValidTarget(QWER.Range + QWER.Width, true, QWER.RangeCheckFrom) && t.NetworkId != hero.NetworkId))
            {
                var prediction = QWER.GetPrediction(hero);
                var powCalc = Math.Pow((QWER.Width + extraWith + hero.BoundingRadius), 2);
                if (prediction.UnitPosition.ToVector2().DistanceSquared(QWER.From.ToVector2(), QWER.GetPrediction(t).CastPosition.ToVector2(), true) <= powCalc)
                {
                    return true;
                }
                else if (prediction.UnitPosition.ToVector2().DistanceSquared(QWER.From.ToVector2(), t.Position.ToVector2(), true) <= powCalc)
                {
                    return true;
                }

            }
            return false;
        }

        public static bool CanHitSkillShot(AIBaseClient target, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (args.Target == null && target.IsValidTarget(float.MaxValue,false))
            {

                var pred = Prediction.Prediction.GetPrediction(target, 0.25f).CastPosition;
                if (pred == null)
                    return false;

                if (args.SData.LineWidth > 0)
                {
                    var powCalc = Math.Pow(args.SData.LineWidth + target.BoundingRadius, 2);
                    if (pred.ToVector2().DistanceSquared(args.End.ToVector2(), args.Start.ToVector2(), true) <= powCalc || target.Position.ToVector2().DistanceSquared(args.End.ToVector2(), args.Start.ToVector2(), true) <= powCalc)
                    {
                        return true;
                    } 
                }
                else if (target.Distance(args.End) < 50 + target.BoundingRadius || pred.Distance(args.End) < 50 + target.BoundingRadius)
                {
                    return true;
                }  
            }
            return false;
        }

        public static float GetKsDamage(AIHeroClient t, Spell QWER)
        {
            var totalDmg = QWER.GetDamage(t);
            totalDmg -= t.HPRegenRate;

            if (totalDmg > t.Health)
            {
                if (Player.HasBuff("summonerexhaust"))
                    totalDmg = totalDmg * 0.6f;

                if (t.HasBuff("ferocioushowl"))
                    totalDmg = totalDmg * 0.7f;

                if (t.CharacterName == "Blitzcrank" && !t.HasBuff("BlitzcrankManaBarrierCD") && !t.HasBuff("ManaBarrier"))
                {
                    totalDmg -= t.Mana / 2f;
                }
            }
            //if (Thunderlord && !Player.HasBuff( "masterylordsdecreecooldown"))
            //totalDmg += (float)Player.CalcDamage(t, Damage.DamageType.Magical, 10 * Player.Level + 0.1 * Player.FlatMagicDamageMod + 0.3 * Player.FlatPhysicalDamageMod);
            totalDmg += (float)GetIncomingDamage(t);
            return totalDmg;
        }

        public static bool ValidUlt(AIHeroClient target)
        {
            if (target.HasBuffOfType(BuffType.PhysicalImmunity) || target.HasBuffOfType(BuffType.SpellImmunity)
                || target.IsInvulnerable || target.HasBuffOfType(BuffType.Invulnerability) || target.HasBuff("kindredrnodeathbuff")
                || target.HasBuffOfType(BuffType.SpellShield) || target.Health - GetIncomingDamage(target) < 1)
                return false;
            else
                return true;
        }

        public static bool CanMove(AIHeroClient target)
        {
            if (target.MoveSpeed < 50 || target.IsStunned || target.HasBuffOfType(BuffType.Stun) || target.HasBuffOfType(BuffType.Fear) || target.HasBuffOfType(BuffType.Snare) || target.HasBuffOfType(BuffType.Knockup) || target.HasBuff("Recall") ||
                target.HasBuffOfType(BuffType.Knockback) || target.HasBuffOfType(BuffType.Charm) || target.HasBuffOfType(BuffType.Taunt) || target.HasBuffOfType(BuffType.Suppression) || (!target.IsMoving))
            {
                return false;
            }
            else
                return true;
        }

        public static int GetBuffCount(AIBaseClient target, string buffName)
        {
            foreach (var buff in target.Buffs.Where(buff => buff.Name.ToLower() == buffName.ToLower()))
            {
                if (buff.Count == 0)
                    return 1;
                else
                    return buff.Count;
            }
            return 0;
        }

        public static int CountEnemyMinions(AIBaseClient target, float range)
        {
            var allMinions = Cache.GetMinions(target.Position, range);
            if (allMinions != null)
                return allMinions.Count;
            else
                return 0;
        }

        public static float GetPassiveTime(AIBaseClient target, string buffName)
        {
            return
                target.Buffs.OrderByDescending(buff => buff.EndTime - Game.Time)
                    .Where(buff => buff.Name.ToLower() == buffName.ToLower())
                    .Select(buff => buff.EndTime)
                    .FirstOrDefault() - Game.Time;
        }

        public static Vector3 GetTrapPos(float range)
        {
            foreach (var enemy in GameObjects.EnemyHeroes.Where(enemy => enemy.IsValid && enemy.Distance(Player.Position) < range && (enemy.HasBuff("zhonyasringshield") || enemy.HasBuff("BardRStasis"))))
            {
                return enemy.Position;
            }

            foreach (var obj in ObjectManager.Get<EffectEmitter>().Where(obj => obj.IsValid && obj.Position.Distance(Player.Position) < range ))
            {
                var name = obj.Name.ToLower();
                
                if (name.Contains("GateMarker_red.troy".ToLower()) || name.Contains("global_ss_teleport_target_red.troy".ToLower())
                    || name.Contains("R_indicator_red.troy".ToLower()))
                    return obj.Position;
            }

            return Vector3.Zero;
        }

        public static bool IsMovingInSameDirection(AIBaseClient source, AIBaseClient target)
        {
            var sourceLW = source.GetWaypoints().Last().ToVector3();

            if (sourceLW == source.Position || !source.IsMoving)
                return false;

            var targetLW = target.GetWaypoints().Last().ToVector3();

            if (targetLW == target.Position || !target.IsMoving)
                return false;

            Vector2 pos1 = sourceLW.ToVector2() - source.Position.ToVector2();
            Vector2 pos2 = targetLW.ToVector2() - target.Position.ToVector2();
            var getAngle = pos1.AngleBetween(pos2);

            if(getAngle < 25)
                return true;
            else
                return false;
        }

        public static bool CollisionYasuo(Vector3 from, Vector3 to)
        {
            if (!YasuoInGame)
                return false;

            if (Game.Time - yasuoWall.CastTime > 4)
                return false;

            var level = yasuoWall.WallLvl;
            var wallWidth = (350 + 50 * level);
            var wallDirection = (yasuoWall.CastPosition.ToVector2() - yasuoWall.YasuoPosition.ToVector2()).Normalized().Perpendicular();
            var wallStart = yasuoWall.CastPosition.ToVector2() + wallWidth / 2f * wallDirection;
            var wallEnd = wallStart - wallWidth * wallDirection;

            if (wallStart.Intersection(wallEnd, to.ToVector2(), from.ToVector2()).Intersects)
            {
                return true;
            }
            return false;
            
        }

        public static void DrawTriangleOKTW(float radius, Vector3 position, System.Drawing.Color color, float bold = 1)
        {
            var positionV2 = Drawing.WorldToScreen(position);
            Vector2 a = new Vector2(positionV2.X + radius, positionV2.Y + radius / 2);
            Vector2 b = new Vector2(positionV2.X - radius, positionV2.Y + radius / 2);
            Vector2 c = new Vector2(positionV2.X, positionV2.Y - radius);
            Drawing.DrawLine(a[0], a[1], b[0], b[1], bold, color);
            Drawing.DrawLine(b[0], b[1], c[0], c[1], bold, color);
            Drawing.DrawLine(c[0], c[1], a[0], a[1], bold, color);
        }

        public static void DrawLineRectangle(Vector3 start2, Vector3 end2, int radius, float width, System.Drawing.Color color)
        {
            Vector2 start = start2.ToVector2();
            Vector2 end = end2.ToVector2();
            var dir = (end - start).Normalized();
            var pDir = dir.Perpendicular();

            var rightStartPos = start + pDir * radius;
            var leftStartPos = start - pDir * radius;
            var rightEndPos = end + pDir * radius;
            var leftEndPos = end - pDir * radius;

            var rStartPos = Drawing.WorldToScreen(new Vector3(rightStartPos.X, rightStartPos.Y, ObjectManager.Player.Position.Z));
            var lStartPos = Drawing.WorldToScreen(new Vector3(leftStartPos.X, leftStartPos.Y, ObjectManager.Player.Position.Z));
            var rEndPos = Drawing.WorldToScreen(new Vector3(rightEndPos.X, rightEndPos.Y, ObjectManager.Player.Position.Z));
            var lEndPos = Drawing.WorldToScreen(new Vector3(leftEndPos.X, leftEndPos.Y, ObjectManager.Player.Position.Z));

            Drawing.DrawLine(rStartPos, rEndPos, width, color);
            Drawing.DrawLine(lStartPos, lEndPos, width, color);
            Drawing.DrawLine(rStartPos, lStartPos, width, color);
            Drawing.DrawLine(lEndPos, rEndPos, width, color);
        }

        public static List<Vector3> CirclePoints(float CircleLineSegmentN, float radius, Vector3 position)
        {
            List<Vector3> points = new List<Vector3>();
            for (var i = 1; i <= CircleLineSegmentN; i++)
            {
                var angle = i * 2 * Math.PI / CircleLineSegmentN;
                var point = new Vector3(position.X + radius * (float)Math.Cos(angle), position.Y + radius * (float)Math.Sin(angle), position.Z);
                points.Add(point);
            }
            return points;
        }

        private static void Game_OnWndProc(GameWndEventArgs args)
        {
            if (args.Msg == 123 && blockMove)
            {
                blockMove = false;
                blockAttack = false;
                Orbwalking.Attack = true;
                Orbwalking.Move = true;
                Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
            }
        }

        private static void AIBaseClient_OnDoCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (args.Target != null && args.SData != null)
            {
                if (args.Target.Type == GameObjectType.AIHeroClient && !sender.IsMelee && args.Target.Team != sender.Team)
                {
                    IncomingDamageList.Add(new UnitIncomingDamage { Damage = new Spell(sender.GetSpellSlot(args.SData.Name), float.MaxValue).GetDamage(args.Target as AIBaseClient), TargetNetworkId = (int)args.Target.NetworkId, Time = Game.Time, Skillshot = false });
                }
            }
        }

        private static void AIBaseClient_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (args.SData == null)
            {
                return;
            }
            /////////////////  HP prediction
            var targed = args.Target as AIBaseClient;
            
            if (targed != null)
            {
                if (targed.Type == GameObjectType.AIHeroClient && targed.Team != sender.Team && sender.IsMelee)
                {
                    IncomingDamageList.Add(new UnitIncomingDamage { Damage = new Spell(sender.GetSpellSlot(args.SData.Name), float.MaxValue).GetDamage(targed), TargetNetworkId = (int)args.Target.NetworkId, Time = Game.Time, Skillshot = false });
                }
            }
            else
            {
                foreach (var champion in ChampionList.Where(champion => !champion.IsDead && champion.IsVisible && champion.Team != sender.Team && champion.Distance(sender) < 2000))
                {
                    if (CanHitSkillShot(champion,args))
                    {
                        IncomingDamageList.Add(new UnitIncomingDamage { Damage = new Spell(sender.GetSpellSlot(args.SData.Name), float.MaxValue).GetDamage(champion), TargetNetworkId = (int)champion.NetworkId, Time = Game.Time, Skillshot = true });
                    }
                }

                if (!YasuoInGame)
                    return;

                if (sender.IsAlly == true || sender.IsMinion() || Orbwalker.IsAutoAttack(args.SData.Name) || sender.Type != GameObjectType.AIHeroClient)
                    return;

                if (args.SData.Name == "YasuoWMovingWall")
                {
                    yasuoWall.CastTime = Game.Time;
                    yasuoWall.CastPosition = sender.Position.Extend(args.End, 400);
                    yasuoWall.YasuoPosition = sender.Position;
                    yasuoWall.WallLvl = sender.Spellbook.Spells[1].Level;
                }
            }
        }

        private static void Spellbook_OnCastSpell(Spellbook sender, SpellbookCastSpellEventArgs args)
        {
            if (blockSpells)
            {
                args.Process = false;
            }
        }

        private static void AIBaseClient_OnIssueOrder(AIBaseClient sender, AIBaseClientIssueOrderEventArgs args)
        {
            if (!sender.IsMe)
                return;

            if (blockMove && args.Order != GameObjectOrder.AttackUnit)
            {
                args.Process = false;
            }
            if (blockAttack && args.Order == GameObjectOrder.AttackUnit)
            {
                args.Process = false;
            }
        }

    }

    class UnitIncomingDamage
    {
        public int TargetNetworkId { get; set; }
        public float Time { get; set; }
        public double Damage { get; set; }
        public bool Skillshot { get; set; }
    }

    class YasuoWall
    {
        public Vector3 YasuoPosition { get; set; }
        public float CastTime { get; set; }
        public Vector3 CastPosition { get; set; }
        public float WallLvl { get; set; }

        public YasuoWall()
        {
            CastTime = 0;
        }
    }
}
