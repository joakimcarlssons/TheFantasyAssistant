namespace TFA.Domain.Exceptions;

public class SourceFetcherNotFoundException<TData>() : Exception($"No source fetcher set up for data of type {typeof(TData)}.");
