using TFA.Application.Common.Keys;
using TFA.Application.Features.Deadline;
using TFA.Application.Interfaces.Repositories;
using TFA.Application.Interfaces.Services;

namespace TFA.Application.Features.DeadlineSummary.Events;

public sealed class DeadlineSummaryEvent(
    IFirebaseRepository db,
    IPresenter<DeadlineSummaryData> presenter) : INotificationHandler<DeadlineSummaryData>
{
    public Task Handle(DeadlineSummaryData data, CancellationToken cancellationToken)
    {
        string gameweekKey = data.FantasyType.GetDataKey(KeyType.LastCheckedDeadline);
        return Task.WhenAll([
            presenter.Present(data, cancellationToken),
            db.Update(gameweekKey, data.Gameweek.Id, cancellationToken)
        ]);
    }
}
