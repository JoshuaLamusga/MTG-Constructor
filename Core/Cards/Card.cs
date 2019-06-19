using MtgConstructor.Cards.Parse;

namespace MtgConstructor.Cards
{
    /// <summary>
    /// Couples a card with a quantity.
    /// </summary>
    public class Card
    {
        public CardInfo cardInfo;
        public int quantity;

        public Card(CardInfo card, int quantity)
        {
            cardInfo = card;
            this.quantity = quantity;
        }

        public Card(Card other)
        {
            cardInfo = other.cardInfo;
            quantity = other.quantity;
        }
    }
}
