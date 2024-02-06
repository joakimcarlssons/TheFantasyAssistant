using TFA.Domain.Models;

namespace TFA.Application.Interfaces.Services;

public interface IBaseDataService : IDataService<ErrorOr<FantasyBaseData>>
{
    Task<ErrorOr<KeyedBaseData>> GetKeyedData(FantasyType fantasyType, CancellationToken cancellationToken = default);
}