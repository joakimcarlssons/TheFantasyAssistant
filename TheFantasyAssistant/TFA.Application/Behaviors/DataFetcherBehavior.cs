using TFA.Application.Common.Commands;
using TFA.Application.Interfaces.Services;

namespace TFA.Application.Behaviors;

internal class DataFetcherBehavior<TRequest, TResponse>(
        IDataService<TResponse> dataService) : IPipelineBehavior<TRequest, TResponse>
    where TResponse : IErrorOr
    where TRequest : AbstractDataCommand<TResponse>
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        request.Data = await dataService.GetData(request.FantasyType, cancellationToken);
        return await next();
    }
}