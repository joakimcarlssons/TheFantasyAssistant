using Swashbuckle.AspNetCore.Annotations;
using TFA.Application.Features.FixtureLiveUpdate.Commands;

namespace TFA.Api.Modules;

public sealed class FixturesModule : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost(Endpoints.FPLFixturesLiveUpdate,
        [Tags(EndpointTags.Fixtures)]
        [SwaggerOperation(Summary = "Checks for changes in ongoing fixtures and sends data to subscribing services.")]
        [SwaggerResponse(StatusCodes.Status202Accepted, "Data was loaded and validated. Subscribing services has been notified.")]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Something went wrong processing the data.")] 
        async (
            ISender sender,
            ILogger<FixturesModule> logger,
            CancellationToken cancellationToken) => 
                (await sender.Send(new GameweekLiveUpdateCommand(FantasyType.FPL), cancellationToken))
                    .GetDataProcessingResult(logger));

        app.MapPost(Endpoints.FASFixturesLiveUpdate,
        [Tags(EndpointTags.Fixtures)]
        [SwaggerOperation(Summary = "Checks for changes in ongoing fixtures and sends data to subscribing services.")]
        [SwaggerResponse(StatusCodes.Status202Accepted, "Data was loaded and validated. Subscribing services has been notified.")]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Something went wrong processing the data.")]
        async (
            ISender sender,
            ILogger<FixturesModule> logger,
            CancellationToken cancellationToken) =>
                (await sender.Send(new GameweekLiveUpdateCommand(FantasyType.Allsvenskan), cancellationToken))
                    .GetDataProcessingResult(logger));
    }
}
