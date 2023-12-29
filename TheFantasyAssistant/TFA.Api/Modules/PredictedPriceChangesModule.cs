
using Swashbuckle.AspNetCore.Annotations;
using TFA.Application.Features.PredictedPriceChanges.Commands;

namespace TFA.Api.Modules;

public class PredictedPriceChangesModule : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost(Endpoints.FPLPredictedPriceChanges,
        [Tags(EndpointTags.PredictedPriceChanges)]
        [SwaggerOperation(Summary = "Fetches predicted FPL price changes and sends it to subscribing services.")]
        [SwaggerResponse(StatusCodes.Status202Accepted, "Data was fetched and sent to subscribing services.")]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Failed to process any data.")]
        async (
            ISender sender,
            ILogger<PredictedPriceChangesModule> logger,
            CancellationToken cancellationToken) =>
                (await sender.Send(new PredictedPriceChangeCommand(FantasyType.FPL), cancellationToken))
                    .GetDataProcessingResult(logger));
    }
}
