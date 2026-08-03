// ReSharper disable UnusedAutoPropertyAccessor.Global

namespace Inss.Platform.Component.Options;

public sealed class AnalyticsOptions
{
    public string Url { get; init; }
    
    public string SiteId { get; init; }
    
    public string SecurityHash { get; init; }
    
    public bool IsEnabled => !string.IsNullOrWhiteSpace(Url) && !string.IsNullOrWhiteSpace(SiteId);
}