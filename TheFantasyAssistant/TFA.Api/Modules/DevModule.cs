
using TFA.Infrastructure.Services;

namespace TFA.Api.Modules;

/// <summary>
/// Only to be used in development.
/// </summary>
public class DevModule : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        //app.MapPost("dataService",
        //[Tags(EndpointTags.Development)] 
        //async (
        //    IFotmobService dataService,
        //    CancellationToken cancellationToken) =>
        //{
        //    return Results.Ok();
        //});
    }
}
