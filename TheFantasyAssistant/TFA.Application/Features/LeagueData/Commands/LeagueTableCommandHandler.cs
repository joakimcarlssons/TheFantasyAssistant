using TFA.Application.Common.Keys;
using TFA.Application.Interfaces.Repositories;

namespace TFA.Application.Features.LeagueData.Commands;

public sealed class LeagueTableCommandHandler(
    IFirebaseRepository db) : IRequestHandler<LeagueTableCommand, ErrorOr<LeagueTableData>>
{
    public async Task<ErrorOr<LeagueTableData>> Handle(LeagueTableCommand data, CancellationToken cancellationToken)
    {
        if (data.Data.IsError)
            return data.Data;

        await db.Update(data.FantasyType.GetDataKey(KeyType.LeagueData), data.Data.Value, cancellationToken);
        return data.Data;
    }
}
