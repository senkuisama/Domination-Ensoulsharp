using System;
using System.Collections.Generic;
using Color = System.Drawing.Color;
using System.Linq;
using EnsoulSharp;
using SharpDX;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using EnsoulSharp.SDK.Utility;

using DominationAIO.Champions.Helper;

namespace DaoHungAIO.Champions
{
    public static class Viktor
    {
        
        private const string CHAMP_NAME = "Viktor";
        private static readonly AIHeroClient player = ObjectManager.Player;

        public static List<Spell> SpellList = new List<Spell>();
        // Spells
        private static Spell Q, W, E, R;
        private static readonly int maxRangeE = 1225;
        private static readonly int lengthE = 700;
        private static readonly int speedE = 1050;
        private static readonly int rangeE = 525;
        private static int lasttick = 0;
        private static SharpDX.Vector3 GapCloserPos;
        private static bool AttacksEnabled
        {
            get
            {
                if (keyLinks["comboActive"].GetValue<MenuKeyBind>().Active)
                {
                    return ((!Q.IsReady() || player.Mana < Q.Instance.ManaCost) && (!E.IsReady() || player.Mana < E.Instance.ManaCost) && (!boolLinks["qAuto"].GetValue<MenuBool>().Enabled || player.HasBuff("viktorpowertransferreturn")));
                }
                else if (keyLinks["harassActive"].GetValue<MenuKeyBind>().Active)
                {
                    return ((!Q.IsReady() || player.Mana < Q.Instance.ManaCost) && (!E.IsReady() || player.Mana < E.Instance.ManaCost));
                }
                return true;
            }
        }
        // Menu
        public static Menu menu;

        // Menu links
        public static Dictionary<string, MenuBool> boolLinks = new Dictionary<string, MenuBool>();
        public static Dictionary<string, MenuColor> circleLinks = new Dictionary<string, MenuColor>();
        public static Dictionary<string, MenuKeyBind> keyLinks = new Dictionary<string, MenuKeyBind>();
        public static Dictionary<string, MenuSlider> sliderLinks = new Dictionary<string, MenuSlider>();
        public static Dictionary<string, MenuList> stringLinks = new Dictionary<string, MenuList>();


        /*private static void OrbwalkerOnBeforeAttack(
    Object sender,
    BeforeAttackEventArgs args
)
        {
            if (args.Type == OrbwalkerType.BeforeAttack) {
                if (args.Target.Type == GameObjectType.AIHeroClient)
                {
                    args.Process = AttacksEnabled;
                }
                else
                    args.Process = true;
            }
            if(args.Type == OrbwalkerType.NonKillableMinion)
            {
                QLastHit((AIBaseClient)args.Target);
            }
            

        }*/
        /*private static void OrbwalkerOnBeforeAttack()
        {
            if (FunnySlayerCommon.OnAction.BeforeAA)
            {
                if (args.Target.Type == GameObjectType.AIHeroClient)
                {
                    args.Process = AttacksEnabled;
                }
                else
                    args.Process = true;
            }
            if (FunnySlayerCommon.OnAction.OnNonKillableMinion)
            {
                QLastHit((AIBaseClient)args.Target);
            }


        }*/

        public static void LoadViktor()
        {
            // Champ validation
          



            // Define spells
            Q = new Spell(SpellSlot.Q, 600);
            W = new Spell(SpellSlot.W, 700);
            E = new Spell(SpellSlot.E, rangeE);
            R = new Spell(SpellSlot.R, 700);
            Spell Emax = new Spell(SpellSlot.E, 1025);
            SpellList.Add(Q);
            SpellList.Add(W);
            SpellList.Add(E);
            SpellList.Add(R);
            SpellList.Add(Emax);
            // Finetune spells
            Q.SetTargetted(0.25f, 2000);
            W.SetSkillshot(1.4f, 300, float.MaxValue, false, SpellType.Circle);
            E.SetSkillshot(0, 55, speedE, false, SpellType.Line);
            R.SetSkillshot(0.25f, 300f, float.MaxValue, false, SpellType.Circle);

            // Create menu
            SetupMenu();

            // Register events
            Game.OnUpdate += Game_OnGameUpdate;
            Drawing.OnDraw += Drawing_OnDraw;
            AntiGapcloser.OnGapcloser += AntiGapcloser_OnEnemyGapcloser;
            //Orbwalker.OnAction += OrbwalkerOnBeforeAttack;
            Interrupter.OnInterrupterSpell += Interrupter2_OnInterruptableTarget;
        }
  
        /*private static void QLastHit(AIBaseClient minion)
        {
            bool castQ = ((keyLinks["waveUseQLH"].GetValue<MenuKeyBind>().Active) || boolLinks["waveUseQ"].GetValue<MenuBool>().Enabled && keyLinks["waveActive"].GetValue<MenuKeyBind>().Active);
            if (castQ)
            {
                var distance = player.Distance(minion);
                var t = 250 + (int)distance / 2;
                var predHealth = HealthPrediction.GetPrediction(minion, t, 0);
                // Console.WriteLine(" Distance: " + distance + " timer : " + t + " health: " + predHealth);
                if (predHealth > 0 && Q.can(minion))
                {
                    Q.Cast(minion);
                }
            }
        }*/
        private static void Game_OnGameUpdate(EventArgs args)
        {
            // Combo
            if (keyLinks["comboActive"].GetValue<MenuKeyBind>().Active)
                OnCombo();
            // Harass�
            if (keyLinks["harassActive"].GetValue<MenuKeyBind>().Active)
                OnHarass();
            // WaveClear
            if (keyLinks["waveActive"].GetValue<MenuKeyBind>().Active)
                OnWaveClear();

            if (keyLinks["jungleActive"].GetValue<MenuKeyBind>().Active)
                OnJungleClear();

            if (keyLinks["FleeActive"].GetValue<MenuKeyBind>().Active)
                Flee();

            if (keyLinks["forceR"].GetValue<MenuKeyBind>().Active)
            {
                if (R.IsReady())
                {
                    List<AIHeroClient> ignoredchamps = new List<AIHeroClient>();

                    foreach (var hero in HeroManager.Enemies)
                    {
                        if (!boolLinks["RU" + hero.CharacterName].GetValue<MenuBool>().Enabled)
                        {
                            ignoredchamps.Add(hero);
                        }
                    }
                    AIHeroClient RTarget = TargetSelector.GetTarget(R.Range, DamageType.Magical);
                    if (RTarget.IsValidTarget())
                    {
                        R.Cast(RTarget);
                    }
                }

            }
            // Ultimate follow
            if (R.Instance.Name != "ViktorChaosStorm" && boolLinks["AutoFollowR"].GetValue<MenuBool>().Enabled && Environment.TickCount - lasttick > 0)
            {
                var stormT = TargetSelector.GetTarget(1100, DamageType.Magical);
                if (stormT != null)
                {
                    R.Cast(stormT.Position);
                    lasttick = Environment.TickCount + 500;
                }
            }
        }

