using System.Text.Json.Serialization;

namespace TFA.Infrastructure.Dtos.Gameweek;

internal sealed record FantasyGameweekRequest(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("finished")] bool IsFinished,
    [property: JsonPropertyName("is_current")] bool IsCurrent,
    [property: JsonPropertyName("is_next")] bool IsNext,
    [property: JsonPropertyName("deadline_time")] DateTime Deadline,
    [property: JsonPropertyName("chip_plays")] IReadOnlyList<FantasyGameweekChipRequest> ChipsPlayed);

internal sealed record FantasyGameweekChipRequest(
    [property: JsonPropertyName("chip_name")] string? ChipName,
    [property: JsonPropertyName("num_played")] int NumberOfPlays);