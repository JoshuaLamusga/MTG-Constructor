using MtgConstructor.Cards.Parse;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace MtgConstructor.Client
{
    /// <summary>
    /// Performs client interactions with the Scryfall server.
    /// </summary>
    public static class ApiScryfall
    {
        /// <summary>
        /// Returns all cards read from JSON provided by the server.
        /// </summary>
        public static async Task<List<CardInfo>> GetCards()
        {
            using (HttpClient client = new HttpClient())
            {
                string response = await client.GetStringAsync($"https://archive.scryfall.com/json/scryfall-default-cards.json");
                return JsonConvert.DeserializeObject<List<CardInfo>>(response);
            }
        }

        /// <summary>
        /// Returns set collection info.
        /// </summary>
        public static async Task<SetCollectionInfo> GetSets()
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client
                .GetAsync($"https://api.scryfall.com/sets/")
                .ConfigureAwait(false))
                {
                    string data = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return JsonConvert.DeserializeObject<SetCollectionInfo>(data);
                }
            }
        }

        /// <summary>
        /// Returns a byte array representing the image of a card.
        /// </summary>
        public static Task<byte[]> GetCardImage(string imageUrl)
        {
            using (HttpClient client = new HttpClient())
            {
                return client.GetByteArrayAsync(imageUrl);
            }
        }

        /// <summary>
        /// Returns a list of all creature types.
        /// </summary>
        public static Task<CatalogInfo> GetCreatureTypes()
        {
            return GetCatalog<CatalogInfo>("creature-types");
        }

        /// <summary>
        /// Returns a list of strings for the given catalog.
        /// </summary>
        private static async Task<T> GetCatalog<T>(string catalogName)
        {
            using (HttpClient client = new HttpClient())
            {
                using (HttpResponseMessage response = await client
                .GetAsync($"https://api.scryfall.com/catalog/{catalogName}")
                .ConfigureAwait(false))
                {
                    string data = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    return JsonConvert.DeserializeObject<T>(data);
                }
            }
        }
    }
}
