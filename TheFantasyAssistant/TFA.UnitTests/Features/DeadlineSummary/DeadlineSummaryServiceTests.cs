using ErrorOr;
using Microsoft.Extensions.Logging;
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

    private readonly FantasyType FantasyType = FantasyType.FPL;

    [Fact]
    public async Task GetData_ShouldReturnExpectedDataFromInput()
    {
        SetupMockFantasyData();
        SetupMockFotmobData();
        SetupMockFirebaseRepository();

        DeadlineSummaryService service = new(
            MockLogger,
            MockBaseDataService,
            MockFotmobService,
            MockFirebaseRepository);
        
        ErrorOr<DeadlineSummaryData> result = await service.GetData(FantasyType);
    }

    private void SetupMockFirebaseRepository()
    {
        MockFirebaseRepository
            .Get<int>(DataKeysHandler.GetDataKey(FantasyType, KeyType.LastCheckedDeadline))
            .Returns(0);
    }

    private void SetupMockFotmobData()
    {

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
        => [];

    private IReadOnlyList<Team> CreateFantasyTeams()
        => [];

    private IReadOnlyList<Gameweek> CreateFantasyGameweeks()
        => [];
    private IReadOnlyList<Fixture> CreateFantasyFixtures()
        => [];

}
