﻿using System.Diagnostics.CodeAnalysis;
using TFA.Application.Common.Commands;
using TFA.Application.Errors;
using TFA.Application.Features.GameweekFinished;
using TFA.Application.Features.Transforms;

namespace TFA.Infrastructure.Services;

public sealed class DiscordBotService(
    IBaseDataService baseData) : IBotService
{
    public async ValueTask<ErrorOr<IBotCommandResponse>> HandleCommand(
        [ConstantExpected] string command, 
        FantasyType fantasyType, 
        IReadOnlyDictionary<string, string> options)
            => command switch
            {
                BotCommands.BestFixtures.Name => await GetBestFixtures(fantasyType, options),
                _ => Errors.Bot.CommandNotImplemented
            };

    private async Task<BestFixturesCommandResponse> GetBestFixtures(FantasyType fantasyType, IReadOnlyDictionary<string, string> options)
    {
        ErrorOr<KeyedBaseData> fantasyData = await baseData.GetKeyedData(fantasyType);
        if (fantasyData.IsError)
        {
            return new("Failed to get enough data.");
        }

        if (options.TryGetValue("fromGw", out string? fromGw)
            && options.TryGetValue("toGw", out string? toGw)
            && int.TryParse(fromGw, out int from)
            && int.TryParse(toGw, out int to))
        {
            IReadOnlyList<GameweekSummaryTeam> result = TeamTransforms.GetTeamsOrderedByFixtureDifficulty(
                fantasyData.Value,
                from,
                to,
                5,
                (data, team, fixture, isDouble) =>
                {
                    bool isHome = fixture.HomeTeamId == team.Id;
                    return new GameweekSummaryTeamOpponent(
                        fixture.Id,
                        fixture.GameweekId ?? 1,
                        FixtureTransforms.GetOpponentShortName(data, isHome, fixture.HomeTeamId, fixture.AwayTeamId),
                        fixture.GetFixtureDifficulty(isHome),
                        isHome);
                },
                (team, opponents, blankGameweeks) =>
                {
                    return new GameweekSummaryTeam(
                        team.Id,
                        team.Name,
                        team.ShortName,
                        team.Position ?? 99,
                        opponents.Sum(opp => opp.FixtureDifficulty) + Math.Max((blankGameweeks * 6), 0),
                        opponents.OrderBy(opp => opp.Gameweek).ToList());
                });

            // Todo: Should get a way to use the contentbuilder here
            return new(result[0].ShortName);
        }

        return new("Invalid data provided.");
    }
}