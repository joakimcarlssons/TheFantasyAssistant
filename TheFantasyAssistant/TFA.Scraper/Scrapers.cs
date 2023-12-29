using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using TFA.Scraper.Contracts;
using TFA.Scraper.Data;
using TFA.Scraper.Services;

namespace TFA.Scraper;

/// <summary>
/// The scraper endpoints.
/// </summary>
public class Scrapers
{
    private readonly ILeagueService _leagueService;
    private readonly IPredictedPriceChangesService _ppcService;

    public Scrapers(ILeagueService leagueService, IPredictedPriceChangesService ppcService)
    {
        _leagueService = leagueService;
        _ppcService = ppcService;
    }

    [FunctionName("fpl-league-data")]
    public IActionResult FPLeagueData([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req, CancellationToken cancellationToken)
        => StartScrapingAndReturnActionResult(body => _leagueService.ScrapeLeagueData(LeagueType.PremierLeague, body.CallbackUrl), req.Body);

    [FunctionName("fpl-ppc")]
    public IActionResult FPLPPC([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req, CancellationToken cancellationToken)
        => StartScrapingAndReturnActionResult(body => _ppcService.ScrapePredictedPriceChangingPlayers(body.CallbackUrl), req.Body);

    [FunctionName("fas-league-data")]
    public IActionResult FASLeagueData([HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req, CancellationToken cancellationToken)
            => StartScrapingAndReturnActionResult(body => _leagueService.ScrapeLeagueData(LeagueType.Allsvenskan, body.CallbackUrl), req.Body);


    /// <summary>
    /// This will trigger a given scraper (prefferably inherited from <see cref="AbstractScraperService" />)
    /// which will fire off on a different thread and then post the result back in a provided callback url.
    /// </summary>
    /// <param name="scraper">The scraper to fire.</param>
    /// <param name="requestBody">The body containing the mandatory parameters to get a valid data flow.</param>
    /// <returns>
    /// <see cref="StatusCodes.Status400BadRequest"/> in case the body does not match the contract of <see cref="ScraperRequest"/>.
    /// Else <see cref="StatusCodes.Status202Accepted" />.
    /// </returns>
    private static IActionResult StartScrapingAndReturnActionResult(Action<ScraperRequest> scraper, Stream requestBody)
    {
        ScraperRequest? scraperRequest = requestBody.ParseScraperRequest();
        if (scraperRequest is null)
        {
            // This means the scraper request provided by the caller was set up wrong
            return new BadRequestResult();
        }

        // Invoke the passed scraper
        scraper.Invoke(scraperRequest);
        return new AcceptedResult();
    }
}
