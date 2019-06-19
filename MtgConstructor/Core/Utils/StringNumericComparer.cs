using System.Collections.Generic;

namespace MtgConstructor.Utils
{
    /// <summary>
    /// Sorts lexicographically for strings and numerically for numbers.
    /// </summary>
    public class StringNumericComparer : IComparer<string>
    {
        public int Compare(string val1, string val2)
        {
            bool val1AsDouble = double.TryParse(val1, out double xVal);
            bool val2AsDouble = double.TryParse(val2, out double yVal);

            if (val1AsDouble && val2AsDouble)
            {
                return xVal.CompareTo(yVal);
            }

            if (!val1AsDouble && !val2AsDouble)
            {
                return val1.CompareTo(val2);
            }

            //Sorts numbers first and strings last.
            if (val1AsDouble)
            {
                return -1;
            }

            return 1;
        }
    }
}