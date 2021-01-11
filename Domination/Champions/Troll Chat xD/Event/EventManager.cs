using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsoulSharp;
using EnsoulSharp.SDK;
using Troll_Chat_xD.Helper;
using Troll_Chat_xD.Menu;
using SharpDX;
using Color = System.Drawing.Color;

namespace Troll_Chat_xD.Event
{
    public class EventManager : MenuManager
    {
        public static void LoadEvents()
        {
            Game.OnUpdate += GameOnOnTick;
            Game.OnWndProc += GameOnOnWndProc;
            Drawing.OnDraw += DrawingOnOnDraw;
        }

        private static void DrawingOnOnDraw(EventArgs args)
        {
            DrawText(ObjectManager.Player.Position, 0, +50, Enabled ? Color.White : Color.Red,
                Enabled ? "Troll Chat Enabled" : "Troll Chat Disabled", DrawStatus);
        }
        public static void DrawText( Vector3 position, float addPosX, float addPosY, Color color, string text, bool checkValue)
        {
            if (checkValue)
            {
                var pos = Drawing.WorldToScreen(position);
                Drawing.DrawText(pos.X + addPosX, pos.Y + addPosY, color, text);
            }
        }
        private static void GameOnOnWndProc(GameWndEventArgs args)
        {
            if (args.Msg != 0x20a || !MouseScrollEnabled)
                return;
            ChangeEnabledStatus();
        }

        private static void ChangeEnabledStatus()
        {
            if (Enabled)
            {
                Config.SetMenuBool("Settings", "Enabled", false);
            }
            else
            {
                Config.SetMenuBool("Settings", "Enabled", true);
            }
        }

        public static void PrintXD()
        {
            Game.Say("##       ##   ########  ", true);
            Game.Say(".##     ##    ##            ## ", true);
            Game.Say("..##   ##     ##            ## ", true);
            Game.Say(".....###        ##            ## ", true);
            Game.Say("..##   ##     ##            ## ", true);
            Game.Say(".##     ##    ##            ## ", true);
            Game.Say("##       ##   ########  ", true);

        }
        public static void PrintMiddleFinger()
        {
            Game.Say("....................../´¯/) ", true);
            Game.Say("....................,/¯../ ", true);
            Game.Say(".................../..../ ", true);
            Game.Say("............./´¯/'...'/´¯¯`·¸ ", true);
            Game.Say("........../'/.../..../......./¨¯\\ ", true);
            Game.Say("........('(...´...´.... ¯~/'...') ", true);
            Game.Say(".........\\.................'...../ ", true);
            Game.Say("..........''...\\.......... _.·´ ", true);
        }

        public static void PrintWP()
        {
            Game.Say("                            ", true);
            Game.Say("#      #    ###### ", true);
            Game.Say("#  #  #   #          #", true);
            Game.Say("#  #  #   #          #", true);
            Game.Say("#  #  #   ###### ", true);
            Game.Say("#  #  #   #     ", true);
            Game.Say("#  #  #   #     ", true);
            Game.Say(" ## ##    #     ", true);
        }
        public static void PrintGG()
        {
            Game.Say("                            ", true);
            Game.Say("     #####      ##### ", true);
            Game.Say(" #               #        ", true);
            Game.Say(" #               #      ", true);
            Game.Say(" #  ####   #  ####", true);
            Game.Say(" #         #   #         #", true);
            Game.Say(" #         #   #         #", true);
            Game.Say("     #####      ##### ", true);

        }

        public static void PrintDick()
        {
            Game.Say(".  ___", true);
            Game.Say(". //     7     ", true);
            Game.Say("(_,_  / \\", true);
            Game.Say(". \\        \\     ", true);
            Game.Say(".  \\        \\", true);
            Game.Say(". _\\        \\__ ", true);
            Game.Say(".(       \\       )", true);
            Game.Say(". \\____\\___/", true);
        }
        private static void GameOnOnTick(EventArgs args)
        {
            if (MenuManager.PrintDick)
            {
                if (Enabled)
                {
                    PrintDick();
                }
                Config.SetMenuKeybind("Settings", "PrintDick", false);
            }
            if (MenuManager.PrintGG)
            {
                if (Enabled)
                {
                    PrintGG();
                }
                Config.SetMenuKeybind("Settings", "PrintGG", false);
            }
            if (MenuManager.PrintMiddleFinger)
            {
                if (Enabled)
                {
                    PrintMiddleFinger();
                }
                Config.SetMenuKeybind("Settings", "PrintMiddleFinger", false);
            }
            if (MenuManager.PrintWP)
            {
                if (Enabled)
                {
                    PrintWP();
                }
                Config.SetMenuKeybind("Settings", "PrintWP", false);
            }
            if (MenuManager.PrintXD)
            {
                if (Enabled)
                {
                    PrintXD();
                }
                Config.SetMenuKeybind("Settings", "PrintXD", false);
            }
        }
    }
}
