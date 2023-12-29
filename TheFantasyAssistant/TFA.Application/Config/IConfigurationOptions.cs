using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace TFA.Application.Config;

public interface IConfigurationOptions
{
    /// <summary>
    /// The key of the configuration in the settings file.
    /// </summary>
    string Key { get; }
}

public static class ConfigurationOptionsExtensions
{
    /// <summary>
    /// Used to add and validate configuration options from appsettings.
    /// </summary>
    /// <typeparam name="TOptions">The options to register</typeparam>
    /// <param name="services">The service collection.</param>
    public static IServiceCollection AddConfigurationOptions<TOptions>(this IServiceCollection services)
        where TOptions : class, IConfigurationOptions
    {
        services.AddOptions<TOptions>()
            .BindConfiguration(((IConfigurationOptions)(Activator.CreateInstance(typeof(TOptions)))!).Key)
            .ValidateDataAnnotations()
            .ValidateOnStart();

        return services;
    }
}

