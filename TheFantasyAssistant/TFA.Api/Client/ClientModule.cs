﻿using TFA.Api.Modules;
using TFA.Application.Common.Keys;
using TFA.Application.Interfaces.Repositories;

namespace TFA.Api.Client;

public class ClientModule : IModule
{
    public void RegisterEndpoints(IEndpointRouteBuilder app)
    {
        app.MapGet("client/data", async (
            IFirebaseRepository db,
            CancellationToken cancellationToken) =>
        {
            return await db.Get<FantasyBaseData>(DataKeys.FAS_BASE_DATA, cancellationToken);
        });
    }
}
