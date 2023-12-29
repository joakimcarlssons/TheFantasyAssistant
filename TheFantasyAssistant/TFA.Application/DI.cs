using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TFA.Application.Behaviors;
using TFA.Application.Common.Notifications;
using TFA.Application.Config;

namespace TFA.Application;

public static class DI
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        Assembly assembly = typeof(AssemblyReference).Assembly;

        services.AddTransient<INotificationPublisher, OutboxPublisher>();
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(assembly);
        });

        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(DataFetcherBehavior<,>));
        services.AddScoped(typeof(IPipelineBehavior<,>), typeof(DataValidationBehavior<,>));

        services.AddValidatorsFromAssembly(assembly);
        services.AddConfigurations();

        return services;
    }

    private static IServiceCollection AddConfigurations(this IServiceCollection services)
    {
        services.AddConfigurationOptions<SourceOptions>();
        services.AddConfigurationOptions<EmailOptions>();

        return services;
    }
}
