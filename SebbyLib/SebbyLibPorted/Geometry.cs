using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using EnsoulSharp;
using EnsoulSharp.SDK;

namespace SebbyLibPorted
{
    public static class Geometry
    {
		public static Vector2[] CircleCircleIntersection(Vector2 center1, Vector2 center2, float radius1, float radius2)
		{
			var num = center1.Distance(center2);
			if (num > radius1 + radius2 || num <= Math.Abs(radius1 - radius2))
			{
				return new Vector2[0];
			}
			var num2 = (radius1 * radius1 - radius2 * radius2 + num * num) / (2f * num);
			var scale = (float)Math.Sqrt((double)(radius1 * radius1 - num2 * num2));
			var vector = (center2 - center1).Normalized();
			var left = center1 + num2 * vector;
			var vector2 = left + scale * vector.Perpendicular();
			var vector3 = left - scale * vector.Perpendicular();
			return new Vector2[]
			{
				vector2,
				vector3
			};
		}

		public static object[] VectorMovementCollision(Vector2 startPoint1, Vector2 endPoint1, float v1, Vector2 startPoint2, float v2, float delay = 0f)
		{
			var x = startPoint1.X;
			var y = startPoint1.Y;
			var x2 = endPoint1.X;
			var y2 = endPoint1.Y;
			var x3 = startPoint2.X;
			var y3 = startPoint2.Y;
			var num = x2 - x;
			var num2 = y2 - y;
			var num3 = (float)Math.Sqrt((num * num + num2 * num2));
			var num4 = float.NaN;
			var num5 = (Math.Abs(num3) > float.Epsilon) ? (v1 * num / num3) : 0f;
			var num6 = (Math.Abs(num3) > float.Epsilon) ? (v1 * num2 / num3) : 0f;
			var num7 = x3 - x;
			var num8 = y3 - y;
			var num9 = num7 * num7 + num8 * num8;
			if (num3 > 0f)
			{
				if (Math.Abs(v1 - 3.40282347E+38f) < 1.401298E-45f)
				{
					float num10 = num3 / v1;
					num4 = ((v2 * num10 >= 0f) ? num10 : float.NaN);
				}
				else if (Math.Abs(v2 - 3.40282347E+38f) < 1.401298E-45f)
				{
					num4 = 0f;
				}
				else
				{
					var num11 = num5 * num5 + num6 * num6 - v2 * v2;
					var num12 = -num7 * num5 - num8 * num6;
					if (Math.Abs(num11) < 1.401298E-45f)
					{
						if (Math.Abs(num12) < 1.401298E-45f)
						{
							num4 = ((Math.Abs(num9) < float.Epsilon) ? 0f : float.NaN);
						}
						else
						{
							float num13 = -num9 / (2f * num12);
							num4 = ((v2 * num13 >= 0f) ? num13 : float.NaN);
						}
					}
					else
					{
						var num14 = num12 * num12 - num11 * num9;
						if (num14 >= 0f)
						{
							var num15 = (float)Math.Sqrt((double)num14);
							var num16 = (-num15 - num12) / num11;
							num4 = ((v2 * num16 >= 0f) ? num16 : float.NaN);
							num16 = (num15 - num12) / num11;
							float num17 = (v2 * num16 >= 0f) ? num16 : float.NaN;
							if (!float.IsNaN(num17) && !float.IsNaN(num4))
							{
								if (num4 >= delay && num17 >= delay)
								{
									num4 = Math.Min(num4, num17);
								}
								else if (num17 >= delay)
								{
									num4 = num17;
								}
							}
						}
					}
				}
			}
			else if (Math.Abs(num3) < 1.401298E-45f)
			{
				num4 = 0f;
			}
			return new object[]
			{
				num4,
				(!float.IsNaN(num4)) ? new Vector2(x + num5 * num4, y + num6 * num4) : default(Vector2)
			};
		}
	}
}
