using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominationAIO.NewPlugins
{
    public static class MySylas
    {
        private static Spell Q, W, E, R;
        private static Menu SMenu = null;

        private static class XerathMenu
        {
            public static class QMenu
            {
                public static MenuBool UseQCombo = new MenuBool("UseQCombo", "Use Q in Combo");
                public static MenuBool UseQHarass = new MenuBool("UseQHarass", "Use Q in Harass");
                public static MenuBool UseQClear = new MenuBool("UseQClear", "Use Q in Clear");
                public static MenuSlider QClearHit = new MenuSlider("QClearHit", "---> When hit >= x", 3, 1, 6);
                public static MenuSlider QManaClear = new MenuSlider("QManaClear", "---> Clear mana >= x %", 50);
            }
            public static class WMenu
            {
                public static MenuBool UseWCombo = new MenuBool("UseWCombo", "Use W in Combo");
                public static MenuSlider OnlyWWhenp = new MenuSlider("Only When player", "Only When Player Heath < x% ", 50, 0, 101);
                public static MenuSlider OnlyWWhent = new MenuSlider("Only When target", "Only When Target Heath < x% ", 70, 0, 101);
                public static MenuBool UseWHarass = new MenuBool("UseWHarass", "Use W in Harass");
            }
            public static class EMenu
            {
                public static MenuBool UseECombo = new MenuBool("UseECombo", "Use E in Combo");
                public static MenuBool UseEHarass = new MenuBool("UseEHarass", "Use E in Harass");
                public static MenuBool AutoE = new MenuBool("AutoE", "Auto E on best target");
            }                    
        }
        public static void LoadSylas()
        {

        }
    }
}
