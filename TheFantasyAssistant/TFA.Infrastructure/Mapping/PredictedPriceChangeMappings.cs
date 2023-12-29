using Mapster;
using TFA.Application.Features.PredictedPriceChanges;
using TFA.Infrastructure.Dtos.Player;

namespace TFA.Infrastructure.Mapping;

public class PredictedPriceChangeMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<HubPlayerRequest, PredictedPriceChangePlayer>()
            .MapToConstructor(true)
            .Map(dest => dest.Player, src => src.PlayerData)
            .Map(dest => dest.TeamName, src => src.Team.TeamName)
            .Map(dest => dest.TeamShortName, src => src.Team.TeamShortName)
            .Map(dest => dest.PriceTarget, src => src.Details.PriceInfo != null ? src.Details.PriceInfo.PriceTarget : 0);
    }
}
