namespace TFA.UnitTests.Builders;

internal class GameweekBuilder : AbstractFakerBuilder<Gameweek> 
{
    public GameweekBuilder(int gameweek)
    {
        _model.IsRecord()
            .RuleFor(gw => gw.Id, _ => gameweek)
            .RuleFor(gw => gw.IsFinished, _ => false)
            .RuleFor(gw => gw.IsCurrent, _ => false)
            .RuleFor(gw => gw.IsNext, _ => false)
            .RuleFor(gw => gw.Deadline, _ => DateTime.MinValue);
    }

    public GameweekBuilder WithIsFinished()
    {
        _model.RuleFor(gw => gw.IsFinished, _ => true);
        return this;
    }

    public GameweekBuilder WithIsCurrent()
    {
        _model.RuleFor(gw => gw.IsCurrent, _ => true);
        return this;
    }

    public GameweekBuilder WithIsNext()
    {
        _model.RuleFor(gw => gw.IsNext, _ => true);
        return this;
    }

    public GameweekBuilder WithDeadline(DateTime deadline)
    {
        _model.RuleFor(gw => gw.Deadline, _ => deadline);
        return this;
    }
}
