using System.Diagnostics.CodeAnalysis;
using TFA.Application.Features.GameweekFinished;

namespace TFA.Application.Interfaces.Services;

public interface IBotService
{
    ValueTask<ErrorOr<IBotCommandResponse>> HandleCommand(
        [ConstantExpected] string command, 
        FantasyType fantasyType,
        IReadOnlyDictionary<string, string> options);
}

public interface IBotCommandResponse;

public sealed record BestFixturesCommandResponse(IReadOnlyList<GameweekSummaryTeam> Teams) : IBotCommandResponse;
