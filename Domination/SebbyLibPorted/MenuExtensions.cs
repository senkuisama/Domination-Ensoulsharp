using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using SharpDX;
using System.Drawing;

namespace SebbyLibPorted
{  
    public static class MenuExtensions
    {
        public static Menu SubMenu(this Menu menu, string menuName)
        {
            return menu[menuName] as Menu;
        }

        public static Menu AddSubMenu(this Menu menu, Menu addMenu)
        {
            menu.Add(addMenu);
            return addMenu;
        }

        public static object AddItem(this Menu menu, MenuItem items)
        {
            try
            {
                menu.Add(items);
                return items;
            }
            catch
            {
                Console.WriteLine(items.Name);
                throw new Exception();
            }
        }

        public static MenuBool AddSpellDraw(this Menu menu, SpellSlot slot)
        {
            MenuBool a;
            switch (slot)
            {
                case SpellSlot.Q:
                    a = new MenuBool("DrawQRange", "Draw Q Range");
                    menu.Add(a);
                    menu.Add(new MenuColor("DrawQColor", "^ Q Color", SharpDX.Color.Indigo));
                    return a;
                case SpellSlot.W:
                    a = new MenuBool("DrawWRange", "Draw W Range");
                    menu.Add(a);
                    menu.Add(new MenuColor("DrawWColor", "^ W Color", SharpDX.Color.Yellow));
                    return a;
                case SpellSlot.E:
                    a = new MenuBool("DrawERange", "Draw E Range");
                    menu.Add(a);
                    menu.Add(new MenuColor("DrawEColor", "^ E Color", SharpDX.Color.Green));
                    return a;
                case SpellSlot.Item1:
                    a = new MenuBool("DrawEMaxRange", "Draw E Max Range");
                    menu.Add(a);
                    menu.Add(new MenuColor("DrawEMaxColor", "^ E Max Color", SharpDX.Color.Green));
                    return a;
                case SpellSlot.R:
                    a = new MenuBool("DrawRRange", "Draw R Range");
                    menu.Add(a);
                    menu.Add(new MenuColor("DrawRColor", "^ R Color", SharpDX.Color.Gold));
                    return a;
            }

            return null;


        }

        public static void AddSpellDraw(this Menu menu, string slotName, SharpDX.Color color)
        {
            menu.Add(new MenuBool("Draw" + slotName + "Range", "Draw " + slotName + " Range"));
            menu.Add(new MenuColor("Draw" + slotName + "Color", "^ " + slotName + " Color", color));
        }

        public static MenuSlider SetValue(this MenuSlider menuItem, Slider sliderValue)
        {
            menuItem.Value = sliderValue.value;
            menuItem.MinValue = sliderValue.minValue;
            menuItem.MaxValue = sliderValue.maxValue;
            return menuItem;
        }

        public static AMenuComponent Item(this Menu menu, string name, bool championUnique = false)
        {
            if (championUnique)
            {
                name = ObjectManager.Player.CharacterName + name;
            }

            //Search in our own items
            foreach (var item in menu.Components.ToArray().Where(item => !(item.Value is Menu) && item.Value.Name == name))
            {
                return item.Value;
            }

            //Search in submenus
            foreach (var subMenu in menu.Components.ToArray().Where(x => x.Value is Menu))
            {
                foreach (var item in (subMenu.Value as Menu)?.Components)
                {
                    if (item.Value is Menu)
                    {
                        var result = (item.Value as Menu).Item(name, championUnique);
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

        public static List<AMenuComponent> Items(this Menu menu)
        {
            return menu.Components.Values.ToList();
        }

        //public static MenuBool SetValue(this MenuBool menuItem, bool boolean)
        //{
        //    menuItem.SetValue(boolean);
        //    return menuItem;
        //}
    }

    public class Slider
    {
        public int value;
        public int minValue;
        public int maxValue;

        public Slider(int setValue = 0, int setMinValue = 0, int setMaxValue = 100)
        {
            this.value = setValue;
            if (setMaxValue < setMinValue)
            {

                this.minValue = setMaxValue;
                this.maxValue = setMinValue;
            }
            else
            {
                this.minValue = setMinValue;
                this.maxValue = setMaxValue;

            }
        }
    }
}
