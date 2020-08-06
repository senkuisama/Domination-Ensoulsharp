namespace DaoHungAIO.Evade
{
    #region

    using EnsoulSharp;

    #endregion

    internal class BlockSpellData
    {
        public SpellSlot SpellSlot { get; set; }
        public string ChampionName { get; set; }

        public string CharacterName { get { return ChampionName; } }
    }
}
