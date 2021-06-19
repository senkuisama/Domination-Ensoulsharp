using EnsoulSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using SharpDX;
using Color = System.Drawing.Color;


namespace xSaliceResurrected_Rework.Pluging
{
    public class Anivia
    {
        private GameObject _qMissle;
        private bool _eCasted;
        private GameObject _rObj;
        private bool _rFirstCreated;
        private bool _rByMe;
        private Spell Q, W, E, R;
        private Menu Menu = new Menu("Anivia", "xSaliceResurrected_Rework Anivia", true);
        public static readonly List<Spell> SpellList = new List<Spell>();

        public Anivia()
        {
            Q = new Spell(SpellSlot.Q, 1000f);
            W = new Spell(SpellSlot.W, 950f);
            E = new Spell(SpellSlot.E, ObjectManager.Player.GetCurrentAutoAttackRange());
            E.SetTargetted(float.MaxValue, 2000);
            R = new Spell(SpellSlot.R, 700f);

            Q.SetSkillshot(0.25f, 110f, 870f, false, SpellType.Line);
            W.SetSkillshot(0.25f, 1.0f, float.MaxValue, false, SpellType.Line);
            R.SetSkillshot(0.25f, 300f, float.MaxValue, false, SpellType.Circle);

            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);

            var combo = new Menu("Combo", "Combo");
            {
                combo.Add(new MenuBool("selected", "Focus Selected Target", true).SetValue(true));
                combo.Add(new MenuBool("UseQCombo", "Use Q", true).SetValue(true));
                combo.Add(new MenuBool("UseWCombo", "Use W", true).SetValue(true));
                combo.Add(new MenuBool("UseECombo", "Use E", true).SetValue(true));
                combo.Add(new MenuBool("UseEComboOnly", "Use E| Only Dounle Cast", true).SetValue(true));
                combo.Add(new MenuBool("UseRCombo", "Use R", true).SetValue(true));                
            }

            Menu.Add(combo);

            var harass = new Menu("Harass", "Harass");
            {
                harass.Add(new MenuBool("UseQHarass", "Use Q", true).SetValue(false));
                harass.Add(new MenuBool("UseEHarass", "Use E", true).SetValue(true));
                harass.Add(new MenuBool("UseRHarass", "Use R", true).SetValue(true));
                harass.Add(new MenuKeyBind("FarmT", "Harass (toggle)!", Keys.N, KeyBindType.Toggle));
            }

            Menu.Add(harass);


            var farm = new Menu("Farm", "Farm");
            {
                farm.Add(new MenuBool("UseQFarm", "Use Q", true).SetValue(false));
                farm.Add(new MenuBool("UseEFarm", "Use E", true).SetValue(false));
                farm.Add(new MenuBool("UseRFarm", "Use R", true).SetValue(false));
                Menu.Add(farm);
            }

            var misc = new Menu("Misc", "Misc");
            {
                //misc.Add(AoeSpellManager.AddHitChanceMenuCombo(true, false, false, false));
                misc.Add(new MenuKeyBind("snipe", "W/Q Snipe", Keys.T, KeyBindType.Press));
                misc.Add(new MenuBool("UseInt", "Use Spells to Interrupt", true).SetValue(true));
                misc.Add(new MenuBool("detonateQ", "Auto Detonate Q", true).SetValue(true));
                misc.Add(new MenuBool("detonateQ2", "Pop Q Behind Enemy", true).SetValue(true));
                misc.Add(new MenuBool("wallKill", "Wall Enemy on killable", true).SetValue(true));
                misc.Add(new MenuBool("UseGap", "Use W for GapCloser", true).SetValue(true));
                misc.Add(new MenuBool("checkR", "Auto turn off R", true).SetValue(true));
                misc.Add(new MenuBool("smartKS", "Use Smart KS System", true).SetValue(true));
                Menu.Add(misc);
            }

