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

        // Adjust the timeout since it can vary some depending on if the app is sleeping or not
        _httpClient.Timeout = TimeSpan.FromMinutes(3);

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
        try
        {
            // Trigger all scheduled requests
            HttpResponseMessage[] failedRequests = (await Task
                .WhenAll(_services
                    .Where(service => service.RunTime == runTime)
                    .Select(service => TriggerRequest(service, cancellationToken))))

                // Extract all failed request
                .Where(req => !req.IsSuccessStatusCode)
                .ToArray();

            foreach (HttpResponseMessage failedRequest in failedRequests)
            {
                // Handle failed request
                Uri requestUri = failedRequest.RequestMessage.RequestUri;
                HttpStatusCode statusCode = failedRequest.StatusCode;

                string message = $"{EmailTypes.Warning}: Request to {requestUri} failed with status code {(int)statusCode}";
                await _email.SendAsync(message, message);
            }
        }
        catch (Exception ex)
        {
            await _email.SendAsync($"{EmailTypes.Error}: {ex.Message}", $"{ex.Message}\n\n{ex.StackTrace}");
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
