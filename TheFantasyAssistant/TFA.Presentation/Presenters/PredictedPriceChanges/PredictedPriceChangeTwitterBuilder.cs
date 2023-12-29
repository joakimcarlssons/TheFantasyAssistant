using System.Diagnostics.CodeAnalysis;
using TFA.Application.Features.BaseData.Events;
using TFA.Application.Features.PredictedPriceChanges;
using TFA.Domain.Data;

namespace TFA.Presentation.Presenters.PredictedPriceChanges;

public class PredictedPriceChangeTwitterBuilder : AbstractContentBuilder<PredictedPriceChangeData, string>
{
    /// <summary>
    /// At the moment there are only support for FPL Price predictions.
    /// If this is implemented for more fantasy types a Present model is needed including the Fantasy Type.
    /// See <see cref="BaseDataPresentModel" /> as an example.
    /// </summary>
    private readonly FantasyType FantasyType = FantasyType.FPL;
    public override Presenter Presenter => Presenter.Twitter;

    public override IReadOnlyList<string> Build(PredictedPriceChangeData data)
        => BuildTwitterContent(data);

    private IReadOnlyList<string> BuildTwitterContent(PredictedPriceChangeData data)
        => [
            BuildStringContent(BuildPossiblePriceRisesContent, data.RisingPlayers, "Possible Price Rises", Emoji.ArrowLowerLeft),
            BuildStringContent(BuildPossiblePriceFallersContent, data.FallingPlayers, "Possible Price Fallers", Emoji.ArrowLowerLeft)
        ];

    private string BuildPossiblePriceRisesContent(IReadOnlyList<PredictedPriceChangePlayer> risingPlayers, string header, [ConstantExpected] string emoji)
        => new ContentBuilder()
                .AppendStandardHeader(FantasyType, header)
                .AppendTextLines(player =>
                    $"{emoji} {player.Player.DisplayName} {(string.IsNullOrWhiteSpace(player.TeamShortName) ? string.Empty : ("#" + player.TeamShortName))}", risingPlayers);

    private string BuildPossiblePriceFallersContent(IReadOnlyList<PredictedPriceChangePlayer> fallingPlayers, string header, [ConstantExpected] string emoji)
        => new ContentBuilder()
                .AppendStandardHeader(FantasyType, header)
                .AppendTextLines(player =>
                    $"{emoji} {player.Player.DisplayName} {(string.IsNullOrWhiteSpace(player.TeamShortName) ? string.Empty : ("#" + player.TeamShortName))}", fallingPlayers);
}
