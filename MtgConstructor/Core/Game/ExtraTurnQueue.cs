using MtgConstructor.Cards;
using System;
using System.Collections.Generic;
using System.Text;

namespace MtgConstructor.Game
{
    /// <summary>
    /// If the extra turn queue isn't empty, these turns must be taken in
    /// order before taking turns resumes from its last known position.
    /// </summary>
    public class ExtraTurnStack
    {
        private Stack<ExtraTurn> turns;

        public void Add(ExtraTurn turn)
        {
            turns.Push(turn);
        }

        public ExtraTurn GetTurn()
        {
            return turns.Pop();
        }

        public bool HasTurn()
        {
            return turns.Count > 0;
        }
    }
}
