using TFA.Domain.Data;

namespace TFA.Client.State;

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
            stateManager.TrySet(StateKeys.SelectedFantasyType, FantasyType.FPL, false);
        }

        Initialized = true;
    }
}

public sealed class StateKeys
{
    public const string IsLoading = nameof(IsLoading);
    public const string SelectedFantasyType = nameof(SelectedFantasyType);
}
