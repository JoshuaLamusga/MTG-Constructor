using System.Collections.Generic;

namespace MtgConstructor.Cards.Parse
{
    /// <summary>
    /// Describes a list of data.
    /// </summary>
    public class CatalogInfo
    {
        public string @object { get; set; }
        public string uri { get; set; }
        public int total_values { get; set; }
        public List<string> data { get; set; }
    }
}
