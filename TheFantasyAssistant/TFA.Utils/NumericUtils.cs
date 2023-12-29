namespace TFA.Utils;

public static class NumericUtils
{
    public static bool Between(this int value, int minValue, int maxValue, bool includeLimit = false)
        => includeLimit
            ? value >= minValue && value <= maxValue
            : value > minValue && value < maxValue;
}
