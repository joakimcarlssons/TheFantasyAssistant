using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using TFA.Application.Interfaces.Services;
using TFA.Domain.Data;

namespace TFA.Presentation.Bots.Discord;

public sealed class TeamChoiceProvider : ChoiceProvider
{
    //public Task<IEnumerable<DiscordApplicationCommandOptionChoice>> Provider()
    //{

    //    //if(Presentation.ServiceProvider?.BuildServiceProvider() is { } services
    //    //   && services.GetService<IBaseDataService>() is { } fantasyData)
    //    //{
    //    //if (await fantasyData.GetData(FantasyType.FPL) is { IsError: false } data)
    //    //{
    //    //    return data.Value.Teams
    //    //        .Select(team => new DiscordApplicationCommandOptionChoice(team.Name, team.Id));
    //    //}
    //    //}

    //    return Task.FromResult<IEnumerable<DiscordApplicationCommandOptionChoice>>([]);
    //}

    public override async Task<IEnumerable<DiscordApplicationCommandOptionChoice>> Provider()
    {
        if (Services.GetService(typeof(IBaseDataService)) is IBaseDataService fantasyData
            && (await fantasyData.GetData(FantasyType.FPL) is { IsError: false } data))
        {
            return data.Value.Teams
                .Select(team => new DiscordApplicationCommandOptionChoice(team.Name, team.Id));
        }

        return [];
    }
}
