using DSharpPlus.Entities;

namespace TFA.Presentation.Bots.Discord;

/// <summary>
/// Common responses used by the discord bot
/// </summary>
public static class DiscordBotResponses
{
    public static DiscordInteractionResponseBuilder CommandDoesNotMatchContext()
        => new DiscordInteractionResponseBuilder()
            .WithContent("The command can't be called within this context.");
}
