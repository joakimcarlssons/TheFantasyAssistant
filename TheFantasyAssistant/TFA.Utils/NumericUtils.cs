using System.Globalization;

namespace TFA.Utils;

public static class NumericUtils
{
    public static decimal ToDecimal(this string? value, decimal backup = decimal.Zero)
    {
        return decimal.TryParse(value?.Replace(',', '.') ?? "0", CultureInfo.InvariantCulture, out decimal res)
            ? res : backup;
    }

    public static bool Between(this int value, int minValue, int maxValue, bool includeLimit = false)
        => includeLimit
            ? value >= minValue && value <= maxValue
            : value > minValue && value < maxValue;
}
