using TFA.Application.Common.Data;
using TFA.Application.Features.Deadline;
using TFA.Domain.Exceptions;
using TFA.Domain.Models.Players;
using TFA.Domain.Models.Teams;

namespace TFA.Infrastructure.Mapping;

public class DeadlineSummaryMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<(Player Player, FotmobPlayerDetails PlayerDetails, Team Team), DeadlineSummaryPlayerToTarget>()
            .MapToConstructor(true)
            .Map(dest => dest.PlayerId, src => src.Player.Id)
            .Map(dest => dest.TeamId, src => src.Team.Id)
            .Map(dest => dest.TeamShortName, src => src.Team.ShortName)
            .Map(dest => dest.ExpectedPoints, src => src.Player.ExpectedPointsNextGameweek)
            .Map(dest => dest, src => src.PlayerDetails)
            .Map(dest => dest, src => src.Player);

        config.ForType<(Player Player, Team Team), DeadlineSummaryPlayerRiskingSuspension>()
            .MapToConstructor(true)
            .Map(dest => dest.PlayerId, src => src.Player.Id)
            .Map(dest => dest.TeamId, src => src.Team.Id)
            .Map(dest => dest.DisplayName, src => src.Player.DisplayName)
            .Map(dest => dest.TeamName, src => src.Team.Name)
            .Map(dest => dest.TeamShortName, src => src.Team.ShortName);
    }
}

internal static class CustomDeadlineSummaryMapper
{
    internal static IReadOnlyList<DeadlineSummaryPlayerToTarget> ToDeadlineSummaryPlayerToTarget(
        this IEnumerable<Player> players,
        IReadOnlyDictionary<int, FotmobPlayerDetails> playerDetailsById,
        IReadOnlyDictionary<int, Team> teamsById,
        FantasyType fantasyType,
        int numberOfPlayers)
    {
        IReadOnlyList<Player> playersToTarget = players
            .Where(player =>
                player.Position != PlayerPosition.Goalkeeper
                && player.ChanceOfPlayingNextRound == 100
                && player.Status == PlayerStatuses.Available
                && playerDetailsById.ContainsKey(player.Id)
                && teamsById.ContainsKey(player.TeamId))
            .OrderByDescending(player => player.ExpectedPointsNextGameweek.ToDecimal())
            .ThenByDescending(player => player.Form)
            .ThenByDescending(player => fantasyType switch
            {
                FantasyType.FPL => player.Bps,
                FantasyType.Allsvenskan => (player.AttackingBonus + player.DefendingBonus),
                _ => throw new FantasyTypeNotSupportedException()
            })
            .ThenByDescending(player => player.SelectedByPercent.ToDecimal())
            .Take(numberOfPlayers)
            .ToList();

        return playersToTarget.Select(player =>
                (player, playerDetailsById[player.Id], teamsById[player.TeamId]).Adapt<DeadlineSummaryPlayerToTarget>())
                .ToList();
    }
}