using Microsoft.Xna.Framework;
using System;

namespace MtgConstructor.Game.SpriteFramework
{
    /// <summary>
    /// A floating-point rectangle which allows for smooth animation.
    /// </summary>
    public struct SmoothRect : IEquatable<SmoothRect>
    {
        #region Variables
        public float X, Y, Width, Height;

        /// <summary>
        /// The first Y coordinate.
        /// </summary>
        public float Top
        {
            get
            {
                return Y;
            }
        }

        /// <summary>
        /// The second Y coordinate.
        /// </summary>
        public float Bottom
        {
            get
            {
                return Y + Height;
            }
        }

        /// <summary>
        /// The first X coordinate.
        /// </summary>
        public float Left
        {
            get
            {
                return X;
            }
        }

        /// <summary>
        /// The second X coordinate.
        /// </summary>
        public float Right
        {
            get
            {
                return X + Width;
            }
        }

        /// <summary>
        /// The X and Y coordinates.
        /// </summary>
        public Vector2 Position
        {
            get
            {
                return new Vector2(X, Y);
            }
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a rectangle from a position and dimensions.
        /// </summary>
        /// <param name="X">The x-coordinate.</param>
        /// <param name="Y">The y-coordinate.</param>
        /// <param name="Width">The width of the rectangle.</param>
        /// <param name="Height">The height of the rectangle.</param>
        public SmoothRect(float X, float Y, float Width, float Height)
        {
            this.X = X;
            this.Y = Y;
            this.Width = Width;
            this.Height = Height;
        }

        /// <summary>
        /// Creates a rectangle from a position and dimensions.
        /// </summary>
        /// <param name="pos">The X and Y coordinates.</param>
        /// <param name="Width">The width of the rectangle.</param>
        /// <param name="Height">The height of the rectangle.</param>
        public SmoothRect(Vector2 pos, float Width, float Height)
        {
            X = pos.X;
            Y = pos.Y;
            this.Width = Width;
            this.Height = Height;
        }

        /// <summary>
        /// Creates one rectangle from another.
        /// </summary>
        public SmoothRect(SmoothRect rect)
        {
            X = rect.X;
            Y = rect.Y;
            Width = rect.Width;
            Height = rect.Height;
        }
        #endregion

        #region Methods
        /// <summary>
        /// Determines whether the specified rectangle is equivalent.
        /// </summary>
        /// <param name="other">
        /// The rectangle to compare to this rectangle.
        /// </param>
        /// <returns>
        /// True if the objects are equal, false otherwise.
        /// </returns>
        public bool Equals(SmoothRect other)
        {
            return
                X == other.X &&
                Y == other.Y &&
                Width == other.Width &&
                Height == other.Height;
        }

        /// <summary>
        /// Returns a <see cref="Rectangle"/> instance, rounding values to integers.
        /// </summary>
        /// <returns>
        /// A <see cref="Rectangle"/> with the same position and dimensions.
        /// </returns>
        public Rectangle ToRect()
        {
            return new Rectangle(
                (int)Math.Round(X),
                (int)Math.Round(Y),
                (int)Math.Round(Width),
                (int)Math.Round(Height));
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// An empty rectangle with all values set to zero.
        /// </summary>
        public static SmoothRect Empty
        {
            get
            {
                return new SmoothRect(0, 0, 0, 0);
            }
        }

        /// <summary>
        /// Returns a new rectangle just large enough to fit two others.
        /// </summary>
        /// <param name="rect1">
        /// The first rectangle to include in the union.
        /// </param>
        /// <param name="rect2">
        /// The second rectangle to include in the union.
        /// </param>
        /// <returns>
        /// A rectangle that fits around two others.
        /// </returns>
        public static SmoothRect Union(SmoothRect rect1, SmoothRect rect2)
        {
            return new SmoothRect(
                Math.Min(rect1.X, rect2.X),
                Math.Min(rect1.Y, rect2.Y),
                Math.Max(rect1.X + rect1.Width, rect2.X + rect2.Width),
                Math.Max(rect1.Y + rect1.Height, rect2.Y + rect2.Height));
        }

        /// <summary>
        /// Returns whether or not two rectangles overlap.
        /// </summary>
        /// <param name="rect1">
        /// The first rectangle to include in the union.
        /// </param>
        /// <param name="rect2">
        /// The second rectangle to include in the union.
        /// </param>
        /// <returns>
        /// True if the rectangles overlap, false otherwise.
        /// </returns>
        public static bool IsIntersecting(SmoothRect rect1, SmoothRect rect2)
        {
            return rect1.Left <= rect2.Right &&
                rect1.Right >= rect2.Left &&
                rect1.Top <= rect2.Bottom &&
                rect1.Bottom >= rect2.Top;
        }
        #endregion
    }
}