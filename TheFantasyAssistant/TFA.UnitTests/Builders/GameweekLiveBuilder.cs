using TFA.Infrastructure.Dtos.Gameweek;

namespace TFA.UnitTests.Builders;

internal class GameweekLiveBuilder : AbstractFakerBuilder<FantasyGameweekLiveRequest>
{
    public GameweekLiveBuilder()
    {
        _model.IsRecord();
    }

    public GameweekLiveBuilder WithPlayers(IReadOnlyList<FantasyGameweekLivePlayerRequest> players)
    {
        _model.RuleFor(x => x.Players, _ => players);
        return this;
    }
}

internal class GameweekLivePlayerBuilder : AbstractFakerBuilder<FantasyGameweekLivePlayerRequest>
{
    public GameweekLivePlayerBuilder(int playerId)
    {
        _model.IsRecord()
            .RuleFor(p => p.PlayerId, _ => playerId);
    }

    public GameweekLivePlayerBuilder WithGameweekStats(FantasyGameweekLivePlayerStatsRequest stats)
    {
        _model.RuleFor(x => x.GameweekStats, _ => stats);
        return this;
    }

    public GameweekLivePlayerBuilder WithGameweekDetails(IReadOnlyList<FantasyGameweekLivePlayerDetailsRequest> gameweekDetails)
    {
        _model.RuleFor(x => x.GameweekDetails, _ => gameweekDetails);
        return this;
    }
}

internal class GameweekLivePlayerStatsBuilder : AbstractFakerBuilder<FantasyGameweekLivePlayerStatsRequest>
{
    public GameweekLivePlayerStatsBuilder()
    {
        _model.IsRecord();
    }

    public GameweekLivePlayerStatsBuilder WithPoints(int points)
    {
        _model.RuleFor(x => x.Points, _ => points);
        return this;
    }

    public GameweekLivePlayerStatsBuilder WithGoals(int goals)
    {
        _model.RuleFor(x => x.Goals, _ => goals);
        return this;
    }

    public GameweekLivePlayerStatsBuilder WithAssists(int assists)
    {
        _model.RuleFor(x => x.Assists, _ => assists);
        return this;
    }

    public GameweekLivePlayerStatsBuilder WithMinutesPlayed(int minutesPlayed)
    {
        _model.RuleFor(x => x.MinutesPlayed, _ => minutesPlayed);
        return this;
    }
}

internal class GameweekLivePlayerDetailsBuilder : AbstractFakerBuilder<FantasyGameweekLivePlayerDetailsRequest>
{
    public GameweekLivePlayerDetailsBuilder(int fixtureId)
    {
        _model.IsRecord()
            .RuleFor(x => x.FixtureId, _ => fixtureId);
    }
}
