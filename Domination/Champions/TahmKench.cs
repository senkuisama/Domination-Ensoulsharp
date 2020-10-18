using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominationAIO.Champions
{
    public static class TahmKench
    {
        public static void Load()
        {
            Game.OnUpdate += Game_OnUpdate;
            var menu = new Menu("Menu", "...", true);
            menu.Add(Bug).Permashow();
            menu.Add(timer);
            menu.Add(SpamTimer);
            menu.Add(SpamTick);
            menu.Attach();
            Game.OnWndProc += Game_OnWndProc;
            Drawing.OnDraw += Drawing_OnDraw;
            AIBaseClient.OnProcessSpellCast += AIBaseClient_OnProcessSpellCast;
        }

        private static void AIBaseClient_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            //if (sender.IsMe && args.Slot == SpellSlot.Q)
                //Game.Print(args.TotalTime);
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (selectedobj != null)
                Render.Circle.DrawCircle(selectedobj.Position, 70, System.Drawing.Color.Green, 10);
        }

        private static AIBaseClient selectedobj = null;
        private static void Game_OnWndProc(GameWndProcEventArgs args)
        {
            if (args.Msg != (ulong)WindowsMessages.LBUTTONDOWN)
            {
                return;
            }

            selectedobj = ObjectManager.Get<AIBaseClient>().Where(i => i.IsValid()
            && (i.Type == GameObjectType.AIHeroClient || i.Type == GameObjectType.AIMinionClient) && i.Distance(Game.CursorPos) <= i.BoundingRadius + 100f).FirstOrDefault();

            if(selectedobj != null)
            new Spell(SpellSlot.W).Cast(selectedobj);
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (GameObjects.Player.IsDead || !Bug.Active || selectedobj == null)
                return;
            //Render.Circle.DrawCircle(ObjectManager.Player.Position.Extend(selectedobj.Position, ObjectManager.Player.GetRealAutoAttackRange() + 50), 50, System.Drawing.Color.Red);
            if ((new Spell(SpellSlot.Q)).IsReady())
                GameObjects.Player.Spellbook.CastSpell(SpellSlot.Q, Game.CursorPos);

            for (int i = 0; i <= SpamTimer.Value; i += 10)
            {
                DelayAction.Add(timer.Value + i, SpamW);
                continue;
            }
        }
        private static void SpamW()
        {
            //Game.Print("W");
            /*var W = new Spell(SpellSlot.W);
            //for (int i = 0; i <= 3; i++)
            //{
            W.Cast(true);*/

            for (int i = 0; i <= SpamTick.Value;i++)
            {
                GameObjects.Player.Spellbook.CastSpell(SpellSlot.W);
            }
            //Game.Print("W");

            //continue;
            //}
        }
        private static MenuKeyBind Bug = new MenuKeyBind("Bug", "Bug", System.Windows.Forms.Keys.A, KeyBindType.Press);
        private static MenuSlider timer = new MenuSlider("Timer", "Timer Delay after Q", 635, 0, 2000);
        private static MenuSlider SpamTick = new MenuSlider("SpamTick", "Spam Tick", 1, 0, 5);
        private static MenuSlider SpamTimer = new MenuSlider("SpamTimer", "Spam Time", 1000, 0, 2000);
    }
}
