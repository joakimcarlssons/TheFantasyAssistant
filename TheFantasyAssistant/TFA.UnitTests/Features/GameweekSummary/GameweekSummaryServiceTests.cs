using ErrorOr;
using Microsoft.Extensions.Logging;
using TFA.Application.Common.Keys;
using TFA.Application.Features.GameweekFinished;
using TFA.Application.Interfaces.Repositories;
using TFA.Application.Interfaces.Services;
using TFA.Infrastructure.Services;

namespace TFA.UnitTests.Features.GameweekSummary;

public class GameweekSummaryServiceTests : IClassFixture<MappingFixture>
{
    private readonly ILogger<GameweekSummaryService> MockLogger = Substitute.For<ILogger<GameweekSummaryService>>();
    private readonly IBaseDataService MockBaseDataService = Substitute.For<IBaseDataService>();
    private readonly IFotmobService MockFotmobService = Substitute.For<IFotmobService>();
    private readonly IGameweekDetailsService MockGameweekDetailsService = Substitute.For<IGameweekDetailsService>();
    private readonly IFirebaseRepository MockFirebaseRepository = Substitute.For<IFirebaseRepository>();

    private FantasyType FantasyType = FantasyType.Unknown;

    [Fact]
    public async Task GetData_FPL_ShouldReturnExpectedDataFromInput()
    {
        SetFantasyType(FantasyType.FPL);
        SetupMockFantasyData();
        SetupMockFotmobData();
        SetupMockGameweekDetailsData();
        SetupMockFirebaseRepository();

        GameweekSummaryService service = new(
            MockLogger,
            MockBaseDataService,
            MockFotmobService,
            MockGameweekDetailsService,
            MockFirebaseRepository);

        ErrorOr<GameweekSummaryData> result = await service.GetData(FantasyType);

        result.IsError.Should().BeFalse();

        // Top performing players
        result.Value.TopPerformingPlayers.Count.Should().Be(10);
        result.Value.TopPerformingPlayers[0].PlayerId.Should().Be(7);
        result.Value.TopPerformingPlayers[1].PlayerId.Should().Be(4);
        result.Value.TopPerformingPlayers[2].PlayerId.Should().Be(5);
        result.Value.TopPerformingPlayers[3].PlayerId.Should().Be(6);
        result.Value.TopPerformingPlayers[4].PlayerId.Should().Be(8); // Most goals
        result.Value.TopPerformingPlayers[5].PlayerId.Should().Be(9); // Most assists
        result.Value.TopPerformingPlayers[6].PlayerId.Should().Be(11); // Least minutes played
        result.Value.TopPerformingPlayers[7].PlayerId.Should().Be(10);
        result.Value.TopPerformingPlayers[8].PlayerId.Should().Be(1);
        result.Value.TopPerformingPlayers[9].PlayerId.Should().Be(2);

        // Teams with best upcoming fixtures
        result.Value.TeamsWithBestUpcomingFixtures.Count.Should().Be(5);
        result.Value.TeamsWithBestUpcomingFixtures[0].TeamId.Should().Be(6); // Best placing
        result.Value.TeamsWithBestUpcomingFixtures[1].TeamId.Should().Be(1);
        result.Value.TeamsWithBestUpcomingFixtures[2].TeamId.Should().Be(2);
        result.Value.TeamsWithBestUpcomingFixtures[3].TeamId.Should().Be(3);
        result.Value.TeamsWithBestUpcomingFixtures[4].TeamId.Should().Be(4);
    }

    private void SetFantasyType(FantasyType fantasyType)
        => FantasyType = fantasyType;

    private void SetupMockFirebaseRepository()
    {
        MockFirebaseRepository
            .Get<int>(FantasyType.GetDataKey(KeyType.LastCheckedDeadline))
            .Returns(1);

        MockFirebaseRepository
            .Get<int>(FantasyType.GetDataKey(KeyType.LastCheckedFinishedGameweek))
            .Returns(0);
    }

    private void SetupMockFotmobData()
    {
        MockFotmobService
            .GetFixtureDetailsForGameweek(Arg.Any<KeyedBaseData>(), Arg.Any<Gameweek>(), FantasyType)
            .Returns(new List<FixtureDetails>()
            {
                new FixtureDetailsBuilder(1)
                    .WithGameweek(1)
                    .WithHomeTeam(
                        new FixtureTeamDetailsBuilder(1)
                            .WithLineUp(new FixtureTeamDetailsLineUpBuilder()
                                .WithStartingPlayers(
                                [
                                    new FixtureTeamPlayerDetailsBuilder(1),
                                    new FixtureTeamPlayerDetailsBuilder(2),
                                    new FixtureTeamPlayerDetailsBuilder(3),
                                    new FixtureTeamPlayerDetailsBuilder(4),
                                    new FixtureTeamPlayerDetailsBuilder(5)
                                ])
                                .WithBenchPlayers([])))
                    .WithAwayTeam(
                        new FixtureTeamDetailsBuilder(2)
                            .WithLineUp(new FixtureTeamDetailsLineUpBuilder()
                                .WithStartingPlayers(
                                [
                                    new FixtureTeamPlayerDetailsBuilder(6),
                                    new FixtureTeamPlayerDetailsBuilder(7),
                                    new FixtureTeamPlayerDetailsBuilder(8),
                                    new FixtureTeamPlayerDetailsBuilder(9),
                                    new FixtureTeamPlayerDetailsBuilder(10),
                                    new FixtureTeamPlayerDetailsBuilder(11)
                                ])
                                .WithBenchPlayers([])))
            });
    }

