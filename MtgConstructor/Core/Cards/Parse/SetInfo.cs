using MtgConstructor.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace MtgConstructor.Cards.Parse
{
    public class SetCollectionInfo
    {
        public string Object { get; set; }
        [JsonProperty("has_more")]
        public bool HasMore { get; set; }
        public List<SetInfo> Data { get; set; }
    }

    public class SetInfo
    {
        public string Object { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Uri { get; set; }
        [JsonProperty("scryfall_uri")]
        public string ScryfallUri { get; set; }
        [JsonProperty("search_uri")]
        public string SearchUri { get; set; }
        [JsonProperty("released_at")]
        public string ReleasedAt { get; set; }
        [JsonProperty("set_type")]
        public SetType SetType { get; set; }
        [JsonProperty("card_count")]
        public int CardCount { get; set; }
        public bool Digital { get; set; }
        [JsonProperty("foil_only")]
        public bool FoilOnly { get; set; }
        [JsonProperty("icon_svg_uri")]
        public string IconSvgUri { get; set; }
        [JsonProperty("parent_set_code")]
        public string ParentSetCode { get; set; }
        [JsonProperty("mtgo_code")]
        public string MtgoCode { get; set; }
        [JsonProperty("block_code")]
        public string BlockCode { get; set; }
        public string Block { get; set; }
    }

    /// <summary>
    /// A list of supported set types.
    /// </summary>
    public enum SetType
    {
        [AsString("core")]
        Core,
        [AsString("expansion")]
        Expansion,
        [AsString("masters")]
        Masters,
        [AsString("masterpiece")]
        Masterpiece,
        [EnumMember(Value = "from_the_vault")]
        [AsString("from the vault")]
        FromTheVault,
        [AsString("spellbook")]
        Spellbook,
        [EnumMember(Value = "premium_deck")]
        [AsString("premium deck")]
        PremiumDeck,
        [EnumMember(Value = "duel_deck")]
        [AsString("duel deck")]
        DuelDeck,
        [EnumMember(Value = "draft_innovation")]
        [AsString("draft innovation")]
        DraftInnovation,
        [EnumMember(Value = "treasure_chest")]
        [AsString("treasure chest")]
        TreasureChest,
        [AsString("commander")]
        Commander,
        [AsString("planechase")]
        Planechase,
        [AsString("archenemy")]
        Archenemy,
        [AsString("vanguard")]
        Vanguard,
        [AsString("funny")]
        Funny,
        [AsString("starter")]
        Starter,
        [AsString("box")]
        Box,
        [AsString("promo")]
        Promo,
        [AsString("token")]
        Token,
        [AsString("memorabilia")]
        Memorabilia
    }
}
