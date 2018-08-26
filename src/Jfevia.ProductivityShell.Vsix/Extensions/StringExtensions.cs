using System;
using EnvDTE;

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

        /// <summary>
        ///     Converts the string to its language equivalent.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The language.</returns>
        /// <exception cref="ArgumentOutOfRangeException">value - null</exception>
        public static string ToLanguage(this string value)
        {
            switch (value)
            {
                case CodeModelLanguageConstants.vsCMLanguageVB:
                    return "VisualBasic";
                case CodeModelLanguageConstants.vsCMLanguageCSharp:
                    return "CSharp";
                case CodeModelLanguageConstants.vsCMLanguageVC:
                case CodeModelLanguageConstants.vsCMLanguageMC:
                    return "Cpp";
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }

        /// <summary>
        ///     Converts the string to its equivalent default extension.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The default extension.</returns>
        /// <exception cref="ArgumentOutOfRangeException">value - null</exception>
        public static string ToDefaultExtension(this string value)
        {
            switch (value)
            {
                case "VisualBasic":
                    return ".vb";
                case "CSharp":
                    return ".cs";
                case "Cpp":
                    return "cpp";
                default:
                    throw new ArgumentOutOfRangeException(nameof(value), value, null);
            }
        }
    }
}