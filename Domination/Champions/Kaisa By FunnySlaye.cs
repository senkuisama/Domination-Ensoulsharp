using EnsoulSharp.SDK;
using EnsoulSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.MenuUI;
using SharpDX;

namespace DominationAIO.Champions
{   
    public class menuclass
    {        
        public class combo
        {
            public static MenuBool useq = new MenuBool("useq", "Use Combo Q");
            public static MenuBool useqafteraa = new MenuBool("useqafteraa", "--- Only if after aa", false);
            public static MenuSlider useqminion = new MenuSlider("useqminion", "Max Minions to use Q", 3, 1, 5);

            public static MenuBool usew = new MenuBool("usew", "Use Combo W");
            public static MenuBool usewout = new MenuBool("usewout", "--- Only if target is out of aa range");

            public static MenuBool usee = new MenuBool("usee", "Use Combo E");
            public static MenuSlider edistance = new MenuSlider("edistance", "--- If target distnace to kaisa <= ", (int)ObjectManager.Player.GetRealAutoAttackRange() + 300, (int)ObjectManager.Player.GetRealAutoAttackRange(), (int)ObjectManager.Player.GetRealAutoAttackRange() + 600);

            public static MenuBool user = new MenuBool("user", "use Combo R");
            public static MenuSlider rt = new MenuSlider("rt", "--- If target heath <= x%", 30, 0, 100);
            public static MenuSlider rr = new MenuSlider("rr", "--- If target distance to player >= ", (int)ObjectManager.Player.GetRealAutoAttackRange() + 200, (int)ObjectManager.Player.GetRealAutoAttackRange(), 3000);
            public static MenuSlider rp = new MenuSlider("rp", "--- If kaisa heath <= x%", 30, 0, 100);
            public static MenuBool usercombat = new MenuBool("usercombat", "Logic R on combat");
        }
        public class harass
        {
            public static MenuBool useq = new MenuBool("useq", "Use Q harass");
            public static MenuBool usew = new MenuBool("usew", "Use W harass");
        }
        public class farm
        {
            public static MenuBool useq = new MenuBool("useq", "Use Q farm");
            public static MenuBool usew = new MenuBool("usew", "Use W farm");
            public static MenuBool usee = new MenuBool("usee", "Use E farm");
        }
        public class misc
        {
            public static MenuBool usee = new MenuBool("usee", "E anti gap");
            public static MenuBool usew = new MenuBool("usew", "W ks");
        }
    }
    public class Kaisa
    {
        private static Spell Q, W, E, R;
        public AIHeroClient Player = ObjectManager.Player;

        public static void ongameload()
        {
            setspell.LoadSpells();
            setmenu.LoadMenus();
            Init.events();
        }

        public class setspell

        {           
            public static void LoadSpells()
            {
                Q = new Spell(SpellSlot.Q, 645);
                W = new Spell(SpellSlot.W, 3000);
                E = new Spell(SpellSlot.E);                

                Q.SetTargetted(0.25f, 1800);
                W.SetSkillshot(0.4f, 100, 1750, true, true, EnsoulSharp.SDK.Prediction.SkillshotType.Line);
            }
        }
        public class setmenu
        {
            public static void LoadMenus()
            {
                var kaisa = new Menu("kaisa", "Kaisa Setting", true);
                var combo = new Menu("combo", "Combo");
                var harass = new Menu("harass", "Harass");
                var farm = new Menu("farm", "Farm");
                var misc = new Menu("misc", "Misc");

                combo.Add(menuclass.combo.useq);
                combo.Add(menuclass.combo.useqafteraa);
                combo.Add(menuclass.combo.useqminion);
                combo.Add(menuclass.combo.usew);
                combo.Add(menuclass.combo.usewout);
                combo.Add(menuclass.combo.usee);
                combo.Add(menuclass.combo.edistance);
                combo.Add(menuclass.combo.user);
                combo.Add(menuclass.combo.rt);
                combo.Add(menuclass.combo.rr);
                combo.Add(menuclass.combo.rp);
                combo.Add(menuclass.combo.usercombat);

                harass.Add(menuclass.harass.useq);
                harass.Add(menuclass.harass.usew);

                farm.Add(menuclass.farm.useq);
                farm.Add(menuclass.farm.usew);
                farm.Add(menuclass.farm.usee);

                misc.Add(menuclass.misc.usee);
                misc.Add(menuclass.misc.usew);

                kaisa.Add(combo);
                kaisa.Add(harass);
                kaisa.Add(farm);
                kaisa.Add(misc);
                kaisa.Attach();
            }
        }

        public class Init
        {
            private static bool aa, ba, oa;
            public static void events()
            {
                Game.OnUpdate += Game_OnUpdate;
                Orbwalker.OnAction += Orbwalker_OnAction;
                Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
            }

            private static void Gapcloser_OnGapcloser(AIHeroClient sender, Gapcloser.GapcloserArgs args)
            {
                if (sender.IsMe) return;

                if(menuclass.misc.usee.Enabled && E.IsReady(0))
                {
                    if(args.EndPosition.DistanceToPlayer() < 350)
                    {
                        E.Cast(Game.CursorPos);
                    }
                }
            }

