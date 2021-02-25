using System;
using System.Collections.Generic;
using System.Linq;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Utility;
using SharpDX;

namespace FSpred.Prediction
{
	public static class AoePrediction
	{
		public static PredictionOutput GetPrediction(PredictionInput input)
		{
			switch (input.Type)
			{
			case SkillshotType.SkillshotLine:
				return AoePrediction.Line.GetPrediction(input);
			case SkillshotType.SkillshotCircle:
				return AoePrediction.Circle.GetPrediction(input);
			case SkillshotType.SkillshotCone:
				return AoePrediction.Cone.GetPrediction(input);
			default:
				return new PredictionOutput();
			}
		}

		internal static List<AoePrediction.PossibleTarget> GetPossibleTargets(PredictionInput input)
		{
			List<AoePrediction.PossibleTarget> list = new List<AoePrediction.PossibleTarget>();
			var originalUnit = input.Unit;
			var heroes = ObjectManager.Get<AIHeroClient>().Where(i => !i.IsAlly).ToList();
			Predicate<AIBaseClient> mach;
			Predicate<AIBaseClient> match;
			match = (mach = ((AIBaseClient h) => h.NetworkId != originalUnit.NetworkId && h.IsValidTarget(input.Range + 200f + input.RealRadius, true, input.RangeCheckFrom)));
			foreach (var unit in heroes.FindAll(match))
			{
				input.Unit = unit;
				PredictionOutput prediction = Prediction.GetPrediction(input, false, false);
				if (prediction.Hitchance >= HitChance.High)
				{
					list.Add(new AoePrediction.PossibleTarget
					{
						Position = prediction.UnitPosition.ToVector2(),
						Unit = unit
					});
				}
			}
			return list;
		}

		public static class Circle
		{
			public static PredictionOutput GetPrediction(PredictionInput input)
			{
				PredictionOutput prediction = Prediction.GetPrediction(input, false, true);
				List<AoePrediction.PossibleTarget> list = new List<AoePrediction.PossibleTarget>
				{
					new AoePrediction.PossibleTarget
					{
						Position = prediction.UnitPosition.ToVector2(),
						Unit = input.Unit
					}
				};
				if (prediction.Hitchance >= HitChance.Medium)
				{
					list.AddRange(AoePrediction.GetPossibleTargets(input));
				}
				while (list.Count > 1)
				{
					var mec = Mec.GetMec((from h in list
					select h.Position).ToList<Vector2>());
					if (mec.Radius <= input.RealRadius - 10f && Vector2.DistanceSquared(mec.Center, input.RangeCheckFrom.ToVector2()) < input.Range * input.Range)
					{
						PredictionOutput predictionOutput = new PredictionOutput();
						predictionOutput.AoeTargetsHit = (from hero in list
														 select hero.Unit).ToList<AIBaseClient>();
						predictionOutput.CastPosition = mec.Center.ToVector3();
						predictionOutput.UnitPosition = prediction.UnitPosition;
						predictionOutput.Hitchance = prediction.Hitchance;
						predictionOutput.Input = input;
						predictionOutput._aoeTargetsHitCount = list.Count;
						return predictionOutput;
					}
					float num = -1f;
					int index = 1;
					for (int i = 1; i < list.Count; i++)
					{
						float num2 = Vector2.DistanceSquared(list[i].Position, list[0].Position);
						if (num2 > num || num.CompareTo(-1f) == 0)
						{
							index = i;
							num = num2;
						}
					}
					list.RemoveAt(index);
				}
				return prediction;
			}
		}

		public static class Cone
		{
			public static PredictionOutput GetPrediction(PredictionInput input)
			{
				PredictionOutput prediction = Prediction.GetPrediction(input, false, true);
				List<AoePrediction.PossibleTarget> list = new List<AoePrediction.PossibleTarget>
				{
					new AoePrediction.PossibleTarget
					{
						Position = prediction.UnitPosition.ToVector2(),
						Unit = input.Unit
					}
				};
				if (prediction.Hitchance >= HitChance.Medium)
				{
					list.AddRange(AoePrediction.GetPossibleTargets(input));
				}
				if (list.Count > 1)
				{
					List<Vector2> list2 = new List<Vector2>();
					foreach (AoePrediction.PossibleTarget possibleTarget in list)
					{
						possibleTarget.Position -= input.From.ToVector2();
					}
					for (int i = 0; i < list.Count; i++)
					{
						for (int j = 0; j < list.Count; j++)
						{
							if (i != j)
							{
								Vector2 item = (list[i].Position + list[j].Position) * 0.5f;
								if (!list2.Contains(item))
								{
									list2.Add(item);
								}
							}
						}
					}
					int num = -1;
					Vector2 vector = default(Vector2);
					List<Vector2> points = (from t in list
					select t.Position).ToList<Vector2>();
					foreach (Vector2 vector2 in list2)
					{
						int hits = AoePrediction.Cone.GetHits(vector2, (double)input.Range, input.Radius, points);
						if (hits > num)
						{
							vector = vector2;
							num = hits;
						}
					}
					vector += input.From.ToVector2();
					if (num > 1 && input.From.ToVector2().DistanceSquared(vector) > 2500f)
					{
						return new PredictionOutput
						{
							Hitchance = prediction.Hitchance,
							_aoeTargetsHitCount = num,
							UnitPosition = prediction.UnitPosition,
							CastPosition = vector.ToVector3(),
							Input = input
						};
					}
				}
				return prediction;
			}

