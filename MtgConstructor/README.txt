This readme will contain advice about this library and what it can do.
For now, this functions as a list of features:

Done:
- Advanced card searches
- Deck generation

Todo:
* Test edge cases of CardSlot functionality, especially behavior with percentages.
- Finish deck generation in DeckGenHelper and remove partial implementation in Program.cs.
- Check DeckGenerator for completeness and test behavior.
- Compare ReadCards in CardDataRetrieval.cs to ReadDeck in Parser.cs. Move the better one to CardDataRetrieval.
- Move DetectDeckFormat from Parser.cs to something more suitable. Let CardById and CardByName get superceded by whichever is better: Parser.cs or CardDataRetrieval.cs, and move the better one to CardDataRetrieval.cs.
- Parser should have nothing but a homeless ExportCards function and static card loading. Move static loading to Program.cs.
- Test all Get*() methods in CardInfo for rigor.
- Make a GameInfo folder with enum lists of all sorts of different values, or arrays generated from a cardpool (as an faster alternative to the catalog API call).
- Look at FilterQuery for flaws, incomplete areas, and vulnerabilities with regex include/exclude and string toLower, value comparisons across keys, etc.
- FilterQuery functions that may throw a FilterQueryException should make note of such.
- Look at SortQuery to make sure it works, test edge cases, look for vulnerabilities.
- CardCollection should have an option to support cards that aren't strictly equal; Card should have an overload for equality comparison.
- Ensure deck validations work on an existing deck, on a card to be added to a deck, and check the deck's supposed formats.
- Make sure everything is using CardInfo, Card, List<Card>, CardCollection, Deck in an appropriate manner. Make overloads if convenient.
- Check DeckMetadata for functionality, purpose, vulnerabilities on edge cases, etc.
- Ensure deck validations take banlists into consideration.
- Handle Drafts and make sure they construct a subsequent Game properly. Add an option to dry-test without launching into a game.
- Handle drafts during a game after handling basic game turn structures. Setup game for other game modes.
- Finish the game aspect. Good luck with that.

Roadmap:
- Deck generation from sample hand / cards
- Deck analysis
- Deck editor (gui)
- Deck validations and detection of valid format
- Drafting under different rules with X players
- Draft simulation and draft into game (gui)
- Gameplay under simple formats
- Gameplay under other formats