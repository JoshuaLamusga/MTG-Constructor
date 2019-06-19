using System;
using System.Collections.Generic;
using System.Text;

namespace MtgConstructor.Game
{
    public enum GameFormat
    {
        /// <summary>
        /// A player is designated as the archenemy and plays the top card of
        /// the scheme deck from their command zone at the start of each of
        /// their turns. Everyone else is on a team against the archenemy.
        /// Archenemy life total is 40, and they take the first turn. 
        /// </summary>
        Archenemy,

        /// <summary>
        /// Similar to Archenemy, except everyone has a scheme deck in their
        /// command zone, starts with 40 life, and plays the top card of it
        /// at the start of each of their turns. Who goes first is chosen
        /// randomly.
        /// </summary>
        ArchenemyFreeForAll,

        /// <summary>
        /// Regular except that range of influence is set to a random player
        /// for each player. If a player gets themself via range of influence,
        /// they can instead target anyone. Killing a target acquires their
        /// range of influence.
        /// </summary>
        Assassin,

        /// <summary>
        /// Similar to commander, except players start with 25 life for two
        /// players, or 30 for more. Any legendary planeswalker can be a
        /// commander (or legendary creature). 1 relaxed mulligan. 21 damage
        /// with a commander doesn't end the game.
        /// </summary>
        Brawl,

        /// <summary>
        /// Commanders are taken out of the deck and into the command zone
        /// before players draw. Some commanders permit a second commander.
        /// The player may cast their commander from the command zone as if it
        /// were in their hand, adding (2) to the mana cost for each time it's
        /// been cast already this game. If a commander were to leave the
        /// battlefield, the player may put it into their command zone instead.
        /// If a commander deals 21+ combat damage to a player, that player
        /// loses the game. Players start with 40 life.
        /// </summary>
        Commander,

        /// <summary>
        /// Two teams of three players. One of them on each side is an
        /// 'emperor' and the rest can't target their enemy emperor. Each
        /// creature has "T: Target teammate gains control of this creature".
        /// </summary>
        Emperor,

        /// <summary>
        /// Players draft instead of drawing. They don't lose the game for
        /// drawing from an empty library. Libraries are shuffled before a
        /// search is performed. The library is the cards they draft from.
        /// </summary>
        LiveBoosterDraft,

        /// <summary>
        /// Similar to live booster drafting, except the cards are randomly
        /// assigned from a pool.
        /// </summary>
        LiveCubeDraft,

        /// <summary>
        /// Each player has a planar deck in addition to their normal deck. At
        /// the start of the game, each player flips over the top card of their
        /// planar deck. When phenomenons are flipped, keep going until a plane
        /// is flipped. Each card flipped affects the game as written. As a
        /// sorcery, players can 'roll a die' to get chaos or planeswalk, to
        /// either activate the shown plane's chaos ability or flip to the next
        /// plane, and can pay cumulative (1) for each time they've activated
        /// the ability on the same turn to roll again.
        /// </summary>
        Planar,

        /// <summary>
        /// Like planar, except players share a communal deck of plane cards.
        /// </summary>
        PlanarCommunal,

        /// <summary>
        /// A game with any number of players in any configuration of teams,
        /// following their designated range of influence, if any. Being on
        /// the same team only means allies win if all other teams are
        /// destroyed, and lose only when all allies are destroyed. This
        /// format doesn't combine life, mana, or turns.
        /// </summary>
        Regular,

        /// <summary>
        /// Each player uses a different mono-colored deck, with exactly five
        /// players in the game. Decks are validated when players choose their
        /// roles. 
        /// </summary>
        Star,

        /// <summary>
        /// A variant of regular where teams are restricted to exactly two
        /// people. Life and turns are shared among allies, and they all get
        /// priority before the enemy team does. Traditionally, this is played
        /// with two teams, but this game enables more. Mana isn't shared.
        /// </summary>
        TwoHeadedGiant,

        /// <summary>
        /// A variant of two-headed giant where allies share one mana pool.
        /// </summary>
        TwoHeadedGiantShareLands,

        /// <summary>
        /// A draft format where players have as much mana as they'd like
        /// to draw, but can only cast one spell per turn. Each player starts
        /// with five cards.
        /// </summary>
        Type4,

        /// <summary>
        /// Players start the game with a card that modifies their starting
        /// and max hand, life, and gives them a static ability effective
        /// throughout the game.
        /// </summary>
        Vanguard
    }
}
