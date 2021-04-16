using System;
using System.Collections.Generic;
using EnsoulSharp;
using EnsoulSharp.SDK;
using SharpDX;

namespace FSpred.Prediction
{
	public class PredictionOutput
	{
		public int AoeTargetsHitCount
		{
			get
			{
				var max = Math.Max(this._aoeTargetsHitCount, this.AoeTargetsHit.Count);
				return Math.Max(max, 1);
			}
		}
		public Vector3 CastPosition
		{
			get
			{
				if (this._castPosition.IsValid() && this._castPosition.ToVector2().IsValid())
				{
					return this._castPosition.SetZ(null);
				}
				if (this.Input.Unit == null)
				{
					return Vector3.Zero;
				}
				return this.Input.Unit.Position;
			}
			set
			{
				this._castPosition = value;
			}
		}
		public Vector3 UnitPosition
		{
			get
			{
				if (this._unitPosition.ToVector2().IsValid())
				{
					return this._unitPosition.SetZ(null);
				}
				if (this.Input.Unit == null)
				{
					return Vector3.Zero;
				}
				return this.Input.Unit.Position;
			}
			set
			{
				this._unitPosition = value;
			}
		}

		public List<AIBaseClient> AoeTargetsHit = new List<AIBaseClient>();

		public List<AIBaseClient> CollisionObjects = new List<AIBaseClient>();

		public HitChance Hitchance = HitChance.Impossible;

		internal int _aoeTargetsHitCount;

		internal PredictionInput Input;

		private Vector3 _castPosition;

		private Vector3 _unitPosition;
	}
}
