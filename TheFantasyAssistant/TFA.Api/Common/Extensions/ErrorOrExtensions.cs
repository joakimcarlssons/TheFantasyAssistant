using ErrorOr;
using TFA.Application.Common.Extensions;
using TFA.Application.Errors;
using TFA.Infrastructure.Services;

namespace TFA.Api.Common.Extensions;

public static class ErrorOrExtensions
{
    /// <summary>
    /// Produces a standardized data processing result with status code <see cref="StatusCodes.Status202Accepted"/> on success
    /// and <see cref="StatusCodes.Status422UnprocessableEntity"/> on failure.
    /// </summary>
    /// <typeparam name="TResponse">The type of the response value. Will not be handled in this result.</typeparam>
    /// <typeparam name="TModule">The module requesting the result. Needed for logger.</typeparam>
    public static IResult GetDataProcessingResult<TResponse, TModule>(this ErrorOr<TResponse> result, ILogger<TModule> logger)
        => result.Match(
            _ => Results.Accepted(),
            errors =>
            {
                if (errors.Any(error => error == Errors.Service.Skipped))
                {
                    // If the error is just that the job is skipped, do nothing.
                    return Results.Ok("Job skipped.");
                }

                // Todo: Implement emailing as part of error log
                //logger.LogError("The following errors was found when processing data: {Errors}", errors.ToErrorString());

                if (EmailService.Instance is { } email)
                {
                    email.SendErrorMessage(
                        "Errors when processing data",
                        $"The following errors was found when processing data: {errors.ToErrorString()}");
                }

                return Results.ValidationProblem(
                        statusCode: StatusCodes.Status422UnprocessableEntity,
                        errors: errors.ToErrorDictionary());
            });

    /// <summary>
    /// Sets up a response object of the errors from a request.
    /// </summary>
    public static IDictionary<string, string[]> ToErrorDictionary(this List<Error> errors)
    {
        Dictionary<string, string[]> dic = [];
        foreach (Error error in errors)
        {
            dic.Add(error.Code, [error.Description]);
        }

        return dic;
    }
}
