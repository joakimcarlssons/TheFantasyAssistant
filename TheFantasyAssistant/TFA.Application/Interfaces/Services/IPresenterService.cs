namespace TFA.Application.Interfaces.Services;

public interface IPresenter<TPresentable> where TPresentable : IPresentable
{
    Task Present(TPresentable data, CancellationToken cancellationToken = default);
}

public interface IPresentable;
