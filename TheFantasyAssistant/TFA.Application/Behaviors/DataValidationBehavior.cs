using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using ValidationResult = FluentValidation.Results.ValidationResult;

namespace TFA.Application.Behaviors;

public class DataValidationBehavior<TRequest, TResponse>(
        IServiceProvider serviceProvider) : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : IErrorOr
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // Try to get a validator, if none is setup then continue
        IValidator<TRequest>? validator = serviceProvider.GetService<IValidator<TRequest>>();
        if (validator is null)
        {
            return await next();
        }

        ValidationResult? validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (validationResult.IsValid)
        {
            return await next();
        }

        List<Error> errors = validationResult.Errors
            .ConvertAll(validationFailure => Error.Validation(
                validationFailure.PropertyName,
                validationFailure.ErrorMessage));

        // todo: fix
        // Can be found in error or library
        // or https://www.youtube.com/watch?v=FXP3PQ03fa0&list=PLzYkqgWkHPKBcDIP5gzLfASkQyTdy0t4k&index=8
        return (dynamic)errors;
    }
}