        private static void PredictCastE(AIHeroClient target)
        {
            // Helpers
            bool inRange = SharpDX.Vector2.DistanceSquared(target.Position.ToVector2(), player.Position.ToVector2()) < E.Range * E.Range;
            FSpred.Prediction.PredictionOutput prediction;
            bool spellCasted = false;

            // Positions
            SharpDX.Vector3 pos1, pos2;

            // Champs
            var nearChamps = (from champ in ObjectManager.Get<AIHeroClient>() where champ.IsValidTarget(maxRangeE) && target != champ select champ).ToList();
            var innerChamps = new List<AIHeroClient>();
            var outerChamps = new List<AIHeroClient>();
            foreach (var champ in nearChamps)
            {
                if (SharpDX.Vector2.DistanceSquared(champ.Position.ToVector2(), player.Position.ToVector2()) < E.Range * E.Range)
                    innerChamps.Add(champ);
                else
                    outerChamps.Add(champ);
            }

            // Minions
            var nearMinions = GameObjects.GetMinions(player.Position, maxRangeE);
            var innerMinions = new List<AIBaseClient>();
            var outerMinions = new List<AIBaseClient>();
            foreach (var minion in nearMinions)
            {
                if (SharpDX.Vector2.DistanceSquared(minion.Position.ToVector2(), player.Position.ToVector2()) < E.Range * E.Range)
                    innerMinions.Add(minion);
                else
                    outerMinions.Add(minion);
            }

            // Main target in close range
            if (inRange)
            {
                // Get prediction reduced speed, adjusted sourcePosition
                E.Speed = speedE * 0.9f;
                E.From = target.Position + (SharpDX.Vector3.Normalize(player.Position - target.Position) * (lengthE * 0.1f));
                prediction = FSpred.Prediction.Prediction.GetPrediction(E, target);// E.GetPrediction(target);
                E.From = player.Position;

                // Prediction in range, go on
                if (prediction.CastPosition.Distance(player.Position) < E.Range)
                    pos1 = prediction.CastPosition;
                // Prediction not in range, use exact position
                else
                {
                    pos1 = target.Position;
                    E.Speed = speedE;
                }

                // Set new sourcePosition
                E.From = pos1;
                E.RangeCheckFrom = pos1;

                // Set new range
                E.Range = lengthE;

                // Get next target
                if (nearChamps.Count > 0)
                {
                    // Get best champion around
                    var closeToPrediction = new List<AIHeroClient>();
                    foreach (var enemy in nearChamps)
                    {
                        // Get prediction
                        prediction = FSpred.Prediction.Prediction.GetPrediction(E, enemy);
                        // Validate target
                        if (prediction.Hitchance >= FSpred.Prediction.HitChance.High && SharpDX.Vector2.DistanceSquared(pos1.ToVector2(), prediction.CastPosition.ToVector2()) < (E.Range * E.Range) * 0.8)
                            closeToPrediction.Add(enemy);
                    }

                    // Champ found
                    if (closeToPrediction.Count > 0)
                    {
                        // Sort table by health DEC
                        if (closeToPrediction.Count > 1)
                            closeToPrediction.Sort((enemy1, enemy2) => enemy2.Health.CompareTo(enemy1.Health));

                        // Set destination
                        prediction = FSpred.Prediction.Prediction.GetPrediction(E, closeToPrediction[0]);
                        pos2 = prediction.CastPosition;

                        // Cast spell
                        CastE(pos1, pos2);
                        spellCasted = true;
                    }
                }

                // Spell not casted
                if (!spellCasted)
                {
                    CastE(pos1, FSpred.Prediction.Prediction.GetPrediction(E, target).CastPosition);
                }

                // Reset spell
                E.Speed = speedE;
                E.Range = rangeE;
                E.From = player.Position;
                E.RangeCheckFrom = player.Position;
            }

            // Main target in extended range
            else
            {
                // Radius of the start point to search enemies in
                float startPointRadius = 150;

                // Get initial start point at the border of cast radius
                SharpDX.Vector3 startPoint = player.Position + SharpDX.Vector3.Normalize(target.Position - player.Position) * rangeE;

                // Potential start from postitions
                var targets = (from champ in nearChamps where SharpDX.Vector2.DistanceSquared(champ.Position.ToVector2(), startPoint.ToVector2()) < startPointRadius * startPointRadius && SharpDX.Vector2.DistanceSquared(player.Position.ToVector2(), champ.Position.ToVector2()) < rangeE * rangeE select champ).ToList();
                if (targets.Count > 0)
                {
                    // Sort table by health DEC
                    if (targets.Count > 1)
                        targets.Sort((enemy1, enemy2) => enemy2.Health.CompareTo(enemy1.Health));

                    // Set target
                    pos1 = targets[0].Position;
                }
                else
                {
                    var minionTargets = (from minion in nearMinions where SharpDX.Vector2.DistanceSquared(minion.Position.ToVector2(), startPoint.ToVector2()) < startPointRadius * startPointRadius && SharpDX.Vector2.DistanceSquared(player.Position.ToVector2(), minion.Position.ToVector2()) < rangeE * rangeE select minion).ToList();
                    if (minionTargets.Count > 0)
                    {
                        // Sort table by health DEC
                        if (minionTargets.Count > 1)
                            minionTargets.Sort((enemy1, enemy2) => enemy2.Health.CompareTo(enemy1.Health));

                        // Set target
                        pos1 = minionTargets[0].Position;
                    }
                    else
                        // Just the regular, calculated start pos
                        pos1 = startPoint;
                }

                // Predict target position
                E.From = pos1;
                E.Range = lengthE;
                E.RangeCheckFrom = pos1;
                prediction = FSpred.Prediction.Prediction.GetPrediction(E, target);

                // Cast the E
                if (prediction.Hitchance >= FSpred.Prediction.HitChance.High)
                    CastE(pos1, prediction.CastPosition);

                // Reset spell
                E.Range = rangeE;
                E.From = player.Position;
                E.RangeCheckFrom = player.Position;
            }

        }

