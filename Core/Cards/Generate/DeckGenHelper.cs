using MtgConstructor.Cards.Parse;
using MtgConstructor.Data;
using MtgConstructor.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MtgConstructor.Cards.Generate
{
    public static class DeckGenHelper
    {
        private static Random rng = new Random();

        public enum DeckTheme
        {
            None,
            Tribal,
            Keyword
        }

        #region Methods
        /// <summary>
        /// Generates a deck with the given preferences. Preferences
        /// conflicting with the format are ignored.
        /// </summary>
        public static CardCollection Generate(List<CardInfo> collection, DeckPreferences preferences)
        {
            CardCollection deck = new CardCollection();

            // Gets amount of lands in each color.
            float totalRatio = preferences.Ratios.landRatio
                + preferences.Ratios.creatureRatio
                + preferences.Ratios.otherPermRatio
                + preferences.Ratios.nonpermanentRatio;

            deck.Add(GenerateLands(collection, preferences, totalRatio));

            int maxEdhRank = 40000;
            SortQuery edhRankSort = new SortQuery(SortQuery.SortKey.edhrecRank, SortQuery.SortCondition.Most, maxEdhRank);

            // Determine colors to exclude.
            string strColorFilter = string.Empty;
            if (preferences.Colors.Count > 0 && preferences.Colors.Count < 5)
            {
                List<string> colorSegments = new List<string>();
                if (!preferences.Colors.Contains(CardInfo.Color.W))
                    { strColorFilter += "[manacost excludes W] && [oracletext excludes {W}] && "; }
                if (!preferences.Colors.Contains(CardInfo.Color.U))
                    { strColorFilter += "[manacost excludes U] && [oracletext excludes {U}] && "; }
                if (!preferences.Colors.Contains(CardInfo.Color.B))
                    { strColorFilter += "[manacost excludes B] && [oracletext excludes {B}] && "; }
                if (!preferences.Colors.Contains(CardInfo.Color.R))
                    { strColorFilter += "[manacost excludes R] && [oracletext excludes {R}] && "; }
                if (!preferences.Colors.Contains(CardInfo.Color.G))
                    { strColorFilter += "[manacost excludes G] && [oracletext excludes {G}] && "; }

                strColorFilter = strColorFilter.Substring(0, strColorFilter.Length - 3); // No && at end.
            }
            else if (preferences.Colors.Count != 5)
            {
                strColorFilter = "[manacost excludes W] && [manacost excludes U] && [manacost excludes B] "
                    + "&& [manacost excludes R] && [manacost excludes G] && [oracletext excludes {W}] "
                    + "&& [oracletext excludes {U}] && [oracletext excludes {B}] && [oracletext excludes {R}] "
                    + "&& [oracletext excludes {G}]";
            }

            // Add creatures.
            var creatureCards = edhRankSort.Execute(
                FilterQuery.FromString(strColorFilter).Execute(collection.Where(o =>
                o.GetTypes().Contains("Creature")).ToList()))
                .OrderBy(o => o.Value / 10 + rng.Next(maxEdhRank))
                .Select(o => o.Key)
                .Take((int)(preferences.Ratios.creatureRatio / totalRatio * preferences.DeckSize))
                .ToList();
            foreach (CardInfo card in creatureCards)
            {
                deck.Add(new Card(card, 1));
            }

            // Add non-land, non-creature permanents.
            var nonlandCards = edhRankSort.Execute(
                FilterQuery.FromString(strColorFilter).Execute(collection.Where(o =>
                !o.GetTypes().Contains("Creature") &&
                (o.GetTypes().Contains("Enchantment") ||
                o.GetTypes().Contains("Artifact") ||
                o.GetTypes().Contains("Planeswalker"))).ToList()))
                .OrderBy(o => o.Value / 10 + rng.Next(maxEdhRank))
                .Select(o => o.Key)
                .Take((int)(preferences.Ratios.otherPermRatio / totalRatio * preferences.DeckSize))
                .ToList();
            foreach (CardInfo card in nonlandCards)
            {
                deck.Add(new Card(card, 1));
            }

            // Add non-permanents.
            var nonpermanentCards = edhRankSort.Execute(
                FilterQuery.FromString(strColorFilter).Execute(collection.Where(o =>
                o.GetTypes().Contains("Instant") ||
                o.GetTypes().Contains("Sorcery")).ToList()))
                .OrderBy(o => o.Value / 10 + rng.Next(maxEdhRank))
                .Select(o => o.Key)
                .Take((int)(preferences.Ratios.nonpermanentRatio / totalRatio * preferences.DeckSize))
                .ToList();
            foreach (CardInfo card in nonpermanentCards)
            {
                deck.Add(new Card(card, 1));
            }

            return deck;
        }

        /// <summary>
        /// Generates lands.
        /// </summary>
        private static List<Card> GenerateLands(
            List<CardInfo> collection,
            DeckPreferences preferences,
            float totalRatio)
        {
            List<string> basicLands = new List<string>();
            if (preferences.Colors.Count == 0) { basicLands.Add("Wastes"); }
            if (preferences.Colors.Contains(CardInfo.Color.W)) { basicLands.Add("Plains"); }
            if (preferences.Colors.Contains(CardInfo.Color.U)) { basicLands.Add("Island"); }
            if (preferences.Colors.Contains(CardInfo.Color.B)) { basicLands.Add("Swamp"); }
            if (preferences.Colors.Contains(CardInfo.Color.R)) { basicLands.Add("Mountain"); }
            if (preferences.Colors.Contains(CardInfo.Color.G)) { basicLands.Add("Forest"); }

            List<Card> cards = new List<Card>();
            float landPerColor = preferences.Ratios.landRatio / totalRatio * preferences.DeckSize;
            if (preferences.Colors.Count > 0)
            {
                landPerColor /= preferences.Colors.Count;
            }

            // Add basic lands.
            for (int i = 0; i < basicLands.Count; i++)
            {
                var card = collection.Where(o => o.Name == basicLands[i]).First();
                cards.Add(new Card(card, (int)landPerColor));
            }

            return cards;
        }
        #endregion

        #region Internal Classes
        /// <summary>
        /// Represents the number of cards in each basic card category, where
        /// nonland means permanents other than lands and nonpermanent means
        /// everything else.
        /// </summary>
        public class CardRatios
        {
            public readonly float landRatio, creatureRatio, otherPermRatio, nonpermanentRatio;
            public static CardRatios Average = new CardRatios(20, 20, 12, 8);
            public static CardRatios LowLands = new CardRatios(15, 23, 12, 10);
            public static CardRatios HighLands = new CardRatios(25, 19, 10, 6);
            public static CardRatios HighNonPermanent = new CardRatios(20, 14, 10, 16);
            public static CardRatios LowNonPermanent = new CardRatios(20, 25, 13, 2);

            public CardRatios(float landRatio, float creatureRatio, float otherPermRatio, float nonpermanentRatio)
            {
                this.landRatio = landRatio;
                this.creatureRatio = creatureRatio;
                this.otherPermRatio = otherPermRatio;
                this.nonpermanentRatio = nonpermanentRatio;
            }
        }

        /// <summary>
        /// Preferences-based generation uses this data to help generate a
        /// deck.
        /// </summary>
        public class DeckPreferences
        {
            public static DeckPreferences Random()
            {
                DeckPreferences prefs = new DeckPreferences();
                Random rng = new Random();

                // Randomize distribution of card types.
                switch (rng.Next(6))
                {
                    case 0:
                    case 1:
                        prefs.Ratios = CardRatios.Average;
                        break;
                    case 2:
                        prefs.Ratios = CardRatios.HighLands;
                        break;
                    case 3:
                        prefs.Ratios = CardRatios.LowLands;
                        break;
                    case 4:
                        prefs.Ratios = CardRatios.HighNonPermanent;
                        break;
                    case 5:
                        prefs.Ratios = CardRatios.LowNonPermanent;
                        break;
                }

                // Randomize deck colors.
                var colors = Enum.GetValues(typeof(CardInfo.Color)).OfType<CardInfo.Color>().ToList();

                // 12.5% chance to be colorless
                if (rng.Next(8) > 0)
                {
                    // 58.33% chance to end up monocolored
                    CardInfo.Color firstColor = colors[rng.Next(colors.Count)];
                    colors.Remove(firstColor);
                    prefs.Colors.Add(firstColor);

                    // 21.88% chance to end up bicolored
                    if (rng.Next(3) == 0)
                    {
                        CardInfo.Color secondColor = colors[rng.Next(colors.Count)];
                        colors.Remove(secondColor);
                        prefs.Colors.Add(secondColor);

                        // 2.43% chance to end up tricolored
                        if (rng.Next(4) == 0)
                        {
                            CardInfo.Color thirdColor = colors[rng.Next(colors.Count)];
                            colors.Remove(thirdColor);
                            prefs.Colors.Add(thirdColor);

                            if (rng.Next(3) != 0)
                            {
                                // 3.64% chance to end up five-colored
                                if (rng.Next(4) != 0)
                                {
                                    prefs.Colors.Add(colors[0]);
                                    prefs.Colors.Add(colors[1]);
                                }

                                // 1.22% chance to end up four-colored
                                else
                                {
                                    CardInfo.Color fourthColor = colors[rng.Next(colors.Count)];
                                    colors.Remove(fourthColor);
                                    prefs.Colors.Add(fourthColor);
                                }
                            }
                        }
                    }
                }

                return prefs;
            }

            /// <summary>
            /// Desired colors of the deck, such as green and white. Duplicate
            /// values are ignored.
            /// </summary>
            public List<CardInfo.Color> Colors = new List<CardInfo.Color>();

            /// <summary>
            /// The ratio of lands to nonland and non-permanents, which shapes
            /// the basic aspect of a deck.
            /// </summary>
            public CardRatios Ratios = CardRatios.Average;

            /// <summary>
            /// A given format will override any conflicting preferences.
            /// </summary>
            public DeckFormat Format = DeckFormat.Casual;

            /// <summary>
            /// If non-empty, cards will be of the named type if able.
            /// </summary>
            public string themeTribal = string.Empty;

            /// <summary>
            /// If non-empty, cards will include the named keyword if able.
            /// </summary>
            public string themeKeyword = string.Empty;

            /// <summary>
            /// The preferred deck size. Default 60.
            /// </summary>
            public int DeckSize = 60;
        }
        #endregion
    }
}
