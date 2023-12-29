namespace TFA.Api.Modules;

public interface IModule
{
    void RegisterEndpoints(IEndpointRouteBuilder app);
}

public static class ModulesInstaller
{
    public static IEndpointRouteBuilder AddModules<TStartup>(this IEndpointRouteBuilder app)
    {
        IReadOnlyList<IModule> modules = typeof(TStartup).Assembly.ExportedTypes.Where(x =>
            typeof(IModule).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract)
            .Select(Activator.CreateInstance).Cast<IModule>().ToList();

        foreach (IModule module in modules)
        {
            module.RegisterEndpoints(app);
        }

        return app;
    }
}