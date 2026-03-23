// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace GovUk.Forms.Components.Options;

public sealed class ExternalApiOptions
{
    public string Url { get; init; }

    public int LifetimeMinutes { get; init; } = 5;

    public int RetryCount { get; init; } = 3;

    public int CountBeforeBreaking { get; init; } = 3;

    public int BreakDurationSeconds { get; init; } = 30;
}