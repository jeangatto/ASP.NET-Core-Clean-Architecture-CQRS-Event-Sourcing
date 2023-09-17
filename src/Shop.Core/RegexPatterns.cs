using System.Text.RegularExpressions;

namespace Shop.Core;

public static partial class RegexPatterns
{
    /// <summary>
    /// Regular expression pattern to validate email addresses
    /// </summary>
    public static readonly Regex EmailIsValid = EmailRegexPatternAttr();

    [GeneratedRegex(
        @"^([0-9a-zA-Z]([+\-_.][0-9a-zA-Z]+)*)+@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,17})$",
        RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant)]
    private static partial Regex EmailRegexPatternAttr();
}
