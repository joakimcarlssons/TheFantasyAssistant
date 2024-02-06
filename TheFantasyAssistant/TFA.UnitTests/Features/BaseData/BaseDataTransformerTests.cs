using TFA.Application.Features.BaseData.Transforms;

namespace TFA.UnitTests.Features.BaseData;

public class BaseDataTransformerTests : IClassFixture<MappingFixture>
{
    [Fact]
    public void Transform_NoChanges_ShouldReturnEmptyLists()
    {
        FantasyBaseData data = new FantasyBaseDataBuilder().Build();

        BaseDataTransformer transformer = new();
        TransformedBaseData? result = transformer.Transform(data, data);

        result.Should().NotBeNull();
        result.PlayerPriceChanges.RisingPlayers.Should().BeEmpty();
        result.PlayerPriceChanges.FallingPlayers.Should().BeEmpty();
        result.PlayerStatusChanges.AvailablePlayers.Should().BeEmpty();
        result.PlayerStatusChanges.DoubtfulPlayers.Should().BeEmpty();
        result.PlayerStatusChanges.UnavailablePlayers.Should().BeEmpty();
        result.NewPlayers.Should().BeEmpty();
        result.PlayerTransfers.Should().BeEmpty();
    }

    [Fact]
    public void Transform_WhenPlayerPricesAreChanging_PlayersAreAddedAsRisingAndFalling()
    {
        Player risingPlayer = new PlayerBuilder()
            .WithId(1)
            .WithTeamId(1)
            .WithPrice(1M)
            .Build();
        Player newRisingPlayer = risingPlayer with
        {
            Price = 1.1M
        };

        Player fallingPlayer = new PlayerBuilder()
            .WithId(2)
            .WithTeamId(1)
            .WithPrice(1M)
            .Build();
        Player newFallingPlayer = fallingPlayer with
        {
            Price = 0.9M
        };

        Team team = new TeamBuilder()
            .WithId(1)
            .WithShortName("TEST")
            .Build();

        FantasyBaseData prevData = new FantasyBaseDataBuilder()
            .WithPlayers(risingPlayer, fallingPlayer)
            .WithTeams(team)
            .Build();

        FantasyBaseData newData = new FantasyBaseDataBuilder()
            .WithPlayers(newRisingPlayer, newFallingPlayer)
            .WithTeams(team)
            .Build();

        BaseDataTransformer transformer = new();
        TransformedBaseData? result = transformer.Transform(newData, prevData);

        result.Should().NotBeNull();
        result.PlayerPriceChanges.RisingPlayers.Should().HaveCount(1);
        result.PlayerPriceChanges.RisingPlayers[0].PlayerId.Should().Be(1);
        result.PlayerPriceChanges.FallingPlayers.Should().HaveCount(1);
        result.PlayerPriceChanges.FallingPlayers[0].PlayerId.Should().Be(2);
    }

    [Fact]
    public void Transform_WhenPlayersStatusIsChangedToZero_PlayerIsSetAsUnavailable()
    {
        Player hundredToZeroPlayer = new PlayerBuilder()
            .WithId(1)
            .WithTeamId(1)
            .WithChanceOfPlayingNextRound(100)
            .Build();
        Player newHundredToZeroPlayer = hundredToZeroPlayer with
        {
            ChanceOfPlayingNextRound = 0
        };

        Player fiftyToZeroPlayer = new PlayerBuilder()
            .WithId(2)
            .WithTeamId(1)
            .WithChanceOfPlayingNextRound(50)
            .Build();
        Player newFiftyToZeroPlayer = fiftyToZeroPlayer with
        {
            ChanceOfPlayingNextRound = 0
        };

        Team team = new TeamBuilder()
            .WithId(1)
            .WithShortName("TEST")
            .Build();

        FantasyBaseData prevData = new FantasyBaseDataBuilder()
            .WithPlayers(hundredToZeroPlayer, fiftyToZeroPlayer)
            .WithTeams(team)
            .Build();

        FantasyBaseData newData = new FantasyBaseDataBuilder()
            .WithPlayers(newHundredToZeroPlayer, newFiftyToZeroPlayer)
            .WithTeams(team)
            .Build();

        BaseDataTransformer transformer = new();
        TransformedBaseData? result = transformer.Transform(newData, prevData);

        result.Should().NotBeNull();
        result.PlayerStatusChanges.UnavailablePlayers.Should().HaveCount(2);
    }

