using System.Drawing;

namespace DaoHungAIO.Evade
{
    #region

    using EnsoulSharp;
    using EnsoulSharp.SDK;
    using EnsoulSharp.SDK.MenuUI;
    
    using SharpDX;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using DominationAIO.Common;

    #endregion

    internal class EvadeTargetManager
    {

        public static Menu Menu, AttackMenu, SpellMenu, WhiteList;
        public static readonly List<SpellData> Spells = new List<SpellData>();
        private static readonly List<Targets> DetectedTargets = new List<Targets>();
        private static IOrderedEnumerable<AIHeroClient> bestAllies;

        public static void Attach(Menu mainMenu)
        {
            Menu = new Menu("EvadeTargetMenu", "格挡指向性技能");
            {
                Menu.Add(new MenuBool("Brian.EvadeTargetMenu.EvadeTargetW", "启用"));
                Menu.Add(new MenuBool("Brian.EvadeTargetMenu.CC", "队友被控自动套盾"));
            }

            WhiteList = new Menu("whitelist", "套盾白名单");
            {
                foreach (var target in GameObjects.AllyHeroes)
                {
                    WhiteList.Add(new MenuBool(target.CharacterName.ToLower(), "启用于: " + target.CharacterName));
                }
            }
            Menu.Add(WhiteList);

            InitSpells();
            AttackMenu = new Menu("Brian.EvadeTargetMenu.DodgeAttackMenu", "套盾挡平A");
            {
                AttackMenu.Add(new MenuBool("Brian.EvadeTargetMenu.BAttack", "普攻"));
                AttackMenu.Add(new MenuSlider("Brian.EvadeTargetMenu.BAttackHpU", "^- 若血量 <=", 80, 1, 100));
                AttackMenu.Add(new MenuBool("Brian.EvadeTargetMenu.CAttack", "暴击"));
                AttackMenu.Add(new MenuSlider("Brian.EvadeTargetMenu.CAttackHpU", "^- 若血量 <=", 80, 1, 100));
                AttackMenu.Add(new MenuBool("Brian.EvadeTargetMenu.Turret", "防御塔攻击"));
                AttackMenu.Add(new MenuBool("Brian.EvadeTargetMenu.Minion", "小兵攻击"));
                AttackMenu.Add(new MenuSlider("Brian.EvadeTargetMenu.HP", "^- 若血量 <=", 20, 1, 100));
            }
            Menu.Add(AttackMenu);

         

            mainMenu.Add(Menu);


          
           
            GameObject.OnCreate += OnCreate;
            GameObject.OnDelete += OnDestroy;
        }

