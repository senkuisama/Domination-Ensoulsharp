using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using SharpDX;
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
        private static bool Loaded = false;

        public static void CheckOnAction(bool attackMenu = false, Menu attackto = null)
        {
            if (Loaded)
                return;

            Loaded = true;
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

            if (MenuClass.SetOrbWalkerTarget.Enabled)
            {
                AIBaseClient.OnProcessSpellCast += AIBaseClient_OnProcessSpellCast;
                Game.OnUpdate += Game_OnUpdate1;
            }
            else
            {
                Game.OnUpdate -= Game_OnUpdate1;
                AIBaseClient.OnProcessSpellCast -= AIBaseClient_OnProcessSpellCast;
            }

            MenuClass.SetOrbWalkerTarget.ValueChanged += (a, e) =>
            {
                if (MenuClass.SetOrbWalkerTarget.Enabled)
                {
                    Game.OnUpdate += Game_OnUpdate1;
                    AIBaseClient.OnProcessSpellCast += AIBaseClient_OnProcessSpellCast;
                }
                else
                {
                    Game.OnUpdate -= Game_OnUpdate1;
                    AIBaseClient.OnProcessSpellCast -= AIBaseClient_OnProcessSpellCast;
                }
            };
        }
        private static float CastTimeDelay => ObjectManager.Player.AttackCastDelay * 1000;
        private static void AIBaseClient_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (!sender.IsMe)
                return;

            if (Orbwalker.IsAutoAttack(args.SData.Name) && !Orbwalker.IsAutoAttackReset(args.SData.Name))
            {
                LastAttack = Variables.GameTimeTickCount;
            }
        }

        public static int LastDisableMove = 0;
        public static int LastAttack = 0;
        private static int LastSendPrintChat = 0;
        private static void Game_OnUpdate1(EventArgs args)
        {
            if(ObjectManager.Player.CharacterName == "Yasuo")
            {
                if (DominationAIO.NewPlugins.Yasuo.MyYS.CheckImDashing)
                {
                    Orbwalker.AttackEnabled = false;
                    ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    Orbwalker.MoveEnabled = true;


                    if (Orbwalker.ActiveMode <= OrbwalkerMode.LastHit)
                        Orbwalker.Orbwalk(null, Game.CursorPos);
                    else
                        Orbwalker.SetOrbwalkerPosition(Vector3.Zero);
                }
                else
                {
                    if (Orbwalker.ActiveMode <= OrbwalkerMode.LastHit)
                        Orbwalker.Orbwalk(Orbwalker.GetTarget(), Game.CursorPos);
                    else
                        Orbwalker.SetOrbwalkerPosition(Vector3.Zero);

                    Orbwalker.AttackEnabled = true;
                }

                Set();
            }
            else
            {
                Set();
            }                       
            if (AfterAA && !ObjectManager.Player.HasBuff("katarinarsound") && !ObjectManager.Player.Spellbook.IsChanneling)
            {
                LastDisableMove = 0;
                Orbwalker.MoveEnabled = true;
            }
        }

        private static void Set()
        {
            if (Variables.GameTimeTickCount - LastAttack < CastTimeDelay || ObjectManager.Player.HasBuff("katarinarsound"))
            {
                LastDisableMove = Variables.GameTimeTickCount;
                Orbwalker.MoveEnabled = false;
            }
            else
            {
                Orbwalker.MoveEnabled = true;
            }
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
