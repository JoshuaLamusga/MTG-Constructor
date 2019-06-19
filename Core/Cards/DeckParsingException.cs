using System;

namespace MtgConstructor.Cards
{
	/// <summary>
	/// Occurs when a deck is incompatible or corrupted and can't load.
	/// </summary>
	public class DeckParsingException : Exception
    {
        public DeckParsingException(string message) : base(message)
        {
        }
    }
}
