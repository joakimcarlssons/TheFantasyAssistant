using TFA.Scheduler.Config;

namespace TFA.Scheduler.Data;

/// <summary>
/// The runtime specified in a <see cref="ServiceOption"/>.
/// </summary>
public enum RunTime
{
    Never = 0,
    EveryHour = 1,
    Every6Hours = 2,
    PM9 = 3,
    Every12Hours = 4,
    Every30Minutes = 5,
    Every3Hours = 6,
    PM6 = 7,
    PM7 = 8,
    PM8 = 9,
    PM810 = 10,
    Every5Minutes = 11,
}

/// <summary>
/// The <see cref="RunTime" /> translated into Chron times.
/// </summary>
public sealed class RunTimes
{
    internal const string Never = "0 5 31 2 *";
    internal const string EveryMinute = "* * * * *";
    internal const string Every30Minutes = "*/30 * * * *";
    internal const string EveryHour = "0 * * * *";
    internal const string Every3Hours = "5 */3 * * *";
    internal const string Every6Hours = "0 */6 * * *";
    internal const string Every12Hours = "0 */12 * * *";
    internal const string Every5Minutes = "*/5 * * * *";

    internal const string PM6 = "0 18 * * *";
    internal const string PM7 = "0 19 * * *";
    internal const string PM8 = "0 20 * * *";
    internal const string PM9 = "0 21 * * *";
    internal const string PM810 = "10 20 * * *";
}
