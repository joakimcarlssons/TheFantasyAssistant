
namespace TFA.Application.Features.GameweekFinished.Commands;

public sealed class GameweekSummaryCommandHandler(IPublisher publisher) : IRequestHandler<GameweekSummaryCommand, ErrorOr<GameweekSummaryData>>
{
    public async Task<ErrorOr<GameweekSummaryData>> Handle(GameweekSummaryCommand request, CancellationToken cancellationToken)
    {
        if (request.Data.IsError)
            return request.Data;

        await publisher.Publish(request.Data.Value, cancellationToken);
        return request.Data;
    }
}
