using MtgConstructor.Cards.Parse;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MtgConstructor.Cards.Generate
{
    /// <summary>
    /// Generates stats and collections of related cards based on heuristics.
    /// </summary>
    public class DeckGenerator
    {
        #region Members
        /// <summary>
        /// The list of card slot definitions used to generate a deck.
        /// </summary>
        public List<CardSlot> Slots { get; set; }

        /// <summary>
        /// The slot definition used to add cards until the deck minimum is
        /// met, if the associated behavior is used and this is not null.
        /// </summary>
        public CardSlot RemainderSlot { get; set; }

        /// <summary>
        /// Minimum number of cards required to finish generation.
        /// Default 60.
        /// </summary>
        private int minCards;

        /// <summary>
        /// Maximum number of cards allowed in generation.
        /// Default 60.
        /// </summary>
        private int maxCards;

        private Random rng;

        /// <summary>
        /// Order of execution. Default sequential (loop at end).
        /// </summary>
        private ExecutionOrder executionOrder;

        /// <summary>
        /// If not null, this filter is used to remove extra cards.
        /// Default null.
        /// </summary>
        public FilterQuery StripExcessCardsFilter { get; set; }

        /// <summary>
        /// A value from 0 to 1 describing the percent of cards to
        /// remove, rounded to the nearest integer. Default 0.
        /// </summary>
        public float PercentExtraCardsToRemove { get; set; }

        /// <summary>
        /// After the deck has this many copies of a card, it can't be selected
        /// by card slots. Set to 0 to disable. Default 4.
        /// </summary>
        public int BlacklistCopyThreshold { get; set; }
        #endregion

        #region Enums
        public enum ExecutionOrder
        {
            SequentialLoopEnd,
            SequentialFillRemaining,
            Random
        }
        #endregion

        #region Constructors
        public DeckGenerator()
        {
            Initialize();
        }

        public DeckGenerator(List<CardSlot> slots)
        {
            Initialize();
            Slots = slots;
        }
        #endregion

        #region Methods
        private void Initialize()
        {
            Slots = new List<CardSlot>();
            RemainderSlot = null;
            minCards = 60;
            maxCards = 60;
            executionOrder = ExecutionOrder.SequentialLoopEnd;
            StripExcessCardsFilter = null;
            PercentExtraCardsToRemove = 0;
            BlacklistCopyThreshold = 4;
            rng = new Random();
        }

        /// <summary>
        /// Sets the minimum and maximum number of cards allowed in the deck.
        /// </summary>
        public void SetDeckSize(int minCards, int maxCards)
        {
            this.minCards = minCards;
            this.maxCards = maxCards;
        }

        /// <summary>
        /// How the deck generator iterates through card slots.
        /// </summary>
        public void SetExecutionOrder(ExecutionOrder executionOrder)
        {
            this.executionOrder = executionOrder;
        }

        /// <summary>
        /// Generates a deck with the given collection of cards.
        /// </summary>
        public CardCollection Generate(List<CardInfo> collection)
        {
            CardCollection deck = new CardCollection();
            List<CardInfo> cardPool = new List<CardInfo>(collection);

            if (Slots.Count == 0)
            {
                return deck;
            }

            int slotIndex = -1;
            var slotsToUse = Slots;
            while (deck.GetCardCount() < minCards)
            {
                if (BlacklistCopyThreshold > 0)
                {
                    var maxCopyCards = deck.Cards.Where(o => o.quantity > BlacklistCopyThreshold).ToList();
                    maxCopyCards.ForEach(o => cardPool.RemoveAll((p) => p.Name == o.cardInfo.Name));
                }

                if (executionOrder == ExecutionOrder.Random)
                {
                    slotIndex = rng.Next(slotsToUse.Count);
                }
                else if (slotsToUse[0] != RemainderSlot)
                {
                    slotIndex++;
                    if (slotIndex >= slotsToUse.Count)
                    {
                        if (executionOrder == ExecutionOrder.SequentialFillRemaining
                            && RemainderSlot != null)
                        {
                            int remainingCards = minCards - deck.GetCardCount();
                            RemainderSlot.SetQuantity(remainingCards, remainingCards);
                            slotsToUse = new List<CardSlot>() { RemainderSlot };
                        }

                        slotIndex = 0;
                    }
                }

                if (!slotsToUse[slotIndex].ShouldExecuteSlot())
                {
                    continue;
                }

                var chosenCards = slotsToUse[slotIndex].Execute(cardPool);
                int cardCount = deck.GetCardCount();
                chosenCards.ForEach(o =>
                {
                    if (cardCount < maxCards)
                    {
                        deck.Add(new Card(o, 1));
                        cardCount++;

                        if (BlacklistCopyThreshold > 0)
                        {
                            Card card = deck.Cards.First(p => p.cardInfo.Name == o.Name);
                            card.quantity = Math.Min(card.quantity, BlacklistCopyThreshold);
                        }
                    }
                });
            }

            if (deck.GetCardCount() > minCards && StripExcessCardsFilter != null)
            {
                List<CardInfo> cardsWithDuplicates = new List<CardInfo>();
                deck.Cards.ForEach(o =>
                {
                    for (int i = 0; i < o.quantity; i++)
                    {
                        cardsWithDuplicates.Add(o.cardInfo);
                    }
                });

                var cardsToRemove = StripExcessCardsFilter.Execute(cardsWithDuplicates);

                int extraCards = deck.GetCardCount() - maxCards;
                int maxToRemove = (int)(extraCards * PercentExtraCardsToRemove);

                for (int i = 0; i < cardsToRemove.Count && i < maxToRemove; i++)
                {
                    deck.Cards.RemoveAll(o => o.cardInfo == cardsToRemove[i]);
                }
            }

            return deck;
        }
        #endregion
    }
}
