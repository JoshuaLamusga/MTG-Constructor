using MtgConstructor.Cards;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace MtgConstructor.Game
{
    /// <summary>
    /// Represents functions to manage gameplay for players.
    /// </summary>
    public class Game
    {
        public event Action<Player> OnFirstPlayerChosen;

        public static Random Rng = new Random();

        private Player activePlayer;
        private StepsAndPhases activeStep;
        private ExtraTurnStack extraTurns;
        private List<Player> players;
        private List<Team> teams;

        public Game()
        {
            activePlayer = null;
            activeStep = StepsAndPhases.UntapStep;
            extraTurns = new ExtraTurnStack();
            players = new List<Player>();
            teams = new List<Team>();
        }

        #region Public Functions
        /// <summary>
        /// Starts a new match, choosing randomly amongst losers to decide
        /// who starts.
        /// </summary>
        public void StartMatch()
        {
            List<Player> newPlayers = new List<Player>();

            foreach (Player player in players)
            {
                newPlayers.Add(new Player(player.Life, player.Team));
            }

            players = newPlayers;
            Player startingPlayer = GetStartingPlayer();
        }

        public void NextTurn()
        {
            if (extraTurns.HasTurn())
            {
                ExtraTurn turn = extraTurns.GetTurn();
                // turn.Source.Source.Source
                // ability.card with ability.player
            }
        }
        #endregion

        #region Helper Functions
        /// <summary>
        /// Returns the player that goes first at the start of a match.
        /// </summary>
        public Player GetStartingPlayer()
        {
            var losers = players.Where((o) => o.HasLost).ToList();
            if (losers.Count > 0)
            {
                return losers
                    .OrderBy((o) => Rng.Next())
                    .First();
            }

            return players
                .OrderBy((o) => Rng.Next())
                .First();
        }
        #endregion
    }
}
