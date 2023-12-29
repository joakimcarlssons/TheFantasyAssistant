using TFA.Application.Features.LeagueData;
using TFA.Domain.Models.Teams;
using TFA.Infrastructure.Dtos.Fotmob;

namespace TFA.Infrastructure.Mapping;

public class LeagueMappings : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.ForType<(FotmobLeagueTableTeam FotmobTeam, Team FantasyTeam), LeagueTableTeam>()
            .MapToConstructor(true)
            .Map(src => src.TeamId, dest => dest.FantasyTeam.Id)
            .Map(src => src.GoalsScored, dest => ParseGoalsScored(dest.FotmobTeam.GoalDifferenceStr))
            .Map(src => src.GoalsConceded, dest => ParseGoalsConceded(dest.FotmobTeam.GoalDifferenceStr))
            .Map(src => src, dest => dest.FotmobTeam);
    }

    private static int ParseGoalsScored(string goalDifference)
    {
        if (goalDifference.Split("-") is { Length: 2 } parts
            && int.TryParse(parts[0], out int goalsScored))
        {
            return goalsScored;
        }

        return 0;
    }

    private static int ParseGoalsConceded(string goalDifference)
    {
        if (goalDifference.Split("-") is { Length: 2 } parts
            && int.TryParse(parts[1], out int goalsConceded))
        {
            return goalsConceded;
        }

        return 0;
    }
}
