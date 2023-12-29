using AngleSharp;
using AngleSharp.Dom;
using Microsoft.Extensions.Options;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TFA.Scraper.Config;
using TFA.Scraper.Data;
using TFA.Scraper.Models;

namespace TFA.Scraper.Services;

public interface ILeagueService : IScraperService
{
    void ScrapeLeagueData(LeagueType leagueType, string callbackUrl);
}

internal class LeagueService : AbstractScraperService, ILeagueService
{
    private readonly LeagueOptions _leagueOptions;

    public LeagueService(
        IHttpClientFactory clientFactory, 
        IOptions<ChromeOptions> chromeOptions, 
        IOptions<ApiOptions> apiOptions,
        IOptions<LeagueOptions> leagueOptions,
        IEmailService email) 
        : base(clientFactory, chromeOptions, apiOptions, email)
    {
        _leagueOptions = leagueOptions.Value;
    }

    public void ScrapeLeagueData(LeagueType leagueType, string callbackUrl)
        => ScrapeAsync(async (browser) => await GetLeagueDataAsync(browser, leagueType), callbackUrl);

    private async Task<League> GetLeagueDataAsync(IBrowser browser, LeagueType leagueType)
    {
        string url = leagueType switch
        {
            LeagueType.PremierLeague => _leagueOptions.PremierLeagueUrl,
            LeagueType.Allsvenskan => _leagueOptions.AllsvenskanUrl,
            _ => throw new NotImplementedException(),
        };

        using IPage page = await OpenNewPage(browser, url);

        string? content = await page.GetContentAsync();
        IBrowsingContext context = BrowsingContext.New(Configuration.Default);
        IDocument document = await context.OpenAsync(req => req.Content(content));

        IHtmlCollection<IElement>? teams = 
            document.QuerySelector(".full-league-table")
                ?.QuerySelector("tbody")
                ?.QuerySelectorAll("tr");

        if (teams is not { Length: > 0 })
            throw new InvalidDataException($"No teams were found in League scraper for url {url}.");

        HashSet<LeagueTeam> parsedTeams = ParseTeams(teams);

        await page.CloseAsync();
        return new League
        {
            Teams = parsedTeams
        };
    }

    private static string GetValue(IEnumerable<IElement> nodes, string className)
            => nodes.First(node => node.ClassName!.Contains(className)).TextContent;

    private static int GetNumericValue(IEnumerable<IElement> nodes, string className)
        => int.Parse(nodes.First(node => node.ClassName!.Contains(className)).TextContent);

    private static HashSet<LeagueTeam> ParseTeams(IEnumerable<IElement> fetchedTeams)
    {
        HashSet<LeagueTeam> parsedTeams = new();

        foreach (IElement team in fetchedTeams)

        {
            IHtmlCollection<IElement> props = team.QuerySelectorAll("td");
            parsedTeams.Add(new()
            {
                Position = GetNumericValue(props, "position"),
                Name = GetValue(props, "team"),
                MatchesPlayed = GetNumericValue(props, "mp"),
                Wins = GetNumericValue(props, "win"),
                Draws = GetNumericValue(props, "draw"),
                Losses = GetNumericValue(props, "loss"),
                GoalsScored = GetNumericValue(props, "gf"),
                GoalsConceded = GetNumericValue(props, "ga"),
                GoalDifference = GetNumericValue(props, "gd"),
                Points = GetNumericValue(props, "points")
            });
        }

        return parsedTeams;
    }
}
