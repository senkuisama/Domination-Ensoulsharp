using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.MenuUI.Values;
using EnsoulSharp.SDK.Prediction;
using FunnySlayerCommon;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Template
{
    internal class Program
    {
        /*private static void Main(string[] args)
        {
            GameEvent.OnGameLoad += OnGameLoad;
        }*/
        private static void OnGameLoad()
        {
            if (ObjectManager.Player.CharacterName != "Irelia")
                return;

            loaded.OnLoad();
        }
    }
    internal class MenuSettings
    {
        public class QSettings
        {
            public static MenuBool Qcombo = new MenuBool("Qcombo", "Q in Combo [Gap_closer | KillSteal]");
            public static MenuList QListComboMode = new MenuList("QListComboMode", "Q List Combo Mode", new string[] {" Gapcloser Logic ", " Dancing Logic ", " High Logic ","Extream Logic"}, 1);
            public static MenuBool QStacks = new MenuBool("QStacks", "Use Q Stack Passive Logic");
            public static MenuBool QDancing = new MenuBool("QDancing", "----> Q Dancing logic");
            public static MenuSlider QHeath = new MenuSlider("Q heath", "When Health Percent <= ", 85);

            public static MenuBool CheckQDmgITems = new MenuBool("CheckItemDmg", "Q Dmg Check Items", false);
        }
        public class WSettings
        {
            public static MenuBool Wcombo = new MenuBool("Wcombo", "W in Combo", false);
            public static MenuSliderButton Wdelay = new MenuSliderButton("WDelay", "Recast W", 0, 0, 1000);
        }

        public class ESettings
        {
            public static MenuBool Ecombo = new MenuBool("Ecombo", "E in Combo");
            public static MenuBool ImproveE = new MenuBool("ImproveE", "Improve E prediction", false);
            public static MenuSlider ImproveEDelay = new MenuSlider("ImproveE Delay", "Delay E prediction (Default 600)", 1150, 400, 1400);
            public static MenuSeparator Efeedback = new MenuSeparator("Efeedback", "E logic if not good Feedback it to FunnySlayer#0348");
        }

        public class RSettings
        {
            public static MenuBool Rcombo = new MenuBool("Rcombo", "R in Combo [Calculator Dmg]");
            public static MenuSlider Rheath = new MenuSlider("Rheath", "Target Heath", 60);
            public static MenuSlider Rhit = new MenuSlider("Rhit", "Hit Count", 2, 2, 5);
        }

        public class ClearSettings
        {
            public static MenuBool DrawMinions = new MenuBool("DrawMinions", "Draw On Minions");
            public static MenuBool QireClear = new MenuBool("QireClear", "Q Clear");
            public static MenuSlider QireMana = new MenuSlider("QireMana", "Min Mana to use : ", 40);
        }
        public class KeysSettings
        {
            public static MenuKeyBind TurretKey = new MenuKeyBind("TurretKey", "Turret Key", System.Windows.Forms.Keys.A, KeyBindType.Toggle);
            public static MenuKeyBind SemiE = new MenuKeyBind("SemiE", "E Using Key", System.Windows.Forms.Keys.G, KeyBindType.Press);
            public static MenuKeyBind SemiR = new MenuKeyBind("SemiR", "R Using Key", System.Windows.Forms.Keys.T, KeyBindType.Press);
            public static MenuKeyBind FleeKey = new MenuKeyBind("FleeKey", "Flee Key", System.Windows.Forms.Keys.Z, KeyBindType.Press);

            public static MenuKeyBind AutoClearMinions = new MenuKeyBind("Auto Clear Minions", "Clear Minions AUTO", System.Windows.Forms.Keys.N, KeyBindType.Toggle);
        }
    }
    public class loaded
    {
        private static AIHeroClient objPlayer = ObjectManager.Player;
        private static Menu myMenu;
        public static Spell Q;
        public static Spell W;
        public static Spell E, E1, E2;
        public static Spell R, R2;

        public static void OnLoad()
        {
            Q = new Spell(SpellSlot.Q, 600f);
            Q.SetTargetted(0f, Qspeed());           

            W = new Spell(SpellSlot.W, 825f);
            W.SetCharged("IreliaW", "ireliawdefense", 800, 800, 0);

            E = new Spell(SpellSlot.E, 775f);
            E.SetSkillshot(0.25f, 5f, 1800f, false, false, SkillshotType.Line);

            E1 = new Spell(SpellSlot.Unknown, 775f);
            E1.SetSkillshot(0.15f + (0.25f * 2), 5f, 1800f, false, false, SkillshotType.Circle);

            E2 = new Spell(SpellSlot.Unknown, 775f);
            E2.SetSkillshot(0.25f, 5f, 1800f, false, false, SkillshotType.Line);

            R = new Spell(SpellSlot.R, 1000);
            R.SetSkillshot(2f, 100, 1000f, true, SkillshotType.Line);
            

            R2 = new Spell(SpellSlot.Unknown, 1000);
            R.SetSkillshot(0.25f, 300, 1500, false, SkillshotType.Line);

            myMenu = new Menu(objPlayer.CharacterName, "Irelia The Flash", true);
            Menu FStarget = new Menu("FS Target", "Target Selector");
            FStarget.AddTargetSelectorMenu();
            myMenu.Add(FStarget);

            Menu Qmenu = new Menu("Qmenu", "Q Settings")
            {
                MenuSettings.QSettings.Qcombo,
                MenuSettings.QSettings.QListComboMode,
                MenuSettings.QSettings.QStacks,
                MenuSettings.QSettings.QDancing,
                MenuSettings.QSettings.QHeath,
                MenuSettings.QSettings.CheckQDmgITems,
            };

            Menu Wmenu = new Menu("Wmenu", "W Settings")
            {
                MenuSettings.WSettings.Wcombo,
                MenuSettings.WSettings.Wdelay,
            };

            Menu Emenu = new Menu("Emenu", "E Settings")
            {
                MenuSettings.ESettings.Ecombo,
                MenuSettings.ESettings.ImproveE,
                MenuSettings.ESettings.ImproveEDelay,
                MenuSettings.ESettings.Efeedback,
            };

            Menu Rmenu = new Menu("Rmenu", "R Settings")
            {
                MenuSettings.RSettings.Rcombo,
                MenuSettings.RSettings.Rheath,
                MenuSettings.RSettings.Rhit,
            };

            Menu ClearMenu = new Menu("ClearMenu", "Clear Settings")
            {
                MenuSettings.ClearSettings.DrawMinions,
                MenuSettings.ClearSettings.QireClear,
                MenuSettings.ClearSettings.QireMana,
            };

            Menu Keys = new Menu("Keys", "Keys");
            Keys.Add(MenuSettings.KeysSettings.TurretKey).Permashow();
            Keys.Add(MenuSettings.KeysSettings.FleeKey).Permashow();
            Keys.Add(MenuSettings.KeysSettings.SemiE).Permashow();
            Keys.Add(MenuSettings.KeysSettings.SemiR).Permashow();
            Keys.Add(MenuSettings.KeysSettings.AutoClearMinions).Permashow();

            myMenu.Add(Qmenu);
            myMenu.Add(Wmenu);
            myMenu.Add(Emenu);
            myMenu.Add(Rmenu);
            myMenu.Add(ClearMenu);
            myMenu.Add(Keys);

            myMenu.Attach();
                       
            AIHeroClient.OnBuffLose += AIHeroClient_OnBuffLose;
            Game.OnUpdate += IRELIA_KS;
            Game.OnUpdate += IRELIA_RCOMBO;
            AIBaseClient.OnProcessSpellCast += AIBaseClient_OnProcessSpellCast;          
            Game.OnUpdate += IRELIA_QCOMBO;
            Game.OnUpdate += IRELIA_ECOMBO;
            Game.OnUpdate += OnUpdate;
            Drawing.OnDraw += Drawing_OnDraw;                   
            Game.OnUpdate += IRELIA_LANECLEAR;                                           
            Drawing.OnDraw += DRAW_RPOS;
        }

        private static void IRELIA_QCOMBO(EventArgs args)
        {
            if (objPlayer.IsDead)
                return;

            if (Orbwalker.ActiveMode != OrbwalkerMode.Combo)
                return;

            if (!Q.IsReady() || !MenuSettings.QSettings.Qcombo.Enabled)
                return;

            var targets = ObjectManager.Get<AIHeroClient>().Where(i => i!= null && !i.IsDead && !i.IsAlly && i.IsValidTarget(3000)).OrderBy(i => i.Distance(ObjectManager.Player));
            if (targets == null)
                return;

            GapCloserTargetCanKillable();

            var target = FSTargetSelector.GetFSTarget(3000);
            if (target == null)
                return;
           
            var Heallist = new List<float>
            {
                0f, 12f , 14f, 16f, 18f, 20f
            };

            var Heal = Heallist[Q.Level] * 0.2f * objPlayer.TotalAttackDamage;

            if(MenuSettings.QSettings.QListComboMode.Index == 0)
            {
                GapcloserLogic(target);
            }
            if (MenuSettings.QSettings.QListComboMode.Index == 1)
            {
                DancingLogic(target);
            }
            if (MenuSettings.QSettings.QListComboMode.Index == 2)
            {
                HighLogic(target);
            }
            if (MenuSettings.QSettings.QListComboMode.Index == 3)
            {
                ExtreamLogic(target);
            }            
        }
        private static void GapcloserLogic(AIBaseClient target)
        {
            if (target == null)
                return;

            var targets = ObjectManager.Get<AIHeroClient>().Where(i => i != null && !i.IsDead && !i.IsAlly && i.IsValidTarget(3000)).OrderBy(i => i.Distance(ObjectManager.Player));
            if (targets == null)
                return;

            QGapCloserPos(target.Position);
        }
        private static void DancingLogic(AIBaseClient target)
        {
            if (target == null)
                return;

            var targets = ObjectManager.Get<AIHeroClient>().Where(i => i != null && !i.IsDead && !i.IsAlly && i.IsValidTarget(3000)).OrderBy(i => i.Distance(ObjectManager.Player)).ThenBy(i => i.Health);
            if (targets == null)
                return;

            if (CanQ(target))
            {
                if (objPlayer.HealthPercent <= MenuSettings.QSettings.QHeath.Value && MenuSettings.QSettings.QDancing.Enabled)
                {
                    var MaxObjs = ObjectManager.Get<AIBaseClient>().Where(i => !i.IsAlly && CanQ(i, MenuSettings.QSettings.CheckQDmgITems.Enabled) && Q.CanCast(i)).OrderByDescending(i => i.Distance(target));
                    var MinObjs = ObjectManager.Get<AIBaseClient>().Where(i => !i.IsAlly && CanQ(i, MenuSettings.QSettings.CheckQDmgITems.Enabled) && Q.CanCast(i)).OrderBy(i => i.Distance(target));
                    if (MaxObjs != null && MinObjs != null && MaxObjs.Count() >= 2 && MinObjs.Count() >= 2)
                    {
                        AIBaseClient Gap = null;
                        AIBaseClient Flee = null;


                        foreach (var max in MaxObjs)
                        {
                            foreach (var min in MinObjs.Where(i => i.IsValidTarget(Q.Range)
                                    && !i.IsDead
                                    && !i.IsAlly
                                    && CanQ(i)
                                    && (
                                    (objPlayer.HealthPercent <= MenuSettings.QSettings.QHeath.Value && MenuSettings.QSettings.QDancing.Enabled) ?
                                    i.Position.Distance(target) <= objPlayer.Distance(target) + objPlayer.GetRealAutoAttackRange()
                                    : i.Position.Distance(target) <= objPlayer.Distance(target)
                                    || i.Position.Distance(target) <= objPlayer.GetRealAutoAttackRange() + 50)))
                            {
                                if (Gap != null && Flee != null && Vector3.Distance(Gap.Position, Flee.Position) <= Q.Range)
                                {
                                    if (Gap.NetworkId != Flee.NetworkId)
                                    {
                                        break;
                                    }
                                }

                                if (Gap != null)
                                {
                                    if (max.Distance(Gap) <= Q.Range)
                                    {
                                        Flee = max;
                                        break;
                                    }
                                }
                                else
                                {
                                    Flee = max;
                                }

                                if (Flee != null)
                                {
                                    if (min.Distance(Flee) <= Q.Range)
                                    {
                                        Gap = min;
                                        break;
                                    }
                                }
                                else
                                {
                                    Gap = min;
                                    continue;
                                }
                            }

                            if (Gap != null && Flee != null && Vector3.Distance(Gap.Position, Flee.Position) <= Q.Range)
                            {
                                if (Gap.NetworkId != Flee.NetworkId)
                                {
                                    break;
                                }
                            }
                        }

                        if (Flee != null && Gap != null && Gap.NetworkId != Flee.NetworkId && Vector3.Distance(Gap.Position, Flee.Position) <= Q.Range)
                        {
                            if (Flee.IsValidTarget(Q.Range))
                            {
                                if (!UnderTower(Flee.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                                {
                                    if (Q.Cast(Flee) == CastStates.SuccessfullyCasted)
                                    {
                                        if (Q.Cast(Gap) == CastStates.SuccessfullyCasted)
                                        {
                                            return;
                                        }
                                        else
                                        {
                                            QGapCloserPos(target.Position);
                                        }
                                    }
                                }
                                else
                                {
                                    QGapCloserPos(target.Position);
                                }
                            }
                            else
                            {
                                QGapCloserPos(target.Position);
                            }
                        }
                        else
                        {
                            QGapCloserPos(target.Position);
                        }
                    }
                    else
                    {
                        QGapCloserPos(target.Position);
                    }
                }
                else
                {
                    QGapCloserPos(target.Position);
                }
            }
            else
            {
                foreach (var t in targets.OrderBy(i => i.Health))
                {
                    if (CanQ(t))
                    {
                        if (objPlayer.HealthPercent <= MenuSettings.QSettings.QHeath.Value && MenuSettings.QSettings.QDancing.Enabled)
                        {
                            var MaxObjs = ObjectManager.Get<AIBaseClient>().Where(i => !i.IsAlly && CanQ(i, MenuSettings.QSettings.CheckQDmgITems.Enabled) && Q.CanCast(i)).OrderByDescending(i => i.Distance(t));
                            var MinObjs = ObjectManager.Get<AIBaseClient>().Where(i => !i.IsAlly && CanQ(i, MenuSettings.QSettings.CheckQDmgITems.Enabled) && Q.CanCast(i)).OrderBy(i => i.Distance(t));
                            if (MaxObjs != null && MinObjs != null && MaxObjs.Count() >= 2 && MinObjs.Count() >= 2)
                            {
                                AIBaseClient Gap = null;
                                AIBaseClient Flee = null;


                                foreach (var max in MaxObjs)
                                {
                                    foreach (var min in MinObjs.Where(i => i.IsValidTarget(Q.Range)
                                            && !i.IsDead
                                            && !i.IsAlly
                                            && CanQ(i)
                                            && (
                                            (objPlayer.HealthPercent <= MenuSettings.QSettings.QHeath.Value && MenuSettings.QSettings.QDancing.Enabled) ?
                                            i.Position.Distance(t) <= objPlayer.Distance(t) + objPlayer.GetRealAutoAttackRange()
                                            : i.Position.Distance(t) <= objPlayer.Distance(t)
                                            || i.Position.Distance(t) <= objPlayer.GetRealAutoAttackRange() + 50)))
                                    {
                                        if (Gap != null && Flee != null && Vector3.Distance(Gap.Position, Flee.Position) <= Q.Range)
                                        {
                                            if (Gap.NetworkId != Flee.NetworkId)
                                            {
                                                break;
                                            }
                                        }

                                        if (Gap != null)
                                        {
                                            if (max.Distance(Gap) <= Q.Range)
                                            {
                                                Flee = max;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            Flee = max;
                                        }

                                        if (Flee != null)
                                        {
                                            if (min.Distance(Flee) <= Q.Range)
                                            {
                                                Gap = min;
                                                break;
                                            }
                                        }
                                        else
                                        {
                                            Gap = min;
                                            continue;
                                        }
                                    }

                                    if (Gap != null && Flee != null && Vector3.Distance(Gap.Position, Flee.Position) <= Q.Range)
                                    {
                                        if (Gap.NetworkId != Flee.NetworkId)
                                        {
                                            break;
                                        }
                                    }
                                }

                                if (Flee != null && Gap != null && Gap.NetworkId != Flee.NetworkId && Vector3.Distance(Gap.Position, Flee.Position) <= Q.Range)
                                {
                                    if (Flee.IsValidTarget(Q.Range))
                                    {
                                        if (!UnderTower(Flee.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                                        {
                                            if (Q.Cast(Flee) == CastStates.SuccessfullyCasted)
                                            {
                                                if (Q.Cast(Gap) == CastStates.SuccessfullyCasted)
                                                {
                                                    return;
                                                }
                                                else
                                                {
                                                    QGapCloserPos(target.Position);
                                                    continue;
                                                }
                                            }
                                        }
                                        else
                                        {
                                            QGapCloserPos(target.Position);
                                            continue;
                                        }                                                
                                    }
                                    else
                                    {
                                        QGapCloserPos(target.Position);
                                        continue;
                                    }
                                }
                                else
                                {
                                    QGapCloserPos(target.Position);
                                    continue;
                                }

                            }
                            else
                            {
                                QGapCloserPos(target.Position);
                                continue;
                            }
                        }
                        else
                        {
                            QGapCloserPos(target.Position);
                            continue;
                        }
                    }
                    else
                    {
                        QGapCloserPos(target.Position);
                        continue;
                    }
                }
            }
        }

        private static void HighLogic(AIBaseClient target)
        {
            if (target == null)
                return;

            var targets = ObjectManager.Get<AIHeroClient>().Where(i => i != null && !i.IsDead && !i.IsAlly && i.IsValidTarget(3000)).OrderBy(i => i.Distance(ObjectManager.Player)).ThenBy(i => i.Health);
            if (targets == null)
                return;

            if (CanQ(target))
            {
                if (objPlayer.HealthPercent <= MenuSettings.QSettings.QHeath.Value && MenuSettings.QSettings.QDancing.Enabled)
                {
                    var objs = ObjectManager.Get<AIBaseClient>().Where(i => i != null 
                    && !i.IsDead 
                    && i.IsEnemy 
                    && !i.IsAlly
                    && CanQ(i)
                    && i.MaxHealth > 10
                    && i.Distance(target) < Q.Range
                    && (i.Type == GameObjectType.AIMinionClient || i.Type == GameObjectType.AIHeroClient))
                        .OrderByDescending(i => i.Health).ThenByDescending(i => i.Distance(target));
                    
                    if (objs != null)
                    {
                        foreach(var obj in objs)
                        {
                            if(obj != null)
                            {
                                if (obj.IsValidTarget(Q.Range))
                                {
                                    if (obj.Distance(target) < Q.Range)
                                    {
                                        if (!UnderTower(obj.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                                        {
                                            if (Q.Cast(obj) == CastStates.SuccessfullyCasted)
                                                return;
                                        }
                                        else
                                        {
                                            QGapCloserPos(target.Position);
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        QGapCloserPos(target.Position);
                                        continue;
                                    }
                                }
                                else
                                {
                                    QGapCloserPos(target.Position);
                                    continue;
                                }   
                            }
                            else
                            {
                                QGapCloserPos(target.Position);
                                continue;
                            }
                        }
                    }
                    else
                    {
                        QGapCloserPos(target.Position);                        
                    }
                }
                else
                {
                    QGapCloserPos(target.Position);
                }
            }
            else
            {
                foreach (var t in targets.OrderBy(i => i.Health))
                {
                    if (CanQ(t))
                    {
                        if (objPlayer.HealthPercent <= MenuSettings.QSettings.QHeath.Value && MenuSettings.QSettings.QDancing.Enabled)
                        {
                            var objs = ObjectManager.Get<AIBaseClient>().Where(i => i != null
                            && !i.IsDead
                            && i.IsEnemy
                            && !i.IsAlly
                            && CanQ(i)
                            && i.MaxHealth > 10
                            && (i.Type == GameObjectType.AIMinionClient || i.Type == GameObjectType.AIHeroClient))
                                .OrderByDescending(i => i.Health).ThenByDescending(i => i.Distance(t));


                            if (objs != null)
                            {
                                foreach (var obj in objs)
                                {
                                    if (obj != null)
                                    {
                                        if (obj.IsValidTarget(Q.Range))
                                        {
                                            if (obj.Distance(t) < Q.Range)
                                            {
                                                if (!UnderTower(obj.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                                                {
                                                    if (Q.Cast(obj) == CastStates.SuccessfullyCasted)
                                                        return;
                                                }
                                                else
                                                {
                                                    QGapCloserPos(target.Position);
                                                    continue;
                                                }
                                            }
                                            else
                                            {
                                                QGapCloserPos(target.Position);
                                                continue;
                                            }
                                        }
                                        else
                                        {
                                            QGapCloserPos(target.Position);
                                            continue;
                                        }                                            
                                    }
                                    else
                                    {
                                        QGapCloserPos(target.Position);
                                        continue;
                                    }
                                }
                            }
                            else
                            {
                                QGapCloserPos(target.Position);
                                continue;
                            }
                        }
                        else
                        {
                            QGapCloserPos(target.Position);
                            continue;
                        }
                    }
                    else
                    {
                        var newobjs = ObjectManager.Get<AIBaseClient>().Where(i => i != null
                            && !i.IsDead
                            && i.IsEnemy
                            && !i.IsAlly
                            && CanQ(i)
                            && i.MaxHealth > 10
                            && i.Distance(target) < objPlayer.GetRealAutoAttackRange()
                            && (i.Type == GameObjectType.AIMinionClient || i.Type == GameObjectType.AIHeroClient))
                                .OrderByDescending(i => i.Health).ThenByDescending(i => i.Distance(target));

                        var objs = ObjectManager.Get<AIBaseClient>().Where(i => i != null
                            && !i.IsDead
                            && i.IsEnemy
                            && !i.IsAlly
                            && CanQ(i)
                            && i.MaxHealth > 10
                            && (i.Type == GameObjectType.AIMinionClient || i.Type == GameObjectType.AIHeroClient))
                                .OrderByDescending(i => i.Health).ThenByDescending(i => i.Distance(t));

                        if (newobjs != null && objs != null)
                        {
                            foreach (var obj in objs)
                            {
                                if (obj != null)
                                {
                                    if (obj.IsValidTarget(Q.Range))
                                    {
                                        if (obj.Distance(t) < Q.Range)
                                        {
                                            if (!UnderTower(obj.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                                            {
                                                if (objPlayer.Spellbook.CastSpell(SpellSlot.Q, obj) || Q.Cast(obj) == CastStates.SuccessfullyCasted)
                                                    return;
                                            }
                                            else
                                            {
                                                QGapCloserPos(target.Position);
                                                continue;
                                            }
                                        }
                                        else
                                        {
                                            QGapCloserPos(target.Position);
                                            continue;
                                        }
                                    }
                                    else
                                    {
                                        QGapCloserPos(target.Position);
                                        continue;
                                    }   
                                }
                                else
                                {
                                    QGapCloserPos(target.Position);
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            QGapCloserPos(target.Position);
                            continue;
                        }
                    }
                }
            }
        }

        private static void ExtreamLogic(AIBaseClient target)
        {
            if (target == null)
                return;

            var targets = ObjectManager.Get<AIHeroClient>().Where(i => i != null && !i.IsDead && !i.IsAlly && i.IsValidTarget(3000)).OrderBy(i => i.Distance(ObjectManager.Player)).ThenBy(i => i.Health);
            if (targets == null)
                return;

            if (CanQ(target))
            {
                if (objPlayer.HealthPercent <= MenuSettings.QSettings.QHeath.Value && MenuSettings.QSettings.QDancing.Enabled)
                {
                    var objs = ObjectManager.Get<AIBaseClient>().Where(i => i != null
                    && !i.IsDead
                    && i.IsEnemy
                    && !i.IsAlly
                    && i.Distance(target) < objPlayer.GetRealAutoAttackRange() + 50
                    && CanQ(i)
                    && i.MaxHealth > 10
                    && (i.Type == GameObjectType.AIMinionClient || i.Type == GameObjectType.AIHeroClient))
                        .OrderByDescending(i => i.Health).ThenByDescending(i => i.Distance(target));

                    if (objs != null)
                    {
                        if(objs.ToList().Count > 1 || objs.Count() > 1)
                        {
                            foreach(var obj in objs.ThenByDescending(i => i.Distance(target)))
                            {
                                if(obj != null)
                                {
                                    var tempobjs = ObjectManager.Get<AIBaseClient>().Where(i => i != null
                                    && !i.IsDead
                                    && i.IsEnemy
                                    && !i.IsAlly
                                    && i.Distance(obj) < Q.Range
                                    && CanQ(i)
                                    && i.MaxHealth > 10
                                    && (i.Type == GameObjectType.AIMinionClient || i.Type == GameObjectType.AIHeroClient))
                                        .OrderByDescending(i => i.Health).ThenByDescending(i => i.Distance(obj));

                                    if(tempobjs != null)
                                    {
                                        foreach(var tempobj in tempobjs)
                                        {
                                            if(tempobj != null)
                                            {
                                                if (tempobj.IsValidTarget(Q.Range))
                                                {
                                                    if (!UnderTower(tempobj.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                                                    {
                                                        if (Q.Cast(tempobj) == CastStates.SuccessfullyCasted)
                                                        {
                                                            EnsoulSharp.SDK.Utility.DelayAction.Add(100, () =>
                                                            {
                                                                if (Q.Cast(obj) == CastStates.SuccessfullyCasted)
                                                                    return;
                                                            });
                                                        }
                                                        else
                                                        {
                                                            HighLogic(target);
                                                            continue;
                                                        }
                                                    }
                                                    else
                                                    {
                                                        HighLogic(target);
                                                        continue;
                                                    }
                                                }
                                                else
                                                {
                                                    HighLogic(target);
                                                    continue;
                                                }                                              
                                            }
                                            else
                                            {
                                                HighLogic(target);
                                                continue;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        HighLogic(target);
                                        continue;
                                    }
                                }
                                else
                                {
                                    HighLogic(target);
                                    continue;
                                }
                            }
                        }
                        else
                        {
                            if(objs.Count() == 1 || objs.ToList().Count == 1)
                            {
                                HighLogic(target);
                            }
                            else
                            {
                                QGapCloserPos(target.Position);
                            }
                        }                       
                    }
                    else
                    {
                        HighLogic(target);
                    }
                }
                else
                {
                    HighLogic(target);
                }
            }
            else
            {
                foreach (var t in targets.OrderBy(i => i.Health))
                {
                    if (CanQ(t))
                    {
                        if (objPlayer.HealthPercent <= MenuSettings.QSettings.QHeath.Value && MenuSettings.QSettings.QDancing.Enabled)
                        {
                            var objs = ObjectManager.Get<AIBaseClient>().Where(i => i != null
                            && !i.IsDead
                            && i.IsEnemy
                            && !i.IsAlly
                            && i.Distance(t) < objPlayer.GetRealAutoAttackRange() + 50
                            && CanQ(i)
                            && i.MaxHealth > 10
                            && (i.Type == GameObjectType.AIMinionClient || i.Type == GameObjectType.AIHeroClient))
                                .OrderByDescending(i => i.Health).ThenByDescending(i => i.Distance(t));

                            if (objs != null)
                            {
                                if (objs.ToList().Count > 1 || objs.Count() > 1)
                                {
                                    foreach (var obj in objs.ThenByDescending(i => i.Distance(t)))
                                    {
                                        if (obj != null)
                                        {
                                            var tempobjs = ObjectManager.Get<AIBaseClient>().Where(i => i != null
                                            && !i.IsDead
                                            && i.IsEnemy
                                            && !i.IsAlly
                                            && i.Distance(obj) < Q.Range
                                            && CanQ(i)
                                            && i.MaxHealth > 10
                                            && (i.Type == GameObjectType.AIMinionClient || i.Type == GameObjectType.AIHeroClient))
                                                .OrderByDescending(i => i.Health).ThenByDescending(i => i.Distance(obj));

                                            if (tempobjs != null)
                                            {
                                                foreach (var tempobj in tempobjs)
                                                {
                                                    if (tempobj != null)
                                                    {
                                                        if (tempobj.IsValidTarget(Q.Range))
                                                        {
                                                            if (!UnderTower(tempobj.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                                                            {
                                                                if (Q.Cast(tempobj) == CastStates.SuccessfullyCasted)
                                                                {
                                                                    EnsoulSharp.SDK.Utility.DelayAction.Add(300, () =>
                                                                    {
                                                                        if (Q.Cast(obj) == CastStates.SuccessfullyCasted)
                                                                            return;
                                                                    });
                                                                }
                                                                else
                                                                {
                                                                    HighLogic(target);
                                                                    continue;
                                                                }
                                                            }
                                                            else
                                                            {
                                                                HighLogic(target);
                                                            }
                                                        }
                                                        else
                                                        {
                                                            HighLogic(target);
                                                            continue;
                                                        }  
                                                    }
                                                    else
                                                    {
                                                        HighLogic(target);
                                                        continue;
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                HighLogic(target);
                                                continue;
                                            }
                                        }
                                        else
                                        {
                                            HighLogic(target);
                                            continue;
                                        }
                                    }
                                }
                                else
                                {
                                    if (objs.Count() == 1 || objs.ToList().Count == 1)
                                    {
                                        HighLogic(t);
                                    }
                                    else
                                    {
                                        QGapCloserPos(target.Position);
                                        continue;
                                    }
                                }
                            }
                            else
                            {
                                HighLogic(target);
                                continue;
                            }
                        }
                        else
                        {
                            HighLogic(target);
                            continue;
                        }
                    }
                    else
                    {
                        HighLogic(target);
                        continue;
                    }
                }
            }
        }

        private static void DRAW_RPOS(EventArgs args)
        {
            if (objPlayer.IsDead)
                return;

            if (GetRPos1 != null)
            {
                GetRPos1.Draw(System.Drawing.Color.Red, 1);
            }
            if (GetRPos2 != null)
            {
                GetRPos2.Draw(System.Drawing.Color.Red, 1);
            }
        }

        public static Geometry.Rectangle GetRPos1;
        public static Geometry.Rectangle GetRPos2;
        private static void IRELIA_RCOMBO(EventArgs args)
        {
            if (objPlayer.IsDead)
            {
                GetRPos1 = null;
                GetRPos2 = null;
                return;
            }

            if (Orbwalker.ActiveMode != OrbwalkerMode.Combo)
            {
                GetRPos1 = null;
                GetRPos2 = null;
                return;
            }

            if (!R.IsReady() || !MenuSettings.RSettings.Rcombo.Enabled)
            {
                GetRPos1 = null;
                GetRPos2 = null;
                return;
            }

            var target = FSTargetSelector.GetFSTarget(1000);
            if (target == null || target.HasBuff("ireliamark"))
                return;
            var pred = FSpred.Prediction.Prediction.PredictUnitPosition(target, 600);
            if (pred != null && pred.IsValid())
            {
                if (target.HealthPercent <= MenuSettings.RSettings.Rheath.Value && pred.DistanceToPlayer() < R.Range && target.IsValidTarget(R.Range))
                {
                    if (R.Cast(pred))
                        return;
                }
                GetRPos1 = new Geometry.Rectangle(objPlayer.Position, pred.ToVector3(), 110);
                GetRPos2 = new Geometry.Rectangle(pred, pred.Extend(objPlayer.Position, -450), 300);

                var TargetCount1 = GameObjects.EnemyHeroes.Where(i => GetRPos1.IsInside(i.Position)).Count();
                var TargetCount2 = GameObjects.EnemyHeroes.Where(i => GetRPos2.IsInside(i.Position) && !GameObjects.EnemyHeroes.Where(a => GetRPos1.IsInside(a.Position)).Any(h => i.NetworkId == h.NetworkId)).Count();

                if (TargetCount1 >= MenuSettings.RSettings.Rhit.Value
                    || TargetCount2 >= MenuSettings.RSettings.Rhit.Value
                    || TargetCount1 + TargetCount2 >= MenuSettings.RSettings.Rhit.Value
                    )
                {
                    if(pred.DistanceToPlayer() <= 1000)
                        if (R.Cast(pred))
                            return;
                }
            }
            else
            {
                GetRPos1 = null;
                GetRPos2 = null;
            }
        }

        private static void IRELIA_ECOMBO(EventArgs args)
        {
            if (objPlayer.IsDead)
                return;

            if (Orbwalker.ActiveMode != OrbwalkerMode.Combo)
                return;

            if (!E.IsReady())
                return;

            if (MenuSettings.ESettings.Ecombo.Enabled)
            {
                NewEPred();
            }
        }

        private static void IRELIA_KS(EventArgs args)
        {
            if (objPlayer.IsDead)
                return;

            if (!Q.IsReady())
                return;

            var minions = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(600)).Where(i => i.Health <= GetQDmg(i)).OrderBy(i => i.DistanceToPlayer());

            if (minions == null)
                return;

            foreach (var min in minions)
            {
                if (!UnderTower(min.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                {
                    if (Q.CastOnUnit(min))
                        return;
                }
            }
        }

        private static void IRELIA_LANECLEAR(EventArgs args)
        {
            if (objPlayer.IsDead)
                return;

            if (!Q.IsReady())
                return;

            if (!MenuSettings.ClearSettings.QireClear.Enabled) return;
            if (objPlayer.ManaPercent <= MenuSettings.ClearSettings.QireMana.Value) return;

            if (Orbwalker.ActiveMode != OrbwalkerMode.LaneClear && Orbwalker.ActiveMode != OrbwalkerMode.LastHit && !MenuSettings.KeysSettings.AutoClearMinions.Active)
                return;

            var minions = ObjectManager.Get<AIMinionClient>().Where(i => i != null && !i.IsDead && !i.IsAlly && i.IsValidTarget(600)).OrderBy(i => i.Health);

            if (minions == null)
                return;

            foreach (var min in minions)
            {
                if (CanQ(min))
                {
                    if (!UnderTower(min.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                    {
                        if (Q.Cast(min) == CastStates.SuccessfullyCasted || Q.CastOnUnit(min))
                            return;
                    }
                }
            }
        }

        public static float SheenTimer = 0;
        private static void AIHeroClient_OnBuffLose(AIBaseClient sender, AIBaseClientBuffLoseEventArgs args)
        {
            if (sender.MemoryAddress == objPlayer.MemoryAddress || sender.NetworkId == objPlayer.NetworkId || sender.IsMe)
            {
                if (args.Buff.Name == "sheen"
                    || args.Buff.Name == "trinityforce"
                    || args.Buff.Name.Contains("sheen")
                    || args.Buff.Name.Contains("trinity")
                    || args.Buff.Name.Contains("iceborn")
                    || args.Buff.Name.Contains("Lich"))
                {
                    SheenTimer = Environment.TickCount;
                }
            }
        }

        private static void Drawing_OnDraw(EventArgs args)
        {
            if (!MenuSettings.ClearSettings.DrawMinions.Enabled) return;

            var minions = GameObjects.GetMinions(2000).Where(i => CanQ(i)).OrderByDescending(i => i.DistanceToPlayer()).ToList();

            if (minions == null) return;

            foreach (var min in minions)
            {
                Drawing.DrawCircle(min.Position, 70, System.Drawing.Color.White);
            }
        }

        public static Vector3 ECatPos;

        private static void AIBaseClient_OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (sender.IsMe && args.SData.Name == "IreliaEMissile")
            {
                ECatPos = args.End;
            }

            if(sender.IsMe && args.Slot != SpellSlot.Unknown)
            {
                if(args.Slot <= SpellSlot.R)
                {
                    //ObjectManager.Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                }
            }
        }
        private static void OnUpdate(EventArgs args)
        {
            Q.Speed = Qspeed();
            if (objPlayer.IsDead || objPlayer.IsRecalling())
                return;

            if (MenuSettings.KeysSettings.FleeKey.Active)
            {
                objPlayer.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);
                QGapCloserPos(Game.CursorPos);
            }

            if (MenuSettings.KeysSettings.SemiR.Active || MenuSettings.KeysSettings.SemiE.Active)
            {
                CastManual();
            }
        }

        private static void CastManual()
        {
            if (MenuSettings.KeysSettings.SemiE.Active && E.IsReady())
            {

                #region New E pred
                var targets = GameObjects.EnemyHeroes.Where(i => i.IsValidTarget(2000) && !i.IsDead);
                var target = TargetSelector.GetTarget(775);
                {
                    if (target == null) return;

                    float ereal = 0.3f + Game.Ping / 1000;

                    if (E.IsReady(0))
                    {
                        if (!objPlayer.HasBuff("IreliaE"))
                        {
                            if (E1.GetPrediction(target).CastPosition.DistanceToPlayer() < 775)
                            {
                                Geometry.Circle circle = new Geometry.Circle(objPlayer.Position, 600, 50);

                                {
                                    foreach (var onecircle in circle.Points)
                                    {
                                        if (onecircle.Distance(target) > 450)
                                        {
                                            E.Cast(onecircle);
                                        }
                                    }
                                }
                            }
                        }
                        if (E.IsReady() && E.Name != "IreliaE" && ECatPos.IsValid())
                        {
                            Vector2 vector2 = FSpred.Prediction.Prediction.PredictUnitPosition(target, 600);
                            if (vector2.Distance(ObjectManager.Player.Position) < E.Range)
                            {
                                Vector2 v3 = vector2;
                                int slider = 775;
                                for (int j = 50; j <= slider; j += 50)
                                {
                                    Vector2 vector3 = vector2.Extend(ECatPos.ToVector2(), (float)(-(float)j));
                                    if (vector3.Distance(ObjectManager.Player) >= E.Range)
                                    {
                                        break;
                                    }
                                    else
                                    {
                                        v3 = vector3;
                                        continue;
                                    }
                                }
                                if (E.Cast(v3.ToVector3()))
                                {
                                    return;
                                }
                            }
                        }
                    }
                }
                #endregion



            }
            if (MenuSettings.KeysSettings.SemiR.Active && R.IsReady())
            {
                #region R
                try
                {
                    var targets = TargetSelector.GetTargets(900);
                    Vector3 Rpos = Vector3.Zero;

                    if (!targets.Any()) return;
                    foreach (var Rprediction in targets.Select(i => R.GetPrediction(i)).Where(i => i.Hitchance >= HitChance.High || (i.Hitchance >= HitChance.Medium && i.AoeTargetsHitCount > 1)).OrderByDescending(i => i.AoeTargetsHitCount))
                    {
                        Rpos = Rprediction.CastPosition;
                    }
                    if (Rpos != Vector3.Zero)
                    {
                        R.Cast(Rpos);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine("R.cast Error" + ex);
                }
                #endregion
            }
        }
             
        public static bool UnderTower(Vector3 pos, float bonusrange = 0)
        {
            return
                ObjectManager.Get<AITurretClient>()
                    .Any(i => i.IsEnemy && !i.IsDead && (i.Distance(pos) < 850 + ObjectManager.Player.BoundingRadius + bonusrange));
        }
        public static void GapCloserTargetCanKillable()
        {
            var target = GameObjects.EnemyHeroes.Where(i => !i.IsDead && i.IsValidTarget(2000) && CanQ(i)).OrderBy(i => i.DistanceToPlayer()).FirstOrDefault();
            if (target == null || !Q.IsReady()) return;

            if (target.IsValidTarget(Q.Range))
            {
                if (!UnderTower(target.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                {
                    if (Q.Cast(target) == CastStates.SuccessfullyCasted || Q.CastOnUnit(target))
                        return;
                }
            }

            var gapobjs = ObjectManager.Get<AIBaseClient>().Where(i => !i.IsAlly && !i.IsDead && i.IsValidTarget(2000) && CanQ(i)).ToArray();
            if (gapobjs.Any())
            {
                foreach (var obj1 in gapobjs)
                {
                    if (obj1.Distance(target) < Q.Range && obj1.IsValidTarget(Q.Range))
                    {
                        if (!UnderTower(obj1.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                        {
                            if (Q.Cast(obj1) == CastStates.SuccessfullyCasted || Q.CastOnUnit(obj1))
                                return;
                        }
                    }

                    if (gapobjs.Count() >= 2)
                    {
                        foreach (var obj2 in gapobjs.Where(i => i.IsValidTarget(Q.Range)))
                        {
                            if (obj1.Distance(target) < Q.Range)
                            {
                                if (obj2.Distance(obj1) < Q.Range)
                                {
                                    if (!UnderTower(obj2.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                                    {
                                        if (Q.Cast(obj2) == CastStates.SuccessfullyCasted || Q.CastOnUnit(obj2))
                                            return;
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        public static void QGapCloserPos(Vector3 pos)
        {
            var obj = ObjectManager.Get<AIBaseClient>().Where(i => i.IsValidTarget(Q.Range)
            && !i.IsDead
            && !i.IsAlly
            && CanQ(i)
            && (
            (objPlayer.HealthPercent <= MenuSettings.QSettings.QHeath.Value && MenuSettings.QSettings.QDancing.Enabled) ?
            i.Position.Distance(pos) <= objPlayer.Distance(pos) + objPlayer.GetRealAutoAttackRange() 
            : i.Position.Distance(pos) <= objPlayer.Distance(pos)
            || i.Position.Distance(pos) <= objPlayer.GetRealAutoAttackRange() + 50)
            ).OrderBy(i => i.Distance(pos)).FirstOrDefault();

            if (Q.IsReady() && obj != null)
            {
                if (!UnderTower(obj.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                    if (Q.Cast(obj) == CastStates.SuccessfullyCasted || Q.CastOnUnit(obj))
                        return;
            }
        }
        public static bool CanQ(AIBaseClient target, bool CheckItems = true)
        {
            if (target.Health <= GetQDmg(target, CheckItems) || target.HasBuff("ireliamark"))
            {
                return true;
            }              
            else
                return false;
        }

        private static void NewEPred()
        {
            var target = FSTargetSelector.GetFSTarget(1000);
         
            {
                if (target != null && !target.HasBuff("ireliamark"))
                {
                    float ereal = 0.275f + Game.Ping / 1000;

                    if (E.IsReady(0))
                    {
                        if (MenuSettings.ESettings.ImproveE.Enabled)
                        {
                            if (E.Name != "IreliaE" && ECatPos.IsValid())
                            {
                                /*var Epred = SebbyLibPorted.Prediction.Prediction.GetPrediction(target, MenuSettings.ESettings.ImproveEDelay.Value / 1000);
                                if (Epred.CastPosition.IsValid() && Epred.CastPosition.DistanceToPlayer() <= E.Range)
                                {
                                    var RangeCheck = (objPlayer.CountEnemyHeroesInRange(1000) > 2 ? 1000 : 350);
                                    for (float i = RangeCheck; i > 50; i -= 20)
                                    {
                                        var CastPos = Epred.CastPosition.Extend(ECatPos, -i);
                                        if (CastPos.DistanceToPlayer() < E.Range)
                                        {
                                            if (E.Cast(CastPos) || E.Cast(CastPos, true))
                                                return;
                                        }
                                    }
                                }*/

                                var vector2 = FSpred.Prediction.Prediction.PredictUnitPosition(target, MenuSettings.ESettings.ImproveEDelay.Value);
                                if (E.Name != "IreliaE" && vector2.IsValid())
                                {
                                    var RangeCheck = (objPlayer.CountEnemyHeroesInRange(1000) > 2 ? 1000 : 300);
                                    for (float i = RangeCheck; i > 50; i -= 20)
                                    {
                                        var CastPos = vector2.Extend(ECatPos, -i);
                                        if(CastPos.DistanceToPlayer() < E.Range)
                                        {
                                            if (E.Cast(CastPos) || E.Cast(CastPos, true))
                                                return;
                                        }
                                        else
                                        {
                                            continue;
                                        }
                                    }
                                }
                                else
                                {
                                    return;
                                }
                            }
                            else
                            {
                                if (E1.GetPrediction(target).CastPosition.DistanceToPlayer() < 800)
                                {
                                    Geometry.Circle circle = new Geometry.Circle(objPlayer.Position, 600, 50);

                                    {
                                        foreach (var onecircle in circle.Points)
                                        {
                                            if (onecircle.Distance(target) > 600)
                                            {
                                                if (E.Cast(onecircle))
                                                {
                                                    var vector2 = FSpred.Prediction.Prediction.GetPrediction(target, 600);
                                                    var v3 = Vector2.Zero;
                                                    if (vector2.CastPosition.IsValid() && vector2.CastPosition.Distance(objPlayer.Position) < E.Range - 100)
                                                        for (int j = 50; j <= 900; j += 50)
                                                        {
                                                            var vector3 = vector2.CastPosition.Extend(ECatPos.ToVector2(), -j);
                                                            if (vector3.Distance(ObjectManager.Player) >= E.Range && v3 != Vector2.Zero)
                                                            {
                                                                if (E.Cast(v3) || E.Cast(v3))
                                                                {
                                                                    return;
                                                                }
                                                                break;
                                                            }
                                                            else
                                                            {
                                                                v3 = vector3.ToVector2();
                                                                continue;
                                                            }
                                                        }
                                                }
                                            }
                                        }
                                    }
                                }
                                if (objPlayer.IsDashing())
                                {
                                    if (objPlayer.GetDashInfo().EndPos.Distance(target) < 775)
                                    {
                                        Geometry.Circle circle = new Geometry.Circle(objPlayer.Position, 775, 50);

                                        {
                                            foreach (var onecircle in circle.Points.Where(i => i.Distance(target) > 775))
                                            {
                                                if (E.Cast(onecircle))
                                                {
                                                    var vector2 = FSpred.Prediction.Prediction.PredictUnitPosition(target, 600);
                                                    var v3 = vector2;
                                                    if (vector2.IsValid() && vector2.Distance(objPlayer.Position.ToVector2()) < E.Range - 100)
                                                        for (int j = 50; j <= 900; j += 50)
                                                        {
                                                            var vector3 = vector2.Extend(ECatPos.ToVector2(), -j);
                                                            if (vector3.Distance(ObjectManager.Player) >= E.Range)
                                                            {
                                                                if (E.Cast(v3.ToVector3()) || E.Cast(v3))
                                                                {
                                                                    return;
                                                                }
                                                                break;
                                                            }
                                                            else
                                                            {
                                                                v3 = vector3;
                                                                continue;
                                                            }
                                                        }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (objPlayer.HasBuff("IreliaE"))
                            {
                                var castepos = Vector2.Zero;
                                Geometry.Circle onecircle = new Geometry.Circle(objPlayer.Position, 725);
                                foreach (var circle in onecircle.Points)
                                {
                                    E.Delay = (725 - E2.GetPrediction(target).CastPosition.Distance(objPlayer)) / E.Speed + 0.25f + ereal;
                                }

                                var epred = E.GetPrediction(target);

                                for (int i = 10000; i > 55; i -= 50)
                                {
                                    if (epred.CastPosition.Extend(ECatPos, -i).ToVector2().DistanceToPlayer() <= 775)
                                        castepos = epred.CastPosition.Extend(ECatPos, -i).ToVector2();

                                    if (castepos != Vector2.Zero)
                                    {
                                        if (E.Cast(castepos))
                                            return;
                                    }
                                }
                            }
                        }

                        if (E.IsReady() && E.Name == "IreliaE")
                        {
                            if (E1.GetPrediction(target).CastPosition.DistanceToPlayer() < 825)
                            {
                                Geometry.Circle circle = new Geometry.Circle(objPlayer.Position, 600, 50);

                                {
                                    foreach (var onecircle in circle.Points)
                                    {
                                        if (onecircle.Distance(target) > 600)
                                        {
                                            if (E.Cast(onecircle))
                                            {
                                                var vector2 = FSpred.Prediction.Prediction.GetPrediction(target, 600);
                                                var v3 = Vector2.Zero;
                                                if (vector2.CastPosition.IsValid() && vector2.CastPosition.Distance(objPlayer.Position) < E.Range - 100)
                                                    for (int j = 50; j <= 900; j += 50)
                                                    {
                                                        var vector3 = vector2.CastPosition.Extend(ECatPos.ToVector2(), -j);
                                                        if (vector3.Distance(ObjectManager.Player) >= E.Range && v3 != Vector2.Zero)
                                                        {
                                                            if (E.Cast(v3) || E.Cast(v3))
                                                            {
                                                                return;
                                                            }
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            v3 = vector3.ToVector2();
                                                            continue;
                                                        }
                                                    }
                                            }
                                        }
                                    }
                                }
                            }
                            if (objPlayer.IsDashing())
                            {
                                if (objPlayer.GetDashInfo().EndPos.Distance(target) < 775)
                                {
                                    Geometry.Circle circle = new Geometry.Circle(objPlayer.Position, 775, 50);

                                    {
                                        foreach (var onecircle in circle.Points.Where(i => i.Distance(target) > 775))
                                        {
                                            if (E.Cast(onecircle))
                                            {
                                                var vector2 = FSpred.Prediction.Prediction.PredictUnitPosition(target, 600);
                                                var v3 = vector2;
                                                if (vector2.IsValid() && vector2.Distance(objPlayer.Position.ToVector2()) < E.Range - 100)
                                                    for (int j = 50; j <= 900; j += 50)
                                                    {
                                                        var vector3 = vector2.Extend(ECatPos.ToVector2(), -j);
                                                        if (vector3.Distance(ObjectManager.Player) >= E.Range)
                                                        {
                                                            if (E.Cast(v3.ToVector3()) || E.Cast(v3))
                                                            {
                                                                return;
                                                            }
                                                            break;
                                                        }
                                                        else
                                                        {
                                                            v3 = vector3;
                                                            continue;
                                                        }
                                                    }
                                            }
                                        }
                                    }
                                }
                            }
                        }                        
                    }
                }
            }

            if (target != null && target.HasBuff("ireliamark"))
            {
                var targets = TargetSelector.GetTargets(2000).Where(i => !CanQ(i)).OrderBy(i => i.Health / objPlayer.GetAutoAttackDamage(target));

                if (targets != null && E.IsReady())
                {
                    foreach (var tg in targets)
                    {
                        if (MenuSettings.ESettings.ImproveE.Enabled)
                        {
                            if (E.Name != "IreliaE" && ECatPos.IsValid())
                            {
                                var vector2 = FSpred.Prediction.Prediction.PredictUnitPosition(tg, 600);
                                var v3 = vector2;
                                if (vector2.IsValid() && vector2.Distance(objPlayer.Position.ToVector2()) < E.Range - 100)
                                    for (int j = 50; j <= 900; j += 50)
                                    {
                                        var vector3 = vector2.Extend(ECatPos.ToVector2(), -j);
                                        if (vector3.Distance(ObjectManager.Player) >= E.Range)
                                        {
                                            if (E.Cast(v3.ToVector3()) || E.Cast(v3))
                                            {
                                                return;
                                            }
                                            break;
                                        }
                                        else
                                        {
                                            v3 = vector3;
                                            continue;
                                        }
                                    }
                            }
                        }
                        else
                        {
                            if (objPlayer.HasBuff("IreliaE") && ECatPos.IsValid())
                            {
                                var castepos = Vector2.Zero;
                                Geometry.Circle onecircle = new Geometry.Circle(objPlayer.Position, 725);
                                foreach (var circle in onecircle.Points)
                                {
                                    E.Delay = (725 - E2.GetPrediction(tg).CastPosition.Distance(objPlayer)) / E.Speed + 0.25f + (0.3f + Game.Ping / 1000);
                                }

                                var epred = E.GetPrediction(target);

                                for (int i = 10000; i > 55; i -= 50)
                                {
                                    if (epred.CastPosition.Extend(ECatPos, -i).ToVector2().DistanceToPlayer() <= 775)
                                        castepos = epred.CastPosition.Extend(ECatPos, -i).ToVector2();

                                    if (castepos != Vector2.Zero)
                                    {
                                        if (E.Cast(castepos))
                                            return;
                                    }
                                }
                            }
                        }

                        if(E.Name == "IreliaE")
                        {
                            var poswillE = tg.Position.Extend(target.Position, 300);
                            if(poswillE.IsValid() && poswillE.DistanceToPlayer() <= E.Range)
                            {
                                if (E.Cast(poswillE))
                                {
                                    var vector2 = FSpred.Prediction.Prediction.PredictUnitPosition(tg, 600);
                                    var v3 = vector2;
                                    if (vector2.IsValid() && vector2.Distance(objPlayer.Position.ToVector2()) < E.Range - 100)
                                        for (int j = 50; j <= 900; j += 50)
                                        {
                                            var vector3 = vector2.Extend(ECatPos.ToVector2(), -j);
                                            if (vector3.Distance(ObjectManager.Player) >= E.Range)
                                            {
                                                if (E.Cast(v3.ToVector3()) || E.Cast(v3))
                                                {
                                                    return;
                                                }
                                                break;
                                            }
                                            else
                                            {
                                                v3 = vector3;
                                                continue;
                                            }
                                        }
                                }
                            }
                            else
                            {
                                if (E1.GetPrediction(tg).CastPosition.DistanceToPlayer() < 825)
                                {
                                    Geometry.Circle circle = new Geometry.Circle(objPlayer.Position, 600, 50);

                                    {
                                        foreach (var onecircle in circle.Points)
                                        {
                                            if (onecircle.Distance(target) > 600)
                                            {
                                                if (E.Cast(onecircle))
                                                {
                                                    var vector2 = FSpred.Prediction.Prediction.PredictUnitPosition(tg, 600);
                                                    var v3 = vector2;
                                                    if (vector2.IsValid() && vector2.Distance(objPlayer.Position.ToVector2()) < E.Range - 100)
                                                        for (int j = 50; j <= 900; j += 50)
                                                        {
                                                            var vector3 = vector2.Extend(ECatPos.ToVector2(), -j);
                                                            if (vector3.Distance(ObjectManager.Player) >= E.Range)
                                                            {
                                                                if (E.Cast(v3.ToVector3()) || E.Cast(v3))
                                                                {
                                                                    return;
                                                                }
                                                                break;
                                                            }
                                                            else
                                                            {
                                                                v3 = vector3;
                                                                continue;
                                                            }
                                                        }
                                                }
                                            }
                                        }
                                    }
                                }
                                if (objPlayer.IsDashing())
                                {
                                    if (objPlayer.GetDashInfo().EndPos.Distance(tg) < 775)
                                    {
                                        Geometry.Circle circle = new Geometry.Circle(objPlayer.Position, 775, 50);

                                        {
                                            foreach (var onecircle in circle.Points.Where(i => i.Distance(target) > 775))
                                            {
                                                if (E.Cast(onecircle))
                                                {
                                                    var vector2 = FSpred.Prediction.Prediction.PredictUnitPosition(tg, 600);
                                                    var v3 = vector2;
                                                    if (vector2.IsValid() && vector2.Distance(objPlayer.Position.ToVector2()) < E.Range - 100)
                                                        for (int j = 50; j <= 900; j += 50)
                                                        {
                                                            var vector3 = vector2.Extend(ECatPos.ToVector2(), -j);
                                                            if (vector3.Distance(ObjectManager.Player) >= E.Range)
                                                            {
                                                                if (E.Cast(v3.ToVector3()) || E.Cast(v3))
                                                                {
                                                                    return;
                                                                }
                                                                break;
                                                            }
                                                            else
                                                            {
                                                                v3 = vector3;
                                                                continue;
                                                            }
                                                        }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        public static float[] QBaseDamage = { 0f, 5f, 25f, 45f, 65f, 85f };
        public static float[] QBonusDamage = { 0f, 55f, 75f, 95f, 115f, 135f };

        private static float PassiveDamage(AIBaseClient target)
        {
            var ireliaPassiveDamage = 15f + (float)(ObjectManager.Player.Level - 1) * 3f;
            ireliaPassiveDamage += 0.25f * objPlayer.GetBonusPhysicalDamage();

            return ireliaPassiveDamage;
        }

        public static float Sheen()
        {
            if (Variables.TickCount < SheenTimer + 1600)
                return 0f;

            if (GameObjects.Player.CanUseItem((int)ItemId.Trinity_Force) 
                && ObjectManager.Player.CanUseItem((int)ItemId.Trinity_Force))
            {
                return GameObjects.Player.BaseAttackDamage * 2;
            }
            if (GameObjects.Player.CanUseItem((int)ItemId.Sheen) 
                && ObjectManager.Player.CanUseItem((int)ItemId.Sheen))
            {
                return GameObjects.Player.BaseAttackDamage;
            }
            
            return 0f;
        }

        public static float GetQDmg(AIBaseClient target, bool CheckItem = true)
        {
            if (target == null)
                return 0f;

            int level = Q.Level;
            if (level == 0)
            {
                return 0f;
            }
            var normaldmg = 5f + (level - 1) * 20f + ObjectManager.Player.TotalAttackDamage * 0.6f;
            if (target.IsMinion)
            {
                normaldmg += 55f + (float)(level - 1) * 20f;
            }
            var passivedmg = 0f;
            if (objPlayer.HasBuff("ireliapassivestacksmax"))
            {
                passivedmg = 15f + (ObjectManager.Player.Level - 1) * 3f + ObjectManager.Player.GetBonusPhysicalDamage() * 0.25f;
                passivedmg = (float)EnsoulSharp.SDK.Damage.CalculateMagicDamage(ObjectManager.Player, target, (double)passivedmg);
            }
            if (CheckItem == true)
                normaldmg += Sheen();

            return (float)EnsoulSharp.SDK.Damage.CalculatePhysicalDamage(ObjectManager.Player, target, normaldmg) + passivedmg;
            /*var Qdmg = QBaseDamage[Q.Level];
            var Qdmgbonus = QBonusDamage[Q.Level];

            //Normal Dmg
            {
                if(CheckItem == true)
                Qdmg += Sheen();
                Qdmg += 0.6f * GameObjects.Player.TotalAttackDamage;
            }
            
            //minion dmg
            if (target.IsMinion && target is AIMinionClient)
            {
                Qdmg += Qdmgbonus;
            }
           
            if (objPlayer.HasBuff("ireliapassivestacksmax"))
            {
                return (float)objPlayer.CalculatePhysicalDamage(target, Qdmg) + (float)objPlayer.CalculateMagicDamage(target, PassiveDamage(target));
            }
            else
            {
                return (float)objPlayer.CalculatePhysicalDamage(target, Qdmg);
            }*/
            
            /*if (objPlayer.HasBuff("ireliapassivestacksmax"))
            {
                return Q.GetDamage(target) + (float)objPlayer.CalculatePhysicalDamage(target, Sheen(target)) + (float)objPlayer.CalculateMagicDamage(target, PassiveDamage(target));
            }
            else
            {
                return Q.GetDamage(target) + (float)objPlayer.CalculatePhysicalDamage(target, Sheen(target));
            }*/
        }

        /*private static double GetQDmg(AIBaseClient target)
        {
            double dmgQ = Q.GetDamage(target);
            double dmgSheen = 0;
            double dmgMinions = 60;
            float passive = 0;

            bool sheen = false;
            bool trinity = false;

            if (ObjectManager.Player.CanUseItem((int)ItemId.Trinity_Force))
            {
                trinity = false;
                DelayAction.Add(10, () => { trinity = true; });
            }
            else { trinity = false; }
            if (ObjectManager.Player.CanUseItem((int)ItemId.Sheen))
            {
                sheen = false;
                DelayAction.Add(10, () => { sheen = true; });
            }
            else { sheen = false; }

            if (ObjectManager.Player.Level > 0 && ObjectManager.Player.HasBuff("ireliapassivestacksmax"))
            {
                passive = (objPlayer.Level - 1) * (ObjectManager.Player.Level == 1 ? 0 : 3) + 15;
            }
            if (ObjectManager.Player.HasBuff("Sheen") || sheen)
            {
                dmgSheen = ObjectManager.Player.TotalAttackDamage - 5;
            }
            if (trinity)
            {
                dmgSheen = (ObjectManager.Player.TotalAttackDamage - 30) * 2;
            }
            switch (Q.Level)
            {
                case 1:
                    dmgQ = 5f + ObjectManager.Player.TotalAttackDamage * 60 / 100;
                    dmgMinions = 60;
                    break;
                case 2:
                    dmgQ = 25f + ObjectManager.Player.TotalAttackDamage * 60 / 100;
                    dmgMinions = 100;
                    break;
                case 3:
                    dmgQ = 45f + ObjectManager.Player.TotalAttackDamage * 60 / 100;
                    dmgMinions = 140;
                    break;
                case 4:
                    dmgQ = 65f + ObjectManager.Player.TotalAttackDamage * 60 / 100;
                    dmgMinions = 180;
                    break;
                case 5:
                    dmgQ = 85f + ObjectManager.Player.TotalAttackDamage * 60 / 100;
                    dmgMinions = 220;
                    break;
            }
            double Alldmg = dmgQ + dmgSheen;
            if (target.IsMinion && !target.IsJungle())
            {
                Alldmg = dmgMinions + (ObjectManager.Player.TotalAttackDamage * 60 / 100) + dmgSheen;
            }
            return Q.GetDamage(target) + ObjectManager.Player.CalculateMagicDamage(target, passive) + objPlayer.CalculateDamage(target, DamageType.Physical, dmgSheen);
        }*/
        public static float Qspeed()
        {
            return 1500 + objPlayer.MoveSpeed;
        }
        public static List<AIBaseClient> Q_ListAIBaseClient(float minvalue = 0, float maxvalue = 0, AIBaseClient target = null)
        {
            if (target == null)
                return ObjectManager.Get<AIBaseClient>().Where(i => !i.IsDead && i.IsValidTarget(maxvalue) && (minvalue != 0 ? i.DistanceToPlayer() >= minvalue : i.IsValidTarget(maxvalue)) && !i.IsAlly && CanQ(i)).ToList();
            else
                return ObjectManager.Get<AIBaseClient>().Where(i => !i.IsDead && i.IsValidTarget(maxvalue) && (minvalue != 0 ? i.DistanceToPlayer() >= minvalue : i.IsValidTarget(maxvalue)) && !i.IsAlly && CanQ(i) && i.NetworkId != target.NetworkId).ToList();
        }
    }
}
