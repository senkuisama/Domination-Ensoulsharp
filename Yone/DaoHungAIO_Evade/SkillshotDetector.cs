// Copyright 2014 - 2014 Esk0r
// SkillshotDetector.cs is part of Evade.
// 
// Evade is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// Evade is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with Evade. If not, see <http://www.gnu.org/licenses/>.

// GitHub: https://github.com/Esk0r/LeagueSharp/blob/master/Evade

namespace DaoHungAIO.Evade
{
    #region

    using EnsoulSharp;
    using EnsoulSharp.SDK;
    using EnsoulSharp.SDK.Utility;
    using SharpDX;
    using System;
    using System.Linq;
    using System.Text.RegularExpressions;

    using DominationAIO.Common;

    #endregion

    internal static class SkillshotDetector
    {
        private static IOrderedEnumerable<AIHeroClient> bestAllies;

        public delegate void OnDeleteMissileH(Skillshot skillshot, MissileClient missile);

        public delegate void OnDetectSkillshotH(Skillshot skillshot);

        public static event OnDetectSkillshotH OnDetectSkillshot;
        public static event OnDeleteMissileH OnDeleteMissile;

        static SkillshotDetector()
        {
            AIBaseClient.OnProcessSpellCast += OnProcessSpellCast;
            GameObject.OnDelete += MissileOnDelete;
            GameObject.OnCreate += MissileOnCreate;
            GameObject.OnCreate += OnCreate;
            GameObject.OnDelete += OnDelete;
        }

        private static void OnCreate(GameObject sender,EventArgs args)
        {
            if(sender == null)
            {
                return;
            }
            var spellData = SpellDatabase.GetBySourceObjectName(sender.Name);

            if (spellData == null)
            {
                return;
            }

            if (EvadeManager.SkillShotMenu["Evade" + spellData.CharacterName.ToLower()][
                    "Enabled" + spellData.MenuItemName] == null)
            {
                return;
            }

            TriggerOnDetectSkillshot(DetectionType.ProcessSpell, spellData, Utils.GameTimeTickCount - Game.Ping / 2,
                sender.Position.ToVector2(), sender.Position.ToVector2(), sender.Position.ToVector2(),
                GameObjects.Heroes.MinOrDefault(h => h.IsAlly ? 1 : 0));
        }

        private static void OnDelete(GameObject sender, EventArgs args)
        {
            if (!sender.IsValid)
            {
                return;
            }

            for (var i = EvadeManager.DetectedSkillshots.Count - 1; i >= 0; i--)
            {
                var skillshot = EvadeManager.DetectedSkillshots[i];

                if (skillshot.SpellData.ToggleParticleName != "" &&
                    new Regex(skillshot.SpellData.ToggleParticleName).IsMatch(sender.Name))
                {
                    EvadeManager.DetectedSkillshots.RemoveAt(i);
                }
            }
        }

        private static void MissileOnCreate(GameObject sender,EventArgs args)
        {


            var missile = sender as MissileClient;

            if (missile == null || !missile.IsValid)
            {
                return;
            }


            var unit = missile.SpellCaster as AIHeroClient;

            if (unit == null || !unit.IsValid || unit.Team == ObjectManager.Player.Team)
            {
                return;
            }

            var spellData = SpellDatabase.GetByMissileName(missile.SData.Name);

            if (spellData == null)
            {
                return;
            }
   
            DelayAction.Add(0, delegate
            {
                ObjSpellMissionOnOnCreateDelayed(sender);
            });
        }

        private static void ObjSpellMissionOnOnCreateDelayed(GameObject sender)
        {

            var missile = sender as MissileClient;

            if (missile == null || !missile.IsValid)
            {
                return;
            }

            var unit = missile.SpellCaster as AIHeroClient;

            if (unit == null || !unit.IsValid || unit.Team == ObjectManager.Player.Team)
            {
                return;
            }

            var spellData = SpellDatabase.GetByMissileName(missile.SData.Name);

            if (spellData == null)
            {
                return;
            }

            var missilePosition = missile.Position.ToVector2();
            var unitPosition = missile.StartPosition.ToVector2();
            var endPos = missile.EndPosition.ToVector2();
            var direction = (endPos - unitPosition).Normalized();

            if (unitPosition.Distance(endPos) > spellData.Range || spellData.FixedRange)
            {
                endPos = unitPosition + direction * spellData.Range;
            }

            if (spellData.ExtraRange != -1)
            {
                endPos = endPos +
                         Math.Min(spellData.ExtraRange, spellData.Range - endPos.Distance(unitPosition)) * direction;
            }
 
            var castTime = Utils.GameTimeTickCount - Game.Ping / 2 - (spellData.MissileDelayed ? 0 : spellData.Delay) -
                           (int) (1000f * missilePosition.Distance(unitPosition) / spellData.MissileSpeed);
            
            TriggerOnDetectSkillshot(DetectionType.RecvPacket, spellData, castTime, unitPosition, endPos, endPos, unit);
        }