        private static float MathFloat = 0;
        private static void FSPredictionECast(AIBaseClient target)
        {
            if (target == null || !target.IsValidTarget())
                return;

            E.Speed = speedE;
            E.Range = rangeE;
            E.From = player.Position;
            E.RangeCheckFrom = player.Position;

            var SkillShotE = new Spell(SpellSlot.Unknown, lengthE);
            SkillShotE.SetSkillshot(0.55f, 55f, 1500f, false, SpellType.Line);

            var TempVector = target.Position - ObjectManager.Player.Position;
            var NewVector = Vector3.Normalize(TempVector);
            var tempspell = new Spell(SpellSlot.Unknown, maxRangeE);
            tempspell.SetSkillshot(1f, 55f, 1500f, false, SpellType.Line);
            var temppred = FSpred.Prediction.Prediction.GetPrediction(tempspell, target);

            if (target.DistanceToPlayer() <= 525)
            {
                //TryFindPos1                
                if(temppred.Hitchance >= FSpred.Prediction.HitChance.High)
                {
                    var ThePred = FSpred.Prediction.Prediction.GetPrediction(SkillShotE, target);
                    var posE1 = ThePred.CastPosition.Extend(temppred.CastPosition, -200);
                    if (ThePred.Hitchance >= FSpred.Prediction.HitChance.High)
                    {
                        UpdateThePos(posE1, temppred, ThePred);
                    }
                    else
                    {
                        posE1 = target.Position.Extend(temppred.CastPosition, -350);
                        UpdateThePos(target, posE1, temppred);
                    }
                }
                else
                {                    
                    var Pos1 = ObjectManager.Player.Position + NewVector * 200;

                    if(Pos1.DistanceToPlayer() <= 525)
                    {
                        UpdateThePos(Pos1);
                    }
                }
            }
            else
            {
                var epred = FSpred.Prediction.Prediction.GetPrediction(E, target);
                int i = 300;

                float alpha = FsEpred.Value * (float)Math.PI / 180;

                if (AutoCalculator.Enabled)
                {
                    var simeplepos = (180 / Math.PI);
                    var first = target.Position - ObjectManager.Player.Position;
                    var second = temppred.CastPosition - ObjectManager.Player.Position;
                    var a = 25;
                    var third = target.Position.Extend(temppred.CastPosition, a);

                    while (third.DistanceToPlayer() > 525)
                    {
                        a += 25;
                        third = target.Position.Extend(temppred.CastPosition, a);

                        if (a > rangeE)
                            break;
                    }

                    var fourth = third - ObjectManager.Player.Position;

                    var letcalculator = (first.X * fourth.X + first.Y * fourth.Y) 
                        / ((Math.Sqrt(first.X * first.X + first.Y * first.Y)) * (Math.Sqrt(fourth.X * fourth.X + fourth.Y * fourth.Y)));

                    var mathcompleted = simeplepos * Math.Acos(letcalculator);

                    if(mathcompleted != double.NaN)
                    {
                        alpha = (float)(mathcompleted) * (float)Math.PI / 180;
                    }

                    MathFloat = Convert.ToInt32(mathcompleted);
                }

                while (epred.Hitchance < FSpred.Prediction.HitChance.High)
                {
                    var vv = ObjectManager.Player.Position.Extend(target.Position, i);

                    var cp = ObjectManager.Player.Position.ToVector2() +
                                 (vv.ToVector2() - ObjectManager.Player.Position.ToVector2()).Rotated
                                     (1 * alpha);

                    var cp2 = ObjectManager.Player.Position.ToVector2() +
                                 (vv.ToVector2() - ObjectManager.Player.Position.ToVector2()).Rotated
                                     (-1 * alpha);
                    int d = 1;
                    if (temppred.Hitchance >= FSpred.Prediction.HitChance.High)
                    {
                        if (target.Position.Extend(temppred.CastPosition, -350).Distance(cp2) <= target.Position.Extend(temppred.CastPosition, -300).Distance(cp))
                        {
                            d = -1;
                        }
                    }

                    switch (d)
                    {
                        case -1:
                            UpdateThePos(cp2.ToVector3());
                            break;
                        default:
                            UpdateThePos(cp.ToVector3());
                            break;
                    }

                    i += 25;
                    if (i > 525)
                    {
                        //UpdateThePos(vv);
                        break;
                    }
                }
            }

            var Epred = FSpred.Prediction.Prediction.GetPrediction(E, target);
            if(Epred.Hitchance >= FSpred.Prediction.HitChance.High && E.From.DistanceSquared(ObjectManager.Player.Position) <= 275625 && Epred.CastPosition.DistanceSquared(ObjectManager.Player.Position) <= 1500625 && E.From.DistanceSquared(Epred.CastPosition) <= 490000)
            {
                CastE(E.From, Epred.CastPosition);
            }

            UpdateThePos(ObjectManager.Player.Position.Extend(target.Position, 525));
            {
                var Epred2 = FSpred.Prediction.Prediction.GetPrediction(E, target);
                if (Epred2.Hitchance >= FSpred.Prediction.HitChance.High && E.From.DistanceSquared(ObjectManager.Player.Position) <= 275625 && Epred.CastPosition.DistanceSquared(ObjectManager.Player.Position) <= 1500625 && E.From.DistanceSquared(Epred.CastPosition) <= 490000)
                {
                    CastE(E.From, Epred2.CastPosition);
                }
            }
            
            E.Speed = speedE;
            E.Range = rangeE;
            E.From = player.Position;
            E.RangeCheckFrom = player.Position;
        }
        private static void UpdateThePos(Vector3 vector3)
        {
            E.UpdateSourcePosition(vector3, vector3);
        }
        private static void UpdateThePos(AIBaseClient target, Vector3 zero, FSpred.Prediction.PredictionOutput temppred)
        {
            if (zero.DistanceToPlayer() <= 525)
            {
                E.UpdateSourcePosition(zero, zero);
            }
            else
            {
                if (temppred.CastPosition.DistanceToPlayer() <= 525)
                {
                    E.UpdateSourcePosition(temppred.CastPosition, temppred.CastPosition);
                }
                else
                {
                    var circel = new Geometry.Circle(FSpred.Prediction.Prediction.PredictUnitPosition(target, 500), 350).Points.ToList();
                    circel.RemoveAll(i => i.DistanceToPlayer() >= 525);
                    var pos = circel[0].ToVector3();
                    if (!pos.IsZero)
                    {
                        E.UpdateSourcePosition(pos, pos);
                    }
                }
            }
        }

