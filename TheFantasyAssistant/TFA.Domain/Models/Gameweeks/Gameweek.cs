namespace TFA.Domain.Models.Gameweeks;

public sealed record Gameweek(
    [property: JsonPropertyName("id")] int Id,
    [property: JsonPropertyName("is_finished")] bool IsFinished,
    [property: JsonPropertyName("is_current")] bool IsCurrent,
    [property: JsonPropertyName("is_next")] bool IsNext,
    [property: JsonPropertyName("deadline_time")] DateTime Deadline,
    [property: JsonPropertyName("chip_plays")] IReadOnlyList<GameweekChip>? ChipsPlayed,
    [property: JsonPropertyName("is_reset")] bool IsReset
) : IEntity;

public sealed record GameweekChip(
    [property: JsonPropertyName("chip_name")] string? ChipName,
    [property: JsonPropertyName("number_of_plays")] int NumberOfPlays);