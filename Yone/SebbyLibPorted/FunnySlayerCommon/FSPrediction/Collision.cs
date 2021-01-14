using System;
using System.Collections.Generic;
using System.Linq;
using EnsoulSharp;
using EnsoulSharp.SDK;
using SharpDX;

namespace FSpred.Prediction
{
	public static class Collision
	{
		public static List<AIBaseClient> GetCollision(List<Vector3> positions, PredictionInput input)
		{
			var list = new List<AIBaseClient>();
			Func<AIBaseClient, bool> khong;
			Func<AIBaseClient, bool> mot;
			Predicate<AIBaseClient> hai;
			Predicate<AIBaseClient> ba;
			foreach (Vector3 v in positions)
			{
				var collisionObjects = input.CollisionObjects;
				int i = 0;
				while (i < collisionObjects.Length)
				{
					switch (collisionObjects[i])
					{
					case CollisionableObjects.Minions:
					{
								var minions = (from x in GameObjects.Minions
											   where x.IsValid() && !x.IsAlly
											   select x).ToList<AIBaseClient>();
						Func<AIBaseClient, bool> predicate;
								predicate = (khong = (minion => minion.IsValidTarget(Math.Min(input.Range + input.Radius + 100f, 2000f), true, input.RangeCheckFrom)));
						foreach (AIBaseClient AIBaseClient in minions.Where(predicate))
						{
							float distance = AIBaseClient.Position.Distance(input.From);
							input.Unit = AIBaseClient;
							if ((double)Prediction.GetPrediction(input, false, false).UnitPosition.ToVector2().Distanced(input.From.ToVector2(), v.ToVector2(), true, true) <= Math.Pow((double)(input.Radius + (float)(AIBaseClient.IsMoving ? 50 : 15) + AIBaseClient.BoundingRadius), 2.0) && !Collision.MinionIsDead(input, AIBaseClient, distance))
							{
								list.Add(AIBaseClient);
							}
						}
						var neutralMinions = (from x in GameObjects.Minions
																	where x.IsValid() && !x.IsAlly
																	select x).ToList<AIBaseClient>();
								Func<AIBaseClient, bool> predicate2;
								predicate2 = (mot = ((AIBaseClient minion) => minion.IsValidTarget(Math.Min(input.Range + input.Radius + 100f, 2000f), true, input.RangeCheckFrom)));
						using (IEnumerator<AIBaseClient> enumerator2 = neutralMinions.Where(predicate2).GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								AIBaseClient AIBaseClient2 = enumerator2.Current;
								input.Unit = AIBaseClient2;
								if ((double)Prediction.GetPrediction(input, false, false).UnitPosition.ToVector2().Distanced(input.From.ToVector2(), v.ToVector2(), true, true) <= Math.Pow((double)(input.Radius + 15f + AIBaseClient2.BoundingRadius), 2.0))
								{
									list.Add(AIBaseClient2);
								}
							}
							break;
						}
						goto IL_236;
					}
					case CollisionableObjects.Heroes:
						goto IL_236;
					case CollisionableObjects.Walls:
						goto IL_3D5;
					case CollisionableObjects.Allies:
						goto IL_307;
					}
					IL_433:
					i++;
					continue;
					IL_236:
					List<AIBaseClient> heroes = GameObjects.EnemyHeroes.ToList<AIBaseClient>();
					Predicate<AIBaseClient> match;
					match = (hai = ((AIBaseClient hero) => hero.IsValidTarget(Math.Min(input.Range + input.Radius + 100f, 2000f), true, input.RangeCheckFrom)));
					using (List<AIBaseClient>.Enumerator enumerator3 = heroes.FindAll(match).GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							AIBaseClient AIBaseClient3 = enumerator3.Current;
							input.Unit = AIBaseClient3;
							if ((double)Prediction.GetPrediction(input, false, false).UnitPosition.ToVector2().Distanced(input.From.ToVector2(), v.ToVector2(), true, true) <= Math.Pow((double)(input.Radius + 50f + AIBaseClient3.BoundingRadius), 2.0))
							{
								list.Add(AIBaseClient3);
							}
						}
						goto IL_433;
					}
					IL_307:
					List<AIBaseClient> heroes2 = GameObjects.AllyHeroes.ToList<AIBaseClient>();
					Predicate<AIBaseClient> match2;
					match2 = (ba = ((AIBaseClient hero) => Vector3.Distance(ObjectManager.Player.Position, hero.Position) <= Math.Min(input.Range + input.Radius + 100f, 2000f)));
					using (List<AIBaseClient>.Enumerator enumerator3 = heroes2.FindAll(match2).GetEnumerator())
					{
						while (enumerator3.MoveNext())
						{
							AIBaseClient AIBaseClient4 = enumerator3.Current;
							input.Unit = AIBaseClient4;
							if ((double)Prediction.GetPrediction(input, false, false).UnitPosition.ToVector2().Distanced(input.From.ToVector2(), v.ToVector2(), true, true) <= Math.Pow((double)(input.Radius + 50f + AIBaseClient4.BoundingRadius), 2.0))
							{
								list.Add(AIBaseClient4);
							}
						}
						goto IL_433;
					}
					IL_3D5:
					float num = v.Distance(input.From) / 20f;
					for (int j = 0; j < 20; j++)
					{
						if (input.From.ToVector2().Extend(v.ToVector2(), num * (float)j).IsWall())
						{
							list.Add(ObjectManager.Player);
						}
					}
					goto IL_433;
				}
			}
			return list.Distinct<AIBaseClient>().ToList<AIBaseClient>();
		}

		private static bool MinionIsDead(PredictionInput input, AIBaseClient minion, float distance)
		{
			float num = distance / input.Speed + input.Delay;
			if (Math.Abs(input.Speed - 3.40282347E+38f) < 1.401298E-45f)
			{
				num = input.Delay;
			}
			int time = (int)(num * 1000f) - Game.Ping;
			return LaneClearHealthPrediction(minion, time, 0) <= 0f;
		}
		public static float LaneClearHealthPrediction(AIBaseClient unit, int time, int delay = 70)
		{
			float num = 0f;			
			return unit.Health - num;
		}	
	}
}
