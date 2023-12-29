namespace TFA.Twitter;

internal static class TweetValidator
{
    private const int TweetLength = 280;

    internal static bool IsValidLength(this string content)
    {
        // Twitter is counting emojis as two characters.
        // To tackle this we count on one emoji per new line.
        int numberOfLines = content.Split("\n").Length - 1;
        return (content.Length + numberOfLines) <= TweetLength;
    }
}
