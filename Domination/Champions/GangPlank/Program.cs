using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Remoting.Channels;
using System.Text;
using System.Threading.Tasks;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.Utility;
using SharpDX;
using SharpDX.Direct3D;
using Color = System.Drawing.Color;

namespace e.Motion_Gangplank
{
    internal class Program
    {
        /*public static void Main(string[] args)
        {
            GameEvent.OnGameLoad += Game_OnGameLoad;
            //Game_OnGameLoad(new EventArgs());
        }*/

        #region Declaration

        //private static int BarrelTime;
        private static bool BarrelAAForced;
        private static Random Rand = new Random();
        private static DelayManager QDelay;
        private static AIHeroClient UltimateTarget;
        private static bool UltimateToBeUsed;
        private static Dictionary<string, BuffType> Buffs = new Dictionary<string, BuffType>()
        {
            {"Charm",BuffType.Charm},
            {"Slow",BuffType.Slow },
            {"Poison",BuffType.Poison},
            {"Blind",BuffType.Blind},
            {"Silence",BuffType.Silence},
            {"Stun",BuffType.Stun},
            {"Flee",BuffType.Flee},
            {"Polymorph",BuffType.Polymorph},
            {"Snare",BuffType.Snare},
            {"Taunt",BuffType.Taunt},
            {"Suppression",BuffType.Suppression},
            {"Knockup",BuffType.Knockup},
            {"Knockback",BuffType.Knockback},
            {"Sleep",BuffType.Sleep}
        };
        private static readonly List<Vector2> BarrelPositions = new List<Vector2>()
        {
            new Vector2(1205, 12097),
            new Vector2(1335, 12468),
            new Vector2(1577, 12820),
            new Vector2(1872, 13011),
            new Vector2(2252, 13299),
            new Vector2(2632, 13520)
        };
        public static Spell Q, W, E, R;
        public static AIHeroClient Player => ObjectManager.Player;
        public static List<Barrel> AllBarrel = new List<Barrel>();
        public static Vector3 EnemyPosition;
        
        
        #endregion


        public static void Game_OnGameLoad()
        {
            if (Player.CharacterName != "Gangplank")
            {
                return;
            }
            Q = new Spell(SpellSlot.Q, 625);
            W = new Spell(SpellSlot.W, 0);
            E = new Spell(SpellSlot.E, 1000);
            R = new Spell(SpellSlot.R);
            #region Menu

            Config Menu = new Config();
            Menu.Initialize();
            #endregion

            QDelay = new DelayManager(Q,1500);
            Game.Print("<font color='#bb0000'>e</font>.<font color='#0000cc'>Motion</font> Gangplank loaded");
            //SetBarrelTime();
             

            #region Subscriptions

            Drawing.OnDraw += OnDraw;
            Game.OnUpdate += GameOnUpdate;
            GameObject.OnCreate += OnCreate;
            AIBaseClient.OnDoCast += CheckForBarrel;
            AIBaseClient.OnProcessSpellCast += OnProcessSpellCast;
           #endregion

        }

        private static void ForceCast(AIHeroClient target, Vector3 barrelPosition)
        {
            E.Cast(barrelPosition.ExtendToMaxRange(Player.Position.ExtendToMaxRange(target.Position, 980), 685));
        }

