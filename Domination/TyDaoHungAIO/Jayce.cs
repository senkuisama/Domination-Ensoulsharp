using System;
using System.Collections.Generic;
using System.Linq;
using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using SharpDX;
using EnsoulSharp.SDK.Utility;
using Color = System.Drawing.Color;
using SharpDX.Direct3D9;

namespace DaoHungAIO.Champions
{
    public static class Jayce
    {
        private const string CharName = "Jayce";

        private static Menu Config;

        private static HpBarIndicator hpi = new HpBarIndicator();

        private static AIHeroClient Player = ObjectManager.Player;


        private static SummonerItems sumItems = new SummonerItems(Player);

        private static Spellbook sBook = Player.Spellbook;


        private static SpellDataInst Qdata = sBook.GetSpell(SpellSlot.Q);
        private static SpellDataInst Wdata = sBook.GetSpell(SpellSlot.W);
        private static SpellDataInst Edata = sBook.GetSpell(SpellSlot.E);
        private static SpellDataInst Rdata = sBook.GetSpell(SpellSlot.R);
        private static Spell Q1 = new Spell(SpellSlot.Q, 1050);//Emp 1470
        private static Spell QEmp1 = new Spell(SpellSlot.Q, 1600);//Emp 1470
        private static Spell W1 = new Spell(SpellSlot.W, 0);
        private static Spell E1 = new Spell(SpellSlot.E, 650);
        private static Spell R1 = new Spell(SpellSlot.R, 0);

        private static Spell Q2 = new Spell(SpellSlot.Q, 600);
        private static Spell W2 = new Spell(SpellSlot.W, 285);
        private static Spell E2 = new Spell(SpellSlot.E, 240);
        private static Spell R2 = new Spell(SpellSlot.R, 0);

        private static AIBaseClientProcessSpellCastEventArgs castEonQ = null;
        private static int castedTimeUnreach = 0;

        private static MissileClient myCastedQ = null;

        private static AIHeroClient lockedTarg = null;

        private static AIHeroClient castedQon = null;

        private static Vector3 castQon = new Vector3(0, 0, 0);

        /* COOLDOWN STUFF */
        private static float[] rangTrueQcd = { 8, 8, 8, 8, 8, 8 };
        private static float[] rangTrueWcd = { 13, 11.4f, 9.8f, 8.2f, 6.6f, 5 };
        private static float[] rangTrueEcd = { 16, 16, 16, 16, 16, 16 };

        private static float[] hamTrueQcd = { 16, 14, 12, 10, 8, 6 };
        private static float[] hamTrueWcd = { 10, 10, 10, 10, 10, 10 };
        private static float[] hamTrueEcd = { 15, 14, 13, 12, 11, 10 };

        private static float rangQCD = 0, rangWCD = 0, rangECD = 0;
        private static float hamQCD = 0, hamWCD = 0, hamECD = 0;

        private static float rangQCDRem = 0, rangWCDRem = 0, rangECDRem = 0;
        private static float hamQCDRem = 0, hamWCDRem = 0, hamECDRem = 0;


        /* COOLDOWN STUFF END */
        private static bool isHammer = false;



        public static void JayceLoad()
        {
            Notifications.Add(new Notification("Dao Hung AIO fuck WWapper", "Jayce credit  Detusk"));

            setSkillShots();
            try
            {

                Config = new Menu("Jayce", "DH.Jayce", true);
                //Combo
                var combo = new Menu("combo", "Combo Sharp");
                combo.Add(new MenuSlider("HitChanceQ", "Q hitchance", 2, 1, 4));
                combo.Add(new MenuBool("comboItems", "Use Items"));
                combo.Add(new MenuBool("hammerKill", "Hammer if killable"));
                combo.Add(new MenuBool("parlelE", "use pralel gate"));
                combo.Add(new MenuKeyBind("fullDMG", "Do full damage", Keys.A, KeyBindType.Press));
                combo.Add(new MenuKeyBind("injTarget", "Tower Injection", Keys.G, KeyBindType.Press));
                combo.Add(new MenuKeyBind("awsPress", "Press for awsomeee!!", Keys.Z, KeyBindType.Press));
                combo.Add(new MenuSlider("eAway", "Gate distance from side", 60, 3, 60));

                //Extra
                var extra = new Menu("extra", "Extra Sharp");
                extra.Add(new MenuKeyBind("shoot", "Shoot manual Q", Keys.T, KeyBindType.Press));

                extra.Add(new MenuBool("gapClose", "Kick Gapclosers"));
                extra.Add(new MenuBool("autoInter", "Interupt spells"));
                extra.Add(new MenuBool("useMunions", "Q use Minion colision"));
                extra.Add(new MenuBool("killSteal", "Killsteal")).SetValue(false);
                extra.Add(new MenuBool("packets", "Use Packet cast")).SetValue(false);

                //Debug
                var draw = new Menu("draw", "Drawing");
                draw.Add(new MenuBool("drawCir", "Draw circles"));
                draw.Add(new MenuBool("drawCD", "Draw CD"));
                draw.Add(new MenuBool("drawFull", "Draw full combo dmg"));

                Config.Add(combo);
                Config.Add(extra);
                Config.Add(draw);
                Config.Attach();


                Drawing.OnDraw += onDraw;
                Drawing.OnEndScene += OnEndScene;

                Game.OnUpdate += OnGameUpdate;

                GameObject.OnCreate += onCreate;
                GameObject.OnDelete += onDelete;

                //AIBaseClient.OnDamage += onDamage;

                AIBaseClient.OnProcessSpellCast += AIBaseClientProcessSpellCast;
                AntiGapcloser.OnGapcloser += OnGapcloserEvent;
                Interrupter.OnInterrupterSpell += InterrupterSpellHandler;
                //SmoothMouse.start();

                Game.Print("<font color=\"#05FAAC\"><b>DH.Jayce:</b></font> Feedback send to facebook yts.1996 Sayuto");
                Game.Print("<font color=\"#FF9900\"><b>Credits: Detuks</b></font>");
                Game.Print("<font color=\"#FA053D\"><b>Notice: This script not include Farm/JungClear</b></font>");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                Game.Print("Oops. Something went wrong with DH.Jayce");
            }

        }

