using TFA.Application.Common.Data;
using TFA.Application.Config;
using TFA.Domain.Exceptions;
using TFA.Domain.Models;
using TFA.Domain.Models.Fixtures;
using TFA.Domain.Models.Gameweeks;
using TFA.Domain.Models.Teams;
using TFA.Infrastructure.Dtos.Fotmob;
using TFA.Infrastructure.Mapping;

namespace TFA.Infrastructure.Services;

public interface IFotmobService
{
    Task<IReadOnlyList<FotmobPlayerDetails>> GetFotmobPlayerDetails(KeyedBaseData fantasyData, FantasyType fantasyType, CancellationToken cancellationToken);
    Task<IReadOnlyList<FixtureDetails>> GetFixtureDetailsForGameweek(KeyedBaseData fantasyData, Gameweek gameweek, FantasyType fantasyType, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<FotmobLeagueTableTeam>> GetFotmobLeagueTable(FantasyType fantasyType, CancellationToken cancellationToken = default);
}

public sealed class FotmobService(
    HttpClient httpClient,
    ILogger<FotmobService> logger,
    IOptions<SourceOptions> sourceOptions) : IFotmobService
{
    private readonly FotmobOptions Options = sourceOptions.Value.Fotmob;

    public async Task<IReadOnlyList<FotmobPlayerDetails>> GetFotmobPlayerDetails(KeyedBaseData fantasyData, FantasyType fantasyType, CancellationToken cancellationToken)
    {
        ILookup<(string PlayerName, string TeamName), KeyValuePair<string, FotmobStat>> playerStatsByName = (await Task.WhenAll(
            Options.PlayerData
                .GetType()
                .GetProperties()
                .Where(prop => prop.Name != nameof(Options.PlayerData.BaseUrl))
                .Select(prop => GetFotmobPlayerStat(fantasyType, prop.Name, cancellationToken))))
            .Where(stats => stats is not null)
            .SelectMany(stats => stats!)
            .Where(stat => !string.IsNullOrWhiteSpace(stat.StatName) && !string.IsNullOrWhiteSpace(stat.TeamName))
            .ToLookup(stat => (stat.EntityName, stat.TeamName!), stat => new KeyValuePair<string, FotmobStat>(stat.StatName!, stat));

        return playerStatsByName.ToPlayerDetails(
                fantasyData.PlayersById.Values.ToList(),
                fantasyData.TeamsByName);
    }

    public async Task<IReadOnlyList<FixtureDetails>> GetFixtureDetailsForGameweek(
        KeyedBaseData fantasyData,
        Gameweek gameweek,
        FantasyType fantasyType, 
        CancellationToken cancellationToken = default)
    {
        // Make sure we have a gameweek to check
        if (await GetFotmobLeagueData(fantasyType, cancellationToken) is { FixtureWrapper.Fixtures.Count: > 0 } leagueData)
        {
            DateTime nextGameweekDeadline = fantasyData.GameweeksById.TryGetValue(gameweek.Id + 1, out Gameweek? nextGameweek)
                ? nextGameweek.Deadline
                : gameweek.Deadline;

            IReadOnlyList<FotmobFixture> fixturesToCheck = leagueData.FixtureWrapper.Fixtures
                .Where(fixture => 
                    fixture.FixtureStatus.KickOffTime > gameweek.Deadline
                    && fixture.FixtureStatus.KickOffTime < nextGameweekDeadline)
                .ToList();

            List<FixtureDetails> fixtureDetails = [];
            foreach (FotmobFixture fotmobFixture in fixturesToCheck)
            {
                if (string.IsNullOrWhiteSpace(fotmobFixture.FotmobFixtureId))
                    continue;

                if (await GetFotmobFixtureDetails(fotmobFixture.FotmobFixtureId, cancellationToken) is { } fotmobFixtureDetails)
                {
                    if (fantasyData.TeamsByName.TryGetValue(fotmobFixture.HomeTeam.TeamName.ToCommonTeamName(), out Team? homeTeam)
                        && fantasyData.TeamsByName.TryGetValue(fotmobFixture.AwayTeam.TeamName.ToCommonTeamName(), out Team? awayTeam)
                        && GetFixture([.. fantasyData.FixturesByHomeTeamId[homeTeam.Id]], awayTeam.Id) is { } fixture)
                    {
                        fixtureDetails.Add(fotmobFixtureDetails.ToFixtureDetails(
                            fixture,
                            homeTeam,
                            awayTeam,
                            [.. fantasyData.PlayersByTeamId[homeTeam.Id]],
                            [.. fantasyData.PlayersByTeamId[awayTeam.Id]]));
                    }
                    else
                    {
                        logger.LogWarning(
                            "Failed to map Fotmob fixture details. Team name for either {HomeTeam} or {AwayTeam} could not be mapped",
                            fotmobFixture.HomeTeam.TeamName,
                            fotmobFixture.AwayTeam.TeamName);
                        continue;
                    }
                }
            }

            return fixtureDetails;
        }

        return [];
    }

    public async Task<IReadOnlyList<FotmobLeagueTableTeam>> GetFotmobLeagueTable(FantasyType fantasyType, CancellationToken cancellationToken = default)
    {
        return await GetFotmobLeagueData(fantasyType, cancellationToken) is { Tables.Length: 1 } leagueData
            ? leagueData.Tables[0].Data.Table.Teams
            : [];
    }

    private static Fixture? GetFixture(IReadOnlyList<Fixture> homeTeamFixtures, int awayTeamId)
        => homeTeamFixtures.FirstOrDefault(fixture => fixture.AwayTeamId == awayTeamId);

    private async Task<IReadOnlyList<FotmobStat>?> GetFotmobPlayerStat(FantasyType fantasyType, string statPropertyName, CancellationToken cancellationToken)
    {
        string statTypeSlug = RequireStatTypeSlug(statPropertyName);
        return (await httpClient.GetFromJsonAsync<FotmobTopListRoot>(ConstructStatUrl(fantasyType, statTypeSlug), cancellationToken))?
            .TopLists.FirstOrDefault()?.Stats
            .Select(stat => stat with { StatName = statPropertyName })
            .ToList();
    }

    private Task<FotmobLeagueRoot?> GetFotmobLeagueData(FantasyType fantasyType, CancellationToken cancellationToken)
        => httpClient.GetFromJsonAsync<FotmobLeagueRoot>(ConstructLeagueUrl(fantasyType), cancellationToken);

    private Task<FotmobFixtureDetailsRoot?> GetFotmobFixtureDetails(string fotmobFixtureId, CancellationToken cancellationToken)
        => httpClient.GetFromJsonAsync<FotmobFixtureDetailsRoot>(ConstructFixtureDetailsUrl(fotmobFixtureId), cancellationToken);

    /// <summary>
    /// Requires the value of a stat type in the Fotmob Options.
    /// The value is expected to be the url slug used to get the correct data.
    /// If such a slug is not configured an exception will be thrown.
    /// </summary>
    /// <param name="statPropertyName">The name of the stat property hoding the slug value.</param>
    private string RequireStatTypeSlug(string statPropertyName)
        => Options.PlayerData.GetType()?.GetProperty(statPropertyName)?.GetValue(Options.PlayerData)?.ToString()
            ?? throw new NullReferenceException(nameof(Options.PlayerData));

    private string ConstructStatUrl(FantasyType fantasyType, string statTypeSlug)
        => fantasyType switch
        {
            FantasyType.FPL => $"{Options.PlayerData.BaseUrl}/stats/{Options.PL.LeagueId}/season/{Options.PL.SeasonId}/{statTypeSlug}.json",
            FantasyType.Allsvenskan => $"{Options.BaseUrl}/stats/{Options.Allsvenskan.LeagueId}/season/{Options.Allsvenskan.SeasonId}/{statTypeSlug}.json",
            _ => throw new FantasyTypeNotSupportedException()
        };

    private string ConstructLeagueUrl(FantasyType fantasyType)
        => fantasyType switch
        {
            FantasyType.FPL => $"{Options.BaseUrl}/leagues?id={Options.PL.LeagueId}",
            FantasyType.Allsvenskan => $"{Options.BaseUrl}/leagues?id={Options.Allsvenskan.LeagueId}",
            _ => throw new FantasyTypeNotSupportedException()
        };

    private string ConstructFixtureDetailsUrl(string fotmobFixtureId)
        => $"{Options.BaseUrl}/matchDetails?matchId={fotmobFixtureId}";
}
