using Swashbuckle.AspNetCore.Annotations;
using TFA.Application.Features.BaseData.Commands;
using TFA.Application.Features.BaseData.Queries;
using TFA.Application.Common.Queries;
using Microsoft.AspNetCore.Mvc;

namespace TFA.Api.Modules;

public class BaseDataModule : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapPost(Endpoints.FPLBaseDataRefresh,
        [Tags(EndpointTags.BaseDataRefresh)]
        [SwaggerOperation(Summary = "Refreshes FPL Base Data and sends it on to subscribing services.")]
        [SwaggerResponse(StatusCodes.Status202Accepted, "Data was loaded and validated. Subscribing services has been notified.")]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Refreshed data could not be processed.")]
        async (
            ISender sender, 
            ILogger<BaseDataModule> logger, 
            CancellationToken cancellationToken) =>
                (await sender.Send(new BaseDataCommand(FantasyType.FPL), cancellationToken))
                    .GetDataProcessingResult(logger));

        app.MapPost(Endpoints.FASBaseDataRefresh,
        [Tags(EndpointTags.BaseDataRefresh)]
        [SwaggerOperation(Summary = "Refreshes FAS Base Data and sends it on to subscribing services.")]
        [SwaggerResponse(StatusCodes.Status202Accepted, "Data was loaded and validated. Subscribing services has been notified.")]
        [SwaggerResponse(StatusCodes.Status422UnprocessableEntity, "Refreshed data could not be processed.")]
        async (
            ISender sender, 
            ILogger<BaseDataModule> logger, 
            CancellationToken cancellationToken) =>
                (await sender.Send(new BaseDataCommand(FantasyType.Allsvenskan), cancellationToken))
                    .GetDataProcessingResult(logger));

        app.MapGet(Endpoints.FPLPlayers,
        [Tags(EndpointTags.FPLBaseData)]
        [SwaggerOperation(Summary = "Get the latest load of FPL players.")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IEnumerable<Player>))]
        async (
            [FromServices] ISender sender,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 25,
            [FromQuery] string name = "") =>
        {
            return Results.Ok(await sender.Send(
                GetPlayers(
                    FantasyType.FPL,
                    new PlayerFilter(page, pageSize, name))));
        });

        app.MapGet(Endpoints.FASPlayers,
        [Tags(EndpointTags.FASBaseData)]
        [SwaggerOperation(Summary = "Get the latest load of FAS players.")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IEnumerable<Player>))]
        async (
            [FromServices] ISender sender,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 25,
            [FromQuery] string name = "") =>
        {
            return Results.Ok(await sender.Send(
                GetPlayers(
                    FantasyType.Allsvenskan, 
                    new PlayerFilter(page, pageSize, name))));
        });

        app.MapGet(Endpoints.FPLTeams,
        [Tags(EndpointTags.FPLBaseData)]
        [SwaggerOperation(Summary = "Get the latest load of FPL teams.")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IEnumerable<Team>))]
        async (
            [FromServices] ISender sender,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 25,
            [FromQuery] string name = "") =>
        {
            return Results.Ok(await sender.Send(
                GetTeams(
                    FantasyType.FPL,
                    new TeamFilter(page, pageSize, name))));
        });

        app.MapGet(Endpoints.FASTeams,
        [Tags(EndpointTags.FASBaseData)]
        [SwaggerOperation(Summary = "Get the latest load of FAS teams.")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IEnumerable<Team>))]
        async (
            [FromServices] ISender sender,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 25,
            [FromQuery] string name = "") =>
        {
            return Results.Ok(await sender.Send(
                GetTeams(
                    FantasyType.Allsvenskan,
                    new TeamFilter(page, pageSize, name))));
        });

        app.MapGet(Endpoints.FPLGameweeks,
        [Tags(EndpointTags.FPLBaseData)]
        [SwaggerOperation(Summary = "Get the latest load of FPL gameweeks.")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IEnumerable<Gameweek>))]
        async (
            [FromServices] ISender sender,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 25) =>
        {
            return Results.Ok(await sender.Send(
                GetGameweeks(
                    FantasyType.FPL,
                    new GameweekFilter(page, pageSize))));
        });

        app.MapGet(Endpoints.FASGameweeks,
        [Tags(EndpointTags.FASBaseData)]
        [SwaggerOperation(Summary = "Get the latest load of FAS gameweeks.")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IEnumerable<Gameweek>))]
        async (
            [FromServices] ISender sender,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 25) =>
        {
            return Results.Ok(await sender.Send(
                GetGameweeks(
                    FantasyType.Allsvenskan,
                    new GameweekFilter(page, pageSize))));
        });

        app.MapGet(Endpoints.FPLFixtures,
        [Tags(EndpointTags.FPLBaseData)]
        [SwaggerOperation(Summary = "Get the latest load of FPL fixtures.")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IEnumerable<Fixture>))]
        async (
            [FromServices] ISender sender,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 25) =>
        {
            return Results.Ok(await sender.Send(
                GetFixtures(
                    FantasyType.FPL,
                    new FixtureFilter(page, pageSize))));
        });

        app.MapGet(Endpoints.FASFixtures,
        [Tags(EndpointTags.FASBaseData)]
        [SwaggerOperation(Summary = "Get the latest load of FAS fixtures.")]
        [SwaggerResponse(StatusCodes.Status200OK, Type = typeof(IEnumerable<Fixture>))]
        async (
            [FromServices] ISender sender,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 25) =>
        {
            return Results.Ok(await sender.Send(
                GetFixtures(
                    FantasyType.Allsvenskan,
                    new FixtureFilter(page, pageSize))));
        });
    }

    /// <summary>
    /// Common method to get a collection of filtered players.
    /// </summary>
    private static BaseDataQuery<IEntity> GetPlayers(FantasyType fantasyType, PlayerFilter filter)
        => new(new BaseDataQueryFilter<IEntity>(
                fantasyType,
                typeof(Player),
                (players) => players.ApplyFilters<Player>(p => p.Filter(filter))));

    /// <summary>
    /// Common method to get a collection of filtered teams.
    /// </summary>
    private static BaseDataQuery<IEntity> GetTeams(FantasyType fantasyType, TeamFilter filter)
        => new(new BaseDataQueryFilter<IEntity>(
                fantasyType,
                typeof(Team),
                (teams) => teams.ApplyFilters<Team>(t => t.Filter(filter))));

    /// <summary>
    /// Common method to get a collection of filtered gameweeks.
    /// </summary>
    private static BaseDataQuery<IEntity> GetGameweeks(FantasyType fantasyType, GameweekFilter filter)
        => new(new BaseDataQueryFilter<IEntity>(
                fantasyType,
                typeof(Gameweek),
                (gameweeks) => gameweeks.ApplyFilters<Gameweek>(gw => gw.Filter(filter))));

    /// <summary>
    /// Common method to get a collection of filtered fixtures.
    /// </summary>
    private static BaseDataQuery<IEntity> GetFixtures(FantasyType fantasyType, FixtureFilter filter)
        => new(new BaseDataQueryFilter<IEntity>(
                fantasyType,
                typeof(Fixture),
                (fixtures) => fixtures.ApplyFilters<Fixture>(f => f.Filter(filter))));
}