        private static void UpdateThePos(Vector3 zero, FSpred.Prediction.PredictionOutput temppred, FSpred.Prediction.PredictionOutput ThePred)
        {
            if (zero.DistanceToPlayer() <= 525)
            {
                E.UpdateSourcePosition(zero, zero);
            }
            else
            {
                if (temppred.CastPosition.DistanceToPlayer() <= 525)
                {
                    E.UpdateSourcePosition(temppred.CastPosition, temppred.CastPosition);
                }
                else
                {
                    if (ThePred.CastPosition.DistanceToPlayer() <= 525)
                    {
                        E.UpdateSourcePosition(ThePred.CastPosition, ThePred.CastPosition);
                    }
                    else
                    {
                        var circel = new Geometry.Circle(ThePred.CastPosition.ToVector2(), 350).Points.ToList();
                        circel.RemoveAll(i => i.DistanceToPlayer() >= 525);
                        var pos = circel[0].ToVector3();
                        if (!pos.IsZero)
                        {
                            E.UpdateSourcePosition(pos, pos);
                        }
                    }
                }
            }
        }

        private static void OnCombo()
        {

            try
            {
                bool useQ = boolLinks["comboUseQ"].GetValue<MenuBool>().Enabled && Q.IsReady();
                bool useW = boolLinks["comboUseW"].GetValue<MenuBool>().Enabled && W.IsReady();
                bool useE = boolLinks["comboUseE"].GetValue<MenuBool>().Enabled && E.IsReady();
                bool useR = boolLinks["comboUseR"].GetValue<MenuBool>().Enabled && R.IsReady();

                bool killpriority = boolLinks["spPriority"].GetValue<MenuBool>().Enabled && R.IsReady();
                bool rKillSteal = boolLinks["rLastHit"].GetValue<MenuBool>().Enabled;
                AIHeroClient Etarget = FunnySlayerCommon.FSTargetSelector.GetFSTarget(maxRangeE);
                AIHeroClient Qtarget = FunnySlayerCommon.FSTargetSelector.GetFSTarget(Q.Range);
                AIHeroClient RTarget = FunnySlayerCommon.FSTargetSelector.GetFSTarget(R.Range);
                if (killpriority && Qtarget != null & Etarget != null && Etarget != Qtarget && ((Etarget.Health > TotalDmg(Etarget, false, true, false, false)) || (Etarget.Health > TotalDmg(Etarget, false, true, true, false) && Etarget == RTarget)) && Qtarget.Health < TotalDmg(Qtarget, true, true, false, false))
                {
                    Etarget = Qtarget;
                }

                if (RTarget != null && rKillSteal && useR && boolLinks["RU" + RTarget.CharacterName].GetValue<MenuBool>().Enabled)
                {
                    if (TotalDmg(RTarget, true, true, false, false) < RTarget.Health && TotalDmg(RTarget, true, true, true, true) > RTarget.Health)
                    {
                        R.Cast(RTarget.Position);
                    }
                }

                if (useQ)
                {

                    if (Qtarget != null)
                        Q.Cast(Qtarget);
                }

                if (useE)
                {
                    if (Etarget != null)
                    {
                        if(EpredictionList.Index == 0)
                        {
                            PredictCastE(Etarget);
                        }
                        else
                        {
                            FSPredictionECast(Etarget);
                        }
                    }
                }
               
                if (useW)
                {
                    var t = FunnySlayerCommon.FSTargetSelector.GetFSTarget(W.Range);

                    if (t != null)
                    {
                        var pred = FSpred.Prediction.Prediction.GetPrediction(W, t);

                        if (t.Path.Count() < 2)
                        {
                            if (t.HasBuffOfType(BuffType.Slow))
                            {
                                if (pred.Hitchance >= FSpred.Prediction.HitChance.High)
                                    W.Cast(pred.CastPosition);
                            }
                            if (t.CountEnemyHeroesInRange(250) > 2)
                            {
                                if (pred.Hitchance >= FSpred.Prediction.HitChance.High)
                                    W.Cast(pred.CastPosition);
                            }
                        }

                        if (pred.Hitchance >= FSpred.Prediction.HitChance.High && t.DistanceToPlayer() <= W.Range)
                            W.Cast(pred.CastPosition);
                    }
                }
                if (useR && R.Instance.Name == "ViktorChaosStorm" && player.CanCast && !player.Spellbook.IsCastingSpell)
                {

                    foreach (var unit in HeroManager.Enemies.Where(h => h.IsValidTarget(R.Range)))
                    {
                        R.CastIfWillHit(unit, Array.IndexOf(stringLinks["HitR"].GetValue<MenuList>().Items, stringLinks["HitR"].GetValue<MenuList>().SelectedValue) + 1);

                    }
                }
            }
            catch (Exception error)
            {
                Console.WriteLine(error.ToString());
            }
        }

