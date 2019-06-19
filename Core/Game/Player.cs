using MtgConstructor.Cards;
using System;
using System.Collections.Generic;
using System.Text;

namespace MtgConstructor.Game
{
    /// <summary>
    /// Represents a player in an active game.
    /// </summary>
    public class Player
    {
        public bool HasLost;
        public Zone Battlefield;
        public Zone Command;
        public CounterCollection Counters;
        public Zone Exile;
        public Zone Graveyard;
        public Zone Hand;
        public Zone Library;
        public int Life;
        public CardCollection Mainboard;
        public ManaCollection ManaPool;
        public CardCollection Sideboard;
        public Team Team;

        public Player(int life, Team team)
        {
            HasLost = false;
            Battlefield = new Zone(new CardCollection());
            Command = new Zone(new CardCollection());
            Counters = new CounterCollection();
            Exile = new Zone(new CardCollection());
            Graveyard = new Zone(new CardCollection());
            Hand = new Zone(new CardCollection());
            Library = new Zone(new CardCollection());
            Life = life;
            Mainboard = new CardCollection();
            ManaPool = new ManaCollection();
            Sideboard = new CardCollection();
            Team = team;
        }
    }
}
