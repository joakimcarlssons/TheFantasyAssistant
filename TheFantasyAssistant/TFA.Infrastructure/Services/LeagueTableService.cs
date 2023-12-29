using TFA.Application.Common.Extensions;
using TFA.Application.Errors;
using TFA.Application.Features.LeagueData;
using TFA.Domain.Models.Teams;
using TFA.Infrastructure.Dtos.Fotmob;
using TFA.Infrastructure.Mapping;

namespace TFA.Infrastructure.Services;

public class LeagueTableService(
    ILogger<LeagueTableService> logger,
    IFotmobService fotmob,
    IBaseDataService baseData) : IDataService<ErrorOr<LeagueTableData>>
{
    public async Task<ErrorOr<LeagueTableData>> GetData(FantasyType fantasyType, CancellationToken cancellationToken = default)
    {
        IReadOnlyList<FotmobLeagueTableTeam> tableTeams = await fotmob.GetFotmobLeagueTable(fantasyType, cancellationToken);
        if (tableTeams.Count == 0)
        {
            logger.LogDataFetchingError(typeof(FotmobLeagueTableTeam), typeof(LeagueTableService), $"No teams were found in table for fantasy type {fantasyType}.");
            return Errors.Service.Fetching;
        }

        ErrorOr<KeyedBaseData> fantasyData = await baseData.GetKeyedData(fantasyType, cancellationToken);
        if (fantasyData.IsError)
        {
            logger.LogDataFetchingError(typeof(KeyedBaseData), typeof(LeagueTableService), fantasyData.Errors.ToErrorString());
            return Errors.Service.Fetching;
        }

        // Map all teams possible. If not all teams can be mapped it will be caught in the validator.
        IReadOnlyList<LeagueTableTeam> teams = tableTeams
            .Where(team => fantasyData.Value.TeamsByName.ContainsKey(team.FotmobTeamName.ToCommonTeamName()))
            .Select(team =>
            {
                Team fantasyTeam = fantasyData.Value.TeamsByName[team.FotmobTeamName.ToCommonTeamName()];
                return (team, fantasyTeam).Adapt<LeagueTableTeam>();
            })
            .ToList();

        return new LeagueTableData(fantasyType, teams);
    }
}
