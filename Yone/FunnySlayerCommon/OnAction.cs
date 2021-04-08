using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FunnySlayerCommon
{
    public static class OnAction
    {       
        private static bool baa, oaa, aaa, killminion, premove;

        public static void CheckOnAction(bool attackMenu = false, Menu attackto = null)
        {
            if (attackMenu && attackto != null)
            {
                var menu = new Menu("___OnAction_MenuAttack", "_OrbWalker.OnAction", true);
                menu.Add(new MenuSeparator("___Orbwalker_OnAfterAttack", "Orbwalker.OnAfterAttack"));
                menu.Add(new MenuSeparator("___Orbwalker_OnBeforeAttack", "Orbwalker.OnBeforeAttack"));
                menu.Add(new MenuSeparator("___Orbwalker_OnAttack", "Orbwalker.OnAttack"));
                menu.Add(new MenuSeparator("___Orbwalker_OnNonKillableMinion", "Orbwalker.OnNonKillableMinion"));
                menu.Add(new MenuSeparator("___Orbwalker_OnPreMove", "Orbwalker.OnPreMove"));

                attackto.Add(menu);
            }     
            
            Orbwalker.OnAfterAttack += Orbwalker_OnAfterAttack;
            Orbwalker.OnBeforeAttack += Orbwalker_OnBeforeAttack;
            Orbwalker.OnAttack += Orbwalker_OnAttack;
            Orbwalker.OnNonKillableMinion += Orbwalker_OnNonKillableMinion;
            Orbwalker.OnBeforeMove += Orbwalker_OnBeforeMove;
            //Game.OnUpdate += Game_OnUpdate;
        }

        private static void Orbwalker_OnBeforeMove(object sender, BeforeMoveEventArgs e)
        {
            if (aaa == true)
            {
                aaa = false;
            }
            if (baa == true)
            {
                baa = false;
            }
            if (oaa == true)
            {
                oaa = false;
            }
            if (killminion == true)
            {
                killminion = false;
            }
            if (premove == false)
            {
                premove = true;
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                if(aaa == true)
                {
                    aaa = false;
                }
                if(baa == true)
                {
                    baa = false;
                }
                if(oaa == true)
                {
                    oaa = false;
                }
                if(killminion == true)
                {
                    killminion = false;
                }
                if(premove == true)
                {
                    premove = false;
                }
            }
            else
            {
                if (ObjectManager.Player.IsWindingUp)
                {
                    if (oaa == false)
                    {
                        oaa = true;
                    }
                    if (baa == true)
                    {
                        baa = false;
                    }
                }

                if (ObjectManager.Player.IsMoving)
                {
                    if (oaa == true)
                    {
                        oaa = false;
                    }
                    if (baa == true)
                    {
                        baa = false;
                    }
                }
            }
        }

        private static void Orbwalker_OnNonKillableMinion(object sender, NonKillableMinionEventArgs e)
        {
            if (killminion == false)
            {
                killminion = true;
            }
            if (aaa == true)
            {
                aaa = false;
            }
            if (baa == true)
            {
                baa = false;
            }
            if (oaa == true)
            {
                oaa = false;
            }
            if (premove == true)
            {
                premove = false;
            }
        }

        private static void Orbwalker_OnAttack(object sender, AttackingEventArgs e)
        {
            if (aaa == true)
            {
                aaa = false;
            }
            if (baa == true)
            {
                baa = false;
            }
            if (oaa == false)
            {
                oaa = true;
            }
            if (killminion == true)
            {
                killminion = false;
            }
            if (premove == true)
            {
                premove = false;
            }
        }

        private static void Orbwalker_OnBeforeAttack(object sender, BeforeAttackEventArgs e)
        {
            if (aaa == true)
            {
                aaa = false;
            }
            if (baa == false)
            {
                baa = true;
            }
            if (oaa == true)
            {
                oaa = false;
            }
            if (killminion == true)
            {
                killminion = false;
            }
            if (premove == true)
            {
                premove = false;
            }
        }

        private static void Orbwalker_OnAfterAttack(object sender, AfterAttackEventArgs e)
        {
            if (aaa == false)
            {
                aaa = true;
            }
            if (baa == true)
            {
                baa = false;
            }
            if (oaa == true)
            {
                baa = false;
            }
            if (killminion == true)
            {
                killminion = false;
            }
            if (premove == true)
            {
                premove = false;
            }
        }

        public static bool AfterAA
        {
            get
            {
                return aaa;
            }
            set
            {
                aaa = value;
            }
        }
        public static bool BeforeAA
        {
            get
            {
                return baa;
            }
            set
            {
                baa = value;
            }
        }
        public static bool OnAA
        {
            get
            {
                return oaa;
            }
            set
            {
                oaa = value;
            }
        }
        public static bool OnNonKillableMinion
        {
            get
            {
                return killminion;
            }
            set
            {
                killminion = value;
            }
        }
        public static bool OnPreMove
        {
            get
            {
                return premove;
            }
            set
            {
                premove = value;
            }
        }
    }
}
