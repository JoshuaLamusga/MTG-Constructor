using MtgConstructor.Cards;
using System;
using System.Collections.Generic;
using System.Text;

namespace MtgConstructor.Game
{
    /// <summary>
    /// Represents steps and phases in a turn associated with a player,
    /// useful for tracking skipped and extra phases or steps.
    /// </summary>
    public class TurnStructure
    {
        // If any of these are negative, that many are skipped instead.
        int NumExtraCombatPhases;
        int NumExtraDrawSteps;
        int NumExtraEndPhases;
        int NumExtraMainPhases;
        int NumExtraTurns;
        int NumExtraUntapSteps;
        int NumExtraUpkeepSteps;

        // Use for perpetual skipping, as with cards like Stasis.
        bool SkipCombat;
        bool SkipDraw;
        bool SkipEndingPhase;
        bool SkipUntap;
        bool SkipUpkeep;
    }
}
