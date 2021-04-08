using System;
using EnsoulSharp;
using EnsoulSharp.SDK;
using SharpDX;

namespace FSpred.Prediction
{
	public class PredictionInput
	{
		public Vector3 From
		{
			get
			{
				if (!this._from.ToVector2().IsValid())
				{
					return ObjectManager.Player.Position;
				}
				return this._from;
			}
			set
			{
				this._from = value;
			}
		}

		public Vector3 RangeCheckFrom
		{
			get
			{
				if (this._rangeCheckFrom.ToVector2().IsValid())
				{
					return this._rangeCheckFrom;
				}
				if (!this.From.ToVector2().IsValid())
				{
					return ObjectManager.Player.Position;
				}
				return this.From;
			}
			set
			{
				this._rangeCheckFrom = value;
			}
		}

		internal float RealRadius
		{
			get
			{
				if (!this.UseBoundingRadius)
				{
					return this.Radius;
				}
				return this.Radius + ((this.Type == SkillshotType.SkillshotLine) ? (this.Unit.BoundingRadius / 2f) : 0f);
			}
		}

		public bool Aoe;

		public bool CollisionYasuoWall;

		public bool Collision;

		public CollisionableObjects[] CollisionObjects = new CollisionableObjects[]
		{
			CollisionableObjects.Minions,
			CollisionableObjects.YasuoWall
		};

		public float Delay;

		public float Radius = 1f;

		public float Range = float.MaxValue;

		public float Speed = float.MaxValue;

		public SkillshotType Type;

		public AIBaseClient Unit = ObjectManager.Player;

		public bool UseBoundingRadius = true;

		private Vector3 _from;

		private Vector3 _rangeCheckFrom;
	}
}
