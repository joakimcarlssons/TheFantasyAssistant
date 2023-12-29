using TFA.Application.Interfaces.Services;

namespace TFA.Application.Features.PredictedPriceChanges;

public sealed record PredictedPriceChangeData(
    FantasyType FantasyType,
    IReadOnlyList<PredictedPriceChangePlayer> RisingPlayers,
    IReadOnlyList<PredictedPriceChangePlayer> FallingPlayers) : INotification, IPresentable;

public sealed record PredictedPriceChangePlayer(
    Player Player,
    decimal PriceTarget,
    string TeamName,
    string TeamShortName);
