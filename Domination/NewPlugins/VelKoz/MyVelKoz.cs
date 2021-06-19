using EnsoulSharp;
using EnsoulSharp.SDK;
using EnsoulSharp.SDK.MenuUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using HitChance = FSpred.Prediction.HitChance;

namespace DominationAIO.NewPlugins
{
    public static class MyVelKoz
    {
        private static Menu VMenu = new Menu("FunnySlayerVelkoz", "FunnySlayer Velkoz", true);
        private static Spell Q, W, E, R;
        private static Spell Qsplits;
        private static int MaxRange = 1485;
        private static class VKMenuSettings
        {
            public static MenuBool DisableAA = new MenuBool("DisabledAA", "Disable AA when spell ready");
            public static class Qmenu
            {
                public static MenuBool useQ = new MenuBool("useQ", "Use Q");
                public static MenuBool Qexc = new MenuBool("useQExc", "Enchance Q");
                public static MenuBool Qcombo = new MenuBool("Qcombo", "Use on Combo");
                public static MenuBool Qharass = new MenuBool("Qharass", "Use on Harass");
                public static MenuBool autoQ2 = new MenuBool("autoQ2", "Auto Q splits");
            }
            public static class Wmenu
            {
                public static MenuBool useW = new MenuBool("useW", "Use W");
                public static MenuBool Wcombo = new MenuBool("Wcombo", "Use on Combo");
                public static MenuBool Wharass = new MenuBool("Wharass", "Use on Harass");
            }
            public static class Emenu
            {
                public static MenuBool useE = new MenuBool("useE", "Use E");
                public static MenuBool Ecombo = new MenuBool("Ecombo", "Use on Combo");
                public static MenuBool Eharass = new MenuBool("Eharass", "Use on Harass");
            }
            public static class Rmenu
            {
                public static MenuBool useR = new MenuBool("useR", "Use R");
                public static MenuBool Aim = new MenuBool("Aim", "Auto Aim");
            }
        }
        private static Spell newspell = new Spell(SpellSlot.Unknown, MaxRange);
        public static void VelkozLoad()
        {
            Q = new Spell(SpellSlot.Q, 1000f);
            W = new Spell(SpellSlot.W, 1000f);
            E = new Spell(SpellSlot.E, 900f);
            R = new Spell(SpellSlot.R, MaxRange);

            Qsplits = new Spell(SpellSlot.Q, 1050f);

            var delay = 0.25f;
            Q.SetSkillshot(delay, 80f, 1400f, true, SpellType.Line);
            W.SetSkillshot(delay, 100f, 2000f, false, SpellType.Line);
            E.SetSkillshot(delay + 0.75f, 225f, 1500f, false, SpellType.Circle);
            R.SetSkillshot(delay, 80f, float.MaxValue, false, SpellType.Line);

            newspell.SetSkillshot(2f, 80f, 1400f, false, SpellType.Line);
            Qsplits.SetSkillshot(delay, 50f, 2000f, true, SpellType.Line);

            var qs = new Menu("qs", "Q Settings");
            qs.Add(VKMenuSettings.Qmenu.useQ);
            qs.Add(VKMenuSettings.Qmenu.Qexc);
            qs.Add(VKMenuSettings.Qmenu.Qcombo);
            qs.Add(VKMenuSettings.Qmenu.Qharass);
            qs.Add(VKMenuSettings.Qmenu.autoQ2);
            var ws = new Menu("ws", "W Settings");
            ws.Add(VKMenuSettings.Wmenu.useW);
            ws.Add(VKMenuSettings.Wmenu.Wcombo);
            ws.Add(VKMenuSettings.Wmenu.Wharass);
            var es = new Menu("es", "E Settings");
            es.Add(VKMenuSettings.Emenu.useE);
            es.Add(VKMenuSettings.Emenu.Ecombo);
            es.Add(VKMenuSettings.Emenu.Eharass);
            var rs = new Menu("rs", "R Settings");
            rs.Add(VKMenuSettings.Rmenu.useR);
            rs.Add(VKMenuSettings.Rmenu.Aim);

            VMenu.Add(VKMenuSettings.DisableAA);
            VMenu.Add(qs);
            VMenu.Add(ws);
            VMenu.Add(es);
            VMenu.Add(rs);

            VMenu.Attach();

            Game.OnUpdate += Game_OnUpdate;
            Game.OnUpdate += Game_OnUpdate1;
            MissileClient.OnCreate += MissileClient_OnCreate;
            //MissileClient.OnDelete += MissileClient_OnDelete;
        }
        
