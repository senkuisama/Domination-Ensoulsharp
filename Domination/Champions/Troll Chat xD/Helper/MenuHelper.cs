using EnsoulSharp.SDK.MenuUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Menu = EnsoulSharp.SDK.MenuUI.Menu;

namespace Troll_Chat_xD.Helper
{
    public static class MenuHelper
    {
        public static void AddMenuBool(this EnsoulSharp.SDK.MenuUI.Menu menu, string name, string displayName, bool value = true)
        {
            menu.Add(new MenuBool(name, displayName, value));
        }
        public static void AddMenuSlider(this EnsoulSharp.SDK.MenuUI.Menu menu, string name, string displayName, int value = 0, int minValue = 0, int maxValue = 0)
        {
            menu.Add(new MenuSlider(name, displayName, value, minValue, maxValue));
        }
        public static void AddMenuKeybind(this EnsoulSharp.SDK.MenuUI.Menu menu, string name, string displayName, EnsoulSharp.SDK.MenuUI.Keys key, KeyBindType type)
        {
            menu.Add(new MenuKeyBind(name, displayName, key, type));
        }
        public static void AddMenuSeperator(this EnsoulSharp.SDK.MenuUI.Menu menu, string name, string displayName)
        {
            menu.Add(new MenuSeparator(name, displayName));
        }
        public static void AddMenuList(this EnsoulSharp.SDK.MenuUI.Menu menu, string name, string displayName, string[] items, int selectedValue)
        {
            menu.Add(new MenuList(name, displayName, items, selectedValue));
        }


        public static bool GetMenuBool(this EnsoulSharp.SDK.MenuUI.Menu menu, string mainMenuName, string secondMenuName)
        {
            return menu[mainMenuName][secondMenuName].GetValue<MenuBool>().Enabled;
        }
        public static int GetMenuSlider(this EnsoulSharp.SDK.MenuUI.Menu menu, string mainMenuName, string secondMenuName)
        {
            return menu[mainMenuName][secondMenuName].GetValue<MenuSlider>().Value;
        }
        public static bool GetMenuKeybind(this EnsoulSharp.SDK.MenuUI.Menu menu, string mainMenuName, string secondMenuName)
        {
            return menu[mainMenuName][secondMenuName].GetValue<MenuKeyBind>().Active;
        }
        public static string GetMenuList(this EnsoulSharp.SDK.MenuUI.Menu menu, string mainMenuName, string secondMenuName)
        {
            return menu[mainMenuName][secondMenuName].GetValue<MenuList>().SelectedValue;
        }


        public static void SetMenuBool(this EnsoulSharp.SDK.MenuUI.Menu menu, string mainMenuName, string secondMenuName, bool newValue)
        {
            menu[mainMenuName][secondMenuName].GetValue<MenuBool>().SetValue(newValue);
        }
        public static void SetMenuSlider(this EnsoulSharp.SDK.MenuUI.Menu menu, string mainMenuName, string secondMenuName, int newValue)
        {
            menu[mainMenuName][secondMenuName].GetValue<MenuSlider>().SetValue(newValue);
        }
        public static void SetMenuKeybind(this EnsoulSharp.SDK.MenuUI.Menu menu, string mainMenuName, string secondMenuName, bool newValue)
        {
            menu[mainMenuName][secondMenuName].GetValue<MenuKeyBind>().SetValue(newValue);
        }
        public static void SetMenuList(this EnsoulSharp.SDK.MenuUI.Menu menu, string mainMenuName, string secondMenuName, int newValue)
        {
            menu[mainMenuName][secondMenuName].GetValue<MenuList>().SetValue(newValue);
        }
    }
}
