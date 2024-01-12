namespace TFA.Application.Features.BaseData.Transforms;

public record TransformedBaseData(
    PlayerPriceChanges PlayerPriceChanges,
    PlayerStatusChanges PlayerStatusChanges,
    IReadOnlyList<NewPlayer> NewPlayers,
    IReadOnlyList<PlayerTransfer> PlayerTransfers,
    IReadOnlyList<DoubleGameweek> DoubleGameweeks,
    IReadOnlyList<BlankGameweek> BlankGameweeks);

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

public sealed record NewPlayer(
    int PlayerId,
    string DisplayName,
    string Position,
    decimal Price,
    int TeamId,
    string TeamShortName);

public sealed record PlayerTransfer(
    int PlayerId,
    string DisplayName,
    int PrevTeamId,
    string PrevTeamShortName,
    int NewTeamId,
    string NewTeamShortName);

public sealed record BlankGameweek(
    int Gameweek,
    int TeamId,
    string TeamName,
    string TeamShortName);

public sealed record DoubleGameweek(
    int Gameweek,
    int TeamId,
    string TeamName,
    string TeamShortName,
    IReadOnlyList<FixtureOpponent> Opponents);

public sealed record FixtureOpponent(
    int TeamId,
    string TeamShortName,
    int FixtureDifficulty,
    bool IsHome);