using System;

namespace Jfevia.ProductivityShell.Vsix.Extensions
{
    internal static class StringExtensions
    {
        /// <summary>
        ///     Replaces the specified string.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="search">The search.</param>
        /// <param name="replace">The replace.</param>
        /// <param name="comparisonType">Type of the comparison.</param>
        /// <param name="count">The count.</param>
        /// <returns>The string with all instances replaced.</returns>
        public static string Replace(this string text, string search, string replace, StringComparison comparisonType,
            int count)
        {
            while (count > 0)
            {
                var pos = text.IndexOf(search, comparisonType);
                if (pos < 0)
                    return text;

                text = $"{text.Substring(0, pos)}{replace}{text.Substring(pos + search.Length)}";
                count--;
            }

            return text;
        }
    }
}