using MtgConstructor.Cards.Parse;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MtgConstructor.Cards.Generate
{
    /// <summary>
    /// Uses queries to assign points to cards matching criteria, for use
    /// in deck generation.
    /// </summary>
    public class CardSlot
    {
        #region Members
        /// <summary>
        /// Assigns the given amount of points to any card matching the
        /// predicate. Generally, it's a good idea to use high filter values
        /// if using sort queries that produce a wide range of score values.
        /// </summary>
        public List<Tuple<FilterQuery, int>> Filters { get; set; }

        /// <summary>
        /// These execute in order to assign points to cards best matching
        /// the given criteria. Note that e.g. sorting by most CMC with Gleemax
        /// in the card pool will give it a score of 1000000 and the next card
        /// 100, so it can vary widely. Assign points in filters accordingly.
        /// </summary>
        public List<SortQuery> Sorts { get; set; }

        private int minQuantity;
        private int maxQuantity;
        private int scoreTieThreshold;
        private SelectionMode selectionMode;
        private CardScoresTied tiedScoresBehavior;
        private Random rng;

        /// <summary>
        /// Cards with matching names won't be selected.
        /// </summary>
        public List<CardInfo> Blacklist { get; set; }

        /// <summary>
        /// A value from 0 - 100 for the percent chance this slot is applied.
        /// Default 100.
        /// </summary>
        public int PercentChanceToApplySlot;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a card slot for the given quantity of cards.
        /// If a percent is provided, use 0 - 100 to represent it.
        /// </summary>
        public CardSlot(int numCardsToMatch)
        {
            Filters = new List<Tuple<FilterQuery, int>>();
            Sorts = new List<SortQuery>();
            Blacklist = new List<CardInfo>();
            PercentChanceToApplySlot = 100;
            minQuantity = numCardsToMatch;
            maxQuantity = numCardsToMatch;
            selectionMode = SelectionMode.RandomByPointWeight;
            tiedScoresBehavior = CardScoresTied.Random;
            rng = new Random();
        }

        public CardSlot(CardSlot other)
        {
            Filters = new List<Tuple<FilterQuery, int>>(other.Filters);
            Sorts = new List<SortQuery>(other.Sorts);
            Blacklist = new List<CardInfo>(other.Blacklist);
            PercentChanceToApplySlot = other.PercentChanceToApplySlot;
            minQuantity = other.minQuantity;
            maxQuantity = other.maxQuantity;
            selectionMode = other.selectionMode;
            tiedScoresBehavior = other.tiedScoresBehavior;
            rng = new Random();
        }
        #endregion

        #region Enums
        public enum SelectionMode
        {
            RandomByPointWeight,
            MostPoints
        }

        public enum CardScoresTied
        {
            Random,
            Edhrank
        }
        #endregion

        #region Methods
        /// <summary>
        /// Returns all cards chosen by this slot, treating its value as the
        /// expected number of cards to find. Always executes if called.
        /// Returns as many cards as possible if insufficient cards are given.
        /// </summary>
        public List<CardInfo> Execute(List<CardInfo> collection)
        {
            return Execute(collection, 0);
        }

        /// <summary>
        /// Returns all cards chosen by this slot, treating its value as a
        /// percent of the expected deck size to find. Always executes if
        /// called. Returns as many cards as possible if insufficient cards are
        /// given.
        /// </summary>
        public List<CardInfo> Execute(List<CardInfo> collection, int expectedDeckSize)
        {
            var cardPool = collection.Except(Blacklist).ToList();
            var cardScores = new Dictionary<CardInfo, double>();
            var sortResults = new List<Dictionary<CardInfo, double>>();
            var filterResults = new List<List<CardInfo>>();
            var chosenCards = new List<CardInfo>();

            if (cardPool.Count == 0)
            {
                return chosenCards;
            }

            // Adds all scores from sorts and filters.
            Sorts.ForEach(o => sortResults.Add(o.Execute(cardPool)));
            Filters.ForEach(o => filterResults.Add(o.Item1.Execute(cardPool)));

            for (int i = 0; i < cardPool.Count; i++)
            {
                cardScores[cardPool[i]] = 0;

                for (int j = 0; j < sortResults.Count; j++)
                {
                    if (sortResults[j].ContainsKey(cardPool[i]))
                    {
                        cardScores[cardPool[i]] += sortResults[j][cardPool[i]];
                    }
                }

                for (int j = 0; j < filterResults.Count; j++)
                {
                    if (filterResults[j].Contains(cardPool[i]))
                    {
                        cardScores[cardPool[i]] += Filters[j].Item2;
                    }
                }
            }

            // Prevents similar-scoring cards from being chosen top to bottom.
            if (selectionMode == SelectionMode.RandomByPointWeight)
            {
                cardPool = cardPool.OrderBy(o => rng.Next()).ToList();
            }

            // Gets number of cards to be picked.
            int numCards = 0;

            if (expectedDeckSize > 0)
            {
                int newMinQuantity = (int)(minQuantity / 100.0 * expectedDeckSize);
                int newMaxQuantity = (int)((maxQuantity / 100.0 * expectedDeckSize) - newMinQuantity);
                if (newMaxQuantity > 0) newMaxQuantity = rng.Next(newMaxQuantity);
                numCards = newMinQuantity + newMaxQuantity;
            }
            else
            {
                numCards = minQuantity + rng.Next(maxQuantity - minQuantity);
            }

            // Picks cards with the chosen selection mode.
            var orderedScores = cardScores.OrderByDescending(o => o.Value).ToList();
            decimal totalScore = (decimal)Math.Round(cardScores.Sum((o) => o.Value));

            if (selectionMode == SelectionMode.MostPoints && scoreTieThreshold <= 0)
            {
                return orderedScores.GetRange(0, numCards).Select(o => o.Key).ToList();
            }

            for (int i = 0; i < numCards; i++)
            {
                switch (selectionMode)
                {
                    case SelectionMode.MostPoints:
                        var cardsTiedForScore = orderedScores
                            .Where(o => o.Value >= orderedScores[0].Value - scoreTieThreshold)
                            .ToList();

                        KeyValuePair<CardInfo, double> entry = cardsTiedForScore
                            .OrderBy(o => (tiedScoresBehavior == CardScoresTied.Random)
                                ? rng.Next() : o.Key.EdhrecRank).First();

                        chosenCards.Add(entry.Key);
                        orderedScores.Remove(entry);
                        break;
                    case SelectionMode.RandomByPointWeight:
                        int randomScore = (totalScore > 0)
                            ? rng.Next((int)totalScore)
                            : -rng.Next((int)Math.Abs(totalScore));
                            double scoreCount = 0;

                        for (int j = 0; j < cardPool.Count; j++)
                        {
                            scoreCount += cardScores[cardPool[j]];
                            if (scoreCount >= randomScore)
                            {
                                totalScore -= (decimal)cardScores[cardPool[j]];
                                chosenCards.Add(cardPool[j]);
                                cardPool.Remove(cardPool[j]);
                                break;
                            }
                        }
                        break;
                }
            }

            return chosenCards;
        }

        /// <summary>
        /// Scores are considered tied within this threshold.
        /// </summary>
        public void SetScoreTieThreshold(int scoreDifference)
        {
            scoreTieThreshold = scoreDifference;
        }

        /// <summary>
        /// Cards can be chosen with score weighting based on filter/sort
        /// matches, or not.
        /// </summary>
        public void SetSelectionMode(SelectionMode mode)
        {
            selectionMode = mode;
        }

        /// <summary>
        /// Returns randomly whether this slot should execute or not.
        /// </summary>
        public bool ShouldExecuteSlot()
        {
            return rng.Next(100) <= PercentChanceToApplySlot;
        }

        /// <summary>
        /// This card slot will attempt to match up to this many cards.
        /// </summary>
        public void SetQuantity(int minNumberOfCards, int maxNumberOfCards)
        {
            minQuantity = minNumberOfCards;
            maxQuantity = maxNumberOfCards;
        }

        /// <summary>
        /// Defines how ties are broken when two cards have the same score.
        /// </summary>
        public void SetTieScoreBehavior(CardScoresTied tiedScoresBehavior)
        {
            this.tiedScoresBehavior = tiedScoresBehavior;
        }
        #endregion
    }
}
