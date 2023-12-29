using TFA.Application.Interfaces.Services;

namespace TFA.Presentation.Presenters;

public interface IPresenter
{
    string Key { get; }
    Task Present(IPresentable presentable, CancellationToken cancellationToken = default);
}

internal sealed class PresenterKeys
{
    internal const string Twitter = "Twitter";
    internal const string Discord = "Discord";
}