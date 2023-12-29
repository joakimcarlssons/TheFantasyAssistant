using TFA.Application.Interfaces.Services;

namespace TFA.Application.Features.BaseData.Events;

public sealed class BaseDataEvent(IPresenter<BaseDataPresentModel> presenter) : INotificationHandler<BaseDataPresentModel>
{
    public Task Handle(BaseDataPresentModel data, CancellationToken cancellationToken)
        => presenter.Present(data, cancellationToken);
}
