using MtgConstructor.Cards;
using System;
using System.Collections.Generic;
using System.Text;

namespace MtgConstructor.Game
{
    /// <summary>
    /// Represents a group of allied player in a game.
    /// </summary>
    public class Team
    {
        private List<Player> allies;

        public Team(List<Player> allies)
        {
            this.allies = new List<Player>(allies);
        }
    }
}
