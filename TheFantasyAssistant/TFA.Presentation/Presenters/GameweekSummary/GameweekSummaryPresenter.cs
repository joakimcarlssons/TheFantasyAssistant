using Microsoft.Extensions.DependencyInjection;
using TFA.Application.Features.GameweekFinished;
using TFA.Application.Interfaces.Services;

namespace TFA.Presentation.Presenters.GameweekSummary;

public class GameweekSummaryPresenter(
    [FromKeyedServices(PresenterKeys.Twitter)] IPresenter twitter,
    [FromKeyedServices(PresenterKeys.Discord)] IPresenter discord) : IPresenter<GameweekSummaryData>
{
    public Task Present(GameweekSummaryData data, CancellationToken cancellationToken = default)
        => Task.WhenAll([
                twitter.Present(data, cancellationToken),
                discord.Present(data, cancellationToken)
            ]);
}
