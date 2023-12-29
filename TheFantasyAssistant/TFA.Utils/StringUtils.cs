using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace TFA.Utils;

public static partial class StringUtils
{
    public static string NullIsEmpty(this string? s)
            => s is null ? string.Empty : s;

    public static string NormalizeDiacritics(this string input)
    {
        return DiacriticsRegex()
            .Replace(input, m =>
            {
                string normalized = m.Value.Normalize(NormalizationForm.FormD);
                StringBuilder sb = new();

                foreach (char c in normalized)
                {
                    if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                    {
                        sb.Append(c);
                    }
                }

                return sb.ToString().Normalize(NormalizationForm.FormC);
            })
            .Replace('ø', 'o');
    }

    [GeneratedRegex("[^\x20-\x7E]", RegexOptions.IgnoreCase, "sv-SE")]
    private static partial Regex DiacriticsRegex();
}
