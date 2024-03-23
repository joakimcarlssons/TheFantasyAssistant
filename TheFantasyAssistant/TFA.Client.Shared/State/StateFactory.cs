using Microsoft.Extensions.DependencyInjection;
using TFA.Domain.Data;

namespace TFA.Client.Shared.State;

public static class StateFactory
{
    private static bool Initialized = false;

    public static IServiceCollection AddStateManager(this IServiceCollection services)
    {
        services.AddScoped<IStateManager, StateManager>();
        return services;
    }

    public static void InitializeState(IStateManager stateManager)
    {
        if (!Initialized)
        {
            stateManager.TrySet(StateKey.FantasyType, FantasyType.FPL, false);
        }

        Initialized = true;
    }
}


public enum StateKey
{
    IsLoading,
    FantasyType,
    BaseData,
}
