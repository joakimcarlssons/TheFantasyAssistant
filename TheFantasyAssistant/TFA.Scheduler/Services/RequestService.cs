using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using TFA.Scheduler.Config;
using TFA.Scheduler.Data;

namespace TFA.Scheduler.Services;

public interface IRequestService
{
    Task HandleScheduledRequests(RunTime runTime, CancellationToken cancellationToken);
}

public class RequestService : IRequestService
{
    private readonly HttpClient _httpClient;
    private readonly ApiOptions _apiOptions;
    private readonly IReadOnlyList<ServiceOption> _services;
    private readonly IEmailService _email;

    public RequestService(HttpClient httpClient, IOptions<ApiOptions> apiOptions, IOptions<List<ServiceOption>> services, IEmailService email)
    {
        _httpClient = httpClient;

        // Adjust the timeout since it can vary some depending on if the app is sleeping or not.
        _httpClient.Timeout = TimeSpan.FromMinutes(5);

        _services = services.Value;
        _apiOptions = apiOptions.Value;
        _email = email;
    }

    /// <summary>
    /// Picks out all services with the given <paramref name="runTime"/> and triggers them.
    /// Sends notifications by email for every failed request.
    /// </summary>
    /// <param name="runTime">The <see cref="RunTime"/> of the requests to be made.</param>
    public async Task HandleScheduledRequests(RunTime runTime, CancellationToken cancellationToken)
    {
        string latestTriggeredRequestUrlSuffix = string.Empty;

        try
        {
            foreach (ServiceOption service in _services.Where(s => s.RunTime == runTime && s.Enabled))
            {
                int maxRetries = 2;
                int retryCount = 0;

                while (retryCount < maxRetries)
                {
                    try
                    {
                        latestTriggeredRequestUrlSuffix = service.UrlSuffix;
                        await HandleRequest(service, cancellationToken);
                        break;
                    }
                    catch (TaskCanceledException)
                    {
                        retryCount++;
                        if (retryCount >= maxRetries)
                        {
                            throw;
                        }

                        // Wait a second before trying again
                        await Task.Delay(1000, cancellationToken);
                    }
                    catch
                    {
                        throw;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            await _email.SendAsync($"{EmailTypes.Error}: {ex.GetType().Name} when calling {latestTriggeredRequestUrlSuffix}", $"{ex.Message}\n\n{ex.StackTrace}");
        }
    }

    /// <summary>
    /// Handles the actual request and sends an email in case of a non success status code.
    /// </summary>
    private async Task HandleRequest(ServiceOption service, CancellationToken cancellationToken)
    {
        if (await TriggerRequest(service, cancellationToken) is { IsSuccessStatusCode: false } failedRequest)
        {
            // Handle failed request
            Uri requestUri = failedRequest.RequestMessage.RequestUri;
            HttpStatusCode statusCode = failedRequest.StatusCode;

            ErrorAction action = statusCode switch
            {
                // Timeouts will happen quite regular as long as the Api is running on free Azure App Service.
                // No need to send emails for each timeout, the retry mechanism will take care of it in case the Api don't go up as expected
                HttpStatusCode.GatewayTimeout
                or HttpStatusCode.RequestTimeout => ErrorAction.Log,

                // Default is to send a warning email
                _ => ErrorAction.Email
            };

            if (action == ErrorAction.Email)
            {
                string message = $"{EmailTypes.Warning}: Request to {requestUri} failed with status code {(int)statusCode}";
                await _email.SendAsync(message, message);
            }
        }
    }

    /// <summary>
    /// Setting up a request towards a given service and triggers a request.
    /// </summary>
    /// <param name="service">The service to be triggered</param>
    private Task<HttpResponseMessage> TriggerRequest(ServiceOption service, CancellationToken cancellationToken)
    {
        HttpRequestMessage request = new()
        {
            RequestUri = new Uri($"{_apiOptions.Url}{service.UrlSuffix}"),
            Method = HttpMethod.Post,
        };

        request.Headers.Add(ApiOptions.ApiKeyHeaderValue, _apiOptions.ApiKey);
        return _httpClient.SendAsync(request, cancellationToken);
    }
}

public enum ErrorAction
{
    None,
    Log,
    Email
}