using Bogus;

namespace TFA.UnitTests.Builders;

internal abstract class AbstractFakerBuilder<TModel>
    where TModel : class
{
    protected readonly Faker<TModel> _model;

    public AbstractFakerBuilder()
    {
        Randomizer.Seed = new Random(100);
        _model = new Faker<TModel>()
            .RuleForType(typeof(string), f => f.Random.AlphaNumeric(5))
            .RuleForType(typeof(decimal), f => 0M)
            .RuleForType(typeof(int), f => 0)
            .RuleForType(typeof(int?), f => (int?)0)
            .RuleForType(typeof(DateTime), f => f.Date.Recent());
    }

    public TModel Build() => _model.Generate();
    public static implicit operator TModel(AbstractFakerBuilder<TModel> builder) => builder.Build();
}
