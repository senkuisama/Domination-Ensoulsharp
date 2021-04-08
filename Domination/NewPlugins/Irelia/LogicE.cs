using EnsoulSharp;
using EnsoulSharp.SDK;
using FunnySlayerCommon;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominationAIO.NewPlugins
{
    public static class LogicE
    {
        public static void EPrediction(bool Checkbuff = true)
        {
            var target = TargetSelector.GetTarget(775);

            if (!Irelia.E.IsReady() || target == null)
                return;

            {
                if (target != null && (!target.HasBuff("ireliamark") || !Checkbuff))
                {
                    //float ereal = 0.275f + Game.Ping / 1000;

                    if (Irelia.E.IsReady(0))
                    {
                        if (Irelia.E.Name != "IreliaE" && Irelia.E1Pos.IsValid())
                        {
                            if (MenuSettings.ESettings.Emode.Index == 0)
                            {
                                var pos = FSpred.Prediction.Prediction.PredictUnitPosition(target, MenuSettings.ESettings.EDelay.Value).ToVector3();

                                {
                                    if (pos.IsValid())
                                    {
                                        int range = 1000;
                                        if (ObjectManager.Player.CountEnemyHeroesInRange(775) > 2)
                                        {
                                            range = 1000;
                                        }
                                        else
                                        {
                                            range = 350;
                                        }

                                        for (int i = range; i > 50; i--)
                                        {
                                            var poscast = pos.Extend(Irelia.E1Pos, -i);
                                            if (poscast.IsValid() && poscast.Distance(ObjectManager.Player.Position) < 775)
                                            {
                                                if (Irelia.E.Cast(poscast))
                                                    return;
                                            }
                                            else
                                            {
                                                continue;
                                            }
                                        }
                                    }
                                }
                            }
                            if (MenuSettings.ESettings.Emode.Index == 2)
                            {
                                var tempE = new Spell(SpellSlot.Unknown, 775);
                                tempE.SetSkillshot(MenuSettings.ESettings.EDelay.Value / 1000, 1, 2000, false, EnsoulSharp.SDK.SpellType.Line);
                                {
                                    var pred = FSpred.Prediction.Prediction.GetPrediction(tempE, target);
                                    if (pred.Hitchance >= FSpred.Prediction.HitChance.High && pred.CastPosition.IsValid())
                                    {
                                        int range = 0;
                                        if (ObjectManager.Player.CountEnemyHeroesInRange(775) > 2)
                                        {
                                            range = 1000;
                                        }
                                        else
                                        {
                                            range = 350;
                                        }

                                        for (int i = range; i > 50; i--)
                                        {
                                            var pos = pred.CastPosition.Extend(Irelia.E1Pos, -i);
                                            if (pos.IsValid() && pos.Distance(ObjectManager.Player.Position) < 775)
                                            {
                                                if (Irelia.E.Cast(pos))
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
                            }
                            if (MenuSettings.ESettings.Emode.Index == 1)
                            {
                                var tempE = new Spell(SpellSlot.Unknown, 775);
                                tempE.SetSkillshot(0.25f, 1, 2000, false, EnsoulSharp.SDK.SpellType.Line);
                                {
                                    var pred = FSpred.Prediction.Prediction.GetPrediction(tempE, target);
                                    if (pred.Hitchance >= FSpred.Prediction.HitChance.High && pred.CastPosition.IsValid())
                                    {
                                        int range = 2000;
                                        if (ObjectManager.Player.CountEnemyHeroesInRange(775) > 2)
                                        {
                                            range = 2000;
                                        }
                                        else
                                        {
                                            range = MenuSettings.ESettings.E1vs1Range.Value;
                                        }

                                        for (int i = range; i > 50; i--)
                                        {
                                            var spelldelay = new Spell(SpellSlot.Unknown, 775);
                                            var pos = pred.CastPosition.Extend(Irelia.E1Pos, -i);

                                            spelldelay.SetSkillshot(0.25f +
                                                (pos.Distance(ObjectManager.Player.Position) - pred.CastPosition.Distance(ObjectManager.Player.Position)) / 2000,
                                                1f, 2000, false, EnsoulSharp.SDK.SpellType.Line
                                                );
                                            var spelldelaypred = SebbyLibPorted.Prediction.Prediction.GetPrediction(spelldelay, target);
                                            if (spelldelaypred.Hitchance >= SebbyLibPorted.Prediction.HitChance.High)
                                            {
                                                var castpos = spelldelaypred.CastPosition.Extend(Irelia.E1Pos, -i + 10);
                                                if (castpos.Distance(ObjectManager.Player.Position) <= 775)
                                                {
                                                    if (Irelia.E.Cast(castpos))
                                                        return;
                                                }
                                                else
                                                {
                                                    continue;
                                                }
                                            }
                                            else
                                            {
                                                return;
                                            }
                                        }
                                    }
                                    else
                                    {
                                        return;
                                    }
                                }
                            }
                        }
                        else
                        {
                            {
                                if (Irelia.E.GetPrediction(target).CastPosition.DistanceToPlayer() <= 800)
                                {
                                    if(ObjectManager.Player.CountEnemyHeroesInRange(775) >= 2)
                                    {
                                        foreach(var gettarget in ObjectManager.Get<AIHeroClient>().Where(i => !i.IsAlly && !i.IsDead && i.IsValidTarget(775)).OrderBy(i => i.DistanceToPlayer()))
                                        {
                                            if (gettarget == null)
                                                return;

                                            if(gettarget.NetworkId == target.NetworkId)
                                            {
                                                continue;
                                            }
                                            else
                                            {
                                                if(target.DistanceToPlayer() > gettarget.DistanceToPlayer())
                                                {
                                                    var castpos = gettarget.Position.Extend(target.Position, -200);
                                                    if(castpos.DistanceToPlayer() <= 775)
                                                    {
                                                        if (Irelia.E.Cast(castpos))
                                                            return;
                                                    }
                                                    else
                                                    {
                                                        continue;
                                                    }
                                                }
                                                else
                                                {
                                                    var castpos = target.Position.Extend(gettarget.Position, -200);
                                                    if (castpos.DistanceToPlayer() <= 775)
                                                    {
                                                        if (Irelia.E.Cast(castpos))
                                                            return;
                                                    }
                                                    else
                                                    {
                                                        continue;
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        Geometry.Circle circle = new Geometry.Circle(ObjectManager.Player.Position, 600, 50);

                                        {
                                            foreach (var onecircle in circle.Points)
                                            {
                                                if (onecircle.Distance(target) > 600)
                                                {
                                                    if (Irelia.E.Cast(onecircle))
                                                    {
                                                        var vector2 = FSpred.Prediction.Prediction.GetPrediction(target, 600);
                                                        var v3 = Vector2.Zero;
                                                        if (vector2.CastPosition.IsValid() && vector2.CastPosition.Distance(ObjectManager.Player.Position) < Irelia.E.Range - 100)
                                                            for (int j = 50; j <= 900; j += 50)
                                                            {
                                                                var vector3 = vector2.CastPosition.Extend(Irelia.E1Pos.ToVector2(), -j);
                                                                if (vector3.Distance(ObjectManager.Player) >= Irelia.E.Range && v3 != Vector2.Zero)
                                                                {
                                                                    if (Irelia.E.Cast(v3))
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
                                }
                                else return;
                            }
                            /*if (Irelia.E.GetPrediction(target).CastPosition.DistanceToPlayer() <= 800)
                            {
                                Geometry.Circle circle = new Geometry.Circle(ObjectManager.Player.Position, 600, 50);

                                {
                                    foreach (var onecircle in circle.Points)
                                    {
                                        if (onecircle.Distance(target) > 600)
                                        {
                                            if (Irelia.E.Cast(onecircle))
                                            {
                                                var vector2 = FSpred.Prediction.Prediction.GetPrediction(target, 600);
                                                var v3 = Vector2.Zero;
                                                if (vector2.CastPosition.IsValid() && vector2.CastPosition.Distance(ObjectManager.Player.Position) < Irelia.E.Range - 100)
                                                    for (int j = 50; j <= 900; j += 50)
                                                    {
                                                        var vector3 = vector2.CastPosition.Extend(Irelia.E1Pos.ToVector2(), -j);
                                                        if (vector3.Distance(ObjectManager.Player) >= Irelia.E.Range && v3 != Vector2.Zero)
                                                        {
                                                            if (Irelia.E.Cast(v3))
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
                            if (ObjectManager.Player.IsDashing())
                            {
                                if (ObjectManager.Player.GetDashInfo().EndPos.Distance(target) < 775)
                                {
                                    Geometry.Circle circle = new Geometry.Circle(ObjectManager.Player.Position, 775, 50);

                                    {
                                        foreach (var onecircle in circle.Points.Where(i => i.Distance(target) > 775))
                                        {
                                            if (Irelia.E.Cast(onecircle))
                                            {
                                                var vector2 = FSpred.Prediction.Prediction.PredictUnitPosition(target, 600);
                                                var v3 = vector2;
                                                if (vector2.IsValid() && vector2.Distance(ObjectManager.Player.Position.ToVector2()) < Irelia.E.Range - 100)
                                                    for (int j = 50; j <= 900; j += 50)
                                                    {
                                                        var vector3 = vector2.Extend(Irelia.E1Pos.ToVector2(), -j);
                                                        if (vector3.Distance(ObjectManager.Player) >= Irelia.E.Range)
                                                        {
                                                            if (Irelia.E.Cast(v3.ToVector3()))
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
                            }*/
                        }
                    }
                }
            }
        }
    }
}
