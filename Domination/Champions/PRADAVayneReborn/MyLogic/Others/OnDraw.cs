using DominationAIO.Champions.Helper;
using EnsoulSharp;
using EnsoulSharp.SDK;
using PRADA_Vayne.MyUtils;
using System;
using System.Drawing;
using System.Linq;

namespace PRADA_Vayne.MyLogic.Others
{
    public static partial class Events
    {
        public static void OnDraw(EventArgs args)
        {
            if (Program.MainMenu.GetMenuBool("Drawing Settings", "drawenemywaypoints"))
                foreach (var e in GameObjects.EnemyHeroes.Where(en =>
                    en.IsVisible && !en.IsDead && en.Distance(Heroes.Player) < 2500))
                {
                    var ip = Drawing.WorldToScreen(e.Position); //start pos

                    var wp = e.GetWaypoints();
                    var c = wp.Count - 1;
                    if (wp.Count() <= 1) break;

                    var w = Drawing.WorldToScreen(wp[c].ToVector3()); //endpos

                    Drawing.DrawLine(ip.X, ip.Y, w.X, w.Y, 2, Color.Red);
                }
        }
    }
}