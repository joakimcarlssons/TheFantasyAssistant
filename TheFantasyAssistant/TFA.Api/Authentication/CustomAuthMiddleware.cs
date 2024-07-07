using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using TFA.Api.Authentication;
using TFA.Api.Client;

namespace TFA.Api.Middlewares;

/// <summary>
/// Authentication middleware to verify access of various endpoints.
/// </summary>
public class CustomAuthMiddleware(RequestDelegate next, IOptions<AuthOptions> options)
{
    private readonly AuthOptions authOptions = options.Value;

    public async Task InvokeAsync(HttpContext context) 
    {
        // Handle preflight OPTIONS requests
        if (context.Request.Method == HttpMethod.Options.Method)
        {
            context.Response.Headers.Append("Access-Control-Allow-Origin", authOptions.ClientUrl);
            context.Response.Headers.Append("Access-Control-Allow-Methods", "GET, OPTIONS");
            context.Response.Headers.Append("Access-Control-Allow-Headers", $"Content-Type, Authorization, x-requested-with, x-signalr-user-agent, {AuthOptions.ApiKeyHeaderName}, {AuthOptions.ClientKeyHeaderName}");
            context.Response.Headers.Append("Access-Control-Allow-Credentials", "true");
            context.Response.StatusCode = StatusCodes.Status204NoContent;
            return;
        }

        // Verify the origin for WebSocket requests
        if (context.Request.Path.StartsWithSegments("/wss"))
        {
            if (!context.Request.Headers.TryGetValue("Origin", out StringValues origins) || !origins.Contains(authOptions.ClientUrl))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return;
            }

            await next(context);
            return;
        }

        // Clients will do an intial connection via WebSockets in order to receive a key.
        // The key is then used to authenticate the requests.
        if (context.Request.Path.StartsWithSegments("/client"))
        {
            if (!context.Request.Headers.TryGetValue(AuthOptions.ClientKeyHeaderName, out StringValues clientKey))
            {
                context.Response.StatusCode = StatusCodes.Status403Forbidden;
                return;
            }

            foreach (string connection in ClientHub.GetConnections())
            {
                if (connection.Equals(clientKey))
                {
                    await next(context);
                    return;
                }
            }
            
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return;
        }

        // Ordinary Api calls are authenticated via Api Keys
        // These endpoints are typically called by other internal services
        if (!context.Request.Headers.TryGetValue(AuthOptions.ApiKeyHeaderName, out StringValues key))
        {
            // In case no key is provided
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return;
        }

        if (!authOptions.ApiKey.Equals(key))
        {
            // In case the key is not correct
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        await next(context);
    }
}
