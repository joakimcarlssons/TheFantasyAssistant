using Microsoft.Extensions.DependencyInjection;
using TFA.Application.Features.BaseData.Events;
using TFA.Application.Interfaces.Services;

namespace TFA.Presentation.Presenters.BaseData;

public class BaseDataPresenter(
    [FromKeyedServices(PresenterKeys.Twitter)] IPresenter twitter,
    [FromKeyedServices(PresenterKeys.Discord)] IPresenter discord) : IPresenter<BaseDataPresentModel>
{
    public Task Present(BaseDataPresentModel data, CancellationToken cancellationToken)
        => Task.WhenAll([
            twitter.Present(data, cancellationToken),
            discord.Present(data, cancellationToken)
        ]);
}
