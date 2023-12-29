using DSharpPlus;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using TFA.Discord.Config;

namespace TFA.Discord;

public static class Setup
{
    public static IServiceCollection AddDiscord(this IServiceCollection services)
    {
        services.AddOptions<DiscordOptions>()
            .BindConfiguration(DiscordOptions.Key)
            .ValidateDataAnnotations();

        services.AddSingleton<IDiscordService, DiscordService>();
        services.AddSingleton(serviceProvider =>
            new DiscordClient(new DiscordConfiguration
            {
                Token = serviceProvider.GetRequiredService<IOptions<DiscordOptions>>().Value.Token,
                TokenType = TokenType.Bot,
                Intents = DiscordIntents.AllUnprivileged
            }));

        return services;
    }
}
