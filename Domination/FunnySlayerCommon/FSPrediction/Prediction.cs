using System;
using System.Collections.Generic;
using System.Linq;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using SharpDX;

namespace FSpred.Prediction
{
	public static class Prediction
	{
		public static PredictionOutput GetPrediction(this Spell spell, AIBaseClient unit, bool aoe = false, float overrideRange = -1f, CollisionableObjects[] collisionable = null)
		{
			bool flag = false;
			if ((int)spell.Type == 1)
			{
				flag = true;
			}
			return Prediction.GetPrediction(new PredictionInput
			{
				CollisionYasuoWall = false,
				Unit = unit,
				Delay = spell.Delay,
				Radius = spell.Width,
				Speed = spell.Speed,
				From = spell.From,
				Range = ((overrideRange > 150f) ? overrideRange : spell.Range),
				Collision = spell.Collision,
				Type = (SkillshotType)spell.Type,
				RangeCheckFrom = spell.RangeCheckFrom,
				Aoe = (flag || aoe),
				CollisionObjects = (collisionable ?? new CollisionableObjects[1])
			});
		}

		public static PredictionOutput GetPrediction(this Spell spell, AIBaseClient unit, out PredictionInput getinput, bool aoe = false, float overrideRange = -1f, CollisionableObjects[] collisionable = null)
		{
			bool flag = false;
			if ((SkillshotType)spell.Type == SkillshotType.SkillshotCircle)
			{
				flag = true;
			}

			getinput = new PredictionInput
			{
				CollisionYasuoWall = false,
				Unit = unit,
				Delay = spell.Delay,
				Radius = spell.Width,
				Speed = spell.Speed,
				From = spell.From,
				Range = ((overrideRange > 150f) ? overrideRange : spell.Range),
				Collision = spell.Collision,
				Type = (SkillshotType)spell.Type,
				RangeCheckFrom = spell.RangeCheckFrom,
				Aoe = (flag || aoe),
				CollisionObjects = (collisionable ?? new CollisionableObjects[1])
			};
			return Prediction.GetPrediction(getinput);
		}
		public static bool CastSpell(this Spell spell, AIHeroClient target, HitChance hit = HitChance.High)
		{
			try
			{
				var prediction = FSpred.Prediction.Prediction.GetPrediction(spell, target, false, -1f, null);
				if (prediction.AoeTargetsHitCount > 1 && prediction.Hitchance >= HitChance.High)
				{
					return spell.Cast(prediction.CastPosition);
				}
				if (prediction.Hitchance >= hit)
				{
					return spell.Cast(prediction.CastPosition);
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex);
			}
			return false;
		}
		public static PredictionOutput GetPrediction(AIBaseClient unit, float delay)
		{
			return Prediction.GetPrediction(new PredictionInput
			{
				Unit = unit,
				Delay = delay
			});
		}

		public static PredictionOutput GetPrediction(AIBaseClient unit, float delay, float radius)
		{
			return Prediction.GetPrediction(new PredictionInput
			{
				Unit = unit,
				Delay = delay,
				Radius = radius
			});
		}

		public static Vector2 PredictUnitPosition(AIBaseClient unit, int time)
		{
			float num = (float)time / 1000f * unit.MoveSpeed;
			List<Vector2> waypoints = unit.GetWaypoints();
			for (int i = 0; i < waypoints.Count - 1; i++)
			{
				Vector2 v = waypoints[i];
				Vector2 to = waypoints[i + 1];
				float num2 = v.Distance(to);
				if (num2 > num)
				{
					return v.Extend(to, num);
				}
				num -= num2;
			}
			if (waypoints.Count != 0)
			{
				return waypoints.Last<Vector2>();
			}
			return unit.Position.ToVector2();
		}

		public static PredictionOutput GetPrediction(AIBaseClient unit, float delay, float radius, float speed)
		{
			return Prediction.GetPrediction(new PredictionInput
			{
				Unit = unit,
				Delay = delay,
				Radius = radius,
				Speed = speed
			});
		}

		public static PredictionOutput GetPrediction(AIBaseClient unit, float delay, float radius, float speed, CollisionableObjects[] collisionable)
		{
			return Prediction.GetPrediction(new PredictionInput
			{
				Unit = unit,
				Delay = delay,
				Radius = radius,
				Speed = speed,
				CollisionObjects = collisionable
			});
		}

