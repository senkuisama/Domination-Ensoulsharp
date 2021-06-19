using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominationAIO.NewPlugins
{
    public static class MyBaseUlt
    {
        private static List<BaseChampions> GetChampions = new List<BaseChampions>();
        private static List<AIBaseClient> BaseUltChampions = new List<AIBaseClient>();
        private static MenuKeyBind key = new MenuKeyBind("Disable BS", "Disable Base Ult", Keys.Space, KeyBindType.Press);
        private static MenuSlider MaxDmg = new MenuSlider("MaxDmg", "Max Dmg = %", 80, 60, 100);
        public static void LoadBaseUlt()
        {
            var menu = new Menu("BaseUltLoad", "Loaded Base Ult", true);
            menu.Add(key);
            menu.Add(new MenuSeparator("Draven", "Draven"));
            menu.Add(new MenuSeparator("Ezreal", "Ezreal"));
            menu.Add(new MenuSeparator("Ashe", "Ashe"));
            menu.Add(new MenuSeparator("Jinx", "Jinx"));
            menu.Add(new MenuSeparator("Senna", "Senna"));

            menu.Add(MaxDmg);
            menu.Attach();
            var Base = GameObjects.EnemySpawnPoints.FirstOrDefault();
            if (GameObjects.EnemyHeroes != null)
            {
                foreach (var target in GameObjects.EnemyHeroes)
                {
                    var get = new BaseChampions(target, Base.Position, Teleport.TeleportStatus.Unknown, Teleport.TeleportType.Unknown, 0, 0);
                    GetChampions.Add(get);
                }
            }
            Teleport.OnTeleport += Teleport_OnTeleport;
            Game.OnUpdate += Game_OnUpdate;
            //Game.OnUpdate += Game_OnUpdate1;
            Drawing.OnDraw += Drawing_OnDraw;
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if(BaseUltChampions != null)
            {
                var i = 1;
                foreach(var target in BaseUltChampions)
                {
                    if (i >= 5)
                        return;

                    var name = target.CharacterName;
                    var pos = ObjectManager.Player.HPBarPosition;
                    Drawing.DrawText(pos.X, pos.Y + 20 * i, System.Drawing.Color.Red, name + " Can Base Ult");
                    i++;
                }
            }
        }

        private static void Game_OnUpdate1(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
                return;
            var BaseSpell = new Spell(SpellSlot.R);
            if(ObjectManager.Player.CharacterName == "Draven" && BaseSpell.Name != "DravenRCast")
            {
                if(BaseSpell.IsReady() && BaseSpell.Level >= 1)
                {
                    var missisle = ObjectManager.Get<MissileClient>().Where(i => i.Name == DravenRMissisle && i.IsValid).FirstOrDefault();
                    if(missisle != null && missisle.Type == GameObjectType.MissileClient)
                    {
                        if (missisle.Position.Distance(GameObjects.EnemySpawnPoints.FirstOrDefault().Position) <= 400)
                        {
                            if (BaseSpell.Cast())
                                return;
                        }
                    }
                }
            }
        }
        private static string DravenRMissisle = "DravenR";
        private static int Delay = 0;
        private static int Speed = 0;
        private static void Game_OnUpdate(EventArgs args)
        {
            switch (ObjectManager.Player.CharacterName)
            {
                case "Ezreal":
                    Delay = 1000;
                    Speed = 2000;
                    break;
                case "Draven":
                    Delay = 500;
                    Speed = 2000;
                    break;
                case "Ashe":
                    Delay = 250;
                    Speed = 1600;
                    break;
                case "Senna":
                    Delay = 1000;
                    Speed = 20000;
                    break;
            }

            Delay += Game.Ping / 2;

            if (key.Active)
                return;

            if (ObjectManager.Player.IsDead)
                return;
            var R = new Spell(SpellSlot.R);
            if (GameObjects.EnemyHeroes != null && R.IsReady())
            {
                foreach(var target in GameObjects.EnemyHeroes)
                {
                    var getinfo = GetChampions.FirstOrDefault(i => i.NetWorkID == target.NetworkId);

                    if(getinfo.Type == Teleport.TeleportType.Recall && getinfo.Status == Teleport.TeleportStatus.Start)
                    {                        
                        if(ObjectManager.Player.CharacterName == "Jinx")
                        {
                            var delayshort = 2000 / 1500 * 1000;
                            var maxspeed = 2200;
                            var delay = 500;

                            if(target.Health <= R.GetDamage(target))
                            {
                                if(!BaseUltChampions.Contains(target))
                                    BaseUltChampions.Add(target);
                                if(ObjectManager.Player.Distance(getinfo.PosBaseUlt) > 2000)
                                {
                                    delay = 500 + delayshort;
                                    if((getinfo.PosBaseUlt.DistanceToPlayer() - 2000) / maxspeed * 1000 + delay >= getinfo.Duration - (Variables.GameTimeTickCount - getinfo.Start))
                                    {
                                        if ((getinfo.PosBaseUlt.DistanceToPlayer() - 2000) / maxspeed * 1000 + delay <= getinfo.Duration - (Variables.GameTimeTickCount - getinfo.Start) + 750)
                                        {
                                            if (R.Cast(getinfo.PosBaseUlt))
                                                return;
                                        }                                        
                                    }
                                }
                                else
                                {
                                    if (getinfo.PosBaseUlt.DistanceToPlayer() / 1500 * 1000 + Delay >= getinfo.Duration - (Variables.GameTimeTickCount - getinfo.Start))
                                    {
                                        if (getinfo.PosBaseUlt.DistanceToPlayer() / 1500 * 1000 + Delay <= 750 + getinfo.Duration - (Variables.GameTimeTickCount - getinfo.Start))
                                        {
                                            if (R.Cast(getinfo.PosBaseUlt))
                                                return;
                                        }                                        
                                    }
                                }
                            }
                        }
                        if (ObjectManager.Player.CharacterName != "Draven" || R.Name == "DravenRCast")
                        {
                            if (target.Health <= GetRDmg(target))
                            {
                                if (!BaseUltChampions.Contains(target))
                                    BaseUltChampions.Add(target);
                                if (getinfo.PosBaseUlt.DistanceToPlayer() / Speed * 1000 + Delay >= getinfo.Duration - (Variables.GameTimeTickCount - getinfo.Start))
                                {
                                    if (getinfo.PosBaseUlt.DistanceToPlayer() / Speed * 1000 + Delay <= 750 + getinfo.Duration - (Variables.GameTimeTickCount - getinfo.Start))
                                    {
                                        if (R.Cast(getinfo.PosBaseUlt))
                                            return;
                                    }
                                }
                            }

                        }                      
                    }
                    else
                    {
                        if (BaseUltChampions.Contains(target))
                            BaseUltChampions.Remove(target);
                        continue;
                    }
                }
            }
            else
            {
                BaseUltChampions.Clear();
            }
        }

        private static double GetRDmg(this AIHeroClient target)
        {
            var R = new Spell(SpellSlot.R);

            switch (ObjectManager.Player.CharacterName)
            {
                case "Ashe":
                    return ObjectManager.Player.GetAsheRDmg(target);
                case "Ezreal":
                    return ObjectManager.Player.GetEzrealRDmg(target);
                case "Draven":
                    return ObjectManager.Player.GetDravenRDmg(target);
                case "Jinx":
                    return ObjectManager.Player.GetJinxRDmg(target);
                case "Senna":
                    return ObjectManager.Player.GetSennaRDmg(target);
                default:
                    return R.GetDamage(target);
            }          
        }

        private static double GetAsheRDmg(this AIHeroClient sources, AIBaseClient target)
        {
            var RBaseDamage = new double[] { 0f, 200, 400, 600 };
            var rLevel = (new Spell(SpellSlot.R)).Level;

            var rBaseDamage = RBaseDamage[rLevel] +
                              ObjectManager.Player.TotalMagicalDamage;

            return ObjectManager.Player.CalculateDamage(target, DamageType.Magical, rBaseDamage);
        }
        private static double GetEzrealRDmg(this AIHeroClient sources, AIBaseClient target)
        {
            var RBaseDamage = new double[] { 0f, 350f, 500f, 650f };
            var rLevel = (new Spell(SpellSlot.R)).Level;

            var rBaseDamage = RBaseDamage[rLevel] +
                              ObjectManager.Player.GetBonusPhysicalDamage() +
                              0.9f * ObjectManager.Player.TotalMagicalDamage;

            return ObjectManager.Player.CalculateDamage(target, DamageType.Magical, rBaseDamage);
        }
        private static double GetDravenRDmg(this AIHeroClient sources, AIBaseClient target)
        {
            var Dmgs = new double[]
            {
                175,
                275,
                375
            };
            var BonusDmgs = new double[]
            {
                1.1,
                1.3,
                1.5
            };

            var level = (new Spell(SpellSlot.R)).Level;
            if (level == 0)
                return 0;

            var bonusad = sources.GetBonusPhysicalDamage();
            var alldmg = sources.CalculatePhysicalDamage(target, Dmgs[level - 1] + BonusDmgs[level - 1] * bonusad);

            return alldmg;
        }

        private static double GetJinxRDmg(this AIHeroClient sources, AIBaseClient target)
        {
            var level = (new Spell(SpellSlot.R)).Level;
            if (level == 0)
                return 0;

            double dmg = 0;
            var list1 = new double[]
            {
                0, 250, 400, 550,
            };

            var list2 = new double[]
            {
                0, 0.25, 0.3, 0.35
            };

            dmg += 1.5 * ObjectManager.Player.GetBonusPhysicalDamage();

            dmg += list1[level];

            dmg += list2[level] * target.MaxHealth - target.Health;

            var alldmg = sources.CalculatePhysicalDamage(target, dmg);

            return alldmg;
        }

        private static double GetSennaRDmg(this AIHeroClient sources, AIBaseClient target)
        {
            var level = (new Spell(SpellSlot.R)).Level;
            if (level == 0)
                return 0;

            double dmg = 0;
            var list = new double[]
            {
                0, 250 , 375 , 500,
            };

            dmg += list[level];
            dmg += ObjectManager.Player.GetBonusPhysicalDamage();
            dmg += 0.5 * ObjectManager.Player.TotalMagicalDamage;

            var alldmg = sources.CalculatePhysicalDamage(target, dmg);

            return alldmg;
        }
        private static void Teleport_OnTeleport(AIBaseClient sender, Teleport.TeleportEventArgs args)
        {
            var Champion = GetChampions.FirstOrDefault(i => i.NetWorkID == sender.NetworkId);

            if (Champion != null)
            {
                Champion.Status = args.Status;
                Champion.Type = args.Type;
                Champion.Duration = args.Duration;
                //Champion.Start = args.Start;

                if (args.Status == Teleport.TeleportStatus.Start)
                {
                    Champion.Start = Variables.GameTimeTickCount;
                }
                else
                {
                    Champion.Start = 0;
                }
            }
            else
            {
                if(sender is AIHeroClient)
                {
                    var Base = GameObjects.EnemySpawnPoints.FirstOrDefault();
                    var get = new BaseChampions((AIHeroClient)sender, Base.Position, args.Status, args.Type, args.Duration, args.Start);
                    GetChampions.Add(get);
                }               
            }
        }

        private class BaseChampions
        {
            public bool IsRecall = false;
            public bool Teleporting = false;

            public int NetWorkID { get; set; }            
            public Vector3 PosBaseUlt { get; set; }
            public Teleport.TeleportStatus Status { get; set; }
            public Teleport.TeleportType Type { get; set; }
            public int Duration { get; set; }
            public int Start { get; set; }

            public BaseChampions(AIHeroClient target, Vector3 pos, Teleport.TeleportStatus sts, Teleport.TeleportType type, int duration, int start)
            {
                NetWorkID = target.NetworkId;
                PosBaseUlt = pos;
                Status = sts;
                Type = type;
                Duration = duration;
                Start = start;
            }
        }
    }
}
