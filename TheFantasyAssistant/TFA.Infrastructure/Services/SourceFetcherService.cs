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
            
            DataKeys.FPL_FINISHED_GAMEWEEK_FIXTURES
            or DataKeys.FAS_FINISHED_GAMEWEEK_FIXTURES
                => Task.FromResult(GetEmptyReadOnlyEnumerable<TData>()),

            _ => serviceScope.ServiceProvider.GetService<IDataService<ErrorOr<TData>>>() is { } service
                ? service.GetData(DataKeysHandler.GetFantasyType(key), cancellationToken)
                : throw new SourceFetcherNotFoundException<TData>()
        };
    }

    private static ErrorOr<TData> GetEmptyReadOnlyEnumerable<TData>()
    {
        Type elementType = (typeof(TData).GetInterfaces().FirstOrDefault(x =>
            x.IsGenericType 
            && x.GetGenericTypeDefinition() == typeof(IEnumerable<>))?.GetGenericArguments()[0]) ?? throw new InvalidOperationException("The provided type does not implement IEnumerable<T>.");

        object? emptyList = typeof(Enumerable)
            .GetMethod("Empty")?
            .MakeGenericMethod(elementType)
            .Invoke(null, null);

        return (TData)typeof(Enumerable)?
            .GetMethod("ToList")?
            .MakeGenericMethod(elementType)
            .Invoke(null, [emptyList])!;
    }

    private static ErrorOr<TData> GetSingleNumericSourceValue<TData>() =>
        typeof(TData) == typeof(int)
            ? (TData)(object)1
            : Errors.Service.InvalidData;
}
