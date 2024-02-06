using TFA.Domain.Models;

namespace TFA.Application.Features.Transforms;

public static class FixtureTransforms
{
    public static int GetFixtureDifficulty(this Fixture fixture, bool isHome)
        => isHome ? fixture.HomeTeamDifficulty : fixture.AwayTeamDifficulty;

    public static string GetOpponentShortName(KeyedBaseData fantasyData, bool isHome, int homeTeamId, int awayTeamId)
        => isHome ? fantasyData.TeamsById[awayTeamId].ShortName : fantasyData.TeamsById[homeTeamId].ShortName;
}
