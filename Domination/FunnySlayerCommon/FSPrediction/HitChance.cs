using System;

namespace FSpred.Prediction
{
	public enum HitChance
	{
		Immobile = 5,
		Dashing = 6,
		VeryHigh = 4,
		High = 3,
		Medium = 2,
		Low = 1,
		Impossible = -2,
		OutOfRange = 0,
		Collision = -1
	}
}
