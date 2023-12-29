namespace TFA.UnitTests.Builders;

internal class TeamBuilder : AbstractFakerBuilder<Team>
{
	public TeamBuilder()
	{
		_model.IsRecord();
	}

	public TeamBuilder WithId(int id)
	{
        _model.RuleFor(p => p.Id, _ => id);
        return this;
	}

    public TeamBuilder WithName(string name)
    {
        _model.RuleFor(p => p.Name, _ => name);
        return this;
    }

    public TeamBuilder WithShortName(string shortName)
    {
        _model.RuleFor(p => p.ShortName, _ => shortName);
        return this;
    }
}
