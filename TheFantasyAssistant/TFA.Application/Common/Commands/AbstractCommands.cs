namespace TFA.Application.Common.Commands;

public abstract class AbstractDataCommand<TResponse>(FantasyType fantasyType) : IRequest<TResponse>
    where TResponse : IErrorOr
{
    public FantasyType FantasyType { get; init; } = fantasyType;
    public TResponse? Data { get; set; }
}
