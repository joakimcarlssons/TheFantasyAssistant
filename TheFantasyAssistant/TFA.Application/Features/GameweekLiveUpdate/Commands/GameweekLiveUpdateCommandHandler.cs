
using Mapster;
using TFA.Application.Features.GameweekLiveUpdate.Events;

namespace TFA.Application.Features.FixtureLiveUpdate.Commands;

public sealed class GameweekLiveUpdateCommandHandler(IPublisher publisher) : IRequestHandler<GameweekLiveUpdateCommand, ErrorOr<GameweekLiveUpdateData>>
{
    public async Task<ErrorOr<GameweekLiveUpdateData>> Handle(GameweekLiveUpdateCommand request, CancellationToken cancellationToken)
    {
        if (request.Data.IsError)
            return request.Data;

        await publisher.Publish(
            request.Data.Value.Adapt<GameweekLiveUpdatePresentModel>(), 
            cancellationToken);
        return request.Data;
    }
}
