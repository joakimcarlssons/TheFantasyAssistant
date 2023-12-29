using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using TFA.Scraper.Config;
using TFA.Scraper.Services;

[assembly: FunctionsStartup(typeof(TFA.Scraper.Startup))]
namespace TFA.Scraper;

public class Startup : FunctionsStartup
{
    public override void ConfigureAppConfiguration(IFunctionsConfigurationBuilder builder)
    {
        if (builder is null) throw new ArgumentNullException(nameof(builder));
        var context = builder.GetContext();

        builder.ConfigurationBuilder
            .AddAppsettingsFile(context)
            .AddEnvironmentVariables();
    }

    public override void Configure(IFunctionsHostBuilder builder)
    {
        IConfiguration config = builder.GetContext().Configuration;

        builder.Services.Configure<ApiOptions>(config.GetSection(ApiOptions.Key));
        builder.Services.Configure<ChromeOptions>(config.GetSection(ChromeOptions.Key));
        builder.Services.Configure<LeagueOptions>(config.GetSection(LeagueOptions.Key));
        builder.Services.Configure<PredictedPriceChangesOptions>(config.GetSection(PredictedPriceChangesOptions.Key));
        builder.Services.Configure<EmailOptions>(config.GetSection(EmailOptions.Key));

        builder.Services.AddSingleton<IEmailService, EmailService>();
        builder.Services.AddTransient<ILeagueService, LeagueService>();
        builder.Services.AddTransient<IPredictedPriceChangesService, PredictedPriceChangesService>();
    }
}

public static class ConfigurationBuilderExtensions
{
    public static IConfigurationBuilder AddAppsettingsFile(this IConfigurationBuilder builder, FunctionsHostBuilderContext context, bool useEnvironment = false)
    {
        if (context is null) 
            throw new ArgumentNullException(nameof(context));

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