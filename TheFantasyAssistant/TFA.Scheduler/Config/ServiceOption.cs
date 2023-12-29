using System.ComponentModel.DataAnnotations;
using TFA.Scheduler.Data;

namespace TFA.Scheduler.Config;

/// <summary>
/// Configuration for a specific scheduled service
/// </summary>
public class ServiceOption
{
    /// <summary>
    /// The configuration key wrapping all services
    /// </summary>
    public const string Key = "Services";

    [Required]
    public string Name { get; init; }

    [Required]
    public string UrlSuffix { get; init; }

    public RunTime RunTime { get; init; } = RunTime.Never;

    public bool Enabled { get; init; } = true;
}