        private static void OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.Slot == SpellSlot.Q && E.IsReady(200) && args.Target.Name == "Barrel")
            {
                Barrel attackedBarrel = AllBarrel.Find(b => b.GetNetworkID() == args.Target.NetworkId);
                List<Barrel> barrelsInRange = GetBarrelsInRange(attackedBarrel).ToList();
                if (Config.Menu["Combo"]["combo.triplee"].GetValue<MenuBool>().Enabled && barrelsInRange.Any())
                {                   
                    //Triple-Logic
                    foreach (Barrel barrel in barrelsInRange)
                    {
                        DelayAction.Add(Helper.GetQTime(args.Target.Position) - 100 - Game.Ping / 2, () => InvokeTriplePlacement(barrel, AllBarrel));
                    }

                    foreach (var enemy in GameObjects.EnemyHeroes)
                    {
                        if (enemy.Position.Distance(args.Target.Position) >= 350 &&
                           !barrelsInRange.Any(b => b.GetBarrel().Distance(enemy) <= 350) &&
                            barrelsInRange.Any(b => b.GetBarrel().Distance(enemy) <= 850))
                        {
                           DelayAction.Add(400 + Game.Ping/2, () => ForceCast(enemy,barrelsInRange.First(b => b.GetBarrel().Distance(enemy) >= 350 && b.GetBarrel().Distance(enemy) <= 850).GetBarrel().Position));
                        }
                    }
                }
                if (Config.Menu["Combo"]["combo.doublee"].GetValue<MenuBool>().Enabled && attackedBarrel.GetBarrel().Distance(Player) >= 610)
                {
                    //Double Logic
                    foreach (var enemy in GameObjects.EnemyHeroes)
                    {
                        if (args.Target.Position.Distance(enemy.Position) >= 350 && args.Target.Position.Distance(enemy.Position) <= 850)
                        {
                            DelayAction.Add(200 + Game.Ping / 2, () => ForceCast(enemy,args.Target.Position));
                        }
                    }
                }
                
            }
        }


        private static void OnDraw(EventArgs args)
        {
            DrawRanges();
            KillstealDrawing();
            Warning();
            DrawE();
        }

        private static void DrawRanges()
        {           
            if (Config.Menu["Drawings"]["drawings.q"].GetValue<MenuBool>().Enabled && Q.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, Q.Range, Color.IndianRed);
            }
            if (Config.Menu["Drawings"]["drawings.e"].GetValue<MenuBool>().Enabled && E.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, E.Range, Color.IndianRed);
            }
        }

        private static void KillSteal()
        {
            if (Config.Menu["Killsteal"]["killsteal.q"].GetValue<MenuBool>().Enabled && Q.IsReady())
            {
                foreach (var enemy in GameObjects.EnemyHeroes)
                {
                    if (enemy.Health < Q.GetDamage(enemy) && Player.Distance(enemy) <= Q.Range)
                    {
                        Q.Cast(enemy);
                    }
                }
            }
            if (Config.Menu["Killsteal"]["killsteal.r"].GetValue<MenuBool>().Enabled && Config.Menu["Key"]["key.r"].GetValue<MenuKeyBind>().Active && R.IsReady() && UltimateToBeUsed && UltimateTarget != null)
            {
                R.Cast(SPrediction.Prediction.GetFastUnitPosition(UltimateTarget,150));
            }
        }

        private static void CheckForBarrel(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (args.Target != null && args.Target.Name == "Barrel")
            {
                for (int i = 0; i < AllBarrel.Count; i++)
                {
                    
                    if (AllBarrel.ElementAt(i).GetBarrel().NetworkId == args.Target.NetworkId)
                    {
                        if (sender.IsMelee)
                        {
                            AllBarrel.ElementAt(i).ReduceBarrelAttackTick();
                        }
                        else
                        {
                            int i1 = i;
                            DelayAction.Add((int)(args.Start.Distance(args.End)/args.SData.MissileSpeed), () => { AllBarrel.ElementAt(i1).ReduceBarrelAttackTick(); });
                        }
                    }
                }
            }
            
        }

        private static void CleanBarrel()
        {
            for (int i = AllBarrel.Count - 1; i >= 0; i--)
            {
                //Console.WriteLine("Looped");
                // ReSharper disable once CompareOfFloatsByEqualityOperator
                if (AllBarrel.ElementAt(i).GetBarrel() == null || AllBarrel.ElementAt(i).GetBarrel().Health == 0)
                {
                    AllBarrel.RemoveAt(i);
                    break;
                }
            }
        }

        private static void OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name == "Barrel")
            {
                AllBarrel.Add(new Barrel((AIMinionClient)sender));
            }
        }
        

        private static void GameOnUpdate(EventArgs args)
        {
            KillSteal();
            Harass();
            QDelay.CheckEachTick();
            AutoE();
            CleanBarrel();
            Combo();
            Lasthit();
            Cleanse();
            SemiAutomaticE();
        }

        private static void SemiAutomaticE()
        {
            if (E.IsReady() && Config.Menu["Key"]["key.e"].GetValue<MenuKeyBind>().Active)
            {
                
                if (Config.Menu["Key"]["key.emode"].GetValue<MenuList>().Index == 1)
                {
                    float lowest = 1600;
                    Vector3 bPos = Vector3.Zero;
                    foreach (Barrel barrel in AllBarrel)
                    {
                        if (barrel.GetBarrel().Distance(Game.CursorPos) < lowest)
                        {
                            bPos = barrel.GetBarrel().Position;
                            lowest = barrel.GetBarrel().Distance(Game.CursorPos);
                        }
                    }
                    if (lowest != 1600f)
                    {
                        E.Cast(bPos.Extend(Game.CursorPos, Math.Min(685, lowest)));
                    }
                }
                else if(Config.Menu["Key"]["key.emode"].GetValue<MenuList>().Index == 2 && Q.IsReady())
                {
                    IEnumerable<Barrel> toExplode = AllBarrel.Where(b => b.CanQNow() && b.GetBarrel().Distance(Player) <= Q.Range);
                    if (toExplode.Any())
                    {
                        float lowest = 1600;
                        Barrel bar = null;
                        foreach (Barrel barrel in AllBarrel)
                        {
                            if (barrel.GetBarrel().Distance(Game.CursorPos) < lowest)
                            {
                                bar = barrel;
                                lowest = barrel.GetBarrel().Distance(Game.CursorPos);
                            }
                        }
                        if(bar != null)
                        {
                            E.Cast(bar.GetBarrel().Position.Extend(Game.CursorPos, Math.Min(685, lowest)));
                            QDelay.Delay(bar.GetBarrel());
                        }
                    }
                }
            }
        }

        private static void Harass()
        {           
            if (Q.IsReady() && Config.Menu["Harass"]["harass.q"].GetValue<MenuBool>() && Orbwalker.ActiveMode == OrbwalkerMode.Harass)
            {
                AIHeroClient target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                if (target != null)
                {
                    Q.Cast(target);
                }
            }
        }

        private static void AutoE()
        {
            //Auto E - Static List
            if (Config.Menu["Miscellanious"]["misc.autoE"].GetValue<MenuBool>() && E.IsReady() && E.Instance.Ammo > 1 && !AllBarrel.Any(b => b.GetBarrel().Distance(Player) <= 1200))
            {
                AIHeroClient target = TargetSelector.GetTarget(1400,DamageType.Physical);
                List<Vector2> possiblePositions = BarrelPositions.Where(pos => pos.Distance(Player) <= E.Range).ToList();
                if (target != null && possiblePositions.Count != 0)
                {
                    float minDist = 2000;
                    Vector2 castPos = Vector2.Zero;
                    foreach (var pos in possiblePositions.Where(pos => pos.Distance(target) < minDist))
                    {
                        castPos = new Vector2(pos.X + Rand.Next(0,21) - 10, pos.Y + Rand.Next(0,21) - 10);
                        minDist = pos.Distance(target);
                    }
                    E.Cast(castPos);
                    
                }
                
            }
        }

        private static void DrawE()
        {            
            if (E.IsReady() && Config.Menu["Drawings"]["drawings.ex"].GetValue<MenuBool>())
            {
                float lowest = 1600;
                Vector3 bPos = Vector3.Zero;
                foreach (Barrel barrel in AllBarrel)
                {
                    if (barrel.GetBarrel().Distance(Game.CursorPos) < lowest)
                    {
                        bPos = barrel.GetBarrel().Position;
                        lowest = barrel.GetBarrel().Distance(Game.CursorPos);
                    }
                }
                if (lowest != 1600f)
                {
                    Render.Circle.DrawCircle(bPos.ExtendToMaxRange(Game.CursorPos,685),350,Color.ForestGreen);
                    Drawing.DrawLine(Drawing.WorldToScreen(bPos),Drawing.WorldToScreen(bPos.ExtendToMaxRange(Game.CursorPos,685)),5,Color.ForestGreen);
                }
            }
        }

        private static void Warning()
        {
            if ((Player.Position.Distance(new Vector3(394, 461, 171)) <= 1000 ||
                 Player.Position.Distance(new Vector3(14340, 14391, 170)) <= 1000) &&
                Player.GetBuffCount("gangplankbilgewatertoken") >= 500 && Config.Menu["Drawings"]["drawings.warning"].GetValue<MenuBool>())
            {
                Drawing.DrawText(200,200,Color.Red,"Don't forget to buy Ultimate Upgrade with Silver Serpents");
            }
        }

        private static void KillstealDrawing()
        {
            if (Config.Menu["Killsteal"]["killsteal.r"].GetValue<MenuBool>().Enabled && R.IsReady())
            {
                int minKillWave = 20;
                UltimateTarget = null;
                foreach (AIHeroClient enemy in GameObjects.EnemyHeroes)
                {
                    if (enemy.IsTargetable && enemy.IsVisible && !enemy.IsDead)
                    {
                        int killWave = 1 + (int)((enemy.Health - (Player.HasBuff("GangplankRUpgrade2")?(R.Instance.Level + 20 + Player.TotalMagicalDamage*0.1)*3:0))/R.GetDamage(enemy));
                        if (killWave < minKillWave)
                        {
                            minKillWave = killWave;
                            UltimateTarget = enemy;
                        }
                    }
                }
                //Config.Menu["Killsteal"]["killsteal.minwave"].GetValue<MenuSlider>().Value
                if (UltimateTarget != null && minKillWave <= (Player.HasBuff("GangplankRUpgrade1") ? 18 : 12) &&
                    minKillWave <= Config.Menu["Killsteal"]["killsteal.minwave"].GetValue<MenuSlider>().Value)
                {
                    UltimateToBeUsed = true;
                    Drawing.DrawText(200, 260, Color.Tomato,
                        UltimateTarget.CharacterName + " is killable " +
                        (minKillWave < 1 ? "only with Death Daughter [R] Upgrade" : "with " + minKillWave + " R Waves"));
                }
                else
                {
                    UltimateToBeUsed = false;
                }
            }
        }


        private static void Combo(bool extended = false,AIHeroClient sender = null)
        {
            if (Orbwalker.ActiveMode != OrbwalkerMode.Combo)
            {
                return;
            }

            //if (Config.Item("combo.r").GetValue<bool>())
            //{
            //    AIHeroClient RTarget = GameObjects.EnemyHeroes.FirstOrDefault(t => t.CountAlliesInRange(660) > 0);
            //    if (RTarget != null)
            //    {
            //        R.CastIfWillHit(RTarget, Config.Item("combo.rmin").GetValue<Slider>().Value);
            //    }
            //}
            //Config.Menu["Combo"]["combo.qe"].GetValue<MenuBool>()
            if (Config.Menu["Combo"]["combo.aae"].GetValue<MenuBool>() && Orbwalker.CanAttack())
            {
                List<Barrel> barrelsInAutoAttackRange = AllBarrel.Where(b => b.GetBarrel().Distance(Player) <= ObjectManager.Player.GetRealAutoAttackRange() && b.CanAANow()).ToList();
                if (barrelsInAutoAttackRange.Any() && (Player.Buffs.All(buff => buff.Name != "gangplankpassiveattack")))
                {
                    BarrelAAForced = false;
                    foreach(Barrel b in barrelsInAutoAttackRange)
                    {
                        if(GameObjects.EnemyHeroes.Any(enemy => b.GetBarrel().Position.CannotEscapeFromAA(enemy) || GetBarrelsInRange(b).Any(bar => bar.GetBarrel().Position.CannotEscapeFromAA(enemy))))
                        {
                            Orbwalker.Orbwalk(b.GetBarrel(), Game.CursorPos);
                            BarrelAAForced = true;
                        }
                    }
                }
            }

            if (Config.Menu["Combo"]["combo.qe"].GetValue<MenuBool>() && Q.IsReady() && !BarrelAAForced)
            {
                AIHeroClient target = TargetSelector.GetTarget(1200, DamageType.Physical);
                if (target != null)
                {
                    EnemyPosition = target.Position;
                    Helper.GetPredPos(target);
                    if (extended && target != sender)
                    {
                        extended = false;
                    }

                    foreach (var b in AllBarrel)
                    {
                        if (b.CanQNow() && (b.GetBarrel().Position.CannotEscape(target, extended) || GetBarrelsInRange(b).Any(bb => bb.GetBarrel().Position.CannotEscape(target, extended, true))))
                        {
                            QDelay.Delay(b.GetBarrel());
                            break;
                        }
                    }
                    if (E.IsReady() && !QDelay.Active())
                    {
                        if (Config.Menu["Combo"]["combo.doublee"].GetValue<MenuBool>().Enabled)
                        {
                            foreach (var b in AllBarrel)
                            {
                                if (b.CanQNow() && b.GetBarrel().Distance(Player) > 615 &&
                                    b.GetBarrel().Distance(target) < 850)
                                {
                                    Q.Cast(b.GetBarrel());
                                    break;
                                }
                            }
                        }
                        if (Config.Menu["Combo"]["combo.ex"].GetValue<MenuBool>())
                        {
                            foreach (var b in AllBarrel)
                            {
                                var castPos = b.GetBarrel().Position.ExtendToMaxRange(Helper.PredPos.ToVector3(),685);

                                if (b.CanQNow() && castPos.Distance(Player.Position) < 1000 &&
                                    castPos.CannotEscape(target, extended, true))
                                {
                                    E.Cast(castPos);
                                    QDelay.Delay(b.GetBarrel());
                                    break;
                                }
                            }
                        }
                    }
                }
            }

            //Triple - Logic
            if (Q.IsReady() && E.IsReady() && Config.Menu["Combo"]["combo.triplee"].GetValue<MenuBool>().Enabled)
            {
                IEnumerable<Barrel> validBarrels = AllBarrel.Where(b => b.CanQNow() && b.GetBarrel().Distance(Player) <= 625);
                foreach (Barrel validBarrel in validBarrels)
                {
                    IEnumerable<Barrel> inRange = GetBarrelsInRange(validBarrel);
                    if (
                        inRange.Any(
                            b =>
                                GameObjects.EnemyHeroes.Any(
                                    e => b.GetBarrel().Distance(e.Position) < 1100 && e.Distance(Player.Position) < 1000)))
                    {
                        Q.Cast(validBarrel.GetBarrel());
                    }
                }
            }
            if (Q.IsReady() && E.Instance.Ammo >= 2 && Config.Menu["Combo"]["combo.triplee"].GetValue<MenuBool>().Enabled)
            {
                List<Barrel> GetValidBarrels = AllBarrel.Where(b => b.CanQNow(400) && b.GetBarrel().Distance(Player) <= 625).ToList();
                AIHeroClient target = TargetSelector.GetTarget(1200, DamageType.Physical);
                if (target != null && GetValidBarrels.Any(b => b.GetBarrel().Distance(target) <= 1200))
                {
                    E.Cast(GetValidBarrels.First(b => b.GetBarrel().Distance(target) <= 1200).GetBarrel().Position.ExtendToMaxRange(Player.Position.ExtendToMaxRange(target.Position, 980), 685));
                    DelayAction.Add(600, () => QDelay.Delay(GetValidBarrels.First().GetBarrel()));
                }
            }
            //Config.Menu["Key"]["key.q"].GetValue<MenuKeyBind>().Active

            if (Config.Menu["Combo"]["combo.q"].GetValue<MenuBool>() && Q.IsReady())
            {
                AIHeroClient target = TargetSelector.GetTarget(Q.Range, DamageType.Physical);
                if (target != null && (Config.Menu["Key"]["key.q"].GetValue<MenuKeyBind>().Active || (!E.IsReady() && !AllBarrel.Any(b => b.GetBarrel().Position.Distance(target.Position) < 600))))
                {
                    Q.Cast(target);
                }
            }
            if (E.IsReady() && E.Instance.Ammo > 1 && Config.Menu["Combo"]["combo.e"].GetValue<MenuBool>() && !AllBarrel.Any(b => b.GetBarrel().Position.Distance(Player.Position) <= 1200))
            {
                AIHeroClient target = TargetSelector.GetTarget(1000, DamageType.Physical);
                if (target == null) return;
                Helper.GetPredPos(target);
                Vector2 castPos = target.Position.Extend(Helper.PredPos.ToVector3(), 200).ToVector2();
                if (Player.Distance(castPos) <= E.Range)
                {
                    E.Cast(castPos);
                }
                else
                {
                    E.Cast(Player.Position.Extend(castPos.ToVector3(), 1000));
                }
            }
        }

        private static void InvokeTriplePlacement(Barrel connectingBarrel, IEnumerable<Barrel> hitTest)
        {
            if (!E.IsReady())
            {
                return;
            }
            IEnumerable<AIHeroClient> invokedEnemies = GameObjects.EnemyHeroes.Where(e => e.Position.Distance(connectingBarrel.GetBarrel().Position) < 1370 && !hitTest.Any(b => b.GetBarrel().Position.Distance(e.Position) < 340));
            foreach (AIHeroClient enemy in invokedEnemies)
            {
                //Nice Algorithm with Bad Coding Style following
                //DRY - Do Repeat Yourself
                Vector3 tryPosition = enemy.Position;
                if (tryPosition.Distance(connectingBarrel.GetBarrel().Position) <= 685 &&
                    tryPosition.Distance(Player.Position) <= 1000)
                {
                    TriplePlacement(enemy, tryPosition);
                    return;
                }
                tryPosition = Player.Position.ExtendToMaxRange(enemy.Position, 1000);
                if (tryPosition.Distance(connectingBarrel.GetBarrel().Position) <= 685)
                {
                    TriplePlacement(enemy, tryPosition);
                    return;
                }
                tryPosition = connectingBarrel.GetBarrel().Position.ExtendToMaxRange(enemy.Position, 685);
                if (tryPosition.Distance(Player.Position) <= 1000)
                {
                    TriplePlacement(enemy, tryPosition);
                    return;
                }
                List<Vector2> optimalPositions = Helper.IntersectCircles(Player.Position.ToVector2(), 995, connectingBarrel.GetBarrel().Position.ToVector2(), 680);
                if (optimalPositions.Count == 2)
                {
                    TriplePlacement(enemy, 
                        optimalPositions[0].ToVector3().Distance(enemy.Position) 
                        < optimalPositions[1].ToVector3().Distance(enemy.Position) 
                        ? optimalPositions[0].ToVector3() : optimalPositions[1].ToVector3());
                }
            }
        }

        private static void TriplePlacement(AIHeroClient enemy, Vector3 position)
        {
            if (position.Distance(enemy.Position) <= 340)
            {
                E.Cast(position);
            }
        }

        private static IEnumerable<Barrel> GetBarrelsInRange (Barrel initalBarrel)
        {
            return AllBarrel.Where(b => b.GetBarrel().Position.Distance(initalBarrel.GetBarrel().Position) <= 685 && b != initalBarrel);
        }

        private static void Lasthit()
        {
            //Config.Menu["Cleanse"]["cleanse.w"].GetValue<MenuBool>()
            if (Orbwalker.ActiveMode != OrbwalkerMode.LastHit || Player.ManaPercent <= Config.Menu["Lasthit"]["lasthit.mana"].GetValue<MenuSlider>().Value)
            {
                return;
            }
            if (Q.IsReady())
            {
                foreach (Barrel barrel in AllBarrel)
                {
                    if (barrel.CanQNow() && 
                       // MinionManager.GetMinions(barrel.GetBarrel().Position,650).Any(m => m.Health < Q.GetDamage(m) && m.Distance(barrel.GetBarrel()) <= 380)
                       GameObjects.EnemyMinions.Where(i => i.IsValid() && i.Distance(barrel.GetBarrel()) <= 325).Any(m => m.Health < Q.GetDamage(m))
                        )
                    {
                        Q.Cast(barrel.GetBarrel());
                    }
                }
                
                if (Config.Menu["Lasthit"]["lasthit.q"].GetValue<MenuBool>() && (!AllBarrel.Any(b => b.GetBarrel().Position.Distance(Player.Position) < 1200) || Config.Menu["Key"]["key.q"].GetValue<MenuKeyBind>().Active))
                {
                    var lowHealthMinion = GameObjects.EnemyMinions.FirstOrDefault(i => i.IsValidTarget(Q.Range));
                    if (lowHealthMinion != null && lowHealthMinion.Health <= Q.GetDamage(lowHealthMinion))
                        Q.Cast(lowHealthMinion);
                }
            }
        }

        private static void Cleanse()
        {
            if (W.IsReady() && Config.Menu["Cleanse"]["cleanse.w"].GetValue<MenuBool>())
            {
                if (Buffs.Any(entry => Config.Menu["Enable Cleanse for:"]["cleanse.bufftypes." + entry.Key].GetValue<MenuBool>() && Player.HasBuffOfType(entry.Value)))
                {
                    W.Cast();
                }
            }
        }
    }
}
