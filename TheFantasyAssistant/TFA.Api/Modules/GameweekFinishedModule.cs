
using Swashbuckle.AspNetCore.Annotations;
using TFA.Application.Features.GameweekFinished.Commands;

namespace TFA.Api.Modules;

public class GameweekFinishedModule : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost(Endpoints.FPLGameweekFinished,
        [Tags(EndpointTags.Summaries)]
        [SwaggerOperation(Summary = "Checks if an FPL gameweek has finished and triggers subscribing services if true.")]
        [SwaggerResponse(StatusCodes.Status202Accepted, "Request was received and processed.")]
        async (
            ISender sender,
            ILogger<GameweekFinishedModule> logger,
            CancellationToken cancellationToken) =>
                (await sender.Send(new GameweekSummaryCommand(FantasyType.FPL), cancellationToken))
                    .GetDataProcessingResult(logger));

        app.MapPost(Endpoints.FASGameweekFinished,
        [Tags(EndpointTags.Summaries)]
        [SwaggerOperation(Summary = "Checks if an Allsvenskan gameweek has finished and triggers subscribing services if true.")]
        [SwaggerResponse(StatusCodes.Status202Accepted, "Request was received and processed.")]
        async (
            ISender sender,
            ILogger<GameweekFinishedModule> logger,
            CancellationToken cancellationToken) =>
                (await sender.Send(new GameweekSummaryCommand(FantasyType.Allsvenskan), cancellationToken))
                    .GetDataProcessingResult(logger));
    }
}
