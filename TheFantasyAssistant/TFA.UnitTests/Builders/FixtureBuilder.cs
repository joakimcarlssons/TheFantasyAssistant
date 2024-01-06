namespace TFA.UnitTests.Builders;

internal class FixtureBuilder : AbstractFakerBuilder<Fixture>
{
    public FixtureBuilder(int id)
    {
        _model.IsRecord()
            .RuleFor(f => f.Id, _ => id);
    }

    public FixtureBuilder WithGameweek(int gameweek)
    {
        _model.RuleFor(f => f.GameweekId, _ => gameweek);
        return this;
    }

    public FixtureBuilder WithHomeTeam(int homeTeamId)
    {
        _model.RuleFor(f => f.HomeTeamId, _ => homeTeamId);
        return this;
    }

    public FixtureBuilder WithAwayTeam(int awayTeamId)
    {
        _model.RuleFor(f => f.AwayTeamId, _ => awayTeamId);
        return this;
    }

    public FixtureBuilder WithHomeTeamDifficulty(int difficulty)
    {
        _model.RuleFor(f => f.HomeTeamDifficulty, _ => difficulty);
        return this;
    }

    public FixtureBuilder WithAwayTeamDifficulty(int difficulty)
    {
        _model.RuleFor(f => f.AwayTeamDifficulty, _ => difficulty);
        return this;
    }
}
