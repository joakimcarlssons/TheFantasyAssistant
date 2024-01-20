using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using TFA.Application.Interfaces.Services;
using TFA.Domain.Data;

namespace TFA.Presentation.Bots.Discord.ChoiceProviders;

public abstract class TeamChoiceProvider(FantasyType fantasyType) : ChoiceProvider
{
    public virtual async Task<IEnumerable<DiscordApplicationCommandOptionChoice>> ProvideTeams()
    {
        if (Services.GetService(typeof(IBaseDataService)) is IBaseDataService fantasyData
            && (await fantasyData.GetData(fantasyType) is { IsError: false } data))
        {
            return data.Value.Teams
                .Select(team => new DiscordApplicationCommandOptionChoice(team.Name, team.Id));
        }

        return [];
    }
}


public class FPLTeamChoiceProvider() : TeamChoiceProvider(FantasyType.FPL)
{
    public override Task<IEnumerable<DiscordApplicationCommandOptionChoice>> Provider() => ProvideTeams();
}

public class AllsvenskanTeamChoiceProvider() : TeamChoiceProvider(FantasyType.Allsvenskan)
{
    public override Task<IEnumerable<DiscordApplicationCommandOptionChoice>> Provider() => ProvideTeams();
}