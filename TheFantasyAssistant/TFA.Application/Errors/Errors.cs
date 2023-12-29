namespace TFA.Application.Errors;

public static partial class Errors
{
    public static class Service
    {
        public static Error Skipped = Error.Custom(
            ErrorTypes.Skipped, 
            ErrorCodes.Skipped(nameof(Service)), 
            "Service was skipped.");

        public static Error InvalidData = Error.Validation(
            ErrorCodes.InvalidData(nameof(Service)),
            "Service was provided with invalid data.");

        public static Error Fetching = Error.NotFound(
            ErrorCodes.Fetching(nameof(Service)),
            "Service was unable to fetch data.");
    }
}
