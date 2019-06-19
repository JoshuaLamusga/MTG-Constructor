using System.Collections.Generic;
using System.Linq;

namespace MtgConstructor.Cards
{
    /// <summary>
    /// Represents a card collection with automatic card grouping on addition.
    /// Does not preserve differences between cards with the same name.
    /// </summary>
    public class CardCollection
    {
		public List<Card> Cards { get; set; }

		public CardCollection()
        {
			Cards = new List<Card>();
        }

        public CardCollection(CardCollection other)
        {
			Cards = new List<Card>(other.Cards);
        }

        /// <summary>
        /// Adds cards and combines duplicates automatically.
        /// </summary>
        public void Add(Card addition)
        {
            Card existingCard = Cards.FirstOrDefault(o => o.cardInfo.Name == addition.cardInfo.Name);
            if (existingCard != null)
            {
                existingCard.quantity += addition.quantity;
            }
            else
            {
                Cards.Add(addition);
            }
        }

        /// <summary>
        /// Adds cards and combines duplicates automatically.
        /// </summary>
        public void Add(List<Card> additions)
        {
            for (int i = 0; i < additions.Count; i++)
            {
                Add(additions[i]);
            }
        }

		/// <summary>
		/// Removes the specified card.
		/// </summary>
		public void Remove(Card removal)
		{
			Cards.Remove(removal);
		}

        /// <summary>
        /// Removes each card in the specified list.
        /// </summary>
        public void Remove(List<Card> removals)
        {
            removals.ForEach(o => Cards.Remove(o));
        }

        /// <summary>
        /// Counts all cards and duplicates in the deck.
        /// </summary>
        public int GetCardCount()
        {
            return Cards.Sum(o => o.quantity);
        }
    }
}
