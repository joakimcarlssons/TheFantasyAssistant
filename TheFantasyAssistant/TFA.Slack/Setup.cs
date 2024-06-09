using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using SlackNet.AspNetCore;
using TFA.Slack.Config;

namespace TFA.Slack;

public static class Setup
{
    public static IServiceCollection AddSlack(this IServiceCollection services)
    {
        services.AddOptions<SlackOptions>()
            .BindConfiguration(SlackOptions.Key)
            .ValidateDataAnnotations();

        services.AddSingleton<ISlackService, SlackService>();
        return services;
    }

    public static IApplicationBuilder UseSlack(this IApplicationBuilder app)
    {
        app.UseSlackNet();
        return app;
    }
}
