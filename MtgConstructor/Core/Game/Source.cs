using MtgConstructor.Cards;
using System;
using System.Collections.Generic;
using System.Text;

namespace MtgConstructor.Game
{
    /// <summary>
    /// Represents information about the origin of something.
    /// </summary>
    public class Source
    {
        public DateTime Timestamp
        {
            get;
            private set;
        }

        public Source()
        {
            // TODO: Ability, spell, etc. that it came from. Copy info in case it can't be found.
            throw new NotImplementedException();
        }
    }
}
