using Mapster;
using MapsterMapper;
using MudBlazor.Services;
using System.Reflection;
using TFA.Application.Config;
using TFA.Application.Interfaces.Repositories;
using TFA.Application.Interfaces.Services;
using TFA.Client.Config;
using TFA.Client.Data.Repositories;
using TFA.Client.Data.Services;
using TFA.Client.State;

namespace TFA.Client;

public static class DI
{
    public static IServiceCollection AddClientServices(this IServiceCollection services)
    {
        services.AddMudServices();
        services.AddMemoryCache();
        services.AddClientMappings();
        services.AddStateManager();

        services.AddHttpClient<IReadOnlyFirebaseRepository, ReadOnlyFirebaseRepository>();
        services.AddScoped<IBaseDataService, ClientBaseDataService>();

        // Configs
        services.AddConfigurationOptions<FirebaseOptions>();

        return services;
    }

    private static IServiceCollection AddClientMappings(this IServiceCollection services)
    {
        TypeAdapterConfig config = TypeAdapterConfig.GlobalSettings;
        config.Scan(Assembly.GetExecutingAssembly());

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
        return services;
    }
}
