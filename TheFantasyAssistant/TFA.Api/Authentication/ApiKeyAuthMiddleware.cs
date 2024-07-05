using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using TFA.Api.Authentication;

namespace TFA.Api.Middlewares;

/// <summary>
/// Authentication middleware to check for an Api Key.
/// </summary>
public class ApiKeyAuthMiddleware(RequestDelegate next, IOptions<AuthOptions> options)
{
    private readonly AuthOptions authOptions = options.Value;

    public async Task InvokeAsync(HttpContext context) 
    {
        if (context.Request.Path.StartsWithSegments("/wss"))
        {
            // Ignore Api Key Auth for WSS connections
            await next(context);
            return;
        }

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
