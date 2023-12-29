using Microsoft.Extensions.DependencyInjection;
using TFA.Twitter.Config;

namespace TFA.Twitter;

public static class Setup
{
    public static IServiceCollection AddTwitter<TTwitterService>(this IServiceCollection services) 
        where TTwitterService : AbstractTwitterService
    {
        services.AddOptions<TwitterOptions>()
            .BindConfiguration(TwitterOptions.Key)
            .ValidateDataAnnotations();

        services.AddTransient<ITwitterService, TTwitterService>();
                
        return services;
    }
}
