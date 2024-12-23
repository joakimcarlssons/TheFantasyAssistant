﻿using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using TFA.Scheduler.Config;
using TFA.Scheduler.Services;

[assembly: FunctionsStartup(typeof(TFA.Scheduler.Startup))]
namespace TFA.Scheduler;

public class Startup : FunctionsStartup
{
    public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
    {
        ArgumentNullException.ThrowIfNull(builder);

        FunctionsHostBuilderContext context = builder.GetContext();

        builder.ConfigurationBuilder
            .AddAppsettingsFile(context)
            .AddEnvironmentVariables();
    }

    public override void Configure(IFunctionsHostBuilder builder)
    {
        IConfiguration config = builder.GetContext().Configuration;

        builder.Services.Configure<ApiOptions>(config.GetSection(ApiOptions.Key));
        builder.Services.Configure<List<ServiceOption>>(config.GetSection(ServiceOption.Key));
        builder.Services.Configure<EmailOptions>(config.GetSection(EmailOptions.Key));

        builder.Services.AddSingleton<IEmailService, EmailService>();
        builder.Services.AddHttpClient<IRequestService, RequestService>();
        builder.Services.AddSingleton<SchedulerService>();
    }
}

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddAppsettingsFile(this IConfigurationBuilder builder, FunctionsHostBuilderContext context, bool useEnvironment = false)
    {
        ArgumentNullException.ThrowIfNull(context);

        string environmentSection = useEnvironment
            ? $".{context.EnvironmentName}"
            : string.Empty;

        builder.AddJsonFile(
            path: Path.Combine(context.ApplicationRootPath, $"appsettings{environmentSection}.json"),
            optional: true,
            reloadOnChange: false);

        return builder;
    }
}