        //private static void onDamage(AttackableUnit sender, AttackableUnitDamageEventArgs args)
        //{
        //    if (args.SourceNetworkId == Player.NetworkId)
        //    {
        //    }
        //}

        private static void onDelete(GameObject sender, EventArgs args)
        {
            if (myCastedQ != null && myCastedQ.NetworkId == sender.NetworkId)
            {
                myCastedQ = null;
                castedQon = null;
            }
        }

        private static void onCreate(GameObject sender, EventArgs args)
        {
            //Console.WriteLine(sender.Name+" TYPE: "+sender.Type);

            if (sender is AIMinionClient && sender.Name == "hiu" && E1.IsReady())
            {
            }

            if (sender is MissileClient)
            {
                var mis = (MissileClient)sender;
                if (mis.SpellCaster.IsMe)
                {
                    //Console.WriteLine("My MIssle rdy");
                    myCastedQ = mis;
                }

            }
        }

        private static void InterrupterSpellHandler(AIHeroClient sender, Interrupter.InterruptSpellArgs args)
        {

            if (Config["extra"].GetValue<MenuBool>("autoInter").Enabled && (int)args.DangerLevel > 0)
                knockAway(sender);
        }

        private static void OnEndScene(EventArgs args)
        {
            if (Config["combo"].GetValue<MenuKeyBind>("awsPress").Active)
            {
                hpi.drawAwsomee();
            }

            if (Config["draw"].GetValue<MenuBool>("drawFull").Enabled)
                foreach (var enemy in GameObjects.EnemyHeroes.Where(ene => !ene.IsDead && ene.IsVisible))
                {
                    hpi.unit = enemy;
                    hpi.drawDmg(getJayceFullComoDmg(enemy), Color.Yellow);
                }
        }

