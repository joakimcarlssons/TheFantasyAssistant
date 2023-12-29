using Microsoft.Extensions.DependencyInjection;
using TFA.Application.Interfaces.Services;

namespace TFA.Presentation.Common.ContentBuilders;

public interface IContentBuilder<TIn, TOut> where TIn : IPresentable
{
    Presenter Presenter { get; }
    IReadOnlyList<TOut> Build(TIn data);
}

public static class ContentBuilderServiceHelpers
{
    /// <summary>
    /// Dynamically invokes the Build method of a <see cref="IContentBuilder{TPresentable}" /> with a given <see cref="IPresentable"/> and a <see cref="Presenter"/>.
    /// Also clears out any empty content from the result.
    /// </summary>
    /// <param name="serviceProvider">The service provider to use in order to get the correct service.</param>
    /// <param name="data">The data to create content from.</param>
    /// <param name="presenter">The presenter to create content for.</param>
    /// <returns>The created content from the invoked service.</returns>
    public static IReadOnlyList<T> InvokeContentBuilderBuildMethod<T>(
        this IServiceProvider serviceProvider, 
        IPresentable data, 
        Presenter presenter)
    {

        return serviceProvider.GetServices(typeof(IContentBuilder<,>).MakeGenericType(data.GetType(), typeof(T)))
            .SelectMany(contentBuilder =>
            {
                if (contentBuilder is not null)
                {
                    Type contentBuilderType = contentBuilder.GetType();
                    if (contentBuilderType.GetProperty(nameof(Presenter))?.GetValue(contentBuilder) is Presenter key
                        && key == presenter
                        && contentBuilderType.GetMethod("Build") is { } buildMethod)
                    {
                        return (buildMethod.Invoke(contentBuilder, [data]) as IReadOnlyList<T> ?? []);
                    }
                }

                return [];
            })
            .ToList();
    }

    /// <inheritdoc cref="InvokeContentBuilderBuildMethod" />
    /// <remarks>Adds a sanitizer to filter out certain data.</remarks>
    public static IReadOnlyList<T> InvokeContentBuilderBuildMethod<T>(
        this IServiceProvider serviceProvider,
        IPresentable data,
        Presenter presenter,
        Func<T, bool> sanitizer) =>
            serviceProvider.InvokeContentBuilderBuildMethod<T>(data, presenter)
                .Where(sanitizer)
                .ToList();
}