    [Fact]
    public void Transform_WhenPlayersStatusIsChangedBetweenZeroAndHundred_PlayerIsSetAsDoubtful()
    {
        Player hundredToFiftyPlayer = new PlayerBuilder()
                    .WithId(1)
                    .WithTeamId(1)
                    .WithChanceOfPlayingNextRound(100)
                    .Build();
        Player newHundredToFiftyPlayer = hundredToFiftyPlayer with
        {
            ChanceOfPlayingNextRound = 50
        };

        Player fiftyToTwentyFivePlayer = new PlayerBuilder()
            .WithId(2)
            .WithTeamId(1)
            .WithChanceOfPlayingNextRound(50)
            .Build();
        Player newFiftyToTwentyFivePlayer = fiftyToTwentyFivePlayer with
        {
            ChanceOfPlayingNextRound = 25
        };

        Player zeroToFiftyPlayer = new PlayerBuilder()
            .WithId(3)
            .WithTeamId(1)
            .WithChanceOfPlayingNextRound(0)
            .Build();
        Player newZeroToFiftyPlayer = zeroToFiftyPlayer with
        {
            ChanceOfPlayingNextRound = 50
        };

        Team team = new TeamBuilder()
            .WithId(1)
            .WithShortName("TEST")
            .Build();

        FantasyBaseData prevData = new FantasyBaseDataBuilder()
            .WithPlayers(hundredToFiftyPlayer, fiftyToTwentyFivePlayer, zeroToFiftyPlayer)
            .WithTeams(team)
            .Build();

        FantasyBaseData newData = new FantasyBaseDataBuilder()
            .WithPlayers(newHundredToFiftyPlayer, newFiftyToTwentyFivePlayer, newZeroToFiftyPlayer)
            .WithTeams(team)
            .Build();

        BaseDataTransformer transformer = new();
        TransformedBaseData? result = transformer.Transform(newData, prevData);

        result.Should().NotBeNull();
        result.PlayerStatusChanges.DoubtfulPlayers.Should().HaveCount(3);
    }

    [Fact]
    public void Transform_WhenPlayersStatusIsChangedToHundred_PlayerIsSetAsAvailable()
    {
        Player zeroToHundredPlayer = new PlayerBuilder()
            .WithId(1)
            .WithTeamId(1)
            .WithChanceOfPlayingNextRound(0)
            .Build();
        Player newZeroToHundredPlayer = zeroToHundredPlayer with
        {
            ChanceOfPlayingNextRound = 100
        };

        Player fiftyToHundredPlayer = new PlayerBuilder()
            .WithId(2)
            .WithTeamId(1)
            .WithChanceOfPlayingNextRound(50)
            .Build();
        Player newFiftyToHundredPlayer = fiftyToHundredPlayer with
        {
            ChanceOfPlayingNextRound = 100
        };

        Team team = new TeamBuilder()
            .WithId(1)
            .WithShortName("TEST")
            .Build();

        FantasyBaseData prevData = new FantasyBaseDataBuilder()
            .WithPlayers(zeroToHundredPlayer, fiftyToHundredPlayer)
            .WithTeams(team)
            .Build();

        FantasyBaseData newData = new FantasyBaseDataBuilder()
            .WithPlayers(newZeroToHundredPlayer, newFiftyToHundredPlayer)
            .WithTeams(team)
            .Build();

        BaseDataTransformer transformer = new();
        TransformedBaseData? result = transformer.Transform(newData, prevData);

        result.Should().NotBeNull();
        result.PlayerStatusChanges.AvailablePlayers.Should().HaveCount(2);
    }

