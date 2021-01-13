using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominationAIO.NewPlugins
{
    public static class Irelia
    {
        public static Spell Q = new Spell(EnsoulSharp.SpellSlot.Q, 580f);
        public static Spell W = new Spell(EnsoulSharp.SpellSlot.W, 800f);
        public static Spell E = new Spell(EnsoulSharp.SpellSlot.E, 775f);
        public static Spell R = new Spell(EnsoulSharp.SpellSlot.R, 900f);

        private static AIHeroClient Player = null;
        public static Vector3 E1Pos = Vector3.Zero;
        private static Menu IreliaMainMenu = null;
        public static float SheenTimer = 0;
        public static void NewIre()
        {
            if(ObjectManager.Player == null)
            {
                return;
            }
            else
            {
                Player = ObjectManager.Player;
            }

            Game.Print("Disable block dash spell in EzEvade misc");
        
            Q.SetTargetted(0f, float.MaxValue);
            W.SetCharged("IreliaW", "ireliawdefense", 800, 800, 0);
            E.SetSkillshot(0.25f, 5f, 2000f, false, SpellType.Line);
            R.SetSkillshot(0.4f, 200, 1000, false, SpellType.Line);

            IreliaMainMenu = new Menu("___Irelia", "FunnySlayer Irelia", true);
            IreliaMainMenu.AttackToMenu();
            IreliaMainMenu.Attach();

            Game.OnUpdate += EventsIrelia.KS;
            AIBaseClient.OnProcessSpellCast += EventsIrelia.AIBaseClient_OnProcessSpellCast;
            Game.OnUpdate += EventsIrelia.Combo;
            AIBaseClient.OnBuffRemove += EventsIrelia.AIBaseClient_OnBuffLose;
            Game.OnUpdate += EventsIrelia.Game_OnUpdate;
            Drawing.OnDraw += EventsIrelia.Drawing_OnDraw;

            Game.OnUpdate += Clear;
            Game.OnUpdate += Game_OnUpdate;
        }
       
        private static void Game_OnUpdate(EventArgs args)
        {
            if (Orbwalker.ActiveMode != OrbwalkerMode.Combo || R.IsReady() || MenuSettings.RSettings.Rcombo.Enabled)
            {
                LogicR.GetRPos1 = null;
                LogicR.GetRPos2 = null;
            }
        }

        private static void Clear(EventArgs args)
        {            
            if (ObjectManager.Player.IsDead)
                return;

            if (!Q.IsReady())
                return;

            if (!MenuSettings.ClearSettings.QireClear.Enabled) return;
            if (ObjectManager.Player.ManaPercent <= MenuSettings.ClearSettings.QireMana.Value) return;

            if (Orbwalker.ActiveMode != OrbwalkerMode.LaneClear && Orbwalker.ActiveMode != OrbwalkerMode.LastHit && !MenuSettings.KeysSettings.AutoClearMinions.Active)
                return;

            var minions = ObjectManager.Get<AIMinionClient>().Where(i => i != null && !i.IsDead && !i.IsAlly && i.IsValidTarget(600)).OrderBy(i => i.Health);

            if (minions == null)
                return;

            foreach (var min in minions)
            {
                if (Helper.CanQ(min) && min.DistanceToPlayer() < 600f)
                {
                    if (!Helper.UnderTower(min.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                    {
                        if (Q.Cast(min) == CastStates.SuccessfullyCasted || Q.CastOnUnit(min))
                            return;
                    }
                }
            }
        }
    }
}
