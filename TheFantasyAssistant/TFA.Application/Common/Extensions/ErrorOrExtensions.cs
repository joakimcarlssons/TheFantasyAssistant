namespace TFA.Application.Common.Extensions;

public static class ErrorOrExtensions
{
    /// <summary>
    /// Converts all errors into a string to be read in loggers etc.
    /// </summary>
    public static string ToErrorString(this List<Error> errors)
        => string.Join("\n", errors.Select(error => $"{error.Code}: {error.Description}"));
}
