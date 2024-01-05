using ErrorOr;

namespace TFA.UnitTests.Builders;

internal class ErrorOrBuilder<T>
{
    private T? Result;
    private Error Error;

    public ErrorOrBuilder<T> WithResult(T result)
    {
        Result = result;
        return this;
    }

    public ErrorOrBuilder<T> WithError(Error error)
    {
        Error = error;
        return this;
    }

    public Task<ErrorOr<T>> BuildTaskResult() => Task.FromResult(BuildResult());
    public Task<ErrorOr<T>> BuildTaskError() => Task.FromResult(BuildError());
    public ErrorOr<T> BuildResult() => Result ?? throw new NotImplementedException();
    public ErrorOr<T> BuildError() => Error;
}
