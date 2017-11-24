using System;

namespace ProductivityShell.Extensions
{
    internal static class StringExtensions
    {
        public static string Replace(this string text, string search, string replace, StringComparison comparisonType, int count)
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