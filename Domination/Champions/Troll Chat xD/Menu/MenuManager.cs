using EnsoulSharp.SDK.MenuUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Troll_Chat_xD.Core;
using Troll_Chat_xD.Helper;

namespace Troll_Chat_xD.Menu
{
    public class MenuManager : Troll
    {
        public static EnsoulSharp.SDK.MenuUI.Menu Config;

        public static void LoadMenu()
        {
            Config = new EnsoulSharp.SDK.MenuUI.Menu("TrollChat", "Troll Chat xD", true);
            Config.Add(TrollMenu());
            Config.Attach();
        }

        public static EnsoulSharp.SDK.MenuUI.Menu TrollMenu()
        {
            var menu = new EnsoulSharp.SDK.MenuUI.Menu("Settings", "Settings");
            menu.AddMenuBool("Enabled","Enabled");
            menu.AddMenuBool("MouseScrollEnabled", "Change Enable/Disable Status with Mouse Scroll",false);
            menu.AddMenuBool("DrawStatus", "Draw Status",false);
            menu.AddMenuKeybind("PrintGG", "Print GG", Keys.N, KeyBindType.Toggle);
            menu.AddMenuKeybind("PrintWP", "Print WP", Keys.H, KeyBindType.Toggle);
            menu.AddMenuKeybind("PrintMiddleFinger", "Print Middle Finger", Keys.Z, KeyBindType.Toggle);
            menu.AddMenuKeybind("PrintXD", "Print XD", Keys.G, KeyBindType.Toggle);
            menu.AddMenuKeybind("PrintDick","Prind Dick",Keys.J,KeyBindType.Toggle);

            return menu;
        }
        public static bool PrintDick { get { return Config.GetMenuKeybind("Settings", "PrintDick"); } }

        public static bool DrawStatus { get { return Config.GetMenuBool("Settings", "DrawStatus"); } }
        public static bool Enabled { get { return Config.GetMenuBool("Settings", "Enabled"); } }
        public static bool MouseScrollEnabled { get { return Config.GetMenuBool("Settings", "MouseScrollEnabled"); } }

        public static bool PrintXD { get { return Config.GetMenuKeybind("Settings", "PrintXD"); } }

        public static bool PrintGG { get { return Config.GetMenuKeybind("Settings", "PrintGG"); } }
        public static bool PrintWP { get { return Config.GetMenuKeybind("Settings", "PrintWP"); } }
        public static bool PrintMiddleFinger { get { return Config.GetMenuKeybind("Settings", "PrintMiddleFinger"); } }

    }
}
