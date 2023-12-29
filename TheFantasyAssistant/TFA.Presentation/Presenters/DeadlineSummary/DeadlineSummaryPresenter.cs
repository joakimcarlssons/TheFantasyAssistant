using Microsoft.Extensions.DependencyInjection;
using TFA.Application.Features.Deadline;
using TFA.Application.Interfaces.Services;

namespace TFA.Presentation.Presenters.DeadlineSummary;

public class DeadlineSummaryPresenter(
    [FromKeyedServices(PresenterKeys.Twitter)] IPresenter twitter,
    [FromKeyedServices(PresenterKeys.Discord)] IPresenter discord) : IPresenter<DeadlineSummaryData>
{
    public Task Present(DeadlineSummaryData data, CancellationToken cancellationToken = default)
        => Task.WhenAll([
                twitter.Present(data, cancellationToken),
                discord.Present(data, cancellationToken)
            ]);
}
