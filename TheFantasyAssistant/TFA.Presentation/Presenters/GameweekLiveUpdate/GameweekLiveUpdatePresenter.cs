using Microsoft.Extensions.DependencyInjection;
using TFA.Application.Features.GameweekLiveUpdate.Events;
using TFA.Application.Interfaces.Services;

namespace TFA.Presentation.Presenters.GameweekLiveUpdate;

public class GameweekLiveUpdatePresenter(
    //[FromKeyedServices(PresenterKeys.Discord)] IPresenter discord
    ) : IPresenter<GameweekLiveUpdatePresentModel>
{
    public Task Present(GameweekLiveUpdatePresentModel data, CancellationToken cancellationToken = default)
        => Task.WhenAll([]);
}
