using EnsoulSharp;
using EnsoulSharp.SDK;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DominationAIO.NewPlugins
{
    public static class LogicQ
    {
        public static void FocusCanQTarget(AIBaseClient target)
        {
            if (target == null || !Irelia.Q.IsReady() || !MenuSettings.QSettings.Qcombo.Enabled)
                return;

            if (FunnySlayerCommon.OnAction.OnAA)
                return;

            if (Helper.CanQ(target) && target.Position.ReturnUnderTower())
            {
                var obj = ObjectManager.Get<AIBaseClient>().Where(i => 
                i.NetworkId != target.NetworkId 
                && !i.IsDead && Helper.CanQ(i) 
                && i.DistanceToPlayer() <= Irelia.Q.Range 
                && i.Position.Distance(target.Position) <= Irelia.Q.Range 
                && i.Position.ReturnUnderTower() && i.MaxHealth > 5 
                && !i.IsAlly).OrderByDescending(i => i.Health)
                .ThenByDescending(i => i.Distance(target))
                .ThenBy(i => i.Type == GameObjectType.AIMinionClient);


                if (obj != null && obj.FirstOrDefault() != null)
                {
                    var getobj = obj.FirstOrDefault();
                    if (Irelia.Q.CanCast(getobj))
                    {
                        if (Irelia.Q.Cast(getobj) == CastStates.SuccessfullyCasted)
                            return;
                        else
                            return;
                    }
                }
            }

            QGapCloserPos(target.Position);
            return;
        }


        public static bool ReturnUnderTower(this Vector3 pos)
        {
            return (!Helper.UnderTower(pos) || MenuSettings.KeysSettings.TurretKey.Active);
        }

        public static void NewHighLogic(AIBaseClient target)
        {
            if (target == null || !Irelia.Q.IsReady() || !MenuSettings.QSettings.Qcombo.Enabled)
                return;

            var nearobj = ObjectManager.Get<AIBaseClient>().FirstOrDefault(i => !i.IsDead
                                                        && Irelia.Q.CanCast(i)
                                                        && !i.IsAlly
                                                        && Helper.CanQ(i)
                                                        && i.MaxHealth > 5
                                                        && i.Distance(target) < ObjectManager.Player.GetRealAutoAttackRange() + 50);

            if (nearobj != null || Helper.CanQ(target))
            {
                var obj = ObjectManager.Get<AIBaseClient>().FirstOrDefault(i => !i.IsDead
                                    && Irelia.Q.CanCast(i)
                                    && i.IsEnemy
                                    && !i.IsAlly
                                    && Helper.CanQ(i)
                                    && i.MaxHealth > 5
                                    && i.Distance(target) < Irelia.Q.Range);

                if (obj != null && Irelia.Q.CanCast(obj))
                {
                    if (!Helper.UnderTower(obj.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                    {
                        if (Irelia.Q.Cast(obj) == CastStates.SuccessfullyCasted)
                            return;
                        else
                            return;
                    }
                    else
                    {
                        QGapCloserPos(target.Position);
                        return;
                    }
                }
                else
                {
                    QGapCloserPos(target.Position);
                    return;
                }
            }
            else
            {
                QGapCloserPos(target.Position);
                return;

            }
        }

        public static void NewExtreamLogic(AIBaseClient target)
        {
            if (target == null || !Irelia.Q.IsReady() || !MenuSettings.QSettings.Qcombo.Enabled)
                return;

            if (!Helper.CanQ(target))
            {
                NewHighLogic(target);
            }
            else
            {
                var objs = ObjectManager.Get<AIBaseClient>().Where(i =>
                !i.IsDead
                && Irelia.Q.CanCast(i)
                && !i.IsAlly
                && i.Distance(target) < ObjectManager.Player.GetRealAutoAttackRange() + MenuSettings.QSettings.BonusQ.Value
                && Helper.CanQ(i)
                && i.MaxHealth > 5)
                    .OrderByDescending(i => i.Health);


                if (objs == null)
                {
                    NewHighLogic(target);
                    return;
                }
                else
                {
                    if (objs.Count() > 1)
                    {
                        var tempobjs = ObjectManager.Get<AIBaseClient>().Where(i => !i.IsDead
                                && Irelia.Q.CanCast(i)
                                && i.IsEnemy
                                && !i.IsAlly
                                && i.Distance(objs.ToArray().FirstOrDefault()) <= Irelia.Q.Range
                                && Helper.CanQ(i)
                                && i.MaxHealth > 5)
                                    .OrderByDescending(i => i.Distance(objs.ToArray().FirstOrDefault()));

                        var newobj = tempobjs.FirstOrDefault(tempobj =>
                        (tempobj.Distance(ObjectManager.Player.Position) <= (Irelia.Q.Range) && Irelia.Q.CanCast(tempobj))
                        && (!Helper.UnderTower(tempobj.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                        );

                        if(newobj == null || newobj.NetworkId == target.NetworkId || newobj.DistanceToPlayer() > Irelia.Q.Range || newobj.DistanceToPlayer() >= 600f)
                        {
                            NewHighLogic(target);
                            return;
                        }else
                        {
                            if(Irelia.Q.CanCast(newobj))
                            if (Irelia.Q.Cast(newobj) == CastStates.SuccessfullyCasted)
                            {
                                if (objs.ToArray().FirstOrDefault().DistanceToPlayer() < 600f)
                                    if (Irelia.Q.Cast(objs.ToArray().FirstOrDefault()) == CastStates.SuccessfullyCasted)
                                        return;
                                        else
                                            return;
                                }
                            else
                            {
                                NewHighLogic(target);
                                return;
                            }
                            else
                            {
                                NewHighLogic(target);
                                return;
                            }
                        }                    
                    }
                    else
                    {
                        NewHighLogic(target);
                        return;
                    }
                }
            }
        }

        public static void HighLogic(AIBaseClient target)
        {
            if (target == null)
                return;

            var objs = ObjectManager.Get<AIBaseClient>().Where(i => i != null
                                    && !i.IsDead
                                    && i.IsEnemy
                                    && !i.IsAlly
                                    && Helper.CanQ(i)
                                    && i.MaxHealth > 5
                                    && i.Distance(target) < Irelia.Q.Range)
                                    .OrderByDescending(i => i.Health).ThenByDescending(i => i.Distance(target));

            var obj1 = ObjectManager.Get<AIBaseClient>().Where(i => Irelia.Q.CanCast(i)
                                  && !i.IsDead
                                  && !i.IsAlly
                                  && Helper.CanQ(i)
                                  && i.Position.Distance(target.Position) <= ObjectManager.Player.Distance(target.Position) + ObjectManager.Player.GetRealAutoAttackRange()).OrderBy(i => i.DistanceToPlayer()).FirstOrDefault();

            if (Helper.CanQ(target))
            {
                //if (objPlayer.HealthPercent <= MenuSettings.QSettings.QHeath.Value && MenuSettings.QSettings.QDancing.Enabled)
                //{
                if (objs != null)
                {
                    foreach (var obj in objs)
                    {
                        if (obj != null)
                        {
                            if (obj.IsValidTarget(Irelia.Q.Range))
                            {
                                if (obj.Distance(target) < Irelia.Q.Range)
                                {
                                    if (!Helper.UnderTower(obj.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                                    {
                                        if (Irelia.Q.Cast(obj) == CastStates.SuccessfullyCasted)
                                            return;
                                    }
                                    else
                                    {
                                        {
                                            if (Irelia.Q.IsReady() && obj1 != null && obj1.DistanceToPlayer() <= 600f)
                                            {
                                                if (!Helper.UnderTower(obj1.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                                                    if (Irelia.Q.Cast(obj1) == CastStates.SuccessfullyCasted)
                                                        return;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    {
                                        if (Irelia.Q.IsReady() && obj1 != null && obj1.DistanceToPlayer() <= 600f)
                                        {
                                            if (!Helper.UnderTower(obj1.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                                                if (Irelia.Q.Cast(obj1) == CastStates.SuccessfullyCasted)
                                                    return;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                {
                                    if (Irelia.Q.IsReady() && obj1 != null && obj1.DistanceToPlayer() <= 600f)
                                    {
                                        if (!Helper.UnderTower(obj1.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                                            if (Irelia.Q.Cast(obj1) == CastStates.SuccessfullyCasted)
                                                return;
                                    }
                                }
                            }
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
                //}
                //else
                //{
                //    QGapCloserPos(target.Position);
                //}
            }
            else
            {
                var targets = ObjectManager.Get<AIHeroClient>().Where(i => i != null && !i.IsDead && !i.IsAlly && i.IsValidTarget(1500)).OrderBy(i => i.Health).ThenBy(i => i.DistanceToPlayer());
                if (targets != null)
                {
                    foreach (var t in targets)
                    {
                        if (Helper.CanQ(t))
                        {
                            //if (objPlayer.HealthPercent <= MenuSettings.QSettings.QHeath.Value && MenuSettings.QSettings.QDancing.Enabled)
                            //{
                            if (objs != null)
                            {
                                foreach (var obj in objs)
                                {
                                    if (obj != null)
                                    {
                                        if (obj.IsValidTarget(Irelia.Q.Range))
                                        {
                                            if (obj.Distance(t) < Irelia.Q.Range)
                                            {
                                                if (!Helper.UnderTower(obj.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                                                {
                                                    if (Irelia.Q.Cast(obj) == CastStates.SuccessfullyCasted)
                                                        return;
                                                }
                                                else
                                                {
                                                    {
                                                        if (Irelia.Q.IsReady() && obj1 != null && obj1.DistanceToPlayer() <= 600f)
                                                        {
                                                            if (!Helper.UnderTower(obj1.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                                                                if (Irelia.Q.Cast(obj1) == CastStates.SuccessfullyCasted)
                                                                    return;
                                                        }
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                {
                                                    if (Irelia.Q.IsReady() && obj1 != null && obj1.DistanceToPlayer() <= 600f)
                                                    {
                                                        if (!Helper.UnderTower(obj1.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                                                            if (Irelia.Q.Cast(obj1) == CastStates.SuccessfullyCasted)
                                                                return;
                                                    }
                                                }
                                            }
                                        }
                                        else
                                        {
                                            {
                                                if (Irelia.Q.IsReady() && obj1 != null && obj1.DistanceToPlayer() <= 600f)
                                                {
                                                    if (!Helper.UnderTower(obj1.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                                                        if (Irelia.Q.Cast(obj1) == CastStates.SuccessfullyCasted)
                                                            return;
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        {
                                            if (Irelia.Q.IsReady() && obj1 != null && obj1.DistanceToPlayer() <= 600f)
                                            {
                                                if (!Helper.UnderTower(obj1.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                                                    if (Irelia.Q.Cast(obj1) == CastStates.SuccessfullyCasted)
                                                        return;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                {
                                    if (Irelia.Q.IsReady() && obj1 != null && obj1.DistanceToPlayer() <= 600f)
                                    {
                                        if (!Helper.UnderTower(obj1.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                                            if (Irelia.Q.Cast(obj1) == CastStates.SuccessfullyCasted)
                                                return;
                                    }
                                }
                            }
                            //}
                            //else
                            //{
                            //    QGapCloserPos(target.Position);
                            //}
                        }
                        else
                        {
                            var aobjs = ObjectManager.Get<AIBaseClient>().Where(i => i != null
                                                    && !i.IsDead
                                                    && i.IsEnemy
                                                    && !i.IsAlly
                                                    && Helper.CanQ(i)
                                                    && i.MaxHealth > 5)
                                                        .OrderByDescending(i => i.Health).ThenByDescending(i => i.Distance(t));

                            if (aobjs != null)
                            {
                                var nearobj = ObjectManager.Get<AIBaseClient>().Where(i => i != null
                                                        && !i.IsDead
                                                        && i.IsEnemy
                                                        && !i.IsAlly
                                                        && Helper.CanQ(i)
                                                        && i.MaxHealth > 5
                                                        && i.Distance(target) < ObjectManager.Player.GetRealAutoAttackRange() + 50)
                                                            .OrderByDescending(i => i.Health).ThenByDescending(i => i.Distance(target)).FirstOrDefault();

                                if (nearobj != null)
                                {
                                    var bobj = aobjs.Where(i => i.Distance(nearobj.Position) < Irelia.Q.Range && (!Helper.UnderTower(i.Position) || MenuSettings.KeysSettings.TurretKey.Active)).OrderByDescending(i => i.Distance(nearobj)).FirstOrDefault();

                                    if (bobj != null && bobj.DistanceToPlayer() < 600f)
                                    {
                                        if (Irelia.Q.Cast(bobj) == CastStates.SuccessfullyCasted)
                                            return;
                                    }
                                    else
                                    {
                                        {
                                            if (Irelia.Q.IsReady() && obj1 != null && obj1.DistanceToPlayer() <= 600f)
                                            {
                                                if (!Helper.UnderTower(obj1.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                                                    if (Irelia.Q.Cast(obj1) == CastStates.SuccessfullyCasted)
                                                        return;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    {
                                        if (Irelia.Q.IsReady() && obj1 != null && obj1.DistanceToPlayer() <= 600f)
                                        {
                                            if (!Helper.UnderTower(obj1.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                                                if (Irelia.Q.Cast(obj1) == CastStates.SuccessfullyCasted)
                                                    return;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    {                      
                        if (Irelia.Q.IsReady() && obj1 != null && obj1.DistanceToPlayer() <= 600f)
                        {
                            if (!Helper.UnderTower(obj1.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                                if (Irelia.Q.Cast(obj1) == CastStates.SuccessfullyCasted)
                                    return;
                        }
                    }
                }       
            }
        }
        public static void ExtreamLogic(AIBaseClient target)
        {
            if (target == null)
                return;
           
            if (Helper.CanQ(target))
            {
                //if (objPlayer.HealthPercent <= MenuSettings.QSettings.QHeath.Value)
                //{
                    var objs = ObjectManager.Get<AIBaseClient>().Where(i => i != null
                    && !i.IsDead
                    && i.IsEnemy
                    && !i.IsAlly
                    && i.Distance(target) < ObjectManager.Player.GetRealAutoAttackRange() + 50
                    && Helper.CanQ(i)
                    && i.MaxHealth > 5)
                        .OrderByDescending(i => i.Health).ThenByDescending(i => i.Distance(target)).ThenByDescending(i => i.Distance(target));

                    if (objs != null)
                    {
                        if (objs.Count() > 1)
                        {
                            foreach (var obj in objs)
                            {
                                if (obj != null)
                                {
                                    var tempobjs = ObjectManager.Get<AIBaseClient>().Where(i => i != null
                                    && !i.IsDead
                                    && i.IsEnemy
                                    && !i.IsAlly
                                    && i.Distance(obj) < Irelia.Q.Range
                                    && Helper.CanQ(i)
                                    && i.MaxHealth > 5)
                                        .OrderByDescending(i => i.Health).ThenByDescending(i => i.Distance(obj));

                                    if (tempobjs != null)
                                    {
                                        foreach (var tempobj in tempobjs)
                                        {
                                            if (tempobj != null)
                                            {
                                                if (tempobj.Distance(ObjectManager.Player.Position) <= (Irelia.Q.Range))
                                                {
                                                    if (!Helper.UnderTower(tempobj.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                                                    {
                                                        if (Irelia.Q.Cast(tempobj) == CastStates.SuccessfullyCasted)
                                                        {
                                                            EnsoulSharp.SDK.Utility.DelayAction.Add(300, () =>
                                                            {
                                                                if (obj.DistanceToPlayer() < 600f)
                                                                    if (Irelia.Q.Cast(obj) == CastStates.SuccessfullyCasted)
                                                                        return;
                                                                    else
                                                                        return;
                                                            });
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
                                                    HighLogic(target);
                                                }
                                            }
                                            else
                                            {
                                                HighLogic(target);
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
                //}
                //else
                //{
                //    HighLogic(target);
                //    return;
                //}
            }
            else
            {
                var targets = ObjectManager.Get<AIHeroClient>().Where(i => i != null && !i.IsDead && !i.IsAlly && i.IsValidTarget(3000)).OrderBy(i => i.Distance(ObjectManager.Player)).ThenBy(i => i.Health);
                if (targets != null)
                {
                    foreach (var t in targets)
                    {
                        if (Helper.CanQ(t))
                        {
                            //if (objPlayer.HealthPercent <= MenuSettings.QSettings.QHeath.Value && MenuSettings.QSettings.QDancing.Enabled)
                            //{
                            var objs = ObjectManager.Get<AIBaseClient>().Where(i => i != null
                            && !i.IsDead
                            && i.IsEnemy
                            && !i.IsAlly
                            && i.Distance(t) < ObjectManager.Player.GetRealAutoAttackRange() + 50
                            && Helper.CanQ(i)
                            && i.MaxHealth > 5)
                                .OrderByDescending(i => i.Health).ThenByDescending(i => i.Distance(t)).ThenByDescending(i => i.Distance(t));

                            if (objs != null)
                            {
                                if (objs.Count() > 1)
                                {
                                    foreach (var obj in objs)
                                    {
                                        if (obj != null)
                                        {
                                            var tempobjs = ObjectManager.Get<AIBaseClient>().Where(i => i != null
                                            && !i.IsDead
                                            && i.IsEnemy
                                            && !i.IsAlly
                                            && i.Distance(obj) < Irelia.Q.Range
                                            && Helper.CanQ(i)
                                            && i.MaxHealth > 5)
                                                .OrderByDescending(i => i.Health).ThenByDescending(i => i.Distance(obj));

                                            if (tempobjs != null)
                                            {
                                                foreach (var tempobj in tempobjs)
                                                {
                                                    if (tempobj != null)
                                                    {
                                                        if (tempobj.Distance(ObjectManager.Player.Position) <= (Irelia.Q.Range))
                                                        {
                                                            if (!Helper.UnderTower(tempobj.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                                                            {
                                                                if (Irelia.Q.Cast(tempobj) == CastStates.SuccessfullyCasted)
                                                                {
                                                                    EnsoulSharp.SDK.Utility.DelayAction.Add(300, () =>
                                                                    {
                                                                        if (obj.DistanceToPlayer() <= 600f)
                                                                            if (Irelia.Q.Cast(obj) == CastStates.SuccessfullyCasted)
                                                                                return;
                                                                            else
                                                                                return;
                                                                    });
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
                                                            HighLogic(target);
                                                        }
                                                    }
                                                    else
                                                    {
                                                        HighLogic(target);
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
                                }
                                else
                                {
                                    HighLogic(t);
                                }
                            }
                            else
                            {
                                HighLogic(target);
                            }
                            //}
                            //else
                            //{
                            //    HighLogic(target);
                            //}
                        }
                        else
                        {
                            HighLogic(target);
                        }
                    }
                }
                else
                {
                    HighLogic(target);
                }
            }
        }
        public static void QGapCloserPos(Vector3 pos)
        {
            var obj = ObjectManager.Get<AIBaseClient>().Where(i => Irelia.Q.CanCast(i)
            && !i.IsDead
            && !i.IsAlly
            && Helper.CanQ(i)
            && i.Position.Distance(pos) <= ObjectManager.Player.Distance(pos) + MenuSettings.QSettings.BonusQ.Value).OrderBy(i => i.DistanceToPlayer()).FirstOrDefault();

            if (Irelia.Q.IsReady() && obj != null && obj.DistanceToPlayer() <= 600f)
            {
                if (!Helper.UnderTower(obj.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                    if (Irelia.Q.Cast(obj) == CastStates.SuccessfullyCasted)
                        return;
            }
            else
            {
                return;
            }
        }

        public static void GapCloserTargetCanKillable()
        {
            var target = GameObjects.EnemyHeroes.Where(i => !i.IsDead && i.IsValidTarget(2000) && Helper.CanQ(i)).OrderBy(i => i.Health).ThenBy(i => i.DistanceToPlayer()).FirstOrDefault();
            if (target == null || !Irelia.Q.IsReady()) return;

            if (target.IsValidTarget(Irelia.Q.Range))
            {
                if (!Helper.UnderTower(target.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                {
                    if (Irelia.Q.Cast(target) == CastStates.SuccessfullyCasted)
                        return;
                }
            }

            var gapobjs = ObjectManager.Get<AIBaseClient>().Where(i => !i.IsAlly && !i.IsDead && i.IsValidTarget(2000) && Helper.CanQ(i)).ToArray();
            if (gapobjs.Any())
            {
                foreach (var obj1 in gapobjs)
                {
                    if (obj1.Distance(target) < Irelia.Q.Range && Irelia.Q.CanCast(target))
                    {
                        if (!Helper.UnderTower(obj1.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                        {
                            if (Irelia.Q.Cast(obj1) == CastStates.SuccessfullyCasted)
                                return;
                        }
                    }

                    if (gapobjs.Count() >= 2)
                    {
                        var check = gapobjs.Where(i => Irelia.Q.CanCast(i));
                        foreach (var obj2 in check)
                        {
                            if (obj1.Distance(target) < Irelia.Q.Range)
                            {
                                if (obj2.Distance(obj1) < Irelia.Q.Range)
                                {
                                    if (!Helper.UnderTower(obj2.Position) || MenuSettings.KeysSettings.TurretKey.Active)
                                    {
                                        if (Irelia.Q.Cast(obj2) == CastStates.SuccessfullyCasted)
                                            return;
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
