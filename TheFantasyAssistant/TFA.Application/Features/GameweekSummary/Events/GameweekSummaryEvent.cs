using TFA.Application.Common.Keys;
using TFA.Application.Features.GameweekFinished;
using TFA.Application.Interfaces.Repositories;
using TFA.Application.Interfaces.Services;

namespace TFA.Application.Features.GameweekSummary.Events;

public sealed class GameweekSummaryEvent(
    IFirebaseRepository db,
    IPresenter<GameweekSummaryData> presenter) : INotificationHandler<GameweekSummaryData>
{
    public Task Handle(GameweekSummaryData data, CancellationToken cancellationToken)
    {
        string gameweekKey = data.FantasyType.GetDataKey(KeyType.LastCheckedFinishedGameweek);
        return Task.WhenAll([
            presenter.Present(data, cancellationToken),
            db.Update(gameweekKey, data.Gameweek.Id, cancellationToken)
        ]);
    }
}
