using TFA.Application.Interfaces.Services;

namespace TFA.Application.Features.PredictedPriceChanges.Events;

public sealed class PredictedPriceChangeEvent(IPresenter<PredictedPriceChangeData> presenter) : INotificationHandler<PredictedPriceChangeData>
{
    public Task Handle(PredictedPriceChangeData data, CancellationToken cancellationToken)
        => presenter.Present(data, cancellationToken);
}
