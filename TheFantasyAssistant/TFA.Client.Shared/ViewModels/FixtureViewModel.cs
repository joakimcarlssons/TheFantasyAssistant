namespace TFA.Client.Shared.ViewModels;

public sealed record FixtureViewModel(
    string Opponent,
    bool IsHome,
    int FixtureDifficulty);