		public static PredictionOutput GetPrediction(PredictionInput input)
		{
			return Prediction.GetPrediction(input, true, true);
		}

		public static void Initialize()
		{
			string[] hitchance = {
				"Medium",
				"High",
				"Very High"
			};
			_menu = new Menu("Prediction", "FS Prediction Hitchance", true);
			item = new MenuSlider("PredMaxRange", "Max Range %", 100, 0, 100);
			var QHitChance = new MenuList("QHitChance", "Q Hit Chance", hitchance, 1);
			var WHitChance = new MenuList("WHitChance", "W Hit Chance", hitchance, 1);
			var EHitChance = new MenuList("EHitChance", "E Hit Chance", hitchance, 1);
			var RHitChance = new MenuList("RHitChance", "R Hit Chance", hitchance, 1);
			_menu.Add(QHitChance);
			_menu.Add(WHitChance);
			_menu.Add(EHitChance);
			_menu.Add(RHitChance);

			_menu.Add(item);
			_menu.Attach();

			//AIHeroClient.OnProcessSpellCast += AIHeroClient_OnProcessSpellCast;
		}
		public static void Shutdown()
		{

		}

		public static PredictionOutput GetDashingPrediction(PredictionInput input)
		{
			var dashInfo = input.Unit.GetDashInfo();
			PredictionOutput predictionOutput = new PredictionOutput
			{
				Input = input
			};
			var vector = dashInfo.EndPos;
			var positionOnPath = Prediction.GetPositionOnPath(input, new List<Vector2>
			{
				input.Unit.Position.ToVector2(),
				vector
			}, dashInfo.Speed);
			if (positionOnPath.Hitchance >= HitChance.High && positionOnPath.UnitPosition.ToVector2().Distanced(input.Unit.Position.ToVector2(), vector, true, false) < 200f)
			{
				positionOnPath.CastPosition = positionOnPath.UnitPosition;
				positionOnPath.Hitchance = HitChance.Dashing;
				return positionOnPath;
			}
			if (input.Unit.GetWaypoints().PathLength() > 200f && input.Delay / 2f + input.From.ToVector2().Distance(vector) / input.Speed - 0.25f <= input.Unit.Distance(vector) / dashInfo.Speed + input.RealRadius / input.Unit.MoveSpeed && !vector.IsZero && vector.IsValid())
			{
				return new PredictionOutput
				{
					CastPosition = vector.ToVector3(),
					UnitPosition = vector.ToVector3(),
					Hitchance = HitChance.Dashing
				};
			}
			predictionOutput.CastPosition = input.Unit.GetWaypoints().Last<Vector2>().ToVector3();
			predictionOutput.UnitPosition = predictionOutput.CastPosition;
			return predictionOutput;
		}

		public static PredictionOutput GetImmobilePrediction(PredictionInput input, double remainingImmobileT)
		{
			if ((double)(input.Delay + input.Unit.Distance(input.From) / input.Speed) <= remainingImmobileT + (double)(input.RealRadius / input.Unit.MoveSpeed))
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
			};
		}

