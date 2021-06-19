using DominationAIO.Properties;
using EnsoulSharp;
using EnsoulSharp.SDK;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
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
        public Bitmap BitMapOnWorld;
        public Bitmap BitMapOnMinimap;

        public TrackerHelper(AIBaseClient Object)
        {
            Unit = Object;
            ValidPosition = Object.Position;
            InValidPosition = Object.Position;
            BitMapOnWorld = Load(Object.CharacterName);
            BitMapOnMinimap = Load(Object.CharacterName, true);

            Drawing.OnEndScene += Drawing_OnEndScene;
        }

        public void Drawing_OnEndScene(EventArgs args)
        {
            if (!Program.LoadTracker.Enabled || Unit.IsDead || GameObjects.EnemySpawnPoints.Any(i => i.Position.Distance(Unit) < 850 + Unit.BoundingRadius))
                return;

            if (Unit.IsValidTarget() || Unit.IsVisible)
            {
                ValidPosition = Unit.Position;
                InValidPosition = Unit.Position;
            }
            else
            {
                InValidPosition = Unit.Position;
            }

            if (InValidPosition.DistanceToPlayer() <= 3000)
            {
                var line = new Geometry.Line(ObjectManager.Player.Position, InValidPosition);
                line.Draw   (System.Drawing.Color.Red);
            }

            if (ValidPosition.IsOnScreen() || InValidPosition.IsOnScreen())
            {
                /*if (!Unit.IsVisible)
                {
                    var posdraw = Drawing.WorldToScreen(new Vector3(InValidPosition.X - 50, InValidPosition.Y + 50, InValidPosition.Z));

                    using (var r = new Render.Sprite(BitMapOnWorld, posdraw))
                    {
                        r.Draw();
                        r.Dispose();
                    }
                }*/

                if (InValidPosition.IsOnScreen())
                {
                    var pos = Drawing.WorldToScreen(InValidPosition);
                    var r = new Render.Text(Unit.CharacterName, pos, 20, new ColorBGRA(0 , 0, 255, 255));
                    Render.Circle.DrawCircle(InValidPosition, 50, System.Drawing.Color.Red);
                    r.Draw();
                    r.Dispose();
                }
            }

            if (!Unit.IsValidTarget() && !Unit.IsVisible)
            {
                Vector2 posdraw = Vector2.Zero;
                if (Drawing.WorldToMinimap(new Vector3(InValidPosition.X - 1000, InValidPosition.Y + 1000, InValidPosition.Z), out posdraw))
                {
                    using (var r = new Render.Sprite(BitMapOnMinimap, posdraw))
                    {
                        r.Draw();
                        r.Dispose();
                    }
                }
                else
                {

                }
            }
        }

        public TrackerHelper(AIBaseClient Object, Vector3 ValidPos)
        {
            Unit = Object;
            ValidPosition = ValidPos;
            InValidPosition = Object.Position;
            BitMapOnWorld = Load(Object.CharacterName);
            BitMapOnMinimap = Load(Object.CharacterName, true);

            Drawing.OnEndScene += Drawing_OnEndScene;
        }
        public TrackerHelper(AIBaseClient Object, Vector3 ValidPos, Vector3 InValidPos)
        {
            Unit = Object;
            ValidPosition = ValidPos;
            InValidPosition = InValidPos;
            BitMapOnWorld = Load(Object.CharacterName);
            BitMapOnMinimap = Load(Object.CharacterName, true);

            Drawing.OnEndScene += Drawing_OnEndScene;
        }

        public static Bitmap Load(string championName, bool onminimap = false, int size = -1)
        {
            var path = Path.Combine(Program.ImagesFolderPath, championName + ".png");
            string cachedPath = path;
            var image = Image.FromFile(cachedPath);
            Bitmap bitmap = null;
            if (onminimap)
            {
                bitmap = resizeImage(image, false, 15);
                if (File.Exists(cachedPath))
                {
                    return trapsrant(new Bitmap(bitmap), 75);
                }
            }
            else
            {
                bitmap = resizeImage(image, false, size);
                if (File.Exists(cachedPath))
                {
                    return trapsrant(new Bitmap(bitmap), 80);
                }
            }         
            return null;
        }

        public static Bitmap SpellLoad(string spellname, bool onminimap = false, int size = -1)
        {
            var path = Path.Combine(Path.GetDirectoryName(Program.pathfound) + @"\Cache\Images\Spells", spellname + ".png");
            string cachedPath = path;
            if(!File.Exists(cachedPath))           
                path = Path.Combine(Path.GetDirectoryName(Program.pathfound) + @"\Cache\Images\Spells", spellname + "Wrapper.png");
            cachedPath = path;

            if (!File.Exists(cachedPath))
                return null;

            var image = Image.FromFile(cachedPath);
            Bitmap bitmap = null;
            if (onminimap)
            {
                bitmap = resizeImage(image, false, 15);
                if (File.Exists(cachedPath))
                {
                    return trapsrant(new Bitmap(bitmap), 75);
                }
            }
            else
            {
                bitmap = resizeImage(image, false, size);
                if (File.Exists(cachedPath))
                {
                    return trapsrant(new Bitmap(bitmap), 80);
                }
            }
            return null;
        }

        public static Bitmap Load(int NetWorkID)
        {
            var championName = ObjectManager.GetUnitByNetworkId<AIHeroClient>(NetWorkID).CharacterName;
            var path = Path.Combine(Program.ImagesFolderPath, championName + ".png");
            string cachedPath = path;
            var image = Image.FromFile(cachedPath);
            Bitmap bitmap = null;
            bitmap = resizeImage(image, false, 450);
            if (File.Exists(cachedPath))
            {
                return trapsrant(new Bitmap(bitmap), 100);
            }
            return null;
        }

        public static Bitmap resizeImage(Image img, bool spell = false, int sizex = -1)
        {
            sizex = sizex == -1 ? 30 : sizex;
            var mod = sizex * 0.01f;
            var mod2 = spell ? 0.627f : 1;
            var size = new Size((int)(img.Width * mod * mod2), (int)(img.Height * mod * mod2));
            var newbtm = new Bitmap(img, size);
            img.Dispose();
            return newbtm;
        }
        public static Bitmap trapsrant(Image image, int prec)
        {
            var percent = Math.Min(1, prec * 0.01f);
            var org = new Bitmap(image);
            var trans = new Bitmap(image.Width, image.Height);
            var c = System.Drawing.Color.Black;
            var v = System.Drawing.Color.Black;
            for (int i = 0; i < image.Width; i++)
            {
                var x = i;
                for (int j = 0; j < image.Height; j++)
                {
                    var y = j;
                    c = org.GetPixel(x, y);
                    v = System.Drawing.Color.FromArgb((int)(255 * percent), c.R, c.G, c.B);
                    trans.SetPixel(x, y, v);
                }
            }
            org.Dispose();
            image.Dispose();

            return trans;
        }
    }   
}
