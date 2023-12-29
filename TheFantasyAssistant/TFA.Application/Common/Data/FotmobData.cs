namespace TFA.Application.Common.Data;

public sealed record FotmobData(
    IReadOnlyList<FotmobPlayerDetails> PlayerData,
    IReadOnlyList<FixtureDetails> CurrentGameweekFixtureDetails);

/// <summary>
/// Needed as a class in order to set the properties by reflection.
/// </summary>
public sealed record FotmobPlayerDetails(int PlayerId, string PlayerName, string? TeamName)
{
    public double ExpectedGoals { get; set; }
    public double ExpectedGoalsPer90 { get; set; }
    public double ExpectedAssists { get; set; }
    public double ExpectedAssistsPer90 { get; set; }
    public double ShotsOnTargetPer90 { get; set; }
    public double ShotsPer90 { get; set; }
    public double ChancesCreated { get; set; }
    public double BigChancesCreated { get; set; }
    public double InterceptionsPer90 { get; set; }
    public double ClearancesPer90 { get; set; }
    public double BlocksPer90 { get; set; }
}
