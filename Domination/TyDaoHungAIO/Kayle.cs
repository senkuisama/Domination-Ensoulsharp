using System;
using System.Linq;
using DominationAIO.Champions.Helper;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.Utility;

namespace DaoHungAIO.Champions
{
    class Kayle
    {
        private Spell q, w, e, r;
        private static AIHeroClient Player = ObjectManager.Player;
        private Menu config;
        private readonly MenuBool qcombo = new MenuBool("qcombo", "use Q");
        private readonly MenuBool wcombo = new MenuBool("wcombo", "use W");
        private readonly MenuBool wcombooutaa = new MenuBool("wcombooutaa", "^ Only out range AA", false);
        private readonly MenuBool ecombo = new MenuBool("ecombo", "use E");
        private readonly MenuBool ecomboafteraa = new MenuBool("ecomboafteraa", "^ After AA");

        private readonly MenuBool qharass = new MenuBool("qharass", "use Q");
        private readonly MenuBool eharass = new MenuBool("eharass", "use E");
        private readonly MenuBool eafteraa = new MenuBool("eafteraa", "^ After AA");
        private readonly MenuSlider manaharass = new MenuSlider("manaharass", "Minimum mana", 30);


        private readonly MenuBool qclear = new MenuBool("qclear", "use Q");
        private readonly MenuBool eclear = new MenuBool("eclear", "use E");
        private readonly MenuBool eclearafteraa = new MenuBool("eclearafteraa", "^ After AA");
        private readonly MenuSlider manaclear = new MenuSlider("manaclear", "Minimum mana", 30);


        private readonly MenuBool qlasthit = new MenuBool("qlasthit", "use Q");
        private readonly MenuBool elasthit = new MenuBool("elasthit", "use E");
        private readonly MenuBool ifcantaa = new MenuBool("ifcantaa", "If can not aa");
        private readonly MenuSlider manalasthit = new MenuSlider("manalasthit", "Minimum mana", 30);

        private readonly Menu AutoR = new Menu("autor", "Auto R");
        private readonly Menu HelpAlly = new Menu("HelpAlly", "Help Ally");
        private static readonly MenuSlider MiscBlockTurret = new MenuSlider("MiscBlockTurret", "Misc Block Turret When Hp low (0 = off)", 20);
        private static readonly Menu BlockList = new Menu("BlockList", "Block List");

        private readonly MenuBool qdraw = new MenuBool("qdraw", "Draw Q");
        private readonly MenuBool wdraw = new MenuBool("wdraw", "Draw W");
        private readonly MenuBool edraw = new MenuBool("edraw", "Draw E");
        private readonly MenuBool rdraw = new MenuBool("rdraw", "Draw R");
        public Kayle()
        {
            q = new Spell(SpellSlot.Q, 900);
            w = new Spell(SpellSlot.W, 900);
            e = new Spell(SpellSlot.E);
            r = new Spell(SpellSlot.R, 900);
            q.SetSkillshot(0.25f, 25, 1600, true, SpellType.Line);

            config = new Menu("kayle", "DH.Kayle", true);
            Menu combo = new Menu("combo", "Combo");
            combo.Add(qcombo);
            combo.Add(wcombo);
            combo.Add(wcombooutaa);
            combo.Add(ecombo);
            combo.Add(ecomboafteraa);

            Menu harass = new Menu("harass", "Harass");
            harass.Add(qharass);
            harass.Add(eharass);
            harass.Add(eafteraa);
            harass.Add(manaharass);

            Menu clear = new Menu("clear", "Clear");
            clear.Add(qclear);
            clear.Add(eclear);
            clear.Add(eclearafteraa);
            clear.Add(manaclear);

            Menu lasthit = new Menu("lasthit", "Last hit");
            lasthit.Add(qlasthit);
            lasthit.Add(elasthit);
            lasthit.Add(ifcantaa);
            lasthit.Add(manalasthit);

            Menu misc = new Menu("misc", "Misc");
            InitBlockSkill();
            HelpAlly.Add(new MenuBool("enable", "Enable", false));
            InitHelpAlly();
            AutoR.Add(new MenuBool("enable", "Enable"));
            
            //AutoR.Add(HelpAlly);
            AutoR.Add(BlockList);
            AutoR.Add(MiscBlockTurret);
            misc.Add(AutoR);

            Menu draw = new Menu("draw", "Draw");
            draw.Add(qdraw);
            draw.Add(wdraw);
            draw.Add(edraw);
            draw.Add(rdraw);

            config.Add(combo);
            config.Add(harass);
            config.Add(clear);
            config.Add(lasthit);
            config.Add(misc);
            config.Add(draw);
            config.Attach();

            Game.OnUpdate += OnTick;
            AIHeroClient.OnProcessSpellCast += OnProcessSpellCast;
            AIHeroClient.OnDoCast += OnDoCast;
            //Game.OnWndProc += OnWndProc;

            Drawing.OnDraw += Drawing_OnDraw;
            //AIBaseClient.OnBuffGain += OnBuffGain;
            //Orbwalker.OnAction += OnAction;
            AIBaseClient.OnDoCast += OnBasicAttack;
            Game.OnUpdate += OnAction;
            //Game.Print("Dont forget setup Block List skill in Misc menu");

        }