        public static AMenuComponent Item(this Menu menu, string name, bool championUnique = false)
        {
            if (championUnique)
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
                        var result = (item.Value as Menu).Item(name, championUnique);
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
        private static void OnGapcloserEvent(AIHeroClient sender, AntiGapcloser.GapcloserArgs args)
        {
            if (Config["extra"].GetValue<MenuBool>("gapClose").Enabled)
                knockAway(sender);
        }      
        private static void OnGameUpdate(EventArgs args)
        {

            checkForm();
            processCDs();
            if(Config.Item("HitChanceQ").GetValue<MenuSlider>().Value != (int)Q1.MinHitChance)
            {
                Q1.MinHitChance = (HitChance)Config.Item("HitChanceQ").GetValue<MenuSlider>().Value;
            }
            if (Config["extra"].GetValue<MenuKeyBind>("shoot").Active)
            {
                shootQE(Game.CursorPos, true);
            }
            if (myCastedQ != null && (Config["combo"].GetValue<MenuKeyBind>("fullDMG").Active || Orbwalker.ActiveMode == OrbwalkerMode.Combo))
            {
                castEonSpell(myCastedQ);
            }
            /* if (castedQon != null && !isHammer)
            {
                if((getJayceEQDmg(castedQon) > castedQon.Health ||
                 castedQon.Distance(Player) > E1.Range || !Config.Item("useExploit")))
                {
                    if (castQon.X != 0)
                        shootQE(castQon);
                }
                else
                {
                    doExploit(castedQon);
                }
            }*/
            //}



            if (Config["combo"].GetValue<MenuKeyBind>("fullDMG").Active)//fullDMG
            {
                activateMura();
                AIHeroClient target = TargetSelector.GetTarget(getBestRange(), DamageType.Physical);
                if (lockedTarg == null)
                    lockedTarg = target;
                doFullDmg(lockedTarg);
            }
            else
            {
                lockedTarg = null;
            }

            if (Config["combo"].GetValue<MenuKeyBind>("injTarget").Active)//fullDMG
            {
                activateMura();
                AIHeroClient target = TargetSelector.GetTarget(getBestRange(), DamageType.Physical);
                if (lockedTarg == null)
                    lockedTarg = target;
                doJayceInj(lockedTarg);
            }
            else
            {
                lockedTarg = null;
            }

            // if (castEonQ != null && (castEonQ. - 2) > Game.Time)
            //    castEonQ = null;

            if (Orbwalker.ActiveMode == OrbwalkerMode.Combo)
            {
                activateMura();
                AIHeroClient target = TargetSelector.GetTarget(getBestRange(), DamageType.Physical);
                doCombo(target);
            }

            if (Config["extra"].GetValue<MenuBool>("killSteal").Enabled)
                doKillSteal();

            if (Orbwalker.ActiveMode == OrbwalkerMode.Harass)
            {
                deActivateMura();
            }

            if (Orbwalker.ActiveMode == OrbwalkerMode.LaneClear)
            {
                deActivateMura();
            }
        }

        private static void onDraw(EventArgs args)
        {
            //Draw CD
            if (Config["draw"].GetValue<MenuBool>("drawCD").Enabled)
                drawCD();

            if (!Config["draw"].GetValue<MenuBool>("drawCir").Enabled)
                return;
            Drawing.DrawCircle(Player.Position, !isHammer ? 1100 : 600, Color.Red);

            Drawing.DrawCircle(Player.Position, 1550, Color.Violet);
        }



        private static void AIBaseClientProcessSpellCast(AIBaseClient obj, AIBaseClientProcessSpellCastEventArgs arg)
        {
            if (obj.IsMe)
            {

                if (arg.SData.Name == "jayceshockblast")
                {
                    //  castEonQ = arg;
                }
                else if (arg.SData.Name == "jayceaccelerationgate")
                {
                    castEonQ = null;
                    // Console.WriteLine("Cast dat E on: " + arg.SData.Name);
                }
                getCDs(arg);
            }
        }


        private static void setSkillShots()
        {
            Q1.SetSkillshot(0.3f, 70f, 1500, true, SpellType.Line);
            QEmp1.SetSkillshot(0.3f, 70f, 2180, true, SpellType.Line);
            // QEmp1.SetSkillshot(0.25f, 70f, float.MaxValue, false, Prediction.SkillshotType.Line);
        }


        private static void doCombo(AIHeroClient target)
        {
            if (target == null)
                return;
            castOmen(target);
            if (!isHammer)
            {
                //if (castEonQ != null)
                //    castEonSpell(target);
                //DO QE combo first
                if (E1.IsReady() && Q1.IsReady() && gotManaFor(true, false, true))
                {
                    castQEPred(target);
                }
                else if (Q1.IsReady() && gotManaFor(true))
                {
                    castQPred(target);
                }
                else if (W1.IsReady() && gotManaFor(false, true) && targetInRange(getClosestEnem(), 650f))
                {
                    W1.Cast();
                    sumItems.cast(SummonerItems.ItemIds.Ghostblade);
                }//and wont die wih 1 AA
                else if (!Q1.IsReady() && !W1.IsReady() && R1.IsReady() && hammerWillKill(target) && hamQCDRem == 0 && hamECDRem == 0)// will need to add check if other form skills ready
                {
                    R1.Cast();
                }
            }
            else
            {
                if (!Q2.IsReady() && R2.IsReady() && Player.Distance(getClosestEnem()) > 350)
                {
                    sumItems.cast(SummonerItems.ItemIds.Ghostblade);
                    R2.Cast();
                }
                if (Q2.IsReady() && gotManaFor(true) && targetInRange(target, Q2.Range) && Player.Distance(target) > 300)
                {
                    sumItems.cast(SummonerItems.ItemIds.Ghostblade);
                    Q2.Cast(target);
                }
                if (E2.IsReady() && gotManaFor(false, false, true) && targetInRange(target, E2.Range) && shouldIKnockDatMadaFaka(target))
                {
                    E2.Cast(target);
                }
                if (W2.IsReady() && gotManaFor(false, true) && targetInRange(target, W2.Range))
                {
                    W2.Cast();
                }

            }
        }


        private static void doFullDmg(AIHeroClient target)
        {
            if (target == null)
                return; ;
            castIgnite(target);
            if (!isHammer)
            {
                if (castEonQ != null)
                {
                    castEonSpell(target);
                }
                //DO QE combo first
                if (E1.IsReady() && Q1.IsReady() && gotManaFor(true, false, true))
                {
                    castQEPred(target);
                }
                else if (Q1.IsReady() && gotManaFor(true))
                {
                    castQPred(target);
                }
                else if (W1.IsReady() && gotManaFor(false, true) && targetInRange(getClosestEnem(), 1000f))
                {

                    sumItems.cast(SummonerItems.ItemIds.Ghostblade);
                    W1.Cast();
                }
                else if (!Q1.IsReady() && !W1.IsReady() && R1.IsReady() && hamQCDRem == 0 && hamECDRem == 0)// will need to add check if other form skills ready
                {
                    R1.Cast();
                }
            }
            else
            {
                if (!Q2.IsReady() && R2.IsReady() && Player.Distance(getClosestEnem()) > 350)
                {

                    sumItems.cast(SummonerItems.ItemIds.Ghostblade);
                    R2.Cast();
                }
                if (Q2.IsReady() && gotManaFor(true) && targetInRange(target, Q2.Range))
                {
                    Q2.Cast(target);
                }
                if (E2.IsReady() && gotManaFor(false, false, true) && targetInRange(target, E2.Range) && (!gotSpeedBuff()) || (getJayceEHamDmg(target) > target.Health))
                {
                    E2.Cast(target);
                }
                if (W2.IsReady() && gotManaFor(false, true) && targetInRange(target, W2.Range))
                {
                    W2.Cast();
                }

            }
        }

        private static void doJayceInj(AIHeroClient target)
        {
            if (lockedTarg != null)
                target = lockedTarg;
            else
                lockedTarg = target;


            if (isHammer)
            {
                try
                {
                    castIgnite(target);

                    if (/*inMyTowerRange(posAfterHammer(target)) &&*/ E2.IsReady())
                        E2.Cast(target);

                    //If not in flash range  Q to get in it
                    if (Player.Distance(target) > 400 && targetInRange(target, 600f))
                        Q2.Cast(target);

                    if (!E2.IsReady() && !Q2.IsReady())
                        R2.Cast();
                    AIBaseClient tower = GameObjects.AllyTurrets.Where(tur => tur.Health > 0).OrderBy(tur => Player.Distance(tur)).FirstOrDefault();
                    var pos = getBestPosToHammer(target.Position);
                    if (tower != null && pos != null && Player.Distance(pos) < 400 && tower.Distance(target) < 1500)
                    {
                        Player.Spellbook.CastSpell(Player.GetSpellSlot("SummonerFlash"), getBestPosToHammer(target.Position));
                    }
                    Player.IssueOrder(GameObjectOrder.AttackUnit, target);
                }
                catch { }
            }
            else
            {
                if (E1.IsReady() && Q1.IsReady() && gotManaFor(true, false, true))
                {
                    PredictionOutput po = QEmp1.GetPrediction(target);
                    var dist = Player.Distance(po.UnitPosition);
                    if (dist <= E1.Range && getJayceEQDmg(target) < target.Health)
                    {
                        // if (Jayce.Config.Item("useExploit"))
                        //     doExploit(target);
                        // else
                        if (shootQE(po.CastPosition, dist > 550))
                            castedQon = target;
                    }
                    else
                    {
                        if (po.Hitchance >= HitChance.Medium && Player.Distance(po.UnitPosition) < (QEmp1.Range + target.BoundingRadius))
                        {
                            castQon = po.CastPosition;
                            castedQon = target;
                        }
                    }

                    // QEmp1.CastIfHitchanceEquals(target, Prediction.HitChance.HighHitchance);
                }
                else if (Q1.IsReady() && gotManaFor(true) && !E1.IsReady(1000))
                {
                    if (Q1.Cast(target.Position))
                        castedQon = target;
                }
                else if (W1.IsReady() && gotManaFor(false, true) && targetInRange(getClosestEnem(), 1000f))
                {
                    W1.Cast();
                }
            }
        }


        /*  private static Vector3 posAfterInj(AIBaseClient target)
          {
              Vector3 ve = getBestPosToHammer(target.Position);
              return posAfterHammer()
          }*/


        private static void doKillSteal()
        {
            try
            {
                if (rangQCDRem == 0 && rangECDRem == 0 && gotManaFor(true, false, true))
                {
                    List<AIHeroClient> deadEnes = GameObjects.EnemyHeroes.Where(ene => getJayceEQDmg(ene) > ene.Health && ene.IsValid && ene.Distance(Player.Position) < 1800).ToList();
                    foreach (var enem in deadEnes)
                    {
                        if (Player.Distance(enem) < 300)
                            continue;
                        if (QEmp1.GetPrediction(enem).Hitchance >= HitChance.Medium)
                        {
                            if (isHammer && R2.IsReady())
                            {
                                R2.Cast();
                            }
                            castQEPred(enem);
                        }
                    }
                }
                else if (rangQCDRem == 0 && gotManaFor(true))
                {
                    List<AIHeroClient> deadEnes = GameObjects.EnemyHeroes.Where(ene => getJayceQDmg(ene) > ene.Health && ene.IsValid && ene.Distance(Player.Position) < 1200).ToList();
                    foreach (var enem in deadEnes)
                    {
                        if (Q1.GetPrediction(enem).Hitchance >= HitChance.Medium)
                        {
                            if (isHammer && R2.IsReady())
                            {
                                R2.Cast();
                            }
                            castQPred(enem);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }

        }

        private static void castQEPred(AIHeroClient target)
        {
            if (isHammer || target == null)
                return;
            PredictionOutput po = QEmp1.GetPrediction(target);
            var dist = Player.Distance(po.UnitPosition);
            if (po.Hitchance >= HitChance.Low && dist < (QEmp1.Range + target.BoundingRadius))
            {
                // if()
                //doExploit(target);
                // else
                // {
                if (shootQE(po.CastPosition, dist > 550))
                    castedQon = target;
                // }
            }
            else if (po.Hitchance == HitChance.Collision && Jayce.Config["extra"].GetValue<MenuBool>("useMunions").Enabled)
            {
                AIBaseClient fistCol = po.CollisionObjects.OrderBy(unit => unit.Distance(Player.Position)).First();
                if (fistCol.Distance(po.UnitPosition) < (180 - fistCol.BoundingRadius / 2) && fistCol.Distance(target.Position) < (180 - fistCol.BoundingRadius / 2))
                {
                    shootQE(po.CastPosition);
                }
            }
        }

        private static void castQPred(AIHeroClient target)
        {
            if (isHammer)
                return;
            PredictionOutput po = Q1.GetPrediction(target);
            if (po.Hitchance == HitChance.Collision && Jayce.Config["extra"].GetValue<MenuBool>("useMunions").Enabled)
            {
                AIBaseClient fistCol = po.CollisionObjects.OrderBy(unit => unit.Distance(Player.Position)).FirstOrDefault();
                if (fistCol != null && fistCol.Distance(po.UnitPosition) < (180 - fistCol.BoundingRadius / 2) && fistCol.Distance(target.Position) < (100 - fistCol.BoundingRadius / 2))
                {
                    if (Q1.Cast(po.CastPosition))
                        castedQon = target;
                }

            }
            else
            {
                Q1.Cast(target);
            }
        }

        private static Vector3 getBestPosToHammer(Vector3 target)
        {
            AIBaseClient tower = GameObjects.Turrets.Where(tur => tur.IsAlly && tur.Health > 0).OrderBy(tur => Player.Distance(tur)).First();
            return target + Vector3.Normalize(tower.Position - target) * (-120);
        }

        private static Vector3 posAfterHammer(AIBaseClient target)
        {
            return getBestPosToHammer(target.Position) + Vector3.Normalize(getBestPosToHammer(target.Position) - Player.Position) * 600;
        }

        private static AIHeroClient getClosestEnem()
        {
            return GameObjects.Heroes.Where(ene => ene.IsEnemy && ene.IsValidTarget()).OrderBy(ene => Player.Distance(ene)).First();
        }

        private static float getBestRange()
        {
            float range = 0;
            if (!isHammer)
            {
                if (Q1.IsReady() && E1.IsReady() && gotManaFor(true, false, true))
                {
                    range = 1750;
                }
                else if (Q1.IsReady() && gotManaFor(true))
                {
                    range = 1150;
                }
                else
                {
                    range = 500;
                }
            }
            else
            {
                if (Q1.IsReady() && gotManaFor(true))
                {
                    range = 600;
                }
                else
                {
                    range = 300;
                }
            }
            return range + 50;
        }




        private static bool shootQE(Vector3 pos, bool man = false)
        {
            try
            {
                if (isHammer && R2.IsReady())
                    R2.Cast();
                if (!E1.IsReady() || !Q1.IsReady() || isHammer)
                    return false;

                if (Jayce.Config["extra"].GetValue<MenuBool>("packets").Enabled)
                {
                    packetCastQ(pos.ToVector2());
                    packetCastE(getParalelVec(pos));
                }
                else
                {
                    Vector3 bPos = Player.Position - Vector3.Normalize(pos - Player.Position) * 50;

                    Player.IssueOrder(GameObjectOrder.MoveTo, bPos);
                    Q1.Cast(pos);
                    if (man)
                        E1.Cast(getParalelVec(pos));
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
            return true;
        }

        private static bool shouldIKnockDatMadaFaka(AIHeroClient target)
        {
            //if (useSmartKnock(target) && R2.IsReady() && target.CombatType == GameObjectCombatType.Melee)
            // {
            //  return true;
            // }
            float damageOn = getJayceEHamDmg(target);

            if (damageOn > target.Health * 0.9f)
            {
                return true;
            }
            if (((Player.Health / Player.MaxHealth) < 0.15f) /*&& target.CombatType == GameObjectCombatType.Melee*/)
            {
                return true;
            }
            Vector3 posAfter = target.Position + Vector3.Normalize(target.Position - Player.Position) * 450;
            if (inMyTowerRange(posAfter))
            {
                return true;
            }

            return false;
        }

        private static bool useSmartKnock(AIHeroClient target)
        {
            float trueAARange = Player.BoundingRadius + target.AttackRange;
            float trueERange = target.BoundingRadius + E2.Range;

            float dist = Player.Distance(target);
            Vector2 movePos = new Vector2();
            if (target.IsMoving)
            {
                Vector2 tpos = target.Position.ToVector2();
                Vector2 path = target.Path[0].ToVector2() - tpos;
                path.Normalize();
                movePos = tpos + (path * 100);
            }
            float targ_ms = (target.IsMoving && Player.Distance(movePos) < dist) ? target.MoveSpeed : 0;
            float msDif = (Player.MoveSpeed * 0.7f - targ_ms) == 0 ? 0.0001f : (targ_ms - Player.MoveSpeed * 0.7f);
            float timeToReach = (dist - trueAARange) / msDif;
            if (dist > trueAARange && dist < trueERange && target.IsMoving)
            {
                if (timeToReach > 1.7f || timeToReach < 0.0f)
                {
                    return true;
                }
            }
            return false;
        }

        private static bool inMyTowerRange(Vector3 pos)
        {
            return GameObjects.AllyTurrets.Where(tur => tur.Health > 0).Any(tur => pos.Distance(tur.Position) < (850 + Player.BoundingRadius));
        }

        private static void castEonSpell(GameObject mis)
        {
            if (isHammer || !E1.IsReady())
                return;
            if (Player.Distance(myCastedQ.Position) < 250)
            {
                E1.Cast(getParalelVec(mis.Position));
            }

        }


        private static bool targetInRange(AIBaseClient target, float range)
        {
            if (target == null)
                return false;
            float dist2 = Vector2.DistanceSquared(target.Position.ToVector2(), Player.Position.ToVector2());
            float range2 = range * range + target.BoundingRadius * target.BoundingRadius;
            return dist2 < range2;
        }

        private static void checkForm()
        {
            isHammer = !Qdata.SData.Name.ToLower().Contains("jayceshockblast");
        }


        private static bool gotSpeedBuff()//jaycehypercharge
        {
            return Player.Buffs.Any(bi => bi.Name.ToLower().Contains("jaycehypercharge"));
        }

        private static Vector2 getParalelVec(Vector3 pos)
        {
            if (Jayce.Config["combo"].GetValue<MenuBool>("parlelE").Enabled)
            {
                Random rnd = new Random();
                int neg = rnd.Next(0, 1);
                int away = Jayce.Config["combo"].GetValue<MenuSlider>("eAway").Value;
                away = (neg == 1) ? away : -away;
                var v2 = Vector3.Normalize(pos - Player.Position) * away;
                var bom = new Vector2(v2.Y, -v2.X);
                return Player.Position.ToVector2() + bom;
            }
            else
            {
                var dpos = Player.Distance(pos);
                var v2 = Vector3.Normalize(pos - Player.Position) * ((dpos < 300) ? dpos + 10 : 300);
                var bom = new Vector2(v2.X, v2.Y);
                return Player.Position.ToVector2() + bom;
            }
        }

        //Need to fix!!
        private static bool gotManaFor(bool q = false, bool w = false, bool e = false)
        {
            float manaNeeded = 0;
            if (q)
                manaNeeded += Qdata.ManaCost;
            if (w)
                manaNeeded += Wdata.ManaCost;
            if (e)
                manaNeeded += Edata.ManaCost;
            // Console.WriteLine("Mana: " + manaNeeded);
            return manaNeeded <= Player.Mana;
        }

        private static float calcRealCD(float time)
        {
            return time + (time * Player.PercentCooldownMod);
        }

        private static void processCDs()
        {
            hamQCDRem = ((hamQCD - Game.Time) > 0) ? (hamQCD - Game.Time) : 0;
            hamWCDRem = ((hamWCD - Game.Time) > 0) ? (hamWCD - Game.Time) : 0;
            hamECDRem = ((hamECD - Game.Time) > 0) ? (hamECD - Game.Time) : 0;

            rangQCDRem = ((rangQCD - Game.Time) > 0) ? (rangQCD - Game.Time) : 0;
            rangWCDRem = ((rangWCD - Game.Time) > 0) ? (rangWCD - Game.Time) : 0;
            rangECDRem = ((rangECD - Game.Time) > 0) ? (rangECD - Game.Time) : 0;
        }

        private static void getCDs(AIBaseClientProcessSpellCastEventArgs spell)
        {
            try
            {
                //Console.WriteLine(spell.SData.Name + ": " + Q2.Level);

                if (spell.SData.Name == "JayceToTheSkies")
                    hamQCD = Game.Time + calcRealCD(hamTrueQcd[Q2.Level - 1]);
                if (spell.SData.Name == "JayceStaticField")
                    hamWCD = Game.Time + calcRealCD(hamTrueWcd[W2.Level - 1]);
                if (spell.SData.Name == "JayceThunderingBlow")
                    hamECD = Game.Time + calcRealCD(hamTrueEcd[E2.Level - 1]);

                if (spell.SData.Name.ToLower() == "jayceshockblast")
                    rangQCD = Game.Time + calcRealCD(rangTrueQcd[Q1.Level - 1]);
                if (spell.SData.Name.ToLower() == "jaycehypercharge")
                    rangWCD = Game.Time + calcRealCD(rangTrueWcd[W1.Level - 1]);
                if (spell.SData.Name.ToLower() == "jayceaccelerationgate")
                    rangECD = Game.Time + calcRealCD(rangTrueEcd[E1.Level - 1]);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private static void drawCD()
        {
            var pScreen = Drawing.WorldToScreen(Player.Position);

            // Drawing.DrawText(Drawing.WorldToScreen(Player.Position)[0], Drawing.WorldToScreen(Player.Position)[1], System.Drawing.Color.Green, "Q: wdeawd ");
            pScreen[0] -= 20;

            if (isHammer)
            {
                if (rangQCDRem == 0)
                    Drawing.DrawText(pScreen.X - 60, pScreen.Y, Color.Green, "Q: Rdy");
                else
                    Drawing.DrawText(pScreen.X - 60, pScreen.Y, Color.Red, "Q: " + rangQCDRem.ToString("0.0"));

                if (rangWCDRem == 0)
                    Drawing.DrawText(pScreen.X, pScreen.Y, Color.Green, "W: Rdy");
                else
                    Drawing.DrawText(pScreen.X, pScreen.Y, Color.Red, "W: " + rangWCDRem.ToString("0.0"));

                if (rangECDRem == 0)
                    Drawing.DrawText(pScreen.X + 60, pScreen.Y, Color.Green, "E: Rdy");
                else
                    Drawing.DrawText(pScreen.X + 60, pScreen.Y, Color.Red, "E: " + rangECDRem.ToString("0.0"));
            }
            else
            {
                // pScreen.Y += 30;
                if (hamQCDRem == 0)
                    Drawing.DrawText(pScreen.X - 60, pScreen.Y, Color.Green, "Q: Rdy");
                else
                    Drawing.DrawText(pScreen.X - 60, pScreen.Y, Color.Red, "Q: " + hamQCDRem.ToString("0.0"));

                if (hamWCDRem == 0)
                    Drawing.DrawText(pScreen.X, pScreen.Y, Color.Green, "W: Rdy");
                else
                    Drawing.DrawText(pScreen.X, pScreen.Y, Color.Red, "W: " + hamWCDRem.ToString("0.0"));

                if (hamECDRem == 0)
                    Drawing.DrawText(pScreen.X + 60, pScreen.Y, Color.Green, "E: Rdy");
                else
                    Drawing.DrawText(pScreen.X + 60, pScreen.Y, Color.Red, "E: " + hamECDRem.ToString("0.0"));
            }
        }


        private static void packetCastQ(Vector2 pos)
        {
            
            Q1.Cast(pos);
            //Packet.C2S.Cast.Encoded(new Packet.C2S.Cast.Struct(0, SpellSlot.Q, Player.NetworkId, pos.X, pos.Y, Player.Position.X, Player.Position.Y)).Send();
        }

        private static void packetCastE(Vector2 pos)
        {
            
                E1.Cast(pos);
            //Packet.C2S.Cast.Encoded(new Packet.C2S.Cast.Struct(0, SpellSlot.E, Player.NetworkId, pos.X, pos.Y, Player.Position.X, Player.Position.Y)).Send();
        }

        private static void knockAway(AIBaseClient target)
        {
            if (!targetInRange(target, 270) || hamECDRem != 0 || E1.Level == 0)
                return;

            if (!isHammer && R2.IsReady())
                R1.Cast();
            if (isHammer && E2.IsReady() && targetInRange(target, 260))
                E2.Cast(target);

        }

        private static bool hammerWillKill(AIBaseClient target)
        {
            if (!Jayce.Config["combo"].GetValue<MenuBool>("hammerKill").Enabled || target == null)
                return false;
            float damage = (float)Player.GetAutoAttackDamage(target) + 50;
            damage += getJayceEHamDmg(target);
            damage += getJayceQHamDmg(target);

            return (target.Health < damage);
        }


        private static float getJayceFullComoDmg(AIBaseClient target)
        {
            if (target == null)
                return 0f;
            float dmg = 0;
            //Ranged
            if (!isHammer || R1.IsReady())
            {
                if (rangECDRem == 0 && rangQCDRem == 0 && Q1.Level != 0 && E1.Level != 0)
                {
                    dmg += getJayceEQDmg(target);
                }
                else if (rangQCDRem == 0 && Q1.Level != 0)
                {
                    dmg += getJayceQDmg(target);
                }
                float hyperMulti = W1.Level * 0.15f + 0.7f;
                if (rangWCDRem == 0 && W1.Level != 0)
                {
                    dmg += getJayceAADmg(target) * 3 * hyperMulti;
                }
            }
            //Hamer
            if (isHammer || R1.IsReady())
            {
                if (hamECDRem == 0 && E2.Level != 0)
                {
                    dmg += getJayceEHamDmg(target);
                }
                if (hamQCDRem == 0 && Q2.Level != 0)
                {
                    dmg += getJayceQHamDmg(target);
                }
            }
            return dmg;
        }

        private static float getJayceAADmg(AIBaseClient target)
        {
            return (float)Player.GetAutoAttackDamage(target);

        }

        private static float getJayceEQDmg(AIBaseClient target)
        {
            return
                (float)
                    Player.CalculateDamage(target, DamageType.Physical,
                        (7 + (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level * 77)) +
                        (1.68 * ObjectManager.Player.FlatPhysicalDamageMod));


        }

        private static float getJayceQDmg(AIBaseClient target)
        {
            return (float)Player.CalculateDamage(target, DamageType.Physical,
                                    (5 + (ObjectManager.Player.Spellbook.GetSpell(SpellSlot.Q).Level * 55)) +
                                    (1.2 * ObjectManager.Player.FlatPhysicalDamageMod));
        }

        private static float getJayceEHamDmg(AIBaseClient target)
        {
            if (target == null)
                return 0f;
            double percentage = 5 + (3 * Player.Spellbook.GetSpell(SpellSlot.E).Level);
            return (float)Player.CalculateDamage(target, DamageType.Magical,
                    ((target.MaxHealth / 100) * percentage) + (ObjectManager.Player.FlatPhysicalDamageMod));
        }

        private static float getJayceQHamDmg(AIBaseClient target)
        {
            return (float)Player.CalculateDamage(target, DamageType.Physical,
                                (-25 + (Player.Spellbook.GetSpell(SpellSlot.Q).Level * 45)) +
                                (1.0 * Player.FlatPhysicalDamageMod));
        }

        private static void castIgnite(AIHeroClient target)
        {
            if (targetInRange(target, 600) && (target.Health / target.MaxHealth) * 100 < 25)
                sumItems.castIgnite(target);
        }

        private static void castOmen(AIHeroClient target)
        {
            if (Player.Distance(target) < 430)
                sumItems.cast(SummonerItems.ItemIds.Omen);
        }

        private static void activateMura()
        {
            if (Player.Buffs.Count(buf => buf.Name == "Muramana") == 0)
                sumItems.cast(SummonerItems.ItemIds.Muramana);
        }

        private static void deActivateMura()
        {
            if (Player.Buffs.Count(buf => buf.Name == "Muramana") != 0)
                sumItems.cast(SummonerItems.ItemIds.Muramana);
        }

    }

    class SummonerItems
    {
        private AIHeroClient player;
        private Spellbook sumBook;
        private SpellSlot ignite;
        private SpellSlot smite;


        public enum ItemIds
        {
            //MuramanaDe = 3043,
            Muramana = 3042,
            Tiamat = 3077,
            Hydra = 3074,
            MercScim = 3139,
            Hextech = 3146,
            SwordOD = 3131,
            Ghostblade = 3142,
            BotRK = 3153,
            Cutlass = 3144,

            Omen = 3143
        }

        public SummonerItems(AIHeroClient myHero)
        {
            player = myHero;
            sumBook = player.Spellbook;
            ignite = player.GetSpellSlot("summonerdot");
            smite = player.GetSpellSlot("SummonerSmite");
        }

        public void castIgnite(AIHeroClient target)
        {
            if (ignite != SpellSlot.Unknown && sumBook.CanUseSpell(ignite) == SpellState.Ready)
                sumBook.CastSpell(ignite, target);
        }

        public void castSmite(AIHeroClient target)
        {
            if (smite != SpellSlot.Unknown && sumBook.CanUseSpell(smite) == SpellState.Ready)
                sumBook.CastSpell(smite, target);
        }

        public void cast(ItemIds item)
        {
            var itemId = (int)item;
            if (Items.CanUseItem(ObjectManager.Player, itemId))
                Items.UseItem(ObjectManager.Player, itemId);
        }

        public void cast(ItemIds item, Vector3 target)
        {
            var itemId = (int)item;
            if (Items.CanUseItem(ObjectManager.Player, itemId))
                player.Spellbook.CastSpell(getInvSlot(itemId).SpellSlot, target);

        }

        public void cast(ItemIds item, AIBaseClient target)
        {
            var itemId = (int)item;
            if (Items.CanUseItem(ObjectManager.Player, itemId))
                Items.UseItem(ObjectManager.Player, itemId, target);
        }

        private InventorySlot getInvSlot(int id)
        {
            return player.InventoryItems.FirstOrDefault(iSlot => (int)iSlot.Id == id);
        }
    }

    class HpBarIndicator
    {

        public static SharpDX.Direct3D9.Device dxDevice = Drawing.Direct3DDevice;
        public static SharpDX.Direct3D9.Line dxLine;
        public static SharpDX.Direct3D9.Sprite sprite;



        public AIHeroClient unit { get; set; }

        public float width = 104;

        public float hight = 9;




        public HpBarIndicator()
        {
            dxLine = new Line(dxDevice) { Width = 9 };
            sprite = new Sprite(dxDevice);

            Drawing.OnPreReset += DrawingOnOnPreReset;
            Drawing.OnPostReset += DrawingOnOnPostReset;
            AppDomain.CurrentDomain.DomainUnload += CurrentDomainOnDomainUnload;
            AppDomain.CurrentDomain.ProcessExit += CurrentDomainOnDomainUnload;

        }


        private static void CurrentDomainOnDomainUnload(object sender, EventArgs eventArgs)
        {
            sprite.Dispose();
            dxLine.Dispose();
        }

        private static void DrawingOnOnPostReset(EventArgs args)
        {
            dxLine.OnResetDevice();
            sprite.OnResetDevice();
        }

        private static void DrawingOnOnPreReset(EventArgs args)
        {
            sprite.OnLostDevice();
            dxLine.OnLostDevice();
        }

        public void drawAwsomee()
        {
        }

        private Vector2 Offset
        {
            get
            {
                if (unit != null)
                {
                    return unit.IsAlly ? new Vector2(34, 9) : new Vector2(10, 20);
                }

                return new Vector2();
            }
        }

        public Vector2 startPosition
        {

            get { return new Vector2(unit.HPBarPosition.X + Offset.X, unit.HPBarPosition.Y + Offset.Y); }
        }


        private float getHpProc(float dmg = 0)
        {
            float health = ((unit.Health - dmg) > 0) ? (unit.Health - dmg) : 0;
            return (health / unit.MaxHealth);
        }

        private Vector2 getHpPosAfterDmg(float dmg)
        {
            float w = getHpProc(dmg) * width;
            return new Vector2(startPosition.X + w, startPosition.Y);
        }

        public void drawDmg(float dmg, System.Drawing.Color color)
        {
            var hpPosNow = getHpPosAfterDmg(0);
            var hpPosAfter = getHpPosAfterDmg(dmg);

            fillHPBar(hpPosNow, hpPosAfter, color);
            //fillHPBar((int)(hpPosNow.X - startPosition.X), (int)(hpPosAfter.X- startPosition.X), color);
        }

        private void fillHPBar(int to, int from, System.Drawing.Color color)
        {
            Vector2 sPos = startPosition;

            for (int i = from; i < to; i++)
            {
                Drawing.DrawLine(sPos.X + i, sPos.Y, sPos.X + i, sPos.Y + 9, 1, color);
            }
        }

        private void fillHPBar(Vector2 from, Vector2 to, System.Drawing.Color color)
        {
            dxLine.Begin();

            dxLine.Draw(new[]
                                    {
                                        new Vector2((int)from.X, (int)from.Y + 4f),
                                        new Vector2( (int)to.X, (int)to.Y + 4f)
                                    }, new ColorBGRA(255, 255, 00, 90));
            // Vector2 sPos = startPosition;
            //Drawing.DrawLine((int)from.X, (int)from.Y + 9f, (int)to.X, (int)to.Y + 9f, 9f, color);

            dxLine.End();
        }

    }

}
