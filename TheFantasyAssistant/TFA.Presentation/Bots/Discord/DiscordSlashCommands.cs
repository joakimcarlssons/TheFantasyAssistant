using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;

namespace TFA.Presentation.Bots.Discord;

public class DiscordSlashCommands : ApplicationCommandModule
{
    [SlashCommand("Ping", "Ping to get pong")]
    public static async Task PingPongCommand(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(
            InteractionResponseType.ChannelMessageWithSource,
            new DiscordInteractionResponseBuilder()
                .WithContent("Pong!"));
    }

    [SlashCommand("PingDelay", "Ping to get pong with a delay")]
    public static async Task DelayedTask(InteractionContext ctx)
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);

        await Task.Delay(500);

        await ctx.EditResponseAsync(new DiscordWebhookBuilder()
            .WithContent("Sorry for the wait. Here's your Pong!"));
    }
}
