using TFA.Domain.Models;

namespace TFA.Application.Features.Transforms;

public static class FixtureTransforms
{
    public static int GetFixtureDifficulty(this Fixture fixture, bool isHome)
        => isHome ? fixture.HomeTeamDifficulty : fixture.AwayTeamDifficulty;

    public static string GetOpponentShortName(KeyedBaseData fantasyData, bool isHome, int homeTeamId, int awayTeamId)
        => isHome ? fantasyData.TeamsById[awayTeamId].ShortName : fantasyData.TeamsById[homeTeamId].ShortName;

    public static (int HomeTeamDifficulty, int AwayTeamDifficulty) CalculateFixtureDifficulty(Team homeTeam, Team awayTeam, Fixture fixture)
    {
        int homeTeamIndex = GetTeamBaseIndex(homeTeam.Name);
        int awayTeamIndex = GetTeamBaseIndex(awayTeam.Name);

        // Set intial difficulty based on index
        int homeTeamDifficulty = GetFixtureDifficultyBasedOnIndex(homeTeamIndex, awayTeamIndex, true);
        int awayTeamDifficulty = GetFixtureDifficultyBasedOnIndex(awayTeamIndex, homeTeamIndex, false);

        // After 10 gameweeks, start checking table placings etc.
        if (fixture.GameweekId > 10)
        {
            // Opponent is top 5 => 3 points
            if (homeTeam.Position <= 5) awayTeamDifficulty += 3;
            if (awayTeam.Position <= 5) homeTeamDifficulty += 3;

            // Team with better placing => 1 point
            // Team with worse placing => 2 points
            if (awayTeam.Position > homeTeam.Position)
            {
                awayTeamDifficulty += 1;

                // If placing is > 4 places => 3 points
                if ((awayTeam.Position - homeTeam.Position) > 4)
                {
                    awayTeamDifficulty += 3;
                }
                else
                {
                    awayTeamDifficulty += 2;
                }

                // If placing is > 6 places => Team leading -1 point
                if ((awayTeam.Position - homeTeam.Position) > 6)
                {
                    awayTeamDifficulty -= 1;
                }
            }
            else
            {
                homeTeamDifficulty += 1;

                // If placing is > 4 places => 3 points
                if ((homeTeam.Position - awayTeam.Position) > 4)
                {
                    homeTeamDifficulty += 3;
                }

                // If placing is > 6 places => Team leading -1 point
                if ((homeTeam.Position - awayTeam.Position) > 6)
                {
                    homeTeamDifficulty -= 1;
                }
            }
        }

        // Min points are 2, Max points are 5
        if (homeTeamDifficulty < 2) homeTeamDifficulty = 2;
        if (awayTeamDifficulty < 2) awayTeamDifficulty = 2;

        // Max points are 5
        if (homeTeamDifficulty > 5) homeTeamDifficulty = 5;
        if (awayTeamDifficulty > 5) awayTeamDifficulty = 5;

        return (homeTeamDifficulty, awayTeamDifficulty);
    }

    private static int GetTeamBaseIndex(string teamName)
        => teamName switch
        {
            TeamNames.HACKEN => 4,
            TeamNames.DJURGARDEN => 5,
            TeamNames.MALMO => 5,
            TeamNames.ELFSBORG => 4,
            TeamNames.HAMMARBY => 3,
            TeamNames.KALMAR => 3,
            TeamNames.AIK => 3,
            TeamNames.GOTEBORG => 2,
            TeamNames.MJALLBY => 2,
            TeamNames.NORRKOPING => 2,
            TeamNames.BROMMAPOJKARNA => 2,
            TeamNames.VARNAMO => 1,
            TeamNames.SIRIUS => 1,
            TeamNames.GAIS => 1,
            TeamNames.VASTERAS => 1,
            TeamNames.HALMSTAD => 2,
            _ => 1
        };

    private static int GetFixtureDifficultyBasedOnIndex(int teamIndex, int opponentIndex, bool isHome)
    {
        // Base index will be the index of the opponent
        int fixtureDifficulty = opponentIndex;

        // If the team is playing home, remove 1 point
        if (isHome)
        {
            fixtureDifficulty -= 1;
        }
        else
        {
            fixtureDifficulty += 1;
        }

        // If the index is larger than one between the teams, add the differing points
        if (teamIndex < opponentIndex && (opponentIndex - teamIndex > 1))
        {
            fixtureDifficulty += (opponentIndex - teamIndex);
        }

        return fixtureDifficulty;
    }
}
