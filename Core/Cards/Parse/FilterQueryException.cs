using System;

namespace MtgConstructor.Cards
{
    /// <summary>
    /// Occurs when a filter query can't be parsed correctly.
    /// </summary>
    public class FilterQueryException : Exception
    {
        public FilterQueryException(string message) : base(message)
        {
        }
    }
}