    private void SetupMockGameweekDetailsData()
    {
        MockGameweekDetailsService
            .GetGameweekDetailsData(FantasyType, Arg.Any<int>())
            .Returns(new GameweekLiveBuilder()
                .WithPlayers(
                [
                    new GameweekLivePlayerBuilder(1)
                        .WithGameweekStats(new GameweekLivePlayerStatsBuilder()
                            .WithPoints(1)
                            .WithMinutesPlayed(45)),

                    new GameweekLivePlayerBuilder(2)
                        .WithGameweekStats(new GameweekLivePlayerStatsBuilder()
                            .WithPoints(1)
                            .WithMinutesPlayed(50)),

                    new GameweekLivePlayerBuilder(3)
                        .WithGameweekStats(new GameweekLivePlayerStatsBuilder()
                            .WithPoints(0)),

                    new GameweekLivePlayerBuilder(4)
                        .WithGameweekStats(new GameweekLivePlayerStatsBuilder()
                            .WithPoints(10)),

                    new GameweekLivePlayerBuilder(5)
                        .WithGameweekStats(new GameweekLivePlayerStatsBuilder()
                            .WithPoints(9)),

                    new GameweekLivePlayerBuilder(6)
                        .WithGameweekStats(new GameweekLivePlayerStatsBuilder()
                            .WithPoints(8)),

                    new GameweekLivePlayerBuilder(7)
                        .WithGameweekStats(new GameweekLivePlayerStatsBuilder()
                            .WithPoints(11)),

                    new GameweekLivePlayerBuilder(8)
                        .WithGameweekStats(new GameweekLivePlayerStatsBuilder()
                            .WithPoints(5)
                            .WithGoals(2)),

                    new GameweekLivePlayerBuilder(9)
                        .WithGameweekStats(new GameweekLivePlayerStatsBuilder()
                            .WithPoints(5)
                            .WithGoals(1)
                            .WithAssists(2)),

                    new GameweekLivePlayerBuilder(10)
                        .WithGameweekStats(new GameweekLivePlayerStatsBuilder()
                            .WithPoints(5)
                            .WithGoals(1)
                            .WithAssists(1)
                            .WithMinutesPlayed(90)),

                    new GameweekLivePlayerBuilder(11)
                        .WithGameweekStats(new GameweekLivePlayerStatsBuilder()
                            .WithPoints(5)
                            .WithGoals(1)
                            .WithAssists(1)
                            .WithMinutesPlayed(85)),
                ]));

        MockGameweekDetailsService
            .GetFixturesDetailsForPlayer(FantasyType, Arg.Any<int>(), Arg.Any<IReadOnlySet<int>>(), Arg.Any<CancellationToken>())
            .Returns([]);
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
                    .WithTeamId(1),
                
                new PlayerBuilder(2)
                    .WithTeamId(1),
                
                new PlayerBuilder(3)
                    .WithTeamId(1),
                
                new PlayerBuilder(4)
                    .WithTeamId(1),

                new PlayerBuilder(5)
                    .WithTeamId(1),

                new PlayerBuilder(6)
                    .WithTeamId(2),

                new PlayerBuilder(7)
                    .WithTeamId(2),

                new PlayerBuilder(8)
                    .WithTeamId(2),

                new PlayerBuilder(9)
                    .WithTeamId(2),

                new PlayerBuilder(10)
                    .WithTeamId(2),

                new PlayerBuilder(11)
                    .WithTeamId(2),
        ];

    private IReadOnlyList<Team> CreateFantasyTeams()
        => [
                new TeamBuilder(1)
                    .WithName("T1")
                    .WithPosition(2),
                
                new TeamBuilder(2)
                    .WithName("T2"),

                new TeamBuilder(3)
                    .WithName("T3"),

                new TeamBuilder(4)
                    .WithName("T4"),

                new TeamBuilder(5)
                    .WithName("T5"),

                new TeamBuilder(6)
                    .WithName("T6")
                    .WithPosition(1)
        ];

    private IReadOnlyList<Gameweek> CreateFantasyGameweeks()
        => [
                new GameweekBuilder(1)
                    .WithIsFinished()
           ];

    private IReadOnlyList<Fixture> CreateFantasyFixtures()
        => [
                new FixtureBuilder(1)
                    .WithGameweek(1)
                    .WithHomeTeam(1)
                    .WithAwayTeam(2),

                new FixtureBuilder(2)
                    .WithGameweek(2)
                    .WithHomeTeam(1)
                    .WithAwayTeam(3)
                    .WithHomeTeamDifficulty(2)
                    .WithAwayTeamDifficulty(3),

                new FixtureBuilder(3)
                    .WithGameweek(2)
                    .WithHomeTeam(2)
                    .WithAwayTeam(4)
                    .WithHomeTeamDifficulty(3)
                    .WithAwayTeamDifficulty(3),

                new FixtureBuilder(4)
                    .WithGameweek(2)
                    .WithHomeTeam(5)
                    .WithAwayTeam(6)
                    .WithHomeTeamDifficulty(3)
                    .WithAwayTeamDifficulty(2),
        ];
}
