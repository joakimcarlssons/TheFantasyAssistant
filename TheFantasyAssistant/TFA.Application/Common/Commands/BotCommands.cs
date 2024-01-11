namespace TFA.Application.Common.Commands;

public sealed class BotCommands
{
    public sealed partial record BestFixtures
    {
        public const string Name = "best-fixtures";
        public const string Description = "Get the teams with best fixtures in a range of given gameweeks.";
    };

    public sealed partial record TeamFixtures
    {
        public const string Name = "team-fixtures";
        public const string Description = "Get the fixtures for a specific team.";
    }
}