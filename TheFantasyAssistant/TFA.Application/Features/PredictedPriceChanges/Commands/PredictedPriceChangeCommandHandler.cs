
namespace TFA.Application.Features.PredictedPriceChanges.Commands;

public sealed class PredictedPriceChangeCommandHandler(IPublisher publisher) : IRequestHandler<PredictedPriceChangeCommand, ErrorOr<PredictedPriceChangeData>>
{
    public async Task<ErrorOr<PredictedPriceChangeData>> Handle(PredictedPriceChangeCommand request, CancellationToken cancellationToken)
    {
        if (request.Data.IsError)
            return request.Data;

        await publisher.Publish(request.Data.Value, cancellationToken);
        return request.Data;
    }
}
