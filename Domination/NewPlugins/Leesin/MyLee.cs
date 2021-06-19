using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.Utility;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DominationAIO.NewPlugins
{
    public static class MyLee
    {
        public static Spell Q, W, E, R, Rout;
        public static Spell Ward, Smite, Flash;
        private static Menu LeeMn = new Menu("FunnySlayerLee", "FunnySlayer LeeSin", true);
        public static void MyLeeLoad()
        {
            Q = new Spell(SpellSlot.Q, 1200f);
            Q.SetSkillshot(0.25f, 55f, 1800f, true, SpellType.Line);
            W = new Spell(SpellSlot.W, 700f);
            E = new Spell(SpellSlot.E, 350f);
            R = new Spell(SpellSlot.R, 365f);
            R.SetTargetted(0.25f, float.MaxValue);
            Rout = new Spell(SpellSlot.Unknown, 1200f);
            Rout.SetSkillshot(0.75f, 60, 1200, false, SpellType.Line);
            Ward = new Spell(SpellSlot.Unknown, 600f);
            Smite = new Spell(ObjectManager.Player.GetSpellSlotFromName("summonersmite"), 500f);
            Flash = new Spell(ObjectManager.Player.GetSpellSlotFromName("summonerflash"), 400f);
            //Rout.SetSkillshot(0.25f, 100f, 1200f, false, SpellType.Line);


            spells.Add(Q);
            spells.Add(W);
            spells.Add(E);
            spells.Add(R);

            //FunnySlayerCommon.MenuClass.AddTargetSelectorMenu(LeeMn);
            LeeMenu.MenuAttack(LeeMn);

            Game.OnUpdate += Game_OnUpdate;
            AIBaseClient.OnProcessSpellCast += AIBaseClient_OnProcessSpellCast;
            AIBaseClient.OnBuffAdd += AIBaseClient_OnBuffAdd;
            //Drawing.OnEndScene += Drawing_OnEndScene;

            Game.OnUpdate += Game_OnUpdate1;
            Game.OnUpdate += Game_OnUpdate2;
            Drawing.OnEndScene += Drawing_OnEndScene1;
        }

        private static void Drawing_OnEndScene1(EventArgs args)
        {
            if (LeeMenu.LeeR.LogicInsec.Active)
            {
                var maxhit = 1;
                var pos = Vector3.Zero;
                var insectarget = FindtargetInsecMaxhit(out maxhit, out pos);
                if (insectarget != null)
                {
                    Render.Circle.DrawCircle(insectarget.Position, 100, System.Drawing.Color.Black);
                    Render.Circle.DrawCircle(pos, 100, System.Drawing.Color.Blue);
                    var objpos = Drawing.WorldToScreen(pos);

                    Drawing.DrawText(objpos, System.Drawing.Color.Blue, maxhit.ToString());


                    var line = new Geometry.Rectangle(insectarget.Position.ToVector2(), insectarget.Position.Extend(pos, -900).ToVector2(), 60);
                    line.Draw(System.Drawing.Color.Blue);

                }
            }
        }

        private static void DoStepQ()
        {

        }

        private static void DoStepW()
        {

        }

        private static void DoStepR()
        {

        }

        private static void Insec()
        {

        }

        private static void Game_OnUpdate2(EventArgs args)
        {
            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    DoCombo();
                    break;
                case OrbwalkerMode.LaneClear:
                    DoClear();
                    break;
            }
        }

        private static void Game_OnUpdate1(EventArgs args)
        {
            foreach (var item in spells)
            {
                if (!isState2(item))
                {
                    switch (item.Slot)
                    {
                        case SpellSlot.Unknown:
                            break;
                        case SpellSlot.Q:
                            if (Variables.GameTimeTickCount - LastQTimer >= 500)
                                LastQTimer = 0;
                            break;
                        case SpellSlot.W:
                            LastWTimer = 0;
                            break;
                        case SpellSlot.E:
                            LastETimer = 0;
                            break;
                    }
                }
            }

            if (Ward.Slot == SpellSlot.Unknown)
            {
                if (Ward.Slot != ObjectManager.Player.GetWardSlot().SpellSlot && ObjectManager.Player.GetWardSlot().SpellSlot != SpellSlot.Unknown)
                    Ward.Slot = ObjectManager.Player.GetWardSlot().SpellSlot;
            }
        }

        private static void Drawing_OnEndScene(EventArgs args)
        {
            var obj = FindWBestTarget(Game.CursorPos, false);
            if (obj != null)
                Render.Circle.DrawCircle(obj.Position, 50, System.Drawing.Color.White);
        }

        private static int LastQBuffTime = 0;
        private static void AIBaseClient_OnBuffAdd(AIBaseClient sender, AIBaseClientBuffAddEventArgs args)
        {
            if (!sender.IsAlly)
            {
                if (args.Buff.Name.ToString().Contains("BlindMonkQOne") || args.Buff.Name.ToString().Contains("blindmonkqonechaos"))
                    LastQBuffTime = Variables.GameTimeTickCount;
            }
        }

        private static int LastQTimer, LastWTimer, LastETimer = 0;
        private static int LastSpell = 0;
        private static void AIBaseClient_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (sender.IsMe)
            {
                if (args.Slot <= SpellSlot.E)
                {
                    LastSpell = Variables.GameTimeTickCount;
                }
                if (args.Slot == SpellSlot.Q && !isState2(Q))
                {
                    LastQTimer = Variables.GameTimeTickCount;
                }
                if (args.Slot == SpellSlot.W)
                {
                    LastWTimer = Variables.GameTimeTickCount;
                }
                if (args.Slot == SpellSlot.E)
                {
                    LastETimer = Variables.GameTimeTickCount;
                }
            }
        }
        private static List<Spell> spells = new List<Spell>();
        private static void Game_OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
            {
                LastSpell = 0;
            }

            DoWWrad();
        }

        private static AIHeroClient FindtargetInsecMaxhit(out int maxtargethit)
        {
            AIHeroClient FindtargetInsecMaxhit = null;
            int maxhit = 1;
            List<AIHeroClient> targethit = new List<AIHeroClient>();

            if (HaveQBuff() != null)
            {
                var ThisPoly = new Geometry.Rectangle(ObjectManager.Player.Position.Extend(HaveQBuff().Position, -150), HaveQBuff().Position.Extend(ObjectManager.Player.Position, -150), 350);
                var alltarget = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(1300) && !i.IsDead && ThisPoly.IsInside(i.Position)).OrderByDescending(i => i.DistanceToPlayer());
                AIHeroClient target;
                Geometry.Circle circle;
                Vector2 pos;
                Vector3 lastpos;
                Geometry.Rectangle line;
                int targethitCount = 1;
                List<AIHeroClient> thetargethit = new List<AIHeroClient>();
                IEnumerable<AIHeroClient> check = null;

                foreach (var item in alltarget)
                {
                    target = item;
                    circle = new Geometry.Circle(target.Position, 250, 40);

                    foreach (var itiem in circle.Points)
                    {
                        pos = itiem;
                        targethitCount = 1;
                        lastpos = target.Position.Extend(pos, -900f);
                        {
                            line = new Geometry.Rectangle(target.Position, lastpos, 60);

                            thetargethit.Add(target);

                            check = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget() && i.NetworkId != target.NetworkId && line.IsInside(Rout.GetPrediction(i).CastPosition) && i.DistanceToPlayer() <= 900 * 2 && !i.IsDead);
                            targethitCount += check.Count();
                            thetargethit.AddRange(check);

                            /*foreach (var gettarget in GameObjects.EnemyHeroes.Where(i => i.IsValidTarget() && i.NetworkId != target.NetworkId && line.IsInside(Rout.GetPrediction(i).CastPosition) && i.DistanceToPlayer() <= 900 * 2 && !i.IsDead))
                            {
                                newtarget = gettarget;
                                targethitCount += 1;
                                thetargethit.Add(newtarget);
                            }*/

                            if (targethitCount > maxhit)
                            {
                                FindtargetInsecMaxhit = target;
                                maxhit = targethitCount;
                                targethit = thetargethit;
                            }
                        }
                    }
                }
            }

            maxtargethit = maxhit;
            return FindtargetInsecMaxhit;
        }

        private static AIHeroClient FindtargetInsecMaxhit(out int maxtargethit, out Vector3 PosInsec)
        {
            AIHeroClient FindtargetInsecMaxhit = null;
            int maxhit = 1;
            Vector3 posinsec = Vector3.Zero;
            bool tomuchleesin = false;

            tomuchleesin = GameObjects.AllyHeroes.Where(i => i.CharacterName == ObjectManager.Player.CharacterName).Count() >= 2;

            if (HaveQBuff() != null && !tomuchleesin)
            {
                var ThisPoly = new Geometry.Rectangle(ObjectManager.Player.Position.Extend(HaveQBuff().Position, -150), HaveQBuff().Position.Extend(ObjectManager.Player.Position, -150), 350);
                var alltarget = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(1300) && !i.IsDead && ThisPoly.IsInside(i.Position)).OrderByDescending(i => i.DistanceToPlayer());
                Geometry.Circle circle;
                Vector3 lastpos;
                Geometry.Rectangle line;
                int targethitCount = 1;
                List<AIHeroClient> thetargethit = new List<AIHeroClient>();
                IEnumerable<AIHeroClient> check = null;
                List<AIHeroClient> targethit = new List<AIHeroClient>();


                foreach (var item in alltarget)
                {
                    var target = item;
                    circle = new Geometry.Circle(target.Position, 250, LeeMenu.LeeR.user.Value);

                    foreach (var itiem in circle.Points)
                    {
                        var pos = itiem;
                        targethitCount = 1;
                        lastpos = target.Position.Extend(pos, -900f);
                        {
                            line = new Geometry.Rectangle(target.Position, lastpos, 60);

                            thetargethit.Add(target);

                            check = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget() && i.NetworkId != target.NetworkId && line.IsInside(Rout.GetPrediction(i).CastPosition) && i.DistanceToPlayer() <= 900 * 2 && !i.IsDead);
                            targethitCount += check.Count();
                            thetargethit.AddRange(check);

                            /*foreach (var gettarget in GameObjects.EnemyHeroes.Where(i => i.IsValidTarget() && i.NetworkId != target.NetworkId && line.IsInside(Rout.GetPrediction(i).CastPosition) && i.DistanceToPlayer() <= 900 * 2 && !i.IsDead))
                            {
                                newtarget = gettarget;
                                targethitCount += 1;
                                thetargethit.Add(newtarget);
                            }*/

                            if (targethitCount > maxhit)
                            {
                                FindtargetInsecMaxhit = target;
                                maxhit = targethitCount;
                                targethit = thetargethit;
                                posinsec = pos.ToVector3();
                            }
                        }
                    }
                }
            }

            if (tomuchleesin)
            {
                var alltarget = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(1300) && !i.IsDead && i.DistanceToPlayer() <= 375)
                    .OrderByDescending(i => i.DistanceToPlayer());
                Geometry.Circle circle;
                Vector3 lastpos;
                Geometry.Rectangle line;
                int targethitCount = 1;
                List<AIHeroClient> thetargethit = new List<AIHeroClient>();
                IEnumerable<AIHeroClient> check = null;
                List<AIHeroClient> targethit = new List<AIHeroClient>();

                foreach (var item in alltarget)
                {
                    var target = item;
                    circle = new Geometry.Circle(target.Position, 250, LeeMenu.LeeR.user.Value);

                    foreach (var itiem in circle.Points)
                    {
                        var pos = itiem;
                        targethitCount = 1;
                        lastpos = target.Position.Extend(pos, -900f);
                        {
                            line = new Geometry.Rectangle(target.Position, lastpos, 60);

                            thetargethit.Add(target);

                            check = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget() && i.NetworkId != target.NetworkId && line.IsInside(Rout.GetPrediction(i).CastPosition) && i.DistanceToPlayer() <= 900 * 2 && !i.IsDead);
                            targethitCount += check.Count();
                            thetargethit.AddRange(check);

                            /*foreach (var gettarget in GameObjects.EnemyHeroes.Where(i => i.IsValidTarget() && i.NetworkId != target.NetworkId && line.IsInside(Rout.GetPrediction(i).CastPosition) && i.DistanceToPlayer() <= 900 * 2 && !i.IsDead))
                            {
                                newtarget = gettarget;
                                targethitCount += 1;
                                thetargethit.Add(newtarget);
                            }*/

                            if (targethitCount > maxhit)
                            {
                                FindtargetInsecMaxhit = target;
                                maxhit = targethitCount;
                                targethit = thetargethit;
                                posinsec = pos.ToVector3();
                            }
                        }
                    }
                }
            }

            PosInsec = posinsec;
            maxtargethit = maxhit;
            return FindtargetInsecMaxhit;
        }

        private static AIHeroClient FindtargetInsecMaxhit(out int maxtargethit, out List<AIHeroClient> gettargethit)
        {
            AIHeroClient FindtargetInsecMaxhit = null;
            int maxhit = 1;
            List<AIHeroClient> targethit = new List<AIHeroClient>();

            if (HaveQBuff() != null)
            {
                var ThisPoly = new Geometry.Rectangle(ObjectManager.Player.Position.Extend(HaveQBuff().Position, -200), HaveQBuff().Position.Extend(ObjectManager.Player.Position, -200), 350);
                var alltarget = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(1300) && !i.IsDead && ThisPoly.IsInside(i.Position)).OrderByDescending(i => i.DistanceToPlayer());
                AIHeroClient target;
                Geometry.Circle circle;
                Vector2 pos;
                Vector3 lastpos;
                Geometry.Rectangle line;
                int targethitCount = 1;
                List<AIHeroClient> thetargethit = new List<AIHeroClient>();
                IEnumerable<AIHeroClient> check = null;

                foreach (var item in alltarget)
                {
                    target = item;
                    circle = new Geometry.Circle(target.Position, 250, 40);

                    foreach (var itiem in circle.Points)
                    {
                        pos = itiem;
                        targethitCount = 1;
                        lastpos = target.Position.Extend(pos, -900f);
                        {
                            line = new Geometry.Rectangle(target.Position, lastpos, 60);

                            thetargethit.Add(target);

                            check = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget() && i.NetworkId != target.NetworkId && line.IsInside(Rout.GetPrediction(i).CastPosition) && i.DistanceToPlayer() <= 900 * 2 && !i.IsDead);
                            targethitCount += check.Count();
                            thetargethit.AddRange(check);

                            /*foreach (var gettarget in GameObjects.EnemyHeroes.Where(i => i.IsValidTarget() && i.NetworkId != target.NetworkId && line.IsInside(Rout.GetPrediction(i).CastPosition) && i.DistanceToPlayer() <= 900 * 2 && !i.IsDead))
                            {
                                newtarget = gettarget;
                                targethitCount += 1;
                                thetargethit.Add(newtarget);
                            }*/

                            if (targethitCount > maxhit)
                            {
                                FindtargetInsecMaxhit = target;
                                maxhit = targethitCount;
                                targethit = thetargethit;
                            }
                        }
                    }
                }
            }
            gettargethit = targethit;
            maxtargethit = maxhit;
            return FindtargetInsecMaxhit;
        }

        private static AIHeroClient FindtargetInsecMaxhit()
        {
            AIHeroClient FindtargetInsecMaxhit = null;
            int maxhit = 1;

            if (HaveQBuff() != null)
            {
                var ThisPoly = new Geometry.Rectangle(ObjectManager.Player.Position.Extend(HaveQBuff().Position, -200), HaveQBuff().Position.Extend(ObjectManager.Player.Position, -200), 350);
                var alltarget = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(1300) && !i.IsDead && ThisPoly.IsInside(i.Position)).OrderByDescending(i => i.DistanceToPlayer());
                AIHeroClient target;
                Geometry.Circle circle;
                Vector2 pos;
                Vector3 lastpos;
                Geometry.Rectangle line;
                int targethitCount = 1;
                AIHeroClient newtarget;
                List<AIHeroClient> targethit = new List<AIHeroClient>();
                foreach (var item in alltarget)
                {
                    target = item;
                    circle = new Geometry.Circle(target.Position, 250, 40);

                    foreach (var itiem in circle.Points)
                    {
                        pos = itiem;
                        targethitCount = 1;
                        lastpos = target.Position.Extend(pos, -900f);
                        {
                            line = new Geometry.Rectangle(target.Position, lastpos, 60);

                            targethit.Add(target);
                            foreach (var gettarget in GameObjects.EnemyHeroes.Where(i => i.IsValidTarget() && i.NetworkId != target.NetworkId && i.DistanceToPlayer() <= 900 * 2 && !i.IsDead))
                            {
                                newtarget = gettarget;
                                var check = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget() && i.NetworkId != target.NetworkId && line.IsInside(Rout.GetPrediction(i).CastPosition) && i.DistanceToPlayer() <= 900 * 2 && !i.IsDead);
                                targethitCount += check.Count();
                                targethit.AddRange(check);
                            }
                            if (targethitCount >= 2 && targethitCount > maxhit)
                            {
                                FindtargetInsecMaxhit = target;
                                maxhit = targethitCount;
                            }
                        }
                    }
                }
            }

            return FindtargetInsecMaxhit;
        }

        /// <summary>
        /// New Logic Insec
        /// </summary>
        /// <param name="TheDictionary">Find pos and target</param>
        /// <returns></returns>
        /// 

        private static Dictionary<Vector2, int> TheDictionary = new Dictionary<Vector2, int>();
        private static void PosUpdate()
        {
            TheDictionary.Clear();
            if (HaveQBuff() != null)
            {
                var line = new Geometry.Rectangle(ObjectManager.Player.Position.Extend(HaveQBuff().Position, -150)
                    , HaveQBuff().Position.Extend(ObjectManager.Player.Position, -150), 250);
                var alltargetinline = GameObjects.EnemyHeroes.Where(i => line.IsInside(i.Position) && i.IsValidTarget() && !i.IsDead)
                    .OrderByDescending(i => i.DistanceToPlayer());
                AIHeroClient target = null;
                Geometry.Circle circle;
                foreach (var item in alltargetinline)
                {
                    target = item;
                    circle = new Geometry.Circle(target.Position, 250, 40);

                    foreach (var itiem in circle.Points)
                    {
                        if (TheDictionary.ContainsKey(itiem) == false)
                        {
                            TheDictionary.Add(itiem, target.NetworkId);
                        }
                    }
                }
            }
        }

        private static Vector3 FindPos(out AIHeroClient target, out int hitcount)
        {
            AIHeroClient gtarget = null;

            Vector3 thepos = Vector3.Zero;
            int hit = 1;
            var vector = TheDictionary.Keys.ToArray().OrderByDescending(i => i.DistanceToPlayer());
            foreach (var item in vector)
            {
                int max = 1;
                var pos = item;
                var ttarget = GameObjects.EnemyHeroes.Where(i => i.NetworkId == TheDictionary[pos]).FirstOrDefault();
                var lastpos = ttarget.Position.Extend(pos, -900f);
                {
                    var line = new Geometry.Rectangle(ttarget.Position, lastpos, 60);

                    int targethitCount = 1;
                    List<AIHeroClient> targethit = new List<AIHeroClient>() { ttarget };
                    AIHeroClient newtarget = null;
                    foreach (var gettarget in GameObjects.EnemyHeroes.Where(i => i.IsValidTarget()
                    && i.NetworkId != ttarget.NetworkId && i.DistanceToPlayer() <= 900 * 2 && !i.IsDead))
                    {
                        newtarget = gettarget;

                        var pred = Rout.GetPrediction(newtarget);
                        if (pred.Hitchance >= HitChance.High && !targethit.Contains(newtarget))
                        {
                            if (line.IsInside(pred.UnitPosition) || line.IsInside(pred.CastPosition))
                            {
                                targethitCount += 1;
                                targethit.Add(newtarget);
                            }
                        }
                    }

                    if (targethitCount > max)
                    {
                        gtarget = ttarget;
                        thepos = pos.ToVector3();
                        hit = targethitCount;
                    }
                }
            }
            hitcount = hit;
            target = gtarget;
            return thepos;
        }

        private static Vector3 FindPosInsecMaxhit(AIHeroClient target)
        {
            Vector3 FindPosInsecMaxhit = Vector3.Zero;
            int maxhit = 1;
            Vector2 findpos = Vector2.Zero;
            AIHeroClient newtarget = null;
            int targethitCount = 1;
            List<AIHeroClient> targethit = new List<AIHeroClient>();
            if (target != null && target.IsValidTarget())
            {
                var circle = new Geometry.Circle(target.Position, 250, 40);

                foreach (var itiem in circle.Points)
                {
                    findpos = itiem;

                    var lastpos = target.Position.Extend(findpos, -900f);
                    {
                        var line = new Geometry.Rectangle(target.Position, lastpos, 60);

                        targethitCount = 1;
                        targethit.Add(target);

                        var check = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget() && i.NetworkId != target.NetworkId && line.IsInside(Rout.GetPrediction(i).CastPosition) && i.DistanceToPlayer() <= 900 * 2 && !i.IsDead);
                        targethitCount += check.Count();
                        targethit.AddRange(check);

                        if (targethitCount > maxhit)
                        {
                            FindPosInsecMaxhit = itiem.ToVector3();
                        }
                    }
                }
            }

            return FindPosInsecMaxhit;
        }
        private static void DoWWrad()
        {
            var wardslot = ObjectManager.Player.GetWardSlot().SpellSlot;
            if (wardslot == SpellSlot.Unknown)
                return;

            var wward = new Spell(wardslot, 600f);

            if (LeeMenu.LeeKey.WWardKeyPress.Active && W.IsReady() && !isState2(W))
            {
                AIBaseClient obj = FindWBestTarget(Game.CursorPos, false, LeeMenu.LeeW.BonusWRange.Value);
                int value = 0;
                int extend = 0;

                Vector3 pos = Vector3.Zero;
                if (obj != null && obj.DistanceToPlayer() <= 700 && obj.Distance(Game.CursorPos) <= LeeMenu.LeeW.BonusWRange.Value)
                {
                    if (W.CastOnUnit(obj))
                        return;
                }
                else
                {
                    pos = ObjectManager.Player.Position.Extend(Game.CursorPos, 595f);
                    Render.Circle.DrawCircle(pos, 50, System.Drawing.Color.White);

                    if (pos.IsWall())
                    {
                        extend = 0;
                        for (int i = 0; i < 200; i++)
                        {
                            value = i;
                            var posextend = Game.CursorPos.Extend(ObjectManager.Player.Position, value);
                            if (posextend.IsWall())
                            {
                                extend = i;
                                continue;
                            }
                            else
                            {
                                break;
                            }
                        }

                        if (extend < 100)
                        {
                            if (wward.IsReady())
                            {
                                if (wward.Cast(pos))
                                    if (W.Cast(pos))
                                        return;
                            }
                        }
                    }
                    else
                    {
                        if (wward.IsReady())
                        {
                            if (wward.Cast(pos))
                                if (W.Cast(pos))
                                    return;
                        }
                    }
                }

                return;
            }
        }
        private static bool DoROut()
        {
            if (!R.IsReady())
                return false;



            return false;
        }

        private enum Step
        {
            None,
            CastQ,
            CastWardW,
            CastW,
            CastR
        }
        private static Step InsexStep = Step.None;
        private static void DoCombo()
        {
            {
                var rtarget = R.GetTarget();

                if (R.IsReady() && W.IsReady() && !isState2(W))
                {
                    var maxhit = 1;

                    //try find another pos
                    if (ObjectManager.Player.IsDashing())
                    {
                        if (LeeMenu.LeeR.LogicInsec.Index == 0)
                        {
                            PosUpdate();
                            AIHeroClient target = null;
                            var pos = FindPos(out target, out maxhit);
                            var obj = FindWBestTarget(pos, false, 25);
                            Render.Circle.DrawCircle(pos, 50, System.Drawing.Color.White);
                            if (target != null && maxhit >= 2 && pos.DistanceToPlayer() <= 600 && target.DistanceToPlayer() <= 350)
                            {
                                if (obj != null && obj.Distance(pos) <= 25 && obj.DistanceToPlayer() <= 700)
                                {
                                    if (W.CastOnUnit(obj))
                                    {
                                        R.Cast(target);
                                        return;
                                    }
                                }
                                else
                                {
                                    if (WardReady())
                                    {
                                        if (Ward.Cast(pos))
                                            if (W.Cast(pos))
                                            {
                                                R.Cast(target);
                                                return;
                                            }
                                    }
                                }
                            }
                        }
                        else
                        {
                            var pos = Vector3.Zero;
                            var readtarget = FindtargetInsecMaxhit(out maxhit, out pos);

                            //var pos = FindPosInsecMaxhit(readtarget);

                            if (readtarget.DistanceToPlayer() <= 375 && !pos.IsZero && pos.DistanceToPlayer() <= 600)
                            {
                                if (maxhit >= 2)
                                {
                                    var obj = FindWBestTarget(pos, false, 25);

                                    if (obj != null && obj.Distance(pos) <= 25 && obj.DistanceToPlayer() <= 700)
                                    {
                                        if (W.CastOnUnit(obj))
                                        {
                                            R.Cast(readtarget);
                                            return;
                                        }
                                    }
                                    else
                                    {
                                        if (WardReady())
                                        {
                                            if (Ward.Cast(pos))
                                                if (W.Cast(pos))
                                                {
                                                    R.Cast(readtarget);
                                                    return;
                                                }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }

                if (rtarget != null && R.IsReady())
                {
                    if (R.GetDamage(rtarget) + (rtarget.HasQBuff() ? Q.GetDamage(rtarget) : 0) >= rtarget.Health)
                    {
                        R.Cast(rtarget);
                        return;
                    }

                    var lastpos = rtarget.Position.Extend(ObjectManager.Player.Position, -900f);

                    {
                        var line = new Geometry.Rectangle(rtarget.Position, lastpos, 60);
                        line.Draw(System.Drawing.Color.Blue);

                        int targethitCount = 1;
                        List<AIHeroClient> targethit = new List<AIHeroClient>() { rtarget };
                        AIHeroClient target = null;
                        foreach (var gettarget in GameObjects.EnemyHeroes.Where(i => i.IsValidTarget() && i.DistanceToPlayer() <= 1200 && !i.IsDead))
                        {
                            target = gettarget;

                            var pred = Rout.GetPrediction(target);
                            if (pred.Hitchance >= HitChance.High && !targethit.Contains(target))
                            {
                                if (line.IsInside(pred.CastPosition) || line.IsInside(pred.UnitPosition))
                                {
                                    targethitCount += 1;
                                    targethit.Add(target);
                                }
                            }
                        }

                        if (targethitCount >= 3)
                        {
                            R.Cast(rtarget);
                            return;
                        }

                        if (targethitCount >= 2)
                        {
                            if (targethit.Where(i => i.Health <= R.GetDamage(i)).FirstOrDefault() != null)
                            {
                                R.Cast(rtarget);
                                return;
                            }
                        }
                    }

                    if (LeeMenu.LeeKey.InSec.Active && WardReady() && W.IsReady() && !isState2(W))
                    {
                        /*int max = 1;
                        AIHeroClient readtarget = rtarget;
                        foreach (var item in GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(375) && !i.IsDead))
                        {
                            var target = item;
                            var circle = new Geometry.Circle(target.Position, 250, 40);
                            foreach (var itiem in circle.Points)
                            {
                                var pointaccepted = itiem;

                                lastpos = target.Position.Extend(pointaccepted, -900f);

                                var line = new Geometry.Rectangle(rtarget.Position, lastpos, 60);

                                int targethitCount = 1;
                                List<AIHeroClient> targethit = new List<AIHeroClient>() { rtarget };
                                foreach (var gettarget in GameObjects.EnemyHeroes.Where(i => i.IsValidTarget() && i.DistanceToPlayer() <= 1200 && !i.IsDead))
                                {
                                    var ntarget = gettarget;

                                    var pred = Rout.GetPrediction(target);
                                    if (pred.Hitchance >= HitChance.High && !targethit.Contains(target))
                                    {
                                        if (line.IsInside(pred.CastPosition) || line.IsInside(pred.UnitPosition))
                                        {
                                            targethitCount += 1;
                                            targethit.Add(target);
                                        }
                                    }
                                }

                                if (max < targethitCount)
                                {
                                    readtarget = target;
                                }
                            }                           
                        }*/
                    }
                }
            }

            if (Q.IsReady() && (LeeMenu.LeeQ.UseQ1.Enabled || LeeMenu.LeeQ.UseQ2.Enabled))
                if (CastQ())
                    return;

            if (CastWandE())
                return;

            return;
        }

        private static bool CastQ()
        {
            if (Q.IsReady())
            {
                if (!isState2(Q) && LeeMenu.LeeQ.UseQ1.Enabled)
                {
                    var findqtarget = ObjectManager.Get<AIHeroClient>().Where(i =>
                    i.IsValidTarget(1300)
                    && !i.IsDead
                    && !i.IsAlly
                    && FSpred.Prediction.Prediction.GetPrediction(Q, i).Hitchance
                    >= FSpred.Prediction.HitChance.High)
                        .OrderBy(i => i.Health)
                        .FirstOrDefault();

                    if (findqtarget != null)
                    {
                        var pred = FSpred.Prediction.Prediction.GetPrediction(Q, findqtarget);
                        if (pred.Hitchance >= FSpred.Prediction.HitChance.High)
                        {
                            return Q.Cast(pred.CastPosition);
                        }
                    }
                    else
                    {
                        var refindtarget = TargetSelector.GetTargets(Q.Range + 400, DamageType.Physical).FirstOrDefault();

                        if (refindtarget != null && Q.GetPrediction(refindtarget).Hitchance < HitChance.High)
                        {
                            var pred = FSpred.Prediction.Prediction.GetPrediction(Q, refindtarget);

                            if (pred.CastPosition.DistanceToPlayer() <= 1100 && pred.Hitchance >= FSpred.Prediction.HitChance.High)
                            {
                                return Q.Cast(pred.CastPosition);
                            }
                            else
                            {
                                var gobj = ObjectManager.Get<AIBaseClient>().Where(i => i.IsAlly && !i.IsDead && (W.CanCast(i) || i.Name.ToString().ToLower().Contains("ward")) && i.DistanceToPlayer() <= W.Range);
                                if (W.IsReady() && !isState2(W) && (gobj != null && gobj.FirstOrDefault() != null))
                                {
                                    foreach (var objs in gobj.Where(i => i.Distance(refindtarget.Position) <= 1100))
                                    {
                                        var obj = objs;
                                        var newspell = new Spell(SpellSlot.Unknown, 1100);
                                        newspell.SetSkillshot(0.25f, 55f, 1800f, true, SpellType.Line, HitChance.High, obj.Position, obj.Position);

                                        if (Q.GetCollision(obj.Position.ToVector2(), new List<Vector2>() { pred.CastPosition.ToVector2() }).Count < 1 && newspell.GetPrediction(refindtarget).Hitchance >= HitChance.High)
                                        {
                                            return W.CastOnUnit(obj);
                                        }
                                    }
                                }
                                else
                                {
                                    if (SmiteReady() && LeeMenu.LeeQ.QSmite.Enabled)
                                    {
                                        var qline = new Geometry.Rectangle(ObjectManager.Player.Position, pred.CastPosition, 65);

                                        var getcollision = GameObjects.AiBaseObjects.Where(i => !i.IsDead && i.IsValidTarget() && qline.IsInside(i.Position));

                                        if (getcollision.Count() == 1 && pred.CastPosition.DistanceToPlayer() <= 1100)
                                        {
                                            var cc = getcollision.FirstOrDefault();
                                            if (cc.Type == GameObjectType.AIMinionClient && cc.IsValidTarget(Smite.Range) && cc.Health <= SmiteDmg())
                                            {
                                                if (Q.Cast(pred.CastPosition))
                                                {
                                                    DelayAction.Add(250 + (int)(cc.DistanceToPlayer() / Q.Speed * 1000) - 100, () =>
                                                    {
                                                        Smite.CastOnUnit(cc);
                                                    }
                                                    );
                                                    return true;
                                                }
                                            }
                                        }

                                        /*var qcollision = Q.GetCollision(refindtarget.Position.ToVector2(), new List<Vector2>() { pred.CastPosition.ToVector2() });
                                        if(qcollision.Count == 1 && pred.CastPosition.DistanceToPlayer() <= 1100)
                                        {
                                            if(qcollision.FirstOrDefault().Type == GameObjectType.AIMinionClient && qcollision.FirstOrDefault().IsValidTarget(Smite.Range) && qcollision.FirstOrDefault().Health <= SmiteDmg())
                                            {
                                                if(Q.Cast(pred.CastPosition))
                                                {
                                                    return Smite.CastOnUnit(qcollision.FirstOrDefault());
                                                }
                                            }
                                        }*/
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    if (LeeMenu.LeeQ.UseQ2.Enabled)
                    {
                        var target = HaveQBuff();
                        if (target != null)
                        {
                            if (target.Type == GameObjectType.AIHeroClient)
                            {
                                if (Variables.GameTimeTickCount - LastQTimer >= LeeMenu.LeeQ.ShouldWait.Value || Variables.GameTimeTickCount - LastQBuffTime >= 2500 || target.Health <= Q.GetDamage(target) + ObjectManager.Player.GetAutoAttackDamage(target))
                                {
                                    return Q.Cast();
                                }
                                if (LeeMenu.LeeQ.ShouldWait.Enabled)
                                {

                                    if (FSpred.Prediction.Prediction.PredictUnitPosition(target, (int)(Variables.GameTimeTickCount - LastQBuffTime + 100)).DistanceToPlayer() >= 1250 || target.Position.DistanceToPlayer() >= 1290)
                                    {
                                        return Q.Cast();
                                    }
                                }
                                else
                                {
                                    return Q.Cast();
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }


        private static double SmiteDmg()
        {
            var basedmg = 370;
            if (ObjectManager.Player.Level <= 18)
            {
                basedmg = 800 + (ObjectManager.Player.Level - 17) * 50;
            }
            if (ObjectManager.Player.Level <= 14)
            {
                basedmg = 600 + (ObjectManager.Player.Level - 9) * 40;
            }
            if (ObjectManager.Player.Level <= 9)
            {
                basedmg = 450 + (ObjectManager.Player.Level - 4) * 30;
            }
            if (ObjectManager.Player.Level <= 4)
            {
                basedmg += 4 * 20;
            }

            return basedmg;
        }

        private static bool CastWandE()
        {
            if (TargetSelector.GetTarget(E.Range, DamageType.Physical).Health <= ObjectManager.Player.GetAutoAttackDamage(TargetSelector.GetTarget(E.Range, DamageType.Physical)) + E.GetDamage(TargetSelector.GetTarget(E.Range, DamageType.Physical)) && E.IsReady() && !isState2(E))
            {
                return E.Cast();
            }
            if (W.IsReady() && TargetSelector.GetTarget(ObjectManager.Player.GetCurrentAutoAttackRange() + 100, DamageType.Physical) != null && (isState2(W) ? Variables.GameTimeTickCount - LastWTimer >= 1000 : Variables.GameTimeTickCount - LastSpell >= 1000))
            {
                if (isState2(W))
                {
                    return W.Cast();
                }
                else
                {
                    var obj = FindWBestTarget(TargetSelector.GetTarget(W.Range, DamageType.Physical).Position, false, LeeMenu.LeeW.BonusWRange.Value, new List<GameObjectType>() { GameObjectType.AIHeroClient });
                    if (obj != null)
                        return W.CastOnUnit(obj);

                    return W.CastOnUnit(ObjectManager.Player);
                }
            }
            else
            {
                if (E.IsReady() && !FunnySlayerCommon.OnAction.OnAA && !FunnySlayerCommon.OnAction.BeforeAA)
                {
                    if (!isState2(E))
                    {
                        if (TargetSelector.GetTarget(E.Range, DamageType.Physical) != null)
                        {
                            if (Variables.GameTimeTickCount - LastSpell >= 1000)
                            {
                                return E.Cast();
                            }
                            else
                            {
                                if (TargetSelector.GetTarget(E.Range, DamageType.Physical).Health <= ObjectManager.Player.GetAutoAttackDamage(TargetSelector.GetTarget(E.Range, DamageType.Physical)) + E.GetDamage(TargetSelector.GetTarget(E.Range, DamageType.Physical)) + (R.IsReady() ? R.GetDamage(TargetSelector.GetTarget(E.Range, DamageType.Physical)) + (Q.IsReady() ? Q.GetDamage(TargetSelector.GetTarget(E.Range, DamageType.Physical)) : 0) : 0))
                                {
                                    return E.Cast();
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }
        private static void DoClear()
        {
            if (!Q.IsReady() && !W.IsReady() && !E.IsReady())
                return;

            var jungles = GameObjects.Jungle.Where(i => i.IsValidTarget(Q.Range)).OrderBy(i => i.MaxHealth);

            if (jungles.FirstOrDefault() != null)
            {
                var jungle = jungles.FirstOrDefault();

                if (Q.IsReady() && (isState2(Q) ? Variables.GameTimeTickCount - LastQTimer >= 1000 : Variables.GameTimeTickCount - LastSpell >= 1000))
                {
                    if (isState2(Q))
                    {
                        Q.Cast();
                        return;
                    }
                    else
                    {
                        Q.Cast(jungle.Position);
                        return;
                    }
                }

                if (W.IsReady() && jungle.DistanceToPlayer() <= ObjectManager.Player.GetCurrentAutoAttackRange() + 100 && (isState2(W) ? Variables.GameTimeTickCount - LastWTimer >= 1000 : Variables.GameTimeTickCount - LastSpell >= 1000))
                {
                    var wobj = FindWBestTarget(jungle.Position, false, 200, new List<GameObjectType>() { GameObjectType.AIHeroClient });
                    if (wobj != null)
                    {
                        W.CastOnUnit(wobj);
                        return;
                    }
                    else
                    {
                        if (isState2(W))
                        {
                            W.Cast();
                            return;
                        }
                        else
                        {
                            W.CastOnUnit(ObjectManager.Player);
                            return;
                        }
                    }
                }

                if (E.IsReady() && jungle.DistanceToPlayer() <= E.Range && Variables.GameTimeTickCount - LastSpell >= 1000)
                {
                    if (isState2(E))
                    {
                        E.Cast();
                        return;
                    }
                    else
                    {
                        E.Cast();
                        return;
                    }
                }
            }
        }

        /*public static FSpred.Prediction.CollisionableObjects[] QCollisionableObjects = new FSpred.Prediction.CollisionableObjects[]
        {
            FSpred.Prediction.CollisionableObjects.Minions,
            FSpred.Prediction.CollisionableObjects.YasuoWall,
            FSpred.Prediction.CollisionableObjects.Heroes
        };
     
        public static List<GameObjectType> ObjectType = new List<GameObjectType>()
        {
            GameObjectType.AIHeroClient,
            GameObjectType.AIMinionClient,
        };*/

        private static AIBaseClient FindWBestTarget(Vector3 pos, bool checktype = false, float rangcheck = 200, List<GameObjectType> type = null)
        {
            var objcanwon = new List<AIBaseClient>();
            objcanwon.AddRange(ObjectManager.Get<AIBaseClient>().Where(i => i.IsAlly && !i.IsDead && i.DistanceToPlayer() <= W.Range && i.Distance(pos) <= rangcheck).ToList());
            objcanwon.AddRange(GameObjects.AllyHeroes);
            objcanwon.RemoveAll(i => i.IsDead || i.DistanceToPlayer() > 700 || i.Distance(pos) >= rangcheck || i.NetworkId == ObjectManager.Player.NetworkId);
            objcanwon.OrderBy(i => i.Type == GameObjectType.AIHeroClient).ThenBy(i => i.Distance(pos));
            if (objcanwon != null && objcanwon.FirstOrDefault() != null)
            {
                if (checktype && type != null)
                {
                    return objcanwon.Where(i => type.Contains(i.Type) || i.Name.ToString().ToLower().Contains("ward")).FirstOrDefault();
                }

                return objcanwon.FirstOrDefault();

            }
            return null;
        }


        private static IOrderedEnumerable<AIBaseClient> FindWBestTarget(Vector3 pos)
        {
            var objcanwon = ObjectManager.Get<AIBaseClient>().Where(i => i.IsAlly && !i.IsDead && (W.CanCast(i) || i.Name.ToString().ToLower().Contains("ward")) && i.DistanceToPlayer() <= W.Range).OrderBy(i => i.Distance(pos)).ThenBy(i => i.Type == GameObjectType.AIHeroClient);
            if (objcanwon != null && objcanwon.FirstOrDefault() != null)
            {
                return objcanwon;

            }

            return null;
        }


        private static bool isState2(Spell skill)
        {
            return skill.Name.ToString().Contains("Two");
            //false;
        }

        private static bool HasQBuff(this AIBaseClient unit)
        {
            return (unit.HasBuff("BlindMonkQOne") || unit.HasBuff("blindmonkqonechaos"));
        }
        private static AIBaseClient HaveQBuff()
        {
            return
                ObjectManager.Get<AIBaseClient>()
                    .Where(a => a.IsValidTarget(1300) && a.HasQBuff())
                    .FirstOrDefault();
        }

        public static bool FlashReady()
        {
            if (Flash.Slot != SpellSlot.Unknown && Flash.IsReady())
            {
                return true;
            }

            return false;
        }
        public static bool SmiteReady()
        {
            if (Smite.Slot != SpellSlot.Unknown && Smite.IsReady())
            {
                return true;
            }

            return false;
        }
        public static bool WardReady()
        {
            if (Ward.Slot != SpellSlot.Unknown && Ward.IsReady())
            {
                return true;
            }

            return false;
        }
    }

    public static class LeeMenu
    {
        public static void MenuAttack(this Menu menu)
        {
            var Q = new Menu("QSettings", "Q Settings");
            Q.Add(LeeQ.UseQ1);
            Q.Add(LeeQ.UseQ2);
            Q.Add(LeeQ.ShouldWait);
            Q.Add(LeeQ.QSmite);
            Q.Add(LeeQ.QonTurret);

            var W = new Menu("WSettings", "W Settings");
            W.Add(LeeW.UseW1);
            W.Add(LeeW.UseW2);
            W.Add(LeeW.BonusWRange);
            var E = new Menu("ESettings", "E Settings");
            E.Add(LeeE.UseE1);
            E.Add(LeeE.UseE2);

            var R = new Menu("RSettings", "R Settings");
            R.Add(LeeR.user);
            R.Add(LeeR.LogicInsec).Permashow();
            var key = new Menu("key", "Keys");
            key.Add(LeeMenu.LeeKey.WWardKeyPress).Permashow();
            key.Add(LeeKey.InSec).Permashow();

            menu.Add(Q);
            menu.Add(W);
            menu.Add(E);
            menu.Add(R);
            menu.Add(key);
            menu.Attach();
        }
        public static class LeeQ
        {
            public static MenuBool UseQ1 = new MenuBool("UseQ1Lee", "Use Q1 combo");

            public static MenuBool UseQ2 = new MenuBool("UseQ2Lee", "Use Q2 combo");

            public static MenuSliderButton ShouldWait = new MenuSliderButton("Should Wait", "Should Wait", 2500, 1000, 2800);

            public static MenuBool QSmite = new MenuBool("QSmite", "Q Smite");

            public static MenuKeyBind QonTurret = new MenuKeyBind("Qonturret", "Q on Turret (Toggle)", Keys.T, KeyBindType.Toggle);

        }

        public static class LeeW
        {
            public static MenuBool UseW1 = new MenuBool("W1Lee", "Use W1");
            public static MenuBool UseW2 = new MenuBool("W2Lee", "Use W2");
            public static MenuSlider BonusWRange = new MenuSlider("BonusWRange", "W Bonus Range", 150, 0, 200);
        }

        public static class LeeE
        {
            public static MenuBool UseE1 = new MenuBool("E1Lee", "Use E1");
            public static MenuBool UseE2 = new MenuBool("E2Lee", "Use E2", false);
        }

        public static class LeeR
        {
            public static MenuSliderButton user = new MenuSliderButton("user", "R Combo", 40, 40, 150);
            public static MenuList LogicInsec = new MenuList("LogicInsec", "Logic Insec", new string[] { "Old", "New" }, 0);
        }

        public static class LeeKey
        {
            public static MenuKeyBind WWardKeyPress = new MenuKeyBind("WWardKeyPress", "W Ward (Press)", Keys.G, KeyBindType.Press);
            public static MenuKeyBind InSec = new MenuKeyBind("InSec", "Insec Key (Toggle)", Keys.A, KeyBindType.Toggle);
        }
    }
}