        private static MissileClient Qm = null;
        private static void MissileClient_OnDelete(GameObject sender, EventArgs args)
        {
            if (sender.Type == GameObjectType.MissileClient)
            {
                var obj = sender as MissileClient;
                if (obj != null)
                {
                    if (obj.SpellCaster != null)
                    {
                        if (obj.SpellCaster.IsMe)
                        {
                            if (obj.SData.Name.ToLower().Contains("velkozqmissile"))
                            {
                                Qm = null;
                            }
                        }
                    }
                }
            }
            else
            {
                return;
            }
        }

        private static void MissileClient_OnCreate(GameObject sender, EventArgs args)
        {
            if(sender.Type == GameObjectType.MissileClient)
            {
                var obj = sender as MissileClient;
                if(obj != null)
                {
                    if(obj.SpellCaster != null)
                    {
                        if (obj.SpellCaster.IsMe)
                        {
                            if (obj.SData.Name.ToLower().Contains("velkozqmissile"))
                            {
                                Qm = obj;
                            }
                        }
                    }
                }
            }
            else
            {
                return;
            }
        }
        private static void Game_OnUpdate1(EventArgs args)
        {
            if (ObjectManager.Player.IsDead)
                return;

            if (ObjectManager.Player.Spellbook.IsChanneling)
                Orbwalker.AttackEnabled = false;
            else
                 if (VKMenuSettings.DisableAA.Enabled)
                    Orbwalker.AttackEnabled = !(W.IsReady() || E.IsReady() || Q.IsReady());
                 else
                    Orbwalker.AttackEnabled = true;

            if (ObjectManager.Player.Spellbook.IsChanneling && VKMenuSettings.Rmenu.Aim.Enabled)
            {
                var endPoint = new Vector2();
                foreach (var obj in ObjectManager.Get<GameObject>())
                {
                    if (obj != null && obj.IsValid && obj.Name.Contains("Velkoz_") &&
                        obj.Name.Contains("_R_Beam_End"))
                    {
                        endPoint = ObjectManager.Player.Position.ToVector2() +
                                   R.Range * (obj.Position - ObjectManager.Player.Position).ToVector2().Normalized();
                        break;
                    }
                }

                if (endPoint.IsValid())
                {
                    var targets = new List<AIBaseClient>();

                    foreach (var enemy in ObjectManager.Get<AIHeroClient>().Where(h => h.IsValidTarget(R.Range)))
                    {
                        if (enemy.Position.ToVector2().Distance(ObjectManager.Player.Position.ToVector2(), endPoint, true) < 400)
                            targets.Add(enemy);
                    }
                    if (targets.Count > 0)
                    {
                        var target = targets.OrderBy(t => t.Health / Q.GetDamage(t)).ToList()[0];
                        ObjectManager.Player.Spellbook.UpdateChargedSpell(SpellSlot.R, FSpred.Prediction.Prediction.PredictUnitPosition(target, 300).ToVector3(), false, false);
                    }
                    else
                    {
                        ObjectManager.Player.Spellbook.UpdateChargedSpell(SpellSlot.R, Game.CursorPos, false, false);
                    }
                }

                return;
            }
            if(Qm != null && Q.Name != "VelkozQ" && Qm.IsValid)
            {
                Qsplits.UpdateSourcePosition(Qm.Position, Qm.Position);
                
                var qMissilePosition = Qm.Position.ToVector2();
                var perpendicular = (Qm.EndPosition - Qm.StartPosition).ToVector2().Normalized().Perpendicular();

                var lineSegment1End = qMissilePosition + perpendicular * Qsplits.Range;
                var lineSegment2End = qMissilePosition - perpendicular * Qsplits.Range;

                var potentialTargets = new List<AIBaseClient>();
                foreach (
                    var enemy in
                        ObjectManager.Get<AIHeroClient>()
                            .Where(
                                h =>
                                    h.IsValidTarget() &&
                                    h.Position.ToVector2()
                                        .Distance(qMissilePosition, Qm.EndPosition.ToVector2(), true) < 700))
                {
                    potentialTargets.Add(enemy);
                }
                foreach (
                    var enemy in
                        ObjectManager.Get<AIHeroClient>()
                            .Where(
                                h =>
                                    h.IsValidTarget() &&
                                    (potentialTargets.Count == 0 ||
                                     h.NetworkId == potentialTargets.OrderBy(t => t.Health / Q.GetDamage(t)).ToList()[0].NetworkId) &&
                                    (h.Position.ToVector2().Distance(qMissilePosition, Qm.EndPosition.ToVector2(), true) > Q.Width + h.BoundingRadius)))
                {
                    var prediction = FSpred.Prediction.Prediction.GetPrediction(Qsplits, enemy);
                    var d1 = prediction.UnitPosition.ToVector2().Distance(qMissilePosition, lineSegment1End, true);
                    var d2 = prediction.UnitPosition.ToVector2().Distance(qMissilePosition, lineSegment2End, true);
                    if (prediction.Hitchance >= HitChance.High &&
                        (d1 < Qsplits.Width || d2 < Qsplits.Width))
                    {
                        if(Orbwalker.ActiveMode <= OrbwalkerMode.Harass || VKMenuSettings.Qmenu.autoQ2.Enabled)
                            if (Q.Cast())
                                return;
                    }
                }
            }
        }
        private static void Game_OnUpdate(EventArgs args)
        {
            if (ObjectManager.Player.IsDead || FunnySlayerCommon.OnAction.OnAA || FunnySlayerCommon.OnAction.BeforeAA || ObjectManager.Player.Spellbook.IsChanneling)
                return;

            if (LastCast.LastCastPacketSent.Slot == SpellSlot.R && R.IsReady())
                return;

            if (VKMenuSettings.DisableAA.Enabled)
                Orbwalker.AttackEnabled = !(W.IsReady() || E.IsReady() || Q.IsReady());
            else
                Orbwalker.AttackEnabled = true;

            if(Orbwalker.ActiveMode == OrbwalkerMode.Combo && !ObjectManager.Player.Spellbook.IsChanneling)
            {
                if (DoSpell(VKMenuSettings.Qmenu.Qcombo.Enabled, VKMenuSettings.Wmenu.Wcombo.Enabled, VKMenuSettings.Emenu.Ecombo.Enabled))
                    return;
            }
            if(Orbwalker.ActiveMode == OrbwalkerMode.Harass && !ObjectManager.Player.Spellbook.IsChanneling)
            {
                if (DoSpell(VKMenuSettings.Qmenu.Qharass.Enabled, VKMenuSettings.Wmenu.Wharass.Enabled, VKMenuSettings.Emenu.Eharass.Enabled))
                    return;
            }
        }