            var draw = new Menu("Drawings", "Drawings");
            /*{
                draw.Add(new MenuItem("QRange", "Q range", true).SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
                draw.Add(new MenuItem("WRange", "W range", true).SetValue(new Circle(true, Color.FromArgb(100, 255, 0, 255))));
                draw.Add(new MenuItem("ERange", "E range", true).SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));
                draw.Add(new MenuItem("RRange", "R range", true).SetValue(new Circle(false, Color.FromArgb(100, 255, 0, 255))));

                var drawComboDamageMenu = new MenuItem("Draw_ComboDamage", "Draw Combo Damage", true).SetValue(true);
                var drawFill = new MenuItem("Draw_Fill", "Draw Combo Damage Fill", true).SetValue(new Circle(true, Color.FromArgb(90, 255, 169, 4)));
                draw.Add(drawComboDamageMenu);
                draw.Add(drawFill);
                DamageIndicator.DamageToUnit = GetComboDamage;
                DamageIndicator.Enabled = drawComboDamageMenu.GetValue<MenuBool>().Enabled;
                DamageIndicator.Fill = drawFill.GetValue<Circle>().Active;
                DamageIndicator.FillColor = drawFill.GetValue<Circle>().Color;
                drawComboDamageMenu.ValueChanged +=
                    delegate (object sender, OnValueChangeEventArgs eventArgs)
                    {
                        DamageIndicator.Enabled = eventArgs.GetNewValue<bool>();
                    };
                drawFill.ValueChanged +=
                    delegate (object sender, OnValueChangeEventArgs eventArgs)
                    {
                        DamageIndicator.Fill = eventArgs.GetNewValue<Circle>().Active;
                        DamageIndicator.FillColor = eventArgs.GetNewValue<Circle>().Color;
                    };
                Menu.AddSubMenu(draw);
            }*/

            /*var customMenu = new Menu("Custom Perma Show", "Custom Perma Show");
            {
                var myCust = new CustomPermaMenu();
                customMenu.Add(new MenuItem("custMenu", "Move Menu", true).SetValue(new KeyBind("L".ToCharArray()[0], KeyBindType.Press)));
                customMenu.AddItem(new MenuItem("enableCustMenu", "Enabled", true).SetValue(true));
                customMenu.AddItem(myCust.AddadToMenu("Combo Active: ", "Orbwalk"));
                customMenu.AddItem(myCust.AddToMenu("Harass Active: ", "Farm"));
                customMenu.AddItem(myCust.AddToMenu("Harass(T) Active: ", "FarmT"));
                customMenu.AddItem(myCust.AddToMenu("Laneclear Active: ", "LaneClear"));
                customMenu.AddItem(myCust.AddToMenu("Escape Active: ", "Flee"));
                customMenu.AddItem(myCust.AddToMenu("Snipe Active: ", "snipe"));
                Menu.AddSubMenu(customMenu);
            }*/
            Menu.Attach();
            OnUp();
        }
        private AIHeroClient Player => ObjectManager.Player;

