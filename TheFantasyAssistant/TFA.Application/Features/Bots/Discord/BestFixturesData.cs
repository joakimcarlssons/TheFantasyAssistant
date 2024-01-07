using TFA.Application.Common.Data;

namespace TFA.Application.Features.Bots.Discord;

public sealed record DiscordCommandBestFixturesTeam(
    int TeamId,
    string Name,
    int Position,
    int TotalDifficulty,
    IReadOnlyList<DiscordCommandBestFixturesTeamOpponent> Opponents) : SummaryTeamToTarget(Opponents.Count, TotalDifficulty, Position);

public sealed record DiscordCommandBestFixturesTeamOpponent(
    int FixtureId,
    int Gameweek,
    string OpponentShortName,
    int FixtureDifficulty,
    bool IsHome) : SummaryTeamOpponent;
