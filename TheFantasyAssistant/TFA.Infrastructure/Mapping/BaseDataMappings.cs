using TFA.Application.Common.Transforms;
using TFA.Application.Features.BaseData.Transforms;
using TFA.Application.Features.LeagueData;
using TFA.Domain.Models.Fixtures;
using TFA.Domain.Models.Gameweeks;
using TFA.Domain.Models.Players;
using TFA.Domain.Models.Teams;
using TFA.Infrastructure.Dtos.Fixture;
using TFA.Infrastructure.Dtos.Gameweek;
using TFA.Infrastructure.Dtos.Player;
using TFA.Infrastructure.Dtos.Team;

namespace TFA.Infrastructure.Mapping;

public class BaseDataMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<FantasyPlayerRequest, Player>()
            .MapToConstructor(true)
            .Map(dest => dest.FullName, src => $"{ src.FirstName } { src.LastName }")
            .Map(dest => dest.Price, src => Convert.ToDecimal(src.Price) / 10)
            .Map(dest => dest.CleanSheetsPerMatch, src => src.CleanSheetsPerMatch == null ? 0 : src.CleanSheetsPerMatch);

        config.ForType<FantasyTeamRequest, Team>()
            .MapToConstructor(true);

        config.ForType<FantasyGameweekRequest, Gameweek>()
            .MapToConstructor(true);

        config.ForType<FantasyFixtureRequest, Fixture>()
            .MapToConstructor(true);

        config.ForType<(Player Player, Team Team, decimal PrevPrice), PlayerPriceChange>()
            .MapToConstructor(true)
            .Map(dest => dest.PlayerId, src => src.Player.Id)
            .Map(dest => dest.PreviousPrice, src => src.PrevPrice)
            .Map(dest => dest.CurrentPrice, src => src.Player.Price)
            .Map(dest => dest.TeamId, src => src.Team.Id)
            .Map(dest => dest.TeamShortName, src => src.Team.ShortName)
            .Map(dest => dest, src => src.Player);

        config.ForType<(Player Player, Team Team), PlayerStatusChange>()
            .MapToConstructor(true)
            .Map(dest => dest.PlayerId, src => src.Player.Id)
            .Map(dest => dest.TeamShortName, src => src.Team.ShortName)
            .Map(dest => dest, src => src.Player);

        config.ForType<(Player Player, Team Team), NewPlayer>()
            .MapToConstructor(true)
            .Map(dest => dest.PlayerId, src => src.Player.Id)
            .Map(dest => dest.TeamShortName, src => src.Team.ShortName)
            .Map(dest => dest.Position, src => src.Player.Position.TransformPlayerPosition())
            .Map(dest => dest, src => src.Player);

        config.ForType<(Player Player, Team NewTeam, Team PrevTeam), PlayerTransfer>()
            .MapToConstructor(true)
            .Map(dest => dest.PlayerId, src => src.Player.Id)
            .Map(dest => dest.PrevTeamId, src => src.PrevTeam.Id)
            .Map(dest => dest.PrevTeamShortName, src => src.PrevTeam.ShortName)
            .Map(dest => dest.NewTeamId, src => src.NewTeam.Id)
            .Map(dest => dest.NewTeamShortName, src => src.NewTeam.ShortName)
            .Map(dest => dest, src => src.Player);

        config.ForType<(Team FantasyTeam, LeagueTableTeam TableTeam), Team>()
            .MapToConstructor(true)
            .Map(src => src, dest => dest.TableTeam)
            .Map(src => src, dest => dest.FantasyTeam);
    }
}
