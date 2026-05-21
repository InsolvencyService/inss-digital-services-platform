// ReSharper disable UnusedAutoPropertyAccessor.Global

using System.ComponentModel.DataAnnotations;

namespace Inss.Common.Infrastructure.Options;

public class ExternalApiOptions
{
    [Required]
    public string Url { get; init; }

    public int LifetimeMinutes { get; init; } = 5;

    public int RetryCount { get; init; } = 3;

    public int CountBeforeBreaking { get; init; } = 3;

    public int BreakDurationSeconds { get; init; } = 30;
}