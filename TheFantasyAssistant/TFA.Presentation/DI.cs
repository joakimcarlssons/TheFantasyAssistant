using Microsoft.Extensions.DependencyInjection;
using TFA.Application.Interfaces.Services;
using TFA.Discord;
using TFA.Presentation.Common.Services;
using TFA.Presentation.Presenters;
using TFA.Twitter;

namespace TFA.Presentation;

public static class Presentation
{
    public static IServiceCollection AddPresentation(this IServiceCollection services)
    {
        services
            .AddTwitter<TwitterService>()
            .AddDiscord(typeof(AssemblyReference).Assembly);

        services
            .AddPresenters()
            .AddContentBuilders();

        return services;
    }

    private static IServiceCollection AddPresenters(this IServiceCollection services)
    {
        // Add presenters with IPresenter<> interface
        typeof(AssemblyReference).Assembly.ExportedTypes
            .Where(x => !x.IsAbstract
                && x.IsClass
                && x.GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPresenter<>)))
            .ToList()
            .ForEach(presenter =>
            {
                foreach (Type interfaceType in presenter.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IPresenter<>)))
                {
                    services.AddScoped(interfaceType, presenter);
                }
            });

        // Add presenters with IPresenter interface
        typeof(AssemblyReference).Assembly.ExportedTypes
            .Where(x => typeof(IPresenter).IsAssignableFrom(x) && !x.IsAbstract && x.IsClass)
            .Select(presenter =>
            {
                ServiceProvider serviceProvider = services.BuildServiceProvider();
                object[] dependencies = presenter
                    .GetConstructors()
                    .First()
                    .GetParameters()
                    .Select(parameter => serviceProvider.GetService(parameter.ParameterType)!)
                    .ToArray() ?? [];

                return ActivatorUtilities.CreateInstance(serviceProvider, presenter, dependencies);
            })
            .ToList()
            .ForEach(presenter =>
            {
                services.AddKeyedScoped(typeof(IPresenter), ((IPresenter)presenter).Key, presenter.GetType());
            });

        return services;
    }

    private static IServiceCollection AddContentBuilders(this IServiceCollection services)
    {
        typeof(AssemblyReference).Assembly.ExportedTypes
            .Where(x => !x.IsAbstract 
                && x.IsClass 
                && x.GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IContentBuilder<,>)))
            .ToList()
            .ForEach(contentBuilder =>
            {
                foreach (Type interfaceType in contentBuilder.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IContentBuilder<,>)))
                {
                    services.AddScoped(interfaceType, contentBuilder);
                }
            });

        return services;
    }
}

