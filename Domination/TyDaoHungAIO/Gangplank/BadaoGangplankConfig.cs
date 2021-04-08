using System;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.Utility;
using SharpDX;
using Color = System.Drawing.Color;
using Menu = EnsoulSharp.SDK.MenuUI.Menu;

namespace DaoHungAIO.Champions.Gangplank
{
    public static class BadaoGangplankConfig
    {
        public static Menu config;
        public static void BadaoActivate()
        {

            // spells init
            BadaoMainVariables.Q = new Spell(SpellSlot.Q, 625);
            BadaoMainVariables.W = new Spell(SpellSlot.W);
            BadaoMainVariables.E = new Spell(SpellSlot.E,1000);
            BadaoMainVariables.R = new Spell(SpellSlot.R);

            // main menu
            config = new Menu(ObjectManager.Player.CharacterName, "badao " + ObjectManager.Player.CharacterName, true);


            // Combo
            Menu Combo = config.Add(new Menu("Combo", "Combo"));
            BadaoGangplankVariables.ComboE1 = Combo.Add(new MenuBool("ComboE1", "Place 1st Barrel")).SetValue(true);
            BadaoGangplankVariables.ComboQSave = Combo.Add(new MenuBool("ComboQSave", "Save Q if can detonate barrels")).SetValue(false);


            // Harass
            Menu Harass = config.Add(new Menu("Harass", "Harass"));
            BadaoGangplankVariables.HarassQ = Harass.Add(new MenuBool("HarassQ", "Q")).SetValue(true);


            // LaneClear
            Menu LaneClear = config.Add(new Menu("LaneClear", "LaneClear"));
            BadaoGangplankVariables.LaneQ = LaneClear.Add(new MenuBool("LaneQ", "Use Q last hit")).SetValue(true);

            // JungleClear
            Menu JungleClear = config.Add(new Menu("JungleClear", "JungleClear"));
            BadaoGangplankVariables.JungleQ = JungleClear.Add(new MenuBool("jungleQ", "Use Q last hit")).SetValue(true);

            //Auto
            Menu Auto = config.Add(new Menu("Auto", "Auto"));
            BadaoGangplankVariables.AutoWLowHealth = Auto.Add(new MenuBool("AutoWLowHealth", "W when low health")).SetValue(true);
            BadaoGangplankVariables.AutoWLowHealthValue = Auto.Add(new MenuSlider("AutoWLowHealthValue", "% HP to W", 20, 1, 100));
            BadaoGangplankVariables.AutoWCC = Auto.Add(new MenuBool("AutoWCC", "W anti CC")).SetValue(true);

            //Flee nam o trong config luon nha
            Menu Flee = config.Add(new Menu("Flee", "Flee"));
            BadaoGangplankVariables.FleeKey = Flee.Add(new MenuKeyBind("FleeKey", "Flee Key", Keys.G, KeyBindType.Press));

            //Draw nam o trong config luon ne
            Menu Draw = config.Add(new Menu("Draw", "Draw"));
            BadaoGangplankVariables.DrawQ = Draw.Add(new MenuBool("DrawQ", "Q")).SetValue(true);
            BadaoGangplankVariables.DrawE = Draw.Add(new MenuBool("DrawE", "E")).SetValue(true);
            BadaoGangplankVariables.DrawEPlacement = Draw.Add(new MenuBool("DrawEPlacement", "E Chain range")).SetValue(true);

            // attach to mainmenu
            config.Attach();

            Drawing.OnDraw += Drawing_OnDraw;
            Game.OnUpdate += Game_OnUpdate;
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (!BadaoGangplankVariables.FleeKey.GetValue<MenuKeyBind>().Active)
                return;
            Orbwalker.Orbwalk(null, Game.CursorPos);
            if (BadaoMainVariables.Q.IsReady())
            {
                foreach (var barrel in BadaoGangplankBarrels.QableBarrels())
                {
                    if (BadaoMainVariables.Q.Cast(barrel.Bottle) == CastStates.SuccessfullyCasted)
                        return;
                }
            }
            if (Orbwalker.CanAttack())
            {
                foreach (var barrel in BadaoGangplankBarrels.AttackableBarrels())
                {
                    Orbwalker.AttackEnabled = false;
                    Orbwalker.MoveEnabled = false;
                    DelayAction.Add(100 + Game.Ping, () =>
                    {
                        Orbwalker.AttackEnabled = true;
                        Orbwalker.MoveEnabled = true;
                    }); 
                    if (ObjectManager.Player.IssueOrder(GameObjectOrder.AttackUnit, barrel.Bottle))
                        return;
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
                return;
            if (BadaoGangplankVariables.DrawQ.GetValue<MenuBool>().Enabled)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, BadaoMainVariables.Q.Range, Color.Yellow);
            }
            if (BadaoGangplankVariables.DrawE.GetValue<MenuBool>().Enabled)
            {
                Render.Circle.DrawCircle(ObjectManager.Player.Position, BadaoMainVariables.E.Range, Color.Pink);
            }
            if (BadaoGangplankVariables.DrawEPlacement.GetValue<MenuBool>().Enabled)
            {
                foreach (var barrel in BadaoGangplankBarrels.Barrels)
                {
                    Render.Circle.DrawCircle(barrel.Bottle.Position, 660, Color.Red );
                }
            }
        }
    }
}
