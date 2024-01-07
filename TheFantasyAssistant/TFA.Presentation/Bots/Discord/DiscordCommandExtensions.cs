using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using ErrorOr;
using TFA.Application.Common.Extensions;
using TFA.Application.Interfaces.Services;
using TFA.Domain.Data;

namespace TFA.Presentation.Bots.Discord;

public static class DiscordCommandExtensions
{
    public static Task CreateCommandResponse<TMatch>(this ErrorOr<IBotCommandResponse> handledCommand, InteractionContext ctx, Func<TMatch, Task> responseBuilder)
        where TMatch : IBotCommandResponse
    {
        return handledCommand.Match(
            value =>
            {
                return handledCommand.Value is TMatch match
                    ? responseBuilder.Invoke(match)
                    : Task.CompletedTask;
            },
            errors => ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .WithContent(handledCommand.Errors.ToErrorString())));
    }

    public static FantasyType GetFantasyType(this InteractionContext ctx)
        => ctx.Channel.Parent.Name switch
        {
            string n when n.Equals("Dev", StringComparison.InvariantCultureIgnoreCase) 
                => FantasyType.FPL,
            string n when n.Equals("FPL", StringComparison.InvariantCultureIgnoreCase) 
                => FantasyType.FPL,
            string n when n.Equals("Fantasy Allsvenskan", StringComparison.InvariantCultureIgnoreCase)
                => FantasyType.Allsvenskan,
            _ => FantasyType.Unknown
        };
}