        private static void Flee()
        {
            Orbwalker.Move(Game.CursorPos);
            if (!Q.IsReady() || !(player.HasBuff("viktorqaug") || player.HasBuff("viktorqeaug") || player.HasBuff("viktorqwaug") || player.HasBuff("viktorqweaug")))
            {
                return;
            }
            var closestminion = GameObjects.GetMinions(Q.Range, MinionTypes.All, MinionTeam.Enemy).MinOrDefault(m => player.Distance(m));
            var closesthero = HeroManager.Enemies.MinOrDefault(m => player.Distance(m) < Q.Range);
            if (closestminion.IsValidTarget(Q.Range))
            {
                Q.Cast(closestminion);
                return;
            }
            else if (closesthero.IsValidTarget(Q.Range))
            {
                Q.Cast(closesthero);
                return;
            }
        }


        private static void OnHarass()
        {
            // Mana check
            if ((player.Mana / player.MaxMana) * 100 < sliderLinks["harassMana"].GetValue<MenuSlider>().Value)
                return;
            bool useE = boolLinks["harassUseE"].GetValue<MenuBool>().Enabled && E.IsReady();
            bool useQ = boolLinks["harassUseQ"].GetValue<MenuBool>().Enabled && Q.IsReady();
            if (useQ)
            {
                var qtarget = TargetSelector.GetTarget(Q.Range, DamageType.Magical);
                if (qtarget != null)
                    Q.Cast(qtarget);
            }
            if (useE)
            {
                var harassrange = sliderLinks["eDistance"].GetValue<MenuSlider>().Value;
                var target = TargetSelector.GetTarget(harassrange, DamageType.Magical);

                if (target != null)
                    if (EpredictionList.Index == 0)
                    {
                        PredictCastE(target);
                    }
                    else
                    {
                        FSPredictionECast(target);
                    }
            }
        }

        private static void OnWaveClear()
        {
            // Mana check
            if ((player.Mana / player.MaxMana) * 100 < sliderLinks["waveMana"].GetValue<MenuSlider>().Value)
                return;

            bool useQ = boolLinks["waveUseQ"].GetValue<MenuBool>().Enabled && Q.IsReady();
            bool useE = boolLinks["waveUseE"].GetValue<MenuBool>().Enabled && E.IsReady();

            /*if (useQ)
            {
                foreach (var minion in GameObjects.GetMinions(player.Position, player.AttackRange))
                {
                    if (Q.IsKillable(minion) && minion.CharacterName.Contains("Siege"))
                    {
                        QLastHit(minion);
                        break;
                    }
                }
            }*/

            if (useE)
                PredictCastMinionE();
        }

        private static void OnJungleClear()
        {
            // Mana check
            if ((player.Mana / player.MaxMana) * 100 < sliderLinks["waveMana"].GetValue<MenuSlider>().Value)
                return;

            bool useQ = boolLinks["waveUseQ"].GetValue<MenuBool>().Enabled && Q.IsReady();
            bool useE = boolLinks["waveUseE"].GetValue<MenuBool>().Enabled && E.IsReady();

            if (useQ)
            {
                foreach (var minion in GameObjects.Jungle.Where(x => x.IsValidTarget(player.AttackRange)).OrderBy(x => x.MaxHealth).ToList())
                {
                    Q.Cast(minion);
                }
            }

            if (useE)
                PredictCastMinionEJungle();
        }

        public static FarmLocation GetBestLaserFarmLocation(bool jungle)
        {
            var bestendpos = new SharpDX.Vector2();
            var beststartpos = new SharpDX.Vector2();
            var minionCount = 0;
            List<AIBaseClient> allminions;
            var minimalhit = sliderLinks["waveNumE"].GetValue<MenuSlider>().Value;
            if (!jungle)
            {
                allminions = GameObjects.GetMinions(maxRangeE).ToList();

            }
            else
            {
                allminions = GameObjects.Jungle.Where(x => x.IsValidTarget(maxRangeE)).ToList<AIBaseClient>();
            }
            var minionslist = (from mnion in allminions select mnion.Position.ToVector2()).ToList<SharpDX.Vector2>();
            var posiblePositions = new List<SharpDX.Vector2>();
            posiblePositions.AddRange(minionslist);
            var max = posiblePositions.Count;
            for (var i = 0; i < max; i++)
            {
                for (var j = 0; j < max; j++)
                {
                    if (posiblePositions[j] != posiblePositions[i])
                    {
                        posiblePositions.Add((posiblePositions[j] + posiblePositions[i]) / 2);
                    }
                }
            }

            foreach (var startposminion in allminions.Where(m => player.Distance(m) < rangeE))
            {
                var startPos = startposminion.Position.ToVector2();

                foreach (var pos in posiblePositions)
                {
                    if (pos.Distance(startPos) <= lengthE * lengthE)
                    {
                        var endPos = startPos + lengthE * (pos - startPos).Normalized();

                        var count =
                            minionslist.Count(pos2 => pos2.Distance(startPos, endPos, true) <= 140 * 140);

                        if (count >= minionCount)
                        {
                            bestendpos = endPos;
                            minionCount = count;
                            beststartpos = startPos;
                        }

                    }
                }
            }
            if ((!jungle && minimalhit < minionCount) || (jungle && minionCount > 0))
            {
                //Console.WriteLine("MinimalHits: " + minimalhit + "\n Startpos: " + beststartpos + "\n Count : " + minionCount);
                return new FarmLocation(beststartpos, bestendpos, minionCount);
            }
            else
            {
                return new FarmLocation(beststartpos, bestendpos, 0);
            }
        }



        private static bool PredictCastMinionEJungle()
        {
            var farmLocation = GetBestLaserFarmLocation(true);

            if (farmLocation.MinionsHit > 0)
            {
                CastE(farmLocation.Position1, farmLocation.Position2);
                return true;
            }

            return false;
        }

