using MtgConstructor.Cards.Parse;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;
using System.Text;

namespace MtgConstructor.Cards.Parse
{
    public class Parser
    {
        public static List<CardInfo> AllCards;
        public static List<SetInfo> AllSets;
        private static readonly string AllCardsPath = "AllCards.json";
        private static readonly string AllSetsPath = "AllSets.json";

        static Parser()
        {
            AllCards = ReadCards();
            AllSets = ReadSets();
        }

        /// <summary>
        /// Loads all cards into memory.
        /// </summary>
        private static List<CardInfo> ReadCards()
        {
            return JsonConvert.DeserializeObject<List<CardInfo>>(File.ReadAllText(AllCardsPath));
        }

        /// <summary>
        /// Loads all sets into memory.
        /// </summary>
        private static List<SetInfo> ReadSets()
        {
            return JsonConvert.DeserializeObject<List<SetInfo>>(File.ReadAllText(AllSetsPath));
        }

        /// <summary>
        /// Returns null or the card based on the case-insensitive name if found.
        /// </summary>
        public static CardInfo CardByName(string name)
        {
            for (int i = 0; i < AllCards.Count; i++)
            {
                string cardName = AllCards[i].Name.ToLower();
                if (cardName == name.Trim().ToLower())
                {
                    return AllCards[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Returns null or the card based on its multiverse id. Note that you
        /// can use this and the variations attribute to find card versions.
        /// </summary>
        public static CardInfo CardById(string id)
        {
            for (int i = 0; i < AllCards.Count; i++)
            {
                string cardId = AllCards[i].Id;
                if (cardId == id)
                {
                    return AllCards[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Returns an empty list or a list of all cards with multiverse IDs
        /// matching the variation IDs provided.
        /// </summary>
        public static List<CardInfo> CardVariations(List<string> variationIds)
        {
            List<CardInfo> variations = new List<CardInfo>();

            for (int i = 0; i < variationIds.Count; i++)
            {
                variations.Add(CardById(variationIds[i]));
            }

            return variations;
        }

        /// <summary>
        /// Given a deck, detects and applies flags describing the deck in
        /// terms of legality, supported formats, and more.
        /// </summary>
        public static void DetectDeckFormat(Deck deck)
        {
            throw new NotImplementedException(); //TODO
        }

        /// <summary>
        /// Returns a list of cards without reprints or non-unique names that
        /// exist on Gatherer. Options to ignore Planar/Scheme/token layouts
        /// and funny sets exist.
        /// </summary>
        /// <param name="noFormatLayouts">Don't return tokens.</param>
        public static List<CardInfo> FilterCards(
            List<CardInfo> cards,
            List<SetInfo> sets,
            bool noFunnySets,
            bool noFormatLayouts)
        {
            List<CardInfo> filteredCards = new List<CardInfo>(cards);
            filteredCards.RemoveAll(card => {
                return card.MultiverseIds == null ||
                    card.MultiverseIds.Count == 0 ||
                    (noFormatLayouts && (
                        card.Layout == CardInfo.Layouts.Planar ||
                        card.Layout == CardInfo.Layouts.Scheme ||
                        card.Layout == CardInfo.Layouts.Token)) ||
                    card.Reprint == true ||
                    (noFunnySets && sets.Find(o => o.Code == card.Set).SetType == SetType.Funny);
            });

            return filteredCards
                .GroupBy(card => card.Name)
                .Select(group => group.First())
                .ToList();
        }

        /// <summary>
        /// Returns cards loaded from the given string.
        /// </summary>
        public static Deck ReadDeck(string data)
        {
            Deck deck = new Deck();

            string[] lines = data.Split('\n');
            CardInfo card = null;
            int quantity = 1;

            for (int i = 0; i < lines.Length; i++)
            {
                //Matches quantity separate from name.
                var matches = Regex.Matches(lines[i], @"x*?\d+x*?|.+");

                if (matches.Count == 0)
                {
                    throw new DeckParsingException($"'{lines[i]}' isn't part of a known format.");
                }

                if (matches.Count == 2 &&
                    !int.TryParse(matches[0].Value.Trim(), out quantity))
                {
                    throw new DeckParsingException($"'{matches[0].Value}' isn't a valid quantity for the card.");
                }

                card = CardByName(matches[0].Value.Trim());
                if (card == null)
                {
                    throw new DeckParsingException($"'{matches[0]}' isn't a known card.");
                }

                deck.Mainboard.Add(new Card(card, quantity));
            }

            return deck;
        }

        /// <summary>
        /// Writes a list of cards in a "[quantity]x [cardName]" format
        /// that many third-party MTG tools can read.
        /// </summary>
        public static string ExportCards(List<Card> cards)
        {
            string deckString = string.Empty;
            for (int i = 0; i < cards.Count; i++)
            {
                if (i != 0)
                {
                    deckString += Environment.NewLine;
                }

                deckString += $"{cards[i].quantity}x {cards[i].cardInfo.Name}";
            }

            return deckString;
        }
    }
}
