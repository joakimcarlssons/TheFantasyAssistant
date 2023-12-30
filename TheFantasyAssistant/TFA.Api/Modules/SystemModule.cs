
namespace TFA.Api.Modules;

/// <summary>
/// Used for system endpoints such as keeping the server alive.
/// </summary>
public class SystemModule : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost(Endpoints.StayAlive,
        [Tags(EndpointTags.System)]  
        (ILogger<SystemModule> logger) =>
        {
            logger.LogInformation("Staying alive, staying alive...");
            return Results.Accepted();
        });
    }
}
