using EnsoulSharp.SDK;
using EnsoulSharp;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;
using PRADA_Vayne.MyLogic.Others;
using PRADA_Vayne.MyUtils;
using System.Drawing;
using System.Linq;
using Color = SharpDX.Color;
using DominationAIO.Champions.Helper;

namespace PRADA_Vayne.MyInitializer
{
    public static partial class PRADALoader
    {
        public static void LoadMenu()
        {
            ConstructMenu();
            //InitOrbwalker();
            FinishMenuInit();
        }

        public static void ConstructMenu()
        {
            Program.MainMenu = new Menu("PRADA Vayne", "Prada Vayne Best of the best", true);
            Program.ComboMenu = new Menu("Combo Settings", "combomenu");
            Program.LaneClearMenu =
                new Menu("Laneclear Settings", "laneclearmenu");
            Program.EscapeMenu = new Menu("Escape Settings", "escapemenu");

            Program.DrawingsMenu =
                new Menu("Drawing Settings", "drawingsmenu");
            Program.DrawingsMenu.AddMenuBool("streamingmode", "Disable All Drawings");
            Program.DrawingsMenu.AddMenuBool("drawenemywaypoints", "Draw Enemy Waypoints");
            Program.SkinhackMenu = new Menu("Skin Hack", "skinhackmenu");
            Program.OrbwalkerMenu =
                new Menu("Orbwalker", "orbwalkermenu");
            Program.ComboMenu.AddMenuBool("QCombo", "Auto Tumble");
            Program.ComboMenu
                .AddMenuList("QMode", "Q Mode: ", new[] { "PRADA", "TO MOUSE" }, 0);
            Program.ComboMenu.AddMenuSlider(
                "QMinDist", "Min dist from enemies", 375, 325, 525);
            Program.ComboMenu.AddMenuList(
                "QOrderBy", "Q to position", new[]
                    {"CLOSETOMOUSE", "CLOSETOTARGET"}, 0);
            Program.ComboMenu.AddMenuBool("QChecks", "Q Safety Checks");
            Program.ComboMenu.AddMenuBool("EQ", "Q After E");
            Program.ComboMenu.AddMenuBool("QR", "Q after Ult");
            Program.ComboMenu.AddMenuBool("OnlyQinCombo", "Only Q in COMBO", false);
            Program.ComboMenu.AddMenuBool("FocusTwoW", "Focus 2 W Stacks", false);
            Program.ComboMenu.AddMenuBool("ECombo", "Auto Condemn");
            Program.ComboMenu
                .AddMenuKeybind("ManualE", "Semi-Manual Condemn", System.Windows.Forms.Keys.E, KeyBindType.Press);
            Program.ComboMenu.AddMenuList("EMode", "E Mode", new[]
            {
                "PRADASMART", "PRADAPERFECT", "MARKSMAN", "SHARPSHOOTER", "GOSU", "VHR", "PRADALEGACY", "FASTEST",
                "OLDPRADA"
            }, 7);
            Program.ComboMenu.AddMenuSlider("EPushDist", "E Push Distance", 475, 300, 475);
            Program.ComboMenu.AddMenuSlider("EHitchance", "E % Hitchance", 50, 0, 100);
            Program.ComboMenu.AddMenuBool("RCombo", "Auto Ult");
            Program.EscapeMenu.AddMenuBool("QUlt", "Smart Q-Ult");
            Program.EscapeMenu.AddMenuBool("EInterrupt", "Use E to Interrupt");

            Program.EscapeMenu.AddMenuBool("Anti-Gapcloser", "antigapcloser");
            foreach (var hero in Heroes.EnemyHeroes)
            {
                var championName = hero.CharacterName;
                Program.EscapeMenu.AddMenuBool(
                    "antigc" + championName, championName);
                    //{ Lists.CancerChamps.Any(entry => championName == entry) };
                    //.SetValue(Lists.CancerChamps.Any(entry => championName == entry)));
            }

            Program.LaneClearMenu.AddMenuBool("QLastHit", "Use Q to Lasthit");
            Program.LaneClearMenu.AddMenuSlider(
                "QLastHitMana", "Min Mana% for Q Lasthit", 45, 0, 100);
            Program.LaneClearMenu.AddMenuBool("EJungleMobs", "Use E on Jungle Mobs");
            Program.SkinhackMenu.AddMenuBool("shkenabled", "Enabled");
            Program.SkinhackMenu.AddMenuList("skin", "Skin: ", new[]
                    {
                        "Classic", "Vindicator", "Aristocrat", "Dragonslayer", "Heartseeker", "SKT T1", "Arclight",
                        "Dragonslayer Green", "Dragonslayer Red", "Dragonslayer Azure", "Soulstealer"
                    }, 0);
                /*new MenuItem().SetValue(
                new StringList(
                    new[]
                    {
                        "Classic", "Vindicator", "Aristocrat", "Dragonslayer", "Heartseeker", "SKT T1", "Arclight",
                        "Dragonslayer Green", "Dragonslayer Red", "Dragonslayer Azure", "Soulstealer"
                    }, 10))).SetFontStyle(FontStyle.Bold, Color.Gold).ValueChanged += (sender, args) =>
            {
                Utility.DelayAction.Add(250, SkinHack.RefreshSkin);
            };*/
        }

        /*public static void InitOrbwalker()
        {
            Program.Orbwalker = new Orbwalker(Program.OrbwalkerMenu);
        }*/

        public static void FinishMenuInit()
        {
            Program.MainMenu.Add(Program.ComboMenu);
            Program.MainMenu.Add(Program.LaneClearMenu);
            Program.MainMenu.Add(Program.EscapeMenu);
            Program.MainMenu.Add(Program.SkinhackMenu);
            Program.MainMenu.Add(Program.DrawingsMenu);
            Program.MainMenu.Add(Program.OrbwalkerMenu);

            Program.MainMenu.Attach();
        }
    }
}