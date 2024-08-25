using Microsoft.Extensions.DependencyInjection;
using TFA.Application.Features.PredictedPriceChanges;
using TFA.Application.Interfaces.Services;

namespace TFA.Presentation.Presenters.PredictedPriceChanges;

public class PredictedPriceChangePresenter(
    [FromKeyedServices(PresenterKeys.Twitter)] IPresenter twitter,
    [FromKeyedServices(PresenterKeys.Discord)] IPresenter discord) : IPresenter<PredictedPriceChangeData>
{
    public Task Present(PredictedPriceChangeData data, CancellationToken cancellationToken = default)
        => Task.WhenAll([
                //twitter.Present(data, cancellationToken),
                discord.Present(data, cancellationToken)
            ]);
}