    [Fact]
    public void Transform_WhenPlayerIsChangingNewsAndPlayerIsNotAvailable_PlayerIsTransformedToCorrectStatus()
    {
        Player doubtfulPlayer = new PlayerBuilder()
            .WithId(1)
            .WithTeamId(1)
            .WithChanceOfPlayingNextRound(75)
            .Build();
        Player newDoubtfulPlayer = doubtfulPlayer with
        {
            News = "Some news"
        };

        Player unavailablePlayer = new PlayerBuilder()
            .WithId(2)
            .WithTeamId(1)
            .WithChanceOfPlayingNextRound(0)
            .Build();
        Player newUnavailablePlayer = unavailablePlayer with
        {
            News = "Some news"
        };

        Player availablePlayer = new PlayerBuilder()
            .WithId(3)
            .WithTeamId(1)
            .WithChanceOfPlayingNextRound(100)
            .Build();
        Player newAvailablePlayer = availablePlayer with
        {
            News = "Some news"
        };

        Team team = new TeamBuilder()
            .WithId(1)
            .WithShortName("TEST")
            .Build();

        FantasyBaseData prevData = new FantasyBaseDataBuilder()
            .WithPlayers(doubtfulPlayer, unavailablePlayer, availablePlayer)
            .WithTeams(team)
            .Build();

        FantasyBaseData newData = new FantasyBaseDataBuilder()
            .WithPlayers(newDoubtfulPlayer, newUnavailablePlayer, newAvailablePlayer)
            .WithTeams(team)
            .Build();

        BaseDataTransformer transformer = new();
        TransformedBaseData? result = transformer.Transform(newData, prevData);

        result.Should().NotBeNull();
        result.PlayerStatusChanges.AvailablePlayers.Should().BeEmpty();
        result.PlayerStatusChanges.DoubtfulPlayers.Should().HaveCount(1);
        result.PlayerStatusChanges.UnavailablePlayers.Should().HaveCount(1);
    }

    [Fact]
    public void Transform_WhenPlayerIsAdded_PlayerIsTransformedToNewPlayer_AndIsNotAddedToOtherTransforms()
    {
        Player player = new PlayerBuilder()
            .WithId(1)
            .WithTeamId(1)
            .WithChanceOfPlayingNextRound(100)
            .Build();

        Team team = new TeamBuilder()
            .WithId(1)
            .WithShortName("TEST")
            .Build();

        FantasyBaseData prevData = new FantasyBaseDataBuilder()
            .WithPlayers()
            .WithTeams(team)
            .Build();

        FantasyBaseData newData = new FantasyBaseDataBuilder()
            .WithPlayers(player)
            .WithTeams(team)
            .Build();

        BaseDataTransformer transformer = new();
        TransformedBaseData? result = transformer.Transform(newData, prevData);

        result.Should().NotBeNull();
        result.NewPlayers.Should().HaveCount(1);
        result.NewPlayers[0].PlayerId.Should().Be(1);

        result.PlayerPriceChanges.RisingPlayers.Should().BeEmpty();
        result.PlayerStatusChanges.AvailablePlayers.Should().BeEmpty();
        result.PlayerTransfers.Should().BeEmpty();
    }

    [Fact]
    public void Transform_WhenPlayerIsChangingTeam_PlayerIsTransformedToTransferredPlayer()
    {
        Player player = new PlayerBuilder()
            .WithId(1)
            .WithTeamId(1)
            .WithChanceOfPlayingNextRound(100)
            .Build();
        Player transferredPlayer = player with
        {
            TeamId = 2
        };

        Team team = new TeamBuilder()
            .WithId(1)
            .WithShortName("TEST")
            .Build();

        Team newTeam = new TeamBuilder()
            .WithId(2)
            .WithShortName("TEST2")
            .Build();

        FantasyBaseData prevData = new FantasyBaseDataBuilder()
            .WithPlayers(player)
            .WithTeams(team, newTeam)
            .Build();

        FantasyBaseData newData = new FantasyBaseDataBuilder()
            .WithPlayers(transferredPlayer)
            .WithTeams(team, newTeam)
            .Build();

        BaseDataTransformer transformer = new();
        TransformedBaseData? result = transformer.Transform(newData, prevData);

        result.Should().NotBeNull();
        result.PlayerTransfers.Should().HaveCount(1);
        result.PlayerTransfers[0].PlayerId.Should().Be(1);
        result.PlayerTransfers[0].PrevTeamId.Should().Be(1);
        result.PlayerTransfers[0].NewTeamId.Should().Be(2);
    }