        public struct FarmLocation
        {
            /// <summary>
            /// The minions hit
            /// </summary>
            public int MinionsHit;

            /// <summary>
            /// The start position
            /// </summary>
            public SharpDX.Vector2 Position1;


            /// <summary>
            /// The end position
            /// </summary>
            public SharpDX.Vector2 Position2;

            /// <summary>
            /// Initializes a new instance of the <see cref="FarmLocation"/> struct.
            /// </summary>
            /// <param name="position">The position.</param>
            /// <param name="minionsHit">The minions hit.</param>
            public FarmLocation(SharpDX.Vector2 startpos, SharpDX.Vector2 endpos, int minionsHit)
            {
                Position1 = startpos;
                Position2 = endpos;
                MinionsHit = minionsHit;
            }
        }
        private static bool PredictCastMinionE()
        {
            var farmLoc = GetBestLaserFarmLocation(false);
            if (farmLoc.MinionsHit > 0)
            {
                Console.WriteLine("Minion amount: " + farmLoc.MinionsHit + "\n Startpos: " + farmLoc.Position1 + "\n EndPos: " + farmLoc.Position2);

                CastE(farmLoc.Position1, farmLoc.Position2);
                return true;
            }

            return false;
        }

     
        private static void CastE(SharpDX.Vector3 source, SharpDX.Vector3 destination)
        {
            E.Cast(source, destination);
        }

        private static void CastE(SharpDX.Vector2 source, SharpDX.Vector2 destination)
        {
            E.Cast(source, destination);
        }

        private static void Interrupter2_OnInterruptableTarget(AIHeroClient sender, Interrupter.InterruptSpellArgs args)
        {
            var unit = args.Sender;
            if (args.DangerLevel >= Interrupter.DangerLevel.High && unit.IsEnemy)
            {
                var useW = boolLinks["wInterrupt"].GetValue<MenuBool>().Enabled;
                var useR = boolLinks["rInterrupt"].GetValue<MenuBool>().Enabled;

                if (useW && W.IsReady() && unit.IsValidTarget(W.Range) &&
                    (Game.Time + 1.5 + W.Delay) >= args.EndTime)
                {
                    if (W.Cast(unit) == CastStates.SuccessfullyCasted)
                        return;
                }
                else if (useR && unit.IsValidTarget(R.Range) && R.Instance.Name == "ViktorChaosStorm")
                {
                    R.Cast(unit);
                }
            }
        }

        private static void AntiGapcloser_OnEnemyGapcloser(
    AIHeroClient sender,
    AntiGapcloser.GapcloserArgs args
)
        {
            if (sender.IsAlly)
            {
                return;
            }
            if (boolLinks["miscGapcloser"].GetValue<MenuBool>().Enabled && W.IsInRange(args.EndPosition) && sender.IsEnemy && args.EndPosition.DistanceToPlayer() < 200)
            {
                GapCloserPos = args.EndPosition;
                if (args.StartPosition.Distance(args.EndPosition) > sender.Spellbook.GetSpell(args.Slot).SData.CastRangeDisplayOverride && sender.Spellbook.GetSpell(args.Slot).SData.CastRangeDisplayOverride > 100)
                {
                    GapCloserPos = args.StartPosition.Extend(args.EndPosition, sender.Spellbook.GetSpell(args.Slot).SData.CastRangeDisplayOverride);
                }
                W.Cast(GapCloserPos.ToVector2());
            }
        }
        private static void AutoW()
        {
            if (!W.IsReady() || !boolLinks["autoW"].GetValue<MenuBool>().Enabled)
                return;

            var tPanth = HeroManager.Enemies.Find(h => h.IsValidTarget(W.Range) && h.HasBuff("Pantheon_GrandSkyfall_Jump"));
            if (tPanth != null)
            {
                if (W.Cast(tPanth) == CastStates.SuccessfullyCasted)
                    return;
            }

            foreach (var enemy in HeroManager.Enemies.Where(h => h.IsValidTarget(W.Range)))
            {
                if (enemy.HasBuff("rocketgrab2"))
                {
                    var t = ObjectManager.Get<AIHeroClient>().Where(i => i.IsAlly).ToList().Find(h => h.CharacterName.ToLower() == "blitzcrank" && h.Distance((AttackableUnit)player) < W.Range);
                    if (t != null)
                    {
                        if (W.Cast(t) == CastStates.SuccessfullyCasted)
                            return;
                    }
                }
                if (enemy.HasBuffOfType(BuffType.Stun) || enemy.HasBuffOfType(BuffType.Snare) ||
                         enemy.HasBuffOfType(BuffType.Charm) || enemy.HasBuffOfType(BuffType.Fear) ||
                         enemy.HasBuffOfType(BuffType.Taunt) || enemy.HasBuffOfType(BuffType.Suppression) ||
                         enemy.IsStunned || enemy.IsRecalling())
                {
                    if (W.Cast(enemy) == CastStates.SuccessfullyCasted)
                        return;
                }
                if (W.GetPrediction(enemy).Hitchance == HitChance.Immobile)
                {
                    if (W.Cast(enemy) == CastStates.SuccessfullyCasted)
                        return;
                }
            }
        }
        private static void Drawing_OnDraw(EventArgs args)
        {
            // All circles
            if (player.IsDead)
                return;

            var pos = Drawing.WorldToScreen(ObjectManager.Player.Position);
            Drawing.DrawText(pos.X - 20, pos.Y + 20, Color.Yellow, MathFloat.To<string>());


            foreach (var spell in SpellList)
            {
                var menuBool = menu.Item("Draw" + spell.Slot + "Range").GetValue<MenuBool>();
                var menuColor = menu.Item("Draw" + spell.Slot + "Color").GetValue<MenuColor>();
                if (menuBool.Enabled)
                {
                    Render.Circle.DrawCircle(player.Position, spell.Range, menuColor.Color.ToSystemColor());
                }

            }
        }

