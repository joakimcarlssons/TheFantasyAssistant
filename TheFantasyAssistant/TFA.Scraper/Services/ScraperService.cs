using Microsoft.Extensions.Options;
using PuppeteerSharp;
using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TFA.Scraper.Config;
using TFA.Scraper.Extensions;
namespace TFA.Scraper.Services;

public interface IScraperService
{
    void ScrapeAsync<TData>(Func<IBrowser, Task<TData>> scraperFunc, string callbackUrl) where TData : class;
}

public abstract class AbstractScraperService : IScraperService
{
    protected readonly IHttpClientFactory _clientFactory;
    protected readonly ChromeOptions _chromeOptions;
    protected readonly ApiOptions _apiOptions;
    protected readonly IEmailService _email;

    public AbstractScraperService(
        IHttpClientFactory clientFactory, 
        IOptions<ChromeOptions> chromeOptions, 
        IOptions<ApiOptions> apiOptions,
        IEmailService email)
    {
        _clientFactory = clientFactory;
        _chromeOptions = chromeOptions.Value;
        _apiOptions = apiOptions.Value;
        _email = email;
    }

    /// <summary>
    /// Will perform scraping on a different thread
    /// and send the data to a given callback URL.
    /// </summary>
    public virtual void ScrapeAsync<TData>(Func<IBrowser, Task<TData>> scraperFunc, string callbackUrl) where TData : class
    {
        Thread thread = new(async () =>
        {
            try
            {
                using (IBrowser browser = await SetupBrowser())
                {
                    TData data = await scraperFunc.Invoke(browser);
                    await HandleScrapedData(data, callbackUrl);
                }
            }
            catch (Exception ex)
            {
                await _email.SendAsync($"{EmailTypes.Error}: {ex.Message}", $"{ex.Message}\n\n{ex.StackTrace}");
            }
        })
        {
            IsBackground = true
        };
        thread.Start();
    }

    /// <summary>
    /// Sets up the browser to be used when scraping data.
    /// In debug mode we use a local copy of the chrome.exe.
    /// </summary>
    private async Task<IBrowser> SetupBrowser()
    {
        BrowserFetcher browserFetcher = new(new BrowserFetcherOptions
        {
            Path = Path.GetTempPath(),
        });


        if (!browserFetcher.RevisionInfo(BrowserFetcher.DefaultChromiumRevision).Downloaded && !browserFetcher.RevisionInfo(BrowserFetcher.DefaultChromiumRevision).Local)
        {
            await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
        }
        IBrowser browser = await Puppeteer.LaunchAsync(new LaunchOptions
        {
            Headless = true,
#if DEBUG
            ExecutablePath = _chromeOptions.ChromeExecutablePath
#else
            ExecutablePath = browserFetcher.RevisionInfo(BrowserFetcher.DefaultChromiumRevision.ToString()).ExecutablePath
#endif
        });

        return browser;
    }

    /// <summary>
    /// This will handle the scraped data by posting it to a given callback url.
    /// </summary>
    /// <typeparam name="TData">The type of data that will be sent.</typeparam>
    /// <param name="data">The data that will be sent.</param>
    /// <param name="callbackUrl">The url where the data will be sent to.</param>
    protected virtual async Task HandleScrapedData<TData>(TData data, string callbackUrl) where TData : class
    {
        using var client = _clientFactory.CreateClient();
        client.DefaultRequestHeaders.Add(ApiOptions.ApiKeyHeaderValue, _apiOptions.ApiKey);
        await client.ExecutePostAsync(callbackUrl, data);
    }

    /// <summary>
    /// Opens a new page in a given browser and navigates to a given url.
    /// Will time out after a set amount of seconds if the page is idle.
    /// </summary>
    /// <param name="browser">The browser to open the page in.</param>
    /// <param name="url">The url to navigate to on the page.</param>
    protected virtual async Task<IPage> OpenNewPage(IBrowser browser, string url)
    {
        const int timeOutTime = 10000;

        IPage page = await browser.NewPageAsync();
        await page.SetUserAgentAsync(_chromeOptions.UserAgent);
        await page.GoToAsync(url);
        await page.WaitForNetworkIdleAsync(new() { Timeout = timeOutTime })
            .ContinueWith(async _ =>
            {
                await _email.SendAsync($"{EmailTypes.Error}: Scraper timed out", $"Scraper for url {url} timed out after {timeOutTime} ms.");
            });

        return page;
    }
}
