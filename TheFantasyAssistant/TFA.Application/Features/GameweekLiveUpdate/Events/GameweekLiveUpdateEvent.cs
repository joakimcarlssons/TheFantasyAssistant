using TFA.Application.Interfaces.Services;

namespace TFA.Application.Features.GameweekLiveUpdate.Events;

public sealed class GameweekLiveFixtureFinishedEvent(IPresenter<GameweekLiveUpdatePresentModel> presenter) : INotificationHandler<GameweekLiveUpdatePresentModel>
{
    public Task Handle(GameweekLiveUpdatePresentModel data, CancellationToken cancellationToken)
        => presenter.Present(data, cancellationToken);
}