            private static void Orbwalker_OnAction(object sender, OrbwalkerActionArgs args)
            {
                if (args.Type == OrbwalkerType.BeforeAttack)
                {
                    ba = true;
                }
                else ba = false;
                if (args.Type == OrbwalkerType.OnAttack)
                {
                    oa = true;
                }
                else oa = false;
                if (args.Type == OrbwalkerType.AfterAttack)
                {
                    aa = true;
                }
                else aa = false;
            }

            private static void Game_OnUpdate(EventArgs args)
            {
                if (ObjectManager.Player.IsDead) return;

                R = new Spell(SpellSlot.R, CheckRRange());

                var targets = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(3000) && !i.IsDead);


                if (targets != null)
                    foreach (var target in targets)
                    {
                        if (target == null) return;

                        if(menuclass.misc.usew.Enabled && W.IsReady(0))
                        {
                            if(target.Health <= W.GetDamage(target))
                            {
                                var wpred = W.GetPrediction(target, false, -1, EnsoulSharp.SDK.Prediction.CollisionObjects.Minions | EnsoulSharp.SDK.Prediction.CollisionObjects.YasuoWall);
                                if (wpred.CastPosition != Vector3.Zero && wpred.Hitchance >= EnsoulSharp.SDK.Prediction.HitChance.High)
                                {
                                    if (!oa && !ba) W.Cast(wpred.CastPosition);
                                }
                            }
                        }

                        if (Orbwalker.ActiveMode == OrbwalkerMode.Combo)
                        {
                            if (menuclass.combo.useq.Enabled && Q.IsReady(0) && target.IsValidTarget(Q.Range))
                            {
                                if (GetMinionInRange(Q.Range) <= menuclass.combo.useqminion.Value)
                                {
                                    if (menuclass.combo.useqafteraa.Enabled)
                                    {
                                        if (aa) Q.Cast(target);
                                    }
                                    else
                                    {
                                        Q.Cast(target);
                                    }
                                }
                            }
                            if (menuclass.combo.usew.Enabled && W.IsReady(0) && target.IsValidTarget(W.Range))
                            {
                                var wpred = W.GetPrediction(target, false, -1, EnsoulSharp.SDK.Prediction.CollisionObjects.Minions | EnsoulSharp.SDK.Prediction.CollisionObjects.YasuoWall);
                                if(menuclass.combo.usewout.Enabled)
                                {
                                    if (wpred.CastPosition != Vector3.Zero && wpred.Hitchance >= EnsoulSharp.SDK.Prediction.HitChance.High)
                                    {
                                        if (!oa && !ba && target.DistanceToPlayer() > ObjectManager.Player.GetRealAutoAttackRange()) W.Cast(wpred.CastPosition);
                                    }
                                }
                                else
                                {
                                    if(wpred.CastPosition != Vector3.Zero && wpred.Hitchance >= EnsoulSharp.SDK.Prediction.HitChance.High)
                                    {
                                        if (!oa && !ba) W.Cast(wpred.CastPosition);
                                    }
                                }
                            }
                            if (menuclass.combo.usee.Enabled && E.IsReady(0))
                            {
                                if(!oa && !ba)
                                {
                                    if(target.DistanceToPlayer() < menuclass.combo.edistance.Value)
                                    {
                                        E.Cast(Game.CursorPos);
                                    }
                                }
                            }
                            if (menuclass.combo.user.Enabled && R.IsReady(0))
                            {
                                if(target.HealthPercent <= menuclass.combo.rp.Value && target.DistanceToPlayer() > menuclass.combo.rr.Value)
                                {
                                    if(Rpos() != Vector3.Zero)
                                    R.Cast(Rpos());
                                }
                                if(ObjectManager.Player.HealthPercent > 0 && ObjectManager.Player.HealthPercent < menuclass.combo.rp.Value)
                                {
                                    if (Rpos() != Vector3.Zero)
                                        R.Cast(Rpos());
                                }
                                if(GetHeroesInRange(1000) >= 3 && ObjectManager.Player.HealthPercent > 0 && ObjectManager.Player.HealthPercent < 75)
                                {
                                    if (Rpos() != Vector3.Zero && menuclass.combo.usercombat)
                                        R.Cast(Rpos());
                                }
                            }
                        }
                        if (Orbwalker.ActiveMode == OrbwalkerMode.Harass)
                        {
                            if (menuclass.harass.useq.Enabled && Q.IsReady(0) && target.IsValidTarget(Q.Range))
                            {
                                Q.Cast(target);
                            }
                            if (menuclass.harass.usew.Enabled && W.IsReady(0) && target.IsValidTarget(W.Range))
                            {
                                var wpred = W.GetPrediction(target, false, -1, EnsoulSharp.SDK.Prediction.CollisionObjects.Minions | EnsoulSharp.SDK.Prediction.CollisionObjects.YasuoWall);
                                if (wpred.CastPosition != Vector3.Zero && wpred.Hitchance >= EnsoulSharp.SDK.Prediction.HitChance.High)
                                {
                                    if (!oa && !ba) W.Cast(wpred.CastPosition);
                                }
                            }
                        }
                    }

