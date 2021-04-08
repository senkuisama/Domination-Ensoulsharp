using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SPredictionMash;
using EnsoulSharp.SDK;
using SharpDX;
using EnsoulSharp;
using EnsoulSharp.SDK.MenuUI;

namespace e.Motion_Gangplank
{
    public static class Helper
    {
        private const int QDELAY = 300;
        public static Vector2 PredPos;
        public static int GetQTime(Vector3 position)
        {
            return (int)(Program.Player.Distance(position) / 2.6f + QDELAY + 
                Game.Ping/2
                );
        }

        public static bool GetPredPos(AIHeroClient enemy, bool additionalReactionTime = false, bool additionalBarrelTime = false)
        {
            PredPos = SPredictionMash.Prediction.GetFastUnitPosition(enemy, Config.Menu["Miscellanious"]["misc.enemyReactionTime"].GetValue<MenuSlider>().Value);
            float reactionDistance = Config.Menu["Miscellanious"]["misc.enemyReactionTime"].GetValue<MenuSlider>().Value  +  (additionalReactionTime? Config.Menu["Miscellanious"]["misc.additionalReactionTime"].GetValue<MenuSlider>().Value : 0) * enemy.MoveSpeed*0.001f;
            if (PredPos.Distance(enemy) > reactionDistance)
            {
                PredPos = enemy.Position.Extend(PredPos.ToVector3(), reactionDistance).ToVector2();
            }
            return true;
        }
        public static bool CannotEscapeFromAA(this Vector3 kegPosition,AIHeroClient enemy)
        {
            GetPredPos(enemy);
            return enemy.Position.Distance(kegPosition) <= 325 && PredPos.Distance(kegPosition) < 325 - enemy.MoveSpeed * Program.Player.AttackCastDelay * 0.8f;
        }

        public static bool CannotEscape(this Vector3 kegPosition, AIHeroClient enemy, bool additionalReactionTime = false, bool additionalBarrelTime = false)
        {
            GetPredPos(enemy, additionalReactionTime, additionalBarrelTime);
            float fac = 0.0008f  * enemy.MoveSpeed * (GetQTime(kegPosition) + (additionalBarrelTime ? 400 : 0) - (additionalReactionTime ? Config.Menu["Miscellanious"]["misc.additionalReactionTime"].GetValue<MenuSlider>().Value : 0) - Config.Menu["Miscellanious"]["misc.enemyReactionTime"].GetValue<MenuSlider>().Value);
            //if (enemy.Position.Distance(kegPosition) <= 325 && PredPos.Distance(kegPosition) < 325 - enemy.MoveSpeed*(GetQTime(kegPosition) + (additionalBarrelTime ? 400 : 0) - (additionalReactionTime ? Config.Item("misc.additionalReactionTime").GetValue<Slider>().Value : 0) - Config.Item("misc.enemyReactionTime").GetValue<Slider>().Value) * 0.00095f)
            //Chat.Print(fac.ToString());
            if (enemy.Position.Distance(kegPosition) <= 325 && PredPos.Distance(kegPosition) < 325 - fac)
            {
                
                //Chat.Print("Distance:" + PredPos.Distance(kegPosition));
                //Chat.Print("Max Distance:" + (400 - enemy.MoveSpeed * (GetQTime(kegPosition) + (additionalBarrelTime ? 400 : 0) - (additionalReactionTime ? Config.Item("misc.additionalReactionTime").GetValue<Slider>().Value : 0) - Config.Item("misc.reactionTime").GetValue<Slider>().Value) * 0.00095f));
                return true;
            }
            return false;
        }

        public static Vector3 ExtendToMaxRange(this Vector3 startPosition, Vector3 endPosition, float maxrange)
        {
            return startPosition.Extend(endPosition, Math.Min(startPosition.Distance(endPosition), maxrange));
        }

        public static List<Vector2> IntersectCircles(Vector2 posA, float distA, Vector2 posB, float distB)
        {
            // A, B = [ x, y ]
            // return = [ Q1, Q2 ] or [ Q ] or [] where Q = [ x, y ]
            var ab0 = posB.X - posA.X;
            var ab1 = posB.Y - posA.Y;
            var c = Math.Sqrt(ab0 * ab0 + ab1 * ab1);
            if (c == 0)
            {
                // no distance between centers
                return new List<Vector2>();
            }
            var x = (distA * distA + c * c - distB * distB) / (2 * c);
            var y = distA * distA - x * x;
            if (y < 0)
            {
                // no intersection
                return new List<Vector2>();
            }
            if (y > 0)
            {
                y = Math.Sqrt(y);
            }
            // compute unit vectors ex and ey
            var ex0 = ab0 / c;
            var ex1 = ab1 / c;
            var ey0 = -ex1;
            var ey1 = ex0;
            var q1x = (float)(posA.X + x * ex0);
            var q1y = (float)(posA.Y + x * ex1);
            if (y == 0)
            {
                // one touch point
                return new List<Vector2> {new Vector2(q1x, q1y)};
            }
            // two intersections
            var q2x = (float)(q1x - y * ey0);
            var q2y = (float)(q1y - y * ey1);
            q1x += (float)(y * ey0);
            q1y += (float)(y * ey1);
            return new List<Vector2> { new Vector2(q1x, q1y), new Vector2(q2x, q2y)};
        }
    }
}
