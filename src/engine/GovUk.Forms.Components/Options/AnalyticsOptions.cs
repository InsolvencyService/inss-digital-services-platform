// ReSharper disable UnusedAutoPropertyAccessor.Global
namespace GovUk.Forms.Components.Options;

public sealed class AnalyticsOptions
{
    public string Url { get; init; }
    
    public string SiteId { get; init; }
    
    public bool IsEnabled => !string.IsNullOrWhiteSpace(Url) && !string.IsNullOrWhiteSpace(SiteId);
}