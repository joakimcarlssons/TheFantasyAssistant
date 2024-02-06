using System.Collections.Concurrent;

namespace TFA.Client.State;

public interface IStateManager
{
    event Action<string> StateChanged;
    T? TryGet<T>(string key, T? backupValue = default) where T : notnull;
    bool TrySet<T>(string key, T value, bool notifyChange = true) where T : notnull;
    bool TryRemove(string key);
    void Clear();
}

public class StateManager : IStateManager
{
    private readonly ConcurrentDictionary<string, object> State = new();

    public event Action<string>? StateChanged;
    private void NotifyStateChanged(string key) => StateChanged?.Invoke(key);

    public T? TryGet<T>(string key, T? backupValue = default) where T : notnull
    {
        if (State.TryGetValue(key, out object? storedValue))
        {
            if (storedValue is T value)
            {
                return value;
            }
        }

        if (backupValue != null)
        {
            TrySet(key, backupValue, false);
        }

        return backupValue;
    }

    public bool TrySet<T>(string key, T value, bool notifyChange) where T : notnull
    {
        lock (State)
        {
            if (State.TryGetValue(key, out object? storedValue))
            {
                if (storedValue is not T currentValue)
                {
                    //error = "Provided type does not match with stored type in state.";
                    return false;
                }

                if (State.TryUpdate(key, value, currentValue))
                {
                    if (notifyChange && !currentValue.Equals(value))
                    {
                        NotifyStateChanged(key);
                    }
                                
                    return true;
                }

                //error = $"Failed to update state for {key}";
                return false;
            }
            else if (State.TryAdd(key, value))
            {
                if (notifyChange)
                {
                    NotifyStateChanged(key);
                }
                
                return true;
            }

            //error = $"Failed to set value {value} for key {key}";
            return false;
        }
    }

    public bool TryRemove(string key)
    {
        lock (State)
        {
            return State.Remove(key, out _);
        }
    }

    public void Clear() => State.Clear();
}