using MtgConstructor.Cards;
using System;
using System.Collections.Generic;
using System.Text;

namespace MtgConstructor.Game
{
    /// <summary>
    /// Couples the player and abilities tied to the extra turn.
    /// </summary>
    public class ExtraTurn
    {
        public List<Ability> Abilities
        {
            get;
            private set;
        }

        public Source Source
        {
            get;
            private set;
        }

        public ExtraTurn(Source source, List<Ability> abilities)
        {
            Source = source;
            Abilities = new List<Ability>(abilities);
        }
    }
}
