using DSharpPlus;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using ErrorOr;
using System.Diagnostics.CodeAnalysis;
using TFA.Application.Common.Commands;
using TFA.Application.Interfaces.Services;
using TFA.Domain.Data;

namespace TFA.Presentation.Bots.Discord;

public class DiscordSlashCommands(IBotService bot) : ApplicationCommandModule
{
    [SlashCommand(BotCommands.BestFixtures.Name, BotCommands.BestFixtures.Description)]
    public async Task BestFixturesCommand(InteractionContext ctx,
        [Option("from", "From what gameweek to check")] long fromGw,
        [Option("to", "To what gameweek to check")] long toGw)
    {
        await WrapResponseAsync<BestFixturesCommandResponse>(
            ctx, 
            ctx.GetFantasyType(),
            BotCommands.BestFixtures.Name,
            new Dictionary<string, string>
            {
                { "fromGw", fromGw.ToString() },
                { "toGw", toGw.ToString() }
            },
            x => ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .WithContent(x.Content)));
    }

    /// <summary>
    /// Common wrapper to dynamically call the HandleCommand method on the implemented <see cref="IBotService"/>
    /// and then create a common response if the command was successfully implemented or not.
    /// </summary>
    /// <typeparam name="T">The expected return type from the handler inheriting <see cref="IBotCommandResponse"/>.</typeparam>
    /// <param name="ctx">The context to create response in.</param>
    /// <param name="command">The command to handle the response for.</param>
    /// <param name="responseBuilder">The builder to be invoked if the command was handled successfully.</param>
    private async Task WrapResponseAsync<T>(
        InteractionContext ctx,
        FantasyType fantasyType,
        [ConstantExpected] string command, 
        IReadOnlyDictionary<string, string> options, 
        Func<T, Task> responseBuilder) 
        where T : IBotCommandResponse
    {
        await ctx.CreateResponseAsync(InteractionResponseType.DeferredChannelMessageWithSource);
        ErrorOr<IBotCommandResponse> response = await bot.HandleCommand(command, fantasyType, options);
        await response.CreateCommandResponse<T>(ctx, responseBuilder.Invoke);
    }
}
