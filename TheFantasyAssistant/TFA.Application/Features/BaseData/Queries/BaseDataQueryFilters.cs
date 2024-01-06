namespace TFA.Application.Features.BaseData.Queries;

public static class BaseDataQueryFilters
{
    /// <summary>
    /// Apply given filters to a collection of players.
    /// </summary>
    public static IEnumerable<Player> Filter(
        this IEnumerable<Player> players, PlayerFilter filter)
    => players
        .Where(p =>
            (p.DisplayName?.Contains(filter.Name, StringComparison.InvariantCultureIgnoreCase) ?? true)
            || (p.FullName?.Contains(filter.Name, StringComparison.InvariantCultureIgnoreCase) ?? true))
        .Skip((filter.Page - 1) * filter.PageSize)
        .Take(filter.PageSize);

    /// <summary>
    /// Apply given filters to a collection of teams.
    /// </summary>
    public static IEnumerable<Team> Filter(
        this IEnumerable<Team> teams, TeamFilter filter)
    => teams
        .Where(t =>
            (t.Name?.Contains(filter.Name, StringComparison.InvariantCultureIgnoreCase) ?? true))
        .Skip((filter.Page - 1) * filter.PageSize)
        .Take(filter.PageSize);

    /// <summary>
    /// Apply given filters to a collection of gameweeks.
    /// </summary>
    public static IEnumerable<Gameweek> Filter(
        this IEnumerable<Gameweek> gameweeks, GameweekFilter filter)
    => gameweeks
        .Where(gw => true)
        .Skip((filter.Page - 1) * filter.PageSize)
        .Take(filter.PageSize);

    /// <summary>
    /// Apply given filters to a collection of fixtures.
    /// </summary>
    public static IEnumerable<Fixture> Filter(
        this IEnumerable<Fixture> fixtures, FixtureFilter filter)
    => fixtures
        .Where(f => true)
        .Skip((filter.Page - 1) * filter.PageSize)
        .Take(filter.PageSize);
}

public record struct PlayerFilter(int Page, int PageSize, string Name);
public record struct TeamFilter(int Page, int PageSize, string Name);
public record struct GameweekFilter(int Page, int PageSize);
public record struct FixtureFilter(int Page, int PageSize);