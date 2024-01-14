using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using TFA.Application.Interfaces.Services;
using TFA.Domain.Data;

namespace TFA.Presentation.Bots.Discord;

public sealed class TeamChoiceProvider : ChoiceProvider
{
    public override async Task<IEnumerable<DiscordApplicationCommandOptionChoice>> Provider()
    {
        if (Services.GetService(typeof(IBaseDataService)) is IBaseDataService fantasyData
            && (await fantasyData.GetData(FantasyType.FPL) is { IsError: false } fplData)
            && (await fantasyData.GetData(FantasyType.Allsvenskan) is { IsError: false } fasData))
        {
            return fplData.Value.Teams
                //.Concat(fasData.Value.Teams)
                .Select(team => new DiscordApplicationCommandOptionChoice(team.Name, team.Id));
        }

        return [];
    }
}
