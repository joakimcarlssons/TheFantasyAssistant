namespace TFA.Application.Interfaces.Services;

public interface IDataService<T> where T : IErrorOr
{
    Task<T> GetData(FantasyType fantasyType, CancellationToken cancellationToken = default);
}
