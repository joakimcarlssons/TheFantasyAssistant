
using Swashbuckle.AspNetCore.Annotations;
using TFA.Application.Features.LeagueData.Commands;

namespace TFA.Api.Modules;

public sealed class LeagueModule : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost(Endpoints.FPLLeagueTableRefresh,
        [Tags(EndpointTags.LeagueData)]
        [SwaggerOperation(Summary = "Refreshes the FPL teams with the current Premier League data.")]
        [SwaggerResponse(StatusCodes.Status202Accepted, "Request was received and processed.")]
        async (
            ISender sender,
            ILogger<LeagueModule> logger,
            CancellationToken cancellationToken) =>
                (await sender.Send(new LeagueTableCommand(FantasyType.FPL), cancellationToken))
                    .GetDataProcessingResult(logger));

        app.MapPost(Endpoints.FASLeagueTableRefresh,
        [Tags(EndpointTags.LeagueData)]
        [SwaggerOperation(Summary = "Refreshes the Allsvenskan teams with the current Allsvenskan data.")]
        [SwaggerResponse(StatusCodes.Status202Accepted, "Request was received and processed.")]
        async (
            ISender sender,
            ILogger<LeagueModule> logger,
            CancellationToken cancellationToken) =>
                (await sender.Send(new LeagueTableCommand(FantasyType.Allsvenskan), cancellationToken))
                    .GetDataProcessingResult(logger));
    }
}
