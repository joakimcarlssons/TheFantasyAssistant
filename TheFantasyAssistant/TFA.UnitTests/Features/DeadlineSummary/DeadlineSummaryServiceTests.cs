using ErrorOr;
using Microsoft.Extensions.Logging;
using TFA.Application.Common.Data;
using TFA.Application.Common.Keys;
using TFA.Application.Features.Deadline;
using TFA.Application.Interfaces.Repositories;
using TFA.Infrastructure.Services;

namespace TFA.UnitTests.Features.DeadlineSummary;

public class DeadlineSummaryServiceTests : IClassFixture<MappingFixture>
{
    private readonly ILogger<DeadlineSummaryService> MockLogger = Substitute.For<ILogger<DeadlineSummaryService>>();
    private readonly IBaseDataService MockBaseDataService = Substitute.For<IBaseDataService>();
    private readonly IFotmobService MockFotmobService = Substitute.For<IFotmobService>();
    private readonly IFirebaseRepository MockFirebaseRepository = Substitute.For<IFirebaseRepository>();

    private FantasyType FantasyType = FantasyType.Unknown;

    [Fact]
    public async Task GetData_FPL_ShouldReturnExpectedDataFromInput()
    {
        SetFantasyType(FantasyType.FPL);
        SetupMockFantasyData();
        SetupMockFotmobData();
        SetupMockFirebaseRepository();

        DeadlineSummaryService service = new(
            MockLogger,
            MockBaseDataService,
            MockFotmobService,
            MockFirebaseRepository);
        
        ErrorOr<DeadlineSummaryData> result = await service.GetData(FantasyType);

        result.IsError.Should().BeFalse();

        // Players to target
        result.Value.PlayersToTarget.Count.Should().Be(5);
        result.Value.PlayersToTarget[0].PlayerId.Should().Be(2);
        result.Value.PlayersToTarget[1].PlayerId.Should().Be(3);
        result.Value.PlayersToTarget[2].PlayerId.Should().Be(4);
        result.Value.PlayersToTarget[3].PlayerId.Should().Be(5);
        result.Value.PlayersToTarget[4].PlayerId.Should().Be(6);

        // Players risking suspension
        result.Value.PlayersRiskingSuspension.Count.Should().Be(3);
        result.Value.PlayersRiskingSuspension[0].PlayerId.Should().Be(7);
        result.Value.PlayersRiskingSuspension[1].PlayerId.Should().Be(8);
        result.Value.PlayersRiskingSuspension[2].PlayerId.Should().Be(9);

        // Teams to target
        result.Value.TeamsToTarget.Count.Should().Be(4);
        result.Value.TeamsToTarget[0].TeamId.Should().Be(2); // Team 2 has a double gameweek and should be ranked first
        result.Value.TeamsToTarget[1].TeamId.Should().Be(7); // Difficulty 2 and better placing
        result.Value.TeamsToTarget[2].TeamId.Should().Be(1); // Difficulty 2 and worse placing
        result.Value.TeamsToTarget[3].TeamId.Should().Be(5); // Difficulty 3 and better placing

        // Teams with best upcoming fixtures
        // Team 6 and 7 have blank gameweeks and should not be counted here even if they have the lowest total difficulty
        result.Value.TeamsWithBestUpcomingFixtures.Count.Should().Be(3);
        result.Value.TeamsWithBestUpcomingFixtures[0].TeamId.Should().Be(1); // By difficulty
        result.Value.TeamsWithBestUpcomingFixtures[1].TeamId.Should().Be(4); // By difficulty
        result.Value.TeamsWithBestUpcomingFixtures[2].TeamId.Should().Be(2); // By placing
    }

    private void SetFantasyType(FantasyType fantasyType)
        => FantasyType = fantasyType;

    private void SetupMockFirebaseRepository()
    {
        MockFirebaseRepository
            .Get<int>(DataKeysHandler.GetDataKey(FantasyType, KeyType.LastCheckedDeadline))
            .Returns(0);
    }

    private void SetupMockFotmobData()
    {
        MockFotmobService
            .GetFotmobPlayerDetails(Arg.Any<KeyedBaseData>(), FantasyType, Arg.Any<CancellationToken>())
            .Returns(new List<FotmobPlayerDetails>()
            {
                new FotmobPlayerDetailsBuilder(2),
                new FotmobPlayerDetailsBuilder(3),
                new FotmobPlayerDetailsBuilder(4),
                new FotmobPlayerDetailsBuilder(5),
                new FotmobPlayerDetailsBuilder(6),
                new FotmobPlayerDetailsBuilder(7),
                new FotmobPlayerDetailsBuilder(8),
                new FotmobPlayerDetailsBuilder(9),
            });
    }

