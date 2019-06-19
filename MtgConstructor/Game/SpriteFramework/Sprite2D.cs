using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MtgConstructor.Game.SpriteFramework
{
    /// <summary>
    /// Represents a drawable graphic, which can be loaded upfront or deferred
    /// until requested.
    /// </summary>
    public class Sprite2D
    {
        #region Variables
        /// <summary>
        /// The (counter-clockwise) rotation angle in radians.
        /// Default 0.
        /// </summary>
        public float Angle;

        /// <summary>
        /// The drawing order for overlapping sprites as a value from 0.0 to 1.0, inclusive.
        /// Sprites with a smaller depth are drawn close to last and appear on top of others.
        /// Default 0.
        /// </summary>
        public float Depth;

        /// <summary>
        /// The point in local texture coordinates to rotate, scale, and draw from.
        /// Default <see cref="Vector2.Zero"/>.
        /// </summary>
        public Vector2 Origin;

        /// <summary>
        /// The screen coordinates to draw the texture at.
        /// Default <see cref="Vector2.Zero"/>.
        /// </summary>
        public Vector2 Position;

        /// <summary>
        /// The horizontal and vertical scaling transform to apply to the sprite.
        /// Default <see cref="Vector2.One"/>.
        /// </summary>
        public Vector2 Scaling;

        /// <summary>
        /// The alpha to blend the sprite with. Default 1 (no change).
        /// </summary>
        public float TextureAlphaBlend;

        /// <summary>
        /// A color to blend the sprite with. Default <see cref="Color.White"/> (no change).
        /// </summary>
        public Color TextureColorBlend;

        /// <summary>
        /// Optional mirroring options to draw the texture horizontally and/or vertically flipped.
        /// Default <see cref="SpriteEffects.None"/>.
        /// </summary>
        public SpriteEffects TextureMirroring;

        /// <summary>
        /// The rectangle defining which portion of the source texture to use. If null, the full
        /// dimensions of the texture will be used.
        /// Default <see cref="SmoothRect.Empty"/>.
        /// </summary>
        public SmoothRect TextureSourceRect;

        /// <summary>
        /// The relative project path for a texture.
        /// Default empty string.
        /// </summary>
        public string TextureUri;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a sprite with default settings.
        /// </summary>
        public Sprite2D(string textureUri)
        {
            Angle = 0;
            Depth = 0;
            Origin = Vector2.Zero;
            Position = Vector2.Zero;
            Scaling = Vector2.One;
            TextureAlphaBlend = 1;
            TextureColorBlend = Color.White;
            TextureMirroring = SpriteEffects.None;
            TextureSourceRect = SmoothRect.Empty;
            TextureUri = textureUri;
        }

        /// <summary>
        /// Creates a deep copy of a sprite from another.
        /// </summary>
        public Sprite2D(Sprite2D sprite)
        {
            Angle = sprite.Angle;
            Depth = sprite.Depth;
            Origin = sprite.Origin;
            Position = sprite.Position;
            Scaling = sprite.Scaling;
            TextureAlphaBlend = sprite.TextureAlphaBlend;
            TextureColorBlend = sprite.TextureColorBlend;
            TextureMirroring = sprite.TextureMirroring;
            TextureSourceRect = sprite.TextureSourceRect;
            TextureUri = sprite.TextureUri;
        }
        #endregion

        /// <summary>
        /// Draws the sprite based on its complexity.
        /// </summary>
        /// <param name="spriteBatch"></param>
        /// <param name="mngr"></param>
        public void Draw(SpriteBatch spriteBatch, ContentManager mngr = null)
        {
            // Textures are internally cached by the content manager. If it's pre-loaded, it will
            // be returned. Otherwise, this will load it.
            Texture2D texture = mngr?.Load<Texture2D>(TextureUri);

            Color colorAlphaBlend = TextureColorBlend * TextureAlphaBlend;

            if (texture == null || texture.IsDisposed)
            {
                return;
            }

            if (Angle != 0 || !Origin.Equals(Vector2.Zero) || TextureMirroring != SpriteEffects.None || Depth != 0)
            {
                Rectangle sourceRectangle = TextureSourceRect.Equals(SmoothRect.Empty)
                    ? new Rectangle(0, 0, texture.Width, texture.Height)
                    : TextureSourceRect.ToRect();

                if (Scaling.Equals(Vector2.One))
                {
                    // Texture, position, source, color, rotation, origin, effects, depth
                    spriteBatch.Draw(
                        texture,
                        new Rectangle(
                            (int)Math.Round(Position.X),
                            (int)Math.Round(Position.Y),
                            texture.Width,
                            texture.Height),
                        sourceRectangle,
                        colorAlphaBlend,
                        Angle,
                        Origin,
                        TextureMirroring,
                        Depth);
                }
                else
                {
                    // Texture, position, source, color, rotation, origin, scale, effects, depth
                    spriteBatch.Draw(
                        texture,
                        Position,
                        sourceRectangle,
                        colorAlphaBlend,
                        Angle,
                        Origin,
                        Scaling,
                        TextureMirroring,
                        Depth);
                }
            }
            else if (!TextureSourceRect.Equals(SmoothRect.Empty))
            {
                Rectangle sourceRectangle = TextureSourceRect.Equals(SmoothRect.Empty)
                    ? new Rectangle(0, 0, texture.Width, texture.Height)
                    : TextureSourceRect.ToRect();

                if (Scaling.Equals(Vector2.One))
                {
                    // Texture, position, source, color
                    spriteBatch.Draw(
                        texture,
                        Position,
                        sourceRectangle,
                        colorAlphaBlend);
                }
                else
                {
                    // Texture, position, scale, source, color
                    spriteBatch.Draw(
                        texture,
                        new Rectangle(
                            (int)Math.Round(Position.X),
                            (int)Math.Round(Position.Y),
                            (int)(texture.Width * Scaling.X),
                            (int)(texture.Height * Scaling.Y)),
                        sourceRectangle,
                        colorAlphaBlend);
                }
            }
            else
            {
                if (Scaling.Equals(Vector2.One))
                {
                    // Texture, position, color
                    spriteBatch.Draw(
                        texture,
                        Position,
                        colorAlphaBlend);
                }
                else
                {
                    // Texture, position, scale, color
                    spriteBatch.Draw(
                        texture,
                        new Rectangle(
                            (int)Math.Round(Position.X),
                            (int)Math.Round(Position.Y),
                            (int)(texture.Width * Scaling.X),
                            (int)(texture.Height * Scaling.Y)),
                        colorAlphaBlend);
                }
            }
        }
    }
}
