namespace DaoHungAIO.Evade
{
    #region

    using EnsoulSharp;

    using System.Collections.Generic;

    #endregion

    internal static class BlockSpellDataBase
    {
        public static List<BlockSpellData> Spells = new List<BlockSpellData>();

        static BlockSpellDataBase()
        {
            #region Alistar

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Alistar",
                    SpellSlot = SpellSlot.Q
                });

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Alistar",
                    SpellSlot = SpellSlot.W
                });

            #endregion Alistar

            #region Blitzcrank

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Blitzcrank",
                    SpellSlot = SpellSlot.E
                });

            #endregion Blitzcrank
            #region Nasus

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Nasus",
                    SpellSlot = SpellSlot.Q
                });

            #endregion Nasus

            #region Chogath

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Chogath",
                    SpellSlot = SpellSlot.R
                });

            #endregion Chogath

            #region Darius

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Darius",
                    SpellSlot = SpellSlot.R
                });

            #endregion Darius

            #region Brand

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Brand",
                    SpellSlot = SpellSlot.R
                });
            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Brand",
                    SpellSlot = SpellSlot.E
                });

            #endregion Brand
            #region Elise

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Elise",
                    SpellSlot = SpellSlot.Q
                });

            #endregion Elise

            #region FiddleSticks

             Spells.Add(
                 new BlockSpellData
                  {
                    ChampionName = "FiddleSticks",
                      SpellSlot = SpellSlot.Q
                });

            #endregion Fiddlesticks

            #region Gangplank

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Gangplank",
                    SpellSlot = SpellSlot.Q
                });

            #endregion Gangplank

            #region Garen

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Garen",
                    SpellSlot = SpellSlot.R
                });

            #endregion Garen

            #region Hecarim

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Hecarim",
                    SpellSlot = SpellSlot.E
                });

            #endregion Hecarim

            #region Irelia

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Irelia",
                    SpellSlot = SpellSlot.E
                });

            #endregion Irelia

            #region Jarvan

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Jarvan",
                    SpellSlot = SpellSlot.R
                });

            #endregion Jarvan

            #region Kalista

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Kalista",
                    SpellSlot = SpellSlot.E
                });

            #endregion Kalista

            #region Karthus

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Karthus",
                    SpellSlot = SpellSlot.R
                });

            #endregion Karthus

            #region Kayle

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Kayle",
                    SpellSlot = SpellSlot.Q
                });

            #endregion Kayle

            #region LeeSin

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "LeeSin",
                    SpellSlot = SpellSlot.R
                });

            #endregion LeeSin

            #region Lissandra

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Lissandra",
                    SpellSlot = SpellSlot.R
                });

            #endregion Lissandra

            #region Malzahar

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Malzahar",
                    SpellSlot = SpellSlot.R
                });

            #endregion Malzahar

            #region Mordekaiser

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Mordekaiser",
                    SpellSlot = SpellSlot.Q
                });

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Mordekaiser",
                    SpellSlot = SpellSlot.R
                });

            #endregion Mordekaiser

            #region Morgana

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Morgana",
                    SpellSlot = SpellSlot.R
                });

            #endregion Morgana

            #region Nasus

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Nasus",
                    SpellSlot = SpellSlot.W
                });

            #endregion Nasus

            #region Nautilus

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Nautilus",
                    SpellSlot = SpellSlot.R
                });

            #endregion Nautilus

            #region Nocturne

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Nocturne",
                    SpellSlot = SpellSlot.R
                });

            #endregion Nocturne

            #region Olaf

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Olaf",
                    SpellSlot = SpellSlot.E
                });

            #endregion Olaf

            #region Jax

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Jax",
                    SpellSlot = SpellSlot.E
                });

            #endregion Jax

            #region Pantheon

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Pantheon",
                    SpellSlot = SpellSlot.Q,
                });

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Pantheon",
                    SpellSlot = SpellSlot.W
                });

            #endregion Pantheon

            #region Renekton

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Renekton",
                    SpellSlot = SpellSlot.W
                });

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Renekton",
                    SpellSlot = SpellSlot.Q
                });
            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Renekton",
                    SpellSlot = SpellSlot.E
                });

            #endregion Renekton


            #region Rengar

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Rengar",
                    SpellSlot = SpellSlot.Q
                });

            #endregion Rengar

            #region Riven

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Riven",
                    SpellSlot = SpellSlot.Q
                });

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Riven",
                    SpellSlot = SpellSlot.W
                });

            #endregion Riven

            #region Ryze

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Ryze",
                    SpellSlot = SpellSlot.W
                });

            #endregion Ryze

            #region Singed

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Singed",
                    SpellSlot = SpellSlot.E
                });

            #endregion Singed

            #region Syndra

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Syndra",
                    SpellSlot = SpellSlot.R
                });

            #endregion Syndra

            #region TahmKench

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "TahmKench",
                    SpellSlot = SpellSlot.W
                });

            #endregion TahmKench

            #region Tristana

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Tristana",
                    SpellSlot = SpellSlot.R
                });

            #endregion Tristana

            #region Trundle

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Trundle",
                    SpellSlot = SpellSlot.R
                });

            #endregion Trundle

            #region TwistedFate

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "TwistedFate",
                    SpellSlot = SpellSlot.W
                });

            #endregion TwistedFate

            #region Veigar

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Veigar",
                    SpellSlot = SpellSlot.R
                });

            #endregion Veigar

            #region Vi

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Vi",
                    SpellSlot = SpellSlot.R
                });

            #endregion Vi


            #region Volibear

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Volibear",
                    SpellSlot = SpellSlot.Q
                });

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Volibear",
                    SpellSlot = SpellSlot.W
                });

            #endregion Volibear

            #region Warwick

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Warwick",
                    SpellSlot = SpellSlot.R
                });

            #endregion Warwick

            #region XinZhao

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "XinZhao",
                    SpellSlot = SpellSlot.Q
                });

            #endregion XinZhao

            #region Zed

            Spells.Add(
                new BlockSpellData
                {
                    ChampionName = "Zed",
                    SpellSlot = SpellSlot.R
                });

            #endregion Zed
        }

    }
}
