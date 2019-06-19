using System;

namespace MtgConstructor.Utils
{
    public static class TypeOperations
    {
        /// <summary>
        /// Returns true if both numbers are within 0.001 of each other.
        /// </summary>
        private static bool IsWithinRange(double numericValue1, double numericValue2)
        {
            return Math.Abs(numericValue1 - numericValue2) <= 0.001;
        }

        /// <summary>
        /// Returns true if both values are equal when represented as strings.
        /// </summary>
        public static bool IsLooselyEqual(object value1, object value2, bool strictCasing)
        {
            if (value1 == null || value2 == null)
            {
                return value1 == value2;
            }

            if (double.TryParse(value1.ToString(), out double value1Double) &&
                double.TryParse(value2.ToString(), out double value2Double))
            {
                return IsWithinRange(value1Double, value2Double);
            }

            if (strictCasing)
            {
                return value1.ToString() == value2.ToString();
            }

            return value1.ToString().ToLower() == value2.ToString().ToLower();
        }
    }
}
