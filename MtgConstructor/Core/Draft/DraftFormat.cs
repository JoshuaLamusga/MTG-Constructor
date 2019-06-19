namespace MtgConstructor.Draft
{
    public enum DraftFormat
    {
        /// <summary>
        /// Each player opens 6 booster packs to construct a deck.
        /// </summary>
        SealedDeck,

        /// <summary>
        /// Players open a booster pack, select a card, and pass everything
        /// else to the left, select a card, etc. until there are no cards
        /// left to pass around. Repeat this process two more times, except
        /// pass to the right in the second round. Players add basic lands as
        /// desired. Minimum deck size is 40.
        /// </summary>
        BoosterDraft,

        /// <summary>
        /// Same as booster draft, except decks are randomly swapped after
        /// deck so nobody has the deck they constructed. In matches,
        /// an extra point is awarded for anyone that built a losing deck.
        /// </summary>
        BoosterBackDraft,

        /// <summary>
        /// Players draft cards similar to a booster draft, but cards are
        /// randomly picked from a pool.
        /// </summary>
        CubeDraft,

        /// <summary>
        /// Same as cube draft, except decks are randomly swapped after
        /// deck so nobody has the deck they constructed. In matches,
        /// an extra point is awarded for anyone that built a losing deck.
        /// </summary>
        CubeBackDraft,

        /// <summary>
        /// Each player starts with a random basic land only they know about,
        /// then players each open a booster pack to which another random basic
        /// land is publicly added, then draft their hand of 7 cards like
        /// normal. No mulligans are allowed.
        /// </summary>
        LiveBoosterDraft
    }
}