		internal static PredictionOutput GetPositionOnPath(PredictionInput input, List<Vector2> path, float speed = -1f)
		{
			speed = ((Math.Abs(speed - -1f) < float.Epsilon) ? input.Unit.MoveSpeed : speed);
			if (path.Count <= 1)
			{
				return new PredictionOutput
				{
					Input = input,
					UnitPosition = input.Unit.Position,
					CastPosition = input.Unit.Position,
					Hitchance = HitChance.VeryHigh
				};
			}
			float num = path.PathLength();
			if (num >= input.Delay * speed - input.RealRadius && Math.Abs(input.Speed - 3.40282347E+38f) < 1.401298E-45f)
			{
				float num2 = input.Delay * speed - input.RealRadius;
				for (int i = 0; i < path.Count - 1; i++)
				{
					Vector2 vector = path[i];
					Vector2 vector2 = path[i + 1];
					float num3 = vector.Distance(vector2);
					if (num3 >= num2)
					{
						Vector2 value = (vector2 - vector).Normalized();
						Vector2 v = vector + value * num2;
						Vector2 v2 = vector + value * ((i == path.Count - 2) ? Math.Min(num2 + input.RealRadius, num3) : (num2 + input.RealRadius));
						return new PredictionOutput
						{
							Input = input,
							CastPosition = v.ToVector3(),
							UnitPosition = v2.ToVector3(),
							Hitchance = (HitChance.High)
						};
					}
					num2 -= num3;
				}
			}
			if (num >= input.Delay * speed - input.RealRadius && Math.Abs(input.Speed - 3.40282347E+38f) > 1.401298E-45f)
			{
				float distance = input.Delay * speed - input.RealRadius;
				if ((input.Type == SkillshotType.SkillshotLine || input.Type == SkillshotType.SkillshotCone) && input.From.DistanceSquared(input.Unit.Position) < 40000f)
				{
					distance = input.Delay * speed;
				}
				path = path.CutPath(distance);
				float num4 = 0f;
				int j = 0;
				while (j < path.Count - 1)
				{
					Vector2 vector3 = path[j];
					Vector2 vector4 = path[j + 1];
					float num5 = vector3.Distance(vector4) / speed;
					Vector2 value2 = (vector4 - vector3).Normalized();
					vector3 -= speed * num4 * value2;
					object[] array = VectorMovementCollision(vector3, vector4, speed, input.From.ToVector2(), input.Speed, num4);
					float num6 = (float)array[0];
					Vector2 vector5 = (Vector2)array[1];
					if (vector5.IsValid() && num6 >= num4 && num6 <= num4 + num5)
					{
						if (vector5.DistanceSquared(vector4) >= 20f)
						{
							Vector2 v3 = vector5 + input.RealRadius * value2;
							SkillshotType type = input.Type;
							return new PredictionOutput
							{
								Input = input,
								CastPosition = vector5.ToVector3(),
								UnitPosition = v3.ToVector3(),
								Hitchance = (HitChance.High)
							};
						}
						break;
					}
					else
					{
						num4 += num5;
						j++;
					}
				}
			}
			Vector2 v4 = path.Last<Vector2>();
			return new PredictionOutput
			{
				Input = input,
				CastPosition = v4.ToVector3(),
				UnitPosition = v4.ToVector3(),
				Hitchance = HitChance.Medium
			};
		}
		public static object[] VectorMovementCollision(Vector2 startPoint1, Vector2 endPoint1, float v1, Vector2 startPoint2, float v2, float delay = 0f)
		{
			float x = startPoint1.X;
			float y = startPoint1.Y;
			float x2 = endPoint1.X;
			float y2 = endPoint1.Y;
			float x3 = startPoint2.X;
			float y3 = startPoint2.Y;
			float num = x2 - x;
			float num2 = y2 - y;
			float num3 = (float)Math.Sqrt((double)(num * num + num2 * num2));
			float num4 = float.NaN;
			float num5 = (Math.Abs(num3) > float.Epsilon) ? (v1 * num / num3) : 0f;
			float num6 = (Math.Abs(num3) > float.Epsilon) ? (v1 * num2 / num3) : 0f;
			float num7 = x3 - x;
			float num8 = y3 - y;
			float num9 = num7 * num7 + num8 * num8;
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
					float num11 = num5 * num5 + num6 * num6 - v2 * v2;
					float num12 = -num7 * num5 - num8 * num6;
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
						float num14 = num12 * num12 - num11 * num9;
						if (num14 >= 0f)
						{
							float num15 = (float)Math.Sqrt((double)num14);
							float num16 = (-num15 - num12) / num11;
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
		public static PredictionOutput GetPrediction(PredictionInput input, bool ft, bool checkCollision)
		{
			PredictionOutput predictionOutput = null;
			if (!input.Unit.IsValidTarget(float.MaxValue, false))
			{
				return new PredictionOutput();
			}
			if (ft)
			{
				input.Delay += Game.Ping / 2000f + 
					0.06f;
				if (input.Aoe)
				{
					return AoePrediction.GetPrediction(input);
				}
			}
			if (Math.Abs(input.Range - 3.40282347E+38f) > 1.401298E-45f && (double)input.Unit.DistanceSquared(input.RangeCheckFrom) > Math.Pow((double)input.Range * 1.5, 2.0))
			{
				return new PredictionOutput
				{
					Input = input
				};
			}


			if (input.Unit.IsDashing())
			{
				predictionOutput = Prediction.GetDashingPrediction(input);
			}
			else
			{
				double num = Prediction.UnitIsImmobileUntil(input.Unit);
				if (num >= 0.0)
				{
					predictionOutput = Prediction.GetImmobilePrediction(input, num);
				}
				else
				{
					input.Range = input.Range * item.Value / 100f;
				}
			}


			if (predictionOutput == null)
			{
				predictionOutput = Prediction.GetStandardPrediction(input);
			}


			if (Math.Abs(input.Range - 3.40282347E+38f) > 1.401298E-45f)
			{
				if (predictionOutput.Hitchance >= HitChance.High && (double)input.RangeCheckFrom.DistanceSquared(input.Unit.Position) > Math.Pow((double)(input.Range + input.RealRadius * 3f / 4f), 2.0))
				{
					predictionOutput.Hitchance = HitChance.Medium;
				}
				if ((double)input.RangeCheckFrom.DistanceSquared(predictionOutput.UnitPosition) > Math.Pow((double)(input.Range + ((input.Type == SkillshotType.SkillshotCircle) ? input.RealRadius : 0f)), 2.0))
				{
					predictionOutput.Hitchance = HitChance.OutOfRange;
				}
				if ((double)input.RangeCheckFrom.DistanceSquared(predictionOutput.CastPosition) > Math.Pow((double)input.Range, 2.0))
				{
					if (predictionOutput.Hitchance != HitChance.OutOfRange)
					{
						predictionOutput.CastPosition = input.RangeCheckFrom + input.Range * (predictionOutput.UnitPosition - input.RangeCheckFrom).ToVector2().Normalized().ToVector3();
					}
					else
					{
						predictionOutput.Hitchance = HitChance.OutOfRange;
					}
				}
			}
			if (checkCollision && input.Collision)
			{
				var positions = new List<Vector3>
				{
					predictionOutput.UnitPosition,
					predictionOutput.CastPosition
				};
				AIBaseClient originalUnit = input.Unit;
				predictionOutput.CollisionObjects = Collision.GetCollision(positions, input);
				predictionOutput.CollisionObjects.RemoveAll(x => x.NetworkId == originalUnit.NetworkId);
				predictionOutput.Hitchance = ((predictionOutput.CollisionObjects.Count > 0) ? HitChance.Collision : predictionOutput.Hitchance);
			}
			if (input.CollisionYasuoWall && predictionOutput.Hitchance > HitChance.Impossible && new List<Vector3>
			{
				predictionOutput.UnitPosition,
				predictionOutput.CastPosition,
				input.Unit.Position
			}.GetYasuoWallCollision(input.From))
			{
				predictionOutput.Hitchance = HitChance.Collision;
			}
			if (predictionOutput.Hitchance == HitChance.High || predictionOutput.Hitchance == HitChance.VeryHigh)
			{
				List<Vector2> waypoints = input.Unit.GetWaypoints();
				if (waypoints.Count > 1 != input.Unit.IsMoving)
				{
					predictionOutput.Hitchance = HitChance.Medium;
				}
				else if (waypoints.Count > 0)
				{
					Vector3 v = waypoints.Last<Vector2>().ToVector3();
					float num2 = v.Distance(input.Unit.Position);
					float num3 = input.From.Distance(input.Unit.Position);
					float num4 = v.Distance(input.From);
					float num5 = num3 / input.Speed;
					if (Math.Abs(input.Speed - 3.40282347E+38f) < 1.401298E-45f)
					{
						num5 = 0f;
					}
					float num6 = num5 + input.Delay;
					float num7 = input.Unit.MoveSpeed * num6 * 0.35f;
					if (input.Type == SkillshotType.SkillshotCircle)
					{
						num7 -= input.Radius / 2f;
					}
					if (num4 <= num3 && num3 > input.Range - num7)
					{
						predictionOutput.Hitchance = HitChance.Medium;
					}
					if (num2 > 0f && num2 < 100f)
					{
						predictionOutput.Hitchance = HitChance.Medium;
					}
				}
			}
			return predictionOutput;
		}
		public static bool GetYasuoWallCollision(this List<Vector3> positions, Vector3 From)
		{
			foreach (Vector3 end in positions)
			{
				if (GetYasuoWallCollision(From, end) != Vector3.Zero)
				{
					return true;
				}
			}
			return false;
		}
		public static Vector3 GetYasuoWallCollision(Vector3 start, Vector3 end)
		{
			if (Environment.TickCount - _lastYasuoWallCasted <= 4500)
			{
				AIBaseClient obj_AI_Base = (from i in ObjectManager.Get<AIBaseClient>()
										   where !i.IsDead && i.Name.ToLower().Contains("windwall")
										   orderby i.DistanceSquared(_lastYasuoWallStart)
										   select i).FirstOrDefault<AIBaseClient>();
				if (obj_AI_Base != null)
				{
					string value = obj_AI_Base.Name.Substring(obj_AI_Base.Name.Length - 1, 1);
					int num = 300 + 50 * Convert.ToInt32(value);
					var value2 = (obj_AI_Base.Position - _lastYasuoWallStart).ToVector2().Normalized().Perpendicular();
					var lineSegment1Start = obj_AI_Base.Position.ToVector2() + (float)num / 2f * value2;
					var lineSegment1End = obj_AI_Base.Position.ToVector2() - (float)num / 2f * value2;
					var intersectionResult = lineSegment1Start.Intersection(lineSegment1End, start.ToVector2(), end.ToVector2());
					if (intersectionResult.Intersects)
					{
						return intersectionResult.Point.ToVector3();
					}
				}
			}
			return Vector3.Zero;
		}

        private static void AIHeroClient_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
			if (sender.IsValidTarget(0f, false) && sender.Team != ObjectManager.Player.Team && args.SData.Name == "YasuoW")
			{
				_lastYasuoWallCasted = Environment.TickCount;
				_lastYasuoWallStart = sender.Position;
			}
		}
		public static float _lastYasuoWallCasted;
		public static Vector3 _lastYasuoWallStart;

