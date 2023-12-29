namespace TFA.UnitTests.Builders;

internal class PlayerBuilder : AbstractFakerBuilder<Player>
{
    public PlayerBuilder()
    {
        _model.IsRecord()
            .RuleFor(p => p.FirstName, f => f.Name.FirstName())
            .RuleFor(p => p.LastName, f => f.Name.LastName());
    }

    public PlayerBuilder WithId(int id)
    {
        _model.RuleFor(p => p.Id, _ => id);
        return this;
    }

    public PlayerBuilder WithPrice(decimal price)
    {
        _model.RuleFor(p => p.Price, _ => price);
        return this;
    }

    public PlayerBuilder WithTeamId(int teamId)
    {
        _model.RuleFor(p => p.TeamId, _ => teamId);
        return this;
    }

    public PlayerBuilder WithPosition(PlayerPosition position)
    {
        _model.RuleFor(p => p.Position, _ => position);
        return this;
    }

    public PlayerBuilder WithChanceOfPlayingNextRound(int chanceOfPlayingNextRound)
    {
        _model.RuleFor(p => p.ChanceOfPlayingNextRound, _ => chanceOfPlayingNextRound);
        return this;
    }

    public PlayerBuilder WithNews(string news)
    {
        _model.RuleFor(p => p.News, _ => news);
        return this;
    }

    public PlayerBuilder WithYellowCards(int yellowCards)
    {
        _model.RuleFor(p => p.YellowCards, _ => yellowCards);
        return this;
    }

    public PlayerBuilder WithRedCards(int redCards)
    {
        _model.RuleFor(p => p.RedCards, _ => redCards);
        return this;
    }
}
