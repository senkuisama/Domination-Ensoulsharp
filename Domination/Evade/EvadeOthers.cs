namespace DaoHungAIO.Evade
{
    #region

    using EnsoulSharp;
    using EnsoulSharp.SDK;
    using EnsoulSharp.SDK.MenuUI;
    
    using SharpDX;
    using System;
    using System.Linq;


    #endregion
    internal class EvadeOthers
    {
        public static Menu Menu;
        private static int RivenQTime;
        private static float RivenQRange;
        private static Vector2 RivenDashPos;
        private static IOrderedEnumerable<AIHeroClient> bestAllies;

        internal static void Attach(Menu evadeMenu)
        {

            Menu = new Menu("EvadeOthers", "格挡特定技能");
            {
                Menu.Add(new MenuBool("EnabledWDodge", "启用格挡"));
                Menu.Add(new MenuSlider("EnabledWHP", "启用当血量 <= x%", 100));
            }

            try
            {
                foreach (
               var hero in
               GameObjects.EnemyHeroes.Where(
                   i => BlockSpellDataBase.Spells.Any(a => a.CharacterName == i.CharacterName)))
                {
                    foreach (
            var spell in
            BlockSpellDataBase.Spells.Where(
                x =>
                    ObjectManager.Get<AIHeroClient>().Any(
                        a => a.IsEnemy &&
                             string.Equals(a.CharacterName, x.CharacterName,
                                 StringComparison.CurrentCultureIgnoreCase))))
                    {

                        if (String.Equals(spell.CharacterName, hero.CharacterName, StringComparison.InvariantCultureIgnoreCase))
                        {
                            var heroMenu = new Menu("Block" + spell.CharacterName.ToLower(), spell.CharacterName);
                            heroMenu.Add(new MenuBool("BlockSpell" + spell.SpellSlot, spell.CharacterName + " " + spell.SpellSlot));
                            Menu.Add(heroMenu);

                        }
                    }
                }
            }
            catch { }

            evadeMenu.Add(Menu);

            Game.OnUpdate += OnUpdate;
            AIBaseClient.OnProcessSpellCast += OnProcessSpellCast;
            AIBaseClient.OnPlayAnimation += OnPlayAnimation;
            Dash.OnDash += OnDash;
        }

        private static void OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.CharacterName == "Janna" ||
                ObjectManager.Player.CharacterName == "Rakan" ||
                ObjectManager.Player.CharacterName == "Lulu" || ObjectManager.Player.CharacterName == "Ivern" ||
                ObjectManager.Player.CharacterName == "Karma")
            {
                bestAllies = GameObjects.AllyHeroes
                    .Where(t =>
                        t.Distance(ObjectManager.Player) < DominationAIO.Common.Champion.E.Range)
                    .OrderBy(x => x.Health);
            }
            if (ObjectManager.Player.CharacterName == "Lux" || ObjectManager.Player.CharacterName == "Sona" ||
                ObjectManager.Player.CharacterName == "Taric")

            {
                bestAllies = GameObjects.AllyHeroes
                    .Where(t =>
                        t.Distance(ObjectManager.Player) < DominationAIO.Common.Champion.W.Range)
                    .OrderBy(x => x.Health);
            }
            if (bestAllies != null)
            {
                foreach (var ally in bestAllies)
                {

                    if (ObjectManager.Player.IsDead)
                    {
                        return;
                    }
                    if (ally != null)
                    {

                        if (ally.HasBuff("karthusfallenonetarget"))
                        {
                            if ((ally.GetBuff("karthusfallenonetarget")
                                     .EndTime -
                                 Game.Time) *
                                1000 <= 300)
                            {
                                if (ObjectManager.Player.CharacterName == "Janna" ||
                                    ObjectManager.Player.CharacterName == "Rakan" ||
                                    ObjectManager.Player.CharacterName == "Lulu" ||
                                    ObjectManager.Player.CharacterName == "Ivern" ||
                                    ObjectManager.Player.CharacterName == "Karma")

                                {
                                    if (EvadeTargetManager.Menu["whitelist"][
                                            ally.CharacterName.ToLower()] != null)
                                    {
                                        DominationAIO.Common.Champion.E.CastOnUnit(ally);
                                    }
                                }
                                if (ObjectManager.Player.CharacterName == "Lux" ||
                                    ObjectManager.Player.CharacterName == "Sona" ||
                                    ObjectManager.Player.CharacterName == "Taric")

                                {
                                    if (EvadeTargetManager.Menu["whitelist"][
                                            ally.CharacterName.ToLower()] != null)
                                    {
                                        DominationAIO.Common.Champion.W.CastOnUnit(ally);
                                    }
                                }
                            }
                        }
                        if (ally.HasBuff("nautilusgrandlinetarget"))
                        {
                            if ((ally.GetBuff("nautilusgrandlinetarget")
                                     .EndTime -
                                 Game.Time) *
                                1000 <= 300)
                            {

                                if (ObjectManager.Player.CharacterName == "Janna" ||
                                    ObjectManager.Player.CharacterName == "Rakan" ||
                                    ObjectManager.Player.CharacterName == "Lulu" || ObjectManager.Player.CharacterName == "Ivern" ||
                                    ObjectManager.Player.CharacterName == "Karma")

                                {
                                    if (EvadeTargetManager.Menu["whitelist"][
                                            ally.CharacterName.ToLower()] != null)
                                    {
                                        DominationAIO.Common.Champion.E.CastOnUnit(ally);
                                    }
                                }
                                if (ObjectManager.Player.CharacterName == "Lux" ||
                                    ObjectManager.Player.CharacterName == "Sona" ||
                                    ObjectManager.Player.CharacterName == "Taric")

                                {
                                    if (EvadeTargetManager.Menu["whitelist"][
                                            ally.CharacterName.ToLower()] != null)
                                    {
                                        DominationAIO.Common.Champion.W.CastOnUnit(ally);
                                    }
                                }
                            }
                        }
                        if (ally.HasBuff("nocturneparanoiadash"))
                        {
                            if ((ally.GetBuff("nocturneparanoiadash")
                                     .EndTime -
                                 Game.Time) *
                                1000 <= 300)
                            {


                                if (GameObjects.EnemyHeroes.FirstOrDefault(
                                        x =>
                                            !x.IsDead && x.CharacterName.ToLower() == "nocturne" &&
                                            x.Distance(ally) < 500) != null)
                                {
                                    if (ObjectManager.Player.CharacterName == "Janna" ||
                                        ObjectManager.Player.CharacterName == "Rakan" || ObjectManager.Player.CharacterName == "Ivern" ||
                                        ObjectManager.Player.CharacterName == "Lulu" ||
                                        ObjectManager.Player.CharacterName == "Karma")

                                    {
                                        if (EvadeTargetManager.Menu["whitelist"][
                                                ally.CharacterName.ToLower()] != null
    )
                                        {
                                            DominationAIO.Common.Champion.E.CastOnUnit(ally);
                                        }
                                    }
                                    if (ObjectManager.Player.CharacterName == "Lux" ||
                                        ObjectManager.Player.CharacterName == "Sona" ||
                                        ObjectManager.Player.CharacterName == "Taric")

                                    {
                                        if (EvadeTargetManager.Menu["whitelist"][
                                                ally.CharacterName.ToLower()] != null
    )
                                        {
                                            DominationAIO.Common.Champion.W.CastOnUnit(ally);
                                        }
                                    }
                                }
                            }
                        }
                        if (ally.HasBuff("soulshackles"))
                        {
                            if ((ally.GetBuff("soulshackles")
                                     .EndTime -
                                 Game.Time) *
                                1000 <= 300)
                            {

                                if (ObjectManager.Player.CharacterName == "Janna" ||
                                    ObjectManager.Player.CharacterName == "Rakan" ||
                                    ObjectManager.Player.CharacterName == "Lulu" || ObjectManager.Player.CharacterName == "Ivern" ||
                                    ObjectManager.Player.CharacterName == "Karma")

                                {
                                    if (EvadeTargetManager.Menu["whitelist"][
                                            ally.CharacterName.ToLower()] != null
)
                                    {
                                        DominationAIO.Common.Champion.E.CastOnUnit(ally);
                                    }
                                }
                                if (ObjectManager.Player.CharacterName == "Lux" ||
                                    ObjectManager.Player.CharacterName == "Sona" ||
                                    ObjectManager.Player.CharacterName == "Taric")

                                {
                                    if (EvadeTargetManager.Menu["whitelist"][
                                            ally.CharacterName.ToLower()] != null
)
                                    {
                                        DominationAIO.Common.Champion.W.CastOnUnit(ally);
                                    }
                                }
                            }
                        }
                        if (ally.HasBuff("zedrdeathmark"))
                        {
                            if ((ally.GetBuff("zedrdeathmark")
                                     .EndTime -
                                 Game.Time) *
                                1000 <= 300)
                            {

                                if (ObjectManager.Player.CharacterName == "Janna" ||
                                    ObjectManager.Player.CharacterName == "Rakan" ||
                                    ObjectManager.Player.CharacterName == "Lulu" || ObjectManager.Player.CharacterName == "Ivern" ||
                                    ObjectManager.Player.CharacterName == "Karma")

                                {
                                    if (EvadeTargetManager.Menu["whitelist"][
                                            ally.CharacterName.ToLower()] != null
)
                                    {
                                        DominationAIO.Common.Champion.E.CastOnUnit(ally);
                                    }
                                }
                                if (ObjectManager.Player.CharacterName == "Lux" ||
                                    ObjectManager.Player.CharacterName == "Sona" ||
                                    ObjectManager.Player.CharacterName == "Taric")

                                {
                                    if (EvadeTargetManager.Menu["whitelist"][
                                            ally.CharacterName.ToLower()] != null
)
                                    {
                                        DominationAIO.Common.Champion.W.CastOnUnit(ally);
                                    }
                                }
                            }
                        }

                        foreach (var target in GameObjects.EnemyHeroes.Where(x => !x.IsDead && x.IsValidTarget()))
                        {
                            switch (target.CharacterName)
                            {
                                case "Jax":
                                    if (Menu["Blockjax"]["BlockSpellE"] != null &&
                                        Menu["Blockjax"]["BlockSpellE"].GetValue<MenuBool>().Enabled)
                                    {
                                        if (target.HasBuff("jaxcounterstrike"))
                                        {

                                            var buff = target.GetBuff("JaxCounterStrike");

                                            if ((buff.EndTime - Game.Time) * 1000 <= 650 &&
                                                ally.Distance(target) <= 350f)
                                            {

                                                if (ObjectManager.Player.CharacterName == "Janna" ||
                                                    ObjectManager.Player.CharacterName == "Rakan" ||
                                                    ObjectManager.Player.CharacterName == "Lulu" || ObjectManager.Player.CharacterName == "Ivern" ||
                                                    ObjectManager.Player.CharacterName == "Karma")

                                                {

                                                    if (EvadeTargetManager.Menu["whitelist"][
                                                            ally.CharacterName.ToLower()] != null
                )
                                                    {
                                                        DominationAIO.Common.Champion.E.CastOnUnit(ally);
                                                    }
                                                }
                                                if (ObjectManager.Player.CharacterName == "Lux" ||
                                                    ObjectManager.Player.CharacterName == "Sona" ||
                                                    ObjectManager.Player.CharacterName == "Taric")

                                                {
                                                    if (EvadeTargetManager.Menu["whitelist"][
                                                            ally.CharacterName.ToLower()] != null
                )
                                                    {
                                                        DominationAIO.Common.Champion.W.CastOnUnit(ally);
                                                    }
                                                }
                                            }

                                        }
                                    }


                                    break;
                                case "Riven":
                                    if (Menu["Blockriven"]["BlockSpellQ"] != null &&
                                        Menu["Blockriven"]["BlockSpellQ"].GetValue<MenuBool>().Enabled)
                                    {
                                        if (Utils.GameTimeTickCount - RivenQTime <= 100 && RivenDashPos.IsValid() &&
                                            ally.Distance(target) <= RivenQRange)
                                        {
                                            if (ObjectManager.Player.CharacterName == "Janna" ||
                                                ObjectManager.Player.CharacterName == "Rakan" ||
                                                ObjectManager.Player.CharacterName == "Lulu" || ObjectManager.Player.CharacterName == "Ivern" ||
                                                ObjectManager.Player.CharacterName == "Karma")

                                            {
                                                if (EvadeTargetManager.Menu["whitelist"][
                                                        ally.CharacterName.ToLower()] != null
            )
                                                {

                                                    DominationAIO.Common.Champion.E.CastOnUnit(ally);
                                                }
                                            }
                                            if (ObjectManager.Player.CharacterName == "Lux" ||
                                                ObjectManager.Player.CharacterName == "Sona" ||
                                                ObjectManager.Player.CharacterName == "Taric")

                                            {
                                                if (EvadeTargetManager.Menu["whitelist"][
                                                        ally.CharacterName.ToLower()] != null
            )
                                                {
                                                    DominationAIO.Common.Champion.W.CastOnUnit(ally);
                                                }
                                            }

                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
        }

        private static void OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs Args)
        {
                if (ObjectManager.Player.IsDead)
                {
                    return;
                }
                var ally = Args.Target as AIHeroClient;
                if (ally != null)
                {

                    if (EvadeTargetManager.AttackMenu["Brian.EvadeTargetMenu.Turret"] != null)
                    {
                        if ( ally.IsAlly)
                        {
                            if (ObjectManager.Player.CharacterName == "Janna" ||
                                ObjectManager.Player.CharacterName == "Rakan" || ObjectManager.Player.CharacterName == "Ivern" ||
                                ObjectManager.Player.CharacterName == "Lulu" ||
                                ObjectManager.Player.CharacterName == "Karma")

                            {
                                if (sender.Name.ToLower().Contains("tower") && Args.Target.IsAlly &&
                                    Args.Target.Distance(ObjectManager.Player) <
                                    DominationAIO.Common.Champion.E.Range)
                                {
                                    if (EvadeTargetManager.Menu["whitelist"][
                                            ally.CharacterName.ToLower()] != null
)
                                    {
                                        DominationAIO.Common.Champion.E.CastOnUnit(Args.Target);
                                    }
                                }

                            }
                            if (ObjectManager.Player.CharacterName == "Lux" || ObjectManager.Player.CharacterName == "Sona" ||
                                ObjectManager.Player.CharacterName == "Taric")

                            {
                                if (sender.Name.ToLower().Contains("tower") && Args.Target.IsAlly &&
                                    Args.Target.Distance(ObjectManager.Player) <
                                    DominationAIO.Common.Champion.W.Range)
                                {
                                    if (EvadeTargetManager.Menu["whitelist"][
                                            ally.CharacterName.ToLower()] != null
)
                                    {
                                        DominationAIO.Common.Champion.W.CastOnUnit(Args.Target);
                                    }
                                }

                            }
                        }
                    }
                }

                var target = sender as AIHeroClient;

                if (ally != null)
                {
                if (!sender.IsMinion())
                {
                    if (ally.IsAlly)
                    {
                        if (Args.SData.Name.Contains("BasicAttack") &&
                            EvadeTargetManager.AttackMenu["Brian.EvadeTargetMenu.BAttack"] != null &&
                            ally.HealthPercent <=
                            EvadeTargetManager.AttackMenu["Brian.EvadeTargetMenu.BAttackHpU"]
                                .GetValue<MenuSlider>().Value)
                        {


                            if (ObjectManager.Player.CharacterName == "Janna" ||
                                ObjectManager.Player.CharacterName == "Rakan" ||
                                ObjectManager.Player.CharacterName == "Lulu" || ObjectManager.Player.CharacterName == "Ivern" ||
                                ObjectManager.Player.CharacterName == "Karma")

                            {
                                if (Args.Target.IsAlly &&
                                    Args.Target.Distance(ObjectManager.Player) <
                                    DominationAIO.Common.Champion.E.Range)
                                {
                                    if (EvadeTargetManager.Menu["whitelist"][
                                            ally.CharacterName.ToLower()] != null
)
                                    {
                                        DominationAIO.Common.Champion.E.CastOnUnit(Args.Target);
                                    }
                                }

                            }
                            if (ObjectManager.Player.CharacterName == "Lux" ||
                                ObjectManager.Player.CharacterName == "Sona" ||
                                ObjectManager.Player.CharacterName == "Taric")

                            {
                                if (Args.Target.IsAlly &&
                                    Args.Target.Distance(ObjectManager.Player) <
                                   DominationAIO.Common.Champion.W.Range)
                                {
                                    if (EvadeTargetManager.Menu["whitelist"][
                                            ally.CharacterName.ToLower()] != null
)
                                    {
                                        DominationAIO.Common.Champion.W.CastOnUnit(Args.Target);
                                    }
                                }

                            }
                        }
                    }
                    }
                }
                if (ally != null)
                {
                    if (sender.IsMinion())
                    {

                        if (ally.IsAlly)
                        {
                            if (ObjectManager.Player.CharacterName == "Janna" ||
                                ObjectManager.Player.CharacterName == "Rakan" ||
                                ObjectManager.Player.CharacterName == "Lulu" || ObjectManager.Player.CharacterName == "Ivern" ||
                                ObjectManager.Player.CharacterName == "Karma")

                            {
                                if (Args.Target.IsAlly && Args.Target.Distance(DominationAIO.Common.Champion.Player) <
                                   DominationAIO.Common.Champion.E.Range)
                                {

                                    if (
                                        EvadeTargetManager
                                            .AttackMenu["Brian.EvadeTargetMenu.Minion"] != null && ally.HealthPercent <=
                                        EvadeTargetManager
                                            .AttackMenu["Brian.EvadeTargetMenu.HP"]
                                            .GetValue<MenuSlider>().Value)
                                    {
                                        if (EvadeTargetManager.Menu["whitelist"][
                                                ally.CharacterName.ToLower()] != null
    )
                                        {
                                            DominationAIO.Common.Champion.E.CastOnUnit(ally);
                                        }
                                    }
                                }
                            }
                            if (ObjectManager.Player.CharacterName == "Lux" ||
                                ObjectManager.Player.CharacterName == "Sona" ||
                                ObjectManager.Player.CharacterName == "Taric")

                            {
                                if (Args.Target.IsAlly && Args.Target.Distance(DominationAIO.Common.Champion.Player) <
                                    DominationAIO.Common.Champion.W.Range)
                                {

                                    if (
                                        EvadeTargetManager
                                            .AttackMenu["Brian.EvadeTargetMenu.Minion"] != null && ally.HealthPercent <=
                                        EvadeTargetManager
                                            .AttackMenu["Brian.EvadeTargetMenu.HP"]
                                            .GetValue<MenuSlider>().Value)
                                    {
                                        if (EvadeTargetManager.Menu["whitelist"][
                                                ally.CharacterName.ToLower()] != null
    )
                                        {
                                            DominationAIO.Common.Champion.W.CastOnUnit(ally);
                                        }
                                    }
                                }
                            }

                        }
                    }
                    if (Args.SData.Name.Contains("crit")  &&
                      EvadeTargetManager.AttackMenu["Brian.EvadeTargetMenu.CAttack"] != null
                        && ally.HealthPercent <= EvadeTargetManager
                            .AttackMenu["Brian.EvadeTargetMenu.CAttackHpU"].GetValue<MenuSlider>().Value)
                    {
                       // if (ally.IsHero)
                        {
                            if (ObjectManager.Player.CharacterName == "Janna" ||
                                ObjectManager.Player.CharacterName == "Rakan" ||
                                ObjectManager.Player.CharacterName == "Lulu" || ObjectManager.Player.CharacterName == "Ivern" ||
                                ObjectManager.Player.CharacterName == "Karma")

                            {
                                if (Args.Target.IsAlly && Args.Target.Distance(DominationAIO.Common.Champion.Player) <
                                    DominationAIO.Common.Champion.E.Range)
                                {
                                    if (EvadeTargetManager.Menu["whitelist"][
                                            ally.CharacterName.ToLower()] != null
)
                                    {
                                        DominationAIO.Common.Champion.E.CastOnUnit(ally);
                                    }
                                }


                            }
                            if (ObjectManager.Player.CharacterName == "Lux" ||
                                ObjectManager.Player.CharacterName == "Sona" ||
                                ObjectManager.Player.CharacterName == "Taric")

                            {
                                if (Args.Target.IsAlly && Args.Target.Distance(DominationAIO.Common.Champion.Player) <
                                   DominationAIO.Common.Champion.W.Range)
                                {
                                    if (EvadeTargetManager.Menu["whitelist"][
                                            ally.CharacterName.ToLower()] != null
)
                                    {
                                        DominationAIO.Common.Champion.W.CastOnUnit(ally);
                                    }
                                }

                            }

                        }
                    }
                }

                if (target == null || target.Team == ObjectManager.Player.Team || !target.IsValid ||
                    Args.Target == null || string.IsNullOrEmpty(Args.SData.Name) ||
                    Args.SData.Name.Contains("BasicAttack"))
                {

                    return;
                }

                var spellData =
                    EvadeTargetManager.Spells.FirstOrDefault(
                        i =>
                            i.SpellNames.Contains(Args.SData.Name.ToLower()));
                if (spellData != null)
                {
                    return;
                }
                if (Args.SData.Name == "CaitlynAceintheHole")
                {
                    return;
                }
                if (ObjectManager.Player.CharacterName == "Janna" ||
                    ObjectManager.Player.CharacterName == "Rakan" ||
                    ObjectManager.Player.CharacterName == "Lulu" || ObjectManager.Player.CharacterName == "Ivern" ||
                    ObjectManager.Player.CharacterName == "Karma")

                {
                    if (Args.Target.Distance(DominationAIO.Common.Champion.Player) < DominationAIO.Common.Champion.E.Range)
                    {
                        if (ally != null  && ally.IsAlly)
                        {
                            if (EvadeTargetManager.Menu["whitelist"][
                                    ally.CharacterName.ToLower()] != null)
                            {
                                DominationAIO.Common.Champion.E.CastOnUnit(Args.Target);
                            }
                        }

                    }
                }
                if (ObjectManager.Player.CharacterName == "Lux" ||
                    ObjectManager.Player.CharacterName == "Sona" ||
                    ObjectManager.Player.CharacterName == "Taric")

                {
                    if (Args.Target.Distance(DominationAIO.Common.Champion.Player) < DominationAIO.Common.Champion.W.Range)
                    {
                        if (ally != null  && ally.IsAlly)
                        {
                            if (EvadeTargetManager.Menu["whitelist"][
                                    ally.CharacterName.ToLower()] != null)
                            {
                                DominationAIO.Common.Champion.W.CastOnUnit(Args.Target);
                            }
                        }
                    }
                }
            

        }

        private static void OnPlayAnimation(AIBaseClient sender, AIBaseClientPlayAnimationEventArgs Args)
        {
            var riven = sender as AIHeroClient;

            if (riven == null || riven.Team == ObjectManager.Player.Team || riven.CharacterName != "Riven" || !riven.IsValid)
            {
                return;
            }


            if (Menu["Block" + riven.CharacterName.ToLower()]["BlockSpell" + SpellSlot.Q.ToString()] != null &&
                Menu["Block" + riven.CharacterName.ToLower()]["BlockSpell" + SpellSlot.Q.ToString()].GetValue<MenuBool>().Enabled)
            {
                if (Args.Animation.ToLower() == "spell1c")
                {
                    RivenQTime = Utils.GameTimeTickCount;
                    RivenQRange = riven.HasBuff("RivenFengShuiEngine") ? 225f : 150f;
                }
            }
        }

        private static void OnDash(object obj, Dash.DashArgs Args)
        {
            var riven = obj as AIHeroClient;

            if (riven == null || riven.Team == ObjectManager.Player.Team || riven.CharacterName != "Riven" || !riven.IsValid)
            {
                return;
            }

            if (Menu["Block" + riven.CharacterName.ToLower()]["BlockSpell" + SpellSlot.Q.ToString()] != null &&
               Menu["Block" + riven.CharacterName.ToLower()]["BlockSpell" + SpellSlot.Q.ToString()].GetValue<MenuBool>().Enabled)
            {
                RivenDashPos = Args.EndPos;
            }
        }
    }
}