using TFA.Application.Common.Data;

namespace TFA.UnitTests.Builders;

internal class FotmobPlayerDetailsBuilder : AbstractFakerBuilder<FotmobPlayerDetails>
{
    public FotmobPlayerDetailsBuilder(int playerId)
    {
        _model.IsRecord()
            .RuleFor(p => p.PlayerId, _ => playerId);
    }

    public FotmobPlayerDetails WithExpectedGoals(double expectedGoals)
    {
        _model.RuleFor(p => p.ExpectedGoals, _ => expectedGoals);
        return this;
    }
}
