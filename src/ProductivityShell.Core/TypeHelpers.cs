using System.Globalization;

namespace ProductivityShell.Core
{
    public class TypeHelpers
    {
        public static string FullyQualifiedClassName(string ns, string className)
        {
            return !string.IsNullOrWhiteSpace(ns)
                ? string.Format(CultureInfo.InvariantCulture, "{0}.{1}", ns, className)
                : className;
        }
    }
}