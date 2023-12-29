using FluentValidation;

namespace TFA.Application.Features.LeagueData.Commands;

public sealed class LeagueTableCommandValidator : AbstractValidator<LeagueTableData>
{
    public LeagueTableCommandValidator()
    {
        RuleFor(x => x).Custom((data, context) =>
        {
            if (!ValidateTeamCount(data.FantasyType, data.Teams.Count))
            {
                context.AddFailure("Incorrect amount of teams.");
            }
        });
        RuleForEach(x => x.Teams).Custom((team, context) =>
        {
            if (!(team.GoalsScored - team.GoalsConceded == team.GoalDifference))
            {
                context.AddFailure("Goal difference is not calculated correctly.");
            }
        });
    }

    private static bool ValidateTeamCount(FantasyType fantasyType, int teamCount)
        => fantasyType switch
        {
            FantasyType.FPL => teamCount == ExpectedLeagueTeamCount.PremierLeague,
            FantasyType.Allsvenskan => teamCount == ExpectedLeagueTeamCount.Allsvenskan,
            _ => true
        };
}