        private static void ProcessLink(string key, object value)
        {
            if (value is MenuList)
            {
                stringLinks.Add(key, (MenuList)value);
            }
            else if (value is MenuSlider)
            {
                sliderLinks.Add(key, (MenuSlider)value);
            }

            else if (value is MenuKeyBind)
            {
                keyLinks.Add(key, (MenuKeyBind)value);
            }
            else
            {
                boolLinks.Add(key, (MenuBool)value);
            }
            
               
        }
        private static float TotalDmg(AIBaseClient enemy, bool useQ, bool useE, bool useR, bool qRange)
        {
            var qaaDmg = new Double[] { 20, 40, 60, 80, 100 };
            var damage = 0d;
            var rTicks = sliderLinks["rTicks"].GetValue<MenuSlider>().Value;
            bool inQRange = ((qRange && enemy.InAutoAttackRange()) || qRange == false);
            //Base Q damage
            if (useQ && Q.IsReady() && inQRange)
            {
                damage += player.GetSpellDamage(enemy, SpellSlot.Q);
                damage += player.CalculateDamage(enemy, DamageType.Magical, qaaDmg[Q.Level - 1] + 0.5 * player.TotalMagicalDamage + player.TotalAttackDamage);
            }

            // Q damage on AA
            if (useQ && !Q.IsReady() && player.HasBuff("viktorpowertransferreturn") && inQRange)
            {
                damage += player.CalculateDamage(enemy, DamageType.Magical, qaaDmg[Q.Level - 1] + 0.5 * player.TotalMagicalDamage + player.TotalAttackDamage);
            }

            //E damage
            if (useE && E.IsReady())
            {
                if (player.HasBuff("viktoreaug") || player.HasBuff("viktorqeaug") || player.HasBuff("viktorqweaug"))
                    damage += player.GetSpellDamage(enemy, SpellSlot.E);
                else
                    damage += player.GetSpellDamage(enemy, SpellSlot.E);
            }

            //R damage + 2 ticks
            if (useR && R.Level > 0 && R.IsReady() && R.Instance.Name == "ViktorChaosStorm")
            {
                damage += player.GetSpellDamage(enemy, SpellSlot.R) * rTicks;
                damage += player.GetSpellDamage(enemy, SpellSlot.R);
            }

            // Ludens Echo damage
            if (Items.HasItem(player, 3285))
                damage += player.CalculateDamage(enemy, DamageType.Magical, 100 + player.FlatMagicDamageMod * 0.1);

            //sheen damage
            if (Items.HasItem(player, 3057))
                damage += player.CalculateDamage(enemy, DamageType.Physical, 0.5 * player.BaseAttackDamage);

            //lich bane dmg
            if (Items.HasItem(player, 3100))
                damage += player.CalculateDamage(enemy, DamageType.Magical, 0.5 * player.FlatMagicDamageMod + 0.75 * player.BaseAttackDamage);

            return (float)damage;
        }
        private static float GetComboDamage(AIBaseClient enemy)
        {

            return TotalDmg(enemy, true, true, true, false);
        }

        public static Menu AddSubMenu(this Menu menu, Menu addMenu)
        {
            menu.Add(addMenu);
            return addMenu;
        }
        public static object AddItem(this Menu menu, MenuItem items)
        {
            try
            {
                menu.Add(items);
                return items;
            }
            catch
            {
                Console.WriteLine(items.Name);
                throw new Exception();
            }
        }
        public static MenuBool AddSpellDraw(this Menu menu, SpellSlot slot)
        {
            MenuBool a;
            switch (slot)
            {
                case SpellSlot.Q:
                    a = new MenuBool("DrawQRange", "Draw Q Range");
                    menu.Add(a);
                    menu.Add(new MenuColor("DrawQColor", "^ Q Color", ColorBGRA.FromRgba(179)));
                    return a;
                case SpellSlot.W:
                    a = new MenuBool("DrawWRange", "Draw W Range");
                    menu.Add(a);
                    menu.Add(new MenuColor("DrawWColor", "^ W Color", ColorBGRA.FromRgba(179)));
                    return a;
                case SpellSlot.E:
                    a = new MenuBool("DrawERange", "Draw E Range");
                    menu.Add(a);
                    menu.Add(new MenuColor("DrawEColor", "^ E Color", ColorBGRA.FromRgba(179)));
                    return a;
                case SpellSlot.Item1:
                    a = new MenuBool("DrawEMaxRange", "Draw E Max Range");
                    menu.Add(a);
                    menu.Add(new MenuColor("DrawEMaxColor", "^ E Max Color", ColorBGRA.FromRgba(179)));
                    return a;
                case SpellSlot.R:
                    a = new MenuBool("DrawRRange", "Draw R Range");
                    menu.Add(a);
                    menu.Add(new MenuColor("DrawRColor", "^ R Color", ColorBGRA.FromRgba(179)));
                    return a;
            }

            return null;


        }
        public static MenuSlider SetValue(this MenuSlider menuItem, Slider sliderValue)
        {
            menuItem.Value = sliderValue.value;
            menuItem.MinValue = sliderValue.minValue;
            menuItem.MaxValue = sliderValue.maxValue;
            return menuItem;
        }
        public class Slider
        {
            public int value;
            public int minValue;
            public int maxValue;

