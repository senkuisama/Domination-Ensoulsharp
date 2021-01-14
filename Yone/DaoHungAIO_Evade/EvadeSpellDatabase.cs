// Copyright 2014 - 2014 Esk0r
// EvadeSpellDatabase.cs is part of Evade.
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

    using System.Collections.Generic;

    #endregion

    internal class EvadeSpellDatabase
    {
        public static List<EvadeSpellData> Spells = new List<EvadeSpellData>();

        static EvadeSpellDatabase()
        {
            if (ObjectManager.Player.CharacterName == "Janna")
            {
                //Spells.Add(new EvadeSpellData
                //{
                //    Name = "FioraQ",
                //    Slot = SpellSlot.Q,
                //    Range = 340,
                //    Delay = 50,
                //    Speed = 1100,
                //    _dangerLevel = 2
                //});

                Spells.Add(new EvadeSpellData
                {
                    Name = "EyeOfTheStorm",
                    Slot = SpellSlot.E,
                    Range = 800,
                    Delay = 200,
                    Speed = int.MaxValue,
                    _dangerLevel = 1
                });
            }
            if (ObjectManager.Player.CharacterName == "Rakan")
            {

                Spells.Add(new EvadeSpellData
                {
                    Name = "RakanE",
                    Slot = SpellSlot.E,
                    Range = 550,
                    Delay = 200,
                    Speed = int.MaxValue,
                    _dangerLevel = 1
                });
            }
            if (ObjectManager.Player.CharacterName == "Sona")
            {

                Spells.Add(new EvadeSpellData
                {
                    Name = "sonaw",
                    Slot = SpellSlot.W,
                    Range = 400,
                    Delay = 250,
                    Speed = int.MaxValue,
                    _dangerLevel = 1
                });
            }
            if (ObjectManager.Player.CharacterName == "Lulu")
            {

                Spells.Add(new EvadeSpellData
                {
                    Name = "LuluE",
                    Slot = SpellSlot.E,
                    Range = 650,
                    Delay = 250,
                    Speed = int.MaxValue,
                    _dangerLevel = 1
                });
            }
            if (ObjectManager.Player.CharacterName == "Ivern")
            {

                Spells.Add(new EvadeSpellData
                {
                    Name = "IvernE",
                    Slot = SpellSlot.E,
                    Range = 650,
                    Delay = 250,
                    Speed = int.MaxValue,
                    _dangerLevel = 1
                });
            }

        }
    }
}
