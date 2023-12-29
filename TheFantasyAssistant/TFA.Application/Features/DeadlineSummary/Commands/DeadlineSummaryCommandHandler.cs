using TFA.Application.Features.Deadline;
using TFA.Application.Features.Deadline.Commands;

namespace TFA.Application.Features.DeadlineSummary.Commands;

public sealed class DeadlineSummaryCommandHandler(IPublisher publisher) : IRequestHandler<DeadlineSummaryCommand, ErrorOr<DeadlineSummaryData>>
{
    public async Task<ErrorOr<DeadlineSummaryData>> Handle(DeadlineSummaryCommand request, CancellationToken cancellationToken)
    {
        if (request.Data.IsError)
            return request.Data;

        await publisher.Publish(request.Data.Value, cancellationToken);
        return request.Data;
    }
}
