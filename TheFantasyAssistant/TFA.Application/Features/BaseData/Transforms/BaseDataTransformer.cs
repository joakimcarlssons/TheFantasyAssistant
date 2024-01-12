using Mapster;
using TFA.Application.Features.Transforms;
using TFA.Application.Services.BaseData;
using TFA.Utils;

namespace TFA.Application.Features.BaseData.Transforms;

internal record struct GameweekChanges(
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

        GameweekChanges gameweekChanges = GetGameweekFixtureChanges(newData, prevData);

        return new(priceChanges, statusChanges, newPlayers, transferredPlayers, gameweekChanges.DoubleGameweeks, gameweekChanges.BlankGameweeks);
    }

    private GameweekChanges GetGameweekFixtureChanges(FantasyBaseData newData, FantasyBaseData prevData)
    {
        Dictionary<int, Team> teamsById = newData.Teams.ToDictionary(team => team.Id);

        List<DoubleGameweek> doubleGameweeks = [];
        foreach (Team team in newData.Teams)
        {
            Dictionary<int, int> prevFixtureCountByGameweek = prevData.Fixtures
                .Where(fixture => 
                    fixture.GameweekId is not null 
                    && (fixture.HomeTeamId == team.Id || fixture.AwayTeamId == team.Id))
                .GroupBy(fixture => fixture.GameweekId!.Value)
                .ToDictionary(x => x.Key, x => x.ToList().Count); ;

             Dictionary<int, List<Fixture>> newFixturesByGameweek = newData.Fixtures
                .Where(fixture =>
                    fixture.GameweekId is not null
                    && (fixture.HomeTeamId == team.Id || fixture.AwayTeamId == team.Id))
                .GroupBy(fixture => fixture.GameweekId!.Value)
                .ToDictionary(x => x.Key, x => x.ToList());

            foreach ((int gameweek, List<Fixture> fixtures) in newFixturesByGameweek)
            {
                if (prevFixtureCountByGameweek.TryGetValue(gameweek, out int count) && fixtures.Count > count)
                {
                    IReadOnlyList<FixtureOpponent> opponents = fixtures.Select(x =>
                    {
                        bool isHome = x.HomeTeamId == team.Id;

                        int fixtureDifficulty = isHome
                            ? x.HomeTeamDifficulty 
                            : x.AwayTeamDifficulty;

                        Team opponentTeam = isHome
                            ? teamsById[x.AwayTeamId]
                            : teamsById[x.HomeTeamId];

                        return new FixtureOpponent(opponentTeam.Id, opponentTeam.ShortName, fixtureDifficulty, isHome);
                    }).ToList();

                    doubleGameweeks.Add(new DoubleGameweek(gameweek, team.Id, team.Name, team.ShortName, opponents));
                }
            }
        }

        return new GameweekChanges(doubleGameweeks, []);

        //Dictionary<int, ILookup<int, Fixture>> prevDataFixturesByTeamIdAndGameweek = prevData.Fixtures
        //    .GroupBy(x => x.HomeTeamId)
        //    .Concat(prevData.Fixtures.GroupBy(x => x.AwayTeamId))
        //    .ToDictionary(
        //        x =>  x.Key, 
        //        x => x
        //        .Where(f => f.GameweekId is not null)
        //        .ToLookup(f => f.GameweekId!.Value));

        //Dictionary<int, ILookup<int, Fixture>> newDataFixturesByTeamIdAndGameweek = prevData.Fixtures
        //    .GroupBy(x => x.HomeTeamId)
        //    .Concat(prevData.Fixtures.GroupBy(x => x.AwayTeamId))
        //    .ToDictionary(
        //        x => x.Key,
        //        x => x
        //        .Where(f => f.GameweekId is not null)
        //        .ToLookup(f => f.GameweekId!.Value));

        //Dictionary<int, Team> teamsById = newData.Teams.ToDictionary(team => team.Id);

        //IReadOnlyList<DoubleGameweek> doubleGameweeks = newDataFixturesByTeamIdAndGameweek
        //    .Select(x =>
        //    {
        //        ILookup<int, Fixture> prevFixturesByGameweek = prevDataFixturesByTeamIdAndGameweek[x.Key];
        //        var doubleGameweeks = x.Value.Where(f => f.ToArray().Length > prevFixturesByGameweek[f.Key].ToArray().Length)
        //            .SelectMany(d => );
        //    })
        //    .ToList();
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

    private IReadOnlyList<DoubleGameweek> GetDoubleGameweeks(FantasyBaseData newData, FantasyBaseData prevData)
    {


        return [];
    }

    private IReadOnlyList<BlankGameweek> GetBlankGameweeks(FantasyBaseData newData, FantasyBaseData prevData)
    {
        return [];
    }
}