                if (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear)
                {
                    if(GetMinionInRange(ObjectManager.Player.GetRealAutoAttackRange()) > 3)
                    {
                        if (menuclass.farm.useq) Q.Cast();
                        if (menuclass.farm.usee && !oa && !ba) E.Cast(Game.CursorPos);
                    }
                    if (GetMinionInRange(ObjectManager.Player.GetRealAutoAttackRange() + 300) < 1 && GetMinionInRange(W.Range) >= 1)
                    {
                        var thisminion = GameObjects.EnemyMinions.Where(i => !i.IsValidTarget(ObjectManager.Player.GetRealAutoAttackRange() + 300) && i.DistanceToPlayer() < W.Range && i.Health < W.GetDamage(i)).FirstOrDefault(i => i.DistanceToPlayer() < W.Range);
                        var wpred = W.GetPrediction(thisminion, false, -1, EnsoulSharp.SDK.Prediction.CollisionObjects.Minions | EnsoulSharp.SDK.Prediction.CollisionObjects.YasuoWall);
                        if (wpred.CastPosition != Vector3.Zero && wpred.Hitchance >= EnsoulSharp.SDK.Prediction.HitChance.High)
                        {
                            if (menuclass.farm.usew.Enabled) W.Cast(wpred.CastPosition);
                        }
                    }
                }
            }

            private static int GetMinionInRange(float range)
            {
                return GameObjects.EnemyMinions.Count(minion => minion.IsValidTarget(range));
            }
            private static int GetHeroesInRange(float range)
            {
                return GameObjects.EnemyHeroes.Count(minion => minion.IsValidTarget(range));
            }

            private static int CheckRRange()
            {
                int range = 1500;
                if (R.Level == 1)
                {
                    range = 1500;
                }
                if (R.Level == 2)
                {
                    range = 2250;
                }
                if (R.Level == 3)
                {
                    range = 3000;
                }
                return range;
            }

            private static Vector3 Rpos()
            {
                var pos = Vector3.Zero;
                var targets = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(R.Range) && i.HasBuff("kaisapassivemarker"));
                var turret = GameObjects.EnemyTurrets.Where(i => i.IsValidTarget(R.Range)).FirstOrDefault();
                if (targets == null) pos = Vector3.Zero;
                if (GetHeroesInRange(R.Range) == 0) pos = Vector3.Zero;
                /*foreach(var target in targets)
                {
                        if (target == null || !target.HasBuff("kaisapassivemarker")) pos = Vector3.Zero;
                    Geometry.Circle newcircle = new Geometry.Circle(target.Position, ObjectManager.Player.GetRealAutoAttackRange() - 75, 20);
                        foreach(var checkpoint in newcircle.Points.Where(i => i.DistanceToPlayer() < R.Range))
                        {
                            if (GetHeroesInRange(R.Range) == 0) pos = Vector3.Zero;
                            if(GetHeroesInRange(1000) >= 2)
                            {
                                if (checkpoint.CountEnemyHeroesInRange(400) < GetHeroesInRange(1000))
                                {
                                    pos = checkpoint.ToVector3();
                                }
                            }
                            else
                            {
                                if(checkpoint.Distance(turret) > ObjectManager.Player.GetRealAutoAttackRange())
                                {
                                    pos = checkpoint.ToVector3();
                                }
                            }                           
                        }
                }*/
                foreach (var target in targets.Where(i => i.IsValidTarget(R.Range)))
                {
                    if (target == null) pos = Vector3.Zero;
                    Geometry.Circle newcircle = new Geometry.Circle(target.Position, ObjectManager.Player.GetRealAutoAttackRange() - 150, 20);
                    foreach (var checkpoint in newcircle.Points.Where(i => i.DistanceToPlayer() < R.Range))
                    {
                        if(checkpoint.CountEnemyHeroesInRange(400) == 1)
                        {
                            if (GetHeroesInRange(700) >= 2)
                            {
                                if (GetHeroesInRange(700) > checkpoint.CountEnemyHeroesInRange(400))
                                {
                                    if (checkpoint.Distance(turret) > ObjectManager.Player.GetRealAutoAttackRange())
                                    {
                                        pos = checkpoint.ToVector3();
                                    }
                                }
                            }
                        }
                        if(checkpoint.CountEnemyHeroesInRange(400) == 2)
                        {
                            if(GetHeroesInRange(700) >= 3)
                            {
                                if (GetHeroesInRange(700) > checkpoint.CountEnemyHeroesInRange(400))
                                {
                                    if (checkpoint.Distance(turret) > ObjectManager.Player.GetRealAutoAttackRange())
                                    {
                                        pos = checkpoint.ToVector3();
                                    }
                                }
                            }
                        }
                        if (GetHeroesInRange(700) > checkpoint.CountEnemyHeroesInRange(400) + 1)
                        {
                            if (checkpoint.Distance(turret) > ObjectManager.Player.GetRealAutoAttackRange())
                            {
                                pos = checkpoint.ToVector3();
                            }
                        }
                    }
                }

                return pos;
            }
        }
    }
}
