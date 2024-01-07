namespace TFA.Application.Errors;

public sealed class ErrorTypes
{
    public const int Skipped = 50;
}

public static class ErrorCodes
{
    public static string Skipped(string source) => string.Concat(source, ".Skipped");
    public static string InvalidData(string source) => string.Concat(source, ".InvalidData");
    public static string Fetching(string source) => string.Concat(source, ".Fetching");

    public const string BotCommandNotImplemented = "BotCommand.NotImplemented";
}
