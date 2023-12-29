namespace TFA.Infrastructure;

public static partial class Logging
{
    [LoggerMessage(LogLevel.Error, "Failed to get {ExpectedData} from {FromService}. Error: {Error}")]
    public static partial void LogDataFetchingError(this ILogger logger, Type ExpectedData, Type FromService, string Error);

    [LoggerMessage(LogLevel.Error, "Could not proceed with service {FromService} becuase data {NotFoundData} was invalid.")]
    public static partial void LogCouldNotProceedBecauseOfInvalidData(this ILogger logger, Type FromService, Type NotFoundData);

    [LoggerMessage(LogLevel.Information, "Skipped job {SkippedJob} with reason: {Reason}")]
    public static partial void LogJobSkipped(this ILogger logger, Type SkippedJob, string Reason);
}
