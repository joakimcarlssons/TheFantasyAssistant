using TFA.Application.Interfaces.Repositories;
using TFA.Application.Config;

namespace TFA.Infrastructure;

public static class DI
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        services.AddDataServices()
            .AddCustomHttpClient<IFotmobService, FotmobService>()
            .AddCustomHttpClient<IGameweekDetailsService, GameweekDetailsService>();

        services.AddScoped<IFirebaseRepository, FirebaseRepository>();
        services.AddSingleton<IEmailService, EmailService>();

        services.AddMappings();
        services.AddConfigurations();

        return services;
    }

    private static IServiceCollection AddDataServices(this IServiceCollection services)
    {
        // Add services directly inheriting IDataService<> interface
        typeof(AssemblyReference).Assembly.ExportedTypes
            .Where(x => !x.IsAbstract
                && x.IsClass
                && x.GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDataService<>)))
            .ToList()
            .ForEach(service =>
            {
                foreach (Type interfaceType in service.GetInterfaces()
                    .Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDataService<>)))
                {
                    if (service.TakesInTypeInConstructor(typeof(HttpClient)))
                    {
                        typeof(DI).GetMethod(nameof(AddCustomHttpClient))!
                            .MakeGenericMethod(interfaceType, service)
                            .Invoke(null, [services]);
                    }
                    else
                    {
                        services.AddScoped(interfaceType, service);
                    }
                }
            });

        // Add services inheriting an inteface which is inheriting IDataService<> interface
        typeof(AssemblyReference).Assembly.ExportedTypes
            .Concat(typeof(Application.AssemblyReference).Assembly.ExportedTypes)
            .Where(x => x.IsInterface
                && x.GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IDataService<>)))
            .ToList()
            .ForEach(additionalInterface =>
            {
                typeof(AssemblyReference).Assembly.ExportedTypes
                    .Where(x => !x.IsAbstract && x.IsClass && additionalInterface.IsAssignableFrom(x))
                    .ToList()
                    .ForEach(service =>
                    {
                        if (service.TakesInTypeInConstructor(typeof(HttpClient)))
                        {
                            typeof(DI).GetMethod(nameof(AddCustomHttpClient))!
                                .MakeGenericMethod(additionalInterface, service)
                                .Invoke(null, [services]);
                        }
                        else
                        {
                            services.AddScoped(additionalInterface, service);
                        }
                    });
            });

        return services;
    }

    public static IServiceCollection AddCustomHttpClient<TInterface, TImplementation>(this IServiceCollection services)
        where TInterface : class
        where TImplementation : class, TInterface
    {
        services.AddHttpClient<TInterface, TImplementation>().ConfigurePrimaryHttpMessageHandler(config => new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
        });

        return services;
    }

    private static IServiceCollection AddMappings(this IServiceCollection services)
    {
        TypeAdapterConfig config = TypeAdapterConfig.GlobalSettings;
        config.Scan(
            Domain.AssemblyReference.Assembly,
            Application.AssemblyReference.Assembly,
            Assembly.GetExecutingAssembly());

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
        return services;
    }

    private static IServiceCollection AddConfigurations(this IServiceCollection services)
    {
        services.AddConfigurationOptions<FirebaseOptions>();
        return services;
    }
}
