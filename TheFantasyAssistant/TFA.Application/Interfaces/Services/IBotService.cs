using System.Diagnostics.CodeAnalysis;

namespace TFA.Application.Interfaces.Services;

public interface IBotService
{
    ValueTask<ErrorOr<IBotCommandResponse>> HandleCommand(
        [ConstantExpected] string command, 
        FantasyType fantasyType,
        IReadOnlyDictionary<string, string> options);
}

public interface IBotCommandResponse;

public sealed record BestFixturesCommandResponse(string Content) : IBotCommandResponse;
