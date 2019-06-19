using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace MtgConstructor.Cards.Parse
{
    public class CardDataRetrieval
    {
        private List<CardInfo> collection;

        public CardDataRetrieval(List<CardInfo> collection)
        {
            this.collection = collection;
        }

        /// <summary>
		/// Returns cards loaded from the given string, which should separate
		/// cards per line of text, optionally prefixing them with a quantity,
		/// e.g. "2x My Card Name".
        /// </summary>
        public CardCollection ReadCards(string data)
        {
            CardCollection deck = new CardCollection();

            string[] lines = data.Split('\n');

            for (int i = 0; i < lines.Length; i++)
            {
				if (lines[i].Trim() == "")
				{
					continue;
				}

				CardInfo card = null;
				int quantity = 1;

                //Matches quantity separate from name.
                var matches = Regex.Matches(lines[i], @"x*\d+x*|.+");

				if (matches.Count == 0)
				{
					throw new DeckParsingException($"'{lines[i]}' isn't part of a known format.");
				}
				else if (matches.Count == 1)
				{
					card = CardByName(matches[0].Value.Trim());
				}
				else
				{
					card = CardByName(matches[1].Value.Trim());
					if  (!int.TryParse(matches[0].Value.Replace("x", "").Trim(), out quantity))
					{
						throw new DeckParsingException($"'{matches[0].Value}' isn't a valid quantity for the card.");
					}
				}

                if (card == null)
                {
                    throw new DeckParsingException($"'{matches[0]}' isn't a known card.");
                }

                deck.Add(new Card(card, quantity));
            }

            return deck;
        }

        /// <summary>
        /// Returns null or the card based on the case-insensitive name if found.
        /// </summary>
        public CardInfo CardByName(string cardName)
        {
            string nameToTest = cardName.Trim().ToLower();
            for (int i = 0; i < collection.Count; i++)
            {
                if (collection[i].Name.ToLower() == nameToTest)
                {
                    return collection[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Returns a list of linked cards.
        /// </summary>
        public List<CardInfo> GetCardFaces(string cardName)
        {
            CardInfo card = CardByName(cardName);
            List<CardInfo> cards = new List<CardInfo>();

            if (card == null || card.CardFaces == null)
            {
                return cards;
            }

            List<CardInfo.CardFace> cardFaces = card.CardFaces;

            for (int i = 0; i < cardFaces.Count; i++)
            {
                cards.Add(CardByName(cardFaces[i].Name));
            }

            return cards;
        }

        /// <summary>
        /// Returns a new list with no card reprints.
        /// </summary>
        public List<CardInfo> FilterReprints(List<CardInfo> collection)
        {
            List<CardInfo> newCollection = new List<CardInfo>(collection);
            newCollection.RemoveAll(o => o.Reprint);
            return newCollection;
        }

        /// <summary>
        /// Returns a new list with no un-sets.
        /// </summary>
        public List<CardInfo> FilterUnSets(List<CardInfo> collection)
        {
            List<CardInfo> newCollection = new List<CardInfo>(collection);
            return newCollection;
        }
    }
}
