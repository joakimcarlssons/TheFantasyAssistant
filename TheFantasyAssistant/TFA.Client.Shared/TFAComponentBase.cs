using Microsoft.AspNetCore.Components;
using System.Diagnostics;
using TFA.Application.Interfaces.Services;
using TFA.Client.Shared.State;
using TFA.Domain.Data;
using TFA.Domain.Models;

namespace TFA.Client.Shared;

public abstract class TFAComponentBase : LayoutComponentBase
{
    [Inject]
    public IStateManager StateManager { get; set; } = null!;

    [Inject]
    public IBaseDataService BaseDataService { get; set; } = null!;

    private readonly HashSet<StateKey> StateSubscriptions = [];

    public FantasyBaseData? BaseData => StateManager?.TryGet<FantasyBaseData>(StateKey.BaseData);

    public bool IsLoading { get; set; } = true;

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        StateManager.StateChanged += async key =>
        {
            // Common handling of certain states
            await HandleStateChanged(key);

            // Only refresh component if they subscribe on state
            if (StateSubscriptions.Contains(key))
            {
                StateHasChanged();
            }
        };

        if (BaseData is null)
        {
            try
            {
                // Since this is the first load of BaseData we don't know the FantasyType
                await SetBaseData(FantasyType.Unknown);
            }
            finally
            {
                // Always stop loading after initial data load
                StopLoad();
            }
        }
    }

    public void RegisterStateSubscriptions(params StateKey[] states)
    {
        foreach (StateKey state in states)
        {
            StateSubscriptions.Add(state);
        }
    }

    private async Task HandleStateChanged(StateKey key)
    {
        if (key == StateKey.IsLoading)
        {
            IsLoading = StateManager.TryGet<bool>(StateKey.IsLoading);
        }
        else if (key == StateKey.FantasyType)
        {
            // Because the change of FantasyType requires a reload of data we show a loader
            StartLoad();

            // Pass the unknown FantasyType to make sure new data is loaded
            if (!(await SetBaseData(FantasyType.Unknown)))
            {
                // Set error
            }

            StopLoad();
        }
    }

    private async Task<bool> SetBaseData(FantasyType fantasyType)
    {
        // Check the time for the load, add small delay if loading is too fast
        Stopwatch sw = Stopwatch.StartNew();

        const int MinDelayTime = 500;
        bool result = false;

        // Get the state fantasy type since that will probably be the most correct one
        FantasyType stateFantasyType = StateManager.TryGet<FantasyType>(StateKey.FantasyType);

        // If the fantasy type is not changed and we have data stored in state, use it.
        if (stateFantasyType == fantasyType
            && StateManager.TryGet<FantasyBaseData>(StateKey.BaseData) is { } baseData)
        {
            StateManager.TrySet(StateKey.BaseData, baseData);
            result = true;
        }

        // If the provided state does not match the one in state it probably means we need to refresh the data.
        if (await BaseDataService.GetData(stateFantasyType) is { IsError: false } data)
        {
            StateManager.TrySet(StateKey.BaseData, data.Value);
            result = true;
        }

        if (sw.ElapsedMilliseconds < MinDelayTime)
        {
            await Task.Delay(MinDelayTime - (int)sw.ElapsedMilliseconds);
        }

        return result;
    }

    private void StartLoad() => StateManager.TrySet(StateKey.IsLoading, true);
    private void StopLoad() => StateManager.TrySet(StateKey.IsLoading, false);
}
