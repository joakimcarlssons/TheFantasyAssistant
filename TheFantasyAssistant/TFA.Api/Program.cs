using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using TFA.Api;
using TFA.Api.Client;
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

    builder.Services.AddSignalR();
    builder.Services.AddCors(opt =>
    {
        string? allowedClient = builder.Configuration["ClientUrl"];

        opt.AddPolicy("Client", builder => builder
            .AllowAnyMethod()
            .AllowAnyHeader()
            .SetIsOriginAllowed(origin => origin == allowedClient)
            .AllowCredentials());
    });

    builder.Services.AddHealthChecks();
}


WebApplication app = builder.Build();
{
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseCors("Client");
        app.MapHub<ClientHub>("wss/client").RequireCors("Client");
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
