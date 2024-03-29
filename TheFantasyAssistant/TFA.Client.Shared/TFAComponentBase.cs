using Microsoft.AspNetCore.Components;
using System.Diagnostics;
using TFA.Application.Interfaces.Services;
using TFA.Client.Shared.State;
using TFA.Domain.Data;
using TFA.Domain.Models;

namespace TFA.Client.Shared;

public abstract class TFAComponentBase(StateKey[] states) : LayoutComponentBase
{
    [Inject]
    public IStateManager StateManager { get; set; } = null!;

    [Inject]
    public IBaseDataService BaseDataService { get; set; } = null!;

    protected readonly IReadOnlySet<StateKey> StateSubscriptions = states
        // Always add a few states applicable to all components
        .Concat([StateKey.IsLoading])
        .ToHashSet();

    protected KeyedBaseData? BaseData => StateManager?.TryGet<KeyedBaseData>(StateKey.BaseData);
    protected FantasyType SelectedFantasyType
    {
        get => StateManager.TryGet(StateKey.FantasyType, FantasyType.FPL);
        set => StateManager.TrySet(StateKey.FantasyType, value);
    }

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

        // If the fantasy type is not changed and we have data stored in state, use it.
        if (SelectedFantasyType == fantasyType
            && StateManager.TryGet<KeyedBaseData>(StateKey.BaseData) is { } baseData)
        {
            StateManager.TrySet(StateKey.BaseData, baseData);
            result = true;
        }

        // If the provided state does not match the one in state it probably means we need to refresh the data.
        if (await BaseDataService.GetKeyedData(SelectedFantasyType) is { IsError: false } data)
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
