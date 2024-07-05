using TFA.Slack;

namespace TFA.Api.Modules;

/// <summary>
/// Only to be used in development.
/// </summary>
public sealed class DevModule : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost("slack",
        [Tags(EndpointTags.Development)]
        async (
            ISlackService slackService,
            CancellationToken cancellationToken) =>
        {
            await slackService.SendWebhookMessageAsync("Test");
            return Results.Ok();
        });
    }
}