        private float GetComboDamage(AIBaseClient enemy)
        {
            if (enemy == null)
                return 0;

            var damage = 0d;

            if (Q.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.Q);

            if (E.IsReady() & (Q.IsReady() || R.IsReady()))
                damage += Player.GetSpellDamage(enemy, SpellSlot.E) * 2;
            else if (E.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.E);

            if (R.IsReady())
                damage += Player.GetSpellDamage(enemy, SpellSlot.R) * 3;

            //damage = ItemManager.CalcDamage(enemy, damage);

            return (float)damage;
        }
        public static AMenuComponent Item(Menu menu, string name, bool championUnique = false)
        {
            if (!championUnique)
            {
                name = ObjectManager.Player.CharacterName + name;
            }

            //Search in our own items
            foreach (var item in menu.Components.ToArray().Where(item => !(item.Value is Menu) && item.Value.Name == name))
            {
                return item.Value;
            }

            //Search in submenus
            foreach (var subMenu in menu.Components.ToArray().Where(x => x.Value is Menu))
            {
                foreach (var item in (subMenu.Value as Menu)?.Components)
                {
                    if (item.Value is Menu)
                    {
                        var result = Item((item.Value as Menu), name, championUnique);
                        if (result != null)
                        {
                            return result;
                        }
                    }
                    else if (item.Value.Name == name)
                    {
                        return item.Value;

                    }
                }

            }

            return null;
        }
        private void Combo()
        {
            var range = Q.IsReady() ? Q.Range : E.Range;
            var focusSelected = Item(Menu, "selected", true).GetValue<MenuBool>().Enabled;
            var target = TargetSelector.GetTarget(range, DamageType.Magical);

            if (TargetSelector.SelectedTarget != null)
            {
                if (focusSelected && TargetSelector.SelectedTarget.Distance(Player.Position) < range)
                {
                    target = TargetSelector.SelectedTarget;
                }
            }

            if (target == null)
                return;

            var dmg = GetComboDamage(target);

            var itemTarget = TargetSelector.GetTarget(750, DamageType.Magical);

            /*if (itemTarget != null)
            {
                ItemManager.Target = itemTarget;

                if (dmg > itemTarget.Health - 50)
                    ItemManager.KillableTarget = true;

                ItemManager.UseTargetted = true;
            }*/

            if (Item(Menu, "UseRCombo", true).GetValue<MenuBool>().Enabled && R.IsReady() &&
                target.IsValidTarget(R.Range))
            {
                if (ShouldR(target))
                {
                    R.Cast(target.Position);
                }
            }

            if (Item(Menu, "UseQCombo", true).GetValue<MenuBool>().Enabled && Q.IsReady() &&
                target.IsValidTarget(Q.Range) && !HaveQ2)
            {
                var QPred = Q.GetPrediction(target);

                if (QPred.Hitchance >= HitChance.VeryHigh)
                {
                    Q.Cast(QPred.CastPosition);
                }
            }

            if (Item(Menu, "UseECombo", true).GetValue<MenuBool>().Enabled && E.IsReady() &&
                target.IsValidTarget(E.Range) && ShouldE(target))
            {
                E.CastOnUnit(target);
            }

            if (Item(Menu, "UseWCombo", true).GetValue<MenuBool>().Enabled && W.IsReady() &&
                target.IsValidTarget(W.Range) && ShouldUseW(target))
            {
                CastW(target);
            }
        }
        private bool HaveQ2 => ObjectManager.Player.HasBuff(Q.Name);
        private bool HaveR2 => ObjectManager.Player.HasBuff(R.Name);
        private void Harass()
        {
            var range = Q.IsReady() && Item(Menu, "UseQHarass", true).GetValue<MenuBool>().Enabled ? Q.Range : E.Range;
            var target = TargetSelector.GetTarget(range, DamageType.Magical);

            if (target.IsValidTarget(range))
            {
                if (Item(Menu, "UseRHarass", true).GetValue<MenuBool>().Enabled && R.IsReady() &&
                    target.IsValidTarget(R.Range))
                {
                    R.Cast(target.Position);
                }

                if (Item(Menu ,"UseQHarass", true).GetValue<MenuBool>().Enabled && Q.IsReady() &&
                    target.IsValidTarget(Q.Range) && !HaveQ2)
                {
                    var Qpred = Q.GetPrediction(target);

                    if (Qpred.Hitchance >= HitChance.VeryHigh)
                    {
                        Q.Cast(Qpred.CastPosition);
                    }
                }

                if (Item(Menu ,"UseEHarass", true).GetValue<MenuBool>().Enabled && E.IsReady() &&
                    target.IsValidTarget(E.Range) && target.HasBuff("chilled"))
                {
                    E.CastOnUnit(target);
                }
            }
        }

        private void SmartKs()
        {
            if (!Item(Menu ,"smartKS", true).GetValue<MenuBool>().Enabled)
                return;

            foreach (var target in ObjectManager.Get<AIHeroClient>().Where(x => x.IsValidTarget(1300)))
            {
                //ER
                if (Player.Distance(target.Position) <= R.Range && !_rFirstCreated &&
                    Player.GetSpellDamage(target, SpellSlot.R) + Player.GetSpellDamage(target, SpellSlot.E) * 2 >
                    target.Health + 50)
                {
                    if (R.IsReady() && E.IsReady() && ShouldR(target))
                    {
                        E.CastOnUnit(target);
                        R.CastOnUnit(target);
                        return;
                    }
                }

                //QR
                if (Player.Distance(target.Position) <= R.Range && ShouldQ() &&
                    Player.GetSpellDamage(target, SpellSlot.Q) + Player.GetSpellDamage(target, SpellSlot.R) >
                    target.Health + 30)
                {
                    if (W.IsReady() && R.IsReady())
                    {
                        W.Cast(target);
                        return;
                    }
                }

                //Q
                if (Player.Distance(target.Position) <= Q.Range && ShouldQ() &&
                    Player.GetSpellDamage(target, SpellSlot.Q) > target.Health + 30)
                {
                    if (Q.IsReady())
                    {
                        Q.Cast(target);
                        return;
                    }
                }

                //E
                if (Player.Distance(target.Position) <= E.Range &&
                    Player.GetSpellDamage(target, SpellSlot.E) > target.Health + 30)
                {
                    if (E.IsReady())
                    {
                        E.CastOnUnit(target);
                        return;
                    }
                }
            }
        }

