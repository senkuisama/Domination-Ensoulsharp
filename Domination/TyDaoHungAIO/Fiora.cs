using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.Utility;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using static DaoHungAIO.Champions.GetTargets;
using static DaoHungAIO.Champions.Combos;
using static DaoHungAIO.Champions.Fiora;
using static DaoHungAIO.Champions.FioraPassive;
using static DaoHungAIO.Champions.MathAndExtensions;
using static EnsoulSharp.SDK.Prediction;
using Color = System.Drawing.Color;
using Geometry = EnsoulSharp.SDK.Geometry;

namespace DaoHungAIO.Champions
{
    internal class HeroManager
    {
        public static IEnumerable<AIHeroClient> Enemies
        {
            get { return GameObjects.EnemyHeroes; }
        }
    }

    //class EvadeSkillShots
    //{
    //    private static AIHeroClient Player = ObjectManager.Player;
    //    #region Evade
    //    public static void Evading()
    //    {
    //        var parry = Evade.EvadeSpellDatabase.Spells.FirstOrDefault(i => i.Enable && i.IsReady && i.Slot == SpellSlot.W);
    //        if (parry == null)
    //        {
    //            return;
    //        }
    //        var skillshot =
    //            Evade.Evade.SkillshotAboutToHit(Player, 0 + Game.Ping + Fiora.Config["Evade"]["Spells"][parry.Name].GetValue<MenuSlider>("WDelay").Value)
    //                .Where(
    //                    i =>
    //                    parry.DangerLevel <= i.DangerLevel)
    //                .MaxOrDefault(i => i.DangerLevel);
    //        if (skillshot != null)
    //        {
    //            var target =  GetTargets.GetTarget(Fiora.W.Range);
    //            if (target.IsValidTarget(Fiora.W.Range))
    //                Player.Spellbook.CastSpell(parry.Slot, target.Position);
    //            else
    //            {
    //                var hero = HeroManager.Enemies.FirstOrDefault(x => x.IsValidTarget(Fiora.W.Range));
    //                if (hero != null)
    //                    Player.Spellbook.CastSpell(parry.Slot, hero.Position);
    //                else
    //                    Player.Spellbook.CastSpell(parry.Slot, Player.Position.Extend(skillshot.Start.ToVecter3(), 100));
    //            }
    //        }
    //    }
    //    #endregion Evade

    //}
    //public class CustomDamageIndicator
    //{
    //    private const int BAR_WIDTH = 104;
    //    private const int LINE_THICKNESS = 9;

    //    private static Utility.HpBarDamageIndicator.DamageToUnitDelegate damageToUnit;

    //    private static readonly Vector2 BarOffset = new Vector2(10, 25);

    //    private static System.Drawing.Color _drawingColor;
    //    public static System.Drawing.Color DrawingColor
    //    {
    //        get { return _drawingColor; }
    //        set { _drawingColor = System.Drawing.Color.FromArgb(170, value); }
    //    }

    //    public static bool Enabled { get; set; }

    //    public static void Initialize(Utility.HpBarDamageIndicator.DamageToUnitDelegate damageToUnit)
    //    {
    //        // Apply needed field delegate for damage calculation
    //        CustomDamageIndicator.damageToUnit = damageToUnit;
    //        DrawingColor = System.Drawing.Color.DeepPink;
    //        Enabled = true;

    //        // Register event handlers
    //        Drawing.OnDraw += Drawing_OnDraw;
    //    }

    //    private static void Drawing_OnDraw(EventArgs args)
    //    {
    //        if (Enabled)
    //        {
    //            foreach (var unit in HeroManager.Enemies.Where(u => u.IsValidTarget() && u.IsHPBarRendered))
    //            {
    //                // Get damage to unit
    //                var damage = damageToUnit(unit);

    //                // Continue on 0 damage
    //                if (damage <= 0)
    //                    continue;

    //                // Get remaining HP after damage applied in percent and the current percent of health
    //                var damagePercentage = ((unit.Health - damage) > 0 ? (unit.Health - damage) : 0) / unit.MaxHealth;
    //                var currentHealthPercentage = unit.Health / unit.MaxHealth;

    //                // Calculate start and end point of the bar indicator
    //                var startPoint = new Vector2((int)(unit.HPBarPosition.X + BarOffset.X + damagePercentage * BAR_WIDTH), (int)(unit.HPBarPosition.Y + BarOffset.Y) - 5);
    //                var endPoint = new Vector2((int)(unit.HPBarPosition.X + BarOffset.X + currentHealthPercentage * BAR_WIDTH) + 1, (int)(unit.HPBarPosition.Y + BarOffset.Y) - 5);

    //                // Draw the line
    //                Drawing.DrawLine(startPoint, endPoint, LINE_THICKNESS, DrawingColor);
    //            }
    //        }
    //    }
    //}
    public static class  GetTargets
    {
        
        public static bool FocusUlted { get { return Fiora.Config["TargetingModes"].GetValue<MenuBool>("FocusUltedTarget").Enabled; } }
        private static AIHeroClient Player = ObjectManager.Player;
        public static TargetMode TargetingMode
        {
            get
            {
                var menuindex = Fiora.Config["TargetingModes"].GetValue<MenuList>("TargetingMode").SelectedValue; //"Optional","Selected","Priority","Normal"
                switch (menuindex)
                {
                    case "Optional":
                        return GetTargets.TargetMode.Optional;
                    case "Selected":
                        return GetTargets.TargetMode.Selected;
                    case "Priority":
                        return GetTargets.TargetMode.Priority;
                    default:
                        return GetTargets.TargetMode.Normal;
                }

            }
        }
        public enum TargetMode
        {
            Normal = 0,
            Optional = 1,
            Selected = 2,
            Priority = 3
        }
        public static AIHeroClient GetTarget(float range = 500)
        {
            if (TargetingMode == GetTargets.TargetMode.Normal)
                return GetStandarTarget(range);
            if (TargetingMode == GetTargets.TargetMode.Optional)
                return GetOptionalTarget();
            if (TargetingMode == GetTargets.TargetMode.Priority)
                return GetPriorityTarget();
            if (TargetingMode == GetTargets.TargetMode.Selected)
                return GetSelectedTarget();
            return null;
        }
        public static AIHeroClient GetUltedTarget()
        {
            if (!FocusUlted)
                return null;
            return HeroManager.Enemies.FirstOrDefault(x => x != null && x.IsValid && FioraPassive.FioraUltiPassiveObjects
                                .Any(i => i != null && i.IsValid && i.Position.ToVector2().Distance(x.Position.ToVector2()) <= 50));
        }

        public static AIHeroClient GetStandarTarget(float range)
        {
            var ulted = GetUltedTarget();
            if (ulted.IsValidTarget(500))
                return ulted;
            return TargetSelector.GetTarget(range, DamageType.Physical);
        }

        public static float PriorityRange { get { return Fiora.Config["TargetingModes"]["Priority"].GetValue<MenuSlider>("PriorityRange").Value; } }
        public static int PriorityValue(AIHeroClient target)
        {
            return Fiora.Config["TargetingModes"]["Priority"].GetValue<MenuSlider>("Priority" + target.CharacterName).Value;
        }
        public static AIHeroClient GetPriorityTarget()
        {
            var ulted = GetUltedTarget();
            if (ulted.IsValidTarget(PriorityRange))
                return ulted;
            return HeroManager.Enemies.Where(x => x.IsValidTarget(PriorityRange) && !x.IsDead)
                                    .OrderByDescending(x => PriorityValue(x))
                                    .ThenBy(x => x.Health)
                                    .FirstOrDefault();
        }

        public static float SelectedRange { get { return Fiora.Config["TargetingModes"]["Selected"].GetValue<MenuSlider>("SelectedRange").Value; } }
        public static bool SwitchIfNoTargeted { get { return Fiora.Config["TargetingModes"]["Selected"].GetValue<MenuBool>("SelectedSwitchIfNoSelected").Enabled; } }
        public static AIHeroClient GetSelectedTarget()
        {
            var ulted = GetUltedTarget();
            if (ulted.IsValidTarget(SelectedRange))
                return ulted;
            var tar = TargetSelector.SelectedTarget;
            var tarD = tar.IsValidTarget(SelectedRange) && !tar.IsDead ? tar : null;
            if (tarD != null)
                return tarD;
            else
            {
                if (SwitchIfNoTargeted)
                    return GetOptionalTarget();
                return null;
            }
        }

        public static AIHeroClient SuperOldOptionalTarget = null;
        public static AIHeroClient OldOptionalTarget = null;
        public static AIHeroClient PreOptionalTarget = null;
        public static AIHeroClient OptionalTarget = null;
        public static float OptionalRange { get { return Fiora.Config["TargetingModes"]["Optional"].GetValue<MenuSlider>("OptionalRange").Value; } }
        public static uint OptionalKey { get { return (uint)Fiora.Config["TargetingModes"]["Optional"].GetValue<MenuKeyBind>("OptionalSwitchTargetKey").Key; } }
        public static AIHeroClient GetOptionalTarget()
        {
            var ulted = GetUltedTarget();
            if (ulted.IsValidTarget(OptionalRange))
            {
                OptionalTarget = ulted;
                return OptionalTarget;
            }
            if (OptionalTarget.IsValidTarget(OptionalRange) && !OptionalTarget.IsDead)
                return OptionalTarget;
            OptionalTarget = HeroManager.Enemies.Where(x => x.IsValidTarget(OptionalRange) && !x.IsDead)
                                .OrderBy(x => Player.Distance(x.Position)).FirstOrDefault();
            return OptionalTarget;
        }
        public static void Game_OnWndProc(GameWndEventArgs args)
        {
            if (args.Msg == (uint)WindowsKeyMessages.KEYDOWN)
            {
                if (args.WParam == (uint)OptionalKey)
                {
                    OptionalTarget = GetOptionalTarget();
                    if (OptionalTarget == null)
                    {
                        PreOptionalTarget = HeroManager.Enemies.Where(x => x.IsValidTarget(OptionalRange) && !x.IsDead)
                                                       .OrderBy(x => OldOptionalTarget != null ? x.NetworkId == OldOptionalTarget.NetworkId : x.IsEnemy)
                                                       .ThenBy(x => Player.Distance(x.Position)).FirstOrDefault();
                        if (PreOptionalTarget != null)
                        {
                            OptionalTarget = PreOptionalTarget;
                        }
                        return;
                    }
                    PreOptionalTarget = HeroManager.Enemies.Where(x => x.IsValidTarget(OptionalRange) && !x.IsDead && x.NetworkId != OptionalTarget.NetworkId)
                                                   .OrderBy(x => OldOptionalTarget != null ? x.NetworkId == OldOptionalTarget.NetworkId : x.IsEnemy)
                                                   .ThenBy(x => Player.Distance(x.Position)).FirstOrDefault();
                    if (PreOptionalTarget != null)
                    {
                        OldOptionalTarget = OptionalTarget;
                        OptionalTarget = PreOptionalTarget;
                    }
                    return;
                }
            }
            if (args.Msg == (uint)WindowsKeyMessages.LBUTTONDOWN)
            {
                OptionalTarget = GetOptionalTarget();
                if (OptionalTarget == null)
                {
                    PreOptionalTarget = HeroManager.Enemies.Where(x => x.IsValidTarget(OptionalRange)
                                                    && x.IsValidTarget(400, true, Game.CursorPos) && !x.IsDead)
                                                   .OrderBy(x => Game.CursorPos.ToVector2().Distance(x.Position.ToVector2())).FirstOrDefault();
                    if (PreOptionalTarget != null)
                    {
                        OptionalTarget = PreOptionalTarget;
                    }
                    return;
                }
                PreOptionalTarget = HeroManager.Enemies.Where(x => x.IsValidTarget(OptionalRange)
                                                && x.IsValidTarget(400, true, Game.CursorPos) && !x.IsDead)
                                               .OrderBy(x => Game.CursorPos.ToVector2().Distance(x.Position.ToVector2())).FirstOrDefault();
                if (PreOptionalTarget != null)
                {
                    OldOptionalTarget = OptionalTarget;
                    OptionalTarget = PreOptionalTarget;
                }
                return;
            }
        }
    }
    internal class EvadeTarget
    {


        private static readonly List<Targets> DetectedTargets = new List<Targets>();

        private static readonly List<SpellData> Spells = new List<SpellData>();
        private static AIHeroClient Player = ObjectManager.Player;



        internal static void Init()
        {
            LoadSpellData();

            Spells.RemoveAll(i => !HeroManager.Enemies.Any(
            a =>
            string.Equals(
                a.CharacterName,
                i.CharacterName,
                StringComparison.InvariantCultureIgnoreCase)));

            var evadeMenu = new Menu("EvadeTarget", "Evade Targeted SkillShot");
            {
                evadeMenu.Add(new MenuBool("W", "Use W"));
                //var aaMenu = new Menu("AA", "Auto Attack");
                //{
                //    aaMenu.Add(new MenuBool("B", "Basic Attack", false));
                //    aaMenu.Add(new MenuSlider("BHpU", "-> If Hp < (%)", 35));
                //    aaMenu.Add(new MenuBool("C", "Crit Attack", false));
                //    aaMenu.Add(new MenuSlider("CHpU", "-> If Hp < (%)", 40));
                //    evadeMenu.Add(aaMenu);
                //}
                try
                {
                    foreach (var hero in
                        HeroManager.Enemies.Where(
                            i =>
                            Spells.Any(
                                a =>
                                string.Equals(
                                    a.CharacterName,
                                    i.CharacterName,
                                    StringComparison.InvariantCultureIgnoreCase))))
                    {
                        evadeMenu.Add(new Menu(hero.CharacterName.ToLowerInvariant(), "-> " + hero.CharacterName));
                    }
                } catch { }
                try
                {
                    foreach (var spell in
                        Spells.Where(
                            i =>
                            HeroManager.Enemies.Any(
                                a =>
                                string.Equals(
                                    a.CharacterName,
                                    i.CharacterName,
                                    StringComparison.InvariantCultureIgnoreCase))))
                    {
                        ((Menu)evadeMenu[spell.CharacterName.ToLowerInvariant()]).Add(new MenuBool(
                            spell.MissileName,
                            spell.MissileName + " (" + spell.Slot + ")",
                            false));
                    }
                } catch { }
            }
            Fiora.Config.Add(evadeMenu);
            Game.OnUpdate += OnUpdateTarget;
            GameObject.OnCreate += ObjSpellMissileOnCreate;
            GameObject.OnDelete += ObjSpellMissileOnDelete;
        }

