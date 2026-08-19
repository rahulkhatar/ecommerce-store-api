using System.Text.RegularExpressions;

namespace ECommerce.Application.Common;

public static partial class Slug
{
    public static string From(string name)
    {
        var lower = name.Trim().ToLowerInvariant();
        var hyphenated = NonAlphanumericRegex().Replace(lower, "-");
        return CollapseHyphensRegex().Replace(hyphenated, "-").Trim('-');
    }

    [GeneratedRegex(@"[^a-z0-9]+")]
    private static partial Regex NonAlphanumericRegex();

    [GeneratedRegex(@"-{2,}")]
    private static partial Regex CollapseHyphensRegex();
}
