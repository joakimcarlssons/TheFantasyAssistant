namespace TFA.Application.Features.BaseData.Transforms;

public record TransformedBaseData(
    PlayerPriceChanges PlayerPriceChanges,
    PlayerStatusChanges PlayerStatusChanges,
    IReadOnlyList<NewPlayer> NewPlayers,
    IReadOnlyList<PlayerTransfer> PlayerTransfers);

public sealed record PlayerPriceChanges(
    IReadOnlyList<PlayerPriceChange> RisingPlayers,
    IReadOnlyList<PlayerPriceChange> FallingPlayers);

public sealed record PlayerStatusChanges(
    IReadOnlyList<PlayerStatusChange> AvailablePlayers,
    IReadOnlyList<PlayerStatusChange> DoubtfulPlayers,
    IReadOnlyList<PlayerStatusChange> UnavailablePlayers);

public sealed record PlayerStatusChange(
    int PlayerId,
    string DisplayName,
    int ChanceOfPlayingNextRound,
    string? News,
    int TeamId,
    string TeamShortName);

public sealed record PlayerPriceChange(
    int PlayerId,
    string DisplayName,
    decimal PreviousPrice,
    decimal CurrentPrice,
    int TeamId,
    string TeamShortName);

public record NewPlayer(
    int PlayerId,
    string DisplayName,
    string Position,
    decimal Price,
    int TeamId,
    string TeamShortName);

public record PlayerTransfer(
    int PlayerId,
    string DisplayName,
    int PrevTeamId,
    string PrevTeamShortName,
    int NewTeamId,
    string NewTeamShortName);
