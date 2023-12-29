using System.Diagnostics.CodeAnalysis;
using TFA.Application.Features.BaseData.Events;
using TFA.Application.Features.PredictedPriceChanges;
using TFA.Domain.Data;

namespace TFA.Presentation.Presenters.PredictedPriceChanges;

public sealed class PredictedPriceChangeTwitterBuilder : AbstractContentBuilder<PredictedPriceChangeData, string>
{

    private FantasyType FantasyType = FantasyType.Unknown;
    public override Presenter Presenter => Presenter.Twitter;

    public override IReadOnlyList<string> Build(PredictedPriceChangeData data)
    {
        FantasyType = data.FantasyType;
        return BuildTwitterContent(data);
    }

    private IReadOnlyList<string> BuildTwitterContent(PredictedPriceChangeData data)
        => [
            BuildStringContent(BuildPossiblePriceRisesContent, data.RisingPlayers, "Possible Price Rises", Emoji.ArrowLowerLeft),
            BuildStringContent(BuildPossiblePriceFallersContent, data.FallingPlayers, "Possible Price Fallers", Emoji.ArrowLowerLeft)
        ];

    private string BuildPossiblePriceRisesContent(IReadOnlyList<PredictedPriceChangePlayer> risingPlayers, string header, [ConstantExpected] string emoji)
        => new ContentBuilder()
                .AppendStandardHeader(FantasyType, header)
                .AppendTextLines(player =>
                    $"{emoji} {player.Player.DisplayName} #{player.TeamShortName}", risingPlayers);

    private string BuildPossiblePriceFallersContent(IReadOnlyList<PredictedPriceChangePlayer> fallingPlayers, string header, [ConstantExpected] string emoji)
        => new ContentBuilder()
                .AppendStandardHeader(FantasyType, header)
                .AppendTextLines(player =>
                    $"{emoji} {player.Player.DisplayName} #{player.TeamShortName}", fallingPlayers);
}
