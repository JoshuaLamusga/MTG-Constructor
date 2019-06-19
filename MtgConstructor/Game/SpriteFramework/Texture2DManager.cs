using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MtgConstructor.Game.SpriteFramework
{
    /// <summary>
    /// Stores the textures of the project and their urls. Unless specified otherwise, textures are
    /// lazy-loaded to improve memory usage.
    /// </summary>
    public static class Texture2DManager
    {
        /// <summary>
        /// A list of textures keyed by project reference URI.
        /// </summary>
        private static ConcurrentDictionary<string, Texture2D> textures;

        static Texture2DManager()
        {
            textures = new ConcurrentDictionary<string, Texture2D>();
        }

        /// <summary>
        /// Adds a texture path reference if it has no entry yet. Returns success.
        /// </summary>
        /// <param name="uri">
        /// A URI for a texture local to the project.
        /// </param>
        /// <returns>
        /// True if the uri was successfully added, false if it already exists.
        /// </returns>
        public static bool Add(string uri)
        {
            return textures.TryAdd(uri, null);
        }

        /// <summary>
        /// Adds any number of texture path references, ignoring duplicates.
        /// </summary>
        /// <param name="uris">
        /// A collection of URIs for textures local to the project.
        /// </param>
        public static void Add(ICollection<string> uris)
        {
            for (int i = 0; i < uris.Count; i++)
            {
                textures.TryAdd(uris.ElementAt(i), null);
            }
        }

        /// <summary>
        /// Adds a texture path reference with associated texture, updating the existing entry
        /// if it exists when doUpdate is true.
        /// </summary>
        /// <param name="uri">
        /// A URI for a texture local to the project.
        /// </param>
        /// <param name="texture">
        /// The associated 2D texture if loaded, else null.
        /// </param>
        /// <param name="doUpdate">
        /// If true and the provided URI already exists, its texture will be replaced with the
        /// texture provided.
        /// </param>
        public static void AddOrUpdate(string uri, Texture2D texture, bool doUpdate = false)
        {
            if (!textures.TryAdd(uri, texture) && doUpdate)
            {
                textures[uri]?.Dispose();
                textures[uri] = texture;
            }
        }

        /// <summary>
        /// Adds any number of URIs for textures local to the project with associated textures,
        /// updating the existing entries if doUpdate is true. Adds entries up to the minimum of
        /// the length of the uris and textures arrays.
        /// </summary>
        /// <param name="uris">
        ///A collection of URIs for textures local to the project. This collection should have the
        ///same count as the textures collection.
        /// </param>
        /// <param name="textures">
        /// A collection of 2D Textures to be loaded. This collection should have the same count
        /// as the URIs collection.
        /// </param>
        /// <param name="doUpdate">
        /// If true and any of the provided URIs already exist, their textures will be replaced
        /// with the associated textures provided.
        /// </param>
        public static void AddOrUpdate(
            ICollection<string> uris,
            ICollection<Texture2D> textures,
            bool doUpdate = false)
        {
            for (int i = 0; i < uris.Count && i < textures.Count; i++)
            {
                if (!Texture2DManager.textures.TryAdd(uris.ElementAt(i), textures.ElementAt(i)) && doUpdate)
                {
                    Texture2DManager.textures[uris.ElementAt(i)]?.Dispose();
                    Texture2DManager.textures[uris.ElementAt(i)] = textures.ElementAt(i);
                }
            }
        }

        /// <summary>
        /// Returns whether the given uri is a known key.
        /// </summary>
        /// <param name="uri">
        /// A URI for a texture local to the project.
        /// </param>
        /// <returns>
        /// True if the given uri is a known key, otherwise false.
        /// </returns>
        public static bool Exists(string uri)
        {
            return textures.ContainsKey(uri);
        }

        /// <summary>
        /// Returns the texture associated with a texture path reference. If a content manager
        /// instance is provided and there is no associated texture yet, it will be loaded.
        /// </summary>
        /// <param name="uri">
        /// A URI for a texture local to the project.
        /// </param>
        /// <param name="mngr">
        /// An instance of a content manager.
        /// </param>
        /// <returns>
        /// The cached texture if available, or the texture loaded if a content manager instance is
        /// provided, or null.
        /// </returns>
        public static Texture2D Get(string uri, ContentManager mngr = null)
        {
            if (textures.TryGetValue(uri, out Texture2D texture))
            {
                if (texture == null)
                {
                    textures[uri] = mngr?.Load<Texture2D>(uri);
                    return textures[uri];
                }

                return texture;
            }

            textures.TryAdd(uri, mngr?.Load<Texture2D>(uri));
            return textures[uri];
        }

        /// <summary>
        /// Gets all entries in the texture manager.
        /// </summary>
        /// <returns>
        /// An array of key-value pairs containing all URIs and their corresponding textures.
        /// </returns>
        public static KeyValuePair<string, Texture2D>[] GetAllEntries()
        {
            return textures.ToArray();
        }

        /// <summary>
        /// Adds all of the given texture path URIs to a dictionary if not already present, then
        /// retrieves or loads textures for them, returning a matching list.
        /// </summary>
        /// <param name="uris">
        /// A collection of URIs for textures local to the project.
        /// </param>
        /// <param name="mngr">
        /// An instance of a content manager.
        /// </param>
        public static void LoadTextures(ICollection<string> uris, ContentManager mngr)
        {
            foreach (string uri in uris)
            {
                Texture2D texture = mngr.Load<Texture2D>(uri);
                if (!textures.TryAdd(uri, texture))
                {
                    textures[uri]?.Dispose();
                    textures[uri] = texture;
                }
            }
        }

        /// <summary>
        /// Loads textures with Parallel.ForEach. Adds all of the given texture path URIs to a
        /// dictionary if not already present, then retrieves or loads textures for them in
        /// parallel, returning a matching list.
        /// </summary>
        /// <param name="uris">
        /// A collection of URIs for textures local to the project.
        /// </param>
        /// <param name="mngr">
        /// An instance of a content manager.
        /// </param>
        public static void LoadTexturesParallel(ICollection<string> uris, ContentManager mngr)
        {
            Parallel.ForEach(uris, (uri) =>
            {
                AddOrUpdate(uri, mngr.Load<Texture2D>(uri), true);
            });
        }

        /// <summary>
        /// Removes the entry for the given uri from the texture manager. This should not be needed
        /// often. Use <see cref="UnloadTextures"/> to unload textures instead. Returns success.
        /// </summary>
        /// <param name="uri">
        /// A URI for a texture local to the project.
        /// </param>
        /// <returns>
        /// True if the associated entry was removed, false if the uri was not a known key.
        /// </returns>
        public static bool Remove(string uri)
        {
            bool wasRemoved = textures.TryRemove(uri, out Texture2D removed);
            removed?.Dispose();

            return wasRemoved;
        }

        /// <summary>
        /// Removes each entry for the given uris. This should not be needed often. Use
        /// <see cref="UnloadTextures"/> to unload textures instead.
        /// </summary>
        /// <param name="uris">
        /// A collection of URIs for textures local to the project.
        /// </param>
        public static void Remove(ICollection<string> uris)
        {
            for (int i = 0; i < uris.Count; i++)
            {
                textures.TryRemove(uris.ElementAt(i), out Texture2D removed);
                removed?.Dispose();
            }
        }

        /// <summary>
        /// Removes all entries in the texture manager.
        /// </summary>
        public static void RemoveAll()
        {
            textures.Clear();
        }

        /// <summary>
        /// Disposes and discards all of the given textures.
        /// </summary>
        /// <param name="uris">
        /// A collection of URIs for textures local to the project.
        /// </param>
        public static void UnloadTextures(ICollection<string> uris)
        {
            foreach (string uri in uris)
            {
                textures[uri]?.Dispose();
                textures[uri] = null;
            }
        }

        /// <summary>
        /// Updates the texture for an existing texture path reference.
        /// </summary>
        /// <param name="uri">
        /// A URI in the texture manager for a texture local to the project.
        /// </param>
        /// <param name="texture">
        /// An associated 2D texture if loaded, else null.
        /// </param>
        public static void Update(string uri, Texture2D texture)
        {
            textures[uri]?.Dispose();
            textures[uri] = texture;
        }

        /// <summary>
        /// Updates any number of textures for existing texture path references local to the
        /// project. Updates entries up to the minimum of the length of the uris and textures
        /// arrays.
        /// </summary>
        /// <param name="uri">
        /// A URI in the texture manager for a texture local to the project.
        /// </param>
        /// <param name="texture">
        /// An associated 2D texture if loaded, else null.
        /// </param>
        public static void Update(ICollection<string> uris, ICollection<Texture2D> textures)
        {
            for (int i = 0; i < uris.Count && i < textures.Count; i++)
            {
                Texture2DManager.textures[uris.ElementAt(i)]?.Dispose();
                Texture2DManager.textures[uris.ElementAt(i)] = textures.ElementAt(i);
            }
        }
    }
}
