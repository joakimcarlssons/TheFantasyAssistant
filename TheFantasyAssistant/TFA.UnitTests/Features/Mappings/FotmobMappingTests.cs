using TFA.Infrastructure.Mapping;

namespace TFA.UnitTests.Features.Mappings;

public class FotmobMappingTests
{
    [Theory]
    [InlineData("Kennedy Bakircioglu", 1)]
    [InlineData("Nahír Bésârá", 2)]
    [InlineData("Gabriel", 4)]
    [InlineData("Gabriel Martinelli", 5)]
    [InlineData("Heung-Min Son", 6)]
    [InlineData("Vito Hammershøj-Mistrati", 7)]
    [InlineData("Lucas Paquetá", 8)]
    [InlineData("Emerson Royal", 9)]
    [InlineData("Bruno Guimaraes", 10)]
    [InlineData("Pascal Gross", 11)]
    public void GetFantasyPlayerFromFotmob_ReturnsCorrectPlayer(string fotmobPlayerName, int expectedPlayerId)
    {
        Player? player = CustomFotmobMapper.GetFantasyPlayerFromFotmob(fotmobPlayerName, GetPlayers(), GetTeam());
        player.Should()
            .NotBe(null)
            .And
            .Match(player => ((Player)player).Id == expectedPlayerId);
    }

    private static IReadOnlyList<Player> GetPlayers()
        => [
                new PlayerBuilder(1)
                    .WithTeamId(1)
                    .WithDisplayName("Kennedy"),

                new PlayerBuilder(2)
                    .WithTeamId(1)
                    .WithDisplayName("Besara"),

                new PlayerBuilder(3)
                    .WithTeamId(2)
                    .WithDisplayName("Besara")
                    .WithNews("Should not be picked due to being in another team"),

                new PlayerBuilder(4)
                    .WithTeamId(1)
                    .WithDisplayName("Gabriel")
                    .WithFullName("Gabriel dos Santos Magalhães"),

                new PlayerBuilder(5)
                    .WithTeamId(1)
                    .WithDisplayName("Martinelli")
                    .WithFullName("Gabriel Martinelli Silva"),

                new PlayerBuilder(6)
                    .WithTeamId(1)
                    .WithDisplayName("Son")
                    .WithFullName("Son Heung-min"),

                new PlayerBuilder(7)
                    .WithTeamId(1)
                    .WithFullName("Vito Hammershöy Mistrati"),

                new PlayerBuilder(8)
                    .WithTeamId(1)
                    .WithDisplayName("L.Paquetá"),

                new PlayerBuilder(9)
                    .WithTeamId(1)
                    .WithDisplayName("E.Royal"),

                new PlayerBuilder(10)
                    .WithTeamId(1)
                    .WithFullName("Bruno Guimarães Rodriguez Moura"),

                new PlayerBuilder(11)
                    .WithTeamId(1)
                    .WithFullName("Pascal Groß")
           ];

    private static Team GetTeam()
        => new TeamBuilder(1);
}