            public Slider(int setValue = 0, int setMinValue = 0, int setMaxValue = 100)
            {
                this.value = setValue;
                if (setMaxValue < setMinValue)
                {

                    this.minValue = setMaxValue;
                    this.maxValue = setMinValue;
                }
                else
                {
                    this.minValue = setMinValue;
                    this.maxValue = setMaxValue;

                }
            }
        }
        private static MenuList EpredictionList = new MenuList("EpredictionList", "E Prediction ", new string[] { "Normal Pred", "Fs Pred" }, 0);
        private static MenuSlider FsEpred = new MenuSlider("FsEPred", "Fs E Pred Value ", 15, 5, 120);
        private static MenuBool AutoCalculator = new MenuBool("AutoCalculator", "Auto Calculator Fs E Pred Value ", false);
        private static void SetupMenu()
        {

            menu = new Menu("Viktor", "Viktor Ported", true);
            // Combo
            var targetSelectorMenu = new Menu("Target Selector", "Target Selector");
            /*FunnySlayerCommon.MenuClass.AddTargetSelectorMenu(targetSelectorMenu);
            menu.Add(targetSelectorMenu);*/

            var subMenu = menu.AddSubMenu(new Menu("Combo", "Combo"));

            
            ProcessLink("comboUseQ", subMenu.AddItem(new MenuBool("comboUseQ", "Use Q")));
            ProcessLink("comboUseW", subMenu.AddItem(new MenuBool("comboUseW", "Use W")));
            ProcessLink("comboUseE", subMenu.AddItem(new MenuBool("comboUseE", "Use E")));
            subMenu.Add(EpredictionList);
            subMenu.Add(FsEpred);
            subMenu.Add(AutoCalculator);

            ProcessLink("comboUseR", subMenu.AddItem(new MenuBool("comboUseR", "Use R")));
            ProcessLink("qAuto", subMenu.AddItem(new MenuBool("qAuto", "Dont autoattack without passive")));
            ProcessLink("comboActive", subMenu.AddItem(new MenuKeyBind("comboActive", "Combo active", Keys.Space, KeyBindType.Press)));

            subMenu = menu.AddSubMenu(new Menu("Rconfig", "R config"));
            ProcessLink("HitR", subMenu.AddItem(new MenuList("HitR", "Auto R if: ", new string[] { "1 target", "2 targets", "3 targets", "4 targets", "5 targets" }, 3)));
            ProcessLink("AutoFollowR", subMenu.AddItem(new MenuBool("AutoFollowR", "Auto Follow R")));
            ProcessLink("rTicks", subMenu.AddItem(new MenuSlider("rTicks", "Ultimate ticks to count").SetValue(new Slider(2, 1, 14))));


            subMenu = subMenu.AddSubMenu(new Menu("Ronetarget", "R one target"));
            ProcessLink("forceR", subMenu.AddItem(new MenuKeyBind("forceR", "Force R on target", Keys.T, KeyBindType.Press)));
            ProcessLink("rLastHit", subMenu.AddItem(new MenuBool("rLastHit", "1 target ulti")));
            foreach (var hero in HeroManager.Enemies)
            {
                try
                {
                    ProcessLink("RU" + hero.CharacterName, subMenu.AddItem(new MenuBool("RU" + hero.CharacterName, "Use R on: " + hero.CharacterName)));
                }
                catch { }
            }


            subMenu = menu.AddSubMenu(new Menu("Testfeatures", "Test features"));
            ProcessLink("spPriority", subMenu.AddItem(new MenuBool("spPriority", "Prioritize kill over dmg")));


            // Harass
            subMenu = menu.AddSubMenu(new Menu("Harass", "Harass"));
            ProcessLink("harassUseQ", subMenu.AddItem(new MenuBool("harassUseQ", "Use Q")));
            ProcessLink("harassUseE", subMenu.AddItem(new MenuBool("harassUseE", "Use E")));
            ProcessLink("harassMana", subMenu.AddItem(new MenuSlider("harassMana", "Mana usage in percent (%)").SetValue(new Slider(30))));
            ProcessLink("eDistance", subMenu.AddItem(new MenuSlider("eDistance", "Harass range with E").SetValue(new Slider(maxRangeE, rangeE, maxRangeE))));
            ProcessLink("harassActive", subMenu.AddItem(new MenuKeyBind("harassActive", "Harass active", Keys.C, KeyBindType.Press)));

            // WaveClear
            subMenu = menu.AddSubMenu(new Menu("WaveClear", "WaveClear"));
            ProcessLink("waveUseQ", subMenu.AddItem(new MenuBool("waveUseQ", "Use Q")));
            ProcessLink("waveUseE", subMenu.AddItem(new MenuBool("waveUseE", "Use E")));
            ProcessLink("waveNumE", subMenu.AddItem(new MenuSlider("waveNumE", "Minions to hit with E").SetValue(new Slider(2, 1, 10))));
            ProcessLink("waveMana", subMenu.AddItem(new MenuSlider("waveMana", "Mana usage in percent (%)").SetValue(new Slider(30))));
            ProcessLink("waveActive", subMenu.AddItem(new MenuKeyBind("waveActive", "WaveClear active", Keys.V, KeyBindType.Press)));
            ProcessLink("jungleActive", subMenu.AddItem(new MenuKeyBind("jungleActive", "JungleClear active", Keys.G, KeyBindType.Press)));

            subMenu = menu.AddSubMenu(new Menu("LastHit", "LastHit"));
            ProcessLink("waveUseQLH", subMenu.AddItem(new MenuKeyBind("waveUseQLH", "Use Q", Keys.A, KeyBindType.Press)));

            // Harass
            subMenu = menu.AddSubMenu(new Menu("Flee", "Flee"));
            ProcessLink("FleeActive", subMenu.AddItem(new MenuKeyBind("FleeActive", "Flee mode", Keys.Z, KeyBindType.Press)));

            // Misc
            subMenu = menu.AddSubMenu(new Menu("Misc", "Misc"));
            ProcessLink("rInterrupt", subMenu.AddItem(new MenuBool("rInterrupt", "Use R to interrupt dangerous spells")));
            ProcessLink("wInterrupt", subMenu.AddItem(new MenuBool("wInterrupt", "Use W to interrupt dangerous spells")));
            ProcessLink("autoW", subMenu.AddItem(new MenuBool("autoW", "Use W to continue CC")));
            ProcessLink("miscGapcloser", subMenu.AddItem(new MenuBool("miscGapcloser", "Use W against gapclosers")));

            // Drawings
            subMenu = menu.AddSubMenu(new Menu("Drawings", "Drawings"));
            ProcessLink("drawRangeQ", subMenu.AddSpellDraw(SpellSlot.Q));
            ProcessLink("drawRangeW", subMenu.AddSpellDraw(SpellSlot.W));
            ProcessLink("drawRangeE", subMenu.AddSpellDraw(SpellSlot.E));
            ProcessLink("drawRangeR", subMenu.AddSpellDraw(SpellSlot.R));
            menu.Attach();
        }
    }
}