        private static void MissileOnDelete(GameObject sender, EventArgs args)
        {
            var missile = sender as MissileClient;

            if (missile == null || !missile.IsValid)
            {
                return;
            }

            var caster = missile.SpellCaster as AIHeroClient;

            if (caster == null || !caster.IsValid || caster.Team == ObjectManager.Player.Team)
            {
                return;
            }

            var spellName = missile.SData.Name;

            if (OnDeleteMissile != null)
            {
                foreach (var skillshot in EvadeManager.DetectedSkillshots)
                {
                    if (
                        skillshot.SpellData.MissileSpellName.Equals(spellName,
                            StringComparison.InvariantCultureIgnoreCase) &&
                        skillshot.Unit.NetworkId == caster.NetworkId &&
                        (missile.EndPosition.ToVector2() - missile.StartPosition.ToVector2()).AngleBetween(skillshot.Direction) <
                        10 && skillshot.SpellData.CanBeRemoved)
                    {
                        OnDeleteMissile(skillshot, missile);
                        break;
                    }
                }
            }

            EvadeManager.DetectedSkillshots.RemoveAll(
                skillshot =>
                    (skillshot.SpellData.MissileSpellName.Equals(spellName,
                         StringComparison.InvariantCultureIgnoreCase) ||
                     skillshot.SpellData.ExtraMissileNames.Contains(spellName,
                         StringComparer.InvariantCultureIgnoreCase)) &&
                    (skillshot.Unit.NetworkId == caster.NetworkId &&
                     (missile.EndPosition.ToVector2() - missile.StartPosition.ToVector2()).AngleBetween(skillshot.Direction) <
                     10 &&
                     skillshot.SpellData.CanBeRemoved || skillshot.SpellData.ForceRemove));
        }


        internal static void TriggerOnDetectSkillshot(DetectionType detectionType, SpellData spellData, int startT,
            Vector2 start, Vector2 end, Vector2 originalEnd, AIBaseClient unit)
        {

            var skillshot = new Skillshot(detectionType, spellData, startT, start, end, unit)
            {
                OriginalEnd = originalEnd
            };

            OnDetectSkillshot?.Invoke(skillshot);
        }

        private static void OnProcessSpellCast(AIBaseClient sender, AIBaseClientProcessSpellCastEventArgs args)
        {
            if (sender == null || !sender.IsValid || !(sender is AIHeroClient))
            {
                return;
            }

            if (args.SData.Name == "dravenrdoublecast")
            {
                EvadeManager.DetectedSkillshots.RemoveAll(
                    s => s.Unit.NetworkId == sender.NetworkId && s.SpellData.SpellName == "DravenRCast");
            }

            if (!sender.IsValid || sender.Team == ObjectManager.Player.Team)
            {
                return;
            }
            var spellData = SpellDatabase.GetByName(args.SData.Name);
            if (spellData == null)
            {
                return;
            }

            var startPos = new Vector2();

            if (spellData.FromObject != "")
            {
                foreach (var o in ObjectManager.Get<GameObject>())
                {
                    if (o.Name.Contains(spellData.FromObject))
                    {
                        startPos = o.Position.ToVector2();
                    }
                }
            }
            else
            {
                startPos = sender.Position.ToVector2();
            }

            if (spellData.FromObjects != null && spellData.FromObjects.Length > 0)
            {
                foreach (var obj in ObjectManager.Get<GameObject>())
                {
                    if (obj.IsEnemy && spellData.FromObjects.Contains(obj.Name))
                    {
                        var start = obj.Position.ToVector2();
                        var end = start + spellData.Range * (args.End.ToVector2() - obj.Position.ToVector2()).Normalized();

                        TriggerOnDetectSkillshot(
                            DetectionType.ProcessSpell, spellData, Utils.GameTimeTickCount - Game.Ping / 2, start, end,
                            end,
                            sender);
                    }
                }
            }

            if (!startPos.IsValid())
            {
                return;
            }

            var endPos = args.End.ToVector2();
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

            if (bestAllies == null)
                return;
            foreach (var ally in bestAllies)
            {
                if (spellData.SpellName == "LucianQ" && args.Target != null &&
                    args.Target.NetworkId == ally.NetworkId)
                {
                    return;
                }


                var direction = (endPos - startPos).Normalized();

                if (startPos.Distance(endPos) > spellData.Range || spellData.FixedRange)
                {
                    endPos = startPos + direction * spellData.Range;
                }


                if (spellData.ExtraRange != -1)
                {
                    endPos = endPos +
                             Math.Min(spellData.ExtraRange, spellData.Range - endPos.Distance(startPos)) * direction;
                }

                TriggerOnDetectSkillshot(
                    DetectionType.ProcessSpell, spellData, Utils.GameTimeTickCount - Game.Ping / 2, startPos, endPos,
                    args.End.ToVector2(), sender);
            }
        }
    }
}
