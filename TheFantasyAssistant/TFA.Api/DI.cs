using Mapster;
using MapsterMapper;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;
using TFA.Api.Authentication;
using TFA.Application.Config;

namespace TFA.Api;

public static class DI
{
    public static IServiceCollection AddApiConfigurations(this IServiceCollection services)
    {
        services.AddConfigurationOptions<AuthOptions>();

        return services;
    }
    
    public static IServiceCollection AddApiMappings(this IServiceCollection services)
    {
        TypeAdapterConfig config = TypeAdapterConfig.GlobalSettings;
        config.Scan(
            Domain.AssemblyReference.Assembly,
            Application.AssemblyReference.Assembly,
            Infrastructure.AssemblyReference.Assembly,
            Assembly.GetExecutingAssembly());

        services.AddSingleton(config);
        services.AddScoped<IMapper, ServiceMapper>();
        return services;
    }

    public static IServiceCollection AddSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(opt =>
        {
            opt.EnableAnnotations();
            opt.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "The Fantasy Assistant",
                Version = "v1",
            });
            opt.OperationFilter<AddRequiredHeaderParameters>();
        });

        return services;
    }
}

public class AddRequiredHeaderParameters : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        operation.Parameters ??= new List<OpenApiParameter>();
        operation.Parameters.Add(new OpenApiParameter
        {
            Name = AuthOptions.ApiKeyHeaderName,
            In = ParameterLocation.Header,
            Required = true,
            AllowEmptyValue = true,
            Schema = new OpenApiSchema
            {
                Type = "string",
            },
            Example = new OpenApiString("Dev")
        });
    }
}
