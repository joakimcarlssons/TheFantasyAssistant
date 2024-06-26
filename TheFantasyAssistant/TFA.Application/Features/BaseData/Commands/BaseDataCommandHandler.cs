﻿using Mapster;
using Microsoft.Extensions.Logging;
using TFA.Application.Common.Keys;
using TFA.Application.Features.BaseData.Events;
using TFA.Application.Features.BaseData.Transforms;
using TFA.Application.Features.LeagueData;
using TFA.Application.Features.Transforms;
using TFA.Application.Interfaces.Repositories;
using TFA.Domain.Models;

namespace TFA.Application.Features.BaseData.Commands;

public sealed class BaseDataCommandHandler(
    ILogger<BaseDataCommandHandler> logger, 
    IPublisher publisher, 
    IFirebaseRepository db) : IRequestHandler<BaseDataCommand, ErrorOr<FantasyBaseData>>
{
    public async Task<ErrorOr<FantasyBaseData>> Handle(BaseDataCommand data, CancellationToken cancellationToken)
    {
        if (data.Data.IsError)
            return data.Data;

        string dataKey = data.FantasyType.GetDataKey(KeyType.BaseData);
        FantasyBaseData baseData = data.Data.Value;

        try
        {
            FantasyBaseData storedData = await db.Get<FantasyBaseData>(dataKey, cancellationToken);

            // Apply league data, if any
            if (await db.Get<LeagueTableData>(data.FantasyType.GetDataKey(KeyType.LeagueData), cancellationToken) is { } leagueData
                && leagueData.FantasyType == data.FantasyType)
            {
                IReadOnlyDictionary<int, LeagueTableTeam> leagueTeamsById = leagueData
                    .Teams
                    .ToDictionary(team => team.TeamId);

                IReadOnlyList<Team> teamsWithLeagueData = data.Data.Value.Teams
                   .Where(team => leagueTeamsById.ContainsKey(team.Id))
                   .Select(team => (team, leagueTeamsById[team.Id]).Adapt<Team>())
                   .ToList();

                baseData = baseData with
                {
                    Teams = teamsWithLeagueData
                };
            }

            // Apply fixture difficulties if necessary
            if (data.FantasyType == FantasyType.Allsvenskan)
            {
                IReadOnlyDictionary<int, Team> teamsById = baseData.Teams.ToDictionary(team => team.Id);
                baseData = baseData with
                {
                    Fixtures = baseData.Fixtures
                        .Where(fixture => teamsById.ContainsKey(fixture.HomeTeamId) && teamsById.ContainsKey(fixture.AwayTeamId))
                        .Select(fixture =>
                        {
                            (int HomeTeamDifficulty, int AwayTeamDifficulty) = FixtureTransforms.CalculateFixtureDifficulty(teamsById[fixture.HomeTeamId], teamsById[fixture.AwayTeamId], fixture);
                            return fixture with
                            {
                                HomeTeamDifficulty = HomeTeamDifficulty,
                                AwayTeamDifficulty = AwayTeamDifficulty
                            };
                        })
                        .ToList()
                };
            }

            await publisher.Publish(
                new BaseDataPresentModel(
                    data.FantasyType, 
                    data.Data.Value.Transform<FantasyBaseData, TransformedBaseData>(storedData)),
                cancellationToken);
        }
        catch (Exception ex)
        {
            logger.LogError("{Exception}", ex.Message);
            throw;
        }
        finally
        {
            await db.Update(dataKey, baseData, cancellationToken);
        }

        return data.Data;
    }
}