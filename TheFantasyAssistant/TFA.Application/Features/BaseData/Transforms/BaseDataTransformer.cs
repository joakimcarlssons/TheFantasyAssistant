using Mapster;
using TFA.Application.Features.Transforms;
using TFA.Application.Services.BaseData;
using TFA.Utils;

namespace TFA.Application.Features.BaseData.Transforms;

public class BaseDataTransformer : ITransformer<FantasyBaseData, TransformedBaseData>
{
    /// <summary>
    /// Store the teams by for easy lookup.
    /// </summary>
    private Dictionary<int, Team> TeamsById = [];

    public TransformedBaseData Transform(FantasyBaseData newData, FantasyBaseData prevData)
    {
        TeamsById = newData.Teams.ToDictionary(team => team.Id, team => team);

        PlayerPriceChanges priceChanges = GetPlayerPriceChanges(newData, prevData);
        PlayerStatusChanges statusChanges = GetPlayerStatusChanges(newData, prevData);
        IReadOnlyList<NewPlayer> newPlayers = GetNewPlayers(newData, prevData);
        IReadOnlyList<PlayerTransfer> transferredPlayers = GetTransferredPlayers(newData, prevData);

        return new(priceChanges, statusChanges, newPlayers, transferredPlayers);
    }

    /// <summary>
    /// Get players where the price has changed compared to the last loaded dataset.
    /// </summary>
    /// <param name="newData">The dataset containing the new data.</param>
    /// <param name="prevData">The dataset containing the stored data.</param>
    private PlayerPriceChanges GetPlayerPriceChanges(FantasyBaseData newData, FantasyBaseData prevData)
    {
        Dictionary<int, decimal> playerPriceLookup = prevData.Players
            .Where(prevPlayer => prevPlayer.Id > 0)
            .ToDictionary(prevPlayer => prevPlayer.Id, prevPlayer => prevPlayer.Price);

        IReadOnlyList<PlayerPriceChange> risingPlayers = newData.Players
            .Where(player => playerPriceLookup.TryGetValue(player.Id, out decimal prevPlayerPrice) && player.Price > prevPlayerPrice)
            .Select(player =>
                (player, TeamsById[player.TeamId], playerPriceLookup[player.Id]).Adapt<PlayerPriceChange>())
            .ToList();

        IReadOnlyList<PlayerPriceChange> fallingPlayers = newData.Players
            .Where(player => playerPriceLookup.TryGetValue(player.Id, out decimal prevPlayerPrice) && player.Price < prevPlayerPrice)
            .Select(player =>
                (player, TeamsById[player.TeamId], playerPriceLookup[player.Id]).Adapt<PlayerPriceChange>())
            .ToList();

        return new(risingPlayers, fallingPlayers);
    }

    /// <summary>
    /// Get players where the status has changed compared to the last loaded dataset.
    /// </summary>
    /// <param name="newData">The dataset containing the new data.</param>
    /// <param name="prevData">The dataset containing the stored data.</param>
    private PlayerStatusChanges GetPlayerStatusChanges(FantasyBaseData newData, FantasyBaseData prevData)
    {
        Dictionary<int, Player> prevPlayersById = prevData.Players
            .Where(prevPlayer => prevPlayer.Id > 0)
            .ToDictionary(player => player.Id, player => player);

        IReadOnlyList<PlayerStatusChange> changedPlayers = newData.Players
            .Where(player =>
            {
                if (prevPlayersById.TryGetValue(player.Id, out Player? prevPlayer))
                {
                    if (!Equals(player.ChanceOfPlayingNextRound, prevPlayer.ChanceOfPlayingNextRound)) return true;
                    if (player.ChanceOfPlayingNextRound < 100 && !Equals(player.News, prevPlayer.News)) return true;
                }

                return false;
            })
            .Select(player => (player, TeamsById[player.TeamId]).Adapt<PlayerStatusChange>())
            .ToList();

        IReadOnlyList<PlayerStatusChange> availablePlayers = changedPlayers.Where(player => player.ChanceOfPlayingNextRound == 100).ToList();
        IReadOnlyList<PlayerStatusChange> doubtfulPlayers = changedPlayers.Where(player => player.ChanceOfPlayingNextRound.Between(0, 100)).ToList();
        IReadOnlyList<PlayerStatusChange> unavailablePlayers = changedPlayers.Where(player => player.ChanceOfPlayingNextRound == 0).ToList();

        return new(availablePlayers, doubtfulPlayers, unavailablePlayers);
    }

    /// <summary>
    /// Get players that didn't exist in the last loaded dataset.
    /// </summary>
    /// <param name="newData">The dataset containing the new data.</param>
    /// <param name="prevData">The dataset containing the stored data.</param>
    private IReadOnlyList<NewPlayer> GetNewPlayers(FantasyBaseData newData, FantasyBaseData prevData)
    {
        return newData.Players.Where(player => !prevData.Players.Any(prevPlayer => player.Id == prevPlayer.Id))
            .Select(player => (player, TeamsById[player.TeamId]).Adapt<NewPlayer>())
            .ToList();
    }

    /// <summary>
    /// Get players where the team has changed from the last loaded dataset.
    /// </summary>
    /// <param name="newData">The dataset containing the new data.</param>
    /// <param name="prevData">The dataset containing the stored data.</param>
    private IReadOnlyList<PlayerTransfer> GetTransferredPlayers(FantasyBaseData newData, FantasyBaseData prevData)
    {
        Dictionary<int, int> playerTeamLookup = prevData.Players
            .Where(prevPlayer => prevPlayer.Id > 0)
            .ToDictionary(player => player.Id, player => player.TeamId);

        return newData.Players
            .Where(player => playerTeamLookup.TryGetValue(player.Id, out int prevTeamId) && player.TeamId != prevTeamId)
            .Select(player => (player, TeamsById[player.TeamId], TeamsById[playerTeamLookup[player.Id]]).Adapt<PlayerTransfer>())
            .ToList();
    }
}