        private bool ShouldQ()
        {
            return Variables.GameTimeTickCount - Q.LastCastAttemptTime > 2000;
        }

        private bool ShouldR(bool isCombo)
        {
            if (_rObj == null)
            {
                return true;
            }
            if (_rByMe)
            {
                return false;
            }

            return _eCasted || isCombo;
        }

        private bool ShouldR(AIBaseClient target)
        {
            if (!HaveR2 || _rObj == null)
                return true;
            else
            {
                if (_rObj.Distance(target) >= 400)
                    return true;
            }

            return false;
        }

        private bool ShouldE(AIHeroClient target)
        {
            if (checkChilled(target))
                return true;

            if (Player.GetSpellDamage(target, SpellSlot.E) >= target.Health)
                return true;

            return !Item(Menu, "UseEComboOnly", true).GetValue<MenuBool>().Enabled;
        }

        private bool ShouldUseW(AIHeroClient target)
        {
            if (GetComboDamage(target) >= target.Health - 20 && Item(Menu, "wallKill", true).GetValue<MenuBool>().Enabled)
                return true;

            if (_rFirstCreated && _rObj != null)
            {
                if (_rObj.Position.Distance(target.Position) > 300)
                {
                    return true;
                }
            }

            return false;
        }

        private void CastW(AIHeroClient target)
        {
            var pred = W.GetPrediction(target);
            var vec = new Vector3(pred.CastPosition.X - Player.Position.X, 0,
                pred.CastPosition.Z - Player.Position.Z);

            var castBehind = pred.CastPosition + Vector3.Normalize(vec) * 125;

            if (W.IsReady())
                W.Cast(castBehind);
        }

        private void CastWEscape(AIHeroClient target)
        {
            var pred = W.GetPrediction(target);
            var vec = new Vector3(pred.CastPosition.X - Player.Position.X, 0,
                pred.CastPosition.Z - Player.Position.Z);

            var castBehind = pred.CastPosition - Vector3.Normalize(vec) * 125;

            if (W.IsReady())
                W.Cast(castBehind);
        }

        private bool checkChilled(AIHeroClient target)
        {
            return target.Buffs.Count(i => i.Name.ToLower().Contains("chilled")) >= 1;
        }

