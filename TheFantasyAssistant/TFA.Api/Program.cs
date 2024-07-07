using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Serilog;
using TFA.Api;
using TFA.Api.Authentication;
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
        AuthOptions authOptions = new();
        builder.Configuration.Bind("Authentication", authOptions);

        opt.AddPolicy("Client", builder => builder
            .WithOrigins(authOptions.ClientUrl, $"{authOptions.ClientUrl}/")
            .WithMethods("GET", "OPTIONS")
            .AllowAnyHeader()
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

app.UseMiddleware<CustomAuthMiddleware>();
app.UseCors("Client");

app.AddModules<Program>();
app.MapHub<ClientHub>("wss/client").RequireCors("Client");

app.Run();
