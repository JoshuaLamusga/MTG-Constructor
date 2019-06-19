using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MtgConstructor.Cards.Parse
{
    /// <summary>
    /// Used to compose sort queries that evaluate each card in a collection
    /// and assigns a number based on how well it matches the query.
    /// </summary>
    public class SortQuery
    {
        private readonly SortKey key;
        private readonly SortCondition condition;
        private readonly int score;

        /// <summary>
        /// The card property to test.
        /// </summary>
        public enum SortKey
        {
            cmc,
            colorsLength,
            edhrecRank,
            loyalty,
            manaCostWhiteSymbols,
            manaCostBlueSymbols,
            manaCostBlackSymbols,
            manaCostRedSymbols,
            manaCostGreenSymbols,
            manaCostColorless,
            power,
            rarity,
            toughness
        }

        /// <summary>
        /// How the sort works.
        /// </summary>
        public enum SortCondition
        {
            Least,
            Most
        }

        /// <summary>
        /// Creates a query that favors cards with the smallest or greatest
        /// value in the attribute identified by the key.
        /// </summary>
        public SortQuery(SortKey key, SortCondition condition, int score)
        {
            this.key = key;
            this.condition = condition;
            this.score = score;
        }

        /// <summary>
        /// Returns the value of the field named by the given key.
        /// </summary>
        private object GetField(CardInfo card)
        {
            switch (key)
            {
                case SortKey.cmc:
                    return card.Cmc;
                case SortKey.colorsLength:
                    return card.Colors?.Count;
                case SortKey.edhrecRank:
                    return card.EdhrecRank;
                case SortKey.loyalty:
                    return card.Loyalty;
                case SortKey.manaCostWhiteSymbols:
                    if (card.ManaCost == null) { return null; }
                    return Regex.Matches(card.ManaCost, "{W}").Count;
                case SortKey.manaCostBlueSymbols:
                    if (card.ManaCost == null) { return null; }
                    return Regex.Matches(card.ManaCost, "{U}").Count;
                case SortKey.manaCostBlackSymbols:
                    if (card.ManaCost == null) { return null; }
                    return Regex.Matches(card.ManaCost, "{B}").Count;
                case SortKey.manaCostRedSymbols:
                    if (card.ManaCost == null) { return null; }
                    return Regex.Matches(card.ManaCost, "{R}").Count;
                case SortKey.manaCostGreenSymbols:
                    if (card.ManaCost == null) { return null; }
                    return Regex.Matches(card.ManaCost, "{G}").Count;
                case SortKey.manaCostColorless:
                    if (card.ManaCost == null) { return null; }
                    return Regex.Match(card.ManaCost, @"(?<={)\d+(?=})").Value;
                case SortKey.power:
                    return card.Power;
                case SortKey.rarity:
                    return (int)card.Rarity;
                case SortKey.toughness:
                    return card.Toughness;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Returns all cards in the given collection with associated weights.
        /// </summary>
        public Dictionary<CardInfo, double> Execute(List<CardInfo> collection)
        {
            var cards = new Dictionary<CardInfo, double>();
            List<CardInfo> result;

            result = collection
                .Where(o => double.TryParse(GetField(o)?.ToString(), out double temp)
                    && !double.IsInfinity(temp) && !double.IsNaN(temp))
                .OrderBy(o => GetField(o))
                .ToList();

            if (result.Count > 0)
            {
                double maxValue = double.Parse(GetField(result[result.Count - 1]).ToString());

                for (int i = 0; i < result.Count; i++)
                {
                    double cardValue = double.Parse(GetField(result[i]).ToString());

                    switch (condition)
                    {
                        case SortCondition.Least:
                            cards[result[i]] = score / maxValue * (maxValue - cardValue);
                            break;
                        default:
                            cards[result[i]] = score / maxValue * cardValue;
                            break;
                    }
                }
            }

            return cards;
        }
    }
}
