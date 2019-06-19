using System;

namespace MtgConstructor.Utils
{
    /// <summary>
    /// Simulates string enums with a decorator.
    /// </summary>
    public class AsString : Attribute
    {
        public AsString(string value)
        {
            Value = value;
        }

        public string Value { get; }

        /// <summary>
        /// Returns the string representation of an enum value,
        /// requiring that it uses the AsString decorator.
        /// </summary>
        public static string Get(Enum value)
        {
            string output = null;
            Type type = value.GetType();

            AsString[] attrs = type.GetField(value.ToString())
                .GetCustomAttributes(typeof(AsString), false) as AsString[];

            if (attrs.Length > 0)
            {
                output = attrs[0].Value;
            }

            return output;
        }
    }
}
