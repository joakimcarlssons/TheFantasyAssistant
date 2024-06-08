using Mapster;
using TFA.Application.Features.Transforms;
using TFA.Domain.Models;
using TFA.Utils;

namespace TFA.Application.Features.BaseData.Transforms;

internal record struct GameweekFixtureChanges(
    IReadOnlyList<DoubleGameweek> DoubleGameweeks,
    IReadOnlyList<BlankGameweek> BlankGameweeks);

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
        GameweekFixtureChanges gameweekChanges = GetGameweekFixtureChanges(newData, prevData);

        return new(priceChanges, statusChanges, newPlayers, transferredPlayers, gameweekChanges.DoubleGameweeks, gameweekChanges.BlankGameweeks);
    }

    private GameweekFixtureChanges GetGameweekFixtureChanges(FantasyBaseData newData, FantasyBaseData prevData)
    {
        int currentGameweek = newData.Gameweeks.FirstOrDefault(gw => gw.IsCurrent)?.Id ?? 1;

        IReadOnlyList<DoubleGameweek> doubleGameweeks = newData.Teams
            .SelectMany(team => newData.Fixtures
                // Filter fixtures related to the current team with a valid Gameweek
                .Where(fixture => fixture.GameweekId is not null 
                    && fixture.GameweekId > currentGameweek
                    && (fixture.HomeTeamId == team.Id || fixture.AwayTeamId == team.Id))
                .GroupBy(fixture => fixture.GameweekId!.Value)
                // Create a group of fixtures for each gameweek
                .Select(x => new
                {
                    Gameweek = x.Key,
                    Fixtures = x.ToList(),
                    PreviousFixtureCount = prevData.Fixtures
                        .Count(f => f.GameweekId == x.Key && (f.HomeTeamId == team.Id || f.AwayTeamId == team.Id))
                })
                // Filter out the gameweeks where the fixture count is not greater than before
                .Where(x => x.Fixtures.Count > 1 && x.Fixtures.Count > x.PreviousFixtureCount)
                // For each gameweek, create a DoubleGameweek object
                .Select(x =>
                {
                    IReadOnlyList<FixtureOpponent> opponents = x.Fixtures
                        .Select(fixture =>
                        {
                            bool isHome = fixture.HomeTeamId == team.Id;

                            int fixtureDifficulty = isHome 
                                ? fixture.HomeTeamDifficulty 
                                : fixture.AwayTeamDifficulty;

                            Team opponentTeam = isHome 
                                ? TeamsById[fixture.AwayTeamId] 
                                : TeamsById[fixture.HomeTeamId];

                            return new FixtureOpponent(opponentTeam.Id, opponentTeam.ShortName, fixtureDifficulty, isHome);
                        }).ToList();

                    return new DoubleGameweek(x.Gameweek, team.Id, team.Name, team.ShortName, opponents);
                }))
            .ToList();

        IReadOnlyList<BlankGameweek> blankGameweeks = newData.Teams
            .SelectMany(team => prevData.Fixtures
                .Where(fixture => fixture.GameweekId is not null
                    && fixture.GameweekId > currentGameweek
                    && (fixture.HomeTeamId == team.Id || fixture.AwayTeamId == team.Id))
                .GroupBy(fixture => fixture.GameweekId!.Value)
                .Select(x => new 
                {
                    Gameweek = x.Key,
                    FixtureCount = x.ToArray().Length,
                    NewDataFixtureCount = newData.Fixtures
                        .Count(f => f.GameweekId == x.Key && (f.HomeTeamId == team.Id || f.AwayTeamId == team.Id))
                })
                .Where(x => x.FixtureCount > 0 && x.NewDataFixtureCount == 0)
                .Select(x => new BlankGameweek(x.Gameweek, team.Id, team.Name, team.ShortName)))
            .ToList();

        return new GameweekFixtureChanges(doubleGameweeks, blankGameweeks);
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