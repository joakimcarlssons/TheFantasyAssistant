using DSharpPlus.Entities;
using TFA.Application.Features.PredictedPriceChanges;
using TFA.Discord.Config;
using TFA.Domain.Data;
using TFA.Domain.Exceptions;

namespace TFA.Presentation.Presenters.PredictedPriceChanges;

public sealed class PredictedPriceChangeDiscordBuilder : AbstractContentBuilder<PredictedPriceChangeData, DiscordPresentModel>
{
    public override Presenter Presenter => Presenter.Discord;

    public override IReadOnlyList<DiscordPresentModel> Build(PredictedPriceChangeData data)
        => [
            BuildPossiblePriceRisesContent(data),
            BuildPossiblePriceFallersContent(data)
        ];

    private static DiscordPresentModel BuildPossiblePriceRisesContent(PredictedPriceChangeData data)
        => new(new DiscordEmbedBuilder()
            .WithTitle(new ContentBuilder()
                .AppendStandardHeader(data.FantasyType, $"Possible Price Rises"))
            .WithFooter(NowDate)
            .WithDescription(new ContentBuilder()
                .AppendTextLines(player =>
                    $"{Emoji.ArrowUpperLeft} {player.Player.DisplayName} #{player.TeamShortName}",
                    data.RisingPlayers)),
            data.FantasyType switch
            {
                FantasyType.FPL => DiscordChannels.FPLPriceChanges,
                FantasyType.Allsvenskan => DiscordChannels.AllsvenskanPriceChanges,
                _ => throw new FantasyTypeNotSupportedException()
            });

    private static DiscordPresentModel BuildPossiblePriceFallersContent(PredictedPriceChangeData data)
        => new(new DiscordEmbedBuilder()
            .WithTitle(new ContentBuilder()
                .AppendStandardHeader(data.FantasyType, $"Possible Price Fallers"))
            .WithFooter(NowDate)
            .WithDescription(new ContentBuilder()
                .AppendTextLines(player =>
                    $"{Emoji.ArrowLowerLeft} {player.Player.DisplayName} #{player.TeamShortName}",
                    data.FallingPlayers)),
            data.FantasyType switch
            {
                FantasyType.FPL => DiscordChannels.FPLPriceChanges,
                FantasyType.Allsvenskan => DiscordChannels.AllsvenskanPriceChanges,
                _ => throw new FantasyTypeNotSupportedException()
            });
}
