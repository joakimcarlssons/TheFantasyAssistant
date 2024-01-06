namespace TFA.UnitTests.Builders;

internal class TeamBuilder : AbstractFakerBuilder<Team>
{
	public TeamBuilder()
	{
		_model.IsRecord();
	}

    public TeamBuilder(int id) : this()
    {
        _model.IsRecord()
            .RuleFor(t => t.Id, _ => id);
    }

	public TeamBuilder WithId(int id)
	{
        _model.RuleFor(t => t.Id, _ => id);
        return this;
	}

    public TeamBuilder WithName(string name)
    {
        _model.RuleFor(t => t.Name, _ => name);
        return this;
    }

    public TeamBuilder WithShortName(string shortName)
    {
        _model.RuleFor(t => t.ShortName, _ => shortName);
        return this;
    }

    public TeamBuilder WithMatchesPlayed(int matchesPlayed)
    {
        _model.RuleFor(t => t.MatchesPlayed, _ => matchesPlayed);
        return this;
    }

    public TeamBuilder WithPosition(int position)
    {
        _model.RuleFor(t => t.Position, _ => position);
        return this;
    }
}