        private void DetonateQ()
        {
            if (_qMissle == null || !Q.IsReady())
                return;

            foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(x => x.IsValidTarget(1500)).OrderByDescending(GetComboDamage))
            {
                if (ShouldDetonate(enemy))
                {
                    Q.Cast();
                }
            }
        }

        private bool ShouldDetonate(AIHeroClient target)
        {
            if (Item(Menu, "detonateQ2", true).GetValue<MenuBool>().Enabled)
            {
                if (target.Distance(_qMissle.Position) < Q.Width + target.BoundingRadius)
                    return true;
            }

            return target.Distance(_qMissle.Position) < Q.Width + target.BoundingRadius;
        }

        private void Snipe()
        {
            var range = Q.Range;
            var focusSelected =  Item(Menu, "selected", true).GetValue<MenuBool>().Enabled;
            var qTarget = TargetSelector.GetTarget(range, DamageType.Magical);

            if (TargetSelector.SelectedTarget != null)
                if (focusSelected && TargetSelector.SelectedTarget.Distance(Player.Position) < range)
                    qTarget = TargetSelector.SelectedTarget;

            Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            if (qTarget == null)
                return;

            if (W.IsReady() && Q.IsReady() && Player.Distance(qTarget.Position) < W.Range)
                CastW(qTarget);

            if (!W.IsReady() && Q.IsReady() && Player.Distance(qTarget.Position) < Q.Range &&
                Q.GetPrediction(qTarget).Hitchance >= HitChance.High && ShouldQ())
            {
                Q.Cast(Q.GetPrediction(qTarget).CastPosition);
            }
        }

        private void CheckR()
        {
            if (_rObj == null)
                return;

            var hit = ObjectManager.Get<AIHeroClient>().Count(x => _rObj.Position.Distance(x.Position) < 475 && x.IsValidTarget(R.Range + 500));

            if (hit < 1 && R.IsReady() && _rFirstCreated && R.IsReady())
            {
                R.Cast();
            }
        }

        private void Escape()
        {
            Player.IssueOrder(GameObjectOrder.MoveTo, Game.CursorPos);

            var enemy = (from champ in ObjectManager.Get<AIHeroClient>() where champ.IsValidTarget(1500) select champ).ToList();

            var hero = enemy.FirstOrDefault();

            if (hero != null && Q.IsReady() && Player.Distance(hero.Position) <= Q.Range && Q.GetPrediction(hero).Hitchance >= HitChance.High && ShouldQ())
            {
                Q.Cast(enemy.FirstOrDefault());
            }

            if (hero != null && (W.IsReady() && Player.Distance(hero.Position) <= W.Range))
            {
                CastWEscape(enemy.FirstOrDefault());
            }
        }

        private void Farm()
        {
            var allMinionsQ = GameObjects.GetMinions(ObjectManager.Player.Position, Q.Range,
                MinionTypes.All, MinionTeam.Enemy);
            var allMinionsR = GameObjects.GetMinions(ObjectManager.Player.Position, R.Range,
                MinionTypes.All, MinionTeam.Enemy);
            var allMinionsE = GameObjects.GetMinions(ObjectManager.Player.Position, E.Range,
                MinionTypes.All, MinionTeam.Enemy);

            var useQ = Item(Menu,"UseQFarm", true).GetValue<MenuBool>().Enabled;
            var useE = Item(Menu,"UseEFarm", true).GetValue<MenuBool>().Enabled;
            var useR = Item(Menu,"UseRFarm", true).GetValue<MenuBool>().Enabled;

            int hit = 0;

            if (useQ && Q.IsReady() && ShouldQ())
            {
                var qPos = Q.GetLineFarmLocation(allMinionsQ);
                if (qPos.MinionsHit >= 3)
                {
                    Q.Cast(qPos.Position);
                }
            }

            if (useR & R.IsReady() && !_rFirstCreated)
            {
                var rPos = R.GetCircularFarmLocation(allMinionsR);
                if (Player.Distance(rPos.Position) < R.Range)
                    R.Cast(rPos.Position);
            }

            if (!ShouldQ() && _qMissle != null)
            {
                if (useQ && Q.IsReady())
                {
                    hit += allMinionsQ.Count(enemy => enemy.Distance(_qMissle.Position) < 110);
                }

                if (hit >= 2 && Q.IsReady())
                    Q.Cast();
            }

            if (_rFirstCreated)
            {
                hit += allMinionsR.Count(enemy => enemy.Distance(_rObj.Position) < 400);

                if (hit < 2 && R.IsReady())
                    R.Cast();
            }

            if (useE && allMinionsE.Count > 0 && E.IsReady())
                E.Cast(allMinionsE.FirstOrDefault());
        }

        private void Game_OnGameUpdate(EventArgs args)
        {
            //check if player is dead
            if (Player.IsDead)
                return;

            //detonate Q check
            var detQ = Item(Menu,"detonateQ", true).GetValue<MenuBool>().Enabled;
            if (detQ && HaveQ2)
                DetonateQ();

            //checkR
            var rCheck = Item(Menu,"checkR", true).GetValue<MenuBool>().Enabled;
            if (rCheck && _rFirstCreated && !Item(Menu,"LaneClear", true).GetValue<MenuKeyBind>().Active && _rByMe)
                CheckR();


            //check ks
            SmartKs();

            switch (Orbwalker.ActiveMode)
            {
                case OrbwalkerMode.Combo:
                    Combo();
                    break;
                /*case Orbwalking.OrbwalkingMode.Flee:
                    Escape();
                    break;*/
                case OrbwalkerMode.LastHit:
                    break;
                case OrbwalkerMode.Harass:
                    Harass();
                    break;
                case OrbwalkerMode.LaneClear:
                    Farm();
                    break;
                case OrbwalkerMode.None:
                    /*if (Item(Menu,"FarmT", true).GetValue<KeyBind>().Active)
                        Harass();

                    if (Item(Menu,"snipe", true).GetValue<KeyBind>().Active)
                        Snipe();*/
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private void Drawing_OnDraw(EventArgs args)
        {
            foreach (var spell in SpellList)
            {
                /*var menuItem = Item(Menu,spell.Slot + "Range", true).GetValue<>();
                if (menuItem.Active)
                    Render.Circle.DrawCircle(Player.Position, spell.Range, menuItem.Color);*/
            }
        }
        private int LastR = 0;
        private void Obj_AI_Base_OnProcessSpellCast(AIBaseClient unit, AIBaseClientProcessSpellCastEventArgs attack)
        {
            if (!unit.IsMe) return;

            if (attack.Slot == SpellSlot.R)
                LastR = Variables.GameTimeTickCount;

            var castedSlot = Player.GetSpellSlot(attack.SData.Name);

            if (castedSlot == SpellSlot.E)
            {
                _eCasted = true;
            }

            if (castedSlot == SpellSlot.Q && ShouldQ())
            {
                Q.LastCastAttemptTime = Environment.TickCount;
            }

        }

        private void AntiGapcloser_OnEnemyGapcloser(AIHeroClient Sender, AntiGapcloser.GapcloserArgs gapcloser)
        {
            if (!Item(Menu,"UseGap", true).GetValue<MenuBool>().Enabled) return;

            if (W.IsReady() && Sender.IsValidTarget(W.Range))
            {
                var vec = Player.Position -
                              Vector3.Normalize(Player.Position - Sender.Position) * 1;
                W.Cast(vec);
            }
        }

        /*protected override void Interrupter_OnPosibleToInterrupt(AIHeroClient unit, Interrupter2.InterruptableTargetEventArgs spell)
        {
            if (!Item(Menu,"UseInt", true).GetValue<MenuBool>().Enabled) return;

            if (unit.IsValidTarget(Q.Range) && Q.IsReady())
            {
                Q.Cast(unit);
            }

            if (unit.IsValidTarget(W.Range) && W.IsReady())
            {
                W.Cast(unit);
            }
        }*/

        private void GameObject_OnCreate(GameObject obj, EventArgs args)
        {
            //Q
            if (obj.IsValid && obj.Name.Contains(Q.Name) && (obj as MissileClient).SpellCaster.IsMe)
            {
                _qMissle = obj;
            }

            //R
            if (obj.IsValid && obj.Name.Contains("Anivia") && obj.Name.Contains("R"))
            {
                if ((obj as EffectEmitter).IsAlly)
                    return;

                if(LastR - Variables.GameTimeTickCount <= 50 || Variables.GameTimeTickCount - LastR <= 50)
                {
                    _rByMe = true;
                    _rObj = obj;
                    _rFirstCreated = true;
                }               
            }
        }

        private void GameObject_OnDelete(GameObject obj, EventArgs args)
        {
            //Q
            if (Player.Distance(obj.Position) < 1500)
            {
                if (obj.IsValid && obj.Name.Contains(Q.Name) && (obj as MissileClient).SpellCaster.IsMe)
                {
                    _qMissle = null;
                }

                //R
                if (obj.IsValid && obj.Name.Contains("Anivia") && obj.Name.Contains("R"))
                {
                    if ((obj as EffectEmitter).IsAlly)
                        return;

                    if (LastR - Variables.GameTimeTickCount <= 50 || Variables.GameTimeTickCount - LastR <= 50)
                    {
                        _rObj = null;
                        _rFirstCreated = false;
                        _rByMe = false;
                    }                    
                }
            }
        }

        private void OnUp()
        {
            MissileClient.OnDelete += GameObject_OnDelete;
            MissileClient.OnCreate += GameObject_OnCreate;
            AntiGapcloser.OnGapcloser += AntiGapcloser_OnEnemyGapcloser;
            AIBaseClient.OnProcessSpellCast += Obj_AI_Base_OnProcessSpellCast;
            Game.OnUpdate += Game_OnGameUpdate;
        }
    }
}
