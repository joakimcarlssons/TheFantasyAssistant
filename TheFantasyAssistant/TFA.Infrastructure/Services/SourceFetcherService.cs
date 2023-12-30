using TFA.Application.Common.Keys;
using TFA.Application.Errors;
using TFA.Domain.Exceptions;

namespace TFA.Infrastructure.Services;

public interface ISourceFetcherService
{
    Task<ErrorOr<TData>> GetSourceData<TData>(string key, CancellationToken cancellationToken);
}

public class SourceFetcherService(IServiceScopeFactory scopeFactory) : ISourceFetcherService
{
    public Task<ErrorOr<TData>> GetSourceData<TData>(string key, CancellationToken cancellationToken)
    {
        using IServiceScope serviceScope = scopeFactory.CreateScope();

        return key switch
        {
            DataKeys.FPL_LATEST_CHECKED_DEADLINE
            or DataKeys.FPL_LATEST_CHECKED_FINISHED_GAMEWEEK
            or DataKeys.FAS_LATEST_CHECKED_DEADLINE
            or DataKeys.FAS_LATEST_CHECKED_FINISHED_GAMEWEEK
                => Task.FromResult(GetSingleNumericSourceValue<TData>()),

            _ => serviceScope.ServiceProvider.GetService<IDataService<ErrorOr<TData>>>() is { } service
                ? service.GetData(DataKeysHandler.GetFantasyType(key), cancellationToken)
                : throw new SourceFetcherNotFoundException<TData>()
        };
    }

    private static ErrorOr<TData> GetSingleNumericSourceValue<TData>() =>
        typeof(TData) == typeof(int)
            ? (TData)(object)1
            : Errors.Service.InvalidData;
}
