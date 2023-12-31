using Microsoft.Extensions.DependencyInjection;
using TFA.Application.Interfaces.Services;

namespace TFA.Presentation.Presenters;

public abstract class AbstractPresenter(IServiceScopeFactory scopeFactory) : IPresenter
{
    public abstract string Key { get; }

    public abstract Task Present(IPresentable presentable, CancellationToken cancellationToken = default);

    protected IEnumerable<T> BuildContent<T>(IPresentable data, Presenter presenter)
    {
        using IServiceScope serviceScope = scopeFactory.CreateScope();
        foreach (T content in serviceScope.ServiceProvider.InvokeContentBuilderBuildMethod<T>(data, presenter))
        {
            yield return content;
        }

        yield break;
    }

    protected IEnumerable<T> BuildContent<T>(IPresentable data, Presenter presenter, Func<T, bool> sanitizer)
    {
        using IServiceScope serviceScope = scopeFactory.CreateScope();
        foreach (T content in serviceScope.ServiceProvider.InvokeContentBuilderBuildMethod<T>(data, presenter, sanitizer))
        {
            yield return content;
        }

        yield break;
    }
}
