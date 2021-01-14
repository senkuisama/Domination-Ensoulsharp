// Copyright 2014 - 2014 Esk0r
// EvadeSpellData.cs is part of Evade.
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
    using EnsoulSharp.SDK.MenuUI;


    #endregion

    public class EvadeSpellData
    {
        public delegate float MoveSpeedAmount();

        public bool CanShieldAllies;
        public string CheckSpellName = "";
        public int Delay;
        public bool IsDash;
        public bool IsInvulnerability;
        public bool IsMovementSpeedBuff;
        public bool IsShield;
        public bool IsSpellShield;
        public float Range;
        public string Name;
        public bool SelfCast;
        public SpellSlot Slot;
        public int Speed;
        public int _dangerLevel;

        public EvadeSpellData() { }

        public EvadeSpellData(string name, SpellSlot slot, float range, int delay, int speed, int dangerLevel,
            bool isSpellShield = false)
        {
            Name = name;
            Slot = slot;
            Range = range;
            Delay = delay;
            Speed = speed;
            _dangerLevel = dangerLevel;
            IsSpellShield = isSpellShield;
        }

        public int DangerLevel
        {
            get
            {
                return EvadeManager.EvadeSpellMenu["DangerLevel_" + Slot] == null
                    ? _dangerLevel
                    : EvadeManager.EvadeSpellMenu["DangerLevel_" + Slot].GetValue<MenuSlider>().Value;
            }
            set { _dangerLevel = value; }
        }

        public bool Enabled => EvadeManager.EvadeSpellMenu["Enabled" + Slot] == null || 
            EvadeManager.EvadeSpellMenu["Enabled" + Slot].GetValue<MenuBool>().Enabled;

        public bool Tower => EvadeManager.EvadeSpellMenu["Tower" + Slot] == null ||
            EvadeManager.EvadeSpellMenu["Tower" + Slot].GetValue<MenuBool>().Enabled;

        public bool IsReady()
        {
            return (CheckSpellName == "" || ObjectManager.Player.Spellbook.GetSpell(Slot).Name == CheckSpellName) &&
                   ObjectManager.Player.Spellbook.CanUseSpell(Slot)==SpellState.Ready;
        }
    }
}