    private void SetupMockFantasyData()
    {
        IReadOnlyList<Player> players = CreateFantasyPlayers();
        IReadOnlyList<Team> teams = CreateFantasyTeams();
        IReadOnlyList<Gameweek> gameweeks = CreateFantasyGameweeks();
        IReadOnlyList<Fixture> fixtures = CreateFantasyFixtures();

        MockBaseDataService.GetKeyedData(FantasyType)
            .Returns(new ErrorOrBuilder<KeyedBaseData>()
                .WithResult(new KeyedBaseData(
                    players.ToDictionary(p => p.Id),
                    players.ToLookup(p => p.TeamId),
                    teams.ToDictionary(team => team.Id),
                    teams.ToDictionary(team => team.Name),
                    gameweeks.ToDictionary(gw => gw.Id),
                    fixtures.ToDictionary(fixture => fixture.Id),
                    fixtures.ToLookup(fixture => fixture.GameweekId ?? 0),
                    fixtures.ToLookup(fixture => fixture.HomeTeamId),
                    fixtures.ToLookup(fixture => fixture.AwayTeamId)))
                .BuildTaskResult());
    }

    private IReadOnlyList<Player> CreateFantasyPlayers()
        => [
                new PlayerBuilder(1)
                    .WithDisplayName("Highest expected points but not considered due to being goalkeeper.")
                    .WithStatus(PlayerStatuses.Available)
                    .WithChanceOfPlayingNextRound(100)
                    .WithPosition(PlayerPosition.Goalkeeper)
                    .WithExpectedPointsNextGameweek("100"),
                
                new PlayerBuilder(2)
                    .WithDisplayName("Highest expected points, should be top player to target.")
                    .WithStatus(PlayerStatuses.Available)
                    .WithChanceOfPlayingNextRound(100)
                    .WithPosition(PlayerPosition.Attacker)
                    .WithExpectedPointsNextGameweek("99")
                    .WithTeamId(1),

                new PlayerBuilder(3)
                    .WithDisplayName("Should be second player to target based on form.")
                    .WithStatus(PlayerStatuses.Available)
                    .WithChanceOfPlayingNextRound(100)
                    .WithPosition(PlayerPosition.Attacker)
                    .WithExpectedPointsNextGameweek("98")
                    .WithForm("10")
                    .WithTeamId(1),

                new PlayerBuilder(4)
                    .WithDisplayName("Should be third player to target based on form.")
                    .WithStatus(PlayerStatuses.Available)
                    .WithChanceOfPlayingNextRound(100)
                    .WithPosition(PlayerPosition.Attacker)
                    .WithExpectedPointsNextGameweek("98")
                    .WithForm("9")
                    .WithTeamId(1),

                new PlayerBuilder(5)
                    .WithDisplayName("Should be fourth player to target based on selected by.")
                    .WithStatus(PlayerStatuses.Available)
                    .WithChanceOfPlayingNextRound(100)
                    .WithPosition(PlayerPosition.Attacker)
                    .WithExpectedPointsNextGameweek("98")
                    .WithForm("9")
                    .WithSelectedByPercent("1")
                    .WithTeamId(1),

                new PlayerBuilder(6)
                    .WithDisplayName("Should be fifth player to target based on selected by.")
                    .WithStatus(PlayerStatuses.Available)
                    .WithChanceOfPlayingNextRound(100)
                    .WithPosition(PlayerPosition.Attacker)
                    .WithExpectedPointsNextGameweek("98")
                    .WithForm("9")
                    .WithSelectedByPercent("2")
                    .WithTeamId(1),

                new PlayerBuilder(7)
                    .WithDisplayName(
                        "Should not be considered as player to target because of chance to play." +
                        "Should be flagged as player risking suspension.")
                    .WithStatus(PlayerStatuses.Available)
                    .WithChanceOfPlayingNextRound(75)
                    .WithPosition(PlayerPosition.Attacker)
                    .WithExpectedPointsNextGameweek("101")
                    .WithYellowCards(4)
                    .WithTeamId(1),

                new PlayerBuilder(8)
                    .WithDisplayName(
                        "Should not be considered as player to target because of status." +
                        "Should be flagged as player risking suspension.")
                    .WithStatus(PlayerStatuses.Unavailable)
                    .WithChanceOfPlayingNextRound(100)
                    .WithPosition(PlayerPosition.Attacker)
                    .WithExpectedPointsNextGameweek("101")
                    .WithYellowCards(9)
                    .WithTeamId(2),

                new PlayerBuilder(9)
                    .WithDisplayName("Should be flagged as player risking suspension.")
                    .WithYellowCards(14)
                    .WithTeamId(3),
            ];