        private static void InitSpells()
        {
            Spells.Add(
                new SpellData
                { ChampionName = "Ahri", SpellNames = new[] { "ahrifoxfiremissiletwo" }, Slot = SpellSlot.W });
            Spells.Add(
                new SpellData
                { ChampionName = "Ahri", SpellNames = new[] { "ahritumblemissile" }, Slot = SpellSlot.R });
            Spells.Add(
                new SpellData { ChampionName = "Akali", SpellNames = new[] { "akalimota" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData { ChampionName = "Anivia", SpellNames = new[] { "frostbite" }, Slot = SpellSlot.E });
            Spells.Add(
                new SpellData { ChampionName = "Annie", SpellNames = new[] { "disintegrate" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Caitlyn",
                    SpellNames = new[] { "caitlynaceintheholemissile" },
                    Slot = SpellSlot.R
                });
            Spells.Add(
                new SpellData
                { ChampionName = "Cassiopeia", SpellNames = new[] { "cassiopeiae" }, Slot = SpellSlot.E });
            Spells.Add(
                new SpellData { ChampionName = "Elise", SpellNames = new[] { "elisehumanq" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "" +
                                   "Ezreal" +
                                   "",
                    SpellNames = new[] { "ezrealarcaneshiftmissile" },
                    Slot = SpellSlot.E
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "FiddleSticks",
                    SpellNames = new[] { "fiddlesticksdarkwind", "fiddlesticksdarkwindmissile" },
                    Slot = SpellSlot.E
                });
            Spells.Add(
                new SpellData { ChampionName = "Gangplank", SpellNames = new[] { "parley" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData { ChampionName = "Janna", SpellNames = new[] { "sowthewind" }, Slot = SpellSlot.W });
            Spells.Add(
                new SpellData { ChampionName = "Kassadin", SpellNames = new[] { "nulllance" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Katarina",
                    SpellNames = new[] { "katarinaq", "katarinaqmis" },
                    Slot = SpellSlot.Q
                });
            Spells.Add(
                new SpellData
                { ChampionName = "Kayle", SpellNames = new[] { "judicatorreckoning" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Leblanc",
                    SpellNames = new[] { "leblancchaosorb", "leblancchaosorbm" },
                    Slot = SpellSlot.Q
                });
            Spells.Add(new SpellData { ChampionName = "Lulu", SpellNames = new[] { "luluw" }, Slot = SpellSlot.W });
            Spells.Add(
                new SpellData
                { ChampionName = "Malphite", SpellNames = new[] { "seismicshard" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "MissFortune",
                    SpellNames = new[] { "missfortunericochetshot", "missFortunershotextra" },
                    Slot = SpellSlot.Q
                });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Nami",
                    SpellNames = new[] { "namiwenemy", "namiwmissileenemy" },
                    Slot = SpellSlot.W
                });
            Spells.Add(
                new SpellData { ChampionName = "Nunu", SpellNames = new[] { "iceblast" }, Slot = SpellSlot.E });
            Spells.Add(
                new SpellData { ChampionName = "Pantheon", SpellNames = new[] { "pantheonq" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Ryze",
                    SpellNames = new[] { "spellflux", "spellfluxmissile" },
                    Slot = SpellSlot.E
                });
            Spells.Add(
                new SpellData { ChampionName = "Shaco", SpellNames = new[] { "twoshivpoison" }, Slot = SpellSlot.E });
            Spells.Add(
                new SpellData { ChampionName = "Shen", SpellNames = new[] { "shenvorpalstar" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData { ChampionName = "Sona", SpellNames = new[] { "sonaqmissile" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData { ChampionName = "Swain", SpellNames = new[] { "swaintorment" }, Slot = SpellSlot.E });
            Spells.Add(
                new SpellData { ChampionName = "Syndra", SpellNames = new[] { "syndrar" }, Slot = SpellSlot.R });
            Spells.Add(
                new SpellData { ChampionName = "Taric", SpellNames = new[] { "dazzle" }, Slot = SpellSlot.E });
            Spells.Add(
                new SpellData { ChampionName = "Teemo", SpellNames = new[] { "blindingdart" }, Slot = SpellSlot.Q });
            Spells.Add(
                new SpellData
                { ChampionName = "Tristana", SpellNames = new[] { "detonatingshot" }, Slot = SpellSlot.E });
            Spells.Add(
                new SpellData
                { ChampionName = "TwistedFate", SpellNames = new[] { "bluecardattack" }, Slot = SpellSlot.W });
            Spells.Add(
                new SpellData
                { ChampionName = "TwistedFate", SpellNames = new[] { "goldcardattack" }, Slot = SpellSlot.W });
            Spells.Add(
                new SpellData
                { ChampionName = "TwistedFate", SpellNames = new[] { "redcardattack" }, Slot = SpellSlot.W });
            Spells.Add(
                new SpellData
                {
                    ChampionName = "Urgot",
                    SpellNames = new[] { "urgotheatseekinghomemissile" },
                    Slot = SpellSlot.Q
                });
            Spells.Add(
                new SpellData { ChampionName = "Vayne", SpellNames = new[] { "vaynecondemn" }, Slot = SpellSlot.E });
            Spells.Add(
                new SpellData
                { ChampionName = "Veigar", SpellNames = new[] { "veigarprimordialburst" }, Slot = SpellSlot.R });
            Spells.Add(
                new SpellData
                { ChampionName = "Viktor", SpellNames = new[] { "viktorpowertransfer" }, Slot = SpellSlot.Q });
        }


        private static void OnCreate(GameObject sender,EventArgs args)
        {
                var missile = sender as MissileClient;
                if (missile == null)
                {
                    return;
                }
                if (!missile.SpellCaster.IsValid ||
                    missile.SpellCaster.Team == ObjectManager.Player.Team)
                {
                    return;
                }
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


                var hero = missile.SpellCaster as AIHeroClient;
                if (hero == null)
                {
                    return;
                }
                var aaa = GameObjects.AllyHeroes
                        .Where(t =>
                            t.Distance(missile.Target.Position) < 100).FirstOrDefault();
                var spellData =
                    Spells.FirstOrDefault(
                        i =>
                            i.SpellNames.Contains(missile.SData.Name.ToLower()));

                if (spellData == null)
                {
                    return;
                }



                foreach (var ally in bestAllies)
                {
                    if (aaa == ally)
                    {
                        if (ObjectManager.Player.CharacterName == "Janna" ||
                            ObjectManager.Player.CharacterName == "Rakan" ||
                            ObjectManager.Player.CharacterName == "Lulu" || ObjectManager.Player.CharacterName == "Ivern" ||
                            ObjectManager.Player.CharacterName == "Karma")
                        {
                            if (DaoHungAIO.Evade.EvadeTargetManager.Menu["whitelist"][
                                        ally.CharacterName.ToLower()] != null &&
                                aaa.Distance(ObjectManager.Player) < DominationAIO.Common.Champion.E.Range)
                            {
                                Champion.E.CastOnUnit((aaa));
                            }
                        }
                        if (ObjectManager.Player.CharacterName == "Lux" || ObjectManager.Player.CharacterName == "Sona" ||
                            ObjectManager.Player.CharacterName == "Taric")

                        {
                            if (DaoHungAIO.Evade.EvadeTargetManager.Menu["whitelist"][ally.CharacterName.ToLower()] != null &&
                                aaa.Distance(ObjectManager.Player) < Champion.W.Range)
                            {
                                Champion.W.CastOnUnit(aaa);
                            }
                        }
                    }


                }
            
        }
    

        private static void OnDestroy(GameObject sender,EventArgs args)
        {
            if (!sender.IsValid)
            {
                return;
            }
            var missile = sender as MissileClient;
            if (missile == null)
            {
                return;
            }          

            if (missile.SpellCaster.IsValid && missile.SpellCaster.Team != ObjectManager.Player.Team)
            {
                DetectedTargets.RemoveAll(i => i.Obj.NetworkId == missile.NetworkId);
            }
        }

        public class SpellData
        {
            public string ChampionName;
            public SpellSlot Slot;
            public string[] SpellNames = { };

            public string MissileName => SpellNames.First();
        }

        private class Targets
        {
            public MissileClient Obj;
            public Vector3 Start;
        }
    }
}
