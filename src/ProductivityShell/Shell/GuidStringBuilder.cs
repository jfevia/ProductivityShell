using System;

namespace ProductivityShell.Shell
{
    internal sealed class GuidStringBuilder
    {
        public Guid Guid { get; set; } = default(Guid);

        public GuidFormattingOption Format { get; set; } = GuidFormattingOption.HyphenSeparation;

        public LetterCase Case { get; set; } = LetterCase.UpperCase;

        public override string ToString()
        {
            var guidString = Guid.ToString(GetFormatString(Format));

            return Case == LetterCase.UpperCase ? guidString.ToUpper() : guidString.ToLower();
        }

        private static string GetFormatString(GuidFormattingOption format)
        {
            switch (format)
            {
                case GuidFormattingOption.NoHyphensOrEnclosureSymbols:
                    return "N";
                case GuidFormattingOption.HyphenSeparation:
                    return "D";
                case GuidFormattingOption.HyphenSeparationAndBraceEnclosure:
                    return "B";
                case GuidFormattingOption.HyphenSeparationAndParenthesisEnclosure:
                    return "P";
                case GuidFormattingOption.Hexadecimal:
                    return "X";
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }
        }
    }
}