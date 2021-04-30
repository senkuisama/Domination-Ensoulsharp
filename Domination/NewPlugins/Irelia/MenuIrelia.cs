
using EnsoulSharp.SDK.MenuUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FunnySlayerCommon;

namespace DominationAIO.NewPlugins
{
    public static class MenuSettings
    {
        public static void AttackToMenu(this Menu myMenu)
        {
            //Menu FStarget = new Menu("FS Target", "Target Selector");
            //FStarget.AddTargetSelectorMenu();
            //myMenu.Add(FStarget);

            Menu Qmenu = new Menu("Qmenu", "Q Settings")
            {
                MenuSettings.QSettings.Qcombo,
                MenuSettings.QSettings.QListComboMode,
                MenuSettings.QSettings.CheckQDmgITems,
                MenuSettings.QSettings.BonusQ,
                QSettings.DrawQ,
                QSettings.Qdelay,
            };

            Menu Wmenu = new Menu("Wmenu", "W Settings")
            {
                MenuSettings.WSettings.Wcombo,
                MenuSettings.WSettings.Wdelay,
            };

            Menu Emenu = new Menu("Emenu", "E Settings")
            {
                MenuSettings.ESettings.Ecombo,
                MenuSettings.ESettings.Emode,
                MenuSettings.ESettings.EDelay,
                MenuSettings.ESettings.Efeedback,
                ESettings.E1vs1Range,
            };

            Menu Rmenu = new Menu("Rmenu", "R Settings")
            {
                MenuSettings.RSettings.Rcombo,
                MenuSettings.RSettings.Rheath,
                MenuSettings.RSettings.Rhit,
            };

            Menu ClearMenu = new Menu("ClearMenu", "Clear Settings")
            {
                MenuSettings.ClearSettings.DrawMinions,
                MenuSettings.ClearSettings.QireClear,
                MenuSettings.ClearSettings.QireMana,
            };

            Menu Keys = new Menu("Keys", "Keys");
            Keys.Add(MenuSettings.KeysSettings.TurretKey).Permashow();
            Keys.Add(MenuSettings.KeysSettings.FleeKey).Permashow();
            Keys.Add(MenuSettings.KeysSettings.SemiE).Permashow();
            Keys.Add(MenuSettings.KeysSettings.SemiR).Permashow();
            Keys.Add(MenuSettings.KeysSettings.AutoClearMinions).Permashow();

            myMenu.Add(Qmenu);
            myMenu.Add(Wmenu);
            myMenu.Add(Emenu);
            myMenu.Add(Rmenu);
            myMenu.Add(ClearMenu);
            myMenu.Add(Keys);

            SetEValue();

            QSettings.QListComboMode.Index = 3;
            ESettings.Emode.Index = 1;

            ESettings.Emode.ValueChanged += Emode_ValueChanged;
        }

        private static void Emode_ValueChanged(object sender, EventArgs e)
        {
            SetEValue();
        }

        public static void SetEValue()
        {
            if (ESettings.Emode.Index == 0)
            {
                ESettings.EDelay.Value = 600;
            }
            if(ESettings.Emode.Index == 2)
            {
                ESettings.EDelay.Value = 1150;
            }
        }
        public class QSettings
        {
            public static MenuBool Qcombo = new MenuBool("Qcombo", "Q in Combo [Gap_closer | KillSteal]");
            public static MenuList QListComboMode = new MenuList("QListComboMode", "Q List Combo Mode", new string[] { " Simple Gapcloser ", " Focus On Target CanQ ", " Try Dancing "}, 1);
            public static MenuBool CheckQDmgITems = new MenuBool("CheckItemDmg", "Q Dmg Check Items", true);
            public static MenuSlider BonusQ = new MenuSlider("_BonusQ Range", "Bonus Q Range (unit)", 50, 0, 75);
            public static MenuBool DrawQ = new MenuBool("_ireDrawQ", "Draw Q range");
            public static MenuSlider Qdelay = new MenuSlider("_Q Delay", "Q Delay (ms) ", 200, 0, 1000);
        }
        public class WSettings
        {
            public static MenuBool Wcombo = new MenuBool("Wcombo", "W in Combo", false);
            public static MenuSliderButton Wdelay = new MenuSliderButton("WDelay", "Recast W", 0, 0, 1000);
        }

        public class ESettings
        {
            public static MenuBool Ecombo = new MenuBool("Ecombo", "E in Combo");
            public static MenuList Emode = new MenuList("ImproveE", "E Logic : ", new string[] { "Simple E", "Calculator E delay", "Calculator E Pred" }, 0);
            public static MenuSlider EDelay = new MenuSlider("ImproveE Delay", "Delay E prediction (Default 600)", 1150, 400, 1400);
            public static MenuSlider E1vs1Range = new MenuSlider("E1vs1Range", "Range E when 1vs1", 350, 100, 1000);
            public static MenuSeparator Efeedback = new MenuSeparator("Efeedback", "E logic if not good Feedback it to FunnySlayer#0348");
        }

        public class RSettings
        {
            public static MenuBool Rcombo = new MenuBool("Rcombo", "R in Combo [Calculator Dmg]");
            public static MenuSlider Rheath = new MenuSlider("Rheath", "Target Heath", 60);
            public static MenuSlider Rhit = new MenuSlider("Rhit", "Hit Count", 2, 2, 5);
        }

        public class ClearSettings
        {
            public static MenuBool DrawMinions = new MenuBool("DrawMinions", "Draw On Minions");
            public static MenuBool QireClear = new MenuBool("QireClear", "Q Clear");
            public static MenuSlider QireMana = new MenuSlider("QireMana", "Min Mana to use : ", 40);
        }
        public class KeysSettings
        {
            public static MenuKeyBind TurretKey = new MenuKeyBind("TurretKey", "Turret Key", Keys.A, KeyBindType.Toggle);
            public static MenuKeyBind SemiE = new MenuKeyBind("SemiE", "E Using Key", Keys.G, KeyBindType.Press);
            public static MenuKeyBind SemiR = new MenuKeyBind("SemiR", "R Using Key", Keys.T, KeyBindType.Press);
            public static MenuKeyBind FleeKey = new MenuKeyBind("FleeKey", "Flee Key", Keys.Z, KeyBindType.Press);

            public static MenuKeyBind AutoClearMinions = new MenuKeyBind("Auto Clear Minions", "Clear Minions AUTO", Keys.N, KeyBindType.Toggle);
        }
    }
}