        private void OnAction(EventArgs args)
        {
                if (
                //args.Sender != null && args.Sender.IsMe && args.Type == OrbwalkerType.AfterAttack
                FunnySlayerCommon.OnAction.AfterAA
                )
                {

                    if (Orbwalker.ActiveMode == OrbwalkerMode.Combo && ecomboafteraa.Enabled)
                    {
                        var target = TargetSelector.GetTarget(Player.GetRealAutoAttackRange(), DamageType.Magical);
                        var selectedTarget = TargetSelector.SelectedTarget;
                        if (selectedTarget != null && selectedTarget.IsValidTarget(Player.GetRealAutoAttackRange()))
                        {
                            target = selectedTarget;
                        }
                        if (target == null)
                        {
                            return;
                        }
                        if (e.IsReady() && ecombo.Enabled && target.InAutoAttackRange())
                        {
                            e.Cast();
                        }
                    }
                    if (Orbwalker.ActiveMode == OrbwalkerMode.Harass && Player.ManaPercent > manaharass.Value && eafteraa.Enabled)
                    {
                        var target = TargetSelector.GetTarget(Player.GetRealAutoAttackRange(), DamageType.Magical);
                        var selectedTarget = TargetSelector.SelectedTarget;
                        if (selectedTarget != null && selectedTarget.IsValidTarget(Player.GetRealAutoAttackRange()))
                        {
                            target = selectedTarget;
                        }
                        if (target == null)
                        {
                            return;
                        }
                        if (e.IsReady() && eharass.Enabled && target.InAutoAttackRange())
                        {
                            e.Cast();
                        }
                    }
                    if (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear && Player.ManaPercent > manaclear.Value && eclearafteraa.Enabled)
                    {
                        var targetsJug = GameObjects.GetJungles(Player.GetRealAutoAttackRange(), JungleType.All, JungleOrderTypes.MaxHealth);
                        var targetLane = GameObjects.GetMinions(Player.GetRealAutoAttackRange(), MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth);
                        var targets = targetLane.Concat(targetsJug).ToList<AIBaseClient>();
                        if (targets.Count() > 0 && e.IsReady()) {
                            e.Cast();
                        }
                    }
                }

            }