			internal static int GetHits(Vector2 end, double range, float angle, List<Vector2> points)
			{
				return (from point in points
				let edge1 = end.Rotated(-angle / 2f)
				let edge2 = edge1.Rotated(angle)
				where (double)point.DistanceSquared(default(Vector2)) < range * range && edge1.CrossProduct(point) > 0f && point.CrossProduct(edge2) > 0f
				select point).Count<Vector2>();
			}
		}

		public static class Line
		{
			public static PredictionOutput GetPrediction(PredictionInput input)
			{
				PredictionOutput prediction = Prediction.GetPrediction(input, false, true);
				List<AoePrediction.PossibleTarget> list = new List<AoePrediction.PossibleTarget>
				{
					new AoePrediction.PossibleTarget
					{
						Position = prediction.UnitPosition.ToVector2(),
						Unit = input.Unit
					}
				};
				if (prediction.Hitchance >= HitChance.Medium)
				{
					list.AddRange(AoePrediction.GetPossibleTargets(input));
				}
				if (list.Count > 1)
				{
					List<Vector2> list2 = new List<Vector2>();
					foreach (AoePrediction.PossibleTarget possibleTarget in list)
					{
						Vector2[] candidates = AoePrediction.Line.GetCandidates(input.From.ToVector2(), possibleTarget.Position, input.Radius, input.Range);
						list2.AddRange(candidates);
					}
					var num = -1;
					var vector = default(Vector2);
					List<Vector2> list3 = new List<Vector2>();
					List<Vector2> list4 = (from t in list
					select t.Position).ToList<Vector2>();
					foreach (Vector2 vector2 in list2)
					{
						if (AoePrediction.Line.GetHits(input.From.ToVector2(), vector2, (double)(input.Radius + input.Unit.BoundingRadius / 3f - 10f), new List<Vector2>
						{
							list[0].Position
						}).Count<Vector2>() == 1)
						{
							List<Vector2> list5 = AoePrediction.Line.GetHits(input.From.ToVector2(), vector2, (double)input.Radius, list4).ToList<Vector2>();
							int count = list5.Count;
							if (count >= num)
							{
								num = count;
								vector = vector2;
								list3 = list5.ToList<Vector2>();
							}
						}
					}
					if (num > 1)
					{
						float num2 = -1f;
						Vector2 left = default(Vector2);
						Vector2 right = default(Vector2);
						for (int i = 0; i < list3.Count; i++)
						{
							for (int j = 0; j < list3.Count; j++)
							{
								Vector2 segmentStart = input.From.ToVector2();
								Vector2 segmentEnd = vector;
								var projectionInfo = list4[i].ProjectOn(segmentStart, segmentEnd);
								var projectionInfo2 = list4[j].ProjectOn(segmentStart, segmentEnd);
								float num3 = Vector2.DistanceSquared(list3[i], projectionInfo.LinePoint) + Vector2.DistanceSquared(list3[j], projectionInfo2.LinePoint);
								if (num3 >= num2 && (projectionInfo.LinePoint - list4[i]).AngleBetween(projectionInfo2.LinePoint - list4[j]) > 90f)
								{
									num2 = num3;
									left = list4[i];
									right = list4[j];
								}
							}
						}
						return new PredictionOutput
						{
							Hitchance = prediction.Hitchance,
							_aoeTargetsHitCount = num,
							UnitPosition = prediction.UnitPosition,
							CastPosition = ((left + right) * 0.5f).ToVector3(),
							Input = input
						};
					}
				}
				return prediction;
			}

			internal static Vector2[] GetCandidates(Vector2 from, Vector2 to, float radius, float range)
			{
				Vector2 vector = (from + to) / 2f;
				Vector2[] array = CircleCircleIntersection(from, vector, radius, from.Distance(vector));
				if (array.Length > 1)
				{
					Vector2 vector2 = array[0];
					Vector2 vector3 = array[1];
					vector2 = from + range * (to - vector2).Normalized();
					vector3 = from + range * (to - vector3).Normalized();
					return new Vector2[]
					{
						vector2,
						vector3
					};
				}
				return new Vector2[0];
			}
			public static Vector2[] CircleCircleIntersection(Vector2 center1, Vector2 center2, float radius1, float radius2)
			{
				float num = center1.Distance(center2);
				if (num > radius1 + radius2 || num <= Math.Abs(radius1 - radius2))
				{
					return new Vector2[0];
				}
				float num2 = (radius1 * radius1 - radius2 * radius2 + num * num) / (2f * num);
				float scale = (float)Math.Sqrt((double)(radius1 * radius1 - num2 * num2));
				Vector2 vector = (center2 - center1).Normalized();
				Vector2 left = center1 + num2 * vector;
				Vector2 vector2 = left + scale * vector.Perpendicular();
				Vector2 vector3 = left - scale * vector.Perpendicular();
				return new Vector2[]
				{
				vector2,
				vector3
				};
			}

			internal static IEnumerable<Vector2> GetHits(Vector2 start, Vector2 end, double radius, List<Vector2> points)
			{
				return from p in points
					   where (double)p.Distanced(start, end, true, true) <= radius * radius
					   select p;
			}			
		}
		public static float Distanced(this Vector2 point, Vector2 segmentStart, Vector2 segmentEnd, bool onlyIfOnSegment = false, bool squared = false)
		{
			var projectionInfo = point.ProjectOn(segmentStart, segmentEnd);
			if (!projectionInfo.IsOnSegment && onlyIfOnSegment)
			{
				return float.MaxValue;
			}
			if (!squared)
			{
				return Vector2.Distance(projectionInfo.SegmentPoint, point);
			}
			return Vector2.DistanceSquared(projectionInfo.SegmentPoint, point);
		}
		internal class PossibleTarget
		{
			public Vector2 Position;

			public AIBaseClient Unit;
		}
	}
}
