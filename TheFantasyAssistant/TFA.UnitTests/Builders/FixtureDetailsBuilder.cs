namespace TFA.UnitTests.Builders;

internal class FixtureDetailsBuilder : AbstractFakerBuilder<FixtureDetails>
{
    public FixtureDetailsBuilder(int fixtureId)
    {
        _model.IsRecord()
            .RuleFor(fd => fd.FixtureId, _ => fixtureId);
    }

    public FixtureDetailsBuilder WithGameweek(int gameweek)
    {
        _model.RuleFor(fd => fd.Gameweek, _ => gameweek);
        return this;
    }

    public FixtureDetailsBuilder WithKickOffTime(DateTime kickOffTime)
    {
        _model.RuleFor(fd => fd.KickOffTime, _ => kickOffTime);
        return this;
    }

    public FixtureDetailsBuilder WithHomeTeam(FixtureTeamDetails homeTeam)
    {
        _model.RuleFor(fd => fd.HomeTeam, _ => homeTeam);
        return this;
    }

    public FixtureDetailsBuilder WithAwayTeam(FixtureTeamDetails awayTeam)
    {
        _model.RuleFor(fd => fd.AwayTeam, _ => awayTeam);
        return this;
    }
}

internal class FixtureTeamDetailsBuilder : AbstractFakerBuilder<FixtureTeamDetails>
{
    public FixtureTeamDetailsBuilder(int teamId)
    {
        _model.IsRecord()
            .RuleFor(t => t.TeamId, _ => teamId);
    }

    public FixtureTeamDetailsBuilder WithLineUp(FixtureTeamDetailsLineUp lineUp)
    {
        _model.RuleFor(t => t.LineUp, _ => lineUp);
        return this;
    }
}

internal class FixtureTeamDetailsLineUpBuilder : AbstractFakerBuilder<FixtureTeamDetailsLineUp>
{
    public FixtureTeamDetailsLineUpBuilder()
    {
        _model.IsRecord();
    }

    public FixtureTeamDetailsLineUpBuilder WithStartingPlayers(IReadOnlyList<FixtureTeamPlayerDetails> startingPlayers)
    {
        _model.RuleFor(x => x.StartingPlayers, _ => startingPlayers);
        return this;
    }

    public FixtureTeamDetailsLineUpBuilder WithBenchPlayers(IReadOnlyList<FixtureTeamPlayerDetails> benchPlayers)
    {
        _model.RuleFor(x => x.BenchPlayers, _ => benchPlayers);
        return this;
    }
}

internal class FixtureTeamPlayerDetailsBuilder : AbstractFakerBuilder<FixtureTeamPlayerDetails>
{
    public FixtureTeamPlayerDetailsBuilder(int playerId)
    {
        _model.IsRecord()
            .RuleFor(p => p.PlayerId, _ => playerId);
    }
}
