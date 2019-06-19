using MtgConstructor.Cards.Parse;
using MtgConstructor.Data;
using System.Collections.Generic;

namespace MtgConstructor.Cards
{
    /// <summary>
    /// Metadata associated with decks for controlled compile-time checking.
    /// </summary>
    public class DeckMetadata
    {
        private string description;
        private List<DeckFormat> enforcedFormats;
        private List<FilterQuery> enforcedQueries;
        private string title;

        public DeckMetadata()
        {
            description = null;
            enforcedFormats = new List<DeckFormat>();
            enforcedQueries = new List<FilterQuery>();
            title = null;
        }

        public DeckMetadata(DeckMetadata other)
        {
            description = other.description;
            enforcedFormats = new List<DeckFormat>(other.enforcedFormats);
            enforcedQueries = new List<FilterQuery>(other.enforcedQueries);
            title = other.title;
        }

        /// <summary>
        /// All cards in the deck must match this format to be valid.
        /// </summary>
        public void AddEnforcedFormat(DeckFormat format)
        {
            if (!enforcedFormats.Contains(format))
            {
                enforcedFormats.Add(format);
            }
        }

        /// <summary>
        /// All cards in the deck must match this search query to be valid.
        /// </summary>
        public void AddEnforcedQuery(FilterQuery query)
        {
            enforcedQueries.Add(query);
        }

        public void RemoveFormat(DeckFormat format)
        {
            enforcedFormats.Remove(format);
        }

        public void RemoveQuery(FilterQuery query)
        {
            enforcedQueries.Remove(query);
        }

        public DeckFormat[] GetFormats()
        {
            return enforcedFormats.ToArray();
        }

        public FilterQuery[] GetQueries()
        {
            return enforcedQueries.ToArray();
        }

        public void SetTitle(string title)
        {
            this.title = title;
        }

        public void SetDescription(string description)
        {
            this.description = description;
        }
    }
}
