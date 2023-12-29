using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Extensions.Options;
using TFA.Application.Config;
using TFA.Application.Interfaces.Services;

namespace TFA.Api.Exceptions;

public class GlobalExceptionHandler : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
    {
        // Send an email with the thrown exception
        if (httpContext.RequestServices.GetService<IEmailService>() is { } emailService
            && httpContext.RequestServices.GetService<IOptions<EmailOptions>>()?.Value is { } emailOptions)
        {
            await emailService.SendExceptionEmail(exception);
        }

        return true;
    }
}
