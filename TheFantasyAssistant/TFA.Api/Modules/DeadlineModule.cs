
using Swashbuckle.AspNetCore.Annotations;
using TFA.Application.Features.Deadline.Commands;

namespace TFA.Api.Modules;

public sealed class DeadlineModule : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost(Endpoints.FPLDeadline,
        [Tags(EndpointTags.Summaries)]
        [SwaggerOperation(Summary = "Checks if the FPL deadline is approaching and trigger subscribing services if true.")]
        [SwaggerResponse(StatusCodes.Status202Accepted, "Request was received and processed.")]
        async (
            ISender sender,
            ILogger<DeadlineModule> logger,
            CancellationToken cancellationToken) =>
                (await sender.Send(new DeadlineSummaryCommand(FantasyType.FPL), cancellationToken))
                    .GetDataProcessingResult(logger));

        app.MapPost(Endpoints.FASDeadline,
        [Tags(EndpointTags.Summaries)]
        [SwaggerOperation(Summary = "Checks if the Allsvenskan deadline is approaching and trigger subscribing services if true.")]
        [SwaggerResponse(StatusCodes.Status202Accepted, "Request was received and processed.")]
        async (
            ISender sender,
            ILogger<DeadlineModule> logger,
            CancellationToken cancellationToken) =>
                (await sender.Send(new DeadlineSummaryCommand(FantasyType.Allsvenskan), cancellationToken))
                    .GetDataProcessingResult(logger));
    }
}
