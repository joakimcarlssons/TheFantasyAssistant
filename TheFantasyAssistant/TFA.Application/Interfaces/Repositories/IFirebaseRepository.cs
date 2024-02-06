namespace TFA.Application.Interfaces.Repositories;

public interface IReadOnlyFirebaseRepository
{
    /// <summary>
    /// Get data stored in Firebase by its data key.
    /// </summary>
    /// <typeparam name="TData">The expected type of the stored data.</typeparam>
    /// <param name="key">The identifier of the data in the database.</param>
    ValueTask<TData> Get<TData>(string key, CancellationToken cancellationToken = default);
}

public interface IFirebaseRepository : IReadOnlyFirebaseRepository
{
    /// <summary>
    /// Adds new data to Firebase with a provided data key.
    /// </summary>
    /// <typeparam name="TData">The type of the data to store.</typeparam>
    /// <param name="key">The identifier of the data in the database.</param>
    /// <param name="data">The actual data to be stored.</param>
    Task Add<TData>(string key, TData data, CancellationToken cancellationToken = default);

    /// <summary>
    /// Completely removes a record in the database with the provided data key.
    /// </summary>
    /// <param name="key">The identifier of the data to be removed.</param>
    Task Delete(string key, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates data with a provided key in the database.
    /// </summary>
    /// <typeparam name="TData">The type of the data to be stored with the provided key.</typeparam>
    /// <param name="key">The key of the data to be updated.</param>
    /// <param name="data">The new data.</param>
    Task Update<TData>(string key, TData data, CancellationToken cancellationToken = default);
}

public readonly record struct FirebaseResponse(
    [property: JsonExtensionData] Dictionary<string, object>? Data);