        private static void LoadSpellData()
        {
            Spells.Add(
                new SpellData { CharacterName = "Ahri", SpellNames = new[] { "ahrifoxfiremissiletwo" }, Slot = SpellSlot.W });
            Spells.Add(
                new SpellData { CharacterName = "Ahri", SpellNames = new[] { "ahritumblemissile" }, Slot = SpellSlot.R });
            Spells.Add(
                new SpellData { CharacterName = "Anivia", SpellNames = new[] { "frostbite" }, Slot = SpellSlot.E });
            Spells.Add(
                new SpellData { CharacterName = "Annie", SpellNames = new[] { "disintegrate" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Brand",
                    SpellNames = new[] { "brandconflagrationmissile" },
                    Slot = SpellSlot.E
                });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Brand",
                    SpellNames = new[] { "brandwildfire", "brandwildfiremissile" },
                    Slot = SpellSlot.R
                });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Caitlyn",
                    SpellNames = new[] { "caitlynaceintheholemissile" },
                    Slot = SpellSlot.R
                });
            Spells.Add(
                new SpellData { CharacterName = "Cassiopeia", SpellNames = new[] { "cassiopeiatwinfang" }, Slot = SpellSlot.E });
            Spells.Add(
                new SpellData { CharacterName = "Elise", SpellNames = new[] { "elisehumanq" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Ezreal",
                    SpellNames = new[] { "ezrealarcaneshiftmissile" },
                    Slot = SpellSlot.E
                });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "FiddleSticks",
                    SpellNames = new[] { "fiddlesticksdarkwind", "fiddlesticksdarkwindmissile" },
                    Slot = SpellSlot.E
                });
            Spells.Add(
                new SpellData { CharacterName = "Gangplank", SpellNames = new[] { "parley" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData { CharacterName = "Janna", SpellNames = new[] { "sowthewind" }, Slot = SpellSlot.W });
            Spells.Add(
                new SpellData { CharacterName = "Kassadin", SpellNames = new[] { "nulllance" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Katarina",
                    SpellNames = new[] { "katarinaq", "katarinaqmis" },
                    Slot = SpellSlot.Q
                });
            Spells.Add(
                new SpellData { CharacterName = "Kayle", SpellNames = new[] { "judicatorreckoning" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Leblanc",
                    SpellNames = new[] { "leblancchaosorb", "leblancchaosorbm" },
                    Slot = SpellSlot.Q
                });
            Spells.Add(new SpellData { CharacterName = "Lulu", SpellNames = new[] { "LuluW" }, Slot = SpellSlot.W });
            Spells.Add(
                new SpellData { CharacterName = "Malphite", SpellNames = new[] { "seismicshard" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "MissFortune",
                    SpellNames = new[] { "missfortunericochetshot", "missFortunershotextra" },
                    Slot = SpellSlot.Q
                });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Nami",
                    SpellNames = new[] { "namiwenemy", "namiwmissileenemy" },
                    Slot = SpellSlot.W
                });
            Spells.Add(
                new SpellData { CharacterName = "Nunu", SpellNames = new[] { "iceblast" }, Slot = SpellSlot.E });
            Spells.Add(
                new SpellData { CharacterName = "Pantheon", SpellNames = new[] { "pantheonw" }, Slot = SpellSlot.W });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Ryze",
                    SpellNames = new[] { "spellflux", "spellfluxmissile" },
                    Slot = SpellSlot.E
                });
            Spells.Add(
                new SpellData { CharacterName = "Shaco", SpellNames = new[] { "twoshivpoison" }, Slot = SpellSlot.E });
            Spells.Add(
                new SpellData { CharacterName = "Shen", SpellNames = new[] { "shenvorpalstar" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData { CharacterName = "Sona", SpellNames = new[] { "sonaqmissile" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData { CharacterName = "Swain", SpellNames = new[] { "swaintorment" }, Slot = SpellSlot.E });
            Spells.Add(
                new SpellData { CharacterName = "Syndra", SpellNames = new[] { "syndrar" }, Slot = SpellSlot.R });
            Spells.Add(
                new SpellData { CharacterName = "Taric", SpellNames = new[] { "dazzle" }, Slot = SpellSlot.E });
            Spells.Add(
                new SpellData { CharacterName = "Teemo", SpellNames = new[] { "blindingdart" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData { CharacterName = "Tristana", SpellNames = new[] { "detonatingshot" }, Slot = SpellSlot.E });
            Spells.Add(
                new SpellData { CharacterName = "Tristana", SpellNames = new[] { "tristanar" }, Slot = SpellSlot.R });
            Spells.Add(
                new SpellData { CharacterName = "TwistedFate", SpellNames = new[] { "bluecardattack" }, Slot = SpellSlot.W });
            Spells.Add(
                new SpellData { CharacterName = "TwistedFate", SpellNames = new[] { "goldcardattack" }, Slot = SpellSlot.W });
            Spells.Add(
                new SpellData { CharacterName = "TwistedFate", SpellNames = new[] { "redcardattack" }, Slot = SpellSlot.W });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Urgot",
                    SpellNames = new[] { "urgotheatseekinghomemissile" },
                    Slot = SpellSlot.Q
                });
            Spells.Add(
                new SpellData { CharacterName = "Vayne", SpellNames = new[] { "vaynecondemnmissile" }, Slot = SpellSlot.E });
            Spells.Add(
                new SpellData { CharacterName = "Veigar", SpellNames = new[] { "veigarprimordialburst" }, Slot = SpellSlot.R });
            Spells.Add(
                new SpellData { CharacterName = "Viktor", SpellNames = new[] { "viktorpowertransfer" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Vladimir",
                    SpellNames = new[] { "vladimirtidesofbloodnuke" },
                    Slot = SpellSlot.E
                });
        }

        private static void ObjSpellMissileOnCreate(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;
            if (missile == null || !missile.IsValid)
            {
                return;
            }
            var caster = missile.SpellCaster as AIHeroClient;
            if(caster == null || !caster.IsValid)
                caster = GetTarget(W.Range);

            //if (caster == null || !caster.IsValid || caster.Team == ObjectManager.Player.Team || !(missile != null && missile.IsMe))
            //{
            //    return;
            //}
            //Game.Print("has caster");
            var spellData =
                Spells.FirstOrDefault(
                    i =>
                    i.SpellNames.Contains(missile.SData.Name.ToLower())
                    && Fiora.Config["EvadeTarget"][i.CharacterName.ToLowerInvariant()].GetValue<MenuBool>(i.MissileName).Enabled);

            //if (spellData == null && Orbwalker.IsAutoAttack(missile.SData.Name)
            //    && (!missile.SData.Name.ToLower().Contains("crit")
            //            ? Fiora.Config["EvadeTarget"]["AA"].GetValue<MenuBool>("B")
            //              && Player.HealthPercent < Fiora.Config["EvadeTarget"]["AA"].GetValue<MenuSlider>("BHpU").Value
            //            : Fiora.Config["EvadeTarget"]["AA"].GetValue<MenuBool>("C")
            //              && Player.HealthPercent < Fiora.Config["EvadeTarget"]["AA"].GetValue<MenuSlider>("CHpU").Value))
            //{
            //    spellData = new SpellData { CharacterName = caster.CharacterName, SpellNames = new[] { missile.SData.Name } };
            //}

            if (spellData == null)
            {
                return;
            }

            DetectedTargets.Add(new Targets { Start = caster.Position, Obj = missile });
        }

        private static void ObjSpellMissileOnDelete(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;
            if (missile == null || !missile.IsValid)
            {
                return;
            }
            var caster = missile.SpellCaster as AIHeroClient;
            if (caster == null || !caster.IsValid || caster.Team == Player.Team)
            {
                return;
            }
            DetectedTargets.RemoveAll(i => i.Obj.NetworkId == missile.NetworkId);
        }

        private static void OnUpdateTarget(EventArgs args)
        {
            if (Player.IsDead)
            {
                return;
            }
            if (Player.HasBuffOfType(BuffType.SpellShield) || Player.HasBuffOfType(BuffType.SpellImmunity))
            {
                return;
            }
            if (!Fiora.Config["EvadeTarget"].GetValue<MenuBool>("W").Enabled || !Fiora.W.IsReady())
            {
                return;
            }
                foreach (var target in
                    DetectedTargets.Where(i => i != null && i.Obj != null && i.Obj.SData != null && Fiora.W.IsInRange(i.Obj, 150 + 
                    Game.Ping * i.Obj.SData.MissileSpeed / 1000
                    )).OrderBy(i => i.Obj.Position.Distance(Player.Position)))
                {
                    var tar = TargetSelector.GetTarget(Fiora.W.Range, DamageType.Physical);
                    if (tar.IsValidTarget(Fiora.W.Range))
                        Player.Spellbook.CastSpell(SpellSlot.W, tar.Position);
                    else
                    {
                        var hero = HeroManager.Enemies.FirstOrDefault(x => x.IsValidTarget(Fiora.W.Range));
                        if (hero != null)
                            Player.Spellbook.CastSpell(SpellSlot.W, hero.Position);
                        else
                            Player.Spellbook.CastSpell(SpellSlot.W, Player.Position.Extend(target.Start, 100));
                    }
                }
        }



        private class SpellData
        {


            public string CharacterName;

            public SpellSlot Slot;

            public string[] SpellNames = { };



            public string MissileName
            {
                get
                {
                    return this.SpellNames.First();
                }
            }


        }

        private class Targets
        {
         

            public MissileClient Obj;

            public Vector3 Start;


        }
    }
    public static class FioraPassive
    {
  
        public static List<Vector2> GetRadiusPoints(Vector2 targetpredictedpos, Vector2 passivepredictedposition)
        {
            List<Vector2> RadiusPoints = new List<Vector2>();
            for (int i = 50; i <= 300; i = i + 25)
            {
                var x = targetpredictedpos.Extend(passivepredictedposition, i);
                for (int j = -45; j <= 45; j = j + 5)
                {
                    RadiusPoints.Add(x.RotateAroundPoint(targetpredictedpos, j * (float)(Math.PI / 180)));
                }
            }
            return RadiusPoints;
        }
        public static PassiveStatus GetPassiveStatus(this AIHeroClient target, float delay = 0.25f)
        {
            var allobjects = GetPassiveObjects()
                .Where(x => x.Object != null && x.Object.IsValid
                           && x.Object.Position.ToVector2().Distance(target.Position.ToVector2()) <= 50);
            var targetpredictedpos = Prediction.GetPrediction(target, delay).UnitPosition.ToVector2();
            if (!allobjects.Any())
            {
                return new PassiveStatus(false, PassiveType.None, new Vector2(), new List<PassiveDirection>(), new List<Vector2>());
            }
            else
            {
                var x = allobjects.First();
                var listdirections = new List<PassiveDirection>();
                foreach (var a in allobjects)
                {
                    listdirections.Add(a.PassiveDirection);
                }
                var listpositions = new List<Vector2>();
                foreach (var a in listdirections)
                {
                    if (a == PassiveDirection.NE)
                    {
                        var pos = targetpredictedpos;
                        pos.Y = pos.Y + 200;
                        listpositions.Add(pos);
                    }
                    else if (a == PassiveDirection.NW)
                    {
                        var pos = targetpredictedpos;
                        pos.X = pos.X + 200;
                        listpositions.Add(pos);
                    }
                    else if (a == PassiveDirection.SE)
                    {
                        var pos = targetpredictedpos;
                        pos.X = pos.X - 200;
                        listpositions.Add(pos);
                    }
                    else if (a == PassiveDirection.SW)
                    {
                        var pos = targetpredictedpos;
                        pos.Y = pos.Y - 200;
                        listpositions.Add(pos);
                    }
                }
                if (x.PassiveType == FioraPassive.PassiveType.PrePassive)
                {
                    return new PassiveStatus(true, PassiveType.PrePassive, targetpredictedpos, listdirections, listpositions);
                }
                if (x.PassiveType == FioraPassive.PassiveType.NormalPassive)
                {
                    return new PassiveStatus(true, PassiveType.NormalPassive, targetpredictedpos, listdirections, listpositions);
                }
                if (x.PassiveType == FioraPassive.PassiveType.UltiPassive)
                {
                    return new PassiveStatus(true, PassiveType.UltiPassive, targetpredictedpos, listdirections, listpositions);
                }
                return new PassiveStatus(false, PassiveType.None, new Vector2(), new List<PassiveDirection>(), new List<Vector2>());
            }
        }
        public static List<PassiveObject> GetPassiveObjects()
        {
            List<PassiveObject> PassiveObjects = new List<PassiveObject>();
            foreach (var x in FioraPrePassiveObjects.Where(i => i != null && i.IsValid))
            {
                PassiveObjects.Add(new PassiveObject(x.Name, x, PassiveType.PrePassive, GetPassiveDirection(x)));
            }
            foreach (var x in FioraPassiveObjects.Where(i => i != null && i.IsValid))
            {
                PassiveObjects.Add(new PassiveObject(x.Name, x, PassiveType.NormalPassive, GetPassiveDirection(x)));
            }
            foreach (var x in FioraUltiPassiveObjects.Where(i => i != null && i.IsValid))
            {
                PassiveObjects.Add(new PassiveObject(x.Name, x, PassiveType.UltiPassive, GetPassiveDirection(x)));
            }
            return PassiveObjects;
        }
        public static PassiveDirection GetPassiveDirection(EffectEmitter x)
        {
            if (x.Name.Contains("NE"))
            {
                return PassiveDirection.NE;
            }
            else if (x.Name.Contains("SE"))
            {
                return PassiveDirection.SE;
            }
            else if (x.Name.Contains("NW"))
            {
                return PassiveDirection.NW;
            }
            else
            {
                return PassiveDirection.SW;
            }
        }
        public class PassiveStatus
        {
            public bool HasPassive;
            public PassiveType PassiveType;
            public Vector2 TargetPredictedPosition;
            public List<PassiveDirection> PassiveDirections = new List<PassiveDirection>();
            public List<Vector2> PassivePredictedPositions = new List<Vector2>();
            public PassiveStatus(bool hasPassive, PassiveType passiveType, Vector2 targetPredictedPosition
                , List<PassiveDirection> passiveDirections, List<Vector2> passivePredictedPositions)
            {
                HasPassive = hasPassive;
                PassiveType = passiveType;
                TargetPredictedPosition = targetPredictedPosition;
                PassiveDirections = passiveDirections;
                PassivePredictedPositions = passivePredictedPositions;
            }
        }
        public enum PassiveType
        {
            None, PrePassive, NormalPassive, UltiPassive
        }
        public enum PassiveDirection
        {
            NE, SE, NW, SW
        }
        public class PassiveObject
        {
            public string PassiveName;
            public EffectEmitter Object;
            public PassiveType PassiveType;
            public PassiveDirection PassiveDirection;
            public PassiveObject(string passiveName, EffectEmitter obj, PassiveType passiveType, PassiveDirection passiveDirection)
            {
                PassiveName = passiveName;
                Object = obj;
                PassiveType = passiveType;
                PassiveDirection = passiveDirection;
            }
        }
        public static List<EffectEmitter> FioraUltiPassiveObjects = new List<EffectEmitter>();
        //{
        //    get
        //    {
        //        var x = ObjectManager.Get<EffectEmitter>()
        //        .Where(a => a.Name.Contains("_R_Mark") || (a.Name.Contains("_R") && a.Name.Contains("Timeout_FioraOnly.troy")))
        //        .ToList();
        //        return x;
        //    }
        //}
        public static List<EffectEmitter> FioraPassiveObjects = new List<EffectEmitter>();
        //{
        //    get
        //    {
        //        var x = ObjectManager.Get<EffectEmitter>().Where(a => FioraPassiveName.Contains(a.Name)).ToList();
        //        return x;
        //    }
        //}
        public static List<EffectEmitter> FioraPrePassiveObjects = new List<EffectEmitter>();
        //{
        //    get
        //    {
        //        var x = ObjectManager.Get<EffectEmitter>().Where(a => FioraPrePassiveName.Contains(a.Name)).ToList();
        //        return x;
        //    }
        //}
        public static List<string> FioraPassiveName = new List<string>()
        {
            "_Passive_NE",
            "_Passive_SE",
            "_Passive_NW",
            "_Passive_SW",
            "_Passive_NE_Timeout",
            "_Passive_SE_Timeout",
            "_Passive_NW_Timeout",
            "_Passive_SW_Timeout"
        };
        public static List<string> FioraPrePassiveName = new List<string>()
        {
            "_Passive_NE_Warning",
            "_Passive_SE_Warning",
            "_Passive_NW_Warning",
            "_Passive_SW_Warning"
        };

        private static bool IsPassiveMark(this String name, List<string> FioraPassiveName)
        {
            foreach (var aName in FioraPassiveName)
            {
                if (name.Contains(aName) && name.Contains("Fiora"))
                    return true;
            }
            return false;
        }
        public static void FioraPassiveUpdate()
        {
            FioraPrePassiveObjects = new List<EffectEmitter>();
            FioraPassiveObjects = new List<EffectEmitter>();
            FioraUltiPassiveObjects = new List<EffectEmitter>();
            //ObjectManager.Get<EffectEmitter>().Where(ee => ee.Name.Contains("Fiora")).ForEach(ee => Game.Print(ee.Name));
            var ObjectEmitter = ObjectManager.Get<EffectEmitter>()
                                             .Where(a => a.Name.IsPassiveMark(FioraPassiveName.Concat(FioraPrePassiveName).ToList())
                                             || (a.Name.Contains("_R_Mark") && a.Name.Contains("Fiora"))
                                             || (a.Name.Contains("_R") && a.Name.Contains("_FioraOnly") && a.Name.Contains("Fiora")))
                                             .ToList();
            FioraPrePassiveObjects.AddRange(ObjectEmitter.Where(a => a.Name.IsPassiveMark(FioraPrePassiveName)));
            FioraPassiveObjects.AddRange(ObjectEmitter.Where(a => a.Name.IsPassiveMark(FioraPassiveName)));
            FioraUltiPassiveObjects.AddRange(ObjectEmitter
                .Where(a =>
                       (a.Name.Contains("_R_Mark") && a.Name.Contains("Fiora"))
                       || (a.Name.Contains("_R") && a.Name.Contains("_FioraOnly") && a.Name.Contains("Fiora"))));
        }
 
    }
    
    internal class OtherSkill
    {
        private static readonly List<SpellData> Spells = new List<SpellData>();

        // riven variables
        private static AIHeroClient Player = ObjectManager.Player;
        private static int RivenDashTick;
        private static int RivenQ3Tick;
        private static Vector2 RivenDashEnd = new Vector2();
        private static float RivenQ3Rad = 150;

        // fizz variables
        private static Vector2 FizzFishEndPos = new Vector2();
        private static GameObject FizzFishChum = null;
        private static int FizzFishChumStartTick;
        private static Menu evadeMenu;

        internal static void Init()
        {
            LoadSpellData();
            Spells.RemoveAll(i => !HeroManager.Enemies.Any(
                            a =>
                            string.Equals(
                                a.CharacterName,
                                i.CharacterName,
                                StringComparison.InvariantCultureIgnoreCase)));
            try
            {
                evadeMenu = new Menu("EvadeOthers", "Block Other skils");
                {
                    evadeMenu.Add(new MenuBool("W", "Use W"));
                    foreach (var hero in
                        HeroManager.Enemies.Where(
                            i =>
                            Spells.Any(
                                a =>
                                string.Equals(
                                    a.CharacterName,
                                    i.CharacterName,
                                    StringComparison.InvariantCultureIgnoreCase))))
                    {
                        evadeMenu.Add(new Menu(hero.CharacterName.ToLowerInvariant(), "-> " + hero.CharacterName));
                    }
                }
            }
            catch { }
            try {
                foreach (var spell in
                    Spells.Where(
                        i =>
                        HeroManager.Enemies.Any(
                            a =>
                            string.Equals(
                                a.CharacterName,
                                i.CharacterName,
                                StringComparison.InvariantCultureIgnoreCase))))
                {
                    ((Menu)evadeMenu[spell.CharacterName.ToLowerInvariant()]).Add(new MenuBool(
                        spell.CharacterName + spell.Slot,
                        spell.CharacterName + " (" + (spell.Slot == SpellSlot.Unknown ? "Passive" : spell.Slot.ToString()) + ")",
                        false));
                }
            } catch {}
            Fiora.Config.Add(evadeMenu);
            Game.OnUpdate += Game_OnUpdate;
            GameObject.OnCreate += GameObject_OnCreate;
            GameObject.OnDelete += GameObject_OnDelete;
            AIBaseClient.OnDoCast += AIHeroClient_OnProcessSpellCast;
            AIBaseClient.OnPlayAnimation += AIHeroClient_OnPlayAnimation;
            AIHeroClient.OnBuffAdd += OnBuffGain;
            Dash.OnDash += Unit_OnDash;
        }

        private static void GameObject_OnDelete(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;
            if (missile == null || !missile.IsValid)
                return;
            var caster = missile.SpellCaster as AIHeroClient;
            if (!(caster is AIHeroClient) || caster.Team == Player.Team)
                return;
            if (missile.SData.Name == "FizzMarinerDoomMissile")
            {
                FizzFishEndPos = missile.Position.ToVector2();
            }
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            if (sender.Name == "Fizz_UltimateMissile_Orbit.troy" && FizzFishEndPos.IsValid()
                && sender.Position.ToVector2().Distance(FizzFishEndPos) <= 300)
            {
                FizzFishChum = sender;
                if (Variables.GameTimeTickCount >= FizzFishChumStartTick + 5000)
                    FizzFishChumStartTick = Variables.GameTimeTickCount;
            }
        }

        private static void Unit_OnDash(
    AIBaseClient sender,
    Dash.DashArgs args
)
        {
            var caster = sender as AIHeroClient;
            if (caster == null || !caster.IsValid || caster.Team == Player.Team)
            {
                return;
            }
            // riven dash
            if (caster.CharacterName == "Riven"
                && Fiora.Config["EvadeOthers"][("Riven").ToLowerInvariant()]
                .GetValue<MenuBool>("Riven" + SpellSlot.Q).Enabled)
            {
                RivenDashTick = Variables.GameTimeTickCount;
                RivenDashEnd = args.EndPos;
            }
        }

        private static void AIHeroClient_OnPlayAnimation(
    AIBaseClient sender,
    AIBaseClientPlayAnimationEventArgs args
)
        {
            var caster = sender as AIHeroClient;
            if (caster == null || !caster.IsValid || caster.Team == Player.Team)
            {
                return;
            }
            // riven Q3
            if (caster.CharacterName == "Riven"
                && Fiora.Config["EvadeOthers"][("Riven").ToLowerInvariant()]
                .GetValue<MenuBool>("Riven" + SpellSlot.Q).Enabled
                && args.Animation.ToLower() == "spell1c")
            {
                RivenQ3Tick = Variables.GameTimeTickCount;
                if (caster.HasBuff("RivenFengShuiEngine"))
                    RivenQ3Rad = 150;
                else
                    RivenQ3Rad = 225;
            }
            // others
            var spellDatas =
               Spells.Where(
                   i =>
                   caster.CharacterName.ToLowerInvariant() == i.CharacterName.ToLowerInvariant()
                   && Fiora.Config["EvadeOthers"][i.CharacterName.ToLowerInvariant()]
                   .GetValue<MenuBool>(i.CharacterName + i.Slot).Enabled);
            if (!spellDatas.Any())
            {
                return;
            }
            foreach (var spellData in spellDatas)
            {
                //reksaj W
                if (!Player.HasBuff("reksaiknockupimmune") && spellData.CharacterName == "Reksai"
                    && spellData.Slot == SpellSlot.W && args.Animation == "Spell2_knockup")// chua test
                {
                    if (Player.Position.ToVector2().Distance(caster.Position.ToVector2())
                        <= Player.BoundingRadius + caster.BoundingRadius + caster.AttackRange)
                        SolveInstantBlock();
                    return;
                }
            }
        }

        private static void AIHeroClient_OnProcessSpellCast(
    AIBaseClient sender,
    AIBaseClientProcessSpellCastEventArgs args
)
        {
            var caster = sender as AIHeroClient;
            if (caster == null || !caster.IsValid || caster.Team == Player.Team || caster.DistanceToPlayer() >  W.Range)
            {
                return;
            }
            var spellDatas =
               Spells.Where(
                   i =>
                   caster.CharacterName.ToLowerInvariant() == i.CharacterName.ToLowerInvariant()
                   && Fiora.Config["EvadeOthers"][i.CharacterName.ToLowerInvariant()]
                   .GetValue<MenuBool>(i.CharacterName + i.Slot).Enabled);
            if (!spellDatas.Any())
            {
                return;
            }
            foreach (var spellData in spellDatas)
            {
                // auto attack
                if (Orbwalker.IsAutoAttack(args.SData.Name) && args.Target != null && args.Target.IsMe)
                {
                    if (spellData.CharacterName == "Jax" && spellData.Slot == SpellSlot.W && caster.HasBuff("JaxEmpowerTwo"))
                    {
                        SolveInstantBlock();
                        return;
                    }
                    if (spellData.CharacterName == "Yorick" && spellData.Slot == SpellSlot.Q && caster.HasBuff("YorickSpectral"))
                    {
                        SolveInstantBlock();
                        return;
                    }
                    if (spellData.CharacterName == "Poppy" && spellData.Slot == SpellSlot.Q && caster.HasBuff("PoppyDevastatingBlow"))
                    {
                        SolveInstantBlock();
                        return;
                    }
                    if (spellData.CharacterName == "Rengar" && spellData.Slot == SpellSlot.Q
                        && (caster.HasBuff("rengarqbase") || caster.HasBuff("rengarqemp")))
                    {
                        SolveInstantBlock();
                        return;
                    }
                    if (spellData.CharacterName == "Nautilus" && spellData.Slot == SpellSlot.Unknown
                        && (!Player.HasBuff("nautiluspassivecheck")))
                    {
                        SolveInstantBlock();
                        return;
                    }
                    if (spellData.CharacterName == "Udyr" && spellData.Slot == SpellSlot.E && caster.HasBuff("UdyrBearStance")
                        && (Player.HasBuff("udyrbearstuncheck")))
                    {
                        SolveInstantBlock();
                        return;
                    }
                    return;
                }
                // aoe
                if (spellData.CharacterName == "Riven" && spellData.Slot == SpellSlot.W && args.Slot == SpellSlot.W)// chua test
                {
                    if (Player.Position.ToVector2().Distance(caster.Position.ToVector2())
                        <= Player.BoundingRadius + caster.BoundingRadius + caster.AttackRange)
                        SolveInstantBlock();
                    return;
                }
                if (spellData.CharacterName == "Diana" && spellData.Slot == SpellSlot.E && args.Slot == SpellSlot.E)// chua test
                {
                    if (Player.Position.ToVector2().Distance(caster.Position.ToVector2())
                        <= Player.BoundingRadius + 450)
                        SolveInstantBlock();
                    return;
                }
                if (spellData.CharacterName == "Maokai" && spellData.Slot == SpellSlot.R && args.SData.Name == "maokaidrain3toggle")
                {
                    if (Player.Position.ToVector2().Distance(caster.Position.ToVector2())
                        <= Player.BoundingRadius + 575)
                        SolveInstantBlock();
                    return;
                }
                if (spellData.CharacterName == "Kalista" && spellData.Slot == SpellSlot.E && args.Slot == SpellSlot.E)
                {
                    if (Player.Position.ToVector2().Distance(caster.Position.ToVector2())
                        <= 950
                        && Player.HasBuff("kalistaexpungemarker"))
                        SolveInstantBlock();
                    return;
                }
                if (spellData.CharacterName == "Kennen" && spellData.Slot == SpellSlot.W && args.Slot == SpellSlot.W)// chua test
                {
                    if (Player.Position.ToVector2().Distance(caster.Position.ToVector2())
                        <= 800
                        && Player.HasBuff("kennenmarkofstorm") && Player.GetBuffCount("kennenmarkofstorm") == 2)
                        SolveInstantBlock();
                    return;
                }
                
                if (spellData.CharacterName == "Tryndamere" && spellData.Slot == SpellSlot.W && args.Slot == SpellSlot.W)
                {
                    if (Player.Position.ToVector2().Distance(caster.Position.ToVector2())
                        <= Player.BoundingRadius + 850)
                        SolveInstantBlock();
                    return;
                }
                if (spellData.CharacterName == "Sett" && spellData.Slot == SpellSlot.E && args.Slot == SpellSlot.E)
                {
                    if (Player.Position.ToVector2().Distance(caster.Position.ToVector2())
                        <= Player.BoundingRadius + 490)
                        SolveInstantBlock();
                    return;
                }
                if (spellData.CharacterName == "Lissandra" && spellData.Slot == SpellSlot.W && args.Slot == SpellSlot.W)
                {
                    if (Player.Position.ToVector2().Distance(caster.Position.ToVector2())
                        <= Player.BoundingRadius + 490)
                        SolveInstantBlock();
                    return;
                }
                
                if (spellData.CharacterName == "Sett" && spellData.Slot == SpellSlot.R && args.Slot == SpellSlot.R && args.Target.IsMe)
                {
                    SolveInstantBlock();
                    return;
                }
                if (spellData.CharacterName == "Camille" && spellData.Slot == SpellSlot.R && args.Slot == SpellSlot.R && args.Target.IsMe)
                {
                    SolveInstantBlock();
                    return;
                }
                if (spellData.CharacterName == "Tristana" && spellData.Slot == SpellSlot.R && args.Slot == SpellSlot.R && args.Target.IsMe)
                {
                    SolveInstantBlock();
                    return;
                }
                //if (spellData.CharacterName == "Yasuo" && spellData.Slot == SpellSlot.Q && args.SData.Name == "YasuoQE3")
                //{
                //    if (args.Target.IsMe || Player.Position.ToVector2().Distance(caster.Position.ToVector2())
                //          <= Player.BoundingRadius + 375)
                //        SolveInstantBlock();
                //    return;
                //}
                if (spellData.CharacterName == "Yasuo" && spellData.Slot == SpellSlot.Q && args.SData.Name == "YasuoE" && caster.HasBuff("YasuoQ2"))
                {
                    if (Player.Position.ToVector2().Distance(args.To)
                          <= Player.BoundingRadius + 375)
                        Player.Spellbook.CastSpell(SpellSlot.W, args.To);
                    return;
                }
                if (spellData.CharacterName == "Lulu")
                {
                    if (spellData.Slot == SpellSlot.W && args.Slot == SpellSlot.W && args.Target.IsMe)
                    {
                        SolveInstantBlock();
                        return;
                    }

                    if (spellData.Slot == SpellSlot.R && args.Slot == SpellSlot.R && Player.Position.ToVector2().Distance(args.Target.Position.ToVector2())
                          <= Player.BoundingRadius + 150)
                    {
                        SolveInstantBlock();
                        return;
                    }

                    if (args.Target.IsMe || Player.Position.ToVector2().Distance(caster.Position.ToVector2())
                          <= Player.BoundingRadius + 375)
                        SolveInstantBlock();
                }
                if (spellData.CharacterName == "Qiyana" && spellData.Slot == SpellSlot.R && args.Slot == SpellSlot.R)
                {
                    if (Player.Position.ToVector2().Distance(caster.Position.ToVector2())
                        <= Player.BoundingRadius + 350)
                    {
                        Geometry.Rectangle rect = new Geometry.Rectangle(caster.Position, args.End, 280);
                        if (rect.IsInside(Player))
                        {
                            SolveInstantBlock();
                        }
                    }
                    return;
                }
                if (spellData.CharacterName == "Jax" && spellData.Slot == SpellSlot.E && args.Slot == SpellSlot.E && sender.GetBuff("JaxCounterStrike") != null)
                {
                    if (Player.Position.ToVector2().Distance(caster.Position.ToVector2())
                        <= Player.BoundingRadius + 875)
                    {
                        SolveInstantBlock();
                    }
                    return;
                }
                // jax E
                var jax = HeroManager.Enemies.FirstOrDefault(x => x.CharacterName == "Jax" && x.IsValidTarget());

                if (jax != null && jax.HasBuff("JaxCounterStrike")
                    && Fiora.Config["EvadeOthers"][("Jax").ToLowerInvariant()]
                    .GetValue<MenuBool>(("Jax" + SpellSlot.E)).Enabled)
                {
                    var buff = jax.GetBuff("JaxCounterStrike");
                    if (buff != null)
                    {
                        if ((buff.EndTime - Game.Time) * 1000 <= 650 
                            + Game.Ping 
                            && Player.Position.ToVector2().Distance(jax.Position.ToVector2())
                            <= Player.BoundingRadius + jax.BoundingRadius + jax.AttackRange + 100)
                        {
                            SolveInstantBlock();
                            return;
                        }
                    }
                }

                if (spellData.CharacterName == "Azir" && spellData.Slot == SpellSlot.R && args.Slot == SpellSlot.R)// chua test
                {
                    Vector2 start = caster.Position.ToVector2().Extend(args.End.ToVector2(), -300);
                    Vector2 end = start.Extend(caster.Position.ToVector2(), 750);
                    Render.Circle.DrawCircle(start.ToVector3(), 50, System.Drawing.Color.Red);
                    Render.Circle.DrawCircle(end.ToVector3(), 50, System.Drawing.Color.Red);
                    float width = caster.Level >= 16 ? 125 * 6 / 2 :
                                caster.Level >= 11 ? 125 * 5 / 2 :
                                125 * 4 / 2;
                     var Rect = new Geometry.Rectangle(start, end, width);
                    if (!Rect.IsOutside(Player.Position.ToVector2()))
                    {
                        SolveInstantBlock();
                    }
                    return;
                }
            }
        }

        private static void Game_OnUpdate(EventArgs args)
        {
            if (Player.HasBuff("vladimirhemoplaguedebuff") && HeroManager.Enemies.Any(x => x.CharacterName == "Vladimir")
                && Fiora.Config["EvadeOthers"][("Vladimir").ToLowerInvariant()]
                .GetValue<MenuBool>("Vladimir" + SpellSlot.R).Enabled)
            {
                var buff = Player.GetBuff("vladimirhemoplaguedebuff");
                if (buff == null)
                    return;
                SolveBuffBlock(buff);
            }

            if (Player.HasBuff("zedrdeathmark") && HeroManager.Enemies.Any(x => x.CharacterName == "Zed")
                && Fiora.Config["EvadeOthers"][("Zed").ToLowerInvariant()]
                .GetValue<MenuBool>("Zed" + SpellSlot.R).Enabled)
            {
                var buff = Player.GetBuff("zedrdeathmark");
                if (buff == null)
                    return;
                SolveBuffBlock(buff);
            }
            if (Player.HasBuff("tristanaechargesound") && HeroManager.Enemies.Any(x => x.CharacterName == "Tristana")
                && Fiora.Config["EvadeOthers"][("Tristana").ToLowerInvariant()]
                .GetValue<MenuBool>("Tristana" + SpellSlot.E).Enabled)
            {
                var buff = Player.GetBuff("tristanaechargesound");
                if (buff == null)
                    return;
                SolveBuffBlock(buff);
            }

            if (Player.HasBuff("SoulShackles") || Player.HasBuff("morganardebuff") && HeroManager.Enemies.Any(x => x.CharacterName == "Morgana")
                && Fiora.Config["EvadeOthers"][("Morgana").ToLowerInvariant()]
                .GetValue<MenuBool>("Morgana" + SpellSlot.R).Enabled)
            {
                var buff = Player.GetBuff("SoulShackles");
                if (buff == null)
                    buff = Player.GetBuff("morganardebuff");                
                if (buff == null)
                    return;
                SolveBuffBlock(buff);
            }

            if (Player.HasBuff("NocturneUnspeakableHorror") && HeroManager.Enemies.Any(x => x.CharacterName == "Nocturne")
                && Fiora.Config["EvadeOthers"][("Nocturne").ToLowerInvariant()]
                .GetValue<MenuBool>("Nocturne" + SpellSlot.E).Enabled)
            {
                var buff = Player.GetBuff("NocturneUnspeakableHorror");
                if (buff == null)
                    return;
                SolveBuffBlock(buff);
            }

            if (Player.HasBuff("karthusfallenonetarget") && HeroManager.Enemies.Any(x => x.CharacterName == "Karthus")
                && Fiora.Config["EvadeOthers"][("Karthus").ToLowerInvariant()]
                .GetValue<MenuBool>("Karthus" + SpellSlot.R).Enabled)
            {
                var buff = Player.GetBuff("karthusfallenonetarget");
                if (buff == null)
                    return;
                SolveBuffBlock(buff);
            }

            if (Player.HasBuff("KarmaSpiritBind") && HeroManager.Enemies.Any(x => x.CharacterName == "Karma")
                && Fiora.Config["EvadeOthers"][("Karma").ToLowerInvariant()]
                .GetValue<MenuBool>("Karma" + SpellSlot.R).Enabled)
            {
                var buff = Player.GetBuff("KarmaSpiritBind");
                if (buff == null)
                    return;
                SolveBuffBlock(buff);
            }

            if (Player.HasBuff("zoeesleepcountdown") && HeroManager.Enemies.Any(x => x.CharacterName == "Zoe")
                && Fiora.Config["EvadeOthers"][("Zoe").ToLowerInvariant()]
                .GetValue<MenuBool>("Zoe" + SpellSlot.E).Enabled)
            {
                var buff = Player.GetBuff("zoeesleepcountdown");
                if (buff == null)
                    return;
                SolveBuffBlock(buff);
            }

            if ((Player.HasBuff("LeblancE") || (Player.HasBuff("LeblancRE")))
                && HeroManager.Enemies.Any(x => x.CharacterName == "Leblanc")
                && Fiora.Config["EvadeOthers"][("Leblanc").ToLowerInvariant()]
                .GetValue<MenuBool>("Leblanc" + SpellSlot.E).Enabled)
            {
                var buff = Player.GetBuff("LeblancE");
                if (buff != null)
                {
                    SolveBuffBlock(buff);
                    return;
                }
                var buff2 = Player.GetBuff("LeblancRE");
                if (buff2 != null)
                {
                    SolveBuffBlock(buff2);
                    return;
                }
            }

            // jax E
            var jax = HeroManager.Enemies.FirstOrDefault(x => x.CharacterName == "Jax" && x.IsValidTarget());

            if (jax != null && jax.HasBuff("JaxCounterStrike")
                && Fiora.Config["EvadeOthers"][("Jax").ToLowerInvariant()]
                .GetValue<MenuBool>("Jax" + SpellSlot.E).Enabled)
            {
                var buff = jax.GetBuff("JaxCounterStrike");
                if (buff != null)
                {
                    //Game.Print(buff.EndTime - Game.Time);
                    if ((buff.EndTime - Game.Time) * 1000 <= 650 
                        + Game.Ping
                        && Player.Position.ToVector2().Distance(jax.Position.ToVector2())
                        <= Player.BoundingRadius + jax.BoundingRadius + jax.AttackRange + 100)
                    {
                        SolveInstantBlock();
                        return;
                    }
                }
            }

            //maokai R
            var maokai = HeroManager.Enemies.FirstOrDefault(x => x.CharacterName == "Maokai" && x.IsValidTarget());
            if (maokai != null && maokai.HasBuff("MaokaiDrain3")
                && Fiora.Config["EvadeOthers"][("Maokai").ToLowerInvariant()]
                .GetValue<MenuBool>("Maokai" + SpellSlot.R).Enabled)
            {
                var buff = maokai.GetBuff("MaokaiDrain3");
                if (buff != null)
                {
                    if (Player.Position.ToVector2().Distance(maokai.Position.ToVector2())
                        <= Player.BoundingRadius + 475)
                        SolveBuffBlock(buff);
                }
            }

            // nautilus R
            if (Player.HasBuff("nautilusgrandlinetarget") && HeroManager.Enemies.Any(x => x.CharacterName == "Nautilus")
                && Fiora.Config["EvadeOthers"][("Nautilus").ToLowerInvariant()]
                .GetValue<MenuBool>("Nautilus" + SpellSlot.R).Enabled)
            {
                var buff = Player.GetBuff("nautilusgrandlinetarget");
                if (buff == null)
                    return;
                var obj = ObjectManager.Get<GameObject>().Where(x => x.Name == "GrandLineSeeker").FirstOrDefault();
                if (obj == null)
                    return;
                if (obj.Position.ToVector2().Distance(Player.Position.ToVector2()) <= 300 + 70 
                    * Game.Ping / 1000
                    )
                {
                    SolveInstantBlock();
                    return;
                }
            }

            //rammus Q
            var ramus = HeroManager.Enemies.FirstOrDefault(x => x.CharacterName == "Rammus" && x.IsValidTarget());
            if (ramus != null
                && Fiora.Config["EvadeOthers"][("Rammus").ToLowerInvariant()]
                .GetValue<MenuBool>("Rammus" + SpellSlot.Q).Enabled)
            {
                var buff = ramus.GetBuff("PowerBall");
                if (buff != null)
                {
                    var waypoints = ramus.GetWaypoints();
                    if (waypoints.Count == 1)
                    {
                        if (Player.Position.ToVector2().Distance(ramus.Position.ToVector2())
                            <= Player.BoundingRadius + ramus.AttackRange + ramus.BoundingRadius)
                        {
                            SolveInstantBlock();
                            return;
                        }
                    }
                    else
                    {
                        if (Player.Position.ToVector2().Distance(ramus.Position.ToVector2())
                            <= Player.BoundingRadius + ramus.AttackRange + ramus.BoundingRadius
                            + ramus.MoveSpeed * (0.5f + 
                            Game.Ping / 1000
                            ))
                        {
                            if (waypoints.Any(x => x.Distance(Player.Position.ToVector2())
                                <= Player.BoundingRadius + ramus.AttackRange + ramus.BoundingRadius + 70))
                            {
                                SolveInstantBlock();
                                return;
                            }
                            for (int i = 0; i < waypoints.Count() - 2; i++)
                            {
                                if (Player.Position.ToVector2().Distance(waypoints[i], waypoints[i + 1], true)
                                    <= Player.BoundingRadius + ramus.BoundingRadius + ramus.AttackRange + 70)
                                {
                                    SolveInstantBlock();
                                    return;
                                }
                            }
                        }
                    }
                }
            }

            //fizzR
            if (HeroManager.Enemies.Any(x => x.CharacterName == "Fizz")
                && Fiora.Config["EvadeOthers"][("Fizz").ToLowerInvariant()]
                .GetValue<MenuBool>("Fizz" + SpellSlot.R).Enabled)
            {
                if (FizzFishChum != null && FizzFishChum.IsValid
                    && Variables.GameTimeTickCount - FizzFishChumStartTick >= 1500 - 250 - Game.Ping
                    && Player.Position.ToVector2().Distance(FizzFishChum.Position.ToVector2()) <= 250 + Player.BoundingRadius)
                {
                    SolveInstantBlock();
                    return;
                }
            }

            //nocturne R
            var nocturne = HeroManager.Enemies.FirstOrDefault(x => x.CharacterName == "Nocturne" && x.IsValidTarget());
            if (nocturne != null
                && Fiora.Config["EvadeOthers"][("Nocturne").ToLowerInvariant()]
                .GetValue<MenuBool>("Nocturne" + SpellSlot.R).Enabled)
            {
                var buff = Player.GetBuff("nocturneparanoiadash");
                if (buff != null && Player.Position.ToVector2().Distance(nocturne.Position.ToVector2()) <= 300 + 120 )//* Game.Ping / 1000)
                {
                    SolveInstantBlock();
                    return;
                }
            }


            // rivenQ3
            var riven = HeroManager.Enemies.FirstOrDefault(x => x.CharacterName == "Riven" && x.IsValidTarget());
            if (riven != null && Fiora.Config["EvadeOthers"][("Riven").ToLowerInvariant()]
                .GetValue<MenuBool>("Riven" + SpellSlot.Q).Enabled && RivenDashEnd.IsValid())
            {
                if (Variables.GameTimeTickCount - RivenDashTick <= 100 && Variables.GameTimeTickCount - RivenQ3Tick <= 100
                    && Math.Abs(RivenDashTick - RivenQ3Tick) <= 100 && Player.Position.ToVector2().Distance(RivenDashEnd) <= RivenQ3Rad)
                {
                    SolveInstantBlock();
                    return;
                }
            }

        }


        private static void OnBuffGain(AIBaseClient sender, AIBaseClientBuffAddEventArgs args)
        {
            var caster = sender as AIHeroClient;
            if (caster == null || !caster.IsValid || (!caster.IsMe && !caster.IsEnemy))
            {
                return;
            }
            var spellDatas =
               Spells.Where(
                   i =>
                   caster.CharacterName.ToLowerInvariant() == i.CharacterName.ToLowerInvariant()
                   && Fiora.Config["EvadeOthers"][i.CharacterName.ToLowerInvariant()]
                   .GetValue<MenuBool>(i.CharacterName + i.Slot).Enabled);
            if (!spellDatas.Any())
            {
                return;
            }

            foreach (var spellData in spellDatas)
            {
                if (spellData.CharacterName == "Neeko" && spellData.Slot == SpellSlot.R && caster.HasBuff("neekor2"))
                {
                    SolveDelayBlock((int)(.9f * 1000 - 250 
                        - Game.Ping
                        ));
                    return;
                }
            }
        }

        private static void SolveBuffBlock(BuffInstance buff)
        {
            if (Player.IsDead || Player.HasBuffOfType(BuffType.SpellShield) || Player.HasBuffOfType(BuffType.SpellImmunity)
                || !Fiora.Config["EvadeOthers"].GetValue<MenuBool>("W").Enabled || !Fiora.W.IsReady())
                return;
            if (buff == null)
                return;

            var endTime = buff.EndTime;
            if(buff.Name == "NocturneUnspeakableHorror")
            {
                endTime = endTime - 0.9f;
            }
            //Game.Print("detected:" + (buff.EndTime - Game.Time) * 1000);
            if ((buff.EndTime - Game.Time) * 1000 <= 250 + Game.Ping
                )
            {
                //Game.Print("Find target");
                var tar = TargetSelector.GetTarget(Fiora.W.Range, DamageType.Physical);
                if (tar.IsValidTarget(Fiora.W.Range))
                    Player.Spellbook.CastSpell(SpellSlot.W, tar.Position);
                else
                {
                    var hero = HeroManager.Enemies.FirstOrDefault(x => x.IsValidTarget(Fiora.W.Range));
                    if (hero != null)
                        Player.Spellbook.CastSpell(SpellSlot.W, hero.Position);
                    else
                        Player.Spellbook.CastSpell(SpellSlot.W, Player.Position.Extend(Game.CursorPos, 100));
                }
            }
        }

        private static void SolveDelayBlock(int timeDelayMinisecond)
        {
            if (Player.IsDead || Player.HasBuffOfType(BuffType.SpellShield) || Player.HasBuffOfType(BuffType.SpellImmunity)
                || !Fiora.Config["EvadeOthers"].GetValue<MenuBool>("W").Enabled || !Fiora.W.IsReady())
                return;
            DelayAction.Add(timeDelayMinisecond, () =>
            {
                var tar = TargetSelector.GetTarget(Fiora.W.Range, DamageType.Physical);
                if (tar.IsValidTarget(Fiora.W.Range))
                    Player.Spellbook.CastSpell(SpellSlot.W, tar.Position);
                else
                {
                    var hero = HeroManager.Enemies.FirstOrDefault(x => x.IsValidTarget(Fiora.W.Range));
                    if (hero != null)
                        Player.Spellbook.CastSpell(SpellSlot.W, hero.Position);
                    else
                        Player.Spellbook.CastSpell(SpellSlot.W, Player.Position.Extend(Game.CursorPos, 100));
                }
            });

            
        }
        private static void SolveInstantBlock()
        {
            if (Player.IsDead || Player.HasBuffOfType(BuffType.SpellShield) || Player.HasBuffOfType(BuffType.SpellImmunity)
                || !Fiora.Config["EvadeOthers"].GetValue<MenuBool>("W").Enabled || !Fiora.W.IsReady())
                return;
            var tar = TargetSelector.GetTarget(Fiora.W.Range, DamageType.Physical);
            if (tar.IsValidTarget(Fiora.W.Range))
                Player.Spellbook.CastSpell(SpellSlot.W, tar.Position);
            else
            {
                var hero = HeroManager.Enemies.FirstOrDefault(x => x.IsValidTarget(Fiora.W.Range));
                if (hero != null)
                    Player.Spellbook.CastSpell(SpellSlot.W, hero.Position);
                else
                    Player.Spellbook.CastSpell(SpellSlot.W, Player.Position.Extend(Game.CursorPos, 100));
            }
        }
        private static void LoadSpellData()
        {
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Azir",
                    Slot = SpellSlot.R,
                });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Fizz",
                    Slot = SpellSlot.R,
                });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Jax",
                    Slot = SpellSlot.W,
                });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Jax",
                    Slot = SpellSlot.E,
                });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Riven",
                    Slot = SpellSlot.Q,
                });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Riven",
                    Slot = SpellSlot.W,
                });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Diana",
                    Slot = SpellSlot.E,
                });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Kalista",
                    Slot = SpellSlot.E,
                });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Karma",
                    Slot = SpellSlot.W,
                });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Karthus",
                    Slot = SpellSlot.R,
                });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Kennen",
                    Slot = SpellSlot.W,
                });
            //Spells.Add(
            //    new SpellData
            //    {
            //        CharacterName = "Leesin",
            //        Slot = SpellSlot.Q,
            //    });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Leblanc",
                    Slot = SpellSlot.E,
                });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Lulu",
                    Slot = SpellSlot.W,
                });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Lulu",
                    Slot = SpellSlot.R,
                });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Maokai",
                    Slot = SpellSlot.R,
                });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Morgana",
                    Slot = SpellSlot.R,
                });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Nautilus",
                    Slot = SpellSlot.Unknown,
                });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Nautilus",
                    Slot = SpellSlot.R,
                });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Neeko",
                    Slot = SpellSlot.R,
                });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Nocturne",
                    Slot = SpellSlot.E,
                });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Nocturne",
                    Slot = SpellSlot.R,
                });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Qiyana",
                    Slot = SpellSlot.R,
                });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Rammus",
                    Slot = SpellSlot.Q,
                });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Rengar",
                    Slot = SpellSlot.Q,
                });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Reksai",
                    Slot = SpellSlot.W,
                });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Tryndamere",
                    Slot = SpellSlot.E,
                });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Sett",
                    Slot = SpellSlot.E,
                });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Sett",
                    Slot = SpellSlot.R,
                });

            Spells.Add(
                new SpellData
                {
                    CharacterName = "Lissandra",
                    Slot = SpellSlot.W,
                });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Camille",
                    Slot = SpellSlot.R,
                });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Vladimir",
                    Slot = SpellSlot.R,
                });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Zed",
                    Slot = SpellSlot.R,
                });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Zoe",
                    Slot = SpellSlot.E,
                });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Tristana",
                    Slot = SpellSlot.E,
                });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Tristana",
                    Slot = SpellSlot.R,
                });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Udyr",
                    Slot = SpellSlot.E,
                });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Yorick",
                    Slot = SpellSlot.Q,
                });
            Spells.Add(
                new SpellData
                {
                    CharacterName = "Yasuo",
                    Slot = SpellSlot.Q,
                });
        }
        private class SpellData
        {


            public string CharacterName;

            public SpellSlot Slot;


        }
    }
    class Fiora
    {
        private static AIHeroClient Player { get { return ObjectManager.Player; } }

        public static Spell Q, W, E, R;

        private const float LaneClearWaitTimeMod = 2f;

        public static Menu Config;

        internal class TargetedNoMissile
        {
            private static readonly List<SpellData> Spells = new List<SpellData>();

            private static readonly List<DashTarget> DetectedDashes = new List<DashTarget>();
            internal static void Init()
            {
                LoadSpellData();
                Spells.RemoveAll(i => !HeroManager.Enemies.Any(
                a =>
                string.Equals(
                    a.CharacterName,
                    i.CharacterName,
                    StringComparison.InvariantCultureIgnoreCase)));
                var evadeMenu = new Menu("EvadeTargetNone", "Evade Targeted None-SkillShot");
                {
                    evadeMenu.Add(new MenuBool("W", "Use W"));
                    //var aaMenu = new Menu("Auto Attack", "AA");
                    //{
                    //    aaMenu.Bool("B", "Basic Attack");
                    //    aaMenu.Slider("BHpU", "-> If Hp < (%)", 35);
                    //    aaMenu.Bool("C", "Crit Attack");
                    //    aaMenu.Slider("CHpU", "-> If Hp < (%)", 40);
                    //    evadeMenu.Add(aaMenu);
                    //}
                    try
                    {
                        foreach (var hero in
                            HeroManager.Enemies.Where(
                                i =>
                                Spells.Any(
                                    a =>
                                    string.Equals(
                                        a.CharacterName,
                                        i.CharacterName,
                                        StringComparison.InvariantCultureIgnoreCase))))
                        {
                            evadeMenu.Add(new Menu(hero.CharacterName.ToLowerInvariant(), "-> " + hero.CharacterName));
                        }
                    } catch { }
                    try
                    {
                        foreach (var spell in
                            Spells.Where(
                                i =>
                                HeroManager.Enemies.Any(
                                    a =>
                                    string.Equals(
                                        a.CharacterName,
                                        i.CharacterName,
                                        StringComparison.InvariantCultureIgnoreCase))))
                        {
                            ((Menu)evadeMenu[spell.CharacterName.ToLowerInvariant()]).Add(new MenuBool(
                                spell.CharacterName + spell.Slot,
                                spell.CharacterName + " (" + spell.Slot + ")",
                                false));
                        }
                    } catch { }
                }
                Fiora.Config.Add(evadeMenu);
                Game.OnUpdate += OnUpdateDashes;
                AIHeroClient.OnDoCast += AIHeroClient_OnProcessSpellCast;
            }

            private static void AIHeroClient_OnProcessSpellCast(
    AIBaseClient sender,
    AIBaseClientProcessSpellCastEventArgs args
)
            {
                var caster = sender as AIHeroClient;
                if (caster == null || !caster.IsValid || caster.Team == Player.Team || !(args.Target != null && args.Target.IsMe))
                {
                    return;
                }
                var spellData =
                   Spells.FirstOrDefault(
                       i =>
                       caster.CharacterName.ToLowerInvariant() == i.CharacterName.ToLowerInvariant()
                       && (i.UseSpellSlot ? args.Slot == i.Slot :
                       i.SpellNames.Any(x => x.ToLowerInvariant() == args.SData.Name.ToLowerInvariant()))
                       && Fiora.Config["EvadeTargetNone"][i.CharacterName.ToLowerInvariant()]
                       .GetValue<MenuBool>(i.CharacterName + i.Slot).Enabled);
                if (spellData == null)
                {
                    return;
                }
                if (spellData.IsDash)
                {
                    DetectedDashes.Add(new DashTarget { Hero = caster, DistanceDash = spellData.DistanceDash, TickCount = Variables.GameTimeTickCount });
                }
                else
                {
                    if (Player.IsDead)
                    {
                        return;
                    }
                    if (Player.HasBuffOfType(BuffType.SpellShield) || Player.HasBuffOfType(BuffType.SpellImmunity))
                    {
                        return;
                    }
                    if (!Fiora.Config["EvadeTargetNone"].GetValue<MenuBool>("W").Enabled || !W.IsReady())
                    {
                        return;
                    }
                    var tar = TargetSelector.GetTarget(W.Range, DamageType.Magical);
                    if (tar.IsValidTarget(W.Range))
                    {

                        Player.Spellbook.CastSpell(SpellSlot.W, tar.Position);
                    }
                    else
                    {
                        var hero = HeroManager.Enemies.FirstOrDefault(x => x.IsValidTarget(W.Range));
                        if (hero != null)
                        {
                            //Game.Print("Will Cast W3");
                            //W.Cast(hero.Position);
                            Player.Spellbook.CastSpell(SpellSlot.W, hero.Position);

                        }
                        else
                        {

                            //Game.Print("Will Cast W4");
                            //W.Cast(Player.Position.Extend(caster.Position, 100));
                            Player.Spellbook.CastSpell(SpellSlot.W, Player.Position.Extend(caster.Position, 100));
                        }
                    }
                }
            }

            private static void OnUpdateDashes(EventArgs args)
            {
                DetectedDashes.RemoveAll(
                    x =>
                    x.Hero == null || !x.Hero.IsValid
                    || (!x.Hero.IsDashing() && Variables.GameTimeTickCount > x.TickCount + 500));

                if (Player.IsDead)
                {
                    return;
                }
                if (Player.HasBuffOfType(BuffType.SpellShield) || Player.HasBuffOfType(BuffType.SpellImmunity))
                {
                    return;
                }
                if (!Fiora.Config["EvadeTargetNone"].GetValue<MenuBool>("W").Enabled || !W.IsReady())
                {
                    return;
                }
                foreach (var target in
                     DetectedDashes.OrderBy(i => i.Hero.Position.Distance(Player.Position)))
                {
                    var dashdata = target.Hero.GetDashInfo();
                    if (dashdata != null && target.Hero.Position.ToVector2().Distance(Player.Position.ToVector2())
                        < target.DistanceDash + Game.Ping * dashdata.Speed / 1000)
                    {
                        var tar = TargetSelector.GetTarget(W.Range, DamageType.Magical);
                        if (tar.IsValidTarget(W.Range))
                            Player.Spellbook.CastSpell(SpellSlot.W, tar.Position);
                        else
                        {
                            var hero = HeroManager.Enemies.FirstOrDefault(x => x.IsValidTarget(W.Range));
                            if (hero != null)
                                Player.Spellbook.CastSpell(SpellSlot.W, hero.Position);
                            else
                                Player.Spellbook.CastSpell(SpellSlot.W, Player.Position.Extend(target.Hero.Position, 100));
                        }
                    }
                }
            }


            private static void LoadSpellData()
            {
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Alistar",
                        UseSpellSlot = true,
                        Slot = SpellSlot.W
                    });
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Alistar",
                        UseSpellSlot = true,
                        Slot = SpellSlot.W
                    });
                //blitz
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Blitzcrank",
                        Slot = SpellSlot.E,
                        SpellNames = new[] { "PowerFistAttack" }
                    });
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Brand",
                        UseSpellSlot = true,
                        Slot = SpellSlot.E
                    });
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Chogath",
                        UseSpellSlot = true,
                        Slot = SpellSlot.R
                    });
                //darius W confirmed
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Darius",
                        Slot = SpellSlot.W,
                        SpellNames = new[] { "DariusNoxianTacticsONHAttack" }
                    });

                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Darius",
                        UseSpellSlot = true,
                        Slot = SpellSlot.R
                    });
                //ekkoE confirmed
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Ekko",
                        Slot = SpellSlot.E,
                        SpellNames = new[] { "EkkoEAttack" }
                    });
                //eliseQ confirm
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Elise",
                        Slot = SpellSlot.Q,
                        SpellNames = new[] { "EliseSpiderQCast" }
                    });
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Evelynn",
                        UseSpellSlot = true,
                        Slot = SpellSlot.E,
                    });
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Fiddlesticks",
                        UseSpellSlot = true,
                        Slot = SpellSlot.Q,
                    });
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Fizz",
                        UseSpellSlot = true,
                        Slot = SpellSlot.Q,
                    });
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Garen",
                        Slot = SpellSlot.Q,
                        SpellNames = new[] { "GarenQAttack" }
                    });
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Garen",
                        UseSpellSlot = true,
                        Slot = SpellSlot.R,
                    });
                // hercarim E confirmed
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Hecarim",
                        Slot = SpellSlot.E,
                        SpellNames = new[] { "HecarimRampAttack" }
                    });
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Irelia",
                        UseSpellSlot = true,
                        Slot = SpellSlot.Q,
                        IsDash = true
                    });
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Jarvan",
                        UseSpellSlot = true,
                        Slot = SpellSlot.R,
                    });

                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Sett",
                        UseSpellSlot = true,
                        Slot = SpellSlot.R
                    });
                ////jax W later
                //Spells.Add(
                //    new SpellData
                //    {
                //        CharacterName = "Jax",
                //        Slot = SpellSlot.W,
                //        SpellNames = new[] { "JaxEmpowerAttack", "JaxEmpowerTwo" }
                //    });
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Jax",
                        UseSpellSlot = true,
                        Slot = SpellSlot.Q,
                        IsDash = true
                    });
                //jax R confirmed
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Jax",
                        Slot = SpellSlot.R,
                        SpellNames = new[] { "JaxRelentlessAttack" }
                    });
                //jayce Q confirm
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Jayce",
                        Slot = SpellSlot.Q,
                        SpellNames = new[] { "JayceToTheSkies" },
                        IsDash = true,
                        DistanceDash = 400
                    });
                //jayce E confirm
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Jayce",
                        Slot = SpellSlot.E,
                        SpellNames = new[] { "JayceThunderingBlow" }
                    });
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Khazix",
                        UseSpellSlot = true,
                        Slot = SpellSlot.Q,
                    });
                //leesin Q2 later
                //Spells.Add(
                //    new SpellData
                //    {
                //        CharacterName = "Leesin",
                //        Slot = SpellSlot.Q,
                //        SpellNames = new[] { "BlindMonkQTwo" },
                //        IsDash = true
                //    });
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Leesin",
                        UseSpellSlot = true,
                        Slot = SpellSlot.R,
                    });
                //leona Q confirmed
                Spells.Add(
                   new SpellData
                   {
                       CharacterName = "Leona",
                       Slot = SpellSlot.Q,
                       SpellNames = new[] { "LeonaShieldOfDaybreakAttack" }
                   });
                // lissandra R
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Lissandra",
                        UseSpellSlot = true,
                        Slot = SpellSlot.R,
                    });
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Lucian",
                        UseSpellSlot = true,
                        Slot = SpellSlot.Q,
                    });
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Malzahar",
                        UseSpellSlot = true,
                        Slot = SpellSlot.E,
                    });
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Malzahar",
                        UseSpellSlot = true,
                        Slot = SpellSlot.R,
                    });
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Maokai",
                        UseSpellSlot = true,
                        Slot = SpellSlot.W,
                        IsDash = true
                    });
                // mordekaiser R confirmed
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Mordekaiser",
                        UseSpellSlot = true,
                        Slot = SpellSlot.R,
                    });
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Nasus",
                        Slot = SpellSlot.Q,
                        SpellNames = new[] { "NasusQAttack" }
                    });
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "Nasus",
                        UseSpellSlot = true,
                        Slot = SpellSlot.W,
                    });
                Spells.Add(
                    new SpellData
                    {
                        CharacterName = "MonkeyKing",
                        Slot = SpellSlot.Q,
                        SpellNames = new[] { "MonkeyKingQAttack" }
                    });
                //nidalee Q confirmed
                Spells.Add(
                     new SpellData
                     {
                         CharacterName = "Nidalee",
                         Slot = SpellSlot.Q,
                         SpellNames = new[] { "NidaleeTakedownAttack", "Nidalee_CougarTakedownAttack" }
                     });
                Spells.Add(
                     new SpellData
                     {
                         CharacterName = "Olaf",
                         UseSpellSlot = true,
                         Slot = SpellSlot.E,
                     });
                Spells.Add(
                     new SpellData
                     {
                         CharacterName = "Pantheon",
                         UseSpellSlot = true,
                         Slot = SpellSlot.W,
                     });
                //poppy Q later
                //Spells.Add(
                //     new SpellData
                //     {
                //         CharacterName = "Poppy",
                //         Slot = SpellSlot.Q,
                //         SpellNames = new[] { "PoppyDevastatingBlow" }
                //     });
                Spells.Add(
                     new SpellData
                     {
                         CharacterName = "Poppy",
                         UseSpellSlot = true,
                         Slot = SpellSlot.E,
                     });
                Spells.Add(
                     new SpellData
                     {
                         CharacterName = "Poppy",
                         UseSpellSlot = true,
                         Slot = SpellSlot.R,
                     });
                Spells.Add(
                     new SpellData
                     {
                         CharacterName = "Quinn",
                         UseSpellSlot = true,
                         Slot = SpellSlot.E,
                     });
                Spells.Add(
                     new SpellData
                     {
                         CharacterName = "Rammus",
                         UseSpellSlot = true,
                         Slot = SpellSlot.E,
                     });
                Spells.Add(
                     new SpellData
                     {
                         CharacterName = "RekSai",
                         UseSpellSlot = true,
                         Slot = SpellSlot.E,
                     });
                Spells.Add(
                     new SpellData
                     {
                         CharacterName = "Renekton",
                         Slot = SpellSlot.W,
                         SpellNames = new[] { "RenektonExecute", "RenektonSuperExecute" }
                     });
                Spells.Add(
                     new SpellData
                     {
                         CharacterName = "Ryze",
                         UseSpellSlot = true,
                         Slot = SpellSlot.W,
                     });
                Spells.Add(
                     new SpellData
                     {
                         CharacterName = "Singed",
                         UseSpellSlot = true,
                         Slot = SpellSlot.E,
                     });
                Spells.Add(
                     new SpellData
                     {
                         CharacterName = "Skarner",
                         UseSpellSlot = true,
                         Slot = SpellSlot.R,
                     });
                Spells.Add(
                     new SpellData
                     {
                         CharacterName = "TahmKench",
                         UseSpellSlot = true,
                         Slot = SpellSlot.W,
                     });
                Spells.Add(
                     new SpellData
                     {
                         CharacterName = "Talon",
                         UseSpellSlot = true,
                         Slot = SpellSlot.E,
                     });
                //talonQ confirmed
                Spells.Add(
                     new SpellData
                     {
                         CharacterName = "Talon",
                         Slot = SpellSlot.Q,
                         SpellNames = new[] { "TalonNoxianDiplomacyAttack" }
                     });
                Spells.Add(
                     new SpellData
                     {
                         CharacterName = "Trundle",
                         UseSpellSlot = true,
                         Slot = SpellSlot.R,
                     });
                //udyr E : todo : check for stun buff
                Spells.Add(
                     new SpellData
                     {
                         CharacterName = "Udyr",
                         Slot = SpellSlot.E,
                         SpellNames = new[] { "UdyrBearAttack", "UdyrBearAttackUlt" }
                     });
                Spells.Add(
                     new SpellData
                     {
                         CharacterName = "Vi",
                         UseSpellSlot = true,
                         Slot = SpellSlot.R,
                         IsDash = true,
                     });
                Spells.Add(
                     new SpellData
                     {
                         CharacterName = "Shen",
                         UseSpellSlot = true,
                         Slot = SpellSlot.E,
                         IsDash = true,
                     });
                //viktor Q confirmed
                Spells.Add(
                     new SpellData
                     {
                         CharacterName = "Viktor",
                         Slot = SpellSlot.Q,
                         SpellNames = new[] { "ViktorQBuff" }
                     });
                Spells.Add(
                     new SpellData
                     {
                         CharacterName = "Vladimir",
                         UseSpellSlot = true,
                         Slot = SpellSlot.Q,
                     });
                Spells.Add(
                     new SpellData
                     {
                         CharacterName = "Volibear",
                         UseSpellSlot = true,
                         Slot = SpellSlot.W,
                     });
                //volibear Q confirmed
                Spells.Add(
                     new SpellData
                     {
                         CharacterName = "Volibear",
                         Slot = SpellSlot.Q,
                         SpellNames = new[] { "VolibearQAttack" }
                     });
                Spells.Add(
                     new SpellData
                     {
                         CharacterName = "Warwick",
                         UseSpellSlot = true,
                         Slot = SpellSlot.Q,
                     });
                Spells.Add(
                     new SpellData
                     {
                         CharacterName = "Warwick",
                         UseSpellSlot = true,
                         Slot = SpellSlot.R,
                     });
                //xinzhaoQ3 confirmed
                Spells.Add(
                     new SpellData
                     {
                         CharacterName = "XinZhao",
                         Slot = SpellSlot.Q,
                         SpellNames = new[] { "XenZhaoThrust3" }
                     });
                Spells.Add(
                     new SpellData
                     {
                         CharacterName = "Yorick",
                         UseSpellSlot = true,
                         Slot = SpellSlot.E,
                     });
                //yorick Q
                //Spells.Add(
                //     new SpellData
                //     {
                //         CharacterName = "Yorick",
                //         Slot = SpellSlot.Q,
                //         SpellNames = new[] {"" }
                //     });
                Spells.Add(
                 new SpellData
                 {
                     CharacterName = "Zilean",
                     UseSpellSlot = true,
                     Slot = SpellSlot.E,
                 });
            }
            

            private class SpellData
            {
            

                public string CharacterName;

                public bool UseSpellSlot = false;

                public SpellSlot Slot;

                public string[] SpellNames = { };

                public bool IsDash = false;

                public float DistanceDash = 200;

            

                public string MissileName
                {
                    get
                    {
                        return this.SpellNames.First();
                    }
                }

         
            }
            private class DashTarget
            {
               

                public AIHeroClient Hero;

                public float DistanceDash = 200;

                public int TickCount;

              
            }
        }

        public Fiora()
        {
            Q = new Spell(SpellSlot.Q, 400);
            W = new Spell(SpellSlot.W, 750);
            E = new Spell(SpellSlot.E);
            R = new Spell(SpellSlot.R);
            W.SetSkillshot(0.75f, 80, 2000, false, SpellType.Line);
            W.MinHitChance = HitChance.High;


            Config = new Menu(Player.CharacterName, "Fiora By Badao L#", true);
            Menu spellMenu = Config.Add(new Menu("Spell", "Spell"));

            Menu Harass = spellMenu.Add(new Menu("Harass", "Harass"));

            Menu Combo = spellMenu.Add(new Menu("Combo", "Combo"));

            Menu Target = Config.Add(new Menu("TargetingModes", "Targeting Modes"));

            Menu PriorityMode = Target.Add(new Menu("Priority", "Priority Mode"));

            Menu OptionalMode = Target.Add(new Menu("Optional", "Optional Mode"));

            Menu SelectedMode = Target.Add(new Menu("Selected", "Selected Mode"));

            Menu LaneClear = spellMenu.Add(new Menu("LaneClear", "Lane Clear"));


            Menu JungClear = spellMenu.Add(new Menu("JungleClear", "Jungle Clear"));

            Menu Misc = Config.Add(new Menu("Misc", "Misc"));

            Menu Draw = Config.Add(new Menu("Draw", "Draw")); ;

            Harass.Add(new MenuBool("UseQHarass", "Q Enable"));
            Harass.Add(new MenuBool("UseQHarassGap", "Use Q to gapclose"));
            Harass.Add(new MenuBool("UseQHarassPrePass", "Use Q to hit pre-passivespot"));
            Harass.Add(new MenuBool("UseQHarassPass", "Use Q to hit passive"));
            Harass.Add(new MenuBool("UseEHarass", "E Enable"));
            Harass.Add(new MenuSlider("ManaHarass", "Mana Harass", 40, 0, 100));

            Combo.Add(new MenuBool("UseQCombo", "Q Enable"));
            Combo.Add(new MenuBool("UseQComboGap", "Use Qtogapclose"));
            Combo.Add(new MenuBool("UseQComboPrePass", "Use Q to hit pre-passive spot"));
            Combo.Add(new MenuBool("UseQComboPass", "Use Q to hit passive"));
            Combo.Add(new MenuBool("UseQComboGapMinion", "Use Q minion to gapclose", false));
            Combo.Add(new MenuSlider("UseQComboGapMinionValue", "Q minion gapclose if % cdr>=", 25, 0, 40));
            Combo.Add(new MenuBool("UseECombo", "E Enable"));
            Combo.Add(new MenuBool("UseRCombo", "R Enable"));
            Combo.Add(new MenuBool("UseRComboLowHP", "Use R Low HP"));
            Combo.Add(new MenuSlider("UseRComboLowHPValue", "R Low HP if player hp<", 40, 0, 100));
            Combo.Add(new MenuBool("UseRComboKillable", "Use R Killable"));
            Combo.Add(new MenuBool("UseRComboOnTap", "Use R on Tap"));
            Combo.Add(new MenuKeyBind("UseRComboOnTapKey", "R on Tap key", Keys.G, KeyBindType.Press));
            Combo.Add(new MenuBool("UseRComboAlways", "Use R Always", false));

            Target.Add(new MenuList("TargetingMode", "Targeting Mode", new[] { "Optional", "Selected", "Priority", "Normal" }));
            Target.Add(new MenuSlider("OrbwalkToPassiveRange", "Orbwalk To Passive Range", 300, 250, 500));
            Target.Add(new MenuBool("FocusUltedTarget", "Focus Ulted Target", false));

            PriorityMode.Add(new MenuSlider("PriorityRange", "Priority Range", 1000, 300, 1000));
            PriorityMode.Add(new MenuBool("PriorityOrbwalktoPassive", "Orbwalk to Passive"));
            PriorityMode.Add(new MenuBool("PriorityUnderTower", "Under Tower"));
            try
            {
                foreach (var hero in HeroManager.Enemies)
                {
                    PriorityMode.Add(new MenuSlider("Priority" + hero.CharacterName, hero.CharacterName, 2, 1, 5));
                }
            }
            catch { }

            OptionalMode.Add(new MenuSlider("OptionalRange", "Optional Range", 1000, 300, 1000));
            OptionalMode.Add(new MenuBool("OptionalOrbwalktoPassive", "Orbwalk to Passive"));
            OptionalMode.Add(new MenuBool("OptionalUnderTower", "Under Tower", false));
            OptionalMode.Add(new MenuKeyBind("OptionalSwitchTargetKey", "Switch Target Key", Keys.T, KeyBindType.Press));
            OptionalMode.Add(new MenuBool("Note5", "Also Can Left-click the target to switch!"));

            SelectedMode.Add(new MenuSlider("SelectedRange", "Selected Range", 1000, 300, 1000));
            SelectedMode.Add(new MenuBool("SelectedOrbwalktoPassive", "Orbwalk to Passive"));
            SelectedMode.Add(new MenuBool("SelectedUnderTower", "Under Tower", false));
            SelectedMode.Add(new MenuBool("SelectedSwitchIfNoSelected", "Switch to Optional if no target"));

            LaneClear.Add(new MenuBool("UseELClear", "E Enable"));
            LaneClear.Add(new MenuBool("UseTimatLClear", "Use Item"));
            LaneClear.Add(new MenuSlider("minimumManaLC", "minimum Mana", 40, 0, 100));

            JungClear.Add(new MenuBool("UseEJClear", "E Enable"));
            JungClear.Add(new MenuBool("UseTimatJClear", "Use Item"));
            JungClear.Add(new MenuSlider("minimumManaJC", "minimum Mana", 40, 0, 100));

            Misc.Add(new MenuKeyBind("WallJump", "Wall Jump", Keys.H, KeyBindType.Press));

            Draw.Add(new MenuBool("DrawQ", "Draw Q", false));
            Draw.Add(new MenuBool("DrawW", "Draw W", false));
            Draw.Add(new MenuBool("DrawOptionalRange", "Draw Optional Range"));
            Draw.Add(new MenuBool("DrawSelectedRange", "Draw Selected Range"));
            Draw.Add(new MenuBool("DrawPriorityRange", "Draw Priority Range"));
            Draw.Add(new MenuBool("DrawTarget", "Draw Target"));
            Draw.Add(new MenuBool("DrawVitals", "Draw Vitals", false));
            //Draw.Add(new MenuBool("DrawFastDamage", "DrawFastDamage", false)).ValueChanged += DrawHP_ValueChanged;

            if (HeroManager.Enemies.Any())
            {
                //DaoHungAIO.Evade.EvadeManager.Attach();
                //Evade.Evade.Init();
                EvadeTarget.Init();
                TargetedNoMissile.Init();
                OtherSkill.Init();
            }
            Config.Attach();
            Drawing.OnDraw += Drawing_OnDraw;
            Drawing.OnEndScene += Drawing_OnEndScene;

            GameObject.OnCreate += GameObject_OnCreate;
            Game.OnUpdate += Game_OnGameUpdate;
            Game.OnUpdate += OnActionDelegate;
            Orbwalker.OnAfterAttack += Orbwalker_OnAfterAttack;
            //AfterAttackNoTarget += Orbwalker_AfterAttackNoTarget; it is afterAttack with target null
            //OnAttack += OnAttack;
            AIBaseClient.OnProcessSpellCast += oncast;
            Game.OnWndProc +=  GetTargets.Game_OnWndProc;
            //Utility.HpBarDamageIndicator.DamageToUnit = GetFastDamage;
            //Utility.HpBarDamageIndicator.Enabled = DrawHP;
            //CustomDamageIndicator.Initialize(GetFastDamage);
            //CustomDamageIndicator.Enabled = DrawHP;

            //evade
            //FioraProject.Evade.Evade.Evading += EvadeSkillShots.Evading;
        }

        private void Orbwalker_OnAfterAttack(object sender, AfterAttackEventArgs e)
        {
            if (Orbwalker.ActiveMode == OrbwalkerMode.Combo)
            {
                if (Ecombo && E.IsReady())
                {
                    E.Cast();
                }
                else if (HasItem())
                {
                    CastItem();
                }
            }
            if (Orbwalker.ActiveMode == OrbwalkerMode.Harass && (e.Target is AIHeroClient))
            {
                if (Eharass && E.IsReady() && Player.ManaPercent >= Manaharass
                    && Player.CountEnemyHeroesInRange(Player.GetRealAutoAttackRange() + 200) >= 1)
                {
                    E.Cast();
                }
                else if (HasItem())
                {
                    CastItem();
                }
            }
            if (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear)
            {
                // jungclear
                if (EJclear && E.IsReady() && Player.Mana * 100 / Player.MaxMana >= ManaJclear && !ShouldWait()
                    && ObjectManager.Get<AIMinionClient>().Where(i => !i.IsDead && !i.IsAlly && i.MaxHealth > 10 && i.IsJungle()).Count(i => i.DistanceToPlayer() <= ObjectManager.Player.GetCurrentAutoAttackRange() + 200) >= 1)
                {
                    E.Cast();
                }
                else if (TimatJClear && HasItem() && !ShouldWait()
                    && ObjectManager.Get<AIMinionClient>().Where(i => !i.IsDead && !i.IsAlly && i.MaxHealth > 10 && i.IsJungle()).Count(i => i.DistanceToPlayer() <= ObjectManager.Player.GetCurrentAutoAttackRange() + 200) >= 1)
                {
                    CastItem();
                }
                // laneclear
                if (ELclear && E.IsReady() && Player.Mana * 100 / Player.MaxMana >= ManaLclear && !ShouldWait()
                    && Player.Position.CountMinionsInRange(Player.GetRealAutoAttackRange() + 200, false) >= 1)
                {
                    E.Cast();
                }
                else if (TimatLClear && HasItem() && !ShouldWait()
                    && Player.Position.CountMinionsInRange(Player.GetRealAutoAttackRange() + 200, false) >= 1)
                {
                    CastItem();
                }
            }

            if (Orbwalker.ActiveMode == OrbwalkerMode.Combo)
            {
                if (Ecombo && E.IsReady() && Player.CountEnemyHeroesInRange(Player.GetRealAutoAttackRange() + 200) >= 1)
                {
                    E.Cast();
                }
                else if (HasItem() && Player.CountEnemyHeroesInRange(Player.GetRealAutoAttackRange() + 200) >= 1)
                {
                    CastItem();
                }
            }
            if (Orbwalker.ActiveMode == OrbwalkerMode.Harass && (e.Target is AIHeroClient))
            {
                if (Eharass && E.IsReady() && Player.ManaPercent >= Manaharass
                    && Player.CountEnemyHeroesInRange(Player.GetRealAutoAttackRange() + 200) >= 1)
                {
                    E.Cast();
                }
                else if (HasItem() && Player.CountEnemyHeroesInRange(Player.GetRealAutoAttackRange() + 200) >= 1)
                {
                    CastItem();
                }
            }
            if (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear)
            {
                // jungclear
                if (EJclear && E.IsReady() && Player.Mana * 100 / Player.MaxMana >= ManaJclear && !ShouldWait()
                    && Player.Position.CountMinionsInRange(Player.GetRealAutoAttackRange() + 200, true) >= 1)
                {
                    E.Cast();
                }
                else if (TimatJClear && HasItem() && !ShouldWait()
                    && Player.Position.CountMinionsInRange(Player.GetRealAutoAttackRange() + 200, true) >= 1)
                {
                    CastItem();
                }
                // laneclear
                if (ELclear && E.IsReady() && Player.Mana * 100 / Player.MaxMana >= ManaLclear && !ShouldWait()
                    && Player.Position.CountMinionsInRange(Player.GetRealAutoAttackRange() + 200, false) >= 1)
                {
                    E.Cast();
                }
                else if (TimatLClear && HasItem() && !ShouldWait()
                    && Player.Position.CountMinionsInRange(Player.GetRealAutoAttackRange() + 200, false) >= 1)
                {
                    CastItem();
                }
            }
        }

        private static void OnActionDelegate(
    EventArgs args
)
        {
            if (FunnySlayerCommon.OnAction.AfterAA)
            {
                

                /*if (args.Target != null)
                {

                    if (!args.Sender.IsMe)
                        return;
                    
                }
                else
                {
                    if (!args.Sender.IsMe)
                        return;
                    
                }*/
            }
            if (FunnySlayerCommon.OnAction.OnAA)
            {
                if ((Orbwalker.ActiveMode == OrbwalkerMode.Combo))
                {
                    if (Player.CanUseItem((int)ItemId.Youmuus_Ghostblade))
                        Player.UseItem((int)ItemId.Youmuus_Ghostblade);
                }
            }
        }

        private static void GameObject_OnCreate(GameObject sender, EventArgs args)
        {
            //if (!sender.Name.ToLower().Contains("fiora"))
            //    return;
            //Game.PrintChat(sender.Name + sender.Type    );
        }


        private static bool ShouldWait()
        {
            return
                ObjectManager.Get<AIMinionClient>()
                    .Any(
                        minion =>
                            minion.IsValidTarget() && minion.Team != GameObjectTeam.Neutral &&
                            minion.InAutoAttackRange() &&
                            HealthPrediction.GetPrediction(
                                minion, (int)((Player.AttackDelay * 1000) * LaneClearWaitTimeMod)) <=
                            Player.GetAutoAttackDamage(minion));
        }
        private static void Orbwalker_AfterAttackNoTarget(AttackableUnit unit, AttackableUnit target)
        {

        }

        private static void Game_OnGameUpdate(EventArgs args)
        {
            //GameObjects.AllGameObjects.Where(g => g.Name.Contains("FioraPassive")).ForEach(e =>
            //{
            //    Game.Print(e.Name);
            //});
            if (Player.IsDead)
                return;
            FioraPassiveUpdate();
            WallJump();
            if (Orbwalker.ActiveMode == OrbwalkerMode.Combo)
            {
                Combo();
            }
            if (Orbwalker.ActiveMode == OrbwalkerMode.Harass)
            {
                Harass();
            }
            if (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear)
            {

            }
        }
        private static void oncast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            var spell = args.SData;
            if (!sender.IsMe)
                return;
            if (spell.Name.Contains("ItemTiamatCleave"))
            {

            }
            if (spell.Name.Contains("FioraQ"))
            {

            }
            if (spell.Name == "FioraE")
            {

                Orbwalker.ResetAutoAttackTimer();
            }
            if (spell.Name == "ItemTitanicHydraCleave")
            {
                Orbwalker.ResetAutoAttackTimer();
            }
            if (spell.Name.ToLower().Contains("fiorabasicattack"))
            {
            }

        }



        //harass
        public static bool Qharass { get { return Fiora.Config["Spell"]["Harass"].GetValue<MenuBool>("UseQHarass").Enabled;}}
        public static bool Eharass {get{return Fiora.Config["Spell"]["Harass"].GetValue<MenuBool>("UseEHarass").Enabled;}}
        public static bool CastQGapCloseHarass {get{return Fiora.Config["Spell"]["Harass"].GetValue<MenuBool>("UseQHarassGap").Enabled;}}
        public static bool CastQPrePassiveHarass {get{return Fiora.Config["Spell"]["Harass"].GetValue<MenuBool>("UseQHarassPrePass").Enabled;}}
        public static bool CastQPassiveHarasss {get{return Fiora.Config["Spell"]["Harass"].GetValue<MenuBool>("UseQHarassPass").Enabled;}}
        public static int Manaharass {get{return Fiora.Config["Spell"]["Harass"].GetValue<MenuSlider>("ManaHarass").Value;}}

        //combo
        public static bool Qcombo {get{return Fiora.Config["Spell"]["Combo"].GetValue<MenuBool>("UseQCombo").Enabled;}}
        public static bool Ecombo {get{return Fiora.Config["Spell"]["Combo"].GetValue<MenuBool>("UseECombo").Enabled;}}
        public static bool CastQGapCloseCombo {get{return Fiora.Config["Spell"]["Combo"].GetValue<MenuBool>("UseQComboGap").Enabled;}}
        public static bool CastQPrePassiveCombo {get{return Fiora.Config["Spell"]["Combo"].GetValue<MenuBool>("UseQComboPrePass").Enabled;}}
        public static bool CastQPassiveCombo {get{return Fiora.Config["Spell"]["Combo"].GetValue<MenuBool>("UseQComboPass").Enabled;}}
        public static bool CastQMinionGapCloseCombo {get{return Fiora.Config["Spell"]["Combo"].GetValue<MenuBool>("UseQComboGapMinion").Enabled;}}
        public static int ValueQMinionGapCloseCombo {get{return Fiora.Config["Spell"]["Combo"].GetValue<MenuSlider>("UseQComboGapMinionValue").Value;}}
        public static bool Rcombo {get{return Fiora.Config["Spell"]["Combo"].GetValue<MenuBool>("UseRCombo").Enabled;}}
        public static bool UseRComboLowHP {get{return Fiora.Config["Spell"]["Combo"].GetValue<MenuBool>("UseRComboLowHP").Enabled;}}
        public static int ValueRComboLowHP {get{return Fiora.Config["Spell"]["Combo"].GetValue<MenuSlider>("UseRComboLowHPValue").Value;}}
        public static bool UseRComboKillable {get{return Fiora.Config["Spell"]["Combo"].GetValue<MenuBool>("UseRComboKillable").Enabled;}}
        public static bool UseRComboOnTap {get{return Fiora.Config["Spell"]["Combo"].GetValue<MenuBool>("UseRComboOnTap").Enabled;}}
        public static bool RTapKeyActive {get{return Fiora.Config["Spell"]["Combo"].GetValue<MenuKeyBind>("UseRComboOnTapKey").Active;}}
        public static bool UseRComboAlways {get{return Fiora.Config["Spell"]["Combo"].GetValue<MenuBool>("UseRComboAlways").Enabled;}}

        //jclear&&lclear
        public static bool ELclear {get{return Fiora.Config["Spell"]["LaneClear"].GetValue<MenuBool>("UseELClear").Enabled;}}
        public static bool TimatLClear {get{return Fiora.Config["Spell"]["LaneClear"].GetValue<MenuBool>("UseTimatLClear").Enabled;}}
        public static bool EJclear {get{return Fiora.Config["Spell"]["JungleClear"].GetValue<MenuBool>("UseEJClear").Enabled;}}
        public static bool TimatJClear {get{return Fiora.Config["Spell"]["JungleClear"].GetValue<MenuBool>("UseTimatJClear").Enabled;}}
        public static int ManaJclear {get{return Fiora.Config["Spell"]["JungleClear"].GetValue<MenuSlider>("minimumManaJC").Value;}}
        public static int ManaLclear {get{return Fiora.Config["Spell"]["LaneClear"].GetValue<MenuSlider>("minimumManaLC").Value;}}

        //orbwalkpassive
        public static float OrbwalkToPassiveRange {get{return Fiora.Config["TargetingModes"].GetValue<MenuSlider>("OrbwalkToPassiveRange").Value;}}
        public static bool OrbwalkToPassiveTargeted {get{return Fiora.Config["TargetingModes"]["Selected"].GetValue<MenuBool>("SelectedOrbwalktoPassive").Enabled;}}
        public static bool OrbwalkToPassiveOptional {get{return Fiora.Config["TargetingModes"]["Optional"].GetValue<MenuBool>("OptionalOrbwalktoPassive").Enabled;}}
        public static bool OrbwalkToPassivePriority {get{return Fiora.Config["TargetingModes"]["Priority"].GetValue<MenuBool>("PriorityOrbwalktoPassive").Enabled;}}
        public static bool OrbwalkTargetedUnderTower {get{return Fiora.Config["TargetingModes"]["Selected"].GetValue<MenuBool>("SelectedUnderTower").Enabled;}}
        public static bool OrbwalkOptionalUnderTower {get{return Fiora.Config["TargetingModes"]["Optional"].GetValue<MenuBool>("OptionalUnderTower").Enabled;}}
        public static bool OrbwalkPriorityUnderTower {get{return Fiora.Config["TargetingModes"]["Priority"].GetValue<MenuBool>("PriorityUnderTower").Enabled;}}

        //orbwalklastclick


        public static bool DrawQ {get{return Fiora.Config["Draw"].GetValue<MenuBool>("DrawQ").Enabled;}}
        public static bool DrawW {get{return Fiora.Config["Draw"].GetValue<MenuBool>("DrawW").Enabled;}}
        public static bool DrawQcast {get{return Fiora.Config["Draw"].GetValue<MenuBool>("DrawQcast").Enabled;}}
        public static bool DrawOptionalRange {get{return Fiora.Config["Draw"].GetValue<MenuBool>("DrawOptionalRange").Enabled;}}
        public static bool DrawSelectedRange {get{return Fiora.Config["Draw"].GetValue<MenuBool>("DrawSelectedRange").Enabled;}}
        public static bool DrawPriorityRange {get{return Fiora.Config["Draw"].GetValue<MenuBool>("DrawPriorityRange").Enabled;}}
        public static bool DrawTarget {get{return Fiora.Config["Draw"].GetValue<MenuBool>("DrawTarget").Enabled;}}
        public static bool DrawHP {get{return Fiora.Config["Draw"].GetValue<MenuBool>("DrawFastDamage").Enabled;}}
        public static bool DrawVitals {get{return Fiora.Config["Draw"].GetValue<MenuBool>("DrawVitals").Enabled;}}
        //private static void DrawHP_ValueChanged(Object sender,
        // EventArgs e)
        //{
        //    var value = sender as MenuBool;
        //    if (sender != null)
        //    {
        //        //Utility.HpBarDamageIndicator.Enabled = e.GetNewValue<bool>();
        //        CustomDamageIndicator.Enabled = value;
        //    }
        //}
        private static void Drawing_OnDraw(EventArgs args)
        {
            if (Player.IsDead)
                return;
            if (DrawQ)
                Render.Circle.DrawCircle(Player.Position, 400, Color.Green);
            if (DrawW)
            {
                Render.Circle.DrawCircle(Player.Position, W.Range, Color.Green);
            }
            if (DrawOptionalRange && TargetingMode == GetTargets.TargetMode.Optional)
            {
                Render.Circle.DrawCircle(Player.Position, OptionalRange, Color.DeepPink);
            }
            if (DrawSelectedRange && TargetingMode == GetTargets.TargetMode.Selected)
            {
                Render.Circle.DrawCircle(Player.Position, SelectedRange, Color.DeepPink);
            }
            if (DrawPriorityRange && TargetingMode == GetTargets.TargetMode.Priority)
            {
                Render.Circle.DrawCircle(Player.Position, PriorityRange, Color.DeepPink);
            }
            if (DrawTarget && TargetingMode != GetTargets.TargetMode.Normal)
            {
                var hero = GetTarget();
                if (hero != null)
                    Render.Circle.DrawCircle(hero.Position, 75, Color.Yellow, 5);
            }
            if (DrawVitals && TargetingMode != GetTargets.TargetMode.Normal)
            {
                var hero = GetTarget();
                if (hero != null)
                {
                    var status = hero.GetPassiveStatus(0f);
                    if (status.HasPassive && status.PassivePredictedPositions.Any())
                    {
                        foreach (var x in status.PassivePredictedPositions)
                        {
                            Render.Circle.DrawCircle(x.ToVector3(), 50, Color.Yellow);
                        }
                    }
                }
            }
            if (activewalljump)
            {
                var Fstwall = GetFirstWallPoint(Player.Position.ToVector2(), Game.CursorPos.ToVector2());
                if (Fstwall != null)
                {
                    var firstwall = ((Vector2)Fstwall);
                    var pos = firstwall.Extend(Game.CursorPos.ToVector2(), 100);
                    var Lstwall = GetLastWallPoint(firstwall, Game.CursorPos.ToVector2());
                    if (Lstwall != null)
                    {
                        var lastwall = ((Vector2)Lstwall);
                        if (InMiddileWall(firstwall, lastwall))
                        {
                            for (int i = 0; i <= 359; i++)
                            {
                                var pos1 = pos.RotateAround(firstwall, i);
                                var pos2 = firstwall.Extend(pos1, 400);
                                if (pos1.InTheCone(firstwall, Game.CursorPos.ToVector2(), 60) && pos1.IsWall() && !pos2.IsWall())
                                {
                                    Render.Circle.DrawCircle(firstwall.ToVector3(), 50, Color.Green);
                                    goto Finish;
                                }
                            }

                            Render.Circle.DrawCircle(firstwall.ToVector3(), 50, Color.Red);
                        }
                    }
                }
                Finish:;
            }

        }
        private static void Drawing_OnEndScene(EventArgs args)
        {
        }


        private static bool usewalljump = true;
        private static bool activewalljump { get { return Fiora.Config["Misc"].GetValue<MenuKeyBind>("WallJump").Active; } }
        private static int movetick;
        private static void WallJump()
        {
            if (usewalljump && activewalljump)
            {
                var Fstwall = GetFirstWallPoint(Player.Position.ToVector2(), Game.CursorPos.ToVector2());
                if (Fstwall != null)
                {
                    var firstwall = ((Vector2)Fstwall);
                    var Lstwall = GetLastWallPoint(firstwall, Game.CursorPos.ToVector2());
                    if (Lstwall != null)
                    {
                        var lastwall = ((Vector2)Lstwall);
                        if (InMiddileWall(firstwall, lastwall))
                        {
                            var y = Player.Position.Extend(Game.CursorPos, 30);
                            for (int i = 20; i <= 300; i = i + 20)
                            {
                                if (Variables.GameTimeTickCount - movetick < (70 + Math.Min(60, Game.Ping)))
                                    break;
                                if (Player.Distance(Game.CursorPos) <= 1200 && Player.Position.ToVector2().Extend(Game.CursorPos.ToVector2(), i).IsWall())
                                {
                                    Player.IssueOrder(GameObjectOrder.MoveTo, Player.Position.ToVector2().Extend(Game.CursorPos.ToVector2(), i - 20).ToVector3());
                                    movetick = Variables.GameTimeTickCount;
                                    break;
                                }
                                Player.IssueOrder(GameObjectOrder.MoveTo,
                                    Player.Distance(Game.CursorPos) <= 1200 ?
                                    Player.Position.ToVector2().Extend(Game.CursorPos.ToVector2(), 200).ToVector3() :
                                    Game.CursorPos);
                            }
                            if (y.IsWall() && Prediction.GetPrediction(Player, 500).UnitPosition.ToVector2().Distance(Player.Position) <= 10 && Q.IsReady())
                            {
                                var pos = Player.Position.ToVector2().Extend(Game.CursorPos.ToVector2(), 100);
                                for (int i = 0; i <= 359; i++)
                                {
                                    var pos1 = pos.RotateAround(Player.Position.ToVector2(), i);
                                    var pos2 = Player.Position.ToVector2().Extend(pos1, 400);
                                    if (pos1.InTheCone(Player.Position.ToVector2(), Game.CursorPos.ToVector2(), 60) && pos1.IsWall() && !pos2.IsWall())
                                    {
                                        Q.Cast(pos2);
                                    }

                                }
                            }
                        }
                        else if (Variables.GameTimeTickCount - movetick >= (70 + Math.Min(60, Game.Ping)))
                        {
                            Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                            movetick = Variables.GameTimeTickCount;
                        }
                    }
                    else if (Variables.GameTimeTickCount - movetick >= (70 + Math.Min(60, Game.Ping)))
                    {
                        Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                        movetick = Variables.GameTimeTickCount;
                    }
                }
                else if (Variables.GameTimeTickCount - movetick >= (70 + Math.Min(60, Game.Ping)))
                {
                    Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                    movetick = Variables.GameTimeTickCount;
                }
            }
        }
        private static Vector2? GetFirstWallPoint(Vector2 from, Vector2 to, float step = 25)
        {
            var direction = (to - from).Normalized();

            for (float d = 0; d < from.Distance(to); d = d + step)
            {
                var testPoint = from + d * direction;
                var flags = NavMesh.GetCollisionFlags(testPoint.X, testPoint.Y);
                if (flags.HasFlag(CollisionFlags.Wall) || flags.HasFlag(CollisionFlags.Building))
                {
                    return from + (d - step) * direction;
                }
            }

            return null;
        }
        private static Vector2? GetLastWallPoint(Vector2 from, Vector2 to, float step = 25)
        {
            var direction = (to - from).Normalized();
            var Fstwall = GetFirstWallPoint(from, to);
            if (Fstwall != null)
            {
                var firstwall = ((Vector2)Fstwall);
                for (float d = step; d < firstwall.Distance(to) + 1000; d = d + step)
                {
                    var testPoint = firstwall + d * direction;
                    var flags = NavMesh.GetCollisionFlags(testPoint.X, testPoint.Y);
                    if (!flags.HasFlag(CollisionFlags.Wall) && !flags.HasFlag(CollisionFlags.Building))
                    //if (!testPoint.IsWall())
                    {
                        return firstwall + d * direction;
                    }
                }
            }

            return null;
        }
        private static bool InMiddileWall(Vector2 firstwall, Vector2 lastwall)
        {
            var midwall = new Vector2((firstwall.X + lastwall.X) / 2, (firstwall.Y + lastwall.Y) / 2);
            var point = midwall.Extend(Game.CursorPos.ToVector2(), 50);
            for (int i = 0; i <= 350; i = i + 10)
            {
                var testpoint = point.RotateAround(midwall, i);
                var flags = NavMesh.GetCollisionFlags(testpoint.X, testpoint.Y);
                if (!flags.HasFlag(CollisionFlags.Wall) && !flags.HasFlag(CollisionFlags.Building))
                {
                    return false;
                }
            }
            return true;
        }

    }

    public static class Combos
    {
        #region Clear

        #endregion Clear

        #region Harass
        public static void Harass()
        {
            //Qcast
            if (Fiora.Q.IsReady() && Fiora.Qharass && Player.ManaPercent >= Fiora.Manaharass)
            {
                if (Fiora.CastQPassiveHarasss || Fiora.CastQPrePassiveHarass || Fiora.CastQGapCloseHarass)
                {
                    if (TargetingMode == GetTargets.TargetMode.Normal)
                    {
                        foreach (var hero in HeroManager.Enemies.Where(x => x.IsValidTarget() && !x.IsDead)
                            .OrderBy(x => x.Distance(Player.Position)))
                        {
                            var status = hero.GetPassiveStatus(0);
                            if (status.HasPassive
                                && !(hero.InAutoAttackRange()
                                && status.PassivePredictedPositions.Any(x => Player.Position.ToVector2()
                                    .InTheCone(status.TargetPredictedPosition, x, 90))))
                            {
                                if (Fiora.CastQPassiveHarasss && status.PassiveType == FioraPassive.PassiveType.UltiPassive
                                    && castQtoUltPassive(hero, getQPassivedelay(hero)))
                                    goto Wcast;
                                if (Fiora.CastQPassiveHarasss && status.PassiveType == FioraPassive.PassiveType.NormalPassive
                                    && castQtoPassive(hero, getQPassivedelay(hero)))
                                    goto Wcast;
                                if (Fiora.CastQPrePassiveHarass && status.PassiveType == FioraPassive.PassiveType.PrePassive
                                    && castQtoPrePassive(hero, getQPassivedelay(hero)))
                                    goto Wcast;
                                if (Fiora.CastQGapCloseHarass
                                    && castQtoGapClose(hero, getQGapClosedelay(hero)))
                                    goto Wcast;
                            }
                        }
                    }
                    else
                    {
                        var hero = GetTargets.GetTarget();
                        if (hero != null)
                        {
                            var status = hero.GetPassiveStatus(0);
                            if (status.HasPassive
                                && !(hero.InAutoAttackRange()
                                && status.PassivePredictedPositions.Any(x => Player.Position.ToVector2()
                                    .InTheCone(status.TargetPredictedPosition, x, 90))))
                            {
                                if (Fiora.CastQPassiveHarasss && status.PassiveType == FioraPassive.PassiveType.UltiPassive
                                    && castQtoUltPassive(hero, getQPassivedelay(hero)))
                                    goto Wcast;
                                if (Fiora.CastQPassiveHarasss && status.PassiveType == FioraPassive.PassiveType.NormalPassive
                                    && castQtoPassive(hero, getQPassivedelay(hero)))
                                    goto Wcast;
                                if (Fiora.CastQPrePassiveHarass && status.PassiveType == FioraPassive.PassiveType.PrePassive
                                    && castQtoPrePassive(hero, getQPassivedelay(hero)))
                                    goto Wcast;
                                if (Fiora.CastQGapCloseHarass
                                    && castQtoGapClose(hero, getQGapClosedelay(hero)))
                                    goto Wcast;
                            }
                        }
                    }
                }
                if (Fiora.CastQGapCloseHarass && !Fiora.CastQPassiveHarasss && !Fiora.CastQPrePassiveHarass)
                {
                    if (TargetingMode == GetTargets.TargetMode.Normal)
                    {
                        foreach (var hero in HeroManager.Enemies.Where(x => x.IsValidTarget() && !x.IsDead)
                            .OrderBy(x => x.Distance(Player.Position)))
                        {
                            if (castQtoGapClose(hero, getQGapClosedelay(hero)))
                                goto Wcast;
                        }
                    }
                    else
                    {
                        var hero = GetTargets.GetTarget();
                        if (hero != null)
                        {
                            if (castQtoGapClose(hero, getQGapClosedelay(hero)))
                                goto Wcast;
                        }
                    }
                }
            }

            Wcast:

            if (W.IsReady())
            {

            }
        }
        #endregion Harass

        #region Combo

        public static void Combo()
        {
            //Qcast
            if (Q.IsReady() && Qcombo)
            {
                if (CastQPassiveCombo || CastQPrePassiveCombo || CastQGapCloseCombo)
                {
                    if (TargetingMode == GetTargets.TargetMode.Normal)
                    {
                        foreach (var hero in HeroManager.Enemies.Where(x => x.IsValidTarget() && !x.IsDead)
                            .OrderBy(x => x.Distance(Player.Position)))
                        {
                            var status = hero.GetPassiveStatus(0);
                            if (status.HasPassive
                                && !(hero.InAutoAttackRange()
                                && status.PassivePredictedPositions.Any(x => Player.Position.ToVector2()
                                    .InTheCone(status.TargetPredictedPosition, x, 90))))
                            {
                                if (Fiora.CastQPassiveCombo && status.PassiveType == FioraPassive.PassiveType.UltiPassive
                                    && castQtoUltPassive(hero, getQPassivedelay(hero)))
                                    goto Wcast;
                                if (Fiora.CastQPassiveCombo && status.PassiveType == FioraPassive.PassiveType.NormalPassive
                                    && castQtoPassive(hero, getQPassivedelay(hero)))
                                    goto Wcast;
                                if (Fiora.CastQPrePassiveCombo && status.PassiveType == FioraPassive.PassiveType.PrePassive
                                    && castQtoPrePassive(hero, getQPassivedelay(hero)))
                                    goto Wcast;
                                if (Fiora.CastQGapCloseCombo
                                    && castQtoGapClose(hero, getQGapClosedelay(hero)))
                                    goto Wcast;
                            }
                        }
                    }
                    else
                    {
                        var hero = GetTargets.GetTarget();
                        if (hero != null)
                        {
                            var status = hero.GetPassiveStatus(0);
                            if (status.HasPassive
                                && !(hero.InAutoAttackRange()
                                && status.PassivePredictedPositions.Any(x => Player.Position.ToVector2()
                                    .InTheCone(status.TargetPredictedPosition, x, 90))))
                            {
                                if (Fiora.CastQPassiveCombo && status.PassiveType == FioraPassive.PassiveType.UltiPassive
                                    && castQtoUltPassive(hero, getQPassivedelay(hero)))
                                    goto Wcast;
                                if (Fiora.CastQPassiveCombo && status.PassiveType == FioraPassive.PassiveType.NormalPassive
                                    && castQtoPassive(hero, getQPassivedelay(hero)))
                                    goto Wcast;
                                if (Fiora.CastQPrePassiveCombo && status.PassiveType == FioraPassive.PassiveType.PrePassive
                                    && castQtoPrePassive(hero, getQPassivedelay(hero)))
                                    goto Wcast;
                                if (Fiora.CastQGapCloseCombo
                                    && castQtoGapClose(hero, getQGapClosedelay(hero)))
                                    goto Wcast;
                            }
                        }
                    }
                }
                if (Fiora.CastQGapCloseCombo && !Fiora.CastQPassiveCombo && !Fiora.CastQPrePassiveCombo)
                {
                    if (TargetingMode == GetTargets.TargetMode.Normal)
                    {
                        foreach (var hero in HeroManager.Enemies.Where(x => x.IsValidTarget() && !x.IsDead)
                            .OrderBy(x => x.Distance(Player.Position)))
                        {
                            if (castQtoGapClose(hero, getQGapClosedelay(hero)))
                                goto Wcast;
                        }
                    }
                    else
                    {
                        var hero = GetTargets.GetTarget();
                        if (hero != null)
                        {
                            if (castQtoGapClose(hero, getQGapClosedelay(hero)))
                                goto Wcast;
                        }
                    }
                }
                if (Fiora.CastQMinionGapCloseCombo && Math.Abs(Player.PercentCooldownMod) * 100 >= Fiora.ValueQMinionGapCloseCombo)
                {
                    var hero =  GetTargets.GetTarget();
                    if (hero != null && Player.Position.Distance(hero.Position) >= 500)
                    {
                        if (Player.Position.Extend(hero.Position, 400).CountMinionsInRange(300, false) >= 1)
                            Fiora.Q.Cast(Player.Position.Extend(hero.Position, 400));
                    }
                }
            }

            Wcast:

            if (Fiora.W.IsReady())
            {

            }


            if (Fiora.R.IsReady() && Fiora.Rcombo)
            {
                var hero = GetTargets.GetTarget();
                if (hero.IsValidTarget(500) && !hero.IsDead)
                {
                    var status = hero.GetPassiveStatus(0);
                    if (!status.HasPassive || (status.HasPassive && !(hero.InAutoAttackRange()
                         && status.PassivePredictedPositions.Any(x => Player.Position.ToVector2()
                         .InTheCone(status.TargetPredictedPosition, x, 90)))))
                    {
                        if (Fiora.UseRComboLowHP && Player.HealthPercent <= Fiora.ValueRComboLowHP)
                        {
                            Fiora.R.Cast(hero);
                            return;
                        }

                        if (Fiora.UseRComboKillable && GetFastDamage(hero) >= hero.Health && hero.Health >= GetFastDamage(hero) / 3)
                        {
                            Fiora.R.Cast(hero);
                            return;
                        }

                        if (Fiora.UseRComboAlways)
                        {
                            Fiora.R.Cast(hero);
                            return;
                        }
                    }
                    if (Fiora.UseRComboOnTap && Fiora.RTapKeyActive)
                    {
                        Fiora.R.Cast(hero);
                        return;
                    }
                }
            }
        }
        #endregion Combo

        #region Damage
        public static float GetPassiveDamage(AIHeroClient target)
        {
            return
                (0.03f + (0.027f + 0.001f * Player.Level) * Player.FlatPhysicalDamageMod / 100) * target.MaxHealth;
        }
        public static float GetUltiPassiveDamage(AIHeroClient target)
        {
            return GetPassiveDamage(target) * 4;
        }
        public static float GetUltiDamage(AIHeroClient target)
        {
            return GetUltiPassiveDamage(target) + (float)Player.GetAutoAttackDamage(target) * 4;
        }
        public static float GetFastDamage(AIHeroClient target)
        {
            //var statuss = target.GetPassiveStatus(0);
            //if (statuss.HasPassive)
            //{
            //    return statuss.PassivePredictedPositions.Count() * (float)(GetPassiveDamage(target) + Player.GetAutoAttackDamage(target));
            //}
            //return 0;
            ////
            float damage = 0;
            damage += Fiora.Q.GetDamage(target);
            if (Fiora.Q.IsReady())
                damage += Fiora.Q.GetDamage(target);
            if (Fiora.R.IsReady())
            {
                damage += GetUltiDamage(target);
                return damage;
            }
            else
            {
                var status = target.GetPassiveStatus(0);
                if (status.HasPassive)
                {
                    damage += status.PassivePredictedPositions.Count() * (float)(GetPassiveDamage(target) + Player.GetAutoAttackDamage(target));
                    if (status.PassivePredictedPositions.Count() < 3)
                        damage += (3 - status.PassivePredictedPositions.Count()) * (float)Player.GetAutoAttackDamage(target);
                    return damage;
                }
                else
                {
                    damage += (float)Player.GetAutoAttackDamage(target) * 2;
                    return damage;
                }
            }
        }
        #endregion Damage

        #region CastRHelper
        #endregion CastRHelper

        #region CastWHelper

        #endregion CastWHelper

        #region CastQHelper
        private static float getQPassivedelay(AIHeroClient target)
        {
            if (target == null)
                return 0;
            FioraPassive.PassiveStatus targetStatus;
            if (Prediction.GetPrediction(target, 0.25f).UnitPosition.ToVector2().Distance(Player.Position.ToVector2())
                > Player.Position.ToVector2().Distance(target.Position.ToVector2()))
                targetStatus = target.GetPassiveStatus(Player.Position.ToVector2().Distance(target.Position.ToVector2()) / 1100);
            else
                targetStatus = target.GetPassiveStatus(0);
            if (!targetStatus.HasPassive)
                return 0;
            if (targetStatus.PassiveType == FioraPassive.PassiveType.PrePassive || targetStatus.PassiveType == FioraPassive.PassiveType.NormalPassive)
            {
                if (!targetStatus.PassivePredictedPositions.Any())
                    return 0;
                var pos = targetStatus.PassivePredictedPositions.First();
                return Player.Position.ToVector2().Distance(pos) / 1100 + Game.Ping / 1000;
            }
            if (targetStatus.PassiveType == FioraPassive.PassiveType.UltiPassive)
            {
                if (!targetStatus.PassivePredictedPositions.Any())
                    return 0;
                var poses = targetStatus.PassivePredictedPositions;
                var pos = poses.OrderBy(x => Player.Position.ToVector2().Distance(x)).First();
                return Player.Position.ToVector2().Distance(pos) / 1100 + Game.Ping / 1000;
            }
            return 0;
        }
        private static float getQGapClosedelay(AIHeroClient target)
        {
            var distance = Player.Distance(target.Position);
            return
                distance > 400 ?
                400 / 1100 + Game.Ping / 1000 :
                distance / 1100 + Game.Ping / 1000;
        }
        private static bool castQtoGapClose(AIHeroClient target, float delay)
        {
            if (target == null)
                return false;
            var targetpredictedpos = Prediction.GetPrediction(target, delay).UnitPosition.ToVector2();
            var pos = Player.Position.ToVector2().Distance(targetpredictedpos) > 400 ?
                Player.Position.ToVector2().Extend(targetpredictedpos, 400) : targetpredictedpos;
            if (targetpredictedpos.Distance(pos) <= 300 && !pos.IsWall())
            {
                Fiora.Q.Cast(pos);
                return true;
            }
            return false;
        }
        private static bool castQtoPrePassive(AIHeroClient target, float delay)
        {
            if (target == null)
                return false;
            var targetStatus = target.GetPassiveStatus(delay);
            if (targetStatus.PassiveType != FioraPassive.PassiveType.PrePassive)
                return false;
            if (!targetStatus.PassivePredictedPositions.Any())
                return false;
            var passivepos = targetStatus.PassivePredictedPositions.First();
            var poses = FioraPassive.GetRadiusPoints(targetStatus.TargetPredictedPosition, passivepos);
            var pos = poses.Where(x => x.Distance(Player.Position.ToVector2()) <= 400 && !x.IsWall())
                           .OrderBy(x => x.Distance(passivepos)).FirstOrDefault();
            if (pos == null || !pos.IsValid())
                return false;
            Fiora.Q.Cast(pos);
            return true;
        }
        private static bool castQtoPassive(AIHeroClient target, float delay)
        {
            if (target == null)
                return false;
            var targetStatus = target.GetPassiveStatus(delay);
            if (!targetStatus.HasPassive || targetStatus.PassiveType != FioraPassive.PassiveType.NormalPassive)
                return false;
            if (targetStatus.PassiveType != FioraPassive.PassiveType.NormalPassive)
                return false;
            if (!targetStatus.PassivePredictedPositions.Any())
                return false;
            var passivepos = targetStatus.PassivePredictedPositions.First();
            var poses = FioraPassive.GetRadiusPoints(targetStatus.TargetPredictedPosition, passivepos);
            var pos = poses.Where(x => x.Distance(Player.Position.ToVector2()) <= 400 && !x.IsWall())
                           .OrderBy(x => x.Distance(passivepos)).FirstOrDefault();
            if (pos == null || !pos.IsValid())
                return false;
            Fiora.Q.Cast(pos);
            return true;
        }
        private static bool castQtoUltPassive(AIHeroClient target, float delay)
        {
            if (target == null)
                return false;
            var targetStatus = target.GetPassiveStatus(delay);
            if (!targetStatus.HasPassive || targetStatus.PassiveType != FioraPassive.PassiveType.UltiPassive)
                return false;
            if (targetStatus.PassiveType != FioraPassive.PassiveType.UltiPassive)
                return false;
            if (!targetStatus.PassivePredictedPositions.Any())
                return false;
            var passiveposes = targetStatus.PassivePredictedPositions;
            var passivepos = passiveposes.OrderBy(x => Player.Position.ToVector2().Distance(x)).First();
            var poses = FioraPassive.GetRadiusPoints(targetStatus.TargetPredictedPosition, passivepos);
            var pos = poses.Where(x => x.Distance(Player.Position.ToVector2()) <= 400 && !x.IsWall())
                           .OrderBy(x => x.Distance(passivepos)).FirstOrDefault();
            if (pos == null || !pos.IsValid())
                return false;
            Fiora.Q.Cast(pos);
            return true;
        }
        #endregion CastQHelper



        #region Item
        public static bool HasItem()
        {
            if (Player.CanUseItem((int)ItemId.Youmuus_Ghostblade) || Player.CanUseItem((int)ItemId.Galeforce) || Player.CanUseItem((int)ItemId.Stridebreaker) || Player.CanUseItem((int)ItemId.Titanic_Hydra) || Player.CanUseItem((int)ItemId.Goredrinker))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static AIHeroClient Player = ObjectManager.Player;
        public static void CastItem()
        {
            if (Player.CanUseItem((int)ItemId.Youmuus_Ghostblade))
                Player.UseItem((int)ItemId.Youmuus_Ghostblade);
            if (Player.CanUseItem((int)ItemId.Galeforce))
                Player.UseItem((int)ItemId.Galeforce, Game.CursorPos);
            if (Player.CanUseItem((int)ItemId.Titanic_Hydra))
                Player.UseItem((int)ItemId.Titanic_Hydra);
            if (Player.CanUseItem((int)ItemId.Stridebreaker))
                Player.UseItem((int)ItemId.Stridebreaker, Game.CursorPos);
            if (Player.CanUseItem((int)ItemId.Goredrinker))
                Player.UseItem((int)ItemId.Goredrinker, Game.CursorPos);
        }
        #endregion Item
    }
    public static class MathAndExtensions
    {
        #region Math And Extensions
        public static int CountMinionsInRange(this Vector3 Position, float Range, bool JungleTrueEnemyFalse)
        {
            if (JungleTrueEnemyFalse)
            {
                var minion = GameObjects.GetMinions(Range, MinionTypes.All, MinionTeam.Enemy).Count;
                var jung = GameObjects.Jungle.Where(x => x.IsValidTarget(Range)).ToList<AIBaseClient>().Count;
                return minion + jung;
            } else
            {
                return
                GameObjects.GetMinions(Range, MinionTypes.All, MinionTeam.Enemy).Count;
            }
            
        }
        public static float AngleToRadian(this int Angle)
        {
            return Angle * (float)Math.PI / 180f;
        }
        public static bool InTheCone(this Vector2 pos, Vector2 centerconePolar, Vector2 centerconeEnd, double coneAngle)
        {
            return AngleBetween(pos, centerconePolar, centerconeEnd) < coneAngle / 2;
        }
        public static double AngleBetween(Vector2 a, Vector2 center, Vector2 c)
        {
            float a1 = c.Distance(center);
            float b1 = a.Distance(c);
            float c1 = center.Distance(a);
            if (a1 == 0 || c1 == 0) { return 0; }
            else
            {
                return Math.Acos((a1 * a1 + c1 * c1 - b1 * b1) / (2 * a1 * c1)) * (180 / Math.PI);
            }
        }
        public static Vector2 RotateAround(this Vector2 pointToRotate, Vector2 centerPoint, float angleInRadians)
        {
            double cosTheta = Math.Cos(angleInRadians);
            double sinTheta = Math.Sin(angleInRadians);
            return new Vector2
            {
                X =
                    (float)
                    (cosTheta * (pointToRotate.X - centerPoint.X) -
                    sinTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.X),
                Y =
                    (float)
                    (sinTheta * (pointToRotate.X - centerPoint.X) +
                    cosTheta * (pointToRotate.Y - centerPoint.Y) + centerPoint.Y)
            };
        }
        public static double AngleBetween(Vector2 a, Vector2 b)
        {
            var Theta1 = Math.Atan2(a.Y, a.X);
            var Theta2 = Math.Atan2(b.Y, b.X);
            var Theta = Math.Abs(Theta1 - Theta2);
            return
                Theta > 180 ? 360 - Theta : Theta;
        }
        #endregion  Math And Extensions
    }

    class HINH1
    {
        #region HINH1
        private enum DrawType
        {
            Circle = 1,
            HINH1 = 2
        }
        private static int drawtick = 0;
        private static int drawstate = 0;
        private static void DrawDraw(Vector3 center, float radius, System.Drawing.Color color, DrawType DrawedType, int width = 5)
        {
            switch (DrawedType)
            {
                case DrawType.Circle:
                    DrawCircle(center, radius, color, width);
                    break;
                case DrawType.HINH1:
                    DrawHinh1(center, radius, color, width);
                    break;
            }
        }
        private static void DrawHinh1(Vector3 center, float radius, System.Drawing.Color color, int width = 5)
        {
            Render.Circle.DrawCircle(center, radius, color, width, false);
            return;
            var pos1y = center;
            pos1y.X = pos1y.X + radius;
            var pos1 = pos1y.ToVector2().RotateAround(center.ToVector2(), drawstate.AngleToRadian());
            var pos1a = center.Extend(pos1.ToVector3(), radius * 5 / 8).ToVector2().RotateAround(center.ToVector2(), (18).AngleToRadian());
            var pos2 = pos1.RotateAround(center.ToVector2(), (36).AngleToRadian());
            var pos3 = pos1.RotateAround(center.ToVector2(), (72).AngleToRadian());
            var pos4 = pos1.RotateAround(center.ToVector2(), (108).AngleToRadian());
            var pos5 = pos1.RotateAround(center.ToVector2(), (144).AngleToRadian());
            var pos6 = pos1.RotateAround(center.ToVector2(), (180).AngleToRadian());
            var pos7 = pos1.RotateAround(center.ToVector2(), (216).AngleToRadian());
            var pos8 = pos1.RotateAround(center.ToVector2(), (252).AngleToRadian());
            var pos9 = pos1.RotateAround(center.ToVector2(), (288).AngleToRadian());
            var pos10 = pos1.RotateAround(center.ToVector2(), (324).AngleToRadian());
            var pos2a = pos1a.RotateAround(center.ToVector2(), (36).AngleToRadian());
            var pos3a = pos1a.RotateAround(center.ToVector2(), (72).AngleToRadian());
            var pos4a = pos1a.RotateAround(center.ToVector2(), (108).AngleToRadian());
            var pos5a = pos1a.RotateAround(center.ToVector2(), (144).AngleToRadian());
            var pos6a = pos1a.RotateAround(center.ToVector2(), (180).AngleToRadian());
            var pos7a = pos1a.RotateAround(center.ToVector2(), (216).AngleToRadian());
            var pos8a = pos1a.RotateAround(center.ToVector2(), (252).AngleToRadian());
            var pos9a = pos1a.RotateAround(center.ToVector2(), (288).AngleToRadian());
            var pos10a = pos1a.RotateAround(center.ToVector2(), (324).AngleToRadian());
            Drawing.DrawLine(Drawing.WorldToScreen(pos1.ToVector3()), Drawing.WorldToScreen(pos1a.ToVector3()), width, color);
            Drawing.DrawLine(Drawing.WorldToScreen(pos2.ToVector3()), Drawing.WorldToScreen(pos1a.ToVector3()), width, color);
            Drawing.DrawLine(Drawing.WorldToScreen(pos2.ToVector3()), Drawing.WorldToScreen(pos2a.ToVector3()), width, color);
            Drawing.DrawLine(Drawing.WorldToScreen(pos3.ToVector3()), Drawing.WorldToScreen(pos2a.ToVector3()), width, color);
            Drawing.DrawLine(Drawing.WorldToScreen(pos3.ToVector3()), Drawing.WorldToScreen(pos3a.ToVector3()), width, color);
            Drawing.DrawLine(Drawing.WorldToScreen(pos4.ToVector3()), Drawing.WorldToScreen(pos3a.ToVector3()), width, color);
            Drawing.DrawLine(Drawing.WorldToScreen(pos4.ToVector3()), Drawing.WorldToScreen(pos4a.ToVector3()), width, color);
            Drawing.DrawLine(Drawing.WorldToScreen(pos5.ToVector3()), Drawing.WorldToScreen(pos4a.ToVector3()), width, color);
            Drawing.DrawLine(Drawing.WorldToScreen(pos5.ToVector3()), Drawing.WorldToScreen(pos5a.ToVector3()), width, color);
            Drawing.DrawLine(Drawing.WorldToScreen(pos6.ToVector3()), Drawing.WorldToScreen(pos5a.ToVector3()), width, color);
            Drawing.DrawLine(Drawing.WorldToScreen(pos6.ToVector3()), Drawing.WorldToScreen(pos6a.ToVector3()), width, color);
            Drawing.DrawLine(Drawing.WorldToScreen(pos7.ToVector3()), Drawing.WorldToScreen(pos6a.ToVector3()), width, color);
            Drawing.DrawLine(Drawing.WorldToScreen(pos7.ToVector3()), Drawing.WorldToScreen(pos7a.ToVector3()), width, color);
            Drawing.DrawLine(Drawing.WorldToScreen(pos8.ToVector3()), Drawing.WorldToScreen(pos7a.ToVector3()), width, color);
            Drawing.DrawLine(Drawing.WorldToScreen(pos8.ToVector3()), Drawing.WorldToScreen(pos8a.ToVector3()), width, color);
            Drawing.DrawLine(Drawing.WorldToScreen(pos9.ToVector3()), Drawing.WorldToScreen(pos8a.ToVector3()), width, color);
            Drawing.DrawLine(Drawing.WorldToScreen(pos9.ToVector3()), Drawing.WorldToScreen(pos9a.ToVector3()), width, color);
            Drawing.DrawLine(Drawing.WorldToScreen(pos10.ToVector3()), Drawing.WorldToScreen(pos9a.ToVector3()), width, color);
            Drawing.DrawLine(Drawing.WorldToScreen(pos10.ToVector3()), Drawing.WorldToScreen(pos10a.ToVector3()), width, color);
            Drawing.DrawLine(Drawing.WorldToScreen(pos1.ToVector3()), Drawing.WorldToScreen(pos10a.ToVector3()), width, color);

            Drawing.DrawLine(Drawing.WorldToScreen(pos1.ToVector3()), Drawing.WorldToScreen(pos2.ToVector3()), width, color);
            Drawing.DrawLine(Drawing.WorldToScreen(pos3.ToVector3()), Drawing.WorldToScreen(pos2.ToVector3()), width, color);
            Drawing.DrawLine(Drawing.WorldToScreen(pos3.ToVector3()), Drawing.WorldToScreen(pos4.ToVector3()), width, color);
            Drawing.DrawLine(Drawing.WorldToScreen(pos5.ToVector3()), Drawing.WorldToScreen(pos4.ToVector3()), width, color);
            Drawing.DrawLine(Drawing.WorldToScreen(pos5.ToVector3()), Drawing.WorldToScreen(pos6.ToVector3()), width, color);
            Drawing.DrawLine(Drawing.WorldToScreen(pos7.ToVector3()), Drawing.WorldToScreen(pos6.ToVector3()), width, color);
            Drawing.DrawLine(Drawing.WorldToScreen(pos7.ToVector3()), Drawing.WorldToScreen(pos8.ToVector3()), width, color);
            Drawing.DrawLine(Drawing.WorldToScreen(pos9.ToVector3()), Drawing.WorldToScreen(pos8.ToVector3()), width, color);
            Drawing.DrawLine(Drawing.WorldToScreen(pos9.ToVector3()), Drawing.WorldToScreen(pos10.ToVector3()), width, color);
            Drawing.DrawLine(Drawing.WorldToScreen(pos1.ToVector3()), Drawing.WorldToScreen(pos10.ToVector3()), width, color);

            Drawing.DrawLine(Drawing.WorldToScreen(pos1a.ToVector3()), Drawing.WorldToScreen(pos2a.ToVector3()), width, color);
            Drawing.DrawLine(Drawing.WorldToScreen(pos3a.ToVector3()), Drawing.WorldToScreen(pos2a.ToVector3()), width, color);
            Drawing.DrawLine(Drawing.WorldToScreen(pos3a.ToVector3()), Drawing.WorldToScreen(pos4a.ToVector3()), width, color);
            Drawing.DrawLine(Drawing.WorldToScreen(pos5a.ToVector3()), Drawing.WorldToScreen(pos4a.ToVector3()), width, color);
            Drawing.DrawLine(Drawing.WorldToScreen(pos5a.ToVector3()), Drawing.WorldToScreen(pos6a.ToVector3()), width, color);
            Drawing.DrawLine(Drawing.WorldToScreen(pos7a.ToVector3()), Drawing.WorldToScreen(pos6a.ToVector3()), width, color);
            Drawing.DrawLine(Drawing.WorldToScreen(pos7a.ToVector3()), Drawing.WorldToScreen(pos8a.ToVector3()), width, color);
            Drawing.DrawLine(Drawing.WorldToScreen(pos9a.ToVector3()), Drawing.WorldToScreen(pos8a.ToVector3()), width, color);
            Drawing.DrawLine(Drawing.WorldToScreen(pos9a.ToVector3()), Drawing.WorldToScreen(pos10a.ToVector3()), width, color);
            Drawing.DrawLine(Drawing.WorldToScreen(pos1a.ToVector3()), Drawing.WorldToScreen(pos10a.ToVector3()), width, color);

            DrawCircle(center, radius * 2 / 8, color, width, 10);

            if (Variables.GameTimeTickCount >= drawtick + 10)
            {
                drawtick = Variables.GameTimeTickCount;
                drawstate += 2;
            }


        }

        private static void DrawHinh2(Vector3 center, float radius, System.Drawing.Color color, int width = 5)
        {
            var n = 100 - (drawstate % 102);
            DrawCircle(center, radius * n / 100, System.Drawing.Color.Yellow, width * 3, 10);
            DrawCircle(center, radius * (n + 20 > 100 ? n - 80 : n + 20) / 100, System.Drawing.Color.LightGreen);
            DrawCircle(center, radius * (n + 40 > 100 ? n - 60 : n + 40) / 100, System.Drawing.Color.Orange);
            DrawCircle(center, radius * (n + 60 > 100 ? n - 40 : n + 60) / 100, System.Drawing.Color.LightPink);
            DrawCircle(center, radius * (n + 80 > 100 ? n - 20 : n + 80) / 100, System.Drawing.Color.PaleVioletRed);

            if (Variables.GameTimeTickCount >= drawtick + 10)
            {
                drawtick = Variables.GameTimeTickCount;
                drawstate += 2;
            }
        }

        public static void DrawCircle(Vector3 center,
            float radius,
             System.Drawing.Color color,
            int thickness = 5,
            int quality = 60)
        {
            Render.Circle.DrawCircle(center, radius, color, thickness, false);

            //var pointList = new List<Vector3>();
            //for (var i = 0; i < quality; i++)
            //{
            //    var angle = i * Math.PI * 2 / quality;
            //    pointList.Add(
            //        new Vector3(
            //            center.X + radius * (float)Math.Cos(angle), center.Y + radius * (float)Math.Sin(angle),
            //            center.Z));
            //}

            //for (var i = 0; i < pointList.Count; i++)
            //{
            //    var a = pointList[i];
            //    var b = pointList[i == pointList.Count - 1 ? 0 : i + 1];

            //    var aonScreen = Drawing.WorldToScreen(a);
            //    var bonScreen = Drawing.WorldToScreen(b);

            //    Drawing.DrawLine(aonScreen.X, aonScreen.Y, bonScreen.X, bonScreen.Y, thickness, color);
            //}
        }

        #endregion HINH1
    }
}
