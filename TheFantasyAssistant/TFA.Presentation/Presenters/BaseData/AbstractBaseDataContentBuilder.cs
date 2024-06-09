using System.Diagnostics.CodeAnalysis;
using TFA.Application.Features.BaseData.Events;
using TFA.Application.Features.BaseData.Transforms;
using TFA.Domain.Data;

namespace TFA.Presentation.Presenters.BaseData;

public abstract class AbstractBaseDataContentBuilder<TPresentType> : AbstractContentBuilder<BaseDataPresentModel, TPresentType>
{
    public virtual string BuildPlayerPriceChangeContent(IReadOnlyList<PlayerPriceChange> players, FantasyType fantasyType, [ConstantExpected] string header, [ConstantExpected] string emoji)
        => players.Count == 0
            ? string.Empty
            : new ContentBuilder()
                .AppendStandardHeader(fantasyType, header)
                .AppendTextLines(player =>
                    $"{emoji} {player.DisplayName} #{player.TeamShortName} £{player.CurrentPrice.ConvertPriceToString()}m", players);

    public virtual string BuildPlayerStatusAvailableChangeContent(IReadOnlyList<PlayerStatusChange> players, FantasyType fantasyType, [ConstantExpected] string header, [ConstantExpected] string emoji)
        => players.Count == 0
            ? string.Empty
            : new ContentBuilder()
                .AppendStandardHeader(fantasyType, header)
                .AppendTextLines(player =>
                    $"{emoji} {player.DisplayName} #{player.TeamShortName}", players);

    public virtual string BuildPlayerStatusNotAvailableChangeContent(IReadOnlyList<PlayerStatusChange> players, FantasyType fantasyType, [ConstantExpected] string header, [ConstantExpected] string emoji)
        => players.Count == 0
            ? string.Empty
            : new ContentBuilder()
                .AppendStandardHeader(fantasyType, header)
                .AppendTextLines(player =>
                    $"{emoji} {player.DisplayName} #{player.TeamShortName} {(!string.IsNullOrWhiteSpace(player.News) ? $"- [{player.News}]" : string.Empty)}", players);

    public virtual string BuildNewPlayersContent(IReadOnlyList<NewPlayer> players, FantasyType fantasyType, [ConstantExpected] string header, [ConstantExpected] string emoji)
        => players.Count == 0
            ? string.Empty
            : new ContentBuilder()
                .AppendStandardHeader(fantasyType, header)
                .AppendTextLines(player =>
                    $"{emoji} {player.DisplayName} ({player.Position}) #{player.TeamShortName} - [£{player.Price}m]", players);

    public virtual string BuildTransferredPlayersContent(IReadOnlyList<PlayerTransfer> players, FantasyType fantasyType, [ConstantExpected] string header, [ConstantExpected] string emoji)
        => players.Count == 0
            ? string.Empty
            : new ContentBuilder()
                .AppendStandardHeader(fantasyType, header)
                .AppendTextLines(player =>
                    $"{emoji} {player.DisplayName} [#{player.PrevTeamShortName} {Emoji.ArrowRight} #{player.NewTeamShortName}]", players);
}

public sealed class BaseDataContentHeaders
{
    public const string PriceRises = "Price Rises";
    public const string PriceFallers = "Price Fallers";
    public const string PlayersAvailable = "Players Available";
    public const string PlayersDoubtful = "Players Doubtful";
    public const string PlayersUnavailable = "Players Unavailable";
    public const string NewPlayers = "New Players";
    public const string TransferredPlayers = "Transferred Players";
    public const string DoubleGameweekAnnouncement = "Double Gameweek Announcements";
    public const string BlankGameweekAnnouncement = "Blank Gameweek Announcements";
}
