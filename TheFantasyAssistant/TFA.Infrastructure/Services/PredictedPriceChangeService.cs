using TFA.Application.Config;
using TFA.Application.Features.PredictedPriceChanges;
using TFA.Application.Http;
using TFA.Infrastructure.Dtos.Player;

namespace TFA.Infrastructure.Services;

public class PredictedPriceChangeService(
    HttpClient httpClient,
    IOptions<SourceOptions> sourceOptions,
    IMapper mapper) : IDataService<ErrorOr<PredictedPriceChangeData>>
{
    private readonly SourceOptions sources = sourceOptions.Value;

    public Task<ErrorOr<PredictedPriceChangeData>> GetData(FantasyType fantasyType, CancellationToken cancellationToken = default)
        => fantasyType switch
        {
            FantasyType.FPL => GetFPLPredictedPriceChanges(cancellationToken),
            _ => throw new NotImplementedException($"No data service configured for fantasy type {fantasyType}")
        };

    private async Task<ErrorOr<PredictedPriceChangeData>> GetFPLPredictedPriceChanges(CancellationToken cancellationToken)
    {
        ErrorOr<IReadOnlyList<HubPlayerRequest>> hubPlayers = await httpClient.TryGetAsJsonAsync<IReadOnlyList<HubPlayerRequest>>(sources.Hub.Players, cancellationToken);
        if (hubPlayers.IsError)
        {
            return hubPlayers.ErrorsOrEmptyList;
        }

        return new PredictedPriceChangeData(
            FantasyType.FPL,
            ExtractPriceChangingPlayers(hubPlayers.Value).Where(player => player.PriceTarget > 0).ToList(),
            ExtractPriceChangingPlayers(hubPlayers.Value).Where(player => player.PriceTarget < 0).ToList());
    }

    private IEnumerable<PredictedPriceChangePlayer> ExtractPriceChangingPlayers(IReadOnlyList<HubPlayerRequest> hubPlayers)
    {
        foreach (HubPlayerRequest player in hubPlayers.Where(player => player.Details.PriceInfo?.PredictedChangeDay == HubPredictedPriceChangeTimes.Tonight))
        {
            yield return mapper.Map<PredictedPriceChangePlayer>(player);
        }
    }
}
