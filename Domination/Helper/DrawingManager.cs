using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.Utility;
using SharpDX;
using Color = System.Drawing.Color;

namespace DominationAIO.Champions.Helper
{
    public static class DrawingManager
    {
        public static void DrawCircle(this Vector3 position, float range, System.Drawing.Color color, bool checkValue)
        {
            if (checkValue)
                Render.Circle.DrawCircle(position, range, color);
        }

        public static void DrawText(this Vector3 position, float addPosX, float addPosY, Color color, string text, bool checkValue)
        {
            if (checkValue)
            {
                var pos = Drawing.WorldToScreen(position);
                Drawing.DrawText(pos.X + addPosX, pos.Y + addPosY, color, text);
            }
        }
    }
}
