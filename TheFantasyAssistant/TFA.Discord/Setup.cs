﻿using DSharpPlus;
using DSharpPlus.SlashCommands;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.Reflection;
using TFA.Discord.Config;

namespace TFA.Discord;

public static class Setup
{
    public static IServiceCollection AddDiscord(this IServiceCollection services, Assembly assembly)
    {
        services.AddOptions<DiscordOptions>()
            .BindConfiguration(DiscordOptions.Key)
            .ValidateDataAnnotations();

        services
            .AddSingleton<IDiscordService, DiscordService>()
            .AddDiscordClient(assembly);

        return services;
    }

    private static IServiceCollection AddDiscordClient(this IServiceCollection services, Assembly assembly)
    {
        IServiceProvider serviceProvider = services.BuildServiceProvider();
        DiscordClient client = new(new DiscordConfiguration
        {
            Token = serviceProvider.GetRequiredService<IOptions<DiscordOptions>>().Value.Token,
            TokenType = TokenType.Bot,
            Intents = DiscordIntents.AllUnprivileged,
        });

        // Register all slash commands from executing assembly
        SlashCommandsExtension slash = client.UseSlashCommands(new SlashCommandsConfiguration
        {
            Services = serviceProvider
        });

        assembly.GetExportedTypes()
            .Where(x => !x.IsAbstract && x.IsClass && x.IsSubclassOf(typeof(ApplicationCommandModule)))
            .ToList()
            .ForEach(module =>
            {
#if DEBUG
                slash.RegisterCommands(module, 831945050736295937);
#else
                slash.RegisterCommands(module);
#endif
            });

        return services.AddSingleton(client);
    }
}
