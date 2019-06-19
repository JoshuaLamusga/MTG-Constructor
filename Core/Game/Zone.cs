using MtgConstructor.Cards;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MtgConstructor.Game
{
    /// <summary>
    /// Represents a zone and its information.
    /// </summary>
    public class Zone
    {
        private bool isPrivate;
        public CardCollection Cards
        {
            get;
            set;
        }

        public Zone(CardCollection cards)
        {
            Cards = cards;
        }

        /// <summary>
        /// Shuffles the cards in the zone.
        /// </summary>
        public void Shuffle()
        {
            Cards.Cards.OrderBy((o) => Game.Rng.Next());
        }
    }
}
