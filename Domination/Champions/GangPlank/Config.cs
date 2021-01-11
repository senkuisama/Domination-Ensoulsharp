using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;


namespace e.Motion_Gangplank
{
    class Config
    {
        public static Menu Menu;
        public void Initialize()
        {
            Menu = new Menu("e.Motion Gangplank", "e.Motion Gangplank mainMenu", true);

            //Orbwalker
            /*Menu orbwalkerMenu = new Menu("Orbwalker", "orbwalkerMain");
            Orbwalker = new Orbwalking.Orbwalker(orbwalkerMenu);
            Menu.AddSubMenu(orbwalkerMenu);*/

            //Key
            Menu keyMenu = new Menu("Key","key");
            keyMenu.Add(new MenuKeyBind("key.q", "Semi-Automatic Q", Keys.Q, KeyBindType.Press));
            keyMenu.Add(new MenuList("key.emode", "Semi Automatic E Mode", new[] { "Never use", "Place Connecting Barrel", "Place Connecting Barrel + Explode" }, 0));
            keyMenu.Add(new MenuKeyBind("key.e", "Semi-Automatic E", Keys.A, KeyBindType.Press));
            keyMenu.Add(new MenuKeyBind("key.r", "Semi-Automatic R", Keys.Z, KeyBindType.Press));
            Menu.Add(keyMenu);
           
            //Combo
            Menu comboMenu = new Menu("Combo", "combo");
            comboMenu.Add(new MenuBool("combo.q", "Use Q"));
            comboMenu.Add(new MenuBool("combo.qe", "Use Q on Barrel").SetValue(true));
            comboMenu.Add(new MenuBool("combo.aae", "Use Autoattack on Barrel").SetValue(true));
            comboMenu.Add(new MenuBool("combo.e", "Use E").SetValue(true));
            comboMenu.Add(new MenuBool("combo.ex", "Use E to Extend").SetValue(true));
            comboMenu.Add(new MenuBool("combo.doublee", "Use Double E Combo").SetValue(false));
            comboMenu.Add(new MenuBool("combo.triplee", "Use Triple E Combo").SetValue(true));
            comboMenu.Add(new MenuBool("combo.r", "Use R"));
            comboMenu.Add(new MenuSlider("combo.rmin", "Minimum enemies for R", 3, 2,5));
            Menu.Add(comboMenu);

            //Harass
            Menu harassMenu = new Menu("Harass", "harass");
            harassMenu.Add(new MenuBool("harass.q", "Use Q").SetValue(true));
            Menu.Add(harassMenu);

            //Lasthit
            Menu lasthitMenu = new Menu("Lasthit", "lasthit");
            lasthitMenu.Add(new MenuBool("lasthit.q", "Use Q").SetValue(true));
            lasthitMenu.Add(new MenuBool("lasthit.qe", "Use Q on Barrels").SetValue(true));
            lasthitMenu.Add(new MenuSlider("lasthit.mana", "Minimum Mana %").SetValue(30));
            Menu.Add(lasthitMenu);

            //Killsteal
            Menu killstealMenu = new Menu("Killsteal","killsteal");
            killstealMenu.Add(new MenuBool("killsteal.q", "Use Q").SetValue(true));
            killstealMenu.Add(new MenuBool("killsteal.r", "Semi-Automatic R").SetValue(true));
            killstealMenu.Add(new MenuSlider("killsteal.minwave", "Minimum Waves for R", 6, 1, 18));
            Menu.Add(killstealMenu);

            //Drawings
            Menu drawingMenu = new Menu("Drawings","drawings");
            drawingMenu.Add(new MenuBool("drawings.warning", "Remember me to Upgrade Ult").SetValue(true));
            drawingMenu.Add(new MenuBool("drawings.ex", "Draw extended E").SetValue(true));
            drawingMenu.Add(new MenuBool("drawings.q", "Draw Q Range").SetValue(false));
            drawingMenu.Add(new MenuBool("drawings.e", "Draw E Range").SetValue(false));
            Menu.Add(drawingMenu);

            //Cleanse
            Menu cleanseMenu = new Menu("Cleanse","cleanse");
            cleanseMenu.Add(new MenuBool("cleanse.w", "Use W to Cleanse").SetValue(true));

            Menu cleanseBuffs = new Menu("Enable Cleanse for:","cleanse.bufftypes");
            cleanseBuffs.Add(new MenuBool("cleanse.bufftypes.slow", "Slow").SetValue(false));
            cleanseBuffs.Add(new MenuBool("cleanse.bufftypes.poison", "Poison").SetValue(true));
            cleanseBuffs.Add(new MenuBool("cleanse.bufftypes.blind", "Blind").SetValue(true));
            cleanseBuffs.Add(new MenuBool("cleanse.bufftypes.silence", "Silence").SetValue(true));
            cleanseBuffs.Add(new MenuBool("cleanse.bufftypes.stun", "Stun").SetValue(true));
            cleanseBuffs.Add(new MenuBool("cleanse.bufftypes.fear", "Fear").SetValue(true));
            cleanseBuffs.Add(new MenuBool("cleanse.bufftypes.polymorph", "Polymorph").SetValue(true));
            cleanseBuffs.Add(new MenuBool("cleanse.bufftypes.snare", "Snare").SetValue(true));
            cleanseBuffs.Add(new MenuBool("cleanse.bufftypes.taunt", "Taunt").SetValue(true));
            cleanseBuffs.Add(new MenuBool("cleanse.bufftypes.suppression", "Suppression").SetValue(true));
            cleanseBuffs.Add(new MenuBool("cleanse.bufftypes.charm", "Charm").SetValue(true));
            cleanseMenu.Add(cleanseBuffs);

            Menu specialSkills = new Menu("Special Buffs","cleanse.special");
            specialSkills.Add(new MenuBool("cleanse.special.placeholder", "Will be added soon"));
            cleanseMenu.Add(specialSkills);

            Menu.Add(cleanseMenu);

            //Misc
            Menu miscMenu = new Menu("Miscellanious", "misc");
            miscMenu.Add(new MenuSlider("misc.additionalServerTick", "Additional Server Tick").SetValue(30));
            miscMenu.Add(new MenuSlider("misc.enemyReactionTime", "enemy Reaction Time", 150, 0 , 500));
            miscMenu.Add(new MenuSlider("misc.additionalReactionTime", "Additional Reaction Time on Direction Change", 50, 0, 200));
            miscMenu.Add(new MenuBool("misc.trye", "Always extend with E").SetValue(true));
            miscMenu.Add(new MenuBool("misc.autoE", "Place Barrels automatically").SetValue(true));
            Menu.Add(miscMenu);


            //Menu.AddToMainMenu();
            Menu.Attach();
        }
    }
}
