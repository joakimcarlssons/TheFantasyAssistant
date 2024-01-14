
using Microsoft.Extensions.Logging;
using TFA.Application.Interfaces.Repositories;
using TFA.Application.Interfaces.Services;

namespace TFA.Application.Features.FixtureLiveUpdate.Commands;

public sealed class GameweekLiveUpdateCommandHandler(
    ILogger<GameweekLiveUpdateCommandHandler> logger,
    IPublisher publisher,
    IBaseDataService fantasyData,
    IFirebaseRepository db) : IRequestHandler<GameweekLiveUpdateCommand, ErrorOr<GameweekLiveUpdateData>>
{
    public Task<ErrorOr<GameweekLiveUpdateData>> Handle(GameweekLiveUpdateCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
