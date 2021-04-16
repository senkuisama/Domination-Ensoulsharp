using EnsoulSharp;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominationAIO
{
    public class TrackerHelper
    {
        public AIBaseClient Unit;
        public Vector3 ValidPosition;
        public Vector3 InValidPosition;

        public TrackerHelper(AIBaseClient Object)
        {
            Unit = Object;
            ValidPosition = Object.Position;
            InValidPosition = Object.Position;
        }
        public TrackerHelper(AIBaseClient Object, Vector3 ValidPos)
        {
            Unit = Object;
            ValidPosition = ValidPos;
            InValidPosition = Object.Position;
        }
        public TrackerHelper(AIBaseClient Object, Vector3 ValidPos, Vector3 InValidPos)
        {
            Unit = Object;
            ValidPosition = ValidPos;
            InValidPosition = InValidPos;
        }
    }
}