        private void Drawing_OnDraw(EventArgs args)
        {
            if (qdraw.Enabled && q.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, q.Range, System.Drawing.Color.Red, 1);
            }
            if (wdraw.Enabled && w.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, w.Range, System.Drawing.Color.Red, 1);
            }
            if (edraw.Enabled && e.IsReady())
            {
                Render.Circle.DrawCircle(Player.Position, e.Range, System.Drawing.Color.Red, 1);
            }
            if (rdraw.Enabled && r.IsReady())
            {
                MiniMap.DrawCircle(Player.Position, r.Range, System.Drawing.Color.White, 1);
            }
        }

        private void OnBasicAttack(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (sender is AITurretClient && AutoR["enable"].GetValue<MenuBool>().Enabled && sender.Team != Player.Team && args.Target.IsMe)
            {
                if (Player.HealthPercent <= MiscBlockTurret.Value)
                {
                    r.Cast(Player);
                }
            }
        }

        private void OnDoCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            Render.Circle.DrawCircle(args.To, 20, System.Drawing.Color.Red, 10);
            if (sender.IsEnemy && AutoR["enable"].GetValue<MenuBool>().Enabled && !args.SData.Name.IsAutoAttack())
            {
                if (args.Target != null)
                {
                    if ((args.Target.IsMe || args.To.DistanceToPlayer() <= 200 || args.Start.DistanceToPlayer() + args.To.DistanceToPlayer() == args.Start.Distance(args.To)) && EnableBlock(sender, args.Slot))
                    {
                        r.Cast(Player);
                    }

                }
                else
                {
                    if ((args.To.DistanceToPlayer() <= 200 || args.Start.DistanceToPlayer() + args.To.DistanceToPlayer() == args.Start.Distance(args.To)) && EnableBlock(sender, args.Slot))
                    {
                        r.Cast(Player);
                    }
                }
            }
        }
        private bool EnableBlock(AIBaseClient sender, SpellSlot slot)
        {
            var item = BlockList.Item(sender.CharacterName + slot.ToString());
            if (item != null && item.GetValue<MenuBool>().Enabled)
            {
                return true;
            }

            return false;
        }
        private void OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            Render.Circle.DrawCircle(args.To, 20, System.Drawing.Color.Red, 10);
            if (sender.IsEnemy && AutoR["enable"].GetValue<MenuBool>().Enabled && !args.SData.Name.IsAutoAttack())
            {
                if (args.Target != null)
                {
                    if ((args.Target.IsMe || args.To.DistanceToPlayer() <= 200 || args.Start.DistanceToPlayer() + args.To.DistanceToPlayer() == args.Start.Distance(args.To)) && EnableBlock(sender, args.Slot))
                    {
                        r.Cast(Player);
                    }

                }
                else
                {
                    if ((args.To.DistanceToPlayer() <= 200 || args.Start.DistanceToPlayer() + args.To.DistanceToPlayer() == args.Start.Distance(args.To)) && EnableBlock(sender, args.Slot))
                    {
                        r.Cast(Player);

                    }
                }
            }
        }

        private void OnTick(EventArgs args)
        {
                switch (Orbwalker.ActiveMode)
                {
                    case OrbwalkerMode.Combo:
                        DoCombo();
                        break;
                    case OrbwalkerMode.Harass:
                        DoHarass();
                        break;
                    case OrbwalkerMode.LaneClear:
                        DoJungleClear();
                        break;
                    case OrbwalkerMode.LastHit:
                        DoLastHit();
                        break;

                }
        }

        private void DoJungleClear()
        {
            var targetsJug = GameObjects.GetJungles(Player.GetRealAutoAttackRange(), JungleType.All, JungleOrderTypes.MaxHealth);
            var targetLane = GameObjects.GetMinions(Player.GetRealAutoAttackRange(), MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth);
            var targets = targetLane.Concat(targetsJug).ToList<AIBaseClient>();
            if (targets.Count() > 0 && Player.ManaPercent > manaclear.Value)
            {
                if (qclear.Enabled && q.IsReady())
                {
                    var location = q.GetLineFarmLocation(targets);
                    if (location.MinionsHit > 0)
                    {
                        q.Cast(location.Position);
                    }
                }
                if(eclear.Enabled && !eclearafteraa.Enabled && e.IsReady())
                {
                    e.Cast();
                }
            }
        }


        private void DoLastHit()
        {
            var targets = GameObjects.GetMinions(Player.GetRealAutoAttackRange(), MinionTypes.All, MinionTeam.Enemy, MinionOrderTypes.MaxHealth);
            if (targets.Count() > 0 && Player.ManaPercent > manalasthit.Value)
            {
                if (ifcantaa.Enabled && Orbwalker.CanAttack())
                {
                    return;
                }
                if (qclear.Enabled && q.IsReady())
                {
                    var location = q.GetLineFarmLocation(targets);
                    if (location.MinionsHit > 0)
                    {
                        q.Cast(location.Position);
                    }
                }
                if (eclear.Enabled && !eclearafteraa.Enabled && e.IsReady())
                {
                    e.Cast();
                }
            }
        }

        private void DoHarass()
        {

            var target = TargetSelector.GetTarget(q.Range + 300, DamageType.Magical);
            var selectedTarget = TargetSelector.SelectedTarget;
            if (selectedTarget != null && selectedTarget.IsValidTarget(q.Range + 300))
            {
                target = selectedTarget;
            }
            if (target == null || Player.ManaPercent < manaharass.Value)
            {
                return;
            }

            if (q.IsReady() && qharass.Enabled && target.IsValidTarget(q.Range))
            {
                q.CastIfHitchanceMinimum(target, HitChance.Medium);
            }
            if (e.IsReady() && eharass.Enabled && target.InAutoAttackRange() && !eafteraa.Enabled)
            {
                e.Cast();
            }
        }

        private void DoCombo()
        {
            var target = TargetSelector.GetTarget(q.Range + 300, DamageType.Magical);
            var selectedTarget = TargetSelector.SelectedTarget;
            if(selectedTarget != null && selectedTarget.IsValidTarget(q.Range + 300))
            {
                target = selectedTarget;
            }
            if(target == null)
            {
                return;
            }

            if(q.IsReady() && qcombo.Enabled && target.IsValidTarget(q.Range))
            {
                //var cols = q.GetCollisions(target.Position.ToVector2());
                //cols.Units.Where(u => u.IsEnemy || u.IsJungle() || u.IsMinion);
                //Geometry.Line
                q.CastIfHitchanceMinimum(target, HitChance.Medium);
            }
            if(w.IsReady() && wcombo.Enabled)
            {
                if (wcombooutaa.Enabled)
                {
                    if (!target.InAutoAttackRange())
                    {
                        w.Cast(Player);
                    }
                } else
                {
                    w.Cast(Player);
                }
            }
            if(e.IsReady() && ecombo.Enabled && target.InAutoAttackRange() && !ecomboafteraa.Enabled)
            {
                e.Cast();
            }
        }

        private void InitBlockSkill()
        {
            HeroManager.Enemies.ForEach(hero => {
                if (hero.CharacterName == "PracticeTool_TargetDummy")
                {
                    return;
                }
                Menu newMenu = new Menu(hero.CharacterName, hero.CharacterName);
                newMenu.Add(new MenuBool(hero.CharacterName + SpellSlot.R.ToString(), "R"));
                BlockList.Add(newMenu);
            });
        }
        private void InitHelpAlly()
        {
            GameObjects.AllyHeroes.ForEach(hero => {
                if (hero.CharacterName == "PracticeTool_TargetDummy")
                {
                    return;
                }
                HelpAlly.Add(new MenuBool(hero.CharacterName, hero.CharacterName));
            });
        }
    }
}
