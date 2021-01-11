using System;
using System.Collections.Generic;
using System.Linq;
using SharpDX;

namespace FSpred.Prediction
{
	internal class StoredPath
	{
		public Vector2 EndPoint
		{
			get
			{
				return this.Path.LastOrDefault();
			}
		}

		public Vector2 StartPoint
		{
			get
			{
				return this.Path.FirstOrDefault();
			}
		}

		public double Time
		{
			get
			{
				return (double)(Environment.TickCount - this.Tick) / 1000.0;
			}
		}

		public int WaypointCount
		{
			get
			{
				return this.Path.Count;
			}
		}

		public List<Vector2> Path;

		public int Tick;
	}
}
