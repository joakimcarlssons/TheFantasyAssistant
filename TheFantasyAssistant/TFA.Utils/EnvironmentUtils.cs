namespace TFA.Utils;

public static class Env
{
    private const string ENV_VAR = "ASPNETCORE_ENVIRONMENT";
    public static bool IsDevelopment() => Environment.GetEnvironmentVariable(ENV_VAR) == Environments.DEVELOPMENT;
    public static bool IsProduction() => Environment.GetEnvironmentVariable(ENV_VAR) == Environments.PRODUCTION;
}

public sealed class Environments
{
    public const string DEVELOPMENT = "Development";
    public const string PRODUCTION = "Production";
}