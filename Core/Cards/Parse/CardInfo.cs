using MtgConstructor.Utils;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace MtgConstructor.Cards.Parse
{
    /// <summary>
    /// Includes information pertaining to a single card.
    /// </summary>
    public class CardInfo : IComparable
    {
        #region Members
        [JsonProperty("all_parts")]
        public List<RelatedCard> AllParts { get; set; }
        [JsonProperty("arena_id")]
        public string ArenaId { get; set; }
        public string artist { get; set; }
        [JsonProperty("border_color")]
        public BorderColors BorderColor { get; set; }
        [JsonProperty("card_faces")]
        public List<CardFace> CardFaces { get; set; }
        public double Cmc { get; set; }
        [JsonProperty("collector_number")]
        public string CollectorNumber { get; set; }
        [JsonProperty("color_identity")]
        public List<Color> ColorIdentity { get; set; }
        [JsonProperty("color_indicator")]
        public List<Color> ColorIndicator { get; set; }
        public List<Color> Colors { get; set; }
        public bool Colorshifted { get; set; }
        public bool Digital { get; set; }
        [JsonProperty("edhrec_rank")]
        public int EdhrecRank { get; set; }
        [JsonProperty("flavor_text")]
        public string FlavorText { get; set; }
        public bool Foil { get; set; }
        public Frames Frame { get; set; }
        [JsonProperty("full_art")]
        public bool FullArt { get; set; }
        public bool Futureshifted { get; set; }
        [JsonProperty("hand_modifier")]
        public string HandModifier { get; set; }
        [JsonProperty("highres_image")]
        public bool HighresImage { get; set; }
        public string Id { get; set; }
        [JsonProperty("illustration_id")]
        public string IllustrationId { get; set; }
        [JsonProperty("image_uris")]
        public ImageUri ImageUris { get; set; }
        public string Lang { get; set; }
        public Layouts Layout { get; set; }
        public LegalityFormats Legalities { get; set; }
        public string LifeModifier { get; set; }
        public string Loyalty { get; set; }
        [JsonProperty("mana_cost")]
        public string ManaCost { get; set; }
        [JsonProperty("mtgo_foil_id")]
        public string MtgoFoilId { get; set; }
        [JsonProperty("mtgo_id")]
        public string MtgoId { get; set; }
        [JsonProperty("multiverse_ids")]
        public List<object> MultiverseIds { get; set; }
        public string Name { get; set; }
        public bool Nonfoil { get; set; }
        public string Object { get; set; }
        [JsonProperty("oracle_id")]
        public string OracleId { get; set; }
        [JsonProperty("oracle_text")]
        public string OracleText { get; set; }
        public bool Oversized { get; set; }
        public string Power { get; set; }
        [JsonProperty("printed_name")]
        public string PrintedName { get; set; }
        [JsonProperty("printed_text")]
        public string PrintedText { get; set; }
        [JsonProperty("printed_type_line")]
        public string PrintedTypeLine { get; set; }
        [JsonProperty("prints_search_uri")]
        public string PrintsSearchUri { get; set; }
        public Rarities Rarity { get; set; }
        [JsonProperty("related_uris")]
        public RelatedUri RelatedUris { get; set; }
        public bool Reserved { get; set; }
        public bool Reprint { get; set; }
        [JsonProperty("rulings_uri")]
        public string RulingsUri { get; set; }
        [JsonProperty("scryfall_set_uri")]
        public string ScryfallSetUri { get; set; }
        [JsonProperty("scryfall_uri")]
        public string ScryfallUri { get; set; }
        public string Set { get; set; }
        [JsonProperty("set_search_uri")]
        public string SetSearchUri { get; set; }
        [JsonProperty("set_name")]
        public string SetName { get; set; }
        [JsonProperty("set_uri")]
        public string SetUri { get; set; }
        [JsonProperty("story_spotlight_number")]
        public int StorySpotlightNumber { get; set; }
        [JsonProperty("story_spotlight_uri")]
        public string StorySpotlightUri { get; set; }
        public bool Timeshifted { get; set; }
        public string Toughness { get; set; }
        [JsonProperty("type_line")]
        public string TypeLine { get; set; }
        public string Uri { get; set; }
        public string Watermark { get; set; }
        #endregion

        #region Enums
        /// <summary>
        /// Represents existing border colors.
        /// </summary>
        public enum BorderColors
        {
            [AsString("black")]
            Black,
            [AsString("borderless")]
            Borderless,
            [AsString("gold")]
            Gold,
            [AsString("silver")]
            Silver,
            [AsString("white")]
            White
        }

        /// <summary>
        /// Represents colors for e.g. card color or identity.
        /// </summary>
        public enum Color
        {
            [AsString("w")]
            W,
            [AsString("u")]
            U,
            [AsString("b")]
            B,
            [AsString("r")]
            R,
            [AsString("g")]
            G
        }

        /// <summary>
        /// Represents the possible layouts of a card.
        /// </summary>
        public enum Layouts
        {
            [AsString("normal")]
            Normal,
            [AsString("split")]
            Split,
            [AsString("flip")]
            Flip,
            [AsString("transform")]
            Transform,
            [AsString("meld")]
            Meld,
            [AsString("leveler")]
            Leveler,
            [AsString("saga")]
            Saga,
            [AsString("planar")]
            Planar,
            [AsString("scheme")]
            Scheme,
            [AsString("vanguard")]
            Vanguard,
            [AsString("token")]
            Token,
            [EnumMember(Value = "double_faced_token")]
            [AsString("double faced token")]
            DoubleFacedToken,
            [AsString("emblem")]
            Emblem,
            [AsString("augment")]
            Augment,
            [AsString("host")]
            Host
        }

        /// <summary>
        /// Represents the card type. Rare includes the rarity called Special.
        /// </summary>
        public enum Rarities
        {
            [AsString("common")]
            Common = 0,
            [AsString("uncommon")]
            Uncommon = 1,
            [AsString("rare")]
            Rare = 2,
            [AsString("mythic")]
            Mythic = 3
        }

        /// <summary>
        /// Represents the major card format in use, identified by date.
        /// </summary>
        public enum Frames
        {
            [EnumMember(Value = "1993")]
            [AsString("1993")]
            Year1993,
            [EnumMember(Value = "1997")]
            [AsString("1997")]
            Year1997,
            [EnumMember(Value = "2003")]
            [AsString("2003")]
            Year2003,
            [EnumMember(Value = "2015")]
            [AsString("2015")]
            Year2015,
            [EnumMember(Value = "future")]
            [AsString("future")]
            Future
        }

        /// <summary>
        /// Represents possible legalities of a card.
        /// </summary>
        public enum LegalityValues
        {
            [AsString("legal")]
            Legal,
            [EnumMember(Value = "not_legal")]
            [AsString("illegal")]
            NotLegal,
            [AsString("restricted")]
            Restricted,
            [AsString("banned")]
            Banned
        }
        #endregion

        #region Internal classes
        /// <summary>
        /// Contains various uris for card images.
        /// </summary>
        public class ImageUri
        {
            [JsonProperty("art_crop")]
            public string ArtCrop { get; set; }
            [JsonProperty("border_crop")]
            public string BorderCrop { get; set; }
            public string Large { get; set; }
            public string Normal { get; set; }
            public string Png { get; set; }
            public string Small { get; set; }
        }

        /// <summary>
        /// Represents different formats with known legality data.
        /// </summary>
        public class LegalityFormats
        {
            public LegalityValues Brawl { get; set; }
            public LegalityValues Commander { get; set; }
            public LegalityValues Duel { get; set; }
            public LegalityValues Frontier { get; set; }
            public LegalityValues Future { get; set; }
            public LegalityValues Legacy { get; set; }
            public LegalityValues Modern { get; set; }
            public LegalityValues Pauper { get; set; }
            public LegalityValues Penny { get; set; }
            public LegalityValues Standard { get; set; }
            public LegalityValues Vintage { get; set; }
        }

        /// <summary>
        /// Represents additional URIs.
        /// </summary>
        public class RelatedUri
        {
            public string Edhrec { get; set; }
            public string Mtgtop8 { get; set; }
            [JsonProperty("tcgplayer_decks")]
            public string TcgplayerDecks { get; set; }
        }

        /// <summary>
        /// Represents the properties of a card that differ between
        /// multiple cards on one physical card (split, transform, etc.)
        /// </summary>
        public class CardFace
        {
            [JsonProperty("color_indicator")]
            public List<Color> ColorIndicator { get; set; }
            public List<Color> Colors { get; set; }
            [JsonProperty("flavor_text")]
            public string FlavorText { get; set; }
            [JsonProperty("illustration_id")]
            public string IllustrationId { get; set; }
            [JsonProperty("image_uris")]
            public ImageUri ImageUris { get; set; }
            public string Loyalty { get; set; }
            [JsonProperty("mana_cost")]
            public string ManaCost { get; set; }
            public string Name { get; set; }
            public string Object { get; set; }
            [JsonProperty("oracle_text")]
            public string OracleText { get; set; }
            public string Power { get; set; }
            [JsonProperty("printed_name")]
            public string PrintedName { get; set; }
            [JsonProperty("printed_text")]
            public string PrintedText { get; set; }
            [JsonProperty("printed_type_line")]
            public string PrintedTypeLine { get; set; }
            public string Toughness { get; set; }
            [JsonProperty("type_line")]
            public string TypeLine { get; set; }
        }

        /// <summary>
        /// Represents a reference to another card.
        /// </summary>
        public class RelatedCard
        {
            public string Id { get; set; }
            public string Name { get; set; }
            public string Object { get; set; }
            public string Uri { get; set; }
        }
        #endregion

        #region Methods
        public int CompareTo(object obj)
        {
            if (obj is string objAsString)
            {
                return Name.CompareTo(objAsString);
            }

            return -1;
        }

        /// <summary>
        /// Returns a list of individual mana symbols.
        /// </summary>
        public List<string> GetManaCost()
        {
            List<string> costs = new List<string>();

            //Matches everything between each set of braces.
            var groups = Regex.Matches(ManaCost, @"(?<={).*?(?=})");
            for (int i = 0; i < groups.Count; i++)
            {
                costs.Add(groups[i].Value);
            }

            return costs;
        }

        /// <summary>
        /// Returns all types in the type line before the dash.
        /// </summary>
        public List<string> GetTypes()
        {
            var types = TypeLine.Split('—');
            if (types.Length > 0)
            {
                return types[0].Split(' ').ToList();
            }

            return new List<string>();
        }

        /// <summary>
        /// Returns all sub types in the type line after the dash.
        /// </summary>
        public List<string> GetSubTypes()
        {
            var types = TypeLine.Split('—');
            if (types.Length > 1)
            {
                return types[1].Split(' ').ToList();
            }

            return new List<string>();
        }
        #endregion
    }
}
