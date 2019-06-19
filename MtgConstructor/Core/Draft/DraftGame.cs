using MtgConstructor.Cards;
using MtgConstructor.Cards.Parse;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MtgConstructor.Draft
{
    /// <summary>
    /// Simulates drafting with various options.
    /// </summary>
    public class DraftGame
    {
        private DraftFormat format;
        private int numPlayers;
        private List<Game.Player> players;
        private int activePlayer;

        /// <summary>
        /// Calls handlers with the list of players and the index
        /// of the first player in the draft.
        /// </summary>
        public event Action<List<Game.Player>, int> OnDraftStarted;

        #region Constructors
        /// <summary>
        /// Creates a booster-based draft game.
        /// Throws an argument exception if a non-booster format is specified.
        /// </summary>
        public DraftGame(DraftFormat format, int numPlayers, string boosterSet)
        {
            this.format = format;
            this.numPlayers = numPlayers;
            players = new List<Game.Player>();
            activePlayer = 0;
        }

        /// <summary>
        /// Creates a constructed-based draft game.
        /// Throws an argument exception if a booster format is specified.
        /// </summary>
        public DraftGame(DraftFormat format, int numPlayers, List<CardCollection> pool)
        {

        }
        #endregion

        /// <summary>
        /// Creates players and selects one at random to start, then raises
        /// OnDraftStarted.
        /// </summary>
        public void StartDraft()
        {
            players.Clear();
            for (int i = 0; i < numPlayers; i++)
            {
                players.Add(new Game.Player(0, null));
            }

            activePlayer = Game.Game.Rng.Next(players.Count);
            OnDraftStarted?.Invoke(players, activePlayer);
        }

        /// <summary>
        /// Progresses to the next card. If progressing from the last player,
        /// rotates back to the first, proceeding with the draft logically.
        /// </summary>
        public void NextCard()
        {
            // TODO: Read types of drafts and get the next card.

            activePlayer++;
            if (activePlayer == players.Count)
            {
                activePlayer = 0;
            }
        }

        /// <summary>
        /// Generates a booster pack from the given pool. Cards included in
        /// the booster will be removed from the pool if true. Boosters have
        /// 1 basic land, 10 commons, 3 uncommons, 1 rare with a 1/8 chance of
        /// being mythic rare.
        /// 
        /// Warning: Arguments for pool aren't checked; this may cause an
        /// infinite loop if there aren't enough of each kind of rarity to
        /// generate a booster from.
        /// </summary>
        public static List<CardInfo> GenerateBooster(List<CardInfo> pool, bool removeFromPool)
        {
            List<CardInfo> collection = new List<CardInfo>();
            bool hasRare = false;
            int numCommons = 0;
            int numUncommons = 0;

            // Adds a basic (non-Snow) land.
            var lands = pool.Where(o =>
                o.GetTypes().Contains("Basic") &&
                !o.GetTypes().Contains("Snow")).ToList();

            collection.Add(lands[Game.Game.Rng.Next(lands.Count)]);
            if (removeFromPool)
            {
                pool.Remove(collection[0]);
            }

            while (!hasRare || numCommons < 10 || numUncommons < 3)
            {
                CardInfo cardAdded = null;
                CardInfo card = pool[Game.Game.Rng.Next(pool.Count)];

                if (card.GetTypes().Contains("Basic"))
                {
                    continue;
                }

                if ((!hasRare && card.Rarity == CardInfo.Rarities.Mythic && Game.Game.Rng.Next(8) == 0) ||
                    (!hasRare && card.Rarity == CardInfo.Rarities.Rare))
                {
                    hasRare = true;
                    cardAdded = card;
                }
                else if (card.Rarity == CardInfo.Rarities.Uncommon && numUncommons < 3)
                {
                    numUncommons++;
                    cardAdded = card;
                }
                else if (card.Rarity == CardInfo.Rarities.Common && numCommons < 10)
                {
                    numCommons++;
                    cardAdded = card;
                }

                if (cardAdded != null)
                {
                    collection.Add(card);
                    if (removeFromPool)
                    {
                        pool.Remove(card);
                    }
                }
            }

            return collection;
        }
    }
}