        private static bool DoSpell(bool useq = true, bool usew = true, bool usee = true, bool user = true)
        {
            if (Q.IsReady() && useq && VKMenuSettings.Qmenu.useQ.Enabled)
            {
                if(Q.Name == "VelkozQ")
                {
                    var target = TargetSelector.GetTarget(1050f, DamageType.Magical);
                    if (target != null)
                    {
                        var pred = FSpred.Prediction.Prediction.GetPrediction(Q, target);
                        if (pred.Hitchance >= HitChance.High)
                        {
                            if(pred.CastPosition.DistanceToPlayer() <= 1000)
                                return Q.Cast(pred.CastPosition);
                        }
                    }
                    var alltargets = TargetSelector.GetTargets(MaxRange, DamageType.Magical);
                    if (alltargets != null)
                    {
                        var gettarget = alltargets.Where(i =>
                                FSpred.Prediction.Prediction.GetPrediction(newspell, i).Hitchance >= HitChance.High)
                            .OrderBy(i => i.Health)
                            .FirstOrDefault();

                        if (gettarget != null)
                        {
                            var pred = FSpred.Prediction.Prediction.GetPrediction(Q, gettarget);
                            if (pred.Hitchance >= HitChance.High) 
                            {
                                if (pred.CastPosition.DistanceToPlayer() <= 1000)
                                    return Q.Cast(pred.CastPosition);
                            }
                            else
                            {
                                //code by Kortatu
                                var c = 0;
                                var d = 28;
                                if (VKMenuSettings.Qmenu.Qexc.Enabled)
                                {
                                    c = -1;
                                    d = 30;
                                }
                                for (int i = 1; i >= c; i-= 2)
                                {
                                    if(i == 0)
                                    {
                                        i = -1;
                                    }

                                    var newpred = FSpred.Prediction.Prediction.GetPrediction(newspell, gettarget);
                                    if (newpred.Hitchance >= HitChance.High && newpred.CastPosition.DistanceToPlayer() <= MaxRange)
                                    {
                                        float alpha = d * (float)Math.PI / 180;
                                        var cp = ObjectManager.Player.Position.ToVector2() +
                                                 (newpred.CastPosition.ToVector2() - ObjectManager.Player.Position.ToVector2()).Rotated
                                                     (i * alpha);
                                        
                                        if (
                                            Q.GetCollision(ObjectManager.Player.Position.ToVector2(), new List<Vector2>() {cp}).Count ==
                                            0 &&
                                            Qsplits.GetCollision(cp, new List<Vector2>() { newpred.CastPosition.ToVector2() }).Count == 0)
                                        {
                                            return Q.Cast(cp);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (W.IsReady() && usew && VKMenuSettings.Wmenu.useW.Enabled)
            {
                var target = W.GetTarget();
                if (target != null)
                {
                    var pred = FSpred.Prediction.Prediction.GetPrediction(W, target);
                    if (pred.Hitchance >= HitChance.High)
                    {
                        if (pred.CastPosition.DistanceToPlayer() <= 1000)
                            return W.Cast(pred.CastPosition);
                    }
                }
            }
            if (E.IsReady() && usee && VKMenuSettings.Emenu.useE.Enabled)
            {
                var target = E.GetTarget();
                if (target != null)
                {
                    var pred = FSpred.Prediction.Prediction.GetPrediction(E, target);
                    if (pred.Hitchance >= HitChance.High)
                    {
                        if (pred.CastPosition.DistanceToPlayer() <= 900)
                            return E.Cast(pred.CastPosition);
                    }
                }
            }
            if (R.IsReady() && user && !ObjectManager.Player.Spellbook.IsChanneling && Orbwalker.ActiveMode == OrbwalkerMode.Combo && VKMenuSettings.Rmenu.useR.Enabled)
            {
                var target = R.GetTarget();
                if (target != null)
                {
                    if (ObjectManager.Player.GetSpellDamage(target, SpellSlot.R) / 10 *
                        (FSpred.Prediction.Prediction.PredictUnitPosition(target, 750).DistanceToPlayer() <
                         R.Range - 500
                            ? 10
                            : 6) > target.Health)
                    {
                        return R.Cast(target.Position);
                    }
                }
            }
            return false;
        }
    }
}