    private IReadOnlyList<Team> CreateFantasyTeams()
        => [
                new TeamBuilder(1)
                    .WithName("T1")
                    .WithShortName("T1")
                    .WithMatchesPlayed(18)
                    .WithPosition(7),
                
                new TeamBuilder(2)
                    .WithName("T2")
                    .WithShortName("T2")
                    .WithMatchesPlayed(31)
                    .WithPosition(3),
                
                new TeamBuilder(3)
                    .WithName("T3")
                    .WithShortName("T3")
                    .WithMatchesPlayed(37)
                    .WithPosition(6),

                new TeamBuilder(4)
                    .WithName("T4")
                    .WithShortName("T4")
                    .WithPosition(5),

                new TeamBuilder(5)
                    .WithName("T5")
                    .WithShortName("T5")
                    .WithPosition(4),

                new TeamBuilder(6)
                    .WithName("T6")
                    .WithShortName("T6")
                    .WithPosition(2),

                new TeamBuilder(7)
                    .WithName("T7")
                    .WithShortName("T7")
                    .WithPosition(1),
        ];

    private IReadOnlyList<Gameweek> CreateFantasyGameweeks()
        => [
                new GameweekBuilder(1)
                    .WithIsFinished(),
                new GameweekBuilder(2)
                    .WithIsNext()
                    .WithDeadline(DateTime.UtcNow.AddDays(-1)),
           ];

    private IReadOnlyList<Fixture> CreateFantasyFixtures()
        => [
            #region Gameweek 2
            new FixtureBuilder(1)
                    .WithGameweek(2)
                    .WithHomeTeam(1)
                    .WithAwayTeam(2)
                    .WithHomeTeamDifficulty(2)
                    .WithAwayTeamDifficulty(5),

            // Double gameweek for Team 2 in GW 2
            new FixtureBuilder(2)
                    .WithGameweek(2)
                    .WithHomeTeam(2)
                    .WithAwayTeam(3)
                    .WithHomeTeamDifficulty(2)
                    .WithAwayTeamDifficulty(3),

            new FixtureBuilder(3)
                    .WithGameweek(2)
                    .WithHomeTeam(4)
                    .WithAwayTeam(5)
                    .WithHomeTeamDifficulty(3)
                    .WithAwayTeamDifficulty(3),

            new FixtureBuilder(4)
                    .WithGameweek(2)
                    .WithHomeTeam(6)
                    .WithAwayTeam(7)
                    .WithHomeTeamDifficulty(4)
                    .WithAwayTeamDifficulty(2),
            #endregion

            #region Gameweek 3
            new FixtureBuilder(5)
                    .WithGameweek(3)
                    .WithHomeTeam(1)
                    .WithAwayTeam(3)
                    .WithHomeTeamDifficulty(2)
                    .WithAwayTeamDifficulty(3),

            new FixtureBuilder(6)
                    .WithGameweek(3)
                    .WithHomeTeam(2)
                    .WithAwayTeam(4)
                    .WithHomeTeamDifficulty(3)
                    .WithAwayTeamDifficulty(2),

            new FixtureBuilder(7)
                    .WithGameweek(3)
                    .WithHomeTeam(5)
                    .WithAwayTeam(6)
                    .WithHomeTeamDifficulty(4)
                    .WithAwayTeamDifficulty(2),
            #endregion

            #region Gameweek 4
            new FixtureBuilder(8)
                    .WithGameweek(4)
                    .WithHomeTeam(1)
                    .WithAwayTeam(4)
                    .WithHomeTeamDifficulty(2)
                    .WithAwayTeamDifficulty(3),

            new FixtureBuilder(9)
                    .WithGameweek(4)
                    .WithHomeTeam(2)
                    .WithAwayTeam(5)
                    .WithHomeTeamDifficulty(3)
                    .WithAwayTeamDifficulty(3),

            new FixtureBuilder(10)
                    .WithGameweek(4)
                    .WithHomeTeam(7)
                    .WithAwayTeam(3)
                    .WithHomeTeamDifficulty(2)
                    .WithAwayTeamDifficulty(3),
            #endregion
        ];

}
