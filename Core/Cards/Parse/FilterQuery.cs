using MtgConstructor.Utils;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace MtgConstructor.Cards.Parse
{
    /// <summary>
    /// Used to compose filter queries that evaluate each card in a collection
    /// and returns a list of cards matching the expression.
    /// </summary>
    public class FilterQuery
    {
        public static bool UseStrictCaseComparison = false;
        public List<FilterQuery> Children { get; private set; }
        public FilterSetRelation SubQueryRelation { get; private set; }
        public FilterQuery Parent { get; private set; }
        private readonly FilterKey key;
        private readonly FilterKeyModifier keyModifier;
        private readonly FilterValueRelation relation;
        private object value;

        /// <summary>
        /// The card property to be tested against a value.
        /// </summary>
        public enum FilterKey
        {
            arenaId,
            artist,
            borderColor,
            cmc,
            collectorNumber,
            colorIdentity,
            colorIndicator,
            colors,
            colorshifted,
            digital,
            edhrecRank,
            flavorText,
            foil,
            frame,
            fullArt,
            futureshifted,
            handModifier,
            id,
            illustrationId,
            invalid,
            lang,
            layout,
            legalInBrawl,
            legalInCommander,
            legalInCommanderOneVsOne,
            legalInFrontier,
            legalInFuture,
            legalInLegacy,
            legalInModern,
            legalInPauper,
            legalInPenny,
            legalInStandard,
            legalInVintage,
            lifeModifier,
            loyalty,
            manaCost,
            mtgoFoilId,
            mtgoId,
            name,
            nonfoil,
            oracleId,
            oracleText,
            oversized,
            power,
            printedName,
            printedText,
            printedTypeLine,
            rarity,
            reprint,
            reserved,
            set,
            setName,
            storySpotlightNumber,
            subTypes,
            timeshifted,
            toughness,
            typeLine,
            types,
            watermark
        }

        /// <summary>
        /// A modifier for the filter key, such as count for list types.
        /// </summary>
        public enum FilterKeyModifier
        {
            Count,
            Invalid
        }

        /// <summary>
        /// The relation to satisfy between the key and value.
        /// </summary>
        public enum FilterValueRelation
        {
            Equal,
            GreaterThan,
            GreaterThanOrEqual,
            Invalid,
            LessThan,
            LessThanOrEqual,
            NotEqual,
            RegexInclude,
            RegexExclude
        }

        /// <summary>
        /// The relationship between queries.
        /// </summary>
        public enum FilterSetRelation
        {
            Union,
            Intersect
        }

        #region Constructors
        /// <summary>
        /// Creates a query for cards matching the given predicate.
        /// </summary>
        public FilterQuery(
            FilterKey key,
            FilterKeyModifier keyModifier,
            FilterValueRelation relation,
            object value)
        {
            Parent = null;
            Children = null;
            SubQueryRelation = FilterSetRelation.Union;
            this.key = key;
            this.keyModifier = keyModifier;
            this.relation = relation;
            this.value = value;

            if (value is string)
            {
                this.value = (value as string).Trim();
            }

            SetParents(Children);
        }

        /// <summary>
        /// Creates a query that relates other queries together.
        /// </summary>
        public FilterQuery(List<FilterQuery> children, FilterSetRelation subQueryRelation)
        {
            Parent = null;
            Children = children;
            SubQueryRelation = subQueryRelation;
            key = FilterKey.name;
            relation = FilterValueRelation.Equal;
            value = null;

            SetParents(children);
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Returns a filter constructed from the given string, or null
        /// if the string is invalid.
        /// </summary>
        public static FilterQuery FromString(string queryString)
        {
            MatchCollection matches = Regex.Matches(queryString, @"\[|\]|\|\||&&");

            // Parses simple queries without further evaluation.
            if (matches.Count == 0)
            {
                return ParseQuery(queryString);
            }

            List<StringOrQuery> stringQueryList = new List<StringOrQuery>();
            int prevIndex = 0;

            // Adds strings between each match as queries, assembling matches in a list.
            for (int i = 0; i < matches.Count; i++)
            {
                string prevString = queryString
                    .Substring(prevIndex, matches[i].Index - prevIndex)
                    .Trim();

                if (prevString.Length > 0)
                {
                    stringQueryList.Add(new StringOrQuery(ParseQuery(prevString)));
                }

                stringQueryList.Add(new StringOrQuery(matches[i].Value));
                prevIndex = matches[i].Index + matches[i].Length;
            }

            // Adds the string after the last match if it exists.
            if (prevIndex < queryString.Length - 1)
            {
                stringQueryList.Add(new StringOrQuery(ParseQuery(queryString.Substring(prevIndex))));
            }

            // Progressively converts innermost expressions to compound queries until there is only 1 query.
            // This works by repeatedly simplifying a range of elements in the same bracket depth with the same set operator.
            while (stringQueryList.Count > 1)
            {
                int index = 0;
                bool doCreateFilter = false;
                FilterSetRelation? setOpType = null;

                for (int i = 0; i < stringQueryList.Count; i++)
                {
                    if (stringQueryList[i].stringValue == "&&"
                        || stringQueryList[i].stringValue == "||")
                    {
                        // Gets the type of filter.
                        if (setOpType == null)
                        {
                            setOpType = (stringQueryList[i].stringValue == "&&")
                                ? FilterSetRelation.Intersect
                                : FilterSetRelation.Union;
                        }

                        // Performs && first since it has precedence.
                        else if (setOpType == FilterSetRelation.Intersect
                            && stringQueryList[i].stringValue == "||")
                        {
                            doCreateFilter = true;
                            i--;
                        }
                        else if (setOpType == FilterSetRelation.Union
                            && stringQueryList[i].stringValue == "&&")
                        {
                            setOpType = FilterSetRelation.Intersect;
                            index = i - 1;
                        }
                    }

                    // Resets index on [, finishes filter on ] or end of string.
                    else if (stringQueryList[i].stringValue == "[")
                    {
                        index = i;
                        setOpType = null;
                    }
                    else if (stringQueryList[i].stringValue == "]" ||
                        i == stringQueryList.Count - 1)
                    {
                        doCreateFilter = true;
                    }

                    // Uses elements from index to i as a range to create a compound query.
                    if (doCreateFilter)
                    {
                        doCreateFilter = false;

                        bool startsWithBracket = stringQueryList[index].stringValue == "[";
                        bool endsWithBracket = stringQueryList[i].stringValue == "]";

                        // Removes extraneous [] brackets around 1 item.
                        if (startsWithBracket && endsWithBracket && i == index + 2)
                        {
                            stringQueryList.RemoveAt(i);
                            stringQueryList.RemoveAt(index);
                            break;
                        }

                        // Don't delete brackets unless range starts and ends with brackets.
                        if (startsWithBracket && !endsWithBracket)
                        {
                            index++;
                        }
                        if (!startsWithBracket && endsWithBracket)
                        {
                            i--;
                        }

                        // Get queries in range, replace matched range with compound query.
                        List<FilterQuery> queryItems = stringQueryList
                            .GetRange(index, i + 1 - index)
                            .Where(o => o.queryValue != null)
                            .Select(o => o.queryValue)
                            .ToList();

                        stringQueryList.RemoveRange(index, i + 1 - index);

                        stringQueryList.Insert(
                            index, new StringOrQuery(new FilterQuery(
                                queryItems,
                                setOpType ?? FilterSetRelation.Union)));

                        break;
                    }
                }
            }

            return stringQueryList[0].queryValue;
        }

        /// <summary>
        /// Returns a simple filter query based on its string representation.
        /// </summary>
        private static FilterQuery ParseQuery(string queryString)
        {
            var matches = Regex.Matches(queryString.ToLower().Trim(), @"[\w]+|[^\w\s]+");
            if (matches.Count > 2)
            {
                FilterKey key = GetFilterKeyFromString(matches[0].Value);
                FilterKeyModifier mod = GetFilterKeyModifierFromString(matches[1].Value);
                FilterValueRelation rel;
                string val = string.Empty;

                if (mod != FilterKeyModifier.Invalid)
                {
                    if (matches.Count < 4)
                    {
                        throw new FilterQueryException($"Expected 4 matches. Got {matches.Count} matches instead.");
                    }

                    rel = GetFilterValueRelationFromString(matches[2].Value);
                    val = string.Join("", matches.Cast<Match>().Select(o => o.Value).ToArray(), 3, matches.Count - 3);
                }
                else
                {
                    rel = GetFilterValueRelationFromString(matches[1].Value);
                    val = string.Join("", matches.Cast<Match>().Select(o => o.Value).ToArray(), 2, matches.Count - 2);
                }

                if (key == FilterKey.invalid ||
                    rel == FilterValueRelation.Invalid)
                {
                    return null;
                }

                return new FilterQuery(key, mod, rel, val);
            }

            throw new FilterQueryException($"Only {matches.Count} matches were found.");
        }

        /// <summary>
        /// Returns a depth-first node map of connected queries from a
        /// composite query.
        /// </summary>
        private static Queue<FilterQuery> GetNodeMap(FilterQuery query)
        {
            bool isDescending = true;
            FilterQuery node = query;
            Queue<FilterQuery> nodeMap = new Queue<FilterQuery>();

            do
            {
                if (isDescending && node.Children?.Count > 0)
                {
                    node = node.Children[0];
                }
                else
                {
                    int index = node.Parent.Children.IndexOf(node);
                    nodeMap.Enqueue(node);
                    isDescending = false;

                    if (index + 1 < node.Parent.Children.Count)
                    {
                        node = node.Parent.Children[index + 1];
                        isDescending = true;
                    }
                    else
                    {
                        node = node.Parent;
                        isDescending = false;
                    }
                }
            } while (node != query);

            nodeMap.Enqueue(query);
            return nodeMap;
        }

        /// <summary>
        /// Returns the filter key matching the given text, or
        /// FilterKey.Invalid if no match was found.
        /// </summary>
        private static FilterKey GetFilterKeyFromString(string strKey)
        {
            switch (strKey.ToLower().Trim())
            {
                case "arenaid":
                    return FilterKey.arenaId;
                case "artist":
                case "art":
                    return FilterKey.artist;
                case "bordercolor":
                case "border":
                    return FilterKey.borderColor;
                case "convertedmanacost":
                case "cmc":
                    return FilterKey.cmc;
                case "collectornumber":
                    return FilterKey.collectorNumber;
                case "coloridentity":
                case "cid":
                    return FilterKey.colorIdentity;
                case "colorindicator":
                case "cin":
                    return FilterKey.colorIndicator;
                case "colors":
                case "color":
                case "c":
                    return FilterKey.colors;
                case "colorshifted":
                    return FilterKey.colorshifted;
                case "digital":
                    return FilterKey.digital;
                case "edhrank":
                    return FilterKey.edhrecRank;
                case "flavortext":
                case "flavor":
                    return FilterKey.flavorText;
                case "foil":
                    return FilterKey.foil;
                case "frame":
                case "f":
                    return FilterKey.frame;
                case "fullart":
                    return FilterKey.fullArt;
                case "futureshifted":
                    return FilterKey.futureshifted;
                case "handmodifier":
                case "hm":
                    return FilterKey.handModifier;
                case "id":
                    return FilterKey.id;
                case "illustrationid":
                    return FilterKey.illustrationId;
                case "lang":
                    return FilterKey.lang;
                case "layout":
                case "l":
                    return FilterKey.layout;
                case "brawl":
                    return FilterKey.legalInBrawl;
                case "commander":
                    return FilterKey.legalInCommander;
                case "duel":
                case "1v1":
                    return FilterKey.legalInCommanderOneVsOne;
                case "frontier":
                    return FilterKey.legalInFrontier;
                case "future":
                    return FilterKey.legalInFuture;
                case "legacy":
                    return FilterKey.legalInLegacy;
                case "modern":
                    return FilterKey.legalInModern;
                case "pauper":
                    return FilterKey.legalInPauper;
                case "penny":
                    return FilterKey.legalInPenny;
                case "standard":
                    return FilterKey.legalInStandard;
                case "vintage":
                    return FilterKey.legalInVintage;
                case "lifemodifier":
                case "lm":
                    return FilterKey.lifeModifier;
                case "loyalty":
                    return FilterKey.loyalty;
                case "manacost":
                case "mc":
                    return FilterKey.manaCost;
                case "mtgofoilid":
                    return FilterKey.mtgoFoilId;
                case "mtgoid":
                    return FilterKey.mtgoId;
                case "name":
                case "n":
                    return FilterKey.name;
                case "nonfoil":
                    return FilterKey.nonfoil;
                case "oracleid":
                    return FilterKey.oracleId;
                case "oracletext":
                    return FilterKey.oracleText;
                case "oversized":
                    return FilterKey.oversized;
                case "power":
                case "p":
                    return FilterKey.power;
                case "printedname":
                    return FilterKey.printedName;
                case "printedtext":
                    return FilterKey.printedText;
                case "printedtypeline":
                    return FilterKey.printedTypeLine;
                case "rarity":
                case "r":
                    return FilterKey.rarity;
                case "reprint":
                    return FilterKey.reprint;
                case "reserved":
                    return FilterKey.reserved;
                case "set":
                case "s":
                    return FilterKey.set;
                case "setname":
                case "sn":
                    return FilterKey.setName;
                case "storyspotlightnumber":
                    return FilterKey.storySpotlightNumber;
                case "subtypes":
                case "st":
                    return FilterKey.subTypes;
                case "timeshifted":
                    return FilterKey.timeshifted;
                case "toughness":
                case "tn":
                    return FilterKey.toughness;
                case "typeline":
                    return FilterKey.typeLine;
                case "types":
                case "t":
                    return FilterKey.types;
                case "watermark":
                    return FilterKey.watermark;
                default:
                    return FilterKey.invalid;
            }
        }

        /// <summary>
        /// Returns a string representation of the given filter key.
        /// </summary>
        private static string GetStringFromFilterKey(FilterKey key)
        {
            switch (key)
            {
                case FilterKey.legalInBrawl:
                    return "brawl";
                case FilterKey.legalInCommander:
                    return "commander";
                case FilterKey.legalInCommanderOneVsOne:
                    return "duel";
                case FilterKey.legalInFrontier:
                    return "frontier";
                case FilterKey.legalInFuture:
                    return "future";
                case FilterKey.legalInLegacy:
                    return "legacy";
                case FilterKey.legalInModern:
                    return "modern";
                case FilterKey.legalInPauper:
                    return "pauper";
                case FilterKey.legalInPenny:
                    return "penny";
                case FilterKey.legalInStandard:
                    return "standard";
                case FilterKey.legalInVintage:
                    return "vintage";
                default:
                    return key.ToString();
            }
        }

        /// <summary>
        /// Returns the filter key modifier matching the given text, or
        /// FilterKeyModifier.Invalid if no match was found.
        /// </summary>
        private static FilterKeyModifier GetFilterKeyModifierFromString(string strMod)
        {
            switch (strMod.ToLower().Trim())
            {
                case "count":
                    return FilterKeyModifier.Count;
                default:
                    return FilterKeyModifier.Invalid;
            }
        }

        /// <summary>
        /// Returns a string representation of the given filter key modifier.
        /// </summary>
        private static string GetStringFromFilterKeyModifier(FilterKeyModifier mod)
        {
            switch (mod)
            {
                case FilterKeyModifier.Count:
                    return "count";
                default:
                    return null;
            }
        }

        /// <summary>
        /// Returns the filter value relation matching the given text, or
        /// FilterValueRelation.Invalid if no match was found.
        /// </summary>
        private static FilterValueRelation GetFilterValueRelationFromString(string strRel)
        {
            switch (strRel.ToLower().Trim())
            {
                case "=":
                    return FilterValueRelation.Equal;
                case ">":
                    return FilterValueRelation.GreaterThan;
                case ">=":
                    return FilterValueRelation.GreaterThanOrEqual;
                case "<":
                    return FilterValueRelation.LessThan;
                case "<=":
                    return FilterValueRelation.LessThanOrEqual;
                case "!=":
                    return FilterValueRelation.NotEqual;
                case "excludes":
                    return FilterValueRelation.RegexExclude;
                case "includes":
                    return FilterValueRelation.RegexInclude;
                default:
                    return FilterValueRelation.Invalid;
            }
        }

        /// <summary>
        /// Returns a string representation of the given filter value relation.
        /// </summary>
        private static string GetStringFromFilterValueRelation(FilterValueRelation rel)
        {
            switch (rel)
            {
                case FilterValueRelation.Equal:
                    return "=";
                case FilterValueRelation.GreaterThan:
                    return ">";
                case FilterValueRelation.GreaterThanOrEqual:
                    return ">=";
                case FilterValueRelation.LessThan:
                    return "<";
                case FilterValueRelation.LessThanOrEqual:
                    return "<=";
                case FilterValueRelation.NotEqual:
                    return "!=";
                case FilterValueRelation.RegexExclude:
                    return "excludes";
                case FilterValueRelation.RegexInclude:
                    return "includes";
                default:
                    return null;
            }
        }

        /// <summary>
        /// Returns the value of the field named by the given key.
        /// </summary>
        private static object GetField(CardInfo card, FilterKey key)
        {
            switch (key)
            {
                case FilterKey.arenaId:
                    return card.ArenaId;
                case FilterKey.artist:
                    return card.artist;
                case FilterKey.borderColor:
                    return card.BorderColor.ToString();
                case FilterKey.cmc:
                    return card.Cmc;
                case FilterKey.collectorNumber:
                    return card.CollectorNumber;
                case FilterKey.colors:
                    List<string> colorsList = new List<string>();
                    for (int i = 0; i < card.Colors?.Count; i++)
                    {
                        CardInfo.Color color = card.Colors[i];
                        colorsList.Add(AsString.Get(color));
                    }
                    return colorsList;
                case FilterKey.colorshifted:
                    return card.Colorshifted;
                case FilterKey.colorIdentity:
                    List<string> colorIdList = new List<string>();
                    for (int i = 0; i < card.ColorIdentity.Count; i++)
                    {
                        CardInfo.Color color = card.ColorIdentity[i];
                        colorIdList.Add(AsString.Get(color));
                    }
                    return colorIdList;
                case FilterKey.colorIndicator:
                    List<string> colorIndList = new List<string>();
                    for (int i = 0; i < card.ColorIndicator.Count; i++)
                    {
                        CardInfo.Color color = card.ColorIndicator[i];
                        colorIndList.Add(AsString.Get(color));
                    }
                    return colorIndList;
                case FilterKey.digital:
                    return card.Digital;
                case FilterKey.edhrecRank:
                    return card.EdhrecRank;
                case FilterKey.flavorText:
                    return card.FlavorText;
                case FilterKey.foil:
                    return card.Foil;
                case FilterKey.frame:
                    return card.Frame.ToString();
                case FilterKey.fullArt:
                    return card.FullArt;
                case FilterKey.futureshifted:
                    return card.Futureshifted;
                case FilterKey.handModifier:
                    return card.HandModifier;
                case FilterKey.id:
                    return card.Id;
                case FilterKey.illustrationId:
                    return card.IllustrationId;
                case FilterKey.lang:
                    return card.Lang;
                case FilterKey.layout:
                    return card.Layout.ToString();
                case FilterKey.legalInBrawl:
                    return card.Legalities.Brawl.ToString();
                case FilterKey.legalInCommander:
                    return card.Legalities.Commander.ToString();
                case FilterKey.legalInCommanderOneVsOne:
                    return card.Legalities.Duel.ToString();
                case FilterKey.legalInFrontier:
                    return card.Legalities.Frontier.ToString();
                case FilterKey.legalInFuture:
                    return card.Legalities.Future.ToString();
                case FilterKey.legalInLegacy:
                    return card.Legalities.Legacy.ToString();
                case FilterKey.legalInModern:
                    return card.Legalities.Modern.ToString();
                case FilterKey.legalInPauper:
                    return card.Legalities.Pauper.ToString();
                case FilterKey.legalInPenny:
                    return card.Legalities.Penny.ToString();
                case FilterKey.legalInStandard:
                    return card.Legalities.Standard.ToString();
                case FilterKey.legalInVintage:
                    return card.Legalities.Vintage.ToString();
                case FilterKey.lifeModifier:
                    return card.LifeModifier;
                case FilterKey.loyalty:
                    return card.Loyalty;
                case FilterKey.manaCost:
                    return card.ManaCost;
                case FilterKey.mtgoFoilId:
                    return card.MtgoFoilId;
                case FilterKey.mtgoId:
                    return card.MtgoId;
                case FilterKey.name:
                    return card.Name;
                case FilterKey.nonfoil:
                    return card.Nonfoil;
                case FilterKey.oracleId:
                    return card.OracleId;
                case FilterKey.oracleText:
                    return card.OracleText;
                case FilterKey.oversized:
                    return card.Oversized;
                case FilterKey.power:
                    return card.Power;
                case FilterKey.printedName:
                    return card.PrintedName;
                case FilterKey.printedText:
                    return card.PrintedText;
                case FilterKey.printedTypeLine:
                    return card.PrintedTypeLine;
                case FilterKey.rarity:
                    return card.Rarity.ToString();
                case FilterKey.reprint:
                    return card.Reprint;
                case FilterKey.reserved:
                    return card.Reserved;
                case FilterKey.set:
                    return card.Set;
                case FilterKey.setName:
                    return card.SetName;
                case FilterKey.storySpotlightNumber:
                    return card.StorySpotlightNumber;
                case FilterKey.subTypes:
                    return card.GetSubTypes();
                case FilterKey.timeshifted:
                    return card.Timeshifted;
                case FilterKey.toughness:
                    return card.Toughness;
                case FilterKey.typeLine:
                    return card.TypeLine;
                case FilterKey.types:
                    return card.GetTypes();
                case FilterKey.watermark:
                    return card.Watermark;
                default:
                    return null;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Performs a composite query on the given collection.
        /// </summary>
        public List<CardInfo> Execute(List<CardInfo> collection)
        {
            if (Children == null || Children.Count == 0)
            {
                return ExecuteSimpleQuery(collection);
            }

            Queue<FilterQuery> nodeMap = GetNodeMap(this);
            var collections = new Dictionary<FilterQuery, List<CardInfo>>();

            while (nodeMap.Count > 1)
            {
                FilterQuery node = nodeMap.Dequeue();

                if (!collections.ContainsKey(node.Parent))
                {
                    if (node.Children != null)
                    {
                        collections.Add(node.Parent, collections[node]);
                    }
                    else
                    {
                        collections.Add(node.Parent, node.ExecuteSimpleQuery(collection));
                    }
                }
                else
                {
                    switch (node.Parent.SubQueryRelation)
                    {
                        case FilterSetRelation.Union:
                            collections[node.Parent] = collections[node.Parent]
                                .Union(node.ExecuteSimpleQuery(collection)).ToList();
                            break;
                        case FilterSetRelation.Intersect:
                            collections[node.Parent] = collections[node.Parent]
                                .Intersect(node.ExecuteSimpleQuery(collections[node.Parent])).ToList();
                            break;
                    }
                }
            }

            return collections[this];
        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Automatically sets the parents for queries added as children.
        /// </summary>
        private void SetParents(List<FilterQuery> children)
        {
            for (int i = 0; i < children?.Count; i++)
            {
                children[i].Parent = this;
            }
        }

        /// <summary>
        /// Performs a simple query on the given collection.
        /// </summary>
        private List<CardInfo> ExecuteSimpleQuery(List<CardInfo> collection)
        {
            if (Children != null)
            {
                return new List<CardInfo>();
            }

            return collection.Where(o =>
            {
                if (GetField(o, key) is List<string> fieldAsList)
                {
                    if (keyModifier == FilterKeyModifier.Count)
                    {
                        return CompareExpression(fieldAsList.Count);
                    }
                    else if (relation == FilterValueRelation.Equal ||
                        relation == FilterValueRelation.RegexInclude)
                    {
                        return fieldAsList.Any(b => CompareExpression(b));
                    }
                    else if (relation == FilterValueRelation.NotEqual ||
                      relation == FilterValueRelation.RegexExclude)
                    {
                        return fieldAsList.All(b => CompareExpression(b));
                    }
                }

                return CompareExpression(GetField(o, key));
            }).ToList();
        }

        /// <summary>
        /// Returns whether the given field matches this object's value.
        /// </summary>
        private bool CompareExpression(object field)
        {
            switch (relation)
            {
                case FilterValueRelation.Equal:
                    return TypeOperations.IsLooselyEqual(field, value, UseStrictCaseComparison);
                case FilterValueRelation.NotEqual:
                    return !TypeOperations.IsLooselyEqual(field, value, UseStrictCaseComparison);
            }

            if (field == null || value == null)
            {
                return false;
            }

            switch (relation)
            {
                case FilterValueRelation.RegexInclude:
                    try
                    {
                        return Regex.IsMatch(
                            field.ToString(),
                            value.ToString(), RegexOptions.IgnoreCase);
                    }
                    catch
                    {
                        return false;
                    }
                case FilterValueRelation.RegexExclude:
                    try
                    {
                        return !Regex.IsMatch(
                            field.ToString(),
                            value.ToString(), RegexOptions.IgnoreCase);
                    }
                    catch
                    {
                        return false;
                    }
            }

            if (double.TryParse(field.ToString(), out double fieldAsDouble)
                && double.TryParse(value.ToString(), out double valueAsDouble))
            {
                switch (relation)
                {
                    case FilterValueRelation.GreaterThan:
                        return fieldAsDouble > valueAsDouble;
                    case FilterValueRelation.GreaterThanOrEqual:
                        return fieldAsDouble >= valueAsDouble;
                    case FilterValueRelation.LessThan:
                        return fieldAsDouble < valueAsDouble;
                    case FilterValueRelation.LessThanOrEqual:
                        return fieldAsDouble <= valueAsDouble;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns a string constructed from a simple query.
        /// </summary>
        private string ToStringSimpleQuery()
        {
            return $"{GetStringFromFilterKey(key)} {GetStringFromFilterValueRelation(relation)} {value}";
        }
        #endregion

        #region Internal Classes
        private struct StringOrQuery
        {
            public string stringValue;
            public FilterQuery queryValue;

            public StringOrQuery(string stringValue)
            {
                this.stringValue = stringValue;
                queryValue = null;
            }

            public StringOrQuery(FilterQuery queryValue)
            {
                stringValue = null;
                this.queryValue = queryValue;
            }
        }
        #endregion
    }
}
