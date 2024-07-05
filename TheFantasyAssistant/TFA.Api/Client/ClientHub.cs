using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using TFA.Application.Common.Keys;
using TFA.Application.Interfaces.Repositories;

namespace TFA.Api.Client;

public class ClientHub(IFirebaseRepository db) : Hub
{
    public async Task GetFantasyData()
    {
        FantasyBaseData data = await db.Get<FantasyBaseData>(DataKeys.FAS_BASE_DATA);
        await Clients.Caller.SendAsync(nameof(GetFantasyData), data);
    }
}
