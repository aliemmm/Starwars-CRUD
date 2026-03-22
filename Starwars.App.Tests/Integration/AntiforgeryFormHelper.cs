using System.Text.RegularExpressions;

namespace Starwars.App.Tests.Integration;

internal static class AntiforgeryFormHelper
{
    /// <summary>Extracts the antiforgery token from an HTML page that contains a form.</summary>
    public static string ExtractRequestVerificationToken(string html)
    {
        var patterns = new[]
        {
            "name=\"__RequestVerificationToken\"[^>]*value=\"([^\"]+)\"",
            "value=\"([^\"]+)\"[^>]*name=\"__RequestVerificationToken\"",
            "name='__RequestVerificationToken'[^>]*value='([^']+)'",
            "value='([^']+)'[^>]*name='__RequestVerificationToken'"
        };

        foreach (var pattern in patterns)
        {
            var m = Regex.Match(html, pattern, RegexOptions.IgnoreCase);
            if (m.Success)
                return m.Groups[1].Value;
        }

        throw new InvalidOperationException("Could not find __RequestVerificationToken in HTML.");
    }
}
