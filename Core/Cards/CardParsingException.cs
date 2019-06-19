using System;

namespace MtgConstructor.Cards
{
	/// <summary>
	/// Occurs when a card or its information can't be parsed.
	/// </summary>
	public class CardParsingException : Exception
    {
        public CardParsingException(string message) : base(message)
        {
        }
    }
}
