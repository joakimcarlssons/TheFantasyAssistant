using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.FileProviders;
using Serilog;
using TFA.Api;
using TFA.Api.Exceptions;
using TFA.Api.Middlewares;
using TFA.Api.Modules;
using TFA.Application;
using TFA.Infrastructure;
using TFA.Presentation;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
{
    builder.Services.AddSwagger();
    builder.Services
        .AddApiConfigurations()
        .AddApiMappings();

    builder.Services
        .AddMemoryCache()
        .AddApplication()
        .AddInfrastructure()
        .AddPresentation();

    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    builder.Host.UseSerilog((context, configuration) =>
        configuration.ReadFrom.Configuration(context.Configuration));

    builder.Services.AddHealthChecks();
}


WebApplication app = builder.Build();
{    
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }

    app.UseStatusCodePages();
    app.UseExceptionHandler();

    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    });

    app.UseSerilogRequestLogging();
    app.UseHttpsRedirection();

    app.UseDefaultFiles();
    app.UseStaticFiles();
}

app.UseMiddleware<ApiKeyAuthMiddleware>();
app.AddModules<Program>();

app.Run();
