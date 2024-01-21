
using Mapster;
using TFA.Application.Common.Keys;
using TFA.Application.Features.GameweekLiveUpdate.Events;
using TFA.Application.Interfaces.Repositories;

namespace TFA.Application.Features.FixtureLiveUpdate.Commands;

public sealed class GameweekLiveUpdateCommandHandler(
    IPublisher publisher,
    IFirebaseRepository db) : IRequestHandler<GameweekLiveUpdateCommand, ErrorOr<GameweekLiveUpdateData>>
{
    public async Task<ErrorOr<GameweekLiveUpdateData>> Handle(GameweekLiveUpdateCommand data, CancellationToken cancellationToken)
    {
        if (data.Data.IsError)
            return data.Data;

        await publisher.Publish(
            data.Data.Value.Adapt<GameweekLiveUpdatePresentModel>() with
            {
                FantasyType = data.FantasyType
            }, 
            cancellationToken);

        // Update all checked finished fixtures
        string dataKey = data.FantasyType.GetDataKey(KeyType.FinishedFixtures);
        IReadOnlyList<int> previouslyFinishedFixtures = await db.Get<IReadOnlyList<int>>(dataKey, cancellationToken);
        await db.Update(
            dataKey,
            previouslyFinishedFixtures.Union(data.Data.Value.FinishedFixtures.Select(f => f.FixtureId)),
            cancellationToken);

        return data.Data;
    }
}
