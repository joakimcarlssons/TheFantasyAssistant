using TFA.Application.Services.BaseData;

namespace TFA.UnitTests.Builders;

internal class FantasyBaseDataBuilder
{
    private readonly List<Player> _players = new();
    private readonly List<Team> _teams = new();
    private readonly List<Gameweek> _gameweeks = new();
    private readonly List<Fixture> _fixtures = new();

    public FantasyBaseDataBuilder WithPlayers(params Player[] players)
    {
        _players.AddRange(players);
        return this;
    }

    public FantasyBaseDataBuilder WithTeams(params Team[] teams)
    {
        _teams.AddRange(teams);
        return this;
    }

    public FantasyBaseDataBuilder WithGameweeks(params Gameweek[] gameweeks)
    {
        _gameweeks.AddRange(gameweeks);
        return this;
    }

    public FantasyBaseDataBuilder WithFixtures(params Fixture[] fixtures)
    {
        _fixtures.AddRange(fixtures);
        return this;
    }

    public FantasyBaseData Build() => new(_players, _teams, _gameweeks, _fixtures);
}
