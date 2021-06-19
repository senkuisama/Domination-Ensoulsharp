using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunnySlayerCommon
{
    public static class MenuClass
    {
        public static string[] TargetWeight = new string[]
        {
                "Heath", //0
                "Armor", //1
                "Mana", //2
                "Auto Attack to die", //3
                "Ranged", //4
                "Melee", //5
        };

        public static Menu ThisMenu = null;
        public static MenuList SecondMenu;
        public static MenuList GetWeight = new MenuList("Get Weight", "Get Weight", TargetWeight, 0);
        public static MenuBool OrderByNearMouse = new MenuBool("OrdeByNearMouse", "Order by Near Mouse", false);
        public static MenuBool OrderByValue = new MenuBool("OrderByValue", "Order by Value", true);

        public static Menu GetPriority = new Menu("Get Priority", "Get Priority");

        public static MenuBool SetOrbWalkerTarget = new MenuBool("SetOrbWalkerTarget", "Set Combo Orbwalker target", false);
        public static MenuSliderButton DrawTarget = new MenuSliderButton("Draw Target", "Draw Target", 1000, 0, 2000, false);
     
        public static void AddTargetSelectorMenu(this Menu menu)
        {
            ThisMenu = new Menu("FS_Check Target", "FS_Check Target (Beta)");
            var FirstMenu = new MenuSeparator("FS Target Selector", "FS Target Selector");
            ThisMenu.Add(FirstMenu);
            SecondMenu = new MenuList("FS Select Mode", "FS Select Mode", new string[] { "Selected", "Weight", "Priority" }, 1);
            ThisMenu.Add(SecondMenu);

            var Selected = new MenuSeparator("Selected", "Selected");
            var Weight = new MenuSeparator("Weight", "Weight");
            var Priority = new MenuSeparator("Priority", "Priority");

            ThisMenu.Add(Selected);
                      
            foreach(var t in GameObjects.EnemyHeroes.ToArray())
            {
                if(t is AIHeroClient && t.IsEnemy)
                {
                    GetPriority.Add(new MenuSlider("FS Priority " + t.CharacterName + t.NetworkId, (t as AIHeroClient).CharacterName, (t as AIHeroClient).IsRanged ? 5 : 1, 0, 10));
                }
            }

            ThisMenu.Add(Weight);
            ThisMenu.Add(GetWeight);
            ThisMenu.Add(OrderByNearMouse);
            ThisMenu.Add(OrderByValue);


            ThisMenu.Add(Priority);
            ThisMenu.Add(GetPriority);

            ThisMenu.Add(SetOrbWalkerTarget);
            ThisMenu.Add(DrawTarget);

            if (SetOrbWalkerTarget.Enabled)
                Game.OnUpdate += Game_OnUpdate;
            else
                Game.OnUpdate -= Game_OnUpdate;

            SetOrbWalkerTarget.ValueChanged += (a, e) =>
            {
                if (SetOrbWalkerTarget.Enabled)
                    Game.OnUpdate += Game_OnUpdate;
                else
                {
                    Game.OnUpdate -= Game_OnUpdate;
                    Game.Print("Remove Set Combo Target");
                }
            };

            Drawing.OnDraw += Drawing_OnDraw;

            menu.Add(ThisMenu);
        }

        private static void Game_OnUpdate(EventArgs args)
        {          
            if (SetOrbWalkerTarget.Enabled && Orbwalker.ActiveMode <= OrbwalkerMode.Harass)
            {
                if (ObjectManager.Player.CanAttack || Orbwalker.CanAttack())
                {
                    var target = FSTargetSelector.GetFSTarget(ObjectManager.Player.GetCurrentAutoAttackRange());
                    if (target != null)
                    {
                        Orbwalker.Orbwalk(target, Game.CursorPos);
                    }
                    else
                    {
                        Orbwalker.Orbwalk(Orbwalker.GetTarget(), Game.CursorPos);
                    }
                }
                else
                {
                    ResetOrbwalker();
                }
            }
            else
            {
                ResetOrbwalker();
            }          
        }

        public static void ResetOrbwalker()
        {
            if (Orbwalker.ActiveMode > OrbwalkerMode.LastHit)
            {               
                Orbwalker.SetOrbwalkerPosition(Vector3.Zero);
            }
            else
            {
                Orbwalker.Orbwalk(Orbwalker.GetTarget(), Game.CursorPos);
            }
        }

        public static MenuSlider GetSPriority(AIHeroClient t)
        {
            return GetPriority.GetValue<MenuSlider>("FS Priority " + t.CharacterName + t.NetworkId);
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (DrawTarget.Enabled  && FSTargetSelector.GetFSTarget(DrawTarget.ActiveValue) != null)
            {
                var orbtarget = FSTargetSelector.GetFSTarget(ObjectManager.Player.GetCurrentAutoAttackRange());
                if (orbtarget != null && SetOrbWalkerTarget.Enabled && Orbwalker.ActiveMode <= OrbwalkerMode.Harass && (FunnySlayerCommon.OnAction.BeforeAA || FunnySlayerCommon.OnAction.OnAA))
                {
                    var targetpos = Drawing.WorldToScreen(orbtarget.Position);
                    Drawing.DrawLine(new Vector2(targetpos.X, targetpos.Y), new Vector2(targetpos.X + 25, targetpos.Y - 25), 5f, System.Drawing.Color.Red);
                    Drawing.DrawLine(new Vector2(targetpos.X, targetpos.Y), new Vector2(targetpos.X - 25, targetpos.Y - 25), 5f, System.Drawing.Color.Red);

                    Drawing.DrawCircle(orbtarget.Position, 100, System.Drawing.Color.Red);
                }
                else
                {
                    var targetpos = Drawing.WorldToScreen(FSTargetSelector.GetFSTarget(DrawTarget.ActiveValue).Position);
                    Drawing.DrawLine(new Vector2(targetpos.X, targetpos.Y), new Vector2(targetpos.X + 25, targetpos.Y - 25), 5f, System.Drawing.Color.White);
                    Drawing.DrawLine(new Vector2(targetpos.X, targetpos.Y), new Vector2(targetpos.X - 25, targetpos.Y - 25), 5f, System.Drawing.Color.White);

                    Drawing.DrawCircle(FSTargetSelector.GetFSTarget(DrawTarget.ActiveValue).Position, 100, System.Drawing.Color.Red);
                }                 
            }
        }
    }
}
