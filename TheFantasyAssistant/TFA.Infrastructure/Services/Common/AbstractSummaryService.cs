using TFA.Application.Common.Data;
using TFA.Application.Features.Transforms;
using TFA.Domain.Models.Fixtures;

namespace TFA.Infrastructure.Services.Common;

public abstract class AbstractSummaryService<TData> : IDataService<ErrorOr<TData>>
    where TData : SummaryData
{
    public abstract Task<ErrorOr<TData>> GetData(FantasyType fantasyType, CancellationToken cancellationToken = default);

    protected static int GetFixtureDifficulty(bool isHome, Fixture fixture)
        => fixture.GetFixtureDifficulty(isHome);

    protected static string GetOpponentShortName(KeyedBaseData fantasyData, bool isHome, int homeTeamId, int awayTeamId)
        => FixtureTransforms.GetOpponentShortName(fantasyData, isHome, homeTeamId, awayTeamId);
}