    [Fact]
    public void Transform_WhenDoubleGameweeksAreAnnounced_FixturesAreTransformedCorrectly()
    {
        Team team = new TeamBuilder(1)
            .WithName("Team 1")
            .WithShortName("T1");

        Team opponent1 = new TeamBuilder(2)
            .WithName("Opponent 1")
            .WithShortName("OP1");

        Team opponent2 = new TeamBuilder(3)
            .WithName("Opponent 2")
            .WithShortName("OP2");

        Fixture fixture1 = new FixtureBuilder(1)
            .WithGameweek(1)
            .WithHomeTeam(1)
            .WithHomeTeamDifficulty(3)
            .WithAwayTeam(2)
            .WithAwayTeamDifficulty(2);

        Fixture fixture2 = new FixtureBuilder(2)
            .WithGameweek(1)
            .WithHomeTeam(3)
            .WithHomeTeamDifficulty(3)
            .WithAwayTeam(1)
            .WithAwayTeamDifficulty(2);

        FantasyBaseData prevData = new FantasyBaseDataBuilder()
            .WithTeams(team, opponent1, opponent2)
            .WithFixtures(fixture1);

        FantasyBaseData newData = new FantasyBaseDataBuilder()
            .WithTeams(team, opponent1, opponent2)
            .WithFixtures(fixture1, fixture2);

        BaseDataTransformer transformer = new();
        TransformedBaseData? result = transformer.Transform(newData, prevData);

        result.Should().NotBeNull();
        result.DoubleGameweeks.Should().NotBeEmpty().And.HaveCount(1);
        result.DoubleGameweeks[0].TeamId.Should().Be(1);
        result.DoubleGameweeks[0].Opponents.Count.Should().Be(2);
        result.DoubleGameweeks[0].Opponents[0].TeamId.Should().Be(2);
        result.DoubleGameweeks[0].Opponents[0].FixtureDifficulty.Should().Be(3);
        result.DoubleGameweeks[0].Opponents[1].TeamId.Should().Be(3);
        result.DoubleGameweeks[0].Opponents[1].FixtureDifficulty.Should().Be(2);
    }

    [Fact]
    public void Transform_WhenBlankGameweeksAreAnnounces_FixturesAreTransformedCorrectly()
    {
        Team team = new TeamBuilder(1)
            .WithName("Team 1")
            .WithShortName("T1");

        Team team2 = new TeamBuilder(2)
            .WithName("Team 2")
            .WithShortName("T2");

        Team opponent = new TeamBuilder(3)
            .WithName("Opponent 1")
            .WithShortName("OP1");

        Fixture fixture1 = new FixtureBuilder(1)
            .WithGameweek(1)
            .WithHomeTeam(1)
            .WithHomeTeamDifficulty(3)
            .WithAwayTeam(3)
            .WithAwayTeamDifficulty(2);

        Fixture fixture2 = new FixtureBuilder(1)
            .WithGameweek(1)
            .WithHomeTeam(2)
            .WithHomeTeamDifficulty(4)
            .WithAwayTeam(3)
            .WithAwayTeamDifficulty(4);


        FantasyBaseData prevData = new FantasyBaseDataBuilder()
            .WithTeams(team, team2, opponent)
            .WithFixtures(fixture1);

        FantasyBaseData newData = new FantasyBaseDataBuilder()
            .WithTeams(team, team2, opponent)
            .WithFixtures(fixture2);

        BaseDataTransformer transformer = new();
        TransformedBaseData? result = transformer.Transform(newData, prevData);

        result.Should().NotBeNull();
        result.BlankGameweeks.Should().NotBeEmpty().And.HaveCount(1);
        result.BlankGameweeks[0].TeamId.Should().Be(1);
    }
}

