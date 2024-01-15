using System.Diagnostics.CodeAnalysis;
using TFA.Application.Common.Commands;
using TFA.Application.Errors;
using TFA.Application.Features.Bots.Discord;
using TFA.Application.Features.Transforms;
using TFA.Domain.Models.Teams;

namespace TFA.Infrastructure.Services;

public sealed class DiscordBotService(IBaseDataService baseData) : IBotService
{
    public async ValueTask<ErrorOr<IBotCommandResponse>> HandleCommand(
        [ConstantExpected] string command,
        FantasyType fantasyType,
        IReadOnlyDictionary<string, string> options)
            => command switch
            {
                BotCommands.BestFixtures.Name => await GetBestFixtures(fantasyType, options),
                BotCommands.TeamFixtures.Name => await GetTeamFixtures(fantasyType, options),
                _ => Errors.Bot.CommandNotImplemented
            };

    private async Task<TeamFixturesCommandResponse> GetTeamFixtures(FantasyType fantasyType, IReadOnlyDictionary<string, string> options)
    {
        ErrorOr<KeyedBaseData> fantasyData = await baseData.GetKeyedData(fantasyType);
        if (!fantasyData.IsError
            && options.TryGetValue("fromGw", out string? fromGw)
            && options.TryGetValue("toGw", out string? toGw)
            && int.TryParse(fromGw, out int from)
            && int.TryParse(toGw, out int to)
            && options.TryGetValue("teamId", out string? teamId)
            && int.TryParse(teamId, out int id)
            && fantasyData.Value.TeamsById.TryGetValue(id, out Team? team))
        {
            // Todo: this is not efficient...
            int teamCount = fantasyData.Value.TeamsById.Values.ToArray().Length;
            IReadOnlyList<DiscordCommandBestFixturesTeam> result = TeamTransforms.GetTeamsOrderedByFixtureDifficulty(
                fantasyData.Value,
                from,
                to,
                teamCount,
                (data, team, fixture, isDouble) =>
                {
                    bool isHome = fixture.HomeTeamId == team.Id;
                    return new DiscordCommandBestFixturesTeamOpponent(
                        fixture.Id,
                        fixture.GameweekId ?? 1,
                        FixtureTransforms.GetOpponentShortName(data, isHome, fixture.HomeTeamId, fixture.AwayTeamId),
                        fixture.GetFixtureDifficulty(isHome),
                        isHome);
                },
                (team, opponents, blankGameweeks) =>
                {
                    return new DiscordCommandBestFixturesTeam(
                        team.Id,
                        team.Name,
                        team.Position ?? 99,
                        opponents.Sum(opp => opp.FixtureDifficulty) + Math.Max((blankGameweeks * 6), 0),
                        opponents.OrderBy(opp => opp.Gameweek).ToList());
                });

            return new(result.First(t => t.TeamId == team.Id));
        }

        // todo: fix, return ErrorOr maybe?
        throw new NotImplementedException();
    }

    private async Task<BestFixturesCommandResponse> GetBestFixtures(FantasyType fantasyType, IReadOnlyDictionary<string, string> options)
    {
        ErrorOr<KeyedBaseData> fantasyData = await baseData.GetKeyedData(fantasyType);
        if (!fantasyData.IsError
            && options.TryGetValue("fromGw", out string? fromGw)
            && options.TryGetValue("toGw", out string? toGw)
            && int.TryParse(fromGw, out int from)
            && int.TryParse(toGw, out int to))
        {
            IReadOnlyList<DiscordCommandBestFixturesTeam> result = TeamTransforms.GetTeamsOrderedByFixtureDifficulty(
                fantasyData.Value,
                from,
                to,
                5,
                (data, team, fixture, isDouble) =>
                {
                    bool isHome = fixture.HomeTeamId == team.Id;
                    return new DiscordCommandBestFixturesTeamOpponent(
                        fixture.Id,
                        fixture.GameweekId ?? 1,
                        FixtureTransforms.GetOpponentShortName(data, isHome, fixture.HomeTeamId, fixture.AwayTeamId),
                        fixture.GetFixtureDifficulty(isHome),
                        isHome);
                },
                (team, opponents, blankGameweeks) =>
                {
                    return new DiscordCommandBestFixturesTeam(
                        team.Id,
                        team.Name,
                        team.Position ?? 99,
                        opponents.Sum(opp => opp.FixtureDifficulty) + Math.Max((blankGameweeks * 6), 0),
                        opponents.OrderBy(opp => opp.Gameweek).ToList());
                });

            return new(result);
        }

        return new([]);
    }
}