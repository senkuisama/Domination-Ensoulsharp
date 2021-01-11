using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;


namespace DominationAIO.Champions.Helper
{
    public static class MenuHelper
    {
        public static AMenuComponent Item(this EnsoulSharp.SDK.MenuUI.Menu menu, string name, bool championUnique = false)
        {
            if (championUnique)
            {
                name = ObjectManager.Player.CharacterName + name;
            }

            //Search in our own items
            foreach (var item in menu.Components.ToArray().Where(item => !(item.Value is EnsoulSharp.SDK.MenuUI.Menu) && item.Value.Name == name))
            {
                return item.Value;
            }

            //Search in submenus
            foreach (var subMenu in menu.Components.ToArray().Where(x => x.Value is EnsoulSharp.SDK.MenuUI.Menu))
            {
                foreach (var item in (subMenu.Value as EnsoulSharp.SDK.MenuUI.Menu)?.Components)
                {
                    if (item.Value is EnsoulSharp.SDK.MenuUI.Menu)
                    {
                        var result = (item.Value as EnsoulSharp.SDK.MenuUI.Menu).Item(name, championUnique);
                        if (result != null)
                        {
                            return result;
                        }
                    }
                    else if (item.Value.Name == name)
                    {
                        return item.Value;

                    }
                }

            }

            return null;
        }
        public static void AddMenuBool(this EnsoulSharp.SDK.MenuUI.Menu menu, string name, string displayName, bool value = true)
        {
            menu.Add(new MenuBool(name, displayName, value));
        }
        public static void AddMenuSlider(this EnsoulSharp.SDK.MenuUI.Menu menu, string name, string displayName, int value = 0, int minValue = 0, int maxValue = 0)
        {
            menu.Add(new MenuSlider(name, displayName, value, minValue, maxValue));
        }
        public static void AddMenuKeybind(this EnsoulSharp.SDK.MenuUI.Menu menu, string name, string displayName, Keys key, KeyBindType type)
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
            return menu[mainMenuName].GetValue<MenuBool>(secondMenuName).Enabled;
        }
        public static int GetMenuSlider(this EnsoulSharp.SDK.MenuUI.Menu menu, string mainMenuName, string secondMenuName)
        {
            return menu[mainMenuName].GetValue<MenuSlider>(secondMenuName).Value;
        }
        public static bool GetMenuKeybind(this EnsoulSharp.SDK.MenuUI.Menu menu, string mainMenuName, string secondMenuName)
        {
            return menu[mainMenuName].GetValue<MenuKeyBind>(secondMenuName).Active;
        }
        public static string GetMenuList(this EnsoulSharp.SDK.MenuUI.Menu menu, string mainMenuName, string secondMenuName)
        {
            return menu[mainMenuName].GetValue<MenuList>(secondMenuName).SelectedValue;
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
