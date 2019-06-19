namespace MtgConstructor.Data
{
	public enum DeckFormat
	{
		/// <summary>
		/// Limits the card type to Scheme.
		/// </summary>
		Archenemy,

		/// <summary>
		/// Limits the card pool to core sets and most recent two block sets.
		/// Limits to 60 unique cards, except basic lands. One card must be
		/// a legendary creature or planeswalker, or explicitly permitted,
		/// to serve as commander. Some commanders permit a second commander.
		/// </summary>
		Brawl,

		/// <summary>
		/// No restrictions.
		/// </summary>
		Casual,

		/// <summary>
		/// Limits to 100 unique cards, except basic lands. One card must
		/// be a legendary creature, or explicitly permitted, to serve as
		/// commander. Some commanders permit a second commander.
		/// </summary>
		Commander,

		/// <summary>
		/// Limits to one of each card in a 360 or 720 card pool. Not
		/// intended to contain basic lands.
		/// </summary>
		Cube,

		/// <summary>
		/// Limits the card pool to Magic 2015 and later-released sets, bans cards.
		/// </summary>
		Frontier,

		/// <summary>
		/// Bans a superset of the cards listed in Vintage.
		/// </summary>
		Legacy,

		/// <summary>
		/// Limits to 15 cards, bans cards.
		/// </summary>
		MiniMagic,

		/// <summary>
		/// Limits the card pool to 8th Edition and later-released sets, bans cards.
		/// </summary>
		Modern,

		/// <summary>
		/// Limits the card pool to 60 basic lands plus the Momir Vig,
		/// Simic Visionary vanguard card.
		/// </summary>
		Momir,

		/// <summary>
		/// Limits rarity to common.
		/// </summary>
		Pauper,

		/// <summary>
		/// Limits rarity to common with up to 5 uncommon, bans cards.
		/// </summary>
		Peasant,

		/// <summary>
		/// Limits the card type to Plane and Phenomenon.
		/// </summary>
		Planar,

		/// <summary>
		/// Limits to 250 or more cards, with 20 of each color. Multicolored cards
		/// count can count as any one of their colors. Bans cards. 
		/// </summary>
		Prismatic,

		/// <summary>
		/// Limits to sets printed since Alpha until Onslaught.
		/// </summary>
		QLMagic,

		/// <summary>
		/// Limits deck to have cards with CMC 1, 2, 3, 4, 5, and 6 in each
		/// color, counting colorless, with 4 copies of each basic land, not
		/// including colorless.
		/// </summary>
		RainbowStairwell,

		/// <summary>
		/// Limits to one of each card, except for basic lands.
		/// </summary>
		Singleton,

		/// <summary>
		/// Limits the card pool to core sets and most recent two block sets, bans cards.
		/// </summary>
		Standard,

        /// <summary>
        /// Limits to 50 unique cmc 3 or less cards, except basic lands. One
        /// card must be a legendary creature, or explicitly permitted, to
        /// serve as commander. Some commanders permit a second commander.
        /// Bans cards.
        /// </summary>
        TinyLeaders,

        /// <summary>
        /// Requires 1/3 of deck to share a creature type, bans cards.
        /// </summary>
        TribalWars,

		/// <summary>
		/// Bans cards that involve outdated gameplay styles. Restricts
		/// powerful cards.
		/// </summary>
		Vintage,

        /// <summary>
        /// Minimum deck size of 40. Non-basic land cards must be unique. Up to
        /// 3 rare or mythic cards allowed, and up to 9 uncommon.
        /// </summary>
        Warband
    }
}
