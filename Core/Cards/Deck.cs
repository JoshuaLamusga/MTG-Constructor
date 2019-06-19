using MtgConstructor.Cards.Parse;
using MtgConstructor.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MtgConstructor.Cards
{
    /// <summary>
    /// Couples a deck and sideboard with deck metadata.
    /// </summary>
    public class Deck
    {
		public CardCollection Mainboard { get; set; }
        public CardCollection Sideboard { get; set; }
        public DeckMetadata Metadata { get; set; }

        public Deck()
        {
            Mainboard = new CardCollection();
            Sideboard = new CardCollection();
            Metadata = new DeckMetadata();
        }

        public Deck(Deck other)
        {
            Mainboard = new CardCollection(other.Mainboard);
            Sideboard = new CardCollection(other.Sideboard);
            Metadata = new DeckMetadata();
        }

        /// <summary>
        /// Runs metadata restrictions and returns all failing cards with the validation they failed as a string.
        /// </summary>
        public List<DeckValidationResult> Validate(CardCollection board)
        {
            var failingCards = new List<DeckValidationResult>();

            // Validate by queries.
            FilterQuery[] queries = Metadata.GetQueries();
            for (int i = 0; i < queries.Length; i++)
            {
                List<CardInfo> validCards = queries[i].Execute(board.Cards.Select(o => o.cardInfo).ToList());
                for (int j = 0; j < board.Cards.Count; j++)
                {
                    if (!validCards.Contains(board.Cards[j].cardInfo)
                        && !failingCards.Any(o => o.card.cardInfo == board.Cards[j].cardInfo))
                    {
                        failingCards.Add(new DeckValidationResult(board.Cards[j], queries[i]));
                    }
                }
            }

            // Validate by expected formats.
            DeckFormat[] formats = Metadata.GetFormats();
            for (int i = 0; i < formats.Length; i++)
            {
                failingCards.AddRange(ValidateFormat(board, formats));

                failingCards = failingCards
                    .GroupBy(cardUniqueCriteria => cardUniqueCriteria.card.cardInfo)
                    .Select(group => group.First()).ToList();
            }

            return failingCards;
        }

        /// <summary>
        /// Returns cards from the given board that don't match the given format.
        /// </summary>
        private List<DeckValidationResult> ValidateFormat(CardCollection board, DeckFormat[] formats)
        {
            var invalidCards = new List<DeckValidationResult>();

            foreach (DeckFormat format in formats)
            {
                //TODO: Validate by deck for formats like Rainbow Stairwell
                for (int i = 0; i < board.Cards.Count; i++)
                {
                    var card = board.Cards[i];
                    var cardinfo = card.cardInfo;

                    if (format == DeckFormat.Archenemy && cardinfo.GetTypes().Contains("Scheme"))
                    {
                        invalidCards.Add(new DeckValidationResult(card, DeckFormat.Archenemy));
                    }
                }

                // TODO: Validates by card for most formats.
                switch (format)
                {
                    case DeckFormat.Archenemy:
                        break;
                }
            }

            //TODO: Remove all duplicates here.

            return invalidCards;
        }
    }

    public struct DeckValidationResult
    {
        public readonly Card card;
        public readonly DeckFormat? failedFormat;
        public readonly FilterQuery failedQuery;

        public DeckValidationResult(Card card, DeckFormat failedFormat)
        {
            this.card = card;
            this.failedFormat = failedFormat;
            failedQuery = null;
        }

        public DeckValidationResult(Card card, FilterQuery failedQuery)
        {
            this.card = card;
            failedFormat = null;
            this.failedQuery = failedQuery;
        }
    }
}