		public static PredictionOutput GetStandardPrediction(PredictionInput input)
		{
			float num = input.Unit.MoveSpeed;
			if (input.Unit.DistanceSquared(input.From) < 40000f)
			{
				num /= 1.5f;
			}
			var list = input.Unit.GetWaypoints();
			/*var LastMove = input.Unit.GetWaypoints().LastOrDefault();
			if(input.Unit.IsMoving && input.Unit.IsValidTarget(input.Range) && LastMove.IsValid() & LastMove.DistanceToPlayer() <= input.Range && input.Type == SkillshotType.SkillshotLine)
            {
				var DistanceTarget = LastMove.Distance(input.Unit.Position);
				var getrealmovepoint = new Geometry.Circle(input.Unit.Position, DistanceTarget);

				var getlist = getrealmovepoint.Points.Where(i => i.Distance(LastMove) <= LastMove.Distance(input.Unit.Position) && DistanceTarget * DistanceTarget + input.Unit.DistanceToPlayer() * input.Unit.DistanceToPlayer() == i.DistanceToPlayer() * i.DistanceToPlayer()).ToList();

				if(getlist != null)
                {
					list = getlist;
                }
                else
                {
					list = input.Unit.GetWaypoints();
				}
            }
            else
            {
				list = input.Unit.GetWaypoints();
			}*/
			
			return Prediction.GetPositionOnPath(input, list, num);
		}

		public static double UnitIsImmobileUntil(AIBaseClient unit)
		{
			return (from buff in unit.Buffs
			where buff.IsActive && Game.Time <= buff.EndTime && (buff.Type == BuffType.Charm || buff.Type == BuffType.Knockup || buff.Type == BuffType.Stun || buff.Type == BuffType.Suppression || buff.Type == BuffType.Snare)
			select buff).Aggregate(0.0, (double current, BuffInstance buff) => Math.Max(current, (double)buff.EndTime)) - (double)Game.Time;
		}

		private static Menu _menu;
		private static MenuSlider item;
	}
}
