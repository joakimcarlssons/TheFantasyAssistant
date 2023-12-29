using AngleSharp;
using AngleSharp.Dom;
using Microsoft.Extensions.Options;
using PuppeteerSharp;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using TFA.Scraper.Config;
using TFA.Scraper.Models;

namespace TFA.Scraper.Services;

public interface IPredictedPriceChangesService : IScraperService
{
    /// <summary>
    /// Will get predicted rising and falling players.
    /// </summary>
    /// <remarks>As of right now this is only set up for FPL.</remarks>
    void ScrapePredictedPriceChangingPlayers(string callbackUrl);
}

internal class PredictedPriceChangesService : AbstractScraperService, IPredictedPriceChangesService
{
    private readonly PredictedPriceChangesOptions _ppcOptions;

    public PredictedPriceChangesService(
        IHttpClientFactory clientFactory, 
        IOptions<ChromeOptions> chromeOptions, 
        IOptions<ApiOptions> apiOptions, 
        IOptions<PredictedPriceChangesOptions> ppcOptions,
        IEmailService email) 
        : base(clientFactory, chromeOptions, apiOptions, email)
    {
        _ppcOptions = ppcOptions.Value;
    }

    /// <inheritdoc />
    public void ScrapePredictedPriceChangingPlayers(string callbackUrl)
        => ScrapeAsync(async (browser) =>
        {
            IReadOnlyList<PredictedPlayerPriceChange> risingPlayers = await GetRisingPlayers(browser);
            IReadOnlyList<PredictedPlayerPriceChange> fallingPlayers = await GetFallingPlayers(browser);

            return new PredictedPriceChangingPlayers
            {
                RisingPlayers = risingPlayers,
                FallingPlayers = fallingPlayers
            };
        }, callbackUrl);


    /// <summary>
    /// Fetches the players with the highest chance of a price rise.
    /// </summary>
    private async Task<IReadOnlyList<PredictedPlayerPriceChange>> GetRisingPlayers(IBrowser browser)
    {
        // Set up and navigate to new page
        string url = _ppcOptions.FPLUrl;
        using IPage page = await OpenNewPage(browser, url);

        // Scrape content
        string? content = await page.GetContentAsync();
        IBrowsingContext context = BrowsingContext.New(Configuration.Default);
        IDocument document = await context.OpenAsync(req => req.Content(content));

        IHtmlCollection<IElement>? players = document
            .QuerySelector("#myDataTable")
            ?.QuerySelector("tbody")
            ?.QuerySelectorAll("tr");

        if (players is not { Length: > 0})
            throw new InvalidDataException($"No rising players were found in PPC Scraper for url {url}.");

        await page.CloseAsync();
        return ParsePlayers(players);
    }

    /// <summary>
    /// Fetches the players with the highest chance of a price drop.
    /// Will run recursively until a certain amount of players is fetched.
    /// </summary>
    private async Task<IReadOnlyList<PredictedPlayerPriceChange>> GetFallingPlayers(IBrowser browser, bool fetchPenultimate = false, IEnumerable<IElement>? fetchedPlayers = null)
    {
        string url = _ppcOptions.FPLUrl;
        using IPage page = await OpenNewPage(browser, url);
        await page.ClickAsync(".last");

        if (fetchPenultimate)
        {
            // This is the button for the second last page
            // This is clicked in case we didn't fetch enough players on last page
            await page.ClickAsync("*[data-dt-idx='6']");
        }

        string content = await page.GetContentAsync();
        IBrowsingContext context = BrowsingContext.New(Configuration.Default);
        IDocument document = await context.OpenAsync(req => req.Content(content));

        IHtmlCollection<IElement>? loadedPlayers = document
            .QuerySelector("#myDataTable")
            ?.QuerySelector("tbody")
            ?.QuerySelectorAll("tr");

        if (loadedPlayers is not { Length: > 0 })
            throw new InvalidDataException($"No falling players were found in PPC scraper for url {url}.");

        List<IElement> players = new();
        if (fetchPenultimate)
            players.AddRange(fetchedPlayers ?? Array.Empty<IElement>());
        players.AddRange(loadedPlayers);

        await page.CloseAsync();

        // Rerun the scraper if we haven't fetched atleast 10 players
        return players.Count < 10 
            ? await GetFallingPlayers(browser, true, players) 
            : ParsePlayers(players);
    }

    /// <summary>
    /// Parses a collection of scraped elements into <see cref="PredictedPlayerPriceChange "/>.
    /// </summary>
    /// <param name="fetchedPlayers">The scraped elements expected to be players.</param>
    private static IReadOnlyList<PredictedPlayerPriceChange> ParsePlayers(IEnumerable<IElement> fetchedPlayers)
    {
        List<PredictedPlayerPriceChange> parsedPlayers = new();

        foreach (IElement player in fetchedPlayers)
        {
            IHtmlCollection<IElement> props = player.QuerySelectorAll("td");

            parsedPlayers.Add(new()
            {
                DisplayName = props[1]?.QuerySelector("span")?.InnerHtml ?? string.Empty,
                TeamName = props[2]?.InnerHtml ?? string.Empty,
                Price = decimal.Parse(props[6].InnerHtml[1..^1], CultureInfo.InvariantCulture),
                PriceTarget = decimal.Parse(props[^2].QuerySelector("div")?.InnerHtml ?? props[^2].InnerHtml, CultureInfo.InvariantCulture)
            });
        }

        return parsedPlayers;
    }
}
