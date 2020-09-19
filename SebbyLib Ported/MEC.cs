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
	public static class MEC
	{
		public static void FindMinimalBoundingCircle(List<Vector2> points, out Vector2 center, out float radius)
		{
			var list = MakeConvexHull(points);
			var vector = points[0];
			var num = float.MaxValue;
			for (int i = 0; i < list.Count - 1; i++)
			{
				for (int j = i + 1; j < list.Count; j++)
				{
					var vector2 = new Vector2((list[i].X + list[j].X) / 2f, (list[i].Y + list[j].Y) / 2f);
					var num2 = vector2.X - list[i].X;
					var num3 = vector2.Y - list[i].Y;
					var num4 = num2 * num2 + num3 * num3;
					if (num4 < num && CircleEnclosesPoints(vector2, num4, points, i, j, -1))
					{
						vector = vector2;
						num = num4;
					}
				}
			}
			for (int k = 0; k < list.Count - 2; k++)
			{
				for (int l = k + 1; l < list.Count - 1; l++)
				{
					for (int m = l + 1; m < list.Count; m++)
					{
						Vector2 vector3;
						float num5;
						FindCircle(list[k], list[l], list[m], out vector3, out num5);
						if (num5 < num && CircleEnclosesPoints(vector3, num5, points, k, l, m))
						{
							vector = vector3;
							num = num5;
						}
					}
				}
			}
			center = vector;
			if (num == 3.40282347E+38f)
			{
				radius = 0f;
				return;
			}
			radius = (float)Math.Sqrt((double)num);
		}

		public static MecCircle GetMec(List<Vector2> points)
		{
			var center = default(Vector2);
			float radius;
			FindMinimalBoundingCircle(MakeConvexHull(points), out center, out radius);
			return new MecCircle(center, radius);
		}

		public static List<Vector2> MakeConvexHull(List<Vector2> points)
		{
			points = HullCull(points);
			var best_pt = new Vector2[]
			{
				points[0]
			};
			IEnumerable<Vector2> source = points;
			Func<Vector2, bool> a;
			Func<Vector2, bool> predicate;
			predicate = (a = ((Vector2 pt) => pt.Y < best_pt[0].Y || (pt.Y == best_pt[0].Y && pt.X < best_pt[0].X)));
			foreach (Vector2 vector in source.Where(predicate))
			{
				best_pt[0] = vector;
			}
			var list = new List<Vector2>
			{
				best_pt[0]
			};
			points.Remove(best_pt[0]);
			var num = 0f;
			while (points.Count != 0)
			{
				var x = list[list.Count - 1].X;
				var y = list[list.Count - 1].Y;
				best_pt[0] = points[0];
				var num2 = 3600f;
				foreach (Vector2 vector2 in points)
				{
					var num3 = AngleValue(x, y, vector2.X, vector2.Y);
					if (num3 >= num && num2 > num3)
					{
						num2 = num3;
						best_pt[0] = vector2;
					}
				}
				var num4 = AngleValue(x, y, list[0].X, list[0].Y);
				if (num4 >= num && num2 >= num4)
				{
					break;
				}
				list.Add(best_pt[0]);
				points.Remove(best_pt[0]);
				num = num2;
			}
			return list;
		}

		private static float AngleValue(float x1, float y1, float x2, float y2)
		{
			var num = x2 - x1;
			var num2 = Math.Abs(num);
			var num3 = y2 - y1;
			var num4 = Math.Abs(num3);
			float num5;
			if (num2 + num4 == 0f)
			{
				num5 = 40f;
			}
			else
			{
				num5 = num3 / (num2 + num4);
			}
			if (num < 0f)
			{
				num5 = 2f - num5;
			}
			else if (num3 < 0f)
			{
				num5 = 4f + num5;
			}
			return num5 * 90f;
		}

		private static bool CircleEnclosesPoints(Vector2 center, float radius2, List<Vector2> points, int skip1, int skip2, int skip3)
		{
			return (from point in points.Where((Vector2 t, int i) => i != skip1 && i != skip2 && i != skip3)
					let dx = center.X - point.X
					let dy = center.Y - point.Y
					select dx * dx + dy * dy).All((float test_radius2) => test_radius2 <= radius2);
		}

		private static void FindCircle(Vector2 a, Vector2 b, Vector2 c, out Vector2 center, out float radius2)
		{
			var num = (b.X + a.X) / 2f;
			var num2 = (b.Y + a.Y) / 2f;
			var num3 = b.X - a.X;
			var num4 = -(b.Y - a.Y);
			var num5 = (c.X + b.X) / 2f;
			var num6 = (c.Y + b.Y) / 2f;
			var num7 = c.X - b.X;
			var num8 = -(c.Y - b.Y);
			var num9 = (num2 * num4 * num8 + num5 * num4 * num7 - num * num3 * num8 - num6 * num4 * num8) / (num4 * num7 - num3 * num8);
			var num10 = (num9 - num) * num3 / num4 + num2;
			center = new Vector2(num9, num10);
			var num11 = num9 - a.X;
			var num12 = num10 - a.Y;
			radius2 = num11 * num11 + num12 * num12;
		}

		private static RectangleF GetMinMaxBox(List<Vector2> points)
		{
			var vector = new Vector2(0f, 0f);
			var vector2 = vector;
			var vector3 = vector;
			var vector4 = vector;
			GetMinMaxCorners(points, ref vector, ref vector2, ref vector3, ref vector4);
			var x = vector.X;
			var y = vector.Y;
			var x2 = vector2.X;
			if (y < vector2.Y)
			{
				y = vector2.Y;
			}
			if (x2 > vector4.X)
			{
				x2 = vector4.X;
			}
			float y2 = vector4.Y;
			if (x < vector3.X)
			{
				x = vector3.X;
			}
			if (y2 > vector3.Y)
			{
				y2 = vector3.Y;
			}
			return MinMaxBox = new RectangleF(x, y, x2 - x, y2 - y);
		}

		private static void GetMinMaxCorners(List<Vector2> points, ref Vector2 ul, ref Vector2 ur, ref Vector2 ll, ref Vector2 lr)
		{
			ul = points[0];
			ur = ul;
			ll = ul;
			lr = ul;
			foreach (Vector2 vector in points)
			{
				if (-vector.X - vector.Y > -ul.X - ul.Y)
				{
					ul = vector;
				}
				if (vector.X - vector.Y > ur.X - ur.Y)
				{
					ur = vector;
				}
				if (-vector.X + vector.Y > -ll.X + ll.Y)
				{
					ll = vector;
				}
				if (vector.X + vector.Y > lr.X + lr.Y)
				{
					lr = vector;
				}
			}
			MinMaxCorners = new Vector2[]
			{
				ul,
				ur,
				lr,
				ll
			};
		}

		private static List<Vector2> HullCull(List<Vector2> points)
		{
			var culling_box = GetMinMaxBox(points);
			var list = (from pt in points
								  where pt.X <= culling_box.Left || pt.X >= culling_box.Right || pt.Y <= culling_box.Top || pt.Y >= culling_box.Bottom
								  select pt).ToList<Vector2>();
			CulledPoints = new Vector2[list.Count];
			list.CopyTo(CulledPoints);
			return list;
		}

		public static RectangleF MinMaxBox;

		public static Vector2[] MinMaxCorners;

		public static Vector2[] CulledPoints;

		public struct MecCircle
		{
			public MecCircle(Vector2 center, float radius)
			{
				this.Center = center;
				this.Radius = radius;
			}

			public Vector2 Center;

			public float Radius;
		}
	}
}
